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
        
        public BattleState Reverse() => new BattleState(
            new Dictionary<StateParameter, double>(EnemyParams), new Dictionary<StateParameter, double>(MyParams));
        public BattleState Copy() => new BattleState(
            new Dictionary<StateParameter, double>(MyParams), new Dictionary<StateParameter, double>(EnemyParams));

        public BattleState(Dictionary<StateParameter, double> myParams,
            Dictionary<StateParameter,double> enemyParams)
        {
            MyParams = myParams;
            EnemyParams = enemyParams;
        }
    }
}