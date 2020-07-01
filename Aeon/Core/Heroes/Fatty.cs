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
        private double GotBonusHP = 0;
        
        public Fatty()
        {
            HeroClass = HeroClasses.Fatty;
        }

        public override void Init(Hero enemy)
        {
            Shop.Costs[Stat.Health].MulAmount(FatMultiplier);
            base.Init(enemy);
        }

        public override bool TryToBuy(Stat stat, bool opt)
        {
            if (!base.TryToBuy(stat, opt)) return false;
            if (stat != Stat.Health) return true;
            GotBonusHP += Shop.GetPrice(Stat.Health, opt).amount - InitCosts[Stat.Health].GetAmount(opt);
            return true;
        }

        public override void EndBattle(bool win)
        {
            base.EndBattle(win);
            Stats.AddStat(Stat.Regen, BonusRegen);
        }

        public override double GetAbilityState() => GotBonusHP;
    }
}