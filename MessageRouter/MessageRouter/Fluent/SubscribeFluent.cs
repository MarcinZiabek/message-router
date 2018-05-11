using System;
using MessageRouter.Infrastructure;

namespace MessageRouter.Fluent
{
    public class SubscribeFluent
    {
        private readonly IMessageRouter _router;
        private readonly string _incomingAddress;
        
        public SubscribeFluent(IMessageRouter router, string incomingAddress)
        {
            _router = router;
            _incomingAddress = incomingAddress;
        }

        public SubscribeWithResponseFluent WithResponse(string outcomingAddress)
        {
            return new SubscribeWithResponseFluent(_router, _incomingAddress, outcomingAddress);
        }

        public void WithHandler(Action action) => _router.RegisterSubscriber(_incomingAddress, action);
        public void WithHandler<T>(Action<T> action) => _router.RegisterSubscriber(_incomingAddress, action);
    }
    
    public class SubscribeWithResponseFluent
    {
        private readonly IMessageRouter _router;
        private readonly string _incomingAddress;
        private readonly string _outcomingAddress;
        
        public SubscribeWithResponseFluent(IMessageRouter router, string incomingAddress, string outcomingAddress)
        {
            _router = router;
            _incomingAddress = incomingAddress;
            _outcomingAddress = outcomingAddress;
        }
        
        public void WithHandler(Action action) => _router.RegisterSubscriber(_incomingAddress, _outcomingAddress, action);
        public void WithHandler<T>(Action<T> action) => _router.RegisterSubscriber(_incomingAddress, _outcomingAddress, action);
        public void WithHandler<T>(Func<T> action) => _router.RegisterSubscriber(_incomingAddress, _outcomingAddress, action);
        public void WithHandler<T, TK>(Func<T, TK> action) => _router.RegisterSubscriber(_incomingAddress, _outcomingAddress, action);
    }
}