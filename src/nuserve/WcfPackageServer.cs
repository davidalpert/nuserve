using System;
using System.Linq;
using System.IO;
using log4net.Config;
using Topshelf;
using FubuCore.Binding;
using System.ServiceModel;
using NuGet.Server;
using NuGet.Server.DataServices;
using System.Data.Services;
using System.ServiceProcess;
using System.ServiceModel.Web;
using System.ServiceModel.Description;

namespace nuserve
{
    public class WcfPackageServer
    {
        private ServiceHost serviceHost;

        public WcfPackageServer()
        {
        }

        public void Start()
        {
            Stop();

            Uri uri = new UriBuilder("http", "localhost", 5656).Uri;

            // Create a ServiceHost for the CalculatorService type and 
            // provide the base address.
            serviceHost = new ServiceHost(typeof(PackageService));

            var webHttpBinding = new WebHttpBinding(WebHttpSecurityMode.None);
            serviceHost.AddServiceEndpoint(typeof(PackageService), webHttpBinding, uri);

            // Open the ServiceHostBase to create listeners and start 
            // listening for messages.
            serviceHost.Open();

            Console.WriteLine("Listenting at: {0}", uri);
        }

        public void Stop()
        {
            if (serviceHost != null)
            {
                serviceHost.Close();
                serviceHost = null;
            }
        }

        public void Pause()
        {
            Stop();
        }

        public void Continue()
        {
            Start();
        }
    }
}
