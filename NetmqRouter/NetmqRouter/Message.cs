using System;
using System.Collections.Generic;
using System.Text;

namespace NetmqRouter
{
    internal struct Message
    {
        public string RouteName { get; set; }
        public RouteDataType DataType { get; set; }
        public byte[] Buffer { get; set; }

        public Message(string routeName, RouteDataType dataType, byte[] buffer)
        {
            RouteName = routeName;
            DataType = dataType;
            Buffer = buffer;
        }
    }
}
