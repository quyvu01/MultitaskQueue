using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var res = MultitaskQueue.TaskManager<string>.Instance.Run(DoSomething, new CancellationTokenSource(TimeSpan.FromSeconds(2)).Token);
            var res1 = MultitaskQueue.TaskManager<string>.Instance.Run(DoSomething, new CancellationTokenSource(TimeSpan.FromSeconds(2)).Token);
            res.ContinueWith(ret => Console.WriteLine(ret));
            res1.ContinueWith(ret => Console.WriteLine(ret));
            Console.ReadKey();
        }
        static string DoSomething()
        {
            Thread.Sleep(4000);
            return "Hello World";
        }
    }
}
