using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Controls;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Since the Button test type doesn’t need be a state type and we want to support cleanup, so we make the type a Singleton
    /// 
    /// Usage:
    /// using (ButtonValidator validator = ButtonValidator.GetValidator)
    /// {
    ///     validator.Validate(source, eventName, t, isEventFired, key);
    /// }
    /// </summary>
    public class ButtonValidator : IDisposable
    {
        private bool isDisposed = false;
        private static ButtonValidator validator = new ButtonValidator();
        private ButtonValidator() { }

        /// <summary>
        /// Returns an instance of ButtonValidator
        /// </summary>
        public static ButtonValidator GetValidator
        {
            get
            {
                return validator;
            }
        }

        /// <summary>
        /// Validate the ButtonBase types event whether fire or not after a key is pressed
        /// </summary>
        /// <typeparam name="T">EventArgs type</typeparam>
        /// <param name="source">A ButtonBase reference</param>
        /// <param name="eventName">An event name: string type</param>
        /// <param name="t">EventArgs value</param>
        /// <param name="isEventFired">Whether the event fire or not</param>
        /// <param name="key">A key</param>
        public void Validate<T>(ButtonBase source, string eventName, T t, bool shouldEventFire, Microsoft.Test.Input.Key key) where T : EventArgs
        {
            if (source == null)
            {
                throw new ArgumentNullException("Fail: the source is null.");
            }

            if (String.IsNullOrEmpty(eventName))
            {
                throw new ArgumentNullException("Fail: the eventName is empty.");
            }

            source.Focus();

            RoutedEventArgs routedEventArgs = t as RoutedEventArgs;
            if (routedEventArgs == null)
            {
                throw new ArgumentNullException("Fail: it is not RoutedEventArgs.");
            }

            routedEventArgs.Source = source;
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            EventHelper.ExpectEvent<T>(delegate()
            {
                switch (source.ClickMode)
                {
                    case ClickMode.Release:
                        Microsoft.Test.Input.Keyboard.Type(key);
                        break;
                    case ClickMode.Press:
                        Microsoft.Test.Input.Keyboard.Press(key);
                        break;
                    case ClickMode.Hover:
                        throw new TestValidationException("Fail: ClickMode.Hover is not supported in key scenario.");
                }
                DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
            }, source, eventName, t, shouldEventFire);

            // Cleanup
            if (source.ClickMode == ClickMode.Press)
            {
                Microsoft.Test.Input.Keyboard.Release(key);
                DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
            }
        }

        /// <summary>
        /// Validate the ButtonBase types event whether fire or not after a mouse button is click
        /// </summary>
        /// <typeparam name="T">EventArgs type</typeparam>
        /// <param name="source">A ButtonBase reference</param>
        /// <param name="eventName">An event name: string type</param>
        /// <param name="t">EventArgs value</param>
        /// <param name="isEventFired">Whether the event fire or not</param>
        /// <param name="mouseButton">A MouseButton</param>
        public void Validate<T>(ButtonBase source, string eventName, T t, bool shouldEventFire, System.Windows.Input.MouseButton mouseButton) where T : EventArgs
        {
            if (source == null)
            {
                throw new ArgumentNullException("Fail: the source is null.");
            }

            if (String.IsNullOrEmpty(eventName))
            {
                throw new ArgumentNullException("Fail: the eventName is empty.");
            }

            RoutedEventArgs routedEventArgs = t as RoutedEventArgs;
            if (routedEventArgs == null)
            {
                throw new ArgumentNullException("Fail: it is not RoutedEventArgs.");
            }

            routedEventArgs.Source = source;
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            EventHelper.ExpectEvent<T>(delegate()
            {
                if (source.ClickMode == ClickMode.Hover)
                {
                    InputHelper.MouseMoveToElementCenter(source);
                    DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);
                }
                else
                {
                    InputHelper.MouseClickButtonCenter(source, source.ClickMode, mouseButton);
                }
            }, source, eventName, t, shouldEventFire);
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
