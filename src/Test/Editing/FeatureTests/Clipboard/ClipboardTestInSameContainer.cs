// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 14 $ $Source: //depot/private/wcp_dev_platform/windowstest/client/wcptests/uis/forms/BVT/DataTransfer/Clipboard/ClipboardTestInSameContainer.cs $")]
namespace DataTransfer
{
    #region Namespance

    using System;
    using System.Threading;
    using System.Windows.Threading;

    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Markup;

    using Test.Uis.Data;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;
    using Test.Uis.TestTypes;
    using Microsoft.Test.Imaging;
    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    #endregion Namespance

    #region ClipboardTestInSameContainer

    /// <summary>
    /// Verifies that cut/paste and copy/paste works in same contianer.
    /// 50 P0: TB:  Test.exe /TestCaseType:ClipboardTestInSameContainer /ContainerType1:Canvas /Priority:0 /EditableBox1:TextBox
    /// 51 P1: TB:  Test.exe /TestCaseType:ClipboardTestInSameContainer /ContainerType1:Canvas /Priority:1 /EditableBox1:TextBox
    /// 53 P0: RTB: Test.exe /TestCaseType:ClipboardTestInSameContainer /ContainerType1:Canvas /Priority:0
    /// 52 P1: RTB: Test.exe /TestCaseType:ClipboardTestInSameContainer /ContainerType1:Canvas /Priority:1
    /// </summary>
    [Test(2, "Clipboard", "ClipboardTestInSameContainer1", MethodParameters = "/TestCaseType:ClipboardTestInSameContainer /ContainerType1:Canvas /Priority:0 /EditableBox1:TextBox", Timeout=120)]
    [Test(2, "Clipboard", "ClipboardTestInSameContainer2", MethodParameters = "/TestCaseType:ClipboardTestInSameContainer /ContainerType1:Canvas /Priority:0", Timeout = 120)]
    [Test(2, "Clipboard", "ClipboardTestInSameContainer3", MethodParameters = "/TestCaseType:ClipboardTestInSameContainer /ContainerType1:Canvas /Priority:1 /EditableBox1:TextBox", Timeout = 120)]
    [Test(2, "Clipboard", "ClipboardTestInSameContainer4", MethodParameters = "/TestCaseType:ClipboardTestInSameContainer /ContainerType1:Canvas /Priority:1", Timeout = 120)]
    [TestOwner("Microsoft"), TestTactics("50,53,51,52"), TestWorkItem("13,14"), TestBugs("6,7,278,17,279")]
    public class ClipboardTestInSameContainer : CustomTestCase
    {
        private UIElementWrapper _editBox1;              // Editable box1
        private string _editableBox1;                    // Editable box1
        private TestCaseData _testCaseData;
        private string _originalText;                    // Original text in editbox1
        private string _originalSelectedXmlText;         // Original selected text in Xml format before any action
        private string _originalTextBeforePaste;         // Original text in editbox1 before paste bug after undo/redo
        private string _originalSelectedText;            // Original text selection in editbox1
        private string _actualClipboardXmlData;          // Actual clipboard data in Xml format from copy or cut in editbox1
        private string _actualClipboardTextData;         // Actual clipboard data in Text format from coyp or cut in editbox1

        private int _selectionLength;                    // The length of selected text. Ex: 'abc' is 3
        private int _testCaseIndex;                      // Test case index default is 0
        private int[] _testFailed;                       // Array of all test that failed
        private bool _isTestFailed;                      // To remember if test failed
        string _failID;                                  // ID of test case that failed
        private int _endIndex;                           // End of test case index

        private void BuildWindow()
        {
            Panel topPanel;
            FrameworkElement box1;

            // The containers can be TextBox and RichTextBox
            _editableBox1 = Settings.GetArgument("EditableBox1");
            if (_editableBox1 == "")
            {
                // Default to RichTextBox
                _editableBox1 = "RichTextBox";
            }

            // topPanel can be any Canvas, FlowPanel, DockPanel
            topPanel = (Panel)ReflectionUtils.CreateInstanceOfType(Settings.GetArgument("ContainerType1"), new object[] {});
            topPanel.Background = Brushes.Lavender;

            // editBox1, editBox2 can be TextBox or RichTextBox
            box1 = TextEditableType.GetByName(_editableBox1).CreateInstance();
            box1.Height = 100;
            box1.Width = 250;
            box1.SetValue(TextBox.TextWrappingProperty, TextWrapping.Wrap);
            box1.SetValue(TextBox.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Auto);
            box1.SetValue(TextBox.AcceptsReturnProperty, true);
            box1.SetValue(TextBox.FontFamilyProperty, new FontFamily("Microsoft Sans Serif"));
            box1.SetValue(TextBox.FontSizeProperty, 11.0);
            box1.Name = "tb1";
            topPanel.Children.Add(box1);

            MainWindow.Content = topPanel;
        }

        private enum TestCaseAction
        {
            CutPaste,
            CopyPaste,
            CutPasteAPI,
            CopyPasteAPI,
        }

        struct TestCaseData
        {
            public TestCaseAction TestAction;
            public string TestString;
            public int StartSelection;
            public int EndSelection;
            public int PasteLocationX;
            public int PasteLocationY;
            public int Priority;

            public TestCaseData(TestCaseAction testAction, string testString, int startSelection,
                                int endSelection, int pasteLocationX, int pasteLocationY, int priority)
            {
                this.TestAction = testAction;
                this.TestString = testString;
                this.StartSelection = startSelection;
                this.EndSelection = endSelection;
                this.PasteLocationX = pasteLocationX;
                this.PasteLocationY = pasteLocationY;
                this.Priority = priority;
            }

            public static TestCaseData[] Cases = new TestCaseData[] {
                // 0: Copy single space then paste [Regression_Bug6]
                new TestCaseData(TestCaseAction.CopyPaste, "abc def ghi", 3, 1, 40, 40, 0),
                // 1: cut miltiple spaces then paste [Regression_Bug6]
                new TestCaseData(TestCaseAction.CutPaste, "abc    def ghi", 3, 4, 50, 40, 0),
                // 2: Copy a word then paste to begining of a second line
                new TestCaseData(TestCaseAction.CopyPaste, "abc def ghi\r\n123 456 789\r\npqr stu xyz", 4, 4, 10, 40, 0),
                // 3: Copy a word then paste to begining of a second line
                new TestCaseData(TestCaseAction.CopyPasteAPI, "abc def ghi\r\n123 456 789\r\npqr stu xyz", 4, 4, 10, 40, 0),
                // 4: Cut 2 words then paste to end of a second line
                new TestCaseData(TestCaseAction.CutPaste, "abc def ghi\r\n123 456 789\r\npqr stu xyz", 4, 7, 100, 40, 0),
                // 5: Cut 2 words then paste to end of a second line
                new TestCaseData(TestCaseAction.CutPasteAPI, "abc def ghi\r\n123 456 789\r\npqr stu xyz", 4, 7, 100, 40, 0),
                // 6: Cut a line of text then paste to middle of a second line
                new TestCaseData(TestCaseAction.CutPaste, "abc def ghi\r\n123 456 789\r\npqr stu xyz\r\n", 24, 12, 30, 40, 0),
                // 7: Copy a word then paste to the begining of document/paragraph
                new TestCaseData(TestCaseAction.CopyPaste, "<Paragraph>abc #$% ghi</Paragraph><Paragraph>"+
                    "</Paragraph><Paragraph>pqr ~@: xyz</Paragraph>", 19, 4, 10, 40, 0),
                // 8: Copy a plain word then paste in front of U/I/B
                new TestCaseData(TestCaseAction.CopyPaste, "abc <Underline><Italic><Bold>Mix</Bold></Italic></Underline>",
                    0, 4, 30, 40, 0),
                // 9: Copy a plain word then paste after of U/I/B
                new TestCaseData(TestCaseAction.CopyPaste, "abc <Underline><Italic><Bold>Mix</Bold></Italic></Underline>",
                    0, 4, 60, 40, 0),
                // 10: Copy mix bold and plain text then paste [Regression_Bug7]
                new TestCaseData(TestCaseAction.CopyPaste, "<Bold>xxx</Bold>o<Bold>yyy</Bold>", 1, 9, 40, 40, 2),
                // 11: Copy a plain word then paste in middle of U/I/B [Regression_Bug278]
                new TestCaseData(TestCaseAction.CopyPaste, "abc <Underline><Italic><Bold>Mix</Bold></Italic></Underline>", 0, 4, 40, 40, 1),
                // 12: Copy a U/I/B word then paste in middle of U [Regression_Bug278]
                new TestCaseData(TestCaseAction.CopyPaste, "<Run FontStyle='Italic' FontWeight='Bold' TextDecorations='Underline'>abc</Run>def",
                    0, 7, 60, 40, 1),
                // 13: Copy formated text and then pate to begining of line
                new TestCaseData(TestCaseAction.CopyPaste,
                    "axxx <Bold Background=\"red\" Foreground=\"green\" FontSize=\"24\""+
                    " FontFamily=\"Comic Sans MS\" FontStyle=\"Oblique\">"+
                    "rich</Bold> yyyc", 1, 14, 10, 40, 0),
                // 14: Copy formated text and paste at the end of line [Regression_Bug17]
                new TestCaseData(TestCaseAction.CopyPaste, "<Italic><Bold>Mix</Bold></Italic>", 0, 7, 40, 40, 0),
                // 15: Cut/paste Underline text to then end of line [Regression_Bug279]
                new TestCaseData(TestCaseAction.CutPaste, "<Run TextDecorations='Underline'>abc</Run> def",
                    0, 4, 60, 40, 1),
                // 16: Cut/paste miltiple lines of paragraph [Regression_Bug280]
                new TestCaseData(TestCaseAction.CutPaste, "<Paragraph>a</Paragraph><Paragraph>b</Paragraph>",
                    0, 10, 10, 40, 1),
            };
        }

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            _endIndex = TestCaseData.Cases.Length;

            _testFailed = new int[TestCaseData.Cases.Length];
            _isTestFailed = false;
            for (int i = 0; i < _testFailed.Length; i++)
                _testFailed[i] = 0;

            _testCaseIndex = Settings.GetArgumentAsInt("CaseID");
            RunCase();
        }

        private void RunCase()
        {
            if (_testCaseIndex < _endIndex)
            {
                _testCaseData = TestCaseData.Cases[_testCaseIndex];
                if (Settings.GetArgumentAsInt("Priority") == _testCaseData.Priority)
                {
                    QueueDelegate(StartTest);
                }
                else
                {
                    _testCaseIndex++;
                    QueueHelper.Current.QueueDelegate(RunCase);
                }
            }
            else
            {
                if (_isTestFailed)
                {
                    // Log case id that failed.
                    for (int i = 0; i < _testFailed.Length; i++)
                    {
                        if (_testFailed[i] == -1)
                            _failID += " " + i + ",";
                    }
                    Log("The following test cases have failed: [" + _failID + "]" +
                        " To re-run append /CaseID:<test case number>.");
                    Logger.Current.ReportResult(false, "At lease one of test has failed.", false);
                }
                else
                {
                    Logger.Current.ReportSuccess();
                }
            }
        }

        private void StartTest()
        {
            BuildWindow();
            this.MainWindow.Width = 300;
            this.MainWindow.Height = 225;
            //Find element
            _editBox1 = new UIElementWrapper(ElementUtils.FindElement(MainWindow, "tb1"));

            Log("*******Running new test case:[" + _testCaseIndex + "]*******");
            Log("EditBox type [" + _editableBox1 + "]");
            Log("Test Action:[" + _testCaseData.TestAction + "]");

            QueueDelegate(DoMouseClick);
        }

        private void DoMouseClick()
        {
            MouseInput.MouseClick(_editBox1.Element);
            QueueDelegate(DoSelectText);
        }

        private void DoSelectText()
        {
            _testCaseData = TestCaseData.Cases[_testCaseIndex];

            // Disable undo for setting text programmatically
            ((TextBoxBase)_editBox1.Element).IsUndoEnabled = false;

            // Put text content into editBox1 then make selection
            if (_editBox1.Element is TextBox)
                _editBox1.Text = _testCaseData.TestString;
            else //RichTextBox
                _editBox1.XamlText = _testCaseData.TestString;
            _editBox1.Select(_testCaseData.StartSelection, _testCaseData.EndSelection);

            // SelectionLength is the length of editBox1.SelectionLength. Ex: 'abc' is 3
            // However, if '<Paragraph>abc</Paragraph>' is 5 becaue it counts \r\n but if manually do caret walker, it's only 4
            if (XamlUtils.TextRange_GetXml(_editBox1.SelectionInstance).Contains("</Paragraph><Paragraph>"))
            {
                _selectionLength = _editBox1.SelectionLength - TextUtils.CountOccurencies(XamlUtils.TextRange_GetXml(_editBox1.SelectionInstance), "</Paragraph><Paragraph>");
            }
            else if (XamlUtils.TextRange_GetXml(_editBox1.SelectionInstance).Contains("\r\n"))
            {
                _selectionLength = _editBox1.SelectionLength - TextUtils.CountOccurencies(XamlUtils.TextRange_GetXml(_editBox1.SelectionInstance), "\r\n");
            }
            else
            {
                _selectionLength = _editBox1.SelectionLength;
            }
            
            _originalText = _editBox1.Text;

            QueueDelegate(DoCutOrCopy);
        }

        private void DoCutOrCopy()
        {
            // Enable undo for user action
            ((TextBoxBase)_editBox1.Element).IsUndoEnabled = true;
            ((TextBoxBase)_editBox1.Element).Focus();
            // Get Xml text from existing selection
            _originalSelectedXmlText = XamlUtils.TextRange_GetXml(_editBox1.SelectionInstance);
            // Get plain text from existing selection 
            _originalSelectedText = _editBox1.GetSelectedText(false, false);

            if (_testCaseData.TestAction == TestCaseAction.CutPaste)
            {
                KeyboardInput.TypeString("^x");
            }
            else if (_testCaseData.TestAction == TestCaseAction.CopyPaste)
            {
                KeyboardInput.TypeString("^c");
            }
            else if (_testCaseData.TestAction == TestCaseAction.CutPasteAPI)
            {
                ((TextBoxBase)_editBox1.Element).Cut();                
            }
            else if (_testCaseData.TestAction == TestCaseAction.CopyPasteAPI)
            {
                ((TextBoxBase)_editBox1.Element).Copy();
            }
            QueueDelegate(VerifyAfterDoCutOrCopy);
        }

        private void VerifyAfterDoCutOrCopy()
        {
            // If cut, verify content is removed and there is no selection
            if ((_testCaseData.TestAction == TestCaseAction.CutPaste)||
                (_testCaseData.TestAction == TestCaseAction.CutPasteAPI) )
            {
                FailIfStringEqual(_originalText, _editBox1.Text, "Text should rearrange after cut.");
                FailIfStringNotEqual("", _editBox1.GetSelectedText(false, false), "There should be no selection after cut.");
            }
            // If copy, verify content and selection are remain
            else
            {
                FailIfStringNotEqual(_originalText, _editBox1.Text, "Text should remain after copy.");
                FailIfStringNotEqual(_originalSelectedText, _editBox1.GetSelectedText(false, false), "Selection should remain after copy.");
            }

            // Compare clipboard data
            if (!Clipboard.GetDataObject().GetDataPresent(DataFormats.Xaml))
            {
                FailIfStringNotEqual(Clipboard.GetDataObject().GetFormats().Length.ToString(), "3", "There should be 3 formats.");
            }
            else
            {
                FailIfStringNotEqual(Clipboard.GetDataObject().GetFormats().Length.ToString(), "6", "There should be 6 formats.");
                
                _actualClipboardXmlData = Clipboard.GetDataObject().GetData(DataFormats.Xaml).ToString();

                // StartFragment get inserted to Clipboard data, I'm removing it then compare with original string
                if (_actualClipboardXmlData.Contains("<!--StartFragment-->"))
                {
                    _actualClipboardXmlData = _actualClipboardXmlData.Replace("<!--StartFragment-->", "");
                    _actualClipboardXmlData = _actualClipboardXmlData.Replace("<!--EndFragment-->", "");
                }

                // Verify clipboard xml content matched with original selection xml content
                FailIfStringNotEqual(_originalSelectedXmlText, _actualClipboardXmlData, "Clipboard Xml data matched.");
            }
            // Verify clipboard text content matched with original selection text content
            _actualClipboardTextData = Clipboard.GetDataObject().GetData(DataFormats.Text).ToString();
            FailIfStringNotEqual(_originalSelectedText, _actualClipboardTextData, "Clipboard Text data matched.");

            // DoUndo after cut or copy
            KeyboardInput.TypeString("^z");

            QueueDelegate(VerifyUndo);
        }

        private void VerifyUndo()
        {
            // Verify undo: nothing should happen for copy.  For cut, content should come back
            FailIfStringNotEqual(_originalText, _editBox1.Text, "Text should remain after Undo.");
            FailIfStringNotEqual(_originalSelectedText, _editBox1.GetSelectedText(false, false), "Selection should remain after Undo.");

            // Do redo
            KeyboardInput.TypeString("^y");

            QueueDelegate(VerifyRedo);
        }

        private void VerifyRedo()
        {
            // If redo cut, verify content is removed and there is no selection
            if ((_testCaseData.TestAction == TestCaseAction.CutPaste)||
                (_testCaseData.TestAction == TestCaseAction.CutPasteAPI))
            {
                FailIfStringEqual(_originalText, _editBox1.Text, "Text should rearrange after redo for cut.");
                FailIfStringNotEqual("", _editBox1.GetSelectedText(false, false), "There should be no selection after redo for cut.");
            }
            // If redo copy, verify content and selection are remain
            else
            {
                FailIfStringNotEqual(_originalText, _editBox1.Text, "Text should remain after redo for copy.");
                FailIfStringNotEqual(_originalSelectedText, _editBox1.GetSelectedText(false, false), "Selection should remain after redo for copy.");
            }

            // Save original text before paste and use it after undo paste
            _originalTextBeforePaste = _editBox1.Text;

            // Find a point, mouse click on it then paste
            MouseInput.MouseClick(_testCaseData.PasteLocationX, _testCaseData.PasteLocationY);

            QueueDelegate(DoPaste);
        }

        private void DoPaste()
        {          
            if ((_testCaseData.TestAction == TestCaseAction.CopyPasteAPI) ||
                (_testCaseData.TestAction == TestCaseAction.CutPasteAPI))
            {
                ((TextBoxBase)_editBox1.Element).Paste();
            }
            else
            {
                KeyboardInput.TypeString("^v");
            }

            QueueDelegate(SelectPastedText);
        }

        private void SelectPastedText()
        {
            // After paste, back track to select the pasted character(s) then compare the selection xml with original xml
            for (int i = 0; i < _selectionLength; i++)
            {
                KeyboardInput.TypeString("+{Left}");
            }
            QueueDelegate(VerifyPaste);
        }

        private void VerifyPaste()
        {
            string actualSelectedXmlText = XamlUtils.TextRange_GetXml(_editBox1.SelectionInstance);
            //Remove the HasTrailingParagraphBreakOnPaste reference
            if( (actualSelectedXmlText.Contains(" HasTrailingParagraphBreakOnPaste=\"False\""))&&
                (!_originalSelectedXmlText.Contains(" HasTrailingParagraphBreakOnPaste=\"False\"")))
            {
                actualSelectedXmlText = actualSelectedXmlText.Replace(" HasTrailingParagraphBreakOnPaste=\"False\"", "");
            }

            // Verify pasted content has no selection and is at correct location
            FailIfStringNotEqual(_originalSelectedXmlText, actualSelectedXmlText, "Pasted text matched.");
            QueueDelegate(DoUndo);
        }

        private void DoUndo()
        {
            // Do Undo after paste
            KeyboardInput.TypeString("^z");
            QueueDelegate(VerifyUndoAfterPaste);
        }

        private void VerifyUndoAfterPaste()
        {
            FailIfStringNotEqual(_originalTextBeforePaste, _editBox1.Text, "Undo matched.");

            // Do Redo after undo paste
            KeyboardInput.TypeString("^y");

            QueueDelegate(SelectRedoText);
        }

        private void SelectRedoText()
        {
            // After redo, back track to select the redone character(s) then compare the selection xml with original xml
            for (int i = 0; i < _selectionLength; i++)
            {
                KeyboardInput.TypeString("+{Left}");
            }
            QueueDelegate(VerifyRedoAfterUndoPaste);
        }

        private void VerifyRedoAfterUndoPaste()
        {
            string actualSelectedXmlText = XamlUtils.TextRange_GetXml(_editBox1.SelectionInstance);
            //Remove the HasTrailingParagraphBreakOnPaste reference on actual, if original doesnt have.
            if ((actualSelectedXmlText.Contains(" HasTrailingParagraphBreakOnPaste=\"False\"")) &&
                (!_originalSelectedXmlText.Contains(" HasTrailingParagraphBreakOnPaste=\"False\"")))
            {
                actualSelectedXmlText = actualSelectedXmlText.Replace(" HasTrailingParagraphBreakOnPaste=\"False\"", "");
            }

            // This verification should be the same as after paste state.
            FailIfStringNotEqual(_originalSelectedXmlText, actualSelectedXmlText, "Redo matched.");                       

            // Run next test case
            _testCaseIndex++;
            RunCase();
        }

        private bool FailIfStringEqual(string ExpectedString, string ActualString, string Reason)
        {
            // If string are equal, fail the case
            if (ExpectedString == ActualString)
            {
                Logger.Current.ReportResult(false, Reason +
                "\nExpect [" + ExpectedString + "]" +
                "\nActual [" + ActualString + "]", true);
                _testFailed[_testCaseIndex] = -1;
                _isTestFailed = true;
            }
            return true;
        }

        private bool FailIfStringNotEqual(string ExpectedString, string ActualString, string Reason)
        {
            // If string are not equal, fail the case
            if (ExpectedString != ActualString)
            {
                Logger.Current.ReportResult(false, Reason +
                "\nExpect [" + ExpectedString + "]" +
                "\nActual [" + ActualString + "]", true);
                _testFailed[_testCaseIndex] = -1;
                _isTestFailed = true;
            }
            return true;
        }
    }

    #endregion ClipboardTestInSameContainer
}
