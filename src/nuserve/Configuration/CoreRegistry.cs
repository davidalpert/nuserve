using System;
using System.Linq;
using FubuCore.Configuration;
using FubuMVC.StructureMap;
using log4net;
using StructureMap.Configuration.DSL;
using FubuCore.Binding;
using System.Reflection;

namespace nuserve.Configuration
{
    public class CoreRegistry : Registry
    {
        /// <summary>
        /// Initializes a new instance of the CoreRegistry class.
        /// </summary>
        public CoreRegistry()
        {
            Scan(a =>
            {
                a.TheCallingAssembly();

                // wire up ISomething to Something
                a.WithDefaultConventions();

                // build up Settings on demand
                a.Convention<SettingsScanner>();
            });

            For<ILog>().Use(() => LogManager.GetLogger(typeof(IoC)));
        }
    }
}
