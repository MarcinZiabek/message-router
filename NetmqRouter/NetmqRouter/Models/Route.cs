using System;

namespace NetmqRouter.Models
{
    internal class Route
    {
        public string Name { get; set; }
        public Type DataType { get; set; }

        /// <param name="name">Name of the route</param>
        /// <param name="dataType">Type that will be expected to receive</param>
        public Route(string name, Type dataType)
        {
            Name = name;
            DataType = dataType;
        }

        public override bool Equals(object obj)
        {
            return obj is Route r &&
                   r.Name == Name &&
                   r.DataType == DataType;
        }
    }
}