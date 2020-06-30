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
        static void Main(string[] args)
        {
            var rnd = new Random();
            const string dir = "Heroes";
            var agents = new List<IAgent>();
            
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
                    agents.Add(new NetworkAgent(NetworkCreator.ReadFromFile(file), HClass));
                }
            }
            

            for (var i = 1; i <= 1; i++) {

                var tour = new Tournament(agents);
                tour.StartTournament();

                var d = agents.Select(a => ((NetworkAgent) a, tour.GetPts(a)))
                    .ToDictionary(a => a.Item1.Network, b => (double) b.Item2);

                var azz = GeneticAlgorithm.Improve(agents.Select(a => (NetworkAgent) a).Select(a => a.Network).ToList(),
                    GeneticAlgorithm.RandomMerge, list => list.Select(l => d[l]).ToList());

                agents = azz.Select(network => new NetworkAgent(network,
                    (HeroClasses) rnd.Next(0, HeroMaker.TotalClasses - 1))).Cast<IAgent>().ToList();
                
                Console.WriteLine($"<<<<<<<<<<<<<<<<<<<< КОНЕЦ ТУРНИРА №{i} >>>>>>>>>>>>>>>>>>>>>>");

            }
            
            Directory.Delete(dir, true);

            Directory.CreateDirectory(dir);

            for (int i = 0; i < agents.Count; ++i) {
                var network = (NetworkAgent) agents[i];
                network.Network.Save(Path.Join(dir, $"{i}_{agents[i].ChooseClass()}"));
            }
            

            Console.WriteLine("Finished");
        }
    }
}