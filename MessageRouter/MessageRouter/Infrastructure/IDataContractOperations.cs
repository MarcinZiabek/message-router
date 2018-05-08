using System.Collections.Generic;
using MessageRouter.Models;

namespace MessageRouter.Infrastructure
{
    internal interface IDataContractOperations
    {
        IEnumerable<string> GetIncomingRouteNames();

        IEnumerable<Message> CallRoute(Message message);

        SerializedMessage Serialize(Message message);
        Message Deserialize(SerializedMessage message);
    }
}