namespace Aeon.Core.Heroes;

/// <summary>
///     Герой ранней и средней стадий игры. Перед каждым ударом
///     увеличивает количество своей игровой валюты на 1.1.
/// </summary>
public class Beggar: Hero
{
    private const double MoneyBonus = 1.1;
    private double MoneyBegged;

    public Beggar() => HeroClass = HeroClasses.Beggar;

    public override Attack MakeAttack()
    {
        Stats.AddStat(Stat.Money, MoneyBonus);
        MoneyBegged += MoneyBonus;
        return base.MakeAttack();
    }

    public override double GetAbilityState() => MoneyBegged;
}