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
    ///
    /// todo: пока что считается первый удар за бонус, потом сделать нормально
    /// 
    public class Cheater : Hero
    {
        private const double DamageMultiplier = 0.93;
        private const double FirstHitBonus = 2;
        private bool first = true;

        public Cheater()
        {
            /*/
            _shop.Costs[Stat.Attack].MulAmount(DamageMultiplier);
            _shop.Costs[Stat.Spell].MulAmount(DamageMultiplier);
            _stats.MulStat(Stat.Attack, DamageMultiplier);
            _stats.MulStat(Stat.Spell, DamageMultiplier);
            /*/
        }

        public override void StartBattle()
        {
            base.StartBattle();
            first = true;
        }

        public override Attack MakeAttack()
        {
            if (!first) return base.MakeAttack().Scale(DamageMultiplier * FirstHitBonus);
            first = false;
            return base.MakeAttack().Scale(DamageMultiplier);
        }
    }
}