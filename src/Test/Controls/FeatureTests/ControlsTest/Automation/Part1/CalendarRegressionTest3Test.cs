using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// CalendarRegressionTest3Test
    /// Call CalendarRegressionTest3Validator to validate no NullReferenceException occurs when walking tree
    /// </summary>
    [Test(0, "Automation", "Calendar", SupportFiles = @"FeatureTests\Controls\ControlsAutomationTest.exe", Keywords = "MicroSuite")]
    public class CalendarRegressionTest3Test : StepsTest
    {
        public CalendarRegressionTest3Test()
        {
            RunSteps += new TestStep(RunTest);
        }

        public TestResult RunTest()
        {
            using (AutomationValidator test = new CalendarRegressionTest3Validator(@"Calendar\CalendarRegressionTest3.xaml", ""))
            {
                test.Run();
            }

            return TestResult.Pass;
        }
    } 
}
