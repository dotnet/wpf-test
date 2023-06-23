using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// TabControlRegressionTest8Test
    /// Call TabControlRegressionTest8Validator to validate that TabItem contents are always visible to automation
    /// </summary>
    [Test(0, "Automation", "TabControl", SupportFiles = @"FeatureTests\Controls\ControlsAutomationTest.exe", Versions="4.7+")]
    public class TabControlRegressionTest8Test : StepsTest
    {
        public TabControlRegressionTest8Test()
        {
            RunSteps += new TestStep(RunTest);
        }

        public TestResult RunTest()
        {
            AutomationValidator test = new TabControlRegressionTest8Validator(@"TabControl\TabControlRegressionTest8.xaml", "");

            using (test)
            {
                test.Run();
            }

            return test.HasProcessCrashed() ? TestResult.Fail : test.TestResult;
        }
    }
}
