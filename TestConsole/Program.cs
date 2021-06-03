using System;
using System.Threading.Tasks;

namespace TestConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var res = MultitaskQueue.TaskManager<Person>.Instance.RunAsync(DoSomethingAsync);
            var res1 = MultitaskQueue.TaskManager<Person>.Instance.RunAsync(DoSomethingAsync);
            _ = res.ContinueWith(ret => Console.WriteLine(ret.Result));
            _ = res1.ContinueWith(ret => Console.WriteLine(ret.Result));
            Console.ReadKey();
        }

        private static async Task<Person> DoSomethingAsync()
        {
            await Task.Delay(3000);
            return new Person { Name = "Quy" };
        }
    }

    public class Person
    {
        public string Name { get; set; }
    }
}