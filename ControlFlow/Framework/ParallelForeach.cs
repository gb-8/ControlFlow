using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlFlow.Framework
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
            Results = results;
            Slots = slots;
            UnstartedBlocks = new ConcurrentQueue<IBlock>();
            foreach (var block in blocks)
            {
                UnstartedBlocks.Enqueue(block);
            }
        }

        public async Task<BlockExecutionResult> Execute()
        {
            Log.Logger.Debug("ParallelForeach: Executing.");
            var results = await Task.WhenAll(Slots.Select(async s => await s.Execute(UnstartedBlocks)));
            Log.Logger.Debug("ParallelForeach: execution results {results}", string.Join(", ", results.Select(r => r.Status)));
            var aggregateResult = BlockExecutionResult.Aggregate(results);
            Log.Logger.Debug("ParallelForeach: Aggregate execution result: {result}.", aggregateResult.Status);
            return aggregateResult;
        }

        public async Task<BlockExecutionResult> Handle(IMessage message)
        {
            Log.Logger.Debug("ParallelForeach: Handling message.");
            var results = await Task.WhenAll(Slots.Select(s => s.Handle(message, UnstartedBlocks)));
            Log.Logger.Debug("ParallelForeach: message handling results {results}", string.Join(", ", results.Select(r => r.Status)));
            var aggregateResult = BlockExecutionResult.Aggregate(results);
            Log.Logger.Debug("ParallelForeach: Aggregate message handling result: {result}.", aggregateResult.Status);
            return aggregateResult;
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
            Log.Logger.Debug("Slot: Executing slot {label}", label);
            var messages = new List<IMessage>();
            while (blocks.TryDequeue(out IBlock? nextBlock))
            {
                Log.Logger.Debug("Slot: Executing block in slot {label}", label);
                var result = await nextBlock.Execute();
                if (result.Status == ExecutionStatus.Complete)
                {
                    Log.Logger.Debug("Slot: block in slot {label} completed", label);
                    messages.AddRange(result.Messages);
                    results.Add(result);
                    executedBlocks.Add(nextBlock);
                }
                else if (result.Status == ExecutionStatus.Executing)
                {
                    Log.Logger.Debug("Slot: block in slot {label} still executing", label);
                    messages.AddRange(result.Messages);
                    currentBlock = nextBlock;
                    return BlockExecutionResult.Executing(result.Messages.ToArray());
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }

            Log.Logger.Debug("Slot: completed", label);
            return BlockExecutionResult.Succeeded(messages.ToArray());
        }

        public async Task<BlockExecutionResult> Handle(IMessage message, ConcurrentQueue<IBlock> blocks)
        {
            Log.Logger.Debug("Slot: handling message in slot {label}", label);
            if (currentBlock == null)
            {
                Log.Logger.Debug("Slot: no block, so must have completed in slot {label}", label);
                return BlockExecutionResult.Succeeded();
            }
            var result = await currentBlock.Handle(message);
            if (result.Status != ExecutionStatus.Complete)
            {
                Log.Logger.Debug("Slot: current block incomplete", label);
                return result;
            }

            results.Add(result);
            currentBlock = null;
            Log.Logger.Debug("Slot: block complete in slot {label}, so re-executing", label);
            return await Execute(blocks);
        }
    }
}
