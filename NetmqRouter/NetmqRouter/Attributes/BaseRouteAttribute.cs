using System;
using System.Collections.Generic;
using System.Text;

namespace NetmqRouter.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BaseRouteAttribute : Attribute
    {
        public string Name { get; }

        public BaseRouteAttribute(string name)
        {
            Name = name;
        }
    }
}
