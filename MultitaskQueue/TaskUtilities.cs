using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MultitaskQueue
{
    public class TaskUtilities
    {
        public static async Task DoWithTimeout(Task task, int timeout, Func<Task> doIfTimeout = null)
        {
            var anyTask = await Task.WhenAny(task, Task.Delay(timeout));
            if (anyTask != task) await doIfTimeout?.Invoke();
        }
        public static async Task<TResult> DoWithTimeout<TResult>(Task<TResult> task, int timeout, Func<Task<TResult>> doIfTimeout)
        {
            var anyTask = await Task.WhenAny(task, Task.Delay(timeout));
            if (anyTask != task) return await doIfTimeout.Invoke();
            return task.Result;
        }
    }
}
