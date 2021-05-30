using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var res = await MultitaskQueue.TaskManager<string>.Instance.Run(DoSomething, new CancellationTokenSource(TimeSpan.FromSeconds(2)).Token);
            Console.WriteLine(res);
            Console.ReadKey();
        }
        static string DoSomething()
        {
            Thread.Sleep(4000);
            return "Hello World";
        }
    }
}
