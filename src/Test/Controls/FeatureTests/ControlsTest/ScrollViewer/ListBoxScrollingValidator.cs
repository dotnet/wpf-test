using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Test.Controls
{
    public enum DragThumbTo
    {
        Top,
        Middle,
        Bottom
    }

    public enum DragThumbScenario
    {
        TopToBottom,
        TopToMiddle,
        BottomToTop,
        BottomToMiddle
    }

    /// <summary>
    /// ListBoxScrollingValidator
    /// </summary>
    public class ListBoxScrollingValidator : IDisposable
    {
        private bool isDisposed = false;
        private static ListBoxScrollingValidator validator = new ListBoxScrollingValidator();
        private ListBoxScrollingValidator() { }

        /// <summary>
        /// Returns an instance of ButtonValidator
        /// </summary>
        public static ListBoxScrollingValidator GetValidator
        {
            get
            {
                return validator;
            }
        }

        /// <summary>
        /// Validate the ListBox types event whether fire or not after perform an action to scroll, and validate the expectedItemInViewportIndex item is in viewport
        /// </summary>
        /// <param name="control">A ListBox reference</param>
        /// <param name="expectedItemInViewportIndex">Expected Item InViewport Index</param>
        /// <param name="eventName">Event name</param>
        /// <param name="shouldEventFire">Whether the event fire or not</param>
        /// <param name="action">An action</param>
        public void Validate(ListBox control, int expectedItemInViewportIndex, string eventName, bool shouldEventFire, Action action)
        {
            ScrollViewer scrollViewer = VisualTreeHelper.GetVisualChild<ScrollViewer, ListBox>(control);

            RoutedEventHelper.ValidateEvent(scrollViewer, eventName, shouldEventFire, action);

            if (!ViewportHelper.IsInViewport((ListBoxItem)control.ItemContainerGenerator.ContainerFromIndex(expectedItemInViewportIndex)))
            {
                throw new TestValidationException("Fail: ListBoxItem at index " + expectedItemInViewportIndex + " is not in viewport.");
            }
        }

        /// <summary>
        /// Validate the ListBox types event whether fire or not after invoke a scrollviewer scrolling api, and validate the expectedItemInViewportIndex item is in viewport
        /// </summary>
        /// <param name="control">A ListBox reference</param>
        /// <param name="orientation">Scrolling Orientation</param>
        /// <param name="scrollingMode">Scrolling Mode</param>
        /// <param name="expectedItemInViewportIndex">Expected Item InViewport Index</param>
        /// <param name="eventName">Event name</param>
        /// <param name="shouldEventFire">Whether the event fire or not</param>
        public void Validate(ListBox control, Orientation orientation, ScrollingMode scrollingMode, int expectedItemInViewportIndex, string eventName, bool shouldEventFire)
        {
            ScrollViewer scrollViewer = VisualTreeHelper.GetVisualChild<ScrollViewer, ListBox>(control);

            switch (orientation)
            {
                case Orientation.Vertical:
                    Validate(control, expectedItemInViewportIndex, eventName, shouldEventFire, delegate()
                    {
                        switch (scrollingMode)
                        {
                            case ScrollingMode.LineDown:
                                scrollViewer.LineDown();
                                break;
                            case ScrollingMode.LineUp:
                                scrollViewer.LineUp();
                                break;
                            case ScrollingMode.PageDown:
                                scrollViewer.PageDown();
                                break;
                            case ScrollingMode.PageUp:
                                scrollViewer.PageUp();
                                break;
                            default:
                                throw new NotSupportedException("Fail: unsupported scrolling mode " + scrollingMode.ToString() + " in vertical scrolling orientation.");
                        }
                        DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
                    });
                    break;
                default:
                    throw new NotImplementedException("Fail: unsupported scrollingOrientation " + orientation.ToString());
            }
        }

        /// <summary>
        /// Validate the ListBox types event whether fire or not after mouse click to scroll, and validate the expectedItemInViewportIndex item is in viewport
        /// </summary>
        /// <param name="control">A ListBox reference</param>
        /// <param name="orientation">Scrolling Orientation</param>
        /// <param name="scrollingMode">Scrolling Mode</param>
        /// <param name="expectedItemInViewportIndex">Expected Item InViewport Index</param>
        /// <param name="eventName">Event name</param>
        /// <param name="shouldEventFire">Whether the event fire or not</param>
        /// <param name="mouseButton">Mouse Button</param>
        public void Validate(ListBox control, Orientation orientation, ScrollingMode scrollingMode, int expectedItemInViewportIndex, string eventName, bool shouldEventFire, System.Windows.Input.MouseButton mouseButton)
        {
            ScrollViewer scrollViewer = VisualTreeHelper.GetVisualChild<ScrollViewer, ListBox>(control);
            ScrollBar verticalScrollBar = ScrollViewerHelper.GetScrollBar(scrollViewer, ScrollBarPartName.PART_VerticalScrollBar);

            switch (orientation)
            {
                case Orientation.Vertical:
                    RepeatButton repeatButton = ScrollBarHelper.GetRepeatButton(verticalScrollBar, scrollingMode);

                    Validate(control, expectedItemInViewportIndex, eventName, shouldEventFire, delegate()
                    {
                        InputHelper.MouseMoveToElementCenter(repeatButton);
                        DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
                        Microsoft.Test.Input.Mouse.Click(InputHelper.ConvertWpfMouseButtonToTestMouseButton(mouseButton));
                        DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
                    });
                    break;
                default:
                    throw new NotImplementedException("Fail: unsupported scrollingOrientation " + orientation.ToString());
            }
        }

        /// <summary>
        /// Validate the ListBox types event whether fire or not after mouse wheel to scroll, and validate the expectedItemInViewportIndex item is in viewport
        /// </summary>
        /// <param name="control">A ListBox reference</param>
        /// <param name="orientation">Scrolling Orientation</param>
        /// <param name="focusedIndex">Focused item index</param>
        /// <param name="expectedItemInViewportIndex">Expected Item InViewport Index</param>
        /// <param name="eventName">Event name</param>
        /// <param name="shouldEventFire">Whether the event fire or not</param>
        /// <param name="scrollAmount">Scroll Amount</param>
        public void Validate(ListBox control, Orientation orientation, int focusedIndex, int expectedItemInViewportIndex, string eventName, bool shouldEventFire, int scrollAmount)
        {
            ListBoxItem focusedItem = control.ItemContainerGenerator.ContainerFromIndex(focusedIndex) as ListBoxItem;
            if (!ViewportHelper.IsInViewport(focusedItem))
            {
                throw new ArgumentOutOfRangeException("Fail: focusedItem is not in viewport.");
            }

            switch (orientation)
            {
                case Orientation.Vertical:
                    InputHelper.MouseMoveToElementCenter(focusedItem);
                    DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
                    Microsoft.Test.Input.Mouse.Click(Microsoft.Test.Input.MouseButton.Left);
                    DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

                    Validate(control, expectedItemInViewportIndex, eventName, shouldEventFire, delegate()
                    {
                        Microsoft.Test.Input.Mouse.Scroll(scrollAmount);
                        DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
                    });
                    break;
                default:
                    throw new NotImplementedException("Fail: unsupported scrollingOrientation " + orientation.ToString());
            }
        }

        /// <summary>
        /// Validate the ListBox types event whether fire or not after press key to scroll, and validate the expectedItemInViewportIndex item is in viewport
        /// </summary>
        /// <param name="control">A ListBox reference</param>
        /// <param name="orientation">Scrolling Orientation</param>
        /// <param name="focusedIndex">Focused item index</param>
        /// <param name="expectedItemInViewportIndex">Expected Item InViewport Index</param>
        /// <param name="eventName">Event name</param>
        /// <param name="shouldEventFire">Whether the event fire or not</param>
        /// <param name="key">A key</param>
        public void Validate(ListBox control, Orientation orientation, int focusedIndex, int expectedItemInViewportIndex, string eventName, bool shouldEventFire, Key key)
        {
            ListBoxItem focusedItem = control.ItemContainerGenerator.ContainerFromIndex(focusedIndex) as ListBoxItem;
            if (!ViewportHelper.IsInViewport(focusedItem))
            {
                throw new ArgumentOutOfRangeException("Fail: focusedItem is not in viewport.");
            }

            switch (orientation)
            {
                case Orientation.Vertical:
                    focusedItem.Focus();
                    DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

                    Validate(control, expectedItemInViewportIndex, eventName, shouldEventFire, delegate()
                    {
                        Microsoft.Test.Input.Keyboard.Type(InputHelper.ConvertWpfKeyToTestKey(key));
                        DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
                    });
                    break;
                default:
                    throw new NotImplementedException("Fail: unsupported scrollingOrientation " + orientation.ToString());
            }
        }

        /// <summary>
        /// Validate the ListBox types event whether fire or not after mouse drag scrollbar thumb to scroll, and validate the expectedItemInViewportIndex item is in viewport
        /// </summary>
        /// <param name="control">A ListBox reference</param>
        /// <param name="orientation">Scrolling Orientation</param>
        /// <param name="focusedIndex">Focused item index</param>
        /// <param name="expectedItemInViewportIndex">Expected Item InViewport Index</param>
        /// <param name="eventName">Event name</param>
        /// <param name="dragThumbScenario">Drag thumb scenario</param>
        public void Validate(ListBox control, Orientation orientation, int focusedIndex, int expectedItemInViewportIndex, string eventName, DragThumbTo dragThumbTo)
        {
            ListBoxItem focusedItem = control.ItemContainerGenerator.ContainerFromIndex(focusedIndex) as ListBoxItem;
            if (!ViewportHelper.IsInViewport(focusedItem))
            {
                throw new ArgumentOutOfRangeException("Fail: focusedItem is not in viewport.");
            }

            ListBoxItem topItem = control.ItemContainerGenerator.ContainerFromIndex(0) as ListBoxItem;
            ListBoxItem bottomItem = control.ItemContainerGenerator.ContainerFromIndex(control.Items.Count - 1) as ListBoxItem;
            ScrollViewer scrollViewer = VisualTreeHelper.GetVisualChild<ScrollViewer, ListBox>(control);
            ScrollContentPresenter presenter = ScrollViewerHelper.GetScrollContentPresenter(scrollViewer);
            bool shouldEventFire = false;

            if (!ScrollViewer.GetIsDeferredScrollingEnabled(control))
            {
                shouldEventFire = true;
            }
            else
            {
                expectedItemInViewportIndex = focusedIndex;
            }

            switch (orientation)
            {
                case Orientation.Vertical:
                    ScrollBar verticalScrollBar = ScrollViewerHelper.GetScrollBar(scrollViewer, ScrollBarPartName.PART_VerticalScrollBar);

                    focusedItem.Focus();
                    DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

                    Thumb thumb = VisualTreeHelper.GetVisualChild<Thumb, ScrollBar>(verticalScrollBar);

                    Validate(control, expectedItemInViewportIndex, eventName, shouldEventFire, delegate()
                    {
                        InputHelper.MouseMoveToElementCenter(thumb);
                        DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
                        Microsoft.Test.Input.Mouse.Down(Microsoft.Test.Input.MouseButton.Left);
                        DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

                        System.Drawing.Point dragToPoint = new System.Drawing.Point();
                        Window window = Window.GetWindow(control);
                        switch (dragThumbTo)
                        {
                            case DragThumbTo.Top:
                                if (window.FlowDirection == FlowDirection.LeftToRight)
                                {
                                    dragToPoint = new System.Drawing.Point(DpiHelper.ConvertToPhysicalPixel(presenter.PointToScreen(new Point()).X + presenter.ActualWidth), DpiHelper.ConvertToPhysicalPixel(presenter.PointToScreen(new Point()).Y));
                                }
                                else
                                {
                                    dragToPoint = new System.Drawing.Point(DpiHelper.ConvertToPhysicalPixel(presenter.PointToScreen(new Point()).X), DpiHelper.ConvertToPhysicalPixel(presenter.PointToScreen(new Point()).Y));
                                }
                                break;
                            case DragThumbTo.Middle:
                                if (window.FlowDirection == FlowDirection.LeftToRight)
                                {
                                    dragToPoint = new System.Drawing.Point(DpiHelper.ConvertToPhysicalPixel(presenter.PointToScreen(new Point()).X + presenter.ActualWidth), DpiHelper.ConvertToPhysicalPixel(presenter.PointToScreen(new Point()).Y + presenter.ActualHeight / 2));
                                }
                                else
                                {
                                    dragToPoint = new System.Drawing.Point(DpiHelper.ConvertToPhysicalPixel(presenter.PointToScreen(new Point()).X), DpiHelper.ConvertToPhysicalPixel(presenter.PointToScreen(new Point()).Y + presenter.ActualHeight / 2));
                                }
                                break;
                            case DragThumbTo.Bottom:
                                if (window.FlowDirection == FlowDirection.LeftToRight)
                                {
                                    dragToPoint = new System.Drawing.Point(DpiHelper.ConvertToPhysicalPixel(presenter.PointToScreen(new Point()).X + presenter.ActualWidth), DpiHelper.ConvertToPhysicalPixel(presenter.PointToScreen(new Point()).Y + presenter.ActualHeight));
                                }
                                else
                                {
                                    dragToPoint = new System.Drawing.Point(DpiHelper.ConvertToPhysicalPixel(presenter.PointToScreen(new Point()).X), DpiHelper.ConvertToPhysicalPixel(presenter.PointToScreen(new Point()).Y + presenter.ActualHeight));
                                }
                                break;
                        }

                        Microsoft.Test.Input.Mouse.MoveTo(dragToPoint);
                        DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
                    });

                    Microsoft.Test.Input.Mouse.Up(Microsoft.Test.Input.MouseButton.Left);
                    DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
                    break;
                default:
                    throw new NotImplementedException("Fail: unsupported scrollingOrientation " + orientation.ToString());
            }
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
