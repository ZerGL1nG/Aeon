using System;
using System.Collections.Generic;
using System.Linq;
using Aeon.Core;
using Aeon.Core.GameProcess;

namespace Aeon.Agents
{
    public class ConsoleAgent : IAgent
    {
        public IBattleViewer BattleView => _bw;
        private ConsoleBattleViewer _bw;
        public IShopViewer ShopView => _sw;
        private ConsoleShopViewer _sw;
        public bool IsBot => false;

        private bool IsChosen = false;
        //private bool EnterFlag;
        private HeroClasses myClass;

        //public Dictionary<(BattleViewer, ShopViewer), Command> DataSet 
         //   = new Dictionary<(BattleViewer, ShopViewer), Command>();
         
        public Dictionary<List<double>, List<double>> DataSet = new Dictionary<List<double>, List<double>>();

        public ConsoleAgent()
        {
            _bw = new ConsoleBattleViewer();
            _sw = new ConsoleShopViewer();
        }

        

        public Command ShopDecision()
        {
            while (true) {
                var key = KeysMap.GetIndex(Console.ReadKey().Key);
                if (key is null) continue;
                return Command.Parse(key.Value);
            }
        }



        public HeroClasses ChooseClass()
        {
            while (!IsChosen) {
                Console.Clear();
                for (int cl = 0; cl < HeroMaker.TotalClasses; ++cl)
                    Console.WriteLine($"Hero #{cl, 2}: {(HeroClasses)(cl)}");
                Console.Write("Выбранный герой: ");
                if (int.TryParse(Console.ReadLine(), out var num) && num < HeroMaker.TotalClasses && num >= 0) {
                    Console.WriteLine(num);
                    myClass = (HeroClasses) num;
                    IsChosen = true;
                }
                else {
                    Console.Error.WriteLine("Нет такого героя, соси");
                    Console.ReadLine();
                }
            }
            return myClass;
        }
    }
}