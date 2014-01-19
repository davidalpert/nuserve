using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using TechTalk.SpecFlow;

namespace nuserve.features.Steps
{
    [Binding]
    public class PackageRepositorySteps 
    {
        [Given(@"there are (.*) packages in the server's folder")]
        public void GivenThereArePackagesInTheServerSFolder(int n)
        {
            var packageRoot = GlobalSteps.TestContext.PackagesRoot;
            var testPackages = GlobalSteps.TestContext.TestPackages;

            int packagesToTake = Math.Max(0, Math.Min(n, testPackages.Length));

            if (packagesToTake < n)
                throw new InvalidOperationException(string.Format("Only found {0} source packages!", packagesToTake));

            //Debug.WriteLine("Populating {0} with {1} packages:", packageRoot, packagesToTake);

            foreach (var sourcePackage in testPackages.Take(packagesToTake))
            {
                var sourcePath = sourcePackage.FullName;
                var packageFileName = Path.GetFileName(sourcePath);
                var destPath = Path.Combine(packageRoot.FullName, packageFileName);

                Debug.WriteLine(string.Format("- Adding {0}", packageFileName)); 

                File.Copy(sourcePath, destPath, true);
            }
        }
    }
}