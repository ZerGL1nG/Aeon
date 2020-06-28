namespace Aeon.Core.Heroes
{
    /// <summary>
    /// Герой средней и поздней стадий игры. Его критический урон
    /// увеличен на 50% от магии, а критический шанс увеличен на
    /// 10% от магии.
    /// </summary>
    public class Warrior : Hero
    {
        private const double CritDamageBonus = 0.005;
        private const double CritChanceBonus = 0.001;

        public Warrior()
        {
            Stats.AddStat(Stat.CritChance, CritChanceBonus * Stats.GetStat(Stat.Spell));
            Stats.AddStat(Stat.CritDamage, CritDamageBonus * Stats.GetStat(Stat.Spell));
        }

        protected override bool TryToBuy(Stat stat, bool opt)
        {
            if (!base.TryToBuy(stat, opt)) return false;
            if (stat != Stat.Spell) return true;
            var bought = _shop.GetPrice(stat, opt).amount;
            Stats.AddStat(Stat.CritChance, CritChanceBonus * bought);
            Stats.AddStat(Stat.CritDamage, CritDamageBonus * bought);
            return true;
        }
    }
}