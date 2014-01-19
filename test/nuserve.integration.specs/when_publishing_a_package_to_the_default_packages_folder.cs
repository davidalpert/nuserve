using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Machine.Specifications;

namespace nuserve.integration.specs
{
    [Ignore("this test is broken, but the behavior it is meant to test is not necessarily the cause")]
    public class when_publishing_a_package_to_the_default_packages_folder : NuServeContext
    {
        Establish context = () =>
        {
            StartNuServeOn("http://localhost:5051/packages", "http://localhost:5051/");

            NuGetCommandBuilder.BuildListCommandFor("http://localhost:5051/packages").GetPackages().Count().ShouldEqual(3);
        };

        Because of = () => PushPackage("Topshelf.2.2.2.0.nupkg", "", "http://localhost:5051");

        It should_list_4_packages = () =>
            NuGetCommandBuilder.BuildListCommandFor("http://localhost:5051/packages").GetPackages().Count().ShouldEqual(4);
    }
}
