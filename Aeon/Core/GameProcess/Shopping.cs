﻿using System;
using Aeon.Agents;
using Aeon.Core.Heroes;

namespace Aeon.Core.GameProcess
{
    public class Shopping
    {
        public Hero Customer { get; set; }
        public IShopViewer Viewer { get; set; }
        public IAgent Agent { get; set; }
        public bool BotMode { get; set; }

        public Shopping(Hero hero, IShopViewer viewer, IAgent agent, bool bot)
        {
            Customer = hero;
            Viewer= viewer;
            Agent = agent;
            BotMode = bot;
        }

        public void StartShopping()
        {
            Viewer.OnShopUpdate(Customer);
            var com = Agent.ShopDecision();


            var t = 0;
            while (!com.Exit)
            {
                t++;
                if (BotMode && t > 30)
                { 
                    if (Program.debugOutput) Console.WriteLine("Кто-то обосрался");
                    Customer.AutoLose = true;
                    break;
                }
                
                if (Customer.RoundNumber == 0 && com.Opt) break;

                if (com.Ability) {
                    if (!Customer.UseAbility()) break;
                }
                else if (!Customer.TryToBuy(com.Type, com.Opt) && BotMode) break;
                Viewer.OnShopUpdate(Customer);
                com = Agent.ShopDecision();
            }
        }
        
        
    }
}