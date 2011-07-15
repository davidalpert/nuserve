using System;
using System.IO;
using Nancy;
using NuGet;
using nuserve.Infrastructure;
using nuserve.Settings;
using NuGet.Server.Infrastructure;

namespace nuserve
{
    public interface IZipPackageFactory
    {
        IPackage CreatePackageFromRequestBody(Stream requestStream);
    }

    public class ZipPackageFactory : IZipPackageFactory
    {
        public IPackage CreatePackageFromRequestBody(Stream requestStream)
        {
            return new ZipPackage(requestStream);
        }
    }

    public class PackageFilesModule : NancyModule
    {
        IAuthorizePackageOperations authService;
        IServerPackageRepository packageRepo;
        IZipPackageFactory zipPackageFactory;

        /// <summary>
        /// Initializes a new instance of the TestModule class.
        /// </summary>
        /// <param name="authService"></param>
        /// <param name="settings"></param>
        public PackageFilesModule(IAuthorizePackageOperations authService, IServerPackageRepository packageRepo, IZipPackageFactory zipPackageFactory)
            : base("/PackageFiles")
        {
            this.packageRepo = packageRepo;
            this.authService = authService;
            this.zipPackageFactory = zipPackageFactory;

            DefineHandlers();
        }

        private void DefineHandlers()
        {
            Post["/{apiKey}/nupkg"] = parameters =>
            {
                // Get the api key from the route
                string apiKey = parameters.apiKey;

                // Build the package from the request body
                var package = zipPackageFactory.CreatePackageFromRequestBody(Request.Body);

                // Make sure they can access this package
                return Authenticate(apiKey, package.Id, () =>
                {
                    try
                    {
                        packageRepo.AddPackage(package);
                        return new Nancy.Response { StatusCode = HttpStatusCode.OK };
                    }
                    catch
                    {
                        return new Nancy.Response { StatusCode = HttpStatusCode.InternalServerError };
                    }
                });
            };
        }

        private Nancy.Response Authenticate(string apiKey, string id, Func<Nancy.Response> authenticatedContinuation)
        {
            bool isAuthenticated = authService.ClientCanPublishPackage(apiKey, id);

            if (isAuthenticated)
                return authenticatedContinuation();

            return new Nancy.Response
            {
                StatusCode = HttpStatusCode.Unauthorized,
                Contents = GetContents("Access denied for package '{0}'.", id)
            };
        }

        protected static Action<Stream> GetContents(string contents)
        {
            return stream =>
            {
                var writer = new StreamWriter(stream) { AutoFlush = true };
                writer.Write(contents);
            };
        }

        protected static Action<Stream> GetContents(string fmt, params object[] args)
        {
            return stream =>
            {
                var content = String.Format(fmt, args);
                var innerAction = GetContents(content);
                innerAction(stream);
            };
        }
    }
}
