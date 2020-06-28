using System;

namespace Aeon.Core.Heroes
{
    /// <summary>
    /// Герой поздней стадии игры. Начальное здоровье уменьшено
    /// вдвое, а начальный урон вдвое увеличен. В начале каждого
    /// Боя его текущий прирост становится равен
    /// 𝑐𝐼 = (1 + 𝐼) ^ (2 + [0.1(𝑖 − 1)]),
    /// где cI — текущий прирост, I — разовый прирост, i — номер
    /// текущего Боя, [a] — целая часть числа a.
    /// Иначе говоря, в начале боя его прирост срабатывает 2 раза
    /// + 1 раз за каждые 10 боев.
    /// </summary>
    public class Fe11 : Hero
    {
        private const double baseBonus = 2;
        private const double addBonus = 0.1;

        private int battles = 0;
        private int BonusIncome => (int) Math.Floor(baseBonus + battles * addBonus);

        public override void StartBattle()
        {
            ++battles;
            base.StartBattle();
            CurrentIncome = Math.Pow(1 + Stats.GetStat(Stat.Income), BonusIncome);
        }
    }
}