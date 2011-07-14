using System;
using Machine.Specifications;

namespace nuserve.integration.specs
{
    public class when_starting_up_the_NancyPackageServer : NuServeContext
    {
        protected static EasyHttp.Http.HttpClient client;

        Establish context = () =>
        {
            client = new EasyHttp.Http.HttpClient();
        };

        Because of = () => StartNuServeOn("http://localhost:5051/packages", "http://localhost:5051/");

        It should_be_listening = () =>
            server.IsListening.ShouldBeTrue();
    }
}