using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NetmqRouter
{
    public class Route
    {
        internal object Object { get; set; }
        internal MethodInfo Method { get; set; }

        public bool IsAsync { get; set; }

        public string IncomingRouteName { get; set; }
        public RouteDataType IncomingDataType { get; set; }

        public string OutcomingRouteName { get; set; }
        public RouteDataType OutcomingDataType { get; set; }

        public object Call(byte[] data)
        {
            if (IncomingDataType == RouteDataType.Void)
                return Method.Invoke(Object, new object[0]);

            else
                return Method.Invoke(Object, new[] { data });
        }
    }
}
