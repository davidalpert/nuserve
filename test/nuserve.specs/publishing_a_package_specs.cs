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
    public abstract class publishing_a_package_specs : Observes<PackageFilesModule>
    {
        protected static IServerPackageRepository repo;
        protected static Nancy.Response response;
        protected static dynamic requestParameters;

        Establish context = () =>
        {
            repo = depends.on<IServerPackageRepository>();

            depends.on<IZipPackageFactory>().CreatePackageFromRequestBody(Arg.Any<Stream>())
                                                    .Returns(Substitute.For<IPackage>());

            var request = new Nancy.Request("POST", "http://localhost:5151/packagefiles/someSecretKey/nupkg", "http");

            requestParameters = new ExpandoObject(); // to be set by a descendent's Establish clause

            var context = new Nancy.NancyContext()
            {
                Request = request
            };

            sut_setup.run(s => s.Context = context);

        };

        Because of = () =>
        {
            requestParameters.apiKey = "someKey";

            var handle = GetHandlerFor("POST", "/PackageFiles/{apiKey}/nupkg");

            response = handle(requestParameters);
        };

        protected static void ConfigureAuthorizationResponses(bool canClientPublishPackage)
        {
            depends.on<IAuthorizePackageOperations>().ClientCanPublishPackage(null, null)
                                .ReturnsForAnyArgs(call => canClientPublishPackage);
        }

        protected static Func<dynamic, Nancy.Response> GetHandlerFor(string method, string route)
        {
            return sut.Routes.Where(r => r.Description.Method == method).First(r => r.Description.Path == route).Action;
        }
    }

    public class when_handling_an_authorized_request_to_publish_a_package : publishing_a_package_specs
    {
        Establish context = () =>
        {
            ConfigureAuthorizationResponses(true);
        };

        It should_add_the_package_to_the_repo = () => repo.ReceivedWithAnyArgs().AddPackage(null);
    }

    public class when_handling_an_unauthorized_request_to_publish_a_package : publishing_a_package_specs
    {
        Establish context = () =>
        {
            ConfigureAuthorizationResponses(false);
        };

        It should_not_add_the_package_to_the_repo = () => repo.DidNotReceiveWithAnyArgs().AddPackage(null);
    }
}
