using System.Linq.Expressions;
using Aeon.Core.Heroes;

namespace Aeon.Core.GameProcess
{
    public class Battle
    {
        private const int maxBattleIt = 15;
        private Hero First { get;}
        private Hero Second { get; }

        private BattleViewer Viewer1 { get; }
        private BattleViewer Viewer2 { get; }

        public Battle(BattleViewer viewer1, BattleViewer viewer2, Hero first, Hero second)
        {
            Viewer1 = viewer1;
            Viewer2 = viewer2;
            First = first;
            Second = second;
        }
        public void StartBattle()
        {
            Viewer1.Reset();
            Viewer2.Reset();
            First.StartBattle();
            Second.StartBattle();
            
            var state = new BattleState();
            state.MyParams[StateParameter.MaxHp] = First.Stats.GetStat(Stat.Health);
            state.EnemyParams[StateParameter.MaxHp] = Second.Stats.GetStat(Stat.Health);

            var finished = false;
            for(var it = 0; it < maxBattleIt && !finished; it++)
            {

                state.MyParams[StateParameter.CurHp] = First.CurrentHp;
                state.EnemyParams[StateParameter.CurHp] = Second.CurrentHp;
    
                var att1 = First.MakeAttack();
                var att2 = Second.MakeAttack();
                var rec1 = First.ReceiveAttack(att2);
                var rec2 = Second.ReceiveAttack(att1);

                state.MyParams[StateParameter.RecDmg] = rec1.Sum();
                state.EnemyParams[StateParameter.RecDmg] = rec2.Sum();
                
                var dead1 = First.CheckDead();
                var dead2 = Second.CheckDead();

                finished = dead1 || dead2;
                if (finished)
                {
                    First.EndBattle(!dead1);
                    Second.EndBattle(!dead2);
                }

                if (it == maxBattleIt - 1) {
                    First.EndBattle(false);
                    Second.EndBattle(false);
                }
                
                state.MyParams[StateParameter.Regen]    = finished ? First.TryRegen() : 0;
                state.EnemyParams[StateParameter.Regen] = finished ? Second.TryRegen() : 0;
                Viewer1.Update(state.Copy());
                Viewer2.Update(state.Reverse());
                
            }
        }
        
    }
}