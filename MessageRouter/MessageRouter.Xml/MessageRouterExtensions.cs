using System.Text;
using MessageRouter.Infrastructure;

namespace MessageRouter.Xml
{
    public static class MessageRouterExtensions
    {
    public static IMessageRouter RegisterJsonSerializer(this IMessageRouter router, Encoding encoding)
    {
        router.RegisterGeneralSerializer(new XmlObjectSerializer(encoding));
        return router;
    }
        
    public static IMessageRouter RegisterJsonSerializer(this IMessageRouter router)
    {
        router.RegisterGeneralSerializer(new XmlObjectSerializer());
        return router;
    }
    }
}