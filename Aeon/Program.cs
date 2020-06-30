using System;
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
            
            //var d = new Dictionary<NeuralEnvironment, double>();
            
            var agents = new List<IAgent>();
            for (var i = 0; i < HeroMaker.TotalClasses * 10; i++)
            {
                agents.Add(new NetworkAgent(
                    NetworkCreator.Perceptron(90, 20, new List<int> {30, 30, 20}),
                    (HeroClasses)(i % HeroMaker.TotalClasses)));
            }

            for (var i = 1; i <= 10; i++) {

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
            

            Console.WriteLine("Finished");
        }
    }
}