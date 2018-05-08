using System;

namespace MessageRouter.Models
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

        public static Subsriber Create(string routeName, Action action)
        {
            var route = new Route(routeName, null);

            return new Subsriber(route, null, payload =>
            {
                action();
                return null;
            });
        }

        public static Subsriber Create<T>(string routeName, Action<T> action)
        {
            var route = new Route(routeName, typeof(T));

            return new Subsriber(route, null, payload =>
            {
                action((T) payload);
                return null;
            });
        }

        public static Subsriber Create<T>(string incomingRouteName, string outcomingRouteName, Func<T> action)
        {
            var incomingRoute = new Route(incomingRouteName, null);
            var outcomingRoute = new Route(outcomingRouteName, typeof(T));

            return new Subsriber(incomingRoute, outcomingRoute, _ => action());
        }

        public static Subsriber Create<T, TK>(string incomingRouteName, string outcomingRouteName, Func<T, TK> action)
        {
            var incomingRoute = new Route(incomingRouteName, typeof(T));
            var outcomingRoute = new Route(outcomingRouteName, typeof(TK));

            return new Subsriber(incomingRoute, outcomingRoute, payload => action((T) payload));
        }
    }
}
