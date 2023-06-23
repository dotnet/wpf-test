using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// CalendarSelectionAutomationTest
    /// </summary>
    [Test(0, "Automation", "Calendar", SupportFiles = @"FeatureTests\Controls\ControlsAutomationTest.exe", Keywords = "MicroSuite")]
    public class CalendarSelectionAutomationTest : StepsTest
    {
        public CalendarSelectionAutomationTest()
        {
            RunSteps += new TestStep(RunTest);
        }

        public TestResult RunTest()
        {
            using (AutomationValidator test = new CalendarSelectionAutomationValidator(@"Calendar\Calendar.xaml", "calendar"))
            {
                test.Run();
            }

            return TestResult.Pass;
        }
    } 
}
