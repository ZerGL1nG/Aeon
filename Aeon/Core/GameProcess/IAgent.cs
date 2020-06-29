using System.Collections.Generic;

namespace Aeon.Core.GameProcess
{
    public interface IAgent
    {
        public BattleViewer BattleView { get; set; }
        public ShopViewer ShopView { get; set; }
        
        public abstract Command ShopDecision();
    }
}