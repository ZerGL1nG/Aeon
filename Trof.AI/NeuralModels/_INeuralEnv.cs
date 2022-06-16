using Tensorflow.Keras;
using Tensorflow.Keras.ArgsDefinition;
using Tensorflow.Keras.Engine;
using Tensorflow.Keras.Layers;
using Tensorflow.Keras.Utils;

namespace Trof.AI;

public interface INeuralEnv<TInput, TOutput> where TInput : INetworkData where TOutput : INetworkData
{
    TOutput Call(TInput input);
}

public static class ModelMaker
{
    public static Model Perceptron(LayerSettings start, LayerSettings final, params LayerSettings[] layers)
    {
        var model = new Sequential(new());
        var x = new List<LayerSettings>().Append(start).Concat(layers).Append(final);
        foreach (var (amount, actFunc) in x)
            model.Layers.Add(keras.layers.Dense(amount, actFunc.Getf()));

        model.compile(
            keras.optimizers.RMSprop(), 
            keras.losses.MeanSquaredError(),
            metrics: Array.Empty<string>()
        );
        return model;
    }

    public static Activation Getf(this ActFunc func) => func switch
    {
        ActFunc.None => keras.activations.Linear,
        ActFunc.Sigmoid => keras.activations.Sigmoid,
        ActFunc.Softmax => keras.activations.Softmax,
        ActFunc.Tanh => keras.activations.Tanh,
        ActFunc.ReLu => keras.activations.Relu,
        _ => throw new ArgumentOutOfRangeException(nameof(func), func, null)
    };
}

public record LayerSettings(int Amount, ActFunc Func)
{
    public static implicit operator LayerSettings(int x) => new(x, ActFunc.ReLu);
    public static implicit operator LayerSettings((int x, ActFunc a) c) => new(c.x, c.a);
}

public enum ActFunc
{
    None,
    Sigmoid,
    Softmax,
    Tanh,
    ReLu,
}