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

        public void Start(IBlock block)
        {
            var executionResult = block.Execute();
            while (executionResult.Status == ExecutionStatus.Executing)
            {
                var @event = integrator.HandleSideEffect(executionResult.Messages.First());
                executionResult = block.Handle(@event);
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
