namespace Aeon.Core.Heroes
{
    /// <summary>
    /// Герой ранней и средней стадий игры. Перед каждым ударом
    /// увеличивает количество своей игровой валюты на 1.1.
    /// </summary>
    public class Hobo : Hero
    {
        private const double MoneyBonus = 1.1;

        public override Attack MakeAttack()
        {
            Stats.AddStat(Stat.Money, MoneyBonus);
            return base.MakeAttack();
        }
    }
}