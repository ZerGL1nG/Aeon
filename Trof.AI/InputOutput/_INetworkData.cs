using System.Text;

namespace Trof.AI;

public interface INetworkData
{
    IEnumerable<float> Data { get; }
    static abstract int Size { get; }
    static abstract string GetName(int index);
    static abstract INetworkData Ret (IEnumerable<float> data);
}

public static class DataExt
{
    public static string DataP<T>(this T test) where T : INetworkData
    {
        StringBuilder builder = new();
        int i = 0;
        builder.Append($"{typeof(T).Name}: {{ ");
        foreach (var input in test.Data)
        {
            builder.Append($"{T.GetName(i)}={input:+#0.0000;-#0.0000;+0.0000} ");
            ++i;
        }
        builder.Append('}');
        return builder.ToString();
    }
}