using System.Collections.Generic;
using System.Linq;
using Aeon.Core.Heroes;

namespace Aeon.Core.GameProcess
{
    public class BattleViewer
    {
        private List<BattleState> LogState;
        private (double, double) MaxHP;
        
        public BattleViewer()
        {
            LogState = new List<BattleState>();
        }

        public void Update(BattleState state)
        {
            LogState.Add(state);
        }

        private void Draw()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<(StateParameter state, bool isMyState, int turnNumber, double value)> Out() 
            => LogState.SelectMany(UnpackBs);

        private static IEnumerable<(StateParameter, bool, int, double)> UnpackBs(BattleState state, int turn) // индусы!
        {
            foreach (var (key, value) in state.MyParams) yield return (key, true, turn, value);
            foreach (var (key, value) in state.EnemyParams) yield return (key, false, turn, value);
        }

        public void Reset() => LogState = new List<BattleState>();
    }

    public class ShopViewer
    {
        private Stats HeroStats;
        private Shop HeroShop;
        private HeroClasses EnemyID;
        public ShopViewer()
        {
            
        }
        public void Update(Hero customer)
        {
            HeroStats = customer.Stats;
            HeroShop = customer.Shop;
            EnemyID = customer.EnemyId;
        }
    }
}