using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Test.Threading;
// this does not use UIA but does make available user freindly apis that use Microsoft.Test.Input
using UiInput = Annotations.Test.Framework.UIAutomationModule;

namespace Microsoft.Test.Controls
{
    public static class InputHelper
    {
        /// <summary>
        /// Move to center of a UiElement and click
        /// </summary>
        public static void Click(UIElement target)
        {
            UiInput.MoveToAndClickElement(target);
            DispatcherHelper.DoEvents(500);
        }

        /// <summary>
        /// Perform a mouse click at a desired point
        /// </summary>
        public static void ClickAt(Point point)
        {
            UiInput.MoveToAndClick(point);
            DispatcherHelper.DoEvents(1000);
        }

        /// <summary>
        /// Release Left Mouse button
        /// </summary>
        public static void LeftMouseDown()
        {
            UiInput.LeftMouseDown();
            DispatcherHelper.DoEvents(100);
        }

        /// <summary>
        /// Release Left Mouse button over UIElement
        /// </summary>
        public static void LeftMouseDown(UIElement target)
        {
            UiInput.MoveToCenter(target);
            DispatcherHelper.DoEvents(100);
            UiInput.LeftMouseDown();
            DispatcherHelper.DoEvents(100);
        }

        /// <summary>
        /// Release Left Mouse button
        /// </summary>
        public static void LeftMouseUp()
        {
            UiInput.LeftMouseUp();
            DispatcherHelper.DoEvents(100);
        }

        /// <summary>
        /// Release Left Mouse button over UIElement
        /// </summary>
        public static void LeftMouseUp(UIElement target)
        {
            UiInput.MoveToCenter(target);
            DispatcherHelper.DoEvents(100);
            UiInput.LeftMouseUp();
            DispatcherHelper.DoEvents(100);
        }

        /// <summary>
        /// Make a selection by dragging the mouse
        /// </summary>
        public static void DragSelection(UIElement start, UIElement finish)
        {
            UiInput.MoveToCenter(start);
            DispatcherHelper.DoEvents(500);
            UiInput.LeftMouseDown();
            DispatcherHelper.DoEvents(500);

            UiInput.MoveToCenter(finish);
            DispatcherHelper.DoEvents(500);
            UiInput.LeftMouseUp();
            DispatcherHelper.DoEvents(500);
        }

        /// <summary>
        /// Send input to press desired key on keyboard
        /// </summary>
        public static void PressKey(Key key)
        {
            UiInput.PressKey(key);
            DispatcherHelper.DoEvents(100);
        }

        /// <summary />
        public static void PushKey(Key key)
        {
            Microsoft.Test.Input.Input.SendKeyboardInput(key, true);
            DispatcherHelper.DoEvents(100);
        }

        /// <summary />
        public static void ReleaseKey(Key key)
        {
            Microsoft.Test.Input.Input.SendKeyboardInput(key, false);
            DispatcherHelper.DoEvents(100);
        }

        /// <summary>
        /// Check to see if the mouse click point is within wpf client area.
        /// </summary>
        /// <param name="window">wpf window.</param>
        /// <param name="clickPoint">wpf logical pixel.</param>
        /// <returns>
        /// Returns true if the point is within the wpf client area.
        /// Returns false if the point is not within the wpf client area.
        /// </returns>
        public static bool IsMouseClickWithinClientArea(Window window, Point clickPoint)
        {
            FlowDirection windowFlowDirection = window.FlowDirection;
            switch (windowFlowDirection)
            {
                case FlowDirection.LeftToRight:
                    if (clickPoint.X < window.PointToScreen(new Point()).X || clickPoint.X > window.PointToScreen(new Point()).X + window.ActualWidth || clickPoint.Y < window.PointToScreen(new Point()).Y || clickPoint.Y > window.PointToScreen(new Point()).Y + window.ActualHeight)
                    {
                        return false;
                    }
                    break;
                case FlowDirection.RightToLeft:
                    if (clickPoint.X < window.PointToScreen(new Point()).X || clickPoint.X < window.PointToScreen(new Point()).X - window.ActualWidth || clickPoint.Y < window.PointToScreen(new Point()).Y || clickPoint.Y > window.PointToScreen(new Point()).Y + window.ActualHeight)
                    {
                        return false;
                    }
                    break;
            }

            return true;
        }

        /// <summary>
        /// Mouse move to center of element
        /// </summary>
        /// <param name="targetElement">A FrameworkElement reference</param>
        public static void MouseMoveToElementCenter(FrameworkElement targetElement)
        {
            Point elementPointToScreen = targetElement.PointToScreen(new Point());
            Point elementMiddlePointToScreen = default(Point);

            FlowDirection windowFlowDirection = Window.GetWindow(targetElement).FlowDirection;

            // We need to convert element width and height logical pixel to screen pixel.
            double convertedHalfWidth = DpiHelper.ConvertToPhysicalPixel(targetElement.ActualWidth / 2);
            double convertedhalfHeight = DpiHelper.ConvertToPhysicalPixel(targetElement.ActualHeight / 2);

            // We are handling the LTR and RTL flowdirection scenarios.
            switch (windowFlowDirection)
            {
                case FlowDirection.LeftToRight:
                    elementMiddlePointToScreen = new Point(elementPointToScreen.X + convertedHalfWidth, elementPointToScreen.Y + convertedhalfHeight);
                    break;
                case FlowDirection.RightToLeft:
                    elementMiddlePointToScreen = new Point(elementPointToScreen.X - convertedHalfWidth, elementPointToScreen.Y + convertedhalfHeight);
                    break;
            }

            System.Drawing.Point elementCenterPointToScreen = new System.Drawing.Point(Convert.ToInt32(elementMiddlePointToScreen.X), Convert.ToInt32(elementMiddlePointToScreen.Y));
            Microsoft.Test.Input.Mouse.MoveTo(elementCenterPointToScreen);
        }

        /// <summary>
        /// Mouse click on center of Button
        /// </summary>
        /// <param name="targetElement">A ButtonBase reference</param>
        public static void MouseClickButtonCenter(ButtonBase targetElement, ClickMode clickMode, MouseButton mouseButton)
        {
            InputHelper.MouseMoveToElementCenter(targetElement);
            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);

            switch (clickMode)
            {
                case ClickMode.Release:
                    Microsoft.Test.Input.Mouse.Click(ConvertWpfMouseButtonToTestMouseButton(mouseButton));
                    break;
                case ClickMode.Press:
                    Microsoft.Test.Input.Mouse.Down(ConvertWpfMouseButtonToTestMouseButton(mouseButton));
                    break;
                case ClickMode.Hover:
                    throw new TestValidationException("Fail: ClickMode.Hover is not supported in the MouseClickCenter method");
            }
            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);

            // Cleanup
            if (clickMode == ClickMode.Press)
            {
                Microsoft.Test.Input.Mouse.Up(ConvertWpfMouseButtonToTestMouseButton(mouseButton));
                DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);
            }
        }

        /// <summary>
        /// Mouse click on center of FrameworkElement
        /// </summary>
        /// <param name="targetElement">A FrameworkElement reference</param>
        public static void MouseClickCenter(FrameworkElement targetElement, MouseButton mouseButton)
        {
            InputHelper.MouseMoveToElementCenter(targetElement);
            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);
            Microsoft.Test.Input.Mouse.Click(ConvertWpfMouseButtonToTestMouseButton(mouseButton));
            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);
        }

        /// <summary>
        /// Click on the middle of window's chrome to make sure the window has focus because vista window doesn't always actived
        /// </summary>
        /// <param name="window">A window reference</param>
        public static void MouseClickWindowChrome(Window window)
        {
            window.Topmost = true;
            Point point = window.PointToScreen(new Point());
            Point logicalPoint = new Point(point.X + window.ActualWidth / 4, point.Y - 8);
            System.Drawing.Point clickPoint = new System.Drawing.Point(DpiHelper.ConvertToPhysicalPixel(logicalPoint.X), DpiHelper.ConvertToPhysicalPixel(logicalPoint.Y));

            Microsoft.Test.Input.Mouse.MoveTo(clickPoint);
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
            Microsoft.Test.Input.Mouse.Click(Microsoft.Test.Input.MouseButton.Left);
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
        }

        public static Microsoft.Test.Input.Key ConvertWpfKeyToTestKey(System.Windows.Input.Key key)
        {
            foreach (var testKey in Enum.GetValues(typeof(Microsoft.Test.Input.Key)))
            {
                if (testKey.ToString().ToLower() == key.ToString().ToLower())
                {
                    return (Microsoft.Test.Input.Key)testKey;
                }
            }

            return Microsoft.Test.Input.Key.None;
        }

        public static Microsoft.Test.Input.MouseButton ConvertWpfMouseButtonToTestMouseButton(System.Windows.Input.MouseButton mouseButton)
        {
            foreach (var testMouseButton in Enum.GetValues(typeof(Microsoft.Test.Input.MouseButton)))
            {
                if (testMouseButton.ToString().ToLower() == mouseButton.ToString().ToLower())
                {
                    return (Microsoft.Test.Input.MouseButton)testMouseButton;
                }
            }

            return Microsoft.Test.Input.MouseButton.Left;
        }
    }
}
