using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.NeuralNetwork;

public interface INetworkInput
{
    public IEnumerable<float> Inputs { get; }
    public int Size { get; }
}

public class ArrayInput : INetworkInput
{
    private readonly List<float> _list;
    public ArrayInput(IEnumerable<float> values) => _list = values.ToList();
    public IEnumerable<float> Inputs => _list;
    public int Size => _list.Count;
}

public class MultipleInput<T> :INetworkInput where T:INetworkInput
{
    public MultipleInput(params T[] inputs)
    {
        Inputs = inputs.SelectMany(t => t.Inputs);
        Size = inputs.Length * inputs.First().Size;
    }
    public IEnumerable<float> Inputs { get; }
    public int Size { get; }
}

public class ComposeInput : INetworkInput
{
    private readonly INetworkInput[] _inputs;

    public ComposeInput(params INetworkInput[] inputs) => _inputs = inputs;

    public IEnumerable<float> Inputs => _inputs.SelectMany(o => o.Inputs);
    public int Size => _inputs.Select(i => i.Size).Sum();
}