using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// CalendarRegressionTest86Test
    /// Calender/DatePicker Automation for SetFocus is failing due to Focus immidiatley moving to internal UIElement
    /// </summary>
    [Test(0, "Automation", "Calendar", SupportFiles = @"FeatureTests\Controls\ControlsAutomationTest.exe", Keywords = "MicroSuite")]
    public class CalendarRegressionTest86Test : StepsTest
    {
        public CalendarRegressionTest86Test()
        {
            RunSteps += new TestStep(RunTest);
        }

        public TestResult RunTest()
        {
            using (AutomationValidator test = new CalendarRegressionTest86Validator(@"Calendar\Calendar.xaml", "calendar"))
            {
                test.Run();
            }

            return TestResult.Pass;
        }
    } 
}
