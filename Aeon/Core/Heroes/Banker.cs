namespace Aeon.Core.Heroes
{
    /// <summary>
    /// Герой предназначен для поздней игры. После каждой
    /// оптовой покупки в Магазине стоимость всех его оптовых
    /// улучшений уменьшается на 1, а после каждого 3-го оптового
    /// улучшения — еще на 1. Суммарное уменьшение стоимости
    /// может достигать 50.
    /// </summary>
    public class Banker : Hero
    {
        private const double discount = 1;
        private const double bonusDiscount = 1;
        private const int bonusQuent = 3;
        private const double maxDiscount = 50;
        
        private double totalDiscount = 0;
        private int totalTicks = 0;

        public Banker()
        {
            
        }

        protected override bool TryToBuy(Stat stat, bool opt)
        {
            if (!base.TryToBuy(stat, opt)) return false;
            if (!opt || !(totalDiscount < maxDiscount)) return true;
            ++totalTicks;
            var dis = totalTicks % bonusQuent == 0 ? bonusDiscount + discount : discount;
            totalDiscount += dis;
            foreach (var costs in Shop.Costs.Values) {
                costs.discount.AddCost(-dis);
            }
            return true;
        }
    }
}