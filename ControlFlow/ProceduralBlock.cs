namespace ControlFlow
{

    public class ProceduralBlock : IBlock
    {
        private readonly Action action;

        private ProceduralBlock(Action action)
        {
            this.action = action;
        }

        public static ProceduralBlock Create(string text) =>
            new ProceduralBlock(() => Console.WriteLine(text));

        public BlockExecutionResult Execute()
        {
            try
            {
                action();
                return BlockExecutionResult.Succeeded();
            }
            catch (Exception ex)
            {
                return BlockExecutionResult.Failed(ex);
            }
        }

        public BlockExecutionResult Handle(IMessage @event)
        {
            // TODO: Return silently instead?
            throw new NotSupportedException("Procedural blocks do not handle events.");
        }
    }
}
