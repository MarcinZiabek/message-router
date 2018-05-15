namespace MessageRouter.Models
{
    public struct SerializedMessage
    {
        public readonly string RouteName;
        public readonly byte[] Data;

        public SerializedMessage(string routeName, byte[] data)
        {
            RouteName = routeName;
            Data = data;
        }
        
        public override bool Equals(object obj)
        {
            return obj is SerializedMessage r &&
                   r.RouteName == RouteName &&
                   r.Data == Data;
        }
    }
}