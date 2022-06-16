using System.Text;

namespace Trof.AI;

internal class AeonTestOut : INetworkData
{
    public AeonTestOut(IEnumerable<float> floats)
    {
        _floats = floats.ToList();
    }

    public IEnumerable<float> Data => GetData();
    private readonly List<float> _floats;
    public static int Size => 10;
    public static string GetName(int index) => Names[index];

    public static INetworkData Ret(IEnumerable<float> data)
    {
        return new AeonTestOut(SetData(data.ToList()));
    }

    private static readonly float[] Bases = 
        { 100, 15, 0, 0, 1.5f, 0, 1, 0, 1, 0 };
    private static readonly float[] Mults = 
        { 10/22f, 7/3f, 15/7f, 15/.5f, 50/.50f, 13/.2f, 4/2f, 30/15f, 11/5f, 1 };
    private static readonly float[] Divs = Mults.Select(m => 1/m).ToArray();

    private static readonly string[] Names =
        { "HP", "ATT", "MAG", "CHA", "CDM", "INC", "ARM", "DEF", "REG", "GLD" };

    private IEnumerable<float> GetData()
    {
        for (var i = 0; i < Size; ++i)
            yield return (_floats[i] - Bases[i]) * Mults[i];
    }

    private static IEnumerable<float> SetData(List<float> data)
    {
        for (var i = 0; i < Size; ++i)
            yield return data[i] * Divs[i] + Bases[i];
    }
    
    public override string ToString()
    {
        var stb = new StringBuilder();
        for (var i = 0; i < Size; i++) 
            stb.Append($"{Names[i]}={_floats[i],5:F1}; ");
        return stb.ToString();
    }
}