using Tensorflow;
using Tensorflow.Gradients;
using Tensorflow.Keras.Engine;
using Tensorflow.Keras.Losses;
using Tensorflow.Keras.Optimizers;
using Tensorflow.NumPy;

namespace Trof.AI;

public class QPerceptron<TInput, TOutput>: IReinforcementEnv<TInput, TOutput>
    where TInput: INetworkData where TOutput: INetworkData
{
    private readonly OptimizerV2 _optimizer = keras.optimizers.Adam();
    private readonly ILossFunc _lossFunc = keras.losses.MeanSquaredError();
    
    private readonly Model _training;
    private readonly Model _target;

    public string Dir { get; init; } = "./";
    public int? AgentN { get; init; }

    private float _reward;
    private int _action = -1;
    private TInput? _state;

    private readonly QBuffer<TInput> _buffer = new() { MemLength = 10000 };

    private int _turnCount = 0;
    private int _gameCount = 0;
    
    private int _sessionCount = 0;
    private int _sessionLength = 0;

    private const float Gamma = 0.96f;
    private const int GamesInSession = 100;
    private const int SaveEverySession = 5;

    public QPerceptron(ActFunc final, params LayerSettings[] hiddenLayers)
    {
        _training = ModelMaker.Perceptron((TInput.Size, Pass), (TOutput.Size, final), hiddenLayers);
        _target   = ModelMaker.Perceptron((TInput.Size, Pass), (TOutput.Size, final), hiddenLayers);
    }

    public TOutput Call(TInput input)
    {
        _buffer.Add(_state, _action, _reward, input);
        ++_sessionLength;
        ++_turnCount;
        _state = input;
        _reward = 0;
        return _target.CallModel<TInput, TOutput>(input);
    }

    public void TerminateEpisode(TInput terminal)
    {
        _buffer.Add(_state, _action, _reward, terminal);
        _buffer.Add(terminal, -1, 0, default);
        _sessionLength += 2;
        _turnCount = 0;
        _reward = 0;
        _action = -1;
        ++_gameCount;
        if (_gameCount % GamesInSession == 0) Learn();
    }

    IEnumerable<(string filename, Model model)> INeuralEnv<TInput, TOutput>.GetSavingModels() => 
        new[] { ("training", _training), ("target", _target) };

    public event Action<IEnumerable<(string filename, Model model)>>? OnSave;
    public event Action<IEnumerable<(string filename, Model model)>>? OnLoad;

    public bool AckActionReward(int action, float reward)
    {
        _reward += reward;
        _action = action;
        return true;
    }

    private void Learn()
    {
        var actions = _buffer.ActionsArray;
        var states = _buffer.StatesArray;
        var rewards = _buffer.RewardsArray;
        var nextStates = _buffer.NextStatesArray;

        var prediction = _target.predict(nextStates);
        var qCorrect = rewards + Gamma * tf.reduce_max(prediction);

        using var tape = tf.GradientTape();
        var qValues = _training.Apply(states, training: true);
        var qAction = tf.reduce_sum(qValues, axis: 1);
        var loss = _lossFunc.Call(qCorrect, qAction);

        var gradient = tape.gradient(loss, _training.trainable_variables);
        _optimizer.apply_gradients(zip(gradient, _training.trainable_variables.Cast<ResourceVariable>()));

        _sessionLength = 0;
        _sessionCount++;
        if (_sessionCount % SaveEverySession == 0) {
            
            
            
            _training.save_weights(Path.Join(Dir, $"tcx_agent_{AgentN}.h5"));
            _target.load_weights(Path.Join(Dir, $"tcx_agent_{AgentN}.h5"), false, true);
        }
    }
}

public class QBuffer<T> where T: INetworkData
{
    public int MemLength { get; init; }
    public int Count { get; private set; } = 0;
    
    private Queue<T?> _states = new();
    private Queue<T?> _nextStates = new();
    private Queue<int> _actions = new();
    private Queue<float> _rewards = new();

    public void Add(T? state, int action, float reward, T? next)
    {
        _states.Enqueue(state);
        _actions.Enqueue(action);
        _rewards.Enqueue(reward);
        _nextStates.Enqueue(next);
        if (++Count <= MemLength) return;
        _states.Dequeue();
        _actions.Dequeue();
        _rewards.Dequeue();
        _nextStates.Dequeue();
        --Count;
    }

    public NDArray StatesArray => _states.ToNdArray();

    public NDArray NextStatesArray => _nextStates.ToNdArray();

    public NDArray ActionsArray => np.array(_actions.ToArray());
    
    public NDArray RewardsArray => np.array(_rewards.ToArray());
}