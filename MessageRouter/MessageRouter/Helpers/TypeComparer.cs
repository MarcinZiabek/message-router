using System;
using System.Collections.Generic;
using System.Reflection;

namespace MessageRouter.Helpers
{
    /// <summary>
    /// This class can be used for comparing types in order to create list of classes
    /// ordered by inheritance relationships. More general classes are considered as "lower",
    /// more specific classes are "greater".
    /// </summary>
    internal class TypeComparer : IComparer<Type>
    {
        public int Compare(Type typeA, Type typeB)
        {
            if (typeA == null || typeB == null)
                return 0;

            if (typeA.GetTypeInfo().IsSubclassOf(typeB))
                return 1;

            if (typeB.GetTypeInfo().IsSubclassOf(typeA))
                return -1;

            return 0;
        }
    }
}