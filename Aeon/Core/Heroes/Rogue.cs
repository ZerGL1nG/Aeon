using System;
using System.ComponentModel;

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

        private double currCoeff;

        public Rogue()
        {
            HeroClass = HeroClasses.Rogue;
        }
        
        public override void Init(Hero enemy)
        {
            base.Init(enemy);
            currCoeff = 1;
        }

        public override Attack MakeAttack()
        {
            CurrentHp -= (CurrentHp * RogueDamage) / currCoeff;
            Enemy.CurrentHp -= (CurrentHp * EnemyDamage) * currCoeff;
            return base.MakeAttack();
        }

        public override void EndBattle(bool win)
        {
            base.EndBattle(win);
            currCoeff *= Exp;
        }

        public override double GetAbilityState() => (currCoeff-1) * 100;
    }
}