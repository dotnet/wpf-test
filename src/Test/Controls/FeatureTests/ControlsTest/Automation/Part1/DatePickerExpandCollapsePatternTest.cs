using System;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// DatePickerExpandCollapsePatternTest
    /// </summary>
    [Test(0, "Automation", "DatePicker", SupportFiles = @"FeatureTests\Controls\ControlsAutomationTest.exe")]
    public class DatePickerExpandCollapsePatternTest : StepsTest
    {
        [Variation(@"DatePicker\DatePickerExpandCollapsePatternTest.xaml", "datepicker")]
        public DatePickerExpandCollapsePatternTest(string args, string targetName)
        {
            this.args = args;
            this.targetName = targetName;
            RunSteps += new TestStep(RunTest);
        }

        private string args;
        private string targetName;

        public TestResult RunTest()
        {
            using (AutomationValidator test = new DatePickerExpandCollapsePatternValidator(args, targetName))
            {
                test.Run();
            }

            return TestResult.Pass;
        }
    }
}
