using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlFlow
{
    public class EventDrivenBlock : IBlock
    {
        public Guid Id { get; set; }
        public bool Started { get; set; }
        public string Text { get; set; }
        private EventDrivenBlock(Guid id, bool started, string text)
        {
            Id = id;
            Started = started;
            Text = text;
        }

        public static EventDrivenBlock Create(string text) =>
            new EventDrivenBlock(Guid.NewGuid(), false, text);

        public async Task<BlockExecutionResult> Execute()
        {
            var sideEffect = new StartSideEffect(Id);
            return BlockExecutionResult.Executing(sideEffect);
        }

        public async Task<BlockExecutionResult> Handle(IMessage message)
        {
            // TODO: We would actually match by message-specific fields.
            if (message.Id != this.Id)
            {
                return BlockExecutionResult.Unhandled();
            }

            switch (message)
            {
                case StartedEvent started:
                    {
                        this.Started = true;
                        var finish = new FinishSideEffect(this.Id);
                        return BlockExecutionResult.Executing(finish);
                    }
                case FinishedEvent finished:
                    {
                        if (!Started)
                        {
                            // Ignore finised events if we have not started.
                            // TODO: maybe handled this as an error?
                            return BlockExecutionResult.Unhandled();
                        }

                        Console.WriteLine(Text);
                        return BlockExecutionResult.Succeeded();
                    }
                case var unknownMessage:
                    {
                        // Ignore other messages.
                        return BlockExecutionResult.Unhandled();
                    }
            };
        }
    }
}
