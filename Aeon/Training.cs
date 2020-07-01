using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aeon.Core;
using Aeon.Core.GameProcess;
using Aeon.Core.GameProcess.Agents;
using AI.NeuralNetwork;
using AI.NeuralNetwork.Algs;

namespace Aeon
{
    public class Training
    {
        public List<NetworkAgent> Participants;
        public List<IAgent> Opponents;
        //private Dictionary<NetworkAgent, double> _points;
        
        public readonly HeroClasses ClassToTrain;

        public Training(List<NetworkAgent> @class, List<IAgent> agents, HeroClasses classes)
        {
            Participants = @class;
            ClassToTrain = classes;
            Opponents = agents;
        }

        public void Train(int trainNumber)
        {
            for (int i = 0; i < trainNumber; i++) {
                var newGenes = GeneticAlgorithm.Improve(Participants.Select(p => p.Network).ToList(), GeneticAlgorithm.RandomMerge,
                    list => {
                        var points = new Dictionary<NeuralEnvironment, double>();
                        foreach (var player in list) 
                            points[player] = 0;
                        foreach(var player in list) {
                            Parallel.ForEach(Opponents, enemy => {
                                var game = new Game(new NetworkAgent(player, ClassToTrain), enemy);
                                var (score1, score2) = game.Start();
                                lock (points) {
                                    if (score1 > score2)
                                        points[player] += 4;
                                    else if (score1 == score2)
                                        points[player]++;
                                }
                            });
                            Console.WriteLine(points[player]);
                        }
                        return points.Values.ToList();
                    });
                Participants = newGenes.Select(a => new NetworkAgent(a, ClassToTrain)).ToList();
                Console.WriteLine($"------ Подход №{i+1} закончен ------");
            }
            
            foreach (var (networkAgent, agent2) in Participants.TakeLast(5).Zip(Opponents.TakeLast(5), (agent, agent1) => (agent, agent1))) {
                new Game(networkAgent, agent2).Start(true);
            }

            
        }
    }
}