using System;
using MessageRouter.Infrastructure;
using MessageRouter.Models;

namespace MessageRouter.BusinessLogic
{
    public class MessageRouter : IMessageRouter
    {
        private readonly IDataContractBuilder _dataContractBuilder = new DataContractBuilder();
        private readonly IConnection _connection;
        private readonly DataFlowManager _dataFlowManager = new DataFlowManager();

        private int _numberOfSerializationWorkes = 1;
        private int _numberOfHandlingWorkes = 4;

        public event Action<Exception> OnException;

        public MessageRouter(IConnection connection)
        {
            _connection = connection;
            _dataFlowManager.OnException += OnException;
        }

        #region Managing

        public IMessageRouter StartRouting()
        {
            var dataContract = new DataContractManager(_dataContractBuilder);
            _connection.Connect(dataContract.GetIncomingRouteNames());

            _dataFlowManager.CreateWorkers(_connection, dataContract);
            _dataFlowManager.RegisterExceptionsHandler();
            _dataFlowManager.RegisterDataFlow();
            _dataFlowManager.StartWorkers(_numberOfSerializationWorkes, _numberOfHandlingWorkes);

            return this;
        }

        public IMessageRouter StopRouting()
        {
            _dataFlowManager.StopWorkers();
            return this;
        }

        public void Dispose()
        {
            StopRouting();
            Disconnect();
        }

        public IMessageRouter Disconnect()
        {
            _connection.Disconnect();
            return this;
        }

        public IMessageRouter WithWorkerPool(int numberOfSerializationWorkes, int numberOfHandlingWorkes)
        {
            _numberOfSerializationWorkes = numberOfSerializationWorkes;
            _numberOfHandlingWorkes = numberOfHandlingWorkes;

            return this;
        }

        #endregion

        #region Serialization

        public IMessageRouter RegisterTypeSerializer<T>(ITypeSerializer<T> typeSerializer)
        {
            _dataContractBuilder.RegisterSerializer(typeSerializer);
            return this;
        }

        public IMessageRouter RegisterGeneralSerializer<T>(IGeneralSerializer<T> serializer)
        {
            _dataContractBuilder.RegisterGeneralSerializer(serializer);
            return this;
        }

        #endregion

        #region Routing

        public IMessageRouter RegisterEventRoute(string routeName)
        {
            _dataContractBuilder.RegisterRoute(new Route(routeName));
            return this;
        }
        
        public IMessageRouter RegisterRoute(string routeName, Type dataType)
        {
            _dataContractBuilder.RegisterRoute(new Route(routeName, dataType));
            return this;
        }

        #endregion

        #region Subscription

        public IMessageRouter RegisterSubscriber(Subscriber subscriber)
        {
            _dataContractBuilder.RegisterSubscriber(subscriber);
            return this;
        }

        #endregion

        #region Messaging

        public void SendMessage(Message message) => _dataFlowManager.SendMessage(message);

        #endregion
    }
}
