namespace Trof.AI;

internal class AeonTestIn : INetworkData
{
    public AeonTestIn(IEnumerable<float> floats)
    {
        _floats = floats.ToList();
    }

    public IEnumerable<float> Data => _floats;
    private readonly List<float> _floats;
    public static int Size => 75;
    public static string GetName(int index) => $"{index}";

    public static INetworkData Ret(IEnumerable<float> data)
    {
        throw new NotImplementedException();
    }
}