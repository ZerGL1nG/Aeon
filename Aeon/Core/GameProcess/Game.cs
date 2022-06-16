using System;

namespace Aeon.Core.GameProcess
{
    public class Game
    {
        public IAgent Agent1 { get; }
        public IAgent Agent2 { get; }

        public int Agent1Score { get; set; } = 0;
        public int Agent2Score { get; set; } = 0;
        public int TotalBattles { get; set; } = 0;

        public const int TargetWins = 1;
        public const int MaxBattles = 1;

        public static int GetWinner(int score1, int score2, int total)
        {
            if (score1 >= TargetWins)
                return 1;

            if (score2 >= TargetWins)
                return -1;

            if (total >= MaxBattles)
                return score1 > score2 ? 1 : score2 > score1 ? -1 : 0;

            return 0;
        }

        public int Winner => GetWinner(Agent1Score, Agent2Score, TotalBattles);

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
            Agent1.OnGameStart();
            Agent2.OnGameStart();
            if (debug)
            {
                Console.WriteLine();
                Console.WriteLine("---------------------------------");
            }
            while (hero1.TotalWins < TargetWins && hero2.TotalWins < TargetWins && TotalBattles < MaxBattles) {
                new Shopping(hero1, Agent1.ShopView, Agent1, Agent1.IsBot).StartShopping(debug);
                new Shopping(hero2, Agent2.ShopView, Agent2, Agent2.IsBot).StartShopping(debug);



                var b = new Battle(Agent1.BattleView, Agent2.BattleView, hero1, hero2);
                var bw = b.StartBattle();
                ++TotalBattles;
                
                if (debug)
                {
                    Console.WriteLine($"Hero 1: {Agent1.ChooseClass()}{(bw == 1? " <== WINNER" : "")}");
                    foreach (var (stat, value) in hero1.Stats.Out()) {
                        Stat.TryParse(stat, out Stat s);
                        Console.Write($"{stat}: {Stats.RoundStat(value, s)}; ");
                    }
                    Console.WriteLine();
                    Console.WriteLine($"Hero 2: {Agent2.ChooseClass()}{(bw == -1? " <== WINNER" : "")}");
                    foreach (var (stat, value) in hero2.Stats.Out()) {
                        Stat.TryParse(stat, out Stat s);
                        Console.Write($"{stat}: {Stats.RoundStat(value, s)}; ");
                    }
                    Console.WriteLine();
                    Console.WriteLine("---------------------------------");
                }

                if (hero1.AutoLose) {
                    hero1.TotalWins = 0;
                    hero2.TotalWins = TargetWins;
                }

                if (hero2.AutoLose) {
                    hero2.TotalWins = 0;
                    hero1.TotalWins = TargetWins;
                }
            }

            if (Program.debugOutput || debug) {
                Console.WriteLine("---------------------------------");
                Console.WriteLine(
                    $"Игра: {Agent1.ChooseClass()} {hero1.TotalWins} - {hero2.TotalWins} {Agent2.ChooseClass()}, " +
                    $"число игр {TotalBattles}");
                Console.WriteLine();
                Console.WriteLine("===============================");
                Console.WriteLine();
            }

            Agent1Score = hero1.TotalWins;
            Agent2Score = hero2.TotalWins;
            Agent1.OnGameOver(Winner);
            Agent2.OnGameOver(-Winner);
            
            return (hero1.TotalWins, hero2.TotalWins);
        }
    }
}