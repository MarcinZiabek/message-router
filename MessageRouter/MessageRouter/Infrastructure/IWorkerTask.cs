namespace MessageRouter.Infrastructure
{
    internal interface IWorkerTask
    {
        void Start(int numberOfWorkers = 1);
        void Stop();
    }
}