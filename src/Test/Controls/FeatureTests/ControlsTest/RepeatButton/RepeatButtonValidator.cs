using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Since the RepeatButton test type doesn’t need be a state type and we want to support cleanup, so we make the type a Singleton
    /// 
    /// Usage:
    /// using (RepeatButtonValidator validator = RepeatButtonValidator.GetValidator)
    /// {
    ///     validator.Validate(source, isClickEventFired, key);
    /// }
    /// </summary>
    /// </summary>
    public class RepeatButtonValidator : IDisposable
    {
        private bool isDisposed = false;
        private static RepeatButtonValidator validator = new RepeatButtonValidator();
        private RepeatButtonValidator() { }
        static int clickCount = 1;

        static void source_Click(object sender, RoutedEventArgs e)
        {
            clickCount++;
        }

        /// <summary>
        /// Returns an instance of ButtonValidator
        /// </summary>
        public static RepeatButtonValidator GetValidator
        {
            get
            {
                return validator;
            }
        }

        /// <summary>
        /// Key Input secnario
        /// We use click event to validate RepeatButton Delay and Interval
        /// </summary>
        /// <param name="source">A RepeatButton button reference</param>
        /// <param name="isClickEventFired">Whether the click event fire or not</param>
        /// <param name="key">A key</param>
        public void Validate(RepeatButton source, bool shouldEventFire, Key key)
        {
            if (source == null)
            {
                throw new ArgumentNullException("Fail: the source is null.");
            }

            source.Click += source_Click;
            source.Focus();
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            Microsoft.Test.Input.Keyboard.Press(InputHelper.ConvertWpfKeyToTestKey(key));
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            DispatcherOperations.WaitFor(TimeSpan.FromMilliseconds(source.Delay));
            if (shouldEventFire)
            {
                if (clickCount != 3)
                {
                    throw new TestValidationException("Fail: RepeatButton.Delay actual click count " + clickCount + " doesn't equal to expected click count " + 3 + ".");
                }
            }
            else
            {
                if (clickCount != 1)
                {
                    throw new TestValidationException("Fail: RepeatButton.Delay fail - event shouldn't fire.");
                }
            }

            DispatcherOperations.WaitFor(TimeSpan.FromMilliseconds(source.Interval));
            if (shouldEventFire)
            {
                if (clickCount != 4)
                {
                    throw new TestValidationException("Fail: RepeatButton.Interval fail.");
                }
            }
            else
            {
                if (clickCount != 1)
                {
                    throw new TestValidationException("Fail: RepeatButton.Interval fail - event shouldn't fire.");
                }
            }

            // Cleanup
            Microsoft.Test.Input.Keyboard.Release(InputHelper.ConvertWpfKeyToTestKey(key));
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
            source.Click -= source_Click;
            clickCount = 1;
        }

        /// <summary>
        /// Mouse input secnario
        /// We use click event to validate RepeatButton Delay and Interval
        /// </summary>
        /// <param name="source">A RepeatButton button reference</param>
        /// <param name="isClickEventFired">Whether the click event fire or not</param>
        /// <param name="mouseButton">A MouseButton</param>
        public void Validate(RepeatButton source, bool shouldEventFire, System.Windows.Input.MouseButton mouseButton)
        {
            if (source == null)
            {
                throw new ArgumentNullException("Fail: the source is null.");
            }

            source.Click += source_Click;
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            InputHelper.MouseMoveToElementCenter(source);
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
            Microsoft.Test.Input.Mouse.Down(InputHelper.ConvertWpfMouseButtonToTestMouseButton(mouseButton));
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            if (shouldEventFire)
            {
                DispatcherOperations.WaitFor(TimeSpan.FromMilliseconds(source.Delay));
                if (shouldEventFire)
                {
                    if (clickCount != 3)
                    {
                        throw new TestValidationException("Fail: RepeatButton.Delay fail.");
                    }
                }
                else
                {
                    if (clickCount != 1)
                    {
                        throw new TestValidationException("Fail: RepeatButton.Delay fail - event shouldn't fire.");
                    }
                }

                DispatcherOperations.WaitFor(TimeSpan.FromMilliseconds(source.Interval));
                if (shouldEventFire)
                {
                    if (clickCount != 4)
                    {
                        throw new TestValidationException("Fail: RepeatButton.Interval fail.");
                    }
                }
                else
                {
                    if (clickCount != 1)
                    {
                        throw new TestValidationException("Fail: RepeatButton.Interval fail - event shouldn't fire.");
                    }
                }
            }

            // Cleanup
            Microsoft.Test.Input.Mouse.Up(InputHelper.ConvertWpfMouseButtonToTestMouseButton(mouseButton));
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
            source.Click -= source_Click;
            clickCount = 1;
        }

        #region IDisposable Members

        protected virtual void Dispose(bool disposing)
        {
            // When finalizer calls Microsoft.Test.Input.WpfMouse.Reset() will cause exception below.
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
