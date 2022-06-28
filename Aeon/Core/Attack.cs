using Aeon.Core.Heroes;

namespace Aeon.Core;

public class Attack
{
    public readonly bool Critical;
    public readonly double Damage;
    public readonly double Magic;
    public readonly Hero Source;
    public readonly double True;

    public Attack(Hero source, double damage = 0d, double magic = 0d, double @true = 0d, bool critical = false)
    {
        Damage   = damage;
        Magic    = magic;
        True     = @true;
        Critical = critical;
        Source   = source;
    }

    public Attack Scale(double mult) => new Attack(Source, Damage*mult, Magic, True, Critical);

    public Attack ScaleAll(double mult) => new Attack(Source, Damage*mult, Magic*mult, True*mult, Critical);

    public double Sum() => Damage+Magic+True;
}