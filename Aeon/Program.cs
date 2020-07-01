using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;
using System.Xml;
using Aeon.Core;
using Aeon.Core.GameProcess;
using Aeon.Core.GameProcess.Agents;
using AI.NeuralNetwork;
using AI.NeuralNetwork.Algs;

namespace Aeon
{
    class Program
    {
        public static Random rnd = new Random();
        public static bool debugOutput = false;
        
        const string dir = "Heroes";
        const string teachdir = "Teach";
        const string traindir = "Training";
        const string best = "Best";
        
        static void Main(string[] args)
        {
            //PlayBest();
            //Gen(10);
            //Train(0);
            Teach(3);
            Console.WriteLine("Finished");
        }

        public static void Teach(int kek)
        {

            var human = new ConsoleAgent();

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
                if (HeroClasses.TryParse(file.Split("_")[^1], out HClass)) {
                    NetworkAgent agent;
                    agent = new NetworkAgent(NetworkCreator.ReadFromFile(file), HClass);
                    agents.Add(agent);
                    heroDict[HClass].Add(agent);
                }
            }

            Console.WriteLine($"Прочитано {agents.Count} героев");
            
            var tour = new Tournament(agents.Append(human).ToList());
            tour.StartTournament();
            
            var teaching = agents.Where(a => a.ChooseClass() == human.ChooseClass()).Cast<NetworkAgent>().ToList();

            foreach (var agent in teaching) {
                for (var i = 0; i < kek; i++) 
                    BackpropagationAlgorithm.Teach(agent.Network, human.DataSet);
            }
            
            for (var i = 0; i < teaching.Count(); i++) {
                teaching[i].Network.Save(Path.Join(teachdir, $"{i}_teach_{human.ChooseClass()}"));
            }
            
        }
        
        public static void Train(int t)
        {
            var trainedClass = HeroClasses.Fatty;

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
                agents.Where(a => a.ChooseClass() == HeroClasses.Cheater).ToList(),
                HeroClasses.Fatty);
            training.Train(t);
            
            
            for (var i = 0; i < training.Participants.Count; i++) {
                training.Participants[i].Network.Save(Path.Join(traindir, $"{i}_trained_{training.ClassToTrain}"));
            }
            
        }

        public static void Gen(int tours)
        {
            var agents = new List<IAgent>();
            IAgent First = new NetworkAgent(new NeuralEnvironment(), HeroClasses.Cheater);
            IAgent Second = new NetworkAgent(new NeuralEnvironment(), HeroClasses.Cheater);
            
            var heroDict = new Dictionary<HeroClasses, List<IAgent>>();
            var newDict = new Dictionary<HeroClasses, List<IAgent>>();
            for (var i = 0; i < HeroMaker.TotalClasses; ++i) {
                heroDict[(HeroClasses) i] = new List<IAgent>();
                newDict[(HeroClasses) i] = new List<IAgent>();
            }

            /*/
            for (var i = 0; i < 256; i++)
            {
                var @class = (HeroClasses)(i%HeroMaker.TotalClasses);
                var agent = new NetworkAgent(
                    NetworkCreator.Perceptron(90, 20, new List<int> {100}),
                    @class);
                agents.Add(agent);
                heroDict[@class].Add(agent);
            }
            /*/
            var files = Directory.EnumerateFiles(dir, "*.*", SearchOption.TopDirectoryOnly);

            foreach (var file in files) {
                HeroClasses HClass;
                if (!HeroClasses.TryParse(file.Split("_")[^1], out HClass)) continue;
                NetworkAgent agent;
                agent = new NetworkAgent(NetworkCreator.ReadFromFile(file), HClass);
                agents.Add(agent);
                heroDict[HClass].Add(agent);
            }
            
            Console.WriteLine($"Прочитано {agents.Count} героев");
            
            newDict = new Dictionary<HeroClasses, List<IAgent>>();
            for (var i = 1; i <= tours; i++) {

                var tour = new Tournament(agents);
                var list = tour.StartTournament();
                First = list[^1];
                Second = list[^2];
                
                foreach (var heroclass in heroDict) {
                    Console.WriteLine($"Top {heroclass.Key}: {tour.GetPts(heroclass.Value.OrderBy(a => tour.GetPts(a)).Last())} pts");
                    Console.WriteLine($"Avg {heroclass.Key}: {heroclass.Value.Average(a => tour.GetPts(a))} pts");
                }

                var d = agents.Select(a => ((NetworkAgent) a, tour.GetPts(a)))
                    .ToDictionary(a => a.Item1.Network, b => (double) b.Item2);

                agents = new List<IAgent>();

                foreach (var heroPage in heroDict) {
                    var azz = GeneticAlgorithm.Improve(
                        heroPage.Value.Select(a => (NetworkAgent) a)
                            .Select(a => a.Network)
                            .ToList(),
                        GeneticAlgorithm.RandomMerge, 
                        list => list.Select(l => d[l]).ToList());
                    var newAgents = azz.Select(network => new NetworkAgent(network, heroPage.Key))
                        .Cast<IAgent>().ToList();
                    newDict[heroPage.Key] = newAgents;
                    agents.AddRange(newAgents);
                }

                heroDict = newDict;
                newDict = new Dictionary<HeroClasses, List<IAgent>>();

                Console.WriteLine($"<<<<<<<<<<<<<<<<<<<< КОНЕЦ ТУРНИРА №{i} >>>>>>>>>>>>>>>>>>>>>>");

            }
            
            Directory.Delete(dir, true);
            Directory.Delete(best, true);
            Directory.CreateDirectory(dir);
            Directory.CreateDirectory(best);
            
            

            for (var i = 0; i < agents.Count; i++) {
                var c = HeroMaker.TotalClasses;
                var network = (NetworkAgent) agents[i];
                network.Network.Save(Path.Join(dir, $"{i}_{((HeroClasses) (i % c)).ToString()}"));
            }

            var f = (NetworkAgent) (First);
            var s = (NetworkAgent) (Second);
            f.Network.Save(Path.Join(best, $"First_{f.ChooseClass()}"));
            s.Network.Save(Path.Join(best, $"Second_{s.ChooseClass()}"));
            
            
        }
        
        public static void PlayBest()
        {
            IAgent First = new NetworkAgent(new NeuralEnvironment(), HeroClasses.Cheater);
            IAgent Second = new NetworkAgent(new NeuralEnvironment(), HeroClasses.Cheater);
            var files = Directory.EnumerateFiles(best, "*.*", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                HeroClasses HClass;
                if (!HeroClasses.TryParse(file.Split("_")[^1], out HClass)) continue;
                Second = new NetworkAgent(NetworkCreator.ReadFromFile(file), HClass);
                First = Second;
            }


            new Game(First, Second).Start();
        }
    }
}