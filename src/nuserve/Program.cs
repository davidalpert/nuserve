using System;
using System.IO;
using log4net.Config;
using Topshelf;
using nuserve.Configuration;
using nuserve.Infrastructure;

namespace nuserve
{
    public class Program
    {
        static void Main(string[] args)
        {
            IoC.Bootstrap();

            XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config")));

            Topshelf.HostFactory.Run(x =>
            {
                x.Service<ISelfHostingPackageServer>(s =>
                {
                    s.ConstructUsing(name => IoC.Get<ISelfHostingPackageServer>());
                    s.WhenStarted(ns => ns.Start());
                    s.WhenPaused(ns => ns.Pause());
                    s.WhenContinued(ns => ns.Continue());
                    s.WhenStopped(ns => ns.Stop());
                });

                x.RunAsLocalSystem();

                x.SetDisplayName("nuserve.exe");
                x.SetDescription("A lightweight nuget server that can run as a windows service with no dependency on IIS");
                x.SetServiceName("nuserve");
            });
        }
    }
}