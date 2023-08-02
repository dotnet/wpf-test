using System;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// DataGridNonVirtualizedRowTest
    /// </summary>
    [Test(0, "Automation", "DataGrid", SupportFiles = @"FeatureTests\Controls\ControlsAutomationTest.exe")]
    public class DataGridNonVirtualizedRowTest : StepsTest
    {
        [Variation(@"DataGrid\DataGridItemContainerPatternTest.xaml", "datagrid", "11")]
        public DataGridNonVirtualizedRowTest(string args, string targetName, string rowId)
        {
            this.args = args;
            this.targetName = targetName;
            this.rowId = rowId;
            RunSteps += new TestStep(RunTest);
        }

        private string args;
        private string targetName;
        private string rowId;

        public TestResult RunTest()
        {
            // ItemContainerPattern test only runs on win7 and above.
            if (Environment.OSVersion.Version >= new Version("6.1"))
            {
                using (AutomationValidator test = new DataGridNonVirtualizedRowValidator(args, targetName, rowId))
                {
                    test.Run();
                }

                return TestResult.Pass;
            }
            else
            {
                LogComment("Ignore: ItemContainerPattern test only runs on win7 and above.");

                return TestResult.Ignore;
            }
        }
    } 
}
