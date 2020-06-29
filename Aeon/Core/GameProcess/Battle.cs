using System.Linq.Expressions;
using Aeon.Core.Heroes;

namespace Aeon.Core.GameProcess
{
    public class Battle
    {
        private const int maxBattleIt = 100;
        private Hero First { get;}
        private Hero Second { get; }

        private BattleViewer Viewer { get; }

        public Battle(BattleViewer viewer, Hero first, Hero second)
        {
            Viewer = viewer;
            First = first;
            Second = second;
        }
        public void StartBattle()
        {
            Viewer.Reset();
            Second.StartBattle();
            
            var state = new BattleState();
            state.MyParams[StateParameter.MaxHp] = First.Stats.GetStat(Stat.Health);
            state.EnemyParams[StateParameter.MaxHp] = Second.Stats.GetStat(Stat.Health);

            var finished = false;
            for(var it = 0; it < maxBattleIt && !finished; it++)
            {

                state.MyParams[StateParameter.CurHp] = First.Stats.GetStat(Stat.Health);
                state.MyParams[StateParameter.CurHp] = Second.Stats.GetStat(Stat.Health);
    
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
                
                state.MyParams[StateParameter.Regen]    = finished ? First.TryRegen() : 0;
                state.EnemyParams[StateParameter.Regen] = finished ? Second.TryRegen() : 0;
                Viewer.Update(state);
                
            }
        }
        
    }
}