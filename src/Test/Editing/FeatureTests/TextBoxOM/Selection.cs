// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Functional test cases for selection-related APIs for TextBox.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Text/BVT/TextBoxOM/Selection.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    
    using Drawing = System.Drawing;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Input;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;
    using Microsoft.Test.Imaging;

    using Test.Uis.Data;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;        
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;
    using Microsoft.Test.Input;
    using Microsoft.Test.Threading;    

    #endregion Namespaces.

    /// <summary>
    /// Verifies that the Select, SelectAll, SelectedText,
    /// SelectionLength and SelectionStart APIs work
    /// correctly.
    /// <para />
    /// Also verifies the Selection, StartPosition and EndPosition
    /// properties indirectly by callind the VerifyXXX methods
    /// on the base class.
    /// </summary>    
    [Test(0, "TextBox", "TextBoxSelection", MethodParameters = "/TestCaseType=TextBoxSelection", Timeout = 200)]
    [TestOwner("Microsoft"), TestTactics("550,551,552"),
     TestArgument("Text", "Text for control"),
     TestArgument("SelectionStart", "Position for selection start"),
     TestArgument("SelectionLength", "Length of selection"),]
    public class TextBoxSelection: TextBoxTestCase
    {
        #region TestCaseData
        /// <summary>Data driven test cases.</summary>
        internal class TestData
        {
            int _selectionStart;
            int _selectionLength;

            internal int selectionStart { get { return _selectionStart; } }
            internal int selectionLength { get { return _selectionLength; } }

            internal TestData(int start, int length)
            {
                _selectionStart = start;
                _selectionLength = length;
            }

            internal static TestData[] TestCases = new TestData[] {
                new TestData(1,3),
                new TestData(0,0),
                new TestData(0,1),
                new TestData(0,17),
                new TestData(17,0),
                new TestData(17,20),
            };
        }        
        #endregion TestCaseData

        string _testContent = "some simple text.";
        int _currentIndex = 0;
        int _endIndex = TestData.TestCases.Length;        

        #region Main flow.
        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            VerifyInvalidCalls();
            VerifyValidCalls();            
        }

        #endregion Main flow.

        #region Verifications.

        private void VerifyInvalidCalls()
        {
            // Negative start in Select and SelectionStart was moved to
            // TextBoxSelectionReproRegression_Bug581 to unblock BVT.
            Log("Verifying invalid calls to TextBox.Select...");
            try
            {
                TestTextBox.Select(0, -1);
                throw AcceptedException("negative length");
            }
            catch (ArgumentOutOfRangeException)
            {
                LogRejected("negative length");
            }

            Log("Verifying invalid value TextBox.SelectionLength...");
            try
            {
                TestTextBox.SelectionLength = -1;
                throw AcceptedException("negative selection length");
            }
            catch (ArgumentOutOfRangeException)
            {
                LogRejected("negative selection length");
            }
            Log("Verifying invalid value TextBox.SelectionStart...");
            try
            {
                TestTextBox.SelectionStart = -1;
                throw AcceptedException("negative selection start");
            }
            catch (ArgumentOutOfRangeException)
            {
                LogRejected("negative selection start");
            }
            try
            {
                TestTextBox.CaretIndex = -1;
                throw AcceptedException("negative caret index");
            }
            catch (ArgumentOutOfRangeException)
            {
                LogRejected("negative caret index");
            }
        }        

        private void VerifyValidCalls()
        {            
            TestTextBox.Text = string.Empty;
            SetTextBoxProperties(TestTextBox);

            //int start = Settings.GetArgumentAsInt("SelectionStart", true);
            //int length = Settings.GetArgumentAsInt("SelectionLength", true);
            int start = TestData.TestCases[_currentIndex].selectionStart;
            int length = TestData.TestCases[_currentIndex].selectionLength;
            Log("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
            Log("(SelectionStart, SelectionLength) = (" + start + ", " + length + ")");
            Log("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
            int maxLength = _testContent.Length - start;
            int selectedLength = (length <= maxLength)? length : maxLength;

            Log("Selected length expected: " + selectedLength);

            Log("Selecting text through the Select method...");
            TestTextBox.Text = _testContent;
            TestTextBox.Select(start, length);
            Verifier.Verify(TestTextBox.SelectionStart == start,
                "SelectionStart matches setting", true);
            Verifier.Verify(TestTextBox.SelectionLength == selectedLength,
                "SelectionLength matches setting", true);
            VerifyText(TestTextBox, _testContent);
            VerifySelectedText(TestTextBox, _testContent.Substring(start, selectedLength));

            Log("Verifying that SelectionStart does not select automatically");
            TestTextBox.Clear();
            TestTextBox.Text = _testContent;
            TestTextBox.SelectionStart = start;
            VerifyText(TestTextBox, _testContent);
            VerifySelectedText(TestTextBox, String.Empty);

            Log("Verifying that SelectionLength selects from starting point...");
            TestTextBox.SelectionLength = length;
            VerifySelectedText(TestTextBox, _testContent.Substring(start, selectedLength));

            Log("Verifying that SelectionLength can reset selection...");
            TestTextBox.SelectionLength = 0;
            VerifySelectedText(TestTextBox, String.Empty);

            Log("Verifying that SelectAll does not auto-select when text is replaced...");
            TestTextBox.Clear();
            TestTextBox.SelectAll();
            TestTextBox.Text = _testContent;
            VerifySelectedText(TestTextBox, String.Empty);

            Log("Verifying that SelectAll selects all text...");
            TestTextBox.SelectAll();
            VerifySelectedText(TestTextBox, _testContent);

            _currentIndex++;
            if (_currentIndex < _endIndex)
            {
                TestTextBox.Text = string.Empty;                
                QueueDelegate(VerifyValidCalls);
            }
            else
            {
                Logger.Current.ReportSuccess();
            }
        }

        #endregion Verifications.
    }

    /// <summary>
    /// Verify that Select and SelectedText work with regular text, empty text, and
    /// handle newlines and combined characters consistently with the Win32 API.
    /// Verify that the rendering is updated after changes to selection.
    /// </summary>
    [Test(0, "TextBox", "TextBoxSelectionTest", MethodParameters = "/TestCaseType=TextBoxSelectionTest")]
    [TestOwner("Microsoft"), TestTactics("549"), TestBugs("713, 714, 715, 716, 717"), TestWorkItem("84")]
    public class TextBoxSelectionTest : CustomTestCase
    {
        #region Main flow.
        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            SetupCombinatorialEngine();
            QueueDelegate(NextCombination);
        }

        private void NextCombination()
        {
            if (!GetNextCombination())
            {
                Logger.Current.ReportSuccess();
            }
            else
            {
                _textBox = new TextBox();
                _wrapper = new UIElementWrapper(_textBox);
                MainWindow.Content = _textBox;
                _textBox.Height = 500;
                _textBox.Width = 500;
                _textBox.FontFamily = new FontFamily("Tahoma");
                _textBox.TextWrapping = TextWrapping.Wrap;
                _textBox.Text = _testData.Content;
                _textBox.Focus();

                QueueDelegate(PerformTestAction);
            }
        }

        private void PerformTestAction()
        {
            _bmpBeforeSelection = BitmapCapture.CreateBitmapFromElement(_textBox);
            _isEventFired = 0;
            _textBox.SelectionChanged += new RoutedEventHandler(_textBox_SelectionChanged);

            if (_testAction == "Select")
            {
                _textBox.Select(_testData.Start, _testData.Length);
                Verifier.Verify(_isEventFired == 1,
                    "Verifying that SelectionChanged event is fired for Select", true);
            }
            else if (_testAction == "SelectionStart")
            {
                _textBox.SelectionStart = _testData.Start;
                _textBox.SelectionLength = _testData.Length;
                Verifier.Verify(_isEventFired == 2,
                    "Verifying that 2 SelectionChanged events are fired for SelectionStart/SelectionLength", true);
            }
            else if (_testAction == "SelectAll")
            {
                _textBox.SelectAll();
                Verifier.Verify(_isEventFired == 1,
                    "Verifying that SelectionChanged event is fired for SelectAll", true);
            }

            Verifier.Verify(_textBox.SelectionStart == _textBox.CaretIndex,
                "Verifying that SelectionStart==CaretIndex", false);

            _textBox.Focus();
            QueueDelegate(VerifySelection);
        }

        private void VerifySelection()
        {
            _bmpAfterSelection = BitmapCapture.CreateBitmapFromElement(_textBox);

            string logString;
            if (_testAction == "SelectAll")
            {
                logString = "Expected [" + _testData.Content + "] Actual [" + _textBox.SelectedText + "]";
                Verifier.Verify(_textBox.SelectedText == _testData.Content,
                    "Verifying SelectedText after SelectAll: " + logString, true);

                Verifier.Verify(_textBox.CaretIndex == 0,
                    "Verifying that CaretIndex is zero after SelectAll", false);
            }
            else
            {
                logString = "Expected [" + _testData.ExpSelectedString + "] Actual [" + _textBox.SelectedText + "]";
                Verifier.Verify(_textBox.SelectedText == _testData.ExpSelectedString,
                    "Verifying SelectedText after Select(start,length): " + logString, true);
            }

            //Verify that rendering is updated after changes to Selection
            if ((_textBox.SelectedText != string.Empty) && (_textBox.SelectedText != "\r\n"))
            {
                if (ComparisonOperationUtils.AreBitmapsEqual(_bmpBeforeSelection, _bmpAfterSelection, out _bmpDifferences))
                {
                    Logger.Current.LogImage(_bmpAfterSelection, "bmpAfterSelection");
                    throw new Exception("There have been no changes in rendering after selection is made.");
                }
            }

            _prevEventCount = _isEventFired;
            _textBox.SelectionChanged -= new RoutedEventHandler(_textBox_SelectionChanged);
            _textBox.SelectionStart += 1;
            QueueDelegate(RemoveSelectionChangedEvent);
        }

        private void RemoveSelectionChangedEvent()
        {
            Verifier.Verify(_prevEventCount == _isEventFired,
                "Verifying that event handler doesnt get called after the event is removed", true);
            QueueDelegate(TestCaretIndex);
        }

        private void TestCaretIndex()
        {
            TextPointer currentPointer;
            int expectedCaretIndex;

            Log("Verifying Set operation on CaretIndex property");

            //Testing set_CaretIndex for at most 10 positions
            for (int i = 0; (i < 10) && (i < _textBox.Text.Length); i++)
            {
                currentPointer = _wrapper.Start;
                currentPointer = currentPointer.GetPositionAtOffset(0, LogicalDirection.Forward);
                currentPointer = currentPointer.GetPositionAtOffset(i);

                _textBox.CaretIndex = i;

                //Normalize for \r\n and surrogates
                currentPointer = currentPointer.GetInsertionPosition(LogicalDirection.Forward);
                expectedCaretIndex = _wrapper.Start.GetOffsetToPosition(currentPointer);

                Verifier.Verify(expectedCaretIndex == _textBox.CaretIndex,
                    "Veriying that CaretIndex is as expected. Actual [" + _textBox.CaretIndex +
                    "] Expected [" + expectedCaretIndex + "]", false);
            }

            _textBox.CaretIndex = Int32.MaxValue;
            Verifier.Verify(_textBox.CaretIndex == _textBox.Text.Length,
                "Verifying best match service for setting CaretIndex", false);

            //Set selection in a collapsed state (Setup for next delegate)
            _textBox.CaretIndex = _textBox.Text.Length / 2;

            QueueDelegate(TestSetSelectedText);
        }

        private void TestSetSelectedText()
        {
            _bmpBeforeSelection = BitmapCapture.CreateBitmapFromElement(_textBox);
            _textBox.SelectedText = _testSelectedString1;
            QueueDelegate(VerifySetSelectedText_CollapsedState);
        }

        /// <summary>
        /// Verifies the operation of setting TextBox.SelectedText when selection is empty
        /// </summary>
        private void VerifySetSelectedText_CollapsedState()
        {
            _bmpAfterSelection = BitmapCapture.CreateBitmapFromElement(_textBox);

            Verifier.Verify(_textBox.Text.Contains(_testSelectedString1),
                "Verifying that TextBox.Text contains the assigned SelectedText", true);
            
            Verifier.Verify(_textBox.SelectedText.Equals(_testSelectedString1, StringComparison.InvariantCulture),
                "Verifying that TextBox.SelectedText [" + _textBox.SelectedText +
                "] = the assigned SelectedText [" + _testSelectedString1 + "]", true);            

            //Verify that rendering is updated after changes to Selection
            if (ComparisonOperationUtils.AreBitmapsEqual(_bmpBeforeSelection, _bmpAfterSelection, out _bmpDifferences))
            {
                Logger.Current.LogImage(_bmpAfterSelection, "bmpAfterSelection");
                throw new Exception("There have been no changes in rendering after SelectedText is set");
            }

            _bmpBeforeSelection = _bmpAfterSelection;
            _textBox.SelectedText = _testSelectedString2;

            QueueDelegate(VerifySetSelectedText_ExpandedState);
        }

        /// <summary>
        /// Verifies the operation of setting TextBox.SelectedText when selection is non-empty
        /// </summary>
        private void VerifySetSelectedText_ExpandedState()
        {
            _bmpAfterSelection = BitmapCapture.CreateBitmapFromElement(_textBox);

            Verifier.Verify(_textBox.Text.Contains(_testSelectedString2),
                "Verifying that TextBox.Text contains the assigned SelectedText", true);

            Verifier.Verify(_textBox.SelectedText == _testSelectedString2,
                "Verifying that TextBox.SelectedText = the assigned SelectedText", true);

            Verifier.Verify(!_textBox.Text.Contains(_testSelectedString1),
                "Verifying that TextBox.Text doesnt contain the contents which are overwritten", true);

            //Verify that rendering is updated after changes to Selection
            if (ComparisonOperationUtils.AreBitmapsEqual(_bmpBeforeSelection, _bmpAfterSelection, out _bmpDifferences))
            {
                Log("Logging Images bmpBeforeSelection bmpAfterSelection bmpDifferences \r\n");
                Logger.Current.LogImage(_bmpDifferences, "bmpDifferences");
                Logger.Current.LogImage(_bmpBeforeSelection, "bmpBeforeSelection");
                Logger.Current.LogImage(_bmpAfterSelection, "bmpAfterSelection");
                throw new Exception("There have been no changes in rendering after SelectedText is set");
            }

            QueueDelegate(NextCombination);
        }

        void _textBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            _isEventFired += 1;
        }

        #endregion Main flow.

        #region Helper methods.
        /// <summary>
        /// Test case data
        /// </summary>
        public class TestData
        {
            string _content;
            int _start;
            int _length;
            string _expSelectionString;

            /// <summary>
            /// Constructor for TestData
            /// </summary>
            /// <param name="content">content string</param>
            /// <param name="start">start index</param>
            /// <param name="length">length value</param>
            /// <param name="expSelectionString">Expected selection string</param>
            public TestData(string content, int start, int length, string expSelectionString)
            {
                _content = content;
                _start = start;
                _length = length;
                _expSelectionString = expSelectionString;
            }

            /// <summary>Content string</summary>
            public string Content
            {
                get { return _content; }
                set { _content = value; }
            }

            /// <summary>Start index</summary>
            public int Start
            {
                get { return _start; }
                set { _start = value; }
            }

            /// <summary>Length value</summary>
            public int Length
            {
                get { return _length; }
                set { _length = value; }
            }

            /// <summary>Expected string</summary>
            public string ExpSelectedString
            {
                get { return _expSelectionString; }
                set { _expSelectionString = value; }
            }

            /// <summary>Returns all Test case data in a array</summary>
            public static TestData[] Values
            {
                get
                {
                    TestData[] testDataValues = new TestData[] {
                        //***Normal case testing***
                        new TestData(StringData.WrappingLine.Value, 0, 10, "Sample phr"),
                        new TestData("abc\r\n\r\ndef", 1, 8, "bc\r\n\r\nde"),

                        //***Best effort testing***
                        new TestData(string.Empty, 0, 10, string.Empty),
                        new TestData("abc\r\n\r\ndef", 1, Int32.MaxValue, "bc\r\n\r\ndef"),
                        new TestData("abc\r\n\r\ndef", 100, 200, string.Empty),

                        //***end between '/r' and '/n'***
                        new TestData("abc\r\n\r\ndef", 1, 3, "bc\r\n"),

                        //***start between '/r' and '/n'***
                        new TestData("abc\r\n\r\ndef", 4, 5, "\r\ndef"),

                        //***start and end between '/r' and '/n'***
                        new TestData("abc\r\n\r\ndef", 4, 2, "\r\n"),

                        //***Surrogate pair testing***                        
                        new TestData(StringData.SurrogatePair.Value, 0, 1, StringData.SurrogatePair.Value.Substring(0,2)),
                        new TestData(StringData.SurrogatePair.Value, 1, 1, string.Empty),
                        
                        new TestData(StringData.SurrogatePair.Value, 0, 2, StringData.SurrogatePair.Value),

                        //***Combining characters testing***
                        new TestData(StringData.CombiningCharacters.Value, 0, 1, StringData.CombiningCharacters.Value.Substring(0,2)),
                    };
                    return testDataValues;
                }
            }

            /// <summary>
            /// Override ToString for TestData
            /// </summary>
            /// <returns>string</returns>
            public override string ToString()
            {
                return _content + "(" + _start + "," + _length + ")";
            }

        }

        private bool GetNextCombination()
        {
            Hashtable values;

            values = new Hashtable();
            if (_engine.Next(values))
            {
                _testData = (TestData)values["TestData"];
                _testAction = (string)values["TestAction"];
                Log("*** Testing [" + _testAction + "] with TestData [" + _testData.ToString() + "]");
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SetupCombinatorialEngine()
        {
            _engine = CombinatorialEngine.FromDimensions(new Dimension[] {
                new Dimension("TestData", TestData.Values),
                new Dimension("TestAction", new object[] {"Select", "SelectionStart", "SelectAll"})
                });
        }
        #endregion Helper methods.

        #region Private fields.

        /// <summary>Combinatorial engine driving test.</summary>
        private CombinatorialEngine _engine;

        private TextBox _textBox;

        /// <summary>Wrapper around instance being edited.</summary>
        private UIElementWrapper _wrapper;

        private TestData _testData;
        private string _testAction;

        private int _isEventFired;
        private int _prevEventCount;

        System.Drawing.Bitmap _bmpBeforeSelection, _bmpAfterSelection, _bmpDifferences;

        private const string _testSelectedString1 = "x-x-x";
        private const string _testSelectedString2 = "y-y-y";
        #endregion Private fields.
    }


    /// <summary>
    /// Verifies that selection is adjusted for newlines when it is
    /// normalized.
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("548"), TestBugs("637")]
    public class TextBoxSelectionNewLines : TextBoxTestCase
    {
        #region Main flow.

        /// <summary>
        /// Runs the test case.
        /// Index is calculated as unicode offset, so it counts
        /// eacn \r\n combination as 2 - even though it is actially
        /// one caret position, and it would be illegal to insert
        /// any characters between them or expect selection ends
        /// to stay between them.
        /// Because of that after setting SelectionStart to some value
        /// it can be automatically corrected (by adding 1)
        /// if it happens to be between \r and \n. 
        ///</summary>
        public override void RunTestCase()
        {
            TestTextBox.Text = "0\r\n1\r\n2\r\n";
            Log("Verifying that SelectionStart gets pushed as expected...");
            TestTextBox.SelectionStart = 0;
            CheckSelectionStart(0, "Selection at start is valid.");
            TestTextBox.SelectionStart = 1;
            CheckSelectionStart(1, "Selection before \\r is valid.");
            TestTextBox.SelectionStart = 2;
            CheckSelectionStart(3,
                "Selection in newline gets pulled to next value on normalization.");
            TestTextBox.SelectionStart = 0;
            TestTextBox.SelectionStart = 3;
            CheckSelectionStart(3, "Selection after newline remains where it should be.");

            TestTextBox.Text = "0\r1";
            TestTextBox.SelectionStart = 1;
            CheckSelectionStart(1, "Selection start accepted after isolated \\r.");

            TestTextBox.Text = "\r\n";
            TestTextBox.SelectionStart = 0;
            CheckSelectionStart(0, "Selection can be in empty first line.");
            TestTextBox.SelectionStart = 1;
            //--TestTextBox.Selection.MoveToCharacters();
            CheckSelectionStart(2, "Selection pulled into first position.");
            TestTextBox.SelectionStart = 0;
            TestTextBox.SelectionStart = 2;
            CheckSelectionStart(2, "Selection remains in last position.");

            Logger.Current.ReportSuccess();
        }
        
        private void CheckSelectionStart(int expectedValue, string description)
        {
            int actualValue = TestTextBox.SelectionStart;
            string message = description + " [expected=" + expectedValue +
                ",actual=" + actualValue + "]";
            Verifier.Verify(actualValue == expectedValue, message, true);
        }

        #endregion Main flow.
    }

    /// <summary>
    /// A test case specifically written to repro Regression_Bug581 and
    /// permits the regular selection test case to run as a BVT.
    /// </summary>
    [TestOwner("Microsoft"),TestTactics("547"),TestBugs("581")]
    [Test(2, "TextBox", "TextBoxSelectionReproRegression_Bug581", MethodParameters = "/TestCaseType=TextBoxSelectionReproRegression_Bug581")]
    public class TextBoxSelectionReproRegression_Bug581: TextBoxTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            Log("Verifying invalid calls to TextBox.Select...");
            try
            {
                TestTextBox.Select(-1, 0);
                throw AcceptedException("negative start");
            }
            catch (SystemException)
            {
                LogRejected("negative start");
            }

            Log("Verifying invalid value on TextBox.SelectionStart...");
            try
            {
                TestTextBox.SelectionStart = -1;
                throw AcceptedException("negative selection start");
            }
            catch (SystemException)
            {
                LogRejected("negative selection start");
            }

            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.
    }

    /// <summary>
    /// A test case specifically written to repro Regression_Bug602.
    /// Verifies that the selection cannot span different
    /// text boxes.
    /// </summary>
    [Test(0, "Selection", "TextBoxSelectionReproRegression_Bug602", MethodParameters = "/TestCaseType:TextBoxSelectionReproRegression_Bug602")]
    [TestOwner("Microsoft"),TestTactics("544"),TestBugs("602")]
    public class TextBoxSelectionReproRegression_Bug602: TextBoxTestCase
    {
        #region Private fields.

        private TextBox _boxA;
        private TextBox _boxB;
        private DockPanel _winPanel;

        #endregion Private fields.

        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            Log("Adding a top-level panel...");
            _winPanel = new DockPanel();
            MainWindow.Content = _winPanel;

            Log("Adding internal text boxes...");
            _boxA = new TextBox();
            _boxB = new TextBox();

            _boxA.Text = "A";
            _boxB.Text = "V";
            _boxA.Width = _boxB.Width = 100;
            _boxA.Height = _boxB.Height = 80;
            _boxA.FontSize = _boxB.FontSize = 20;

            _winPanel.Children.Add(_boxA);
            _winPanel.Children.Add(_boxB);

            QueueDelegate(SelectAll);
        }

        private void SelectAll()
        {
            UIElementWrapper wrapperA = new UIElementWrapper(_boxA);
            Rect first = wrapperA.GetGlobalCharacterRect(0);

            UIElementWrapper wrapperB = new UIElementWrapper(_boxB);
            Rect last = wrapperB.GetGlobalCharacterRectOfLastCharacter();

            Point startPoint = new Point(first.Left+1, first.Top + first.Height / 2);
            Point endPoint = new Point(last.Right + 4, last.Top + last.Height / 2);

            Log("Dragging from " + startPoint + " to " + endPoint);

            MouseInput.MouseDragInOtherThread(startPoint, endPoint, true, new TimeSpan(0, 0, 1), new SimpleHandler(CheckSelection), _boxA.Dispatcher);
        }

        private void CheckSelection()
        {
            LogBox("A", _boxA);
            LogBox("B", _boxB);

            Verifier.Verify(_boxA.SelectedText == _boxA.Text,
                "All of Box A's text is selected", true);
            Verifier.Verify(_boxB.SelectionLength == 0,
                "Box B has no selection", true);
            Logger.Current.ReportSuccess();
        }

        private void LogBox(string name, TextBox box)
        {
            Log("TextBox " + name + " Text:         [" + box.Text + "]");
            Log("TextBox " + name + " SelectedText: [" + box.SelectedText + "]");
        }

        #endregion Main flow.
    }

    /// <summary>
    /// A test case specifically written to repro Regression_Bug613.
    /// Verifies that the caret can be positioned after a UIElement in
    /// a textbox by mouse click.
    /// </summary>
    [Test(0, "RichEditing", "TextBoxCaretPositioningReproRegression_Bug613", MethodParameters = "/TestCaseType:TextBoxCaretPositioningReproRegression_Bug613")]
    [TestTactics("546"), TestOwner("Microsoft"), TestBugs("Regression_Bug613"),
    TestTitle("TextBoxCaretPositioningReproRegression_Bug613")]
    public class TextBoxCaretPositioningReproRegression_Bug613 : CustomTestCase
    {
        Canvas _canvasPanel;
        RichTextBox _testRTB;
        UIElementWrapper _wrapper;
        Button _button1;

        /// <summary>Runs the test case</summary>
        public override void RunTestCase()
        {
            InlineUIContainer inlineUIContainer;

            _canvasPanel = new Canvas();
            _testRTB = new RichTextBox();
            _testRTB.FontSize = 24;
            _testRTB.Height = 400;
            _testRTB.Width = 400;            
            ((Paragraph)_testRTB.Document.Blocks.FirstBlock).Inlines.Add(new Run("This is a test"));
            _wrapper = new UIElementWrapper(_testRTB);

            _button1 = new Button();
            _button1.Content = "Button";

            inlineUIContainer = new InlineUIContainer();
            inlineUIContainer.Child = _button1;
            ((Paragraph)_testRTB.Document.Blocks.FirstBlock).Inlines.Add(inlineUIContainer);

            _canvasPanel.Children.Add(_testRTB);
            MainWindow.Content = _canvasPanel;

            QueueDelegate(new SimpleHandler(DoMouseClick));
        }

        /// <summary>Does the mouse click to the right of the button</summary>
        private void DoMouseClick()
        {
            Log("Moving the mouse and clicking it at the end of the Button");
            Rect endRect = _wrapper.GetGlobalCharacterRect(_testRTB.Document.ContentEnd);
            MouseInput.MouseClick((int)(endRect.X + 15/*buffer*/), (int)(endRect.Top + (endRect.Height/2)));
            QueueDelegate(new SimpleHandler(VerifyRTBSelection));
        }

        /// <summary>Verifies that no selection has been made.</summary>
        private void VerifyRTBSelection()
        {
            TextSelection actualSelection = _testRTB.Selection;

            //--TextContainer selectionContainer = actualSelection.TextContainer;
            Log("Selection contents of the textbox [" + actualSelection.Text + "]");
            Log("Distance from Start to End positions of the Selection Container [" +
                actualSelection.Start.GetOffsetToPosition(actualSelection.End) + "]");
            Log("Type of the symbol at selection.Start in forward direction: " +
                actualSelection.Start.GetPointerContext(LogicalDirection.Forward).ToString());
            Verifier.Verify(actualSelection.Text == "", "Selection should be empty", true);
            Verifier.Verify(actualSelection.Start.GetOffsetToPosition(actualSelection.End) == 0,
                "Start and End TextPointers of the selection container should be equal", true);
            Logger.Current.ReportSuccess();
        }
    }

    /// <summary>
    /// Verifies that on setting selected text in textbox
    /// the textbox is reinitialised with no text selected - adhoc case
    /// </summary>
    [Test(3, "TextBox", "TextBoxSelectionAfterSetReproRegression_Bug904", MethodParameters = "/TestCaseType:TextBoxSelectionAfterSetReproRegression_Bug904")]
    [TestTactics("545"), TestOwner("Microsoft"), TestBugs("Regression_Bug904")]
    public class TextBoxSelectionAfterSetReproRegression_Bug904 : CustomTestCase
    {
        /// <summary>Runs the test case</summary>
        public override void RunTestCase()
        {
            _canvasPanel = new Canvas();
            _testTB = new TextBox();
            _testTB.FontSize = 24;
            _testTB.Height = 400;
            _testTB.Width = 400;
            _testTB.Text = "hello";
            _testTB.Select(1, 1);
            _testTB.Focus();

            _canvasPanel.Children.Add(_testTB);
            MainWindow.Content = _canvasPanel;

            QueueDelegate(new SimpleHandler(ResetText));
        }

        /// <summary>Resets text in textbox</summary>
        private void ResetText()
        {
            _testTB.Text = "world";
            Log("Moving the mouse and clicking it at the end of the Button");
            QueueDelegate(VerifyTBSelection);
        }

        /// <summary>Verifies that no selection has been made.</summary>
        private void VerifyTBSelection()
        {
            String actualSelection = _testTB.SelectedText;
            Log("Selection contents of the textbox [" + actualSelection + "]");
            Verifier.Verify(actualSelection == "", "Selection should be empty", true);
            Logger.Current.ReportSuccess();
        }

        #region private data.

        Canvas _canvasPanel;
        TextBox _testTB;

        #endregion private data.
    }

    /// <summary>
    /// A test case specifically written to repro Regression_Bug614. AND #Regression_Bug652
    /// Verifies that the selection covers alltext when made bold.
    /// Verifies that after Shift+Ctrl , ctrlL and CtrlR work according to expectations
    /// </summary>
    [Test(3, "Selection", "RichTextBoxSelectionOnBoldRepro", MethodParameters = "/TestCaseType:RichTextBoxSelectionOnBoldRepro")]
    [TestTactics("543"), TestOwner("Microsoft"), TestBugs("614, 652")]
    public class RichTextBoxSelectionOnBoldRepro : CustomTestCase
    {
        /// <summary>Runs the test case</summary>
        public override void RunTestCase()
        {
            KeyboardInput.SetActiveInputLocale("00000401");
            
            _canvasPanel = new Canvas();
            _testRTB = new RichTextBox();
            _testRTB.FontSize = 14;
            _testRTB.Padding=new Thickness();
            _testRTB.Margin = new Thickness(0);
            _testRTB.Height = 400;
            _testRTB.Width = 800;            

            _ControlWrapper = new UIElementWrapper(_testRTB);
            _ControlWrapper.Clear();
            _initialText = "1111111111111111111111111111111111111111111";
            Paragraph p1 = new Paragraph(new Run(_initialText));
            _testRTB.Document.Blocks.Add(p1);
            _canvasPanel.Children.Add(_testRTB);
            MainWindow.Content = _canvasPanel;
            
            QueueDelegate(FindInitialString);
        }

        /// <summary>get string in control element</summary>
        private void FindInitialString()
        {
            _testRTB.Focus();
            _testRTB.SelectAll();
            _initialText = _testRTB.Selection.Text;
            QueueDelegate(DoMouseClick);
        }

        private void DoMouseClick()
        {
            MouseInput.MouseClick(_testRTB);
            QueueDelegate(DoSelection);
        }

        /// <summary>Bolds the text</summary>
        private void DoSelection()
        {            
            KeyboardInput.TypeString("^A^B");
            Log("SELECTED AND BOLDED THE TEXT");
            QueueDelegate(VerifyRTBboldSelection);
        }

        /// <summary>Verifies that selection covers all text.</summary>
        private void VerifyRTBboldSelection()
        {
            _finalText = _testRTB.Selection.Text;
            Log("Before Copy Paste:[" + _initialText + "] After Copy Paste:[" + _finalText+"]");
            Verifier.Verify(_initialText == _finalText, "on making the text bold, selection doesnt cover all text", false);
            
            //next case
            _ControlWrapper.Clear();
            MouseInput.MouseClick(_testRTB);
            QueueDelegate(DoControlHome);
        }

        private void DoControlHome()
        {
            KeyboardInput.TypeString("^{HOME}");
            QueueDelegate(FindInitialCursorLocation);
        }

        /// <summary>Finds initial cursor position</summary>
        private void FindInitialCursorLocation()
        {
            MouseInput.MouseClick(_testRTB);
            _p = _ControlWrapper.GetDocumentRelativeCaretPosition();
            _initialX = (int)_p.X;
            _initialY = (int)_p.Y;
            QueueDelegate(DoControlShiftRight);
        }

        private void DoControlShiftRight()
        {
            if (KeyboardInput.IsBidiInputLanguageInstalled())
            {
                KeyboardEditingData[] data = KeyboardEditingData.GetValues(KeyboardEditingTestValue.ControlShiftRight);
                data[0].PerformAction(_ControlWrapper, null);
                if (_testRTB.FlowDirection != FlowDirection.RightToLeft)
                {
                    Input.SendKeyboardInput(Key.RightCtrl, true);
                    Input.SendKeyboardInput(Key.RightShift, true);
                    Input.SendKeyboardInput(Key.RightCtrl, false);
                    Input.SendKeyboardInput(Key.RightShift, false);
                }
            }
            else
            {
                _testRTB.Document.Blocks.FirstBlock.FlowDirection = FlowDirection.RightToLeft;
            }
            DispatcherHelper.DoEvents();
            QueueDelegate(VerifyChangeFlowDirection);
        }

        /// <summary>Verifies that Crtl shift moves cursor to right</summary>
        private void VerifyChangeFlowDirection()
        {
            _finalX = 0;
            //_finalPoint = _ControlWrapper.GetDocumentRelativeCaretPosition();
            Rect rec = _ControlWrapper.GetGlobalCharacterRect(_testRTB.CaretPosition);

            _finalX = (int)rec.Left;
            _finalY = (int)rec.Bottom;
            Verifier.Verify((_finalX > (int)(_testRTB.Width - 6)), "Cursor should be at the right end Expected [>" +_testRTB.Width.ToString() +" +-13] Actual:[" + _finalX.ToString(), true);
            QueueDelegate(ChangeToLeft);
        }

        /// <summary>Chnage alighment to left</summary>
        private void ChangeToLeft()
        {
            MouseInput.MouseClick(_testRTB);
            QueueDelegate(DoControlL);
        }

        private void DoControlL()
        {
            KeyboardInput.TypeString("^L");
            QueueDelegate(VerifyRTBtextPosition);
        }

        /// <summary>Verifies that cursor has moved to the left</summary>
        private void VerifyRTBtextPosition()
        {
            _finalPoint = _ControlWrapper.GetDocumentRelativeCaretPosition();
            _finalX = (int)(_finalPoint.X + 0.4);
            _finalY = (int)(_finalPoint.Y + 0.4);
            Verifier.Verify((_initialX == _finalX) && (_finalY == _initialY), "Cursor should be in same position Expected [" + _p.ToString() + "] Actual:[" + _finalPoint.ToString(), true);
            Logger.Current.ReportSuccess();
        }

        #region private data.

        private Point _p;
        private int _initialX;
        private int _initialY;
        private Point _finalPoint;
        private string _initialText, _finalText;
        int _finalX;
        int _finalY;

        private Canvas _canvasPanel;
        private RichTextBox _testRTB;
        private UIElementWrapper _ControlWrapper;

        #endregion private data.
    }

    /// <summary>
    /// Regression for Regression_Bug582 - Selection toggle betwwen "empty" and "Paragraph" when continue holding shift-right key at the end of the document
    /// </summary>
    [TestOwner("Microsoft"), TestTactics(""), TestBugs("684"), TestWorkItem(""), TestLastUpdatedOn("July 13, 2006")]
    public class RegressionTest_Regression_Bug684 : CombinedTestCase
    {
        UIElementWrapper _wrapper;
        int _count = 5; 

        /// <summary>
        /// Start to run test.
        /// </summary>
        public override void RunTestCase()
        {
            RichTextBox richTextBox = new RichTextBox();
            richTextBox.Document.Blocks.Add(new Paragraph(new Run("Test")));
            _wrapper = new UIElementWrapper(richTextBox);
            MainWindow.Content = richTextBox;
            QueueDelegate(SetFocus);
        }
        
        void SetFocus()
        {
            _wrapper.Element.Focus();
            QueueDelegate(MakeFirstSelection);
        }

        void MakeFirstSelection()
        {
            KeyboardInput.TypeString("^{END}+{RIGHT}");
            QueueDelegate(MakeSecondSelection);
        }

        void MakeSecondSelection()
        {  
            Verifier.Verify(_wrapper.SelectionInstance.Text == "\r\n", "Failed - Selected text expected[\r\n], actual[" + _wrapper.SelectionInstance.Text + "]");
            KeyboardInput.TypeString("+{RIGHT}");
            _count--;
            if (_count <= 0)
            {
                EndTest();
            }
            else
            {
                QueueDelegate(MakeSecondSelection);
            }
        }
    }

    /// <summary>
    /// we need to make sure that specfy empty decorations to empty decrations will not create Undo unit.
    /// Regression_Bug583 = TextSchema.ValuesAreEqual() should handle comparing null with empty TextEffectCollection values
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("29"), TestBugs("583"), TestWorkItem(""), TestLastUpdatedOn("July 17, 2006")]
    public class RegressionTest_Regression_Bug583 : CombinedTestCase
    {
        RichTextBox _richTextBox; 
        /// <summary>
        /// start the test.
        /// </summary>
        public override void  RunTestCase()
        {
            _richTextBox = new RichTextBox();
            _richTextBox.Document.Blocks.Clear();
            _richTextBox.Document.Blocks.Add(new Paragraph(new Run("First Paragraph.")));
            _richTextBox.Document.Blocks.Add(new Paragraph(new Run("Second Paragraph.")));
            MainWindow.Content = _richTextBox;
            QueueDelegate(SetFocus);
        }

        void SetFocus()
        {
            _richTextBox.Focus();
            
            //Note, textDecoration property will never be null even when null is specified.
            _richTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, null);
            _richTextBox.SelectAll();
            QueueDelegate(SetProperty);
        }

        void SetProperty()
        {
            TextDecorationCollection properties;
            properties = _richTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty) as TextDecorationCollection ;
            Verifier.Verify(properties.Count == 0, "Failed - expected empty TextDecorations property!");

            _richTextBox.Document.Blocks.Add(new Paragraph(new Run("Third Paragraph.")));

            Verifier.Verify(_richTextBox.Document.Blocks.Count ==3, "Expected Paragraphs[3], Actual[" + _richTextBox.Document.Blocks.Count + "]");

            _richTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, new TextDecorationCollection());
            _richTextBox.Undo();

            //This verify that no extra Undo unit is created, specify an empty textDecorationCollection.
            Verifier.Verify(_richTextBox.Document.Blocks.Count == 2, "Expected Paragraphs[2], Actual[" + _richTextBox.Document.Blocks.Count + "]");

            _richTextBox.Document.Blocks.Add(new Paragraph(new Run("Third Paragraph.")));

            TextDecorationCollection decrations = new TextDecorationCollection();
            decrations.Add(TextDecorations.Underline);
            
            //add a decoration to the collection will create an undo unit.
            _richTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, decrations);
            _richTextBox.Undo();
            
            //make sure that undo here only undo the Underline.
            Verifier.Verify(_richTextBox.Document.Blocks.Count == 3, "Expected Paragraphs[3], Actual[" + _richTextBox.Document.Blocks.Count + "]");

            EndTest();            
        }
    }
}
