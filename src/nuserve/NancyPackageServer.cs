using System;
using Nancy.Hosting.Self;
using nuserve.Settings;
using log4net;
using NuGet.Server.Infrastructure;
using System.IO;
using System.Reflection;
using NuGet.Server.DataServices;
using System.Data.Services;

namespace nuserve
{
    public class SelfHostingPackageServer : ISelfHostingPackageServer
    {
        private DataServiceHost serviceHost;
        private NancyHost host = null;
        private ILog log = null;

        EndpointSettings settings;

        /// <summary>
        /// Initializes a new instance of the NuGetPackageServer class.
        /// </summary>
        /// <param name="settings"></param>
        public SelfHostingPackageServer(EndpointSettings settings, ILog log)
        {
            this.settings = settings;
            this.log = log;
        }

        private static void configure_PackageUtility_to_serve_packages_from_our_apps_local_packages_folder()
        {
            var exeRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            PackageUtility.PackagePhysicalPath = Path.Combine(exeRoot, "Packages");
            PackageUtility.ResolveAppRelativePathStrategy = s => Path.Combine(exeRoot, Path.Combine("Packages", s.TrimStart('~', '/')));
        }

        private Uri build_Packages_OData_feed_uri(Uri baseEndpoint)
        {
            var packages_odata_uri = new UriBuilder(baseEndpoint);
            packages_odata_uri.Path = "/nuget/packages";
            return packages_odata_uri.Uri;
        }

        private void start_OData_package_service(Uri baseUri)
        {
            stop_OData_package_service();

            var packages_odata_uri = build_Packages_OData_feed_uri(baseUri);

            Uri[] uriArray = { packages_odata_uri };
            Type serviceType = typeof(Packages);
            serviceHost = new DataServiceHost(serviceType, uriArray);

            serviceHost.Open();

            log.InfoFormat("Packages OData feed now listenting at: {0}", packages_odata_uri);
        }

        private void stop_OData_package_service()
        {
            if (serviceHost != null)
            {
                log.Debug("Packages OData feed is listening.");
                log.Debug("Stopping Packages OData feed...");

                using (serviceHost)
                {
                    serviceHost.Close();
                }
                serviceHost = null;

                log.Info("Packages OData feed stopped.");
            }
        }

        private void start_Nancy_server_to_handle_non_OData_routes(Uri baseUri)
        {
            stop_Nancy_server();

            host = new NancyHost(baseUri);
            host.Start();
            log.InfoFormat("Nancy now listening at: {0}", EndpointUri);
        }

        private void stop_Nancy_server()
        {
            if (host != null)
            {
                log.Debug("Nancy is listening.");
                log.Debug("Stopping Nancy...");

                host.Stop();
                host = null;

                log.Info("Nancy stopped.");
            }
        }

        public void Start()
        {
            configure_PackageUtility_to_serve_packages_from_our_apps_local_packages_folder();

            EndpointUri = settings.GetEndpointUri();

            start_OData_package_service(EndpointUri);

            start_Nancy_server_to_handle_non_OData_routes(EndpointUri);

            log.Info("nuserve started.");

            EndpointUri = EndpointUri;
        }

        public void Stop()
        {
            stop_OData_package_service();

            stop_Nancy_server();

            log.Info("nuserve is stopped.");
        }

        public void Pause()
        {
            host.Stop();
        }

        public void Continue()
        {
            host.Start();
        }

        public bool IsListening
        {
            get
            {
                bool nancyHostIsAlive = host != null;
                bool serviceHostIsAlive = serviceHost != null;

                return nancyHostIsAlive && serviceHostIsAlive;
            }
        }

        public Uri EndpointUri { get; private set; }
    }
}
