namespace ControlFlow
{
    public class StartedEvent : IMessage
    {
        public StartedEvent(Guid id) => this.Id = id;

        public Guid Id { get; }
    }
}
