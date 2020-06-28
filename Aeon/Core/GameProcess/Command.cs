namespace Aeon.Core.GameProcess
{
    public class Command
    {
        public bool Opt { get; set; }
        public Stat Type { get; set; }
        public bool Exit { get; set; }
        public bool Ability { get; set; }
        public Command(Stat type = default, bool opt = false, bool exit = false, bool ability = false)
        {
            Type = type;
            Opt = opt;
            Exit = exit;
            Ability = ability;
        }
    }
}