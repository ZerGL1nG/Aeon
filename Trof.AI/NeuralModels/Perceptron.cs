using Tensorflow;
using Tensorflow.Keras.Engine;
using Tensorflow.NumPy;

namespace Trof.AI;

public class Perceptron<TInput, TOutput>: INeuralEnv<TInput, TOutput>
    where TInput: INetworkData where TOutput: INetworkData
{
    private readonly Model _model;
    private string? _path;
    private IEnumerable<(string filename, Model model)> _savingModels;

    public Perceptron(params LayerSettings[] hiddenLayers): this(Sigmoid, hiddenLayers) { }

    public Perceptron(ActFunc final, params LayerSettings[] hiddenLayers): this(Pass, final, hiddenLayers) { }

    public Perceptron(ActFunc start, ActFunc final, params LayerSettings[] hiddenLayers) =>
        _model = ModelMaker.Perceptron((TInput.Size, start), (TOutput.Size, final), hiddenLayers);

    public TOutput Call(TInput input) => _model.CallModel<TInput, TOutput>(input);

    public void StartEpisode(TInput initial) { }

    public void TerminateEpisode(TInput terminal) { }

    IEnumerable<(string filename, Model model)> INeuralEnv<TInput, TOutput>.GetSavingModels() => 
        new[] { ("model", _model) };

    public event Action<IEnumerable<(string filename, Model model)>>? OnSave;
    public event Action<IEnumerable<(string filename, Model model)>>? OnLoad;

    public void Load(string dir, string name)
    {
        _path = Path.Combine(dir, name);
        if (!File.Exists($"{_path}.h5")) return;
        _model.fit(np.zeros((1, TInput.Size), TF_DataType.TF_FLOAT), np.zeros((1, TOutput.Size), TF_DataType.TF_FLOAT),
                   epochs: 1, verbose: 0);
        _model.load_weights($"{_path}.h5", skip_mismatch: true);
    }


    public void Train(TrofDataset<TInput, TOutput> dataset, int epochs, int verbose = 0, int batch = -1, int cx = -1)
    {
        _model.fit(dataset.InputArray, dataset.OutputArray, batch, epochs, verbose, use_multiprocessing: true);
        if (_path == null) return;
        _model.save_weights($"{_path}.h5");
        if (cx > 0) _model.save_weights($"{_path}_{cx}.h5");
    }
}