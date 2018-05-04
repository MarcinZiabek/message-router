using System;
using NetmqRouter.Exceptions;

namespace NetmqRouter.Attributes
{
    /// <summary>
    /// This attribute can be used for annotating any method that can return a response message.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class RouteAttribute : Attribute
    {
        public string Name { get; }

        /// <param name="name">Name of the response route</param>
        /// <exception cref="NetmqRouterException"></exception>
        public RouteAttribute(string name)
        {
            if(string.IsNullOrEmpty(name))
                throw new NetmqRouterException("Name of the route shouldn't be empty or null.");

            Name = name;
        }
    }
}
