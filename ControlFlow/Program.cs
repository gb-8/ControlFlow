namespace ControlFlow
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var proceduralBlock = Sequence.Create(
                ProceduralBlock.Create("Foo"),
                ProceduralBlock.Create("Bar"),
                ProceduralBlock.Create("Baz"));
            proceduralBlock.Execute();

            var eventDrivenBlock = Sequence.Create(
                EventDrivenBlock.Create("Alpha"),
                EventDrivenBlock.Create("Beta"),
                EventDrivenBlock.Create("Gamma"));
            var eventLoop = new EventLoop(new Integrator());
            eventLoop.Start(eventDrivenBlock);

            var mixedBlock = Sequence.Create(
                ProceduralBlock.Create("Foo"),
                EventDrivenBlock.Create("Alpha"),
                ProceduralBlock.Create("Bar"),
                EventDrivenBlock.Create("Beta"),
                ProceduralBlock.Create("Baz"),
                EventDrivenBlock.Create("Gamma"));
            eventLoop.Start(mixedBlock);
        }
    }
}
