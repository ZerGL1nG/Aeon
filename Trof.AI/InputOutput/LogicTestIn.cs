namespace Trof.AI;

internal record LogicTestIn : INetworkData
{
    public float X { get; init; }
    public float Y { get; init; }
    public float Z { get; init; }

    public IEnumerable<float> Data => new[] { X, Y, Z };
    public static int Size => 3;
    public static string GetName(int index) => Names[index];
    public static INetworkData Ret(IEnumerable<float> data)
    {
        throw new NotImplementedException();
    }

    private static readonly string[] Names = new[] { "X", "Y", "Z" };
}