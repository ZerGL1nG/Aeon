namespace Aeon.Core.Heroes
{
    /// <summary>
    /// Герой поздней стадии игры. У него есть характеристика —
    /// вампиризм. Изначально она равна 15%. После каждого Боя
    /// вампиризм увеличивается на 0.6%. После каждого удара
    /// Повелитель восстанавливает здоровье на 𝑑𝐻 = 𝑑 × 𝑐𝑉,
    /// где dH — восстанавливаемое здоровье, d — урон, cV —
    /// вампиризм.
    /// </summary>
    public class Master : Hero
    {
        private const double LifestealForBattle = 0.006;

        private double Lifesteal = 0.15;

        public override Attack MakeAttack()
        {
            var att = base.MakeAttack();
            Heal(att.Damage * Lifesteal);
            return att;
        }

        public override void EndBattle(bool win)
        {
            Lifesteal += LifestealForBattle;
            base.EndBattle(win);
        }
    }
}