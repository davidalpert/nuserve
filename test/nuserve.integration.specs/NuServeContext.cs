using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Machine.Specifications;
using NuServe.TestHelpers;

namespace nuserve.integration.specs
{
    public abstract class NuServeContext
    {
        protected static NuServeRunner _runner;

        Establish context = () => _runner = new NuServeRunner();

        Cleanup after_each = () => _runner.StopNuServe();

        protected static void StartNuServeOn(string packageListUri, string packagePushUri)
        {
            _runner.StartNuServeOn(packageListUri, packagePushUri);
        }

        public static void PushPackage(string nupkgName, string apiKey, string source)
        {
            var pathToAssembly = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var pathToNugetExe = Path.Combine(pathToAssembly, "Tools", "nuget.exe");
            var pathToTestPackage = Path.Combine(pathToAssembly, "TestPackages", nupkgName);

            File.Exists(pathToNugetExe).ShouldBeTrue();
            File.Exists(pathToTestPackage).ShouldBeTrue();

            // nuget push <package path> [API key] [options]
            Process myProcess = new Process
                {
                    StartInfo =
                        {
                            FileName = pathToNugetExe,
                            CreateNoWindow = true,
                            Arguments = String.Format("push {0} {1} -s {2}", pathToTestPackage, apiKey, source)
                        }
                };

            myProcess.Start().ShouldBeTrue();

            myProcess.WaitForExit(10000).ShouldBeTrue();
        }
    }
}
