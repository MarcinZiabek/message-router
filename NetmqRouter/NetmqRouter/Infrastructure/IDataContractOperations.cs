using System.Collections.Generic;
using NetmqRouter.Models;

namespace NetmqRouter.Infrastructure
{
    internal interface IDataContractOperations
    {
        IEnumerable<string> GetIncomingRouteNames();

        IEnumerable<Message> CallRoute(Message message);

        SerializedMessage Serialize(Message message);
        Message Deserialize(SerializedMessage message);
    }
}