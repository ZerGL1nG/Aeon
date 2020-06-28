using System;
using System.Collections.Generic;

namespace Aeon.Core.GameProcess.Agents
{
    public class NetworkAgent : Agent
    {
        private Func<List<double>, int> _networkJob;
        
        public NetworkAgent(Func<List<double>, int> networkJob)
        {
            _networkJob = networkJob;
        }

        public override Command ShopDecision()
        {
            
            return base.ShopDecision();
        }
    }
}