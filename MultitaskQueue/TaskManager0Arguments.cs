using System;
using System.Threading;
using System.Threading.Tasks;

namespace MultitaskQueue
{
    public sealed class TaskManager<TResult>
    {
        private bool _isRunAnyFunction = false;

        private static readonly Lazy<TaskManager<TResult>> TaskManagerLazy = new Lazy<TaskManager<TResult>>(() => new TaskManager<TResult>());

        private int _maximumTaskRunning = Environment.ProcessorCount;

        private SemaphoreSlim _queueManager;

        private TaskManager() => _queueManager = new SemaphoreSlim(MaximumTaskRunning);

        public int MaximumTaskRunning
        {
            get => _maximumTaskRunning;
            set
            {
                if (_isRunAnyFunction) throw new Exception("Can not set MaximumTaskRunning when any method is started");
                _maximumTaskRunning = value <= 0 ? 1 : value;
                _queueManager = new SemaphoreSlim(MaximumTaskRunning);
            }
        }

        public static TaskManager<TResult> Instance => TaskManagerLazy.Value;

        public Task<TResult> RunAsync(OneOf<Func<TResult>, Func<Task<TResult>>> oneOfFunc, Action<string> exceptionCallback = null)
        {
            _isRunAnyFunction = true;
            return Task.Run(async () =>
            {
                await _queueManager.WaitAsync();
                try
                {
                    return await oneOfFunc.Match(func => Task.Run(func.Invoke), funcTask => funcTask.Invoke());
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
                _queueManager.Release();
            }
            catch (Exception)
            {
                // Ignore
            }
        }
    }
}