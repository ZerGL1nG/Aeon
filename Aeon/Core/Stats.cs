using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aeon.Agents;

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
        public List<double> OutDoubles() => data.Select(o => o.Value).ToList();
        public List<float> OutFloats() => data.Select(o => (float) o.Value).ToList();
        public List<float> OutFloatsActivated() => 
            data.Select(o => StatConverters.Convert(o.Key, (float) o.Value)).ToList();

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

        public override bool Equals(object? obj) => obj is Stats s && Equals(s);

        public bool Equals(Stats other)
        {
            for (int i = 1; i <= 9; i++)
            {
                var s = (Stat)i;
                if (Math.Abs(data[s] - other.data[s]) > 0.001) return false;
            }
            return true;
        }

        public override string ToString()
        {
            var x = new StringBuilder();
            foreach (var (stat, value) in data) {
                x.Append($"{stat}: {Stats.RoundStat(value, stat)}; ");
            }
            return x.ToString();
        }

        public override int GetHashCode() => HashCode.Combine(
            HashCode.Combine(data[Stat.Health], data[Stat.Attack], data[Stat.Spell]),
            HashCode.Combine(data[Stat.CritChance], data[Stat.CritDamage], data[Stat.Income]),
            HashCode.Combine(data[Stat.Armor], data[Stat.Shield], data[Stat.Regen])
        );


        // ЩЩИИИИИИИИИТТТ!!!!!
        private const double shieldConst1 = 0.0075d;
        private const double shieldConst2 = 0.9;
        public const double maxShield = 0.95;
        
        public static double CalculateShield(double coeff) =>
            Math.Min(shieldConst1 * coeff / 
                     (1 + shieldConst1 * Math.Pow(Math.E, shieldConst2 * Math.Log(coeff))), maxShield);
        
    }
}