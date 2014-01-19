using System;
using System.Linq;
using System.Collections.Generic;
using TechTalk.SpecFlow;

namespace nuserve.features.Steps
{
    [Binding]
    public class NuGetSteps
    {
        [When(@"I request a list of packages")]
        public void WhenIRequestAListOfPackages()
        {
            Console.WriteLine("Listing packages:");
        }
        
        [Then(@"I should see (.*) packages")]
        public void ThenIShouldSeePackages(int n)
        {
            Console.WriteLine("Expecting {0} packages...", n);
        }
    }
}