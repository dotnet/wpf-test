using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Test.Controls
{
    public enum LogicalNavigationKey
    {
        Tab,
        ShiftTab,
        CtrlTab,
        CtrlShiftTab
    }

    /// <summary>
    /// The directional navigation key.
    /// </summary>
    public enum DirectionalNavigationKey
    {
        Up,
        Down,
        Left,
        Right,
        Home,
        End,
        PageUp,
        PageDown
    }

    /// <summary>
    /// Since the keyboard navigation test type doesn’t need be a state type and we want to support cleanup, so we make the type a Singleton
    ///
    /// Usage:
    /// using (KeyboardNavigationValidator validator = KeyboardNavigationValidator.GetValidator)
    /// {
    ///     validator.DirectionalNavigate(fromElement, toElement, key);
    /// }
    /// </summary>
    public class KeyboardNavigationValidator : IDisposable
    {
        private bool isDisposed = false;
        private static KeyboardNavigationValidator validator = new KeyboardNavigationValidator();
        private KeyboardNavigationValidator() { }

        private void KeyNavigate(DependencyObject from, DependencyObject to, Key key)
        {
            from.Focus();
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
            Microsoft.Test.Input.Keyboard.Type(InputHelper.ConvertWpfKeyToTestKey(key));
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
            if (System.Windows.Input.Keyboard.FocusedElement != to)
            {
                throw new TestValidationException("Fail: the 'to' Element doesn't have focus.");
            }
        }

        /// <summary>
        /// Returns an instance of KeyboardingValidator
        /// </summary>
        public static KeyboardNavigationValidator GetValidator
        {
            get
            {
                return validator;
            }
        }

        /// <summary>
        /// Programmatically navigate focus scenarios
        /// </summary>
        /// <param name="from">A 'from' element reference</param>
        /// <param name="to">A 'to' element reference</param>
        /// <param name="focusNavigationDirection">a FocusNavigationDirection</param>
        public void FocusNavigationDirectionNavigate(DependencyObject from, DependencyObject to, FocusNavigationDirection focusNavigationDirection)
        {
            if (from == null)
            {
                throw new NullReferenceException("Fail: the from element is null.");
            }

            if (to == null)
            {
                throw new NullReferenceException("Fail: the to element is null.");
            }

            from.Focus();
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
            from.MoveFocus(new TraversalRequest(focusNavigationDirection));
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
            if (System.Windows.Input.Keyboard.FocusedElement != to)
            {
                throw new NullReferenceException("Fail: the 'to' Element doesn't have focus.");
            }
        }

        /// <summary>
        /// Created for valid keys scenarios
        /// The reason to have this method is to help people to understand what are the valid keys to test directional navigation
        /// </summary>
        /// <param name="from">A 'from' element reference</param>
        /// <param name="to">A 'to' elemeent reference</param>
        /// <param name="key">A key</param>
        public void DirectionalNavigate(DependencyObject from, DependencyObject to, DirectionalNavigationKey directionalNavigationKey)
        {
            if (from == null)
            {
                throw new NullReferenceException("Fail: the from element is null.");
            }

            if (to == null)
            {
                throw new NullReferenceException("Fail: the to element is null.");
            }

            Key key = (Key)Enum.Parse(typeof(Key), directionalNavigationKey.ToString(), true);
            DirectionalNavigate(from, to, key);
        }

        /// <summary>
        /// Created for invalid keys scenarios
        /// </summary>
        /// <param name="from">A 'from' element reference</param>
        /// <param name="to">A 'to' elemeent reference</param>
        /// <param name="key">A key</param>
        public void DirectionalNavigate(DependencyObject from, DependencyObject to, Key key)
        {
            if (from == null)
            {
                throw new NullReferenceException("Fail: the from element is null.");
            }

            if (to == null)
            {
                throw new NullReferenceException("Fail: the to element is null.");
            }

            KeyNavigate(from, to, key);
        }

        /// <summary>
        /// Created for delegate scenarios
        /// </summary>
        /// <param name="from">A 'from' element reference</param>
        /// <param name="to">A 'to' elemeent reference</param>
        /// <param name="key">A delegate reference</param>
        public void DirectionalNavigate(DependencyObject from, DependencyObject to, Action action)
        {
            if (from == null)
            {
                throw new NullReferenceException("Fail: the from element is null.");
            }

            if (to == null)
            {
                throw new NullReferenceException("Fail: the to element is null.");
            }

            from.Focus();
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
            action();
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
            if (System.Windows.Input.Keyboard.FocusedElement != to)
            {
                throw new TestValidationException("Fail: the 'to' Element doesn't have focus.");
            }
        }

        /// <summary>
        /// Created for invalid keys scenarios
        /// </summary>
        /// <param name="from">A 'from' element reference</param>
        /// <param name="to">A 'to' elemeent reference</param>
        /// <param name="key">A key</param>
        public void LogicalNavigate(DependencyObject from, DependencyObject to, Key key)
        {
            if (from == null)
            {
                throw new NullReferenceException("Fail: the from element is null.");
            }

            if (to == null)
            {
                throw new NullReferenceException("Fail: the to element is null.");
            }

            KeyNavigate(from, to, key);
        }

        /// <summary>
        /// Created for valid keys scenarios
        /// The reason to have this method is to help people to understand what are the valid keys to test logical navigation
        /// </summary>
        /// <param name="from">A 'from' element reference</param>
        /// <param name="to">A 'to' elemeent reference</param>
        /// <param name="key">A key</param>
        public void LogicalNavigate(DependencyObject from, DependencyObject to, LogicalNavigationKey logicalNavigationKey)
        {
            if (from == null)
            {
                throw new NullReferenceException("Fail: the from element is null.");
            }

            if (to == null)
            {
                throw new NullReferenceException("Fail: the to element is null.");
            }

            from.Focus();
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            switch (logicalNavigationKey)
            {
                case LogicalNavigationKey.Tab:
                    Microsoft.Test.Input.Keyboard.Type(Microsoft.Test.Input.Key.Tab);
                    break;
                case LogicalNavigationKey.ShiftTab:
                    Microsoft.Test.Input.Keyboard.Press(Microsoft.Test.Input.Key.LeftShift);
                    Microsoft.Test.Input.Keyboard.Press(Microsoft.Test.Input.Key.Tab);
                    DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
                    Microsoft.Test.Input.Keyboard.Release(Microsoft.Test.Input.Key.LeftShift);
                    Microsoft.Test.Input.Keyboard.Release(Microsoft.Test.Input.Key.Tab);
                    break;
                case LogicalNavigationKey.CtrlTab:
                    Microsoft.Test.Input.Keyboard.Press(Microsoft.Test.Input.Key.LeftCtrl);
                    Microsoft.Test.Input.Keyboard.Press(Microsoft.Test.Input.Key.Tab);
                    DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
                    Microsoft.Test.Input.Keyboard.Release(Microsoft.Test.Input.Key.LeftCtrl);
                    Microsoft.Test.Input.Keyboard.Release(Microsoft.Test.Input.Key.Tab);
                    break;
                case LogicalNavigationKey.CtrlShiftTab:
                    Microsoft.Test.Input.Keyboard.Press(Microsoft.Test.Input.Key.LeftCtrl);
                    Microsoft.Test.Input.Keyboard.Press(Microsoft.Test.Input.Key.LeftShift);
                    Microsoft.Test.Input.Keyboard.Press(Microsoft.Test.Input.Key.Tab);
                    DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
                    Microsoft.Test.Input.Keyboard.Release(Microsoft.Test.Input.Key.LeftShift);
                    Microsoft.Test.Input.Keyboard.Release(Microsoft.Test.Input.Key.LeftCtrl);
                    Microsoft.Test.Input.Keyboard.Release(Microsoft.Test.Input.Key.Tab);
                    break;
            }
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            if (System.Windows.Input.Keyboard.FocusedElement != to)
            {
                throw new TestValidationException("Fail: the 'to' Element doesn't have focus.");
            }
        }

        #region IDisposable Members

        protected virtual void Dispose(bool disposing)
        {
            // When finalizer calls Mouse.Reset() will cause exception below.
            // "The calling thread must be STA, because many UI components require this."
            // So, remove finalizer.
            if (!isDisposed)
            {
                Microsoft.Test.Input.Mouse.Reset();
                DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);
                Microsoft.Test.Input.Keyboard.Reset();
                DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);

                isDisposed = true;
            }
        }

        public void Cleanup()
        {
            Dispose(true);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }

    public static class FocusExtensions
    {
        public static void Focus(this DependencyObject d)
        {
            UIElement uie;
            ContentElement ce;
            UIElement3D uie3d;

            if ((uie = d as UIElement) != null)
            {
                uie.Focus();
            }
            else if ((ce = d as ContentElement) != null)
            {
                ce.Focus();
            }
            else if ((uie3d = d as UIElement3D) != null)
            {
                uie3d.Focus();
            }
        }

        public static void MoveFocus(this DependencyObject d, TraversalRequest tr)
        {
            UIElement uie;
            ContentElement ce;
            UIElement3D uie3d;

            if ((uie = d as UIElement) != null)
            {
                uie.MoveFocus(tr);
            }
            else if ((ce = d as ContentElement) != null)
            {
                ce.MoveFocus(tr);
            }
            else if ((uie3d = d as UIElement3D) != null)
            {
                uie3d.MoveFocus(tr);
            }
        }
    }
}
