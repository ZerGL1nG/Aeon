using System;

namespace Aeon.Core.GameProcess
{
    public class Game
    {
        public IAgent Agent1 { get; private set; }
        public IAgent Agent2 { get; private set; }

        public int Agent1Score { get; set; } = 0;
        public int Agent2Score { get; set; } = 0;
        public int TotalBattles { get; set; } = 0;

        public const int TargetWins = 5;
        public const int MaxBattles = 30;

        public Game(IAgent agent1, IAgent agent2)
        {
            Agent1 = agent1;
            Agent2 = agent2;
        }

        public (int Agent1Score, int Agent2Score) Start(bool debug = false)
        {
            var hero1 = HeroMaker.Make(Agent1.ChooseClass());
            var hero2 = HeroMaker.Make(Agent2.ChooseClass());
            hero1.Init(hero2);
            hero2.Init(hero1);
            while (hero1.TotalWins < TargetWins && hero2.TotalWins < TargetWins && TotalBattles < MaxBattles) {
                Agent1.ShopView.BattleNumber = TotalBattles;
                Agent2.ShopView.BattleNumber = TotalBattles;
                new Shopping(hero1, Agent1.ShopView, Agent1, Agent1.IsBot).StartShopping(TotalBattles);
                new Shopping(hero2, Agent2.ShopView, Agent2, Agent2.IsBot).StartShopping(TotalBattles);

                if (debug) {
                    Console.WriteLine($"Hero 1: {Agent1.ChooseClass()}");
                    foreach (var (stat, value) in hero1.Stats.Out()) {
                        Console.WriteLine($"{stat}: {value}");
                    }
                    Console.WriteLine();
                    Console.WriteLine($"Hero 2: {Agent2.ChooseClass()}");
                    foreach (var (stat, value) in hero2.Stats.Out()) {
                        Console.WriteLine($"{stat}: {value}");
                    }
                    Console.WriteLine();
                    Console.WriteLine("---------------------------------");
                    Console.WriteLine();
                }

                new Battle(Agent1.BattleView, Agent2.BattleView, hero1, hero2).StartBattle();
                ++TotalBattles;
                if (hero1.AutoLose) {
                    hero1.TotalWins = 0;
                    hero2.TotalWins = TargetWins;
                }
                if (hero2.AutoLose) {
                    hero2.TotalWins = 0;
                    hero1.TotalWins = TargetWins;
                }
            }
            if (Program.debugOutput || debug) Console.WriteLine($"Игра: {Agent1.ChooseClass()} {hero1.TotalWins} - {hero2.TotalWins} {Agent2.ChooseClass()}, число игр {TotalBattles}");
            return (hero1.TotalWins, hero2.TotalWins);
        }
    }
}