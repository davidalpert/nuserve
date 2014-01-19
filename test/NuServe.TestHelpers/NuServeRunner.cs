using System.Linq;
using System.Collections.Generic;
using NSubstitute;
using log4net;
using nuserve.Infrastructure;
using nuserve.Infrastructure.Implementation;
using nuserve.Settings;

namespace NuServe.TestHelpers
{
    public class NuServeRunner
    {
        public ISelfHostingPackageServer Server { get; private set; }

        public void StartNuServeOn(string packageListUri, string packagePushUri)
        {
            var endpointSettings = new EndpointSettings()
                {
                    PackageListUri = packageListUri,
                    PackageManagerUri = packagePushUri
                };
            var log = Substitute.For<ILog>();
            Server = new SelfHostingPackageServer(endpointSettings, log);

            Server.Start();
        }

        public void StopNuServe()
        {
            if (Server != null)
            {
                Server.Stop();
            }
        }
    }
}