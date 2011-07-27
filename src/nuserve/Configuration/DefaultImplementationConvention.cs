using System;
using System.Linq;
using System.Text.RegularExpressions;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace nuserve.Configuration
{
    public class DefaultImplementationConvention : IRegistrationConvention
    {
        static Regex re = new Regex(@"^Default(.+)$");

        public void Process(Type type, Registry registry)
        {
            string typeName = type.Name;
            if (!typeName.StartsWith("Default") || type.IsInterface) return;

            string matchingInterfaceName = re.Replace(typeName, "I$1");
            Type matchingInterfaceType = type.GetInterfaces().FirstOrDefault(i => i.Name == matchingInterfaceName);

            if (matchingInterfaceType != null)
            {
                registry.For(matchingInterfaceType).Use(type);
            }
        }
    }
}
