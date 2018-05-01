using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NetmqRouter.Models;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

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