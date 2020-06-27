using System;
using System.Collections.Generic;

namespace Aeon.Core.Heroes
{
    public class Hero
    {
        private double CritShell { get; set; }
        private double ShieldCoeff { get; set; }
        private double CurrentIncome { get; set; }
        public double CurrentHp { get; set; }

        protected Stats _stats;
        protected Shop _shop;
        private Random Rnd { get; set; }

        private const double winBonus = 20;
        private const double gameMoney = 100;

        private bool RandCrit(double critChance)
        {
            if (CritShell > Rnd.NextDouble())
            {
                CritShell = critChance;
                return true;
            }
            CritShell += CritShell > critChance ? 0.05 : -0.05;
            return false;
        }
        
        
        

        public Hero()
        {
            _stats = new Stats(new Dictionary<Stat, double>()
            {
                {Stat.Health, 100},
                {Stat.Attack, 15},
                {Stat.Spell, 0},
                {Stat.CritChance, 0},
                {Stat.CritDamage, 1.5},
                {Stat.Income, 0},
                {Stat.Armor, 1},
                {Stat.Shield, 0},
                {Stat.Regen, 1},
                {Stat.Money, 100}
            });

            _shop = new Shop(new Dictionary<Stat, StatCosts>() {
                {Stat.Health,     new StatCosts( 10,  22,  87, 220 )},
                {Stat.Attack,     new StatCosts(  7,   3, 120, 60  )},
                {Stat.Spell,      new StatCosts( 15,   7,  90, 46  )},
                {Stat.CritChance, new StatCosts( 15, .05, 104, .40 )},
                {Stat.CritDamage, new StatCosts( 50, .50, 105, 1.2 )},
                {Stat.Income,     new StatCosts( 13, .02, 120, .20 )},
                {Stat.Armor,      new StatCosts(  4,   2, 130, 80  )},
                {Stat.Shield,     new StatCosts( 30,  15, 120, 66  )},
                {Stat.Regen,      new StatCosts( 11,   5, 115, 62  )},
            });
        }
        
        public virtual bool ReceiveAttack(Attack attack)
        {
            var armor = _stats.GetStat(Stat.Armor);
            var shield = Stats.CalculateShield(_stats.GetStat(Stat.Shield));

            var physDamage = Math.Max(0, attack.Damage * (1 - shield) - armor);
            CurrentHp -= (physDamage + attack.Magic + attack.True);
            CheckDead();
            
            return (physDamage <= 0);
        }

        public virtual void TryRegen() => 
            CurrentHp = Math.Min(CurrentHp + _stats.GetStat(Stat.Regen), _stats.GetStat(Stat.Health));

        private bool CheckDead()
        {
            if (CurrentHp > 0) return false;
            Console.WriteLine("Я здох!");
            return true;
        }
        

        public virtual Attack MakeAttack()
        {
            var attack = _stats.GetStat(Stat.Attack);
            var spell = _stats.GetStat(Stat.Spell);
            var cChance = _stats.GetStat(Stat.CritChance);
            var cDamage = _stats.GetStat(Stat.CritDamage);

            return RandCrit(cChance) switch {
                true  => new Attack(attack * cDamage, spell),
                false => new Attack(attack, spell)
            };
        }

        public virtual void StartBattle()
        {
            Reset();
        }

        public virtual void EndBattle(bool win)
        { 
            if(win) _stats.AddStat(Stat.Money, winBonus + gameMoney);
            else _stats.AddStat(Stat.Money, gameMoney);
        }

        protected virtual void Reset()
        {
            CurrentHp = _stats.GetStat(Stat.Health);
            CurrentIncome = 1;
        }

        protected virtual bool TryToBuy(Stat stat, bool opt)
        {
            var price = _shop.GetPrice(stat, opt);
            if (price.cost > _stats.GetStat(Stat.Money)) return false;
            _stats.AddStat(Stat.Money, -price.cost);
            _stats.AddStat(stat, price.amount);
            return true;
        }
    }
}