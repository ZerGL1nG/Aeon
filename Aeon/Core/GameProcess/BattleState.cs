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

    public record BattleStart(double MineMaxHealth, double EnemyMaxHealth);
    public record BattleAttack(double MineHealth, double EnemyHealth, double GiveDamage, double TakeDamage);
    public record BattleHeal(double MineHealth, double EnemyHealth, double MineHeal, double EnemyHeal);
    public record BattleEnd(int TotalRounds, int Winner, int MineWins, int EnemyWins);


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