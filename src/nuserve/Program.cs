using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Topshelf;
using Topshelf.Configuration.Dsl;
using System.Net;
using System.Diagnostics;
using Nancy.Hosting.Self;
using Nancy;
using System.Timers;
using Topshelf.Internal;
using log4net.Config;
using System.IO;

namespace nuserve
{
    public class Program
    {
        static void Main(string[] args)
        {
            XmlConfigurator.ConfigureAndWatch(
                    new FileInfo(".\\log4net.config"));

            Topshelf.HostFactory.Run(x =>
            {
                x.Service<NuGetPackageServer>(s =>
                {
                    //s.SetServiceName("nuserve");
                    s.ConstructUsing(name => new NuGetPackageServer());
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

    public class TownCrier
    {
        readonly Timer _timer;
        public TownCrier()
        {
            _timer = new Timer(1000) { AutoReset = true };
            _timer.Elapsed += (sender, eventArgs) => Console.WriteLine("It is {0} an all is well", DateTime.Now);
        }
        public void Start() { _timer.Start(); }
        public void Stop() { _timer.Stop(); }
    }

    public class TestModule : NancyModule
    {
        public TestModule()
        {
            Get["/"] = parameters =>
            {
                Console.WriteLine("caught request");

                return View["staticview"];
            };
        }
    }

    public class NuGetPackageServer
    {
        private NancyHost host = null;

        public void Start()
        {
            var port = 5555;
            var uri = new UriBuilder("http", "localhost", port).Uri;

            ensure_host_is_stopped();

            host = new NancyHost(uri);
            host.Start();

            Console.WriteLine("Nancy now listening at: {0}", uri);
            //Console.WriteLine("Press enter to stop");
            //Console.ReadKey();

            Console.WriteLine("nuserve started.");
        }

        public void Stop()
        {
            ensure_host_is_stopped();

            Console.WriteLine("nuserve stopped.");
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
            Console.WriteLine("nuserve paused.");
        }

        public void Continue()
        {
            Console.WriteLine("nuserve resumed.");
        }
    }
}
