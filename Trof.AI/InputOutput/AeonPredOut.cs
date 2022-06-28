using System.Text;

namespace Trof.AI;

public class AeonPredOut: INetworkData
{
    public static readonly string[] Names = { "HP", "ATT", "MAG", "CHA", "CDM", "INC", "ARM", "DEF", "REG", "GLD", };
    private static readonly float[] Bases = { 100, 15, 0, 0, 1.5f, 0, 1, 0, 1, 0, };
    private static readonly float[] Mults = { 10/22f, 7/3f, 15/7f, 15/.5f, 50/.50f, 13/.2f, 4/2f, 30/15f, 11/5f, 1, };
    private static readonly float[] Divs  = Mults.Select(m => 1/m).ToArray();
    
    private readonly List<float> _floats;
    
    public AeonPredOut(IEnumerable<float> floats) => _floats = floats.ToList();
    public static int Size => 10;
    
    public IReadOnlyList<float> Floats => _floats;
    public IEnumerable<float> Data => GetData();

    public static string GetName(int index) => Names[index];

    public static INetworkData Ret(IEnumerable<float> data) => new AeonPredOut(SetData(data.ToList()));

    private IEnumerable<float> GetData() 
    {
        for (var i = 0; i < Size; ++i) 
            yield return (_floats[i]-Bases[i])*Mults[i];
    }

    private static IEnumerable<float> SetData(List<float> data) 
    {
        for (var i = 0; i < Size; ++i) 
            yield return data[i]*Divs[i]+Bases[i];
    }

    public override string ToString() 
    {
        var stb = new StringBuilder();
        for (var i = 0; i < Size; i++) 
            stb.Append($"{Names[i]}={_floats[i],5:F1}; ");
        return stb.ToString();
    }

    public static AeonPredOut operator +(AeonPredOut a, AeonPredOut b) =>
        new(a._floats.Zip(b._floats).Select(x => x.First+x.Second));

    public static AeonPredOut operator -(AeonPredOut a, AeonPredOut b) =>
        new(a._floats.Zip(b._floats).Select(x => x.First-x.Second));
}