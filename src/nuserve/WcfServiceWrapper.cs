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
    public class WcfServiceWrapper<TServiceImplementation, TServiceContract>
        : ServiceBase
        where TServiceImplementation : TServiceContract
    {
        private readonly string _serviceUri;
        private ServiceHost _serviceHost;

        public WcfServiceWrapper(string serviceName, string serviceUri)
        {
            _serviceUri = serviceUri;
            ServiceName = serviceName;
        }

        protected override void OnStart(string[] args)
        {
            Start();
        }

        protected override void OnStop()
        {
            Stop();
        }

        public void Start()
        {
            Console.WriteLine(ServiceName + " starting...");
            bool openSucceeded = false;
            try
            {
                if (_serviceHost != null)
                {
                    _serviceHost.Close();
                }

                _serviceHost = new ServiceHost(typeof(TServiceImplementation));
            }
            catch (Exception e)
            {
                Console.WriteLine("Caught exception while creating " + ServiceName + ": " + e);
                return;
            }

            try
            {
                var webHttpBinding = new WebHttpBinding(WebHttpSecurityMode.None);
                _serviceHost.AddServiceEndpoint(typeof(TServiceContract), webHttpBinding, _serviceUri);

                var webHttpBehavior = new WebHttpBehavior
                {
                    DefaultOutgoingResponseFormat = WebMessageFormat.Json
                };
                _serviceHost.Description.Endpoints[0].Behaviors.Add(webHttpBehavior);

                _serviceHost.Open();
                openSucceeded = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Caught exception while starting " + ServiceName + ": " + ex);
            }
            finally
            {
                if (!openSucceeded)
                {
                    _serviceHost.Abort();
                }
            }

            if (_serviceHost.State == CommunicationState.Opened)
            {
                Console.WriteLine(ServiceName + " started at " + _serviceUri);
            }
            else
            {
                Console.WriteLine(ServiceName + " failed to open");
                bool closeSucceeded = false;
                try
                {
                    _serviceHost.Close();
                    closeSucceeded = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ServiceName + " failed to close: " + ex);
                }
                finally
                {
                    if (!closeSucceeded)
                    {
                        _serviceHost.Abort();
                    }
                }
            }
        }

        public new void Stop()
        {
            Console.WriteLine(ServiceName + " stopping...");
            try
            {
                if (_serviceHost != null)
                {
                    _serviceHost.Close();
                    _serviceHost = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Caught exception while stopping " + ServiceName + ": " + ex);
            }
            finally
            {
                Console.WriteLine(ServiceName + " stopped...");
            }
        }
    }
}
