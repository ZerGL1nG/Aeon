using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aeon.Core.GameProcess;

namespace Aeon;

public interface ITournament
{
    public IEnumerable<IAgent> Participants { get; }

    //public List<(IAgent, IAgent)> MakePairs();

    public List<IAgent> StartTournament(bool debug);
}

public class Tournament: ITournament
{
    private List<IAgent> _agents;
    private readonly Dictionary<IAgent, List<IAgent>> _enemies;

    private readonly Dictionary<IAgent, int> _points;

    public Tournament(IEnumerable<IAgent> participants)
    {
        _points  = new Dictionary<IAgent, int>();
        _enemies = new Dictionary<IAgent, List<IAgent>>();
        _agents  = new List<IAgent>(participants);
        foreach (var player in participants) {
            _points[player]  = 0;
            _enemies[player] = new List<IAgent>();
        }
    }

    public IReadOnlyDictionary<IAgent, int> Points => _points;
    public IEnumerable<IAgent> Participants => _agents;


    public List<IAgent> StartTournament(bool show = false)
    {
        var tour        = 1;
        var toursNumber = Math.Ceiling(Math.Log2(_agents.Count));

        while (tour <= toursNumber) {
            var pairs = MakePairs();
            if (show)
                pairs.ForEach(Battle);
            else
                Parallel.ForEach(pairs, Battle);
            _agents = _agents.OrderBy(p => _points[p]*1000000+Buchholz(p)).ToList();
            Console.WriteLine($"=============== Завершён тур {tour++:00} ===================");
        }



        var game = new Game(_agents[^1], _agents[^2]);
        game.Start(true);
        var thing = 25;
        foreach (var agent in _agents.Skip(_agents.Count - 25))
            Console.WriteLine($"Top {thing--}: {agent.ChooseClass()} - {GetPts(agent)} pts");


        return _agents;

        void Battle((IAgent, IAgent) t)
        {
            var (player1, player2) = t;
            var game = new Game(player1, player2);
            var (score1, score2) = game.Start(show);
            lock (_points) {
                if (score1 > score2)
                    _points[player1] += 4;
                else if (score1 < score2)
                    _points[player2] += 4;
                else {
                    _points[player1] += 1;
                    _points[player2] += 1;
                }
            }
        }
    }

    private int Buchholz(IAgent agent) => _enemies[agent].Sum(e => _points[e]);

    public int GetPts(IAgent agent) => _points.ContainsKey(agent)? _points[agent] : throw new ArgumentException();

    private List<(IAgent, IAgent)> MakePairs()
    {
        var pairs = new List<(IAgent, IAgent)>();
        var got   = 0;
        for (var i = 0; i < _agents.Count-1; ++i) {
            var r = Random.Shared.Next(i, _agents.Count-1);
            (_agents[i], _agents[r]) = (_agents[r], _agents[i]);
        }
        while (got < _agents.Count-1) {
            var p1 = _agents[got];
            var p2 = _agents[got+1];
            pairs.Add((p1, p2));
            _enemies[p1].Add(p2);
            _enemies[p2].Add(p1);
            got += 2;
        }
        return pairs;
    }
}