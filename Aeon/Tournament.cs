using System;
using System.Collections.Generic;
using System.Linq;
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
            var toursNumber = Math.Round(Math.Sqrt(Participants.Count)) + 2;

            while (tour <= toursNumber)
            {
                var pairs = MakePairs();
                foreach (var (player1, player2) in pairs)
                {
                    var game = new Game(player1, player2);
                    var (score1, score2) = game.Start();
                    if (score1 > score2)
                        _points[player1]++;
                    else
                        _points[player2]++;
                }
                Participants = Participants.OrderBy(p => _points[p] * 1000000 + Buchholz(p)).ToList();
                tour++;
            }

            return Participants;
        }
    }
}