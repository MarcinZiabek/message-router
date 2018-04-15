using System;

namespace NetmqRouter.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ResponseRouteAttribute : Attribute
    {
        public string Name { get; }

        public ResponseRouteAttribute(string name)
        {
            Name = name;
        }
    }
}
