global using static Tensorflow.Binding;
global using static Tensorflow.KerasApi;
global using static Trof.AI.ActFunc;
global using Console = Colorful.Console;
using System.Drawing;
using Trof.AI.Env;

namespace Trof.AI;

internal static partial class Program
{
    private const string BasePath = @"D:\UsrFiles\AllPacks\P1\AI\";
    private const string Px = BasePath+"NN";
    private const string QPath = BasePath+"QAgents";

    private static Task _kach;
    private static CancellationTokenSource _exit = new();

    private static async Task Main(string[] args)
    {
        var aeonEnv = new AeonEnv(QPath);
        aeonEnv.AddAgents(8, n => new AeonAgent(QPath, n));
        _kach = aeonEnv.Kach(1000, _exit.Token);

        Console.WriteLine();
        Console.WriteLine("Press Esc to exit...", Color.Orange);
        while (true)
            if (Console.ReadKey().Key == ConsoleKey.Escape)
                break;
    }
}