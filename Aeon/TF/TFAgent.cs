using System;
using Tensorflow.Keras.Engine;
using Tensorflow.NumPy;
using static Tensorflow.KerasApi;

namespace Aeon.TF;

public class DQAgent { }

public class TFAgent
{
    private readonly int _actions = 0;
    private int _step;
    private readonly Func<int, float> ExploreStrat = TFTEST.EpsilonGreedy(1, 0, .001f);

    public (int, float, bool) Act(Model policy)
    {
        var rate = ExploreStrat(_step);
        ++_step;
        if (rate > Random.Shared.NextDouble()) return (Random.Shared.Next(_actions), rate, true);
        return (np.argmax(7), rate, false);
    }
}

public static class TFTEST
{
    public static Func<int, float> EpsilonGreedy(float start, float end, float decay) =>
        stepN => end+(start-end)*MathF.Exp(-stepN*decay);

    public static void Test()
    {
        var agent = keras.Sequential();
        agent.add(keras.Input(90));
        agent.add(keras.layers.Dense(50, keras.activations.Sigmoid));
        agent.add(keras.layers.Dense(30, keras.activations.Sigmoid));
        agent.add(keras.layers.Dense(20, keras.activations.Linear));
    }
}