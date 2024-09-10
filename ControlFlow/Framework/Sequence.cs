using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ControlFlow.Framework
{
    public class Sequence : IBlock
    {
        public List<IBlock> Blocks { get; set; }

        public int ProgressIndex { get; set; }

        protected Sequence(List<IBlock> blocks, int progressIndex)
        {
            Blocks = blocks;
            ProgressIndex = ProgressIndex;
        }

        public static Sequence Create(params IBlock[] blocks) => new Sequence(blocks.ToList(), 0);

        public async Task<BlockExecutionResult> Execute() => await Execute(new List<IMessage>());


        public async Task<BlockExecutionResult> Execute(List<IMessage> messages)
        {
            Log.Logger.Debug("Sequence: executing.");
            await Task.CompletedTask;
            Log.Logger.Debug("Sequence: skipping to block {index}.", ProgressIndex);
            foreach (var block in Blocks.Skip(ProgressIndex))
            {
                Log.Logger.Debug("Sequence: executing block {index}.", ProgressIndex);
                var result = await block.Execute();
                messages.AddRange(result.Messages);
                if (result.Status == ExecutionStatus.Executing)
                {
                    Log.Logger.Debug("Sequence: block still executing.");
                    return BlockExecutionResult.Executing([.. messages]);
                }

                ++ProgressIndex;
                Log.Logger.Debug("Sequence: block execution complete, so bumped index to {index}", ProgressIndex);
            }

            return BlockExecutionResult.Succeeded([.. messages]);
        }

        public async Task<BlockExecutionResult> Handle(IMessage message)
        {
            Log.Logger.Debug("Sequence: handling message.");
            await Task.CompletedTask;
            var messages = new List<IMessage>();
            if (ProgressIndex >= Blocks.Count)
            {
                // Must have completed the sequence.
                Log.Logger.Debug("Sequence: index already passed end of sequence so ignoring message.");
                return BlockExecutionResult.Succeeded();
            }

            Log.Logger.Debug("Sequence: passing message to block {index}.", ProgressIndex);
            var result = await Blocks[ProgressIndex].Handle(message);

            if (result.Status == ExecutionStatus.Executing)
            {
                Log.Logger.Debug("Sequence: block still executing.");
                return result;
            }

            Log.Logger.Debug("Sequence: bumped index to {index} and re-eecute.", ProgressIndex);
            ++ProgressIndex;
            messages.AddRange(result.Messages);
            return await Execute(messages);
        }
    }
}
