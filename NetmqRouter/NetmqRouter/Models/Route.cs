using System;

namespace NetmqRouter.Models
{
    internal class Route
    {
        public string Name { get; set; }
        public Type DataType { get; set; }

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