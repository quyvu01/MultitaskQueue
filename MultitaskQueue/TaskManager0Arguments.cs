using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MultitaskQueue
{
    public sealed class TaskManager<TResult>
    {
        private static readonly Lazy<TaskManager<TResult>> _taskManagerLazy = new Lazy<TaskManager<TResult>>(() => new TaskManager<TResult>());

        private int _maximumTaskRunning = Environment.ProcessorCount;

        private TaskManager() => _taskAdded.OnChange += TaskAdded_OnChange;

        ~TaskManager() => _taskAdded.OnChange -= TaskAdded_OnChange;

        private void TaskAdded_OnChange(object sender, ObservableAction action) => RunChecking();

        public int MaximumTaskRunning { get => _maximumTaskRunning; set { _maximumTaskRunning = value <= 0 ? 1 : value; } }

        private readonly ConcurrentDictionaryObservable<Guid, TaskArgs> _taskAdded = new ConcurrentDictionaryObservable<Guid, TaskArgs>();

        private readonly ConcurrentDictionaryObservable<Guid, TaskArgs> _taskRunning = new ConcurrentDictionaryObservable<Guid, TaskArgs>();
        public static TaskManager<TResult> Instance => _taskManagerLazy.Value;

        public int RemainTasks => _taskAdded.Count;

        public int RunningTasks => _taskRunning.Count;

        public async Task<TResult> Run(Func<TResult> function, CancellationToken cancellation = default)
        {
            var taskArgs = new TaskArgs { Function = function, CancellationToken = cancellation };
            _taskAdded.TryAdd(taskArgs.Id, taskArgs);
            return await taskArgs.TaskSource.Task;
        }

        private void RunChecking()
        {
            var taskNeedToRun = _taskAdded.OrderBy(p => p.Value.CreatedTime).Take(MaximumTaskRunning);
            foreach (var task in taskNeedToRun)
            {
                if (_taskRunning.ContainsKey(task.Key)) return;
                _taskRunning.TryAdd(task.Key, task.Value);
                task.Value.CancellationToken.ThrowIfCancellationRequested();
                Task.Run(() =>
                {
                    TResult result = default;
                    try
                    {
                        task.Value.State = State.Running;
                        result = task.Value.Function.Invoke();
                        return result;
                    }
                    catch (Exception ex)
                    {
                        task.Value.Exception = ex;
                        return result;
                    }
                    finally
                    {
                        _taskAdded.TryRemove(task.Key, out _);
                        _taskRunning.TryRemove(task.Key, out _);
                    }
                }, task.Value.CancellationToken).ContinueWith(result =>
                {
                    try
                    {
                        task.Value.TaskSource.TrySetResult(result.Result);
                    }
                    catch (Exception)
                    {
                        task.Value.TaskSource.TrySetCanceled();
                    }
                });
            }
        }

        private class TaskArgs
        {
            public Func<TResult> Function { get; set; }
            public Guid Id { get; set; } = Guid.NewGuid();
            public Exception Exception { get; set; }
            public State State { get; set; } = State.NotStarted;
            public TaskCompletionSource<TResult> TaskSource { get; set; } = new TaskCompletionSource<TResult>();
            public DateTime CreatedTime { get; } = DateTime.Now;

            public CancellationToken CancellationToken { get; set; }
        }
    }
}