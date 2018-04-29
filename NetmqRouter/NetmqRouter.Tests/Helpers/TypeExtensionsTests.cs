using System;
using NetmqRouter.Helpers;
using NUnit.Framework;

namespace NetmqRouter.Tests.Helpers
{
    [TestFixture]
    public class TypeExtensionsTests
    {
        #region test classes

        class ClassA
        {

        }

        class ClassB : ClassA
        {

        }

        #endregion

        [TestCase(typeof(int), typeof(object), ExpectedResult = true)]
        [TestCase(typeof(string), typeof(object), ExpectedResult = true)]
        [TestCase(typeof(int), typeof(string), ExpectedResult = false)]
        [TestCase(typeof(ClassA), typeof(ClassB), ExpectedResult = false)]
        [TestCase(typeof(ClassB), typeof(ClassA), ExpectedResult = true)]
        [TestCase(typeof(void), typeof(ClassA), ExpectedResult = false)]
        [TestCase(typeof(ClassA), typeof(void), ExpectedResult = false)]
        public bool TypeComparison(Type typeA, Type typeB)
        {
            return typeA.IsSameOrSubclass(typeB);
        }
    }
}