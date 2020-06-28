using System;

namespace Aeon.Core.Heroes
{
    /// <summary>
    /// Универсальный герой. Перед каждым ударом
    /// оба героя теряют часть здоровья. Эта часть зависит от
    /// номера текущего Боя.
    /// Разбойник теряет 0.09ℎ𝑝 / 1.02^𝑖−1 текущего здоровья,
    /// где hp — текущее здоровье Разбойника, i — номер Боя.
    /// Герой противника теряет 0.11𝐸ℎ𝑝 × 1.02^𝑖−1 текущего
    /// здоровья, где Ehp — текущее здоровье Героя противника, i
    /// — номер Боя.
    /// </summary>
    public class Rogue : Hero
    {
        private const double RogueDamage = 0.09;
        private const double EnemyDamage = 0.11;
        private const double Exp = 1.02;

        private int BattleNum = 0;

        public override void StartBattle()
        {
            ++BattleNum;
            base.StartBattle();
        }

        public override Attack MakeAttack()
        {
            CurrentHp -= (CurrentHp * RogueDamage) / Math.Pow(Exp, BattleNum - 1);
            Enemy.CurrentHp -= (CurrentHp * EnemyDamage) * Math.Pow(Exp, BattleNum - 1);
            return base.MakeAttack();
        }
    }
}