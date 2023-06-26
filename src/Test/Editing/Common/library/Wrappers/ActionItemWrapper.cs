// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Wrap the commonly used functions.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Common/Library/Wrappers/ActionItemWrapper.cs $")]

namespace Test.Uis.Wrappers
{
    #region Namespaces.

    using System;
    using System.Collections;    
    using System.IO;
    using System.Collections.Generic;
    using System.Globalization;
    using Bitmap = System.Drawing.Bitmap;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Markup;

    using Microsoft.Test.Imaging;
    using Test.Uis.IO;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;

    #endregion Namespaces.

    /// <summary>
    /// Enum for mouse click positioning in the element
    /// Delta from the element boundary will be specified in
    /// the method ClickOnElement local variable delta
    /// </summary>
    public enum ElementPosition
    {
        /// <summary>
        /// upperleft
        /// </summary>
        UpperLeft,

        /// <summary>
        /// middleleft
        /// </summary>
        MiddleLeft,

        /// <summary>
        /// lowerleft
        /// </summary>
        LowerLeft,

        /// <summary>
        /// uppermiddle
        /// </summary>
        UpperMiddle,

        /// <summary>
        /// center
        /// </summary>
        Center,

        /// <summary>
        /// lowermiddle
        /// </summary>
        LowerMiddle,

        /// <summary>
        /// upperright
        /// </summary>
        UpperRight,

        /// <summary>
        /// middleright
        /// </summary>
        MiddleRight,

        /// <summary>
        /// lowerright
        /// </summary>
        LowerRight
    }

    /// <summary>
    /// ActionItemWrapper
    /// Warning: All methods in this class *have* to be static
    /// </summary>
    public static class ActionItemWrapper
    {
        #region Test case argument handling.

        /// <summary>
        /// Wrapper for the ConfigurationSettings.GetArgument method.
        /// </summary>
        /// <param name="key">the key to retrieve argument</param>
        /// <returns>Value of key</returns>
        public static string GetArgument(string key)
        {
            if (key == null || key.Length == 0)
            {
                throw new ArgumentException("Blank argument names are not allowed.", "key");
            }
            return ConfigurationSettings.Current.GetArgument(key);
        }

        /// <summary>
        /// Wrapper for the ConfigurationSettings.GetArgument method.
        /// </summary>
        /// <param name="key">the key to retrieve argument</param>
        /// <returns>Value of key</returns>
        public static bool GetArgumentAsBool(string key)
        {
            if (key == null || key.Length == 0)
            {
                throw new ArgumentException("Blank argument names are not allowed.", "key");
            }
            return ConfigurationSettings.Current.GetArgumentAsBool(key);
        }

        #endregion Test case argument handling.

        #region Image manipulation methods.

        /// <summary>
        /// Captures a bitmap for a given line of text in the element
        /// referred to by the specified UIElementWrapper.
        /// </summary>
        /// <param name="wrapper">Wrapper for the element with text.</param>
        /// <param name="lineIndex">Index of line to capture.</param>
        /// <returns>A bitmap with the specified line.</returns>
        public static Bitmap CaptureElementLine(UIElementWrapper wrapper, string lineIndex)
        {
            if (wrapper == null)
            {
                throw new ArgumentNullException("wrapper");
            }
            if (lineIndex == null)
            {
                throw new ArgumentNullException("lineIndex");
            }

            int intLineIndex = Int32.Parse(lineIndex, CultureInfo.InvariantCulture);

            using(Bitmap control = BitmapCapture.CreateBitmapFromElement(wrapper.Element))
            {
                Rect lineRect = wrapper.GetControlRelativeLineBounds(intLineIndex);
                return BitmapUtils.CreateSubBitmap(control, lineRect);
            }
        }

        #endregion Image manipulation methods.

        #region Creation methods.


        /// <summary>
        /// Sometimes we need a plain simple textbox. The textbox
        /// is returned as UIElementWrapper object.
        /// </summary>
        /// <param name="id">id assigned to the created control</param>
        /// <returns>UIElement as object associated with the created textbox</returns>
        public static UIElementWrapper CreateEmptyDefaultTextBoxAsUIElement(string id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }
            if (id.Length == 0)
            {
                throw new ArgumentException("id cannot be blank", "id");
            }

            Window window = GetDefaultMainWindow();
            IAddChild content = window.Content as IAddChild;
            if (content == null)
            {
                //
                // For backwards compatibility, we allow action-based
                // test cases to create a TextBox in a Window with
                // unassigned content. In this case, we inject
                // a DockPanel on-demand.
                //
                if (window.Content == null)
                {
                    window.Content = new DockPanel();
                    content = (IAddChild) window.Content;
                }
                else
                {
                    throw new InvalidOperationException(
                        "Window content [" + window.Content + "] does not support IAddChild.");
                }
            }

            TextBox textbox = new TextBox();
            textbox.Name = id;
            content.AddChild(textbox);
            //textbox.Height = new Length(100, UnitType.Percent);
            //textbox.Width = new Length(100, UnitType.Percent);
            return new UIElementWrapper (textbox);
        }

        /// <summary>Creates a UIElementWrapper for the element with the given id.</summary>
        /// <param name='id'>ID of element to wrap.</param>
        /// <returns>A new UIElementWrapper instance for the specified element.</returns>
        public static UIElementWrapper CreateTextWrapperForElement(string id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }

            UIElement element = ElementUtils.FindElement(GetDefaultMainWindow(), id);
            if (element == null)
            {
                throw new InvalidOperationException("Unable to find element with id " + id);
            }

            return new UIElementWrapper(element);
        }

        #endregion Creation methods.

        #region Logging methods.

        /// <summary>
        /// Add this action in xml to debug xml failures.
        /// </summary>
        public static void DumpReturnValueList ()
        {
            ActionManager.Current.DumpReturnValueList ();
        }

        /// <summary>
        /// Logs the visual tree of the main window.
        /// </summary>
        public static void DumpVisualTree()
        {
            Visual root = GetDefaultMainWindow();
            Logger.Current.Log(VisualLogger.DescribeVisualTree(root));
        }

        /// <summary>Logs the specified text.</summary>
        /// <param name='text'>Text to log.</param>
        public static void Log(string text)
        {
            Logger.Current.Log(text);
        }

        /// <summary>
        /// Logs the specified image.
        /// </summary>
        /// <param name="image">Image to log.</param>
        /// <param name="name">Name to identify the image.</param>
        public static void LogImage(System.Drawing.Image image, string name)
        {
            Logger.Current.LogImage(image, name);
        }

        #endregion Logging methods.

        #region Element retrieval methods.

        /// <summary>
        /// Find element given the id.
        /// </summary>
        /// <param name="elementID">ID of the element to find.</param>
        /// <returns>Element found.</returns>
        public static UIElement FindElement(string elementID)
        {
            if (String.IsNullOrEmpty(elementID))
            {
                throw new ArgumentException("Argument cannot be empty", "elementID");
            }
            return ElementUtils.FindElement(GetDefaultMainWindow(), elementID);
        }

        /// <summary>
        /// Retrieve main window for the application
        /// </summary>
        /// <returns>Window as object</returns>
        public static Window GetDefaultMainWindow()
        {
            if (Application.Current != null)
            {
                if (Application.Current.Windows != null && Application.Current.Windows.Count > 0)
                {
                    return Application.Current.Windows[0];
                }
            }
            return null;
        }

        /// <summary>
        /// Finds a Visual given the XPath supplied on the main window.
        /// </summary>
        /// <param name='xpath'>XPath to visual.</param>
        /// <returns>Visual object found.</returns>
        /// <remarks>
        /// If multiple visuals or no visuals are found, an exception is thrown.
        /// </remarks>
        public static Visual GetVisualFromPath(string xpath)
        {
            if (xpath == null)
            {
                throw new ArgumentNullException("xpath");
            }

            Visual[] result =
                XPathNavigatorUtils.ListVisuals(GetDefaultMainWindow(), xpath);
            if (result.Length == 0)
            {
                throw new Exception("No Visuals found with path: " + xpath);
            }
            if (result.Length > 1)
            {
                throw new Exception("Multiple Visuals found with path: " + xpath);
            }
            return result[0];
        }

        #endregion Element retrieval methods.

        #region Layout and geometric manipulation methods.

        /// <summary>
        /// wrapper on UIElementWrapper.GetGlobalCharacterRect
        /// </summary>
        /// <param name="elementID">element id</param>
        /// <param name="indexStr"></param>
        /// <returns></returns>
        public static Rect GetGlobalCharacterRect(string elementID, string indexStr)
        {
            if (elementID != null)
            {
                FrameworkElement element = FindElement(elementID) as FrameworkElement;

                if (element != null)
                {
                    int index;

                    try
                    {
                        index = Int32.Parse(indexStr);
                    }
                    catch
                    {
                        Logger.Current.Log("WARNING: indexStr can't be converted, setting it to default");
                        index = 0;
                    }
                    return (new UIElementWrapper(element as UIElement)).GetGlobalCharacterRect(index);
                }
            }

            return Rect.Empty;
        }

        /// <summary>
        /// Retrieves the origin point relative to the screen for the
        /// bounding box of the specified element.
        /// </summary>
        /// <param name='element'>Element to get origin for.</param>
        /// <returns>The origin of the rectangle that bounds the element.</returns>
        public static Point GetScreenRelativeOrigin(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            Rect rect = ElementUtils.GetScreenRelativeRect(element);
            return rect.Location;
        }

        /// <summary>
        /// offset point with X and Y value. The origianl value of point is not changed
        /// and the new value will be returned.
        /// </summary>
        /// <param name="point">Point value to be passed from other method</param>
        /// <param name="offsetXStr">X value in string to offset point</param>
        /// <param name="offsetYStr">Y value in string to offset point</param>
        /// <returns>the point value after it is offset</returns>
        public static Point OffsetPoint(Point point, string offsetXStr, string offsetYStr)
        {
            Point retPoint = new Point(point.X, point.Y);
            if (!String.IsNullOrEmpty(offsetXStr) && !String.IsNullOrEmpty(offsetYStr))
            {
                double offsetX;
                double offsetY;
                try
                {
                    offsetX = Double.Parse(offsetXStr);
                }
                catch
                {
                    Logger.Current.Log("WARNING: Cannot convert offsetXStr [{0}] to doudle, setting default", offsetXStr);
                    offsetX = 0.0f;
                }
                try
                {
                    offsetY = Double.Parse(offsetYStr);
                }
                catch
                {
                    Logger.Current.Log("WARNING: Cannot convert offsetYStr [{0}] to doudle, setting default", offsetYStr);
                    offsetY = 0.0f;
                }

                retPoint.Offset(offsetX, offsetY);

            }
            return retPoint;
        }

        #endregion Layout and geometric manipulation methods.

        #region Property manipulation.

        /// <summary>
        /// Gets the value of the dependency property on the specified target.
        /// </summary>
        /// <param name='target'>Object from which to get the value.</param>
        /// <param name='propertyName'>Name of dependency property to set.</param>
        /// <returns>The property value.</returns>
        /// <example>The following sample shows how to use this method.<code>...
        ///   HorizontalAlignment alignment = (HorizontalAlignment)
        ///       GetDependencyProperty(textbox, "Text.HorizontalAlignment");
        ///   Console.WriteLine("Alignment: " + alignment);
        /// </code></example>
        public static object GetDependencyProperty(object target,
            string propertyName)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            DependencyObject dependencyTarget = target as DependencyObject;
            if (dependencyTarget == null)
            {
                throw new ArgumentException(
                    "target [" + target + "] must be a DependencyObject",
                    "target");
            }

            DependencyProperty property = GetDependencyPropertyFromName(propertyName);
            System.Diagnostics.Debug.Assert(property != null);

            return dependencyTarget.GetValue(property);
        }

        /// <summary>
        /// This is a helper method for GetDependencyProperty and
        /// SetDependencyProperty to get a DependencyProperty
        /// given a name.
        /// </summary>
        /// <param name='propertyName'>Name of dependency property to get.</param>
        /// <returns>The specified DependencyProperty.</returns>
        /// <example>The following sample shows how to use this method.<code>...
        ///   DependencyProperty property = GetDependencyPropertyFromName(
        ///       "Text.HorizontalAlignment");
        ///   System.Diagnostics.Debug.Assert(property != null);
        ///   Console.WriteLine("Alignemt: " + textbox.GetValue(property));
        /// </code></example>
        public static DependencyProperty GetDependencyPropertyFromName(
            string propertyName)
        {
            if (propertyName == null || propertyName.Length == 0)
            {
                throw new ArgumentException("propertyName cannot be empty", "propertyName");
            }

            int dotPos = propertyName.LastIndexOf('.');
            if (dotPos == -1)
            {
                throw new ArgumentException(
                    "propertyName [" + propertyName + "] must be in type.property format",
                    "propertyName");
            }

            string typeName = propertyName.Substring(0, dotPos);
            string fieldName = propertyName.Substring(dotPos + 1) + "Property";
            DependencyProperty property = (DependencyProperty)
                ReflectionUtils.InvokePropertyOrMethod(typeName, fieldName,
                new object[] { }, InvokeType.GetStaticField);
            if (property == null)
            {
                throw new InvalidOperationException(
                    "Unable to find property for " + propertyName);
            }

            return property;
        }

        /// <summary>
        /// Sets the value of the dependency property on the specified target.
        /// </summary>
        /// <param name='target'>Object on which to set value.</param>
        /// <param name='propertyName'>Name of dependency property to set.</param>
        /// <param name='value'>Value of property.</param>
        /// <example>The following sample shows how to use this method.<code>...
        ///   SetDependencyProperty(textbox, "Text.HorizontalAlignment", "Right");
        /// </code></example>
        public static void SetDependencyProperty(object target,
            string propertyName, object value)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            DependencyObject dependencyTarget = target as DependencyObject;
            if (dependencyTarget == null)
            {
                throw new ArgumentException(
                    "target [" + target + "] must be a DependencyObject",
                    "target");
            }

            DependencyProperty property = GetDependencyPropertyFromName(propertyName);
            System.Diagnostics.Debug.Assert(property != null);

            Type propertyType = property.PropertyType;
            value = ReflectionUtils.GetValueForComparison(value, propertyType);
            dependencyTarget.SetValue(property, value);
        }

        /// <summary>
        /// Sets the value of the named property.
        /// </summary>
        /// <param name='target'>Object on which to set value.</param>
        /// <param name='propertyName'>Name of property to set.</param>
        /// <param name='value'>Value of property.</param>
        /// <remarks>
        /// You can use a string equal to "*null" to specify a null value.
        /// </remarks>
        public static void SetProperty(object target, string propertyName, object value)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            if (propertyName == null || propertyName.Length == 0)
            {
                throw new ArgumentException("propertyName cannot be empty", "propertyName");
            }

            Type propertyType = ReflectionUtils.GetPropertyType(
                target.GetType(), propertyName);
            value = ReflectionUtils.GetValueForComparison(value, propertyType);
            ReflectionUtils.SetProperty(target, propertyName, value);
        }

        #endregion Property manipulation.

        #region Input methods.

        /// <summary>Clicks on every element, recursively.</summary>
        /// <param name='element'>Element to start clicking on.</param>
        public static void ClickEveryElementRecursive(Visual element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            ClickEveryElementRecursive(element, new List<Visual>());
        }

        /// <summary>Clicks on every element of the main window.</summary>
        public static void ClickEveryMainWindowElement()
        {
            Window window;

            window = GetDefaultMainWindow();
            System.Diagnostics.Debug.Assert(window != null);
            ClickEveryElementRecursive(window);
        }

        /// <summary>
        /// Click on element given the id of the control
        /// </summary>
        /// <param name="elementID">id of the control</param>
        public static void ClickOnElement(string elementID)
        {
            if (elementID == null)
            {
                throw new ArgumentNullException("elementID");
            }

            UIElement element = ElementUtils.FindElement(GetDefaultMainWindow(), elementID);
            if (element == null)
            {
                throw new InvalidOperationException(
                    "Cannot find element with ID [" + elementID + "]");
            }

            MouseInput.MouseClick(element);
        }

        /// <summary>
        /// Click on element given the id of the control
        /// </summary>
        /// <param name="elementID">id of the control</param>
        /// <param name="relativePositionInElement">see public enum ElementPosition</param>
        public static void ClickOnElement(string elementID, string relativePositionInElement)
        {
            if (elementID == null)
            {
                throw new ArgumentNullException("elementID");
            }

            int delta = 8;

            UIElement element = ElementUtils.FindElement(GetDefaultMainWindow(), elementID);
            if (element == null)
            {
                throw new InvalidOperationException(
                    "Cannot find element with ID [" + elementID + "]");
            }

            ElementPosition position = (ElementPosition)
                Enum.Parse(typeof(ElementPosition), relativePositionInElement, true);
            int x = 0;
            int y = 0;

            Rect r;
            float xFactor;
            float yFactor;

            r = ElementUtils.GetScreenRelativeRect(element);
            UIElementWrapper.HighDpiScaleFactors(out xFactor, out yFactor);
            delta = (int)Math.Round(delta * xFactor);
            switch (position)
            {
                case ElementPosition.UpperLeft:
                    y = (int)(r.Top) + delta;
                    x = (int)(r.Left) + delta;
                    break;

                case ElementPosition.MiddleLeft:
                    x = (int)(r.Left) + delta;
                    y = (int)(r.Top + r.Height / 2);
                    break;

                case ElementPosition.LowerLeft:
                    x = (int)(r.Left) + delta;
                    y = (int)(r.Bottom) - delta;
                    break;
                case ElementPosition.UpperMiddle:
                    x = (int)(r.Left + r.Width / 2);
                    y = (int)(r.Top) + delta;
                    break;
                case ElementPosition.Center:
                    x = (int)(r.Left + r.Width / 2);
                    y = (int)(r.Top + r.Height / 2);
                    break;
                case ElementPosition.LowerMiddle:
                    x = (int)(r.Left + r.Width / 2);
                    y = (int)(r.Bottom) - delta;
                    break;
                case ElementPosition.UpperRight:
                    x = (int)(r.Right) - delta;
                    y = (int)(r.Top) + delta;
                    break;
                case ElementPosition.MiddleRight:
                    x = (int)(r.Right) - delta;
                    y = (int)(r.Top + r.Height / 2);
                    break;
                case ElementPosition.LowerRight:
                    x = (int)(r.Right) - delta;
                    y = (int)(r.Bottom) - delta;
                    break;
            }
            MouseInput.MouseClick(x, y);
        }

        /// <summary>
        /// Click on the last character in the element identified by elementID
        /// </summary>
        /// <param name="elementID">id of the control</param>
        public static void ClickToPositionCaretAfterLastChar(string elementID)
        {
            if (elementID == null)
            {
                throw new ArgumentNullException("elementID");
            }

            UIElement element = ElementUtils.FindElement(GetDefaultMainWindow(), elementID);
            if (element == null)
            {
                throw new InvalidOperationException(
                    "Cannot find element with ID [" + elementID + "]");
            }

            UIElementWrapper elementWrapper = new UIElementWrapper(element);

            Rect rect = elementWrapper.GetGlobalCharacterRectOfLastCharacter();

            int x = (int)MathUtils.GetLargestPossibleIntegerValueWithinTheRect(rect);

            int y = (int)(rect.Top + (rect.Height / 2));

            MouseInput.MouseClick(x, y);

        }

        /// <summary>
        /// Click before the character specified by the offset
        /// </summary>
        /// <param name="elementID">id of the control</param>
        /// <param name="offsetStr">Offset of the character to have the caret positioned</param>
        public static void ClickToPositionCaretBeforeCharacter(string elementID, string offsetStr)
        {
            if (elementID == null)
            {
                throw new ArgumentNullException("elementID");
            }

            if (offsetStr == null)
            {
                throw new ArgumentNullException("offsetStr");
            }

            UIElement element = ElementUtils.FindElement(GetDefaultMainWindow(), elementID);
            if (element == null)
            {
                throw new InvalidOperationException(
                    "Cannot find element with ID [" + elementID + "]");
            }

            UIElementWrapper elementWrapper = new UIElementWrapper(element);

            int offset;

            offset = Int32.Parse(offsetStr);

            Rect rect = elementWrapper.GetGlobalCharacterRect(offset);

            int x = (int)MathUtils.GetLargestPossibleIntegerValueWithinTheRect(rect);

            int y = (int)(rect.Top + (rect.Height / 2));

            MouseInput.MouseClick(x, y);
        }

        /// <summary>
        /// This is to click on the element with coordinates specified
        /// WARNING: If x / y is not specified correctly it will default to 0
        /// and a log message is created.
        /// </summary>
        /// <param name="elementID">ID of the element</param>
        /// <param name="strX">coordinate x relative to the element in string</param>
        /// <param name="strY">coordinate y relative to the element in string</param>
        public static void ClickOnElementWithCoordinates(string elementID, string strX, string strY)
        {
            if (!String.IsNullOrEmpty (elementID))
            {
                FrameworkElement element = FindElement(elementID) as FrameworkElement;

                if (element != null)
                {
                    int pointX;
                    int pointY;
                    float xFactor;
                    float yFactor;

                    UIElementWrapper.HighDpiScaleFactors(out xFactor, out yFactor);
                    try
                    {
                        pointX = (int)Math.Round(Int32.Parse (strX)*xFactor);
                    }
                    catch
                    {
                        Logger.Current.Log ("WARNING: X can't be converted, setting it to default");
                        pointX = 0;
                    }
                    try
                    {
                        pointY = (int)Math.Round(Int32.Parse (strY)*yFactor);
                    }
                    catch
                    {
                        Logger.Current.Log ("WARNING: Y can't be converted, setting it to default");
                        pointY = 0;
                    }

                    Rect rect = ElementUtils.GetScreenRelativeRect (element);
                    int xClick = (int)(rect.Left + pointX > rect.Right ? rect.Right : rect.Left + pointX);
                    int yClick = (int)(rect.Top + pointY > rect.Bottom ? rect.Bottom : rect.Top + pointY);

                    MouseInput.MouseClick (xClick, yClick);
                }
                else
                {
                    Logger.Current.Log ("ElementUtils.FindElement({0}) returns null. No mouse clicking happens", elementID.ToString ());
                }
            }
        }

        #region MouseElementRelative support.

        [Flags]
        private enum MouseCommand
        {
            Click = 0x1, PressDrag = 0x2, Down = 0x4, Up = 0x8, Wheel = 0x10, Move = 0x20
        }

        private static readonly MouseCommand s_mouseMoveBeforeCommands =
            MouseCommand.Click | MouseCommand.Down | MouseCommand.Up;
        private static readonly MouseCommand s_mouseMoveAfterCommands =
            MouseCommand.PressDrag | MouseCommand.Move;

        private static void ParseMouseDescription(string description,
            out MouseCommand command, out string arg, out int x, out int y,
            out bool coordinatesValid)
        {
            System.Diagnostics.Debug.Assert(description != null);
            CultureInfo ci = CultureInfo.InvariantCulture;

            //
            // Parse the action description. The format is as follows:
            //   command [arguments] [x y]
            // Where command is one of the following:
            //   click      (arg: left right)
            //   pressdrag  (arg: left right) [x y: destination]
            //   down       (arg: left right)
            //   up         (arg: left right)
            //   wheel      (arg: distance)
            //   move                         [x y: destination]
            //
            string[] parts = description.Split(' ');
            System.Diagnostics.Debug.Assert(parts.Length > 0);

            command = (MouseCommand)
                Enum.Parse(typeof(MouseCommand), parts[0], true);
            if ((int)command == 0)
                throw new ArgumentException(
                    "Unable to parse command: " + parts[0], "description");

            arg = null;
            x = int.MinValue;
            y = int.MinValue;
            coordinatesValid = false;
            int partIndex = 1;
            if (parts.Length > partIndex)
            {
                if (!Int32.TryParse(parts[partIndex],
                    NumberStyles.Integer, ci, out x))
                {
                    arg = parts[partIndex];
                    partIndex++;
                }
                if (parts.Length > partIndex)
                {
                    float xFactor;
                    float yFactor;
                    UIElementWrapper.HighDpiScaleFactors(out xFactor, out yFactor);
                    x = (int)Math.Round(Int32.Parse(parts[partIndex], ci) * xFactor);
                    partIndex++;
                    y = (int)Math.Round(Int32.Parse(parts[partIndex], ci) *yFactor);
                    coordinatesValid = true;
                }
            }
        }

        private static void MouseMoveRelative(UIElement element, int x, int y)
        {
            Rect rect = ElementUtils.GetScreenRelativeRect(element);
            int xClick = (int)rect.Left + x;
            int yClick = (int)rect.Top + y;
            MouseInput.MouseMove(xClick, yClick);
        }

        #endregion MouseElementRelative support.

        /// <summary>
        /// Performs a mouse action relative to the specified element.
        /// </summary>
        /// <param name='element'>
        /// Element relative to which to perform the action. May be null if not
        /// coordinates are supplied.
        /// </param>
        /// <param name='description'>
        /// Description of the action to be taken.
        /// </param>
        /// <remarks><p>
        /// The format for actions is as follows: command [arguments] [x y].
        /// The values for command are: click, pressdrag, down, up, wheel, move.
        /// click, pressdrag, down and up must have a left or right argument,
        /// determining which button to use. wheel takes a single argument
        /// with the clicks of the wheel.
        /// </p><p>
        /// The click, down, up and wheel command can specify x and y
        /// coordinates; in this case, the mouse is placed at this point
        /// before executing the action. If the command is pressdrag or
        /// move, x and y specify the destination position.
        /// </p></remarks>
        /// <example>
        /// The following sample shows how to perform some mouse gestures.
        /// <code>...
        /// // Left click.
        /// ActionItemWrapper.MouseElementRelative(null, "click left");
        ///
        /// // Right-click an element 20 pixels to the right and 20 pixels
        /// // down from its top-left corner.
        /// ActionItemWrapper.MouseElementRelative(myElement, "click right 20 20");
        ///
        /// // Press and drag the mouse to another point
        /// ActionItemWrapper.MouseElementRelative(myElement, "pressdrag left 20 40");
        ///
        /// // Move to another point and use the wheel (could be joined)
        /// ActionItemWrapper.MouseElementRelative(myElement, "move 20 20");
        /// ActionItemWrapper.MouseElementRelative(myElement, "wheel -100");
        /// </code></example>
        public static void MouseElementRelative(object element, string description)
        {
            UIElement uiElement = element as UIElement;
            if (element != null && uiElement == null)
            {
                throw new ArgumentException(
                    "Element, if supplied, must be a UIElement", "element");
            }
            if (description == null)
            {
                throw new ArgumentNullException("description");
            }
            description = description.Trim();
            if (description.Length == 0)
            {
                throw new ArgumentException(
                    "Mouse action description cannot be blank", "description");
            }

            string arg;
            int x;
            int y;
            bool coordinatesValid;
            MouseCommand command;
            ParseMouseDescription(description, out command, out arg,
                out x, out y, out coordinatesValid);

            bool mayMoveBefore = (command & s_mouseMoveBeforeCommands) == command;
            bool mustMoveAfter = (command & s_mouseMoveAfterCommands) == command;
            System.Diagnostics.Debug.Assert(!(mayMoveBefore && mustMoveAfter));

            if (mustMoveAfter && uiElement == null)
            {
                throw new InvalidOperationException(
                    "Moving for operation is required, but no element has " +
                    "been supplied.");
            }
            if (mustMoveAfter && !coordinatesValid)
            {
                throw new InvalidOperationException(
                    "Moving for operation is required, but no coordinates " +
                    "have been supplied.");
            }

            if (uiElement != null && mayMoveBefore && coordinatesValid)
            {
                MouseMoveRelative(uiElement, x, y);
            }

            switch (command)
            {
                case MouseCommand.Click:
                    if (arg == "left")
                        MouseInput.MouseClick();
                    else if (arg == "right")
                    {
                        MouseInput.RightMouseDown(); MouseInput.RightMouseUp();
                    }
                    else
                        throw new Exception("Unknown button to click: " + arg);
                    break;
                case MouseCommand.Down:
                    if (arg == "left")          MouseInput.MouseDown();
                    else if (arg == "right")    MouseInput.RightMouseDown();
                    else throw new Exception("Unknown button to press: " + arg);
                    break;
                case MouseCommand.Up:
                    if (arg == "left")          MouseInput.MouseUp();
                    else if (arg == "right")    MouseInput.RightMouseUp();
                    else throw new Exception("Unknown button to release: " + arg);
                    break;
                case MouseCommand.Wheel:
                    int clicks = Int32.Parse(arg, CultureInfo.InvariantCulture);
                    MouseInput.MouseWheel(clicks);
                    break;
                case MouseCommand.PressDrag:
                    if (arg == "left")          MouseInput.MouseDown();
                    else if (arg == "right")    MouseInput.RightMouseDown();
                    else throw new Exception("Unknown button to press-drag: " + arg);
                    break;
            }

            if (mustMoveAfter)
            {
                System.Diagnostics.Debug.Assert(uiElement != null);
                System.Diagnostics.Debug.Assert(coordinatesValid);
                MouseMoveRelative(uiElement, x, y);
            }

            if (command == MouseCommand.PressDrag)
            {
                if (arg == "left")          MouseInput.MouseUp();
                else if (arg == "right")    MouseInput.RightMouseUp();
            }
        }

        /// <summary>
        /// Wraps KeyboardInput.TypeString.
        /// </summary>
        /// <param name="keystrokeString">keystroke string to be typed. see KeyboardInput.TypeString for spec</param>
        public static void TypeString(string keystrokeString)
        {
            if (!String.IsNullOrEmpty (keystrokeString))
            {
                KeyboardInput.TypeString (keystrokeString);
            }
        }

        /// <summary>
        /// Selecting characters by dragging mouse
        /// </summary>
        /// <param name="wrapper">UIElementWrapper instance. I don't expect that to be created in xml.
        /// Use FindElement to create it and then pass it to this method</param>
        /// <param name="direction">SelectionDirection of mouse selection</param>
        /// <param name="startIndexInStr">start index of the character stream</param>
        /// <param name="cchInStr">number of characters to be selected</param>
        public static void SelectCharacterByMouse (object wrapper, string direction, string startIndexInStr, string cchInStr)
        {
            if (wrapper == null)
            {
                throw new ArgumentNullException("wrapper");
            }
            UIElementWrapper elementWrapper = wrapper as UIElementWrapper;
            if (elementWrapper == null)
            {
                throw new ArgumentException(
                    "wrapper [" + wrapper + "] is not of type UIElementWrapper",
                    "wrapper");
            }

            if (!String.IsNullOrEmpty (startIndexInStr) && !String.IsNullOrEmpty (cchInStr))
            {
                int startIndex = 0;
                int cch = 0;

                try
                {
                    startIndex = Int32.Parse (startIndexInStr);
                }
                catch
                {
                    Logger.Current.Log ("WARNING: startIndexInStr can't be converted, setting it to default");
                    startIndex = 0;
                }
                try
                {
                    cch = Int32.Parse (cchInStr);
                }
                catch
                {
                    Logger.Current.Log ("WARNING: cchInStr can't be converted, setting it to default");
                    cch = 0;
                }

                UIElementWrapper.SelectionDirection selectionDirection = (UIElementWrapper.SelectionDirection)Enum.Parse(typeof(UIElementWrapper.SelectionDirection), direction);

                elementWrapper.SelectCharacterByMouse(selectionDirection, startIndex, cch);
            }
        }

        /// <summary>
        /// Hold / release one key.
        /// This method takes keystrokeDescription string (e.g. "A" or "{BACK}")
        /// and this string can only hold *one* key. If the keystrokeDescription
        /// contain more than 1 key, it throws exception. Also, this method doesn't
        /// do key combination (e.g. +{BACK}) This method assumes that keystrokeDescription
        /// is interpreted as US English HKL
        /// </summary>
        /// <param name="keystrokeDescription">keystroke description string</param>
        /// <param name="strPressed">hole key = true, false otherwise</param>
        public static void PressOrReleaseOneKey(string keystrokeDescription, string strPressed)
        {
            bool pressed = true;

            if (strPressed == "false")
            {
                 pressed = false;
            }

            KeyboardInput.PressOrReleaseOneKey(keystrokeDescription, pressed);
        }

        #endregion Input methods.

        #region Verification methods.

        /// <summary>
        /// Verifies that a comparison criteria for two bitmaps match.
        /// </summary>
        /// <param name="master">Master bitmap.</param>
        /// <param name="sample">Sampled bitmap.</param>
        /// <param name="comparisonPrefix">Prefix for comparison name.</param>
        /// <example>
        /// The following sample shows how to use this method from an action.
        /// <code>
        /// &lt;SimilarCriteria-MaxColorDistance Value="0.10" /&gt;
        /// &lt;SimilarCriteria-MaxPixelDistance Value="1" /&gt;
        /// ...
        /// &lt;Action MethodName="VerifyBitmapComparison" Type="StaticMethod" ClassName="Test.Uis.Wrappers.ActionItemWrapper"&gt;
        ///  &lt;Param RetrieveFromReturnValue="BeforeScrollImage" /&gt;
        ///  &lt;Param RetrieveFromReturnValue="AfterScrollImage" /&gt;
        ///  &lt;Param Value="SimilarCriteria-" /&gt;
        /// &lt;/Action&gt; </code></example>
        public static void VerifyBitmapComparison(Bitmap master, Bitmap sample,
            string comparisonPrefix)
        {
            ComparisonOperation op = new ComparisonOperation();
            op.MasterImage = master;
            op.SampleImage = sample;
            op.ExecuteServiced(comparisonPrefix);
        }

        /// <summary>
        /// Verifies that two bitmaps are different.
        /// </summary>
        /// <param name="master">Master bitmap.</param>
        /// <param name="sample">Sampled bitmap.</param>
        public static void VerifyBitmapsDifferent(Bitmap master, Bitmap sample)
        {
            Bitmap differences;
            bool areEquals = ComparisonOperationUtils.AreBitmapsEqual(master, sample, out differences);
            if (areEquals)
            {
                Logger.Current.LogImage(master, "master");
                Logger.Current.LogImage(sample, "sample");
                Log("Bitmaps are equal and they were expected to be different.");
                Log("See master and sample files.");
                throw new Exception("Bitmaps are different.");
            }
            else
            {
                Log("Bitmaps are different.");
            }
        }

        /// <summary>
        /// Verifies that two bitmaps are equal.
        /// </summary>
        /// <param name="master">Master bitmap.</param>
        /// <param name="sample">Sampled bitmap.</param>
        public static void VerifyBitmapsEqual(Bitmap master, Bitmap sample)
        {
            Bitmap differences;
            bool areEquals = ComparisonOperationUtils.AreBitmapsEqual(master, sample, out differences);
            if (!areEquals)
            {
                Logger.Current.LogImage(master, "master");
                Logger.Current.LogImage(sample, "sample");
                Logger.Current.LogImage(differences, "diff");
                Log("Bitmaps are different and they were expected to be equal.");
                Log("See master, sample and diff files.");
                throw new Exception("Bitmaps are different.");
            }
            else
            {
                Log("Bitmaps are equal.");
            }
        }

        /// <summary>
        /// Verify text inside a TextBox.
        /// </summary>
        /// <param name="wrapper">UIElementWrapper object to be verified</param>
        /// <param name="expectedText">Expected string</param>
        /// <returns></returns>
        public static bool VerifyElementText(object wrapper, string expectedText)
        {
            if (wrapper == null)
            {
                throw new ArgumentNullException("wrapper");
            }

            UIElementWrapper elementWrapper = wrapper as UIElementWrapper;
            if (elementWrapper == null)
            {
                throw new ArgumentException(
                    "wrapper [" + wrapper + "] is not of type UIElementWrapper",
                    "wrapper");
            }

            string text = elementWrapper.Text;
            FrameworkElement frameworkElement = elementWrapper.Element as FrameworkElement;
            String output = String.Format ("Text in element ID [{0}] is [{1}], expected [{2}]",
                frameworkElement.Name.ToString (), text, expectedText);
            Verifier.Verify(text == expectedText, output, true);
            return true;
        }

        /// <summary>Verifies that two values are equal.</summary>
        /// <param name='value'>First value to compare.</param>
        /// <param name='expected'>Second value to compare (typically expected value).</param>
        /// <param name='description'>Description of what is being compared.</param>
        public static void VerifyEquals(object value, object expected, string description)
        {
            string message = "Expecting " + description + " [" + value +
                "] to be equal to [" + expected + "]";
            Logger.Current.Log(message);
            bool expectedNull = (expected == null || expected.ToString() == "*null");
            bool bothNull = value == null && expectedNull;
            if (!bothNull)
            {
                if (value == null || expected == null)
                {
                    throw new Exception(
                        "Only one of the values is null and cannot be compared." +
                        Environment.NewLine + message);
                }
                System.Diagnostics.Debug.Assert(value != null);
                System.Diagnostics.Debug.Assert(expected != null);

                expected = ReflectionUtils.GetValueForComparison(expected, value.GetType());

                if (!value.Equals(expected))
                {
                    throw new Exception("Verifcation failed for: " + message);
                }
            }
        }

        /// <summary>Verifies that the given value is greater than a known value.</summary>
        /// <param name='value'>Value verified.</param>
        /// <param name='known'>Known value to compare with.</param>
        /// <param name='description'>Description of value verified.</param>
        /// <example>The following sample shows how to use this method.<code>...
        ///   ActionItemWrapper.VerifyGreaterThan(myHeight, 3, "my height");
        /// </code></example>
        public static void VerifyGreaterThan(object value, object known, string description)
        {
            string message = "Expecting " + description + " [" + value +
                "] to be greater than [" + known + "]";
            Logger.Current.Log(message);
            IComparable c = (value as IComparable);
            if (c == null)
            {
                throw new InvalidOperationException("Value does not support IComparable");
            }
            known = ReflectionUtils.GetValueForComparison(known, value.GetType());
            int result = c.CompareTo(known);
            if (!(result > 0))
            {
                throw new Exception("Verification failed for: " + message);
            }
        }

        /// <summary>
        /// verify if the text selected is as expected.
        /// </summary>
        /// <param name="wrapper">UIElementWrapper instance to retrieve element text from</param>
        /// <param name="expectedSelectedText">expected selected text</param>
        public static void VerifySelectedText(object wrapper, string expectedSelectedText)
        {
            if (wrapper == null)
            {
                throw new ArgumentNullException("wrapper");
            }

            UIElementWrapper elementWrapper = wrapper as UIElementWrapper;
            if (elementWrapper == null)
            {
                throw new ArgumentException("Argument wrapper [" + wrapper +
                    "] is not of type UIElementWrapper");
            }

            string output = String.Format(
                "Comparing selected text [{0}] with expected selected text [{1}]",
                elementWrapper.GetSelectedText(false, false), expectedSelectedText);
            Verifier.Verify(elementWrapper.GetSelectedText(false, false) == expectedSelectedText, output, true);
        }

        /// <summary>
        /// Check if the passed bool value is equal to the value specified by boolNameToBeChecked
        /// in testxml
        /// </summary>
        /// <param name="boolToBeChecked">value to be compared</param>
        /// <param name="boolNameToBeChecked">value to be retrieved from testxml</param>
        public static void VerifyBooleanValue(bool boolToBeChecked, string boolNameToBeChecked)
        {
            bool boolInXml = ConfigurationSettings.Current.GetArgumentAsBool(boolNameToBeChecked);

            string message = String.Format("Bool name [{0}] is [{1}], the real value is [{2}]",
                   boolNameToBeChecked,
                   boolInXml.ToString(),
                   boolToBeChecked.ToString());

            Verifier.Verify(boolToBeChecked == boolInXml, message);
        }

        /// <summary>
        /// Get string on the left of caret
        /// </summary>
        /// <param name="wrapper">ElementWrapper object wrapping the tested element</param>
        /// <param name="expectedString">String to be compared against</param>
        public static void VerifyTextOnCaretLeft(object wrapper, string expectedString)
        {
            if (wrapper == null)
            {
                throw new ArgumentNullException("wrapper");
            }

            UIElementWrapper elementWrapper = wrapper as UIElementWrapper;

            if (elementWrapper == null)
            {
                throw new ArgumentException("wrapper [" + wrapper + "] is not of type UIElementWrapper", "wrapper");
            }

            string strOnLeft = elementWrapper.GetTextOutsideSelection(LogicalDirection.Backward);

            string message = String.Format("String on caret left [{0}], expected [{1}]",
                   strOnLeft,
                   expectedString);

            Verifier.Verify(strOnLeft == expectedString, message);
        }

        /// <summary>
        /// Get string on the right of caret
        /// </summary>
        /// <param name="wrapper">ElementWrapper object wrapping the tested element</param>
        /// <param name="expectedString">String to be compared against</param>
        public static void VerifyTextOnCaretRight(object wrapper, string expectedString)
        {
            if (wrapper == null)
            {
                throw new ArgumentNullException("wrapper");
            }

            UIElementWrapper elementWrapper = wrapper as UIElementWrapper;

            if (elementWrapper == null)
            {
                throw new ArgumentException("wrapper [" + wrapper + "] is not of type UIElementWrapper", "wrapper");
            }

            string strOnRight = elementWrapper.GetTextOutsideSelection(LogicalDirection.Forward);
            string message = String.Format("String on caret right [{0}], expected [{1}]", strOnRight, expectedString);

            Verifier.Verify(strOnRight == expectedString, message);
        }

        #endregion Verification methods.

        #region XAML manipulation methods.

        /// <summary>
        /// Sets the contents of the main window to the deserialization of
        /// the specified xaml file.
        /// </summary>
        public static void LoadMainXaml(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            string contents = XamlUtils.GetXamlFileContents(fileName);
            XamlUtils.ReplaceEscapedXaml(ref contents);
            SetMainXaml(contents);
        }

        /// <summary>
        /// Sets the contents of the main window to the deserialization of
        /// the specified xaml text.
        /// </summary>
        /// <param name='xaml'>XAML to deserialize into main window.</param>
        public static void SetMainXaml(string xaml)
        {
            if (Application.Current == null)
            {
                throw new InvalidOperationException("There is no current Application.Current object");
            }

            Window window = Application.Current.MainWindow;
            if (window == null)
            {
                throw new InvalidOperationException("There is no main window in Application.Current");
            }

            XamlUtils.ReplaceEscapedXaml(ref xaml);
            Logger.Current.Log(
                "Loading xaml into main window:" + Environment.NewLine + xaml);
            window.Content = (FrameworkElement) XamlUtils.ParseToObject(xaml);
        }

        #endregion XAML manipulation methods.

        #region Private methods.

        /// <summary>Clicks on every element, recursively.</summary>
        /// <param name='element'>Element to start clicking on.</param>
        /// <param name='visitedItems'>List of items already visited.</param>
        private static void ClickEveryElementRecursive(Visual element,
            List<Visual> visitedItems)
        {
            UIElement clickableElement;     // Element to click on.
            Rect rect;                      // Rectangle to hit test.
            Window window;                  // Window hosting control.

            // Add the element to the list of visited items.
            // This catches problems with the tree.
            visitedItems.Add(element);

            // Click on things that are simple chrome, like borders.
            clickableElement = element as UIElement;
            if (clickableElement != null)
            {
                // This code helps us avoid collapsed elements on
                // the edge of the window. The problem is that these
                // clicks may never generate messages and the looping code to
                // guard against race conditions hangs the test.
                window = ElementUtils.GetWindowFromElement(clickableElement);
                rect = ElementUtils.GetScreenRelativeRect(clickableElement);
                if (rect.Left < (window.ActualWidth - 8) && rect.Top < (window.ActualHeight - 8))
                {
                    Logger.Current.Log("Click on rect: " + rect + "\r\n" +
                        "Window width: " + window.ActualWidth + "\r\n" +
                        "Window height: " + window.ActualHeight + "\r\n");
                    MouseInput.MouseClick(clickableElement);
                }
            }

            // Recursively walk all the children.
            int count = VisualTreeHelper.GetChildrenCount(element);
            for(int i = 0; i < count; i++)
            {
                Visual v = VisualTreeHelper.GetChild(element, i) as Visual;
                if (v != null)
                {
                    if (visitedItems.IndexOf(v) != -1)
                    {
                        throw new Exception("Visual has already been " +
                            "visited before (endless loop detected): " + v);
                    }
                    ClickEveryElementRecursive(v);
                }
            }
        }

        #endregion Private methods.
    }
}