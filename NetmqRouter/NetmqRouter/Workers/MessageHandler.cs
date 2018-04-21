using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NetmqRouter.Infrastructure;

namespace NetmqRouter.Workers
{
    public class MessageHandler : WorkerClassBase
    {
        private readonly Dictionary<string, List<Route>> _routing;
        private readonly ConcurrentQueue<Message> _messageQueue = new ConcurrentQueue<Message>();

        private readonly ITextSerializer _textSerializer;
        private readonly IObjectSerializer _objectSerializer;
        
        public MessageHandler(IEnumerable<Route> routes, ITextSerializer textSerializer, IObjectSerializer objectSerializer)
        {
            _textSerializer = textSerializer;
            _objectSerializer = objectSerializer;
            
            _routing = routes
                .GroupBy(x => x.IncomingRouteName)
                .ToDictionary(x => x.Key, x => x.ToList());
        }

        public void HandleMessage(Message message) => _messageQueue.Enqueue(message);

        protected override bool DoWork()
        {
            if (!_messageQueue.TryDequeue(out var message))
                return false;
                
            if(!_routing.ContainsKey(message.RouteName))
                return false;

            _routing
                [message.RouteName]
                .Where(x => x.IncomingDataType == message.DataType)
                .ToList()
                .ForEach(x => x.Call(message.Buffer, _textSerializer, _objectSerializer));

            return true;
        }
    }
}