using System.Collections.Generic;
using System.Linq;

namespace Aeon.Core.GameProcess
{
   
    public enum StateParameter
    {
        MaxHp,
        CurHp,
        RecDmg,
        Regen,
    }
    
    
    public class BattleState
    {
        public Dictionary<StateParameter, double> EnemyParams;
        public Dictionary<StateParameter, double> MyParams;


        public BattleState()
        {
            EnemyParams = new Dictionary<StateParameter, double>();
            MyParams = new Dictionary<StateParameter, double>();
        }
        
        public BattleState Reverse() => new BattleState(MyParams, EnemyParams);

        public BattleState(Dictionary<StateParameter, double> myParams,
            Dictionary<StateParameter,double> enemyParams)
        {
            MyParams = myParams;
            EnemyParams = enemyParams;
        }
        
        public static BattleState GetEmptyState() => new BattleState(
            new Dictionary<StateParameter, double> {
                {StateParameter.MaxHp, 0},
                {StateParameter.CurHp, 0},
                {StateParameter.Regen, 0},
                {StateParameter.RecDmg, 0}
            }, 
            new Dictionary<StateParameter, double>{
                {StateParameter.MaxHp, 0},
                {StateParameter.CurHp, 0},
                {StateParameter.Regen, 0},
                {StateParameter.RecDmg, 0}
            }
        );
    }
}