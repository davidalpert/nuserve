using System.Linq;
using NuServe.TestHelpers;
using TechTalk.SpecFlow;

namespace nuserve.features.Steps
{
    [Binding]
    public class NuServeSteps
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
    }
}