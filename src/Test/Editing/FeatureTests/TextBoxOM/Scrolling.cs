// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Functional test cases for TextBox members that affect scrolling and     //
//  are not covered elsewhere: LineLeft, LineRight, PageLeft, PageRight     //
//  LineUp, LineDown, PageUp, PageDown, ScrollToHome, ScrollToEnd,          //
//  HorizontalScrollBarVisibility, VerticalScrollBarVisibility,             //
//  ExtentWidth, ExtentHeight, ViewportWidth, ViewportHeight,               //
//  HorizontalOffset, VerticalOffset.                                       //

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Text/BVT/TextBoxOM/Scrolling.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Drawing;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Media;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;
    using Microsoft.Test.Imaging;

    using Test.Uis.Data;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>Choices for the Line functions calls </summary>
    enum FunctionName
    {
        LineDown,
        LineUp,
        LineLeft,
        LineRight,
    }

    /// <summary> Choices for the Page calls in TextBoxBase</summary>
    enum PageFunctionName
    {
        PageDown,
        PageDownInterestingCase,
        PageUp,
        PageUpInterestingCase,
        PageLeft,
        PageRight,
        PageHome,
        PageEnd,
    }

    /// <summary> Choices for the Scroll calls in TextBoxBase</summary>
    enum TextBoxScrollFunction
    {
        Start,
        End,
        InvalidCalls,
    }

    /// <summary> Choices for the Offset operations in TextBoxBase</summary>
    enum OffsetList
    {
        Horizontal,
        Vertical,
    }

    /// <summary> Choices for the Viewport and Extent operations in TextBoxBase</summary>
    enum ViewportExtentList
    {
        HorizontalScrollbarEffect,
        VerticalScrollbarEffect,
        ChangeExtent,
        ChangeViewport,
    }

    enum InputTrigger
    {
        Programmatical,
        // Keyboard,
    }

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
    [Test(0, "TextBox", "TextBoxScrollMethods", MethodParameters = "/TestCaseType=TextBoxScrollMethods", Timeout = 250)]
    [TestOwner("Microsoft"), TestTactics("568"), TestBugs("711,712"), TestLastUpdatedOn("May 11, 2006")]
    public class TextBoxScrollMethods : TextBoxTestCase
    {
        #region Private data.

        /// <summary>Last bitmap taken.</summary>
        private Bitmap _lastBitmap;

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
                _method = method;
                _property = property;
                _comparisonOp = comparisonOp;
                _ignoreInput = ignoreInput;
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
                    // more time, with _testInput = true.
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
            Bitmap current;
            Bitmap delta;

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

            // 
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
            if ((string.Compare(Win32.SafeGetCurrentThemeName(), "Aero", true, System.Globalization.CultureInfo.InvariantCulture) == 0) || 
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

    /// <summary>
    /// Verifies that the scroll methods actually perform scrolling
    /// operations.
    /// </summary>
    /// <remarks>
    /// The following members are tested:
    /// HorizontalScrollBarVisibility, VerticalScrollBarVisibility,
    /// ViewportWidth, ViewportHeight, ExtentWidth, ExtentHeight.
    /// </remarks>
    [Test(0, "TextBox", "TextBoxScrollScrollbars", MethodParameters = "/TestCaseType=TextBoxScrollScrollbars /TestName=TextBoxScrollScrollbars-Sizes")]
    [TestOwner("Microsoft"), TestTactics("567"), TestArgument("Text", "Text content for control.")]
    public class TextBoxScrollScrollbars : TextBoxTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            Log(TextUtils.ConvertToSingleLineAnsi(Text));

            SetTextBoxProperties(TestTextBox);
            TestTextBox.Text = Text;
            MouseInput.MouseClick(TestTextBox);

            QueueHelper.Current.QueueDelegate(TestMembers);
        }

        private void TestMembers()
        {
            //
            // The extent may actually change if the text does not occupy the area
            // without the scrollbars, so we will not verify that here.
            //

            double size, newSize;
            double extent, newExtent;
            Bitmap image;

            Log("Verifying that the horizontal scrollbar affects " +
                "vertical size but not extent.");
            TestTextBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            TestTextBox.UpdateLayout();
            size = TestTextBox.ViewportHeight;
            extent = TestTextBox.ExtentHeight;
            image = BitmapCapture.CreateBitmapFromElement(TestTextBox);
            Logger.Current.LogImage(image, "WithoutScrollbar");
            Log("Without scrollbar: size=" + size + ", extent=" + extent);

            TestTextBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            TestTextBox.UpdateLayout();
            newSize = TestTextBox.ViewportHeight;
            newExtent = TestTextBox.ExtentHeight;
            Log("With scrollbar: size=" + newSize + ", extent=" + newExtent);
            image = BitmapCapture.CreateBitmapFromElement(TestTextBox);
            Logger.Current.LogImage(image, "WithHorizontalScrollbar");
            Verifier.Verify(newSize < size, "Size decreased", true);
            // Verifier.Verify(newExtent == extent, "Extent is unchanged", true);

            Log("Verifying that the vertical scrollbar affects " +
                "horizontal size but not extent.");
            TestTextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            TestTextBox.UpdateLayout();
            size = TestTextBox.ViewportWidth;
            extent = TestTextBox.ExtentWidth;
            Log("Without scrollbar: size=" + size + ", extent=" + extent);

            TestTextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            TestTextBox.UpdateLayout();
            newSize = TestTextBox.ViewportWidth;
            newExtent = TestTextBox.ExtentWidth;
            Log("With scrollbar: size=" + newSize + ", extent=" + newExtent);
            Verifier.Verify(newSize < size, "Size decreased", true);
            // Verifier.Verify(newExtent == extent, "Extent is unchanged", true);

            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.
    }

    /// <summary>
    /// Verifies that the PgUp/PgDown keys scroll by a significant
    /// amount and that they return to the same place.
    /// </summary>
    /// <remarks>PgUp/PgDown are not checked in for TextEditor yet.</remarks>
    [TestOwner("Microsoft"), TestTactics("566")]
    public class TextBoxScrollPgKeys : TextBoxTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            SetTextBoxProperties(TestTextBox);
            TestTextBox.Text = Text;
            MouseInput.MouseClick(TestTextBox);

            QueueHelper.Current.QueueDelegate(GetPositionBeforeDown);
        }

        private void GetPositionBeforeDown()
        {
            Log("Remembering position before PgDown...");
            _beforeDown = TestTextBox.SelectionStart;
            Log("Current position: " + _beforeDown);

            _beforeDownImage = BitmapCapture.CreateBitmapFromElement(TestTextBox);

            KeyboardInput.TypeString("{PGDN}");
            QueueHelper.Current.QueueDelegate(GetPositionAfterDown);
        }

        private void GetPositionAfterDown()
        {
            Log("Verifying that position moved...");
            int position = TestTextBox.SelectionStart;
            Log("Current position: " + position);
            Verifier.Verify(position > _beforeDown);

            KeyboardInput.TypeString("{PGUP}");
            QueueHelper.Current.QueueDelegate(GetPositionAfterUp);
        }

        private void GetPositionAfterUp()
        {
            Log("Verifying that position was restored...");
            int position = TestTextBox.SelectionStart;
            Log("Current position: " + position);
            Verifier.Verify(position == _beforeDown);

            Bitmap bmp = BitmapCapture.CreateBitmapFromElement(TestTextBox);
            Bitmap delta;

            if (!ComparisonOperationUtils.AreBitmapsEqual(_beforeDownImage, bmp, out delta))
            {
                Logger.Current.LogImage(_beforeDownImage, "before");
                Logger.Current.LogImage(bmp, "after");
                Logger.Current.LogImage(delta, "delta");
                throw new Exception("Before and After images are different.");
            }

            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.

        #region Private fields.

        private int _beforeDown;
        private Bitmap _beforeDownImage;

        #endregion Private fields.
    }

    /// <summary>
    /// Verifies that the scrollbars are taken into account when considering
    /// when text should be wrapped.
    /// </summary>
    [Test(0, "TextBox", "TextBoxScrollReproRegression_Bug633", MethodParameters = "/TestCaseType=TextBoxScrollReproRegression_Bug633")]
    [TestOwner("Microsoft"), TestTactics("565"), TestBugs("633")]
    public class TextBoxScrollReproRegression_Bug633 : CustomTestCase
    {
        #region Main flow.

        private TextBox _textbox;

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            // Create a TextBox in a DockPanel.
            // The TextBox has properties and set text such that the
            // second character would slightly overlap the scrollbar.
            // If two lines appear, then this is not the case.
            DockPanel panel = new DockPanel();
            _textbox = new TextBox();
            DockPanel.SetDock(_textbox, Dock.Left);

            _textbox.FontSize = 64;
            _textbox.FontFamily = new System.Windows.Media.FontFamily("Courier New");
            OperatingSystem os = Environment.OSVersion;
            Version ver = os.Version;
            //The theme of Win8 is different with others� OS, so adjust it
            //Default padding value of textbox is 1 on Win7, but the value is 0 on Win8 OS
            if (ver.Major > 6 || ((6 == ver.Major) && ver.Minor > 1))
            {
                _textbox.Width = 99;
            }
            else
            {
                _textbox.Width = 100;
            }
            _textbox.TextWrapping = TextWrapping.Wrap;
            _textbox.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            _textbox.Text = "ii";

            // Add controls to the window.
            panel.Children.Add(_textbox);
            //_textbox.Height = new Length(100, UnitType.Percent);
            MainWindow.Content = panel;

            QueueHelper.Current.QueueDelegate(VerifyRendering);
        }

        private void VerifyRendering()
        {
            const int borderWidth = 8; // probably less, but erring on side of caution.

            Log("Capturing bitmap...");
            Bitmap rendering = BitmapCapture.CreateBitmapFromElement(_textbox);
            Bitmap borderless = BitmapUtils.CreateBorderlessBitmap(rendering, borderWidth);
            Bitmap bw = BitmapUtils.ColorToBlackWhite(borderless);
            System.Drawing.Rectangle r = BitmapUtils.GetBoundingRectangle(bw);

            Log("Height of content: " + r.Height);
            if (r.Height <= 64)
            {
                Log("There appears to be only one line (height <= 64).");
                Logger.Current.LogImage(rendering, "lines");
                throw new Exception("Scrollbars seem not to be taken into account for wrapping.");
            }
            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.
    }

    /// <summary>
    /// Verifies that when we set HorizontalOffset and VerticalOffset, the contents get scrolled.
    /// </summary>
    [Test(0, "TextBoxBase", "TextBoxBaseScrollOffset", MethodParameters = "/TestCaseType=TextBoxBaseScrollOffset")]
    [TestOwner("Microsoft"), TestTactics("564"), TestBugs("634")]
    public class TextBoxBaseScrollOffset : CustomCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Gets the dimensions to combine.</summary>
        protected override Dimension[] DoGetDimensions()
        {
            string[] contents;
            object[] visibilityValues;

            visibilityValues = new object[] {
                ScrollBarVisibility.Auto,
                //ScrollBarVisibility.Disabled
                ScrollBarVisibility.Hidden,
                ScrollBarVisibility.Visible,
            };

            _largeContent = TextUtils.RepeatString(TextUtils.RepeatString(_shortContent, 30) + "\r\n", 15);

            contents = new string[] {
                //_shortContent
                _largeContent
            };

            return new Dimension[] {
                new Dimension("TextEditableType", new TextEditableType[] {TextEditableType.GetByName("TextBox"), 
                    TextEditableType.GetByName("RichTextBox")}),
                new Dimension("HorizontalVisibility", visibilityValues),                
                new Dimension("VerticalVisibility", visibilityValues),
                new Dimension("Offset", new object[] {30.0, Double.MaxValue}),
                new Dimension("Content", contents),                                
            };
        }

        /// <summary>Reads combination values.</summary>
        protected override bool DoReadCombination(Hashtable values)
        {
            _textEditableType = (TextEditableType)values["TextEditableType"];
            _horizontalSBVisibility = (ScrollBarVisibility)values["HorizontalVisibility"];
            _verticalSBVisibility = (ScrollBarVisibility)values["VerticalVisibility"];
            _offset = (double)values["Offset"];
            _content = (string)values["Content"];

            return true;
        }

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            _editControl = (TextBoxBase)_textEditableType.CreateInstance();
            _editControlWrapper = new UIElementWrapper(_editControl);

            _editControl.Height = 150;
            _editControl.Width = 200;
            _editControl.FontSize = 11.0;
            _editControl.FontFamily = new System.Windows.Media.FontFamily("Tahoma");
            if (_editControl is TextBox)
            {
                ((TextBox)_editControl).TextWrapping = TextWrapping.NoWrap;
            }
            _editControlWrapper.Text = _content;

            _editControl.HorizontalScrollBarVisibility = _horizontalSBVisibility;
            _editControl.VerticalScrollBarVisibility = _verticalSBVisibility;

            TestElement = _editControl;

            QueueDelegate(BeforeSettingHorizontalOffset);
        }

        private void BeforeSettingHorizontalOffset()
        {
            _originalHorizontalOffset = _editControl.HorizontalOffset;
            _originalVerticalOffset = _editControl.VerticalOffset;

            _beforeSetting = BitmapCapture.CreateBitmapFromElement(_editControl);

            if (((_textEditableType.Type == typeof(TextBox)) && (!_invalidTestingTBDone)) ||
                ((_textEditableType.Type == typeof(RichTextBox)) && (!_invalidTestingRTBDone)))
            {
                DoInvalidTesting();
            }

            //compute expected HorizontalOffset and VerticalOffset                      
            if (_content == _largeContent)
            {
                if (_offset == Double.MaxValue)
                {
                    //We are setting offset to Double.MaxValue. Since the edit controls in this 
                    //test dont have that much to scroll, the values are set to maximum value 
                    //it can scroll. That value should be atleast > 50.0 for _largeContent                    
                    _expHorizontalOffset = 50.0;
                    _expVerticalOffset = 50.0;
                }
                else
                {
                    _expHorizontalOffset = _offset;
                    _expVerticalOffset = _offset;
                }
            }
            else
            {
                //ShortContent -> so no scrolling -> so the values should be updated to 0.0
                _expHorizontalOffset = 0.0;
                _expVerticalOffset = 0.0;
            }

            if (_verticalSBVisibility == ScrollBarVisibility.Disabled)
            {
                _expVerticalOffset = 0.0;
            }
            if (_horizontalSBVisibility == ScrollBarVisibility.Disabled)
            {
                _expHorizontalOffset = 0.0;
            }

            //setting value for HorizontalOffset
            _editControl.ScrollToHorizontalOffset(_offset);

            QueueDelegate(AfterSettingHorizontalOffset);
        }

        private void AfterSettingHorizontalOffset()
        {
            _afterSetting = BitmapCapture.CreateBitmapFromElement(_editControl);

            if (_editControl is RichTextBox)
            {
                //Verify that rendering is updated after setting HorizontalOffset.
                if (ComparisonOperationUtils.AreBitmapsEqual(_beforeSetting, _afterSetting, out _differences))
                {
                    Log("Bitmaps are equal after setting RTB - Horizontal offset should not work");
                }
                else
                {
                    Logger.Current.LogImage(_afterSetting, "AfterSetting");
                    throw new Exception("There have been changes in rendering after setting HorizontalOffset in RTB.");
                }
                Verifier.Verify(_editControl.HorizontalOffset == _originalHorizontalOffset,
                    "Verifying HorizontalOffset value after setting it:" +
                    " Actual [" + _editControl.HorizontalOffset +
                    " Expected [" + _originalHorizontalOffset + "]", true);

            }
            else
            {
                //if _content == _shortContent, no rendering change is expected
                if ((_content == _largeContent) &&
                    (_horizontalSBVisibility != ScrollBarVisibility.Disabled))
                {
                    //Verify that rendering is updated after setting HorizontalOffset.
                    if (ComparisonOperationUtils.AreBitmapsEqual(_beforeSetting, _afterSetting, out _differences))
                    {
                        Logger.Current.LogImage(_afterSetting, "AfterSetting");
                        throw new Exception("There have been no changes in rendering after setting HorizontalOffset.");
                    }
                }

                if ((_offset == Double.MaxValue) && (_content == _largeContent))
                {
                    Verifier.Verify(_editControl.HorizontalOffset > _expHorizontalOffset,
                        "Verifying HorizontalOffset value after setting it:" +
                        " Actual [" + _editControl.HorizontalOffset +
                        " Expected [" + _expHorizontalOffset + "]", true);
                }
                else
                {
                    Verifier.Verify(_editControl.HorizontalOffset == _expHorizontalOffset,
                        "Verifying HorizontalOffset value after setting it:" +
                        " Actual [" + _editControl.HorizontalOffset +
                        " Expected [" + _expHorizontalOffset + "]", true);
                }
            }
            //restoring value for HorizontalOffset
            _editControl.ScrollToHorizontalOffset(_originalHorizontalOffset);

            //setting value for VerticalOffset
            _editControl.ScrollToVerticalOffset(_offset);

            QueueDelegate(AfterSettingVerticalOffset);
        }

        private void AfterSettingVerticalOffset()
        {
            _afterSetting = BitmapCapture.CreateBitmapFromElement(_editControl);

            //if _content == _shortContent, no rendering change is expected
            if ((_content == _largeContent) &&
                (_verticalSBVisibility != ScrollBarVisibility.Disabled))
            {
                //Verify that rendering is updated after setting HorizontalOffset.
                if (ComparisonOperationUtils.AreBitmapsEqual(_beforeSetting, _afterSetting, out _differences))
                {
                    Logger.Current.LogImage(_afterSetting, "AfterSetting");
                    throw new Exception("There have been no changes in rendering after setting VerticalOffset.");
                }
            }

            if ((_offset == Double.MaxValue) && (_content == _largeContent))
            {
                Verifier.Verify(_editControl.VerticalOffset > _expVerticalOffset,
                    "Verifying VerticalOffset value after setting it:" +
                    " Actual [" + _editControl.VerticalOffset +
                    " Expected [" + _expVerticalOffset + "]", true);
            }
            else
            {
                Verifier.Verify(_editControl.VerticalOffset == _expVerticalOffset,
                    "Verifying VerticalOffset value after setting it:" +
                    " Actual [" + _editControl.VerticalOffset +
                    " Expected [" + _expVerticalOffset + "]", true);
            }

            //restoring value for HorizontalOffset and VerticalOffset            
            _editControl.ScrollToVerticalOffset(_originalVerticalOffset);

            //setting values for both HorizontalOffset and VerticalOffset
            _editControl.ScrollToHorizontalOffset(_offset);
            _editControl.ScrollToVerticalOffset(_offset);

            QueueDelegate(AfterSettingBothOffsets);
        }

        private void AfterSettingBothOffsets()
        {
            _afterSetting = BitmapCapture.CreateBitmapFromElement(_editControl);

            //if _content == _shortContent, no rendering change is expected
            if ((_content == _largeContent) &&
                ((_horizontalSBVisibility != ScrollBarVisibility.Disabled) || (_verticalSBVisibility != ScrollBarVisibility.Disabled)))
            {
                //Verify that rendering is updated after setting HorizontalOffset.
                if (ComparisonOperationUtils.AreBitmapsEqual(_beforeSetting, _afterSetting, out _differences))
                {
                    Logger.Current.LogImage(_afterSetting, "AfterSetting");
                    throw new Exception("There have been no changes in rendering after setting VerticalOffset & HorizontalOffset.");
                }
            }

            if ((_offset == Double.MaxValue) && (_content == _largeContent))
            {
                if (_editControl is RichTextBox)
                {
                    Verifier.Verify(_editControl.HorizontalOffset == 0,
                        "Verifying HorizontalOffset value after setting both the offsets" +
                        " Actual [" + _editControl.HorizontalOffset +
                        " Expected [0]", true);
                }
                else
                {
                    Verifier.Verify(_editControl.HorizontalOffset > _expHorizontalOffset,
                        "Verifying HorizontalOffset value after setting both the offsets" +
                        " Actual [" + _editControl.HorizontalOffset +
                        " Expected [" + _expHorizontalOffset + "]", true);
                }
                Verifier.Verify(_editControl.VerticalOffset > _expVerticalOffset,
                    "Verifying VerticalOffset value after setting both the offsets" +
                    " Actual [" + _editControl.VerticalOffset +
                    " Expected [" + _expVerticalOffset + "]", true);
            }
            else
            {
                _expHorizontalOffset = (_editControl is RichTextBox) ? 0 : _expHorizontalOffset;
                Verifier.Verify(_editControl.HorizontalOffset == _expHorizontalOffset,
                    "Verifying HorizontalOffset value after setting both the offsets" +
                    " Actual [" + _editControl.HorizontalOffset +
                    " Expected [" + _expHorizontalOffset + "]", true);

                Verifier.Verify(_editControl.VerticalOffset == _expVerticalOffset,
                    "Verifying VerticalOffset value after setting both the offsets" +
                    " Actual [" + _editControl.VerticalOffset +
                    " Expected [" + _expVerticalOffset + "]", true);
            }
            QueueDelegate(NextCombination);
        }

        private void DoInvalidTesting()
        {
            Style originalStyle;
            originalStyle = _editControl.Style;

            //Invalid value
            //try
            //{
            //    _editControl.ScrollToHorizontalOffset(-1);
            //    throw new ApplicationException("HorizontalOffset accepts negative value");
            //}
            //catch (ArgumentException)
            //{
            //    Log("ArgumentException thrown as expected when negative value is assigned to HorizontalOffset property");
            //}

            ////Invalid value
            //try
            //{
            //    _editControl.ScrollToVerticalOffset(-1);
            //    throw new ApplicationException("VerticalOffset accepts negative value");
            //}
            //catch (ArgumentException)
            //{
            //    Log("ArgumentException thrown as expected when negative value is assigned to VerticalOffset property");
            //}

            ////Invalid value
            //try
            //{
            //    _editControl.ScrollToVerticalOffset(Double.NegativeInfinity);
            //    throw new ApplicationException("VerticalOffset accepts negative infinity");
            //}
            //catch (ArgumentException)
            //{
            //    Log("ArgumentException thrown as expected when negative infinity is assigned to VerticalOffset property");
            //}

            ////Invalid value
            //try
            //{
            //    _editControl.ScrollToVerticalOffset(Double.PositiveInfinity);
            //    throw new ApplicationException("VerticalOffset accepts positive infinity");
            //}
            //catch (ArgumentException)
            //{
            //    Log("ArgumentException thrown as expected when positive infinity is assigned to VerticalOffset property");
            //}

            //Invalid value
            //try
            //{
            //    _editControl.ScrollToVerticalOffset(Double.NaN);
            //    throw new ApplicationException("VerticalOffset accepts Double.NaN");
            //}
            //catch (ArgumentOutOfRangeException)
            //{
            //    Log("ArgumentOutOfRangeException thrown as expected when Double.NaN is assigned to VerticalOffset property");
            //} 

            //Style without a ScrollViewer
            //_editControl.Style = GetCustomStyle(_textEditableType.Type);
            //_editControl.ScrollToHorizontalOffset(5.0);
            //_editControl.ScrollToVerticalOffset(5.0);
            //Verifier.Verify(_editControl.HorizontalOffset == 5.0,
            //    "Verifying that value of HorizontalOffset with no scrollviewer", true);
            //Verifier.Verify(_editControl.VerticalOffset == 5.0,
            //    "Verifying that value of VerticalOffset with no scrollviewer", true);

            if (_editControl is TextBox)
            {
                _invalidTestingTBDone = true;
            }
            else if (_editControl is RichTextBox)
            {
                _invalidTestingRTBDone = true;
            }

            //restore state
            _editControl.Style = originalStyle;
            _editControl.ScrollToHorizontalOffset(_originalHorizontalOffset);
            _editControl.ScrollToVerticalOffset(_originalVerticalOffset);
        }

        private Style GetCustomStyle(Type editControlType)
        {
            ControlTemplate editControlTemplate;
            FrameworkElementFactory fef;
            Style editControlStyle;

            if ((editControlType != typeof(TextBox)) && (editControlType != typeof(RichTextBox)))
            {
                throw new ArgumentException("Invalid editControlType: Pass either TextBox or RichTextBox types");
            }

            editControlTemplate = new ControlTemplate(editControlType);
            if (editControlType == typeof(TextBox))
            {
                fef = new FrameworkElementFactory(typeof(TextBlock), "TextBoxContent");
            }
            else
            {
                fef = new FrameworkElementFactory(typeof(FrameworkElement).Assembly.GetType("TextFlow"), "TextBoxContent");
            }

            fef.SetValue(TextBoxBase.BackgroundProperty, System.Windows.Media.Brushes.LightBlue);
            editControlTemplate.VisualTree = fef;

            editControlStyle = new Style(editControlType);
            editControlStyle.Setters.Add(new Setter(TextBoxBase.TemplateProperty, editControlTemplate));

            return editControlStyle;
        }

        #endregion Main flow

        #region Private fields

        private TextEditableType _textEditableType;
        private ScrollBarVisibility _horizontalSBVisibility;
        private ScrollBarVisibility _verticalSBVisibility;
        private string _content;
        private const string _shortContent = "abc ";
        private string _largeContent;
        private double _offset;
        private double _originalHorizontalOffset, _originalVerticalOffset;
        private double _expHorizontalOffset, _expVerticalOffset;

        private TextBoxBase _editControl;
        private UIElementWrapper _editControlWrapper;

        private bool _invalidTestingTBDone = false;
        private bool _invalidTestingRTBDone = false;

        private Bitmap _beforeSetting, _afterSetting, _differences;

        #endregion Private fields
    }

    /// <summary>
    /// Explicitly tests the following members:
    /// LineDown
    /// LineUp
    /// LineRight
    /// LineLeft
    /// 
    /// Also Implicitly tests:
    /// GetFirstVisibleIndex (TextBox) //used in helper class
    /// GetLastVisibleIndex (TextBox)  //used in helper class
    /// Horizontal/vertical offset
    /// </summary>
    [Test(0, "TextBoxBase", "TextBoxBaseScrollFunctionsLineTest", MethodParameters = "/TestCaseType:TextBoxBaseScrollFunctionsLineTest", Timeout = 240)]
    [TestOwner("Microsoft"), TestTactics("563"), TestWorkItem("95 ,96")]
    public class TextBoxBaseScrollFunctionsLineTest : ManagedCombinatorialTestCase
    {
        /// <summary>initialization of the run</summary>
        /// <summary> filter for combinations read</summary>
        protected override bool DoReadCombination(Hashtable values)
        {
            bool result = base.DoReadCombination(values);
            if (_editableType == TextEditableType.GetByName("PasswordBox"))
                return false;
            //RichTextBox doesnt have TextWrapping property. It always wraps. Hence 
            //we only test for Wrap==true
            if ((_editableType == TextEditableType.GetByName("RichTextBox")) && (!_wrapText))
                return false;
            return true;
        }

        /// <summary>initialization of the run</summary>
        protected override void DoRunCombination()
        {
            _element = _editableType.CreateInstance();

            if (_element is PasswordBox)
            {
                NextCombination();
            }
            else
            {
                _controlWrapper = new UIElementWrapper(_element);
                if (_element is RichTextBox)
                {
                    ((RichTextBox)_element).Document.PageWidth = 600;
                }
                //setting the Control Properties
                _element.Height = 150;
                _element.Width = 200;
                ((TextBoxBase)_element).FontFamily = new System.Windows.Media.FontFamily("Tahoma");
                ((TextBoxBase)_element).FontSize = 11;

                _controlWrapper.Wrap = _wrapText ? true : false;
                ((TextBoxBase)_element).AcceptsReturn = _AcceptsReturn ? true : false;

                if (_largeMultiLineContent)
                {
                    string str = "";
                    for (int i = 0; i < 40; i++)
                    {
                        str += TextUtils.RepeatString(i.ToString() + "> sample data :P", 10) + "\r\n";
                    }
                    _controlWrapper.Text = str;
                }
                else if (_largeMultiLineContent == false)
                {
                    _controlWrapper.Text = "0>Sample data :)Sample data :)S a m p l e data";
                }

                if (_scrollVisible)
                {
                    ((TextBoxBase)_element).HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                    ((TextBoxBase)_element).VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                }
                _verticalOffset = 0;//sets cursor to (0,0)

                TestElement = _element;
                QueueDelegate(DoFocus);
            }
        }

        /// <summary>Focus on element</summary>
        private void DoFocus()
        {
            MouseInput.MouseClick(_element);
            QueueDelegate(StartCase);
        }

        /// <summary>Focus on element</summary>
        private void StartCase()
        {
            _initialNumber = 0;
            _finalNumber = 0;
            _element = (FrameworkElement)_controlWrapper.Element;
            if (_element is RichTextBox)
            {
                ((TextBoxBase)_element).Padding = new Thickness(0);
                ((TextBoxBase)_element).Margin = new Thickness(0);
            }
            if (_largeMultiLineContent)
            {
                QueueDelegate(InitializeVerticalLineOffsetValue);
            }
            else if ((_controlWrapper.Wrap == false) && (_largeMultiLineContent == false))
            {
                KeyboardInput.TypeString("^{HOME}");
                QueueDelegate(InitializeHorizontalLineOffsetValue);
            }
            else
            {
                KeyboardInput.TypeString("^{HOME}");
                QueueDelegate(ExecuteScrollAction);
            }
        }

        /// <summary>Program controller</summary>
        public void ExecuteScrollAction()
        {
            switch (_FunctionSwitch)
            {
                case FunctionName.LineDown:
                    {
                        if (_element is TextBox)
                        {
                            _verticalLineOffset = GetLineHeight();
                        }
                        QueueDelegate(LineDown);
                        break;
                    }

                case FunctionName.LineUp:
                    {
                        if (_element is TextBox)
                        {
                            _verticalLineOffset = GetLineHeight();
                        }
                        QueueDelegate(LineUp);
                        break;
                    }

                case FunctionName.LineRight:
                    {
                        LineRight();
                        break;
                    }

                case FunctionName.LineLeft:
                    {
                        if (_element is RichTextBox)
                        {
                            FlowDocument fd = ((RichTextBox)_element).Document;
                            fd.ClearValue(FlowDocument.PageWidthProperty);
                        }
                        SetCaret();
                        QueueDelegate(LineLeft);
                        break;
                    }

                default:
                    break;
            }
        }

        /// <summary>Base for setting vertical line offset values</summary>
        public void InitializeVerticalLineOffsetValue()
        {
            _initialOffset = (int)((TextBoxBase)_element).VerticalOffset;
            //included so that mouse clicks out of TEXTBOX
            //& prevent mouseClick on element when calculating vertical offset
            //current case does so because of racearoundconditions with queuedelegate
            //MouseInput.MouseClick(40, 40);
            //_element.Focus();
            ((TextBoxBase)_element).LineDown();
            QueueDelegate(SetVerticalLineOffset);
        }

        /// <summary>Initialize vertical line offset values</summary>
        public void SetVerticalLineOffset()
        {
            int _finalOffset = (int)((TextBoxBase)_element).VerticalOffset;
            _verticalLineOffset = (int)(_finalOffset - _initialOffset);
            KeyboardInput.TypeString("^{HOME}");
            QueueDelegate(InitializeHorizontalLineOffsetValue);
        }

        /// <summary>Base for seeting the horizontal line offset values</summary>
        public void InitializeHorizontalLineOffsetValue()
        {
            _initialOffset = (int)((TextBoxBase)_element).HorizontalOffset;
            ((TextBoxBase)_element).LineRight();
            QueueDelegate(SetHorizontalLineOffset);
        }

        /// <summary>Initialize the horizontal line offset values</summary>
        public void SetHorizontalLineOffset()
        {
            int _finalOffset = (int)((TextBoxBase)_element).HorizontalOffset;
            _horizontalLineOffset = (int)(_finalOffset - _initialOffset);
            QueueDelegate(ExecuteScrollAction);
        }

        /// <summary> sets cursor </summary>
        public void LineDown()
        {
            //_element.Focus();
            MouseInput.MouseClick(_element);
            KeyboardInput.TypeString("^{HOME}");
            QueueDelegate(LineDownOperation);
        }

        /// <summary> Performs Line Down operation </summary>
        private void LineDownOperation()
        {
            _initialImage = BitmapCapture.CreateBitmapFromElement(_element);
            _initialNumber = _controlWrapper.GetIndexOfFirstStringInWindow(0, out _YvalForFirstString);
            ((TextBoxBase)_element).LineDown();
            QueueDelegate(VerifyLineDown);
        }

        /// <summary> Verifies Line Down Operation </summary>
        public void VerifyLineDown()
        {
            //lines move upwards. so for RTB it is necessary to skip the line that goes up
            //Hence PREV value of Y is used
            _finalImage = BitmapCapture.CreateBitmapFromElement(_element);

            _finalNumber = _controlWrapper.GetIndexOfFirstStringInWindow(_YvalForFirstString, out _YvalForFirstString);
            if (_largeMultiLineContent)
            {
                Verifier.Verify(ComparisonOperationUtils.AreBitmapsEqual(_initialImage, _finalImage, out _differenceImage) == false, "Images are same. They should be diff because of 2 LineDown calls", false);
                Verifier.Verify((int)(((TextBoxBase)_element).VerticalOffset) == (_verticalLineOffset), "Vertical Offset is NOT == the number of LineDown invocations Expected [" +
                    _verticalLineOffset.ToString() + "] actual[" + ((TextBoxBase)_element).VerticalOffset.ToString() + "]", false);
                //1 LINE DOWN
                VerifyLineDownTBandRTB(1);
            }
            else
            {
                //Doesnt work for RichTextBox (unstable for some reason - maybe it moves a little)
                //Verifier.Verify(ComparisonOperationUtils.AreBitmapsEqual(_initialImage, _finalImage, out _differenceImage)==true, "Images are NOT same - There is nothing to scroll", false);
                Verifier.Verify((int)(((TextBoxBase)_element).VerticalOffset) == 0, "Vertical Offset is NOT == the number of LineDown invocations Expected [" +
                    _verticalLineOffset.ToString() + "] actual[" + ((TextBoxBase)_element).VerticalOffset.ToString() + "]", false);
                //1 LINE DOWN
                VerifyLineDownTBandRTB(0);
            }
        }

        /// <summary>Verifies Line Numbers on Line Down operation </summary>
        /// <param name="num">this is the number of lines moved down</param>
        public void VerifyLineDownTBandRTB(int num)
        {
            int _linesScrolled = (_finalNumber - _initialNumber);
            if (_controlWrapper.Wrap == false)
            {
                //RichTextBox has padding inbetween lines and this can cause problems is determinig line numbers 
                //if ((_element is RichTextBox) == false)
                {
                    Verifier.Verify(_finalNumber == _initialNumber + num, _element.GetType().Name + " Scrolled down [" +
                            _linesScrolled.ToString() + "] lines. Expected Count [1] \r\n. Initial Line Number [" + _initialNumber.ToString() +
                            "] Final Line Number [" + _finalNumber.ToString() + "] \r\n", false);
                }
            }
            else
            {   // if the text is scrollable (>1 line) then linedown is called 1 times...so it has to be <2
                //but for RTB the wrap makes it lose the margin resulting in 2 lines down
                Verifier.Verify(_finalNumber <= 2, _element.GetType().Name +
                                " Scrolled down [" + _linesScrolled.ToString() + "] lines. Expected Count [1] \r\n", false);
            }
            NextCombination();
        }

        /// <summary> sets cursor </summary>
        public void LineUp()
        {
            //((TextBoxBase)_element).LineDown();
            //((TextBoxBase)_element).LineDown();
            KeyboardInput.TypeString("{PGDN}{PGDN}");
            QueueDelegate(LineUpOperation);
        }

        /// <summary> Performs Line Up operation </summary>
        public void LineUpOperation()
        {
            _initialImage = BitmapCapture.CreateBitmapFromElement(_element);
            _initialNumber = _controlWrapper.GetIndexOfFirstStringInWindow(0, out _YvalForFirstString);
            _verticalOffset = ((TextBoxBase)_element).VerticalOffset;
            ((TextBoxBase)_element).LineUp();
            QueueDelegate(VerifyLineUp);
        }

        /// <summary> Verifies Line Up Operation </summary>
        public void VerifyLineUp()
        {
            //Line moves down from the top. So we need to find the first string. so use default Y value
            _finalImage = BitmapCapture.CreateBitmapFromElement(_element);
            _finalNumber = _controlWrapper.GetIndexOfFirstStringInWindow(0, out _YvalForFirstString);
            if (_largeMultiLineContent)
            {
                Verifier.Verify(ComparisonOperationUtils.AreBitmapsEqual(_initialImage, _finalImage, out _differenceImage) == false, "Images are same. They should be diff because of 2 LineUp calls", false);

                int _actualOffset = (int)(((TextBoxBase)_element).VerticalOffset);
                int _initialOffset = (int)(_verticalOffset);
                Log("current offset +" + _actualOffset.ToString() + " iNITIAL OFFSET=" + _initialOffset.ToString() + "LINE HEIGHT=" + _verticalLineOffset.ToString());
                if (_element is TextBox)
                {
                    int CurrentLineHeight = _initialOffset - _actualOffset;
                    int calculatedLineHeight = (int)(_verticalLineOffset);
                    Verifier.Verify(calculatedLineHeight <= CurrentLineHeight + 1 || calculatedLineHeight >= CurrentLineHeight - 1,
                        "Vertical Offset is NOT equal to the # of LineUp invocations Expected[" + CurrentLineHeight.ToString() +
                        "+-1] Actual [" + calculatedLineHeight.ToString() + "+-1]", false);
                }
                else
                {
                    Verifier.Verify(_actualOffset == (_initialOffset - (int)(_verticalLineOffset)), "Vertical Offset is NOT equal to the # of LineUp invocations", false);
                }

                //1 LINE up
                VerifyLineUpTBandRTB(1);
            }
            else
            {
                //Doesnt work for RichTextBox (unstable for some reason)
                //Verifier.Verify(ComparisonOperationUtils.AreBitmapsEqual(_initialImage, _finalImage, out _differenceImage)==true, "Images are NOT same - There is nothing to scroll", false);
                Verifier.Verify((int)(((TextBoxBase)_element).VerticalOffset) == 0, "Vertical Offset is twice the number of LineDown invocations", false);
                //1 LINE UP
                VerifyLineUpTBandRTB(0);
            }
        }

        /// <summary>Verifies Line Numbers on Line Up operation </summary>
        public void VerifyLineUpTBandRTB(int num)
        {
            int _linesScrolled = (_initialNumber - _finalNumber);
            if (_controlWrapper.Wrap == false)
            {
                Verifier.Verify(_finalNumber == _initialNumber - num, _element.GetType().Name + " Scrolled up [" +
                    _linesScrolled.ToString() + "] lines. Expected Count [2] \r\n", false);
            }
            else
            {
                //if the text is long and occupies more than 2 lines
                if (_element is TextBox)
                {
                    Verifier.Verify(((_finalNumber == 0) || (_finalNumber == _initialNumber - num)), _element.GetType().Name + " Scrolled up [" + _linesScrolled.ToString() + "] lines. Expected Count [0 / 1]  \r\n", false);
                }
                else if (_element is RichTextBox)
                {
                    //the aditional line for RTB is because of wrap
                    //normally when lineUp is performed margin is considered too....
                    //because of wrap this margin is lost.. so oneline up results in 2 lines up
                    Verifier.Verify(((_finalNumber - _initialNumber == 0) || (_linesScrolled <= 2)), _element.GetType().Name + " Scrolled up [" + _linesScrolled.ToString() + "] lines. Expected Count [0 / 2]  \r\n", false);
                }
            }
            NextCombination();
        }

        /// <summary> sets cursor </summary>
        public void LineRight()
        {
            KeyboardInput.TypeString("^{HOME}");
            ((TextBoxBase)_element).ScrollToHorizontalOffset(0); //just to be sure
            QueueDelegate(LineRightOperation);
        }

        /// <summary> Performs Line Right operation </summary>
        public void LineRightOperation()
        {
            _initialImage = BitmapCapture.CreateBitmapFromElement(_element);
            _initialString = _controlWrapper.GetFirstStringInWindow(0, out _YvalForFirstString);
            _initialOffset = ((TextBoxBase)_element).HorizontalOffset;
            ((TextBoxBase)_element).LineRight();
            ((TextBoxBase)_element).LineRight();
            ((TextBoxBase)_element).LineRight();

            QueueDelegate(VerifyLineRight);
        }

        /// <summary> Verifies Line Right Operation </summary>
        private void VerifyLineRight()
        {
            _finalImage = BitmapCapture.CreateBitmapFromElement(_element);
            // 0 is passed because u want the first string
            _finalString = _controlWrapper.GetFirstStringInWindow(0, out _YvalForFirstString);
            _finalOffset = ((TextBoxBase)_element).HorizontalOffset;

            if ((_controlWrapper.Wrap == false) || (_element is RichTextBox))
            {
                Verifier.Verify(ComparisonOperationUtils.AreBitmapsEqual(_initialImage, _finalImage, out _differenceImage) == false,
                                "Images are same. They should be diff because of LineRight calls", false);

                Verifier.Verify(((int)(_finalOffset - _initialOffset) == ((int)(_horizontalLineOffset) * 3)),
                                "Horizontal Offset is NOT equal to the # of LineRight invocations", false);
            }
            else if (_controlWrapper.Wrap == true)
            {
                // this is unstable               
                //  Verifier.Verify(ComparisonOperationUtils.AreBitmapsEqual(_initialImage, _finalImage, out _differenceImage) == false, "Images are different. They should be same because theres no text wrap", false);
                Verifier.Verify(_finalOffset == 0, "Offset should be 0. cant move right", false);
            }
            VerifyLineRightTBandRTB();
        }

        /// <summary>Verifies Line Numbers on Line Right operation </summary>
        public void VerifyLineRightTBandRTB()
        {
            if (_element is TextBox)
            {
                //equals operation because GetFirstStringInWindow returns complete string for textbox, NOT from pointer
                Verifier.Verify(_initialString.Equals(_finalString) == true, "ScrollViewer didnt retain focus on the string. Must have moved wrongly", false);
            }
            else if (_element is RichTextBox)
            {
                Verifier.Verify(_initialString.Length > _finalString.Length, "ScrollViewer didnt move right as expected", false);
            }
            NextCombination();
        }

        /// <summary> sets cursor </summary>
        public void LineLeft()
        {
            //((TextBoxBase)_element).HorizontalOffset=0;
            KeyboardInput.TypeString("{END}");
            QueueDelegate(LineLeftOperation);
        }

        /// <summary> Performs Line Left operation </summary>
        private void LineLeftOperation()
        {
            _initialOffset = ((TextBoxBase)_element).HorizontalOffset;
            _initialImage = BitmapCapture.CreateBitmapFromElement(_element);
            _initialString = _controlWrapper.GetFirstStringInWindow(0, out _YvalForFirstString);

            ((TextBoxBase)_element).LineLeft();
            ((TextBoxBase)_element).LineLeft();
            //((TextBoxBase)_element).LineLeft();
            QueueDelegate(VerifyLineLeft);
        }

        /// <summary> Verifies Line Left Operation </summary>
        private void VerifyLineLeft()
        {
            _finalImage = BitmapCapture.CreateBitmapFromElement(_element);
            // 0 is passed because u want the first string
            _finalString = _controlWrapper.GetFirstStringInWindow(0, out _YvalForFirstString);
            _finalOffset = ((TextBoxBase)_element).HorizontalOffset;

            if ((_controlWrapper.Wrap == false) && (_element is TextBox))
            {
                Verifier.Verify(ComparisonOperationUtils.AreBitmapsEqual(_initialImage, _finalImage, out _differenceImage) == false, "Images are same. They should be diff because of LineLeft calls", false);

                Verifier.Verify(((int)(_initialOffset) - (int)(_finalOffset) == ((int)(_horizontalLineOffset) * 2)), "Horizontal Offset is NOT equal to the # of LineLeft invocations", false);

            }
            else
            {
                //unstable here
                //Verifier.Verify(ComparisonOperationUtils.AreBitmapsEqual(_initialImage, _finalImage, out _differenceImage) == false, "Images are different. They should be same because theres no text wrap", false);
                Verifier.Verify(_finalOffset == 0, "Offset should be 0. cant move left", false);
            }

            VerifyLineLeftTBandRTB();
        }

        /// <summary>Verifies Line Numbers on Line Left operation </summary>
        public void VerifyLineLeftTBandRTB()
        {
            if (_element is TextBox)
            {
                Verifier.Verify(_initialString.Equals(_finalString) == true, "ScrollViewer didnt retain focus on the string. Must have moved wrongly", false);
            }
            else if (_element is RichTextBox)
            {
                Verifier.Verify(_initialString.Length == _finalString.Length, "ScrollViewer sholdnt move right since text wraps", false);
            }
            NextCombination();
        }

        #region Helper Functions

        /// <summary>Gets Line Height </summary>
        private int GetLineHeight()
        {
            System.Windows.Media.FontFamily fontFamily = (System.Windows.Media.FontFamily)_element.GetValue(TextElement.FontFamilyProperty);
            double fontSize = (double)_element.GetValue(TextElement.FontSizeProperty);
            int height = (int)(fontFamily.LineSpacing * fontSize);
            return height;
        }

        /// <summary>Sets the caret to (0,0) </summary>
        public void SetCaret()
        {
            if (_element is TextBox)
            {
                ((TextBox)_element).Select(0, 0);
            }
            else
            {
                ((RichTextBox)_element).Selection.Select(_controlWrapper.SelectionInstance.Start, _controlWrapper.SelectionInstance.Start);
            }
            //   QueueDelegate(InitializeVerticalOffsetValue);
        }

        #endregion Helper Functions

        #region private data.

        private TextEditableType _editableType = null;
        private UIElementWrapper _controlWrapper;
        private bool _AcceptsReturn = false;
        private bool _largeMultiLineContent = false;
        private bool _wrapText = false;
        private bool _scrollVisible = true;
        private double _verticalOffset;
        private Bitmap _initialImage;
        private Bitmap _finalImage;
        private Bitmap _differenceImage;

        private double _YvalForFirstString;
        private FunctionName _FunctionSwitch = 0;

        private int _initialNumber;
        private int _finalNumber;

        private string _initialString = null;
        private string _finalString = null;

        private double _initialOffset;
        private double _finalOffset;

        private double _horizontalLineOffset;
        private double _verticalLineOffset;

        private FrameworkElement _element;

        #endregion private data.
    }

    /// <summary>Tests boundary cases of all scroll fuctions, and offset values </summary>
    [Test(2, "TextBoxBase", "TextBoxBaseScrollFunctionsBoundaryTest", MethodParameters = "/TestCaseType:TextBoxBaseScrollFunctionsBoundaryTest", Timeout=120)]
    [TestOwner("Microsoft"), TestTactics("562"), TestWorkItem("95 ,96")]
    public class TextBoxBaseScrollFunctionsBoundaryTest : ManagedCombinatorialTestCase
    {
        /// <summary>Starts the combinatorial engine </summary>
        protected override void DoRunCombination()
        {
            _element = _editableType.CreateInstance();
            if (_element is PasswordBox)
            {
                NextCombination();
            }
            else
            {
                _controlWrapper = new UIElementWrapper(_element);
                _controlWrapper.Text = "Sample data";
                _element.Height = 150;
                _element.Width = 200;

                TestElement = _element;
                QueueDelegate(DoFocus);
            }
        }

        private void SetText()
        {
            string str = "";
            for (int i = 0; i < 40; i++)
            {
                str += TextUtils.RepeatString(i.ToString() + "> sample data :P", 10) + "\r\n";
            }
            _controlWrapper.Text = str;
        }

        /// <summary>Focus on element</summary>
        private void DoFocus()
        {
            MouseInput.MouseClick(_element);
            QueueDelegate(LineDown);
        }

        /// <summary>Line Down boundary case</summary>
        private void LineDown()
        {
            _controlWrapper.Text = "Sample data";
            ((TextBoxBase)_element).LineDown();
            QueueDelegate(VerifyLineDown);
        }

        /// <summary> Verify Line Down boundary case</summary>
        private void VerifyLineDown()
        {
            Verifier.Verify(((TextBoxBase)_element).VerticalOffset == 0, "Focus has changed. Only one line of text. Cannot move up", true);
            QueueDelegate(LineUp);
        }

        /// <summary>Line Up boundary case</summary>
        private void LineUp()
        {
            ((TextBoxBase)_element).LineUp();
            QueueDelegate(VerifyLineUp);
        }

        /// <summary> Verify Line Up boundary case</summary>
        private void VerifyLineUp()
        {
            Verifier.Verify(((TextBoxBase)_element).VerticalOffset == 0, "Focus has changed. Only one line of text. Cannot move down", true);
            QueueDelegate(LineLeft);
        }

        /// <summary>Line Left boundary case</summary>
        private void LineLeft()
        {
            ((TextBoxBase)_element).LineLeft();
            QueueDelegate(VerifyLineLeft);
        }

        /// <summary> Verify Line Left boundary case</summary>
        private void VerifyLineLeft()
        {
            Verifier.Verify(((TextBoxBase)_element).HorizontalOffset == 0, "Line moved left. Cant do so. Cursor at (0,0)", true);
            KeyboardInput.TypeString("{END}");
            QueueDelegate(LineRight);
        }

        /// <summary>Line Right boundary case</summary>
        private void LineRight()
        {
            _initialOffset = (int)((TextBoxBase)_element).HorizontalOffset;
            ((TextBoxBase)_element).LineRight();
            QueueDelegate(VerifyLineRight);
        }

        /// <summary> Verify Line Right boundary case</summary>
        private void VerifyLineRight()
        {
            int _currentOffset = (int)((TextBoxBase)_element).HorizontalOffset;
            Verifier.Verify(_initialOffset == _currentOffset, "Line moved right. Cant do so. Already at end of line", true);
            _controlWrapper.Text = TextUtils.RepeatString("hello world \r\n", 40);
            KeyboardInput.TypeString("^{HOME}");
            QueueDelegate(PageUp);
        }

        /// <summary>Page Up boundary case</summary>
        private void PageUp()
        {
            ((TextBoxBase)_element).ScrollToVerticalOffset(0);
            ((TextBoxBase)_element).PageUp();
            QueueDelegate(VerifyPageUp);
        }

        /// <summary> Verify Page Up boundary case</summary>
        private void VerifyPageUp()
        {
            Verifier.Verify(((TextBoxBase)_element).VerticalOffset == 0, "Cannot perform PageUp. Cant do so. Already at position (0,0)", true);
            KeyboardInput.TypeString("^{END}");
            QueueDelegate(PageDown);
        }

        /// <summary>Page Down boundary case</summary>
        private void PageDown()
        {
            _initialOffset = (int)(((TextBoxBase)_element).VerticalOffset);
            ((TextBoxBase)_element).PageDown();
            QueueDelegate(VerifyPageDown);
        }

        /// <summary> Verify Page Up boundary case</summary>
        private void VerifyPageDown()
        {
            Verifier.Verify((int)(((TextBoxBase)_element).VerticalOffset) == _initialOffset, "Cannot perform PageDown. Cant do so. Already at position (0,0)", true);
            KeyboardInput.TypeString("{HOME}");
            QueueDelegate(PageLeft);
        }

        /// <summary>Page Left boundary case</summary>
        private void PageLeft()
        {
            _initialOffset = (int)(((TextBoxBase)_element).HorizontalOffset);
            ((TextBoxBase)_element).PageLeft();
            QueueDelegate(VerifyPageLeft);
        }

        /// <summary> Verify Page Left boundary case</summary>
        private void VerifyPageLeft()
        {
            Verifier.Verify(((TextBoxBase)_element).HorizontalOffset == _initialOffset, "Cannot perform PageDown. Cant do so. Already at position (0,0)", true);
            KeyboardInput.TypeString("{END}");
            QueueDelegate(PageRight);
        }

        /// <summary>Page Right boundary case</summary>
        private void PageRight()
        {
            ((TextBoxBase)_element).PageRight();
            QueueDelegate(VerifyPageRight);
        }

        /// <summary> Verify Page Right boundary case</summary>
        private void VerifyPageRight()
        {
            Verifier.Verify(((TextBoxBase)_element).HorizontalOffset == 0, "Cannot perform PageRight. Cant do so. Already at end of line", true);
            KeyboardInput.TypeString("^{HOME}");
            ((TextBoxBase)_element).ScrollToHorizontalOffset(0);
            QueueDelegate(ScrollHome);
        }

        /// <summary>Scroll to Home boundary case</summary>
        private void ScrollHome()
        {
            ((TextBoxBase)_element).ScrollToHome();
            QueueDelegate(CheckScrollHome);
        }

        /// <summary> Verify Scroll Home boundary case</summary>
        private void CheckScrollHome()
        {
            Verifier.Verify((((TextBoxBase)_element).VerticalOffset == 0) && (((TextBoxBase)_element).HorizontalOffset == 0), "Cannot perform ScrollHome. Cant do so. Already at (0,0)", true);
            KeyboardInput.TypeString("^{END}");
            QueueDelegate(ScrollEnd);
        }

        /// <summary>Scroll to End boundary case</summary>
        private void ScrollEnd()
        {
            _initialOffset = (int)((TextBoxBase)_element).VerticalOffset;
            ((TextBoxBase)_element).ScrollToEnd();
            QueueDelegate(CheckScrollEnd);
        }

        /// <summary> Verify Scroll End boundary case</summary>
        private void CheckScrollEnd()
        {
            Verifier.Verify(((int)((TextBoxBase)_element).VerticalOffset == _initialOffset) && (((TextBoxBase)_element).HorizontalOffset == 0), "Cannot perform ScrollEnd. Cant do so. Already at end", true);
            KeyboardInput.TypeString("^{HOME}");
            QueueDelegate(FirstVisibleIndexTB);
        }

        /// <summary>FirstVisibleIndex boundary case</summary>
        private void FirstVisibleIndexTB()
        {
            if (_element is TextBox)
            {
                Verifier.Verify(((TextBox)_element).GetFirstVisibleLineIndex() == 0, "Got the wrong line index. Should be 0", true);
            }
            KeyboardInput.TypeString("^{END}");
            QueueDelegate(LastVisibleIndexTB);
        }

        /// <summary>LastVisibleIndex boundary case</summary>
        private void LastVisibleIndexTB()
        {
            if (_element is TextBox)
            {
                //40 because there are 41 strings . last string is \r\n ("hello world \r\n")
                Verifier.Verify(((TextBox)_element).GetLastVisibleLineIndex() == 40, "Got the wrong line index. Should be 40 Actual[" +
                    ((TextBox)_element).GetLastVisibleLineIndex().ToString() + "]", true);

                ((TextBox)_element).TextWrapping = TextWrapping.NoWrap;
            }

            SetText();
            QueueDelegate(HorVertOffsets);
        }

        /// <summary>Offsets boundary case</summary>
        private void HorVertOffsets()
        {

            ((TextBoxBase)_element).ScrollToVerticalOffset(Double.PositiveInfinity);

            QueueDelegate(VerifyScrollToPositiveInfinity);
        }

        private void VerifyScrollToPositiveInfinity()
        {
            Verifier.Verify(((TextBoxBase)_element).VerticalOffset > 0, " EXPECTED >0 ACTUAL [" + ((TextBoxBase)_element).VerticalOffset +
                "]", true);

            ((TextBoxBase)_element).ScrollToVerticalOffset(Double.NegativeInfinity);
            QueueDelegate(ScrollToNegativeInfinity);
        }

        private void ScrollToNegativeInfinity()
        {
            Verifier.Verify(((TextBoxBase)_element).VerticalOffset == 0, " EXPECTED =0 ACTUAL [" + ((TextBoxBase)_element).VerticalOffset +
                "]", true);

            ((TextBoxBase)_element).ScrollToHorizontalOffset(Double.NegativeInfinity);
            QueueDelegate(VerifyScrollHorizontalToNegativeInfinity);
        }

        private void VerifyScrollHorizontalToNegativeInfinity()
        {
            Verifier.Verify(((TextBoxBase)_element).HorizontalOffset == 0, "EXPECTED 0 ACTUAL [" + ((TextBoxBase)_element).HorizontalOffset +
                                        "]", true);

            ((TextBoxBase)_element).ScrollToHorizontalOffset(Double.PositiveInfinity);
            QueueDelegate(VerifyScrollHorizontalToPositiveInfinity);
        }

        private void VerifyScrollHorizontalToPositiveInfinity()
        {
            if (_element is TextBox)
            {
                Verifier.Verify(((TextBoxBase)_element).HorizontalOffset > 0, "EXPECTED >0 ACTUAL [" + ((TextBoxBase)_element).HorizontalOffset +
                                "]", true);
            }
            else
            {
                Verifier.Verify(((TextBoxBase)_element).HorizontalOffset == 0, "EXPECTED [0] ACTUAL [" + ((TextBoxBase)_element).HorizontalOffset +
                                "]", true);
            }
            try
            {
                ((TextBoxBase)_element).ScrollToVerticalOffset(Double.NaN);
                ((TextBoxBase)_element).UpdateLayout();
                throw new ApplicationException("VerticalOffset accepts Double.NaN");
            }
            catch (ArgumentOutOfRangeException)
            {
                Log("Exception ArgumentOutOfRangeException thrown when passed NaN");
            }
            try
            {
                ((TextBoxBase)_element).ScrollToHorizontalOffset(Double.NaN);
                ((TextBoxBase)_element).UpdateLayout();
                throw new ApplicationException("HorizontalOffset accepts Double.NaN");
            }
            catch (ArgumentOutOfRangeException)
            {
                Log("Exception ArgumentOutOfRangeException thrown when passed NaN");
            }
            NextCombination();
        }

        #region private data.

        private int _initialOffset;
        private FrameworkElement _element;
        private TextEditableType _editableType = null;
        private UIElementWrapper _controlWrapper;

        #endregion private data.
    }

    /// <summary>
    /// Explicitly tests the following members:
    /// PageDown
    /// PageUp
    /// PageRight
    /// PageLeft
    /// 
    /// Also Implicitly tests:
    /// GetFirstVisibleIndex (TextBox) //used in helper class
    /// GetLastVisibleIndex (TextBox)  //used in helper class
    /// Horizontal/vertical offset
    /// </summary>
    [Test(0, "TextBoxBase", "TextBoxBaseScrollFunctionsPageTest", MethodParameters = "/TestCaseType:TextBoxBaseScrollFunctionsPageTest", Timeout = 400)]
    [TestOwner("Microsoft"), TestTactics("561"), TestWorkItem("95 ,96"), TestLastUpdatedOn("July 24, 2006")]
    public class TextBoxBaseScrollFunctionsPageTest : ManagedCombinatorialTestCase
    {
        /// <summary>initialization of the run</summary>
        /// <summary> filter for combinations read</summary>
        protected override bool DoReadCombination(Hashtable values)
        {
            bool result = base.DoReadCombination(values);
            if (_editableType == TextEditableType.GetByName("PasswordBox"))
                return false;
            return true;
        }

        /// <summary>initialization of the run</summary>
        protected override void DoRunCombination()
        {
            _element = _editableType.CreateInstance();

            if ((_element is PasswordBox) || ((_element is RichTextBox) && (_wrapText == false)))
            {
                NextCombination();
            }
            else
            {
                _controlWrapper = new UIElementWrapper(_element);

                if (_element is RichTextBox)
                {
                    ((RichTextBox)_element).Document.PageWidth = 600;
                }

                //setting the Control Properties
                _element.Height = 150;
                _element.Width = 200;
                ((TextBoxBase)_element).FontFamily = new System.Windows.Media.FontFamily("Tahoma");
                ((TextBoxBase)_element).FontSize = 11;

                _controlWrapper.Wrap = _wrapText ? true : false;
                ((TextBoxBase)_element).AcceptsReturn = _AcceptsReturn ? true : false;

                if (_largeMultiLineContent)
                {
                    string str = "";
                    for (int i = 0; i < 40; i++)
                    {
                        str += TextUtils.RepeatString(i.ToString() + "> sample data :P", 10) + "\r\n";
                    }
                    _controlWrapper.Text = str;
                }
                else if (_largeMultiLineContent == false)
                {
                    _controlWrapper.Text = "0>Sample data :)Sample data :)S a m p l e data";
                }

                if (_scrollVisible)
                {
                    ((TextBoxBase)_element).HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                    ((TextBoxBase)_element).VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                }
                TestElement = _element;
                QueueDelegate(DoFocus);
            }
        }

        /// <summary>Focus on element</summary>
        private void DoFocus()
        {
            MouseInput.MouseClick(_element);
            QueueDelegate(StartCase);
        }

        /// <summary>starts the case </summary>
        public void StartCase()
        {
            _element = (FrameworkElement)_controlWrapper.Element;
            _prevLastLine = 0;
            _currLine = 0;
            _iterCount = 0;

            if (_largeMultiLineContent)
            {
                QueueDelegate(InitializeVerticalPageOffsetValue);
            }
            else if ((_controlWrapper.Wrap == false) && (_largeMultiLineContent == false))
            {
                QueueDelegate(InitializeHorizontalPageOffsetValue);
            }
            else
            {
                QueueDelegate(ExecuteScrollAction);
            }
        }

        /// <summary>Program Controller </summary>
        public void ExecuteScrollAction()
        {
            switch (_FunctionSwitch)
            {
                case PageFunctionName.PageDown:
                    {
                        PageDown();
                        break;
                    }

                case PageFunctionName.PageDownInterestingCase:
                    {
                        InitializeElement();
                        _largeMultiLineContent = true;
                        //done because offsets maynot be initialised if initial combination is not multiline
                        if (_iterCount++ == 0)
                        {
                            MouseInput.MouseClick(_element);
                            KeyboardInput.TypeString("^{HOME}");
                            QueueDelegate(InitializeVerticalPageOffsetValue);
                        }
                        else
                        {
                            QueueDelegate(PageDown);
                        }
                        break;
                    }

                case PageFunctionName.PageUp:
                    {
                        PageUp();
                        break;
                    }

                case PageFunctionName.PageUpInterestingCase:
                    {
                        InitializeElement();
                        _largeMultiLineContent = true;
                        //done because offsets maynot be initialised if initial combination is not multiline
                        MouseInput.MouseClick(_element);
                        if (_iterCount++ == 0)
                        {
                            KeyboardInput.TypeString("^{HOME}");
                            QueueDelegate(InitializeVerticalPageOffsetValue);
                        }
                        else
                        {
                            KeyboardInput.TypeString("^{HOME}");
                            ((TextBoxBase)_element).ScrollToVerticalOffset(0);
                            QueueDelegate(PageUp);
                        }
                        break;
                    }

                case PageFunctionName.PageRight:
                    {
                        PageRight();
                        break;
                    }

                case PageFunctionName.PageLeft:
                    {

                        PageLeft();
                        break;
                    }

                case PageFunctionName.PageHome:
                    {
                        KeyboardInput.TypeString("^{END}");
                        QueueDelegate(PageHome);
                        break;
                    }

                case PageFunctionName.PageEnd:
                    {
                        ((TextBoxBase)_element).ScrollToHorizontalOffset(0);
                        ((TextBoxBase)_element).ScrollToVerticalOffset(0);
                        QueueDelegate(PageEnd);
                        break;
                    }

                default:
                    break;
            }
        }

        /// <summary>Base for setting Vertical Page offset value </summary>
        public void InitializeVerticalPageOffsetValue()
        {
            _initialOffset = (int)((TextBoxBase)_element).VerticalOffset;
            // ((TextBoxBase)_element).PageDown();
            ((TextBoxBase)_element).Focus();
            KeyboardInput.TypeString("{PGDN}");
            QueueDelegate(SetVerticalPageOffset);
        }

        /// <summary>Initialize Vertical Page offset value </summary>
        public void SetVerticalPageOffset()
        {
            _finalOffset = (int)((TextBoxBase)_element).VerticalOffset;
            _verticalPageOffset = (int)(_finalOffset - _initialOffset);

            ((TextBoxBase)_element).ScrollToHorizontalOffset(0);
            ((TextBoxBase)_element).ScrollToVerticalOffset(0);
            QueueDelegate(InitializeHorizontalPageOffsetValue);
        }

        /// <summary>Base for setting Horizontal Page offset value </summary>
        public void InitializeHorizontalPageOffsetValue()
        {
            _initialOffset = (int)((TextBoxBase)_element).HorizontalOffset;
            ((TextBoxBase)_element).PageRight();
            QueueDelegate(SetHorizontalPageOffset);
        }

        /// <summary>Initialize Horizontal Page offset value </summary>
        public void SetHorizontalPageOffset()
        {
            int _finalOffset = (int)((TextBoxBase)_element).HorizontalOffset;
            _horizontalPageOffset = (int)(_finalOffset - _initialOffset);
            QueueDelegate(ExecuteScrollAction);
        }

        /// <summary>Sets Cursor </summary>
        private void PageDown()
        {
            SetCaret();
            ((TextBoxBase)_element).ScrollToVerticalOffset(0);
            ((TextBoxBase)_element).ScrollToHorizontalOffset(0);
            QueueDelegate(PageDownOperation);
        }

        /// <summary>Page Down Operation </summary>
        private void PageDownOperation()
        {
            _initialImage = BitmapCapture.CreateBitmapFromElement(_element);
            _prevLastLine = _controlWrapper.GetIndexOfLastStringInWindow(0, out _YvalForLastString);
            Log("******************Make 2 PageDown Calls ******************\r\n");
            ((TextBoxBase)_element).PageDown();
            ((TextBoxBase)_element).PageDown();
            QueueDelegate(VerifyPageDown);
        }

        /// <summary>Verify Page Down Operation </summary>
        private void VerifyPageDown()
        {
            _finalImage = BitmapCapture.CreateBitmapFromElement(_element);
            //used a value of 20 for the y offset so that the snap doesnt occur to the prev element 
            _currLine = _controlWrapper.GetIndexOfFirstStringInWindow(20, out  _YvalForFirstString);

            if (_largeMultiLineContent)
            {
                ComparisonCriteria _criteria = new ComparisonCriteria();
                _criteria.MaxColorDistance = 0.01f;
                Verifier.Verify(ComparisonOperationUtils.AreBitmapsEqualUsingCriteria(_initialImage, _finalImage, _criteria, false) == false, "Images are same. They should be diff because of 2 PageDown calls", false);
                if (_FunctionSwitch == PageFunctionName.PageDownInterestingCase)
                {
                    Verifier.Verify((int)(((TextBoxBase)_element).VerticalOffset) <= (2 * _verticalPageOffset), "Vertical Page Offset on PageDown invocations " +
                        "Expected <=2*[" + _verticalPageOffset.ToString() + "] Actual [" + ((TextBoxBase)_element).VerticalOffset.ToString() + "]", true);
                }
                else
                {
                    bool val = (((int)(((TextBoxBase)_element).VerticalOffset) <= (2 * _verticalPageOffset) + 16) && ((int)(((TextBoxBase)_element).VerticalOffset) > ((2 * _verticalPageOffset) - 20)));
                    Verifier.Verify(val, "Vertical Page Offset on PageDown invocations " +
                        "Expected 2*[" + _verticalPageOffset.ToString() + "] Tolerance [-20/+16] Actual [" + ((TextBoxBase)_element).VerticalOffset.ToString() + "]", true);
                }
            }
            else
            {
                //small content is not more than a page
                Verifier.Verify((int)(((TextBoxBase)_element).VerticalOffset) == 0, "Vertical Offset is NOT 0", false);
            }
            VerifyPageDownTBandRTB();
        }

        /// <summary>Verify Page Down Operation - Check offsets and Line numbers </summary>
        private void VerifyPageDownTBandRTB()
        {
            if (_controlWrapper.Wrap == false)
            {
                bool val = ((_currLine > (_prevLastLine * 2 - 5)) && (_currLine < (_prevLastLine * 2 + 2)));
                Verifier.Verify(val, _element.GetType().Name + " Scrolled down to line #[" +
                        _currLine.ToString() + "] line. Expected <[2 * " + _prevLastLine.ToString() + " +2-5]  \r\n", true);
            }
            else
            {
                // because findfirstelement will not find a number but a char
                // findfirstelement ususe line indices for textbox .so it works!!
                if (_element is TextBox)
                {
                    //if the text is long and occupies more than 2 lines - value will be less than prevline(it can wrap to 2 lines)
                    // if the text is large (>1 line) then Pagedown is called 1 time...so current line has to be == last line on prev page 
                    if (_FunctionSwitch == PageFunctionName.PageDownInterestingCase)
                    {
                        Verifier.Verify(((_currLine > (_prevLastLine)) && (_currLine < (_prevLastLine * 2))), _element.GetType().Name + " Scrolled Expected Line <[" +
                            _prevLastLine.ToString() + "*2] Actual [" + _currLine.ToString() + "]  \r\n", false);
                    }
                    else
                    {
                        Verifier.Verify(((_currLine > (_prevLastLine * 2 - 3)) && (_currLine < (_prevLastLine * 2 + 2))), _element.GetType().Name + " Scrolled Expected Line [" +
                            _prevLastLine.ToString() + "*2+-2] Actual [" + _currLine.ToString() + "]  \r\n", false);
                    }
                }
                else
                {
                    //RichTextBox, text wrap, Large or small content
                    int _offset = ((int)((TextBoxBase)_element).VerticalOffset);
                    Verifier.Verify(_offset <= (2 * _verticalPageOffset), _element.GetType().Name + " Scrolled down 0/1 page. \r\n", false);
                }
            }
            NextCombination();
        }

        /// <summary>Sets Cursor </summary>
        private void PageUp()
        {
            _firstLine = _controlWrapper.GetIndexOfFirstStringInWindow(0, out _YvalForFirstString);
            //equivalent to inputting twice pagedown. However when content is one line + no wrap. PageUp doesnt seem to work
            ((TextBoxBase)_element).PageDown();
            ((TextBoxBase)_element).PageDown();
            QueueDelegate(PageUpOperation);
        }

        /// <summary>Page Up Operation </summary>
        private void PageUpOperation()
        {
            _initialImage = BitmapCapture.CreateBitmapFromElement(_element);
            _initialOffset = (int)((TextBoxBase)_element).VerticalOffset;
            ((TextBoxBase)_element).PageUp();
            ((TextBoxBase)_element).PageUp();
            QueueDelegate(VerifyPageUp);
        }

        /// <summary>Verify Page Up Operation </summary>
        private void VerifyPageUp()
        {
            _finalImage = BitmapCapture.CreateBitmapFromElement(_element);
            _finalOffset = (int)((TextBoxBase)_element).VerticalOffset;
            _currLine = _controlWrapper.GetIndexOfFirstStringInWindow(0, out _YvalForFirstString);

            if (_largeMultiLineContent)
            {
                Verifier.Verify(ComparisonOperationUtils.AreBitmapsEqual(_initialImage, _finalImage, out _differenceImage) == false, "Images are same. They should be diff because of 2 PageUp calls", false);
                if (_FunctionSwitch == PageFunctionName.PageUpInterestingCase)
                {
                    Verifier.Verify((_initialOffset - _finalOffset) <= (_verticalPageOffset * 2), "Didnt move 2 pages up", false);
                }
                else
                {
                    bool val = (((_initialOffset - _finalOffset) <= (_verticalPageOffset * 2 + 16)) && ((_initialOffset - _finalOffset) > (_verticalPageOffset * 2 - 20)));
                    Verifier.Verify(val, "Didnt move 2 pages up", false);
                }
            }
            else
            {
                Verifier.Verify(_finalOffset == 0, "Vertical offset should be 0", false);
            }
            VerifyPageUpTBandRTB();
        }

        /// <summary>Verify Page Up Operation - Check offsets and Line numbers </summary>
        private void VerifyPageUpTBandRTB()
        {
            Verifier.Verify(_currLine == _firstLine, _element.GetType().Name + " Scrolled up to line #[" +
                        _currLine.ToString() + "] line. Expected [0] \r\n", false);

            NextCombination();
        }

        /// <summary>Sets Cursor </summary>
        private void PageLeft()
        {
            SetCaret();
            //((TextBoxBase)_element).HorizontalOffset = 0;
            KeyboardInput.TypeString("^{HOME}{END}");
            QueueDelegate(PageLeftOperation);
        }

        /// <summary>Page Left Operation </summary>
        private void PageLeftOperation()
        {
            _initialImage = BitmapCapture.CreateBitmapFromElement(_element);
            _initialOffset = (int)((TextBoxBase)_element).HorizontalOffset;
            ((TextBoxBase)_element).PageLeft();
            ((TextBoxBase)_element).PageLeft();
            QueueDelegate(VerifyPageLeft);
        }

        /// <summary>Verify Page Left Operation </summary>
        private void VerifyPageLeft()
        {
            _finalOffset = (int)((TextBoxBase)_element).HorizontalOffset;
            _finalImage = BitmapCapture.CreateBitmapFromElement(_element);
            if ((_controlWrapper.Wrap == false) || (_element is RichTextBox))
            {
                Verifier.Verify(ComparisonOperationUtils.AreBitmapsEqual(_initialImage, _finalImage, out _differenceImage) == false, "Images are same. They should be diff because of PageLeft calls", false);

                // _horizontalOffset * 2 --> if there are 2 pages to scroll
                // >0 <horizontalOffset*2 --> if there are less than 2 pages to scroll (THERE CAN BE JUST ONE CHAR more than a page)
                // if content is less than one page in width then _horizontalOffset is set to 0 and condition is true
                int _differenceOffset = (int)(_initialOffset - _finalOffset);
                Verifier.Verify(((_differenceOffset == (_horizontalPageOffset * 2)) || (_differenceOffset > 0 && _differenceOffset < _horizontalPageOffset * 2 + 20)),
                    "Horizontal Offset is NOT equal to the # of PageLeft invocations", false);
            }
            else if (_controlWrapper.Wrap == true)
            {
                //unstable
                // Verifier.Verify(ComparisonOperationUtils.AreBitmapsEqual(_initialImage, _finalImage, out _differenceImage) == false, "Images are different. They should be same because theres text wrap", false);
                Verifier.Verify(_finalOffset == 0, "Offset should be 0. cant move left", false);
            }

            NextCombination();
        }

        /// <summary>Sets Cursor </summary>
        private void PageRight()
        {
            //SetCaret();
            if (_element is RichTextBox)
            {
                FlowDocument fd = ((RichTextBox)_element).Document;
                fd.ClearValue(FlowDocument.PageWidthProperty);
            }
            KeyboardInput.TypeString("{HOME}");
            ((TextBoxBase)_element).ScrollToHorizontalOffset(0);
            QueueDelegate(PageRightOperation);
        }

        /// <summary>Page Right Operation </summary>
        private void PageRightOperation()
        {
            _initialOffset = (int)((TextBoxBase)_element).HorizontalOffset;
            _initialImage = BitmapCapture.CreateBitmapFromElement(_element);

            ((TextBoxBase)_element).PageRight();
            ((TextBoxBase)_element).PageRight();
            QueueDelegate(VerifyPageRight);
        }

        /// <summary>Verify Page Right Operation </summary>
        private void VerifyPageRight()
        {
            _finalOffset = (int)((TextBoxBase)_element).HorizontalOffset;
            _finalImage = BitmapCapture.CreateBitmapFromElement(_element);
            if (_controlWrapper.Wrap == false)
            {
                Verifier.Verify(ComparisonOperationUtils.AreBitmapsEqual(_initialImage, _finalImage, out _differenceImage) == false, "Images are same. They should be diff because of LineRight calls", false);

                //(_horizontalOffsetValue * 2)  if there are 2 pages to scroll right
                // > (_horizontalOffsetValue) <=(_horizontalOffsetValue * 2)  if there is > 1 page to scroll but less than 2 pages
                Verifier.Verify((_finalOffset - _initialOffset) == (_horizontalPageOffset * 2) || (_finalOffset >= _horizontalPageOffset && _finalOffset < _horizontalPageOffset * 2),
                    "Horizontal Offset is NOT equal to the # of PageRight invocations", false);
            }
            else if (_controlWrapper.Wrap == true)
            {
                //unstable
                //  Verifier.Verify(ComparisonOperationUtils.AreBitmapsEqual(_initialImage, _finalImage, out _differenceImage) == false, "Images are different. They should be same because theres no text wrap", false);
                Verifier.Verify(_finalOffset == 0, "Offset should be 0. cant move right", false);
            }
            NextCombination();
        }

        /// <summary>Page Home Operation </summary>
        private void PageHome()
        {
            _initialImage = BitmapCapture.CreateBitmapFromElement(_element);
            ((TextBoxBase)_element).ScrollToHome();
            QueueDelegate(VerifyPageHome);
        }

        /// <summary>Verify Page Home Operation </summary>
        private void VerifyPageHome()
        {
            _finalImage = BitmapCapture.CreateBitmapFromElement(_element);
            Verifier.Verify((((int)(((TextBoxBase)_element).HorizontalOffset) == 0) && (int)(((TextBoxBase)_element).VerticalOffset) == 0),
                               "Cursor not at (0,0)", false);

            int _viewportHeight = (int)(((TextBoxBase)_element).ViewportHeight);
            int _viewportWidth = (int)(((TextBoxBase)_element).ViewportWidth);
            int _extentHeight = (int)(((TextBoxBase)_element).ExtentHeight);
            int _extendWidth = (int)(((TextBoxBase)_element).ExtentWidth);

            //content is larger than viewport then compare images
            if (((_controlWrapper.Wrap == false) && ((_extentHeight > _viewportHeight) || (_extendWidth > _viewportWidth))) ||
            ((_controlWrapper.Wrap == true) && (_extentHeight > _viewportHeight)))
            {
                Verifier.Verify(ComparisonOperationUtils.AreBitmapsEqual(_initialImage, _finalImage, out _differenceImage) == false, "Images are same. They should be diff because of PageHome calls", false);
            }
            NextCombination();
        }

        /// <summary>Page End Operation </summary>
        private void PageEnd()
        {
            _initialImage = BitmapCapture.CreateBitmapFromElement(_element);
            ((TextBoxBase)_element).ScrollToEnd();
            QueueDelegate(VerifyPageEnd);
        }

        /// <summary>Verify Page End Operation </summary>
        private void VerifyPageEnd()
        {
            _finalImage = BitmapCapture.CreateBitmapFromElement(_element);
            int _lastNumber = _controlWrapper.GetIndexOfLastStringInWindow(0, out _YvalForLastString);

            int _viewportHeight = (int)(((TextBoxBase)_element).ViewportHeight);
            int _viewportWidth = (int)(((TextBoxBase)_element).ViewportWidth);
            int _extentHeight = (int)(((TextBoxBase)_element).ExtentHeight);
            int _extendWidth = (int)(((TextBoxBase)_element).ExtentWidth);


            if (_controlWrapper.Wrap == false)
            {
                if (_largeMultiLineContent)
                {
                    //the find GetIndexOfLastStringInWindow function searches for last visible character
                    //hence the count is 39 even though there are 40 lines. last line being \r\n
                    //here its 40 because TextBox function searches for line index and theres a \r\n
                    //at end of each line.. so the last index is 40
                    int number = 0;
                    number = (_element is RichTextBox) ? 39 : 40;
                    Verifier.Verify(_lastNumber == number, "Cursor not at the end Expected[" + number.ToString() + "] Actual[" +
                        _lastNumber.ToString() + "]", true);
                }
                else
                {
                    //one line - starts with 0
                    Verifier.Verify(_lastNumber == 0, "Cursor not at the end Expected[0] Actual" +
                        _lastNumber.ToString() + "]", true);
                }
            }
            else if (_controlWrapper.Wrap == true)
            {
                if (_largeMultiLineContent == true)
                {
                    Verifier.Verify(((TextBoxBase)_element).VerticalOffset > (int)(_extentHeight / 2), "Cursor not at the end", false);
                }
                else
                {
                    Verifier.Verify(((TextBoxBase)_element).VerticalOffset >= 0, "Cursor not at the end", false);
                }
            }

            //content is larger than viewport then compare images
            //content wraps then check the height of the content
            if (((_controlWrapper.Wrap == false) && ((_extentHeight > _viewportHeight))) ||
            ((_controlWrapper.Wrap == true) && (_extentHeight > _viewportHeight)))
            {
                Verifier.Verify(ComparisonOperationUtils.AreBitmapsEqual(_initialImage, _finalImage, out _differenceImage) == false, "Images are same. They should be diff because of PageEnd calls", true);
            }
            NextCombination();
        }

        /// <summary> Initializes to text that is about a screen and a half for interesting cases </summary>
        public void InitializeElement()
        {
            string str = "";
            int bound = (_element is RichTextBox) ? 10 : 23;
            for (int i = 0; i < bound; i++)
            {
                str += TextUtils.RepeatString(i.ToString() + "> sample data", 1) + "\r\n";
            }
            _controlWrapper.Text = str;
        }

        /// <summary>Sets the caret to (0,0) </summary>
        public void SetCaret()
        {
            if (_element is TextBox)
            {
                ((TextBox)_element).Select(0, 0);
            }
            else
            {
                ((RichTextBox)_element).Selection.Select(_controlWrapper.SelectionInstance.Start, _controlWrapper.SelectionInstance.Start);
            }
        }

        #region private data.

        private TextEditableType _editableType = null;
        private UIElementWrapper _controlWrapper;
        private bool _AcceptsReturn = false;
        private bool _largeMultiLineContent;
        private bool _wrapText = false;
        private bool _scrollVisible = true;
        private Bitmap _initialImage;
        private Bitmap _finalImage;
        private Bitmap _differenceImage;

        private int _firstLine;
        private int _currLine;
        private int _prevLastLine;

        private PageFunctionName _FunctionSwitch = 0;

        private int _initialOffset;
        private int _finalOffset;

        private int _verticalPageOffset;
        private int _horizontalPageOffset;

        private double _YvalForLastString;
        private double _YvalForFirstString;

        private int _iterCount = 0;
        private FrameworkElement _element;

        #endregion private data.
    }

    /// <summary>
    /// Explicitly tests the following members:
    /// ScrollToLine (start and end)
    /// 
    /// Also Implicitly tests:
    /// GetFirstVisibleIndex (TextBox) //used in helper class
    /// GetLastVisibleIndex (TextBox)  //used in helper class
    /// Horizontal/vertical offset
    /// </summary>
    [Test(0, "TextBox", "TextBoxScrollFunctionTest1", MethodParameters = "/TestCaseType:TextBoxScrollFunctionTest", Timeout = 240)]
    [Test(2, "PartialTrust", TestCaseSecurityLevel.FullTrust, "TextBoxScrollFunctionTest2", MethodParameters = "/TestCaseType:TextBoxScrollFunctionTest /XbapName=EditingTestDeploy", Timeout = 300)]
    [TestOwner("Microsoft"), TestTactics("558,559"), TestWorkItem("95 ,96")]
    public class TextBoxScrollFunctionTest : ManagedCombinatorialTestCase
    {
        /// <summary>initialization of the run</summary>
        protected override void DoRunCombination()
        {
            _element = _editableType.CreateInstance();

            if (_element is TextBox)
            {
                _controlWrapper = new UIElementWrapper(_element);

                //setting the Control Properties
                _element.Height = 150;
                _element.Width = 200;
                ((TextBoxBase)_element).FontFamily = new System.Windows.Media.FontFamily("Tahoma");
                ((TextBoxBase)_element).FontSize = 11;

                _controlWrapper.Wrap = _wrapText ? true : false;
                ((TextBoxBase)_element).AcceptsReturn = _AcceptsReturn ? true : false;

                if (_largeMultiLineContent)
                {
                    string str = "";
                    for (int i = 0; i < 40; i++)
                    {
                        str += TextUtils.RepeatString(i.ToString() + "> sample data :P", 10) + "\r\n";
                    }
                    _controlWrapper.Text = str;
                }
                else if (_largeMultiLineContent == false)
                {
                    _controlWrapper.Text = "0>Sample data :)Sample data :)S a m p l e data";
                }

                if (_scrollVisible)
                {
                    ((TextBoxBase)_element).HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                    ((TextBoxBase)_element).VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                }

                TestElement = _element;
                QueueDelegate(TestScrollAction);
            }
            else
            {
                NextCombination();
            }
        }

        /// <summary>Program Controller </summary>
        private void TestScrollAction()
        {
            switch (_textBoxScrollSwitch)
            {
                case TextBoxScrollFunction.Start:
                    TestScrollToLineStart();
                    break;

                case TextBoxScrollFunction.End:
                    ((TextBox)_element).ScrollToVerticalOffset(0);
                    ((TextBox)_element).ScrollToHorizontalOffset(0);
                    QueueDelegate(TestScrollToLineEnd);
                    break;

                case TextBoxScrollFunction.InvalidCalls:
                    TestInvalidCalls();
                    break;

                default:
                    break;
            }
        }

        /// <summary>Scroll to line start operation </summary>
        private void TestScrollToLineStart()
        {
            ((TextBox)_element).ScrollToLine(0);
            VerifyTestScrollToLineStart();
        }

        /// <summary>Verify Scroll to line start operation </summary>
        private void VerifyTestScrollToLineStart()
        {
            int _startIndex = ((TextBox)_element).GetFirstVisibleLineIndex();
            Verifier.Verify(_startIndex == 0, "Didnt scroll to first line", false);
            NextCombination();
        }

        /// <summary>Scroll to line end operation </summary>
        private void TestScrollToLineEnd()
        {
            ((TextBox)_element).ScrollToLine(40);
            QueueDelegate(VerifyTestScrollToLineEnd);
        }

        /// <summary>Verify Scroll to line start operation </summary>
        private void VerifyTestScrollToLineEnd()
        {
            int _lastIndex = ((TextBox)_element).GetLastVisibleLineIndex();
            //40 because last line is  /r/n
            Verifier.Verify((_lastIndex == 40) || (((TextBox)_element).VerticalOffset > 0), "didnt scroll to last line", false);
            //Verifier.Verify( 2>1, "didnt scroll to last line", false);
            NextCombination();
        }

        /// <summary>Test invalid calls </summary>
        private void TestInvalidCalls()
        {
            try
            {
                ((TextBox)_element).ScrollToLine(((TextBox)_element).LineCount + 1);
                throw new ApplicationException("Scroll to line accepts > line count value");
            }
            catch (ArgumentOutOfRangeException)
            {
                Log("ArgumentException thrown as expected when value >line count is used");
            }

            try
            {
                ((TextBox)_element).ScrollToLine(-1);
                throw new ApplicationException("Scroll to line accepts -ve value");
            }
            catch (ArgumentOutOfRangeException)
            {
                Log("ArgumentException thrown as expected when -ve value  is used");
            }
            NextCombination();
        }

        #region private  data.

        private TextBoxScrollFunction _textBoxScrollSwitch = 0;
        private FrameworkElement _element = null;
        private TextEditableType _editableType = null;
        private UIElementWrapper _controlWrapper;
        private bool _AcceptsReturn = false;
        private bool _largeMultiLineContent = false;
        private bool _wrapText = false;
        private bool _scrollVisible = true;

        #endregion private  data.
    }


    /// <summary>
    /// Explicitly tests the following members:
    /// Vertical Offset
    /// Horizontal offset
    /// 
    /// Also Implicitly tests:
    /// GetFirstVisibleIndex (TextBox) //used in helper class
    /// GetLastVisibleIndex (TextBox)  //used in helper class
    /// </summary>
    [Test(0, "TextBoxBase", "TextBoxBaseScrollFunctionTestOffsets", MethodParameters = "/TestCaseType:TextBoxBaseScrollFunctionTestOffsets")]
    [TestOwner("Microsoft"), TestTactics("557"), TestWorkItem("95 ,96"), TestBugs("577")]
    public class TextBoxBaseScrollFunctionTestOffsets : ManagedCombinatorialTestCase
    {
        /// <summary>starts the combinatorial engine </summary>
        protected override void DoRunCombination()
        {
            _element = _editableType.CreateInstance();

            if (_element is PasswordBox)
            {
                NextCombination();
            }
            else
            {
                _controlWrapper = new UIElementWrapper(_element);

                //setting the Control Properties
                _element.Height = 150;
                _element.Width = 200;
                ((TextBoxBase)_element).FontFamily = new System.Windows.Media.FontFamily("Tahoma");
                ((TextBoxBase)_element).FontSize = 11;

                _controlWrapper.Wrap = _wrapText ? true : false;
                if (_wrapText == false && _element is RichTextBox)
                {
                    ((RichTextBox)_element).Document.PageWidth = 600;
                }
                ((TextBoxBase)_element).AcceptsReturn = _AcceptsReturn ? true : false;

                if (_largeMultiLineContent)
                {
                    string str = "";
                    for (int i = 0; i < 40; i++)
                    {
                        str += TextUtils.RepeatString(i.ToString() + "> sample data :P", 10) + "\r\n";
                    }
                    _controlWrapper.Text = str;
                }
                else if (_largeMultiLineContent == false)
                {
                    _controlWrapper.Text = "0>Sample data :)Sample data :)S a m p l e data";
                }

                if (_scrollVisible)
                {
                    ((TextBoxBase)_element).HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                    ((TextBoxBase)_element).VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                }

                TestElement = _element;

                QueueDelegate(TestOffsets);
            }
        }

        /// <summary>Program Controller </summary>
        private void TestOffsets()
        {
            switch (_offsetSwitch)
            {
                case OffsetList.Horizontal:
                    {
                        ((TextBoxBase)_element).ScrollToVerticalOffset(0);
                        ((TextBoxBase)_element).ScrollToHorizontalOffset(0);
                        QueueDelegate(TestHorizontalOffsets);
                        break;
                    }

                case OffsetList.Vertical:
                    {
                        TestVerticalOffsets();
                        break;
                    }

                default:
                    break;
            }
        }

        /// <summary>TestHorizontalOffsets </summary>
        private void TestHorizontalOffsets()
        {
            _initialImage = BitmapCapture.CreateBitmapFromElement(_element);

            ((TextBoxBase)_element).ScrollToHorizontalOffset(30);
            QueueDelegate(VerifyRenderingHorizontalOffset);
        }

        /// <summary>Verify rendering of horizontal offset </summary>
        private void VerifyRenderingHorizontalOffset()
        {
            _finalImage = BitmapCapture.CreateBitmapFromElement(_element);
            _indexOfFirstString = _controlWrapper.GetIndexOfFirstStringInWindow(0, out _YvalForFirstString);
            if (_controlWrapper.Wrap == false)
            {
                if ((((TextBoxBase)_element).ExtentWidth > ((TextBoxBase)_element).ViewportWidth))
                {
                    Verifier.Verify(ComparisonOperationUtils.AreBitmapsEqual(_initialImage, _finalImage, out _differenceImage) == false, "Images are same. They should be diff because of HorOffset calls", false);
                }
                else
                {
                    Verifier.Verify(_indexOfFirstString == 0, "Images are diff. The first line starts with 0. ", false);
                    //Verifier.Verify(ComparisonOperationUtils.AreBitmapsEqual(_initialImage, _finalImage, out _differenceImage) == true, "Images are diff. They should be same because scrollbars are disabled", false);
                }
            }
            else if (_controlWrapper.Wrap == true)
            {
                //Verifier.Verify(ComparisonOperationUtils.AreBitmapsEqual(_initialImage, _finalImage, out _differenceImage) == true, "Images are diff. They should be same because scrollbars are disabled", false);
                Verifier.Verify(_indexOfFirstString == 0, "Images are diff. The first line starts with 0.", false);
            }
            QueueDelegate(HorizontalOffsetWhenScrollDisabled);
        }

        /// <summary>set horizontal offset when scrollbar disabled</summary>
        private void HorizontalOffsetWhenScrollDisabled()
        {
            DisableHorizontalScrollbar();
            _initialString = _controlWrapper.GetFirstStringInWindow(0, out _YvalForFirstString);
            _indexOfFirstString = _controlWrapper.GetIndexOfFirstStringInWindow(0, out _YvalForFirstString);
            QueueDelegate(SetNewHorizontalOffset);
        }

        /// <summary> Set Horizontal Offset </summary>
        private void SetNewHorizontalOffset()
        {
            ((TextBoxBase)_element).ScrollToHorizontalOffset(50);
            ((TextBoxBase)_element).ScrollToVerticalOffset(0);
            QueueDelegate(VerifyHorizontalOffsetWhenScrollDisabled);
        }

        /// <summary>Verify rendering of horizontal offset when scrollbar disabled</summary>
        private void VerifyHorizontalOffsetWhenScrollDisabled()
        {
            _finalString = _controlWrapper.GetFirstStringInWindow(0, out  _YvalForFirstString);
            _indexOfFirstString = _controlWrapper.GetIndexOfFirstStringInWindow(0, out _YvalForFirstString);
            Verifier.Verify(_indexOfFirstString == 0, "Images are diff. The first line starts with 0. ", false);

            if (_element is TextBox)
            {
                Verifier.Verify(_finalString.Equals(_initialString) == true, "Strings are supposed to be equal. Currently not", false);
            }
            else
            {
                Verifier.Verify(_finalString.Length == _initialString.Length, "Both Strings are supposed to be equal. Currently not", false);
            }
            //When disabled the text should not be movable.. its fixed...

            NextCombination();
        }

        /// <summary>TestVerticalOffsets </summary>
        private void TestVerticalOffsets()
        {
            //remove this when Regression_Bug577 is corrected
            if ((_element is TextBox) && (_largeMultiLineContent == false))
            {
                //THIS IS because the GetFirstVisibleIndex() function of textbox
                //uses the offset values and when the values are not updated
                //the function returns wrong values
                NextCombination();
            }
            else
            {
                _initialImage = BitmapCapture.CreateBitmapFromElement(_element);

                ((TextBoxBase)_element).ScrollToVerticalOffset(40);
                QueueDelegate(VerifyRenderingVerticalOffset);
            }
        }

        /// <summary>Verify rendering of vertical offset </summary>
        private void VerifyRenderingVerticalOffset()
        {
            ((TextBoxBase)_element).UpdateLayout();
            _finalImage = BitmapCapture.CreateBitmapFromElement(_element);
            _indexOfFirstString = _controlWrapper.GetIndexOfFirstStringInWindow(0, out _YvalForFirstString);

            if ((((TextBoxBase)_element).ExtentHeight > ((TextBoxBase)_element).ViewportHeight))
            {
                Verifier.Verify(ComparisonOperationUtils.AreBitmapsEqual(_initialImage, _finalImage, out _differenceImage) == false, "Images are same. They should be diff because of HorOffset calls", false);
                Verifier.Verify(_indexOfFirstString != 0, "Still in same location 0,0. Needs to be moved down. ", false);
            }
            else
            {
                Verifier.Verify(_indexOfFirstString == 0, "Location has changed. Content is less than viewport. So theres should not be any offset. ", false);
                //Verifier.Verify(ComparisonOperationUtils.AreBitmapsEqual(_initialImage, _finalImage, out _differenceImage) == true, "Images are diff. They should be same because scrollbars are disabled", false);
            }
            QueueDelegate(VerticalOffsetWhenScrollDisabled);
        }

        /// <summary>set vertical offset when scrollbar disabled</summary>
        private void VerticalOffsetWhenScrollDisabled()
        {
            DisableVerticalScrollbar();
            QueueDelegate(GetVerticalOffsetWhenScrollBarDisabled);
        }

        private void GetVerticalOffsetWhenScrollBarDisabled()
        {
            _initialString = _controlWrapper.GetFirstStringInWindow(0, out _YvalForFirstString);
            _indexOfFirstString = _controlWrapper.GetIndexOfFirstStringInWindow(0, out _YvalForFirstString);
            QueueDelegate(SetNewVerticalOffset);
        }

        /// <summary> Set Vertical Offset </summary>
        private void SetNewVerticalOffset()
        {
            ((TextBoxBase)_element).ScrollToVerticalOffset(50);
            ((TextBoxBase)_element).ScrollToHorizontalOffset(0);
            QueueDelegate(VerifyVerticalOffsetWhenScrollDisabled);
        }

        /// <summary>Verify rendering of vertical offset when scrollbar disabled</summary>
        private void VerifyVerticalOffsetWhenScrollDisabled()
        {
            if (_largeMultiLineContent == true)
            {
                //the updated value can be set but it isnt rendered when not possible
                //However it is used by other controls... 
                //maybe a bug
                ((TextBoxBase)_element).ScrollToVerticalOffset(0);
            }

            _finalString = _controlWrapper.GetFirstStringInWindow(0, out  _YvalForFirstString);
            _indexOfFirstString = _controlWrapper.GetIndexOfFirstStringInWindow(0, out _YvalForFirstString);
            Verifier.Verify(_indexOfFirstString == 0, "Images are diff. The first line starts with 0. ", false);
            Verifier.Verify(_finalString.Equals(_initialString) == true, "Strings are supposed to be equal. Currently not", false);
            NextCombination();
        }

        /// <summary>Disable horizontal scrollbar</summary>
        private void DisableHorizontalScrollbar()
        {
            ((TextBoxBase)_element).ScrollToHorizontalOffset(0);
            ((TextBoxBase)_element).ScrollToVerticalOffset(0);
            ((TextBoxBase)_element).HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            ((TextBoxBase)_element).UpdateLayout();
        }

        /// <summary>Disable vertical scrollbar</summary>
        private void DisableVerticalScrollbar()
        {
            ((TextBoxBase)_element).ScrollToHorizontalOffset(0);
            ((TextBoxBase)_element).ScrollToVerticalOffset(0);
            QueueDelegate(DisableVS);
        }

        private void DisableVS()
        {
            ((TextBoxBase)_element).VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            ((TextBoxBase)_element).UpdateLayout();
        }

        #region private data.

        private OffsetList _offsetSwitch = 0;
        private double _YvalForFirstString;
        private double _indexOfFirstString;

        private String _initialString;
        private String _finalString;

        private bool _scrollVisible = true;
        private bool _AcceptsReturn = false;
        private bool _largeMultiLineContent = false;
        private bool _wrapText = false;

        private Bitmap _initialImage;
        private Bitmap _finalImage;
        private Bitmap _differenceImage;

        private FrameworkElement _element = null;
        private TextEditableType _editableType = null;
        private UIElementWrapper _controlWrapper;

        #endregion private data.
    }

    /// <summary>
    /// Explicitly tests the following members:
    /// Viewport height/width
    /// Extent Height/width
    /// </summary>
    [Test(2, "TextBoxBase", "TextBoxBaseScrollFunctionTestViewportExtent", MethodParameters = "/TestCaseType:TextBoxBaseScrollFunctionTestViewportExtent")]
    [TestOwner("Microsoft"), TestTactics("560"), TestWorkItem("95 ,96")]
    public class TextBoxBaseScrollFunctionTestViewportExtent : ManagedCombinatorialTestCase
    {
        /// <summary>initialization of the run</summary>
        /// <summary> filter for combinations read</summary>
        protected override bool DoReadCombination(Hashtable values)
        {
            bool result = base.DoReadCombination(values);
            if (_editableType == TextEditableType.GetByName("PasswordBox"))
                return false;
            return true;
        }

        /// <summary>initialization of the run</summary>
        protected override void DoRunCombination()
        {
            _element = _editableType.CreateInstance();

            _controlWrapper = new UIElementWrapper(_element);

            //setting the Control Properties
            _element.Height = 150;
            _element.Width = 200;
            ((TextBoxBase)_element).FontFamily = new System.Windows.Media.FontFamily("Tahoma");
            ((TextBoxBase)_element).FontSize = 11;

            _controlWrapper.Wrap = _wrapText ? true : false;
            ((TextBoxBase)_element).AcceptsReturn = _AcceptsReturn ? true : false;

            if (_largeMultiLineContent)
            {
                string str = "";
                for (int i = 0; i < 40; i++)
                {
                    str += TextUtils.RepeatString(i.ToString() + "> s a m p l e :P", 10) + "\r\n";
                }
                _controlWrapper.Text = str;
            }
            else if (_largeMultiLineContent == false)
            {
                _controlWrapper.Text = "0>Sample data :)Sample data :)S a m p l e data";
            }

            if (_scrollVisible)
            {
                ((TextBoxBase)_element).HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                ((TextBoxBase)_element).VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            }
            TestElement = _element;
            QueueDelegate(DoFocus);
        }

        /// <summary>Focus on element</summary>
        private void DoFocus()
        {
            _element.Focus();
            QueueDelegate(StartCase);
        }

        /// <summary>Program Controller </summary>
        private void StartCase()
        {
            switch (_viewportExtentSwitch)
            {
                case ViewportExtentList.HorizontalScrollbarEffect:
                    HorizontalScrollBarHidden();
                    break;

                case ViewportExtentList.VerticalScrollbarEffect:

                    VerticalScrollBarHidden();
                    break;

                case ViewportExtentList.ChangeExtent:
                    ChangeContent("sample");
                    QueueDelegate(ChangeExtentHeightAndWidth);
                    break;

                case ViewportExtentList.ChangeViewport:
                    ChangeControlSize(100, 100);
                    QueueDelegate(ChangeViewportHeightAndWidth);
                    break;

                default:
                    break;
            }
        }

        /// <summary> Effect of hiding horizontal scrollbar on extent and viewport height </summary>
        private void HorizontalScrollBarHidden()
        {
            //The wrapping of text results in the viewport height to become unaffected when 
            //horizontal scrollbar is hidden
            _viewportHeight = (int)(((TextBoxBase)_element).ViewportHeight);
            _extentHeight = (int)(((TextBoxBase)_element).ExtentHeight);
            HideHorizontalScrollbar();
            QueueDelegate(CheckEffectOfHiddenHorizontalScrollbar);
        }

        /// <summary> Verify the Effect of hiding horizontal scrollbar on extent and viewport height</summary>
        private void CheckEffectOfHiddenHorizontalScrollbar()
        {
            int _newViewportHeight = (int)(((TextBoxBase)_element).ViewportHeight);
            int _newExtentHeight = (int)(((TextBoxBase)_element).ExtentHeight);
            Verifier.Verify(_newExtentHeight == _extentHeight, "Horizontal scrollbar affected the extent height!", false);
            if ((_controlWrapper.Wrap == true) && (_element is TextBox))
            {
                Verifier.Verify(_newViewportHeight == _viewportHeight, "horizontal scrollbar is already hidden when text is wrapped.", false);
            }
            else
            {
                Verifier.Verify(_newViewportHeight > _viewportHeight, "Hiding horizontal scrollbar didnt affect the size of vertical viewport", false);
            }
            NextCombination();
        }

        /// <summary> Effect of hiding vertical scrollbar on extent and viewport width </summary>
        private void VerticalScrollBarHidden()
        {
            _viewportWidth = (int)(((TextBoxBase)_element).ViewportWidth);
            _extendWidth = (int)(((TextBoxBase)_element).ExtentWidth);
            HideVerticalScrollbar();
            QueueDelegate(CheckEffectOfHiddenVerticalScrollbar);
        }

        /// <summary> Verify the Effect of hiding vertical scrollbar on extent and viewport width </summary>
        private void CheckEffectOfHiddenVerticalScrollbar()
        {
            int _newViewportWidth = (int)(((TextBoxBase)_element).ViewportWidth);
            int _newExtentWidth = (int)(((TextBoxBase)_element).ExtentWidth);
            //if text wraps extent height is set by the viewport boundary.
            //so when the vertical scrollbar is hidden the extent width automatiucally increases.
            if ((_controlWrapper.Wrap == true) || (_element is RichTextBox))
            {
                Verifier.Verify((_newExtentWidth > _extendWidth), "Vertical scrollbar should affect the extent Width! Actual[" +
                    _newExtentWidth.ToString() + "] > Previous [" + _extendWidth.ToString() + "]", false);
            }
            else
            {
                Verifier.Verify((_newExtentWidth == _extendWidth), "Vertical scrollbar didnt affect the extent Width!", false);
            }
            Verifier.Verify(_newViewportWidth >= _viewportWidth, "Hiding Vertical scrollbar should affect the size of horizontal viewport Actual[" +
                    _newViewportWidth.ToString() + "] >= Previous [" + _viewportWidth.ToString() + "]", false);
            NextCombination();
        }

        /// <summary> Hide horizontal scrollbar </summary>
        private void HideHorizontalScrollbar()
        {
            ((TextBoxBase)_element).HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            ((TextBoxBase)_element).UpdateLayout();
        }

        /// <summary> Hide vertical scrollbar </summary>
        private void HideVerticalScrollbar()
        {
            ((TextBoxBase)_element).VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            ((TextBoxBase)_element).UpdateLayout();
        }

        /// <summary> Change the extent by changing content </summary>
        private void ChangeExtentHeightAndWidth()
        {
            _extentHeight = (int)(((TextBoxBase)_element).ExtentHeight);
            _extendWidth = (int)(((TextBoxBase)_element).ExtentWidth);
            if (_index < _stringCount)
            {
                ChangeContent(_stringArray[_index]);
                _index++;
                QueueDelegate(VerifyChangeInExtent);
            }
            else
            {
                NextCombination();
            }
        }

        /// <summary> Verify Change in extent by changing content </summary>
        private void VerifyChangeInExtent()
        {
            int _newExtentHeight = (int)(((TextBoxBase)_element).ExtentHeight);
            int _newExtentWidth = (int)(((TextBoxBase)_element).ExtentWidth);
            if (_index < 2)
            {
                Verifier.Verify(_newExtentHeight > _extentHeight, "New extent height should be greater than prev height", false);
            }
            else
            {
                //incase of wrap it may be equal to prev string 
                Verifier.Verify(_newExtentWidth >= _extendWidth, "New extent width should be greater than prev width", false);
            }
            QueueDelegate(ChangeExtentHeightAndWidth);
        }

        /// <summary> Change the content of the control element </summary>
        private void ChangeContent(string str)
        {
            //SetText(_element, str);
            _controlWrapper.Text = str;
            ((TextBoxBase)_element).UpdateLayout();
            return;
        }

        /// <summary> Change the viewport by changing control size </summary>
        private void ChangeViewportHeightAndWidth()
        {
            _viewportHeight = (int)(((TextBoxBase)_element).ViewportHeight);
            _viewportWidth = (int)(((TextBoxBase)_element).ViewportWidth);
            if (_index < _stringCount)
            {
                ChangeControlSize(_xArray[_index], _yArray[_index]);
                _index++;
                Log(_element.Height.ToString() + "------------");

                QueueDelegate(VerifyChangeInViewportSize);
            }
            else
            {
                NextCombination();
            }
        }

        /// <summary> Verify Change in viewport by changing control size </summary>
        private void VerifyChangeInViewportSize()
        {
            int _newViewportHeight = (int)(((TextBoxBase)_element).ViewportHeight);
            int _newViewportWidth = (int)(((TextBoxBase)_element).ViewportWidth);
            if (_index < 2)
            {
                Verifier.Verify(_newViewportHeight > _viewportHeight, "New Viewport height should be greater than prev height", false);
            }
            else
            {
                Verifier.Verify(_newViewportWidth > _viewportWidth, "New Viewport width should be greater than prev width", false);
            }
            QueueDelegate(ChangeExtentHeightAndWidth);
        }

        /// <summary> Change the size of the control element </summary>
        private void ChangeControlSize(double x, double y)
        {
            _element.Height = y;
            _element.Width = x;
            return;
        }

        #region private data.

        int _viewportHeight;
        int _viewportWidth;
        int _extentHeight;
        int _extendWidth;

        private TextEditableType _editableType = null;
        private UIElementWrapper _controlWrapper;
        private bool _AcceptsReturn = false;
        private bool _largeMultiLineContent = false;
        private bool _wrapText = false;
        private bool _scrollVisible = true;

        private int _stringCount = 4;
        private int _index = 0;
        private ViewportExtentList _viewportExtentSwitch = 0;
        private string[] _stringArray ={"sample\r\nsample",
                                        "sample\r\nsample\r\nsample",
                                        "sample sample",
                                        "sample sample sample"};
        private double[] _xArray = { 100, 100, 150, 200 };
        private double[] _yArray = { 150, 200, 100, 100 };
        private FrameworkElement _element;

        #endregion private data.
    }

    /// <summary>Tests  Line Up and Line down Operation for DCR 42757 </summary>
    [Test(0, "TextBoxBase", "TextBoxLineUpLineDownTest", MethodParameters = "/TestCaseType:TextBoxLineUpLineDownTest", Keywords = "Setup_SanitySuite")]
    [TestOwner("Microsoft"), TestTactics("556"), TestWorkItem("94")]
    public class TextBoxLineUpLineDownTest : ManagedCombinatorialTestCase
    {
        /// <summary>Starts the combinatorial engine </summary>
        protected override void DoRunCombination()
        {
            _element = _editableType.CreateInstance();
            _controlWrapper = new UIElementWrapper(_element);

            if (_element is TextBox)
            {
                _element.Height = 150;
                _element.Width = 200;
                ((TextBox)_element).FontFamily = new System.Windows.Media.FontFamily(_fontFamily);
                ((TextBox)_element).FontSize = _fontSize;

                if (_largeMultiLineContent)
                {
                    string str = "";
                    for (int i = 0; i < 40; i++)
                    {
                        str += TextUtils.RepeatString(i.ToString() + "> sample data :P", 10) + "\r\n";
                    }
                    _controlWrapper.Text = str;
                }
                else if (_largeMultiLineContent == false)
                {
                    _controlWrapper.Text = "0>Sample data :)Sample data :)S a m p l e data";
                }
                ((TextBox)_element).VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                TestElement = _element;
                QueueDelegate(ExecuteTrigger);
            }
            else
            {
                NextCombination();
            }
        }

        /// <summary>Program Controller </summary>
        private void ExecuteTrigger()
        {
            _lineHeight = GetLineHeight();
            _element.Focus();
            KeyboardInput.TypeString("^{HOME}");

            //UNCOMMENT THE ENUM INPUTTRIGGER WHEN LINE SCROLLS BY LINE HEIGHT
            if (_inputSwitch == InputTrigger.Programmatical)
            {
                QueueDelegate(LineDownOperation);
            }
            else
            {
                QueueDelegate(LineDownOperationKeyBoard);
            }
        }

        /// <summary> Performs Line Down operation programmatically </summary>
        private void LineDownOperation()
        {
            _initialImage = BitmapCapture.CreateBitmapFromElement(_element);
            _initialNumber = _controlWrapper.GetIndexOfFirstStringInWindow(0, out _YvalForFirstString);
            ((TextBox)_element).LineDown();
            QueueDelegate(VerifyLineDown);
        }

        /// <summary> Performs Line Down operation through keyboard - click on scrollbar</summary>
        private void LineDownOperationKeyBoard()
        {
            _initialImage = BitmapCapture.CreateBitmapFromElement(_element);
            _initialNumber = _controlWrapper.GetIndexOfFirstStringInWindow(0, out _YvalForFirstString);

            System.Windows.Point p = ActionItemWrapper.GetScreenRelativeOrigin(_element);
            p.X = p.X + _element.Width - 10;
            p.Y = p.Y + _element.Height - 10;
            MouseInput.MouseClick(p);
            QueueDelegate(VerifyLineDown);
        }

        /// <summary> Verifies Line Down Operation </summary>
        public void VerifyLineDown()
        {
            int _currentOffset = (int)(((TextBoxBase)_element).VerticalOffset);
            _finalImage = BitmapCapture.CreateBitmapFromElement(_element);

            _finalNumber = _controlWrapper.GetIndexOfFirstStringInWindow(_YvalForFirstString, out _YvalForFirstString);
            if (_largeMultiLineContent)
            {
                //tolerance value +-1
                Verifier.Verify(ComparisonOperationUtils.AreBitmapsEqual(_initialImage, _finalImage, out _differenceImage) == false, "Images are same. They should be diff because of LineDown call", true);
                Verifier.Verify((_currentOffset <= (_lineHeight) + 1) && (_currentOffset >= (_lineHeight) - 1), "Vertical Offset hasnt changed according to 1 line Down. Expected [" +
                    _lineHeight.ToString() + "+-1] Actual [" + ((TextBoxBase)_element).VerticalOffset.ToString() + "]", true);
                //1 LINE DOWN
                VerifyLineDownTBandRTB(1);
            }
            else
            {
                Verifier.Verify((int)(((TextBoxBase)_element).VerticalOffset) == 0, "Vertical Offset is NOT =0. Expected [0] Actual [" +
                    ((TextBoxBase)_element).VerticalOffset.ToString() + "]", true);
                //1 LINE DOWN
                VerifyLineDownTBandRTB(0);
            }
        }

        /// <summary>Verifies Line Numbers on Line Down operation </summary>
        /// <param name="num">this is the number of lines moved down</param>
        public void VerifyLineDownTBandRTB(int num)
        {
            int _linesScrolled = (_finalNumber - _initialNumber);
            Verifier.Verify(_finalNumber == _initialNumber + num, _element.GetType().Name + " Scrolled down [" +
                    _linesScrolled.ToString() + "] lines. Expected Count [" + num.ToString() + "] \r\n", true);
            LineUp();
        }

        /// <summary> Performs Line Up operation </summary>
        public void LineUp()
        {
            ((TextBox)_element).LineDown();
            ((TextBox)_element).LineDown();
            if (_inputSwitch == InputTrigger.Programmatical)
            {
                QueueDelegate(LineUpOperation);
            }
            else
            {
                QueueDelegate(LineUpOperationKeyBoard);
            }
        }

        /// <summary> Performs Line Up operation programmatically</summary>
        public void LineUpOperation()
        {
            _initialOffset = (int)((TextBox)_element).VerticalOffset;
            _initialImage = BitmapCapture.CreateBitmapFromElement(_element);
            _initialNumber = _controlWrapper.GetIndexOfFirstStringInWindow(0, out _YvalForFirstString);
            ((TextBoxBase)_element).LineUp();
            QueueDelegate(VerifyLineUp);
        }

        /// <summary> Performs Line Up operation  keyboard</summary>
        public void LineUpOperationKeyBoard()
        {
            _initialOffset = (int)((TextBox)_element).VerticalOffset;
            _initialImage = BitmapCapture.CreateBitmapFromElement(_element);
            _initialNumber = _controlWrapper.GetIndexOfFirstStringInWindow(0, out _YvalForFirstString);
            System.Windows.Point p = ActionItemWrapper.GetScreenRelativeOrigin(_element);
            p.X = p.X + _element.Width - 10;
            p.Y = p.Y + 10;
            MouseInput.MouseClick(p);
            QueueDelegate(VerifyLineUp);
        }

        /// <summary> Verifies Line Up Operation </summary>
        public void VerifyLineUp()
        {
            //Line moves down from the top. So we need to find the first string. so use default Y value
            _finalImage = BitmapCapture.CreateBitmapFromElement(_element);
            _finalNumber = _controlWrapper.GetIndexOfFirstStringInWindow(0, out _YvalForFirstString);
            if (_largeMultiLineContent)
            {
                Verifier.Verify(ComparisonOperationUtils.AreBitmapsEqual(_initialImage, _finalImage, out _differenceImage) == false, "Images are same. They should be diff because of LineUp calls", true);

                int _actualOffset = (int)(((TextBoxBase)_element).VerticalOffset);
                int diff = (_initialOffset - _actualOffset);
                Verifier.Verify(((diff <= _lineHeight + 1) && (diff >= _lineHeight - 1)), "Vertical Offset is NOT equal to the # of LineUp invocations Expected [" +
                    _lineHeight.ToString() + "+-1] Actual[" + diff.ToString() + "]", true);
                //1 LINE up
                VerifyLineUpTBandRTB(1);
            }
            else
            {
                Verifier.Verify((int)(((TextBoxBase)_element).VerticalOffset) == 0, "Vertical Offset should be 0. single line content", true);
                //1 LINE UP
                VerifyLineUpTBandRTB(0);
            }
        }

        /// <summary>Verifies Line Numbers on Line Up operation </summary>
        public void VerifyLineUpTBandRTB(int num)
        {
            int _linesScrolled = (_initialNumber - _finalNumber);
            Verifier.Verify(_finalNumber == _initialNumber - num, _element.GetType().Name + " Scrolled up [" +
                 _linesScrolled.ToString() + "] lines. Expected Count [" + num.ToString() + "] \r\n", true);
            NextCombination();
        }

        #region Helper Functions

        /// <summary>Gets Line Height </summary>
        private int GetLineHeight()
        {
            System.Windows.Media.FontFamily fontFamily = (System.Windows.Media.FontFamily)_element.GetValue(TextElement.FontFamilyProperty);
            double fontSize = (double)_element.GetValue(TextElement.FontSizeProperty);
            int height = (int)(fontFamily.LineSpacing * fontSize);
            return height;
        }

        #endregion Helper Functions

        #region private data.

        private int _initialNumber;
        private Bitmap _initialImage, _finalImage, _differenceImage;
        private int _initialOffset;
        private int _finalNumber;
        private bool _largeMultiLineContent = false;
        private int _lineHeight;
        private double _YvalForFirstString;

        private InputTrigger _inputSwitch = 0;
        private int _fontSize = 0;
        private string _fontFamily = null;
        private FrameworkElement _element;
        private TextEditableType _editableType = null;
        private UIElementWrapper _controlWrapper;

        #endregion private data.
    }

    /// <summary>Tests GetFirLastVisibleLineIndex in TextBox </summary>
    [Test(0, "TextBox", "TextBoxGetFirstLastVisibleIndex", MethodParameters = "/TestCaseType:TextBoxGetFirstLastVisibleIndex", Keywords = "Setup_SanitySuite")]
    [TestOwner("Microsoft"), TestTactics("554"), TestWorkItem("93")]
    public class TextBoxGetFirstLastVisibleIndex : ManagedCombinatorialTestCase
    {
        /// <summary>Starts the combinatorial engine </summary>
        protected override void DoRunCombination()
        {
            _element = _editableType.CreateInstance();
            _controlWrapper = new UIElementWrapper(_element);
            OperatingSystem os = Environment.OSVersion;
            Version ver = os.Version;

            if (_element is TextBox)
            {
                _controlWrapper = new UIElementWrapper(_element);
                _tb = _element as TextBox;
                _tb.FontSize = 20;
                _tb.FontWeight = FontWeights.Bold;

                //There are a little different height between win7 and win8 when after pagedown,
                //so reduce 4 pixels on height on win8.                
                if (ver.Major > 6 || ((6 == ver.Major) && ver.Minor > 1))
                {
                    _tb.Height = 116;
                }
                else
                {
                    _tb.Height = 120;
                }

                _tb.Width = 100;
                _tb.FontFamily = new System.Windows.Media.FontFamily("TAHOMA");
                _tb.TextWrapping = _textWrap ? TextWrapping.Wrap : TextWrapping.NoWrap;
                _tb.Text = _multiLine ? GetText() : "This is a textbox Not a RichTextBox";
                TestElement = _element;
                QueueDelegate(DoFocus);
            }
            else
            {
                NextCombination();
            }
        }

        private void DoFocus()
        {
            _element.Focus();
            QueueDelegate(InitialVerification);
        }

        /// <summary>Program Controller </summary>
        private void InitialVerification()
        {
            Bitmap bmp1 = BitmapCapture.CreateBitmapFromElement(_element);
            _numberOfLines = BitmapUtils.CountTextLines(bmp1);
            VerifyLineIndex(_numberOfLines);
            QueueDelegate(DoPageDown);
        }

        private void VerifyLineIndex(int _numberOfLines)
        {
            Verifier.Verify(_numberOfLines == (_tb.GetLastVisibleLineIndex() + 1), "LastVisible Line Index Expected [" +
                _numberOfLines.ToString() + "] Actual [" + _tb.GetLastVisibleLineIndex() + "] + 1 (since its 0 based)", true);
            Verifier.Verify(0 == _tb.GetFirstVisibleLineIndex(), "FirstVisible Line Index Expected [0" +
                 "] Actual [" + _tb.GetFirstVisibleLineIndex() + "] ", true);
            _firstPageLastLine = _tb.GetLastVisibleLineIndex();
        }

        private void DoPageDown()
        {
            _tb.PageDown();
            QueueDelegate(VerifyPageDown);
        }

        private void VerifyPageDown()
        {
            Bitmap bmp1 = BitmapCapture.CreateBitmapFromElement(_element);
            int _numberOfLinesAfterPageDown = BitmapUtils.CountTextLines(bmp1);
            Logger.Current.LogImage(bmp1, "bmp");
            Log("NumberOfLines =[" + _numberOfLines.ToString() + "] After PageDown [" + _numberOfLinesAfterPageDown.ToString() + "]");
            if (_multiLine == true)
            {
                int _lastLineNumber = (_firstPageLastLine == _tb.GetFirstVisibleLineIndex()) ? (_numberOfLines * 2 - 1) : (_numberOfLines * 2); //-1 because last overlapping line becomes first line on page down

                _lastLineNumber = (_numberOfLinesAfterPageDown >= _numberOfLines + 1) ? (_lastLineNumber + 1) : _lastLineNumber;
                //Increment the line number by 1 because if the line overlaps then the second page will have 6 lines and hence (_numberOfLines * 2 - 1 wont be correct.. needs to be incremented
                Verifier.Verify(_lastLineNumber == (_tb.GetLastVisibleLineIndex() + 1), "--LastVisible Line Index Expected [" +
                    _lastLineNumber.ToString() + "] Actual [" + _tb.GetLastVisibleLineIndex() + "] + 1 (since its 0 based)", true);
                Verifier.Verify(_numberOfLinesAfterPageDown >= (_tb.GetFirstVisibleLineIndex() + 1), "#of lines Expected >=[" +
                    _numberOfLines.ToString() + "] Actual [" + _numberOfLinesAfterPageDown.ToString() + "] ", true);
            }
            else
            {
                VerifyLineIndex(_numberOfLines);
            }
            NextCombination();
        }

        private string GetText()
        {
            string str = "";
            for (int i = 0; i < 10; i++)
            {
                str += i.ToString() + "> " + TextUtils.RepeatString("lllll dltk ", 3) + "\r\n";
            }
            return str;
        }

        private int _firstPageLastLine = 0;
        private FrameworkElement _element;
        private TextEditableType _editableType = null;
        private UIElementWrapper _controlWrapper;
        private int _numberOfLines = 0;
        private bool _textWrap = false;
        private bool _multiLine = false;
        private TextBox _tb = null;
    }

    /// <summary>Tests keyboard scrolling </summary>
    [Test(2, "TextBoxBase", "ScrollingPageupDownLineUpDown", MethodParameters = "/TestCaseType:ScrollingPageupDownLineUpDown", Timeout = 500)]
    [TestOwner("Microsoft"), TestTactics("553"), TestWorkItem("88"), TestBugs("635")]
    public class ScrollingPageupDownLineUpDown : ManagedCombinatorialTestCase
    {
        /// <summary>Starts the combinatorial engine </summary>
        protected override void DoRunCombination()
        {
            _element = _editableType.CreateInstance();
            _controlWrapper = new UIElementWrapper(_element);

            if (_element is PasswordBox)
            {
                NextCombination();
            }
            else
            {
                _controlWrapper = new UIElementWrapper(_element);
                ((TextBoxBase)_element).FontSize = 20;
                ((TextBoxBase)_element).AcceptsReturn = true;
                ((TextBoxBase)_element).Height = ((TextBoxBase)_element).Width = 200;
                TestElement = _element;
                _count = 0;
                SetText();
                QueueDelegate(DoWindowFocus);
            }
        }

        private void DoWindowFocus()
        {
            MouseInput.MouseClick(_element);
            KeyboardInput.TypeString("^{HOME}");
            QueueDelegate(DoFocus);
        }

        /// <summary>DoFocus </summary>
        private void DoFocus()
        {
            _element.Focus();
            ((TextBoxBase)_element).ScrollToVerticalOffset(0);
            ((TextBoxBase)_element).ScrollToHorizontalOffset(0);
            //LINE DOWN DOESNT SCROLL BY LINE HEIGHT FOR RTB
            //so we perform 2 more

            ((TextBoxBase)_element).LineDown();
            ((TextBoxBase)_element).LineDown();
            if (_element is RichTextBox)
            {
                ((TextBoxBase)_element).LineDown();
                ((TextBoxBase)_element).LineDown();
            }
            QueueDelegate(GetUpperBoundOnLineDown);
        }

        /// <summary>GetUpperBoundOnLineDown</summary>
        private void GetUpperBoundOnLineDown()
        {
            _expectedVerticalOffsetLineOperation = ((TextBoxBase)_element).VerticalOffset;
            ((TextBoxBase)_element).ScrollToVerticalOffset(0);
            ((TextBoxBase)_element).ScrollToHorizontalOffset(0);
            ((TextBoxBase)_element).PageDown();
            ((TextBoxBase)_element).LineDown();
            ((TextBoxBase)_element).LineDown();
            QueueDelegate(GetUpperBoundOnPageDown);
        }

        /// <summary>GetUpperBoundOnPageDown </summary>
        private void GetUpperBoundOnPageDown()
        {
            _expectedVerticalOffsetPageOperation = ((TextBoxBase)_element).VerticalOffset;
            ((TextBoxBase)_element).ScrollToVerticalOffset(0);
            ((TextBoxBase)_element).ScrollToHorizontalOffset(0);
            QueueDelegate(ScrollPageDown);
        }

        /// <summary>ScrollPageDown </summary>
        private void ScrollPageDown()
        {
            _element.Focus();
            KeyboardInput.TypeString("{PGDN}");
            QueueDelegate(VerifyPageDown);
        }

        /// <summary>VerifyPageDown </summary>
        private void VerifyPageDown()
        {
            Verifier.Verify(((TextBoxBase)_element).VerticalOffset <= _expectedVerticalOffsetPageOperation, "Expected [" +
                _expectedVerticalOffsetPageOperation.ToString() + "] >= Actual [" + ((TextBoxBase)_element).VerticalOffset.ToString() + "]", true);
            _initialOffset = ((TextBoxBase)_element).VerticalOffset;
            QueueDelegate(LineDown);
        }

        /// <summary>LineDown</summary>
        private void LineDown()
        {
            KeyboardInput.TypeString("{DOWN}");
            QueueDelegate(VerifyLineDown);
        }

        /// <summary>VerifyLineDown </summary>
        private void VerifyLineDown()
        {
            double val = ((TextBoxBase)_element).VerticalOffset - _initialOffset;
            if ((val == 0) && (_count++ < 10))
            {
                QueueDelegate(LineDown);
            }
            else
            {
                _count = 0;
                Verifier.Verify(val > 0, "Expected that the text scroll on line down Actual [" + val.ToString() + "]", true);
                Verifier.Verify(val <= _expectedVerticalOffsetLineOperation, "Expected [" +
                    _expectedVerticalOffsetLineOperation.ToString() + "] >= Actual [" + val.ToString() + "]", true);
                ((TextBoxBase)_element).PageDown();
                KeyboardInput.TypeString("{UP}");
                QueueDelegate(LineUp);
            }
        }

        /// <summary>LineUp </summary>
        private void LineUp()
        {
            _initialOffset = ((TextBoxBase)_element).VerticalOffset;
            KeyboardInput.TypeString("{UP}");
            QueueDelegate(VerifyLineUp);
        }

        /// <summary>VerifyLineUp </summary>
        private void VerifyLineUp()
        {

            double val = _initialOffset - ((TextBoxBase)_element).VerticalOffset;
            Verifier.Verify(val > 0, "Expected that the text scroll on line up Actual [" + val.ToString() + "]", true);
            Verifier.Verify(val <= _expectedVerticalOffsetLineOperation, "Expected [" +
                _expectedVerticalOffsetLineOperation.ToString() + "] >= Actual [" + val.ToString() + "]", true);
            _initialOffset = ((TextBoxBase)_element).VerticalOffset;
            KeyboardInput.TypeString("{PGUP}");
            QueueDelegate(VerifyPageUp);
        }

        /// <summary>VerifyPageUp </summary>
        private void VerifyPageUp()
        {
            double val = _initialOffset - ((TextBoxBase)_element).VerticalOffset;
            Verifier.Verify(val > 0, "Expected that the text scroll on page up Actual [" + val.ToString() + "]", true);
            Verifier.Verify(val <= _expectedVerticalOffsetPageOperation, "Expected [" +
                _expectedVerticalOffsetPageOperation.ToString() + "] >= Actual [" + val.ToString() + "]", true);
            QueueDelegate(NextCombination);
        }

        /// <summary>SetText</summary>
        private void SetText()
        {
            string str = "";
            for (int i = 0; i < 200; i++)
            {
                str += i.ToString() + ") String\r\n";
            }
            _controlWrapper.Text = str;
        }

        #region DATA.

        private FrameworkElement _element;
        private TextEditableType _editableType = null;
        private UIElementWrapper _controlWrapper;

        private double _expectedVerticalOffsetLineOperation = 0;
        private double _expectedVerticalOffsetPageOperation = 0;
        private double _initialOffset = 0;
        private int _count = 0;

        #endregion DATA.
    }
}
