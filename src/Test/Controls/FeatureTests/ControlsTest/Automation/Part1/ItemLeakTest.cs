using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// ItemLeakTest
    /// Call ItemLeakValidator to validate that data items don't leak due to automation
    /// </summary>
    // Disabling for .NET Core 3, Fix and re-enable.
    //[Test(0, "Automation", SupportFiles = @"FeatureTests\Controls\ControlsAutomationTest.exe", Versions="4.8+")]
    public class ItemLeakTest : StepsTest
    {
        public ItemLeakTest()
        {
            RunSteps += new TestStep(RunTest);
        }

        public TestResult RunTest()
        {
            ItemLeakValidator test = new ItemLeakValidator(@"ItemsControl\ItemLeak.xaml", "");

            using (test)
            {
                test.Run();
            }

            Log.LogStatus(test.Summary);
            foreach (string s in test.Errors)
            {
                Log.LogStatus(s);
            }

            return test.HasProcessCrashed() ? TestResult.Fail : test.TestResult;
        }
    }
}

