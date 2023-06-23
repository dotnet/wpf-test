using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.Windows.Controls;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Regression Test ContentControls do not display the Name of a RoutedCommand when using .NET 4.5 as they did in prior versions
    /// </description>

    /// </summary>
    [Test(0, "Button", "RegressionButtonRegressionTest12", SecurityLevel = TestCaseSecurityLevel.FullTrust, Versions = "4.5+")]
    public class RegressionButtonRegressionTest12 : XamlTest
    {
        #region Private Data

        private Button testButton;
        private TextBlock textBlock;

        #endregion

        #region Constructor

        public RegressionButtonRegressionTest12()
            : base(@"RegressionButtonRegressionTest12.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(Repro);
        }

        #endregion

        #region Test Steps

        private TestResult Setup()
        {
            Status("Setup specific for RegressionButtonRegressionTest12");
            testButton = (Button)RootElement.FindName("myButton");
            Assert.AssertTrue("Failed to find testButton", testButton != null);
            textBlock = DataGridHelper.FindVisualChild<TextBlock>(testButton);
            Assert.AssertTrue("Failed to find textBlock", textBlock != null);

            LogComment("Setup RegressionButtonRegressionTest12 successfully");
            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            testButton = null;
            textBlock = null;
            return TestResult.Pass;
        }

        private TestResult Repro()
        {
            Status("Start to check textblock's value");
            Assert.AssertTrue("Case failed due to regression bug", textBlock.Text == "Paste");

            LogComment("Button content show Paste correctly");
            return TestResult.Pass;
        }

        #endregion

    }

}
