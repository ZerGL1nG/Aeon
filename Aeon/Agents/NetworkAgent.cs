using System.Collections.Generic;
using System.Linq;
using Aeon.Core;
using Aeon.Core.GameProcess;
using AI.NeuralNetwork;

namespace Aeon.Agents;

public class NetworkAgent : IAgent
{
    public readonly NeuralEnvironment Network;
        
    private readonly HeroClasses _myClass;
        
    public NetworkAgent(NeuralEnvironment network, HeroClasses myClass)
    {
        _battleView = new NetworkBattleViewer();
        _shopView = new NetworkShopViewer();
        Network = network;
        _myClass = myClass;
    }

    public IBattleViewer BattleView => _battleView;
    private readonly NetworkBattleViewer _battleView;
    public IShopViewer ShopView => _shopView;
    private readonly NetworkShopViewer _shopView;
    public bool IsBot => true;

    public Command ShopDecision()
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

    private ComposeInput MakeInput()
    {
        var enemyList = new List<float>();
        for (var i = 0; i < HeroMaker.TotalClasses; i++)
            enemyList.Add(0);
        enemyList[_shopView.EnemyNumber] = 1;

        var govno = new ComposeInput(_battleView, _shopView, new ArrayInput(enemyList));

        return govno;
    }
}