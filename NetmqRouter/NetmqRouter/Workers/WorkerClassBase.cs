using System;
using System.Threading;
using System.Threading.Tasks;
using NetmqRouter.Infrastructure;

namespace NetmqRouter.Workers
{
    internal abstract class WorkerClassBase : IWorkerTask
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private CancellationToken _cancellationToken;
        
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
                
            }
            
        }

        internal abstract bool DoWork();
    }
}