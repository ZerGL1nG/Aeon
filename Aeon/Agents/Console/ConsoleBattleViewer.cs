using Aeon.Core.GameProcess;

namespace Aeon.Agents.Console;

public class ConsoleBattleViewer : IBattleViewer
{
    /*
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
        
        public BattleViewer Copy() => new BattleViewer {LogState = LogState, MaxHP = MaxHP};
        */
        
     //       if (customer.RoundNumber != 0) {
     //       Console.WriteLine($"Бой #{customer.RoundNumber}:");
     //       foreach (var turn in BattleView.Out()) {
     //           var t = turn.ToList();
     //           Console.WriteLine(
     //               $"   ТЫ: {(int) t[1].value,5}/{(int) t[0].value,-5}-{(int) t[2].value,-5}+{(int) t[3].value,-5}" +
     //               $"   ВРАГ: {(int) t[5].value,5}/{(int) t[4].value,-5}-{(int) t[6].value,-5}+{(int) t[7].value,-5}");
     //       }
     //       Console.WriteLine();
     //   }    
        
    public void OnBattleStart(BattleStart model)
    {
        throw new System.NotImplementedException();
    }

    public void OnAttack(BattleAttack model)
    {
        throw new System.NotImplementedException();
    }

    public void OnHeal(BattleHeal model)
    {
        throw new System.NotImplementedException();
    }

    public void OnBattleEnd(BattleEnd model)
    {
        throw new System.NotImplementedException();
    }
}