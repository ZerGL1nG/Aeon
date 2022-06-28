using System.Collections.Generic;

namespace Aeon.Core.GameProcess;

public enum StateParameter { MaxHp, CurHp, RecDmg, Regen, }

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
        MyParams    = new Dictionary<StateParameter, double>();
    }

    public BattleState(Dictionary<StateParameter, double> myParams, Dictionary<StateParameter, double> enemyParams)
    {
        MyParams    = myParams;
        EnemyParams = enemyParams;
    }

    public BattleState Reverse() =>
        new(new Dictionary<StateParameter, double>(EnemyParams), new Dictionary<StateParameter, double>(MyParams));

    public BattleState Copy() =>
        new(new Dictionary<StateParameter, double>(MyParams), new Dictionary<StateParameter, double>(EnemyParams));
}