using System;
using System.Collections.Generic;

namespace TanksGame.Core.Heroes
{
    public class Hero
    {
        private double CritShell { get; set; }
        private double ShieldCoeff { get; set; }
        private double CurrentIncome { get; set; }
        public double CurrentHp { get; set; }

        private Stats _stats;
        private Random Rnd { get; set; }
        private const double shieldConst1 = 0.0075d;
        private const double shieldConst2 = 0.9;
        private const double winBonus = 20;
        private const double gameMoney = 100;

        private bool RandCrit()
        {
            var cc = _stats.GetStat("CritChance");
            if (CritShell > Rnd.NextDouble())
            {
                CritShell = cc;
                return true;
            }
            CritShell += CritShell > cc ? 0.05 : -0.05;
            return false;
        }
        private double CalculateShield(double coeff) =>
            shieldConst1 * coeff /
            (1 + shieldConst1 * Math.Pow(Math.E, shieldConst2 * Math.Log(shieldConst2)));
        
        

        public Hero()
        {
            _stats = new Stats(new Dictionary<string, double>()
            {
                {@"Hp", 100},
                {@"Attack", 15},
                {@"Spell", 0},
                {@"CritChance", 0},
                {@"CritDamage", 1.5},
                {@"Income", 0},
                {@"Armor", 1},
                {@"Shield", 0},
                {@"Regen", 1},
                {@"Money", 100}
            });
        }
        
        public virtual void ReceiveAttack(Attack attack)
        {
            
        }

        public virtual Attack MakeAttack()
        {
            var attack = _stats.GetStat("Attack");
            var spell = _stats.GetStat("Spell");
            var cChance = _stats.GetStat("CritChance");
            var cDamage = _stats.GetStat("CritDamage");
            
            return new Attack();
        }

        public virtual void StartBattle()
        {
            Reset();
        }

        public virtual void EndBattle(bool win)
        { 
            if(win) _stats.AddStat("Money", winBonus + gameMoney);
            else _stats.AddStat("Money", gameMoney);
        }

        protected virtual void Reset()
        {
            CurrentHp = _stats.GetStat("Hp");
            CurrentIncome = 1;
        }
    }
}