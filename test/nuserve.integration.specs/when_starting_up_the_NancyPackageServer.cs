using System;
using System.Net;
using developwithpassion.specifications.nsubstitue;
using EasyHttp.Http;
using Machine.Specifications;
using nuserve;

namespace nuserve.integration.specs
{
    public class when_starting_up_the_NancyPackageServer : Observes<ISelfHostingPackageServer, SelfHostingPackageServer>
    {
        static HttpClient client;

        Establish context = () =>
        {
            client = new HttpClient();
        };

        Because of = () => sut.Start();

        Cleanup after_each = () =>
        {
            sut.Stop();
        };

        It should_be_listening = () =>
            sut.IsListening.ShouldBeTrue();

        It should_handle_a_get_request_to_the_root = () =>
       {
           var result = client.Get(sut.EndpointUri.ToString());

           result.StatusCode.ShouldEqual(HttpStatusCode.OK);

           Console.WriteLine();
           Console.WriteLine(result.RawText);
           Console.WriteLine();
       };
    }
}
