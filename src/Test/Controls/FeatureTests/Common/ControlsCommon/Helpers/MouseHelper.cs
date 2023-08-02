using System;
using System.Windows;
using System.Windows.Threading;

namespace Microsoft.Test.Controls
{
    public static class MouseHelper
    {
        /// <summary>
        /// Move mouse to element top left location
        /// </summary>
        /// <param name="element">element</param>
        public static void MoveToElementLocation(FrameworkElement element)
        {
            MoveToElementLocation(element, 0, 0);
        }

        /// <summary>
        /// Move mouse to element center location
        /// </summary>
        /// <param name="element">element</param>
        public static void MoveToElementCenter(FrameworkElement element)
        {
            Point elementCenterOffset = new Point(element.ActualWidth / 2, element.ActualHeight / 2);
            System.Drawing.Point mouseLocation = GetMoveToLocation(element, elementCenterOffset);
            Microsoft.Test.Input.Mouse.MoveTo(mouseLocation);
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
        }

        /// <summary>
        /// Move mouse to element top left plus x and y offset location
        /// </summary>
        /// <param name="element">element</param>
        /// <param name="xOffset">x offset</param>
        /// <param name="yOffset">y offset</param>
        public static void MoveToElementLocation(FrameworkElement element, double xOffset, double yOffset)
        {
            int convertedXOffset = DpiHelper.ConvertToPhysicalPixel(xOffset);
            int convertedYOffset = DpiHelper.ConvertToPhysicalPixel(yOffset);

            Point elementLocation = new Point(element.PointToScreen(new Point()).X + convertedXOffset, element.PointToScreen(new Point()).Y + convertedYOffset);
            Microsoft.Test.Input.Mouse.MoveTo(new System.Drawing.Point(Convert.ToInt32(elementLocation.X), Convert.ToInt32(elementLocation.Y)));
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
        }

        /// <summary>
        /// Mouse click on center of element
        /// </summary>
        /// <param name="element">element</param>
        /// <param name="mouseButton">MouseButton</param>
        public static void ClickElementCenter(FrameworkElement element, Microsoft.Test.Input.MouseButton mouseButton)
        {
            MoveToElementCenter(element);
            Microsoft.Test.Input.Mouse.Click(mouseButton);
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
        }

        /// <summary>
        /// Get mouse move to location
        /// </summary>
        /// <param name="element">element</param>
        /// <param name="logicalOffset">wpf logical pixel offset</param>
        /// <returns>screen physical pixel location</returns>
        public static System.Drawing.Point GetMoveToLocation(FrameworkElement element, Point logicalOffset)
        {
            Point mouseLocation = default(Point);
            FlowDirection flowDirection = Window.GetWindow(element).FlowDirection;

            // We don't need to convert element location to physical screen pixel because wpf takes care of it.
            Point elementLocation = element.PointToScreen(new Point());

            // We need to convert offset to physical screen pixel since we're pass in wpf logical pixel
            double physicalXOffset = DpiHelper.ConvertToPhysicalPixel(logicalOffset.X);
            double physicalYOffset = DpiHelper.ConvertToPhysicalPixel(logicalOffset.Y);

            switch (flowDirection)
            {
                case FlowDirection.LeftToRight:
                    mouseLocation = new Point(elementLocation.X + physicalXOffset, elementLocation.Y + physicalYOffset);
                    break;
                case FlowDirection.RightToLeft:
                    // We need to subtract physical offsetX because the element location starting point is in right most
                    mouseLocation = new Point(elementLocation.X - physicalXOffset, elementLocation.Y + physicalYOffset);
                    break;
            }

            return new System.Drawing.Point(Convert.ToInt32(mouseLocation.X), Convert.ToInt32(mouseLocation.Y));
        }
    }
}
