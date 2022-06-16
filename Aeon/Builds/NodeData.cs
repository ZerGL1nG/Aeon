namespace Aeon.Builds;

internal class NodeData
{
    private readonly object _lock = new();

    public int Games { get; private set; } = 0;
    public int Wins { get; private set; } = 0;
    public int Loses { get; private set; } = 0;

    public float WinRate => (float)Wins / Games;
    public float LoseRate => (float)Loses / Games;

    public float AvgPts => 3 + WinRate * WinPts + LoseRate * LosePts;
    private const float WinPts = 1;
    private const float LosePts = -3;

    public override string ToString()
    {
        return $"Win:{WinRate,7:P}; Lose:{LoseRate,7:P}; Pts:{AvgPts:F4}";
    }
    
    public void Played(int result)
    {
        lock (_lock)
        {
            ++Games;
            if (result > 0) ++Wins;
            if (result < 0) ++Loses;
        }
    }
}