using ControlFlow.Framework;
using Serilog;

namespace ControlFlow.Domain
{
    class ExecutionController
    {
        public bool PreStepExecutionComplete { get; private set; }
        public bool StepExecutionComplete { get; private set; }
        public IBlock StepExecutionsBlock { get; private set; }

        public ExecutionController(IBlock stepExecutionsBlock)
        {
            StepExecutionsBlock = stepExecutionsBlock;
        }

        public static ExecutionController Create(Plan plan, Func<Step, IBlock> stepExecutionFactory)
        {
            var stepExecutions = Sequence.Create(
                plan.StepBatches
                    .Select(batch => ParallelForeach.Create(
                        3,
                        batch.Select(stepExecutionFactory)
                        .ToArray()))
                    .ToArray());
            return new ExecutionController(stepExecutions);
        }

        public async Task<BlockExecutionResult> Execute() => await Execute(new List<IMessage>());

        public async Task<BlockExecutionResult> Handle(IMessage message)
        {
            await Task.CompletedTask;
            var messages = new List<IMessage>();
            Log.Logger.Debug("Passing message to step execution block.");
            var result = await StepExecutionsBlock.Handle(message);
            if (result.Status == ExecutionStatus.Executing)
            {
                Log.Logger.Debug("Step execution block still executing.");
                return result;
            }

            Log.Logger.Debug("Step execution block completed.");
            StepExecutionComplete = true;
            messages.AddRange(result.Messages);
            return await Execute(messages);
        }

        private async Task<BlockExecutionResult> Execute(List<IMessage> messages)
        {
            if (!PreStepExecutionComplete)
            {
                PreStepExecution();
                PreStepExecutionComplete = true;
            }
            else
            {
                Log.Logger.Debug("Skipping pre-step-execution.");
            }

            if (!StepExecutionComplete)
            {
                var result = await StepExecutionsBlock.Execute();
                messages.AddRange(result.Messages);
                if (result.Status == ExecutionStatus.Executing)
                {
                    return BlockExecutionResult.Executing([.. messages]);
                }
            }
            else
            {
                Log.Logger.Debug("Skipping step-execution.");
            }

            PostStepExecution();
            return BlockExecutionResult.Succeeded([.. messages]);

        }

        private void PreStepExecution()
        {
            Console.WriteLine(nameof(PreStepExecution));
        }

        private async Task<BlockExecutionResult> StepExecution()
        {
            await Task.CompletedTask;
            return BlockExecutionResult.Succeeded();
        }

        private void PostStepExecution()
        {
            Console.WriteLine(nameof(PostStepExecution));
        }
    }
}
