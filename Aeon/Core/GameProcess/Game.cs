using System;

namespace Aeon.Core.GameProcess
{
    public class Game
    {
        public IAgent Agent1 { get; private set; }
        public IAgent Agent2 { get; private set; }

        public int Agent1Score { get; set; } = 0;
        public int Agent2Score { get; set; } = 0;

        public Game(IAgent agent1, IAgent agent2)
        {
            Agent1 = agent1;
            Agent2 = agent2;
        }

        public (int Agent1Score, int Agent2Score) Start()
        {
            var hero1 = HeroMaker.Make(Agent1.ChooseClass());
            var hero2 = HeroMaker.Make(Agent2.ChooseClass());
            hero1.Init(hero2);
            hero2.Init(hero1);
            while (!hero1.Won() && !hero2.Won() && hero1.TotalBattles < 11) {
                new Shopping(hero1, Agent1.ShopView, Agent1, Agent1.IsBot).StartShopping();
                new Shopping(hero2, Agent2.ShopView, Agent2, Agent2.IsBot).StartShopping();
                
                new Battle(Agent1.BattleView, Agent2.BattleView, hero1, hero2).StartBattle();
            }
            Console.WriteLine($"Игра: счет {hero1.TotalWins} - {hero2.TotalWins}, число игр {hero1.TotalBattles}");
            return (hero1.TotalWins, hero2.TotalWins);
        }
    }
}