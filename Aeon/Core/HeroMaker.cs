using Aeon.Core.Heroes;

namespace Aeon.Core;

public enum HeroClasses
{
    Banker,
    Beast,
    Beggar,
    BloodyElf,
    Cheater,
    Fatty,
    Fe11,
    Killer,
    Master,
    Rogue,
    Shifter,
    Thief,
    Vampire,
    Warlock,
    Warrior,
}

public static class HeroMaker
{
    public const int TotalClasses = 15;

    public static Hero Make(HeroClasses @class) =>
        @class switch {
            HeroClasses.Banker    => new Banker(),
            HeroClasses.Beast     => new Beast(),
            HeroClasses.Beggar    => new Beggar(),
            HeroClasses.BloodyElf => new BloodyElf(),
            HeroClasses.Cheater   => new Cheater(),
            HeroClasses.Fatty     => new Fatty(),
            HeroClasses.Fe11      => new Fe11(),
            HeroClasses.Killer    => new Killer(),
            HeroClasses.Master    => new Master(),
            HeroClasses.Rogue     => new Rogue(),
            HeroClasses.Shifter   => new Shifter(),
            HeroClasses.Thief     => new Thief(),
            HeroClasses.Vampire   => new Vampire(),
            HeroClasses.Warlock   => new Warlock(),
            HeroClasses.Warrior   => new Warrior(),
            _                     => new Hero(),
        };
}