namespace Aeon.Core.Heroes
{
    /// <summary>
    /// Герой средней и поздней стадии игры. Способность Вампира
    /// улучшаемая. Изначальный уровень способности 1. В
    /// Магазине за 100 единиц игровой валюты способность можно
    /// улучшить до 2-го, а затем и до 3-го уровней. Перед каждым
    /// ударом Вампир получает 1 заряд крови. Если количество
    /// зарядов крови достигает 3 / 2 / 1, то его количество зарядов
    /// крови уменьшается на это число, Вампир восстанавливает
    /// текущее здоровье и увеличивает свой урон на 20% / 25% /
    /// 30% от его урона. Соответственно уровням 1 / 2 / 3.
    /// Если здоровье героя ниже ноля, а способность поднимает
    /// его выше ноля, герой не умер.
    /// </summary>
    public class Vampire : Hero
    {
        private const double UpgradeCost = 100;
        private readonly double[] Suck = {0.20, 0.25, 0.30};
        private readonly int[] ChargeReq = {3, 2, 1};
        private const int MaxLevel = 3;

        private int AbilityLevel = 1;
        private int CurrentCharge = 0;

        public Vampire()
        {
            HeroClass = HeroClasses.Vampire;
        }
        
        public override bool UseAbility()
        {
            if (AbilityLevel == MaxLevel || Stats.GetStat(Stat.Money) < UpgradeCost) return false;
            Stats.AddStat(Stat.Money, -UpgradeCost);
            ++AbilityLevel;
            return true;
        }

        public override double GetAbilityState() => (double) AbilityLevel;

        public override Attack MakeAttack()
        {
            ++CurrentCharge;
            var att = base.MakeAttack();
            if (CurrentCharge < ChargeReq[AbilityLevel-1]) return att;
            CurrentCharge = 0;
            Heal(Suck[AbilityLevel-1] * att.Damage);
            return new Attack(att.Source, 
                att.Damage * (1 + Suck[AbilityLevel-1]),
                att.Magic,
                att.True,
                att.Critical);
        }
    }
}