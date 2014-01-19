using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TechTalk.SpecFlow;
using nuserve.integration.specs.TestHelpers;

namespace nuserve.features.Steps
{
    [Binding]
    public class Class2
    {
        private NuServeRunner _runner = new NuServeRunner();
        public string PackageListUri = "http://localhost:8080/packages";
        public string PackagePushUri = "http://localhost:8080/";
        private string _packageRoot = @".\Packages";

        [Given(@"nuserve is running")]
        public void GivenNuserveIsRunning()
        {
            //_runner.StartNuServeOn(PackageListUri, PackagePushUri);
        }
        
        [Given(@"there are (.*) packages in the server's folder")]
        public void GivenThereArePackagesInTheServerSFolder(int n)
        {
            Console.WriteLine("Populating {0} with {1} packages:", _packageRoot, n);
            string packageLocation = @"..\..\..\..\..\packages";
            var directories = Directory.GetDirectories(packageLocation);

            if (Directory.Exists(_packageRoot) == false)
            {
                Directory.CreateDirectory(_packageRoot);
            }

            int packagesToTake = Math.Max(0, Math.Min(n, directories.Length));

            var sourcePackages = directories.Take(packagesToTake)
                                            .Select(s => Directory.GetFiles(s, "*.nupkg").FirstOrDefault())
                                            .ToArray();

            foreach (var sourcePackage in sourcePackages)
            {
                var fileName = Path.GetFileName(sourcePackage);
                var destPath = Path.Combine(_packageRoot, fileName);
                Console.WriteLine("- Adding {0}", fileName);
                File.Copy(sourcePackage, destPath);
            }
        }
        
        [When(@"I request a list of packages")]
        public void WhenIRequestAListOfPackages()
        {
            Console.WriteLine("Listing packages:");
        }
        
        [Then(@"I should see (.*) packages")]
        public void ThenIShouldSeePackages(int n)
        {
            Console.WriteLine("Expecting {0} packages...", n);
            ScenarioContext.Current.Pending();
        }
    }
}