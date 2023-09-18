// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Few tests with TextOptions.TextFormattingMode set to Ideal and Display

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Threading;

    using Microsoft.Test.Discovery;
    using Microsoft.Test.Imaging;
    using Microsoft.Test.Logging;
    using Microsoft.Test.Input;
    using Microsoft.Test.Threading;

    using Test.Uis.Data;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.   
    /// <summary>
    /// Verifies that the scroll methods actually perform scrolling
    /// operations.
    /// </summary>
    /// <remarks>
    /// The following members are tested: LineUp, LineDown, LineLeft,
    /// LineRight, PageUp, PageDown, PageLeft, PageRight, ScrollToHome,
    /// ScrollToEnd, HorizontalOffset, VerticalOffset,
    /// ScroolToLine
    /// </remarks>
    [Test(0, "TextBox", "TextBoxScrollMethodsInDisplayMode", MethodParameters = "/TestCaseType=TextBoxScrollMethodTests /TextFormattingmode=Display", Timeout = 250, SupportFiles = @"FeatureTests\Editing\TextBox.xaml", Keywords = "TextFormattingModeTests")]
    [Test(0, "TextBox", "TextBoxScrollMethodsInIdealMode", MethodParameters = "/TestCaseType=TextBoxScrollMethodTests /TextFormattingmode=Ideal", Timeout = 250, SupportFiles = @"FeatureTests\Editing\TextBox.xaml", Keywords = "TextFormattingModeTests")]
    public class TextBoxScrollMethodTests : TextBoxTestCase
    {
        #region Private data.

        /// <summary>Last bitmap taken.</summary>
        private System.Drawing.Bitmap _lastBitmap;

        /// <summary>Last value for horizontal position.</summary>
        private double _lastHorizontal;

        /// <summary>Last value for vertical position.</summary>
        private double _lastVertical;

        /// <summary>Step index.</summary>
        private int _stepIndex;

        /// <summary>Comparison operation.</summary>
        enum ComparisonOp
        {
            EQ, LT, GT
        }

        /// <summary>Vertical or horizontal position properties.</summary>
        enum Pos
        {
            Vertical, Horizontal
        }

        /// <summary>A test step to be executed in sequence.</summary>
        struct TestStep
        {
            #region Constructors.

            /// <summary>Initializes a new TestStep instance.</summary>
            public TestStep(string method, Pos property,
                ComparisonOp comparisonOp)
                : this(method, property, comparisonOp, false)
            {
            }

            /// <summary>
            /// Initializes a new TestStep instance, possibly one that
            /// should be ignored in input emulation cases.
            /// </summary>
            public TestStep(string method, Pos property,
                ComparisonOp comparisonOp, bool ignoreInput)
            {
                this._method = method;
                this._property = property;
                this._comparisonOp = comparisonOp;
                this._ignoreInput = ignoreInput;
            }

            #endregion Constructors.

            #region Public properties.

            /// <summary>Method to invoke.</summary>
            public string Method
            {
                get { return _method; }
            }

            /// <summary>Property to verify after invocation.</summary>
            public Pos Property
            {
                get { return _property; }
            }

            /// <summary>Expected change in property after invocation.</summary>
            public ComparisonOp ComparisonOp
            {
                get { return _comparisonOp; }
            }

            /// <summary>
            /// Whether this step is ignored in the input emulation pass.
            /// </summary>
            public bool IgnoreInput
            {
                get { return _ignoreInput; }
            }

            #endregion Public properties.

            #region Private fields.

            private Pos _property;
            private string _method;
            private ComparisonOp _comparisonOp;
            private bool _ignoreInput;

            #endregion Private fields.
        }

        /// <summary>Steps to execute a test run.</summary>
        private TestStep[] _steps = new TestStep[] {
            new TestStep("LineRight",   Pos.Horizontal, ComparisonOp.GT),
            new TestStep("LineLeft",    Pos.Horizontal, ComparisonOp.LT),
            new TestStep("PageRight",   Pos.Horizontal, ComparisonOp.GT),
            new TestStep("PageLeft",    Pos.Horizontal, ComparisonOp.LT),

            new TestStep("LineRight",   Pos.Vertical,   ComparisonOp.EQ),
            new TestStep("LineLeft",    Pos.Vertical,   ComparisonOp.EQ),
            new TestStep("PageRight",   Pos.Vertical,   ComparisonOp.EQ),
            new TestStep("PageLeft",    Pos.Vertical,   ComparisonOp.EQ),

            new TestStep("LineDown",    Pos.Vertical,   ComparisonOp.GT),
            new TestStep("LineUp",      Pos.Vertical,   ComparisonOp.LT),
            new TestStep("PageDown",    Pos.Vertical,   ComparisonOp.GT),
            new TestStep("PageUp",      Pos.Vertical,   ComparisonOp.LT),

            new TestStep("LineDown",    Pos.Horizontal, ComparisonOp.EQ),
            new TestStep("LineUp",      Pos.Horizontal, ComparisonOp.EQ),
            new TestStep("PageDown",    Pos.Horizontal, ComparisonOp.EQ),
            new TestStep("PageUp",      Pos.Horizontal, ComparisonOp.EQ),

            new TestStep("ScrollToEnd", Pos.Vertical,   ComparisonOp.GT, true),
            new TestStep("ScrollToHome", Pos.Vertical,  ComparisonOp.LT, true),

            new TestStep("ScrollToEnd", Pos.Horizontal, ComparisonOp.EQ, true),
            new TestStep("ScrollToHome", Pos.Horizontal,ComparisonOp.EQ, true),
            //Scrolling to bottom most line
            new TestStep("ScrollToLine_Bottom", Pos.Vertical,   ComparisonOp.GT, true),
            //Scrolling to top most line
            new TestStep("ScrollToLine_Top", Pos.Vertical,  ComparisonOp.LT, true),

            new TestStep("ScrollToLine_Bottom", Pos.Horizontal, ComparisonOp.EQ, true),
            new TestStep("ScrollToLine_Top", Pos.Horizontal,ComparisonOp.EQ, true),
        };

        /// <summary>
        /// Whether the test is running in 'input emulation' mode.
        /// </summary>
        private bool _testInput;

        #endregion Private data.

        #region Configuration settings.

        /// <summary>
        /// Whether to test through user input emulation or through API calls.
        /// </summary>
        public bool TestInput
        {
            get { return _testInput; }
        }

        #endregion Configuration settings.

        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            _testInput = false;
            StartTestCase();
        }

        /// <summary>Starts a test case run.</summary>
        /// <remarks>
        /// The TestInput property should not change within
        /// a given run.
        /// </remarks>
        private void StartTestCase()
        {
            if (TestInput)
            {
                EnsureScrollbarsVisible();
            }

            TestTextBox.FontSize = 36;
            switch (ConfigurationSettings.Current.GetArgument("TextFormattingmode"))
            {
                case "Display": TextOptions.SetTextFormattingMode(TestTextBox, TextFormattingMode.Display);
                    break;
                case "Ideal": TextOptions.SetTextFormattingMode(TestTextBox, TextFormattingMode.Ideal);
                    break;
            }
            string str = "";
            for (int i = 0; i < 16; i++)
            {
                str += i.ToString() + "abcdefghijklmnopqrstuvwxvz" + "\r\n";
            }
            TestTextBox.Text = str;
            Log(TextUtils.ConvertToSingleLineAnsi(Text));

            MouseInput.MouseClick(TestTextBox);
            _stepIndex = 0;

            QueueDelegate(BeforeStep);
        }

        /// <summary>Log and send input or API call for a step.</summary>
        private void BeforeStep()
        {
            string method;  // Name of method to execute.

            GetCurrentStatus();
            method = _steps[_stepIndex].Method;

            if (!TestInput)
            {
                Log("Executing method: " + method);
                // Not using Reflection to catch any breaks at build time.
                switch (method)
                {
                    case "LineRight":
                        TestTextBox.LineRight();
                        break;
                    case "LineLeft":
                        TestTextBox.LineLeft();
                        break;
                    case "LineDown":
                        TestTextBox.LineDown();
                        break;
                    case "LineUp":
                        TestTextBox.LineUp();
                        break;
                    case "PageRight":
                        TestTextBox.PageRight();
                        break;
                    case "PageLeft":
                        TestTextBox.PageLeft();
                        break;
                    case "PageDown":
                        TestTextBox.PageDown();
                        break;
                    case "PageUp":
                        TestTextBox.PageUp();
                        break;
                    case "ScrollToEnd":
                        TestTextBox.ScrollToEnd();
                        break;
                    case "ScrollToHome":
                        TestTextBox.ScrollToHome();
                        break;
                    case "ScrollToLine_Bottom":
                        TestTextBox.LineUp(); //To make sure we are atleast one line up than the bottom most line
                        TestTextBox.ScrollToLine(TestTextBox.LineCount - 1);
                        break;
                    case "ScrollToLine_Top":
                        TestTextBox.LineDown(); //To make sure we are atleast one line down than the top most line
                        TestTextBox.ScrollToLine(0);
                        break;
                    default:
                        throw new Exception("Unexpected method to invoke.");
                }
            }
            else
            {
                if (!_steps[_stepIndex].IgnoreInput)
                {
                    EmulateMethod(method);
                }
            }

            QueueDelegate(AfterStep);
        }

        /// <summary>Verify whether the step succeeded.</summary>
        private void AfterStep()
        {
            bool verify;    // Whether there is no mouse input step.
            verify = !(TestInput && _steps[_stepIndex].IgnoreInput);
            if (verify)
            {
                VerifyBitmapChanged();
                VerifyPropertyChanged();
            }
            _stepIndex++;
            if (_stepIndex < _steps.Length)
            {
                QueueDelegate(BeforeStep);
            }
            else
            {
                if (_testInput == false)
                {
                    // Run the whole test case one
                    // more time, with testInput = true.
                    _testInput = true;
                    QueueDelegate(StartTestCase);
                }
                else
                {
                    // Finished with API and input passes.
                    Logger.Current.ReportSuccess();
                }
            }
        }

        #endregion Main flow.

        #region Verifications.

        /// <summary>Sets up scrollbars to be visible.</summary>
        private void EnsureScrollbarsVisible()
        {
            TestTextBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            TestTextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
        }

        private void GetCurrentStatus()
        {
            _lastHorizontal = TestTextBox.HorizontalOffset;
            _lastVertical = TestTextBox.VerticalOffset;
            _lastBitmap = BitmapCapture.CreateBitmapFromElement(TestTextBox);
        }

        private void VerifyBitmapChanged()
        {
            System.Drawing.Bitmap current;
            System.Drawing.Bitmap delta;

            Log("Verifying that the bitmap changed...");

            current = BitmapCapture.CreateBitmapFromElement(TestTextBox);
            if (ComparisonOperationUtils.AreBitmapsEqual(_lastBitmap, current, out delta))
            {
                Logger.Current.LogImage(_lastBitmap, "last");
                Logger.Current.LogImage(current, "current");
                throw new Exception("Bitmaps are equal");
            }
            current.Dispose();
        }

        private void EmulateMethod(string method)
        {
            const string parentPath = "(.//ScrollViewer/Grid/";

            Log("Emulating method " + method + " through user input...");
            switch (method)
            {
                case "LineRight":
                    ClickPath(parentPath + "ScrollBar[2]//RepeatButton)[4]");
                    break;
                case "LineLeft":
                    ClickPath(parentPath + "ScrollBar[2]//RepeatButton)[1]");
                    break;
                case "LineDown":
                    ClickPath(parentPath + "ScrollBar[1]//RepeatButton)[4]");
                    break;
                case "LineUp":
                    ClickPath(parentPath + "ScrollBar[1]//RepeatButton)[1]");
                    break;
                case "PageRight":
                    ClickPath(parentPath + "ScrollBar[2]//Thumb)", BoxPosition.ToRight);
                    break;
                case "PageLeft":
                    ClickPath(parentPath + "ScrollBar[2]//Thumb)", BoxPosition.ToLeft);
                    break;
                case "PageDown":
                    ClickPath(parentPath + "ScrollBar[1]//Thumb)", BoxPosition.ToBottom);
                    break;
                case "PageUp":
                    ClickPath(parentPath + "ScrollBar[1]//Thumb)", BoxPosition.ToTop);
                    break;
                case "ScrollToEnd":
                    throw new Exception("Cannot emulate ScrollToEnd - mark as IgnoreInput");
                case "ScrollToHome":
                    throw new Exception("Cannot emulate ScrollToHome - mark as IgnoreInput");
                case "ScrollToLine_Top":
                    throw new Exception("Cannot emulate ScrollToLine - mark as IgnoreInput");
                case "ScrollToLine_Bottom":
                    throw new Exception("Cannot emulate ScrollToLine - mark as IgnoreInput");
                default:
                    throw new Exception("Unexpected method to invoke.");
            }
        }

        private enum BoxPosition
        {
            Center,
            ToRight,
            ToLeft,
            ToTop,
            ToBottom
        }

        private void ClickPath(string xpath)
        {
            ClickPath(xpath, BoxPosition.Center);
        }

        private void ClickPath(string xpath, BoxPosition position)
        {
            Rect r;                 // bounding rectangle
            UIElement uiElement;    // UIElement visual
            Visual xpathVisual;     // visual resulting from XPath
            double x, y;            // coordinates to click on

            System.Diagnostics.Debug.Assert(xpath != null);

            xpathVisual = GetVisual(xpath);
            Log("Clicking on position " +
                position.ToString() + " of element " + xpathVisual.ToString() +
                " obtained through path " + xpath);

            uiElement = xpathVisual as UIElement;
            if (uiElement == null)
            {
                throw new NotImplementedException("Rects off Visuals not implemented.");
            }
            r = ElementUtils.GetScreenRelativeRect(uiElement);

            x = r.Left + r.Width / 2;
            y = r.Top + r.Height / 2;
            switch (position)
            {
                case BoxPosition.ToRight:
                    x = r.Left + r.Width + 4;
                    break;
                case BoxPosition.ToLeft:
                    x = r.Left - 4;
                    break;
                case BoxPosition.ToTop:
                    y = r.Top - 4;
                    break;
                case BoxPosition.ToBottom:
                    y = r.Top + r.Height + 4;
                    break;
            }
            MouseInput.MouseClick((int)x, (int)y);
            //Under themes which have special effect, like �Areo� and etc, if the case move the mouse and click immediately,it doesnt work.
            //It need to wait 1 second, click twice is for that.
            if ((string.Compare(Win32.SafeGetCurrentThemeName(), "Aero", true, System.Globalization.CultureInfo.InvariantCulture) == 0)||
                (string.Compare(Win32.SafeGetCurrentThemeName(), "AeroLite", true, System.Globalization.CultureInfo.InvariantCulture) == 0))
            {
                _globalX = x;
                _globalY = y;
                QueueDelegate(new SimpleHandler(MouseClick));
            }
        }

        private void MouseClick()
        {
            MouseInput.MouseClick((int)_globalX, (int)_globalY);
        }

        double _globalX;
        double _globalY;

        private Visual GetVisual(string xpath)
        {
            Visual[] v = XPathNavigatorUtils.ListVisuals(MainWindow, xpath);
            if (v.Length == 0)
            {
                Log("Unable to find xpath" + xpath);
                Log("Visual tree for main window follows.");
                Log(VisualLogger.DescribeVisualTree(MainWindow));
                throw new Exception("Unable to find element for path: " + xpath);
            }
            return v[0];
        }

        private void VerifyPropertyChanged()
        {
            Log("Examining value: " + _steps[_stepIndex].Property.ToString());
            Log("Expecting value to be: " + _steps[_stepIndex].ComparisonOp);

            double prevValue;
            double currValue;

            switch (_steps[_stepIndex].Property)
            {
                case Pos.Horizontal:
                    prevValue = _lastHorizontal;
                    currValue = TestTextBox.HorizontalOffset;
                    break;

                case Pos.Vertical:
                    prevValue = _lastVertical;
                    currValue = TestTextBox.VerticalOffset;
                    break;

                default:
                    throw new Exception("Unexpected position property value.");
            }

            Log("Previous value: " + prevValue);
            Log("Current value:  " + currValue);
            Log("Current change: " + _steps[_stepIndex].ComparisonOp);

            bool result;
            switch (_steps[_stepIndex].ComparisonOp)
            {
                case ComparisonOp.EQ:
                    result = currValue == prevValue;
                    break;
                case ComparisonOp.LT:
                    result = currValue < prevValue;
                    break;
                case ComparisonOp.GT:
                    result = currValue > prevValue;
                    break;
                default:
                    throw new Exception("Unexpected comparision operation value.");
            }

            Verifier.Verify(result);
        }

        #endregion Verifications.
    }
}