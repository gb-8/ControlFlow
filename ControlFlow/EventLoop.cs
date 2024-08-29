using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlFlow
{
    public class EventLoop
    {
        private readonly Integrator integrator;

        public EventLoop(Integrator integrator)
        {
            this.integrator = integrator;
        }

        public async Task Start(IBlock block)
        {
            var executionResult = await block.Execute();
            while (executionResult.Status == ExecutionStatus.Executing)
            {
                var results = new List<BlockExecutionResult>();
                foreach (var message in executionResult.Messages)
                {
                    var @event = integrator.HandleSideEffect(message);
                    results.Add(await block.Handle(@event));
                    executionResult = BlockExecutionResult.Aggregate(results.ToArray());
                }
            }
        }
    }

    public class Integrator
    {
        public IMessage HandleSideEffect(IMessage message)
        {
            switch (message)
            {
                case StartSideEffect start:
                    {
                        return new StartedEvent(start.Id);
                    }
                case FinishSideEffect finish:
                    {
                        return new FinishedEvent(finish.Id);
                    }
                case var _:
                    {
                        throw new NotImplementedException();
                    }
            }
        }
    }
}
