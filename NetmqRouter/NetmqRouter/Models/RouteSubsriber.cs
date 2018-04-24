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
    internal class RouteSubsriber
    {
        public Route Incoming { get; set; }
        public Route Outcoming { get; set; }

        public Func<object, object> Method { get; set; }
    }
}
