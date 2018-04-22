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
    internal class Route
    {
        internal object Object { get; set; }
        internal MethodInfo Method { get; set; }

        public string IncomingRouteName { get; set; }
        public Type IncomingDataType { get; set; }

        public string OutcomingRouteName { get; set; }
        public Type OutcomingDataType { get; set; }

        public object Call(object data)
        {
            var arguments = (IncomingDataType == null) ? new object[0] : new[] {data};
            return Method.Invoke(Object,  arguments);
        }
    }
}
