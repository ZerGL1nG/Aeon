using System;
using Aeon.Core.GameProcess;

namespace Aeon.Agents.Console;

public class ConsoleBattleViewer: IBattleViewer
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

    public void OnBattleStart(BattleStart model) => OnBattleStartShow(model);
    public static void OnBattleStartShow(BattleStart model) => System.Console.WriteLine("Started Battle");

    public void OnAttack(BattleAttack model) => OnAttackShow(model);

    public static void OnAttackShow(BattleAttack model) => 
        System.Console.WriteLine($"Hp-{model.MineHealth,-4} " +
            $"(-{model.TakeDamage,-4}) Enemy-{model.EnemyHealth,-4} (-{model.GiveDamage,-4})");

    public void OnHeal(BattleHeal model) => OnHealShow(model);

    public static void OnHealShow(BattleHeal model) => 
        System.Console.WriteLine($"Hp-{model.MineHealth,-4} " +
            $"(+{model.MineHeal,-4}) Enemy-{model.EnemyHealth,-4} (+{model.EnemyHeal,-4})");

    public void OnBattleEnd(BattleEnd model) => OnBattleEndShow(model);

    public static void OnBattleEndShow(BattleEnd model)
    {
        if (model.Winner == 0)
            System.Console.WriteLine("Draw");
        else if (model.Winner == 1)
            System.Console.WriteLine("Win");
        else
            System.Console.WriteLine("Lose");
    }
}