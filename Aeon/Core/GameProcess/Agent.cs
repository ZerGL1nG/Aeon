namespace Aeon.Core.GameProcess
{
    public class Agent
    {
        public BattleViewer BattleView { get; set; }
        public ShopViewer ShopView { get; set; }
        
        
        public Agent()
        {
            BattleView = new BattleViewer();
            ShopView = new ShopViewer();
        }
        
        public virtual Command ShopDecision()
        {
            return new Command();
        }
    }
}