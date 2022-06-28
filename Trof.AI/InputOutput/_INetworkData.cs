using System.Text;
using Tensorflow;
using Tensorflow.NumPy;

namespace Trof.AI;

public interface INetworkData
{
    IEnumerable<float> Data { get; }
    static abstract int Size { get; }

    static abstract string GetName(int index);

    static abstract INetworkData Ret(IEnumerable<float> data);
}

public static class DataExt
{
    public static NDArray ToNdArray<T>(this T? data) where T: INetworkData =>
        data is null? np.zeros(T.Size) : np.array(data.Data.ToArray());
    
    public static Tensor ToSingleTensor<T>(this T data) where T: INetworkData => ToNdArray(data).reshape((1, -1));

    public static NDArray ToNdArray<T>(this IEnumerable<T?> data) where T: INetworkData => 
        np.array(data.SelectMany(d => d.App()).ToArray()).reshape((-1, T.Size));

    public static IEnumerable<float> App<T>(this T? data, IEnumerable<float>? next = null) where T: INetworkData =>
        (data is null? new float[T.Size] : data.Data).Concat(next ?? Array.Empty<float>());

    public static IEnumerable<float> App<T>(this T? data, params float[] next) where T: INetworkData =>
        App<T>(data, (IEnumerable<float>) next );

    public static string DataP<T>(this T test) where T: INetworkData
    {
        StringBuilder builder = new();
        var i = 0;
        builder.Append($"{typeof(T).Name}: {{ ");
        foreach (var input in test.Data) {
            builder.Append($"{T.GetName(i)}={input:+#0.0000;-#0.0000;+0.0000} ");
            ++i;
        }
        builder.Append('}');
        return builder.ToString();
    }

    public static int MaxIndex<T>(this T data) where T: INetworkData
    {
        var max   = float.MinValue;
        var maxId = int.MinValue;
        var id = 0;
        foreach (float f in data.Data) {
            if (f > max) {
                max = f;
                maxId = id;
            }
            ++id;
        }
        return maxId;
    }
}