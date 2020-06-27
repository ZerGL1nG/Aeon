namespace Aeon.Core
{
    public class Attack
    {
        public readonly double Damage;
        public readonly double Magic;
        public readonly double True;
        public readonly bool Critical;
        public Attack(double damage = 0d, double magic = 0d, double @true = 0d, bool critical = false)
        {
            Damage = damage;
            Magic = magic;
            True = @true;
            Critical = critical;
        }

        public Attack Scale(double mult)
        {
            return new Attack(Damage * mult, Magic * mult, True * mult, Critical);
        }
    }
}