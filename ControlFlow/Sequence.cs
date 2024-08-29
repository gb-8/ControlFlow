using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlFlow
{
    public class Sequence : IBlock
    {
        public List<IBlock> Blocks { get; set; }

        public int ProgressIndex { get; set; }

        private Sequence(List<IBlock> blocks, int progressIndex)
        {
            this.Blocks = blocks;
            this.ProgressIndex = ProgressIndex;
        }

        public static Sequence Create(params IBlock[] blocks) => new Sequence(blocks.ToList(), 0);

        public async Task<BlockExecutionResult> Execute() => await Execute(new List<IMessage>());
        

        private async Task<BlockExecutionResult> Execute(List<IMessage> messages)
        {
            await Task.CompletedTask;
            foreach (var block in this.Blocks.Skip(this.ProgressIndex))
            {
                var result = await block.Execute();
                messages.AddRange(result.Messages);
                if (result.Status == ExecutionStatus.Executing)
                {
                    return BlockExecutionResult.Executing([..messages]);
                }

                if (result.Status == ExecutionStatus.Unexecuted)
                {
                    throw new InvalidOperationException("Unexpected status: block state should never be unexecuted after Execute is called.");
                }

                ++this.ProgressIndex;
            }

            return BlockExecutionResult.Succeeded([.. messages]);
        }

        public async Task<BlockExecutionResult> Handle(IMessage message)
        {
            await Task.CompletedTask;
            var messages = new List<IMessage>();
            var result = await this.Blocks[ProgressIndex].Handle(message);
            if (result.Status == ExecutionStatus.Unexecuted)
            {
                // Do we need to distinguish unexecuted vs message ignored?
                return result;
            }
            
            if (result.Status == ExecutionStatus.Executing)
            {
                return result;
            }

            ++this.ProgressIndex;
            messages.AddRange(result.Messages);
            return await Execute(messages);
        }
    }
}
