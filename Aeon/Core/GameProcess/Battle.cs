using System.Linq.Expressions;
using Aeon.Core.Heroes;

namespace Aeon.Core.GameProcess
{
    public class Battle
    {
        private const int MaxBattleIt = 100;
        private Hero First { get;}
        private Hero Second { get; }

        private IBattleViewer? Viewer1 { get; }
        private IBattleViewer? Viewer2 { get; }

        public Battle(IBattleViewer? viewer1, IBattleViewer? viewer2, Hero first, Hero second)
        {
            Viewer1 = viewer1;
            Viewer2 = viewer2;
            First = first;
            Second = second;
        }
        public int StartBattle()
        {
            First.StartBattle(Second);
            Second.StartBattle(First);
            Viewer1?.OnBattleStart(new(
                First.Stats.GetStat(Stat.Health), 
                Second.Stats.GetStat(Stat.Health))
            );
            Viewer2?.OnBattleStart(new(
                Second.Stats.GetStat(Stat.Health), 
                First.Stats.GetStat(Stat.Health))
            );

            var winner = 0;
            var it = 0;
            for(; it < MaxBattleIt; it++)
            {
                var att1 = First.MakeAttack();
                var att2 = Second.MakeAttack();
                var rec1 = First.ReceiveAttack(att2);
                var rec2 = Second.ReceiveAttack(att1);
                
                Viewer1?.OnAttack(new(First.CurrentHp, Second.CurrentHp, rec1.Sum(), rec2.Sum()));
                Viewer2?.OnAttack(new(Second.CurrentHp, First.CurrentHp, rec2.Sum(), rec1.Sum()));

                var dead1 = First.CheckDead();
                var dead2 = Second.CheckDead();
                if (dead1 || dead2)
                {
                    if (dead2 && !dead1) winner = 1;
                    if (dead1 && !dead2) winner = -1;
                    Viewer1?.OnHeal(new(First.CurrentHp, Second.CurrentHp, 0, 0));
                    Viewer1?.OnHeal(new(Second.CurrentHp, First.CurrentHp, 0, 0));
                    break;
                }

                var regen1 = First.TryRegen();
                var regen2 = Second.TryRegen();

                Viewer1?.OnHeal(new(First.CurrentHp, Second.CurrentHp, regen1, regen2));
                Viewer2?.OnHeal(new(Second.CurrentHp, First.CurrentHp, regen2, regen1));
            }
            
            First.EndBattle(winner == 1);
            Second.EndBattle(winner == -1);
            Viewer1?.OnBattleEnd(new BattleEnd(it+1, winner, First.TotalWins, First.EnemyWins));
            Viewer2?.OnBattleEnd(new BattleEnd(it+1, -winner, Second.TotalWins, Second.EnemyWins));
            return winner;
        }
        
    }
}