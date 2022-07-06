using System;
using System.Collections.Generic;
using System.Linq;
using Aeon.Agents.Network;
using Aeon.Core;
using Aeon.Core.GameProcess;
using AI.NeuralNetwork;
using AI.NeuralNetwork.Algs;

namespace Aeon.Agents.Reinforcement;

public record QAgentSettings(float Epsilon, float Speed, float Gamma);

public class QAgent: NetworkAgent
{
    private const int MemSize = 500;
    private const int BatchSize = 500;
    private const int SessionSize = 125;


    //private const float EarlyOptPunish = 5f;
    //private const float NEMPunish = 2f;
    //private const float EarlyStatReward = 0f;
    private const float GameWinReward = 10f;

    private int _battleNumber;
    private QBattleViewer _battleViewer;
    private bool _learn;

    private readonly Queue<Sample> _memory = new();
    private List<Sample> _preMem = new();

    private QAgentSettings _s;

    private int _sessionCounter;
    //private int _lastCmd = -1;

    private readonly NeuralEnvironment Q;


    //public QAgent(HeroClasses myClass, int[] hiddenLayers)
    //{
    //    _myClass = myClass;
    //    Q = NetworkCreator.Perceptron(90, 20, hiddenLayers);
    //    Qt = Q.Clone();
    //}

    public QAgent(NeuralEnvironment env, HeroClasses myClass) : base(env)
    {
        LearnMode = true;
        _myClass  = myClass;
        Q         = env.Clone();
    }

    public bool LearnMode {
        get => _learn;
        set {
            _learn = value;
            _s     = value? new QAgentSettings(0.02f, 0.02f, 0.98f) : new QAgentSettings(0f, 0f, .98f);
        }
    }

    private float UnspentMoneyPunish => MathF.Pow(_shopView.MoneyF*0.01f, 2);

    public override void OnGameStart()
    {
        base.OnGameStart();
        _battleView   = new QBattleViewer();
        _battleViewer = _battleView as QBattleViewer;
    }
    public override Command ShopDecision()
    {
        INetworkData data      = MakeInput();
        var          netResult = Network.Work(data);
        var          cmd       = EpsGreedy(netResult, _s.Epsilon);
        var          command   = Command.Parse(cmd);

        if (_battleViewer.WasBattle) {
            _preMem[^1]             = _preMem[^1] with { Reward = _battleViewer.Reward, NextState = data, };
            _battleNumber           = _battleViewer.ModelTotalBattles;
            _battleViewer.WasBattle = false;
            _preMem.ForEach(m => _memory.Enqueue(m));
            _preMem = new List<Sample>();
        }

        float shopReward = 0;

        if (_shopView.WillExit(command)) shopReward -= UnspentMoneyPunish;

        //reward = _battleViewer.Reward;
        if (_preMem.Count != 0) _preMem[^1] = _preMem[^1] with { NextState = MakeInput(), };
        _preMem.Add(new Sample(MakeInput(), cmd, shopReward, null));

        _sessionCounter++;

        while (_memory.Count > MemSize) _ = _memory.Dequeue();

        if (_sessionCounter >= SessionSize) {
            _sessionCounter = 0;
            BatchLearn();
            Network = Q.Clone();
        }

        return command;
    }

    public override void OnGameOver(int winner)
    {
        base.OnGameOver(winner);
        var reward = _preMem[^1].Reward+(winner == 1? GameWinReward : 0);
        _preMem[^1] = _preMem[^1] with { Reward = reward, NextState = null, };
        _preMem.ForEach(m => _memory.Enqueue(m));
        _preMem = new List<Sample>();
    }

    private void BatchLearn()
    {
        if (_s.Speed == 0) return;
        List<Sample> samples = new(_memory);
        List<Sample> batch;
        if (BatchSize != MemSize)
#pragma warning disable CS0162
        {
            for (var x = 0; x < Math.Min(BatchSize, _memory.Count); ++x) {
                var pos = Random.Shared.Next(x, samples.Count-1);
                (samples[pos], samples[x]) = (samples[x], samples[pos]);
            }

            batch = samples.Take(Math.Min(BatchSize, _memory.Count)).ToList();
        }
#pragma warning restore CS0162
        batch = samples;

        var dict = new Dictionary<List<float>, List<float>>();

        var maxloss = 0.0f;

        double lossx = 0;

        foreach (var sample in batch) {
            var rt   = sample.NextState is null? 0 : Network.Work(sample.NextState).Max();
            var kek  = Q.Work(sample.State)[sample.Action];
            var loss = rt*_s.Gamma+sample.Reward-kek;
            lossx   += (double)loss*loss;
            maxloss =  Math.Max(maxloss, loss);
            Q.Save(Program.dump+"/Qbefore");
            BackpropagationAlgorithm.BackPropOut(Q, sample.State, loss, sample.Action, _s.Speed);
            Q.Save(Program.dump+"/Qafter");
            System.Console.Write("");
        }

        //System.Console.WriteLine($"Обучение: loss={Math.Sqrt(lossx/samples.Count)};");
    }

    private int EpsGreedy(List<float> values, float eps)
    {
        if (Random.Shared.NextSingle() < eps) return Random.Shared.Next(values.Count);
        var max   = float.MinValue;
        var maxId = -1;
        for (var i = 0; i < values.Count; i++)
            if (max < values[i]) {
                max   = values[i];
                maxId = i;
            }
        return maxId;
    }

    private record Sample(INetworkData State, int Action, float Reward, INetworkData NextState);
}