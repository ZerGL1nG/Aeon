using System;

namespace AI.NeuralNetwork;

public enum ActivationFunctions { None, Sigmoid, Gate, Tanh, ReLu, }

public static class NetworkManager
{
    public static Func<float, float> GetActivationFunc(ActivationFunctions func) =>
        func switch {
            ActivationFunctions.None    => d => d,
            ActivationFunctions.Gate    => d => d > 0? 1 : 0,
            ActivationFunctions.Sigmoid => d => 1/(1+MathF.Pow(MathF.E, -d)),
            ActivationFunctions.Tanh    => d => 2/(1+MathF.Pow(MathF.E, -2*d))-1,
            ActivationFunctions.ReLu    => d => d > 0? d : 0,
            _                           => d => d,
        };

    public static Func<float, float> GetDerivativeFunc(ActivationFunctions func) =>
        func switch {
            ActivationFunctions.None    => d => 1,
            ActivationFunctions.Gate    => d => 0,
            ActivationFunctions.Sigmoid => d => d*(1-d),
            ActivationFunctions.Tanh    => d => 1/(MathF.Cosh(d)*MathF.Cosh(d)),
            ActivationFunctions.ReLu    => d => 1,
            _                           => d => 1,
        };
}