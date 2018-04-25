using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using NetmqRouter.Attributes;
using NetmqRouter.Models;

[assembly: InternalsVisibleTo("NetmqRouter.Tests")]

namespace NetmqRouter.BusinessLogic
{
    internal class ClassAnalyzer
    {
        internal static List<RouteSubsriber> AnalyzeClass(object _object)
        {
            var baseRoute = GetBaseRoute(_object.GetType());
            var routes = FindRoutesInClass(_object).ToList();

            if(baseRoute != null)
                routes.ForEach(x => x.Incoming.Name = $"{baseRoute}/{x.Incoming.Name}");

            return routes;
        }

        internal static string GetBaseRoute(Type type)
        {
            var attribute = type.GetCustomAttributes(typeof(BaseRouteAttribute)).SingleOrDefault();
            return (attribute as BaseRouteAttribute)?.Name;
        }

        internal static IEnumerable<RouteSubsriber> FindRoutesInClass(object _object)
        {
            return _object
                .GetType()
                .GetMethods()
                .Select(x => AnalyzeMethod(_object, x))
                .Where(x => x != null);
        }

        internal static RouteSubsriber AnalyzeMethod(object _object, MethodInfo methodInfo)
        {
            var route = Attribute.GetCustomAttribute(methodInfo, typeof(RouteAttribute)) as RouteAttribute;
            var responseRoute = Attribute.GetCustomAttribute(methodInfo, typeof(ResponseRouteAttribute)) as ResponseRouteAttribute;

            if (route == null)
                return null;

            if (methodInfo.GetParameters().Length > 1)
                throw new NetmqRouterException("RouteSubsriber method cannot have more than one argument");

            var agrumentType = methodInfo.GetParameters().FirstOrDefault()?.ParameterType;

            return new RouteSubsriber()
            {
                Incoming = new Route(route.Name, agrumentType),
                Outcoming = new Route(responseRoute?.Name, methodInfo.ReturnType),
                
                Method = payload => methodInfo?.Invoke(_object, new[] { payload })
            };
        }
    }
}
