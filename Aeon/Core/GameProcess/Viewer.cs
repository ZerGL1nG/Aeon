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

        public IEnumerable<(StateParameter, double)> Out() => LogState.SelectMany(UnpackBs);

        public static IEnumerable<(StateParameter, double)> UnpackBs(BattleState state) // индусы!
        {
            foreach (var param in state.MyParams) yield return (param.Key, param.Value);
            foreach (var param in state.EnemyParams) yield return (param.Key, param.Value);
        }

        public void Reset() => LogState = new List<BattleState>();
    }

    public class ShopViewer
    {
        public ShopViewer()
        {
            
        }
        public void Update(Hero customer)
        {
            throw new System.NotImplementedException();
        }
    }
}