using System.Collections.Generic;
using Aeon.Core;
using Aeon.Core.GameProcess;
using AI.NeuralNetwork;

namespace Aeon.Agents.Network;

public class NetworkAgent : IAgent
{
    public NeuralEnvironment Network { get; protected set; }
        
    protected HeroClasses _myClass;
        
    public NetworkAgent(NeuralEnvironment network, HeroClasses myClass)
    {
        _battleView = new NetworkBattleViewer();
        _shopView = new NetworkShopViewer();
        Network = network;
        _myClass = myClass;
    }

    protected NetworkAgent(NeuralEnvironment network)
    {
        Network = network;
    }

    public virtual IBattleViewer BattleView => _battleView;
    protected NetworkBattleViewer _battleView;
    public virtual IShopViewer ShopView => _shopView;
    protected NetworkShopViewer _shopView;
    public bool IsBot => true;

    public virtual Command ShopDecision()
    {
        Network.Work(MakeInput());
        var workResult = Network.GetMaxOutId();
        return Command.Parse(workResult);
    }

    public HeroClasses ChooseClass()
    {
        if ((int) _myClass == -1) return (HeroClasses) Program.rnd.Next(HeroMaker.TotalClasses);
        return _myClass;
    }

    public virtual void OnGameStart()
    {
        _battleView = new NetworkBattleViewer();
        _shopView = new NetworkShopViewer();
    }
    public virtual void OnGameOver(int winner) { }

    public static List<double> DeepArse(Command command)
    {
        int comNumber = 0;
        if (command.Ability) comNumber = 19;
        else if (command.Exit) comNumber = 18;
        else comNumber = (int) command.Type + (command.Opt ? 9 : 0);
        return MakeCrap(comNumber);
    }

    private static List<double> MakeCrap(int whereIsCrap)
    {
        var list = new List<double>();
        for (int i = 0; i <= 19; i++) 
            list.Add(0);
        list[whereIsCrap] = 1;
        return list;
    }

    private ComposeData MakeInput()
    {
        var enemyList = new List<float>();
        for (var i = 0; i < HeroMaker.TotalClasses; i++)
            enemyList.Add(0);
        enemyList[_shopView.EnemyNumber] = 1;

        var govno = new ComposeData(_battleView, _shopView, new ArrayData(enemyList));

        return govno;
    }
}