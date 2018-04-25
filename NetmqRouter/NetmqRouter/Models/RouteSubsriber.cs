using System;

namespace NetmqRouter.Models
{
    internal class RouteSubsriber
    {
        public Route Incoming { get; set; }
        public Route Outcoming { get; set; }

        public Func<object, object> Method { get; set; }
    }
}
