using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Net;
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
        static void Main(string[] args)
        {
            const string dir = "Heroes";
            var agents = new List<IAgent>();
            
            var heroDict = new Dictionary<HeroClasses, List<IAgent>>();
            var newDict = new Dictionary<HeroClasses, List<IAgent>>();
            for (int i = 0; i < HeroMaker.TotalClasses; ++i) {
                heroDict[(HeroClasses) i] = new List<IAgent>();
                newDict[(HeroClasses) i] = new List<IAgent>();
            }

            IAgent First;
            IAgent Second;

            /*/
            for (var i = 0; i < 256; i++)
            {
                agents.Add(new NetworkAgent(
                    NetworkCreator.Perceptron(90, 20, new List<int> {30, 30, 20}),
                    (HeroClasses)rnd.Next(0, HeroMaker.TotalClasses - 1)));
            }
            /*/
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
            

            for (var i = 1; i <= 1; i++) {

                var tour = new Tournament(agents);
                tour.StartTournament();

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

            Directory.CreateDirectory(dir);
            
            /*/
            for (int i = 0; i < agents.Count; ++i) {
                var network = (NetworkAgent) agents[i];
                network.Network.Save(Path.Join(dir, $"{i}_{agents[i].ChooseClass()}"));
            }
            /*/

            for (int i = 0; i < agents.Count; i++) {
                var c = HeroMaker.TotalClasses;
                var network = (NetworkAgent) agents[i];
                network.Network.Save(Path.Join(dir, $"{i}_{((HeroClasses) (i % c)).ToString()}"));
            }

            Console.WriteLine("Finished");
        }
    }
}