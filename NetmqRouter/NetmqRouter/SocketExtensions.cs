using System;
using System.Collections.Generic;
using System.Text;
using NetmqRouter.Models;
using NetMQ;
using NetMQ.Sockets;

namespace NetmqRouter
{
    internal static class SocketExtensions
    {
        public static bool TryReceiveMessage(this IReceivingSocket socket, out SerializedMessage message)
        {
            var mqMessage = new NetMQMessage();
            message = default(SerializedMessage);

            if (!socket.TryReceiveMultipartMessage(ref mqMessage, 2))
                return false;

            var route = mqMessage[0].ConvertToString();
            var value = mqMessage[1].Buffer;

            message = new SerializedMessage(route, value);
            return true;
        }

        public static void SendMessage(this IOutgoingSocket socket, SerializedMessage message)
        {
            socket
                .SendMoreFrame(message.RouteName)
                .SendFrame(message.Data);
        }
    }
}
