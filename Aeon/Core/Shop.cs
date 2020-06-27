using System.Collections.Generic;

namespace Aeon.Core
{

    public struct Price
    {
        public double cost;
        public double amount;

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
            standard.cost = std_cost;
            standard.amount = std_amount;
            discount.cost = opt_cost;
            discount.amount = opt_amount;
        }

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
        
        public Shop(Dictionary<Stat, StatCosts> costs) => Costs = costs;

        public Price GetPrice(Stat stat, bool opt) => 
            opt ? Costs[stat].discount : Costs[stat].standard;
    }
}