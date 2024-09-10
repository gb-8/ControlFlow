namespace ControlFlow.Framework
{
    public class BlockExecutionResult
    {
        public static BlockExecutionResult Succeeded(params IMessage[] messages) => new BlockExecutionResult(ExecutionStatus.Complete, null, messages);

        public static BlockExecutionResult Failed(Exception ex, params IMessage[] messages) => new BlockExecutionResult(ExecutionStatus.Complete, ex);

        public static BlockExecutionResult Executing(params IMessage[] events) => new BlockExecutionResult(ExecutionStatus.Executing, null, events);

        public static BlockExecutionResult Aggregate(params BlockExecutionResult[] results) =>
            new BlockExecutionResult(
                results.Any(r => r.Status == ExecutionStatus.Executing) ? ExecutionStatus.Executing : ExecutionStatus.Complete,
                results.Where(r => r.Exception != null).Select(r => r.Exception).FirstOrDefault(),
                results.SelectMany(r => r.Messages).ToArray());

        private BlockExecutionResult(ExecutionStatus status, Exception? ex, params IMessage[] messages)
        {
            Status = status;
            Exception = ex;
            Messages = messages;
        }

        public ExecutionStatus Status { get; }

        public Exception? Exception { get; }

        public IEnumerable<IMessage> Messages { get; }
    }
}
