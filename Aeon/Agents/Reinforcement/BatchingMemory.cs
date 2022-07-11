using AI.NeuralNetwork;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.Agents.Reinforcement
{
    public class BatchingMemory
    {
        public int memorySize;
        private int batchSize;
        private bool newSession;
        public Queue<Sample> memory;
        
        public BatchingMemory(int memorySize = 10000, int batchSize = 2000)
        {
            memory = new Queue<Sample>();
            this.memorySize = memorySize;
            this.batchSize = batchSize;
            this.newSession = true;
        }

        public BatchingMemory(BatchingMemory source)
        {
            this.memory = new Queue<Sample>(source.memory);//семпл остается тот же - может быть пиздец памятей XD
            this.memorySize = source.memorySize;
            this.batchSize = source.batchSize;
            this.newSession = source.newSession;
        }

        public void Add(INetworkData State, int Action)
        {
            if (newSession) newSession = false;
            else memory.Last().NextState = State;
            memory.Enqueue(new Sample(State, Action, 0));
            if (memory.Count > memorySize) memory.Dequeue();
        }

        public void Reward(float reward, bool terminal = false)
        {
            newSession = terminal;
            memory.Last().Reward = reward;
        }

        public List<Sample> MakeBatch()
        {
            List<Sample> samples = new(memory);
            for (var x = 0; x < Math.Min(batchSize, memory.Count); ++x)
            {
                var pos = Random.Shared.Next(x, samples.Count - 1);
                (samples[pos], samples[x]) = (samples[x], samples[pos]);
            }

            return samples.Take(Math.Min(batchSize, memory.Count)).ToList();
        }

        public void SaveFile(string dir)
        {
            using var file = File.CreateText(dir);
            file.WriteLine($"MEM {memorySize} {batchSize} {newSession} {memory.Count}");
            foreach (var sample in memory)
            {
                file.WriteLine(
                    $"{Unroll(sample.State.Inputs)};{sample.Action};{sample.Reward};{Unroll(sample.NextState?.Inputs)}");
            }
            file.Close();

            string Unroll(IEnumerable<float>? values)
            {
                if (values is null) return "<>";
                var cx = new StringBuilder();
                foreach (var value in values)
                    cx.Append($"{value},");
                cx.Remove(cx.Length - 1, 1);
                return cx.ToString();
            }
        }

        public void LoadFile(string dir)
        {
            using var file = new StreamReader(File.OpenRead(dir));
            var x0 = file.ReadLine()!.Split(' ');
            memorySize = int.Parse(x0[1]);
            batchSize = int.Parse(x0[2]);
            newSession = bool.Parse(x0[3]);
            memory = new Queue<Sample>(int.Parse(x0[4]));
            while (true)
            {
                var cx = file.ReadLine()?.Split(';');
                if (cx is null) break;
                var sample = new Sample(Roll(cx[0]), int.Parse(cx[1]),
                    float.Parse(cx[2]), Roll(cx[3]));
                memory.Enqueue(sample);
            }
            file.Close();

            INetworkData? Roll(string s) => s switch
            {
                "<>" => null,
                _ => new ArrayData(s.Split(',').Select(float.Parse)),
            };
        }

    }



    public class Sample
    {
        public INetworkData State { get; private set; }
        public int Action { get; set; }
        public float Reward { get; set; }
        public INetworkData NextState { get; set; }
        public Sample(INetworkData State, int Action, float Reward, INetworkData NextState=null) {
            this.State = State;
            this.Action = Action;
            this.Reward = Reward;
            this.NextState = NextState;
        }
    }
}
