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
            Network.Work(MakeInput(this));
            var workResult = Network.GetMaxOutId();
            return parseCommand(workResult);
        }

        public HeroClasses ChooseClass()
        {
            if ((int) _myClass == -1) return (HeroClasses) Program.rnd.Next(HeroMaker.TotalClasses);
            return _myClass;
        }

        public static Command parseCommand(int result) => result < 18 
                ? new Command((Stat)(result % 9 + 1), result >= 9) 
                : new Command(exit: result == 18, ability: result == 19);

        public static List<double> DeepArse(Command command)
        {
            int comNumber = 0;
            if (command.Ability) comNumber = 19;
            else if (command.Exit) comNumber = 18;
            else comNumber = (int) command.Type + (command.Opt ? 9 : 0);
            return MakeCrap(comNumber);
        }

        public static List<double> MakeCrap(int whereIsCrap)
        {
            var list = new List<double>();
            for (int i = 0; i <= 19; i++) 
                list.Add(0);
            list[whereIsCrap] = 1;
            return list;
        }

        public static List<double> MakeInput(IAgent agent)
        {
            var (stats, shop, enemyId, otherStates) = agent.ShopView.Out();
            var battle = agent.BattleView.Out().ToList();

            var enemyList = new List<double>();
            for (var i = 0; i < HeroMaker.TotalClasses; i++)
                enemyList.Add(0);
            enemyList[(int) enemyId] = 1;

            var battleList = hueta(battle);

            var statsList = stats.Out().Select(x => x.Item2);
            
            var shopList = shop.Out();

            return statsList.Concat(enemyList).Concat(battleList).Concat(shopList).Concat(otherStates).ToList();
        }

        private static IEnumerable<double> hueta(List<IEnumerable<(StateParameter, bool, double)>> bred)
        {
            switch (bred.Count) {
                case 0:
                    for (int i = 0; i < 25; i++) 
                        yield return 0;
                    break;
                case 1:
                    yield return bred[0].First(d => d.Item1 == StateParameter.MaxHp).Item3;
                    foreach (var v in bred[0])
                        if (v.Item1 != StateParameter.MaxHp)
                            yield return v.Item3;
                    for (int i = 0; i < 12; i++) 
                        yield return 0;
                    foreach (var v in bred[0])
                        if (v.Item1 != StateParameter.MaxHp)
                            yield return v.Item3;
                    break;
                case 2:
                    yield return bred[0].First(d => d.Item1 == StateParameter.MaxHp).Item3;
                    foreach (var v in bred[0])
                        if (v.Item1 != StateParameter.MaxHp)
                            yield return v.Item3;
                    foreach (var v in bred[1])
                        if (v.Item1 != StateParameter.MaxHp)
                            yield return v.Item3;
                    foreach (var v in bred[^2])
                        if (v.Item1 != StateParameter.MaxHp)
                            yield return v.Item3;
                    foreach (var v in bred[^1])
                        if (v.Item1 != StateParameter.MaxHp)
                            yield return v.Item3;
                    break;
            }
        }
        
    }
}