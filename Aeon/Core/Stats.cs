using System;
using System.Collections.Generic;

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
        
        // ЩЩИИИИИИИИИТТТ!!!!!
        private const double shieldConst1 = 0.0075d;
        private const double shieldConst2 = 0.9;
        public static double CalculateShield(double coeff) =>
            shieldConst1 * coeff /
            (1 + shieldConst1 * Math.Pow(Math.E, shieldConst2 * Math.Log(shieldConst2)));
        
    }
}