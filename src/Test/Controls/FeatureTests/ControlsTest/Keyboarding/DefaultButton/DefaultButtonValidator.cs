using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Test.Controls
{
    public enum DefaultButtonKey
    {
        Enter,
        Escape
    }

    public enum DefaultButtonMode
    {
        IsDefault,
        IsCancel
    }

    /// <summary>
    /// Since the keyboard navigation test type has no state and we want to support cleanup, so we make the type a Singleton
    /// 
    /// Usage:
    /// using (DefaultButtonValidator validator = DefaultButtonValidator.GetValidator)
    /// {
    ///     validator.Validate(DefaultButtonMode.IsDefault, button3, textbox, true, DefaultButtonKey.Enter);
    /// }
    /// </summary>
    public class DefaultButtonValidator : IDisposable
    {
        private bool isDisposed = false;
        private bool actualIsClicked = false;

        private static DefaultButtonValidator validator = new DefaultButtonValidator();
        private DefaultButtonValidator() { }

        public static DefaultButtonValidator GetValidator
        {
            get
            {
                return validator;
            }
        }

        /// <summary>
        /// Created for invalid keys scenarios
        /// </summary>
        /// <param name="defaultButtonMode">IsDefault or IsCancel</param>
        /// <param name="button">A button reference</param>
        /// <param name="focusElement">A focus element reference</param>
        /// <param name="expectedIsClicked">The expected value of click</param>
        /// <param name="key">A key</param>
        public void Validate(DefaultButtonMode defaultButtonMode, Button button, FrameworkElement focusElement, bool expectedIsClicked, Key key)
        {
            if (button == null)
            {
                throw new NullReferenceException("Fail: the button is null.");
            }

            if (focusElement == null)
            {
                throw new NullReferenceException("Fail: the focusElement is null.");
            }

            switch (defaultButtonMode)
            {
                case DefaultButtonMode.IsDefault:
                    button.IsDefault = true;
                    break;
                case DefaultButtonMode.IsCancel:
                    button.IsCancel = true;
                    break;
            }

            button.Click += button_Click;

            focusElement.Focus();
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
            Microsoft.Test.Input.Keyboard.Type(InputHelper.ConvertWpfKeyToTestKey(key));
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            if (actualIsClicked != expectedIsClicked)
            {
                throw new TestValidationException("Fail: the actual isClicked is " + actualIsClicked + " does not equal to the expected isClicked is " + expectedIsClicked);
            }

            // Clean up.
            button.Click -= button_Click;
        }

        void button_Click(object sender, RoutedEventArgs e)
        {
            actualIsClicked = true;
            if (((Button)sender).IsDefault == true)
            {
                if (((Button)sender).IsDefaulted != true)
                {
                    throw new TestValidationException("Fail: IsDefaulted is not true on IsDefault scenario after click event is raised.");
                }
            }
        }

        /// <summary>
        /// Created for valid keys scenarios
        /// The reason to have this method is to help people to understand what are the valid keys to test default button
        /// </summary>
        /// <param name="defaultButtonMode">IsDefault or IsCancel</param>
        /// <param name="button">A button reference</param>
        /// <param name="focusElement">A focus element reference</param>
        /// <param name="expectedIsClicked">The expected value of click</param>
        /// <param name="key">A key</param>
        public void Validate(DefaultButtonMode defaultButtonMode, Button button, FrameworkElement focusElement, bool expectedIsClicked, DefaultButtonKey defaultButtonKey)
        {
            if (button == null)
            {
                throw new NullReferenceException("Fail: the button is null.");
            }

            if (focusElement == null)
            {
                throw new NullReferenceException("Fail: the focusElement is null.");
            }

            Key key = (Key)Enum.Parse(typeof(Key), defaultButtonKey.ToString(), true);
            Validate(defaultButtonMode, button, focusElement, expectedIsClicked, key);
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
}
