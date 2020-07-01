namespace Aeon.Core.Heroes
{
    /// <summary>
    /// Герой ранней стадии игры. Его атака уменьшена на 7%.
    /// Независимо от того, сколько улучшений атаки он
    /// приобретает — у него будет на 7% меньше урона, чем у
    /// пустого героя с аналогичными улучшениями. Во время боя,
    /// если текущее здоровье Героя противника равно
    /// максимальному, то урон Читера умножается на 2.
    /// </summary>
    public class Cheater : Hero
    {
        private const double DamageMultiplier = 0.93;
        private const double FirstHitBonus = 2;
        private bool first = true;

        public Cheater()
        {
            HeroClass = HeroClasses.Cheater;
            
            Shop.Costs[Stat.Attack].MulAmount(DamageMultiplier);
            Stats.MulStat(Stat.Attack, DamageMultiplier);
        }

        public override Attack MakeAttack()
        {
            return (int)(Enemy.Stats.GetStat(Stat.Health)) <= Enemy.CurrentHp
                ? base.MakeAttack().Scale(FirstHitBonus)
                : base.MakeAttack();
        }
    }
}