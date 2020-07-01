using System;
using System.Collections.Generic;
using System.Linq;
using Aeon.Core.Heroes;

namespace Aeon.Core.GameProcess.Agents
{
    public class ConsoleAgent : IAgent
    {
        public BattleViewer BattleView { get; set; }
        public ShopViewer ShopView { get; set; }
        public bool IsBot => false;

        private bool IsChosen = false;

        private HeroClasses myClass;

        //public Dictionary<(BattleViewer, ShopViewer), Command> DataSet 
         //   = new Dictionary<(BattleViewer, ShopViewer), Command>();
         
        public Dictionary<List<double>, List<double>> DataSet = new Dictionary<List<double>, List<double>>();

        public ConsoleAgent()
        {
            BattleView = new BattleViewer();
            ShopView = new ShopViewer();
            KeysDict = KeysList.Select((k, r) => (k, r)).ToDictionary(tuple => tuple.k, tuple => tuple.r);
        }

        private readonly Dictionary<ConsoleKey, int> KeysDict;
        private readonly List<ConsoleKey> KeysList = new List<ConsoleKey> {
            ConsoleKey.Q, 
            ConsoleKey.W, 
            ConsoleKey.E, 
            ConsoleKey.R, 
            ConsoleKey.T, 
            ConsoleKey.Y,
            ConsoleKey.U, 
            ConsoleKey.I, 
            ConsoleKey.O, 
            ConsoleKey.A,
            ConsoleKey.S, 
            ConsoleKey.D, 
            ConsoleKey.F, 
            ConsoleKey.G,
            ConsoleKey.H,
            ConsoleKey.J, 
            ConsoleKey.K, 
            ConsoleKey.L, 
            ConsoleKey.Enter, 
            ConsoleKey.Spacebar,
        };

        public Command ShopDecision()
        {
            Console.Clear();
            Console.WriteLine("Бой:");
            foreach (var turn in BattleView.Out()) {
                var t = turn.ToList();
                Console.WriteLine($"   ТЫ: {t[1].value, 5}/{t[0].value, -5}-{t[2].value, -5}+{t[3].value, -5}" +
                                $"   ВРАГ: {t[5].value, 5}/{t[4].value, -5}-{t[6].value, -5}+{t[7].value, -5}");
            }
            Console.WriteLine();
            var (stats, shop, enemyId, other) = ShopView.Out();
            
            for (var t = 1; t <= 9; ++t) {
                var stat = (Stat) t;
                var price = shop.Costs[stat];
                Console.WriteLine($"{stat, 10}: {Stats.RoundStat(stats.GetStat(stat), stat), 5} " +
                   $"|| Buy[{KeysList[t-1]}]: {shop.RoundBonus(stats.GetStat(stat), stat, false), 2} for {price.standard.cost, -4}" +
                   $"|  Opt[{KeysList[t+8]}]: {shop.RoundBonus(stats.GetStat(stat), stat,  true), 3} for {price.discount.cost, -4}");
            }
            Console.WriteLine();
            Console.WriteLine($"       {myClass, -15} Money: {stats.GetStat(Stat.Money), -10} Ability[{KeysList[^1]}]: {other[3]}");
            Console.WriteLine();

            int com;
            while (!KeysDict.TryGetValue(Console.ReadKey().Key, out com)) { }
            DataSet.Add(NetworkAgent.MakeInput(this), NetworkAgent.MakeCrap(com));
            return NetworkAgent.parseCommand(com);
        }
        
        

        public HeroClasses ChooseClass()
        {
            while (!IsChosen) {
                Console.Clear();
                for (int cl = 0; cl < HeroMaker.TotalClasses; ++cl)
                    Console.WriteLine($"Hero #{cl, 2}: {(HeroClasses)(cl)}");
                Console.Write("Выбранный герой: ");
                if (int.TryParse(Console.ReadLine(), out var num) && num < HeroMaker.TotalClasses && num >= 0) {
                    Console.WriteLine(num);
                    myClass = (HeroClasses) num;
                    IsChosen = true;
                }
                else {
                    Console.Error.WriteLine("Нет такого героя, соси");
                    Console.ReadLine();
                }
            }
            return myClass;
        }
    }
}