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

public class QAgent : NetworkAgent
{
    private QBattleViewer _battleViewer;

    private int _battleNumber = 0;
    //private int _lastCmd = -1;

    private NeuralEnvironment Q;

    private Queue<Sample> _memory = new();
    private List<Sample> _preMem = new();

    private const int MemSize = 2000;
    private const int BatchSize = 100;
    private const int SessionSize = 100;

    private int _sessionCounter = 0;
    private readonly QAgentSettings _s = new(.75f, .01f, .99f);


    private const float EarlyOptPunish = 100;
    private const float NEMPunish = 25;
    private const float EarlyStatReward = 1f;
    private const float GameWinReward = +100f;
    

    //public QAgent(HeroClasses myClass, int[] hiddenLayers)
    //{
    //    _myClass = myClass;
    //    Q = NetworkCreator.Perceptron(90, 20, hiddenLayers);
    //    Qt = Q.Clone();
    //}

    public QAgent(NeuralEnvironment env, HeroClasses myClass): base(env)
    {
        _myClass = myClass;
        Q = env.Clone();
    }

    public override void OnGameStart()
    {
        base.OnGameStart();
        _battleView = new QBattleViewer();
        _battleViewer = _battleView as QBattleViewer;
    }


    private ArrayData MakeEnemyList(int enemyN)
    {
        var enemyList = new List<float>();
        for (var i = 0; i < HeroMaker.TotalClasses; i++)
            enemyList.Add(0);
        enemyList[enemyN] = 1;
        return new ArrayData(enemyList);
    }

    private ComposeData MakeInput() => new(_battleViewer.State, _shopView.State, 
        MakeEnemyList(_shopView.EnemyNumber));

    public override Command ShopDecision()
    {
        INetworkData data = MakeInput();
        var netResult = Network.Work(data);
        int cmd = EpsGreedy(netResult, _s.Epsilon);
        Command command = Command.Parse(cmd);
        
        if (_battleViewer.WasBattle)
        {
            _preMem[^1] = _preMem[^1] with { Reward = _battleViewer.Reward, NextState = MakeInput() };
            _battleNumber = _battleViewer.ModelTotalBattles;
            _battleViewer.WasBattle = false;
            _preMem.ForEach(m => _memory.Enqueue(m));
            _preMem = new();
        }
        
        float shopReward = 0;
        if (_battleNumber == 0)
        {
            if (command.Opt) shopReward -= EarlyOptPunish;
            else if (command.Type is Stat.Health or Stat.Attack or Stat.Spell)
                shopReward += EarlyStatReward * _shopView.GetCost(command);
        }

        if (!command.Ability && !command.Exit && !_shopView.CanBuy(command)) shopReward -= NEMPunish;

        //reward = _battleViewer.Reward;
        if (_preMem.Count != 0)
            _preMem[^1] = _preMem[^1] with { NextState = MakeInput() };
        _preMem.Add(new Sample(MakeInput(), cmd, shopReward, null));
        
        _sessionCounter++;
        
        while (_memory.Count > MemSize)
            _ = _memory.Dequeue();
        
        if (_sessionCounter >= SessionSize)
        {
            _sessionCounter = 0;
            BatchLearn();
            Network = Q.Clone();
        }
        
        return command;
    }

    public override void OnGameOver(int winner)
    {
        base.OnGameOver(winner);
        var reward = _battleViewer.Reward + (winner == 1 ? GameWinReward : 0);
        _preMem[^1] = _preMem[^1] with { Reward = reward, NextState = null };
        _preMem.ForEach(m => _memory.Enqueue(m));
        _preMem = new();
    }

    private void BatchLearn()
    {
        List<Sample> samples = new(_memory);
        for (int x = 0; x < Math.Min(BatchSize, _memory.Count); ++x)
        {
            var pos = Random.Shared.Next(x, samples.Count - 1);
            (samples[pos], samples[x]) = (samples[x], samples[pos]);
        }

        var batch = samples.Take(Math.Min(BatchSize, _memory.Count)).ToList();

        var dict = new Dictionary<List<float>, List<float>>();

        var maxloss = 0.0f;

        double lossx = 0;
        
        foreach (Sample sample in batch)
        {
            var rt = sample.NextState is null ? 0 : Network.Work(sample.NextState).Max();
            var kek = Network.Work(sample.State).Max();
            var loss = rt * _s.Gamma + sample.Reward - kek;
            lossx += (double)loss * loss;
            maxloss = Math.Max(maxloss, loss);
            BackpropagationAlgorithm.BackPropOut(Q, sample.State, -loss, sample.Action, _s.Speed);
        }
        
        //System.Console.WriteLine($"Обучение: loss={Math.Sqrt(lossx/samples.Count)}; lossmax={maxloss}; ");
    }

    private int EpsGreedy(List<float> values, float eps)
    {
        if (Random.Shared.NextSingle() < eps)
            return Random.Shared.Next(values.Count);
        var max = float.MinValue;
        var maxId = -1;
        for (int i = 0; i < values.Count; i++)
            if (max < values[i])
            {
                max = values[i];
                maxId = i;
            }
        return maxId;
    }

    private record Sample(
        INetworkData State,
        int Action,
        float Reward,
        INetworkData NextState
    );
}