using Tensorflow.Keras.Engine;

namespace Trof.AI;

public class QPerceptron<TInput, TOutput> : Perceptron<TInput, TOutput>
    where TInput : INetworkData where TOutput : INetworkData
{

    protected readonly Model Target;
    
    public QPerceptron(ActFunc final, params LayerSettings[] hiddenLayers) : base(final, hiddenLayers)
    {
        Target = ModelMaker.Perceptron(
            (TInput.Size, ActFunc.None), (TOutput.Size, final), hiddenLayers);
    }
    
    
}