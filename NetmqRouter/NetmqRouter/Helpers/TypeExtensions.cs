using System;

namespace NetmqRouter.Helpers
{
    internal static class TypeExtensions
    {
        public static bool IsEqualOrSubclass(this Type type, Type targetType)
        {
            return (type == targetType) || type.IsSubclassOf(targetType);
        }
    }
}