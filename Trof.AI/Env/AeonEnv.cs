using Aeon.Agents.Network;
using Aeon.Agents.Reinforcement;
using Aeon.Core;
using Aeon.Core.GameProcess;
using Trof.AI.Misc;

namespace Trof.AI.Env;

public class AeonEnv: Kachalka<AeonTurnIn, AeonTurnOut>
{
    public AeonEnv(string projectDir): base(projectDir) { }

    public override async Task Kach(int? amount = null, CancellationToken ct = default)
    {
        amount ??= int.MaxValue;
        for (var i = 0; i < amount; i++) {
            Agents.Shuffle();
            Console.WriteLine();
            Console.Write($"Kachalka: #{i,4} ");

            for (var x = 0; x < Agents.Count/2; ++x) {
                Console.Write(".");
                var agent1 = new AgentAdapter(Agents[x * 2]);
                var agent2 = new AgentAdapter(Agents[x * 2 + 1]);
                var game = new Game(agent1, agent2);
                game.Start();
            }
            
            //Parallel.For(0, Agents.Count / 2, x => {
            //    var agent1 = new AgentAdapter(Agents[x * 2]);
            //    var agent2 = new AgentAdapter(Agents[x * 2 + 1]);
            //    var game = new Game(agent1, agent2);
            //    game.Start();
            //});
            
            if (ct.IsCancellationRequested) break;
        }
        foreach (var agent in Agents) {
            await agent.Save(Path.Combine(ProjectDir, agent.ID));
        }
    }

    private class AgentAdapter : Aeon.Core.GameProcess.IAgent
    {
        private IAgent<AeonTurnIn, AeonTurnOut> _adaptee;
        public AgentAdapter(IAgent<AeonTurnIn, AeonTurnOut> agent) => _adaptee = agent;

        public IBattleViewer BattleView => _battleViewer;
        private readonly QBattleViewer _battleViewer = new(10, 5); // 91 = 1 + 15*5
        public IShopViewer ShopView => _shopViewer;
        private readonly NetworkShopViewer _shopViewer = new(); // 50 inputs
        public bool IsBot => true;

        public Command ShopDecision()
        {
            var cx = _adaptee.Decide(MakeInput(), MakeMask());
            return Command.Parse(cx);
        }

        public HeroClasses ChooseClass() => HeroClasses.Banker;

        public void OnGameStart() { }

        public void OnGameOver(int winner)
        {
            if (winner == 1) _adaptee.GetReward(100);
            _adaptee.OnEpisodeEnd(MakeInput());
        }

        private AeonTurnIn MakeInput() => new() {
            BattleData = _battleViewer.Inputs,
            ShopData   = _shopViewer.Inputs,
            EnemyHero  = MakeEnemyList(_shopViewer.EnemyNumber),
        };
        
        private List<float> MakeEnemyList(int enemyN) // 15 inputs
        {
            var enemyList = new List<float>();
            for (var i = 0; i < HeroMaker.TotalClasses; i++) enemyList.Add(0);
            enemyList[enemyN] = 1;
            return enemyList;
        }

        private Mask<AeonTurnOut> MakeMask()
        {
            var allowed = new List<int>();
            for (var i = 0; i < 19; i++) {
                if (_shopViewer.WillExit(Command.Parse(i))) continue;
                allowed.Add(i);
            }
            allowed.Add(19);
            return new Mask<AeonTurnOut>(allowed);
        }
    }
}

public class AeonAgent: NeuralAgent<AeonTurnIn, AeonTurnOut>
{
    public AeonAgent(string path, int id)
    {
        ID = $"AeonAgent{id}";
        NeuralEnv = new QPerceptron<AeonTurnIn, AeonTurnOut>(Pass, 100, 70, 40) 
            { Dir = path, AgentN = id };
        ChoiceMaker = new EGreedy<AeonTurnOut> { Epsilon = 0.1f };
    }

    public override void Init() { }
}

public class AeonTurnIn: INetworkData
{
    public IEnumerable<float> BattleData { private get; init; } = null!;
    public IEnumerable<float> ShopData   { private get; init; } = null!;
    public IEnumerable<float> EnemyHero  { private get; init; } = null!;

    public IEnumerable<float> Data => BattleData.Concat(ShopData).Concat(EnemyHero);
    public static int Size => 91 + 50 + 15;

    public static string GetName(int index) => throw new NotImplementedException();

    public static INetworkData Ret(IEnumerable<float> data) => throw new NotImplementedException();
}

public class AeonTurnOut: INetworkData
{
    private List<float> _data;
    public AeonTurnOut(IEnumerable<float> e) => _data = e.ToList();
    
    public IEnumerable<float> Data => _data;
    public static int Size => 20;

    public static string GetName(int index) => throw new NotImplementedException();

    public static INetworkData Ret(IEnumerable<float> data) => new AeonTurnOut(data);
}
