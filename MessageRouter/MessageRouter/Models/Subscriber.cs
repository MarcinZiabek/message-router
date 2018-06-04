using System;

namespace MessageRouter.Models
{
    public class Subscriber
    {
        internal Route Incoming { get; set; }
        internal Route Outcoming { get; set; }

        internal Func<object, object> Method { get; set; }

        internal Subscriber(Route incoming, Route outcoming, Func<object, object> method)
        {
            Incoming = incoming;
            Outcoming = outcoming;
            Method = method;
        }

        internal Subscriber()
        {

        }

        internal static Subscriber Create(string routeName, Action action)
        {
            var route = new Route(routeName);

            return new Subscriber(route, null, payload =>
            {
                action();
                return null;
            });
        }

        internal static Subscriber Create<T>(string routeName, Action<T> action)
        {
            var route = new Route(routeName, typeof(T));

            return new Subscriber(route, null, payload =>
            {
                action((T) payload);
                return null;
            });
        }

        internal static Subscriber Create(string incomingRouteName, string outcomingRouteName, Action action)
        {
            var incomingRoute = new Route(incomingRouteName);
            var outcomingRoute = new Route(outcomingRouteName);

            return new Subscriber(incomingRoute, outcomingRoute, _ =>
            {
                action();
                return null;
            });
        }

        internal static Subscriber Create<T>(string incomingRouteName, string outcomingRouteName, Action<T> action)
        {
            var incomingRoute = new Route(incomingRouteName, typeof(T));
            var outcomingRoute = new Route(outcomingRouteName);

            return new Subscriber(incomingRoute, outcomingRoute, payload =>
            {
                action((T) payload);
                return null;
            });
        }
        
        internal static Subscriber Create<T>(string incomingRouteName, string outcomingRouteName, Func<T> action)
        {
            var incomingRoute = new Route(incomingRouteName);
            var outcomingRoute = new Route(outcomingRouteName, typeof(T));

            return new Subscriber(incomingRoute, outcomingRoute, _ => action());
        }

        internal static Subscriber Create<T, TK>(string incomingRouteName, string outcomingRouteName, Func<T, TK> action)
        {
            var incomingRoute = new Route(incomingRouteName, typeof(T));
            var outcomingRoute = new Route(outcomingRouteName, typeof(TK));

            return new Subscriber(incomingRoute, outcomingRoute, payload => action((T) payload));
        }

        public override string ToString()
        {
            var outputName = Outcoming?.ToString() ?? "void";
            return $"Subscriber({Incoming.ToString()} -> {outputName})";
        }
    }
}
