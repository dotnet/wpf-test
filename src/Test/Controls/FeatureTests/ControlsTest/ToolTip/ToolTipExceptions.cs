using System;
using System.Windows.Controls;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Input;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Avalon.Test.ComponentModel.Actions;

namespace Avalon.Test.ComponentModel.UnitTests
{
    /// <summary>
    /// <description>
    /// ToolTipExceptions
    /// Open and Close ToolTip.
    /// </description>
    /// </summary>
    [Test(0, "ToolTip", "Exceptions")]
    public class ToolTipExceptions : XamlTest
    {
        #region Private Members
        ToolTip tooltip;
        #endregion

        #region Public Members

        public ToolTipExceptions()
            : base(@"ToolTipBehavior.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(TestExceptions);
        }

        public TestResult Setup()
        {
            Status("Setup");

            WaitForPriority(DispatcherPriority.ApplicationIdle);

            Button button = (Button)RootElement.FindName("button");
            if (button == null)
            {
                throw new TestValidationException("Button is null");
            }

            tooltip = button.ToolTip as ToolTip;
            if (tooltip == null)
            {
                throw new TestValidationException("ToolTip is null.");
            }
            tooltip.PlacementTarget = button;

            LogComment("Setup was successful");

            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            tooltip = null;
            return TestResult.Pass;
        }

        public TestResult TestExceptions()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            ToolTipActions.TestExceptions(tooltip);

            return TestResult.Pass;
        }

        #endregion
    } 
}
