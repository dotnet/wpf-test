using System;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// DataGridItemContainerPatternTest
    /// </summary>
    [Test(0, "Automation", "DataGrid", SupportFiles = @"FeatureTests\Controls\ControlsAutomationTest.exe", Disabled = true)]
    public class DataGridItemContainerPatternTest : StepsTest
    {
        //DISABLEUNSTABLETEST: [Variation(@"DataGrid\DataGridItemContainerPatternTest.xaml", "datagrid", false)]
        //DISABLEUNSTABLETEST: [Variation(@"DataGrid\DataGridItemContainerPatternHasSelectedItemsTest.xaml", "datagrid", true)]
        public DataGridItemContainerPatternTest(string args, string targetName, bool hasSelectedItem)
        {
            this.args = args;
            this.targetName = targetName;
            this.hasSelectedItem = hasSelectedItem;
            RunSteps += new TestStep(RunTest);
        }

        private string args;
        private string targetName;
        private bool hasSelectedItem;

        public TestResult RunTest()
        {
            // DataGridItemContainerPattern test only runs on win7 and above.
            if (Environment.OSVersion.Version >= new Version("6.1"))
            {
                using (AutomationValidator test = new DataGridItemContainerPatternValidator(args, targetName, hasSelectedItem))
                {
                    test.Run();
                }

                return TestResult.Pass;
            }
            else
            {
                LogComment("Ignore: DataGridItemContainerPattern test only runs on win7 and above.");

                return TestResult.Ignore;
            }
        }
    } 
}
