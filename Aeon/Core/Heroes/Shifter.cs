﻿namespace Aeon.Core.Heroes;

/// <summary>
///     Герой ранней стадии игры. В Магазине можно сбросить его
///     улучшения и увеличить количество игровой валюты на 80%
///     единиц валюты, потраченных на сброшенные улучшения.
/// </summary>
public class Shifter: Hero
{
    private const double ResetEfficiency = 0.8;

    private double MoneySpent;

    public Shifter() => throw new System.Exception();

    public override bool TryToBuy(Stat stat, bool opt)
    {
        if (!base.TryToBuy(stat, opt)) return false;
        MoneySpent += Shop.GetPrice(stat, opt).cost;
        return true;
    }

    public override bool UseAbility()
    {
        if (MoneySpent == 0) return false;
        Stats = new Stats(InitStats);
        Stats.AddStat(Stat.Money, MoneySpent*ResetEfficiency);
        MoneySpent = 0;
        return true;
    }

    public override double GetAbilityState() => MoneySpent*ResetEfficiency;
}