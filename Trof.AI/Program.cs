global using static Tensorflow.Binding;
global using static Tensorflow.KerasApi;
using Aeon.Core;

namespace Trof.AI;

internal static partial class Program
{
    private const string BasePath = @"C:\Users\Tupotrof\source_x\NeuralShit\";
    internal const string Px = BasePath + "NN";
    
    private static void Main(string[] args)
    {
        //TestLogic();
        
        //TestBuildsGen("viable.txt", "all.txt");
        //TestBuilds("404.txt");
        
        //TestBuildsGen("top250-20.txt", "top250-20.txt");
        //TestBuilds("data_980.txt");
        //TestBuilds("data_230.txt", "data_20.txt");
        //TestBuilds("data_230.txt");
        //TestBuilds("data_20.txt", "data_20.txt");
        
        //TestBuildsGen("controltop20.txt", "controltop20.txt");
        //TestBuilds("data_20.txt");

        Console.WriteLine();
        Console.WriteLine("Press Esc to exit...");
        while (true) if (Console.ReadKey().Key == ConsoleKey.Escape) break;
    }
}