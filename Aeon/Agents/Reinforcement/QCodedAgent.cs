using Aeon.Agents.Network;
using Aeon.Core;
using Aeon.Core.GameProcess;
using AI.NeuralNetwork;
using AI.NeuralNetwork.Algs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.Agents.Reinforcement
{
    public class QCodedAgent : NetworkAgent
    {

        public const float GameWinReward = 1;
        private HeroClasses enemy;
        public float Epsilon = .5f;
        private List<int> currBuy;
        public static Dictionary<(HeroClasses, HeroClasses), List<int[]>> openings;
        private int sessionIter = 0;
        private int openingId = 0;
        private int id = 0;
        public bool Learning = true;

        public int SessionSize = 500;
        public float Gamma = 0.99f;
        public float Speed = 0.1f;



        private record Sample(INetworkData State, int Action, float Reward, INetworkData NextState);
        public BatchingMemory memory;

 

        static QCodedAgent()
        {
            openings = LoadOpenings();
        }



        public QCodedAgent(int id, HeroClasses myClass, bool generateNew = false, bool learning = true) : base(new NeuralEnvironment(), myClass)
        {
            this.id = id;
            if (generateNew) Generate();
            else LoadAgent();
            currBuy = new List<int>(new int[20]);
            memory = new BatchingMemory();
            this.Learning = learning;
        }

        private void Generate()
        {
            Network = NetworkCreator.Perceptron(90, 20, new[] { 100, 100 });
        }

        public override void OnGameOver(int winner)
        {
            base.OnGameOver(winner);
            memory.Reward(winner, terminal: true);
        }

        public override void OnGameStart()
        {
            base.OnGameStart();
            //Define our policy for this game
            enemy = (HeroClasses)_shopView.EnemyNumber;
            openingId = Random.Shared.Next(openings[(_myClass, enemy)].Count);
            currBuy = new List<int>(new int[20]);
        }
        public override Command ShopDecision()
        {
            var round = _shopView.GetRoundNumber();
            if (round == 0) return GetOpeningCommand();
            return ChooseCommand();
        }


        public Command ChooseCommand()
        {
            INetworkData data = MakeInput();
            var netResult = Network.Work(data);
            var cmd = EpsGreedy(MaskInvalidActions(netResult), Epsilon);
            memory.Add(data, cmd);
            if(Learning)TryLearn();
            return Command.Parse(cmd);
        }

        private void TryLearn()
        {
            if (++sessionIter < SessionSize) return;
            Learn();
        }

        public void Learn()
        {
            var Q = Network.Clone();
            var batch = memory.MakeBatch();
            foreach (var sample in batch)
            {
                var rt = sample.NextState is null ? 0 : Network.Work(sample.NextState).Max();
                var kek = Q.Work(sample.State)[sample.Action];
                var loss = rt * Gamma + sample.Reward - kek;
                BackpropagationAlgorithm.BackPropOut(Q, sample.State, loss, sample.Action, Speed);
            }
            Network = Q;
            //System.Console.WriteLine($"Learning agent {id,2} finished ");
            sessionIter = 0;
        }

        public Command GetOpeningCommand()
        {
            for (var i = 0; i < currBuy.Count; i++)
                if (currBuy[i] < openings[(_myClass, enemy)][openingId][i])
                {
                    currBuy[i]++;
                    return Command.Parse(i);
                }
            currBuy = new List<int>(new int[20]);
            return Command.Parse(18);
        }


        private Dictionary<int, float> MaskInvalidActions(List<float> values)
        {
            var maskedActions = new Dictionary<int, float>();
            for (var action = 0; action < values.Count; action++)
                if (_shopView.LegalAction(Command.Parse(action)))
                    maskedActions[action] = values[action];
            return maskedActions;
        }

        private int EpsGreedy(Dictionary<int, float> actions, float eps)
        {
            if (Learning && Random.Shared.NextSingle() < eps)
            {
                return actions.ToList()[Random.Shared.Next(actions.Count)].Key;//get random avaliable action
            }
            // find best action
            var max = float.MinValue;
            var maxId = 0;
            foreach (var (action, value) in actions)
                if (max < value)
                {
                    max = value;
                    maxId = action;
                }
            return maxId;
        }

        public void SaveMultiagent()
        {
            if (Learning)
            {
                var dir = $@"{Program.qMultiAgents}\_{id}_{_myClass}.txt";
                Network.Save(dir);
            }
        }
        private void LoadAgent()
        {
            var files = Directory.EnumerateFiles(Program.qMultiAgents, "*.*", SearchOption.TopDirectoryOnly);
           
            foreach (var file in files)
            {
                if (int.Parse(file.Split("_")[^2]) == id)
                {
                    var text = File.ReadAllText(file);//read file
                    Network = NetworkCreator.MakeFromString(text);
                    return;
                }
            }
        }


        private static Dictionary<(HeroClasses, HeroClasses), List<int[]>> LoadOpenings()
        {
            var openings = new Dictionary<(HeroClasses, HeroClasses), List<int[]>>();
            var files = Directory.EnumerateFiles(Program.openings, "*.*", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {

                var filename = file.Split("\\").Last()[..^4];
                if (!Enum.TryParse(filename.Split("_")[0], out HeroClasses myClass)) continue;
                if (!Enum.TryParse(filename.Split("_")[^1], out HeroClasses enemyClass)) continue;
                openings[(myClass, enemyClass)] = new List<int[]>();
                var lines = File.ReadAllText(file).Split("\n");
                foreach (var line in lines)
                {
                    if (line.Length < 10) continue;
                    var subs = line[45..104];
                    var spaces = subs.Replace(" ", "").Split(",");
                    var opening = spaces.Select(int.Parse).ToArray();
                    openings[(myClass, enemyClass)].Add(opening);
                }
            }
            return openings;
        }
        
    }

    internal class Policy
    {
        public List<int> Opening;
        public NeuralEnvironment Q;
        public Policy(List<int> opening, NeuralEnvironment q)
        {
            Opening = opening;
            Q = q;
        }
    }


}
