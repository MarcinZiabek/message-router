using System;
using System.Collections.Generic;
using MessageRouter.Models;

namespace MessageRouter.Helpers
{
    /// <summary>
    /// This class can be used for comparing serializers. Rules: a general serializer is always considered
    /// as "lower" in comparison to type a type serializer. If both serializers has the same IsGeneral flag,
    /// target types are compared.
    /// </summary>
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