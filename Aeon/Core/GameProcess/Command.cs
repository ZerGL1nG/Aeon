namespace Aeon.Core.GameProcess;

public class Command
{
    private Command(Stat type = default, bool opt = false, bool exit = false, bool ability = false)
    {
        Type    = type;
        Opt     = opt;
        Exit    = exit;
        Ability = ability;
    }

    public bool Opt { get; set; }
    public Stat Type { get; set; }
    public bool Exit { get; set; }
    public bool Ability { get; set; }

    public static Command Parse(int result) =>
        result < 18
            ? new Command((Stat)(result%9+1), result >= 9)
            : new Command(exit: result == 18, ability: result == 19);

    public override string ToString() =>
        Exit   ? "[Exit]" :
        Ability? "[Ability]" : $"{Type}{(Opt? " (opt)" : "")}";
}