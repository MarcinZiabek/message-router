using System;

namespace NetmqRouter.Models
{
    internal class Subsriber
    {
        public Route Incoming { get; set; }
        public Route Outcoming { get; set; }

        public Func<object, object> Method { get; set; }

        public Subsriber(Route incoming, Route outcoming, Func<object, object> method)
        {
            Incoming = incoming;
            Outcoming = outcoming;
            Method = method;
        }

        public Subsriber()
        {
            
        }
    }
}
