// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//---------------------------------------------------------------------
//
//  Description: Test base class that provides implementors with apis for
//		automating visual tests (e.g. automated input and screenshot testing).
//	
//  Creator: Derek Mehlhorn (derekme)
//  Date Created: 13/23/05
//---------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Automation;
using System.Windows.Input;
using MTI = Microsoft.Test.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Security;
using System.Security.Permissions;
using Microsoft.Test.Threading;
using Microsoft.Test.Logging;
using Microsoft.Test.Display;

namespace Annotations.Test.Framework
{
    /// <summary>
    /// Module that performs simulated mouse input etc, while buffering us from the implementation
    /// details.
    /// 
    /// NOTE: UIAutomation is NOT currently used in this class.  Recent changes to UIAutomation have made
    /// it impossible invoke properties on AutomationElements from inside the MainUI thread.  Therefore, if
    /// we ever want to use UIA, we will have to make our tests multi-threaded.
    /// </summary>
    public class UIAutomationModule
    {
        #region Helpers

        /// <summary>
        /// Returns true if Point is within the Application.MainWindow.
        /// </summary>
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        public static bool PointIsInsideWindow(Point point)
        {
            Window window = Application.Current.MainWindow;
            return (point.X > window.Left &&
                    point.X < (window.Left + window.ActualWidth) &&
                    point.Y > window.Top &&
                    point.Y < (window.Top + window.ActualHeight));
        }

        /// <summary>
        /// Find boudning box of given element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        public static Rect BoundingRectangle(UIElement element)
        {
            Rect bounds = Rect.Empty;
            //
            // If element is actually a FrameworkElement then we can use VisualTransformations to determine
            // its bounds with a much higher precision then the ImageUtility.
            //
            if (element is FrameworkElement)
            {
                try
                {
                    /*
                    FrameworkElement frameworkElement = (FrameworkElement)element;
                    Point relativeToScreen = LocationOnScreen(frameworkElement);
                    bounds = new Rect(relativeToScreen.X, relativeToScreen.Y, frameworkElement.ActualWidth, frameworkElement.ActualHeight);
                     */
                    FrameworkElement frameworkElement = (FrameworkElement)element;
                    Point relativeToScreen = LocationOnScreen(frameworkElement);
                    bounds = new Rect(
                        relativeToScreen.X, 
                        relativeToScreen.Y, 
                        Monitor.ConvertLogicalToScreen(Dimension.Width, frameworkElement.ActualWidth),
                        Monitor.ConvertLogicalToScreen(Dimension.Height, frameworkElement.ActualHeight)
                        );
                    
                }
                catch (Exception)
                {
                    // If this fails just fall through and try to get the bounds using ImageUtility.
                    // Will fail if we are running in PT becuase we cannot get access to the window position.
                }
            }

            if (bounds.Equals(Rect.Empty))
            {
                // Note: ImageUtility returns bounds that are rounded to the nearest Int.
                System.Drawing.Rectangle rectangle = Microsoft.Test.RenderingVerification.ImageUtility.GetScreenBoundingRectangle(element);
                bounds = new Rect(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
            }

            if (bounds.Equals(Rect.Empty))
                throw new Exception("Failed to compute bounding rectangle for element '" + element + "'.");
            return bounds;
        }

        /// <summary>
        /// Find center of Visual.
        /// </summary>
        /// <param name="visual"></param>
        /// <returns></returns>
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        public static Point LocationOnScreen(Visual visual)
        {
            Application currApp = Application.Current;
            Window appWindow = currApp != null ? currApp.MainWindow : null;
            if (appWindow == null || !appWindow.IsAncestorOf(visual))
                appWindow = FindParentWindow(visual);
            Point relativeToWindow = new Point(0, 0);
            GeneralTransform targetToWindowTransform = visual.TransformToVisual(appWindow);
            if (!targetToWindowTransform.TryTransform(new Point(0, 0), out relativeToWindow))
                throw new Exception("Failed to transform point.");
            //
            // Add 4 to X and 24 to Y because _window.Left/Top return Content position, so we
            // adjust for the window chrome.
            //
            // Adding DPI scaling.  Will multiply actual points and the buffer noted above 
            // by the current DPI/96..
            relativeToWindow = new Point(Monitor.ConvertLogicalToScreen(Dimension.Width, relativeToWindow.X), Monitor.ConvertLogicalToScreen(Dimension.Height, relativeToWindow.Y));

            Point relativeToScreen = new Point(relativeToWindow.X + appWindow.Left + Monitor.ConvertLogicalToScreen(Dimension.Width, 4.0), relativeToWindow.Y + appWindow.Top + Monitor.ConvertLogicalToScreen(Dimension.Height, 24.0));
            return relativeToScreen;
        }

        /// <summary>
        /// Find window that is hosting visual.
        /// </summary>
        /// <param name="visual"></param>
        /// <returns></returns>
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        public static Window FindParentWindow(Visual visual)
        {
            DependencyObject parent = visual;
            do
            {
                parent = VisualTreeHelper.GetParent(parent);
            } while (!(parent is Window) && parent != null);

            if (parent == null)
                throw new Exception("No parent window for for visual.");
            return parent as Window;
        }

        /// <summary>
        /// Return the coordinates of the center of given UIElement.
        /// </summary>
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        public static Point GetElementCenter(UIElement element)
        {
            Rect rc = BoundingRectangle(element);
            return new Point(rc.Left + (rc.Width / 2), rc.Top + (rc.Height / 2));
        }

        #endregion

        #region Mouse Operations

        /// <summary>
        /// Move mouse to the ui element with the given id and click.
        /// Clicks in the 0,0 point of the UIElement
        /// </summary>
        public static void MoveToAndClickElement(UIElement element)
        {
            MTI.Input.MoveToAndClick(element);
            DispatcherHelper.DoEvents();
        }

        /// <summary>
        /// Moves mouse to the centre of the UIElement's bounding box, then clicks 
        /// left mouse button at this centre point.
        /// </summary>
        /// <param name="element">Control to click</param>
        public static void MoveToCenterAndClickElement(UIElement element)
        {
            MoveToCenter(element);
            MTI.Input.SendMouseInput(0, 0, 0, MTI.SendMouseInputFlags.LeftDown);
            MTI.Input.SendMouseInput(0, 0, 0, MTI.SendMouseInputFlags.LeftUp);
            DispatcherHelper.DoEvents();
        }

        /// <summary>
        /// Move mouse to the center of this UIElement.
        /// </summary>
        public static void MoveToCenter(UIElement element)
        {
            Point center = GetElementCenter(element);
            MoveTo(center);
            DispatcherHelper.DoEvents();
        }

        /// <summary>
        /// Move mouse to point.
        /// </summary>
        /// <param name="dest"></param>
        public static void MoveTo(Point dest)
        {
            MTI.Input.MoveTo(dest);
            DispatcherHelper.DoEvents();
        }

        /// <summary>
        /// Move mouse by offset.
        /// </summary>
        /// <param name="delta"></param>
        public static void Move(Vector delta)
        {
            MTI.Input.SendMouseInput(delta.X, delta.Y, 0, MTI.SendMouseInputFlags.Move);
            DispatcherHelper.DoEvents();
        }

        /// <summary>
        /// Move mouse to point and click.
        /// </summary>
        /// <param name="dest"></param>
        public static void MoveToAndClick(Point dest)
        {
            MTI.Input.MoveToAndClick(dest);
            DispatcherHelper.DoEvents();
        }

        /// <summary>
        /// Send mouse down event.
        /// </summary>
        public static void LeftMouseDown()
        {
            MTI.Input.SendMouseInput(0, 0, 0, MTI.SendMouseInputFlags.LeftDown);
            DispatcherHelper.DoEvents();
        }

        /// <summary>
        /// Send mouse up event.
        /// </summary>
        public static void LeftMouseUp()
        {
            MTI.Input.SendMouseInput(0, 0, 0, MTI.SendMouseInputFlags.LeftUp);
            DispatcherHelper.DoEvents();
        }

        /// <summary>
        /// Send left mouse click event.
        /// </summary>
        public static void LeftMouseClick()
        {
            MTI.Input.SendMouseInput(0, 0, 0, MTI.SendMouseInputFlags.LeftDown | MTI.SendMouseInputFlags.LeftUp);
            DispatcherHelper.DoEvents();
        }

        /// <summary>
        /// Send right mouse click event.
        /// </summary>
        public static void RightMouseClick()
        {
            MTI.Input.SendMouseInput(0, 0, 0, MTI.SendMouseInputFlags.RightDown | MTI.SendMouseInputFlags.RightUp);
            DispatcherHelper.DoEvents();
        }

        /// <summary>
        /// Send right mouse down event.
        /// </summary>
        public static void RightMouseDown()
        {
            MTI.Input.SendMouseInput(0, 0, 0, MTI.SendMouseInputFlags.RightDown);
            DispatcherHelper.DoEvents();
        }

        /// <summary>
        /// Send right mouse up event.
        /// </summary>
        public static void RightMouseUp()
        {
            MTI.Input.SendMouseInput(0, 0, 0, MTI.SendMouseInputFlags.RightUp);
            DispatcherHelper.DoEvents();
        }

        #endregion

        #region Keyboard Operations

        /// <summary>
        /// Gives keyboard focus to the specified element, then presses the specified key once.
        /// Listeners should be set on the element, if you want events handled.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="keyToPress"></param>
        public static void PressKey(UIElement element, Key keyToPress)
        {
            element.Focus();
            PressKey(keyToPress);
        }

        /// <summary>
        /// Press key.
        /// </summary>
        /// <param name="keyToPress"></param>
        public static void PressKey(Key keyToPress)
        {
            // Click on key (press, then release) once
            MTI.Input.SendKeyboardInput(keyToPress, true);
            MTI.Input.SendKeyboardInput(keyToPress, false);
            DispatcherHelper.DoEvents();
        }

        /// <summary>
        /// Perform a Alt+'Key' operation.
        /// </summary>
        public static void Alt(Key key)
        {
            MTI.Input.SendKeyboardInput(Key.LeftAlt, true);
            PressKey(key);
            MTI.Input.SendKeyboardInput(Key.LeftAlt, false);
            DispatcherHelper.DoEvents();
        }

        /// <summary>
        /// Perform a Ctrl+'Key' operation.
        /// </summary>
        public static void Ctrl(Key key)
        {
            MTI.Input.SendKeyboardInput(Key.LeftCtrl, true);
            PressKey(key);
            MTI.Input.SendKeyboardInput(Key.LeftCtrl, false);
            DispatcherHelper.DoEvents();
        }

        /// <summary>
        /// Perform a Ctrl+Shift+'Key' operation.
        /// </summary>
        public static void CtrlShift(Key key)
        {
            MTI.Input.SendKeyboardInput(Key.LeftCtrl, true);
            MTI.Input.SendKeyboardInput(Key.LeftShift, true);
            PressKey(key);
            MTI.Input.SendKeyboardInput(Key.LeftShift, false);
            MTI.Input.SendKeyboardInput(Key.LeftCtrl, false);
            DispatcherHelper.DoEvents();
        }

        /// <summary>
        /// Send given string using keyboard input events.
        /// </summary>
        /// <param name="msg"></param>
        public static void TypeString(string msg)
        {
            char[] characters = msg.ToCharArray();
            foreach (char character in characters)
            {
                Key key = MapCharToKey(character);
                if (char.IsUpper(character)) MTI.Input.SendKeyboardInput(Key.LeftShift, true);
                MTI.Input.SendKeyboardInput(key, true);
                MTI.Input.SendKeyboardInput(key, false);
                if (char.IsUpper(character)) MTI.Input.SendKeyboardInput(Key.LeftShift, false);
            }
            DispatcherHelper.DoEvents();
        }

        #endregion

        #region Private Methods

        private static Key MapCharToKey(char character)
        {
            string toParse = character.ToString();
            if (char.IsDigit(character))
                toParse = "D" + toParse;
            Key result;
            switch (toParse)
            {
                case ".":
                    result = Key.OemPeriod;
                    break;
                case "\n":
                    result = Key.Enter;
                    break;
                case " ":
                    result = Key.Space;
                    break;
                case "-":
                    result = Key.Subtract;
                    break;
                case ",":
                    result = Key.Separator;
                    break;
                default:
                    result = (Key)Enum.Parse(typeof(Key), toParse, true);
                    break;
            }
            return result;
        }

        #endregion
    }
}
