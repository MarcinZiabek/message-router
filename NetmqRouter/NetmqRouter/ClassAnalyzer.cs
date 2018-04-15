using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using NetmqRouter.Attributes;

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
                routes.ForEach(x => x.IncomingRouteName = $"{baseRoute}/{x.IncomingRouteName}");

            return routes;
        }

        internal static string GetBaseRoute(Type type)
        {
            var attribute = type.GetCustomAttributes(typeof(BaseRouteAttribute)).SingleOrDefault();
            return (attribute as BaseRouteAttribute)?.Name;
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
            var route = Attribute.GetCustomAttribute(methodInfo, typeof(RouteAttribute)) as RouteAttribute;
            var responseRoute = Attribute.GetCustomAttribute(methodInfo, typeof(ResponseRouteAttribute)) as ResponseRouteAttribute;
            var isAsync = Attribute.GetCustomAttribute(methodInfo, typeof(AsyncRouteAttribute)) != null;

            if (route == null)
                return null;

            if (methodInfo.GetParameters().Length > 1)
                throw new NetmqRouterException("Route method cannot have more than one argument");

            var agrumentType = methodInfo.GetParameters().FirstOrDefault()?.ParameterType;
            var returnType = CovertTypeToRouteDataType(methodInfo.ReturnType);
            var routeDataType = CovertTypeToRouteDataType(agrumentType);

            return new Route()
            {
                Object = _object,
                Method = methodInfo,

                IsAsync = isAsync,

                IncomingRouteName = route.Name,
                IncomingDataType = routeDataType,
                
                OutcomingRouteName = responseRoute?.Name,
                OutcomingDataType = returnType
            };
        }

        internal static RouteDataType CovertTypeToRouteDataType(Type type)
        {
            if (type == null || type == typeof(void))
                return RouteDataType.Void;

            if (type == typeof(byte[]))
                return RouteDataType.RawData;

            if (type == typeof(string))
                return RouteDataType.Text;

            return RouteDataType.Object;
        }
    }
}
