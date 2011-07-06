using System;
using StructureMap;

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
