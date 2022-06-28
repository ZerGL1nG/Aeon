using System.Diagnostics;
using System.Drawing;
using Aeon.Builds;
using Aeon.Core;

namespace Trof.AI;

// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
internal static partial class Program
{
    private static void TestLogic(Perceptron<LogicIn, LogicOut> network, int epochs = 1)
    {
        var timer = new Stopwatch();
        timer.Restart();
        var data = new List<(LogicIn, LogicOut)> {
            (new() { X = 0, Y = 0, Z = 0, }, new() { And = 0, Or = 0, Xor = 0, Ovf = 0, }),
            (new() { X = 1, Y = 0, Z = 0, }, new() { And = 0, Or = 1, Xor = 1, Ovf = 0, }),
            (new() { X = 0, Y = 1, Z = 0, }, new() { And = 0, Or = 1, Xor = 1, Ovf = 0, }),
            (new() { X = 1, Y = 1, Z = 0, }, new() { And = 0, Or = 1, Xor = 0, Ovf = 1, }),
            (new() { X = 0, Y = 0, Z = 1, }, new() { And = 0, Or = 1, Xor = 1, Ovf = 0, }),
            (new() { X = 1, Y = 0, Z = 1, }, new() { And = 0, Or = 1, Xor = 0, Ovf = 1, }),
            (new() { X = 0, Y = 1, Z = 1, }, new() { And = 0, Or = 1, Xor = 0, Ovf = 1, }),
            (new() { X = 1, Y = 1, Z = 1, }, new() { And = 1, Or = 1, Xor = 1, Ovf = 1, }),
        };

        var dataset = new TrofDataset<LogicIn, LogicOut>(data, 100);
        //var testset = new TrofDataset<LogicTestIn, LogicTestOut>(data);
        Console.WriteLine($"Dataset loaded in {timer.ElapsedMilliseconds} ms");
        timer.Restart();
        network.Train(dataset, epochs, 1);
        Console.WriteLine();

        Console.WriteLine($"Elapsed {timer.ElapsedMilliseconds} ms");

        foreach (var (logicTestIn, _) in data)
            Console.WriteLine($"{logicTestIn.DataP()} => {network.Call(logicTestIn).DataP()}");

        timer.Restart();
        for (var i = 0; i < 100; i++)
            foreach (var (logicTestIn, _) in data)
                _ = network.Call(logicTestIn);
        timer.Stop();
        Console.WriteLine($"Net called {100*data.Count} times: {timer.ElapsedMilliseconds} ms");
    }

    private static void TestBuildsGen(string file1, string file2)
    {
        var searcher = new Searcher();
        var baseHero = HeroMaker.Make(HeroClasses.Banker);
        searcher.AddClass(baseHero, "Basic");
        searcher.Init();
        searcher.MakeData(file1, file2);
    }

    private static void TrainBuilds(
        Perceptron<AeonPredIn, AeonPredOut> network, string datafile, int epochs = 10, int batchSize = -1)
    {
        var timer = new Stopwatch();
        timer.Start();
        var data = new List<(AeonPredIn, AeonPredOut)>();
        using (var reader = new StreamReader(File.OpenRead(Path.Combine(Searcher.clagen, datafile)))) {
            while (true) {
                var l1 = reader.ReadLine();
                var l2 = reader.ReadLine();
                if (l2 is null) break;
                var ain  = new AeonPredIn(l1!.Trim().Split(' ').Select(float.Parse));
                var aout = new AeonPredOut(l2!.Trim().Split(' ').Select(float.Parse));
                data.Add((ain, aout));
            }
        }
        var dataset = new TrofDataset<AeonPredIn, AeonPredOut>(data);
        Console.WriteLine($"Dataset loaded in {timer.ElapsedMilliseconds} ms");

        timer.Restart();
        network.Train(dataset, epochs, 1, batchSize);
        timer.Stop();

        Console.WriteLine();
        Console.WriteLine($"Elapsed {timer.ElapsedMilliseconds} ms");
    }

    private static void TestBuilds(INeuralEnv<AeonPredIn, AeonPredOut> network, string control, int pick = 10)
    {
        var contr = new List<(AeonPredIn, AeonPredOut)>();
        using (var reader = new StreamReader(File.OpenRead(Path.Combine(Searcher.clagen, control)))) {
            while (true) {
                var l1 = reader.ReadLine();
                var l2 = reader.ReadLine();
                if (l2 is null) break;
                var ain  = new AeonPredIn(l1!.Trim().Split(' ').Select(float.Parse));
                var aout = new AeonPredOut(l2!.Trim().Split(' ').Select(float.Parse));
                contr.Add((ain, aout));
            }
        }

        Console.WriteLine(@" __________________________________________", Color.LightGray);
        Console.Write("/        CONTROL : ", Color.LightGray);
        Console.WriteLine(control, Color.Aqua);

        for (var i = 0; i < pick; ++i) {
            var (input, output) = contr[Random.Shared.Next(contr.Count)];
            var call = network.Call(input);
            Console.WriteLine("| ", Color.LightGray);
            Console.WriteLine($"| Вход: {input.Stats()}", Color.LightGray);
            Console.WriteLine($"| Враг: {output}", Color.LightGray);
            Console.WriteLine($"| Сеть: {call}", Color.LightGray);
            LossOutput(call-output);
        }
        Console.WriteLine(@"\__________________________________________", Color.LightGray);

        void LossOutput(AeonPredOut @out)
        {
            Console.Write("| ", Color.LightGray);
            Console.Write("Мимо: ", Color.Orange);
            for (var i = 0; i < AeonPredOut.Size; ++i) {
                var error = @out.Floats[i];

                var color = Math.Abs(error) switch {
                    0    => Color.FromArgb(000, 255, 255),
                    < 1  => Color.FromArgb(000, 255, 000),
                    < 3  => Color.FromArgb(128, 255, 000),
                    < 5  => Color.FromArgb(192, 255, 000),
                    < 7  => Color.FromArgb(255, 255, 000),
                    < 10 => Color.FromArgb(255, 192, 000),
                    < 15 => Color.FromArgb(255, 128, 000),
                    _    => Color.FromArgb(255, 000, 000),
                };

                Console.Write($"{AeonPredOut.Names[i]}=", Color.LightGray);
                Console.Write($"{error,5:+##0.0;-##0.0;0.0}", color);
                Console.Write("; ", Color.LightGray);
            }
            Console.WriteLine();
        }
    }
}