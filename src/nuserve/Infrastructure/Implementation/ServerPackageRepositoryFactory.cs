using System;
using System.IO;
using System.Reflection;
using NuGet.Server.Infrastructure;
using nuserve.Settings;

namespace nuserve.Infrastructure.Implementation
{
    public class ServerPackageRepositoryFactory : IServerPackageRepositoryFactory
    {
        IPathResolutionStrategy pathResolutionStrategy;

        /// <summary>
        /// Initializes a new instance of the ServerPackageRepositoryFactory class.
        /// </summary>
        /// <param name="settings"></param>
        public ServerPackageRepositoryFactory(IPathResolutionStrategy pathResolutionStrategy)
        {
            this.pathResolutionStrategy = pathResolutionStrategy;
        }

        public IServerPackageRepository BuildServerPackageRepository()
        {
            PackageUtility.ResolveVirtualPathStrategy = s => pathResolutionStrategy.ResolveToPhysicalPath(s);

            bool directoryNotFound = Directory.Exists(PackageUtility.PackagePhysicalPath) == false;
            try
            {
                Directory.CreateDirectory(PackageUtility.PackagePhysicalPath);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(String.Format("Could not create a part of '{0}'", PackageUtility.PackagePhysicalPath), ex);
            }

            return new ServerPackageRepository(PackageUtility.PackagePhysicalPath);
        }
    }
}
