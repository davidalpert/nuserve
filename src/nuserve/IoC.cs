using System;
using StructureMap;
using log4net;
using Microsoft.Practices.ServiceLocation;
using StructureMap.ServiceLocatorAdapter;

namespace nuserve
{
    public class IoC
    {
        public static void Bootstrap()
        {
            ObjectFactory.Initialize(x =>
            {
                x.Scan(a =>
                {
                    a.AssemblyContainingType<Program>();
                    a.LookForRegistries();
                });
            });

            ObjectFactory.AssertConfigurationIsValid();
        }

        public static T Get<T>()
        {
            return ObjectFactory.Container.GetInstance<T>();
        }

        public static object Container { get { return ObjectFactory.Container; } }
    }
}
