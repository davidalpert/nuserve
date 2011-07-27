using System;
using StructureMap;
using nuserve.Configuration;

namespace nuserve
{
    public class IoC
    {
        static bool initialized;
        static object s_lock = new Object();

        public static void Bootstrap()
        {
            if (!initialized)
            {
                lock (s_lock)
                {
                    if (!initialized)
                    {
                        ObjectFactory.Initialize(x =>
                        {
                            x.Scan(a =>
                            {
                                a.AssemblyContainingType<NuserveRegistry>();
                                a.LookForRegistries();
                            });
                        });

                        try
                        {
                            ObjectFactory.AssertConfigurationIsValid();
                        }
                        catch (Exception ex)
                        {
                            //Console.WriteLine(ObjectFactory.WhatDoIHave());
                            throw new InvalidOperationException("StructureMap has been passed an invalid configuration.", ex);
                        }

                        initialized = true;
                    }
                }
            }
        }

        public static T Get<T>()
        {
            return ObjectFactory.Container.GetInstance<T>();
        }

        public static object Container { get { return ObjectFactory.Container; } }
    }
}
