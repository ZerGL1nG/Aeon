using Aeon.Core.Heroes;

namespace Aeon.Core.GameProcess
{
    public class Shopping
    {
        public Hero Customer { get; set; }
        public ShopViewer Viewer { get; set; }
        public IAgent Agent { get; set; }
        public bool BotMode { get; set; }

        public Shopping(Hero hero, ShopViewer viewer, IAgent agent, bool bot)
        {
            Customer = hero;
            Viewer= viewer;
            Agent = agent;
            BotMode = bot;
        }

        public void StartShopping()
        {
            Viewer.Update(Customer);
            var com = Agent.ShopDecision();
            while (!com.Exit)
            {
                if (com.Ability)
                    Customer.UseAbility();
                else if (!Customer.TryToBuy(com.Type, com.Opt) && BotMode) break;
                Viewer.Update(Customer);
                com = Agent.ShopDecision();
            }
        }
        
        
    }
}