namespace Trof.AI.Env;

public abstract class Kachalka<TX, TR> where TX: INetworkData where TR: INetworkData 
{
    public string ProjectDir { get; }

    public Kachalka(string projectDir)
    {
        ProjectDir = projectDir;
    }

    protected List<IAgent<TX, TR>> Agents = new();

    public int AddAgent(IAgent<TX, TR> agent)
    {
        Agents.Add(agent);
        agent.Init();
        return Agents.Count;
    }

    public int AddAgents(int amount, Func<int, IAgent<TX, TR>> factory)
    {
        for (var i = 0; i < amount; i++) {
            int count = Agents.Count;
            AddAgent(factory(count));
        }
        return Agents.Count;
    }

    public abstract Task Kach(int? amount = null, CancellationToken ct = default);
}


public interface IAgent<TX, TR> where TX: INetworkData where TR: INetworkData
{
    string ID { get; }
    Choice<TR> Decide(TX state, Mask<TR>? mask);

    bool GetReward(float reward);

    void OnEpisodeEnd(TX terminal);

    void Init();

    Task Load(string agentDir, int? autosaveIndex = null);
    Task Save(string agentDir, int? autosaveIndex = null);
}

public interface IStratChoice<TR> where TR: INetworkData
{
    public int Act(TR output, Mask<TR>? mask);
    public Choice<TR> Choice(TR output, Mask<TR>? mask) => new(Act(output, mask), this);
}

public class EGreedy<TR>: IStratChoice<TR> where TR: INetworkData
{
    public float Epsilon { get; init; } = 0.1f;

    public int Act(TR output, Mask<TR>? mask)
    {
        bool greed = Random.Shared.NextSingle() < Epsilon;
        if (!greed) return output.MaxIndex();
        return mask?.RandomMasked() ?? Random.Shared.Next(TR.Size);
    }
}

public record Choice<TR>(int Value, IStratChoice<TR>? Choicemaker) where TR: INetworkData
{
    public static implicit operator int(Choice<TR> choice) => choice.Value;

    public static Choice<TR> Null => new Choice<TR>(0, null);
}


public class Mask<TR> where TR: INetworkData
{
    private readonly bool[] _masked = new bool[TR.Size];
    private readonly List<int> _masks;

    public Mask(List<int> good)
    {
        _masks = good;
        foreach (int mask in _masks) _masked[mask] = true;
    }

    public int RandomMasked() => _masks[Random.Shared.Next(_masks.Count)];
}