using System;

namespace Aeon.Core.Heroes
{
    /// <summary>
    /// Герой поздней стадии игры. 15% его урона напрямую
    /// вычитаются из текущего здоровья Героя оппонента (не
    /// подвержены уменьшению разными способностями и
    /// характеристиками). Также у Убийцы есть свой счетчик
    /// нанесенного урона. Когда этот счетчик превышает новое
    /// целевое значение — Убийца получает +10 к атаке. Целевые
    /// значения считаются по формуле 𝐾𝑖 = 75 × 𝑖 × (𝑖 + 1), где
    /// Ki — i-тое целевое значение, i — номер целевого значения.
    /// </summary>
    public class Killer : Hero
    {
        private const double TrueDamageCoeff = 0.15;
        private const double DamageBonus = 10;

        private double ReachedGoals = 0;
        private double TotalDamage = 0;
        private double GetGoal => 75 * (ReachedGoals + 1) * (ReachedGoals + 2);

        public Killer()
        {
            HeroClass = HeroClasses.Killer;
        }
        
        public override Attack MakeAttack()
        {
            var att = base.MakeAttack();
            TotalDamage += att.Damage;
            
            att = new Attack(att.Source, 
                att.Damage * (1 - TrueDamageCoeff), 
                att.Magic, 
                att.True + att.Damage * TrueDamageCoeff, 
                att.Critical);
            
            while (TotalDamage >= GetGoal) {
                ++ReachedGoals;
                Stats.AddStat(Stat.Attack, DamageBonus);
            }

            return att;
        }

        public override double GetAbilityState() => ReachedGoals * DamageBonus;
    }
}