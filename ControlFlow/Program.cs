using ControlFlow.Domain;
using ControlFlow.Framework;
using Serilog;

namespace ControlFlow
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console().CreateLogger();

            var process = Process.Create(
                new Step("Step A1", "role", false),
                new Step("Step A2", "role", true),
                new Step("Step A3", "role", true),
                new Step("Step B1", "role", false),
                new Step("Step B2", "role", true),
                new Step("Step B3", "role", true),
                new Step("Step C1", "role", false),
                new Step("Step C2", "role", true),
                new Step("Step C3", "role", true));

            Console.WriteLine("Procedural execution:");
            var taskController = new TaskController();
            await taskController.Execute(process, []);
            Console.WriteLine("");

            Console.WriteLine("Event-driven execution:");
            var aggregate = Aggregate.Create(process, []);
            var eventLoop = new EventLoop(new Integrator());
            await eventLoop.Start(aggregate);
            Console.WriteLine("");

            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}
