using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("NetmqRouter.Tests")]

namespace NetmqRouter
{
    public class ClassAnalyzer
    {
        internal static List<Route> AnalyzeClass(object _object)
        {
            var baseRoute = GetBaseRoute(_object.GetType());
            var routes = FindRoutesInClass(_object).ToList();

            if(baseRoute != null)
                routes.ForEach(x => x.Name = $"{baseRoute}/{x.Name}");

            return routes;
        }

        internal static string GetBaseRoute(Type type)
        {
            var attribute = type.GetCustomAttributes(typeof(RouteAttribute)).SingleOrDefault();
            return (attribute as RouteAttribute)?.Name;
        }

        internal static IEnumerable<Route> FindRoutesInClass(object _object)
        {
            return _object
                .GetType()
                .GetMethods()
                .Select(x => AnalyzeMethod(_object, x))
                .Where(x => x != null);
        }

        internal static Route AnalyzeMethod(object _object, MethodInfo methodInfo)
        {
            var attribute = Attribute.GetCustomAttribute(methodInfo, typeof(RouteAttribute)) as RouteAttribute;

            if (attribute == null)
                return null;

            if (methodInfo.GetParameters().Length > 1)
                throw new NetmqRouterException("Route method cannot have more than one argument");

            var agrumentType = methodInfo.GetParameters().FirstOrDefault()?.ParameterType;
            var routeDataType = CovertTypeToRouteDataType(agrumentType);

            return new Route()
            {
                Name = attribute.Name,
                Target = arg =>
                {
                    if(routeDataType == RouteDataType.Event)
                        methodInfo.Invoke(_object, new object[0]);

                    else
                        methodInfo.Invoke(_object, new[] {arg});
                },
                DataType = routeDataType
            };
        }

        internal static RouteDataType CovertTypeToRouteDataType(Type type)
        {
            if (type == null)
                return RouteDataType.Event;

            if (type == typeof(byte[]))
                return RouteDataType.RawData;

            if (type == typeof(string))
                return RouteDataType.Text;

            return RouteDataType.Object;
        }
    }
}
