using System;
using Nancy.Hosting.Self;

namespace nuserve
{
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
