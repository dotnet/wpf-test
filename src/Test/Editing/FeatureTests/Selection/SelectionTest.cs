// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Unit testing for public API for TextSelection class

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Threading;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;

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

    /// <summary>
    /// Runs a test case by typing a string and verifying the resulting selection.
    /// </summary>
    [Test(0, "Selection", "SelectionByKeyboardTest1", MethodParameters = "/TestCaseType:SelectionByKeyboardTest /Priority:0", Timeout=480)]
    [Test(2, "Selection", "SelectionByKeyboardTest2", MethodParameters = "/TestCaseType:SelectionByKeyboardTest /Priority:1", Timeout = 480)]
    [TestOwner("Microsoft"), TestTitle("SelectionByKeyboardTest"), TestTactics("671, 672"), TestBugs("142, 811, 812, 813, 814")]
    public class SelectionByKeyboardTest : CustomTestCase
    {
        #region TestCaseData

        /// <summary>Data driven test cases.</summary>
        internal class TestData
        {
            #region PrivateData

            string _testControlTypeName;
            bool _acceptsReturn;
            string _keystrokeString;
            string _selectedText;
            string _dismissSelectionKeystroke;
            string _expStringOnCaretRight;
            string _expStringOnCaretLeft;
            int _priority;

            #endregion PrivateData

            #region InternalProperties

            internal string TestControlTypeName { get { return _testControlTypeName; } }
            internal bool AcceptsReturn { get { return _acceptsReturn; } }
            internal string KeystrokeString { get { return _keystrokeString; } }
            internal string SelectedText { get { return _selectedText; } }
            internal string DismissSelectionKeystroke { get { return _dismissSelectionKeystroke; } }
            internal string ExpStringOnCaretRight { get { return _expStringOnCaretRight; } }
            internal string ExpStringOnCaretLeft { get { return _expStringOnCaretLeft; } }
            internal int Priority { get { return _priority; } }

            #endregion InternalProperties

            internal TestData(string testControlTypeName, bool acceptsReturn, string keystrokeString,
                              string selectedText, string dismissSelectionKeystroke, string expStringOnCaretRight,
                              string expStringOnCaretLeft, int priority)
            {
                _testControlTypeName = testControlTypeName;
                _acceptsReturn = acceptsReturn;
                _keystrokeString = keystrokeString;
                _selectedText = selectedText;
                _dismissSelectionKeystroke = dismissSelectionKeystroke;
                _expStringOnCaretRight = expStringOnCaretRight;
                _expStringOnCaretLeft = expStringOnCaretLeft;
                _priority = priority;
            }

            internal static TestData[] TestCases = new TestData[] {

                #region -------------LEFT-----------------
                new TestData("TextBox", true,
                            "This is a test{ENTER}This is a test{ENTER}{ENTER}This is a test{ENTER}This is a test{LEFT 3}+{LEFT 3}",
                            "a t", "{LEFT}",
                            "a test", "This is a test\r\nThis is a test\r\n\r\nThis is a test\r\nThis is ", 0),

                new TestData("TextBox", true,
                            "This is a test{ENTER}This is a test{ENTER}{ENTER}This is a test{ENTER}This is a test{LEFT 3}+{LEFT 3}",
                            "a t", "{RIGHT}",
                            "est", "This is a test\r\nThis is a test\r\n\r\nThis is a test\r\nThis is a t", 0),
                #endregion --------------LEFT------------------

                #region --------------UP------------------
                new TestData("TextBox", true,
                            "This is a test{ENTER}This is a test{ENTER}{ENTER}This is a test{ENTER}This is a test{LEFT 3}{UP 1}+{UP 3}",
                            "est\r\nThis is a test\r\n\r\nThis is a t", "{LEFT}",
                            "est\r\nThis is a test\r\n\r\nThis is a test\r\nThis is a test", "This is a t", 0),
                new TestData("TextBox", true,
                            "This is a test{ENTER}This is a test{ENTER}{ENTER}This is a test{ENTER}This is a test{LEFT 3}{UP 1}+{UP 3}",
                            "est\r\nThis is a test\r\n\r\nThis is a t", "{RIGHT}",
                            "est\r\nThis is a test", "This is a test\r\nThis is a test\r\n\r\nThis is a t", 0),
                new TestData("TextBox", true,
                            "This is a test{ENTER}This is a test{ENTER}{ENTER}This is a test{ENTER}This is a test{LEFT 3}+{UP 3}",
                            "est\r\n\r\nThis is a test\r\nThis is a t", "{UP}",
                            "est\r\nThis is a test\r\n\r\nThis is a test\r\nThis is a test", "This is a t", 1),
                new TestData("TextBox", true,
                            "This is a test{ENTER}This is a test{ENTER}{ENTER}This is a test{ENTER}This is a test{LEFT 3}+{UP 3}",
                            "est\r\n\r\nThis is a test\r\nThis is a t", "{DOWN}",
                            "est", "This is a test\r\nThis is a test\r\n\r\nThis is a test\r\nThis is a t", 1),
                #endregion --------------UP------------------

                #region -------------DOWN-----------------
                new TestData("TextBox", true,
                            "This is a test{ENTER}This is a test{ENTER}{ENTER}This is a test{LEFT 3}{UP 3}{DOWN 1}+{DOWN 2}",
                            "est\r\n\r\nThis is a t", "{LEFT}",
                            "est\r\n\r\nThis is a test", "This is a test\r\nThis is a t", 0),
                new TestData("TextBox", true,
                            "This is a test{ENTER}This is a test{ENTER}{ENTER}This is a test{LEFT 3}{UP 3}{DOWN 1}+{DOWN 2}",
                            "est\r\n\r\nThis is a t", "{RIGHT}",
                            "est", "This is a test\r\nThis is a test\r\n\r\nThis is a t", 0),
                new TestData("TextBox", true,
                            "This is a test{ENTER}This is a test{ENTER}{ENTER}This is a test{LEFT 3}{UP 3}+{DOWN 3}",
                            "est\r\nThis is a test\r\n\r\nThis is a t", "{UP}",
                            "est\r\nThis is a test\r\n\r\nThis is a test", "This is a t", 1),
                new TestData("TextBox", true,
                            "This is a test{ENTER}This is a test{ENTER}{ENTER}This is a test{ENTER}This is a test{LEFT 3}{UP 4}+{DOWN 3}",
                            "est\r\nThis is a test\r\n\r\nThis is a t", "{DOWN}",
                            "est", "This is a test\r\nThis is a test\r\n\r\nThis is a test\r\nThis is a t", 1),
                #endregion -------------DOWN-----------------

                #region -------------HOME-----------------
                new TestData("TextBox", true,
                            "This is a test{HOME}{RIGHT 11}+{HOME}",
                            "This is a t", "{LEFT}",
                            "This is a test", "", 0),
                new TestData("TextBox", true,
                            "This is a test{HOME}{RIGHT 11}+{HOME}",
                            "This is a t", "{RIGHT}",
                            "est", "This is a t", 0),
                new TestData("TextBox", true,
                            "This is a test{ENTER}This is a test{ENTER}This is a test{UP}{HOME}{RIGHT 11}+{HOME}",
                            "This is a t", "{UP}",
                            "This is a test\r\nThis is a test\r\nThis is a test", "", 1),
                new TestData("TextBox", true,
                            "This is a test{ENTER}This is a test{ENTER}This is a test{UP}{HOME}{RIGHT 11}+{HOME}",
                            "This is a t", "{DOWN}",
                            "est", "This is a test\r\nThis is a test\r\nThis is a t", 1),
                #endregion -------------HOME-----------------

                #region -------------END-----------------
                new TestData("TextBox", true,
                            "This is a test{HOME}{END}{LEFT 7}+{END}",
                            " a test", "{LEFT}",
                            " a test", "This is", 0),
                new TestData("TextBox", true,
                            "This is a test{HOME}{END}{LEFT 7}+{END}",
                            " a test", "{RIGHT}",
                            "", "This is a test", 0),
                new TestData("TextBox", true,
                            "This is a test{ENTER}This is a test{ENTER}This is a test{UP}{LEFT 7}+{END}",
                            " a test", "{UP}",
                            " a test\r\nThis is a test\r\nThis is a test", "This is", 1),
                new TestData("TextBox", true,
                            "This is a test{ENTER}This is a test{ENTER}This is a test{UP}{HOME}{END}{LEFT 7}+{END}",
                            " a test", "{DOWN}",
                            "", "This is a test\r\nThis is a test\r\nThis is a test", 1),
                #endregion -------------END-----------------

                #region -------------PGUP-----------------
                new TestData("TextBox", true,
                            "This is a test{ENTER}This is a test{ENTER}{ENTER}This is a test{ENTER}This is a test{LEFT 3}{PGUP}{DOWN 5}+{PGUP}",
                            "est\r\nThis is a test\r\n\r\nThis is a test\r\nThis is a t", "{LEFT}",
                            "est\r\nThis is a test\r\n\r\nThis is a test\r\nThis is a test", "This is a t", 0),
                new TestData("TextBox", true,
                            "This is a test{ENTER}This is a test{ENTER}{ENTER}This is a test{ENTER}This is a test{LEFT 3}{PGUP}{DOWN 5}+{PGUP}",
                            "est\r\nThis is a test\r\n\r\nThis is a test\r\nThis is a t", "{RIGHT}",
                            "est", "This is a test\r\nThis is a test\r\n\r\nThis is a test\r\nThis is a t", 0),
                new TestData("TextBox", true,
                            "This is a test{ENTER}This is a test{ENTER}{ENTER}This is a test{ENTER}This is a test{LEFT 3}{PGUP}{DOWN 5}+{PGUP}",
                            "est\r\nThis is a test\r\n\r\nThis is a test\r\nThis is a t", "{UP}",
                            "est\r\nThis is a test\r\n\r\nThis is a test\r\nThis is a test", "This is a t", 1),
                new TestData("TextBox", true,
                            "This is a test{ENTER}This is a test{ENTER}{ENTER}This is a test{ENTER}This is a test{LEFT 3}{PGUP}{DOWN 3}+{PGUP}",
                            "est\r\nThis is a test\r\n\r\nThis is a t", "{DOWN}",
                            "est", "This is a test\r\nThis is a test\r\n\r\nThis is a test\r\nThis is a t", 1),
                #endregion -------------PGUP-----------------

                #region -------------PGDW-----------------
                new TestData("TextBox", true,
                            "This is a test{ENTER}This is a test{ENTER}{ENTER}This is a test{ENTER}This is a test{LEFT 3}{UP 5}{PGDN}{PGUP}+{PGDN}",
                            "est\r\nThis is a test\r\n\r\nThis is a test\r\nThis is a t", "{LEFT}",
                            "est\r\nThis is a test\r\n\r\nThis is a test\r\nThis is a test", "This is a t", 0),
                new TestData("TextBox", true,
                            "This is a test{ENTER}This is a test{ENTER}{ENTER}This is a test{ENTER}This is a test{LEFT 3}{UP 5}{PGDN}{PGUP}+{PGDN}",
                            "est\r\nThis is a test\r\n\r\nThis is a test\r\nThis is a t", "{RIGHT}",
                            "est", "This is a test\r\nThis is a test\r\n\r\nThis is a test\r\nThis is a t", 0),
                new TestData("TextBox", true,
                            "This is a test{ENTER}This is a test{ENTER}{ENTER}This is a test{ENTER}This is a test{LEFT 3}{UP 5}{PGDN}{PGUP}{DOWN}+{PGDN}",
                            "est\r\n\r\nThis is a test\r\nThis is a t", "{UP}",
                            "est\r\nThis is a test\r\n\r\nThis is a test\r\nThis is a test", "This is a t", 1),
                new TestData("TextBox", true,
                            "This is a test{ENTER}This is a test{ENTER}{ENTER}This is a test{ENTER}This is a test{LEFT 3}{UP 5}{PGDN}{PGUP}+{PGDN}",
                            "est\r\nThis is a test\r\n\r\nThis is a test\r\nThis is a t", "{DOWN}",
                            "est", "This is a test\r\nThis is a test\r\n\r\nThis is a test\r\nThis is a t", 1),
                #endregion -------------PGDW-----------------

                #region -------------RTOL-----------------
                new TestData("TextBox", false,
                            "This is a test{LEFT 3}+{LEFT 3}",
                            "a t", "{LEFT}",
                            "a test", "This is ", 0),
                new TestData("TextBox", false,
                            "This is a test{LEFT 3}+{LEFT 3}",
                            "a t", "{RIGHT}",
                            "est", "This is a t", 0),
                new TestData("TextBox", true,
                            "This is a test{ENTER}This is a test{ENTER}{ENTER}This is a test{LEFT 3}+{LEFT 34}",
                            "a test\r\nThis is a test\r\n\r\nThis is a t", "{DOWN}",
                            "est", "This is a test\r\nThis is a test\r\n\r\nThis is a t", 1),
                new TestData("TextBox", true,
                            "This is a test{ENTER}This is a test{ENTER}{ENTER}This is a test{LEFT 3}+{LEFT 19}",
                            "a test\r\n\r\nThis is a t", "{UP}",
                            "a test\r\nThis is a test\r\n\r\nThis is a test", "This is ", 1),
                #endregion -------------RTOL-----------------

                #region -------------LTOR-----------------
                new TestData("TextBox", false,
                            "This is a test{LEFT 6}+{RIGHT 3}",
                            "a t", "{RIGHT}",
                            "est", "This is a t", 0),
                new TestData("TextBox", false,
                            "This is a test{LEFT 6}+{RIGHT 3}",
                            "a t", "{LEFT}",
                            "a test", "This is ", 0),
                new TestData("TextBox", true,
                            "This is a test{ENTER}This is a test{ENTER}{ENTER}This is a test{ENTER}This is a test{LEFT 6}{UP 4}+{RIGHT 49}",
                            "a test\r\nThis is a test\r\n\r\nThis is a test\r\nThis is a t", "{UP}",
                            "a test\r\nThis is a test\r\n\r\nThis is a test\r\nThis is a test", "This is ", 1),
                new TestData("TextBox", true,
                            "This is a test{ENTER}This is a test{ENTER}{ENTER}This is a test{ENTER}This is a test{LEFT 6}{UP 4}+{RIGHT 34}",
                            "a test\r\nThis is a test\r\n\r\nThis is a t", "{DOWN}",
                            "est", "This is a test\r\nThis is a test\r\n\r\nThis is a test\r\nThis is a t", 1),
                #endregion -------------LTOR-----------------

                //-------------CNTR-----------------
                new TestData("TextBox", true,
                            "This is a test{ENTER}This is a test{ENTER}{ENTER}This is a test{PGUP}{HOME}^{RIGHT 2}^+{RIGHT 8}",
                            "a test\r\nThis is a test\r\n", "{LEFT}",
                            "a test\r\nThis is a test\r\n\r\nThis is a test", "This is ", 0),
                //-------------CNTL-----------------
                new TestData("TextBox", true,
                            "This is a test{ENTER}This is a test{ENTER}{ENTER}This is a test^{LEFT 2}^+{LEFT 8}",
                            "This is a test\r\n\r\nThis is ", "{LEFT}",
                            "This is a test\r\n\r\nThis is a test", "This is a test\r\n", 0),
                //-------------CNTH-----------------
                new TestData("TextBox", true,
                            "This is a test{ENTER}This is a test{ENTER}{ENTER}This is a test^{HOME}^{RIGHT 10}^+{HOME}",
                            "This is a test\r\nThis is a test\r\n", "{LEFT}",
                            "This is a test\r\nThis is a test\r\n\r\nThis is a test", "", 0),
                //-------------CNTE-----------------
                new TestData("TextBox", true,
                            "This is a test{ENTER}This is a test{ENTER}{ENTER}This is a test^{LEFT 7}^{END}^{LEFT 6}^+{END}",
                            "\r\n\r\nThis is a test", "{RIGHT}",
                            "", "This is a test\r\nThis is a test\r\n\r\nThis is a test", 0),
                //-------------BackSpace-----------------
                new TestData("TextBox", false,
                            "This is a test{LEFT 6}+{RIGHT 3}",
                            "a t", "{BACKSPACE}",
                            "est", "This is ", 0),
                //-------------Delete-----------------
                new TestData("TextBox", false,
                            "This is a test{LEFT 6}+{RIGHT 3}",
                            "a t", "{DELETE}",
                            "est", "This is ", 0),
                //-------------ShiftHome-----------------
                new TestData("TextBox", false,
                            "This is a test{LEFT 3}+{HOME}",
                            "This is a t", "",
                            "est", "", 0),
                //-------------ShiftEnd-----------------
                new TestData("TextBox", false,
                            "This is a test{LEFT 3}+{END}",
                            "est", "",
                            "", "This is a t", 0),
                //-------------CntrlShiftEnd-----------------
                new TestData("TextBox", true,
                            "This is a test{ENTER}This is 2nd line of test (longer){ENTER}This is 3rd line of test{UP 2}^+{END}",
                            "\r\nThis is 2nd line of test (longer)\r\nThis is 3rd line of test", "{UP}{END}",
                            "\r\nThis is 2nd line of test (longer)\r\nThis is 3rd line of test", "This is a test", 0),
                //-------------CntrlShiftHome-----------------
                new TestData("TextBox", true,
                            "This is a test{ENTER}This is 2nd line of test (longer){ENTER}This is 3rd line of test^+{HOME}",
                            "This is a test\r\nThis is 2nd line of test (longer)\r\nThis is 3rd line of test", "{RIGHT}",
                            "", "This is a test\r\nThis is 2nd line of test (longer)\r\nThis is 3rd line of test", 0),

                //-------------Regression_Bug545---------------------
                new TestData("TextBox", true,
                            "This is a test{ENTER}This is 2nd line{ENTER} {ENTER}This is 3rd line{ENTER}This is 4th line^{HOME}{DOWN 4}",
                            "", "{RIGHT}",
                            "his is 4th line", "This is a test\r\nThis is 2nd line\r\n \r\nThis is 3rd line\r\nT", 1),
            };
        }

        /// <summary>Current test data being used</summary>
        private TestData _testData;

        /// <summary>Current TestData index being tested.</summary>
        int _currentIndex = 0;
        int _endIndex;

        bool _testFailed = false;
        int[] _testCaseFailed = new int[TestData.TestCases.Length];

        UIElement _testControl;
        UIElementWrapper _testControlWrapper;
        TextSelection _textSelection;

        #endregion TestCaseData

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            _endIndex = TestData.TestCases.Length;
            for (int i = 0; i < _testCaseFailed.Length; i++)
                _testCaseFailed[i] = 0;
            if (ConfigurationSettings.Current.GetArgumentAsInt("Repro") != 0)
            {
                _currentIndex = ConfigurationSettings.Current.GetArgumentAsInt("Repro");
                _endIndex = _currentIndex + 1;
            }
            
            RunCase();
        }

        /// <summary>Runs for each test in this test case</summary>
        private void RunCase()
        {
            if (_currentIndex < _endIndex)
            {
                _testData = TestData.TestCases[_currentIndex];
                if (ConfigurationSettings.Current.GetArgumentAsInt("Priority") == _testData.Priority)
                {
                    Log("Running test case: " + _currentIndex);
                    SetUpTestCase();
                }
                else
                {
                    _currentIndex++;
                    QueueHelper.Current.QueueDelegate(new SimpleHandler(RunCase));
                }
            }
            else
            {
                if (!_testFailed)
                    Logger.Current.ReportSuccess();
                else
                {
                    string failMessage = "The following test cases have failed: [";
                    for (int i = 0; i < _testCaseFailed.Length; i++)
                    {
                        if (_testCaseFailed[i] == -1)
                            failMessage += " " + i + ",";
                    }
                    failMessage += "]";
                    failMessage += "\nTo re-run just the failed test case, add the following command line to the existing one: /Repro:<test case number>";
                    Log(failMessage);
                    Logger.Current.ReportResult(false, "SelectionByKeyboardTest has failed", false);
                }
            }
        }

        /// <summary>Sets up the testcontrol based on the _testData.</summary>
        private void SetUpTestCase()
        {
            TextBoxBase textBox;

            if (ConfigurationSettings.Current.GetArgument("TestControl") == "")
            {
                if (_testData.TestControlTypeName == "TextBox")
                {
                    _testControl = new TextBox();
                }
                else if (_testData.TestControlTypeName == "RichTextBox")
                {
                    _testControl = new RichTextBox();
                }
                else
                {
                    Logger.Current.ReportResult(false, "Un-usable TestControl", false);
                }
            }
            else
            {
                _testControl = (UIElement)ReflectionUtils.CreateInstanceOfType(
                                ConfigurationSettings.Current.GetArgument("TestControl"), null);
            }
            _testControlWrapper = new UIElementWrapper(_testControl);

            if (_testData.AcceptsReturn)
            {
                textBox = _testControl as TextBoxBase;

                if (textBox != null)
                {
                    textBox.AcceptsReturn = true;
                }
            }

            MainWindow.Content = (FrameworkElement)_testControlWrapper.Element;
            _textSelection = _testControlWrapper.SelectionInstance;
            Verifier.Verify(_textSelection != null, "TextSelection can be obtained from the element", true);
            QueueHelper.Current.QueueDelayedDelegate(new System.TimeSpan(0, 0, 0, 3),
                new SimpleHandler(OnAdded), new object[] {});
        }

        private void OnAdded()
        {
            MouseInput.MouseClick(_testControlWrapper.Element);
            QueueHelper.Current.QueueDelegate(new SimpleHandler(TestTextSelectionProperties));
        }

        /// <summary>Tests the TextSelection.</summary>
        private void TestTextSelectionProperties()
        {
            KeyboardInput.TypeString(_testData.KeystrokeString);
            QueueHelper.Current.QueueDelegate(new SimpleHandler(OnSelection));
        }

        private void OnSelection()
        {
            string selectedText;

            // if IsElementRichText is true we want to preserve formatting tags
            // for verification purposes
            // else we just want plain text
            selectedText = _testControlWrapper.GetSelectedText(_testControlWrapper.IsElementRichText,
                false);

            string output = String.Format(" Selected string value [{0}]. Expected value [{1}]", selectedText, _testData.SelectedText);

            if (selectedText != _testData.SelectedText)
            {
                string message = "TC#" + _currentIndex + output;
                Logger.Current.ReportResult(false, message, true);
                _testFailed = true;
                _testCaseFailed[_currentIndex] = -1;
            }

            KeyboardInput.TypeString(_testData.DismissSelectionKeystroke);
            QueueHelper.Current.QueueDelegate(new SimpleHandler(OnVerifyCaretPosition));
        }

        private void OnVerifyCaretPosition()
        {
            string message;
            string textOutsideSelectionOnLeft;
            string textOutsideSelectionOnRight;
            string output;

            textOutsideSelectionOnLeft = _testControlWrapper.GetTextOutsideSelection(LogicalDirection.Backward,
                _testControlWrapper.IsElementRichText);

            textOutsideSelectionOnRight = _testControlWrapper.GetTextOutsideSelection(LogicalDirection.Forward,
                _testControlWrapper.IsElementRichText);

            output = String.Format("Text on left hand side of caret [{0}]. Expected value [{1}]",
                textOutsideSelectionOnLeft,
                _testData.ExpStringOnCaretLeft);

            if (textOutsideSelectionOnLeft != _testData.ExpStringOnCaretLeft)
            {
                message = "TC#" + _currentIndex + output;
                Logger.Current.ReportResult(false, message, true);
                _testFailed = true;
                _testCaseFailed[_currentIndex] = -1;
            }

            output = String.Format("Text on right hand side of caret [{0}]. Expected value [{1}]",
                textOutsideSelectionOnRight,
                _testData.ExpStringOnCaretRight);

            if (textOutsideSelectionOnRight != _testData.ExpStringOnCaretRight)
            {
                message = "TC#" + _currentIndex + output;
                Logger.Current.ReportResult(false, message, true);
                _testFailed = true;
                _testCaseFailed[_currentIndex] = -1;
            }

            _currentIndex++;
            QueueHelper.Current.QueueDelegate(new SimpleHandler(RunCase));
        }
    }

    /// <summary>
    /// Verifies that when a property is spring-loaded on the selection, all
    /// other properties at that location still have the correct values.
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("510"), TestBugs("618")]
    public class TextSelectionReproRegression_Bug618: TextBoxTestCase
    {
        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            Log("Setting sample text...");
            TestWrapper.Text = "123";

            Log("Changing family (Tahoma) and size (32) on selection...");

            for (int i = 0; i < 2; i++)
            {
                //bool moveResult = TestWrapper.SelectionInstance.End.MoveToNextContextPosition(LogicalDirection.Forward);
                bool moveResult = (TestWrapper.SelectionInstance.End.GetNextContextPosition(LogicalDirection.Forward) == null) ? false : true;
                //--bool moveResult = TestWrapper.SelectionInstance.MoveEnd(LogicalDirection.Forward);

                Verifier.Verify(moveResult, "TextSelection.MoveEnd is successful");
            }

            TextPointer nav = TestWrapper.SelectionInstance.Start;

            TextRange textRange = new TextRange(TestWrapper.SelectionInstance.Start, TestWrapper.SelectionInstance.End);

            // Disabled due to breaking change
            textRange.ApplyPropertyValue(TextElement.FontFamilyProperty, new FontFamily("Tahoma"));
            textRange.ApplyPropertyValue(TextElement.FontSizeProperty, 32.0);

            Log("Collapsing selection to modified range..");
            nav = nav.GetNextInsertionPosition(LogicalDirection.Backward);
            TestWrapper.SelectionInstance.Select(nav, nav);

            Log("Verifying that properties are still set...");
            /*
            Log("FontFamily: " + TestWrapper.SelectionInstance.FontFamily);
            Verifier.Verify(
                TestWrapper.SelectionInstance.FontFamily == "Tahoma",
                "Font family is still Tahoma", true);
            Log("FontSize: " + TestWrapper.SelectionInstance.FontSize.ToString());
            Verifier.Verify(
                TestWrapper.SelectionInstance.FontSize.Pixels == 32,
                "Font family is still 32 pixels", true);

            Log("Modifying spring-loaded font name (Verdana)...");
            TestWrapper.SelectionInstance.FontFamily = "Verdana";
            Log("FontFamily: " + TestWrapper.SelectionInstance.FontFamily);
            Verifier.Verify(
                TestWrapper.SelectionInstance.FontFamily == "Verdana",
                "Font family is still Verdana", true);
            Log("FontSize: " + TestWrapper.SelectionInstance.FontSize.ToString());
            Verifier.Verify(
                TestWrapper.SelectionInstance.FontSize.Pixels == 32,
                "Font family is still 32 pixels", true);
                */

            Logger.Current.ReportSuccess();
        }
    }

    /// <summary>
    /// This test makes sure that springloaded prperty works correctly. The test
    /// first type in initial content (InitialStringContent), set selection to have
    /// bold (at that point selection is empty, but bold should be spring loaded)
    /// and any text typed after that point should be bold.
    /// </summary>
    [Test(0, "RichEditing", "SpringLoadedPropertyTest", MethodParameters = "/TestCaseType:SpringLoadedPropertyTest /xml:testxml.xml /TestName:SpringLoadedPropertyTest--1")]
    [TestOwner("Microsoft"), TestTactics("511"), TestTitle("SpringLoadedPropertyTest"),
     TestArgument("MainXaml", "xaml string in xml"),
     TestArgument("InitialStringContent", "Initial string in the textbox"),
     TestArgument("TestString", "Test string to type"),
     TestArgument("ExpectedString", "Expected string to have property set on")]
    public class SpringLoadedPropertyTest : CustomTestCase
    {
        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            string xamlFile;

            xamlFile = ConfigurationSettings.Current.GetArgument("MainXaml");
            ActionItemWrapper.SetMainXaml(xamlFile);

            QueueDelegate(OnXamlLoaded);
        }

        private void OnXamlLoaded()
        {
            UIElement richtextbox;

            richtextbox = ElementUtils.FindElement(base.MainWindow, "RichTextBox1") as UIElement;
            if (richtextbox == null)
            {
                throw new InvalidOperationException("Cannot find UIElement [RichTextBox]");
            }

            _wrapper = new UIElementWrapper(richtextbox);
            MouseInput.MouseClick(_wrapper.Element);

            QueueDelegate(TypeInput);
        }

        private void TypeInput()
        {
            KeyboardInput.TypeString(ConfigurationSettings.Current.GetArgument("InitialStringContent"));

            QueueDelegate(OnSetSpringLoadedProperty);
        }

        private void OnSetSpringLoadedProperty()
        {
            _wrapper.SelectionInstance.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);
            KeyboardInput.TypeString(ConfigurationSettings.Current.GetArgument("TestString"));

            QueueDelegate(OnVerification);
        }

        private void OnVerification()
        {
            string str, expectedString, message;
            Brush b;

            str = _wrapper.SelectionInstance.End.GetTextInRun(LogicalDirection.Backward);            
            b = _wrapper.SelectionInstance.End.Parent.GetValue(TextElement.ForegroundProperty) as Brush;            

            expectedString = ConfigurationSettings.Current.GetArgument("ExpectedString");
            message = String.Format("Text in red [{0}], expected string [{1}]",
                str, expectedString);

            Verifier.Verify(str == expectedString, message);
            Verifier.Verify(b == Brushes.Red, "Brush is not red");

            Logger.Current.ReportSuccess();
        }

        private UIElementWrapper _wrapper = null;
    }

    /// <summary>
    /// Verifies that text can be selected with the mouse, and that word
    /// selection heuristics work as expected.
    /// </summary>
    /// <remarks>
    /// To run interactively, use the following command line.
    /// EditingTest.exe /TestCaseType=SelectionByMouseTest /NoExit=True
    ///
    /// Currently, this test is not running due to Regression_Bug546 - please stage on running.
    /// </remarks>
    [TestOwner("Microsoft"), TestTitle("SelectionByMouseTest"), TestTactics("512"),
     TestBugs("617,547,548,549,546")]
    public class SelectionByMouseTest: CustomTestCase
    {
        #region Private data.

        class TestData
        {
            /// <summary>Whether plain text is being tested.</summary>
            internal bool IsPlain;

            /// <summary>
            /// Actions to perform for this test case.
            /// </summary>
            /// <remarks>
            /// This is a semi-colon delimited list of strings. Each
            /// string starts with 'mouse ' for an mouse action relative
            /// to the element, or 'type ' for a keyboard entry.
            /// </remarks>
            internal string Actions;

            /// <summary>Resulting selection. May be null to ignore check.</summary>
            internal string Selection;

            /// <summary>Text to initialize control with.</summary>
            internal string Text;

            /// <summary>Initializes a new TestData instance with plain text data.</summary>
            internal static TestData Plain(string text,
                string actions, string selection)
            {
                TestData result;

                result = new TestData();
                result.IsPlain = true;
                result.Text = text;
                result.Actions = actions;
                result.Selection = selection;

                return result;
            }
        }

        /// <summary>Cases to run.</summary>
        /// <remarks>Space inserted every 10 cases (0-9, 10-19, etc).</remarks>
        private TestData[] _cases = new TestData[] {
            // Verifies that the first character can be selected.
            TestData.Plain("This is text.", "mouse move 9 20;mouse pressdrag left 25 20", "T"),
            // Verifies that almost-a-word can be selected.
            TestData.Plain("This is text.", "mouse move 3 20;mouse pressdrag left 50 20", "Thi"),
            // Verifies that whitespace can be selected.
            TestData.Plain("This is text.", "mouse move 65 20;mouse pressdrag left 75 20", " "),
            // Verifies a word can be doble-clicked to be selected. (Regression_Bug549)
            TestData.Plain("This is some text.", "mouse move 140 20;mouse click left;mouse click left", "some "),
            // Verifies that word selection does not anchor to middle (Regression_Bug548).
            TestData.Plain("This is some text.", "mouse click left 10 10;mouse move 140 20;mouse click left;mouse click left;type +{RIGHT}", "some t"),
            // Verifies that consecutive newlines do not disrupt Shift+End (Regression_Bug550).
            TestData.Plain("Line one.\r\n\r\nLine three.", "mouse click left 10 10;type {HOME}+{END}", "Line one.\r\n"),
            // Verifies that clicking past the end of a line does not move to following line (Regression_Bug551).
            TestData.Plain("a\r\nb", "mouse click left 50 10;type +{LEFT}", "a"),
            // Verifies that shift+end anchors selection as expected (Regression_Bug552).
            TestData.Plain("line one\r\nline two\r\nline three", "mouse click left 3 50;type {UP}+{END}+{RIGHT}", "line one\r\nl"),
            // Verifies that shift+home anchors selection as expected.
            TestData.Plain("one\r\ntwo\r\nthree", "mouse click left 50 50;type +{HOME}", "two"),
            // Verifies that word navigation to the right works as expected.
            TestData.Plain("one two three", "mouse click left 3 20;type ^{RIGHT}+{RIGHT}", "t"),

            // Verifies that word navigation to the right works as expected on end
            TestData.Plain("one two three", "mouse click left 3 20;type {END}^{RIGHT}+{LEFT 2}", "ee"),
            // Verifies that word navigation to the left works as expected.
            TestData.Plain("This is text", "mouse click left 50 20;type ^{LEFT}+{RIGHT}", "T"),
            // Verifies that word navigation to the left works as expected on start.
            TestData.Plain("one two three", "mouse click left 3 20;type ^{LEFT}+{RIGHT}", "o"),
            // Verifies that word expansion to the left starting on space works as expected.
            TestData.Plain("one two three", "mouse move 128 20;mouse pressdrag left 117 20", "two "),
            // Verifies that word expansion to the left starting on multiple spaces works as expected (Regression_Bug547).
            TestData.Plain("one two  three", "mouse move 136 20;mouse pressdrag left 126 20", "two  "),
            TestData.Plain("one two  three", "mouse move 7 20;mouse pressdrag left 76 20", "one two  "),
            // Verifies that up/down from selection collapses correctly (Regression_Bug553).
            //TestData.Plain("abc\r\n123", "type ^{HOME}+{RIGHT}{DOWN}+{RIGHT}", "2"),
            //TestData.Plain("abc\r\n123", "type ^{HOME}{RIGHT}+{LEFT}{DOWN}+{RIGHT}", "2"),
            //TestData.Plain("abc\r\n123", "type ^{HOME}{DOWN}+{RIGHT}{UP}+{RIGHT}", "a"),
            //TestData.Plain("abc\r\n123", "type ^{HOME}{DOWN}{RIGHT}+{LEFT}{UP}+{RIGHT}", "a"),

            // Verifies that left/right from selection collapses correctly.
            TestData.Plain("abc", "type ^{HOME}+{RIGHT 2}{RIGHT}+{RIGHT}", "c"),
            TestData.Plain("abc", "type ^{HOME}+{RIGHT 2}{LEFT}+{RIGHT}", "a"),
            TestData.Plain("abc", "type ^{END}+{LEFT 2}{LEFT}+{LEFT}", "a"),
            TestData.Plain("abc", "type ^{END}+{LEFT 2}{RIGHT}+{LEFT}", "c"),
        };

        private FrameworkElement _testedElement;
        private UIElementWrapper _testedWrapper;
        private string[] _actions;
        private int _actionIndex;
        private int _caseIndex;

        #endregion Private data.

        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            _caseIndex = 0;
            BeginCase();
        }

        /// <summary>Begins a new TestData test.</summary>
        private void BeginCase()
        {
            Log("Starting case for data set #" + _caseIndex);
            if (_cases[_caseIndex].IsPlain)
            {
                _testedElement = new TextBox();
                _testedWrapper = new UIElementWrapper(_testedElement);
                _testedWrapper.Text = _cases[_caseIndex].Text;
                _testedElement.SetValue(TextElement.FontFamilyProperty, new FontFamily("Arial"));
                _testedElement.SetValue(TextBox.AcceptsReturnProperty, true);
                _testedElement.SetValue(TextElement.FontSizeProperty, 32.0);
                _testedElement.PreviewMouseMove += TextBoxMouseMove;
            }
            MainWindow.Content = _testedElement;

            _actionIndex = 0;
            _actions = _cases[_caseIndex].Actions.Split(';');
            Log("Actions to execute: [" + _cases[_caseIndex].Actions + "]");

            QueueDelegate(RunAction);
        }

        /// <summary>Runs a single action, if available.</summary>
        private void RunAction()
        {
            string action;  // Action description to execute.

            if (_actionIndex >= _actions.Length)
            {
                FinishedActions();
                return;
            }

            action = _actions[_actionIndex];
            Log("Running action " + _actionIndex + ": " + action);
            if (action.StartsWith("mouse "))
            {
                ActionItemWrapper.MouseElementRelative(_testedElement,
                    action.Substring("mouse ".Length));
            }
            else if (action.StartsWith("type "))
            {
                KeyboardInput.TypeString(action.Substring("type ".Length));
            }
            else
            {
                throw new InvalidOperationException("Cannot identify action type.");
            }

            _actionIndex++;
            QueueDelegate(RunAction);
        }

        /// <summary>Verifies results and logs or launches next TestData.</summary>
        private void FinishedActions()
        {
            Log("Actions finished - verifying results...");
            if (_cases[_caseIndex].Selection != null)
            {
                Log("Expected selection: [" + _cases[_caseIndex].Selection + "]");
                Log("Actual selection:   [" + _testedWrapper.GetSelectedText(false, false) + "]");
                Verifier.Verify(_cases[_caseIndex].Selection == _testedWrapper.GetSelectedText(false, false));
            }

            _caseIndex++;
            if (_caseIndex == _cases.Length)
            {
                Logger.Current.ReportSuccess();
            }
            else
            {
                QueueDelegate(BeginCase);
            }
        }

        /// <summary>
        /// Tracks mouse position - useful for interactive usage and finding
        /// out appropriate coordinates.
        /// </summary>
        private void TextBoxMouseMove(object sender, MouseEventArgs e)
        {
            Point point;    // Point of mouse relative to element being tested.

            point = e.GetPosition(_testedElement);

            MainWindow.Title = "Mouse Coordinates: " + point.X + " - " + point.Y;
        }

        #endregion Main flow.
    }

    /// <summary>
    /// Test for Selection rendering - Verify that selection rendering changes when 
    /// Select() is called when in focus and doesnt changes when not in focus.    
    /// </summary>
    [Test(2, "Selection", "TextSelectionSelectTest", MethodParameters = "/TestCaseType=TextSelectionSelectTest", Keywords = "Setup_SanitySuite")]
    [TestOwner("Microsoft"), TestTactics("669"), TestBugs(""), TestWorkItem("135")]
    public class TextSelectionSelectTest : ManagedCombinatorialTestCase
    {
        #region Main flow

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            StackPanel panel;

            _rtb = new RichTextBox();
            _rtb.Height = _rtb.Width = 200d;

            _rtb.Document.Blocks.Clear();
            _rtb.Document.Blocks.Add(new Paragraph(new Run(_textSample)));

            _otherControl = new TextBox();
            _otherControl.Height = 100d;
            _otherControl.Width = 200d;
            _otherControl.Text = "Other Control";

            panel = new StackPanel();
            panel.Children.Add(_rtb);
            panel.Children.Add(_otherControl);
            TestElement = panel;

            QueueDelegate(DoFocus);
        }

        private void DoFocus()
        {
            if (_withFocus)
            {
                _rtb.Focus();
            }
            else
            {
                _otherControl.Focus();
            }

            _bmpPrevState = BitmapCapture.CreateBitmapFromElement(_rtb);
            _prevSelectionText = _rtb.Selection.Text;

            _tp1 = _rtb.Document.ContentStart.GetPositionAtOffset(0);
            _tp1 = _tp1.GetInsertionPosition(LogicalDirection.Forward);

            _tp2 = _tp1.GetNextInsertionPosition(LogicalDirection.Forward);

            if ( (_tp2!=null) && (_tp1.GetOffsetToPosition(_tp2)>0) )
            {
                Log("-------Test TextSelection loop start-------");
                Log("Calling Select() with textpointers offset at (" +
                    _rtb.Document.ContentStart.GetOffsetToPosition(_tp1) + "," +
                    _rtb.Document.ContentStart.GetOffsetToPosition(_tp2) + ")");
                _rtb.Selection.Select(_tp1, _tp2);

                QueueDelegate(TextSelectionTestLoop);
            }
        }

        private void TextSelectionTestLoop()
        {            
            TextPointer tempPointer;

            _bmpCurrState = BitmapCapture.CreateBitmapFromElement(_rtb);
            _nextSelectionText = _rtb.Selection.Text;

            Verifier.Verify(_prevSelectionText != _nextSelectionText,
                "Verifying that TextSelection.Text has changed after Select is called", false);
            
            if (_withFocus)
            {
                //Verify that selection rendering changes using Select()
                VerifyBitmapsChanged(_bmpPrevState, _bmpCurrState,
                    "Verify that selection rendering changes using Select()", "bmpSelection");
            }
            else
            {
                //Verify that selection rendering do not change when not in focus
                VerifyBitmapsEqual(_bmpPrevState, _bmpCurrState,
                    "Verify that selection rendering do not change when not in focus", "bmpSelection");
            }
                        
            tempPointer = _tp2.GetNextInsertionPosition(LogicalDirection.Forward);
            if ((tempPointer != null) && (_tp2.GetOffsetToPosition(tempPointer) > 0))
            {
                _tp2 = tempPointer;
                _bmpPrevState = _bmpCurrState;
                _prevSelectionText = _nextSelectionText;

                Log("Calling Select() with textpointers offset at (" +
                    _rtb.Document.ContentStart.GetOffsetToPosition(_tp1) + "," +
                    _rtb.Document.ContentStart.GetOffsetToPosition(_tp2) + ")");
                _rtb.Selection.Select(_tp1, _tp2);

                QueueDelegate(TextSelectionTestLoop);
            }
            else
            {
                Log("-------Test TextSelection loop end-------");
                
                //update the prev state
                _bmpPrevState = _bmpCurrState;
                _prevSelectionText = _nextSelectionText;

                //Expand the Selection contents using Text property
                _rtb.Selection.Text = _textSample + _textSample;

                QueueDelegate(CaptureAfterSelectionExpanded);
            }
        }                                

        private void CaptureAfterSelectionExpanded()
        {
            TextPointer tp;

            _bmpCurrState = BitmapCapture.CreateBitmapFromElement(_rtb);
            _nextSelectionText = _rtb.Selection.Text;

            Verifier.Verify(_prevSelectionText != _nextSelectionText,
                "Verifying that TextSelection.Text has changed after Selection is expanded", false);

            //Verifying that selection rendering changes when expanded using Text property
            //NOTE: Bitmaps change even when not in focus because contents are changing.
            VerifyBitmapsChanged(_bmpPrevState, _bmpCurrState,
                "Verifying selection rendering changes when it is expanded using Text property", 
                "bmpSelectionExpanded");            

            //update the prev state
            _bmpPrevState = _bmpCurrState;
            _prevSelectionText = _nextSelectionText;

            //Insert text inside the selection using TextPointer
            tp = _rtb.Selection.Start;
            tp = tp.GetPositionAtOffset(0, LogicalDirection.Backward);
            tp = tp.GetInsertionPosition(LogicalDirection.Forward);            
            tp.InsertTextInRun(_textSample);

            QueueDelegate(CaptureAfterInsertText);
        }

        private void CaptureAfterInsertText()
        {
            _bmpCurrState = BitmapCapture.CreateBitmapFromElement(_rtb);
            _nextSelectionText = _rtb.Selection.Text;

            Verifier.Verify(_prevSelectionText != _nextSelectionText,
                "Verifying that TextSelection.Text has changed after insert text", false);
            
            //Verifying that selection rendering changes when Text isinserted inside 
            //selection using textpointer.
            //NOTE: Bitmaps change even when not in focus because contents are changing.
            VerifyBitmapsChanged(_bmpPrevState, _bmpCurrState,
                "Verifying selection rendering changes after text insertion through TextPointer",
                "bmpSelectionInsertText");         

            //update the prev state
            _bmpPrevState = _bmpCurrState;
            _prevSelectionText = _nextSelectionText;

            //Collapse the selection
            _rtb.Selection.Select(_rtb.CaretPosition, _rtb.CaretPosition);

            QueueDelegate(CaptureAfterCollapse);
        }

        private void CaptureAfterCollapse()
        {
            _bmpCurrState = BitmapCapture.CreateBitmapFromElement(_rtb);
            _nextSelectionText = _rtb.Selection.Text;

            Verifier.Verify(_prevSelectionText != _nextSelectionText,
                "Verifying that TextSelection.Text has changed after collapse", false);

            if (_withFocus)
            {
                //Verifying that selection rendering changes when selection is collapsed
                VerifyBitmapsChanged(_bmpPrevState, _bmpCurrState,
                    "Verifying selection rendering changes after collapsing the selection",
                    "bmpSelectionCollapse");
            }
            else
            {
                //Verifying that selection rendering does changes when collapsed with no focus
                VerifyBitmapsEqual(_bmpPrevState, _bmpCurrState,
                    "Verifying selection rendering doesnt change when collapsed with no focus",
                    "bmpSelectionCollapse");
            }

            //update the prev state
            _bmpPrevState = _bmpCurrState;
            _prevSelectionText = _nextSelectionText;

            //Select all the contents
            _rtb.SelectAll();

            QueueDelegate(CaptureAfterSelectAll);
        }

        private void CaptureAfterSelectAll()
        {
            _bmpCurrState = BitmapCapture.CreateBitmapFromElement(_rtb);
            _nextSelectionText = _rtb.Selection.Text;

            Verifier.Verify(_prevSelectionText != _nextSelectionText,
                "Verifying that TextSelection.Text has changed after SelectAll", false);

            if (_withFocus)
            {
                //Verifying that selection rendering changes after SelectAll
                VerifyBitmapsChanged(_bmpPrevState, _bmpCurrState,
                    "Verifying selection rendering after SelectAll",
                    "bmpSelectionCollapse");
            }
            else
            {
                //Verifying that selection rendering does changes when SelectAll with no focus
                VerifyBitmapsEqual(_bmpPrevState, _bmpCurrState,
                    "Verifying selection rendering doesnt change when SelectAll with no focus",
                    "bmpSelectionCollapse");
            }

            //update the prev state
            _bmpPrevState = _bmpCurrState;
            _prevSelectionText = _nextSelectionText;

            //Clear the contents of the Selection
            _rtb.Selection.Text = string.Empty;

            QueueDelegate(CaptureAfterSelectionCleared);
        }

        private void CaptureAfterSelectionCleared()
        {
            _bmpCurrState = BitmapCapture.CreateBitmapFromElement(_rtb);
            _nextSelectionText = _rtb.Selection.Text;            

            Verifier.Verify(_prevSelectionText != _nextSelectionText,
                "Verifying that TextSelection.Text has changed after Selection is cleared", false);

            //Verifying that selection rendering changes when selection contents are cleared
            //NOTE: Bitmaps change even when not in focus because contents are changing.
            VerifyBitmapsChanged(_bmpPrevState, _bmpCurrState,
                "Verifying selection rendering when it is cleared", "bmpSelectionClear");            

            QueueDelegate(NextCombination);
        }

        private void VerifyBitmapsChanged(System.Drawing.Bitmap prev, System.Drawing.Bitmap curr,
            string logMessage, string logFileName)
        {
            System.Drawing.Bitmap differences;
            
            if (ComparisonOperationUtils.AreBitmapsEqual(prev, curr, out differences))
            {
                Log(logMessage);
                Logger.Current.LogImage(prev, logFileName + "_previous");
                Logger.Current.LogImage(curr, logFileName + "_current");
                throw new Exception("Bitmaps have not changed when expected to change");
            }
        }

        private void VerifyBitmapsEqual(System.Drawing.Bitmap prev, System.Drawing.Bitmap curr,
            string logMessage, string logFileName)
        {
            System.Drawing.Bitmap differences;
            ComparisonCriteria criteria = new ComparisonCriteria();
            criteria.MaxErrorProportion = 0.005f;
            
            if ( !ComparisonOperationUtils.AreBitmapsEqualUsingCriteria(prev, curr, out differences, criteria, false) )
            {
                Log(logMessage);
                Logger.Current.LogImage(prev, logFileName + "_previous");
                Logger.Current.LogImage(curr, logFileName + "_current");
                throw new Exception("Bitmaps have changed when not expected");
            }
        }

        #endregion Main flow

        #region Private data

        /// <summary>Text to be used in the test</summary>
        private string _textSample=string.Empty;

        /// <summary>Whether edit control has focus</summary>
        private bool _withFocus=false;

        private RichTextBox _rtb;
        private TextBox _otherControl;
        private System.Drawing.Bitmap _bmpPrevState,_bmpCurrState;
        private string _prevSelectionText,_nextSelectionText;
        private TextPointer _tp1,_tp2;

        #endregion Private data
    }

    /// <summary>
    /// Test for Selection rendering:
    ///     1. Highlight color is as expected.
    ///     2. Text Color is as expected.
    /// Test Sequence when Highlight DCR is checked in:
    ///     1. Verify the default System colors.
    ///     2. Set new system colors.
    ///     3. Verify the Highlight. 
    /// Note:
    ///     1. RichTextBox will be smart to calculate the highlight color.
    ///     
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("670"), TestBugs(""), TestWorkItem("")]
    public class TextSelectionRenderingTest : ManagedCombinatorialTestCase
    {
        #region Main flow

        /// <summary>
        /// Filter out some unwanted cases
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        protected override bool DoReadCombination(System.Collections.Hashtable values)
        {
            bool result = base.DoReadCombination(values);
            if (_editableType.IsPassword)
            {
                result = false;
            }
            return result; 
              
        }
        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            _wrapper = new UIElementWrapper(_editableType.CreateInstance());
            TestElement = (FrameworkElement)_wrapper.Element;

            _systemHighlightColor = SystemColors.HighlightColor;
            _systemTextColor = SystemColors.HighlightTextColor;
            
            ((Control)_wrapper.Element).Background = Brushes.White ;
            
            //trun acceptsReturn for mutiple lines.
            ((TextBoxBase)_wrapper.Element).AcceptsReturn = true;

            try
            {
                SetSystemHighlightColors(_hightlightColor, _textColor);

                QueueDelegate(SetFocus);
            }
            catch (Exception e)
            {
                SetSystemHighlightColors(_systemHighlightColor, _systemTextColor);
                throw e; 
            }
        }
        //Set the focus to 
        void SetFocus()
        {
            try
            {
                _wrapper.Element.Focus();
                QueueDelegate(MakeSelection);
            }
            catch(Exception e)
            {
                SetSystemHighlightColors(_systemHighlightColor, _systemTextColor);
                throw e; 
            }
        }

        void MakeSelection()
        {
            try
            {
                Test.Uis.Utils.KeyboardInput.TypeString("a{Enter 2}b" + "^{HOME}{Right " + _selectionStartIndex + "}+{RIGHT " + _selectionLength + "}");
            }
            catch (Exception e)
            {
                SetSystemHighlightColors(_systemHighlightColor, _systemTextColor);
                throw e; 
            }

            QueueDelegate(VerifySelectionRendering); 
        }

        void VerifySelectionRendering()
        {
            try
            {
                if (_hightlightColor.R == 255)
                {
                    Test.Uis.Data.TextSelectionData.VerifySelectionRendering(_wrapper, ColorElement.Red, ColorElement.All, 40, 0);
                }
                else if (_hightlightColor.G == 255)
                {
                    Test.Uis.Data.TextSelectionData.VerifySelectionRendering(_wrapper, ColorElement.Green, ColorElement.All, 40, 0);
                }
                else
                {
                    Test.Uis.Data.TextSelectionData.VerifySelectionRendering(_wrapper, ColorElement.Blue, ColorElement.All, 40, 0);
                }

                _selectionLength = _wrapper.SelectionLength;
                _selectedText = _wrapper.SelectionInstance.Text;

                ((Control)_wrapper.Element).FontSize = 100;
            }
            catch(Exception e)
            {
                SetSystemHighlightColors(_systemHighlightColor, _systemTextColor);
                throw e; 
            }
            QueueDelegate(VerifyChangeFontSize);
        }


        /// <summary>
        /// font size change should cause the LightSize change.
        /// </summary>
        void VerifyChangeFontSize()
        {
            try
            {
                if (_hightlightColor.R == 255)
                {
                    Test.Uis.Data.TextSelectionData.VerifySelectionRendering(_wrapper, ColorElement.Red, ColorElement.All, 50, 0);
                }
                else if (_hightlightColor.G == 255)
                {
                    Test.Uis.Data.TextSelectionData.VerifySelectionRendering(_wrapper, ColorElement.Green, ColorElement.All, 50, 0);
                }
                else
                {
                    Test.Uis.Data.TextSelectionData.VerifySelectionRendering(_wrapper, ColorElement.Blue, ColorElement.All, 50, 0);
                }

                Verifier.Verify(_wrapper.SelectionLength == _selectionLength, "Selection length is changed after apply font! Expected["
                    + _selectionLength + "]. Actual[" + _wrapper.SelectionLength + "]");
                Verifier.Verify(_wrapper.SelectionInstance.Text == _selectedText, "Selected text is changed after apply font! Expected["
                    + _selectedText + "]. Actual[" + _wrapper.SelectionInstance.Text + "]");
            }
            catch(Exception e)
            {
                SetSystemHighlightColors(_systemHighlightColor, _systemTextColor);
                throw e;
            }

            //need to reset the System color after each combination since we don't know if this is the last combination. 
            SetSystemHighlightColors(_systemHighlightColor, _systemTextColor);
            QueueDelegate(NextCombination);
        }

        void SetSystemHighlightColors(Color HighlightColor, Color HighlightTextColor)
        {
            Logger.Current.Log("Color to be set on the system: HighlightColor[" + 
                HighlightColor.ToString() + "HighlightTextColor[" + HighlightTextColor.ToString() + "]"); 
            //reset the system color. 13 is for Highlight, 14 is for HilightedText
            _us[0] = Microsoft.Test.Imaging.BitmapUtils.ColorTouint(HighlightColor);
            _elements[0] = 13;
            _us[1] = Microsoft.Test.Imaging.BitmapUtils.ColorTouint(HighlightTextColor);
            _elements[1] = 14;
            Test.Uis.Wrappers.Win32.SafeSetSysColors(2, _elements, _us);
        }

        #endregion Main flow

        #region Private data

        /// <summary>Text to be used in the test</summary>
        private int _selectionStartIndex=0;
        private int _selectionLength=0;
        private TextEditableType _editableType=null;
        private Color _hightlightColor;
        private Color _textColor=Brushes.Black.Color;

        private UIElementWrapper _wrapper; 
        private Color _systemHighlightColor;
        private Color _systemTextColor;
        private uint[] _us = new uint[2];
        private int[] _elements = new int[2];
        private string _selectedText; 

        #endregion Private data
    }
}
