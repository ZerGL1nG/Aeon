using System;
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
            {Stat.Income, new StatCosts(13, .02, 120, 0.2)},
            {Stat.Armor, new StatCosts(4, 2, 130, 80)},
            {Stat.Shield, new StatCosts(30, 15, 120, 66)},
            {Stat.Regen, new StatCosts(11, 5, 115, 62)},
        };
        
        private double CritShell { get; set; }
        protected double CurrentIncome { get; set; }
        public double CurrentHp { get; set; }

        public int RoundNumber { get; private set; } = 0;
        public int TotalWins { get; set; }
        public int EnemyWins => Enemy.TotalWins;

        public bool AutoLose { get; set; }

        public HeroClasses HeroClass { get; protected set; } = (HeroClasses) (-1);
        
        protected double Heal(double h)
        {
            var prevHp = CurrentHp;
            CurrentHp = Math.Min(CurrentHp + h, Stats.GetStat(Stat.Health));
            return CurrentHp - prevHp;
        }

        public Stats Stats;
        public Shop Shop;
        private Attack LastReceivedAttack { get; set; }

        private const double winBonus = 20;
        private const double gameMoney = 100;

        protected Hero Enemy { get; set; }
        public HeroClasses EnemyId => Enemy.HeroClass;

        private bool RandCrit(double critChance)
        {
            /*if (critChance <= 0)
                return false;
            
            if (CritShell > Rnd.NextDouble())
            {
                CritShell = critChance;
                return true;
            }

            CritShell += CritShell > critChance ? 0.05 : -0.05;
            return false;*/
            return critChance > Random.Shared.NextDouble();
        }

        public Hero()
        {
            Stats = Stats.Clone(new Stats(InitStats));
            Shop = Shop.Clone(new Shop(InitCosts));
        }

        public virtual Attack ReceiveAttack(Attack attack)
        {
            var armor = Stats.GetStat(Stat.Armor);
            var shield = Stats.CalculateShield(Stats.GetStat(Stat.Shield));

            var physDamage = Math.Max(0, attack.Damage * (1 - shield) - armor);
            LastReceivedAttack = new Attack(attack.Source, physDamage, attack.Magic, attack.True, attack.Critical);
            CurrentHp -= (physDamage + attack.Magic + attack.True);
            
            return LastReceivedAttack;
        }

        public virtual double TryRegen() => LastReceivedAttack.Damage > 0 ? Heal(Stats.GetStat(Stat.Regen)) : 0;


        public bool CheckDead()
        {
            if (CurrentHp > 0) return false;
            //Console.WriteLine("Я здох!");
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

        public virtual void StartBattle(Hero enemy)
        {
            TotalWins = 0;
            Enemy = enemy;
            Reset();
        }

        public virtual void EndBattle(bool win)
        {
            RoundNumber++;
            if (win) {
                TotalWins++;
                Stats.AddStat(Stat.Money, winBonus + gameMoney);
            }
            else Stats.AddStat(Stat.Money, gameMoney);
        }

        protected virtual void Reset()
        {
            CurrentHp = Stats.GetStat(Stat.Health);
            CurrentIncome = 1;
        }

        public bool CanBuy(Stat stat, bool opt)
        {
            var price = Shop.GetPrice(stat, opt);
            if (price.cost > Stats.GetStat(Stat.Money)) return false;
            if (stat == Stat.Shield && Stats.CalculateShield(Stats.GetStat(Stat.Shield)) >= Stats.maxShield)
                return false;
            return true;
        }
        
        public virtual bool TryToBuy(Stat stat, bool opt)
        {
            if (!CanBuy(stat, opt)) return false;
            var price = Shop.GetPrice(stat, opt);
            Stats.AddStat(Stat.Money, -price.cost);
            Stats.AddStat(stat, price.amount);
            return true;
        }

        public virtual bool CanUseAbility => false;
        public virtual bool UseAbility() => false;
        public virtual double GetAbilityState() => 0;

        public Hero Clone()
        {
            var hero = HeroMaker.Make(HeroClass);
            hero.Stats = Stats.Clone(Stats);
            hero.Shop = Shop.Clone(Shop);
            //hero.Enemy = Enemy;
            //hero.LastReceivedAttack = LastReceivedAttack;
            return hero;
        }
    }
}