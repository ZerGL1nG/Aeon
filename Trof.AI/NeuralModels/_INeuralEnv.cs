using Tensorflow;
using Tensorflow.Keras;
using Tensorflow.Keras.ArgsDefinition;
using Tensorflow.Keras.Engine;
using Tensorflow.Keras.Layers;
using Tensorflow.NumPy;

namespace Trof.AI;

public interface INeuralEnv<TInput, TOutput> 
    where TInput: INetworkData where TOutput: INetworkData
{
    TOutput Call(TInput input);

    void TerminateEpisode(TInput terminal);

    internal IEnumerable<(string filename, Model model)> GetSavingModels();

    event Action<IEnumerable<(string filename, Model model)>> OnSave;
    event Action<IEnumerable<(string filename, Model model)>> OnLoad;
}

public interface IReinforcementEnv<TInput, TOutput>: INeuralEnv<TInput, TOutput>
    where TInput: INetworkData where TOutput: INetworkData
{
    bool AckActionReward(int action, float reward);
}

public static class EnvExt
{
    public static TOut CallModel<TIn, TOut>(this Model model, TIn input) 
        where TIn: INetworkData where TOut: INetworkData
    {
        var tensor = model.Apply(input.ToSingleTensor());
        return (TOut) TOut.Ret(tensor[0].ToArray<float>());
    }
}

public static class ModelMaker
{
    public static Model Perceptron(LayerSettings start, LayerSettings final, params LayerSettings[] layers)
    {
        var model = new Sequential(new SequentialArgs());
        var x = new List<LayerSettings>().Append(start).Concat(layers).Append(final);
        var cyka = 0;
        foreach ((int amount, var actFunc) in x) {
            var govno = new Dense(new DenseArgs() {
                Units = amount, Activation = actFunc.Getf(), Name = $"L{cyka++}_P{amount}",
            });
            model.Layers.Add(govno);
        }

        model.compile(keras.optimizers.RMSprop(), keras.losses.MeanSquaredError(), Array.Empty<string>());
        
        model.Apply(np.zeros((1, start.Amount), TF_DataType.TF_FLOAT));
        
        return model;
    }

    public static Activation Getf(this ActFunc func) =>
        func switch {
            Pass    => keras.activations.Linear,
            Sigmoid => keras.activations.Sigmoid,
            SoftMax => keras.activations.Softmax,
            TgH     => keras.activations.Tanh,
            ReLu    => keras.activations.Relu,
            _       => throw new ArgumentOutOfRangeException(nameof(func), func, null),
        };
}

public record LayerSettings(int Amount, ActFunc Func)
{
    public static implicit operator LayerSettings(int x) => new(x, ReLu);

    public static implicit operator LayerSettings((int x, ActFunc a) c) => new(c.x, c.a);
}

public enum ActFunc { Pass, Sigmoid, SoftMax, TgH, ReLu, }