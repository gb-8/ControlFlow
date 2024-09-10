namespace ControlFlow.Domain
{
    class Process
    {
        public Process(List<Step> steps) => Steps = steps;
        public List<Step> Steps { get; private set; }

        public static Process Create(params Step[] steps) => new Process(steps.ToList());
    }
}
