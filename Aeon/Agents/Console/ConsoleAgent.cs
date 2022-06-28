using System.Collections.Generic;
using Aeon.Core;
using Aeon.Core.GameProcess;

namespace Aeon.Agents.Console;

public class ConsoleAgent: IAgent
{
    private readonly ConsoleBattleViewer _bw;
    private readonly ConsoleShopViewer _sw;

    //public Dictionary<(BattleViewer, ShopViewer), Command> DataSet 
    //   = new Dictionary<(BattleViewer, ShopViewer), Command>();

    public Dictionary<List<double>, List<double>> DataSet = new();

    private bool IsChosen;

    //private bool EnterFlag;
    private HeroClasses myClass;

    public ConsoleAgent()
    {
        _bw = new ConsoleBattleViewer();
        _sw = new ConsoleShopViewer();
    }

    public IBattleViewer BattleView => _bw;
    public IShopViewer ShopView => _sw;
    public bool IsBot => false;

    public Command ShopDecision()
    {
        while (true) {
            var key = KeysMap.GetIndex(System.Console.ReadKey().Key);
            if (key is null) continue;
            return Command.Parse(key.Value);
        }
    }

    public HeroClasses ChooseClass()
    {
        while (!IsChosen) {
            System.Console.Clear();
            for (var cl = 0; cl < HeroMaker.TotalClasses; ++cl)
                System.Console.WriteLine($"Hero #{cl,2}: {(HeroClasses)cl}");
            System.Console.Write("Выбранный герой: ");
            if (int.TryParse(System.Console.ReadLine(), out var num) && num < HeroMaker.TotalClasses && num >= 0) {
                System.Console.WriteLine(num);
                myClass  = (HeroClasses)num;
                IsChosen = true;
            } else {
                System.Console.Error.WriteLine("Нет такого героя, соси");
                System.Console.ReadLine();
            }
        }
        return myClass;
    }

    public void OnGameStart() { }

    public void OnGameOver(int winner) { }
}