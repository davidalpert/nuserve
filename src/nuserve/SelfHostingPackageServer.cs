using System;
using System.Data.Services;
using log4net;
using Nancy.Hosting.Self;
using NuGet.Server.DataServices;
using nuserve.Settings;

namespace nuserve
{
    public class SelfHostingPackageServer : ISelfHostingPackageServer, IDisposable
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

        public bool IsListening
        {
            get
            {
                bool nancyHostIsAlive = host != null;
                bool serviceHostIsAlive = serviceHost != null;

                return nancyHostIsAlive && serviceHostIsAlive;
            }
        }

        public void Start()
        {
            start_OData_package_service();

            start_Nancy_server_to_handle_non_OData_routes();

            log.Info("nuserve started.");
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

        public void Dispose()
        {
            Stop();
        }

        private void start_OData_package_service()
        {
            stop_OData_package_service();

            var packages_odata_uri = new Uri(settings.PackageListUri);

            Uri[] uriArray = { packages_odata_uri };
            Type serviceType = typeof(Packages);
            serviceHost = new DataServiceHost(serviceType, uriArray);

            serviceHost.Open();

            log.InfoFormat("Packages OData feed now listening at: {0}", packages_odata_uri);
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

        private void start_Nancy_server_to_handle_non_OData_routes()
        {
            stop_Nancy_server();

            var mgrUri = new Uri(settings.PackageManagerUri);
            host = new NancyHost(mgrUri);
            host.Start();
            log.InfoFormat("Nancy now listening at: {0}", mgrUri);
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
    }
}
