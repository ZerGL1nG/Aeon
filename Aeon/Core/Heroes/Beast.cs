using System;

namespace Aeon.Core.Heroes;

/// <summary>
///     Герой ранней и средней стадии игры. Урон Зверя
///     увеличивается на 3.9% за каждые недостающие 10%
///     здоровья. (текущего от максимального)
/// </summary>
public class Beast: Hero
{
    private const double HealthThreshold = 0.1;
    private const double DamageBonusMultiplier = 0.039; // +1

    public Beast() => HeroClass = HeroClasses.Beast;

    public override Attack MakeAttack()
    {
        var healthLost = 1-CurrentHp/Stats.GetStat(Stat.Health);
        return base.MakeAttack().Scale(1+DamageBonusMultiplier*Math.Floor(healthLost/HealthThreshold));
    }
}