using AI.NeuralNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.Agents.Reinforcement
{
    internal class BatchingMemory
    {
        private int memorySize;
        private int batchSize;
        private bool newSession;
        private Queue<Sample> memory;
        
        public BatchingMemory(int memorySize = 10000, int batchSize = 2000)
        {
            memory = new Queue<Sample>();
            this.memorySize = memorySize;
            this.batchSize = batchSize;
            this.newSession = true;
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
