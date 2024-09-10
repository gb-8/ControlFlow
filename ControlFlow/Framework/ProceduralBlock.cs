namespace ControlFlow.Framework
{

    public class ProceduralBlock : IBlock
    {
        private readonly Func<Task> action;

        protected ProceduralBlock(Func<Task> action)
        {
            this.action = action;
        }

        public static ProceduralBlock Create(string text) =>
            new ProceduralBlock(async () => { await Task.CompletedTask; Console.WriteLine(text); });

        public static ProceduralBlock CreateWithDelay(int seconds, string text) =>
            new ProceduralBlock(async () => { await Task.Delay(TimeSpan.FromSeconds(seconds)); Console.WriteLine(text); });

        public async Task<BlockExecutionResult> Execute()
        {
            try
            {
                await action();
                return BlockExecutionResult.Succeeded();
            }
            catch (Exception ex)
            {
                return BlockExecutionResult.Failed(ex);
            }
        }

        public async Task<BlockExecutionResult> Handle(IMessage @event)
        {
            await Task.CompletedTask;
            // TODO: Return silently instead?
            throw new NotSupportedException("Procedural blocks do not handle events.");
        }
    }
}
