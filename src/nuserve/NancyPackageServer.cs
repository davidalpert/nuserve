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
    public interface InProcessPackageServer
    {
        void Start();
        void Stop();
        void Pause();
        void Continue();

        bool IsListening { get; }
        Uri EndpointUri { get; }
    }

    public class NancyPackageServer : InProcessPackageServer
    {
        private DataServiceHost serviceHost;
        EndpointSettings settings;

        private ILog log = null;

        private NancyHost host = null;

        /// <summary>
        /// Initializes a new instance of the NuGetPackageServer class.
        /// </summary>
        /// <param name="settings"></param>
        public NancyPackageServer(EndpointSettings settings, ILog log)
        {
            this.log = log;
            this.settings = settings;
        }

        public void Start()
        {
            Stop();

            // configure the PackageUtility to serve packages from our app's local ~/packages/ folder
            var exeRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            PackageUtility.PackagePhysicalPath = Path.Combine(exeRoot, "Packages");
            PackageUtility.ResolveAppRelativePathStrategy = s => Path.Combine(exeRoot, Path.Combine("Packages", s.TrimStart('~', '/')));

            // form the base URI from our EndpointSettings
            var baseUri = new UriBuilder(settings.Protocol, settings.HostName, settings.Port).Uri;

            // form a relative uri for the odata service
            var packages_odata_uri = new UriBuilder(baseUri);
            packages_odata_uri.Path = "/nuget/packages";

            // start up a DataServiceHost to host our PackagesOData service 
            Uri[] uriArray = { packages_odata_uri.Uri };
            Type serviceType = typeof(Packages);
            serviceHost = new DataServiceHost(serviceType, uriArray);
            serviceHost.Open();

            // start up Nancy to host our non-odata routes
            host = new NancyHost(baseUri);
            host.Start();

            log.InfoFormat("Packages OData feed now listenting at: {0}", packages_odata_uri);
            log.InfoFormat("Nancy now listening at: {0}", baseUri);
            log.Info("nuserve started.");

            EndpointUri = baseUri;
            IsListening = true;
        }

        public void Stop()
        {
            if (host != null)
            {
                host.Stop();
                IsListening = false;
                EndpointUri = null;
                host = null;
            }

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

        public bool IsListening { get; private set; }

        public Uri EndpointUri { get; private set; }
    }
}
