using System;

namespace NetmqRouter.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RouteAttribute : Attribute
    {
        public string Name { get; }

        public RouteAttribute(string name)
        {
            Name = name;
        }
    }
}
