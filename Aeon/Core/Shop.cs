using System.Collections.Generic;
using System.Linq;
using Aeon.Agents;

namespace Aeon.Core;

public class Price
{
    public double amount;
    public double cost;

    public Price(double c, double a)
    {
        cost   = c;
        amount = a;
    }

    public void SetCost(double d) => cost = d;

    public void AddCost(double d) => cost += d;

    public void MulCost(double d) => cost *= d;

    public void SetAmount(double d) => amount = d;

    public void AddAmount(double d) => amount += d;

    public void MulAmount(double d) => amount *= d;
}

public class StatCosts
{
    public Price discount;
    public Price standard;

    public StatCosts(double std_cost, double std_amount, double opt_cost, double opt_amount)
    {
        standard = new Price(std_cost, std_amount);
        discount = new Price(opt_cost, opt_amount);
    }

    public double GetCost(bool opt) => opt? discount.cost : standard.cost;

    public double GetAmount(bool opt) => opt? discount.amount : standard.amount;

    public void AddCost(double d)
    {
        standard.AddCost(d);
        discount.AddCost(d);
    }

    public void MulCost(double d)
    {
        standard.MulCost(d);
        discount.MulCost(d);
    }

    public void AddAmount(double d)
    {
        standard.AddAmount(d);
        discount.AddAmount(d);
    }

    public void MulAmount(double d)
    {
        standard.MulAmount(d);
        discount.MulAmount(d);
    }
}

public class Shop
{
    public Dictionary<Stat, StatCosts> Costs;

    public Shop(Dictionary<Stat, StatCosts> costs) => Costs = new Dictionary<Stat, StatCosts>(costs);

    public Price GetPrice(Stat stat, bool opt) => opt? Costs[stat].discount : Costs[stat].standard;

    public static Shop Clone(Shop shop) =>
        new(shop.Costs.ToDictionary(cost => cost.Key,
                                    cost => new StatCosts(cost.Value.standard.cost, cost.Value.standard.amount,
                                                          cost.Value.discount.cost, cost.Value.discount.amount)));

    public int RoundBonus(double startValue, Stat stat, bool opt) =>
        opt
            ? Stats.RoundStat(Costs[stat].discount.amount+startValue, stat)-Stats.RoundStat(startValue, stat)
            : Stats.RoundStat(Costs[stat].standard.amount+startValue, stat)-Stats.RoundStat(startValue, stat);

    public IEnumerable<float> Out()
    {
        for (var i = 1; i <= 9; i++) {
            var costs = Costs[(Stat)i];
            yield return (float)costs.GetCost(false);
            yield return (float)costs.GetAmount(false);
            yield return (float)costs.GetCost(true);
            yield return (float)costs.GetAmount(true);
        }
    }

    public IEnumerable<float> OutActivated()
    {
        for (var i = 1; i <= 9; i++) {
            var st    = (Stat)i;
            var costs = Costs[st];
            yield return StatConverters.Convert(Stat.Money, (float)costs.GetCost(false));
            yield return StatConverters.Convert(st, (float)costs.GetAmount(false));
            yield return StatConverters.Convert(Stat.Money, (float)costs.GetCost(true));
            yield return StatConverters.Convert(st, (float)costs.GetAmount(true));
        }
    }
}