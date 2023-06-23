using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Since the keyboard navigation test type doesn’t need be a state type and we want to support cleanup, so we make the type a Singleton
    /// 
    /// Usage:
    /// using (AccessKeyValidator validator = AccessKeyValidator.GetValidator)
    /// {
    ///     validator.Validate(source, expectedIsAccessKeyPressed, key);
    /// }
    /// </summary>
    public class AccessKeyValidator : IDisposable
    {
        private bool isDisposed = false;
        private bool actualIsAccessKeyPressed = false;
        private static AccessKeyValidator validator = new AccessKeyValidator();
        private AccessKeyValidator() { }

        private void AccessKeyPressedCallback(object sender, AccessKeyPressedEventArgs e)
        {
            actualIsAccessKeyPressed = true;
        }

        /// <summary>
        /// Returns an instance of AccessKeyValidator
        /// </summary>
        public static AccessKeyValidator GetValidator
        {
            get
            {
                return validator;
            }
        }

        /// <summary>
        /// Validate FrameworkElement types: AccessKeyPressed event fired after access key is pressed
        /// </summary>
        /// <param name="source">A FrameworkElement reference</param>
        /// <param name="isAccessKeyPressed">The expected value of isAccessKeyPressed</param>
        /// <param name="key">A key</param>
        public void Validate(FrameworkElement source, bool expectedIsAccessKeyPressed, Key key)
        {
            if (source == null)
            {
                throw new NullReferenceException("Fail: source is null.");
            }

            AccessKeyManager.AddAccessKeyPressedHandler(source, AccessKeyPressedCallback);

            var testKey = InputHelper.ConvertWpfKeyToTestKey(key);
            Microsoft.Test.Input.Keyboard.Press(Microsoft.Test.Input.Key.LeftAlt);
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
            Microsoft.Test.Input.Keyboard.Type(testKey);
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
            Microsoft.Test.Input.Keyboard.Release(Microsoft.Test.Input.Key.LeftAlt);
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            if (actualIsAccessKeyPressed != expectedIsAccessKeyPressed)
            {
                throw new TestValidationException("Fail: actualIsAccessKeyPressed does not equal to expectedIsAccessKeyPressed");
            }

            // Clean up: Remvoe attached event handler.
            AccessKeyManager.RemoveAccessKeyPressedHandler(source, AccessKeyPressedCallback);
            actualIsAccessKeyPressed = false;
        }

        /// <summary>
        /// Validate MenuItem event whether fire or not after an accesskey action is performed
        /// </summary>
        /// <typeparam name="T">EventArgs type</typeparam>
        /// <param name="source">A MenuItem reference</param>
        /// <param name="eventName">An event name: string type</param>
        /// <param name="t">EventArgs value</param>
        /// <param name="isEventFired">Whether the event fire or not</param>
        /// <param name="action">Access key action</param>
        public void Validate<T>(MenuItem source, string eventName, T t, bool isEventFired, Action action) where T : EventArgs
        {
            RoutedEventArgs routedEventArgs = t as RoutedEventArgs;
            if (routedEventArgs == null)
            {
                throw new NullReferenceException("Fail: it is not RoutedEventArgs.");
            }
            routedEventArgs.Source = source;

            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            EventHelper.ExpectEvent<T>(delegate()
            {
                action();
            }, source, eventName, t, isEventFired);
        }

        /// <summary>
        /// Validate ButtonBase types event whether fire or not after an access key is pressed
        /// </summary>
        /// <typeparam name="T">EventArgs type</typeparam>
        /// <param name="source">A ButtonBase reference</param>
        /// <param name="eventName">An event name: string type</param>
        /// <param name="t">EventArgs value</param>
        /// <param name="isEventFired">Whether the event fire or not</param>
        /// <param name="key">A key</param>
        public void Validate<T>(ButtonBase source, string eventName, T t, bool isEventFired, Key key) where T : EventArgs
        {
            RoutedEventArgs routedEventArgs = t as RoutedEventArgs;
            if (routedEventArgs == null)
            {
                throw new NullReferenceException("Fail: it is not RoutedEventArgs.");
            }
            routedEventArgs.Source = source;

            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            EventHelper.ExpectEvent<T>(delegate()
            {
                var testKey = InputHelper.ConvertWpfKeyToTestKey(key);
                Microsoft.Test.Input.Keyboard.Press(Microsoft.Test.Input.Key.LeftAlt);
                DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
                Microsoft.Test.Input.Keyboard.Type(testKey);
                DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
                Microsoft.Test.Input.Keyboard.Release(Microsoft.Test.Input.Key.LeftAlt);
                DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
            }, source, eventName, t, isEventFired);
        }

        /// <summary>
        /// Test Binding target scenario below
        /// <Label Content="_Label" Target="{Binding ElementName=button1}"/>
        /// <Button Name="button1" Content="button1" Width="50"/>
        /// </summary>
        /// <typeparam name="T">EventArgs type</typeparam>
        /// <param name="source">A Label reference</param>
        /// <param name="target">A target reference</param>
        /// <param name="eventName">A target event name: string type</param>
        /// <param name="t">EventArgs value</param>
        /// <param name="isEventFired">Whether the event fire or not</param>
        /// <param name="key">A key</param>
        public void Validate<T>(Label source, FrameworkElement target, string eventName, T t, bool isEventFired, Key key) where T : EventArgs
        {
            if (source == null)
            {
                throw new NullReferenceException("Fail: source is null.");
            }

            if (target == null)
            {
                throw new NullReferenceException("Fail: target is null.");
            }

            if (String.IsNullOrEmpty(eventName))
            {
                throw new NullReferenceException("Fail: target event name is empty.");
            }

            Binding binding = new Binding();
            binding.Source = target;
            source.SetBinding(Label.TargetProperty, binding);

            RoutedEventArgs routedEventArgs = t as RoutedEventArgs;
            if (routedEventArgs == null)
            {
                throw new NullReferenceException("Fail: t is not RoutedEventArgs.");
            }
            routedEventArgs.Source = target;

            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            EventHelper.ExpectEvent<T>(delegate()
            {
                var testKey = InputHelper.ConvertWpfKeyToTestKey(key);
                Microsoft.Test.Input.Keyboard.Press(Microsoft.Test.Input.Key.LeftAlt);
                DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
                Microsoft.Test.Input.Keyboard.Type(testKey);
                DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
                Microsoft.Test.Input.Keyboard.Release(Microsoft.Test.Input.Key.LeftAlt);
                DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
            }, target, eventName, t, isEventFired);
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
