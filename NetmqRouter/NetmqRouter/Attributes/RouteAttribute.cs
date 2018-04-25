using System;

namespace NetmqRouter.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RouteAttribute : Attribute
    {
        public string Name { get; }

        public RouteAttribute(string name)
        {
            if(string.IsNullOrEmpty(name))
                throw new NetmqRouterException("Name of the route shouldn't be empty or null.");
            
            Name = name;
        }
    }
}
