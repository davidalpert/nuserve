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
    public class OdataPackageServer
    {
        private ServiceHost serviceHost;

        public OdataPackageServer()
        {
        }

        public void Start()
        {
            Stop();
            
            Uri uri = new UriBuilder("http", "localhost", 5656).Uri;

            Uri[] uriArray = { uri };
            Type serviceType = typeof(Packages);

            serviceHost = new DataServiceHost(serviceType, uriArray);

            //var webHttpBinding = new WebHttpBinding(WebHttpSecurityMode.None);
            //serviceHost.AddServiceEndpoint(typeof(PackageService), webHttpBinding, uri);

            // Open the ServiceHostBase to create listeners and start 
            // listening for messages.
            serviceHost.Open();

            Console.WriteLine("Listenting at: {0}", uri);
        }

        public void Stop()
        {
            if (serviceHost != null)
            {
                using (serviceHost) { }
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
    public class Program
    {
        static void Main(string[] args)
        {
            //IoC.Bootstrap();

            XmlConfigurator.ConfigureAndWatch(
                    new FileInfo(".\\log4net.config"));

            Topshelf.HostFactory.Run(x =>
            {
                //x.Service < WcfServiceWrapper<Packages, DataService<PackageContext>>(s =>
                x.Service<OdataPackageServer>(s =>
                {
                    //s.SetServiceName("nuserve");
                    s.ConstructUsing(name => new OdataPackageServer());
                    s.WhenStarted(ns => ns.Start());
                    s.WhenPaused(ns => ns.Pause());
                    s.WhenContinued(ns => ns.Continue());
                    s.WhenStopped(ns => ns.Stop());
                });

                x.RunAsLocalSystem();

                x.SetDisplayName("nuserve.exe");
                x.SetDescription("a lightweight nuget server that can run as a windows service with no dependency on IIS");
                x.SetServiceName("nuserve");
            });
        }
    }
}