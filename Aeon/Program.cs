using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aeon.Agents.Network;
using Aeon.Agents.Reinforcement;
using Aeon.Builds;
using Aeon.Core;
using Aeon.Core.GameProcess;
using AI.NeuralNetwork;
using AI.NeuralNetwork.Algs;

namespace Aeon;

internal class Program
{
    public const string Path = @"C:\Users\Tupotrof\source_x\NeuralShit\";
    private const string dir = Path+"Heroes";
    private const string teachdir = Path+"Teach";
    private const string traindir = Path+"Training";
    private const string best = Path+"Best";
    private const string autosave = Path+"autosaves";
    public const string dump = Path+"Dump";
    public static Random rnd = new();
    public static bool debugOutput = false;

    private static void Main(string[] args)
    {
        //AddNew(2, HeroClasses.Banker, new []{30, 30});
        //MakeNew(2, new[]{30, 30});
        //Teach(1, true);
        //Train(10, HeroClasses.Beast, HeroClasses.Killer, true);
        //QTrain(1000);
        //Gen(8);
        //PlayBest(1000, true);
        //PlayBest(10000, true, 100, 1000, 2000);
        //PlayBest(100000, true, 500, 125, 500);
        //PlayBest(1, true);
        //Test();
        //BestTest();

        //var hero1 = HeroMaker.Make(HeroClasses.Banker);
        //hero1.TryToBuy(Stat.Regen, false);
        //hero1.TryToBuy(Stat.Regen, false);
        //hero1.TryToBuy(Stat.Regen, false);
        //hero1.TryToBuy(Stat.Regen, false);
        //hero1.TryToBuy(Stat.Regen, false);
        //hero1.TryToBuy(Stat.Regen, false);
        //var hero2 = HeroMaker.Make(HeroClasses.Banker);
        //hero2.TryToBuy(Stat.Attack, false);
        //var battle = new Battle(null, null, hero1, hero2);
        //battle.StartBattle();

        var searcher = new Searcher();

        searcher.AddClass(HeroMaker.Make(HeroClasses.Banker), "Basic");
        searcher.AddClass(HeroMaker.Make(HeroClasses.Beast), "Beast");
        searcher.AddClass(HeroMaker.Make(HeroClasses.Cheater), "Cheater");
        searcher.AddClass(HeroMaker.Make(HeroClasses.Fatty), "Fatty");
        searcher.AddClass(HeroMaker.Make(HeroClasses.Killer), "Killer");
        searcher.AddClass(HeroMaker.Make(HeroClasses.Master), "Master");
        searcher.AddClass(HeroMaker.Make(HeroClasses.Rogue), "Rogue");
        searcher.AddClass(HeroMaker.Make(HeroClasses.Thief), "Thief");
        searcher.AddClass(HeroMaker.Make(HeroClasses.Vampire), "Vampire");

        var bes = HeroMaker.Make(HeroClasses.BloodyElf);
        bes.UseAbility();
        bes.UseAbility();

        var ber = HeroMaker.Make(HeroClasses.BloodyElf);
        ber.UseAbility();
        ber.UseAbility();
        ber.UseAbility();

        searcher.AddClass(bes, "BloodyElf-Spell");
        searcher.AddClass(ber, "BloodyElf-Regen");
        searcher.Init();

        //searcher.BattleAll();
        //searcher.TestBest(1000);
        searcher.Iterate(250);
        searcher.Iterate(100);
        searcher.Iterate(80);
        searcher.Iterate(60);
        searcher.Iterate(50);
        searcher.Iterate(40);
        searcher.Iterate(30);
        searcher.Iterate(20);

        Console.WriteLine("Finished");
        //Console.ReadLine();
    }

    private static void AddNew(int agents, HeroClasses hero, IReadOnlyCollection<int> hiddenLayers)
    {
        for (var i = 0; i < agents; i++) {
            var agent = NetworkCreator.Perceptron(90, 20, hiddenLayers);
            agent.Save(System.IO.Path.Join(dir, $"{i}_new_{hero}"));
        }
    }

    private static void MakeNew(int agentsForClass, IReadOnlyCollection<int> hiddenLayers)
    {
        Console.WriteLine("АТАС, ТОЧНО УДАЛИТЬ ВСЁ?");
        Console.ReadLine();
        try {
            //Directory.Delete(dir, true);
        } catch (DirectoryNotFoundException) { }
        //Directory.CreateDirectory(dir);
        for (var i = 0; i < agentsForClass; i++)
        for (var j = 0; j < HeroMaker.TotalClasses; j++) {
            var cls   = (HeroClasses)j;
            var agent = NetworkCreator.Perceptron(90, 20, hiddenLayers);
            agent.Save(System.IO.Path.Join(dir, $"{i*HeroMaker.TotalClasses+j}_new_{cls}"));
        }
    }

    //public static void Teach(int iterations, bool replace = false)
    //{
//
    //    //var human = new ConsoleAgent();
//
    //    var agents = new List<IAgent>();
    //    var heroDict = new Dictionary<HeroClasses, List<IAgent>>();
    //    var newDict = new Dictionary<HeroClasses, List<IAgent>>();
    //    for (var i = 0; i < HeroMaker.TotalClasses; ++i) {
    //        heroDict[(HeroClasses) i] = new List<IAgent>();
    //        newDict[(HeroClasses) i] = new List<IAgent>();
    //    }
//
    //    var files = Directory.EnumerateFiles(dir, "*.*", SearchOption.TopDirectoryOnly);
//
    //    foreach (var file in files) {
    //        HeroClasses HClass;
    //        if (HeroClasses.TryParse(file.Split("_")[^1], out HClass)) {
    //            NetworkAgent agent;
    //            agent = new NetworkAgent(NetworkCreator.ReadFromFile(file), HClass);
    //            agents.Add(agent);
    //            heroDict[HClass].Add(agent);
    //        }
    //    }
//
    //    Console.WriteLine($"Прочитано {agents.Count} героев");
    //    
    //    var tour = new Tournament(agents.Append(human).ToList());
    //    tour.StartTournament();
    //    
    //    var teaching = agents.Where(a => a.ChooseClass() == human.ChooseClass()).Cast<NetworkAgent>().ToList();
//
    //    Parallel.ForEach(teaching, agent => {
    //        for (var i = 0; i < iterations; i++)
    //            BackpropagationAlgorithm.Teach(agent.Network, human.DataSet);
    //    });
//
    //    if (replace) {
    //        foreach (var file in files) {
    //            if (!HeroClasses.TryParse(file.Split("_")[^1], out HeroClasses HClass)) continue;
    //            if (HClass == human.ChooseClass())
    //                File.Delete(file);
    //        }
    //    }
    //    var filedir = replace ? dir : teachdir;
    //    for (var i = 0; i < teaching.Count(); i++) {
    //        teaching[i].Network.Save(Path.Join(filedir, $"{i}_teach_{human.ChooseClass()}"));
    //    }
    //    
    //}

    public static void Train(int t, HeroClasses trainedClass, HeroClasses opponent, bool replace = false)
    {
        var agents   = new List<IAgent>();
        var heroDict = new Dictionary<HeroClasses, List<IAgent>>();
        var newDict  = new Dictionary<HeroClasses, List<IAgent>>();
        for (var i = 0; i < HeroMaker.TotalClasses; ++i) {
            heroDict[(HeroClasses)i] = new List<IAgent>();
            newDict[(HeroClasses)i]  = new List<IAgent>();
        }

        var files = Directory.EnumerateFiles(dir, "*.*", SearchOption.TopDirectoryOnly);

        foreach (var file in files) {
            HeroClasses HClass;
            if (!Enum.TryParse(file.Split("_")[^1], out HClass)) continue;
            NetworkAgent agent;
            agent = new NetworkAgent(NetworkCreator.ReadFromFile(file), HClass);
            agents.Add(agent);
            heroDict[HClass].Add(agent);
        }

        Console.WriteLine($"Прочитано {agents.Count} героев, {heroDict[trainedClass].Count} класса {trainedClass}");

        var training = new Training(agents.Where(a => a.ChooseClass() == trainedClass).Cast<NetworkAgent>().ToList(),
                                    agents.Where(a => a.ChooseClass() == opponent).ToList(), trainedClass);
        training.Train(t);

        if (replace)
            foreach (var file in files) {
                if (!Enum.TryParse(file.Split("_")[^1], out HeroClasses HClass)) continue;
                if (HClass == trainedClass) File.Delete(file);
            }
        var filedir = replace? dir : traindir;
        for (var i = 0; i < training.Participants.Count; i++)
            training.Participants[i].Network.Save(System.IO.Path.Join(filedir, $"{i}_trained_{training.ClassToTrain}"));
    }


    public static List<T> ReadFrom<T>(string path) where T: IAgent
    {
        var agents = new List<T>();
        var files  = Directory.EnumerateFiles(path, "*.*", SearchOption.TopDirectoryOnly);

        foreach (var file in files) {
            if (!Enum.TryParse(file.Split("_")[^1], out HeroClasses HClass)) continue;
            var agent = (T)Activator.CreateInstance(typeof(T), NetworkCreator.ReadFromFile(file), HClass);
            agents.Add(agent);
            //heroDict[HClass].Add(agent);
        }
        Console.WriteLine($"Прочитано {agents.Count} героев");
        return agents;
    }


    public static List<(IAgent, int)> Tournament(IEnumerable<IAgent> agents, int i, bool show)
    {
        var tour = new Tournament(agents);
        var list = tour.StartTournament(show);
        Console.WriteLine($"<<<<<<<<<<<<<<<<<<<< КОНЕЦ ТУРНИРА №{i} >>>>>>>>>>>>>>>>>>>>>>");
        return agents.Select(a => (a, tour.GetPts(a))).ToList();
    }

    public static void Save<T>(List<T> agents) where T: NetworkAgent
    {
        Directory.CreateDirectory(dir+"NEW");
        Directory.CreateDirectory(best+"NEW");

        var ix = 0;
        agents.ForEach(agent => {
            var network = agent;
            network.Network.Save(System.IO.Path.Join(dir+"NEW", $"{ix++}_{agent.ChooseClass()}"));
        });

        //for (var i = 0; i < agents.Count; i++) {
        //    //var c = HeroMaker.TotalClasses;
        //    var network = (NetworkAgent) agents[i];
        //    network.Network.Save(Path.Join(dir + "NEW", $"{i}_{(HeroClasses) (i)}"));
        //}

        var f = agents[^1];
        var s = agents[^2];
        f.Network.Save(System.IO.Path.Join(best+"NEW", $"First_{f.ChooseClass()}"));
        s.Network.Save(System.IO.Path.Join(best+"NEW", $"Second_{s.ChooseClass()}"));

        Directory.Delete(dir, true);
        Directory.Delete(best, true);

        Directory.Move(dir+"NEW", dir);
        Directory.Move(best+"NEW", best);
    }


    public static void QTrain(int tours, bool show = false)
    {
        var agents = ReadFrom<QAgent>(dir);

        for (var tour = 0; tour < tours; ++tour) {
            var result = Tournament(agents, tour, show);
        }

        Save(agents);
    }

    public static void Test()
    {
        var agents = ReadFrom<NetworkAgent>(dir);
        Tournament(agents, 0, true);
    }

    public static void Gen(int tours, bool show = false)
    {
        var agents = ReadFrom<NetworkAgent>(dir);

        IAgent First = null, Second = null;

        //newDict = new Dictionary<HeroClasses, List<IAgent>>();
        for (var i = 1; i <= tours; i++) {
            var result = Tournament(agents, i, show);

            var bull = result.Select(k => ((NetworkAgent)k.Item1).Network).ToList();
            var shit = result.Select(x => (float)x.Item2).ToList();

            var govno     = GeneticAlgorithm.Improve(bull, shit, GeneticAlgorithm.RandomMerge);
            var newAgents = govno.Select(network => new NetworkAgent(network, HeroClasses.Thief)).ToList();
            agents = newAgents;

            //heroDict = newDict;
            //newDict = new Dictionary<HeroClasses, List<IAgent>>();

            First  = result[^1].Item1;
            Second = result[^2].Item1;

            Play(First, Second);
        }

        Save(agents);
    }

    private static void Play(IAgent first, IAgent second)
    {
        new Game(first, second).Start(true);
    }

    public static void PlayBest(
        int games = 0, bool debug = false, int debugEveryX = 100, int switchN = 100, int saveEvery = 2000)
    {
        var files = Directory.EnumerateFiles(best, "*.*", SearchOption.TopDirectoryOnly).ToList();
        if (!Enum.TryParse(files[0].Split("_")[^1], out HeroClasses class1)) return;
        if (!Enum.TryParse(files[1].Split("_")[^1], out HeroClasses class2)) return;
        var First  = new QAgent(NetworkCreator.ReadFromFile(files[0]), class1);
        var Second = new QAgent(NetworkCreator.ReadFromFile(files[1]), class2);
        for (var i = 0; i <= games; ++i) {
            if (i%10 == 0) {
                Console.SetCursorPosition(0, Console.GetCursorPosition().Top);
                Console.Write($"After {i} games:");
            }
            First.LearnMode  = i/switchN%2 == 1;
            Second.LearnMode = i/switchN%2 == 0;
            new Game(First, Second).Start(debug && i%debugEveryX == 0);
            if (i%saveEvery == 0 && i != 0) {
                Console.WriteLine("Autosave...");
                First.Network.Save(System.IO.Path.Join(autosave, $"First_x{i}_{First.ChooseClass()}"));
                Second.Network.Save(System.IO.Path.Join(autosave, $"Second_x{i}_{Second.ChooseClass()}"));
            }
        }
        First.Network.Save(System.IO.Path.Join(best, $"First_{First.ChooseClass()}"));
        Second.Network.Save(System.IO.Path.Join(best, $"Second_{Second.ChooseClass()}"));
    }

    public static void BestTest()
    {
        var files = Directory.EnumerateFiles(best, "*.*", SearchOption.TopDirectoryOnly).ToList();
        if (!Enum.TryParse(files[0].Split("_")[^1], out HeroClasses class1)) return;
        if (!Enum.TryParse(files[1].Split("_")[^1], out HeroClasses class2)) return;
        var First  = new NetworkAgent(NetworkCreator.ReadFromFile(files[0]), class1);
        var Second = new NetworkAgent(NetworkCreator.ReadFromFile(files[1]), class2);
        Console.Write("========== TEST WITHOUT EPS ==========");
        new Game(First, Second).Start(true);
    }
}