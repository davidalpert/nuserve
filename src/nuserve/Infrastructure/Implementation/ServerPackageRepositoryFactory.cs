using System;
using System.IO;
using System.Reflection;
using NuGet.Server.Infrastructure;
using nuserve.Settings;

namespace nuserve.Infrastructure.Implementation
{
    public class ServerPackageRepositoryFactory : IServerPackageRepositoryFactory
    {
        RepositorySettings settings;
        IPathResolutionStrategy pathResolutionStrategy;

        /// <summary>
        /// Initializes a new instance of the ServerPackageRepositoryFactory class.
        /// </summary>
        /// <param name="settings"></param>
        public ServerPackageRepositoryFactory(RepositorySettings settings, IPathResolutionStrategy pathResolutionStrategy)
        {
            this.pathResolutionStrategy = pathResolutionStrategy;
            this.settings = settings;
        }

        public IServerPackageRepository BuildServerPackageRepository()
        {
            string pathToRepo = settings.PathToServerPackageRepository;

            if (String.IsNullOrWhiteSpace(pathToRepo))
                throw new InvalidOperationException("Cannot point nuserve's local repository at a null or empty path");

            PackageUtility.PackagePhysicalPath = pathResolutionStrategy.ResolveToPhysicalPath(pathToRepo);

            bool directoryNotFound = Directory.Exists(PackageUtility.PackagePhysicalPath) == false;
            try
            {
                Directory.CreateDirectory(PackageUtility.PackagePhysicalPath);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(String.Format("Could not create a part of '{0}'", PackageUtility.PackagePhysicalPath), ex);
            }

            PackageUtility.ResolveAppRelativePathStrategy = s => Path.Combine(PackageUtility.PackagePhysicalPath, s.TrimStart('~', '/'));

            return new ServerPackageRepository(PackageUtility.PackagePhysicalPath);
        }
    }
}
