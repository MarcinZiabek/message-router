using System;
using System.Reflection;

namespace MessageRouter.Helpers
{
    internal static class TypeExtensions
    {
        public static bool IsSameOrSubclass(this Type type, Type targetType)
        {
            return (type == targetType) || type.GetTypeInfo().IsSubclassOf(targetType);
        }
    }
}