using Tensorflow.Keras.Engine;

namespace Trof.AI.Env;

public class NeuralAgent<TX, TR>: IAgent<TX, TR> where TX: INetworkData where TR: INetworkData
{
    public INeuralEnv<TX, TR> NeuralEnv { protected get; init; }
    public IStratChoice<TR> ChoiceMaker { protected get; init; }

    public string ID { get; protected set; } = Guid.NewGuid().ToString();

    public virtual Choice<TR> Decide(TX state, Mask<TR>? mask)
    {
        LastChoice = ChoiceMaker.Choice(NeuralEnv.Call(state), mask);
        if (NeuralEnv is IReinforcementEnv<TX, TR> env) env.AckActionReward(LastChoice, 0);
        return LastChoice;
    }
    
    public Choice<TR> LastChoice { get; private set; } = Choice<TR>.Null;
    
    public bool GetReward(float reward) => 
        NeuralEnv is IReinforcementEnv<TX, TR> env && env.AckActionReward(LastChoice, reward);

    public virtual void OnEpisodeEnd(TX terminal) => NeuralEnv.TerminateEpisode(terminal);

    public virtual void Init()
    {
        //NeuralEnv.OnSave += Save;
    }

    public async Task Load(string agentDir, int? autosaveIndex = null) => await Task.Run(() => {
        string dir = autosaveIndex is null? agentDir : Path.Combine(agentDir, autosaveIndex.ToString()!);
        foreach ((string filename, Model model) in NeuralEnv.GetSavingModels()) {
            model.load_weights(Path.Combine(dir, filename));
        }
    });

    public async Task Save(string agentDir, int? autosaveIndex = null) => await Task.Run(() => {
        string dir = autosaveIndex is null? agentDir : Path.Combine(agentDir, autosaveIndex.ToString()!);
        foreach ((string filename, Model model) in NeuralEnv.GetSavingModels()) {
            model.save_weights(Path.Combine(dir, filename));
        }
    });
}