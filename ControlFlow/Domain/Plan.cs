namespace ControlFlow.Domain
{
    class Plan
    {
        public List<List<Step>> StepBatches { get; private set; }
        public Plan(List<List<Step>> stepBatches)
             => StepBatches = stepBatches;

        public List<Step>? Next()
        {
            var batch = StepBatches.FirstOrDefault();
            if (batch != null)
            {
                StepBatches.Remove(batch);
            }

            return batch;
        }
    }
}
