using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// TreeViewItemValidator
    /// </summary>
    public class TreeViewItemValidator : IDisposable
    {
        private bool isDisposed = false;
        private static TreeViewItemValidator validator = new TreeViewItemValidator();
        private TreeViewItemValidator() { }

        /// <summary>
        /// Returns an instance of SelectorValidator
        /// </summary>
        public static TreeViewItemValidator GetValidator
        {
            get
            {
                return validator;
            }
        }

        /// <summary>
        /// Validate whether TreeViewItem event fire or not after perform an action
        /// </summary>
        /// <typeparam name="T">A EventArgs type</typeparam>
        /// <param name="control">A reference of TreeViewItem</param>
        /// <param name="fromIndex">from index</param>
        /// <param name="toIndex">to index</param>
        /// <param name="eventName">Event name</param>
        /// <param name="t">A reference of EventArgs</param>
        /// <param name="shouldEventFire">Whether the event fire or not</param>
        /// <param name="action">An action</param>
        public void Validate<T>(ItemsControl control, int fromIndex, int toIndex, string eventName, T t, bool shouldEventFire, Action action) where T : EventArgs
        {
            if (control == null)
            {
                throw new ArgumentNullException("Fail: the source is null.");
            }

            if (String.IsNullOrEmpty(eventName))
            {
                throw new ArgumentNullException("Fail: the eventName is empty.");
            }

            TreeViewItem fromItem = TreeViewHelper.GetContainer(control, fromIndex);
            if (fromItem == null)
            {
                throw new ArgumentNullException("Fail: fromItem is null.");
            }
            fromItem.Focus();

            TreeViewItem toItem = TreeViewHelper.GetContainer(control, toIndex);
            if (toItem == null)
            {
                throw new ArgumentNullException("Fail: toItem is null.");
            }

            RoutedEventArgs routedEventArgs = t as RoutedEventArgs;
            if (routedEventArgs == null)
            {
                throw new ArgumentNullException("Fail: it is not RoutedEventArgs.");
            }

            routedEventArgs.Source = toItem;
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            EventHelper.ExpectEvent<T>(delegate()
            {
                action();
            }, toItem, eventName, t, shouldEventFire);

            if (shouldEventFire)
            {
                switch (eventName)
                {
                    case "Expanded":
                        if (!toItem.IsExpanded)
                        {
                            throw new TestValidationException("Fail: toItem is not Expanded when try to expand the toItem.");
                        }
                        break;
                    case "Collapsed":
                        if (toItem.IsExpanded)
                        {
                            throw new TestValidationException("Fail: toItem is not Collapsed when try to collapse the toItem.");
                        }

                        if (!toItem.IsSelected)
                        {
                            throw new TestValidationException("Fail: toItem is not Selected when try to select the toItem.");
                        }
                        break;
                    case "Selected":
                        if (!toItem.IsSelected)
                        {
                            throw new TestValidationException("Fail: toItem is not Selected when try to select the toItem.");
                        }

                        if (!toItem.IsSelectionActive)
                        {
                            throw new TestValidationException("Fail: toItem.IsSelectionActive is false when try to select the toItem.");
                        }
                        break;
                    case "Unselected":
                        if (toItem.IsSelected)
                        {
                            throw new TestValidationException("Fail: toItem is not Unselected when try to unselect the toItem.");
                        }
                        break;
                    default:
                        throw new NotSupportedException("Fail: unsupported event name " + eventName);
                }
            }
        }

        /// <summary>
        /// Validate whether TreeViewItem event fire or not after a mouse click
        /// </summary>
        /// <typeparam name="T">A EventArgs type</typeparam>
        /// <param name="control">A reference of TreeViewItem</param>
        /// <param name="fromIndex">from index</param>
        /// <param name="toIndex">to index</param>
        /// <param name="eventName">Event name</param>
        /// <param name="t">A reference of EventArgs</param>
        /// <param name="shouldEventFire">Whether the event fire or not</param>
        /// <param name="mouseButton">A MouseButton</param>
        public void Validate<T>(ItemsControl control, int fromIndex, int toIndex, string eventName, T t, bool shouldEventFire, System.Windows.Input.MouseButton mouseButton) where T : EventArgs
        {
            Validate<T>(control, fromIndex, toIndex, eventName, t, shouldEventFire, delegate()
            {
                TreeViewItem toItem = TreeViewHelper.GetContainer(control, toIndex);
                toItem.BringIntoView();
                DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

                switch (eventName)
                {
                    case "Selected":
                    case "Unselected":
                        ContentPresenter treeViewItemHeader = VisualTreeHelper.GetVisualChild<ContentPresenter, ItemsControl>(toItem);
                        InputHelper.MouseMoveToElementCenter(treeViewItemHeader);
                        break;
                    case "Expanded":
                    case "Collapsed":
                        ToggleButton itemExpandCollapseButton = VisualTreeHelper.GetVisualChild<ToggleButton, TreeViewItem>(toItem);
                        InputHelper.MouseMoveToElementCenter(itemExpandCollapseButton);
                        break;
                    default:
                        throw new NotImplementedException("Fail: unsupported event " + eventName + " testing.");
                }

                Microsoft.Test.Input.Mouse.Click(InputHelper.ConvertWpfMouseButtonToTestMouseButton(mouseButton));
                DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
            });
        }

        /// <summary>
        /// Validate whether TreeViewItem event fire or not after a key move
        /// </summary>
        /// <typeparam name="T">A EventArgs type</typeparam>
        /// <param name="control">A reference of TreeViewItem</param>
        /// <param name="fromIndex">from index</param>
        /// <param name="toIndex">to index</param>
        /// <param name="eventName">Event name</param>
        /// <param name="t">A reference of EventArgs</param>
        /// <param name="shouldEventFire">Whether the event fire or not</param>
        /// <param name="key">A Key</param>
        public void Validate<T>(ItemsControl control, int fromIndex, int toIndex, string eventName, T t, bool shouldEventFire, Key key) where T : EventArgs
        {
            Validate<T>(control, fromIndex, toIndex, eventName, t, shouldEventFire, delegate()
            {
                Microsoft.Test.Input.Keyboard.Type(InputHelper.ConvertWpfKeyToTestKey(key));
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
