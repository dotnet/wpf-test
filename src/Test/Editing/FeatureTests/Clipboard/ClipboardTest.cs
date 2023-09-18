// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
using Microsoft.Test.Logging;
using Microsoft.Test.Diagnostics;


[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/forms/BVT/DataTransfer/Clipboard/ClipboardTest.cs $")]
namespace DataTransfer
{
    #region ClipboardTestCase

    /// <summary>
    /// verifies that cut/paste and copy/paste works for:
    /// a. Text type:
    ///     I.      plain text  BVT
    ///     II.     mixText - plain text, inline text, embeded object   BVT
    ///     III.    mixText - plain text, inline text, embeded object, paragraph
    ///     IV.     2 paragraph including rich text
    ///     V.      1 page of text including rich text (no scroll)
    ///     VI.     1 big page of text including of rich text (big text that causes to have scroll)
    /// b. Control type:
    ///     I.      TextBox
    ///     II.     RichTextBox
    ///     III.    Text
    ///     III.    TextPanel
    /// c. Container type:
    ///     I.      Same container
    ///     II.     Cross container
    ///     III.    Cross application
    /// data-driven scenarios.
    /// BVT: 33
    /// P1 : 95792
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("33"), TestLastUpdatedOn("July 11, 2006")]
    public class ClipboardTestCase : CustomTestCase
    {
        private UIElementWrapper _editBox1;
        private UIElementWrapper _editBox2;
        private TestCaseData _testCaseData;
        private string _originalSelectionText;
        private string _actualClipboardData;
        private string _newClipboardData;
        private System.Diagnostics.Process _process;
        private const int milliseconds = 1000 * 10;
        private int _testCaseIndex;
        private int _testPriority = 0;
        private int _caseID;

        private string EditableTypeName
        {
            get
            {
                string result;

                result = Settings.GetArgument("EditableType");
                if (result == "") result = "RichTextBox";

                return result;
            }
        }

        private void BuildWindow()
        {
            Panel topPanel;
            FrameworkElement box1;
            FrameworkElement box2;

            topPanel = (Panel)ReflectionUtils.CreateInstanceOfType(Settings.GetArgument("ContainerType1"), new object[] {});
            topPanel.Background = Brushes.SkyBlue;

            box1 = TextEditableType.GetByName(EditableTypeName).CreateInstance();
            box2 = TextEditableType.GetByName(EditableTypeName).CreateInstance();
            box1.Height = box2.Height = 100;
            box1.Width = box2.Width = 250;
            box1.SetValue(TextBox.TextWrappingProperty, TextWrapping.Wrap);
            box2.SetValue(TextBox.TextWrappingProperty, TextWrapping.Wrap);
            box1.SetValue(TextBox.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Auto);
            box2.SetValue(TextBox.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Auto);
            box1.SetValue(TextBox.AcceptsReturnProperty, true);
            box2.SetValue(TextBox.AcceptsReturnProperty, true);
            box2.SetValue(Canvas.TopProperty, 120d);
            box1.Name = "tb1";
            box2.Name = "tb2";
            if (EditableTypeName == "RichTextBox")
            {
                box1.SetValue(TextBox.FontSizeProperty, 30.0);
                box2.SetValue(TextBox.FontSizeProperty, 30.0);
            }
            topPanel.Children.Add(box1);
            topPanel.Children.Add(box2);

            MainWindow.Content = topPanel;
        }

        private enum TestCaseAction
        {
            CutPaste,
            CopyPaste,
        }
        private enum TestCaseContainer
        {
            SameContainer,
            CrossContainer,
            CrossApp,
        }
        private static string InternationalScripts()
        {
            TextScript[] scripts;
            string textScripts;
            textScripts = "xxx";
            scripts = TextScript.Values;
            for (int i = 0; i < scripts.Length; i++)
            {
                textScripts += scripts[i].Sample;
            }
            textScripts += "yyy";
            return textScripts;
        }
        struct TestCaseData
        {
            public TestCaseAction TestAction;
            public TestCaseContainer TestContainer;
            public string TestString;
            public int StartSelection;
            public int EndSelection;

            public TestCaseData(TestCaseAction testAction, TestCaseContainer testContainer, string testString, int startSelection,
                                int endSelection)
            {
                this.TestAction = testAction;
                this.TestContainer = testContainer;
                this.TestString = testString;
                this.StartSelection = startSelection;
                this.EndSelection = endSelection;
            }

            public static TestCaseData[] Cases = new TestCaseData[] {
                #region BVT
                new TestCaseData(TestCaseAction.CutPaste, TestCaseContainer.SameContainer, "abxxx  yyyef", 2, 8),
                new TestCaseData(TestCaseAction.CutPaste, TestCaseContainer.SameContainer,
                    "axxx<Button>Button1</Button>b<Button>Button2</Button>yyyc", 1, 9),
                new TestCaseData(TestCaseAction.CopyPaste, TestCaseContainer.SameContainer, "abxxxyyyef", 2, 6),
                new TestCaseData(TestCaseAction.CopyPaste, TestCaseContainer.SameContainer,
                    "axxx<Button>Button1</Button>b<Button>Button2</Button>yyyc", 1, 9),
                new TestCaseData(TestCaseAction.CopyPaste, TestCaseContainer.SameContainer,
                    "axxx <Bold Background=\"red\" Foreground=\"green\" FontSize=\"24\""+
                    " FontFamily=\"Comic Sans MS\" FontStyle=\"Oblique\">"+
                    "rich</Bold> yyyc", 1, 14),
                new TestCaseData(TestCaseAction.CopyPaste, TestCaseContainer.SameContainer,
                    "Axxx   a a       yyy   ", 1, 19),  //Regression_Bug6
                new TestCaseData(TestCaseAction.CutPaste, TestCaseContainer.CrossContainer, "abxxx  yyyef", 2, 8),
                new TestCaseData(TestCaseAction.CutPaste, TestCaseContainer.CrossContainer,
                    "axxx<Button>Button1</Button>b<Button>Button2</Button>yyyc", 1, 9),
                new TestCaseData(TestCaseAction.CopyPaste, TestCaseContainer.CrossContainer, "abxxxyyyef", 2, 6),
                new TestCaseData(TestCaseAction.CopyPaste, TestCaseContainer.CrossContainer,
                    "axxx<Button>Button1</Button>b<Button>Button2</Button>yyyc", 1, 9),
                new TestCaseData(TestCaseAction.CopyPaste, TestCaseContainer.CrossContainer,
                    "axxx <Bold Background=\"red\" Foreground=\"green\" FontSize=\"24\""+
                    " FontFamily=\"Comic Sans MS\" FontStyle=\"Oblique\">"+
                    "rich</Bold> yyyc", 1, 14),
                new TestCaseData(TestCaseAction.CopyPaste, TestCaseContainer.CrossContainer, InternationalScripts(),
                    0, 235),
                new TestCaseData(TestCaseAction.CopyPaste, TestCaseContainer.CrossApp,
                    "axxx<Button>Button1</Button>b  <Bold Background=\"red\" Foreground=\"green\" FontSize=\"24\""+
                    " FontFamily=\"Comic Sans MS\" FontStyle=\"Oblique\">"+
                    "rich</Bold><Button>Button2</Button>yyyc", 1, 17),
                #endregion BVT
                #region P1
                new TestCaseData(TestCaseAction.CutPaste, TestCaseContainer.CrossApp, "abxxx  yyyef", 2, 8),
                new TestCaseData(TestCaseAction.CutPaste, TestCaseContainer.CrossApp,
                    "axxx<Button>Button1</Button>b<Button>Button2</Button>yyyc", 1, 9),
                new TestCaseData(TestCaseAction.CopyPaste, TestCaseContainer.CrossApp, "abxxxyyyef", 2, 6),
                new TestCaseData(TestCaseAction.CopyPaste, TestCaseContainer.CrossApp,
                    "axxx <Bold Background=\"red\" Foreground=\"green\" FontSize=\"24\""+
                    " FontFamily=\"Comic Sans MS\" FontStyle=\"Oblique\">"+
                    "rich</Bold> yyyc", 1, 14),
                // 17: Test case for Regression_Bug7 TC:34
                new TestCaseData(TestCaseAction.CopyPaste, TestCaseContainer.SameContainer,
                    "<Bold>xxx</Bold>o<Bold>yyy</Bold>", 1, 9),
                #endregion P1
            };
        }

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            BuildWindow();
            this.MainWindow.Width = 500;
            this.MainWindow.Height = 500;
            this.MainWindow.Title = "ClipboardTestCase FirstApp";

            //Find element
            _editBox1 = new UIElementWrapper(ElementUtils.FindElement(MainWindow, "tb1"));
            _editBox2 = new UIElementWrapper(ElementUtils.FindElement(MainWindow, "tb2"));

            Log("\n" +
                "************************************************************************\n" +
                "* To run each test case append '/TestPriority=4 /CaseID:# /Repro:true' *\n" +
                "* 'CaseID' can be any number from 0 to 16 or the test that fail.       *\n" +
                "************************************************************************");
            _testPriority = Test.Uis.Utils.ConfigurationSettings.Current.GetArgumentAsInt("TestPriority");
            _caseID = ConfigurationSettings.Current.GetArgumentAsInt("CaseID");
            if (_testPriority == 0)
            {
                Log("BVT cases start from 0 to 12");
                _testCaseIndex = 0;
            }
            else if (_testPriority == 1)
            {
                Log("P1 cases start from 13 to 16");
                _testCaseIndex = 13;
            }
            else if (_testPriority == 4)
            {
                _testCaseIndex = _caseID;
            }
            StartTest();
        }

        private void StartTest()
        {
            Log("*******Running new test case:[" + _testCaseIndex + "]*******");
            Log("Test Container:[" + EditableTypeName + "-" + _testCaseData.TestContainer + "]");
            Log("Test Action:[" + _testCaseData.TestAction + "]");
            MouseInput.MouseClick(_editBox1.Element);
            QueueDelegate(new SimpleHandler(DoSelectText));
        }
        private void DoSelectText()
        {
            _testCaseData = TestCaseData.Cases[_testCaseIndex];

            //Set data to editBox
            _editBox1.XamlText = _testCaseData.TestString;

            if (_editBox1.Element is TextBox)
            {
                ((TextBox)_editBox1.Element).Select(_testCaseData.StartSelection, _testCaseData.EndSelection);
            }
            else
            {
                ReflectionUtils.InvokeInstanceMethod(_editBox1, "Select", new object[] { _testCaseData.StartSelection, _testCaseData.EndSelection });
            }
            Log("Actual selection [" + _editBox1.GetSelectedText(false, false) + "]");
            bool repro = ConfigurationSettings.Current.GetArgumentAsBool("Repro");
            if (!repro) // repro=false => run automation
            {
                QueueDelegate(new SimpleHandler(DoCutOrCopy));
            }
        }
        private void DoCutOrCopy()
        {
            //Create Xaml text from existing selection
            _originalSelectionText = XamlUtils.TextRange_GetXml(_editBox1.SelectionInstance);

            if (_testCaseData.TestAction == TestCaseAction.CutPaste)
            {
                KeyboardInput.TypeString("^x");
            }
            else if (_testCaseData.TestAction == TestCaseAction.CopyPaste)
            {
                KeyboardInput.TypeString("^c");
            }
            QueueDelegate(new SimpleHandler(DoPaste));
        }
        private void DoPaste()
        {
            //Compare clipboard data
            _actualClipboardData = Clipboard.GetDataObject().GetData(DataFormats.Xaml).ToString();
            if (_actualClipboardData.Contains("<!--StartFragment-->"))
            {
                _actualClipboardData=_actualClipboardData.Replace("<!--StartFragment-->", "");
                _actualClipboardData=_actualClipboardData.Replace("<!--EndFragment-->", "");
            }
            Verifier.Verify(_originalSelectionText == _actualClipboardData, "Clipboard data matched." +
                "Expect Clipboard Data [" + _originalSelectionText + "]"+
                "Actual Clipboard Data [" + _actualClipboardData + "]");

            //Simplify xml content
            SimplifyXmlString();

            //Check for TestContainer then do paste
            switch (_testCaseData.TestContainer)
            {
                case TestCaseContainer.SameContainer:
                    KeyboardInput.TypeString("^{End}^v");
                    QueueDelegate(RunNextTestCase);
                    break;
                case TestCaseContainer.CrossContainer:
                    MouseInput.MouseClick(_editBox2.Element);
                    QueueDelegate(DoPasteInEditBox2);
                    break;
                case TestCaseContainer.CrossApp:
                    Log("Preparing and loading second app...");
                    _process = Avalon.Test.Win32.Interop.LaunchAProcess("EditingTest.exe",
                        "/TestCaseType=ClipboardTestForSecondApp /TestName=ClipboardTestCase-RichTextBox");
                    QueueDelegate(RunNextTestCase);
                    break;
            }
        }

        private void DoPasteInEditBox2()
        {
            KeyboardInput.TypeString("^a^v");
            QueueDelegate(RunNextTestCase);
        }

        private void RunNextTestCase()
        {
            string actualString;

            if (_testCaseData.TestContainer == TestCaseContainer.SameContainer)
            {
                actualString = _editBox1.XamlText;
                Verifier.Verify(actualString == _testCaseData.TestString, "Paste text matched in SameContainer."+
                    "Expect text after pasted [" + _testCaseData.TestString + "]"+
                    "Actual text in editBox1  [" + actualString + "]");
            }
            else if (_testCaseData.TestContainer == TestCaseContainer.CrossContainer)
            {
                actualString = _editBox2.XamlText;
                if (actualString.Contains("</mscorlib_System_1:String></Text.Text>")
                    || actualString.Contains(" xml:space=\"preserve\""))
                {
                    actualString = actualString.Replace("</mscorlib_System_1:String></Text.Text>", "");
                    actualString = actualString.Replace(
                        "<Text.Text><?Mapping XmlNamespace=\"mscorlib_System_1ns\" ClrNamespace=\"System\" Assembly=\"mscorlib\" ?><mscorlib_System_1:String xmlns:mscorlib_System_1=\"mscorlib_System_1ns\">", "");
                    actualString = actualString.Replace(" xml:space=\"preserve\"", "");
                }
                //Log("Expect text after pasted ["+newClipboardData+"]");
                //Log("Actual text in editBox2  ["+actualString+"]");
                Verifier.Verify(actualString == _newClipboardData, "Pasted text matched in CrossContainer.", true);
            }
            else if (_testCaseData.TestContainer == TestCaseContainer.CrossApp)
            {
                Avalon.Test.Win32.Interop.ProcessWait(_process, milliseconds);
                if (Avalon.Test.Win32.Interop.ProcessExit(_process))
                {
                    Verifier.Verify(Logger.Current.ProcessLog("ClipboardTestLog.txt"), "Test is : "
                        + Logger.Current.ProcessLog("ClipboardTestLog.txt"), true);
                    Logger.Current.Log("Both apps are closed and test is complete.");
                }
            }

            //Run next test case
            _testCaseIndex++;
            if (_testPriority == 0)
            {
                if (_testCaseIndex == 13)
                {
                    Logger.Current.ReportSuccess();
                }
                else
                {
                    Log("Restart next test case...");
                    StartTest();
                }
            }
            else if (_testPriority == 1)
            {
                if (_testCaseIndex == 17)
                {
                    Logger.Current.ReportSuccess();
                }
                else
                {
                    Log("Restart next test case...");
                    StartTest();
                }
            }
            else if (_testPriority == 4)
            {
                Logger.Current.ReportSuccess();
            }
        }

        private void SimplifyXmlString()
        {
            int startIndexFront;
            int startIndexEnd;

            //subtract extra informat from clipbard data string
            startIndexFront = _actualClipboardData.IndexOf("xxx");
            _newClipboardData = _actualClipboardData.Remove(0, startIndexFront);

            startIndexEnd = _newClipboardData.IndexOf("yyy") + 3;
            _newClipboardData = _newClipboardData.Remove(startIndexEnd, _newClipboardData.Length - startIndexEnd);

            //After cut, resave content in editBox1.
            _testCaseData.TestString = _editBox1.XamlText;
            //For SameContainer copy/cut, clipboard text is pasted at the end of editBox1
            _testCaseData.TestString = _testCaseData.TestString + _newClipboardData;

            //remove  xml:space=\"preserve\" from xml string
            //becuase it is repositioned after paste.
            if (_testCaseData.TestString.Contains(" xml:space=\"preserve\""))
            {
                _newClipboardData = _newClipboardData.Replace(" xml:space=\"preserve\"", "");
                _testCaseData.TestString = _testCaseData.TestString.Replace(" xml:space=\"preserve\"", "");
            }
        }
    }

    #endregion ClipboardTestCase

    #region ClipboardInTextBox

    /// <summary>
    /// Test coverage for regression bugs.
    /// TC# 35, 36, 37
    /// </summary>
    [Test(2, "Clipboard", "ClipboardInTextBox1", MethodParameters = "/TestCaseType=ClipboardInTextBox /TextType=PasteBitmapImage")]
    [Test(2, "Clipboard", "ClipboardInTextBox2", MethodParameters = "/TestCaseType=ClipboardInTextBox /TextType=RlTbTextBox")]
    [Test(2, "Clipboard", "ClipboardInTextBox3", MethodParameters = "/TestCaseType=ClipboardInTextBox /TextType=PasteMultilinesInAcceptsReturnFalse")]
    [Test(2, "Clipboard", "ClipboardInTextBox4", MethodParameters = "/TestCaseType=ClipboardInTextBox /TextType=CutCopyPasteAPI")]
    public class ClipboardInTextBox : Test.Uis.TextEditing.TextBoxTestCase
    {
        private string _trString;
        private TextBox _inner;

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            MouseInput.MouseClick(TestTextBox); //mouse click in text to set focus
            TestTextBox.AcceptsReturn = true;
            TestTextBoxAlt.AcceptsReturn = true;
            switch (ConfigurationSettings.Current.GetArgument("TextType"))
            {
                case "PasteBitmapImage": //Regression_Bug8
                    KeyboardInput.TypeString("abc^a"); //select all
                    QueueDelegate(DoAction);
                    break;
                case "RlTbTextBox":
                    TestTextBox.FlowDirection = FlowDirection.RightToLeft;
                    TestTextBoxAlt.FlowDirection = FlowDirection.RightToLeft;
                    TestTextBox.AppendText("abc def\n123 456");
                    KeyboardInput.TypeString("^a"); //select all
                    QueueDelegate(DoAction);
                    break;
                case "PasteInNestTextBox":  //Regression_Bug9
                    TestTextBox.Width = 400;
                    TestTextBox.Text = "abc";
                    _inner = new TextBox();
                    _inner.Width = 100;
                    _inner.Height = 100;
                    ((IAddChild)TestTextBox).AddChild(_inner);
                    KeyboardInput.TypeString("{Left}"); //to clear selection for selected nested textbox
                    QueueDelegate(EditinInnerTextBox);
                    break;
                case "PasteMultilinesInAcceptsReturnFalse": //Regression_Bug10
                    TestTextBoxAlt.AcceptsReturn = false;
                    KeyboardInput.TypeString("abc xxx{Enter}def yyy^a"); //select all
                    QueueDelegate(DoAction);
                    break;
                case "CutCopyPasteAPI":
                    TestTextBox.Text = "abc def";
                    TestTextBox.SelectAll();
                    TestTextBox.Copy();
                    TestTextBoxAlt.Paste();
                    Verifier.Verify(TestTextBoxAlt.Text == TestTextBox.Text, "Text pasted.", true);
                    TestTextBox.Cut();
                    Verifier.Verify(TestTextBox.Text == "", "Text cut.", true);
                    TestTextBoxAlt.Paste();
                    Verifier.Verify(TestTextBoxAlt.Text == "abc defabc def", "Text pasted.", true);
                    Logger.Current.ReportSuccess();
                    break;
            }
        }

        private void EditinInnerTextBox()
        {
            MouseInput.MouseClick(_inner);
            KeyboardInput.TypeString("def^a");
            QueueDelegate(CopyPasteInInnterTextBox);
        }

        private void CopyPasteInInnterTextBox()
        {
            Log("Expect seleted text [def]");
            Log("Actual selected text [" + _inner.SelectedText + "]");
            Verifier.Verify(_inner.SelectedText == "def", "Text typed in inner TextBox.");
            KeyboardInput.TypeString("^c^v");
            QueueDelegate(VerifyTextPasted);
        }

        private void VerifyTextPasted()
        {
            Log("Expect clipboard text [def]");
            Log("Actual clipboard text [" + (Clipboard.GetDataObject()).GetData(DataFormats.Text).ToString() + "]");
            Verifier.Verify((Clipboard.GetDataObject()).GetData(DataFormats.Text).ToString() == "def",
                "Clipboard data matched.");
            Log("Expect Text selection []");
            Log("Actual Text selection [" + _inner.SelectedText + "]");
            Verifier.Verify(_inner.SelectedText == "", "Selection matched.");
            Logger.Current.ReportSuccess();
        }

        private void DoAction()
        {
            TextSelection textSelection = Test.Uis.Utils.TextUtils.GetTextBoxSelection(TestTextBox);

            _trString =XamlUtils.TextRange_GetXml(textSelection);
            if (ConfigurationSettings.Current.GetArgument("TextType") == "PasteBitmapImage")
            {
                System.Drawing.Bitmap myBitmap;
                myBitmap = new System.Drawing.Bitmap(100, 100);
                Clipboard.SetDataObject(myBitmap);
                IDataObject dobj = Clipboard.GetDataObject();
                Verifier.Verify(dobj.GetData(typeof(System.Drawing.Bitmap)).ToString() == myBitmap.ToString(),
                    "Clipboard data set correctly", true);                
                KeyboardInput.TypeString("^v");
                QueueDelegate(VerifyResult);
            }
            else
            {
                KeyboardInput.TypeString("^c"); //copy
                QueueDelegate(DoMouseClick);
            }
        }

        private void DoMouseClick()
        {
            MouseInput.MouseClick(TestControlAlt); //mouse click on senond TextBox to set focus
            QueueDelegate(DoPaste);
        }

        private void DoPaste()
        {
            KeyboardInput.TypeString("^v"); //paste text from TextBox1 into TexBox2
            QueueDelegate(VerifyResult);
        }

        private void VerifyResult()
        {
            if (ConfigurationSettings.Current.GetArgument("TextType") == "PasteBitmapImage")
            {
                Verifier.Verify(TestTextBox.SelectedText == "abc", "Selection matched", true);
            }
            else if (ConfigurationSettings.Current.GetArgument("TextType") == "PasteMultilinesInAcceptsReturnFalse")
            {
                Verifier.Verify(TestTextBoxAlt.Text == "abc xxx", "Only first line should be pasted." + TestTextBoxAlt.Text, true);
            }
            else
            {
                TestTextBoxAlt.SelectAll();
                Log("TextRange Expected [" + _trString + "]");

                TextSelection textSelection = Test.Uis.Utils.TextUtils.GetTextBoxSelection(TestTextBox);

                Log("TextRange Actual[" + XamlUtils.TextRange_GetXml(textSelection) + "]");
                Verifier.Verify(XamlUtils.TextRange_GetXml(textSelection) == _trString,
                    "TextRange.GetXaml matched.", true);
            }
            Logger.Current.ReportSuccess();
        }
    }

    #endregion ClipboardInTextBox

    #region ClipboardTestForSecondApp

    /// <summary>
    /// Second app for Testing clipboard - verifies that clipboard works across application
    /// data-driven scenarios.
    /// </summary>
    public class ClipboardTestForSecondApp : CustomTestCase
    {
        private UIElementWrapper _editBox1;
        private UIElementWrapper _editBox2;
        private string _getClipboardData;

        private string EditableTypeName
        {
            get
            {
                string result;

                result = Settings.GetArgument("EditableType");
                if (result == "") result = "RichTextBox";

                return result;
            }
        }
        
        private void BuildWindow()
        {
            Panel topPanel;
            FrameworkElement box1;
            FrameworkElement box2;

            topPanel = (Panel)ReflectionUtils.CreateInstanceOfType(Settings.GetArgument("ContainerType1")
                , new object[] {});
            topPanel.Background = Brushes.SkyBlue;

            box1 = TextEditableType.GetByName(EditableTypeName).CreateInstance();
            box2 = TextEditableType.GetByName(EditableTypeName).CreateInstance();
            box1.Height = box2.Height = 100;
            box1.Width = box2.Width = 250;
            box1.SetValue(TextBox.TextWrappingProperty, TextWrapping.Wrap);
            box2.SetValue(TextBox.TextWrappingProperty, TextWrapping.Wrap);
            box1.SetValue(TextBox.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Auto);
            box2.SetValue(TextBox.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Auto);
            box1.SetValue(TextBox.AcceptsReturnProperty, true);
            box2.SetValue(TextBox.AcceptsReturnProperty, true);
            box2.SetValue(Canvas.TopProperty, 120d);
            box1.Name = "tb1";
            box2.Name = "tb2";
            box1.SetValue(TextBox.FontFamilyProperty, new FontFamily("Microsoft Sans Serif")); //workaround for Regression_Bug4
            box2.SetValue(TextBox.FontFamilyProperty, new FontFamily("Microsoft Sans Serif")); //workaround for Regression_Bug4
            topPanel.Children.Add(box1);
            topPanel.Children.Add(box2);

            MainWindow.Content = topPanel;
        }

        /// <summary>Runs the test case</summary>
        public override void RunTestCase()
        {
            //Create log file for logging on second app.
            if (Test.Uis.IO.TextFileUtils.Exists("ClipboardTestLog.txt"))
                Test.Uis.IO.TextFileUtils.Delete("ClipboardTestLog.txt");
            Logger.Current.LogToFile("ClipboardTestLog.txt");
            BuildWindow();

            this.MainWindow.Width = 500;
            this.MainWindow.Height = 500;
            this.MainWindow.Left = 520;
            this.MainWindow.Title = "ClipboardTest Second App";

            QueueHelper.Current.QueueDelegate(new SimpleHandler(StartTest));
        }

        /// <summary>Start test action</summary>
        private void StartTest()
        {
            _editBox1 = new UIElementWrapper(ElementUtils.FindElement(MainWindow, "tb1"));
            _editBox2 = new UIElementWrapper(ElementUtils.FindElement(MainWindow, "tb2"));
            _editBox1.Element.Focus();
            SimplifyXmlString();

            //paste into textbox in second app
            RoutedCommand PasteCommand = ApplicationCommands.Paste;
            PasteCommand.Execute(null, _editBox1.Element);

            //verify result after paste then exit second app
            QueueHelper.Current.QueueDelegate(new SimpleHandler(VerifyResult));
        }

        /// <summary>Simpley xml string for comparison</summary>
        private void SimplifyXmlString()
        {
            _getClipboardData = Clipboard.GetDataObject().GetData(DataFormats.Xaml).ToString();

            int startIndexFront;
            int startIndexEnd;

            startIndexFront = _getClipboardData.IndexOf("xxx");
            _getClipboardData = _getClipboardData.Remove(0, startIndexFront);

            startIndexEnd = _getClipboardData.IndexOf("yyy") + 3;
            _getClipboardData = _getClipboardData.Remove(startIndexEnd, _getClipboardData.Length - startIndexEnd);

            if (_getClipboardData.Contains(" xml:space=\"preserve\""))
            {
                _getClipboardData = _getClipboardData.Replace(" xml:space=\"preserve\"", "");
            }
        }

        /// <summary>Verify resutl</summary>
        private void VerifyResult()
        {
            string actualString;
            actualString = _editBox1.XamlText;
            if (actualString.Contains("</mscorlib_System_1:String></Text.Text>")
                || actualString.Contains(" xml:space=\"preserve\""))
            {
                actualString = actualString.Replace("</mscorlib_System_1:String></Text.Text>", "");
                actualString = actualString.Replace(
                    "<Text.Text><?Mapping XmlNamespace=\"mscorlib_System_1ns\" ClrNamespace=\"System\" Assembly=\"mscorlib\" ?><mscorlib_System_1:String xmlns:mscorlib_System_1=\"mscorlib_System_1ns\">", "");
                actualString = actualString.Replace(" xml:space=\"preserve\"", "");
            }
            //Log("Expect text after pasted ["+getClipboardData+"]");
            //Log("Actual text in editBox2  ["+actualString+"]");
            Verifier.Verify(actualString == _getClipboardData, "Text matched.", true);

            Logger.Current.ReportSuccess();
        }
    }

    #endregion ClipboardTestForSecondApp

    #region CustomClipboardData

    /// <summary>
    /// Testing copy/paste and dragdrop with custom data
    /// data-driven scenarios.
    /// </summary>
    [Test(2, "Clipboard", "CustomClipboardData", MethodParameters = "/TestCaseType=CustomClipboardData /TestName=CustomClipboardData-RichTextBox", Disabled=true)]
    [TestOwner("Microsoft"), TestTactics("38"), TestLastUpdatedOn("July 10, 2006")]
    public class CustomClipboardData : CustomTestCase
    {
        #region Private Data

        private UIElementWrapper _editBox1;
        private UIElementWrapper _editBox2;
        private int _counter = 1;
        private bool _mySettingEvent;
        private MyMouseMove _myMouseMove = new MyMouseMove();
        private Rect _rc1;
        private Rect _rc2;
        private System.Windows.Point _p1;
        private System.Windows.Point _p2;        
        private DataObjectCopyingEventHandler _doc;
        private DataObjectSettingDataEventHandler _dos;
        private DataObjectPastingEventHandler _dop;
        private IDataObject _idataobject;
        private const  string customFormatName = "CustomData";

        #endregion

        #region Override Members

        /// <summary>Runs the test case</summary>
        public override void RunTestCase()
        {
            BuildWindow();
            this.MainWindow.Width = 500;
            this.MainWindow.Height = 500;
            Log("\n****************************************************************************\n" +
                "* To manually repro this test case append '/repro:true' to script command. *\n" +
                "* Select abc, copy, Ctrl+end, paste. Verify 999 is pasted.                 *\n" +
                "* Select bc, drag and drop into editbox2. Verify Hello World is dropped.   *\n" +
                "****************************************************************************");
            QueueHelper.Current.QueueDelegate(new SimpleHandler(StartTest));
        }

        #endregion

        #region Private Members

        private string EditableTypeName
        {
            get
            {
                string result;

                result = Settings.GetArgument("EditableType");
                if (result == "") result = "RichTextBox";

                return result;
            }
        }

        private void BuildWindow()
        {
            Panel topPanel;
            FrameworkElement box1;
            FrameworkElement box2;

            topPanel = (Panel)ReflectionUtils.CreateInstanceOfType(Settings.GetArgument("ContainerType1")
                , new object[] { });
            topPanel.Background = Brushes.SkyBlue;

            box1 = TextEditableType.GetByName(EditableTypeName).CreateInstance();
            box2 = TextEditableType.GetByName(EditableTypeName).CreateInstance();
            box1.Height = box2.Height = 100;
            box1.Width = box2.Width = 250;
            box1.SetValue(TextBox.TextWrappingProperty, TextWrapping.Wrap);
            box2.SetValue(TextBox.TextWrappingProperty, TextWrapping.Wrap);
            box1.SetValue(TextBox.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Auto);
            box2.SetValue(TextBox.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Auto);
            box1.SetValue(TextBox.AcceptsReturnProperty, true);
            box2.SetValue(TextBox.AcceptsReturnProperty, true);
            box2.SetValue(Canvas.TopProperty, 120d);
            box1.Name = "tb1";
            box2.Name = "tb2";
            if (EditableTypeName == "RichTextBox")
            {
                box1.SetValue(TextBox.FontSizeProperty, 24.0);
                box2.SetValue(TextBox.FontSizeProperty, 24.0);
            }
            box1.SetValue(TextBox.FontFamilyProperty, new FontFamily("Microsoft Sans Serif")); //workaround for Regression_Bug4
            box2.SetValue(TextBox.FontFamilyProperty, new FontFamily("Microsoft Sans Serif")); //workaround for Regression_Bug4
            topPanel.Children.Add(box1);
            topPanel.Children.Add(box2);
            MainWindow.Content = topPanel;
        }

        private void StartTest()
        {
            _editBox1 = new UIElementWrapper(ElementUtils.FindElement(MainWindow, "tb1"));
            _editBox2 = new UIElementWrapper(ElementUtils.FindElement(MainWindow, "tb2"));
            _editBox1.Element.Focus();
            _editBox1.Text = "abc";
            _editBox2.Text = "";
            _idataobject = new DataObject();
            _idataobject.SetData("789");

            // Attached event handler to allow changing data during operation
            _editBox1.Element.AddHandler(DataObject.CopyingEvent, new DataObjectCopyingEventHandler(myCopying), true);
            _editBox1.Element.AddHandler(DataObject.SettingDataEvent, new DataObjectSettingDataEventHandler(mySetting), true);
            _editBox1.Element.AddHandler(DataObject.PastingEvent, new DataObjectPastingEventHandler(myPasting),true);
            _editBox2.Element.AddHandler(DataObject.PastingEvent, new DataObjectPastingEventHandler(myPasting), true);

            // Testing DataObject Handlers
            _doc = new DataObjectCopyingEventHandler(myCopying);
            _dos = new DataObjectSettingDataEventHandler(mySetting);
            _dop = new DataObjectPastingEventHandler(myPasting);
            DataObject.AddCopyingHandler(_editBox1.Element, _doc);
            DataObject.AddSettingDataHandler(_editBox1.Element, _dos);
            DataObject.AddPastingHandler(_editBox1.Element, _dop);

            Log("Testing invalid scenarios for AddxxxEventHandler and RemomvexxxEventHandler...");
            QueueDelegate(TestingInvalidAddRemoveEventHandler);
        }

        /// <summary>Testing invalid scenarios for AddxxxEventHandler and RemomvexxxEventHandler...</summary>
        private void TestingInvalidAddRemoveEventHandler()
        {
            Log("-DataObjectCopyingEventArgs wil null parameter");
            try
            {
                DataObjectCopyingEventArgs doca = new DataObjectCopyingEventArgs(null, false);
                throw new ApplicationException("DataObjectCopyingEventArgs() accepted a null dataobject.");
            }
            catch(ArgumentNullException)
            {
                Log("DataObjectCopyingEventArgs() reject a null dataobject.");
            }
            Log("-DataObjectSettingDataEventArgs wil null parameter");
            try
            {
                DataObjectSettingDataEventArgs dosa = new DataObjectSettingDataEventArgs(null, "Text");
                throw new ApplicationException("DataObjectSettingDataEventArgs() accepted a null dataobject.");
            }
            catch(ArgumentNullException)
            {
                Log("DataObjectSettingDataEventArgs() reject a null dataobject.");
            }
            try
            {
                DataObjectSettingDataEventArgs dosa = new DataObjectSettingDataEventArgs(_idataobject, null);
                throw new ApplicationException("DataObjectSettingDataEventArgs() accepted a null format.");
            }
            catch(ArgumentNullException)
            {
                Log("DataObjectSettingDataEventArgs() reject a null format.");
            }
            Log("-DataObjectPastingEventArgs wil null parameter");
            try
            {
                DataObjectPastingEventArgs dopa = new DataObjectPastingEventArgs(null, false, "Text");
                throw new ApplicationException("DataObjectPastingEventArgs() accepted a null dataobject.");
            }
            catch(ArgumentNullException)
            {
                Log("DataObjectPastingEventArgs() reject a null dataobject.");
            }
            try
            {
                DataObjectPastingEventArgs dopa = new DataObjectPastingEventArgs(_idataobject, false, null);
                throw new ApplicationException("DataObjectPastingEventArgs() accepted a null format.");
            }
            catch(ArgumentNullException)
            {
                Log("DataObjectPastingEventArgs() reject a null format.");
            }
            try
            {
                // Invalid testing for DataFormatNotPresentOnDataObject
                DataObjectPastingEventArgs dopa = new DataObjectPastingEventArgs(_idataobject, false, "Xml");
                throw new ApplicationException("DataObjectPastingEventArgs() accepted DataFormat not present on DataObject.");
            }
            catch(ArgumentException)
            {
                Log("DataObjectPastingEventArgs() reject DataFormat not present on DataObject.");
            }
            try
            {
                DataObjectPastingEventArgs dopa = new DataObjectPastingEventArgs(_idataobject, false, "Text");
                // Set null value to DataObject
                dopa.DataObject = null;
                throw new ApplicationException("DataObjectPastingEventArgs() accepted null value.");
            }
            catch(ArgumentNullException)
            {
                Log("DataObjectPastingEventArgs() reject null value.");
            }
            Log("-Testing AddCopyingEventHandler with null parameter");
            try
            {
                DataObject.AddCopyingHandler(null, _doc);
                throw new ApplicationException("AddCopyingEventHandler() accepted a null sender.");
            }
            catch (ArgumentNullException)
            {
                Log("AddCopyingEventHandler() reject a null sender.");
            }
            try
            {
                DataObject.AddCopyingHandler(_editBox1.Element, null);
                throw new ApplicationException("AddCopyingEventHandler() accepted a null dataObjectCopyingEventHandler.");
            }
            catch (ArgumentNullException)
            {
                Log("AddCopyingEventHandler() reject a null dataObjectCopyingEventHandler.");
            }
            Log("-Testing AddSettingDataEventHandler with null parameter");
            try
            {
                DataObject.AddSettingDataHandler(null, _dos);
                throw new ApplicationException("AddSettingDataEventHandler() accepted a null sender.");
            }
            catch (ArgumentNullException)
            {
                Log("AddSettingDataEventHandler() reject a null sender.");
            }
            try
            {
                DataObject.AddSettingDataHandler(_editBox1.Element, null);
                throw new ApplicationException("AddSettingDataEventHandler() accepted a null dataObjectSettingDataEventHandler.");
            }
            catch (ArgumentNullException)
            {
                Log("AddSettingDataEventHandler() reject a null dataObjectSettingDataEventHandler.");
            }
            Log("-Testing AddPastingEventHandler with null parameter");
            try
            {
                DataObject.AddPastingHandler(null, _dop);
                throw new ApplicationException("AddPastingEventHandler() accepted a null sender.");
            }
            catch (ArgumentNullException)
            {
                Log("AddPastingEventHandler() reject a null sender.");
            }
            try
            {
                DataObject.AddPastingHandler(_editBox2.Element, null);
                throw new ApplicationException("AddPastingEventHandler() accepted a null dataObjectPastingEventHandler.");
            }
            catch (ArgumentNullException)
            {
                Log("AddPastingEventHandler() reject a null dataObjectPastingEventHandler.");
            }
            //Remove
            Log("-Testing RemoveCopyingEventHandler with null parameter");
            try
            {
                DataObject.RemoveCopyingHandler(null, _doc);
                throw new ApplicationException("RemoveCopyingEventHandler() accepted a null sender.");
            }
            catch (ArgumentNullException)
            {
                Log("RemoveCopyingEventHandler() reject a null sender.");
            }
            try
            {
                DataObject.RemoveCopyingHandler(_editBox1.Element, null);
                throw new ApplicationException("RemoveCopyingEventHandler() accepted a null dataObjectCopyingEventHandler.");
            }
            catch (ArgumentNullException)
            {
                Log("RemoveCopyingEventHandler() reject a null dataObjectCopyingEventHandler.");
            }
            Log("-Testing RemoveSettingDataEventHandler with null parameter");
            try
            {
                DataObject.RemoveSettingDataHandler(null, _dos);
                throw new ApplicationException("RemoveSettingDataEventHandler() accepted a null sender.");
            }
            catch (ArgumentNullException)
            {
                Log("RemoveSettingDataEventHandler() reject a null sender.");
            }
            try
            {
                DataObject.RemoveSettingDataHandler(_editBox1.Element, null);
                throw new ApplicationException("RemoveSettingDataEventHandler() accepted a null dataObjectSettingDataEventHandler.");
            }
            catch (ArgumentNullException)
            {
                Log("RemoveSettingDataEventHandler() reject a null dataObjectSettingDataEventHandler.");
            }
            Log("-Testing RemovePastingEventHandler with null parameter");
            try
            {
                DataObject.RemovePastingHandler(null, _dop);
                throw new ApplicationException("RemovePastingEventHandler() accepted a null sender.");
            }
            catch (ArgumentNullException)
            {
                Log("RemovePastingEventHandler() reject a null sender.");
            }
            try
            {
                DataObject.RemovePastingHandler(_editBox2.Element, null);
                throw new ApplicationException("RemovePastingEventHandler() accepted a null dataObjectPastingEventHandler.");
            }
            catch (ArgumentNullException)
            {
                Log("RemovePastingEventHandler() reject a null dataObjectPastingEventHandler.");
            }

            //Test removing handlers through DataObject
            DataObject.RemoveCopyingHandler(_editBox1.Element, _doc);
            DataObject.RemoveSettingDataHandler(_editBox1.Element, _dos);
            DataObject.RemovePastingHandler(_editBox1.Element, _dop);

            Log("Testing valid scenarios...");
            if (!Settings.GetArgumentAsBool("repro"))
            {
                KeyboardInput.TypeString("^a^c^{End}^v");                
                QueueDelegate(VerifyPaste);
            }
        }

        /// <summary>
        /// When doing cut or copy Copying event is fired.
        /// In this function, I verify DataObject data/clipboard data set correctly.
        /// Then I set new data(Xaml format) to DataObject for paste.
        /// Then I verify new data is set correct.
        /// </summary>
        private void myCopying(object sender, DataObjectCopyingEventArgs args)
        {
            Log("----inside copying event handler------");
            if (_counter == 1)
            {
                // Verify DataObject data/clipboard data
                Verifier.Verify(args.DataObject.GetData(DataFormats.Text).ToString() == _editBox1.Text, "Text matched [" +
                    args.DataObject.GetData(DataFormats.Text).ToString() + "]", true);

                string temp = args.DataObject.GetData(DataFormats.Xaml).ToString();
                Verifier.Verify(temp.Contains("abc</Run></Paragraph></Section>"), "Xml data isn't matched" + temp, true);
                Verifier.Verify(temp.Contains("FontFamily=\"Microsoft Sans Serif\""), "Xml should contain FontFamily", true);
                Verifier.Verify(temp.Contains("FontSize=\"24\""), "Xml should contain FontSize", true);

                Verifier.Verify(args.DataObject.GetData(DataFormats.UnicodeText).ToString() ==
                    _editBox1.Text, "UnicodeText matched [" + args.DataObject.GetData(DataFormats.UnicodeText).ToString() + "]", true);

                Verifier.Verify(args.DataObject.GetData(DataFormats.StringFormat).ToString() ==
                    _editBox1.Text, "StringFormat matched [" + args.DataObject.GetData(DataFormats.StringFormat).ToString() + "]", true);
            }
            
            // Set new data to clipboard for paste
            args.DataObject.SetData(DataFormats.Text, "aaa");
            // Set Xaml data for paste in RichTextBox, TextBox will paste with UnicodeText format
            args.DataObject.SetData(DataFormats.Xaml,
                "<Section xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xml:space=\"preserve\" xml:lang=\"en-US\" "+
                "FontFamily=\"Tahoma\" FontStyle=\"Normal\" FontWeight=\"Normal\" FontStretch=\"Normal\" FontSize=\"11\" "+
                "Foreground=\"#FF000000\" Typography.StandardLigatures=\"True\" Typography.ContextualLigatures=\"True\" "+
                "Typography.DiscretionaryLigatures=\"False\" Typography.HistoricalLigatures=\"False\" "+
                "Typography.AnnotationAlternates=\"0\" Typography.ContextualAlternates=\"True\" "+
                "Typography.HistoricalForms=\"False\" Typography.Kerning=\"True\" Typography.CapitalSpacing=\"False\" "+
                "Typography.CaseSensitiveForms=\"False\" Typography.StylisticSet1=\"False\" Typography.StylisticSet2=\"False\" "+
                "Typography.StylisticSet3=\"False\" Typography.StylisticSet4=\"False\" Typography.StylisticSet5=\"False\" "+
                "Typography.StylisticSet6=\"False\" Typography.StylisticSet7=\"False\" Typography.StylisticSet8=\"False\" "+
                "Typography.StylisticSet9=\"False\" Typography.StylisticSet10=\"False\" Typography.StylisticSet11=\"False\" "+
                "Typography.StylisticSet12=\"False\" Typography.StylisticSet13=\"False\" Typography.StylisticSet14=\"False\" "+
                "Typography.StylisticSet15=\"False\" Typography.StylisticSet16=\"False\" Typography.StylisticSet17=\"False\" "+
                "Typography.StylisticSet18=\"False\" Typography.StylisticSet19=\"False\" Typography.StylisticSet20=\"False\" "+
                "Typography.Fraction=\"Normal\" Typography.SlashedZero=\"False\" Typography.MathematicalGreek=\"False\" "+
                "Typography.EastAsianExpertForms=\"False\" Typography.Variants=\"Normal\" Typography.Capitals=\"Normal\" "+
                "Typography.NumeralStyle=\"Normal\" Typography.NumeralAlignment=\"Normal\" Typography.EastAsianWidths=\"Normal\" "+
                "Typography.EastAsianLanguage=\"Normal\" Typography.StandardSwashes=\"0\" Typography.ContextualSwashes=\"0\" "+
                "Typography.StylisticAlternates=\"0\"><Paragraph><Run>999</Run></Paragraph></Section>");
            args.DataObject.SetData(DataFormats.UnicodeText, "ccc");
            args.DataObject.SetData(DataFormats.StringFormat, "ddd");
            args.DataObject.SetData("MyFormat", "eee");
            args.DataObject.SetData(DataFormats.Rtf, "");           

            if (_counter == 1)
            {
                // Verify new data is set
                Verifier.Verify(args.DataObject.GetData(DataFormats.Text).ToString() == "aaa", "Text matched", true);
                Verifier.Verify(args.DataObject.GetData(DataFormats.Xaml).ToString().Contains("999"), "Xaml matched", true);
                Verifier.Verify(args.DataObject.GetData(DataFormats.UnicodeText).ToString() == "ccc", "UnicodeText matched", true);
                Verifier.Verify(args.DataObject.GetData(DataFormats.StringFormat).ToString() == "ddd", "StringFormat matched", true);
                Verifier.Verify(args.DataObject.GetData("MyFormat").ToString() == "eee", "MyFormat matched\n", true);
            }
            _counter++;
        }

        /// <summary>
        /// When doing cut or copy Setting event is fired.
        /// In this function, I verify DataObject is not null and format is corrected.
        /// </summary>
        private void mySetting(object sender, DataObjectSettingDataEventArgs args)
        {
            Log("----inside setting event handler------");
            _mySettingEvent = true;
            if ((args.Format == "Text" || args.Format == "UnicodeText" || args.Format == "Xaml" || args.Format == DataFormats.Rtf) && args.DataObject != null)
                Logger.Current.ReportResult(true, "Format is matched.", true);
            else
                Logger.Current.ReportResult(false, "Format is not matched.", true);
        }

        /// <summary>
        /// When doing paste or drop Pasting event is fired.
        /// In this function, I check null for FormatToApply.
        /// I verify new data is pasted.
        /// I reset new data for dragdrop operation
        /// I verify new data is set correctly for drag drop
        /// </summary>
        private void myPasting(object sender, DataObjectPastingEventArgs args)
        {
            Log("----inside pasting event handler------");
            // Test null for FormatToApply
            try
            {
                args.FormatToApply = null;
                throw new ArgumentNullException("DataObjectPastingEventArgs() accepted a null FormatToApply.");
            }
            catch
            {
                Log("DataObjectPastingEventArgs() reject a null FormatToApply.");
            }

            // Verify original DataObject
            Verifier.Verify((string)args.SourceDataObject.GetData("Text") == "aaa", "Expect Original DataObject [aaa]\nActual [" +
                args.SourceDataObject.GetData("Text") + "]", true);

            if (_counter == 2)
            {
                // verify new data is pasted
                Verifier.Verify(args.DataObject.GetData(DataFormats.Text).ToString() == "aaa", "Text matched", true);
                Verifier.Verify(args.DataObject.GetData(DataFormats.Xaml).ToString().Contains("999"), "Xaml matched", true);
                Verifier.Verify(args.DataObject.GetData(DataFormats.UnicodeText).ToString() == "ccc", "UnicodeText matched", true);
                Verifier.Verify(args.DataObject.GetData(DataFormats.StringFormat).ToString() == "ddd", "StringFormat matched", true);
                Verifier.Verify(args.DataObject.GetData("MyFormat").ToString() == "eee", "MyFormat matched\n", true);
            }
            // Set FormatToApply = Xml
            args.FormatToApply = DataFormats.Xaml;

            Verifier.Verify(args.DataObject.GetDataPresent("other") == false, "DataFormat other is not present on DataObject.", true);                        

            if (_counter == 3)
            {
                // reset new data for dragdrop
                DataObject dob = new DataObject();

                dob.SetData(DataFormats.Xaml, "888");
                dob.SetData(DataFormats.UnicodeText, "Hello World");
                args.DataObject = dob;

                // verify new data is set for drop operation
                Verifier.Verify(args.DataObject.GetData(DataFormats.Text).ToString() == "Hello World", "Text matched", true);
                Verifier.Verify(args.DataObject.GetData(DataFormats.Xaml).ToString().Contains("888"), "New Xml data is set and matched", true);                
            }
        }

        /// <summary>
        /// After doing Ctrl+v for paste, I check to make sure correct data is pasted.
        /// Then I start the drag drop operation to test the Pasting event.
        /// </summary>
        private void VerifyPaste()
        {
            //Verify text pasted then do dragdrop.
            Verifier.Verify(_mySettingEvent, "Setting event fired: [" + _mySettingEvent + "]", true);
            Verifier.Verify(_editBox1.XamlText.Contains("abc999"), "Pasted text match.\nActual editBox1 content[" + _editBox1.XamlText + "]", true);

            Log("DoDragDrop to test Pasting event...");
            _editBox1.Select(3, 2);
            QueueDelegate(MakeSelectionForDragDrop);
        }

        private void MakeSelectionForDragDrop()
        {
            Verifier.Verify(_editBox1.GetSelectedText(false, false) == "bc", "Selection matched. \n Actual selection[" +
                    _editBox1.GetSelectedText(false, false) + "]", true);

            // Find point on textBox.
            _rc1 = _editBox1.GetGlobalCharacterRect(3, LogicalDirection.Forward);
            _p1 = new System.Windows.Point(_rc1.Left + _rc1.Width / 2, _rc1.Top + _rc1.Height / 2);            
            _rc2 = _editBox2.GetGlobalCharacterRect(0, LogicalDirection.Forward);
            _p2 = new System.Windows.Point(_rc2.Left + _rc2.Width / 2, _rc2.Top + _rc2.Height / 2);            
            InputMonitorManager.Current.IsEnabled = false;

            MouseInput.MouseDragInOtherThread(_p1, _p2, true, TimeSpan.FromSeconds(1), new SimpleHandler(VerifyDragDrop), Dispatcher.CurrentDispatcher);            
        }

        /// <summary>
        /// After drop, I check to make sure correct data is dropped.
        /// Then I start the drag drop operation to test the Pasting event.
        /// Then I test Remove dataobject event handler
        /// Attached new copying and pasting event to test CancelCommand
        /// Then do drag drop to test the CancelCommand
        /// </summary>
        private void VerifyDragDrop()
        {
            Verifier.Verify(_editBox2.Text == "Hello World\r\n", "Drop text matched.\nActual content[" + _editBox2.Text + "]", true);            

            // Attached event and set CancelCommand to true for cut/copy/drag (Drop/paste are still working)
            _editBox2.Element.AddHandler(DataObject.CopyingEvent, new DataObjectCopyingEventHandler(testCancelCopying));
            _editBox1.Element.AddHandler(DataObject.CopyingEvent, new DataObjectCopyingEventHandler(testCancelCopying));

            // Attached event and set CancelCommand to true for paste/drop (cut/copy/drag are working)
            _editBox2.Element.AddHandler(DataObject.PastingEvent, new DataObjectPastingEventHandler(testCancelPasting));

            Log("Do DragDrop to test CancelCommand...");
            _editBox1.Select(3, 2);            
            _editBox1.Element.Focus();
            
            QueueDelegate(MakeSelectionForDragDrop2);
        }

        private void MakeSelectionForDragDrop2()
        {
            Verifier.Verify(_editBox1.GetSelectedText(false, false) == "99", "Selection matched : [" + _editBox1.GetSelectedText(false, false) + "]", true);

            MouseInput.MouseDragInOtherThread(_p1, _p2, true, TimeSpan.FromSeconds(1), new SimpleHandler(VerifyDragDrop2), Dispatcher.CurrentDispatcher);            
        }

        /// <summary>set CancelCommand to true for canceling copy/cut/drag</summary>
        private void testCancelCopying(object sender, DataObjectCopyingEventArgs args)
        {
            args.CancelCommand();
        }

        /// <summary>set CancelCommand to true for paste and drop verify drag drop is false</summary>
        private void testCancelPasting(object sender, DataObjectPastingEventArgs args)
        {
            args.CancelCommand();
            Verifier.Verify(!args.IsDragDrop, "IsDragDrop is canceled", true);
        }        

        /// <summary>
        /// Verify drop target content remains the same.
        /// And drag container remain the same.
        /// </summary>
        private void VerifyDragDrop2()
        {
            Verifier.Verify(_editBox2.XamlText.Contains("Hello World"),
                "CancelCommand disable drop.\n Actual content[" + _editBox2.XamlText + "]", true);
            Verifier.Verify(_editBox1.Text == "a999\r\n\r\n", "Text should not change: [a999\r\n\r\n] Actual[" + _editBox1.Text + "]", true);
            Log("Do cut to test CancelCommand...");
            QueueHelper.Current.QueueDelayedDelegate(new TimeSpan(4), new SimpleHandler(DoCut));
        }

        /// <summary>Do cut to test CacelCommand</summary>
        private void DoCut()
        {
            _editBox2.Select(1, 2);

            Verifier.Verify(_editBox2.GetSelectedText(false, false) == "H", "Selection to be cut : [H] Actual[" + _editBox2.GetSelectedText(false, false) + "]", true);
            RoutedCommand cutCommand = ApplicationCommands.Cut;
            cutCommand.Execute(null, _editBox2.Element);
            Log("Do paste to test CancelCommand...");
            QueueHelper.Current.QueueDelayedDelegate(new TimeSpan(4), new SimpleHandler(DoPaste));
        }

        /// <summary>Verify cut then do paste to test CancelCommand</summary>
        private void DoPaste()
        {
            Verifier.Verify(_editBox2.Text == "Hello World\r\n", "Text should remain the same after cut: [Hello World\r\n] Actual:[" +
                _editBox2.Text + "]", true);
            RoutedCommand pasteCommand = ApplicationCommands.Paste;
            pasteCommand.Execute(null, _editBox2.Element);
            QueueHelper.Current.QueueDelayedDelegate(new TimeSpan(4), new SimpleHandler(DoVerifyPaste));
        }

        /// <summary>Verify paste to test CancelCommand then exit app</summary>
        private void DoVerifyPaste()
        {
            Verifier.Verify(_editBox2.Text == "Hello World\r\n", "Text should remain the same after paste: [Hello World\r\n] Actual:[" +
                _editBox2.Text + "]", true);
            
            DataObject dataObject = new DataObject(customFormatName, new NonSerializableClass());
            Clipboard.Clear();
            try
            {
                Clipboard.SetDataObject(dataObject);
                if (Microsoft.Test.Diagnostics.SystemInformation.WpfVersion == Microsoft.Test.Diagnostics.WpfVersions.Wpf30)
                {
                    Logger.Current.ReportResult(false, "Custom NonSerializable Data should not be placed/accessed on/from the Clipboard in Wpf30");                    
                }
                else // when WpfVersion is Wpf35 or Wpf40
                {                    
                    Verifier.Verify(Clipboard.GetDataObject().GetDataPresent(customFormatName) == true, "Custom data should be accessible from the Clipboard on Version 3.5", false);
                    Verifier.Verify(Clipboard.GetDataObject().GetData(typeof(NonSerializableClass)) == null, "FormatName is registered, requesting for type should return Null", false);
                    try
                    {
                        object o = Clipboard.GetDataObject().GetData(customFormatName);
                        Logger.Current.ReportResult(false, "Requesting for NonSerializable object from Clipboard should throw an Exception");
                    }
                    catch (Exception)
                    {
                        //Currently 2 exception could be thrown - Com exception if the data passed in is    Button
                        //or a 'insufficient memory' exception in the case of a custom class
                        //this is a bug from the unmanaged side.
                        GlobalLog.LogStatus("Exception thrown on requesting NonSerializable format from Clipboard");
                    }
                }
            }
            catch (ArgumentException)
            {
                if (Microsoft.Test.Diagnostics.SystemInformation.WpfVersion == Microsoft.Test.Diagnostics.WpfVersions.Wpf30)
                {
                    GlobalLog.LogStatus("ArgumentException thrown on 3.0 when non serializable object placed on the clipboard");
                }
                else
                {
                    Logger.Current.ReportResult(false, "ArgumentException should be thrown on only 3.0; current Version:" + Microsoft.Test.Diagnostics.SystemInformation.WpfVersion.ToString());
                }
            }

            GlobalLog.LogStatus("End of test case.");
            Logger.Current.ReportSuccess();
        }


        #endregion

        #region Internal Members

        internal class NonSerializableClass
        {
            public NonSerializableClass() { }
        }

        #endregion
    }

    #endregion CustomClipboardData

    #region TestContainer

    /// <summary>
    /// TestContainer base for reuse in other cases.
    /// </summary>
    public abstract class TestContainer : CustomTestCase
    {
        /// <summary>EditableBox1 - this is reuse in other class</summary>
        public string EditableBox1;
        /// <summary>EditableBox2 - this is reuse in other class</summary>
        public string EditableBox2;
        /// <summary>box1 - this is reuse in other class</summary>
        public TextBoxBase box1;
        /// <summary>box2 - this is reuse in other class</summary>
        public TextBoxBase box2;

        /// <summary>Build the main window</summary>
        protected void BuildWindow()
        {
            Panel topPanel;
            string TopPanel;

            // For drag drop cross container, the containers can be TextBox and RichTextBox
            EditableBox1 = Settings.GetArgument("EditableBox1");
            EditableBox2 = Settings.GetArgument("EditableBox2");
            if (EditableBox1 == "")
                EditableBox1 = "RichTextBox";
            if (EditableBox2 == "")
                EditableBox2 = "RichTextBox";
            // topPanel can be any Canvas, StackPanel, DockPanel
            TopPanel = Settings.GetArgument("ContainerType1");
            if (TopPanel == "")
                TopPanel = "Canvas";
            topPanel = (Panel)ReflectionUtils.CreateInstanceOfType(TopPanel, new object[] {});
            topPanel.Background = Brushes.Lavender;

            // editBox1, editBox2 can be TextBox or RichTextBox
            box1 = TextEditableType.GetByName(EditableBox1).CreateInstance() as TextBoxBase;
            box2 = TextEditableType.GetByName(EditableBox2).CreateInstance() as TextBoxBase;
            box1.Height = box2.Height = 100;
            box1.Width = box2.Width = 250;
            if ((box1 is TextBox)&&(box2 is TextBox))
            {
                ((TextBox)box1).TextWrapping = ((TextBox)box2).TextWrapping = TextWrapping.Wrap;
            }
            box1.VerticalScrollBarVisibility = box2.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            box1.AcceptsReturn = box2.AcceptsReturn = true;
            box1.FontFamily = box2.FontFamily = new FontFamily("Microsoft Sans Serif");
            box1.FontSize = box2.FontSize = 11.0;
            box2.SetValue(Canvas.TopProperty, 101.0);
            // if topPanel is DockPanel I want to make sure the box1 and 2 are still in the same location.
            box1.SetValue(DockPanel.DockProperty, Dock.Top);
            box2.SetValue(DockPanel.DockProperty, Dock.Bottom);
            box1.Name = "tb1";
            box2.Name = "tb2";
            topPanel.Children.Add(box1);
            topPanel.Children.Add(box2);

            MainWindow.Content = topPanel;
        }

        /// <summary>Build the main window</summary>
        /// <param name="height">Height of editbox and location for editbox2</param>
        /// <param name="width">Width of editbox</param>
        /// <param name="fontSize">Font size of editBox</param>
        protected void BuildWindow(int height, int width, double fontSize)
        {
            BuildWindow();
            box1.Height = box2.Height = height;
            box1.Width = box2.Width = width;
            box2.SetValue(Canvas.TopProperty, (double)(height + 1));
            box1.FontSize = box2.FontSize = fontSize;
        }
    }

    #endregion TestContainer

    #region CutCopyPasteInTbRtbWithPropertySet

    /// <summary>
    /// Cut Copy Paste in TB, RTB with IsReadOnly, RlTb, AcceptsReturn, AcceptsDigit, MaxLength
    /// </summary>
    [Test(2, "Clipboard", "CutCopyPasteInTbRtbWithPropertySet1", MethodParameters = "/TestCaseType:CutCopyPasteInTbRtbWithPropertySet /Priority:1 /EditableBox1=TextBox")]
    [Test(3, "Clipboard", "CutCopyPasteInTbRtbWithPropertySet2", MethodParameters = "/TestCaseType:CutCopyPasteInTbRtbWithPropertySet /Priority:2 /EditableBox2=TextBox")]
    [Test(3, "Clipboard", "CutCopyPasteInTbRtbWithPropertySet3", MethodParameters = "/TestCaseType:CutCopyPasteInTbRtbWithPropertySet /Priority:3")]
    [TestOwner("Microsoft"), TestTactics("39,40,41,42"), TestWorkItem("43"), TestBugs("13,300,11,12,10,8"), TestLastUpdatedOn("July 11, 2006")]
    public class CutCopyPasteInTbRtbWithPropertySet : TestContainer
    {
        private UIElementWrapper _editBox1;              // Editable box1
        private UIElementWrapper _editBox2;              // Editable box2
        private TestCaseData _testCaseData;
        private int _testCaseIndex = 0;
        private int[] _testFailed;                       // Array of all test that failed
        private bool _isTestFailed;                      // To remember if test failed
        private string _failID;                                  // Name of test case that failed
        private int _endIndex;                           // End of test case index

        struct TestCaseData
        {
            public string TestType;
            public string TestString;
            public string Action;
            public string ExpectString;
            public int Priority;

            public TestCaseData(string testType, string testString, string action, string expectString, int priority)
            {
                this.TestType = testType;
                this.TestString = testString;
                this.Action = action;
                this.ExpectString = expectString;
                this.Priority = priority;
            }

            public static TestCaseData[] Cases = new TestCaseData[] {                
                // rtb -> tb 41 : 0-6
                new TestCaseData("PasteBitmapImage", "abc 123", "^a", "0", 2), //Regression_Bug8
                new TestCaseData("CutIsReadOnly", "abc def{Enter}123 456", "^x", "", 2), // Regression_Bug11
                new TestCaseData("RlTb", "abc def{Enter}123 456", "^c", "abc def\r\n123 456\r\n", 2),
                new TestCaseData("CopyIsReadOnly", "abc def{Enter}123 456", "^c", "abc def\r\n123 456\r\n", 2),
                new TestCaseData("PasteIsReadOnly", "abc def{Enter}123 456", "^c", "", 2),
                new TestCaseData("MaxLength", "abc def{Enter}123 456", "^c", "abc", 2),         // For TextBox property only
                new TestCaseData("AcceptsReturnFalse", "abc 123{Enter}def 456", "^c", "abc 123", 2), // Regression_Bug10
                
                // tb -> rtb 39 : 7 - 10
                new TestCaseData("PasteBitmapImage", "abc 123", "", "1", 1), //Regression_Bug8
                new TestCaseData("CutIsReadOnly", "abc def{Enter}123 456", "^x", "\r\n", 1), // Regression_Bug11
                new TestCaseData("PasteIsReadOnly", "abc def{Enter}123 456", "^c", "\r\n", 1),
                new TestCaseData("AcceptsReturnFalse", "abc 123{Enter}def 456", "^c", "abc 123\r\n", 1), // Regression_Bug12, Regression_Bug13

                // rtb -> rtb 42 : 11 -16
                new TestCaseData("PasteBitmapImage", "abc 123", "", "1", 3), //Regression_Bug8
                new TestCaseData("CutIsReadOnly", "abc def{Enter}123 456", "^x", "\r\n", 3), // Regression_Bug11
                new TestCaseData("RlTb", "abc def{Enter}123 456", "^c", "abc def\r\n123 456\r\n\r\n", 3),
                new TestCaseData("CopyIsReadOnly", "abc def{Enter}123 456", "^c", "abc def\r\n123 456\r\n\r\n", 3),
                new TestCaseData("PasteIsReadOnly", "abc def{Enter}123 456", "^c", "\r\n", 3),
                new TestCaseData("AcceptsReturnFalse", "abc 123{Enter}def 456", "^c", "abc 123\r\ndef 456\r\n\r\n", 3), // Regression_Bug300 (Might be by design)
            };
        }
        
        /// <summary>Override RunTestCase</summary>
        public override void RunTestCase()
        {
            BuildWindow();            
            this.MainWindow.Width = 300;
            this.MainWindow.Height = 225;
            _editBox1 = new UIElementWrapper(ElementUtils.FindElement(MainWindow, "tb1"));
            _editBox2 = new UIElementWrapper(ElementUtils.FindElement(MainWindow, "tb2"));
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
                    Logger.Current.ReportSuccess();
            }
        }

        private void StartTest()
        {
            // Reset Boxs to original state
            if (box1 is TextBox)
            {
                ((TextBox)box1).Clear();
            }
            else
            {
                ((RichTextBox)box1).Document = new FlowDocument(new Paragraph(new Run()));
            }

            if (box2 is TextBox)
            {
                ((TextBox)box2).Clear();
            }
            else
            {
                ((RichTextBox)box2).Document = new FlowDocument(new Paragraph(new Run()));
            }
            
            if (_editBox2.Element is TextBox)
            {
                _editBox2.Element.SetValue(TextBox.MaxLengthProperty, 200);
            }

            box1.IsReadOnly = box2.IsReadOnly = false;
            box1.FlowDirection = box2.FlowDirection = FlowDirection.LeftToRight;
            box1.AcceptsReturn = box2.AcceptsReturn = true;

            _testCaseData = TestCaseData.Cases[_testCaseIndex];
            Log("*******Running new test case:[" + _testCaseIndex + "]*******");
            Log("Test Type :[" + _testCaseData.TestType + "]");
            Log("Test EditableBox1 is:[" + EditableBox1 + "]");
            Log("Test EditableBox2 is:[" + EditableBox2 + "]");
            
            _editBox1.Element.Focus();

            QueueDelegate(TypeTestString);
        }

        private void TypeTestString()
        {
            KeyboardInput.TypeString(_testCaseData.TestString + "^a");

            QueueDelegate(TestAction);
        }

        private void TestAction()
        {
            switch (_testCaseData.TestType)
            {
                case "PasteBitmapImage":
                    System.Drawing.Bitmap myBitmap;
                    myBitmap = new System.Drawing.Bitmap(100, 100);                    
                    Clipboard.SetDataObject(myBitmap);
                    IDataObject dobj = Clipboard.GetDataObject();
                    break;
                case "RlTb": 
                    box1.FlowDirection = box2.FlowDirection = FlowDirection.RightToLeft; 
                    break;
                case "AcceptsReturnFalse": 
                    box2.AcceptsReturn = false; 
                    break;
                case "CutIsReadOnly": 
                    box1.IsReadOnly = true; 
                    break;
                case "CopyIsReadOnly": 
                    box1.IsReadOnly = true; 
                    break;
                case "PasteIsReadOnly": 
                    box1.IsReadOnly = box2.IsReadOnly = true; 
                    break;
                case "MaxLength": 
                    _editBox2.Element.SetValue(TextBox.MaxLengthProperty, 3); 
                    break;
            }

            QueueDelegate(DoAction);
        }

        private void DoAction()
        {
            KeyboardInput.TypeString(_testCaseData.Action);
            QueueDelegate(DoPaste);
        }

        private void DoPaste()
        {
            _editBox2.Element.Focus();
            KeyboardInput.TypeString("^v");

            QueueDelegate(VerifyResult);
        }

        private void VerifyResult()
        {
            string str = _editBox2.Text;
                 
            if (_testCaseData.TestType == "PasteBitmapImage")
            {
                if (_editBox1.Element is TextBox && _editBox2.Element is TextBox)
                    _testCaseData.ExpectString = "abc 123";
                str = TextOMUtils.EmbeddedObjectCountInRange(new TextRange(_editBox2.Start, _editBox2.End)).ToString();
            }

            FailIfStringNotEqual(_testCaseData.ExpectString, str, "Content is not matched.");
            Clipboard.Clear();
            Verifier.Verify(!Clipboard.GetDataObject().GetDataPresent("Text"), "Clipboard should be empty.");

            _testCaseIndex++;
            RunCase();
        }

        private bool FailIfStringNotEqual(string ExpectedString, string ActualString, string Reason)
        {
            // If string are not equal, fail the case
            if (ExpectedString != ActualString)
            {
                Logger.Current.ReportResult(false, Reason + "\nExpect [" + ExpectedString + "]\nActual [" + ActualString + "]");
                _testFailed[_testCaseIndex] = -1;
                _isTestFailed = true;
            }
            return true;
        }
    }

    #endregion CutCopyPasteInTbRtbWithPropertySet

    /// <summary>Behavior for data transfer event handlers.</summary>
    enum DataTransferBehavior
    {
        /// <summary>Add a new format to the data object.</summary>
        AddFormat,
        /// <summary>Remove an existing format from the data object.</summary>
        RemoveFormat,
        /// <summary>Modify the selected format in the operation.</summary>
        /// <remarks>This only applies to Pasting, when selecting a format.</remarks>
        ModifyFormat,
        /// <summary>Modify the content of a format in the data object.</summary>
        ModifyFormatContent,
        /// <summary>Cancel the operation.</summary>
        CancelOperation,
        /// <summary>Throw an exception.</summary>
        ThrowException,
        /// <summary>Do nothing.</summary>
        Nothing,
    }

    /// <summary>
    /// Runs all interesting data transfer extensibility tests..
    /// </summary>
    [Test(3, "Clipboard", "DataTransferExtensibilityTest", MethodParameters = "/TestCaseType=DataTransferExtensibilityTest /InputMonitorEnabled:False", Timeout=240)]
    [TestOwner("Microsoft"), TestWorkItem("10"), TestTactics("44"), TestBugs("14")]
    public class DataTransferExtensibilityTest: ManagedCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Reads a combination and evalutes whether it should be executed.</summary>
        protected override bool DoReadCombination(System.Collections.Hashtable values)
        {
            bool result;

            result = base.DoReadCombination(values);

            // ModifyFormat only makes sense in the context of pasting.
            result = result && (
                _handlerBehavior != DataTransferBehavior.ModifyFormat ||
                _extensibility.Value == DataTransferExtensibility.Pasting);

            // There is no way to remove a format from a data object.
            // Implement and re-enable when Regression_Bug14 is fixed.
            result = result && _handlerBehavior != DataTransferBehavior.RemoveFormat;

            return result;
        }

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            DependencyObject target;
            Delegate handler;

            _element = _editableType.CreateInstance();
            _wrapper = new UIElementWrapper(_element);
            _parent = new DockPanel();
            _fireCount = 0;

            if (!_isHandlerValid || !_isTargetValid)
            {
                HandleInvalidCase();
                NextCombination();
                return;
            }

            _parent.Children.Add(_element);
            TestElement = _parent;

            // Attach the event handler to the extensibility point
            // under test.
            target = (_attachDirectlyOnControl)? _element : _parent;
            handler = _extensibility.CreateDelegate(EventFired);
            _extensibility.AddHandler(target, handler);
            if (_removeHandler)
            {
                _extensibility.RemoveHandler(target, handler);
            }

            QueueDelegate(AfterLayout);
        }

        /// <summary>
        /// Method called to handle an addition / removal invocation 
        /// that is expected to fail.
        /// </summary>
        private void HandleInvalidCase()
        {
            DependencyObject target;
            Delegate handler;

            // To fail, either the target or the handler need to be null.
            System.Diagnostics.Trace.Assert(!_isTargetValid || !_isHandlerValid);

            // Create the arguments for the invocation.
            target = (_isTargetValid)? new TextBox() : null;
            handler = (_isHandlerValid)? _extensibility.CreateDelegate(EventFired) : null;

            try
            {
                if (_removeHandler)
                {
                    Log("Removing handler...");
                    _extensibility.RemoveHandler(target, handler);
                }
                else
                {
                    Log("Adding handler...");
                    _extensibility.AddHandler(target, handler);
                }
                throw new ApplicationException(
                    "Exception expected for invalid operation, but none thrown.");
            }
            catch(SystemException)
            {
                Log("Exception thrown as expected.");
            }
        }

        private void AfterLayout()
        {
            if (_useDragDrop)
            {
            }
            else
            {
                _element.Focus();
                _wrapper.Text = s_originalText;
                _wrapper.SelectAll();

                // Perform the transfer. If the handler is expected to
                // throw an exception, we'll try again and ensure that
                // a later operation *can* succeed.
                try
                {
                    _exceptionThrown = false;
                    PerformTransfer();
                }
                catch(Exception)
                {
                    if (_handlerBehavior != DataTransferBehavior.ThrowException)
                    {
                        throw;
                    }
                    _exceptionThrown = true;
                    PerformTransfer();
                }
            }
            QueueDelegate(NextCombination);
        }

        private void PerformTransfer()
        {
            Clipboard.Clear();

            _fireCount = 0;
            ApplicationCommands.Copy.Execute(null, _element);
            VerifyCopyOperation(Clipboard.GetDataObject());

            if (_editableType.IsPassword)
            {
                Log("Manually copying data to clipboard for PasswordBox, " +
                    "to exercise Pasting paths...");
                Clipboard.SetDataObject(s_originalText);
            }

            _fireCount = 0;
            ApplicationCommands.Paste.Execute(null, _element);
            VerifyPasteOperation();
        }
   
        /// <summary>Event handler for all extensibility events.</summary>
        private void EventFired(object sender, DataObjectEventArgs args)
        {
            // Event-specific arguments. These will be null if the call
            // was made for another event.
            DataObjectCopyingEventArgs copyingArgs;
            DataObjectPastingEventArgs pastingArgs;
            DataObjectSettingDataEventArgs settingArgs;

            copyingArgs = args as DataObjectCopyingEventArgs;
            pastingArgs = args as DataObjectPastingEventArgs;
            settingArgs = args as DataObjectSettingDataEventArgs;

            _fireCount++;

            Verifier.Verify(args.IsDragDrop == _useDragDrop,
                "IsDragDrop (" + args.IsDragDrop + ") is consistent with " +
                "transfer method.", false);

            // Because the point of this test is to exercise customization of
            // system behavior, the verification oracle is tightly coupled to
            // what kind of changes were done. Also, because we are not testing
            // the customization test code, we use hard-coded values for content.
            //
            // The general design then is as follows:
            // - For a given HandlerBehavior / Extensibility pair, there is a
            //   customization of behavior that should be exercised. This 
            //   method figures out what to do and does it.
            // - To verify that the customization propagates correctly, we
            //   verify in VerifyCopyOperation and VerifyPasteOperation. The
            //   verification is tightly coupled to the customization done
            //   here, so there is also code to figure out what the right
            //   HandlerBehavior / Extensibility pair block to execute
            //   should be.

            switch(_handlerBehavior)
            {
                case DataTransferBehavior.AddFormat:
                    switch (_extensibility.Value)
                    {
                        // For copying and setting events, we simply add a new format
                        // (in the correct arguments instance).
                        case DataTransferExtensibility.Copying:
                            copyingArgs.DataObject.SetData(PrivateFormatName, PrivateFormatContent);
                            break;
                        case DataTransferExtensibility.Setting:
                            settingArgs.DataObject.SetData(PrivateFormatName, PrivateFormatContent);
                            break;
                        case DataTransferExtensibility.Pasting:
                            // Cannot add format to a frozen DataObject.
                            try
                            {
                                pastingArgs.DataObject.SetData(DataFormats.Xaml, SampleXaml);
                                throw new ApplicationException(
                                    "Expected an exception for setting data on Pasting DataObject, none thrown.");
                            }
                            catch(SystemException)
                            {
                                Log("Exception thrown as expected for setting data object value.");
                            }
                            break;
                    }
                    break;
                case DataTransferBehavior.CancelOperation:
                    bool shouldCancel;

                    // If we are setting multiple formats, try canceling only plain text.
                    shouldCancel = 
                        _extensibility.Value == DataTransferExtensibility.Setting &&
                        settingArgs.Format == DataFormats.Text;

                    // For all other events, cancel everything.
                    shouldCancel = shouldCancel ||
                        _extensibility.Value != DataTransferExtensibility.Setting;

                    if (shouldCancel)
                    {
                        args.CancelCommand();
                        Verifier.Verify(args.CommandCancelled, 
                            "CommandCancelled [" + args.CommandCancelled + "] after " +
                            "CancelCommand invocation.", true);
                    }
                    break;
                case DataTransferBehavior.ModifyFormat:
                    System.Diagnostics.Trace.Assert(_extensibility.Value == DataTransferExtensibility.Pasting);

                    try
                    {
                        Log("Setting the format to apply to non-existant format [" + PrivateFormatName + "].");
                        pastingArgs.FormatToApply = PrivateFormatName;
                        throw new ApplicationException(
                            "Expecting exception for non-existant format, none thrown.");
                    }
                    catch(SystemException)
                    {
                        Log("Expected exception thrown for non-supported format.");
                    }

                    // Set a valid DataObject / FormatToApply pair.
                    pastingArgs.DataObject = new DataObject(SampleText);
                    pastingArgs.FormatToApply = DataFormats.Text;
                    break;
                case DataTransferBehavior.ModifyFormatContent:
                    switch (_extensibility.Value)
                    {
                        case DataTransferExtensibility.Copying:
                            copyingArgs.DataObject.SetData(DataFormats.Text, SampleText);
                            break;
                        case DataTransferExtensibility.Pasting:
                            if (pastingArgs.FormatToApply == DataFormats.UnicodeText)
                            {
                                pastingArgs.DataObject = new DataObject(DataFormats.UnicodeText, SampleText);
                            }
                            else if (pastingArgs.FormatToApply == DataFormats.Xaml)
                            {
                                pastingArgs.DataObject = new DataObject(DataFormats.Xaml, SampleXaml);
                            }
                            break;
                        case DataTransferExtensibility.Setting:
                            Verifier.Verify(!settingArgs.CommandCancelled,
                                "SettingData CommandCancelled [" + settingArgs.CommandCancelled + 
                                "] is false before being called.");
                            settingArgs.DataObject.SetData(settingArgs.Format, SampleXaml);
                            settingArgs.CancelCommand();
                            break;
                    }
                    break;
                case DataTransferBehavior.RemoveFormat:
                    switch (_extensibility.Value)
                    {
                        case DataTransferExtensibility.Copying:
                            break;
                        case DataTransferExtensibility.Pasting:
                            break;
                        case DataTransferExtensibility.Setting:
                            break;
                    }
                    break;
                case DataTransferBehavior.ThrowException:
                    if (!_exceptionThrown)
                    {
                        throw new Exception("Throwing exception in event callback.");
                    }
                    else
                    {
                        Log("Exception already thrown - skipping for re-verification.");
                    }
                    break;
                case DataTransferBehavior.Nothing:
                    // Nothing to do here.
                    break;
            }
        }

        private void VerifyCopyOperation(IDataObject targetObject)
        {
            // PasswordBox controls are a trivial case.
            if (_element is PasswordBox)
            {
                Verifier.Verify(_fireCount == 0,
                    "No extensibility events fire for PasswordBox.", true);
                return;
            }

            // If the event was removed, it should never have been called.
            if (_removeHandler)
            {
                Verifier.Verify(_fireCount == 0,
                    "No extensibility events fire when handlers are removed.", true);
                return;
            }

            // If we are testing Pasting events, the only thing to test is that
            // we have not fired at this point.
            if (_extensibility.Value == DataTransferExtensibility.Pasting)
            {
                Verifier.Verify(_fireCount == 0,
                    "No extensibility Pasting events fire during a copy operation.", true);
                return;
            }

            System.Diagnostics.Trace.Assert(_handlerBehavior != DataTransferBehavior.ModifyFormat,
                "ModifyFormat should never be verified after Copying - it's a Pasting extensibility mechanism.");

            switch (this._extensibility.Value)
            {
                case DataTransferExtensibility.Copying:
                    switch(_handlerBehavior)
                    {
                        case DataTransferBehavior.AddFormat:
                            VerifyFormatContentContains(targetObject, PrivateFormatName, PrivateFormatContent);
                            break;
                        case DataTransferBehavior.CancelOperation:
                            Log("Formats on clipboard [" + string.Join(",", targetObject.GetFormats()) + "]");
                            Verifier.Verify(targetObject.GetFormats().Length == 0,
                                "There is nothing on the clipboard if Copy is cancelled.", true);
                            break;
                        case DataTransferBehavior.ModifyFormatContent:
                            VerifyFormatContentContains(targetObject, DataFormats.Text, SampleText);
                            break;
                        case DataTransferBehavior.RemoveFormat:
                            Verifier.VerifyValue("available clipboard format count",
                                0, targetObject.GetFormats(false).Length);
                            break;
                        case DataTransferBehavior.ThrowException:
                        case DataTransferBehavior.Nothing:
                            // After the first exception thrown, we run again and expect
                            // regular behavior.
                            VerifyFormatContentContains(targetObject, DataFormats.Text, s_originalText);
                            break;
                    }
                    break;
                case DataTransferExtensibility.Setting:
                    switch(_handlerBehavior)
                    {
                        case DataTransferBehavior.AddFormat:
                            VerifyFormatContentContains(targetObject, PrivateFormatName, PrivateFormatContent);
                            break;
                        case DataTransferBehavior.CancelOperation:
                            VerifyFormatMissing(targetObject, DataFormats.Text);
                            break;
                        case DataTransferBehavior.ModifyFormatContent:
                            VerifyFormatContentContains(targetObject, DataFormats.UnicodeText, SampleXaml);
                            if (targetObject.GetDataPresent(DataFormats.Xaml))
                            {
                                VerifyFormatContentContains(targetObject, DataFormats.Xaml, SampleXaml);
                            }
                            break;
                        case DataTransferBehavior.RemoveFormat:
                            break;
                        case DataTransferBehavior.ThrowException:
                        case DataTransferBehavior.Nothing:
                            // After the first exception thrown, we run again and expect
                            // regular behavior.
                            VerifyFormatContentContains(targetObject, DataFormats.Text, s_originalText);
                            break;
                    }
                    break;
            }
        }

        private void VerifyPasteOperation()
        {
            // If the event was removed, it should never have been called.
            if (_removeHandler)
            {
                Verifier.Verify(_fireCount == 0,
                    "No extensibility events fire when handlers are removed.", true);
                return;
            }

            if (_extensibility.Value != DataTransferExtensibility.Pasting)
            {
                Verifier.Verify(_fireCount == 0, "No events fire on Pasting.", true);
                return;
            }

            switch (_handlerBehavior)
            {
                case DataTransferBehavior.CancelOperation:
                    break;
                case DataTransferBehavior.ModifyFormat:
                    Verifier.Verify(_wrapper.Text.Contains(SampleText),
                        "Text contains overriden text: " + _wrapper.Text, true);
                    break;
                case DataTransferBehavior.ModifyFormatContent:
                    if (_editableType.SupportsParagraphs)
                    {
                        Verifier.Verify(_wrapper.Text.Contains(SampleText),
                            "Control has overwritten sample text: " + _wrapper.Text, true);
                    }
                    else
                    {
                        Verifier.VerifyValue("pasted content", SampleText, _wrapper.Text);
                    }
                    break;
                case DataTransferBehavior.RemoveFormat:
                    break;
                case DataTransferBehavior.AddFormat:
                case DataTransferBehavior.ThrowException:
                case DataTransferBehavior.Nothing:
                    // Adding format on pasting is not supported, so we expect regular behavior.
                    // After the first exception thrown, we run again and expect
                    // regular behavior.
                    if (_editableType.SupportsParagraphs)
                    {
                        Verifier.Verify(_wrapper.Text.Contains(s_originalText),
                            "Control has overwritten sample text: " + _wrapper.Text, true);
                    }
                    else
                    {
                        Verifier.VerifyValue("pasted content", s_originalText, _wrapper.Text);
                    }
                    break;
            }
        }

        private void VerifyFormatContentContains(IDataObject dataObject, string formatName, string expectedContent)
        {
            string content;
            bool contains;

            content = dataObject.GetData(formatName).ToString();
            contains = content.Contains(expectedContent);
            if (!contains)
            {
                Log("Content for format " + formatName + ": [" + content + "]");
                Log("Expected content within: [" + expectedContent + "]");
                throw new Exception("Unable to find expected content " + expectedContent +
                    " in content for format " + formatName);
            }
        }

        private void VerifyFormatMissing(IDataObject dataObject, string format)
        {
            string[] formats;

            formats = dataObject.GetFormats(false);
            Verifier.Verify(Array.IndexOf(formats, format) != -1,
                "Format " + format + " is not in raw formats of data object.", true);
        }

        #endregion Main flow.

        #region Private fields.

        private FrameworkElement _element;
        private UIElementWrapper _wrapper;
        private DockPanel _parent;
        private bool _exceptionThrown;
        private int _fireCount;
        private static DataFormat s_privateFormat = DataFormats.GetDataFormat(PrivateFormatName);
        private const string PrivateFormatName = "TestPrivateClipboardFormat";
        private const string PrivateFormatContent = "PrivateFormatContent";
        private static string s_originalText = TextScript.Latin.Sample;

        /// <summary>Sample text used when extending.</summary>
        private const string SampleText = "xaml text";
        /// <summary>Sample text used when extending (in XAML format).</summary>
        private const string SampleXaml = 
            "<Section xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'><Paragraph>" + 
            SampleText + "</Paragraph></Section>";

        /// <summary>Whether events should be attached directly.</summary>
        private bool _attachDirectlyOnControl = false;

        private DataTransferBehavior _handlerBehavior =0;

        /// <summary>Whether the handler to attach is valid.</summary>
        private bool _isHandlerValid = false;

        /// <summary>Whether the target to attach to is valid.</summary>
        private bool _isTargetValid = false;

        /// <summary>What control we are working on.</summary>
        private TextEditableType _editableType = null;

        /// <summary>Event under test.</summary>
        private DataTransferExtensibilityData _extensibility = null;

        /// <summary>Whether handlers should be removed after being set.</summary>
        private bool _removeHandler = false;

        /// <summary>Whether drag/drop is used (as opposed to keyboard).</summary>
        private bool _useDragDrop = false;

        #endregion Private fields.
    }

    /// <summary>
    /// Test pasting with invalid clipboard data, doesnt thrown any exception
    /// </summary>
    [Test(2, "Clipboard", "InvalidClipboardTest", MethodParameters = "/TestCaseType=InvalidClipboardTest")]
    [TestOwner("Microsoft"), TestTactics("45"), TestBugs("123")]
    public class InvalidClipboardTest : ManagedCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            _rtb = new RichTextBox();
            TestElement = _rtb;

            QueueDelegate(SetFocus);
        }

        private void SetFocus()
        {
            _rtb.Focus();
            SetInvalidClipboardData();

            QueueDelegate(VerifyPaste);
        }

        private void SetInvalidClipboardData()
        {
            if (_testFormat == DataFormats.Rtf)
            {
                _dataObject = new DataObject();
                _dataObject.SetData(DataFormats.Rtf, _invalidData);                
                Clipboard.SetDataObject(_dataObject);
            }
            else if (_testFormat == DataFormats.Xaml)
            {
                _dataObject = new DataObject();
                _dataObject.SetData(DataFormats.Xaml, _invalidData);
                Clipboard.SetDataObject(_dataObject);
            }
            else if (_testFormat == DataFormats.XamlPackage)
            {
                _dataObject = new DataObject();
                _dataObject.SetData(DataFormats.XamlPackage, _invalidData);
                Clipboard.SetDataObject(_dataObject);
            }
        }

        private void VerifyPaste()
        {
            KeyboardInput.TypeString("^v");
            //Expect no exceptions after paste

            QueueDelegate(NextCombination);
        }
        #endregion Main flow.

        #region Private fields

        private string _testFormat=string.Empty;
        private string _invalidData=string.Empty;
        private RichTextBox _rtb;
        private DataObject _dataObject;

        #endregion Private fields
    }

    /// <summary>
    /// Editing: Trying to retrive an Avalon object set on the Clipboard raises an ambiguous exception and too late.
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("28"), TestBugs("387")]
    public class RegressionTest_Regression_Bug387 : CombinedTestCase
    {
        /// <summary>start to run the test.</summary>
        public override void RunTestCase()
        {            
            DataObject dobj = new DataObject(new Button());
            try
            {
                
                Clipboard.SetDataObject(dobj);
                if (SystemInformation.WpfVersion == WpfVersions.Wpf30)
                {
                    throw new Exception("We did not get the expected ArgumentException!");
                }
            }
            catch (ArgumentException e)
            {
                Log("Exception message[" + e.Message + "]");
            }
            EndTest();
           
        }
    }
}
