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

        public object Call(byte[] data, ITextSerializer textSerializer, IObjectSerializer objectSerializer)
        {
            if (IncomingDataType == RouteDataType.Void)
                return Method.Invoke(Object, new object[0]);

            if (IncomingDataType == RouteDataType.Text)
                return Method.Invoke(Object, new[] { (data != null) ? textSerializer.Desialize(data) : null } );

            if (IncomingDataType == RouteDataType.Object)
            {
                object _object = null;

                if (data != null)
                    _object = objectSerializer.Desialize(data, typeof(object));
                
                return Method.Invoke(Object, new[] { _object });
            }

            return Method.Invoke(Object, new[] { data });
        }
    }
}
