namespace MessageRouter.Models
{
    public class Message
    {
        internal readonly string RouteName;
        internal readonly object Payload;

        /// <param name="routeName">Name of the target route.</param>
        /// <param name="payload">Object of type that is compatible with the route's target type.</param>
        internal Message(string routeName, object payload)
        {
            RouteName = routeName;
            Payload = payload;
        }

        public override bool Equals(object obj)
        {
            return obj is Message r &&
                   r.RouteName == RouteName &&
                   r.Payload == Payload;
        }
    }
}
