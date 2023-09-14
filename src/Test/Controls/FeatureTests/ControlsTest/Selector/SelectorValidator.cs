using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Since the Selector test type doesn’t need be a state type and we want to support cleanup, so we make the type a Singleton
    /// 
    /// Usage:
    /// using (SelectorValidator validator = SelectorValidator.GetValidator)
    /// {
    ///     validator.Validate(source, fromIndex, toIndex, eventName, shouldEventFire, key);
    /// }
    /// </summary>
    public class SelectorValidator : IDisposable
    {
        private bool isDisposed = false;
        private static SelectorValidator validator = new SelectorValidator();
        private SelectorValidator() { }

        #region Private Members

        private static void SelectItem(Selector source, int fromIndex)
        {
            ContentControl fromItem = source.ItemContainerGenerator.ContainerFromIndex(fromIndex) as ContentControl;
            if (fromItem == null)
            {
                throw new ArgumentNullException("Fail: the fromItem at " + fromIndex + " is null.");
            }

            Selector.SetIsSelected(source.ItemContainerGenerator.ContainerFromIndex(fromIndex), true);

            fromItem.Focus();
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
        }

        private static void ValidateSelectedItem(Selector source, int toIndex)
        {
            ContentControl toItem = source.ItemContainerGenerator.ContainerFromIndex(toIndex) as ContentControl;
            if (toItem == null)
            {
                throw new ArgumentNullException("Fail: the toItem at " + toIndex + " is null.");
            }

            if (!(toItem is ListViewItem))
            {
                if (!Selector.GetIsSelected(toItem))
                {
                    throw new TestValidationException("Fail: Selector.GetIsSelected(toItem) is false.");
                }

                if (toIndex != source.SelectedIndex)
                {
                    throw new TestValidationException("Fail: the Selector.SelectedIndex doesn't equal to toIndex.");
                }
            }

            if (!Selector.GetIsSelectionActive(toItem))
            {
                throw new TestValidationException("Fail: Selector.GetIsSelectionActive(toItem) is false.");
            }

            if (source.Items[0] is ContentControl)
            {
                if (!object.ReferenceEquals(toItem, source.SelectedValue))
                {
                    throw new TestValidationException("Fail: source.SelectedValue doesn't equal to toItem.");
                }

                if (!object.ReferenceEquals(toItem, source.SelectedItem))
                {
                    throw new TestValidationException("Fail: Selector.SelectedItem doesn't equal to toItem.");
                }
            }
            else
            {
                if (!(toItem is ListViewItem))
                {
                    if (!object.ReferenceEquals(toItem.Content, source.SelectedValue))
                    {
                        throw new TestValidationException("Fail: source.SelectedValue doesn't equal to toItem.Content.");
                    }

                    if (!object.ReferenceEquals(toItem.Content, source.SelectedItem))
                    {
                        throw new TestValidationException("Fail: Selector.SelectedItem doesn't equal to toItem.Content.");
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Returns an instance of SelectorValidator
        /// </summary>
        public static SelectorValidator GetValidator
        {
            get
            {
                return validator;
            }
        }

        /// <summary>
        /// Validate Selector types: whether the event fire or not
        /// Mouse input select
        /// </summary>
        /// <param name="source">A Selector reference</param>
        /// <param name="fromIndex">A from index</param>
        /// <param name="toIndex">A to index</param>
        /// <param name="attachEventIndex"> A attach event index</param>
        /// <param name="eventName">Event name</param>
        /// <param name="shouldEventFire">Whether the event should fire or not</param>
        /// <param name="mouseButton">A MouseButton</param>
        public void Validate(Selector source, int fromIndex, int toIndex, int attachEventIndex, string eventName, bool shouldEventFire, System.Windows.Input.MouseButton mouseButton)
        {
            Validate(source, fromIndex, attachEventIndex, eventName, shouldEventFire, delegate()
            {
                ContentControl toItem = source.ItemContainerGenerator.ContainerFromIndex(toIndex) as ContentControl;
                if (toItem == null)
                {
                    throw new ArgumentNullException("Fail: the toItem is null.");
                }

                InputHelper.MouseClickCenter(toItem, mouseButton);
                DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
            });
        }

        /// <summary>
        /// Validate Selector types: whether the event fire or not
        /// Key input select
        /// </summary>
        /// <param name="source">A Selector reference</param>
        /// <param name="fromIndex">A from index</param>
        /// <param name="toIndex">A to index</param>
        /// <param name="eventName">Event name</param>
        /// <param name="shouldEventFire">Whether the event should fire or not</param>
        /// <param name="key">A Key</param>
        public void Validate(Selector source, int fromIndex, int attachEventIndex, string eventName, bool shouldEventFire, Key key)
        {
            Validate(source, fromIndex, attachEventIndex, eventName, shouldEventFire, delegate()
            {
                Microsoft.Test.Input.Keyboard.Type(InputHelper.ConvertWpfKeyToTestKey(key));
                DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
            });
        }

        /// <summary>
        /// Validate Selector types: whether the event fire or not
        /// </summary>
        /// <param name="source">A Selector reference</param>
        /// <param name="fromIndex">A from index</param>
        /// <param name="toIndex">A to index</param>
        /// <param name="eventName">Event name</param>
        /// <param name="shouldEventFire">Whether the event should fire or not</param>
        /// <param name="action">An action</param>
        public void Validate(Selector source, int fromIndex, int attachEventIndex, string eventName, bool shouldEventFire, Action action)
        {
            if (source == null)
            {
                throw new ArgumentNullException("Fail: the source is null.");
            }

            if (String.IsNullOrEmpty(eventName))
            {
                throw new ArgumentNullException("Fail: the eventName is empty.");
            }

            SelectItem(source, fromIndex);

            // Run test
            if (eventName.EndsWith("Handler"))
            {
                RoutedEventHelper.ValidateEventHandler(source, typeof(Selector), attachEventIndex, eventName, shouldEventFire, action);
            }
            else
            {
                RoutedEventHelper.ValidateEvent(source, eventName, shouldEventFire, action);
                ValidateSelectedItem(source, attachEventIndex);
            }
        }

        /// <summary>
        /// Validate Selector types: whether the event fire or not
        /// Mouse input select
        /// </summary>
        /// <typeparam name="T">An EventArgs type</typeparam>
        /// <param name="source">A Selector reference</param>
        /// <param name="fromIndex">A from index</param>
        /// <param name="toIndex">A to index</param>
        /// <param name="eventName">Event name</param>
        /// <param name="t">An EventArgs value</param>
        /// <param name="shouldEventFire">Whether the event should fire or not</param>
        /// <param name="mouseButton">A MouseButton</param>
        public void Validate<T>(Selector source, int fromIndex, int toIndex, string eventName, T t, bool shouldEventFire, System.Windows.Input.MouseButton mouseButton) where T : EventArgs
        {
            if (source == null)
            {
                throw new ArgumentNullException("Fail: the source is null.");
            }

            if (String.IsNullOrEmpty(eventName))
            {
                throw new ArgumentNullException("Fail: the eventName is empty.");
            }

            SelectItem(source, fromIndex);

            RoutedEventArgs routedEventArgs = t as RoutedEventArgs;
            if (routedEventArgs == null)
            {
                throw new ArgumentNullException("Fail: it is not a RoutedEventArgs since the as cast is null.");
            }
            routedEventArgs.Source = source;
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            EventHelper.ExpectEvent<T>(delegate()
            {
                ContentControl toItem = source.ItemContainerGenerator.ContainerFromIndex(toIndex) as ContentControl;
                if (toItem == null)
                {
                    throw new ArgumentNullException("Fail: the toItem is null.");
                }

                InputHelper.MouseClickCenter(toItem, mouseButton);
                DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
            }, source, eventName, t, shouldEventFire);

            if (shouldEventFire)
            {
                ValidateSelectedItem(source, toIndex);
            }
            else
            {
                ValidateSelectedItem(source, fromIndex);
            }
        }

        /// <summary>
        /// Validate Selector types: whether the event fire or not
        /// Key input select
        /// </summary>
        /// <typeparam name="T">An EventArgs type</typeparam>
        /// <param name="source">A Selector reference</param>
        /// <param name="fromIndex">A from index</param>
        /// <param name="toIndex">A to index</param>
        /// <param name="eventName">Event name</param>
        /// <param name="t">An EventArgs value</param>
        /// <param name="shouldEventFire">Whether the event should fire or not</param>
        /// <param name="key">A Key</param>
        public void Validate<T>(Selector source, int fromIndex, int toIndex, string eventName, T t, bool shouldEventFire, Key key) where T : EventArgs
        {
            if (source == null)
            {
                throw new ArgumentNullException("Fail: the source is null.");
            }

            if (String.IsNullOrEmpty(eventName))
            {
                throw new ArgumentNullException("Fail: the eventName is empty.");
            }

            SelectItem(source, fromIndex);

            RoutedEventArgs routedEventArgs = t as RoutedEventArgs;
            if (routedEventArgs == null)
            {
                throw new ArgumentNullException("Fail: it is not a RoutedEventArgs since the as cast is null.");
            }
            routedEventArgs.Source = source;
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            EventHelper.ExpectEvent<T>(delegate()
            {
                Microsoft.Test.Input.Keyboard.Type(InputHelper.ConvertWpfKeyToTestKey(key));
                DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
            }, source, eventName, t, shouldEventFire);

            if (shouldEventFire)
            {
                ValidateSelectedItem(source, toIndex);
            }
            else
            {
                ValidateSelectedItem(source, fromIndex);
            }
        }

        #region IDisposable Members

        protected virtual void Dispose(bool disposing)
        {
            // When finalizer calls Microsoft.Test.Input.WpfMouse.Reset() will cause exception below.
            // "The calling thread must be STA, because many UI components require this."
            // So remove finalizer.
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
