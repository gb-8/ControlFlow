namespace ControlFlow
{
    public class FinishedEvent : IMessage
    {
        public FinishedEvent(Guid id) => this.Id = id;

        public Guid Id { get; }

    }
}
