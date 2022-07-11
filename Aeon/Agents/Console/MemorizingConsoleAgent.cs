using Aeon.Agents.Network;
using Aeon.Agents.Reinforcement;
using Aeon.Core;
using Aeon.Core.GameProcess;
using AI.NeuralNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.Agents.Console
{
    internal class MemorizingConsoleAgent : ConsoleAgent
    {
        public BatchingMemory memory;
        protected NetworkShopViewer  sw;
        protected NetworkBattleViewer bw;
        public MemorizingConsoleAgent() : base()
        {
            memory = new BatchingMemory();
            _sw = sw = new NetworkShopViewer(true);
            _bw = bw = new NetworkBattleViewer(true);
        }
        public override void OnGameOver(int winner)
        {
            base.OnGameOver(winner);
            memory.Reward(winner, terminal: true);
        }

        public override Command ShopDecision()
        {
            var command = GetCommandId();
            INetworkData data = MakeInput();
            memory.Add(data, command);
            return Command.Parse(command);
        }

        protected ComposeData MakeInput() => new(bw.State, sw.State, MakeEnemyList(sw.EnemyNumber));
        protected static ArrayData MakeEnemyList(int enemyN)
        {
            var enemyList = new List<float>();
            for (var i = 0; i < HeroMaker.TotalClasses; i++) enemyList.Add(0);
            enemyList[enemyN] = 1;
            return new ArrayData(enemyList);
        }
    }
}
