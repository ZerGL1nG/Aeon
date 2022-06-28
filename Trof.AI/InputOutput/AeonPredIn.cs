namespace Trof.AI;

public class AeonPredIn: INetworkData
{
    private static readonly string[] Names = { "HP", "ATT", "MAG", "CHA", "CDM", "INC", "ARM", "DEF", "REG", "GLD", };

    private readonly List<float> _floats;

    public AeonPredIn(IEnumerable<float> floats) => _floats = floats.ToList();

    public IEnumerable<float> Data => _floats;
    public static int Size => 75;

    public static string GetName(int index) => $"{index}";

    public static INetworkData Ret(IEnumerable<float> data) => throw new NotImplementedException();

    public AeonPredOut Stats() => new(_floats.GetRange(0, 10));
}