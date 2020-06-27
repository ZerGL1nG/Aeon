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
            _shop.Costs[Stat.Health].MulAmount(FatMultiplier);
        }

        public override void EndBattle(bool win)
        {
            base.EndBattle(win);
            _stats.AddStat(Stat.Regen, BonusRegen);
        }
    }
}