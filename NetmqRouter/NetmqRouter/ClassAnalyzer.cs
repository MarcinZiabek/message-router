using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NetmqRouter
{
    public class ClassAnalyzer
    {
        public static List<Route> AnalyzeClass(object _object)
        {
            var baseRoute = GetBaseRoute(_object.GetType());
            var routes = FindRoutesInClass(_object).ToList();

            if(baseRoute != null)
                routes.ForEach(x => x.Name = $"{baseRoute}/{x.Name}");

            return routes;
        }

        private static string GetBaseRoute(Type type)
        {
            var attribute = type.GetCustomAttributes(typeof(RouteAttribute)).SingleOrDefault();
            return (attribute as RouteAttribute)?.Name;
        }

        private static IEnumerable<Route> FindRoutesInClass(object _object)
        {
            return _object
                .GetType()
                .GetMethods()
                .Select(x =>
                {
                    var attribute = Attribute.GetCustomAttribute(x, typeof(RouteAttribute)) as RouteAttribute;

                    if (attribute == null)
                        return null;

                    if(x.GetParameters().Length != 1)
                        throw new Exception("Receiver method can have only one argument");

                    var agrumentType = x.GetParameters()[0].ParameterType;
                    var type = RouteDataType.Object;

                    if (agrumentType == typeof(byte[]))
                        type = RouteDataType.RawData;

                    if (agrumentType == typeof(string))
                        type = RouteDataType.Text;

                    return new Route()
                    {
                        Name = attribute.Name,
                        DataType = type
                    };
                })
                .Where(x => x != null);
        }
    }
}
