using System.Text;
using MessageRouter.Infrastructure;

namespace MessageRouter.Json
{
    public static class MessageRouterExtensions
    {
        public static IMessageRouter RegisterJsonSerializer(this IMessageRouter router, Encoding encoding)
        {
            router.RegisterGeneralSerializer(new JsonObjectSerializer(encoding));
            return router;
        }
        
        public static IMessageRouter RegisterJsonSerializer(this IMessageRouter router)
        {
            router.RegisterGeneralSerializer(new JsonObjectSerializer());
            return router;
        }
    }
}