using ControlFlow.Framework;

namespace ControlFlow.Domain
{
    class TaskController
    {
        public async Task Execute(Process process, List<Machine> machines)
        {
            await Task.CompletedTask;
            var plan = new Planner().CreatePlan(process);
            var executionController = ExecutionController.Create(plan, step => ProceduralBlock.Create(step.Name));
            await executionController.Execute();
        }
    }
}
