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

namespace nuserve
{
    public class Program
    {
        static void Main(string[] args)
        {
            var cfg = RunnerConfigurator.New(x =>
            {
                x.Service<NuGetPackageServer>(s =>
                {
                    s.SetServiceName("nuserve");
                    s.ConstructUsing(name => new NuGetPackageServer());
                    s.WhenStarted(ns => ns.Start());
                    s.WhenPaused(ns => ns.Pause());
                    s.WhenContinued(ns => ns.Continue());
                    s.WhenStopped(ns => ns.Stop());
                });

                x.RunAsLocalSystem();
            });

            Runner.Host(cfg, args);
        }
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
