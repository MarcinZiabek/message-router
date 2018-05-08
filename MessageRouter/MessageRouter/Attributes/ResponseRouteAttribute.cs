using System;
using MessageRouter.Exceptions;

namespace MessageRouter.Attributes
{
    /// <summary>
    /// This attribute can be used for annotating any method that should act as a route.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ResponseRouteAttribute : Attribute
    {
        public string Name { get; }

        /// <param name="name">Name of the input route</param>
        /// <exception cref="NetmqRouterException"></exception>
        public ResponseRouteAttribute(string name)
        {
            if(string.IsNullOrEmpty(name))
                throw new NetmqRouterException("Name of the route shouldn't be empty or null.");

            Name = name;
        }
    }
}
