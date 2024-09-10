using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlFlow.Framework
{
    public class EventDrivenBlock : IBlock
    {
        public Guid Id { get; set; }
        public ExecutionStatus ExecutionStatus { get; private set; }
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
            await Task.CompletedTask;
            ExecutionStatus = ExecutionStatus.Executing;
            var sideEffect = new StartSideEffect(Id);
            return BlockExecutionResult.Executing(sideEffect);
        }

        public async Task<BlockExecutionResult> Handle(IMessage message)
        {
            Log.Logger.Debug("Event drive block ({text}): handling message.", Text);

            await Task.CompletedTask;

            // TODO: We would actually match by message-specific fields.
            if (message.Id != Id)
            {
                Log.Logger.Debug("Event drive block ({text}): ignored.", Text);
                return ReturnCurrentStatus();
            }

            switch (message)
            {
                case StartedEvent started:
                    {
                        Log.Logger.Debug("Event drive block ({text}): started, so sending finish side-effect.", Text);
                        Started = true;
                        var finish = new FinishSideEffect(Id);
                        return BlockExecutionResult.Executing(finish);
                    }
                case FinishedEvent finished:
                    {
                        if (!Started)
                        {
                            // Ignore finised events if we have not started.
                            // TODO: maybe handled this as an error?
                            Log.Logger.Debug("Event drive block ({text}): received finished event, but haven't started - ERROR!", Text);
                            return ReturnCurrentStatus();
                        }

                        Console.WriteLine(Text);
                        Log.Logger.Debug("Event drive block ({text}): completed.", Text);
                        return BlockExecutionResult.Succeeded();
                    }
                case var unknownMessage:
                    {
                        // Ignore other messages.
                        Log.Logger.Debug("Event drive block ({text}): ignoring unknown message type: {type}.", Text, message.GetType());
                        return ReturnCurrentStatus();
                    }
            };
        }

        private BlockExecutionResult ReturnCurrentStatus() => ExecutionStatus == ExecutionStatus.Executing
            ? BlockExecutionResult.Executing()
            : BlockExecutionResult.Succeeded();
    }
}
