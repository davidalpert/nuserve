using System;
using System.IO;
using Machine.Specifications;
using Nancy;
using Nancy.Testing;
using NSubstitute;
using NuGet;
using NuGet.Server.Infrastructure;
using StructureMap;
using nuserve.Infrastructure;

namespace nuserve.specs
{
    [Ignore("This one fails with configuring Nancy...")]
    public class when_handling_an_authorized_request_to_publish_a_package_with_Nancy_Testing_Browser
    {
        public class MockBootstrapper : nuserve.Configuration.NuserveNancyBootstrapper
        {
            private bool clientIsAuthorizedToPublish;

            /// <summary>
            /// Initializes a new instance of the MockBootstrapper class.
            /// </summary>
            /// <param name="clientIsAuthorizedToPublish"></param>
            public MockBootstrapper(bool clientIsAuthorizedToPublish)
            {
                this.clientIsAuthorizedToPublish = clientIsAuthorizedToPublish;
            }

            protected override StructureMap.IContainer GetApplicationContainer()
            {
                IContainer nestedContainer = base.GetApplicationContainer().GetNestedContainer();

                nestedContainer.Configure(cfg =>
                {
                    cfg.For<IServerPackageRepository>().Use(Substitute.For<IServerPackageRepository>());

                    var apo = Substitute.For<IAuthorizePackageOperations>();
                    apo.ClientCanPublishPackage(null, null)
                        .ReturnsForAnyArgs(call => clientIsAuthorizedToPublish);

                    cfg.For<IAuthorizePackageOperations>().Use(apo);

                    var zpf = Substitute.For<IZipPackageFactory>();
                    zpf.CreatePackageFromRequestBody(Arg.Any<Stream>())
                        .Returns(Substitute.For<IPackage>());

                    cfg.For<IZipPackageFactory>().Use(zpf);
                });

                return nestedContainer;
            }
        }

        static Browser browser;
        static BrowserResponse response;

        Establish context = () =>
        {
            bool clientIsAuthorizedToPublish = true;
            MockBootstrapper bootstrapper = new MockBootstrapper(clientIsAuthorizedToPublish);
            browser = new Browser(bootstrapper);
        };

        Because of = () => response = browser.Post("http://localhost:5151/packagefiles/someKey/nupkg", with => { });

        It should_respond_with_a_201 = () => response.StatusCode.ShouldEqual(HttpStatusCode.OK);
    }
}
