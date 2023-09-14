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

namespace Avalon.Test.ComponentModel.UnitTests
{
    /// <summary>
    /// <description>
    /// Regression Test 
    /// This is not really a HiDPI issue. The culprit is the PopupControlService.
    /// WithinRenderBounds implementation. The reason this check was introduced 
    /// was to handle the case where an element had capture. In these cases the 
    /// Mouse.DirectlyOver state is tied to the captured element. The render 
    /// bounds check was meant to weed out the case that the mouse wasn't within 
    /// the render bounds of the mouse over element. However the math in this 
    /// implementation is incorrect. The fix implemented now is to hittest 
    /// again in the capture mode and find the raw element that the mouse is 
    /// over and display tooltips with reference to that.
    /// </description>
    /// </summary>
    [Test(1, "ToolTip", "ToolTipRegressionTest71")]
    public class ToolTipRegressionTest71 : XamlTest
    {
        #region Private Members
        Slider slider;
        ToolTip toolTip;
        DispatcherFrame closeFrame = new DispatcherFrame();
        #endregion

        #region Public Members

        public ToolTipRegressionTest71()
            : base(@"ToolTipRegressionTest71.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(MouseOverToOpenToolTip);
        }

        public TestResult Setup()
        {
            Status("Setup");

            WaitForPriority(DispatcherPriority.ApplicationIdle);

            slider = (Slider)RootElement.FindName("slider");

            if (slider == null)
            {
                throw new TestValidationException("Slider is null");
            }

            ToolTipService.SetShowDuration(slider, 3000);
            toolTip = slider.ToolTip as ToolTip;            
            if (toolTip == null)
            {
                throw new TestValidationException("ToolTip is null");
            }
            toolTip.Closed += OnToolTipClosing;

            LogComment("Setup was successful");

            return TestResult.Pass;
        }
        
        public TestResult CleanUp()
        {
            LogComment("wait for tooltip to close");
            Dispatcher.PushFrame(closeFrame);
            WaitFor(3000);

            slider = null;
            toolTip = null;
            return TestResult.Pass;
        }

        public TestResult MouseOverToOpenToolTip()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            // Mouse over top left of slider because it is the 
            UserInput.MouseMove(slider, Convert.ToInt32(slider.Width / 10), Convert.ToInt32(slider.Height / 10));
            QueueHelper.WaitForToolTipOpened(toolTip);

            if (!toolTip.IsOpen)
            {
                throw new TestValidationException("ToolTip is not opened.");
            }

            return TestResult.Pass;
        }

        private void OnToolTipClosing(object sender, EventArgs e)
        {
            toolTip.Closed -= OnToolTipClosing;
            closeFrame.Continue = false;
        }

        #endregion
    } 
}
