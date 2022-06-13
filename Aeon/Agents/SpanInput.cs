using System;
using System.Collections.Generic;
using AI.NeuralNetwork;

namespace Aeon.Agents;

public class SpanInput : INetworkInput
{
    public IEnumerable<float> Inputs => _data;
    public int Size { get; }

    private readonly float[] _data;
    public Span<float> Span => _data;

    public SpanInput(int size)
    {
        Size = size;
        _data = new float[size];
    }
}
