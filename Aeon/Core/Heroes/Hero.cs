﻿using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Aeon.Core.Heroes
{
    public class Hero
    {
        public static readonly Dictionary<Stat, double> InitStats = new Dictionary<Stat, double> {
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
        };

        public static readonly Dictionary<Stat, StatCosts> InitCosts = new Dictionary<Stat, StatCosts> {
            {Stat.Health, new StatCosts(10, 22, 87, 220)},
            {Stat.Attack, new StatCosts(7, 3, 120, 60)},
            {Stat.Spell, new StatCosts(15, 7, 90, 46)},
            {Stat.CritChance, new StatCosts(15, .05, 104, .40)},
            {Stat.CritDamage, new StatCosts(50, .50, 105, 1.2)},
            {Stat.Income, new StatCosts(13, .02, 120, .20)},
            {Stat.Armor, new StatCosts(4, 2, 130, 80)},
            {Stat.Shield, new StatCosts(30, 15, 120, 66)},
            {Stat.Regen, new StatCosts(11, 5, 115, 62)},
        };
        
        private double CritShell { get; set; }
        private double ShieldCoeff { get; set; }
        protected double CurrentIncome { get; set; }
        public double CurrentHp { get; set; }
        protected void Heal(double h) => CurrentHp = Math.Min(CurrentHp + h, Stats.GetStat(Stat.Health));

        public Stats Stats;
        public Shop Shop;
        private Random Rnd { get; set; }

        private const double winBonus = 20;
        private const double gameMoney = 100;

        protected Hero Enemy { get; set; }

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


        public virtual void Init(Hero enemy)
        {
            Enemy = enemy;
        }

        public Hero()
        {
            Stats = new Stats(InitStats);
            Shop = new Shop(InitCosts);
        }
        
        public virtual Attack ReceiveAttack(Attack attack)
        {
            var armor = Stats.GetStat(Stat.Armor);
            var shield = Stats.CalculateShield(Stats.GetStat(Stat.Shield));

            var physDamage = Math.Max(0, attack.Damage * (1 - shield) - armor);
            CurrentHp -= (physDamage + attack.Magic + attack.True);
            
            return new Attack(attack.Source, physDamage, attack.Magic, attack.True, attack.Critical);
        }

        public virtual void TryRegen() => Heal(Stats.GetStat(Stat.Regen));
            

        public bool CheckDead()
        {
            if (CurrentHp > 0) return false;
            Console.WriteLine("Я здох!");
            return true;
        }
        

        public virtual Attack MakeAttack()
        {
            var attack = Stats.GetStat(Stat.Attack);
            var spell = Stats.GetStat(Stat.Spell);
            var cChance = Stats.GetStat(Stat.CritChance);
            var cDamage = Stats.GetStat(Stat.CritDamage);
            var cInc = Stats.GetStat(Stat.Income);
            var att =  RandCrit(cChance) switch {
                true  => new Attack(this, attack * cDamage, spell),
                false => new Attack(this, attack, spell)
            };
            CurrentIncome *= (1 + cInc);
            return att;
        }

        public virtual void StartBattle()
        {
            Reset();
        }

        public virtual void EndBattle(bool win)
        { 
            if(win) Stats.AddStat(Stat.Money, winBonus + gameMoney);
            else Stats.AddStat(Stat.Money, gameMoney);
        }

        protected virtual void Reset()
        {
            CurrentHp = Stats.GetStat(Stat.Health);
            CurrentIncome = 1;
        }

        public virtual bool TryToBuy(Stat stat, bool opt)
        {
            var price = Shop.GetPrice(stat, opt);
            if (price.cost > Stats.GetStat(Stat.Money)) return false;
            Stats.AddStat(Stat.Money, -price.cost);
            Stats.AddStat(stat, price.amount);
            return true;
        }

        public virtual void UseAbility() { }
    }
}