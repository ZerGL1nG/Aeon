using System.Diagnostics;
using Aeon.Builds;
using Aeon.Core;

namespace Trof.AI;

// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident

internal static partial class Program
{
    private static void TestLogic()
    {
        var timer = new Stopwatch();
        timer.Start();
        var network = new Perceptron<LogicTestIn, LogicTestOut>(8, 8);

        Console.WriteLine($"Network loaded in {timer.ElapsedMilliseconds} ms");
        timer.Restart();
        var data = new List<(LogicTestIn, LogicTestOut)>
        {
            (new(){X=0, Y=0, Z=0}, new(){And=0, Or=0, Xor=0, Ovf=0}),
            (new(){X=1, Y=0, Z=0}, new(){And=0, Or=1, Xor=1, Ovf=0}),
            (new(){X=0, Y=1, Z=0}, new(){And=0, Or=1, Xor=1, Ovf=0}),
            (new(){X=1, Y=1, Z=0}, new(){And=0, Or=1, Xor=0, Ovf=1}),
            (new(){X=0, Y=0, Z=1}, new(){And=0, Or=1, Xor=1, Ovf=0}),
            (new(){X=1, Y=0, Z=1}, new(){And=0, Or=1, Xor=0, Ovf=1}),
            (new(){X=0, Y=1, Z=1}, new(){And=0, Or=1, Xor=0, Ovf=1}),
            (new(){X=1, Y=1, Z=1}, new(){And=1, Or=1, Xor=1, Ovf=1}),
        };

        var dataset = new TrofDataset<LogicTestIn, LogicTestOut>(data, 1000);
        var dataset2 = new TrofDataset<LogicTestIn, LogicTestOut>(data, 1);
        //var testset = new TrofDataset<LogicTestIn, LogicTestOut>(data);
        Console.WriteLine($"Dataset loaded in {timer.ElapsedMilliseconds} ms");
        timer.Restart();
        network.Train(dataset, 10, 1);
        Console.WriteLine();
        
        Console.WriteLine($"Elapsed {timer.ElapsedMilliseconds} ms");
        
        foreach (var (logicTestIn, _) in data)
            Console.WriteLine($"{logicTestIn.DataP()} => {network.Call(logicTestIn).DataP()}");
    }

    private static void TestBuildsGen(string file1, string file2)
    {
        var searcher = new Searcher();
        var baseHero = HeroMaker.Make(HeroClasses.Banker);
        searcher.AddClass(baseHero, "Basic");
        searcher.Init();
        searcher.MakeData(file1, file2);
    }

    private static void TestBuilds(string datafile, string? control = null)
    {
        var timer = new Stopwatch();
        timer.Start();
        var network = new Perceptron<AeonTestIn, AeonTestOut>
            (ActFunc.ReLu, (100, ActFunc.ReLu), (70, ActFunc.ReLu), (40, ActFunc.ReLu));
        Console.WriteLine($"Network loaded in {timer.ElapsedMilliseconds} ms");
        var data = new List<(AeonTestIn, AeonTestOut)>();
        timer.Restart();
        using (var reader = new StreamReader(File.OpenRead(Path.Combine(Searcher.clagen, datafile))))
        {
            while (true)
            {
                var l1 = reader.ReadLine();
                var l2 = reader.ReadLine();
                if (l2 is null) break;
                var ain = new AeonTestIn(l1!.Trim().Split(' ').Select(float.Parse));
                var aout = new AeonTestOut(l2!.Trim().Split(' ').Select(float.Parse));
                data.Add((ain, aout));
            }
        }
        var dataset = new TrofDataset<AeonTestIn, AeonTestOut>(data, 1);
        var testdata = new TrofDataset<AeonTestIn, AeonTestOut>(data.Take(100), 1);
        network.Init(testdata, Px, "govno");
        Console.WriteLine($"Dataset loaded in {timer.ElapsedMilliseconds} ms");
        
        timer.Stop();
        var tcx = 50;
        while (true)
        {
            network.Train(dataset, 50, 1, 1000, tcx);
            tcx += 50;
        }

        timer.Stop();
        
        Console.WriteLine();
        Console.WriteLine($"Elapsed {timer.ElapsedMilliseconds} ms");
        
        if (control is null) return;
        var contr = new List<(AeonTestIn, AeonTestOut)>();
        using (var reader = new StreamReader(File.OpenRead(Path.Combine(Searcher.clagen, control))))
        {
            while (true)
            {
                var l1 = reader.ReadLine();
                var l2 = reader.ReadLine();
                if (l2 is null) break;
                var ain = new AeonTestIn(l1!.Trim().Split(' ').Select(float.Parse));
                var aout = new AeonTestOut(l2!.Trim().Split(' ').Select(float.Parse));
                contr.Add((ain, aout));
            }
        }

        foreach (var (aeonTestIn, aeonTestOut) in contr)
        {
            Console.WriteLine();
            Console.WriteLine($"Было: {network.Call(aeonTestIn)}");
            Console.WriteLine($"Надо: {aeonTestOut}");
        }
        
        

        //for (int i = 0; i < 30; i++)
        //{
        //    var (aeonIn, aeonOut) = data[Random.Shared.Next(data.Count)];
        //    Console.WriteLine();
        //    Console.WriteLine($"Было: {network.Call(aeonIn)}");
        //    Console.WriteLine($"Надо: {aeonOut}");
        //}
    }
}