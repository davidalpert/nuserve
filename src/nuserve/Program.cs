using System;
using System.IO;
using log4net.Config;
using Topshelf;

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
}