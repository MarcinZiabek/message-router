using System;
using System.Collections.Generic;
using System.Text;
using NetMQ;
using NetMQ.Sockets;

namespace NetmqRouter
{
    internal static class SocketExtensions
    {
        public static bool TryReceiveMessage(this IReceivingSocket socket, out Message message)
        {
            var mqMessage = new NetMQMessage();
            message = default(Message);

            if (!socket.TryReceiveMultipartMessage(ref mqMessage, 3))
                return false;

            var route = mqMessage[0].ConvertToString();
            var type = (RouteDataType)mqMessage[1].Buffer[0];
            var value = mqMessage[2].Buffer;

            message = new Message(route, type, value);
            return true;
        }

        public static void SendMessage(this IOutgoingSocket socket, Message message)
        {
            socket
                .SendMoreFrame(message.RouteName)
                .SendMoreFrame(new[] { (byte)message.DataType })
                .SendFrame(message.Buffer);
        }
    }
}
