using System.Collections.Generic;

namespace Aeon.Core
{

    public struct Price
    {
        public double cost;
        public double amount;

        public Price(double c, double a)
        {
            cost = c;
            amount = a;
        }

        public void SetCost(double d) => cost = d;
        public void AddCost(double d) => cost += d;
        public void MulCost(double d) => cost *= d;
        public void SetAmount(double d) => amount = d;
        public void AddAmount(double d) => amount += d;
        public void MulAmount(double d) => amount *= d;
    }

    public struct StatCosts
    {
        public Price standard;
        public Price discount;

        public StatCosts(double std_cost, double std_amount, double opt_cost, double opt_amount)
        {
            standard = new Price(std_cost, std_amount);
            discount = new Price(opt_cost, opt_amount);
        }

        public double GetCost(bool opt) => opt ? discount.cost : standard.cost;
        public double GetAmount(bool opt) => opt ? discount.amount : standard.amount;

        public void AddCost(double d)
        {
            standard.cost += d;
            discount.cost += d;
        }

        public void MulCost(double d)
        {
            standard.cost *= d;
            discount.cost *= d;
        }

        public void AddAmount(double d)
        {
            standard.amount += d;
            discount.amount += d;
        }

        public void MulAmount(double d)
        {
            standard.amount *= d;
            discount.amount *= d;
        }
    }
    
    
    
    public class Shop
    {
        public Dictionary<Stat, StatCosts> Costs;
        
        public Shop(Dictionary<Stat, StatCosts> costs) => Costs = new Dictionary<Stat, StatCosts>(costs);

        public Price GetPrice(Stat stat, bool opt) => 
            opt ? Costs[stat].discount : Costs[stat].standard;

        public IEnumerable<double> Out()
        {
            for (var i = 1; i <= 9; i++) {
                var costs = Costs[(Stat) i];
                yield return costs.GetCost(false);
                yield return costs.GetAmount(false);
                yield return costs.GetCost(true);
                yield return costs.GetAmount(true);
            }
        }
    }
}