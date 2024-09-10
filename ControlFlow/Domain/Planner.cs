namespace ControlFlow.Domain
{
    class Planner
    {
        public Plan CreatePlan(Process process)
        {
            if (!process.Steps.Any())
            {
                throw new InvalidOperationException();
            }

            var batches = new List<List<Step>>();
            List<Step> currentBatch = new List<Step>();
            batches.Add(currentBatch);
            currentBatch.Add(process.Steps.First());

            foreach (var step in process.Steps.Skip(1))
            {
                if (!step.RunConcurrentlyWithPreviousStep)
                {
                    currentBatch = new List<Step>();
                    batches.Add(currentBatch);
                }

                currentBatch.Add(step);
            }

            return new Plan(batches);
        }
    }
}
