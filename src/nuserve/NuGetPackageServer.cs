using System;
using Nancy.Hosting.Self;
using nuserve.Settings;
using log4net;

namespace nuserve
{
    public class NuGetPackageServer
    {
        EndpointSettings settings;

        private ILog log = null;

        private NancyHost host = null;

        /// <summary>
        /// Initializes a new instance of the NuGetPackageServer class.
        /// </summary>
        /// <param name="settings"></param>
        public NuGetPackageServer(EndpointSettings settings, ILog log)
        {
            this.log = log;
            this.settings = settings;
        }

        public void Start()
        {
            var uri = new UriBuilder(settings.Protocol, settings.HostName, settings.Port).Uri;

            ensure_host_is_stopped();

            host = new NancyHost(uri);
            host.Start();

            log.InfoFormat("Nancy now listening at: {0}", uri);
            log.Info("nuserve started.");
        }

        public void Stop()
        {
            ensure_host_is_stopped();

            log.Info("nuserve stopped.");
        }

        private void ensure_host_is_stopped()
        {
            if (host != null)
            {
                host.Stop();
                host = null;
            }
        }

        public void Pause()
        {
            log.Info("nuserve paused.");
        }

        public void Continue()
        {
            log.Info("nuserve resumed.");
        }
    }
}
