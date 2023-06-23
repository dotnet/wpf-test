using System;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// DataGridVirtualizedItemPatternTest
    /// </summary>
    [Test(0, "Automation", "DataGrid", SupportFiles = @"FeatureTests\Controls\ControlsAutomationTest.exe")]
    public class DataGridVirtualizedItemPatternTest : StepsTest
    {
        // Rows 0 to 7 is in viewport. Estimate 15 non virtualized items.
        // Validate row 0, 7 that are in viewport and 9 that is not in viewport.
        // Test top and bottom within viewport non virtualized items 0, 7, 9.
        // Test top, middle, bottm virtualized items 20, 30, 40.
        [Variation(@"DataGrid\DataGridItemContainerPatternTest.xaml", "datagrid", "0", false)]
        [Variation(@"DataGrid\DataGridItemContainerPatternTest.xaml", "datagrid", "7", false)]
        [Variation(@"DataGrid\DataGridItemContainerPatternTest.xaml", "datagrid", "9", false)]
        [Variation(@"DataGrid\DataGridItemContainerPatternTest.xaml", "datagrid", "20", true)]
        [Variation(@"DataGrid\DataGridItemContainerPatternTest.xaml", "datagrid", "30", true)]
        [Variation(@"DataGrid\DataGridItemContainerPatternTest.xaml", "datagrid", "40", true)]
        [Variation(@"DataGrid\DataGridItemContainerPatternHasSelectedItemsTest.xaml", "datagrid", "0", false)]
        [Variation(@"DataGrid\DataGridItemContainerPatternHasSelectedItemsTest.xaml", "datagrid", "7", false)]
        [Variation(@"DataGrid\DataGridItemContainerPatternHasSelectedItemsTest.xaml", "datagrid", "9", false)]
        [Variation(@"DataGrid\DataGridItemContainerPatternHasSelectedItemsTest.xaml", "datagrid", "20", true, Disabled=true)]
        [Variation(@"DataGrid\DataGridItemContainerPatternHasSelectedItemsTest.xaml", "datagrid", "30", true, Disabled=true)]
        [Variation(@"DataGrid\DataGridItemContainerPatternHasSelectedItemsTest.xaml", "datagrid", "40", true, Disabled=true)]
        public DataGridVirtualizedItemPatternTest(string args, string targetName, string rowId, bool isVirtualized)
        {
            this.args = args;
            this.targetName = targetName;
            this.rowId = rowId;
            this.isVirtualized = isVirtualized;
            RunSteps += new TestStep(RunTest);
        }

        private string args;
        private string targetName;
        private string rowId;
        private bool isVirtualized;

        public TestResult RunTest()
        {
            // DataGridVirtualizedItemPattern test only runs on win7 and above.
            if (Environment.OSVersion.Version >= new Version("6.1"))
            {
                using (AutomationValidator test = new DataGridVirtualizedItemPatternValidator(args, targetName, rowId, isVirtualized))
                {
                    test.Run();
                }

                return TestResult.Pass;
            }
            else
            {
                LogComment("Ignore: DataGridVirtualizedItemPattern test only runs on win7 and above.");

                return TestResult.Ignore;
            }
        }
    } 
}
