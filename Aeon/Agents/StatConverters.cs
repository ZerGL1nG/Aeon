using System;
using System.Collections.Generic;
using Aeon.Core;
using static System.MathF;

namespace Aeon.Agents;

public static class StatConverters
{
    private static readonly Dictionary<Stat, Func<float, float>> Convs = new()
    {
        [Stat.Money]      = x => Min(Atan(x * 0.01f), 1),
        [Stat.Health]     = x => AtanConv4(x * 0.01f),
        [Stat.Attack]     = x => AtanConv2(x * 0.05f),
        [Stat.Spell]      = x => AtanConv2(x * 0.05f),
        [Stat.CritChance] = x => x,
        [Stat.CritDamage] = x => Atan(x - 1),
        [Stat.Income]     = x => x,
        [Stat.Armor]      = x => AtanConv2(x * 0.05f),
        [Stat.Shield]     = x => (float) Stats.CalculateShield(x),
        [Stat.Regen]      = x => AtanConv2(x * 0.05f),
    };

    public static float Convert(Stat stat, float value) => Convs[stat](value);

    private static float AtanConv4(float x) => Atan(Sqrt(Sqrt(x)) - 1);
    private static float AtanConv2(float x) => Atan(Sqrt(x) - 1);

}