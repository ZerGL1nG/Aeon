using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.NeuralNetwork;

public interface INetworkData
{
    public IEnumerable<float> Inputs { get; }
    public int Size { get; }
}

public class ArrayData : INetworkData
{
    public ArrayData(IReadOnlyCollection<float> values)
    {
        Inputs = values;
        Size = values.Count;
    }
    
    public ArrayData(IEnumerable<float> values)
    {
        Inputs = values;
        Size = Inputs.TryGetNonEnumeratedCount(out int count)? -1 : count;
    }

    public IEnumerable<float> Inputs { get; }
    public int Size { get; }
}

public class MultipleData<T> :INetworkData where T:INetworkData
{
    public MultipleData(params T[] inputs)
    {
        Inputs = inputs.SelectMany(t => t.Inputs);
        Size = inputs.Length * inputs.First().Size;
    }
    public IEnumerable<float> Inputs { get; }
    public int Size { get; }
}

public class ComposeData : INetworkData
{
    private readonly INetworkData[] _inputs;

    public ComposeData(params INetworkData[] inputs)
    {
        _inputs = inputs;
        Size = _inputs.Select(i => i.Size).Sum();
    }

    public IEnumerable<float> Inputs => _inputs.SelectMany(o => o.Inputs);
    public int Size { get; }
}