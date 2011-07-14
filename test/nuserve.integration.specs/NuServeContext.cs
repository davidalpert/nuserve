using System;
using System.Collections.Generic;
using log4net;
using Machine.Specifications;
using NSubstitute;
using NuGet.Commands;
using nuserve;
using nuserve.Settings;

namespace nuserve.integration.specs
{
    public abstract class NuServeContext
    {
        protected static ISelfHostingPackageServer server;
        static EndpointSettings endpointSettings;

        Cleanup after_each = () =>
        {
            StopNuServe();
        };

        protected static void StartNuServeOn(string packageListUri, string packagePushUri)
        {
            StopNuServe();

            endpointSettings = new EndpointSettings()
            {
                PackageListUri = packageListUri,
                PackageManagerUri = packagePushUri
            };
            var log = Substitute.For<ILog>();
            server = new SelfHostingPackageServer(endpointSettings, log);

            server.Start();
        }

        protected static void StopNuServe()
        {
            if (server != null)
            {
                server.Stop();
            }
        }

        protected static NuGet.Commands.ListCommand BuildListCommandFor(string source)
        {
            NuGet.IPackageSourceProvider sourceProvider = Substitute.For<NuGet.IPackageSourceProvider>();
            sourceProvider.LoadPackageSources().Returns(call => new List<NuGet.PackageSource> { new NuGet.PackageSource(source) });
            NuGet.IPackageRepositoryFactory packageRepositoryFactory = new NuGet.PackageRepositoryFactory();
            return new ListCommand(packageRepositoryFactory, sourceProvider);
        }
    }
}
