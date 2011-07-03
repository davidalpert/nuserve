using System;
using System.Linq;
using Nancy;

namespace nuserve
{
    public class TestModule : NancyModule
    {
        public TestModule()
        {
            Get["/"] = parameters =>
            {
                Console.WriteLine("caught request");

                return View["staticview"];
            };
        }
    }
}
