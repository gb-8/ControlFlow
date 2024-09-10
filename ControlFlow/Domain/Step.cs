namespace ControlFlow.Domain
{
    class Step
    {
        public Step(string name, string role, bool runConcurrentWithPreviousStep)
        {
            Name = name;
            Role = role;
            RunConcurrentlyWithPreviousStep = runConcurrentWithPreviousStep;
        }

        public string Name { get; private set; }
        public string Role { get; private set; }
        public bool RunConcurrentlyWithPreviousStep { get; private set; }
    }
}
