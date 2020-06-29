namespace Aeon.Core.Heroes
{
    /// <summary>
    /// Герой ранней и средней стадий игры. Его улучшения дают
    /// на 9% больше здоровья. После каждого Боя он получает +2
    /// регенерации.
    /// </summary>
    public class Fatty : Hero
    {
        private const double FatMultiplier = 1.09;
        private const double BonusRegen = 2;
        
        public Fatty()
        {
            HeroClass = HeroClasses.Fatty;
            
            Shop.Costs[Stat.Health].MulAmount(FatMultiplier);
        }

        public override void EndBattle(bool win)
        {
            base.EndBattle(win);
            Stats.AddStat(Stat.Regen, BonusRegen);
        }
    }
}