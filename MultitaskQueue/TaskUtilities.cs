using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultitaskQueue
{
    public class TaskUtilities
    {
        public static async Task DoWithTimeout(Task task, int timeout, Func<Task> doIfTimeout)
        {
            if (doIfTimeout == null) throw new ArgumentNullException("DoIfTimeout function can not be null!");
            var anyTask = await Task.WhenAny(task, Task.Delay(timeout));
            if (anyTask != task) await doIfTimeout?.Invoke();
        }
        public static async Task<TResult> DoWithTimeout<TResult>(Task<TResult> task, int timeout, Func<Task<TResult>> doIfTimeout)
        {
            if (doIfTimeout == null) throw new ArgumentNullException("DoIfTimeout function can not be null!");
            var anyTask = await Task.WhenAny(task, Task.Delay(timeout));
            if (anyTask != task) return await doIfTimeout.Invoke();
            return task.Result;
        }
        public static Task ParallelForEachAsync<T>(IEnumerable<T> source, int degreeOfParallelization, Func<T, Task> body)
        {
            async Task AwaitPartition(IEnumerator<T> partition)
            {
                using (partition)
                {
                    while (partition.MoveNext())
                    {
                        await body(partition.Current);
                    }
                }
            }
            return Task.WhenAll(Partitioner.Create(source).GetPartitions(degreeOfParallelization).AsParallel().Select(t => AwaitPartition(t)));
        }
    }
}
