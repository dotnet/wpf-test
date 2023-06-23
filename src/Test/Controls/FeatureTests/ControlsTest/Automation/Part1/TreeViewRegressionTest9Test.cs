using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// TabControlRegressionTest9Test
    /// Call TabControlRegressionTest9Validator to validate that TabItem contents are always visible to automation
    /// </summary>
    [Test(0, "Automation", SupportFiles = @"FeatureTests\Controls\ControlsAutomationTest.exe", Versions="4.8+")]
    public class TreeViewRegressionTest9Test : StepsTest
    {
        public TreeViewRegressionTest9Test()
        {
            RunSteps += new TestStep(RunTest);
        }

        public TestResult RunTest()
        {
            TreeViewRegressionTest9Validator test = new TreeViewRegressionTest9Validator(@"TreeView\TreeViewRegressionTest9.xaml", "");

            using (test)
            {
                test.Run();
            }

            Log.LogStatus(test.Summary);
            foreach (string s in test.Errors)
            {
                Log.LogStatus(s);
            }

            return test.HasProcessCrashed() ? TestResult.Fail : test.TestResult;
        }
    }
}
