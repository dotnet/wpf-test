using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// ItemLeakTest
    /// Call ItemReferenceValidator to validate that ItemAutomationPeers update their Item property correctly
    /// </summary>
    [Test(1, "Automation", SupportFiles = @"FeatureTests\Controls\ControlsAutomationTest.exe", Versions="4.8+")]
    public class ItemReferenceTest : StepsTest
    {
        public ItemReferenceTest()
        {
            RunSteps += new TestStep(RunTest);
        }

        public TestResult RunTest()
        {
            ItemReferenceValidator test = new ItemReferenceValidator(@"ItemsControl\ItemReference.xaml", "");

            using (test)
            {
                test.Run();
            }

            Log.LogStatus(test.Summary);

            return test.HasProcessCrashed() ? TestResult.Fail : test.TestResult;
        }
    }
}
