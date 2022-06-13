using static Tensorflow.Binding;
using static Tensorflow.KerasApi;
using Tensorflow;
using Tensorflow.NumPy;

using Aeon.Core;
using Aeon.Core.GameProcess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aeon.Agents;
using AI.NeuralNetwork;
using AI.NeuralNetwork.Algs;

namespace Aeon;

class Program
{
    public static Random rnd = new Random();
    public static bool debugOutput = false;
    
    const string dir = "./Heroes";
    const string teachdir = "./Teach";
    const string traindir = "./Training";
    const string best = "./Best";
    
    static void Main(string[] args)
    {
        AddNew(128, HeroClasses.Thief, new []{30, 30});
        //MakeNew(16, new[]{30, 30});
        //Teach(1, true);
        //Train(10, HeroClasses.Beast, HeroClasses.Killer, true);
        Gen(25);
        //PlayBest();
        
        Console.WriteLine("Finished");
        Console.ReadLine();
    }

    private static void AddNew(int agents, HeroClasses hero, IReadOnlyCollection<int> hiddenLayers)
    {
        for (int i = 0; i < agents; i++)
        {
            var agent = NetworkCreator.Perceptron(90, 20, hiddenLayers);
            agent.Save(Path.Join(dir, $"{i}_new_{hero}"));
        }
    }

    private static void MakeNew(int agentsForClass, IReadOnlyCollection<int> hiddenLayers)
    {
        Console.WriteLine("АТАС, ТОЧНО УДАЛИТЬ ВСЁ?");
        Console.ReadLine();
        try {
            Directory.Delete(dir, true);
        } catch (DirectoryNotFoundException) { }
        Directory.CreateDirectory(dir);
        for (var i = 0; i < agentsForClass; i++)
        {
            for (var j = 0; j < HeroMaker.TotalClasses; j++) {
                var cls = (HeroClasses) j;
                var agent = NetworkCreator.Perceptron(90, 20, hiddenLayers);
                agent.Save(Path.Join(dir, $"{i*HeroMaker.TotalClasses + j}_new_{cls}"));
            }
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
        var agents = new List<IAgent>();
        var heroDict = new Dictionary<HeroClasses, List<IAgent>>();
        var newDict = new Dictionary<HeroClasses, List<IAgent>>();
        for (var i = 0; i < HeroMaker.TotalClasses; ++i) {
            heroDict[(HeroClasses) i] = new List<IAgent>();
            newDict[(HeroClasses) i] = new List<IAgent>();
        }

        var files = Directory.EnumerateFiles(dir, "*.*", SearchOption.TopDirectoryOnly);

        foreach (var file in files) {
            HeroClasses HClass;
            if (!HeroClasses.TryParse(file.Split("_")[^1], out HClass)) continue;
            NetworkAgent agent;
            agent = new NetworkAgent(NetworkCreator.ReadFromFile(file), HClass);
            agents.Add(agent);
            heroDict[HClass].Add(agent);
        }

        Console.WriteLine($"Прочитано {agents.Count} героев, {heroDict[trainedClass].Count} класса {trainedClass}");
        
        var training = new Training(
            agents.Where(a => a.ChooseClass() == trainedClass).Cast<NetworkAgent>().ToList(), 
            agents.Where(a => a.ChooseClass() == opponent).ToList(),
            trainedClass);
        training.Train(t);
        
        if (replace) {
            foreach (var file in files) {
                if (!HeroClasses.TryParse(file.Split("_")[^1], out HeroClasses HClass)) continue;
                if (HClass == trainedClass)
                    File.Delete(file);
            }
        }
        var filedir = replace ? dir : traindir;
        for (var i = 0; i < training.Participants.Count; i++) {
            training.Participants[i].Network.Save(Path.Join(filedir, $"{i}_trained_{training.ClassToTrain}"));
        }
        
    }

    public static void Gen(int tours)
    {
        var agents = new List<IAgent>();
        IAgent First = new NetworkAgent(new NeuralEnvironment(), HeroClasses.Cheater);
        IAgent Second = new NetworkAgent(new NeuralEnvironment(), HeroClasses.Cheater);
        
        //var heroDict = new Dictionary<HeroClasses, List<IAgent>>();
        //var newDict = new Dictionary<HeroClasses, List<IAgent>>();
        //for (var i = 0; i < HeroMaker.TotalClasses; ++i) {
        //    heroDict[(HeroClasses) i] = new List<IAgent>();
        //    newDict[(HeroClasses) i] = new List<IAgent>();
        //}
        
        var files = Directory.EnumerateFiles(dir, "*.*", SearchOption.TopDirectoryOnly);

        foreach (var file in files) {
            if (!HeroClasses.TryParse(file.Split("_")[^1], out HeroClasses HClass)) continue;
            NetworkAgent agent = new(NetworkCreator.ReadFromFile(file), HClass);
            agents.Add(agent);
            //heroDict[HClass].Add(agent);
        }
        
        Console.WriteLine($"Прочитано {agents.Count} героев");
        
        //newDict = new Dictionary<HeroClasses, List<IAgent>>();
        for (var i = 1; i <= tours; i++) {

            var tour = new Tournament(agents);
            var list = tour.StartTournament();
            First = list[^1];
            Second = list[^2];
            
            //foreach (var heroclass in heroDict) {
            //    Console.WriteLine($"Top {heroclass.Key}: {tour.GetPts(heroclass.Value.OrderBy(a => tour.GetPts(a)).Last())} pts");
            //    Console.WriteLine($"Avg {heroclass.Key}: {heroclass.Value.Average(a => tour.GetPts(a))} pts");
            //}

            var d = agents.Select(a => ((NetworkAgent) a, tour.GetPts(a)))
                .ToDictionary(a => a.Item1.Network, b => (double) b.Item2);


            //
            //foreach (var heroPage in heroDict) {
            //    var azz = GeneticAlgorithm.Improve(
            //        heroPage.Value.Select(a => (NetworkAgent) a)
            //            .Select(a => a.Network)
            //            .ToList(),
            //        GeneticAlgorithm.RandomMerge, 
            //        list => list.Select(l => d[l]).ToList());
            //    var newAgents = azz.Select(network => new NetworkAgent(network, heroPage.Key))
            //        .Cast<IAgent>().ToList();
            //    newDict[heroPage.Key] = newAgents;
            //    agents.AddRange(newAgents);
            //}
 
            var bull = tour.Points.Keys.Select(k => ((NetworkAgent)k).Network).ToList();
            var shit = tour.Points.Values.Select(x => (double)x).ToList();
 
 
            var govno = GeneticAlgorithm.Improve(bull, shit, GeneticAlgorithm.RandomMerge);
            var newAgents = govno.Select(network => new NetworkAgent(network, HeroClasses.Thief))
                .Cast<IAgent>().ToList();
            agents = newAgents;

            //heroDict = newDict;
            //newDict = new Dictionary<HeroClasses, List<IAgent>>();
            
            First = list[^1];
            Second = list[^2];

            Console.WriteLine($"<<<<<<<<<<<<<<<<<<<< КОНЕЦ ТУРНИРА №{i} >>>>>>>>>>>>>>>>>>>>>>");

            Play(First, Second);
        }

        Directory.CreateDirectory(dir + "NEW");
        Directory.CreateDirectory(best + "NEW");

        int ix = 0;
        agents.ForEach(agent =>
        {
            var network = (NetworkAgent) agent;
            network.Network.Save(Path.Join(dir + "NEW", $"{ix++}_{agent.ChooseClass()}"));
        });

        //for (var i = 0; i < agents.Count; i++) {
        //    //var c = HeroMaker.TotalClasses;
        //    var network = (NetworkAgent) agents[i];
        //    network.Network.Save(Path.Join(dir + "NEW", $"{i}_{(HeroClasses) (i)}"));
        //}

        var f = (NetworkAgent) (First);
        var s = (NetworkAgent) (Second);
        f.Network.Save(Path.Join(best + "NEW", $"First_{f.ChooseClass()}"));
        s.Network.Save(Path.Join(best + "NEW", $"Second_{s.ChooseClass()}"));

        Directory.Delete(dir, true);
        Directory.Delete(best, true);

        Directory.Move(dir + "NEW", dir);
        Directory.Move(best + "NEW", best);
    }

    private static void Play(IAgent first, IAgent second)
    {
        new Game(first, second).Start(true);
    }

    public static void PlayBest()
    {
        var files = Directory.EnumerateFiles(best, "*.*", SearchOption.TopDirectoryOnly).ToList();
        if (!HeroClasses.TryParse(files[0].Split("_")[^1], out HeroClasses class1)) return;
        if (!HeroClasses.TryParse(files[1].Split("_")[^1], out HeroClasses class2)) return;
        IAgent First  = new NetworkAgent(NetworkCreator.ReadFromFile(files[0]), class1);
        IAgent Second = new NetworkAgent(NetworkCreator.ReadFromFile(files[1]), class2);
        new Game(First, Second).Start(true);
    }
}
