using System;
using System.Dynamic;
using System.IO;
using System.Linq;
using developwithpassion.specifications.nsubstitue;
using Machine.Specifications;
using NSubstitute;
using NuGet;
using NuGet.Server.Infrastructure;
using nuserve.Infrastructure;

namespace nuserve.specs
{
    public class when_handling_an_authorized_request_to_publish_a_package : Observes<PackageFilesModule>
    {
        static IServerPackageRepository repo;

        Establish context = () =>
        {
            repo = depends.on(Substitute.For<IServerPackageRepository>());

            depends.on<IAuthorizePackageOperations>().ClientCanPublishPackage(null, null)
                                                        .ReturnsForAnyArgs(call => true);

            depends.on<IZipPackageFactory>().CreatePackageFromRequestBody(Arg.Any<Stream>())
                                                    .Returns(Substitute.For<IPackage>());

            var request = new Nancy.Request("POST", "http://localhost:5151/packagefiles/someSecretKey/nupkg", "http");

            var context = new Nancy.NancyContext()
            {
                Request = request
            };

            sut_setup.run(s => s.Context = context);
        };

        static Nancy.Response response;

        static Func<dynamic, Nancy.Response> GetHandlerFor(string method, string route)
        {
            return sut.Routes
                        .Where(r => r.Description.Method == method)
                        .First(r => r.Description.Path == route)
                        .Action;
        }

        Because of = () =>
        {
            dynamic parameters = new ExpandoObject();
            parameters.apiKey = "someKey";

            var handle = GetHandlerFor("POST", "/PackageFiles/{apiKey}/nupkg");

            response = handle(parameters);
        };

        It should_add_the_package_to_the_repo = () =>
        {
            repo.ReceivedWithAnyArgs().AddPackage(null);
        };
    }
}
