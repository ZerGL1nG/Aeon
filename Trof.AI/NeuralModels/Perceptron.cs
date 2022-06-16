using Tensorflow;
using Tensorflow.Keras.Engine;
using Tensorflow.NumPy;

namespace Trof.AI;

public class Perceptron<TInput, TOutput> : INeuralEnv<TInput, TOutput> 
    where TInput : INetworkData where TOutput : INetworkData
{
    protected readonly Model Model;
    private string? _path;
    
    public Perceptron(params LayerSettings[] hiddenLayers) : this(ActFunc.Sigmoid, hiddenLayers) { }

    public Perceptron(ActFunc final, params LayerSettings[] hiddenLayers)
    {
        Model = ModelMaker.Perceptron(
            (TInput.Size, ActFunc.None), (TOutput.Size, final), hiddenLayers);
    }

    public void Init(TrofDataset<TInput, TOutput> initset, string dir, string name)
    {
        _path = Path.Combine(dir, name);
        if (!File.Exists(_path)) return;
        Model.fit(initset.InputArray, initset.OutputArray, epochs: 1, verbose: 0);
        Model.load_weights(_path, skip_mismatch: true);
    }


    public void Train(TrofDataset<TInput, TOutput> dataset, int epochs, int verbose = 0, int batch = -1, int cx = -1)
    {
        Model.fit(dataset.InputArray, dataset.OutputArray, batch,
            epochs: epochs, verbose: verbose, use_multiprocessing: true);
        if (_path != null)
        {
            Model.save_weights($"{_path}.h5");
            if (cx > 0) Model.save_weights($"{_path}_{cx}.h5");
        }
    }

    private Tensor Call2(TInput input)
    {
        var dx = new float[1, TInput.Size];
        var x = 0;
        foreach (var v in input.Data)
        {
            dx[0, x] = v;
            ++x;
        }
        var test = np.array(dx);
        return Model.predict(test);
    }

    public TOutput Call(TInput input)
    {
        var tensor = Call2(input);
        return (TOutput) TOutput.Ret(tensor.ToArray<float>());
    }
}