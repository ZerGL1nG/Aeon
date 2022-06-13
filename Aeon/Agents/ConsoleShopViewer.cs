using System;
using System.Collections.Generic;
using System.Linq;
using Aeon.Core;
using Aeon.Core.GameProcess;
using Aeon.Core.Heroes;
using static Aeon.Agents.KeysMap;

namespace Aeon.Agents;

internal class ConsoleShopViewer : IShopViewer
{
   
    public void OnShopUpdate(Hero customer)
    {
        Console.Clear();
        Console.WriteLine();
        
        Console.WriteLine($"{customer.HeroClass, 25} {customer.TotalWins, 2} : {customer.EnemyWins, -2} {customer.EnemyId}");
        Console.WriteLine();
        
        var stats = customer.Stats;
        var shop = customer.Shop;
        
        for (var t = 1; t <= 9; ++t) 
        {
            var stat = (Stat) t;
            var price = customer.Shop.Costs[stat];
            Console.WriteLine($"{stat, 10}: {Stats.RoundStat(stats.GetStat(stat), stat), 5} " +
                              $"|| Buy[{GetKey(t-1)}]: {shop.RoundBonus(stats.GetStat(stat), stat, false), 2} for {price.standard.cost, -4}" +
                              $"|  Opt[{GetKey(t+8)}]: {shop.RoundBonus(stats.GetStat(stat), stat,  true), 3} for {price.discount.cost, -4}");
        }
        Console.WriteLine();
        Console.WriteLine($"       {customer.HeroClass, -15} Money: {stats.GetStat(Stat.Money), -10} Ability[{GetKey(^1)}]: {customer.GetAbilityState()}");
        Console.WriteLine();
        
    }

}

public static class KeysMap
{
    static KeysMap() =>
        _keysDict = _keysList.Select((k, r) => (k, r)).ToDictionary(tuple => tuple.k, tuple => tuple.r);
    
    private static readonly Dictionary<ConsoleKey, int> _keysDict;
    private static readonly List<ConsoleKey> _keysList = new List<ConsoleKey> {
        ConsoleKey.Q, 
        ConsoleKey.W, 
        ConsoleKey.E, 
        ConsoleKey.R, 
        ConsoleKey.T, 
        ConsoleKey.Y,
        ConsoleKey.U, 
        ConsoleKey.I, 
        ConsoleKey.O, 
        ConsoleKey.A,
        ConsoleKey.S, 
        ConsoleKey.D, 
        ConsoleKey.F, 
        ConsoleKey.G,
        ConsoleKey.H,
        ConsoleKey.J, 
        ConsoleKey.K, 
        ConsoleKey.L, 
        ConsoleKey.Enter, 
        ConsoleKey.Spacebar,
    };

    public static ConsoleKey GetKey(Index index) => _keysList[index];

    public static int? GetIndex(ConsoleKey key) => !_keysDict.TryGetValue(key, out int index) ? null : index;
}