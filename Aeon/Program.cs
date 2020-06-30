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
            var agents = new List<IAgent>();
            for (var i = 0; i < HeroMaker.TotalClasses * 10; i++)
            {
                agents.Add(new NetworkAgent(
                    NetworkCreator.Perceptron(90, 20, new List<int>() {40, 20}),
                    (HeroClasses)(i % HeroMaker.TotalClasses)));
            }
            
            agents = new Tournament(agents).StartTournament();
            
            Console.WriteLine("Finished");
        }
    }
}