using System;
using System.Threading;
using System.Threading.Tasks;
using NetmqRouter.Infrastructure;

namespace NetmqRouter.Workers
{
    public abstract class WorkerClassBase : IWorkerTask
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private CancellationToken _cancellationToken;

        public void Start()
        {
            _cancellationToken = _cancellationTokenSource.Token;
            Task.Run(() => DoWorkTask(), _cancellationToken);
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        private async void DoWorkTask()
        {
            while (true)
            {
                if (_cancellationToken.IsCancellationRequested)
                    return;
                
                if(!DoWork())
                    await Task.Delay(TimeSpan.FromMilliseconds(1), _cancellationToken);
            }
        }

        protected abstract bool DoWork();
    }
}