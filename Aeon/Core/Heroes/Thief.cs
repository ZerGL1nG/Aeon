namespace Aeon.Core.Heroes;

/// <summary>
///     Герой ранней и средней стадий игры. После каждого удара
///     максимальное здоровье Вора увеличивается, а здоровье
///     противника уменьшается на 1+w, где w — количество побед
///     Вора.
/// </summary>
public class Thief: Hero
{
    private double HealthSteal = 1;
    private double Stolen;

    public Thief() => HeroClass = HeroClasses.Thief;

    public override Attack MakeAttack()
    {
        Stats.AddStat(Stat.Health, HealthSteal);
        Enemy.Stats.AddStat(Stat.Health, -HealthSteal);
        Stolen += HealthSteal;
        return base.MakeAttack();
    }

    public override void EndBattle(bool win)
    {
        if (win) ++HealthSteal;
        base.EndBattle(win);
    }

    public override double GetAbilityState() => Stolen;
}