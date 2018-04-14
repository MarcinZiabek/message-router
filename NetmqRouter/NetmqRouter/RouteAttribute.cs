using System;
using System.Collections.Generic;
using System.Text;

namespace NetmqRouter
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RouteAttribute : Attribute
    {
        public string Name { get; }

        public RouteAttribute(string name)
        {
            Name = name;
        }
    }
}
