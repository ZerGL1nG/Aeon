using System.Linq.Expressions;
using Aeon.Core.Heroes;

namespace Aeon.Core.GameProcess
{
    public class Battle
    {
        private const int maxBattleIt = 15;
        private Hero First { get;}
        private Hero Second { get; }

        private IBattleViewer Viewer1 { get; }
        private IBattleViewer Viewer2 { get; }

        public Battle(IBattleViewer viewer1, IBattleViewer viewer2, Hero first, Hero second)
        {
            Viewer1 = viewer1;
            Viewer2 = viewer2;
            First = first;
            Second = second;
        }
        public void StartBattle()
        {
            First.StartBattle();
            Second.StartBattle();
            Viewer1.OnBattleStart(new(
                First.Stats.GetStat(Stat.Health), 
                Second.Stats.GetStat(Stat.Health))
            );
            Viewer2.OnBattleStart(new(
                Second.Stats.GetStat(Stat.Health), 
                First.Stats.GetStat(Stat.Health))
            );

            var finished = false;
            for(var it = 0; it < maxBattleIt && !finished; it++)
            {
                var att1 = First.MakeAttack();
                var att2 = Second.MakeAttack();
                var rec1 = First.ReceiveAttack(att2);
                var rec2 = Second.ReceiveAttack(att1);
                
                Viewer1.OnAttack(new(First.CurrentHp, Second.CurrentHp, rec1.Sum(), rec2.Sum()));
                Viewer2.OnAttack(new(Second.CurrentHp, First.CurrentHp, rec2.Sum(), rec1.Sum()));

                var dead1 = First.CheckDead();
                var dead2 = Second.CheckDead();

                finished = dead1 || dead2;
                if (finished)
                {
                    int winner = 0;
                    if (dead2 && !dead1) winner = 1;
                    if (dead1 && !dead2) winner = -1;
                    
                    First.EndBattle(!dead1);
                    Second.EndBattle(!dead2);
                    Viewer1.OnBattleEnd(new BattleEnd(it+1, winner, First.TotalWins, First.EnemyWins));
                    Viewer2.OnBattleEnd(new BattleEnd(it+1, -winner, Second.TotalWins, Second.EnemyWins));
                }

                if (it == maxBattleIt - 1) {
                    First.EndBattle(false);
                    Second.EndBattle(false);
                    Viewer1.OnBattleEnd(new BattleEnd(it+1, 0, First.TotalWins, First.EnemyWins));
                    Viewer2.OnBattleEnd(new BattleEnd(it+1, 0,Second.TotalWins, Second.EnemyWins));
                }
                
                Viewer1.OnHeal(new(First.CurrentHp, Second.CurrentHp,
                    !finished ? First.TryRegen() : 0, !finished ? Second.TryRegen() : 0));
                
                Viewer2.OnHeal(new(Second.CurrentHp, First.CurrentHp,
                    !finished ? Second.TryRegen() : 0, !finished ? First.TryRegen() : 0));
            }
        }
        
    }
}