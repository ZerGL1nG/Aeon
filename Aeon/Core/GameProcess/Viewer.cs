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

        public IEnumerable<IEnumerable<(StateParameter state, bool isMyState, double value)>> Out() =>
            LogState.Select(UnpackBs);
        
            

        public static IEnumerable<(StateParameter, bool, double)> UnpackBs(BattleState state) // индусы!
        {
            foreach (var (key, value) in state.MyParams) yield return (key, true, value);
            foreach (var (key, value) in state.EnemyParams) yield return (key, false, value);
        }

        public void Reset() => LogState = new List<BattleState>();
    }

    public class ShopViewer
    {
        private Stats HeroStats;
        private Shop HeroShop;
        private HeroClasses EnemyID;
        private double AbilityState;
        private double SelfWins;
        private double EnemyWins;
        public double BattleNumber;
        public ShopViewer()
        {
            
        }
        public void Update(Hero customer)
        {
            HeroStats = customer.Stats;
            HeroShop = customer.Shop;
            EnemyID = customer.EnemyId;
            AbilityState = customer.GetAbilityState();
            SelfWins = customer.TotalWins;
            EnemyWins = customer.EnemyWins;
        }

        public (Stats stats, Shop shop, HeroClasses enemyID, List<double> Other) Out() =>
            (HeroStats, HeroShop, EnemyID, new List<double> {BattleNumber, SelfWins, EnemyWins, AbilityState});
    }
}