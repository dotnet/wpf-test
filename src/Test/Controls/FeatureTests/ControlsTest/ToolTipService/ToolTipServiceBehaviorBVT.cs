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
    /// ToolTipServiceBehaviorBVT
    /// </description>
    /// </summary>
    [Test(1, "ToolTipService", "OpeningAndClosingToolTip")]
    public class ToolTipServiceBehaviorBVT : XamlTest
    {
        #region Private Members
        Button defaultButton;
        Button showDurationButton;
        #endregion

        #region Public Members

        public ToolTipServiceBehaviorBVT()
            : base(@"ToolTipServiceBehavior.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(TestOpeningAndClosingToolTip);
        }

        public TestResult Setup()
        {
            Status("Setup");

            WaitForPriority(DispatcherPriority.ApplicationIdle);

            defaultButton = (Button)RootElement.FindName("defaultButton");
            if (defaultButton == null)
            {
                throw new TestValidationException("Default button is null");
            }

            showDurationButton = (Button)RootElement.FindName("showDurationButton");
            if (showDurationButton == null)
            {
                throw new TestValidationException("ShowDuration Button is null");
            }

            LogComment("Setup was successful");

            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            LogComment("Wait for tooltip to close");   	     
            WaitFor(3000);
			
            defaultButton = null;
            showDurationButton = null;
            return TestResult.Pass;
        }

        public TestResult TestOpeningAndClosingToolTip()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            ToolTipServiceActions.OpeningAndClosingToolTip(defaultButton, OpeningToolTipServiceScenario.Default);
            ToolTipServiceActions.OpeningAndClosingToolTip(showDurationButton, OpeningToolTipServiceScenario.ShowDuration);

            return TestResult.Pass;
        }

        #endregion
    } 
}
