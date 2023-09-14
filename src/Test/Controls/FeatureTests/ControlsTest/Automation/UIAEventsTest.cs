using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// UIAEventsTest
    /// </summary>
    [Test(0, "Automation", "UIAEvents", SupportFiles = @"FeatureTests\Controls\ControlsAutomationTest.exe")]
    public class UIAEventsTest : StepsTest
    {
        public UIAEventsTest()
        {
            RunSteps += new TestStep(RunTest);
        }

        public TestResult RunTest()
        {
            using (AutomationValidator test = new UIAEventsValidator(@"UIAEvents\UIAEvents.xaml", "unused"))
            {
                test.Run();
            }

            return TestResult.Pass;
        }
    }
}
