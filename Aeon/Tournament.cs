using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aeon.Core.GameProcess;

namespace Aeon
{
    public class Tournament
    {
        public List<IAgent> Participants;

        private Dictionary<IAgent, int> _points;
        private Dictionary<IAgent, List<IAgent>> _enemies;
        private int Buchholz(IAgent agent) => _enemies[agent].Sum(e => _points[e]);

        public Tournament(List<IAgent> participants)
        {
            _points = new Dictionary<IAgent, int>();
            _enemies = new Dictionary<IAgent, List<IAgent>>();
                Participants = new List<IAgent>(participants);
            foreach (var player in participants)
            {
                _points[player] = 0;
                _enemies[player] = new List<IAgent>();
            }
        }

        public int GetPts(IAgent agent) => _points.ContainsKey(agent) ? _points[agent] : throw new ArgumentException();

        private List<(IAgent, IAgent)> MakePairs()
        {
            var pairs = new List<(IAgent, IAgent)>();
            var got = 0;
            while (got < Participants.Count - 1)
            {
                var p1 = Participants[got];
                var p2 = Participants[got + 1];
                pairs.Add((p1, p2));
                _enemies[p1].Add(p2);
                _enemies[p2].Add(p1);
                got += 2;
            }
            return pairs;
        }
        

        public List<IAgent> StartTournament()
        {
            var tour = 1;
            var toursNumber = Math.Ceiling(Math.Log2(Participants.Count)) + 1;

            while (tour <= toursNumber)
            {
                var pairs = MakePairs();
                Parallel.ForEach(pairs, t => {
                    var (player1, player2) = t;
                    var game = new Game(player1, player2);
                    var (score1, score2) = game.Start();
                    lock (_points) {
                        if (score1 > score2)
                            _points[player1]+=3;
                        else if (score1 < score2)
                            _points[player2]+=3;
                        else {
                            _points[player1]++;
                            _points[player2]++;
                        }
                    }
                });
                Participants = Participants.OrderBy(p => _points[p] * 1000000 + Buchholz(p)).ToList();
                Console.WriteLine($"=============== Завершён тур {tour++} ===================");
            }

            var thing = 25;
            foreach (var agent in Participants.Skip(Participants.Count-25)) {
                Console.WriteLine($"Top {thing--}: {agent.ChooseClass()} - {GetPts(agent)} pts");
            }
            
            var game = new Game(Participants[^1], Participants[^2]);
            game.Start(true);

            return Participants;
        }
    }
}