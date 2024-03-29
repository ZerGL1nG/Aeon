﻿using System;
using System.Collections.Generic;
using AI.NeuralNetwork;

namespace Aeon.Agents;

public class SpanData: INetworkData
{
    private readonly float[] _data;

    public SpanData(int size)
    {
        Size  = size;
        _data = new float[size];
    }

    public Span<float> Span => _data;
    public IEnumerable<float> Inputs => _data;
    public int Size { get; }
}