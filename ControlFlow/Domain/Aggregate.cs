using ControlFlow.Framework;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlFlow.Domain
{
    class Aggregate : IBlock
    {
        private Aggregate(Plan plan, ExecutionController executionController, List<Machine> machines)
        {
            Plan = plan;
            ExecutionController = executionController;
            Machines = machines;
        }

        public Plan Plan { get; private set; }

        public ExecutionController ExecutionController { get; private set; }

        public List<Machine> Machines { get; private set; }

        public static Aggregate Create(Process process, List<Machine> machines)
        {
            var plan = new Planner().CreatePlan(process);
            var executionController = ExecutionController.Create(plan, step => EventDrivenBlock.Create(step.Name));
            return new Aggregate(plan, executionController, machines);
        }

        public async Task<BlockExecutionResult> Execute()
        {
            return await ExecutionController.Execute();
        }

        public async Task<BlockExecutionResult> Handle(IMessage message)
        {
            return await ExecutionController.Handle(message);
        }
    }
}
