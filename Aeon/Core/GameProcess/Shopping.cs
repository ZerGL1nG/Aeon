using System;
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

        public void StartShopping(int round)
        {
            Viewer.Update(Customer);
            var com = Agent.ShopDecision();


            var t = 0;
            while (!com.Exit)
            {
                t++;
                if (BotMode && t > 30)
                { 
                    Console.WriteLine("Кто-то обосрался");
                    Customer.AutoLose = true;
                    break;
                }
                
                if (round == 0 && com.Opt) break;

                if (com.Ability) {
                    if (!Customer.UseAbility()) break;
                }
                else if (!Customer.TryToBuy(com.Type, com.Opt) && BotMode) break;
                Viewer.Update(Customer);
                com = Agent.ShopDecision();
            }
        }
        
        
    }
}