using System;
using Aeon.Agents.Network;
using Aeon.Core.Heroes;

namespace Aeon.Core.GameProcess;

public class Shopping
{
    public Shopping(Hero hero, IShopViewer viewer, IAgent agent, bool bot)
    {
        Customer = hero;
        Viewer   = viewer;
        Agent    = agent;
        BotMode  = bot;
    }

    public Hero Customer { get; set; }
    public IShopViewer Viewer { get; set; }
    public IAgent Agent { get; set; }
    public bool BotMode { get; set; }

    public void StartShopping(bool debug)
    {
        Viewer.OnShopUpdate(Customer);
        var com = Agent.ShopDecision();

        if (false && debug && Agent is NetworkAgent a) {
            //Console.WriteLine();
            Console.WriteLine("-----------------------");
            Console.Write("BattleView:");
            for (var i = 0; i < 25; i++) Console.Write($" {a.Network.Input[i].Result}");
            Console.WriteLine();
            Console.Write("Stats:");
            for (var i = 25; i < 35; i++) Console.Write($" {a.Network.Input[i].Result}");
            Console.WriteLine();
            Console.Write("ShopView:");
            for (var i = 35; i < 75; i++) Console.Write($" {a.Network.Input[i].Result}");
            Console.WriteLine();
            Console.Write("EnemyHeroes:");
            for (var i = 75; i < 90; i++) Console.Write($" {a.Network.Input[i].Result}");
            Console.WriteLine();
            Console.WriteLine("- - - - - - - - - - - -");
        }


        var t = 0;
        while (true) {
            if (debug && Agent is NetworkAgent aa) {
                Console.Write($"Output{t,2}: {com,16}");
                for (var i = 0; i < 20; i++)
                    //Console.Write($" {aa.Network.Output[i].Result}");
                    Console.Write($" {aa.Network.Output[i].Result:+#0.00;-#0.00;+0.00}");
                Console.WriteLine();
            }

            if (com.Exit) break;

            t++;
            if (BotMode && t > 30) break;

            if (Customer.RoundNumber == 0 && com.Opt) break;

            if (com.Ability) {
                if (!Customer.UseAbility()) break;
            } else if (!Customer.TryToBuy(com.Type, com.Opt) && BotMode) break;
            Viewer.OnShopUpdate(Customer);
            com = Agent.ShopDecision();
        }

        if (debug) Console.WriteLine("- - - - - - - - - - - -");
    }
}