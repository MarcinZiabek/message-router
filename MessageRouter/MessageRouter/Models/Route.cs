using System;

namespace MessageRouter.Models
{
    public class Route
    {
        internal string Name { get; set; }
        internal Type DataType { get; set; }

        /// <param name="name">Name of the route</param>
        /// <param name="dataType">Type that will be expected to receive</param>
        internal Route(string name, Type dataType)
        {
            Name = name;
            DataType = dataType;
        }

        /// <param name="name">Name of the route</param>
        internal Route(string name)
        {
            Name = name;
            DataType = typeof(void);
        }

        public string ToString()
        {
            var typeName = DataType == typeof(void) ? "Event" : DataType.Name;
            return $"Route({Name}, {typeName})";
        }
        
        public override bool Equals(object obj)
        {
            return obj is Route r &&
                   r.Name == Name &&
                   r.DataType == DataType;
        }
    }
}