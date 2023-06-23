using System;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// DataGridPeersBugsTest
    /// Pass bug number to validator and invokde DataGridPeersBugsValidator.Run to run regression tests.
    /// </summary>
    [Test(0, "Automation", "DataGrid", SupportFiles = @"FeatureTests\Controls\ControlsAutomationTest.exe")]
    public class DataGridPeersBugsTest : StepsTest
    {
        [Variation(1, "Ensure DataGridColumnHeader.Name shows name instead of index", @"DataGrid\DataGridPeersBugsTest.xaml", "datagrid")]
        [Variation(2, "Ensure virtualized DataGridColumnHeader.ClassName throws ElementNotAvailableException", @"DataGrid\DataGridPeersBugsTest.xaml", "datagrid")]
        public DataGridPeersBugsTest(int bugNumber, string bugDescription, string args, string targetName)
        {
            this.bugNumber = bugNumber;
            this.bugDescription = bugDescription;
            this.args = args;
            this.targetName = targetName;
            RunSteps += new TestStep(RunTest);
        }

        private int bugNumber;
        private string bugDescription;
        private string args;
        private string targetName;

        public TestResult RunTest()
        {
            if (bugNumber == 2 && Environment.OSVersion.Version < new Version("6.1"))
            {
                LogComment("Ignore: DataGridItemContainerPattern test only runs on win7 and above.");

                return TestResult.Ignore;
            }
         
            using (AutomationValidator test = new DataGridPeersBugsValidator(bugNumber, bugDescription, args, targetName))
            {
                test.Run();
            }

            return TestResult.Pass;
        }
    }
}
