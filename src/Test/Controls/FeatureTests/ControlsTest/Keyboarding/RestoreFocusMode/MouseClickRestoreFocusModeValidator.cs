using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Use mouse click to switch window focus to test RestoreFocusMode feature.
    /// </summary>
    public class MouseClickRestoreFocusModeValidator : RestoreFocusModeValidatorBase
    {
        private const int OffsetX = 10;
        private const int OffsetY = 10;

        public MouseClickRestoreFocusModeValidator(FrameworkElement targetElement)
            : base(targetElement)
        {
        }

        /// <summary>
        /// Set window lefttop coordiate to (50, 50), so we click on (10, 10) to lose window focus.
        /// Mouse click to lose window focus.
        /// </summary>
        protected override void LoseWindowFocus()
        {
            window.Left = 50;
            window.Top = 50;
            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);
            Microsoft.Test.Input.Mouse.MoveTo(new System.Drawing.Point(OffsetX, OffsetY));
            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);
            Microsoft.Test.Input.Mouse.Click(Microsoft.Test.Input.MouseButton.Left);
            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);

            if (window.IsActive)
            {
                throw new TestValidationException("Fail: the window is active after lost window focus.");
            }
        }

        /// <summary>
        /// Mouse click to set focus back on window.
        /// </summary>
        protected override void SetFocusBackOnWindow()
        {
            Point windowTopLeftPointToScreenLogicalPixel = window.PointToScreen(new Point());
            System.Drawing.Point windowChormePointToScreen = new System.Drawing.Point(DpiHelper.ConvertToPhysicalPixel(windowTopLeftPointToScreenLogicalPixel.X + window.ActualWidth / 2), DpiHelper.ConvertToPhysicalPixel(windowTopLeftPointToScreenLogicalPixel.Y - 8));
            Microsoft.Test.Input.Mouse.MoveTo(windowChormePointToScreen);
            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);
            Microsoft.Test.Input.Mouse.Click(Microsoft.Test.Input.MouseButton.Left);
            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);

            if (!window.IsActive)
            {
                throw new TestValidationException("Fail: the window is inactive after set focus on window.");
            }
        }
    }
}
