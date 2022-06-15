using System.Collections.Generic;
using AI.NeuralNetwork;

namespace Aeon.DataModels;

public class AeonInput : INetworkData
{
    public IEnumerable<float> Inputs { get; }
    public int Size => 90;
}