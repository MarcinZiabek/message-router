using System;
using System.Threading;
using System.Threading.Tasks;
using MessageRouter.Infrastructure;

namespace MessageRouter.Workers
{
    /// <summary>
    /// This class can be used for creating any worker class that needs to perform repeating task.
    /// </summary>
    internal abstract class WorkerClassBase : IWorkerTask, IExceptionSource
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private CancellationToken _cancellationToken;

        public event Action<Exception> OnException;

        public void Start(int numberOfWorkers = 1)
        {
            _cancellationToken = _cancellationTokenSource.Token;

            for(var i=0; i<numberOfWorkers; i++)
                Task.Run(() => DoWorkTask(), _cancellationToken);
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        private async void DoWorkTask()
        {
            try
            {
                while (true)
                {
                    _cancellationToken.ThrowIfCancellationRequested();

                    if (!DoWork())
                        await Task.Delay(TimeSpan.FromMilliseconds(1), _cancellationToken);
                }
            }
            catch (Exception e)
            {
                OnException?.Invoke(e);
            }

        }

        /// <summary>
        /// Function to be repeated over time.
        /// </summary>
        /// <returns>If returns true, function will be repeated as soon as possible, otherwise after 1 millisecond.</returns>
        internal abstract bool DoWork();
    }
}