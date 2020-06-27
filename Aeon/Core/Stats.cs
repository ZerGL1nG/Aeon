using System;
using System.Collections.Generic;

namespace TanksGame.Core
{
    public class Stats
    {
        public readonly Dictionary<String, double> data;
        
        
        
        
        
        private double _shieldCoeff;

        public Stats(Dictionary<string, double> data)
        {
            this.data = new Dictionary<string, double>(data);
        }

        public void SetStat(string name, double value) => data[name] = value;

        public void AddStat(string name, double value) => data[name] += value;

        public double GetStat(string name)
        {
            if (data.ContainsKey(name))
                return data[name];
            throw new ArgumentException($"No stat named {name} in stats");
        }
        
    }
}