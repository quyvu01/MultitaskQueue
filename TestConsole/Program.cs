using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var res = MultitaskQueue.TaskManager<int>.Instance.RunAsync(DoSomethingAsync, message => Console.WriteLine(message));
            var res1 = MultitaskQueue.TaskManager<int>.Instance.RunAsync(DoSomethingAsync, message => Console.WriteLine(message));
            _ = res.ContinueWith(ret => Console.WriteLine(ret.Result));
            _ = res1.ContinueWith(ret => Console.WriteLine(ret.Result));
            Console.ReadKey();
        }

        private static readonly Func<Task<int>> DoSomethingAsync = async () =>
        {
            var rd = new Random().Next(1000, 3000);
            await Task.Delay(rd);
            return rd;
        };
    }

    public class Person
    {
        public string Name { get; set; }
    }
}