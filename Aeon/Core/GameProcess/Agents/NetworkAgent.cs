using System;
using System.Collections.Generic;
using System.Linq;

namespace Aeon.Core.GameProcess.Agents
{
    public class NetworkAgent : IAgent
    {
        private Func<List<double>, int> _networkJob;

        private readonly HeroClasses _myClass;
        
        public NetworkAgent(Func<List<double>, int> networkJob, HeroClasses myClass)
        {
            BattleView = new BattleViewer();
            ShopView = new ShopViewer();
            _networkJob = networkJob;
            _myClass = myClass;
        }

        public BattleViewer BattleView { get; set; }
        public ShopViewer ShopView { get; set; }
        public bool IsBot => true;

        public Command ShopDecision()
        {
            var (stats, shop, enemyId, otherStates) = ShopView.Out();
            var battle = BattleView.Out().ToList();

            var enemyList = new List<double>(HeroMaker.TotalClasses) {0};
            enemyList[(int) enemyId] = 1;

            var battleList = battle.Take(2).Concat(battle.TakeLast(2))
                .SelectMany(tuples => tuples.Where(x=> x.state != StateParameter.MaxHp).Select(x => x.value))
                .Concat(battle[0].Where(x => x.state == StateParameter.MaxHp && !x.isMyState).Select(x => x.value));

            var statsList = stats.Out().Select(x => x.Item2);
            
            var shopList = shop.Out();

            var workResult = _networkJob(
                statsList.Concat(enemyList).Concat(battleList).Concat(shopList).Concat(otherStates).ToList());
            return parseCommand(workResult);
        }

        public HeroClasses ChooseClass() => _myClass;

        private Command parseCommand(int result)
        {
            throw new System.NotImplementedException();
        }
        
    }
}