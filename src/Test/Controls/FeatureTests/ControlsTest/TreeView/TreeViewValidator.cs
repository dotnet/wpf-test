using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// TreeViewValidator
    /// </summary>
    public class TreeViewValidator : IDisposable
    {
        private bool isDisposed = false;
        private static TreeViewValidator validator = new TreeViewValidator();
        private TreeViewValidator() { }

        /// <summary>
        /// Returns an instance of SelectorValidator
        /// </summary>
        public static TreeViewValidator GetValidator
        {
            get
            {
                return validator;
            }
        }

        /// <summary>
        /// Validate whether TreeView event fire or not after perform an action
        /// </summary>
        /// <typeparam name="T">A EventArgs type</typeparam>
        /// <param name="control">A reference of TreeView</param>
        /// <param name="fromIndex">from index</param>
        /// <param name="toIndex">to index</param>
        /// <param name="eventName">event name</param>
        /// <param name="t">A reference of EventArgs</param>
        /// <param name="shouldEventFire">Whether the event fire or not</param>
        /// <param name="action">An action</param>
        public void Validate<T>(TreeView control, int fromIndex, int toIndex, string eventName, T t, bool shouldEventFire, Action action) where T : EventArgs
        {
            if (String.IsNullOrEmpty(eventName))
            {
                throw new ArgumentNullException("Fail: eventName is empty.");
            }

            TreeViewItem fromItem = TreeViewHelper.GetContainer(control, fromIndex);
            fromItem.Focus();
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            if (eventName.Equals("SelectedItemChanged"))
            {
                RoutedPropertyChangedEventArgs<object> expectedEventArgs = t as RoutedPropertyChangedEventArgs<object>;
                if (expectedEventArgs == null)
                {
                    throw new ArgumentNullException("Fail: it is not RoutedEventArgs.");
                }

                RoutedPropertyChangedEventArgs<object> actualEventArgs = null;

                EventHelper.ExpectEvent<T>(delegate()
                {
                    action();
                }, control, eventName, delegate(object sender, T args)
                {
                    actualEventArgs = args as RoutedPropertyChangedEventArgs<object>;
                });

                if (shouldEventFire)
                {
                    if (expectedEventArgs.OldValue == null)
                    {
                        if (actualEventArgs.OldValue != null)
                        {
                            throw new TestValidationException("Fail: actualEventArgs.OldValue is not null when expectedEventArgs.OldValue is null.");
                        }
                    }
                    else
                    {
                        if (!Object.ReferenceEquals(actualEventArgs.OldValue, expectedEventArgs.OldValue))
                        {
                            throw new TestValidationException("Fail: actualEventArgs.OldValue doesn't equeal to expectedEventArgs.OldValue.");
                        }
                    }

                    if (!Object.ReferenceEquals(actualEventArgs.NewValue, expectedEventArgs.NewValue))
                    {
                        throw new TestValidationException("Fail: actualEventArgs.NewValue doesn't equeal to expectedEventArgs.NewValue.");
                    }

                    if (!Object.ReferenceEquals(control.SelectedItem, control.Items[toIndex]))
                    {
                        throw new TestValidationException("Fail: treeview.SelectedItem doesn't equal to treeview.Items[toIndex].");
                    }

                    if (!(control.Items[0] is TreeViewItem))
                    {
                        if (!String.Equals(control.SelectedValue.ToString(), ""))
                        {
                            throw new TestValidationException("Fail: treeview.SelectedValue doesn't equal to ''.");
                        }
                    }
                }
                else
                {
                    if (!Object.ReferenceEquals(control.SelectedItem, control.Items[fromIndex]))
                    {
                        throw new TestValidationException("Fail: treeview.SelectedItem doesn't equal to treeview.Items[fromIndex].");
                    }
                }
            }
            else
            {
                throw new NotImplementedException("Fail: unsupported event " + eventName + " testing.");
            }
        }

        /// <summary>
        /// Validate whether TreeView event fire or not after a key move
        /// </summary>
        /// <typeparam name="T">A EventArgs type</typeparam>
        /// <param name="control">A reference of TreeView</param>
        /// <param name="fromIndex">from index</param>
        /// <param name="toIndex">to index</param>
        /// <param name="eventName">event name</param>
        /// <param name="t">A reference of EventArgs</param>
        /// <param name="shouldEventFire">Whether the event fire or not</param>
        /// <param name="key">A key</param>
        public void Validate<T>(TreeView control, int fromIndex, int toIndex, string eventName, T t, bool shouldEventFire, Key key) where T : EventArgs
        {
            Validate<T>(control, fromIndex, toIndex, eventName, t, shouldEventFire, delegate()
            {
                Microsoft.Test.Input.Keyboard.Type(InputHelper.ConvertWpfKeyToTestKey(key));
                DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
            });
        }

        /// <summary>
        /// Validate whether TreeView event fire or not after a mouse click
        /// </summary>
        /// <typeparam name="T">A EventArgs type</typeparam>
        /// <param name="control">A reference of TreeView</param>
        /// <param name="fromIndex">from index</param>
        /// <param name="toIndex">to index</param>
        /// <param name="eventName">event name</param>
        /// <param name="t">A reference of EventArgs</param>
        /// <param name="shouldEventFire">Whether the event fire or not</param>
        /// <param name="mouseButton">A MouseButton</param>
        public void Validate<T>(TreeView control, int fromIndex, int toIndex, string eventName, T t, bool shouldEventFire, System.Windows.Input.MouseButton mouseButton) where T : EventArgs
        {
            TreeViewItem toItem = TreeViewHelper.GetContainer(control, toIndex);

            Validate<T>(control, fromIndex, toIndex, eventName, t, shouldEventFire, delegate()
            {
                ContentPresenter treeViewItemHeader = VisualTreeHelper.GetVisualChild<ContentPresenter, TreeViewItem>(toItem);
                InputHelper.MouseMoveToElementCenter(treeViewItemHeader);
                Microsoft.Test.Input.Mouse.Click(InputHelper.ConvertWpfMouseButtonToTestMouseButton(mouseButton));
                DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
            });
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
