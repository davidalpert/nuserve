using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;

namespace nuserve.integration.specs
{
    public class when_requesting_a_list_of_available_packages : NuServeContext
    {
        static NuGet.Commands.ListCommand listCommand;
        static IEnumerable<NuGet.IPackage> packages;

        Establish context = () =>
        {
            StartNuServeOn("http://localhost:5051/packages", "http://localhost:5051/");

            listCommand = NuGetCommandBuilder.BuildListCommandFor("http://localhost:5051/packages");
        };

        Because of = () => packages = listCommand.GetPackages();

        It should_list_3_packages = () => packages.Count().ShouldEqual(3);
    }
}