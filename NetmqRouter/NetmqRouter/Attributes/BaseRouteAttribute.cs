using System;

namespace NetmqRouter.Attributes
{
    /// <summary>
    /// This attribute can be used for adding a prefix for every route inside the class,
    /// for example "baseRoute" + "specificROute" = "baseRoute/specificROute"
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class BaseRouteAttribute : Attribute
    {
        public string Name { get; }

        public BaseRouteAttribute(string name)
        {
            if(string.IsNullOrEmpty(name))
                throw new NetmqRouterException("Name of the route shouldn't be empty or null.");

            Name = name;
        }
    }
}
