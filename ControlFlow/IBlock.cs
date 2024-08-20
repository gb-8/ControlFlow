namespace ControlFlow
{
    public interface IBlock
    {
        BlockExecutionResult Execute();
        BlockExecutionResult Handle(IMessage message);
    }
}
