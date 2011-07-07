using System;
using System.IO;
using Nancy;
using NuGet;
using nuserve.Infrastructure;
using nuserve.Settings;
using NuGet.Server.Infrastructure;

namespace nuserve
{
    public class TestModule : NancyModule
    {
        IAuthorizePackageOperations authService;
        IServerPackageRepository packageRepo;

        /// <summary>
        /// Initializes a new instance of the TestModule class.
        /// </summary>
        /// <param name="authService"></param>
        /// <param name="settings"></param>
        public TestModule(IAuthorizePackageOperations authService, IServerPackageRepository packageRepo)
        {
            this.packageRepo = packageRepo;
            this.authService = authService;

            DefineHandlers();
        }

        private void DefineHandlers()
        {
            Get["/"] = parameters =>
            {
                Console.WriteLine("caught request");

                return View["staticview"];
            };

            Post["/PackageFiles/{apiKey}/nupkg"] = parameters =>
            {
                // Get the api key from the route
                string apiKey = parameters.apiKey;

                // Build the package from the request body
                var package = new ZipPackage(Request.Body);

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
