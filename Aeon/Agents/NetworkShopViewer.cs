using System.Collections.Generic;
using System.Linq;
using Aeon.Core;
using Aeon.Core.GameProcess;
using Aeon.Core.Heroes;
using AI.NeuralNetwork;

namespace Aeon.Agents
{
    public class NetworkShopViewer : IShopViewer, INetworkInput
    {
        private Stats _heroStats;
        private Shop _heroShop;
        private HeroClasses _enemyId;
        private float _abilityState;
        private float _selfWins;
        private float _enemyWins;
        private float _battleNumber;
        public int EnemyNumber => (int)_enemyId;

        public void OnShopUpdate(Hero customer)
        {
            _heroStats = customer.Stats;
            _heroShop = customer.Shop;
            _enemyId = customer.EnemyId;
            _abilityState = (float) customer.GetAbilityState();
            _selfWins = customer.TotalWins;
            _enemyWins = customer.EnemyWins;
            _battleNumber = customer.RoundNumber;
        }

        public NetworkShopViewer Copy() => new NetworkShopViewer {
            _heroStats = Stats.Clone(_heroStats), _heroShop = Shop.Clone(_heroShop), _enemyId = _enemyId, 
            _abilityState = _abilityState, _battleNumber = _battleNumber, _enemyWins = _enemyWins, _selfWins = _selfWins
        };

        public IEnumerable<float> Inputs =>
            _heroStats.OutDoubles().Select(x => (float)x)
                .Concat(new List<float> { _battleNumber, _selfWins, _enemyWins, _abilityState });
        public int Size { get; }
    }
}