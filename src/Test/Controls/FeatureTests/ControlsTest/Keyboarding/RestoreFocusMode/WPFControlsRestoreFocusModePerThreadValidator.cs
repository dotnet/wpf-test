using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Use WPF controls to test RestoreFocusMode on a new thread.
    /// </summary>
    public class WPFControlsRestoreFocusModePerThreadValidator : RestoreFocusModePerThreadValidatorBase
    {
        public WPFControlsRestoreFocusModePerThreadValidator(RestoreFocusMode restoreFocusMode)
            : base(restoreFocusMode)
        {
        }

        protected override void AttachContentToWindow(Window window)
        {
            StackPanel panel = new StackPanel();

            TextBox textbox = new TextBox();
            textbox.Text = "TextBox";
            panel.Children.Add(textbox);

            Win32EditBox win32EditBox = new Win32EditBox();
            win32EditBox.Width = 100;
            win32EditBox.Height = 20;
            panel.Children.Add(win32EditBox);

            window.Content = panel;

            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);

            // Test WPF TextBox
            RunRestoreFocusModeTests(textbox);
        }
    }
}
