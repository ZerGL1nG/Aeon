using AI.NeuralNetwork;
using AI.NeuralNetwork.Algs;
using static Tensorflow.Binding;
using static Tensorflow.KerasApi;
using Tensorflow;
using Tensorflow.NumPy;


var net = new NetworkTest.NeuralNetXor();
net.Run(false);

Console.ReadKey();

/*
var examples = np.array(new float[,]
{
    { 0, 0, 0 },
    { 0, 0, 1 }, 
    { 0, 1, 0 },
    { 0, 1, 1 },
    { 1, 0, 0 },
    { 1, 0, 1 },
    { 1, 1, 0 },
    { 1, 1, 1 },
});

var answers = np.array(new float[,] { 
    { 0 }, { 1 }, { 1 }, { 0 }, 
    { 1 }, { 0 }, { 0 }, { 1 },
});

var _y = np.array(new int[,] { 
    { 0 }, { 1 }, { 1 }, { 0 }, 
    { 1 }, { 0 }, { 0 }, { 1 },
});

(Operation, Tensor, Tensor) make_graph(Tensor input, Tensor output, int num_hidden = 8)
{
    var hiddenWeights1 = tf.Variable(
        tf.truncated_normal(new[] { 3, num_hidden }, seed: 1, stddev: 1 / MathF.Sqrt(2))
    );
    var hiddenActivations1 = tf.nn.relu(tf.matmul(input, hiddenWeights1));
    var hiddenWeights2 = tf.Variable(
        tf.truncated_normal(new[] { num_hidden, num_hidden }, seed: 1, stddev: 1 / MathF.Sqrt(2))
    );
    var hiddenActivations2 = tf.nn.sigmoid(tf.matmul(hiddenWeights1, hiddenWeights2));
    var outputWeights = tf.Variable(tf.truncated_normal(
        new[] {num_hidden, 1}, seed: 17, stddev: 1 / MathF.Sqrt(2)
    ));
    var logits = tf.matmul(hiddenActivations2, outputWeights);
    var predictions = tf.sigmoid(tf.squeeze(logits));
    var loss = tf.reduce_mean(tf.square(predictions - tf.cast(output, tf.float32), "loss"));

    var gs = tf.Variable(0, false, name: "global");
    var trainOp = tf.train.AdamOptimizer(0.001f).minimize(loss, gs);

    return (trainOp, loss, gs);
}

var graph = tf.Graph().as_default();
var input = tf.placeholder(tf.float32, (4, 2));
var output = tf.placeholder(tf.int32, 4);

var (trainOp, loss, gs) = make_graph(examples, answers, 64);
float loss_value = 0;

using var sess = tf.Session(graph);
sess.run(tf.global_variables_initializer());

for (var step = 0; step < 5000;)
{
    (_, step, loss_value) = sess.run((trainOp, gs, loss), (input, examples), (output, _y));
    if (step == 1 || step % 1000 == 0) Console.WriteLine($"Step {step} loss: {loss_value}");
}
Console.WriteLine($"Final loss: {loss_value}");


var examples2 = new float[][]
{
    new float[]{ 0, 0, 0 },
    new float[]{ 0, 0, 1 }, 
    new float[]{ 0, 1, 0 },
    new float[]{ 0, 1, 1 },
    new float[]{ 1, 0, 0 },
    new float[]{ 1, 0, 1 },
    new float[]{ 1, 1, 0 },
    new float[]{ 1, 1, 1 },
};

var answers2 = new float[] { 0, 1, 1, 0, 1, 0, 0, 1 };


var netv = NetworkCreator.Perceptron(3, 1, new[] { 10 });

for (int j = 0; j < 100; j++)
{
    //var data = new ArrayData(examples2[1]);
    //var res = netv.Work(data)[0];
    //Console.WriteLine(res);
    //var loss = answers2[1] - res;
    //BackpropagationAlgorithm.BackPropOut(netv, data, loss, 0, 0.1f);

    var part = 8;
    

    
    for (int i = 0; i < 10000; i++)
    {
        var t = Random.Shared.Next(part);
        var data = new ArrayData(examples2[t]);
        var loss2 = answers2[t] - netv.Work(data)[0];
        BackpropagationAlgorithm.BackPropOut(netv, data, loss2, 0, 0.01f);
    }

    Console.Write($"{j,5}: ");

    for (int i = 0; i < part; i++)
    {
        var data = new ArrayData(examples2[i]);
        Console.Write($"{netv.Work(data)[0]:+#0.000;-#0.000;+0.000} ");
    }

    Console.WriteLine();
    
}

*/