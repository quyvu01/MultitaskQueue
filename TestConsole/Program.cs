using System;
using System.Threading;

namespace TestConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var res = MultitaskQueue.TaskManager<string>.Instance.Run(DoSomething);
            var res1 = MultitaskQueue.TaskManager<string>.Instance.Run(DoSomething);
            _ = res.ContinueWith(ret => Console.WriteLine(ret.Result));
            _ = res1.ContinueWith(ret => Console.WriteLine(ret.Result));
            Console.ReadKey();
        }

        private static string DoSomething()
        {
            Thread.Sleep(3000);
            throw new Exception("hahaha");
            return "Hello World";
        }
    }
}