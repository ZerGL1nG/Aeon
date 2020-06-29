using System;
using System.Collections.Generic;
using System.Linq;

namespace Aeon.Core.GameProcess.Agents
{
    public class NetworkAgent : IAgent
    {
        private Func<List<double>, int> _networkJob;
        
        public NetworkAgent(Func<List<double>, int> networkJob)
        {
            _networkJob = networkJob;
        }

        public BattleViewer BattleView { get; set; }
        public ShopViewer ShopView { get; set; }

        //private List<double> inputs;

        public Command ShopDecision()
        {
            var workResult = _networkJob(new List<double>
            {
                
            });
            return parseCommand(workResult);
        }

        private Command parseCommand(int result)
        {
            throw new System.NotImplementedException();
        }
        
    }
}