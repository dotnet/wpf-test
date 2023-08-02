using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Regression Test 
    /// Ensure the last toolbarpanel child is on toolbarpanel after sizing small then restore the size
    /// </description>
    /// </summary>
    [Test(1, "ToolBar", "ToolBarRegressionTest70", Versions = "4.0+,4.0Client+")] // V4.0 
    public class ToolBarRegressionTest70 : XamlTest
    {
        #region Private Members

        private ToolBar toolbar;

        #endregion

        #region Public Members

        public ToolBarRegressionTest70()
            : base(@"ToolBarRegressionTest70.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(Repro);
        }

        public TestResult Setup()
        {
            Status("Setup");

            toolbar = RootElement.FindName("toolbar") as ToolBar;

            if (toolbar == null)
            {
                throw new TestValidationException("Fail: toolbar is null.");
            }

            // Size the window to a good size for testing
            Window.Width = 350;
            Window.Height = 350;

            LogComment("Setup was successful");

            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            toolbar = null;
            return TestResult.Pass;
        }

        public TestResult Repro()
        {
            string toolbarPanel = "PART_ToolBarPanel";
            double smallerSize = 20;

            // Get before sizing to smaller size last item
            // And cache before sizing last item for later reference validation
            ToolBarPanel beforePanel = (ToolBarPanel)toolbar.Template.FindName(toolbarPanel, toolbar);
            Button beforeSizingLastButton = (Button)beforePanel.Children[beforePanel.Children.Count - 1];

            // Cache the before sizing actual width, so we can restore the width later
            double actualWidth = toolbar.ActualWidth;

            // Perform action: size the toolbar width to smaller size 20
            toolbar.Width = smallerSize;

            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            // Restore width size to before sizing size
            toolbar.Width = actualWidth;

            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            // Get after sizing last item
            ToolBarPanel afterPanel = (ToolBarPanel)toolbar.Template.FindName(toolbarPanel, toolbar);
            Button afterSizingLastButton = (Button)afterPanel.Children[afterPanel.Children.Count - 1];

            // Ensure before sizing last item equals to after sizing last item
            if (!beforeSizingLastButton.Equals(afterSizingLastButton))
            {
                throw new TestValidationException("Fail: beforeSizingLastButton does not equal to afterSizingLastButton");
            }

            return TestResult.Pass;
        }

        #endregion
    }
}
