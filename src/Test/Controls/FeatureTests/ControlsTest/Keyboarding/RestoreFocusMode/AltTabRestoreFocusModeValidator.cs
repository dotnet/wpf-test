using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Use Alt+Tab to switch window focus to test RestoreFocusMode feature.
    /// </summary>
    public class AltTabRestoreFocusModeValidator : RestoreFocusModeValidatorBase
    {
        public AltTabRestoreFocusModeValidator(FrameworkElement targetElement)
            : base(targetElement)
        {
        }

        private void AltTab()
        {
            Microsoft.Test.Input.Keyboard.Press(Microsoft.Test.Input.Key.LeftAlt);
            Microsoft.Test.Input.Keyboard.Press(Microsoft.Test.Input.Key.Tab);
            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);
            Microsoft.Test.Input.Keyboard.Release(Microsoft.Test.Input.Key.Tab);
            Microsoft.Test.Input.Keyboard.Release(Microsoft.Test.Input.Key.LeftAlt);
            DispatcherOperations.WaitFor(TimeSpan.FromMilliseconds(500));
        }

        /// <summary>
        /// Alt+Tab to lose window focus.
        /// </summary>
        protected override void LoseWindowFocus()
        {
            AltTab();

            if (window.IsActive)
            {
                throw new TestValidationException("Fail: the window is active after lost window focus.");
            }
        }

        /// <summary>
        /// Alt+Tab to set focus back on window.
        /// </summary>
        protected override void SetFocusBackOnWindow()
        {
            AltTab();

            if (!window.IsActive)
            {
                throw new TestValidationException("Fail: the window is inactive after set focus on window.");
            }
        }
    }
}
