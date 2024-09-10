namespace ControlFlow.Framework
{
    public class FinishedEvent : IMessage
    {
        public FinishedEvent(Guid id) => Id = id;

        public Guid Id { get; }

    }
}
