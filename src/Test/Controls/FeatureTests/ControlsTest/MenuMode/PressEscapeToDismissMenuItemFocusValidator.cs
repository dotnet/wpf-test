using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Test press escape key to dismiss menuitem focus
    /// </summary>
    public class PressEscapeToDismissMenuItemFocusValidator : MenuModeValidatorBase
    {
        public PressEscapeToDismissMenuItemFocusValidator(string focusTarget, string firstXAML, string secondXAML, bool acquireHwndFocusInMenuMode)
            : base(focusTarget, firstXAML, secondXAML, acquireHwndFocusInMenuMode)
        {
            this.acquireHwndFocusInMenuMode = acquireHwndFocusInMenuMode;
        }

        private bool acquireHwndFocusInMenuMode;

        protected override void Test()
        {
            // Action:
            //   Call window.Focus() method to set focus on window
            // Validation:
            //   Ensure the first window is actived
            Action action1 = () => firstWindow.Focus();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, action1);

            Action validation1 = () =>
            {
                if (!firstWindow.IsActive)
                {
                    throw new TestValidationException("Fail: first window is not Actived after called window.Focus() method.");
                }
            };
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, validation1);

            // Action:
            //   Call MenuItem.Focus() method to set focus on menuitem
            // Validation:
            // Two windows scenario:
            //   Ensure the menuitem (on the second window) is the focused element 
            //   Ensure the second window is actived when AcquireHwndFocusInMenuMode is true; the window is not actived when AcquireHwndFocusInMenuMode is false
            // One window scenario:
            //   Ensure the menuitem (on the window) is the focused element
            //   Ensure the window is actived
            MenuItem item = null;
            Action action2 = () =>
            {
                item = (MenuItem)secondWindow.FindName("SimpleMenuTop");
                if (item == null)
                {
                    throw new TestValidationException("Fail: MenuItem is null.");
                }
                item.Focus();
            };
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, action2);

            Action validaton2 = () =>
            {
                if (Keyboard.FocusedElement != item)
                {
                    throw new TestValidationException("Fail: MenuItem is not the focused element after called MenuItem.Focus() method.");
                }

                if (String.IsNullOrEmpty(secondXAML))
                {
                    if (!firstWindow.IsActive)
                    {
                        throw new TestValidationException("Fail: the first window is not actived after called MenuItem.Focus() method.");
                    }
                }
                else
                {
                    if (acquireHwndFocusInMenuMode)
                    {
                        if (!secondWindow.IsActive)
                        {
                            throw new TestValidationException("Fail: the second window is not actived after called MenuItem.Focus() method when AcquireHwndFocusInMenuMode is true.");
                        }
                    }
                    else
                    {
                        if (secondWindow.IsActive)
                        {
                            throw new TestValidationException("Fail: the second window is actived after called MenuItem.Focus() method when AcquireHwndFocusInMenuMode is false.");
                        }
                    }
                }
            };
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, validaton2);

            // Action:
            //   Press Escape key to dismiss the menuitem focus
            // Validation:
            //   Ensure the menuitem is not the focused element
            Action action3 = () => Microsoft.Test.Input.Keyboard.Press(Microsoft.Test.Input.Key.Escape);
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, action3);

            Action validaton3 = () =>
            {
                if (Keyboard.FocusedElement == item)
                {
                    throw new TestValidationException("Fail: MenuItem is still the focused element after pressed the escape key.");
                }
            };
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, validaton3);

            // Action:
            //   Close main window
            Action closeMainWindow = () => Application.Current.MainWindow.Close();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, closeMainWindow);
        }
    }
}
