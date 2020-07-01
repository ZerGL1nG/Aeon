using System;
using System.Collections.Generic;
using System.Linq;

namespace Aeon.Core
{
    public enum Stat
    {
        Money,
        Health,
        Attack,
        Spell,
        CritChance,
        CritDamage,
        Income,
        Armor,
        Shield, // !!КОЭФФИЦИЕНТ ЩИТА!!
        Regen,
    }
    
    public class Stats
    {
        public IEnumerable<(string, double)> Out() => data.Select(o => (o.Key.ToString(), o.Value));

        private readonly Dictionary<Stat, double> data;

        public Stats(Dictionary<Stat, double> data)
        {
            this.data = new Dictionary<Stat, double>(data);
        }

        public void SetStat(Stat stat, double value) => data[stat] = value;

        public void AddStat(Stat stat, double value) => data[stat] += value;
        public void MulStat(Stat stat, double value) => data[stat] *= value;

        public double GetStat(Stat stat)
        {
            if (data.ContainsKey(stat))
                return data[stat];
            throw new ArgumentException($"No stat named {stat} in stats");
        }

        public static Stats Clone(Stats stats) => new Stats(new Dictionary<Stat, double>(stats.data));

        public static int RoundStat(double value, Stat stat) => stat switch {
                Stat.CritChance => (int) (value * 100),
                Stat.CritDamage =>(int) (value * 100),
                Stat.Income => (int) (value * 100),
                Stat.Shield => (int) (CalculateShield(value) * 100),
                _ => (int) value
            };

        
        // ЩЩИИИИИИИИИТТТ!!!!!
        private const double shieldConst1 = 0.0075d;
        private const double shieldConst2 = 0.9;
        public const double maxShield = 0.95;
        
        public static double CalculateShield(double coeff) =>
            Math.Min(shieldConst1 * coeff / 
                     (1 + shieldConst1 * Math.Pow(Math.E, shieldConst2 * Math.Log(coeff))), maxShield);
        
    }
}