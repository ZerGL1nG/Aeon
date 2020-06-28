using Aeon.Core.Heroes;

namespace Aeon.Core.GameProcess
{
    public class Shopping
    {
        public Hero Customer { get; set; }
        public ShopViewer Viewer { get; set; }
        public Agent Agent { get; set; }

        public Shopping(Hero hero, ShopViewer viewer, Agent agent)
        {
            Customer = hero;
            Viewer= viewer;
            Agent = agent;
        }

        public void StartShopping()
        {
            Viewer.Update(Customer);
            var com = Agent.ShopDecision();
            while (!com.Exit)
            {
                if (com.Ability)
                    Customer.UseAbility();
                else if (!Customer.TryToBuy(com.Type, com.Opt)) break;
                Viewer.Update(Customer);
                com = Agent.ShopDecision();
            }
        }
        
        
    }
}