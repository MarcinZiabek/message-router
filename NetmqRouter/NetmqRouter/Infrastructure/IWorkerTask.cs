namespace NetmqRouter.Infrastructure
{
    internal interface IWorkerTask
    {
        void Start();
        void Stop();
    }
}