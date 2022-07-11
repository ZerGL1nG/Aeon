using System.Collections.Generic;
using System.Linq;
using Aeon.Agents.Console;
using Aeon.Core;
using Aeon.Core.GameProcess;
using Aeon.Core.Heroes;
using AI.NeuralNetwork;

namespace Aeon.Agents.Network;

public class NetworkShopViewer: IShopViewer, INetworkData
{
    private float _abilityState;
    private float _battleNumber;
    private HeroClasses _enemyId;
    private float _enemyWins;
    private Hero _hero;
    private Shop _heroShop;
    private Stats _heroStats;
    private float _selfWins;
    public int EnemyNumber => (int)_enemyId;
    public bool console;

    public NetworkShopViewer(bool console = false)
    {
        this.console = console;
    }
    private double Money => _heroStats.GetStat(Stat.Money);
    public float MoneyF => (float)Money;

    public INetworkData State => new ArrayData(Inputs);

    public IEnumerable<float> Inputs =>
        _heroStats is null
            ? new float[50]
            : _heroStats.OutFloatsActivated().Concat(_heroShop.OutActivated()).Concat(
                new List<float> {
                    StatConverters.Sigmoid(_battleNumber),
                    StatConverters.Sigmoid(_selfWins),
                    StatConverters.Sigmoid(_enemyWins),
                    StatConverters.Sigmoid(_abilityState),
                });

    public int Size { get; } = 50;

    public virtual void OnShopUpdate(Hero customer)
    {
        _hero         = customer;
        _heroStats    = customer.Stats;
        _heroShop     = customer.Shop;
        _enemyId      = customer.EnemyId;
        _abilityState = (float)customer.GetAbilityState();
        _selfWins     = customer.TotalWins;
        _enemyWins    = customer.EnemyWins;
        _battleNumber = customer.RoundNumber;
        if (console) ConsoleShopViewer.OnShopUpdateShow(customer);
    }

    public bool CanBuy(Command command)
    {
        var price = _heroShop.GetPrice(command.Type, command.Opt);
        return price.cost <= Money;
    }

    public bool LegalAction(Command cmd) => cmd.Exit || !((cmd.Ability && !_hero.CanUseAbility)
     || (cmd.Opt && _hero.RoundNumber == 0)
     || !_hero.CanBuy(cmd.Type, cmd.Opt));

    public bool WillExit(Command cmd) =>
        cmd.Exit || !LegalAction(cmd);

    public int GetRoundNumber() => _hero.RoundNumber;
    public float GetCost(Command command) => (float)_heroShop.GetPrice(command.Type, command.Opt).cost;

    public NetworkShopViewer Copy() =>
        new() {
            _heroStats    = Stats.Clone(_heroStats),
            _heroShop     = Shop.Clone(_heroShop),
            _enemyId      = _enemyId,
            _abilityState = _abilityState,
            _battleNumber = _battleNumber,
            _enemyWins    = _enemyWins,
            _selfWins     = _selfWins,
        };
}