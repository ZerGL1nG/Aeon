using System;
using System.Collections.Generic;

namespace Aeon.Core.Heroes
{
    /// <summary>
    /// Универсальный герой. Обладает дополнительной
    /// характеристикой — маной. Также имеет 4 режима своей
    /// способности:
    ///  1. Следующий удар уменьшает количество игровой валюты
    ///     героя противника на 2 (стоит 3 маны)
    ///  2. На время следующего удара магия Кровавого Эльфа
    ///     увеличивается на 30% (стоит 4 маны)
    ///  3. После следующего удара Кровавый Эльф восстанавливает
    ///     20% недостающего текущего здоровья (стоит 5 маны)
    ///  4. Мана не тратится
    /// Перед каждой атакой количество маны увеличивается на 1,
    /// и, если маны хватает на применение текущей способности —
    /// ее стоимость вычитается из маны, и эта способность
    /// применяется. Режим способности можно свободно менять в
    /// Магазине.
    /// </summary>
    public class BloodyElf : Hero
    {
        private enum Abilities
        {
            Nothing,
            MoneyBurn,
            SuperMag,
            UberRegen
        }

        private Abilities CurrentAbility = Abilities.Nothing;
        private int CurrentMana = 0;
        
        private Dictionary<Abilities, int> AbilityManaCost = new Dictionary<Abilities, int> {
            {Abilities.Nothing,   0},
            {Abilities.MoneyBurn, 3},
            {Abilities.SuperMag,  4},
            {Abilities.UberRegen, 5}
        };

        private const double SuperMagMultiplier = 1.3;
        private const double BurnMoney = 2;
        private const double UberRegenCoeff = 0.2;

        private bool regenFlag = false;

        public BloodyElf()
        {
            HeroClass = HeroClasses.BloodyElf;
        }
        
        public override bool UseAbility()
        {
             CurrentAbility = (Abilities)((int)(1 + CurrentAbility) % 4);
             return true;
        }
        public override double GetAbilityState() => (double) CurrentAbility;

        public override Attack MakeAttack()
        {
            var att = base.MakeAttack();
            ++CurrentMana;
            if (AbilityManaCost[CurrentAbility] > CurrentMana) return att;
            switch (CurrentAbility) {
                case Abilities.Nothing:
                    return att;
                case Abilities.MoneyBurn:
                    Enemy.Stats.AddStat(Stat.Money, -BurnMoney);
                    return att;
                case Abilities.SuperMag:
                    return new Attack(att.Source, att.Damage, att.Magic * SuperMagMultiplier, att.True, att.Critical);
                case Abilities.UberRegen:
                    regenFlag = true;
                    return att;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override double TryRegen()
        {
            if (!regenFlag) return base.TryRegen();
            regenFlag = false;
            return base.TryRegen() + Heal((Stats.GetStat(Stat.Health) - CurrentHp) * UberRegenCoeff);
        }
    }
}