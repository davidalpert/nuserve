using System;
using NuGet;
using NuGet.Server.Infrastructure;
using StructureMap.Configuration.DSL;
using System.IO;
using System.Reflection;

namespace nuserve.Configuration
{
    public class NugetServerRegistry : Registry
    {
        /// <summary>
        /// Initializes a new instance of the NugetServerRegistry class.
        /// </summary>
        public NugetServerRegistry()
        {
            // HACK: this doesn't belong here... the PackagePhysicalPath should provided by 
            //       an injectable dependency, but that's deeper into nuget source than I want to go.
            configure_PackageUtility_to_serve_packages_from_our_apps_local_packages_folder();

            For<IHashProvider>().Use(new CryptoHashProvider());
            For<IServerPackageRepository>().Use(new ServerPackageRepository(PackageUtility.PackagePhysicalPath));
            For<IPackageAuthenticationService>().Use<PackageAuthenticationService>();
        }

        private static void configure_PackageUtility_to_serve_packages_from_our_apps_local_packages_folder()
        {
            var exeRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            PackageUtility.PackagePhysicalPath = Path.Combine(exeRoot, "Packages");
            PackageUtility.ResolveAppRelativePathStrategy = s => Path.Combine(exeRoot, Path.Combine("Packages", s.TrimStart('~', '/')));
        }
    }
}
