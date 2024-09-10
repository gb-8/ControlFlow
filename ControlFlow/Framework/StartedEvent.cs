namespace ControlFlow.Framework
{
    public class StartedEvent : IMessage
    {
        public StartedEvent(Guid id) => Id = id;

        public Guid Id { get; }
    }
}
