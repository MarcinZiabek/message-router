using System;
using System.Collections.Generic;
using System.Text;

namespace NetmqRouter
{
    public class Route
    {
        public string Name { get; set; }
        public Action<object> Target { get; set; }
        public RouteDataType DataType { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Route route &&
                   Name == route.Name &&
                   DataType == route.DataType;
        }
    }
}
