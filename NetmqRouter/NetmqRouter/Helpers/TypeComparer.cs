using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("NetmqRouter.Tests")]

namespace NetmqRouter.Helpers
{
    internal class TypeComparer : IComparer<Type>
    {
        public int Compare(Type typeA, Type typeB)
        {
            if (typeA == null || typeB == null)
                return 0;

            if (typeA.IsSubclassOf(typeB))
                return 1;

            if (typeB.IsSubclassOf(typeA))
                return -1;

            return 0;
        }
    }
}