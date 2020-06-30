using System;
using System.Collections.Generic;
using System.Linq;
using AI.NeuralNetwork;

namespace Aeon.Core.GameProcess.Agents
{
    public class NetworkAgent : IAgent
    {
        public NeuralEnvironment Network;
        
        private readonly HeroClasses _myClass;
        
        public NetworkAgent(NeuralEnvironment network, HeroClasses myClass)
        {
            BattleView = new BattleViewer();
            ShopView = new ShopViewer();
            Network = network;
            _myClass = myClass;
        }

        public BattleViewer BattleView { get; set; }
        public ShopViewer ShopView { get; set; }
        public bool IsBot => true;

        public Command ShopDecision()
        {
            var (stats, shop, enemyId, otherStates) = ShopView.Out();
            var battle = BattleView.Out().ToList();

            var enemyList = new List<double>();
            for (var i = 0; i < HeroMaker.TotalClasses; i++)
                enemyList.Add(0);
            enemyList[(int) enemyId] = 1;

            var battleList = battle.Take(2).Concat(battle.TakeLast(2))
                .SelectMany(tuples => tuples.Where(x=> x.state != StateParameter.MaxHp).Select(x => x.value))
                .Concat(battle[0].Where(x => x.state == StateParameter.MaxHp && !x.isMyState).Select(x => x.value));

            var statsList = stats.Out().Select(x => x.Item2);
            
            var shopList = shop.Out();

            Network.Work(
                statsList.Concat(enemyList).Concat(battleList).Concat(shopList).Concat(otherStates).ToList());
            var workResult = Network.GetMaxOutId();
            return parseCommand(workResult);
        }

        public HeroClasses ChooseClass() => _myClass;

        private Command parseCommand(int result)
        {
            throw new System.NotImplementedException();
        }
        
    }
}