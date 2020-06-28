using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Aeon.Core.Heroes;

namespace Aeon.Core
{
    public class Attack
    {
        public readonly double Damage;
        public readonly double Magic;
        public readonly double True;
        public readonly bool Critical;
        public readonly Hero Source;
        public Attack(Hero source, double damage = 0d, double magic = 0d, double @true = 0d, bool critical = false)
        {
            Damage = damage;
            Magic = magic;
            True = @true;
            Critical = critical;
            Source = source;
        }

        public Attack Scale(double mult)
        {
            return new Attack(Source, Damage * mult, Magic, True, Critical);
        }
        
        public Attack ScaleAll(double mult)
        {
            return new Attack(Source, Damage * mult, Magic * mult, True * mult, Critical);
        }

        public double Sum() => (Damage + Magic + True);

    }
}