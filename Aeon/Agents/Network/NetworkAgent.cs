using System.Collections.Generic;
using Aeon.Core;
using Aeon.Core.GameProcess;
using AI.NeuralNetwork;

namespace Aeon.Agents.Network;

public class NetworkAgent: IAgent
{
    protected NetworkBattleViewer _battleView;

    protected HeroClasses _myClass;
    protected NetworkShopViewer _shopView;

    public NetworkAgent(NeuralEnvironment network, HeroClasses myClass)
    {
        _battleView = new NetworkBattleViewer();
        _shopView   = new NetworkShopViewer();
        Network     = network;
        _myClass    = myClass;
    }

    protected NetworkAgent(NeuralEnvironment network) => Network = network;

    public NeuralEnvironment Network { get; protected set; }

    public virtual IBattleViewer BattleView => _battleView;
    public virtual IShopViewer ShopView => _shopView;
    public bool IsBot => true;

    public virtual Command ShopDecision()
    {
        Network.Work(MakeInput());
        var workResult = Network.GetMaxOutId();
        return Command.Parse(workResult);
    }

    public HeroClasses ChooseClass()
    {
        if ((int)_myClass == -1) return (HeroClasses)Program.rnd.Next(HeroMaker.TotalClasses);
        return _myClass;
    }

    public virtual void OnGameStart()
    {
        _battleView = new NetworkBattleViewer();
        _shopView   = new NetworkShopViewer();
    }

    public virtual void OnGameOver(int winner) { }

    public static List<double> DeepArse(Command command)
    {
        var comNumber = 0;
        if (command.Ability)
            comNumber = 19;
        else if (command.Exit)
            comNumber = 18;
        else
            comNumber = (int)command.Type+(command.Opt? 9 : 0);
        return MakeCrap(comNumber);
    }

    private static List<double> MakeCrap(int whereIsCrap)
    {
        var list = new List<double>();
        for (var i = 0; i <= 19; i++) list.Add(0);
        list[whereIsCrap] = 1;
        return list;
    }

    /*private ComposeData MakeInput()
    {
        var enemyList = new List<float>();
        for (var i = 0; i < HeroMaker.TotalClasses; i++) enemyList.Add(0);
        enemyList[_shopView.EnemyNumber] = 1;

        var govno = new ComposeData(_battleView, _shopView, new ArrayData(enemyList));

        return govno;
    }*/
    protected ComposeData MakeInput() => new(_battleView.State, _shopView.State, MakeEnemyList(_shopView.EnemyNumber));

    protected static ArrayData MakeEnemyList(int enemyN)
    {
        var enemyList = new List<float>();
        for (var i = 0; i < HeroMaker.TotalClasses; i++) enemyList.Add(0);
        enemyList[enemyN] = 1;
        return new ArrayData(enemyList);
    }
}