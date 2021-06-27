using System;
using System.Threading;
using System.Threading.Tasks;

namespace MultitaskQueue
{
    public sealed class TaskManager<TResult>
    {
        private bool _isRunAnyFunction = false;

        private static readonly Lazy<TaskManager<TResult>> _taskManagerLazy = new Lazy<TaskManager<TResult>>(() => new TaskManager<TResult>());

        private int _maximumTaskRunning = Environment.ProcessorCount;

        private Semaphore queueManager;

        private TaskManager() => queueManager = new Semaphore(MaximumTaskRunning, MaximumTaskRunning);

        public int MaximumTaskRunning
        {
            get => _maximumTaskRunning;
            set
            {
                if (_isRunAnyFunction) throw new Exception("Can not set MaximumTaskRunning when any method is started");
                _maximumTaskRunning = value <= 0 ? 1 : value;
                queueManager = new Semaphore(_maximumTaskRunning, _maximumTaskRunning);
            }
        }

        public static TaskManager<TResult> Instance => _taskManagerLazy.Value;

        public Task<TResult> RunAsync(OneOf<Func<TResult>, Func<Task<TResult>>> oneOfFunc, Action<string> exceptionCallback = null)
        {
            _isRunAnyFunction = true;
            return Task.Run(async () =>
            {
                queueManager.WaitOne();
                try
                {
                    return await oneOfFunc.Match(func => Task.Run(() => func.Invoke()), funcTask => funcTask.Invoke());
                }
                catch (Exception ex)
                {
                    exceptionCallback?.Invoke(ex.Message);
                    return default;
                }
                finally
                {
                    ReleaseOne();
                }
            });
        }

        private void ReleaseOne()
        {
            try
            {
                queueManager.Release();
            }
            catch (Exception)
            {
                // Ignore
            }
        }
    }
}