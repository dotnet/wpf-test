using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// ButtonAutomationTest
    /// </summary>
    [Test(0, "Automation", "Button", SupportFiles = @"FeatureTests\Controls\ControlsAutomationTest.exe")]
    public class ButtonAutomationTest : StepsTest
    {
        public ButtonAutomationTest()
        {
            RunSteps += new TestStep(RunTest);
        }

        public TestResult RunTest()
        {
            using (AutomationValidator test = new ButtonAutomationValidator(@"Button\Button.xaml", "button"))
            {
                test.Run();
            }

            return TestResult.Pass;
        }
    } 
}
