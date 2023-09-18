// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Functional test cases for some TextBox rendering conditions.

using System;
using System.Collections;
using System.Drawing;
using System.Reflection;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Interop;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Imaging;

using Test.Uis.Data;
using Test.Uis.Loggers;
using Test.Uis.Management;
using Test.Uis.TestTypes;
using Test.Uis.Utils;
using Test.Uis.Wrappers;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Text/BVT/TextBoxOM/Rendering.cs $")]

namespace Test.Uis.TextEditing
{
    /// <summary>
    /// Tests HorizontalContentAlignment and VerticalContentAlignment for TB and PB
    /// </summary>
    [Test(1, "TextBox", "ContentAlignmentTest", MethodParameters = "/TestCaseType:ContentAlignmentTest")]
    [TestOwner("Microsoft"), TestTactics("580"), TestBugs("706,707,708"), TestLastUpdatedOn("April 27, 2006")]
    public class ContentAlignmentTest : ManagedCombinatorialTestCase
    {
        #region Private fields

        private HorizontalAlignment _testHorizontalAlignment = 0;
        private VerticalAlignment _testVerticalAlignment = 0;
        private FlowDirection _testFlowDirection = 0;
        private TextEditableType _editableType = null;

        private Control _testControl;
        private Bitmap _mainBitmap;

        private double _testHeight = 75d;
        private double _testWidth = 150d;

        #endregion Private fields

        #region Main flow

        /// <summary>Starts the combination</summary>
        protected override void DoRunCombination()
        {
            _testControl = (Control)_editableType.CreateInstance();
            _testControl.HorizontalContentAlignment = _testHorizontalAlignment;
            _testControl.VerticalContentAlignment = _testVerticalAlignment;
            _testControl.FlowDirection = _testFlowDirection;

            _testControl.Height = _testHeight;
            _testControl.Width = _testWidth;
            _testControl.FontSize = 14d;
            _testControl.FontWeight = System.Windows.FontWeights.Bold; //To get more black pixels
            _testControl.FontFamily = new System.Windows.Media.FontFamily("Tahoma");

            if (_testControl is TextBox)
            {
                ((TextBox)_testControl).Text = "test";
            }
            else if (_testControl is PasswordBox)
            {
                ((PasswordBox)_testControl).Password = "test";
            }

            TestElement = _testControl;
            QueueDelegate(VerifyContentAlignment);
        }

        private void VerifyContentAlignment()
        {
            Bitmap subBitmap;
            int expectedRegionWithBlackPixels, blackPixelCount;

            _mainBitmap = BitmapCapture.CreateBitmapFromElement(_testControl);
            expectedRegionWithBlackPixels = GetExpectedRenderRegion();

            for (int i = 0; i < 9; i++)
            {
                subBitmap = GetSubBitmapForRegion(i);
                blackPixelCount = BitmapUtils.CountColoredPixels(subBitmap, System.Windows.Media.Colors.Black);

                Log("# of Black pixels in region(" + i.ToString() + "): " +
                    BitmapUtils.CountColoredPixels(subBitmap, System.Windows.Media.Colors.Black).ToString());

                if (expectedRegionWithBlackPixels == i)
                {
                    if (blackPixelCount < 20)
                    {
                        Logger.Current.LogImage(_mainBitmap, "mainBitmap" + i.ToString());
                        Logger.Current.LogImage(subBitmap, "subBitmap" + i.ToString());
                        Verifier.Verify(false, "subBitmap" + i.ToString() + "_X.png expected to have black pixels", true);
                    }
                    else
                    {
                        Verifier.Verify(true, "Black pixels are identified in expected region of rendering", false);
                    }
                }
                else
                {
                    if (blackPixelCount != 0)
                    {
                        Logger.Current.LogImage(_mainBitmap, "mainBitmap" + i.ToString());
                        Logger.Current.LogImage(subBitmap, "subBitmap" + i.ToString());
                        Verifier.Verify(false, "subBitmap" + i.ToString() + "_X.png expected to have 0 black pixels", true);
                    }
                    else
                    {
                        Verifier.Verify(true, "No black pixels are identified in non-expected region of rendering", false);
                    }
                }
            }

            //Verify TestAlignment property for TextBox
            if (_testControl is TextBox)
            {
                ((TextBox)_testControl).TextAlignment = (TextAlignment)_testHorizontalAlignment;

                //Assign TestAlignment property a value different from TestHorizontalAlignment and verify that
                //TestAlignment property value wins by verifying rendering change.
                if ((_testHorizontalAlignment == HorizontalAlignment.Left) ||
                    (_testHorizontalAlignment == HorizontalAlignment.Stretch))
                {
                    ((TextBox)_testControl).TextAlignment = TextAlignment.Center;
                }
                else if (_testHorizontalAlignment == HorizontalAlignment.Center)
                {
                    ((TextBox)_testControl).TextAlignment = TextAlignment.Right;
                }
                else if (_testHorizontalAlignment == HorizontalAlignment.Right)
                {
                    ((TextBox)_testControl).TextAlignment = TextAlignment.Left;
                }
                Log("TestAlignment is set to " + ((TextBox)_testControl).TextAlignment);

                QueueDelegate(VerifyTestAlignmentWinsOverHCA);
            }
            else
            {
                QueueDelegate(NextCombination);
            }
        }

        private void VerifyTestAlignmentWinsOverHCA()
        {
            Bitmap testAlignmentBitmap;
            testAlignmentBitmap = BitmapCapture.CreateBitmapFromElement(_testControl);

            ComparisonCriteria criteria = new ComparisonCriteria();
            criteria.MaxErrorProportion = 0.02f;

            if (ComparisonOperationUtils.AreBitmapsEqualUsingCriteria(_mainBitmap, testAlignmentBitmap, criteria, false))
            {
                Logger.Current.LogImage(_mainBitmap, "BitmapWithHCASet");
                Logger.Current.LogImage(testAlignmentBitmap, "BitmapWithTASet");

                Verifier.Verify(false, "Bitmaps BitmapWithHCASet*.png and BitmapWithTASet*.png are equal when they " +
                    "expected to be different. Setting TextAlignment to a different value than HorizontalContentAlignment " +
                    "should change the alignment", true);
            }

            QueueDelegate(NextCombination);
        }

        #endregion Main flow

        #region Helper functions

        private Bitmap GetSubBitmapForRegion(int region)
        {
            double topOffset, leftOffset;
            topOffset = (region / 3) * (_testHeight / 3);
            leftOffset = (region % 3) * (_testWidth / 3);

            Rect regionRect = new Rect(leftOffset, topOffset, _testWidth / 3, _testHeight / 3);
            regionRect = BitmapUtils.AdjustBitmapSubAreaForDpi(_mainBitmap, regionRect);
            Bitmap subBitmap = BitmapUtils.CreateSubBitmap(_mainBitmap, regionRect);
            return subBitmap;
        }

        /// <summary>
        /// This function returns the expected region (the control is divided into 9 regions
        /// by 3x3 grid) where contents are rendered according to the HorizontalAlignment and
        /// VerticalAlignment values.
        /// </summary>
        /// <returns>
        /// 0 is returned for (0,0), 1 is returned for (0,1),
        /// 7 is returned for (2,1), 8 is retuned for (2,2)
        /// </returns>
        private int GetExpectedRenderRegion()
        {
            int expectedRegion = 0;

            //HorizontalAlignment
            if ((_testHorizontalAlignment == HorizontalAlignment.Left) ||
                (_testHorizontalAlignment == HorizontalAlignment.Stretch))
            {
                expectedRegion = expectedRegion + 0;

                //Adjustment for RightToLeft Flow Direction
                if (_testFlowDirection == FlowDirection.RightToLeft)
                {
                    expectedRegion = expectedRegion + 2;
                }
            }
            else if (_testHorizontalAlignment == HorizontalAlignment.Center)
            {
                expectedRegion = expectedRegion + 1;
            }
            else if (_testHorizontalAlignment == HorizontalAlignment.Right)
            {
                expectedRegion = expectedRegion + 2;

                //Adjustment for RightToLeft Flow Direction
                if (_testFlowDirection == FlowDirection.RightToLeft)
                {
                    expectedRegion = expectedRegion - 2;
                }
            }

            //VerticalAlignment
            if ((_testVerticalAlignment == VerticalAlignment.Top) ||
                (_testVerticalAlignment == VerticalAlignment.Stretch))
            {
                expectedRegion = expectedRegion + 0;
            }
            else if (_testVerticalAlignment == VerticalAlignment.Center)
            {
                expectedRegion = expectedRegion + 3;
            }
            else if (_testVerticalAlignment == VerticalAlignment.Bottom)
            {
                expectedRegion = expectedRegion + 6;
            }

            return expectedRegion;
        }

        #endregion Helper functions
    }

    /// <summary>
    /// Verifies that typing renders content as it's typed, and that
    /// it grows as expected considering MinLines and MaxLines.
    /// </summary>
    [Test(0, "TextBox", "TextBoxRenderTyping", MethodParameters = "/TestCaseType=TextBoxRenderTyping /TextToType=sample")]
    [TestOwner("Microsoft"), TestTactics("579"), TestBugs("709,710"),
     TestArgument("TextToType", "Text to type in control.")]
    public class TextBoxRenderTyping : TextBoxTestCase
    {
        #region Private data.

        private const int MaxPassCount = 5;
        private int _passCount;
        private Bitmap _lastBitmap;
        private Canvas _topCanvas;

        #endregion Private data.

        #region Configuration settings.

        private string TextToType
        {
            get { return Settings.GetArgument("TextToType", true); }
        }

        #endregion Configuration settings.

        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            SetTextBoxProperties(TestTextBox);
            MouseInput.MouseClick(TestTextBox);
            QueueDelegate(TypePass);
        }

        private void TypePass()
        {
            Log("Pass count: " + (_passCount + 1));
            Log("Typing text: [" + TextToType + "]");
            KeyboardInput.TypeString(TextToType);
            QueueDelegate(VerifyContents);
        }

        private void VerifyContents()
        {
            Log("Text: [" + TestTextBox.Text + "]");
            if (_passCount == MaxPassCount)
            {
                QueueDelegate(CheckMaxLines);
            }
            else
            {
                Bitmap current = BitmapCapture.CreateBitmapFromElement(TestControl);
                if (_lastBitmap != null)
                {
                    Log("Comparing bitmaps...");
                    ComparisonOperation op = new ComparisonOperation();
                    op.MasterImage = _lastBitmap;
                    op.SampleImage = current;

                    ComparisonResult result = op.Execute();

                    if (result.CriteriaMet)
                    {
                        Log("Bitmaps are identical - there has been " +
                            "no rendering activity");
                        Logger.Current.LogImage(_lastBitmap, "before");
                        Logger.Current.LogImage(current, "current");
                        throw new Exception("No changes found.");
                    }
                    else
                    {
                        Log("Bitmaps differ after typing");
                    }
                }
                _lastBitmap = current;
                _passCount++;

                QueueDelegate(TypePass);
            }
        }

        #region MinLines and MaxLines testing.

        private void CheckMaxLines()
        {
            TextBox testedBox;  // Box to use for reparent.

            Log("Checking that MaxLines is honored.");

            testedBox = new TextBox();
            testedBox.Name = "TestControl1";
            testedBox.AcceptsReturn = true;
            testedBox.MaxLines = 2;
            testedBox.FontFamily = new System.Windows.Media.FontFamily("Arial");
            testedBox.FontSize = 20;
            testedBox.BorderThickness = new Thickness(0);
            testedBox.BorderBrush = System.Windows.Media.Brushes.Transparent;

            _topCanvas = new Canvas();
            MainWindow.Content = _topCanvas;
            _topCanvas.Children.Add(testedBox);

            QueueDelegate(TypeForMaxLines);
        }

        private void TypeForMaxLines()
        {
            Log("Typing to hit max lines limit...");
            MouseInput.MouseClick(TestTextBox);
            KeyboardInput.TypeString("---{ENTER}---{ENTER}---");

            QueueDelegate(TypeForMaxLinesExtra);
        }

        private void TypeForMaxLinesExtra()
        {
            Log("Typing extra for MaxLines testing (this has crashed in the past)");
            KeyboardInput.TypeString("1");
            QueueDelegate(VerifyMaxLines);
        }

        private void VerifyMaxLines()
        {
            // Verifies that only two lines are visible.
            VerifyLineCount(TestTextBox, 2);

            Log("Verifying that TextBox grows when typing...");
            ResetTestTextBox();
            TestTextBox.MaxLines = 2;
            TestTextBox.TextWrapping = TextWrapping.Wrap;
            TestTextBox.MaxWidth = 250;

            QueueDelegate(TypeForGrowthX);
        }

        private void TypeForGrowthX()
        {
            Log("Retrieving values on empty text box...");
            Verifier.Verify(TestTextBox.Text == "");
            _lastWidth = TestTextBox.ActualWidth;
            _lastHeight = TestTextBox.ActualHeight;
            Log("Height before growing: " + _lastHeight);
            Log("Width before growing: " + _lastWidth);

            Log("Typing enough to make text box grow horizontally...");
            KeyboardInput.TypeString(new string('-', 32));

            QueueDelegate(VerifyGrowthX);
        }

        private void VerifyGrowthX()
        {
            const double delta = 1;
            bool isClose;

            Log("Verifying horizontal growth...");
            isClose = _lastHeight - delta <= TestTextBox.ActualHeight &&
                TestTextBox.ActualHeight <= _lastHeight + delta;
            Verifier.Verify(isClose,
                "Height remains kinda the same: " + TestTextBox.ActualHeight, true);
            Verifier.Verify(_lastWidth < TestTextBox.ActualWidth,
                "New width is greater: " + TestTextBox.ActualWidth, true);

            Log("Typing enough to trigger wrapping...");
            KeyboardInput.TypeString(new string('-', 32));

            QueueDelegate(VerifyGrowthY);
        }

        private void VerifyGrowthY()
        {
            Log("Verifying vertical growth...");
            Verifier.Verify(_lastHeight < TestTextBox.ActualHeight,
                "New height is greater: " + TestTextBox.ActualHeight, true);

            QueueDelegate(CheckInvalidValues);
        }

        private void CheckInvalidValues()
        {
            Log("Verifying that invalid values are rejected...");
            try
            {
                TestTextBox.MinLines = -1;
                throw new ApplicationException("MinLines < 0 accepted");
            }
            catch (SystemException)
            {
                Log("MinLines < 0 rejected.");
            }
            try
            {
                TestTextBox.MaxLines = 0;
                throw new ApplicationException("MaxLines = 0 accepted");
            }
            catch (SystemException)
            {
                Log("MaxLines == 0 rejected.");
            }

            QueueDelegate(CheckOnDemandScrollbar);
        }

        private void CheckOnDemandScrollbar()
        {
            Log("Testing that scrollbars appear on demand...");
            ResetTestTextBox();
            TestTextBox.MaxLines = 3;
            TestTextBox.AcceptsReturn = true;
            TestTextBox.Width = 20;
            TestTextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            QueueDelegate(TypeForOnDemandScrollbar);
        }

        private void TypeForOnDemandScrollbar()
        {
            Log("Snapping shot before scrolbars...");
            _controlSnapshot = BitmapCapture.CreateBitmapFromElement(TestTextBox);

            KeyboardInput.TypeString("{ENTER}{ENTER}");
            QueueDelegate(VerifyOnDemandScrollbar);
        }

        private void VerifyOnDemandScrollbar()
        {
            Log("Snapping shot after typing lines...");
            using (Bitmap snapshotBW = BitmapUtils.ColorToBlackWhite(_controlSnapshot))
            using (Bitmap withBars = BitmapCapture.CreateBitmapFromElement(TestTextBox))
            using (Bitmap withBarsBW = BitmapUtils.ColorToBlackWhite(withBars))
            {
                ComparisonCriteria criteria;
                ComparisonOperation operation;
                ComparisonResult result;

                criteria = new ComparisonCriteria();
                criteria.MaxErrorProportion = 0.30f;

                operation = new ComparisonOperation();
                operation.Criteria = criteria;
                operation.MasterImage = snapshotBW;
                operation.SampleImage = withBarsBW;

                result = operation.Execute();

                if (result.CriteriaMet)
                {
                    Log("Scrollbars not detected.");
                    Logger.Current.LogImage(snapshotBW, "withoutbars");
                    Logger.Current.LogImage(withBarsBW, "withbars");
                    Log(result.ToStringBrief());
                    throw new Exception("Scrollbars not detected.");
                }
            }
            Logger.Current.ReportSuccess();
        }

        #region Constraint conflicts.

        private void CheckMaxHeightIgnored()
        {
            Log("Verifying that MaxHeight is ignored when MinLines or MaxLines is set.");
            Log("Clearing all values...");
            TestTextBox.ClearValue(TextBox.MaxLinesProperty);
            TestTextBox.ClearValue(TextBox.MinLinesProperty);
            TestTextBox.ClearValue(TextBox.MaxHeightProperty);
            TestTextBox.ClearValue(TextBox.MinHeightProperty);

            Log("Setting MaxHeight to 30 and enabling line constraint mode.");
            TestTextBox.MinLines = 2;
            TestTextBox.MaxHeight = 30;

            QueueDelegate(TypeForMaxHeightIgnored);
        }

        private void TypeForMaxHeightIgnored()
        {
            KeyboardInput.TypeString("^a{DEL}this is a text box that grows. see the text box grow. grow text box, grow!");
            QueueDelegate(VerifyMaxHeightIgnored);
        }

        private void VerifyMaxHeightIgnored()
        {
            Log("Computed height: " + TestTextBox.ActualHeight);
            Verifier.Verify(TestTextBox.ActualHeight <= 29 || TestTextBox.ActualHeight >= 31,
                "Computed height was not constrainted by MaxHeight.", true);
            Logger.Current.ReportSuccess();
        }

        #endregion Constraint conflicts.

        private double _lastHeight;
        private double _lastWidth;
        private Bitmap _controlSnapshot;
        //private bool _testingConstraintsDisabledByMinLines;
        //private bool _testingMaxHeightConstraint;

        #endregion MinLines and MaxLines testing.

        #endregion Main flow.

        #region Verifications.

        /// <summary>
        /// Counts the number of lines rendered on the specified TextBox.
        /// </summary>
        /// <param name="textBox">Control to count lines in.</param>
        /// <param name="lineCount">On return, number of lines counted.</param>
        /// <returns>The snapshot of the text box content.</returns>
        internal static Bitmap CountLines(TextBox textBox, out int lineCount)
        {
            if (textBox == null)
            {
                throw new ArgumentNullException("textBox");
            }

            // Create a sub-bitmap to account for borders. Borders
            // are not calculated now, but the default should never be thicker
            // than two pixels!
            using (Bitmap b = BitmapCapture.CreateBitmapFromElement(textBox))
            using (Bitmap noBorders = BitmapUtils.CreateSubBitmap(
                    b, new Rectangle(2, 2, b.Width - 4, b.Height - 4)))
            {
                Bitmap noBordersBW = BitmapUtils.ColorToBlackWhite(noBorders);
                lineCount = BitmapUtils.CountTextLines(noBordersBW);
                return noBordersBW;
            }
        }

        /// <summary>
        /// Verifies the number of lines rendered on the specified TextBox.
        /// </summary>
        /// <param name="textBox">Control to count lines in.</param>
        /// <param name="expectedLineCount">The expected number of lines.</param>
        internal static void VerifyLineCount(TextBox textBox, int expectedLineCount)
        {
            int lineCount;      // Number of lines counted on text box.
            Bitmap snapshot;    // Snapshot for control.

            const string SnapshotFileName = "textbox-lines";

            using (snapshot = CountLines(textBox, out lineCount))
            {
                if (lineCount == expectedLineCount)
                {
                    Logger.Current.Log("Found the expected " + lineCount + " lines.");
                }
                else
                {
                    string message; // Message to display and throw.

                    message = "Expected " + expectedLineCount + " lines, found " + lineCount;
                    Logger.Current.Log(message);
                    Logger.Current.LogImage(snapshot, SnapshotFileName);
                    throw new Exception(message);
                }
            }
        }

        #endregion Verifications.

        #region Helper methods.

        /// <summary>
        /// Resets the Text, MinLines, MaxLines, MinHeight, MaxHeight,
        /// Wrap and AcceptsReturn properties.
        /// </summary>
        private void ResetTestTextBox()
        {
            TestTextBox.Clear();
            TestTextBox.ClearValue(TextBox.MinLinesProperty);
            TestTextBox.ClearValue(TextBox.MaxLinesProperty);
            TestTextBox.ClearValue(TextBox.MinHeightProperty);
            TestTextBox.ClearValue(TextBox.MaxHeightProperty);
            TestTextBox.ClearValue(TextBox.MaxWidthProperty);
            TestTextBox.ClearValue(TextBox.WidthProperty);
            TestTextBox.ClearValue(TextBox.TextWrappingProperty);
            TestTextBox.ClearValue(TextBox.AcceptsReturnProperty);
        }

        #endregion Helper methods.
    }

    /// <summary>
    /// Verifies that the GetRectFromTextPointer API returns
    /// reasonable results for different positions inside the
    /// TextBox.
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("578"),
     TestArgument("Text", "Used for the Text of the TextBox.")]
    public class TextBoxGetRectFromTextPointer : TextBoxTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            if (Text != String.Empty)
            {
                TestTextBox.Text = Text;
            }
            SetTextBoxProperties(TestTextBox);
            VerifyInvalidCalls();

            QueueHelper.Current.QueueDelegate(AfterLayout);
        }

        private void AfterLayout()
        {
            VerifyValidCalls();
            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.

        #region Verifications.

        private void VerifyInvalidCalls()
        {
            TextBox box = TestTextBox;
            System.Diagnostics.Debug.Assert(box != null);
            try
            {
                GetRectFromTextPointer(box, null, LogicalDirection.Forward);
                throw AcceptedException("null TextPointer");
            }
            catch (ArgumentException)
            {
                LogRejected("null TextPointer");
            }
        }

        private static Rect GetRectFromTextPointer(TextBox box,
            TextPointer position, LogicalDirection direction)
        {
            return position.GetCharacterRect(direction);
        }

        private void VerifyValidCalls()
        {
            Rect resultStart, resultEnd, resultMid;
            TextBox box;

            box = TestTextBox;
            System.Diagnostics.Debug.Assert(box != null);

            Log("Verifying text for beginning of content...");
            resultStart = GetRectFromTextPointer(box, Test.Uis.Utils.TextUtils.GetTextBoxStart(box), LogicalDirection.Forward);
            Log("Result for start position: " + resultStart);
            Verifier.Verify(!resultStart.IsEmpty, "Result is not empty", true);

            Log("Verifying text for end of content...");
            resultEnd = GetRectFromTextPointer(box, Test.Uis.Utils.TextUtils.GetTextBoxEnd(box), LogicalDirection.Backward);
            Log("Result for end position: " + resultEnd);
            Verifier.Verify(!resultEnd.IsEmpty, "Result is not empty", true);

            TextPointer nav = Test.Uis.Utils.TextUtils.GetTextBoxStart(box);
            for (int i = 0; i < 2; i++)
            {
                nav = nav.GetNextInsertionPosition(LogicalDirection.Forward);
            }
            resultMid = GetRectFromTextPointer(box, nav, LogicalDirection.Forward);
            Log("Result for middle position: " + resultMid);
            Verifier.Verify(!resultMid.IsEmpty, "Result is not empty", true);

            Verifier.Verify(resultStart.Left < resultMid.Left,
                "The start position is to the left of the mid position", true);
            Verifier.Verify(resultMid.Left < resultEnd.Left,
                "The mid position is to the left of the end position", true);
            Verifier.Verify(resultStart.Left < resultEnd.Left,
                "The start position is to the left of the end position", true);

            Rect resultNewline;
            Rect resultBeforeNewline;
            Log("Looking for a newline in the text...");
            string text = nav.GetTextInRun(LogicalDirection.Forward);
            int newlineDelta = text.IndexOf(Environment.NewLine);
            if (newlineDelta == -1)
            {
                Log("Text from navigator: [" + text + "]");
                throw new Exception("New line not found in text.");
            }

            Log("Verifying the position for the \\r character...");

            for (int i = 0; i < newlineDelta; i++)
            {
                nav = nav.GetNextInsertionPosition(LogicalDirection.Forward);
            }
            resultBeforeNewline = GetRectFromTextPointer(box, nav, LogicalDirection.Backward);
            Log("Result for before newline: " + resultBeforeNewline);
            Verifier.Verify(!resultBeforeNewline.IsEmpty, "Result is not empty", true);

            Log("Verifying the position for the \\n character...");
            resultNewline = GetRectFromTextPointer(box, nav, LogicalDirection.Forward);
            Log("Result for newline: " + resultNewline);
            Verifier.Verify(!resultNewline.IsEmpty, "Result is not empty", true);

            Verifier.Verify(resultNewline == resultBeforeNewline,
                "Results match", true);
        }

        #endregion Verifications.
    }

    /// <summary>
    /// Verifies that the GetTextPointerFromPoint API returns
    /// reasonable results for different points inside the
    /// TextBox.
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("104"), TestBugs("636"),
     TestArgument("SnapToText", "Boolean for whether to snap to text."),
     TestArgument("SourcePointX", "Point to query (x coordinate)."),
     TestArgument("SourcePointY", "Point to query (x coordinate)."),
     TestArgument("Text", "Used for the Text of the TextBox."),
     TestArgument("TextPointerLength", "Expected length to position, -1 for null")]
    public class TextBoxGetTextPointerFromPoint : TextBoxTestCase
    {
        #region Arguments.

        private bool SnapToText
        {
            get { return Settings.GetArgumentAsBool("SnapToText", true); }
        }

        private System.Windows.Point SourcePoint
        {
            get
            {
                double x = double.Parse(
                    Settings.GetArgument("SourcePointX", true),
                    System.Globalization.CultureInfo.InvariantCulture);
                double y = double.Parse(
                    Settings.GetArgument("SourcePointY", true),
                    System.Globalization.CultureInfo.InvariantCulture);
                return new System.Windows.Point(x, y);
            }
        }

        private int TextPointerLength
        {
            get { return Settings.GetArgumentAsInt("TextPointerLength", true); }
        }

        #endregion Arguments.

        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            if (Text != String.Empty)
            {
                TestTextBox.Text = Text;
            }
            SetTextBoxProperties(TestTextBox);
            //VerifyInvalidCalls();

            QueueHelper.Current.QueueDelegate(AfterLayout);
        }

        private void AfterLayout()
        {
            VerifyValidCalls();
            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.

        #region Verifications.

        private void VerifyInvalidCalls()
        {
            TextBox box = TestTextBox;

            LogicalDirection direction = LogicalDirection.Forward;

            System.Diagnostics.Debug.Assert(box != null);
            try
            {
                GetTextPointerFromPoint(
                    new System.Windows.Point(double.NaN, 0), true, out direction);
                throw AcceptedException("NaN point");
            }
            catch (ArgumentException)
            {
                LogRejected("NaN point");
            }
        }

        private TextPointer GetTextPointerFromPoint(System.Windows.Point point,
            bool snapToText, out LogicalDirection direction)
        {
            MethodInfo reflection_GetTextPositionFromPoint = typeof(TextBox).GetMethod("GetTextPositionFromPoint", BindingFlags.NonPublic | BindingFlags.Instance);
            if (reflection_GetTextPositionFromPoint == null)
            {
                throw new Exception("TextBox.GetTextPositionFromPoint method is not accessible");
            }

            TextPointer position =
                (TextPointer)reflection_GetTextPositionFromPoint.Invoke(TestTextBox, new object[2] { point, snapToText });
            direction = position.LogicalDirection;
            return position;
        }

        private void VerifyValidCalls()
        {
            LogicalDirection direction;     // Output direction for query.
            System.Windows.Point p;         // Point to query.
            TextPointer position;          // Position for point.
            bool snapToText;                // Query to closest position.
            int textPositionLength;         // Expected length to position.
            string textToPosition;          // String to position.

            Log("Verifying valid GetTextPointerFromPoint call...");

            // Read values for test case.
            p = SourcePoint;
            snapToText = SnapToText;
            textPositionLength = TextPointerLength;

            // Log values being used.
            Log("Point:      " + p);
            Log("SnapToText: " + SnapToText);
            Log("Expected string length to resulting position: " + textPositionLength);

            // Verify the position.
            position = GetTextPointerFromPoint(p, snapToText, out direction);
            if (position == null)
            {
                Log("Resulting position is null.");
                Verifier.Verify(textPositionLength == -1,
                    "Position is as expected", true);
            }
            else
            {
                textToPosition = position.GetTextInRun(LogicalDirection.Backward);
                Log("Text to position:        [" + textToPosition + "]");
                Log("Text length to position: " + textToPosition.Length);
                if (textPositionLength != textToPosition.Length)
                {
                    LogImageForPoint(p);
                }
                Verifier.Verify(textToPosition.Length == textPositionLength,
                    "Text length matches expected value.", true);
            }
        }

        /// <summary>
        /// Logs the image of the tested control, marking the specified
        /// point.
        /// </summary>
        /// <param name="point">Point to mark.</param>
        private void LogImageForPoint(System.Windows.Point point)
        {
            Bitmap image;   // Captured image of text control.
            int x;          // X-coordinate of pixel to mark.
            int y;          // Y-coordinate of pixel to mark.

            image = BitmapCapture.CreateBitmapFromElement(TestTextBox);

            // Fix out-of-bound points by moving them into the image.
            x = (int)point.X;
            if (x < 0) x = 0;
            if (x >= image.Width) x = image.Width - 1;

            y = (int)point.Y;
            if (y < 0) y = 0;
            if (y > image.Height) y = image.Height - 1;

            // Set the pixel to red and log the image.
            image.SetPixel(x, y, System.Drawing.Color.Red);
            Logger.Current.LogImage(image, "PositionPoint");
        }

        #endregion Verifications.
    }

    /// <summary>
    /// Verifies that the line index can be found from a specificposition
    /// using TextPointer and through API calls.
    /// **********This test case is sensitive to the TestCaseData being used.**********
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("577"), TestLastUpdatedOn("June 27, 2006")]
    public class TextBoxLineFromPosition : TextBoxTestCase
    {
        #region Private data.

        private const string FixedFontFamily = "Courier New";
        private const int FixedFontSize = 14;
        private const int FixedWidth = 150;
        private int _dataInstanceIndex;

        /// <summary>Holds structured ttst case data.</summary>
        struct TestCaseData
        {
            #region Private fields.

            private readonly string _text;
            private readonly bool _wrap;
            private readonly int[] _offsets;
            private readonly int[] _lineIndexes;
            private static TestCaseData[] s_instances;

            #endregion Private fields.

            #region Internal properties.

            internal string Text { get { return this._text; } }
            internal bool Wrap { get { return this._wrap; } }
            internal int[] Offsets { get { return this._offsets; } }
            internal int[] LineIndexes { get { return this._lineIndexes; } }

            /// <summary>Provides access to the actual instances.</summary>
            internal static TestCaseData[] Instances
            {
                get
                {
                    if (s_instances == null)
                    {
                        s_instances = new TestCaseData[]
                        {
                            new TestCaseData("1\r\n2\r\n3", true,
                                new int[] { 0, 1, 3, 5, 6},
                                new int[] { 0, 0, 1, 1, 2}),
                            new TestCaseData("AAAAAAAAAAAAAAAABB", true,
                                new int[] { 8, 16, 17},
                                new int[] { 0, 1, 1}),
                            new TestCaseData("AAAAAAAAAAAAAAAABB", false,
                                new int[] { 8, 16, 17},
                                new int[] { 0, 0, 0})
                        };
                    }
                    System.Diagnostics.Debug.Assert(s_instances != null);
                    return s_instances;
                }
            }

            #endregion Internal properties.

            #region Internal constructor.

            internal TestCaseData(string text, bool wrap, int[] offsets, int[] lineIndexes)
            {
                System.Diagnostics.Debug.Assert(text != null);
                System.Diagnostics.Debug.Assert(offsets != null);
                System.Diagnostics.Debug.Assert(lineIndexes != null);
                System.Diagnostics.Debug.Assert(offsets.Length > 0);
                System.Diagnostics.Debug.Assert(offsets.Length == lineIndexes.Length);

                this._text = text;
                this._wrap = wrap;
                this._offsets = offsets;
                this._lineIndexes = lineIndexes;
            }

            #endregion Internal constructor.
        }

        #endregion Private data.

        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            _dataInstanceIndex = 0;
            SetTestCaseData();
        }

        private void SetTestCaseData()
        {
            string text;
            bool wrap;

            text = TestCaseData.Instances[_dataInstanceIndex].Text;
            wrap = TestCaseData.Instances[_dataInstanceIndex].Wrap;

            Log("Setting TextBox.Text: [" + text + "]");
            Log("Setting TextBox.Wrap: [" + wrap + "]");

            TestTextBox.Text = text;
            TestTextBox.TextWrapping = wrap ? TextWrapping.Wrap : TextWrapping.NoWrap;

            TestTextBox.FontFamily = new System.Windows.Media.FontFamily(FixedFontFamily);
            TestTextBox.FontSize = FixedFontSize;
            TestTextBox.Width = FixedWidth;

            QueueHelper.Current.QueueDelegate(CheckTestCaseData);
        }

        private void CheckTestCaseData()
        {
            TestCaseData data; //Dataset being tested.

            data = TestCaseData.Instances[_dataInstanceIndex];
            for (int i = 0; i < data.Offsets.Length; i++)
            {
                //if the char offset is between \r\n, GetLineIndexFromCharacterIndex normalizes and
                //returns the line index of the following line. Hence this adjustment is necessary
                //in this test case.
                if (data.Text.Substring(data.Offsets[i], 1) == "\n")
                {
                    Verifier.Verify(TestTextBox.GetLineIndexFromCharacterIndex(data.Offsets[i] - 1) == data.LineIndexes[i],
                        "Verifying with GetLineIndexFromCharacterIndex() method", true);
                }
                else
                {
                    Verifier.Verify(TestTextBox.GetLineIndexFromCharacterIndex(data.Offsets[i]) == data.LineIndexes[i],
                        "Verifying with GetLineIndexFromCharacterIndex() method", true);
                }

                Log("Verifying with TextPointers...");
                VerifyPositionAtLine(data.Offsets[i], data.LineIndexes[i]);
            }

            // Advance to the next data instance, if available.
            _dataInstanceIndex++;
            System.Diagnostics.Debug.Assert(_dataInstanceIndex <= TestCaseData.Instances.Length);
            if (_dataInstanceIndex == TestCaseData.Instances.Length)
            {
                Logger.Current.ReportSuccess();
            }
            else
            {
                SetTestCaseData();
            }
        }

        #endregion Main flow.

        #region Verifications.

        /// <summary>
        /// Verifies that the position at the specified offset can
        /// be found in the given line index of the line results
        /// provided.
        /// </summary>
        /// <param name="positionOffset">Offset from start of text container.</param>
        /// <param name="expectedLineIndex">Expected line index for position.</param>
        private void VerifyPositionAtLine(int positionOffset, int expectedLineIndex)
        {
            TextPointer testPosition;  // Position being sought.

            Log("Looking for position at offset " + positionOffset);
            testPosition = Test.Uis.Utils.TextUtils.GetTextBoxStart(TestTextBox);
            testPosition = testPosition.GetPositionAtOffset(positionOffset);
            testPosition = testPosition.GetPositionAtOffset(0, LogicalDirection.Forward);

            Verifier.Verify(LineContainsPosition(expectedLineIndex, testPosition),
                "Position found in correct line - expected " + expectedLineIndex,
                true);
        }

        /// <summary>
        /// Verifies whether the specified line contains a given position.
        /// </summary>
        /// <param name="lineIndex">Line to look into.</param>
        /// <param name="position">Positiong being sought.</param>
        /// <returns>true if the position is in the line, false otherwise.</returns>
        private bool LineContainsPosition(int lineIndex, TextPointer position)
        {
            TextRange lineRange;        // Line range queried.

            System.Diagnostics.Debug.Assert(lineIndex >= 0);
            System.Diagnostics.Debug.Assert(position != null);

            lineRange = GetLineRange(lineIndex);
            Log("Looking for position in line with contents [" + lineRange.Text + "]");
            if (lineRange.End.CompareTo(position) == 0 && lineRange.Text.EndsWith("\r\n"))
            {
                return false;
            }
            else
            {
                return lineRange.Contains(position);
            }
        }

        private TextRange GetLineRange(int lineIndex)
        {
            TextPointer startPosition, position1, position2;
            startPosition = Test.Uis.Utils.TextUtils.GetTextBoxStart(TestTextBox);
            startPosition = startPosition.GetPositionAtOffset(0, LogicalDirection.Forward);
            position1 = startPosition.GetLineStartPosition(lineIndex);
            position2 = startPosition.GetLineStartPosition(lineIndex + 1);
            if (position2 == null)
            {
                position2 = Test.Uis.Utils.TextUtils.GetTextBoxEnd(TestTextBox);
            }

            return (new TextRange(position1, position2));
        }

        #endregion Verifications.
    }

    /// <summary>
    /// Verifies that the text for a given line can be retrieved,
    /// given its index through TextPointer and through API calls.
    /// </summary>
    [Test(0, "TextBox", "TextBoxTextFromLine", MethodParameters = "/TestCaseType:TextBoxTextFromLine")]
    [TestOwner("Microsoft"), TestTactics("576"), TestLastUpdatedOn("June 27, 2006")]
    public class TextBoxTextFromLine : TextBoxTestCase
    {
        #region Private data.

        struct TestCaseData
        {
            #region Internal properties.

            /// <summary>Text in lines.</summary>
            internal readonly string[] Lines;
            /// <summary>Text in control.</summary>
            internal readonly string Text;
            /// <summary>Whether text should wrap.</summary>
            internal readonly bool Wrap;
            /// <summary>Whether text should wrap.</summary>
            internal static TestCaseData[] Cases = new TestCaseData[] {
                new TestCaseData(false, "ab\r\ncd", new string[] { "ab\r\n", "cd" } ),
                new TestCaseData(true, "ab\r\ncd", new string[] { "ab\r\n", "cd" } ),
                new TestCaseData(true, "AAAAAAAAAAAAAAAABB", new string[] { "AAAAAAAAAAAAAAAA", "BB" } ),
                new TestCaseData(false, "AAAAAAAAAAAAAAAABB", new string[] { "AAAAAAAAAAAAAAAABB" } ),
                new TestCaseData(true, "AAAAAAAAAAAAAAAABBBBBBBBBBBBBBBBCCCCCCCCCCCCCCCCDDDDDDDDDDDDDDDD",
                    new string[] { "AAAAAAAAAAAAAAAA", "BBBBBBBBBBBBBBBB", "CCCCCCCCCCCCCCCC", "DDDDDDDDDDDDDDDD" } )
            };
            #endregion Internal properties.

            #region Private methods.

            private TestCaseData(bool wrap, string text, string[] lines)
            {
                System.Diagnostics.Debug.Assert(text != null);
                System.Diagnostics.Debug.Assert(lines != null);

                this.Wrap = wrap;
                this.Text = text;
                this.Lines = lines;
            }

            #endregion Private methods.
        }

        private const string FixedFontFamily = "Courier New";
        private const int FixedFontSize = 14;
        private const int FixedWidth = 150;

        private int _dataInstanceIndex;

        #endregion Private data.

        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            _dataInstanceIndex = 0;
            SetTestCaseData();
        }

        private void SetTestCaseData()
        {
            string text;
            bool wrap;

            text = TestCaseData.Cases[_dataInstanceIndex].Text;
            wrap = TestCaseData.Cases[_dataInstanceIndex].Wrap;

            Log("Setting TextBox.Text: [" + text + "]");
            Log("Setting TextBox.Wrap: [" + wrap + "]");

            TestTextBox.Text = text;
            TestTextBox.TextWrapping = wrap ? TextWrapping.Wrap : TextWrapping.NoWrap; ;

            TestTextBox.FontFamily = new System.Windows.Media.FontFamily(FixedFontFamily);
            TestTextBox.FontSize = FixedFontSize;
            TestTextBox.Width = FixedWidth;

            //The padding is different between Win8(0,0,0,0) and win7(1,1,1,1), so minus 2 pixels on the textbox width on Win8
            OperatingSystem os = Environment.OSVersion;
            Version ver = os.Version;
            if (ver.Major > 6 || ((6 == ver.Major) && ver.Minor > 1))
            {
                TestTextBox.Width = 148;
            }

            QueueHelper.Current.QueueDelegate(CheckTestCaseData);
        }

        private void CheckTestCaseData()
        {
            TestCaseData data;      // Dataset being tested.
            TextRange[] lineRanges; // Text ranges for laid out lines.
            TextRange lineRange;    // Text range of each line. lineResults.Length == data.Lines.Length

            Log("Verifying contents of Line through TextPointer...");
            data = TestCaseData.Cases[_dataInstanceIndex];
            lineRanges = GetLineRanges();
            Verifier.Verify(lineRanges.Length == data.Lines.Length,
                "Lines expected (" + data.Lines.Length + ") Match lines found(" + lineRanges.Length + ").", true);
            for (int i = 0; i < data.Lines.Length; i++)
            {
                lineRange = lineRanges[i];
                Log("Line " + i + " has text [" + lineRange.Text + "], " + "Expected [" + data.Lines[i] + "]");
                Verifier.Verify(lineRange.Text == data.Lines[i]);
            }

            Log("Verifying contents of Line through TextBox line API calls...");
            Verifier.Verify(TestTextBox.LineCount == data.Lines.Length, "Verifying line count through LineCount property", true);
            for (int i = 0; i < TestTextBox.LineCount; i++)
            {
                Verifier.Verify(TestTextBox.GetLineText(i) == data.Lines[i],
                    "Verifying contents of line #" + i.ToString() + " through GetLineText() method", true);
            }

            // Advance to the next data instance, if available.
            _dataInstanceIndex++;
            System.Diagnostics.Debug.Assert(_dataInstanceIndex <= TestCaseData.Cases.Length);
            if (_dataInstanceIndex == TestCaseData.Cases.Length)
            {
                Logger.Current.ReportSuccess();
            }
            else
            {
                SetTestCaseData();
            }
        }

        private TextRange[] GetLineRanges()
        {
            ArrayList results;          // Results accumulated
            LogicalDirection dir;       // Direction for positions
            TextPointer loopPosition;   // Position to start looping from.
            TextPointer linePosition;   // TextPointer moved around.

            dir = LogicalDirection.Forward;
            results = new ArrayList();
            loopPosition = TestWrapper.Start;
            do
            {
                TextPointer startPointer, endPointer;
                linePosition = loopPosition.GetPositionAtOffset(0, dir);
                startPointer = linePosition.GetLineStartPosition(0);
                endPointer = linePosition.GetLineStartPosition(1);

                if (endPointer != null)
                {
                    results.Add(new TextRange(startPointer, endPointer));
                    loopPosition = endPointer.GetPositionAtOffset(0);
                }
                else
                {
                    results.Add(new TextRange(startPointer, TestWrapper.End));
                    break;
                }
            } while (loopPosition.GetOffsetToPosition(TestWrapper.End) > 0);

            return (TextRange[])results.ToArray(typeof(TextRange));
        }

        #endregion Main flow.
    }

    /// <summary>
    /// Calls UpdateLayout on TextBox/RichTextBox/PasswordBox.
    /// Coverage for Regression_Bug580
    /// </summary>
    [Test(0, "TextBox", "TextBoxUpdateLayout", MethodParameters = "/TestCaseType:TextBoxUpdateLayout")]
    [TestOwner("Microsoft"), TestTactics("575"), TestBugs("580"), TestLastUpdatedOn("June 27, 2006")]
    public class TextBoxUpdateLayout : ManagedCombinatorialTestCase
    {
        #region Private fields

        private TextEditableType _editableType = null;
        private Control _control;
        private UIElementWrapper _wrapper;
        private StackPanel _panel;
        private Grid _grid;
        private Button _button;
        private Bitmap _before,_after,_differences;
        private ComparisonCriteria _criteria;

        #endregion Private fields

        /// <summary>Starts the combination</summary>
        protected override void DoRunCombination()
        {
            _panel = new StackPanel();

            _grid = new Grid();
            _grid.Background = System.Windows.Media.Brushes.Aquamarine;
            _grid.Width = _grid.Height = 100;
            _panel.Children.Add(_grid);

            //no functionality to this button but adding this to keep the test case close the repro scenario.
            _button = new Button();
            _button.Content = "Button";
            _panel.Children.Add(_button);

            TestElement = _panel;
            QueueDelegate(AddEditingControl);
        }

        private void AddEditingControl()
        {
            _before = BitmapCapture.CreateBitmapFromElement(_grid);

            _control = (Control)_editableType.CreateInstance();
            _control.Width = _control.Height = 10d;
            _control.Margin = new Thickness(10, 10, 0, 0);
            _control.HorizontalAlignment = HorizontalAlignment.Left;
            _control.VerticalAlignment = VerticalAlignment.Top;
            if (_control is TextBoxBase)
            {
                ((TextBoxBase)_control).VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            }
            if (_control is TextBox)
            {
                ((TextBox)_control).TextWrapping = TextWrapping.Wrap;
            }

            _wrapper = new UIElementWrapper(_control);
            _wrapper.Text = "Editing";

            _grid.Children.Add(_control);

            _control.UpdateLayout();
            Verifier.Verify(_control.ActualWidth == 10d,
                "Verify that layout information (ActualWidth) is calculated after UpdateLayout call", true);
            Verifier.Verify(_control.ActualHeight == 10d,
                "Verify that layout information (ActualHeight) is calculated after UpdateLayout call", true);

            _control.Focus();

            QueueDelegate(VerifyResult);
        }

        private void VerifyResult()
        {
            Verifier.Verify(_control.IsFocused, "Verify that focus is on the control", true);

            _after = BitmapCapture.CreateBitmapFromElement(_grid);

            _criteria = new ComparisonCriteria();
            _criteria.MaxErrorProportion = 0.0099f; //Atleast 1% of the pixels should change.

            if (ComparisonOperationUtils.AreBitmapsEqualUsingCriteria(_before, _after, out _differences, _criteria, false))
            {
                Logger.Current.LogImage(_before, "before");
                Logger.Current.LogImage(_after, "after");
                Logger.Current.LogImage(_differences, "differences");

                Verifier.Verify(false, "Expected the images to be different but they are equal", true);
            }

            Log("Verified that rendering changed after calling UpdateLayout");

            QueueDelegate(NextCombination);
        }
    }

    /// <summary>
    /// Verifies bugs related to textbox rendering
    /// 1. Regression_Bug200 Compare inFocus TextBox and outOfFocus TextBox
    /// 2. Regression_Bug201 Check Background and Foreground property of TextBox
    /// </summary>
    [Test(0, "TextBox", "TextBoxRenderingBugs", MethodParameters = "/TestCaseType:TextBoxRenderingBugs",Disabled=true)]
    [TestOwner("Microsoft"), TestTactics("574"), TestBugs("200, 201"), TestLastUpdatedOn("April 25, 2007")]
    public class TextBoxRenderingBugs : CustomTestCase
    {
        #region Private Data

        private TextBox _tbFocus,_tbNonFocus,_testTB;
        private Canvas _canvasPanel;
        private Bitmap _windowBmp,_tbFocusBmp,_tbNonFocusBmp;
        private Bitmap _tbColorBmpXaml,_tbColorBmpCode;

        #endregion

        #region Override Members.

        /// <summary>Overridden Entry Point</summary>
        public override void RunTestCase()
        {
            Log("Verifying that Regression_Bug200 (Comparison between in-focus and out-focus Textbox) doesnt regress");
            _canvasPanel = new Canvas();
            _tbFocus = new TextBox();
            _tbNonFocus = new TextBox();
            SetTBProps(_tbFocus);
            SetTBProps(_tbNonFocus);
            _tbFocus.SetValue(Canvas.TopProperty, 100d);
            _tbFocus.SetValue(Canvas.LeftProperty, 0d);

            _tbNonFocus.SetValue(Canvas.TopProperty, 300d);
            _tbNonFocus.SetValue(Canvas.LeftProperty, 0d);

            _canvasPanel.Children.Add(_tbFocus);
            _canvasPanel.Children.Add(_tbNonFocus);
            MainWindow.Content = _canvasPanel;
            MainWindow.Left = 100d;

            QueueDelegate(DoInputAction);
        }

        private void SetTBProps(TextBox tb)
        {
            tb.Height = 100;
            tb.Width = 200;
        }

        private void DoInputAction()
        {
            //To make one text box infocus
            MouseInput.MouseClick(_tbFocus);
            QueueDelegate(CaptureSnapShot);
        }

        private void CaptureSnapShot()
        {
            //snapshot of the whole window
            _windowBmp = BitmapCapture.CreateBitmapFromWindow(MainWindow);
            _tbFocusBmp = BitmapCapture.CreateBitmapFromElement(_tbFocus);
            _tbNonFocusBmp = BitmapCapture.CreateBitmapFromElement(_tbNonFocus);
            Logger.Current.LogImage(_tbFocusBmp, "tbFocus");
            Logger.Current.LogImage(_tbNonFocusBmp, "tbNonFocus");
            VerifyRendering();
        }

        /// <summary>Compares the inFocus and outFocus textbox.</summary>
        private void VerifyRendering()
        {
            ComparisonCriteria criteria;
            ComparisonOperation operation;
            ComparisonResult result;

            criteria = new ComparisonCriteria();
            //criteria.MaxErrorProportion = 0.0015f; //TC fails below .001 [Classic theme]
            //In Aero theme since focused textbox has a glowing blue border.
            criteria.MaxErrorProportion = 0.05f;

            operation = new ComparisonOperation();
            operation.Criteria = criteria;
            operation.MasterImage = _tbFocusBmp;
            operation.SampleImage = _tbNonFocusBmp;

            result = operation.Execute();
            if (result.CriteriaMet)
            {
                Log("Regression_Bug200 didnt regress");
                TestBackgroundForegroundXaml();
            }
            else
            {
                Logger.Current.ReportResult(false, "InFocus and OutFocus textbox differ more than expected", false);
            }
        }

        private void TestBackgroundForegroundXaml()
        {
            Log("Verifying that Regression_Bug201 (Background and Foreground properties of Textbox) doesnt regress [XamlVersion]");
            string xamlString = "<DockPanel xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' Background='White'>" +
                "<TextBox Name='TBRegression_Bug201' Width='500' Height='500' FontSize='32' FontFamily='Tahoma' FontWeight='bold' Background='Red' Foreground='Blue' " +
                "AcceptsReturn='true' TextWrapping='Wrap'>Test for Background and Foreground properties</TextBox> </DockPanel>";
            ActionItemWrapper.SetMainXaml(xamlString);
            QueueDelegate(CheckBackgroundForegroundXaml);
        }

        private void CheckBackgroundForegroundXaml()
        {
            _testTB = ElementUtils.FindElement(MainWindow, "TBRegression_Bug201") as TextBox;
            _tbColorBmpXaml = BitmapCapture.CreateBitmapFromElement(_testTB);
            Logger.Current.LogImage(_tbColorBmpXaml, "tbColorBmpXaml");
            Rect textRect1 = _testTB.GetRectFromCharacterIndex(0);
            Rect textRect2 = _testTB.GetRectFromCharacterIndex(1);
            VerifyTextAndBackgroundColor(_tbColorBmpXaml, (int)(textRect1.X + textRect2.X) / 2,
                (int)(textRect1.Y + (textRect1.Height) / 2),
                250, 250,
                Color.Blue, Color.Red);
            TestBackgroundForegroundCode();
        }

        private void TestBackgroundForegroundCode()
        {
            Log("Verifying that Regression_Bug201 (Background and Foreground properties of Textbox) doesnt regress [CodeVersion]");

            DockPanel dpanel = new DockPanel();
            dpanel.Background = System.Windows.Media.Brushes.White;

            _testTB = new TextBox();
            _testTB.Foreground = System.Windows.Media.Brushes.Blue;
            _testTB.Background = System.Windows.Media.Brushes.Red;
            _testTB.Text = "Test for Background and Foreground properties";
            _testTB.FontSize = 32;
            _testTB.FontFamily = new System.Windows.Media.FontFamily("Tahoma");
            _testTB.FontWeight = FontWeights.Bold;
            _testTB.TextWrapping = TextWrapping.Wrap;
            _testTB.Width = 500;
            _testTB.Height = 500;
            _testTB.AcceptsReturn = true;
            _testTB.Name = "TBRegression_Bug201";

            dpanel.Children.Add(_testTB);
            MainWindow.Content = dpanel;
            QueueDelegate(CheckBackgroundForegroundCode);
        }

        private void CheckBackgroundForegroundCode()
        {
            _testTB = ElementUtils.FindElement(MainWindow, "TBRegression_Bug201") as TextBox;
            _tbColorBmpCode = BitmapCapture.CreateBitmapFromElement(_testTB);
            Logger.Current.LogImage(_tbColorBmpCode, "tbColorBmpCode");
            Rect textRect1 = _testTB.GetRectFromCharacterIndex(0);
            Rect textRect2 = _testTB.GetRectFromCharacterIndex(1);
            VerifyTextAndBackgroundColor(_tbColorBmpXaml, (int)(textRect1.X + textRect2.X) / 2,
                (int)(textRect1.Y + (textRect1.Height) / 2),
                250, 250,
                Color.Blue, Color.Red);

            Logger.Current.ReportSuccess();
        }

        private void VerifyTextAndBackgroundColor(Bitmap controlBmp, int textXPos, int textYPos,
            int backGroundXPos, int backGroundYPos, Color expTextColor, Color expBackgroundColor)
        {
            Color textColor = controlBmp.GetPixel((int)(textXPos * controlBmp.HorizontalResolution / 96.0f), (int)(textYPos * controlBmp.HorizontalResolution / 96.0f));
            Log("TextColor: R(" + textColor.R + "), G(" + textColor.G + "), B(" + textColor.B + ")");
            Verifier.Verify((textColor.R == expTextColor.R) &&
                (textColor.G == expTextColor.G) && (textColor.B == expTextColor.B),
                "Verifying the text color", true);

            Color backGroundColor = controlBmp.GetPixel(backGroundXPos, backGroundYPos);

            Log("TextColor: R(" + backGroundColor.R + "), G(" + backGroundColor.G + "), B(" + backGroundColor.B + ")");
            Verifier.Verify((backGroundColor.R == expBackgroundColor.R) &&
                (backGroundColor.G == expBackgroundColor.G) && (backGroundColor.B == expBackgroundColor.B),
                "Verifying the background color", true);

            return;
        }

        #endregion
    }

    /// <summary>
    /// Verifies static rendering in TextBoxView
    /// </summary>
    [Test(0, "TextBox", "TextBoxViewRendering", MethodParameters = "/TestCaseType:TextBoxViewRendering",Disabled=true)]
    [TestOwner("Microsoft"), TestTactics("572"), TestWorkItem("97"), TestLastUpdatedOn("April 25, 2007")]
    public class TextBoxViewRendering : CustomTestCase
    {
        #region Main flow

        /// <summary>Overridden Entry Point</summary>
        public override void RunTestCase()
        {
            StackPanel sp = new StackPanel();
            _tb = new TextBox();
            _tbl = new TextBlock();

            _tbl.FontSize = _tb.FontSize = 24;
            _tbl.Width = _tb.Width = 700;
            _tbl.Height = _tb.Height = 150;
            _tb.Margin = _tb.Padding = _tb.BorderThickness = new Thickness(0);
            _tb.FontFamily = _tbl.FontFamily = new System.Windows.Media.FontFamily("Arial");

            _tbl.TextWrapping = _tb.TextWrapping = TextWrapping.Wrap;
            _tb.AcceptsReturn = true;
            _tb.SnapsToDevicePixels = _tbl.SnapsToDevicePixels = true;

            sp.Children.Add(_tbl);
            sp.Children.Add(_tb);
            MainWindow.Content = sp;
            MainWindow.Width = 800;
            MainWindow.Height = 700;

            _testData = TextScript.Values;
            QueueDelegate(SetData);
        }

        private void SetData()
        {
            if (_testIndex == _testData.Length + 1)
            {
                Logger.Current.ReportSuccess();
            }
            else
                if (_testIndex == _testData.Length)
                {
                    Log("\r\n-------------- English ------------------\r\n");
                    _tbl.Text = _tb.Text = stringData;
                }
                else
                {
                    Log("\r\n--------------" + _testData[_testIndex].Name + "------------------\r\n");
                    string temp = FormatLongString();
                    string str = temp;
                    str = "Cheese :)" + str;
                    Log(_testData[_testIndex].Name);
                    //This loop is removed so that the text is less and hence can fit in a control
                    // with smaller height; smaller value being necessary for lower resolutions
                    //  for (int i = 0; i < 2; i++)
                    //  {
                    if (_testIndex + 1 != _testData.Length)
                    {
                        _testIndex++;
                        temp = FormatLongString();

                        // str = (i==0)?(str+temp):( str + "\r\n" + temp);
                        str += temp;
                        Log(_testData[_testIndex].Name);
                    }
                    // }
                    _tbl.Text = _tb.Text = str;
                }
            _tbl.Padding = new Thickness(_tb.GetRectFromCharacterIndex(0).TopLeft.X, _tb.GetRectFromCharacterIndex(0).TopLeft.Y, 0, 0);
            QueueDelegate(CaptureImage);
        }

        private string FormatLongString()
        {
            string temp = _testData[_testIndex].Sample;
            int count = 20;
            int index = 2;
            while (temp.Length > count)
            {
                temp = temp.Insert(count, "\r\n");
                count = 20 * index;
                index++;
            }
            temp = (temp.Length > 100) ? (temp.Substring(0, 100)) : temp;
            return temp;
        }

        private void CaptureImage()
        {
            _textBoxImage = BitmapCapture.CreateBitmapFromElement(_tb);
            _textBlockImage = BitmapCapture.CreateBitmapFromElement(_tbl);

            //Following code is commented as it doesnt account for textblock spacing
            ////Convert to Black and White
            //textBoxImage = BitmapUtils.ColorToBlackWhite(textBoxImage);
            //textBlockImage = BitmapUtils.ColorToBlackWhite(textBlockImage);
            ////Get a SubBitmap where all black pixels exists
            //textBoxImage = BitmapUtils.CreateSubBitmap(textBoxImage, BitmapUtils.GetBoundingRectangle(textBoxImage));
            //textBlockImage = BitmapUtils.CreateSubBitmap(textBlockImage, BitmapUtils.GetBoundingRectangle(textBlockImage));
            //if (ComparisonOperationUtils.AreBitmapsEqualUsingCriteria(textBoxImage, textBlockImage, comparisonCriteria, true))
            //{
            //    testIndex++;
            //    QueueDelegate(SetData);
            //}
            //else
            //{
            //    Logger.Current.ReportResult(false, "FAILED TEST", false);
            //}

            Rect rectForFirstChar = _tb.GetRectFromCharacterIndex(0);
            double topLeftX = rectForFirstChar.TopLeft.X;
            double topLeftY = rectForFirstChar.TopLeft.Y;

            _textBoxImage = BitmapUtils.CreateBitmapClipped(_textBoxImage, (int)topLeftX, 0, (int)topLeftY, 0);

            _textBoxImage = BitmapUtils.ColorToBlackWhite(_textBoxImage);
            _textBlockImage = BitmapUtils.ColorToBlackWhite(_textBlockImage);

            ComparisonCriteria comparisonCriteria = new ComparisonCriteria();
            comparisonCriteria.MaxPixelDistance = 2;

            bool comparision = false;
            for (double x = 0; x <= topLeftX && comparision == false; x = x + 0.5)
            {
                for (double y = 0; y <= topLeftY && comparision == false; y = y + 0.5)
                {
                    Bitmap textBlockTempImage = BitmapUtils.CreateBitmapClipped(_textBlockImage, new Thickness(x, y, topLeftX - x, topLeftY - y), true);
                    GlobalLog.LogStatus("---- X:" + x + "----Y:" + y);
                    if (ComparisonOperationUtils.AreBitmapsEqualUsingCriteria(_textBoxImage, textBlockTempImage, comparisonCriteria, false))
                    {
                        comparision = true;
                        break;
                    }
                }
            }
            if (comparision)
            {
                _testIndex++;
                QueueDelegate(SetData);
            }
            else
            {
                Logger.Current.ReportResult(false, "FAILED TEST", false);
            }
        }

        #endregion

        #region Private Data.

        private TextBox _tb;
        private TextBlock _tbl;

        private Bitmap _textBoxImage,_textBlockImage;
        private int _testIndex = 0;

        private const string stringData = "ABCDEFGHIJKLMNOPQRSTUVWXYZ\r\nabcdefghijklmnopqrstuvwxyz\r\n0123456789\r\n!@#$%^&*()-_+={}[]\\|\":;'<>?,./";
        private TextScript[] _testData;

        #endregion
    }

    // DISABLEDUNSTABLETEST:
    // TestName: TextBoxViewResizing
    // Area: Editing SubArea: TextBox
    // Disable this case due to high fail rate, will enable after fix it.
    // to find all disabled tests in test tree, use: �findstr /snip DISABLEDUNSTABLETEST�
    /// <summary>
    /// Verifies resizing in TextBoxView
    /// </summary>
    [Test(1, "TextBox", "TextBoxViewResizing", MethodParameters = "/TestCaseType:TextBoxViewResizing /InputMonitorEnabled:False", Timeout = 360, Disabled = true)]
    [TestOwner("Microsoft"), TestTactics("573"), TestWorkItem("99"), TestLastUpdatedOn("April 25, 2007")]
    public class TextBoxViewResizing : CustomTestCase
    {
        #region Main flow

        /// <summary>Overridden Entry Point</summary>
        public override void RunTestCase()
        {
            StackPanel sp = new StackPanel();
            _tb = new TextBox();
            _tbl = new TextBlock();

            _tbl.FontSize = _tb.FontSize = 20;
            _tbl.Height = _tb.Height = 200;
            _tb.Margin = _tb.Padding = _tb.BorderThickness = new Thickness(0);
            _tb.FontFamily = _tbl.FontFamily = new System.Windows.Media.FontFamily("Arial");
            _tbl.TextWrapping = _tb.TextWrapping = TextWrapping.Wrap;
            _tb.AcceptsReturn = true;
            _tb.SnapsToDevicePixels = _tbl.SnapsToDevicePixels = true;

            sp.Children.Add(_tbl);
            sp.Children.Add(_tb);

            MainWindow.Content = sp;
            MainWindow.Height = _windowHeight;
            MainWindow.Width = _windowWidth;
            MainWindow.ResizeMode = ResizeMode.CanResizeWithGrip;
            MainWindow.WindowStyle = WindowStyle.None;

            _testData = TextScript.Values;
            QueueDelegate(SetData);
        }

        private void SetData()
        {
            if (_testIndex == _testData.Length + 1)
            {
                Logger.Current.ReportSuccess();
            }
            else
                if (_testIndex == _testData.Length)
                {
                    Log("\r\n-------------- English ------------------\r\n");
                    _tbl.Text = _tb.Text = stringData;
                }
                else
                {
                    Log("--------------");
                    string temp = FormatLongString();
                    string str = temp;
                    //if ((testData[testIndex].Name == "Hebrew") || (testData[testIndex].Name == "Arabic") || (testData[testIndex].Name == "Syriac"))
                    {
                        str = "Cheese :)" + str;
                        Log(_testData[_testIndex].Name);
                        for (int i = 0; i < 3; i++)
                        {
                            if (_testIndex + 1 != _testData.Length)
                            {
                                _testIndex++;
                                temp = FormatLongString();

                                str = str + "\r\n" + temp;
                                Log(_testData[_testIndex].Name);
                            }
                        }
                    }
                    Log("------------------\r\n");
                    _tbl.Text = _tb.Text = str;
                }
            _tbl.Padding = new Thickness(_tb.GetRectFromCharacterIndex(0).TopLeft.X, _tb.GetRectFromCharacterIndex(0).TopLeft.Y, 0, 0);
            QueueDelegate(CaptureInitialImage);
        }

        private string FormatLongString()
        {
            string temp = _testData[_testIndex].Sample;
            int count = 45;
            int index = 2;
            while (temp.Length > count)
            {
                temp = temp.Insert(count, "\r\n");
                count = 45 * index;
                index++;
            }
            temp = (temp.Length > 100) ? (temp.Substring(0, 100)) : temp;
            return temp;
        }

        private void CaptureInitialImage()
        {
            _textBoxInitialImage = BitmapCapture.CreateBitmapFromElement(_tb);
            _textBoxInitialImage = BitmapUtils.ColorToBlackWhite(_textBoxInitialImage);
            _textBoxInitialImage = BitmapUtils.CreateSubBitmap(_textBoxInitialImage, BitmapUtils.GetBoundingRectangle(_textBoxInitialImage));
            QueueDelegate(ResizeWindow);
        }

        private void ResizeWindow()
        {
            Rect r = ElementUtils.GetScreenRelativeRect(MainWindow);
            MouseInput.MouseDragInOtherThread(new System.Windows.Point((int)(r.BottomRight.X - 10), (int)(r.BottomRight.Y - 10)), new System.Windows.Point((int)(r.BottomRight.X + 600), (int)(r.BottomRight.Y - 10)), true, new TimeSpan(0, 0, 0), new SimpleHandler(CaptureImage), System.Windows.Threading.Dispatcher.CurrentDispatcher);
        }

        private void CaptureImage()
        {
            _textBoxImage = BitmapCapture.CreateBitmapFromElement(_tb);
            _textBlockImage = BitmapCapture.CreateBitmapFromElement(_tbl);
            //Convert to Black and White
            _textBoxImage = BitmapUtils.ColorToBlackWhite(_textBoxImage);
            _textBlockImage = BitmapUtils.ColorToBlackWhite(_textBlockImage);
            //Get a SubBitmap where all black pixels exists
            _textBoxImage = BitmapUtils.CreateSubBitmap(_textBoxImage, BitmapUtils.GetBoundingRectangle(_textBoxImage));
            _textBlockImage = BitmapUtils.CreateSubBitmap(_textBlockImage, BitmapUtils.GetBoundingRectangle(_textBlockImage));

            ComparisonCriteria comparisonCriteria = new ComparisonCriteria();
            comparisonCriteria.MaxErrorProportion = 0.01f;
            comparisonCriteria.MaxColorDistance = 0.01f;

            if (ComparisonOperationUtils.AreBitmapsEqualUsingCriteria(_textBoxImage, _textBlockImage, comparisonCriteria, true))
            {
                if (ComparisonOperationUtils.AreBitmapsEqualUsingCriteria(_textBoxImage, _textBoxInitialImage, comparisonCriteria, false) == false)
                {
                    _testIndex++;
                    MainWindow.Height = _windowHeight;
                    MainWindow.Width = _windowWidth;
                    QueueDelegate(SetData);
                }
                else
                {
                    Log("-----Failed while comparingingthe final and Initial Image------");
                    Log("Logging TextBoxFinalImage TextBoxInitialImage");
                    Logger.Current.LogImage(_textBoxImage, "TextBoxFinalImage");
                    Logger.Current.LogImage(_textBoxInitialImage, "TextBoxInitialImage");
                    Logger.Current.ReportResult(false, "FAILED TEST", false);
                }
            }
            else
            {
                Log("Logging TextBoxImage TextBlockImage");
                Logger.Current.LogImage(_textBoxImage, "TextBoxImage");
                Logger.Current.LogImage(_textBlockImage, "TextBlockImage");
                Logger.Current.ReportResult(false, "FAILED TEST", false);
            }
        }

        #endregion

        #region Private Data

        private TextBox _tb;
        private TextBlock _tbl;

        private Bitmap _textBoxImage,_textBlockImage,_textBoxInitialImage;
        private int _testIndex = 0;
        private double _windowHeight = 500;
        private double _windowWidth = 100;

        private const string stringData = "ABCDEFGHIJKLMNOPQRSTUVWXYZ\r\nabcdefghijklmnopqrstuvwxyz\r\n0123456789\r\n!@#$%^&*()-_+={}[]\\|\":;'<>?,./";
        private TextScript[] _testData;

        #endregion
    }

    /// <summary>
    /// Verifies typing in TextBoxView
    /// </summary>
    [Test(0, "TextBox", "TextBoxViewTyping", MethodParameters = "/TestCaseType:TextBoxViewTyping")]
    [TestOwner("Microsoft"), TestTactics("571"), TestWorkItem("98"), TestLastUpdatedOn("April 25, 2007")]
    public class TextBoxViewTyping : CustomTestCase
    {
        #region Main flow

        /// <summary>Overridden Entry Point</summary>
        public override void RunTestCase()
        {
            StackPanel sp = new StackPanel();
            _tb = new TextBox();
            _tbl = new TextBlock();
            _b = new Button();
            _b.Height = 100;

            _tbl.FontSize = _tb.FontSize = 24;
            _tbl.Height = _tb.Height = 200;
            _tb.Margin = _tb.Padding = _tb.BorderThickness = new Thickness(0);
            _tbl.TextWrapping = _tb.TextWrapping = TextWrapping.Wrap;
            _tb.AcceptsReturn = true;
            _tb.FontWeight = _tbl.FontWeight = FontWeights.Bold;
            _tb.FontFamily = _tbl.FontFamily = new System.Windows.Media.FontFamily("Arial");
            _tb.SnapsToDevicePixels = _tbl.SnapsToDevicePixels = true;

            sp.Children.Add(_tbl);
            sp.Children.Add(_tb);
            sp.Children.Add(_b);

            MainWindow.Content = sp;
            MainWindow.Height = windowHeight;
            MainWindow.Width = windowWidth;
            MainWindow.ResizeMode = ResizeMode.CanResizeWithGrip;
            MainWindow.WindowStyle = WindowStyle.None;

            QueueDelegate(SetData);
        }

        private void SetData()
        {
            _tbl.Text = stringData;
            _tb.Focus();
            string str = stringData.Replace("\n", "");
            KeyboardInput.TypeString(str);
            QueueDelegate(GiveFocusToButton);
        }

        private void GiveFocusToButton()
        {
            _b.Focus();
            _tbl.Padding = new Thickness(_tb.GetRectFromCharacterIndex(0).TopLeft.X, _tb.GetRectFromCharacterIndex(0).TopLeft.Y, 0, 0);
            QueueDelegate(CaptureImage);
        }

        private void CaptureImage()
        {
            _textBoxImage = BitmapCapture.CreateBitmapFromElement(_tb);
            _textBlockImage = BitmapCapture.CreateBitmapFromElement(_tbl);
            //Convert to Black and White
            _textBoxImage = BitmapUtils.ColorToBlackWhite(_textBoxImage);
            _textBlockImage = BitmapUtils.ColorToBlackWhite(_textBlockImage);
            //Get a SubBitmap where all black pixels exists
            _textBoxImage = BitmapUtils.CreateSubBitmap(_textBoxImage, BitmapUtils.GetBoundingRectangle(_textBoxImage));
            _textBlockImage = BitmapUtils.CreateSubBitmap(_textBlockImage, BitmapUtils.GetBoundingRectangle(_textBlockImage));

            ComparisonCriteria comparisonCriteria = new ComparisonCriteria();
            comparisonCriteria.MaxErrorProportion = 0.01f;
            comparisonCriteria.MaxColorDistance = 0.01f;
            if (ComparisonOperationUtils.AreBitmapsEqualUsingCriteria(_textBoxImage, _textBlockImage, comparisonCriteria, false))
            {
                Loggers.Logger.Current.ReportSuccess();
            }
            else
            {
                GlobalLog.LogStatus("-----Failed Image Comparison ------");
                Logger.Current.LogImage(_textBoxImage, "textBoxImage");
                Logger.Current.LogImage(_textBlockImage, "textBlockImage");
                Logger.Current.ReportResult(false, "FAILED TEST", false);
            }

            //***Other method of verification. Restore this if the above verification fails.***
            //Rect rectForFirstChar = tb.GetRectFromCharacterIndex(0);
            //double topLeftX = rectForFirstChar.TopLeft.X;
            //double topLeftY = rectForFirstChar.TopLeft.Y;

            //textBoxImage = BitmapUtils.CreateBitmapClipped(textBoxImage, (int)topLeftX, 0, (int)topLeftY, 0);

            //textBoxImage = BitmapUtils.ColorToBlackWhite(textBoxImage);
            //textBlockImage = BitmapUtils.ColorToBlackWhite(textBlockImage);

            //ComparisonCriteria comparisonCriteria = new ComparisonCriteria();
            //comparisonCriteria.MaxPixelDistance = 2;

            //bool comparision = false;
            //for (double x = 0; x <= topLeftX && comparision == false; x = x + 0.5)
            //{
            //    for (double y = 0; y <= topLeftY && comparision == false; y = y + 0.5)
            //    {
            //        Bitmap textBlockTempImage = BitmapUtils.CreateBitmapClipped(textBlockImage, new Thickness(x, y, topLeftX - x, topLeftY - y), true);
            //        GlobalLog.LogStatus("---- X:" + x + "----Y:" + y);
            //        if (ComparisonOperationUtils.AreBitmapsEqualUsingCriteria(textBoxImage, textBlockTempImage, comparisonCriteria, false))
            //        {
            //            comparision = true;
            //            break;
            //        }
            //    }
            //}

        }

        #endregion

        #region Private Data

        private TextBox _tb;
        private TextBlock _tbl;
        private Button _b;

        private Bitmap _textBoxImage,_textBlockImage;
        private const double windowHeight = 500;
        private const double windowWidth = 1000;

        private const string stringData = "ABCDEFGHIJKLMNOPQRSTUVWXYZ\r\nabcdefghijklmnopqrstuvwxyz\r\n0123456789\r\n!@#$&*()-_[]\\|\":;'<>?,./";

        #endregion
    }

    /// <summary>
    /// Verifies static rendering in TextBoxView
    /// </summary>
    [Test(0, "TextBox", "TextBoxViewRenderingWithFD", MethodParameters = "/TestCaseType:TextBoxViewRenderingWithFD")]
    [TestOwner("Microsoft"), TestTactics("570"), TestLastUpdatedOn("April 25, 2007")]
    public class TextBoxViewRenderingWithFD : CustomTestCase
    {
        #region Main flow

        /// <summary>Overridden Entry Point</summary>
        public override void RunTestCase()
        {
            StackPanel sp = new StackPanel();
            _tb = new TextBox();
            _tbl = new TextBlock();

            _tbl.FontSize = _tb.FontSize = 20;
            _tbl.Width = _tb.Width = 700;
            _tbl.Height = _tb.Height = 150;
            _tb.Margin = _tb.Padding = _tb.BorderThickness = new Thickness(0);
            _tb.FontFamily = _tbl.FontFamily = new System.Windows.Media.FontFamily("Arial");

            _tbl.FlowDirection = FlowDirection.RightToLeft;
            _tbl.TextWrapping = _tb.TextWrapping = TextWrapping.Wrap;
            _tb.AcceptsReturn = true;
            _tb.SnapsToDevicePixels = _tbl.SnapsToDevicePixels = true;

            sp.Children.Add(_tbl);
            sp.Children.Add(_tb);
            MainWindow.Content = sp;
            MainWindow.Width = 800;
            MainWindow.Height = 800;

            _testData = TextScript.Values;
            QueueDelegate(SetData);
        }

        private void SetData()
        {
            _tb.FlowDirection = FlowDirection.LeftToRight;
            if (_testIndex >= _testData.Length + 1)
            {
                Logger.Current.ReportSuccess();
            }
            else
            {
                if (_testIndex == _testData.Length)
                {
                    Log("\r\n-------------- English ------------------\r\n");
                    _tbl.Text = _tb.Text = stringData;
                }
                else
                {
                    Log("\r\n--------------" + _testData[_testIndex].Name + "------------------\r\n");
                    string temp = FormatLongString();
                    string str = temp;
                    str = "Cheese 1111 2222 Cheese 1111 2222 Cheese 1111 2222 Cheese :)" + "\r\n" + str;
                    Log(_testData[_testIndex].Name);
                    //for (int i = 0; i < 3; i++)
                    {
                        if (_testIndex + 1 != _testData.Length)
                        {
                            _testIndex++;
                            temp = FormatLongString();

                            // str = (i == 0) ? (str + temp) : (str + "\r\n" + temp);
                            str += temp;
                            Log(_testData[_testIndex].Name);
                        }
                    }
                    _tbl.Text = _tb.Text = str;
                }
                QueueDelegate(GetInitialSpace);
            }
        }

        private string FormatLongString()
        {
            string temp = _testData[_testIndex].Sample;
            int count = 20;
            int index = 2;
            while (temp.Length > count)
            {
                temp = temp.Insert(count, "\r\n");
                count = 20 * index;
                index++;
            }
            temp = (temp.Length > 100) ? (temp.Substring(0, 100)) : temp;
            return temp;
        }

        private void GetInitialSpace()
        {
            Rect rectForFirstChar = _tb.GetRectFromCharacterIndex(0);
            _topLeftXordInLeftAlignment = rectForFirstChar.TopLeft.X;
            _tbl.Padding = new Thickness(_topLeftXordInLeftAlignment, rectForFirstChar.TopLeft.Y, 0, 0);
            QueueDelegate(ChangeFDToRTL);
        }

        private void ChangeFDToRTL()
        {
            _tb.FlowDirection = FlowDirection.RightToLeft;
            QueueHelper.Current.QueueDelayedDelegate(new TimeSpan(0, 0, 1), new SimpleHandler(CaptureImage), null);
        }

        private void CaptureImage()
        {
            _textBoxImage = BitmapCapture.CreateBitmapFromElement(_tb);
            _textBlockImage = BitmapCapture.CreateBitmapFromElement(_tbl);

            Rect rectForFirstChar = _tb.GetRectFromCharacterIndex(0);
            double topLeftX = _textBoxImage.Width - rectForFirstChar.TopLeft.X;
            double topLeftY = rectForFirstChar.TopLeft.Y;

            _textBoxImage = BitmapUtils.CreateBitmapClipped(_textBoxImage, (int)topLeftX, 0, (int)topLeftY, 0);
            _textBlockImage = BitmapUtils.CreateBitmapClipped(_textBlockImage, (int)(topLeftX - _topLeftXordInLeftAlignment), 0, 0, 0);

            ComparisonCriteria comparisonCriteria = new ComparisonCriteria();
            comparisonCriteria.MaxColorDistance = 0.05f;
            // comparisonCriteria.MaxPixelDistance = 2;

            bool comparision = false;
            for (double x = 0; x <= _topLeftXordInLeftAlignment && comparision == false; x = x + 0.5)
            {
                for (double y = 0; y <= topLeftY && comparision == false; y = y + 0.5)
                {
                    Bitmap textBlockTempImage = BitmapUtils.CreateBitmapClipped(_textBlockImage, new Thickness(x, y, _topLeftXordInLeftAlignment - x, topLeftY - y), true);
                    GlobalLog.LogStatus("---- X:" + x + "----Y:" + y);
                    if (ComparisonOperationUtils.AreBitmapsEqualUsingCriteria(_textBoxImage, textBlockTempImage, comparisonCriteria, true))
                    {
                        comparision = true;
                        break;
                    }
                }
            }
            if (comparision)
            {
                _testIndex++;
                QueueDelegate(SetData);
            }
            else
            {
                Logger.Current.ReportResult(false, "FAILED TEST", false);
            }
        }

        #endregion

        #region Private Data

        private TextBox _tb;
        private TextBlock _tbl;

        private Bitmap _textBoxImage,_textBlockImage;
        private int _testIndex = 0;

        private const string stringData = "ABCDEFGHIJKLMNOPQRSTUVWXYZ\r\nabcdefghijklmnopqrstuvwxyz\r\n0123456789\r\n!@#$%^&*()-_+={}[]\\|\":;'<>?,./";
        private TextScript[] _testData;

        double _topLeftXordInLeftAlignment = 0;
        #endregion Private fields.
    }

    /// <summary>
    /// test TextBox HorizontalAlignment  and textalignment
    /// </summary>
    [Test(0, "TextBox", "TextBoxAlignment", MethodParameters = "/TestCaseType:TextBoxAlignment")]
    [TestOwner("Microsoft"), TestTactics("569"), TestLastUpdatedOn("April 25, 2007")]
    public class TextBoxAlignment : ManagedCombinatorialTestCase
    {
        #region Override Members

        /// <summary>Overridden Entry Point</summary>
        protected override void DoRunCombination()
        {
            MainWindow.Content = null;
            _passwordBox1 = new PasswordBox();
            _passwordBox2 = new PasswordBox();

            _textBox1 = new TextBox();
            _textBox2 = new TextBox();

            _textBox1.Margin = _textBox2.Margin = new Thickness(0);
            _textBox1.Padding = _textBox2.Padding = new Thickness(0);
            _textBox1.BorderBrush = _textBox2.BorderBrush = System.Windows.Media.Brushes.Transparent;
            //textBox2.Foreground = System.Windows.Media.Brushes.Transparent;

            _textBox2.FontSize = _passwordBox1.FontSize = _passwordBox2.FontSize = 20;

            _textBox1.HorizontalAlignment = _textBox2.HorizontalAlignment = _passwordBox1.HorizontalAlignment = _passwordBox2.HorizontalAlignment = _horizontalAlignment;
            _textBox1.TextAlignment = _textAlignment;
            _textBox2.TextAlignment = (_textAlignment == TextAlignment.Right) ? TextAlignment.Left : TextAlignment.Right;
            _textBox1.TextWrapping = _textBox2.TextWrapping = _textWrapping;

            _textBox1.Text = _textBox2.Text = (_textWrapping == TextWrapping.Wrap) ? longStringData : shortStringData;
            _passwordBox2.Foreground = System.Windows.Media.Brushes.Transparent;
            _passwordBox1.Password = shortStringData;

            StackPanel stackPanel = new StackPanel();
            stackPanel.Children.Add(_textBox1);
            stackPanel.Children.Add(_textBox2);
            stackPanel.Children.Add(_passwordBox1);
            stackPanel.Children.Add(_passwordBox2);

            MainWindow.Content = stackPanel;
            QueueDelegate(ImageVerification);
        }

        #endregion

        #region Private Members

        private void ImageVerification()
        {
            CaptureAndCompareSameControls(_textBox1, _textBox2);
            CaptureAndCompareSameControls(_passwordBox1, _passwordBox2);

            QueueDelegate(NextCombination);
        }

        private void CaptureAndCompareSameControls(UIElement element1, UIElement element2)
        {
            bool imageComparisonResult;
            Bitmap differencesImage;
            CaptureImageBasic(element1, element2);
            CompareImages(out imageComparisonResult, out differencesImage);
            if (imageComparisonResult == true)
            {
                Logger.Current.LogImage(_textBoxImage, "Element1");
                Logger.Current.LogImage(_textBlockImage, "Element2");
                Logger.Current.LogImage(differencesImage, "Differences");
            }
            Verifier.Verify(imageComparisonResult == false, "Bitmaps should NOT be equal when second control has transparent text", true);
        }

        private void CompareImages(out bool imageComparisonResult, out Bitmap differencesImage)
        {
            ComparisonCriteria comparisonCriteria = new ComparisonCriteria();
            comparisonCriteria.MaxErrorProportion = 0.01f;
            imageComparisonResult = ComparisonOperationUtils.AreBitmapsEqualUsingCriteria(_textBoxImage, _textBlockImage, out differencesImage, comparisonCriteria, false);
        }

        private void CaptureImageBasic(UIElement _element1, UIElement _element2)
        {
            _textBoxImage = BitmapCapture.CreateBitmapFromElement(_element1);
            _textBlockImage = BitmapCapture.CreateBitmapFromElement(_element2);
        }

        #endregion

        #region Private Data

        private TextBox _textBox1 = null;
        private TextBox _textBox2 = null;
        private PasswordBox _passwordBox1 = null;
        private PasswordBox _passwordBox2 = null;

        private Bitmap _textBoxImage;
        private Bitmap _textBlockImage;

        private const string shortStringData = "HELLO";
        private const string longStringData = " This is a long string to Wrap the text inside TextBox and TextBlock. HELLOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO!!!";

        private HorizontalAlignment _horizontalAlignment = 0;
        private TextAlignment _textAlignment = 0;
        private TextWrapping _textWrapping = 0;

        #endregion
    }
}
