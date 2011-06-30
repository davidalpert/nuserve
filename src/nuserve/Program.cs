using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Topshelf;
using Topshelf.Configuration.Dsl;

namespace nuserve
{
    public class Program
    {
        static void Main(string[] args)
        {
            var cfg = RunnerConfigurator.New(x =>
            {
                x.Service<Server>(s =>
                {
                    s.SetServiceName("nuserve");
                    s.ConstructUsing(name => new Server());
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

    public class Server
    {
        public void Start()
        {
            Console.WriteLine("nuserve started.");
        }

        public void Stop()
        {
            Console.WriteLine("nuserve stopped.");
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
