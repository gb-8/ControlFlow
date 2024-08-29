namespace ControlFlow
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var proceduralBlock = Sequence.Create(
                ProceduralBlock.Create("Foo"),
                ProceduralBlock.Create("Bar"),
                ProceduralBlock.Create("Baz"));
            await proceduralBlock.Execute();

            var eventDrivenBlock = Sequence.Create(
                EventDrivenBlock.Create("Alpha"),
                EventDrivenBlock.Create("Beta"),
                EventDrivenBlock.Create("Gamma"));
            var eventLoop = new EventLoop(new Integrator());
            await eventLoop.Start(eventDrivenBlock);

            var mixedBlock = Sequence.Create(
                ProceduralBlock.Create("Foo"),
                EventDrivenBlock.Create("Alpha"),
                ProceduralBlock.Create("Bar"),
                EventDrivenBlock.Create("Beta"),
                ProceduralBlock.Create("Baz"),
                EventDrivenBlock.Create("Gamma"));
            await eventLoop.Start(mixedBlock);


            var parallelForeachWithProceduralBlocks = ParallelForeach.Create(
                3,
                ProceduralBlock.CreateWithDelay(1, "A"),
                ProceduralBlock.CreateWithDelay(1, "B"),
                ProceduralBlock.CreateWithDelay(1, "C"),
                ProceduralBlock.CreateWithDelay(1, "D"),
                ProceduralBlock.CreateWithDelay(1, "E"),
                ProceduralBlock.CreateWithDelay(1, "F"),
                ProceduralBlock.CreateWithDelay(1, "G"),
                ProceduralBlock.CreateWithDelay(1, "H"),
                ProceduralBlock.CreateWithDelay(1, "I"),
                ProceduralBlock.CreateWithDelay(1, "J"),
                ProceduralBlock.CreateWithDelay(1, "K"));

            await parallelForeachWithProceduralBlocks.Execute();


            var parallelForeachWithEventDrivenBlocks = ParallelForeach.Create(
                3,
                EventDrivenBlock.Create("A"),
                EventDrivenBlock.Create("B"),
                EventDrivenBlock.Create("C"),
                EventDrivenBlock.Create("D"),
                EventDrivenBlock.Create("E"),
                EventDrivenBlock.Create("F"),
                EventDrivenBlock.Create("G"),
                EventDrivenBlock.Create("H"),
                EventDrivenBlock.Create("I"),
                EventDrivenBlock.Create("J"),
                EventDrivenBlock.Create("K"));

            Console.WriteLine("Event drive parallel foreach:");
            await eventLoop.Start(parallelForeachWithEventDrivenBlocks);
        }
    }

}
