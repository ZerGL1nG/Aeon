using System.Collections.Concurrent;
using System.Collections.Generic;
using Aeon.Core;
using Aeon.Core.Heroes;

namespace Aeon.Builds;

internal class SearchNode
{
    public static readonly List<Stat> TestingStats = new() {
        Stat.Health,
        Stat.Attack,
        Stat.Spell,
        Stat.Regen,
        Stat.Armor, //Stat.Income,
        //Stat.Shield, Stat.CritChance, Stat.CritDamage,
    };

    private static readonly Dictionary<string, SearchResult> _nodes = new();
    //{ Stat.Health, Stat.Attack, Stat.Spell };

    public readonly SearchNode ParentNode;

    private readonly Hero _hero;

    public ConcurrentDictionary<string, NodeData> matchups = new();

    private SearchNode(Hero hero, string hc, int depth, SearchNode parent)
    {
        Depth      = depth;
        _hero      = hero;
        HeroClass  = hc;
        ParentNode = parent;
    }

    public int Depth { get; }
    public Hero HeroCopy => _hero.Clone();
    public string HeroClass { get; init; }

    public Stats Stats => _hero.Stats;
    public Shop Shop => _hero.Shop;
    public double MoneyHere => _hero.Stats.GetStat(Stat.Money);

    internal IEnumerable<SearchNode> Init()
    {
        foreach (var stat in TestingStats)
            if (_hero.CanBuy(stat, false)) {
                var h = _hero.Clone();
                h.TryToBuy(stat, false);

                yield return new SearchNode(h, HeroClass, Depth+1, this);
            }
    }

    public void Played(string enemy, int result)
    {
        if (!matchups.ContainsKey(enemy)) matchups[enemy] = new NodeData();
        matchups[enemy].Played(result);
    }

    public static void AddHeroClass(Hero hero, string hc)
    {
        var heroinit = hero.Clone();
        var initnode = new SearchNode(heroinit, hc, 0, null);
        _nodes[hc] = new SearchResult(hc, initnode);
        //_nodes[hc].Add(heroinit.Stats, initnode);
        //initnode.Init();
    }

    public static SearchResult GetNodes(string hc) => _nodes[hc];

    public static void ResetNodes(string s)
    {
        _nodes[s].Reset();
    }
}