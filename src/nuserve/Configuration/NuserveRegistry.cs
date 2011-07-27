using System;
using FubuCore.StructureMap;
using log4net;
using NuGet;
using NuGet.Server.Infrastructure;
using StructureMap.Configuration.DSL;
using nuserve.Infrastructure;

namespace nuserve.Configuration
{
    public class NuserveRegistry : Registry
    {
        /// <summary>
        /// Registers nuserve dependencies with the StructureMap container
        /// configured in the <see cref="IoC"/> class.
        /// </summary>
        public NuserveRegistry()
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

            // these dependencies are required by the Nuget.Server push command
            For<IHashProvider>().Use(new CryptoHashProvider());
            For<IServerPackageRepository>().Singleton().Use(ctx => ctx.GetInstance<IServerPackageRepositoryFactory>().BuildServerPackageRepository());
            For<IPackageAuthenticationService>().Use<PackageAuthenticationService>();
        }
    }
}
