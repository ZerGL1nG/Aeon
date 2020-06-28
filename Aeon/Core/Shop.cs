using System.Collections.Generic;

namespace Aeon.Core
{

    public class Price
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

    public class StatCosts
    {
        public Price standard;
        public Price discount;

        public StatCosts(double std_cost, double std_amount, double opt_cost, double opt_amount)
        {
            standard = new Price(std_cost, std_amount);
            discount = new Price(opt_cost, opt_amount);
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