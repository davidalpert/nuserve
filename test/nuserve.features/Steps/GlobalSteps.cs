using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using NuServe.TestHelpers;
using TechTalk.SpecFlow;

namespace nuserve.features.Steps
{
    [Binding]
    public class GlobalSteps
    {
        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            TestContext = new NuServeTestContext(Assembly.GetExecutingAssembly());
        }

        public static NuServeTestContext TestContext { get; private set; }

        [BeforeScenario]
        public static void BeforeScenario()
        {
            ScenarioContext.Current.Set(TestContext);
        }

        [AfterScenario]
        public static void AfterScenario()
        {
            TestContext.Cleanup();
        }
    }
}