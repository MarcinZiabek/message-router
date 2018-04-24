using System;

namespace NetmqRouter
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
    }
}