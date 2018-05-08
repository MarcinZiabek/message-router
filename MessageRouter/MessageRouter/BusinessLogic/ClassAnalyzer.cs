using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MessageRouter.Attributes;
using MessageRouter.Exceptions;
using MessageRouter.Models;

namespace MessageRouter.BusinessLogic
{
    internal class ClassAnalyzer
    {
        internal static List<Subsriber> AnalyzeClass(object _object)
        {
            var baseRoute = GetBaseRoute(_object.GetType());
            var routes = FindRoutesInClass(_object).ToList();

            if(baseRoute != null)
                routes.ForEach(x => x.Incoming.Name = $"{baseRoute}/{x.Incoming.Name}");

            return routes;
        }

        private static string GetBaseRoute(Type type)
        {
            var attribute = type.GetTypeInfo().GetCustomAttributes(typeof(BaseRouteAttribute)).SingleOrDefault();
            return (attribute as BaseRouteAttribute)?.Name;
        }

        private static IEnumerable<Subsriber> FindRoutesInClass(object _object)
        {
            return _object
                .GetType()
                .GetTypeInfo()
                .DeclaredMethods
                .Select(x => AnalyzeMethod(_object, x))
                .Where(x => x != null);
        }

        private static Subsriber AnalyzeMethod(object _object, MethodInfo methodInfo)
        {
            var route = methodInfo.GetCustomAttribute(typeof(RouteAttribute)) as RouteAttribute;
            var responseRoute = methodInfo.GetCustomAttribute(typeof(ResponseRouteAttribute)) as ResponseRouteAttribute;

            if(responseRoute != null && route == null)
                throw new ConfigurationException($"Method {methodInfo.Name} with RouteResponse attribute does not have Route attribute assigned.");

            if (route == null)
                return null;

            if (methodInfo.GetParameters().Length > 1)
                throw new ConfigurationException($"Method {methodInfo.Name} cannot have more than one argument");

            var agrumentType = methodInfo
                .GetParameters()
                .FirstOrDefault()
                ?.ParameterType
                ?? typeof(void);

            return new Subsriber()
            {
                Incoming = new Route(route.Name, agrumentType),
                Outcoming = (responseRoute == null) ? null : new Route(responseRoute.Name, methodInfo.ReturnType),

                Method = payload => methodInfo.Invoke(_object, new[] { payload })
            };
        }
    }
}
