// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 14 $ $Source: //depot/private/wcp_dev_platform/windowstest/client/wcptests/uis/forms/BVT/DataTransfer/Clipboard/ClipboardTestInCrossContainer.cs $")]
namespace DataTransfer
{
    #region Namespance
    using System;
    using System.Threading;
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
    using Test.Uis.TestTypes;
    using Test.Uis.TextEditing;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;
    using Microsoft.Test.Imaging;
    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    #endregion Namespance

    # region ClipboardTestInCrossContainer
    /// <summary>
    /// Verifies that cut, copy undo then paste with undo/redo works in cross contianer and cross app.
    /// 46 P0: RTB->RTB: Test.exe /TestCaseType:ClipboardTestInCrossContainer /ContainerType1:Canvas /Priority:0
    /// 48 P1: RTB->RTB: Test.exe /TestCaseType:ClipboardTestInCrossContainer /ContainerType1:Canvas /Priority:1
    /// 47 P0: TB -> TB: Test.exe /TestCaseType:ClipboardTestInCrossContainer /ContainerType1:Canvas /Priority:0 /EditableBox1:TextBox /EditableBox2:TextBox
    /// 49 P0: TB ->RTB: Test.exe /TestCaseType:ClipboardTestInCrossContainer /ContainerType1:Canvas /Priority:0 /EditableBox1:TextBox
    /// </summary>
    [Test(2, "Clipboard", "ClipboardTestInCrossContainer1", MethodParameters = "/TestCaseType:ClipboardTestInCrossContainer /ContainerType1:Canvas /Priority:0")]
    [Test(2, "Clipboard", "ClipboardTestInCrossContainer2", MethodParameters = "/TestCaseType:ClipboardTestInCrossContainer /ContainerType1:Canvas /Priority:0 /EditableBox1:TextBox /EditableBox2:TextBox")]
    [Test(2, "Clipboard", "ClipboardTestInCrossContainer3", MethodParameters = "/TestCaseType:ClipboardTestInCrossContainer /ContainerType1:Canvas /Priority:1")]
    [Test(2, "Clipboard", "ClipboardTestInCrossContainer4", MethodParameters = "/TestCaseType:ClipboardTestInCrossContainer /ContainerType1:Canvas /Priority:0 /EditableBox1:TextBox")]
    [TestOwner("Microsoft"), TestTactics("46,47,48,49"), TestWorkItem("11,12"), TestBugs("15,17,16")]
    public class ClipboardTestInCrossContainer : TestContainer
    {
        private UIElementWrapper _editBox1;              // Editable box1
        private UIElementWrapper _editBox2;              // Editable box2
        private TestCaseData _testCaseData;
        private string _originalText;                    // Original text in editbox1
        private string _originalSelectedXmlText;         // Original selected text in Xml format before any action
        private string _originalSelectedText;            // Original text selection in editbox1
        private System.Diagnostics.Process _process;
        private const int milliseconds = 1000 * 50;

        private int _testCaseIndex;                      // Test case index default is 0
        private int[] _testFailed;                       // Array of all test that failed
        private bool _isTestFailed;                      // To remember if test failed
        private string _failID;                          // String that contain case number for failed case
        private int _endIndex;                           // End of test case index

        private enum TestCaseAction
        {
            CutPaste,
            CopyPaste,
        }

        private enum TestCaseContainer
        {
            CrossContainer,
            CrossApp,
            CrossNotepad,
        }
        
        struct TestCaseData
        {
            public TestCaseAction TestAction;
            public TestCaseContainer TestContainer;
            public string TestString;
            public int StartSelection;
            public int EndSelection;
            public int Priority;

            public TestCaseData(TestCaseAction testAction, TestCaseContainer testContainer, string testString,
                               int startSelection, int endSelection, int priority)
            {
                this.TestAction = testAction;
                this.TestContainer = testContainer;
                this.TestString = testString;
                this.StartSelection = startSelection;
                this.EndSelection = endSelection;
                this.Priority = priority;
            }

            public static TestCaseData[] Cases = new TestCaseData[] {
                //#region BVT
                //----------------------Cross Container in same app (TB, RTB)-----------------------                
                // 0: Copy mix text and paragraph and paste [Regression_Bug15]
                new TestCaseData(TestCaseAction.CopyPaste, TestCaseContainer.CrossContainer, "<Paragraph>abc</Paragraph><Paragraph>abc def ghi</Paragraph>", 0, 20, 1),
                // 1: Copy Bold or Italic text then paste cause caret to place infront of pasted text [Regression_Bug17]
                new TestCaseData(TestCaseAction.CopyPaste, TestCaseContainer.CrossContainer, "<Bold>abc</Bold>", 1, 6, 1),
                // 2: Copy part of italic and italic then paste
                new TestCaseData(TestCaseAction.CopyPaste, TestCaseContainer.CrossContainer,
                    "<Italic>Italic</Italic> <Italic>Italic</Italic>", 2, 9, 1),
                // 3: Copy Underline Italic Bold then paste [Regression_Bug16]
                new TestCaseData(TestCaseAction.CopyPaste, TestCaseContainer.CrossContainer,
                    "<Underline>u<Italic>i<Bold>This is Underline Bold italic 1.\r\n This is line2.</Bold>i</Italic>u</Underline>", 0, 65, 1),
                // 4: Cut <b>s</b>s<I>s</I>paste
                new TestCaseData(TestCaseAction.CutPaste, TestCaseContainer.CrossContainer,
                    "<Bold>Bold</Bold> plain <Italic>Italic</Italic>", 0, 25, 1),
                // 5: Copy fomatted text, FontFalmily, FontSize, FotnStype, Background, Forground then paste
                new TestCaseData(TestCaseAction.CopyPaste, TestCaseContainer.CrossContainer,
                    "abc<Run FontFamily=\"Comic Sans MS\" FontSize=\"24\" FontStyle=\"Oblique\""+
                    " Background=\"red\" Foreground=\"green\">rich</Run>def<Bold>Bold</Bold>", 1, 25, 0),
                // 6: Copy international script then paste
                // new TestCaseData(TestCaseAction.CopyPaste, TestCaseContainer.CrossContainer, StringData.MixedScripts.Value, 0, 235, 1),
                // 7: Copy 3 normal paragrah then paste
                new TestCaseData(TestCaseAction.CopyPaste, TestCaseContainer.CrossContainer,
                    "<Paragraph>This is paragraph 1.\r\n This is line2.</Paragraph><Paragraph></Paragraph><Paragraph>This is paragraph 3.</Paragraph>", 0, 50, 1),
                // 8: Copy part of 1st paragraph and last paragraph then paste
                new TestCaseData(TestCaseAction.CopyPaste, TestCaseContainer.CrossContainer,
                    "<Paragraph>This is paragraph 1.\r\n This is line2.</Paragraph><Paragraph></Paragraph><Paragraph>This is paragraph 3.</Paragraph>", 8, 45, 1),
                // 9: Cut paragraphs with indentation/margin then paste
                new TestCaseData(TestCaseAction.CutPaste, TestCaseContainer.CrossContainer,
                    "<Paragraph>This is paragraph 1.\r\n This is line2.</Paragraph><Paragraph></Paragraph>"+
                    "<Paragraph Margin=\"20px,0,0,0\">This is paragraph 3.</Paragraph>", 8, 45, 0),
                // 10: Copy paragraph with center,justify, left, right justification then paste
                new TestCaseData(TestCaseAction.CutPaste, TestCaseContainer.CrossContainer,
                    "<Paragraph TextAlignment=\"Center\">Center justification line1.\r\n This is line2.</Paragraph>"+
                    "<Paragraph TextAlignment=\"Justify\">Justify justification</Paragraph>"+
                    "<Paragraph TextAlignment=\"Left\">Left justification</Paragraph>"+
                    "<Paragraph TextAlignment=\"Right\">Rightjustification</Paragraph>", 8, 100, 1),
                // 11: Copy nested inside populated block<p><I>s</I></P> then paste
                new TestCaseData(TestCaseAction.CopyPaste, TestCaseContainer.CrossContainer,
                    "<Paragraph>This is <Italic>paragraph</Italic> 1.\r\n This is line2.</Paragraph>", 0, 30, 1),
                // 12: Copy bold then paste
                new TestCaseData(TestCaseAction.CopyPaste, TestCaseContainer.CrossContainer,
                    "<Bold>This is bold 1.\r\n This is line2.</Bold>", 0, 35, 0),
                // 13: Copy Italic then paste
                new TestCaseData(TestCaseAction.CopyPaste, TestCaseContainer.CrossContainer,
                    "<Italic>This is italic 1.\r\n This is line2.</Italic>", 0, 35, 0),
                // 14: Copy Underline then paste
                new TestCaseData(TestCaseAction.CopyPaste, TestCaseContainer.CrossContainer,
                    "<Underline>This is Underline 1.\r\n This is line2.</Underline>", 0, 40, 1),
                // 15: Copy Underline Italic Bold then paste
                new TestCaseData(TestCaseAction.CopyPaste, TestCaseContainer.CrossContainer,
                    "<Underline><Italic><Bold>This is Underline Bold italic 1.\r\n This is line2.</Bold></Italic></Underline>", 0, 55, 1),
                // 16: Copy part of bold and italic then paste
                new TestCaseData(TestCaseAction.CopyPaste, TestCaseContainer.CrossContainer,
                    "<Bold>Bold</Bold> <Italic>Italic</Italic>", 2, 9, 0),
                // 17: Cut plain text then paste
                new TestCaseData(TestCaseAction.CutPaste, TestCaseContainer.CrossContainer, "This is a test.", 2, 8, 1),
                //// 18: Cut mix text and Embeded object [Button] then paste
                //new TestCaseData(TestCaseAction.CutPaste, TestCaseContainer.CrossContainer,
                //    "abc<Button>Hello</Button> <Button>World</Button>def", 1, 10, 1),
                //// 19: Copy mix text and Embeded object [RadioButton] then paste
                //new TestCaseData(TestCaseAction.CopyPaste, TestCaseContainer.CrossContainer,
                //    "abc<RadioButton>Hello</RadioButton> <RadioButton>World</RadioButton>def", 1, 10, 1),
                //// 20: Copy mix text and Embeded object [CheckBox] then paste
                //new TestCaseData(TestCaseAction.CopyPaste, TestCaseContainer.CrossContainer,
                //    "abc<CheckBox>Hello</CheckBox> <CheckBox>World</CheckBox>def", 1, 19, 1),
                //// 21: Copy mix text and Embeded object [RadioButtonList] then paste
                //new TestCaseData(TestCaseAction.CopyPaste, TestCaseContainer.CrossContainer,
                //    "abc<ListBox><RadioButton>1</RadioButton>"+
                //    "<RadioButton>2</RadioButton></ListBox>def", 0, 12, 1),
                //// 22: Copy mix text and Embeded object [Hyperlink] then paste
                //new TestCaseData(TestCaseAction.CopyPaste, TestCaseContainer.CrossContainer,
                //    "abc<Hyperlink NavigateUri = \"a.xaml\" FontWeight=\"Bold\">Hyperlink</Hyperlink>def", 0, 9, 1),
                //// 23: Copy mix text and Embeded object [ScrollViewer] then paste
                //new TestCaseData(TestCaseAction.CopyPaste, TestCaseContainer.CrossContainer,
                //    "abc<ScrollViewer Width=\"50\" Height=\"50\"><Border Background=\"lightblue\">"+
                //    "<StackPanel Width=\"80\" Height=\"80\"><Border Background=\"yellow\">"+
                //    "<StackPanel Width=\"40\" Height=\"40\" /></Border></StackPanel></Border></ScrollViewer>def", 0, 13, 1),
                //// 24: Copy mix text and Embeded object [Slider] then paste
                //new TestCaseData(TestCaseAction.CopyPaste, TestCaseContainer.CrossContainer,
                //    "abc<Slider Minimum=\"10\" Maximum=\"200\" SmallChange=\"2\" LargeChange=\"10\"/>def", 0, 12, 1),
                //// 25: Copy mix text and Embeded object [Slider] then paste
                //new TestCaseData(TestCaseAction.CopyPaste, TestCaseContainer.CrossContainer,
                //    "abc<Slider Orientation=\"Vertical\" Minimum=\"10\" Maximum=\"200\" SmallChange=\"2\" LargeChange=\"10\" Value=\"20\"/>def", 0, 12, 1),
                //// 26: Copy mix text and Embeded object [Label] then paste
                //new TestCaseData(TestCaseAction.CopyPaste, TestCaseContainer.CrossContainer,
                //    "abc<Label>Press P</Label>def", 0, 11, 1),
                //// 27: Copy mix text and Embeded object [ListBox with ListBoxItem] then paste
                //new TestCaseData(TestCaseAction.CopyPaste, TestCaseContainer.CrossContainer,
                //    "abc<ListBox><ListBoxItem>1</ListBoxItem><ListBoxItem>2</ListBoxItem></ListBox>def", 0, 13, 1),
                
                //// 28: Copy mix text and Embeded object [ComboBox with ComboBoxItem] then paste
                //new TestCaseData(TestCaseAction.CopyPaste, TestCaseContainer.CrossContainer,
                //    "abc<ComboBox IsEditable=\"true\"><ComboBoxItem>1</ComboBoxItem><ComboBoxItem>2</ComboBoxItem></ComboBox>def", 0, 12, 1),
                ////// 29: Cut mix text and Embeded object [Menu with MenuItem] then paste Regression_Bug18
                //new TestCaseData(TestCaseAction.CutPaste, TestCaseContainer.CrossContainer,
                //    "abc<Menu><MenuItem><MenuItem.Header>_File</MenuItem.Header>"+
                //    "<MenuItem Header=\"New\"></MenuItem><MenuItem Mode=\"Separator\" /></MenuItem></Menu>def", 0, 12, 1),
                //// 30: Copy mix text and Embeded object [Menu with MenuItem] then paste (the pasted File is replace with \xFFFC)
                //new TestCaseData(TestCaseAction.CopyPaste, TestCaseContainer.CrossContainer,
                //    "abc<Menu><MenuItem><MenuItem.Header>_File</MenuItem.Header>"+
                //    "<MenuItem Header=\"New\"></MenuItem><MenuItem Mode=\"Separator\" /></MenuItem></Menu>def", 0, 12, 1),
                //31: Copy mix text and Embeded object [TabControl with TabItem] then paste
                //new TestCaseData(TestCaseAction.CopyPaste, TestCaseContainer.CrossContainer,
                //    "abc<TabControl Height=\"50\" Margin=\"2,2,2,2\"><TabItem Header=\"Item1\">"+
                //    "<StackPanel Width=\"100\"><Button>Button1</Button></StackPanel></TabItem></TabControl>def", 0, 12, 1),
                //// 32: Copy mix text and Editing control [TextBox] then paste
                //new TestCaseData(TestCaseAction.CopyPaste, TestCaseContainer.CrossContainer,
                //    "abc<TextBox>TextBox</TextBox>def", 0, 12, 1),
                //// 33: Copy mix text and Editing control [RichTextBox] then paste
                //new TestCaseData(TestCaseAction.CopyPaste, TestCaseContainer.CrossContainer,
                //    "abc<RichTextBox><FlowDocument><Paragraph>RichTextBox</Paragraph></FlowDocument></RichTextBox>def", 0, 12, 1),
                // 34: Cut mix text and Editing control [Table] then paste
                new TestCaseData(TestCaseAction.CutPaste, TestCaseContainer.CrossContainer,
                    "<Paragraph>abc</Paragraph><Table><TableRowGroup><TableRow><TableCell BorderThickness=\"1px,1px,1px,1px\"><Paragraph>c0.0</Paragraph></TableCell>"+
                    "<TableCell BorderThickness=\"1px,1px,1px,1px\"><Paragraph>c0.1</Paragraph></TableCell></TableRow></TableRowGroup></Table><Paragraph>def</Paragraph>", 0, 25, 1),
                //// 35: Cut mix text and Layout control [Canvas] then paste
                //new TestCaseData(TestCaseAction.CutPaste, TestCaseContainer.CrossContainer,
                //    "abc<Canvas Width=\"100\" Height=\"100\" Background=\"Red\"/>def", 0, 12, 1),
                //// 36: Cut mix text and Layout control [DockPanel] then paste
                //new TestCaseData(TestCaseAction.CutPaste, TestCaseContainer.CrossContainer,
                //    "abc<DockPanel Width=\"100\" Height=\"100\" Background=\"Green\"/>def", 0, 12, 1),
                //// 37: Cut mix text and Layout control [TextPanel] then paste
                //new TestCaseData(TestCaseAction.CutPaste, TestCaseContainer.CrossContainer,
                //    "abc<FlowDocumentScrollViewer Width=\"100\" Height=\"100\"><FlowDocument Background=\"Blue\"><Paragraph>abc</Paragraph></FlowDocument></FlowDocumentScrollViewer>def", 0, 12, 1),
                //// 38: Cut mix text and Layout control [StackPanel] then paste
                //new TestCaseData(TestCaseAction.CutPaste, TestCaseContainer.CrossContainer,
                //    "abc<StackPanel Width=\"100\" Height=\"100\" Background=\"Yellow\"><Button /></StackPanel>def", 0, 12, 1),
                //// 39: Cut mix text and Mil control [Text] then paste
                //new TestCaseData(TestCaseAction.CutPaste, TestCaseContainer.CrossContainer,
                //    "abc<TextBlock Background=\"Yellow\"><Button />Text</TextBlock>def", 0, 12, 1),
                // 40: Cut mix text and Mil control [Image] then paste
                //new TestCaseData(TestCaseAction.CutPaste, TestCaseContainer.CrossContainer,
                //    "abc<Image Source='w.gif' Height='60' Width='60' />def", 0, 12, 1),
                // 41: Cut mix text and Mil control [Ellipse] then paste
                //new TestCaseData(TestCaseAction.CutPaste, TestCaseContainer.CrossContainer,
                //    "abc<Ellipse Fill=\"Yellow\" Width=\"49px\" Height=\"49px\" />def", 0, 12, 1),
                ////------------------------Cross Avalon (TB, RTB)---------------------------
                //// 42:5: Cut mix content then paste
                //new TestCaseData(TestCaseAction.CutPaste, TestCaseContainer.CrossApp,
                //    "abc<Button>Hello</Button> def<Bold>TUnderline Bold italic\r\n This is line2.</Bold>" , 0, 58, 0),
                //------------------------Cross Notepad (RTB, Notepad)---------------------------
                // 43 Copy include \r\n and paste into notepad
                new TestCaseData(TestCaseAction.CopyPaste, TestCaseContainer.CrossNotepad,
                    "Hello World" , 0, 15, 0),
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

            if (Settings.GetArgumentAsInt("CaseID") != 0)
            {
                _testCaseIndex = Settings.GetArgumentAsInt("CaseID");
                _endIndex = _testCaseIndex + 1;
            }
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
                    QueueDelegate(RunCase);
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
            this.MainWindow.Title = "ClipboardTestCase FirstApp";

            //Find element
            _editBox1 = new UIElementWrapper(ElementUtils.FindElement(MainWindow, "tb1"));
            _editBox2 = new UIElementWrapper(ElementUtils.FindElement(MainWindow, "tb2"));

            Log("*******Running new test case:[" + _testCaseIndex + "]*******");
            Log("Test Action:[" + _testCaseData.TestAction + "]");
            Log("Test Container1:[" + EditableBox1 + "-" + _testCaseData.TestContainer + "]");
            if (_testCaseData.TestContainer == TestCaseContainer.CrossContainer)
                Log("Test Container2 is :[" + EditableBox2 + "]"); // Log container type for second box
            QueueDelegate(DoMouseClick);
        }

        private void DoMouseClick()
        {
            MouseInput.MouseClick(_editBox1.Element);
            QueueDelegate(DoSelectText);
        }

        private void DoSelectText()
        {
            // Disable undo for setting text programmatically
            ((TextBoxBase)_editBox1.Element).IsUndoEnabled = false;

            // Put text content into editBox1 then make selection
            if (_editBox1.Element is TextBox)
                _editBox1.Text = _testCaseData.TestString;
            else //RichTextBox
                _editBox1.XamlText = _testCaseData.TestString;

            _editBox1.Select(_testCaseData.StartSelection, _testCaseData.EndSelection);

            _originalText = _editBox1.Text;

            QueueDelegate(DoCutOrCopy);
        }
        private void DoCutOrCopy()
        {
            // Enable undo for user action
            ((TextBoxBase)_editBox1.Element).IsUndoEnabled = true;

            // Get Xml text from existing selection
            _originalSelectedXmlText = XamlUtils.TextRange_GetXml(_editBox1.SelectionInstance);
            // Get plain text from existing selection
            _originalSelectedText = _editBox1.GetSelectedText(false, false);

            if (_testCaseData.TestAction == TestCaseAction.CutPaste)
                KeyboardInput.TypeString("^x");
            else if (_testCaseData.TestAction == TestCaseAction.CopyPaste)
                KeyboardInput.TypeString("^c");

            QueueDelegate(VerifyAfterDoCutOrCopy);
        }
        private void VerifyAfterDoCutOrCopy()
        {
            // If cut, verify content is removed and there is no selection
            if (_testCaseData.TestAction == TestCaseAction.CutPaste)
            {
                FailIfStringEqual(_editBox1.Text, _originalText, "Text should rearrange after cut.");
                FailIfStringNotEqual(_editBox1.GetSelectedText(false, false), "", "There should be no selection after cut.");
            }
            // If copy, verify content and selection are remain
            else
            {
                FailIfStringNotEqual(_editBox1.Text, _originalText, "Text should remain after copy.");
                FailIfStringNotEqual(_editBox1.GetSelectedText(false, false), _originalSelectedText, "Selection should remain after copy.");
            }

            // Compare clipboard data
            if (!Clipboard.GetDataObject().GetDataPresent(DataFormats.Xaml))
            {
                FailIfStringNotEqual(Clipboard.GetDataObject().GetFormats().Length.ToString(), "3", "There should be 3 formats.");
            }
            else
            {
                FailIfStringNotEqual(Clipboard.GetDataObject().GetFormats().Length.ToString(), "6", "There should be 6 formats.");
                string actualClipboardXmlData;
                actualClipboardXmlData = Clipboard.GetDataObject().GetData(DataFormats.Xaml).ToString();

                // StartFragment get inserted to Clipboard data, I'm removing it then compare with original string
                if (actualClipboardXmlData.Contains("<!--StartFragment-->"))
                {
                    actualClipboardXmlData = actualClipboardXmlData.Replace("<!--StartFragment-->", "");
                    actualClipboardXmlData = actualClipboardXmlData.Replace("<!--EndFragment-->", "");
                }

                // Verify clipboard xml content matched with original selection xml content
                FailIfStringNotEqual(_originalSelectedXmlText, actualClipboardXmlData, "Clipboard Xml data matched.");
            }
            // Verify clipboard text content matched with original selection text content
            string actualClipboardTextData;
            actualClipboardTextData = Clipboard.GetDataObject().GetData(DataFormats.UnicodeText).ToString();
            FailIfStringNotEqual(_originalSelectedText, actualClipboardTextData, "Clipboard Text data matched.");

            // DoUndo after cut or copy
            KeyboardInput.TypeString("^z");

            QueueDelegate(VerifyUndo);
        }
        private void VerifyUndo()
        {
            // Verify undo: nothing should happen for copy.  For cut, content should come back
            FailIfStringNotEqual(_editBox1.Text, _originalText, "Text should remain after Undo.");
            FailIfStringNotEqual(_editBox1.GetSelectedText(false, false), _originalSelectedText, "Selection should remain after Undo.");

            // Find a point, mouse click on it then paste
            MouseInput.MouseClick(_editBox2.Element);

            QueueDelegate(DoPaste);
        }
        private void DoPaste()
        {
            if (_testCaseData.TestContainer == TestCaseContainer.CrossApp)
            {
                // Launch second app
                if (_editBox1.Element is TextBox)
                {
                    _process = Avalon.Test.Win32.Interop.LaunchAProcess("EditingTest.exe",
                        "/TestCaseType=ClipboardTestInCrossContainerForSecondApp /EditableBox1:TextBox");
                }
                else
                {
                    _process = Avalon.Test.Win32.Interop.LaunchAProcess("EditingTest.exe",
                        "/TestCaseType=ClipboardTestInCrossContainerForSecondApp");
                }
                QueueDelegate(VerifyPaste);
            }
            else if (_testCaseData.TestContainer == TestCaseContainer.CrossNotepad)
            {
                _process = new System.Diagnostics.Process();
                _process.StartInfo.FileName = "notepad.exe";
                _process.Start();
                //Let piper monitor process so it can be cleaned up if process hang.
                //Test.Uis.Utils.ConfigurationSettings.Current.AutomationFramework.MonitorProcess(process);
                
                // Paste and verify paste in Notepad
                QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromSeconds(2), new SimpleHandler(PasteInNotepad));
            }
            else
            {
                KeyboardInput.TypeString("^v");
                QueueDelegate(VerifyPaste);
                QueueDelegate(DoUndo);
            }
        }

        private void PasteInNotepad()
        {
            // Paste whe notepad has focus.
            InputMonitorManager.Current.IsEnabled = false;
            KeyboardInput.TypeString("^v");
            QueueDelegate(VerifyInNotepad);
        }

        private void VerifyInNotepad()
        {
            // Verify pasted contents.
            System.Windows.Automation.AutomationElement rootElement =
                System.Windows.Automation.AutomationElement.RootElement;
            System.Text.StringBuilder results = new System.Text.StringBuilder();
            IntPtr hwndNotepad = _process.MainWindowHandle;
            if (hwndNotepad == IntPtr.Zero)
            {
                throw new Exception("Unable to get handle of Notepad app window.");
            }
            System.Windows.Automation.AutomationElement notepad = System.Windows.Automation.AutomationElement.FromHandle(hwndNotepad);
            if (notepad == null)
            {
                throw new Exception("Unable to get AutomationElement from Notepad app window handle.");
            }

            System.Windows.Automation.AutomationElement[] element = XPathNavigatorUtils.ListAutomationElements(notepad, "./*[contains(@ClassName, 'Edit')]");
            if (element.Length == 0)
            {
                throw new Exception("Cannot find element of type 'Edit' in Notepad.");
            }
            System.Windows.Automation.TextPattern textPattern;
            object _patternObject;
            element[0].TryGetCurrentPattern(System.Windows.Automation.TextPattern.Pattern, out _patternObject);
            textPattern = (System.Windows.Automation.TextPattern)_patternObject;
            if (textPattern == null)
            {
                throw new Exception("Unable to get TextPattern from Notepad edit control.");
            }
            if (_editBox1.Element is RichTextBox)
            {
                FailIfStringNotEqual("Hello World\r\n", textPattern.DocumentRange.GetText(-1), "Content in Notepad is not matched.");
            }
            else
            {
                FailIfStringNotEqual("Hello World", textPattern.DocumentRange.GetText(-1), "Content in Notepad is not matched.");
            }

            _process.Kill();
            _process.Dispose();
            _process = null;

            // Run next test case
            _testCaseIndex++;
            RunCase();
        }

        private void VerifyPaste()
        {
            if (_testCaseData.TestContainer == TestCaseContainer.CrossApp)
            {
                Avalon.Test.Win32.Interop.ProcessWait(_process, milliseconds);
                if (!Logger.Current.ProcessLog("ClipboardTestingLog.txt"))
                {
                    Logger.Current.ReportResult(false, "Paste in second app is failed.", true);
                    _testFailed[_testCaseIndex] = -1;
                    _isTestFailed = true;
                }
                // Run next test case
                _testCaseIndex++;
                RunCase();
            }
            else if (_editBox1.Element is TextBox)
            {
                // If copy or cut text with out any \r\n (ex: abc) the pasted text will be the same (ex:abc).
                // If copy or cut text with any \r\n (ex: abc\r\nde) the pasted text will add \r\n to the end (ex: abc\r\nde\r\n).
                string actualTemp;
                actualTemp = _editBox2.Text;
                if (!actualTemp.EndsWith("\r\n"))
                {
                    //add \r\n to the end
                    actualTemp = actualTemp.Replace(actualTemp, actualTemp + "\r\n");
                }

                string expectTemp;
                expectTemp = _editBox1.GetSelectedText(false, false);
                if (!expectTemp.EndsWith("\r\n"))
                {
                    //add \r\n to the end
                    expectTemp = expectTemp.Replace(expectTemp, expectTemp + "\r\n");
                }
                FailIfStringNotEqual(expectTemp, actualTemp, "Pasted text from TB to RTB is not equal.");
            }
            else
            {
                TextPointer tpStart1;
                TextPointer tpEnd1;
                TextPointer tpStart2;
                TextPointer tpEnd2;
                tpStart1 = _editBox1.SelectionInstance.Start;
                tpEnd1 = _editBox1.SelectionInstance.End;
                tpStart2 = _editBox2.TextRange.Start;
                tpEnd2 = _editBox2.TextRange.End;
                bool compareTextRangeContents;
                string unmatchedReason;
                compareTextRangeContents = TextTreeTestHelper.CompareTextRangeContents(
                    tpStart1, tpEnd1, tpStart2, tpEnd2, out unmatchedReason);
                if (!compareTextRangeContents)
                {
                    Logger.Current.ReportResult(false, "Pasted text is not matched." +
                    "\nExpect [" + XamlUtils.TextRange_GetXml(_editBox1.SelectionInstance) + "]" +
                    "\nActual [" + XamlUtils.TextRange_GetXml(_editBox2.TextRange) + "]", true);
                    _testFailed[_testCaseIndex] = -1;
                    _isTestFailed = true;
                }
            }
        }
        private void DoUndo()
        {
            // Do undo in second container
            KeyboardInput.TypeString("^z");

            QueueDelegate(VerifyUndoEditBox2);
        }
        private void VerifyUndoEditBox2()
        {
            if (_editBox2.Element is TextBox)
            {
                FailIfStringNotEqual(_editBox2.Text, "", "Content should undo in TextBox.");
            }
            else
            {
                FailIfStringNotEqual(_editBox2.Text, "\r\n", "Content should undo.");
            }

            // Do redo in second container
            KeyboardInput.TypeString("^y");
            QueueDelegate(VerifyRedoEditBox2);
        }
        private void VerifyRedoEditBox2()
        {
            if (_editBox1.Element is TextBox)
            {
                string actualTemp;
                actualTemp = _editBox2.Text;
                if (!actualTemp.EndsWith("\r\n"))
                {
                    //add \r\n to the end
                    actualTemp = actualTemp.Replace(actualTemp, actualTemp + "\r\n");
                }

                string expectTemp;
                expectTemp = _editBox1.GetSelectedText(false, false);
                if (!expectTemp.EndsWith("\r\n"))
                {
                    //add \r\n to the end
                    expectTemp = expectTemp.Replace(expectTemp, expectTemp + "\r\n");
                }
                FailIfStringNotEqual(expectTemp, actualTemp, "Pasted text from TB to RTB is not equal.");
            }
            else
            {
                TextPointer tpStart1;
                TextPointer tpEnd1;
                TextPointer tpStart2;
                TextPointer tpEnd2;
                tpStart1 = _editBox1.SelectionInstance.Start;
                tpEnd1 = _editBox1.SelectionInstance.End;
                tpStart2 = _editBox2.TextRange.Start;
                tpEnd2 = _editBox2.TextRange.End;
                bool compareTextRangeContents;
                string unmatchedReason;

                compareTextRangeContents = Test.Uis.TextEditing.TextTreeTestHelper.CompareTextRangeContents(
                    tpStart1, tpEnd1, tpStart2, tpEnd2, out unmatchedReason);
                if (!compareTextRangeContents)
                {
                    Logger.Current.ReportResult(false, "Undo text is not matched." +
                    "\nExpect [" + XamlUtils.TextRange_GetXml(_editBox1.SelectionInstance) + "]" +
                    "\nActual [" + XamlUtils.TextRange_GetXml(_editBox2.SelectionInstance) + "]", true);
                    _testFailed[_testCaseIndex] = -1;
                    _isTestFailed = true;
                }
            }
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
    # endregion ClipboardTestInCrossContainer
    #region ClipboardTestInCrossContainerForSecondApp
    /// <summary>
    /// Second app for Testing clipboard - verifies that clipboard works across application
    /// data-driven scenarios.
    /// </summary>
    public class ClipboardTestInCrossContainerForSecondApp : CustomTestCase
    {
        private UIElementWrapper _editBox1;
        private string _editableBox1;        // Editable box1

        private void BuildWindow()
        {
            Panel topPanel;
            FrameworkElement box1;

            topPanel = (Panel)ReflectionUtils.CreateInstanceOfType("Canvas", new object[] { });
            topPanel.Background = Brushes.Violet;

            // The containers can be TextBox and RichTextBox
            _editableBox1 = Settings.GetArgument("EditableBox1");
            if (_editableBox1 == "")
            {
                // Default to RichTextBox
                _editableBox1 = "RichTextBox";
            }
            box1 = TextEditableType.GetByName(_editableBox1).CreateInstance();
            box1.Height = 100;
            box1.Width = 250;
            box1.SetValue(TextBox.TextWrappingProperty, TextWrapping.Wrap);
            box1.SetValue(TextBox.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Auto);
            box1.SetValue(TextBox.AcceptsReturnProperty, true);
            box1.Name = "tb1";
            box1.SetValue(TextBox.FontFamilyProperty, new FontFamily("Microsoft Sans Serif"));
            topPanel.Children.Add(box1);

            MainWindow.Content = topPanel;
        }

        /// <summary>Runs the test case</summary>
        public override void RunTestCase()
        {
            //Create log file for logging on second app.
            if (Test.Uis.IO.TextFileUtils.Exists("ClipboardTestingLog.txt"))
                Test.Uis.IO.TextFileUtils.Delete("ClipboardTestingLog.txt");
            Logger.Current.LogToFile("ClipboardTestingLog.txt");
            BuildWindow();
            this.MainWindow.Title = "ClipboardTestInCrossContainer Second App";

            QueueDelegate(StartTest);
        }

        /// <summary>Start test action</summary>
        private void StartTest()
        {
            _editBox1 = new UIElementWrapper(ElementUtils.FindElement(MainWindow, "tb1"));
            _editBox1.Element.Focus();
            Log("Test Container2 is :[" + _editableBox1 + "]");

            //paste into textbox in second app
            RoutedCommand PasteCommand = ApplicationCommands.Paste;
            PasteCommand.Execute(null, _editBox1.Element);

            //verify result after paste then exit second app
            QueueDelegate(VerifyResult);
        }

        /// <summary>Verify resutl</summary>
        private void VerifyResult()
        {
            // Verify pasted text has no selection
            Verifier.Verify(_editBox1.GetSelectedText(false, false) == "", "SelectedText should be empty in second app.");

            // Verify pasted text has correct format
            if (_editBox1.Element is RichTextBox)
            {
                Verifier.Verify(_editBox1.Text == "abc  defTUnderline Bold italic\n This is line2.\r\n\r\n", "Pasted text matched." +
                    "\n Expect text: [abc  defTUnderline Bold italic\n This is line2.\r\n\r\n]" +
                    "\n Actual text: [" + _editBox1.Text + "]");

                if (XamlUtils.TextRange_GetXml(_editBox1.TextRange).Contains("<Run") &&
                    XamlUtils.TextRange_GetXml( _editBox1.TextRange).Contains("<Button") &&
                    XamlUtils.TextRange_GetXml(_editBox1.TextRange).Contains("<Bold"))
                    Logger.Current.ReportResult(true, "Pasted in second app matched.");
            }
            else
            {
                Log("Actual text in TextBox: "+_editBox1.Text);
                Verifier.Verify(_editBox1.Text == "abc<Button>Hello</Button> def<Bold>TUnderline Bold italic\r\n",
               "Pasted in second app matched.");
            }
            Logger.Current.ReportSuccess();
        }
    }
    #endregion ClipboardTestInCrossContainerForSecondApp
}
