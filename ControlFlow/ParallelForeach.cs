using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlFlow
{
    public class ParallelForeach : IBlock
    {
        public List<Slot> Slots { get; private set; }

        public ConcurrentQueue<IBlock> UnstartedBlocks { get; private set; }

        public List<BlockExecutionResult> Results { get; private set; }

        public int MaxParallelism { get; private set; }

        public static ParallelForeach Create(int maxParallelism, params IBlock[] blocks)
        {
            var slots = Enumerable.Range(1, maxParallelism).Select(i => Slot.Create(i));
            return new ParallelForeach(slots.ToList(), blocks.ToList(), new List<BlockExecutionResult>());
        }

        public ParallelForeach(List<Slot> slots, List<IBlock> blocks, List<BlockExecutionResult> results)
        {
            this.Results = results;
            this.Slots = slots;
            this.UnstartedBlocks = new ConcurrentQueue<IBlock>();
            foreach (var block in blocks)
            {
                this.UnstartedBlocks.Enqueue(block);
            }
        }

        public async Task<BlockExecutionResult> Execute()
        {
            var results = await Task.WhenAll(this.Slots.Select(async s => await s.Execute(this.UnstartedBlocks)));
            return BlockExecutionResult.Aggregate(results);
        }

        public async Task<BlockExecutionResult> Handle(IMessage message)
        {
            var results = await Task.WhenAll(this.Slots.Select(s => s.Handle(message, this.UnstartedBlocks)));
            return BlockExecutionResult.Aggregate(results);
        }
    }


    public class Slot
    {
        private readonly int label;
        public IBlock? currentBlock;
        public List<IBlock> executedBlocks;
        public List<BlockExecutionResult> results;

        public Slot(int label, IBlock? currentBlock, List<IBlock> executedBlocks, List<BlockExecutionResult> results)
        {
            this.label = label;
            this.currentBlock = currentBlock;
            this.executedBlocks = executedBlocks;
            this.results = results;
        }

        public static Slot Create(int label) => new Slot(label, null, new List<IBlock>(), new List<BlockExecutionResult>());

        public async Task<BlockExecutionResult> Execute(ConcurrentQueue<IBlock> blocks)
        {
            Console.WriteLine($"Executing slot {label}");
            bool executedSomething = false;
            var messages = new List<IMessage>();
            while (blocks.TryDequeue(out IBlock? nextBlock))
            {
                executedSomething = true;
                Console.WriteLine($"Executing block in slot {label}");
                var result = await nextBlock.Execute();
                if (result.Status == ExecutionStatus.Complete)
                {
                    messages.AddRange(result.Messages);
                    this.results.Add(result);
                    this.executedBlocks.Add(nextBlock);
                }
                else if (result.Status == ExecutionStatus.Executing)
                {
                    messages.AddRange(result.Messages);
                    currentBlock = nextBlock;
                    return BlockExecutionResult.Executing(result.Messages.ToArray());
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }

            return executedSomething
                ? BlockExecutionResult.Executing(messages.ToArray())
                : BlockExecutionResult.Succeeded(messages.ToArray());
        }

        public async Task<BlockExecutionResult> Handle(IMessage message, ConcurrentQueue<IBlock> blocks)
        {
            if (currentBlock == null) return BlockExecutionResult.Unhandled();
            var result = await currentBlock.Handle(message);
            if (result.Status != ExecutionStatus.Complete)
            {
                return result;
            }

            results.Add(result);
            currentBlock = null;
            return await Execute(blocks);
        }
    }
}
