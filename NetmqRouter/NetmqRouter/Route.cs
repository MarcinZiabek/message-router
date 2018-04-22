using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using NetmqRouter.Attributes;
using NetmqRouter.Infrastructure;
using Newtonsoft.Json;

namespace NetmqRouter
{
    public class Route
    {
        internal object Object { get; set; }
        internal MethodInfo Method { get; set; }

        public string IncomingRouteName { get; set; }
        public RouteDataType IncomingDataType { get; set; }

        public string OutcomingRouteName { get; set; }
        public RouteDataType OutcomingDataType { get; set; }

        public object Call(object data)
        {
            var arguments = (IncomingDataType == RouteDataType.Void) ? new object[0] : new[] {data};
            return Method.Invoke(Object,  arguments);
        }
    }
}
