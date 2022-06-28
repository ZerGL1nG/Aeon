using Tensorflow.NumPy;

namespace Trof.AI;

public sealed class TrofDataset<TInput, TOutput> where TInput: INetworkData where TOutput: INetworkData
{
    public readonly NDArray InputArray;
    public readonly NDArray OutputArray;

    public TrofDataset(IEnumerable<ValueTuple<TInput, TOutput>> set, int repeat = 1)
    {
        if (repeat < 1) throw new ArgumentOutOfRangeException(nameof(repeat));
        var x  = new List<float>();
        var y  = new List<float>();
        var xx = new List<float>();
        var yy = new List<float>();
        var count = 0;
        foreach (var (input, output) in set) {
            x.AddRange(input.Data);
            y.AddRange(output.Data);
            ++count;
        }
        for (var iter = 0; iter < repeat; ++iter) {
            xx.AddRange(x);
            yy.AddRange(y);
        }
        count *= repeat;
        InputArray  = np.array(xx.ToArray()).reshape((count, TInput.Size));
        OutputArray = np.array(yy.ToArray()).reshape((count, TOutput.Size));
    }
}