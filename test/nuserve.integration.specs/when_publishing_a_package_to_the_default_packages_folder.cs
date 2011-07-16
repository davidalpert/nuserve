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
        public static void PushPackage(string nupkgName, string apiKey, string source)
        {
            var pathToAssembly = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var pathToNugetExe = Path.Combine(pathToAssembly, "Tools", "nuget.exe");
            var pathToTestPackage = Path.Combine(pathToAssembly, "TestPackages", nupkgName);

            File.Exists(pathToNugetExe).ShouldBeTrue();
            File.Exists(pathToTestPackage).ShouldBeTrue();

            // nuget push <package path> [API key] [options]
            Process myProcess = new Process();

            myProcess.StartInfo.FileName = pathToNugetExe;
            myProcess.StartInfo.CreateNoWindow = true;
            myProcess.StartInfo.Arguments = String.Format("push {0} {1} -s {2}", pathToTestPackage, apiKey, source);

            myProcess.Start().ShouldBeTrue();

            myProcess.WaitForExit(10000).ShouldBeTrue();
        }

        Establish context = () =>
        {
            StartNuServeOn("http://localhost:5051/packages", "http://localhost:5051/");
        };

        Because of = () => PushPackage("Topshelf.2.2.2.0.nupkg", "", "http://localhost:5051");

        It should_list_4_packages = () =>
            BuildListCommandFor("http://localhost:5051/packages").GetPackages().Count().ShouldEqual(4);
    }
}
