using System;
using System.IO;
using System.Reflection;
using NuGet.Server.Infrastructure;
using nuserve.Settings;

namespace nuserve.Infrastructure
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
}
