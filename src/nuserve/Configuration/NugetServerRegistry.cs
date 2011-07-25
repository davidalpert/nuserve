using System;
using NuGet;
using NuGet.Server.Infrastructure;
using StructureMap.Configuration.DSL;
using System.IO;
using System.Reflection;
using log4net;
using FubuMVC.StructureMap;
using nuserve.Settings;

namespace nuserve.Configuration
{
    public class ServerPackageRepositoryFactory : IServerPackageRepositoryFactory
    {
        RepositorySettings settings;

        /// <summary>
        /// Initializes a new instance of the ServerPackageRepositoryFactory class.
        /// </summary>
        /// <param name="settings"></param>
        public ServerPackageRepositoryFactory(RepositorySettings settings)
        {
            this.settings = settings;
        }

        public IServerPackageRepository BuildServerPackageRepository()
        {
            string pathToRepo = settings.PathToServerPackageRepository;

            if (String.IsNullOrWhiteSpace(pathToRepo))
                throw new InvalidOperationException("Cannot point nuserve's local repository at a null or empty path");

            if (pathToRepo.StartsWith("~/"))
            {
                var exeRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                PackageUtility.PackagePhysicalPath = Path.Combine(exeRoot, pathToRepo.TrimStart('~', '/'));
            }
            else
            {
                PackageUtility.PackagePhysicalPath = pathToRepo;
            }

            bool directoryNotFound = Directory.Exists(PackageUtility.PackagePhysicalPath) == false;
            if (directoryNotFound)
                throw new InvalidOperationException(String.Format("'{0}' is not a valid path!", PackageUtility.PackagePhysicalPath));

            PackageUtility.ResolveAppRelativePathStrategy = s => Path.Combine(PackageUtility.PackagePhysicalPath, s.TrimStart('~', '/'));

            return new ServerPackageRepository(PackageUtility.PackagePhysicalPath);
        }
    }

    public interface IServerPackageRepositoryFactory
    {
        IServerPackageRepository BuildServerPackageRepository();
    }

    public class NugetServerRegistry : Registry
    {
        /// <summary>
        /// Initializes a new instance of the NugetServerRegistry class.
        /// </summary>
        public NugetServerRegistry()
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

            // HACK: this doesn't belong here... the PackagePhysicalPath should provided by 
            //       an injectable dependency, but that's deeper into nuget source than I want to go.
            configure_PackageUtility_to_serve_packages_from_our_apps_local_packages_folder();

            For<IHashProvider>().Use(new CryptoHashProvider());
            For<IServerPackageRepository>().Singleton().Use(ctx => ctx.GetInstance<IServerPackageRepositoryFactory>().BuildServerPackageRepository());
            For<IPackageAuthenticationService>().Use<PackageAuthenticationService>();
        }

        private static void configure_PackageUtility_to_serve_packages_from_our_apps_local_packages_folder()
        {
        }
    }
}
