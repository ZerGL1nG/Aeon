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
            if (LogState.Count == 0)
                MaxHP = (state.MyMaxHp, state.EnemyMaxHp);
            LogState.Add(state);
        }

        private void Draw()
        {
            throw new System.NotImplementedException();
        }

        public Dictionary<string, double> GetDict() => Out().ToList().ToDictionary(s=>s.Item1, s=>s.Item2);

        public IEnumerable<(string, double)> Out()
        {
            yield return ("EnemyMaxHP", MaxHP.Item2);
            foreach (var (i, s) in new[] {
                (1, "First"), (2, "Second"), (LogState.Count - 2, "PreLast"), (LogState.Count - 1, "Last")}) { // ЩИТО?!
                foreach (var (item1, item2) in UnpackBS(LogState[i])) {
                    yield return (item1 + "_" + s, item2);
                }
            }
        }

        public static IEnumerable<(string, double)> UnpackBS(BattleState state) // индусы!
        {
            yield return ("MyReceivedDamage", state.MyRecDmg);
            yield return ("MyRegeneration", state.MyRegen);
            yield return ("MyCurrentHP", state.MyCurHp);
            yield return ("EnemyReceivedDamage", state.EnemyRecDmg);
            yield return ("EnemyRegeneration", state.EnemyRegen);
            yield return ("EnemyCurrentHP", state.EnemyCurHp);
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