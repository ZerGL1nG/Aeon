using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aeon.Agents.Network;
using Aeon.Core.GameProcess;
using Aeon.Core.Heroes;

namespace Aeon.Builds;

public class Searcher
{
    private const string clafile = Program.Path + "Meta";
    private const string clatop = Program.Path + "MetaTop";
    private const string claiter = Program.Path + "MetaIterate";
    public const string clagen = Program.Path + "DatasetGen";

    private List<string> _hc = new();
    private Dictionary<string, Hero> _heroes = new();
    public void AddClass(Hero hero, string hc) => _heroes[hc] = hero;

    public void Init()
    {
        foreach (var hero in _heroes)
        {
            _hc.Add(hero.Key);
            SearchNode.AddHeroClass(hero.Value, hero.Key);
        }
    }

    public void BattleAll()
    {
        for (int r1 = 0; r1 < _heroes.Count; r1++)
        for (int r2 = r1; r2 < _heroes.Count; r2++)
        {
            var class1 = _hc[r1];
            var class2 = _hc[r2];

            var classtable1 = SearchNode.GetNodes(class1).Condition(n => n.MoneyHere < 20);
            var classtable2 = SearchNode.GetNodes(class2).Condition(n => n.MoneyHere < 20);

            TestEngage(class1, class2, classtable1, classtable2);
            
            SaveMatchup(class1, class2, classtable1);
            SaveMatchup(class2, class1, classtable2);
        }
    }
    
    public void MakeData(string file1, string file2)
    {
        var class1 = _hc[0];
        var class2 = _hc[0];

        var classtable1 = LoadMatchup(class1, clagen, file1);
        var classtable2 = LoadMatchup(class2, clagen, file2);
        
        DataEngage(class1, class2, classtable1, classtable2);
    }

    public void TestBest(int top = 100)
    {
        for (int r1 = 0; r1 < _heroes.Count; r1++)
        for (int r2 = r1; r2 < _heroes.Count; r2++)
        {
            var class1 = _hc[r1];
            var class2 = _hc[r2];

            var classtable1 = LoadMatchup(class1, class2, top);
            var classtable2 = LoadMatchup(class2, class1, top);
            
            TestEngage(class1, class2, classtable1, classtable2);
            
            SaveMatchup(class1, class2, classtable1, clatop);
            SaveMatchup(class2, class1, classtable2, clatop);
        }
    }
    
    // Win:92,98 %; Lose: 0,36 %; Pts:3,9189 Stats: Health: 122; Attack: 15; Spell: 28; CritChance: 0; CritDamage: 150; Income: 0; Armor: 15; Shield: 0; Regen: 1; Money: 2; 

    private void TestEngage(string class1, string class2, List<SearchNode> classtable1, List<SearchNode> classtable2)
    {
        //Console.WriteLine();
        Console.WriteLine($"Testing: {class1} vs {class2}");
        Console.WriteLine($"Table size: {classtable1.Count}x{classtable2.Count}");
            
        int st = 0;

        var w = Stopwatch.StartNew();
            
        foreach (var nodex1 in classtable1)
        {
            Parallel.ForEach(classtable2, nodex2 =>
            {
                var testx = new Battle(null, null, nodex1.HeroCopy, nodex2.HeroCopy);
                var result = testx.StartBattle();
                nodex1.Played(nodex2.HeroClass, result);
                nodex2.Played(nodex1.HeroClass, -result);
            });

            ++st;
            if (st % 10 != 0) continue;
                
            Console.SetCursorPosition(0, Console.GetCursorPosition().Top);
            Console.Write($"Step: {st,7} / {classtable1.Count,-7}");
        }
        Console.WriteLine();
        Console.WriteLine($"Finished in {w.ElapsedMilliseconds} ms");
        Console.WriteLine();
    }
    
    private void DataEngage(string class1, string class2, List<SearchNode> classtable1, List<SearchNode> classtable2)
    {
        //Console.WriteLine();
        Console.WriteLine($"Testing: {class1} vs {class2}");
        Console.WriteLine($"Table size: {classtable1.Count}x{classtable2.Count}");
            
        int st = 0;

        var w = Stopwatch.StartNew();
        
        List<(float[], float[])> data = new();

        foreach (var nodex1 in classtable1)
        {
            classtable2.ForEach(nodex2 =>
            {
                var viewer = new DataBW();
                var ourStats = nodex1.Stats.OutFloats();
                var enemyStats = nodex2.Stats.OutFloats();
                var testx = new Battle(viewer, null, nodex1.HeroCopy, nodex2.HeroCopy);
                var result = testx.StartBattle();
                data.Add((ourStats.Concat(viewer.Data).ToArray(), enemyStats.ToArray()));
                //nodex1.Played(nodex2.HeroClass, result);
                //nodex2.Played(nodex1.HeroClass, -result);
            });

            ++st;

            Console.SetCursorPosition(0, Console.GetCursorPosition().Top);
            Console.Write($"Step: {st,7} / {classtable1.Count,-7}");
        }
        
        using (var writer = new StreamWriter(File.Create(Path.Combine(clagen, $"data_{st:00}.txt"))))
        {
            data.ForEach(d =>
            {
                var str = new StringBuilder();
                foreach (var f in d.Item1) str.Append($"{f} ");
                str.AppendLine();
                foreach (var f in d.Item2) str.Append($"{f} ");
                writer.WriteLine(str);
            });
            writer.Close();
        }
        
        Console.WriteLine();
        Console.WriteLine($"Finished in {w.ElapsedMilliseconds} ms");
        Console.WriteLine();
    }

    private class DataBW : IBattleViewer
    {
        private const int SavedHits = 10;
        private readonly List<double> _memory = new();
        public IEnumerable<float> Data => _memory.Select(x => (float)x);
        private int i = 0;

        public void OnBattleStart(BattleStart model)
        {
            _memory.Add(model.EnemyMaxHealth);
        }

        public void OnAttack(BattleAttack model)
        {
            if (i >= SavedHits) return;
            _memory.Add(model.GiveDamage);
            _memory.Add(model.TakeDamage);
        }

        public void OnHeal(BattleHeal model)
        {
            if (i >= SavedHits) return;
            _memory.Add(model.MineHeal);
            _memory.Add(model.EnemyHeal);
            _memory.Add(Math.Max(0, model.MineHealth));
            _memory.Add(Math.Max(0, model.EnemyHealth));
            ++i;
        }

        public void OnBattleEnd(BattleEnd model)
        {
            while (_memory.Count < (6 * SavedHits + 1))
                _memory.Add(0);
            _memory.Add(model.Winner);
            _memory.Add(model.EnemyWins);
            _memory.Add(model.MineWins);
            _memory.Add(model.TotalRounds);
        }
    }

    private List<SearchNode> LoadMatchup(string mine, string enemy, int maxStr, string dir = clafile)
        => LoadMatchup(mine, dir, $"{mine}_vs_{enemy}.txt", maxStr);

    private List<SearchNode> LoadMatchup(string mine, string dir, string filename, int maxStr = Int32.MaxValue)
    {
        using var file = File.Open(Path.Combine(dir, filename), FileMode.Open);
        var reader = new StreamReader(file);
        HashSet<string> stats = new();
        for (int i = 0; i < maxStr; ++i)
        {
            var str = reader.ReadLine();
            if (str is null) break;
            stats.Add(str[45..]);
        }
        return SearchNode.GetNodes(mine).Condition(x => stats.Contains(x.Stats.ToString()));
    }

    private void SaveMatchup(string mine, string enemy, List<SearchNode> table, string dir = clafile)
    {
        using var file = System.IO.File.Create(Path.Combine(dir, $"{mine}_vs_{enemy}.txt"));
        var writer = new StreamWriter(file);
        table.Sort((x, y) => y.matchups[enemy].AvgPts.CompareTo(x.matchups[enemy].AvgPts));
        table.ForEach(x => writer.WriteLine(x.matchups[enemy] + " Stats: " + x.Stats));
        writer.Close();
    }

    public void Iterate(int i)
    {
        for (int r1 = 0; r1 < _heroes.Count; r1++)
            SearchNode.ResetNodes(_hc[r1]);
        
        for (int r1 = 0; r1 < _heroes.Count; r1++)
        for (int r2 = r1; r2 < _heroes.Count; r2++)
        {
            var class1 = _hc[r1];
            var class2 = _hc[r2];

            var classtable1 = LoadMatchup(class1, class2, i, claiter);
            var classtable2 = LoadMatchup(class2, class1, i, claiter);
            
            TestEngage(class1, class2, classtable1, classtable2);
            
            SaveMatchup(class1, class2, classtable1, claiter);
            SaveMatchup(class2, class1, classtable2, claiter);
        }
    }
}