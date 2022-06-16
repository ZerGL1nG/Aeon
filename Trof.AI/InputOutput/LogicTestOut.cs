namespace Trof.AI;

internal record LogicTestOut : INetworkData
{
    public float And { get; init; }
    public float Or { get; init; }
    public float Xor { get; init; }
    public float Ovf { get; init; }
    
    public IEnumerable<float> Data => new[] { And, Or, Xor, Ovf };
    public static int Size => 4;
    public static string GetName(int index) => Names[index];
    public static INetworkData Ret(IEnumerable<float> data)
    {
        var d = new List<float>(data);
        return new LogicTestOut() { And = d[0], Or = d[1], Xor = d[2], Ovf = d[3]};
    }

    private static readonly string[] Names = new[] { "AND", "OR", "XOR", "OWF" };
}