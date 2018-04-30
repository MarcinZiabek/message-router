using System;
using System.Collections.Generic;
using NetmqRouter.Models;

namespace NetmqRouter.Helpers
{
    internal class SerializerComparer : IComparer<Serializer>
    {
        private readonly IComparer<Type> _typeComparer;

        public SerializerComparer()
        {
            _typeComparer = new TypeComparer();
        }

        public int Compare(Serializer serializerA, Serializer serializerB)
        {
            if (serializerA == null || serializerB == null)
                return 0;

            if (serializerA.IsGeneral && !serializerB.IsGeneral)
                return -1;

            if (!serializerA.IsGeneral && serializerB.IsGeneral)
                return 1;

            return _typeComparer.Compare(
                serializerA.TargetType,
                serializerB.TargetType);
        }
    }
}