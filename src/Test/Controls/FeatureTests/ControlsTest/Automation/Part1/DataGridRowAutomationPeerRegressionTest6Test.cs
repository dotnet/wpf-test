using System;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// DataGridRowAutomationPeerRegressionTest6Test
    /// </summary>
    [Test(0, "Automation", "DataGrid", SupportFiles = @"FeatureTests\Controls\ControlsAutomationTest.exe")]
    public class DataGridRowAutomationPeerRegressionTest6Test : StepsTest
    {
        [Variation(@"DataGrid\DataGridRowAutomationPeerRegressionTest6Test.xaml", "target")]
        public DataGridRowAutomationPeerRegressionTest6Test(string args, string targetName)
        {
            this.args = args;
            this.targetName = targetName;
            RunSteps += new TestStep(RunTest);
        }

        private string args;
        private string targetName;

        public TestResult RunTest()
        {
            // DataGridRow automation peer test only runs on win7 and above.
            if (Environment.OSVersion.Version >= new Version("6.1"))
            {
                using (AutomationValidator test = new DataGridRowAutomationPeerRegressionTest6Validator(args, targetName))
                {
                    test.Run();
                }

                return TestResult.Pass;
            }
            else
            {
                LogComment("Ignore: DataGridRow automation peer test only runs on win7 and above.");

                return TestResult.Ignore;
            }
        }
    } 
}
