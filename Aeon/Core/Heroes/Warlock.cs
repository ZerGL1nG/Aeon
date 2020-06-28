namespace Aeon.Core.Heroes
{
    /// <summary>
    /// Универсальный герой. В течение каждого из посещений
    /// Магазина игрок может единожды использовать способность
    /// Чернокнижника. Использование стоит 10 единиц игровой
    /// валюты. Способность увеличивает игровую валюту на 17
    /// единиц после следующего Боя.
    /// </summary>
    public class Warlock : Hero
    {
        private const double AbilityCost = 10;
        private const double Bonus = 17;

        private bool AbilityUsed = false;

        public override void UseAbility()
        {
            if (AbilityUsed || AbilityCost > Stats.GetStat(Stat.Money)) return;
            Stats.AddStat(Stat.Money, -AbilityCost);
            AbilityUsed = true;
        }

        public override void EndBattle(bool win)
        {
            if (AbilityUsed) {
                AbilityUsed = false;
                Stats.AddStat(Stat.Money, Bonus);
            }
            base.EndBattle(win);
        }
    }
}