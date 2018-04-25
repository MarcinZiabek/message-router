using System;

namespace NetmqRouter.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ResponseRouteAttribute : Attribute
    {
        public string Name { get; }

        public ResponseRouteAttribute(string name)
        {
            if(string.IsNullOrEmpty(name))
                throw new NetmqRouterException("Name of the route shouldn't be empty or null.");
            
            Name = name;
        }
    }
}
