// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/forms/BVT/DataTransfer/EditCommand/EditCommand.cs $")]

namespace DataTransfer
{
    using System;
    using System.Collections;
    using System.ComponentModel.Design;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Markup;

    using Test.Uis.Data;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;
    using Test.Uis.TestTypes;
    using System.Text;
    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    #region EditCommandText

    /// <summary>
    /// verifies that all valid commands defined in TestCaseAction work in TextBox and RichTextBox
    /// data-driven scenarios.
    /// TC 71 for TextBox, RichTextBox, and their subclass
    /// </summary>
    [Test(2, "Editor", "EditCommandText", MethodParameters = "/TestCaseType=EditCommandText /Pri=1", Timeout=120)]
    [TestOwner("Microsoft"), TestWorkItem(""), TestTactics("71"), TestBugs("32,473")]
    public class EditCommandText : ManagedCombinatorialTestCase
    {
        #region struct

        private struct MyTestCaseData
        {
            public TestCaseAction TestAction;
            public int StartSelection;
            public int EndSelection;

            public MyTestCaseData(TestCaseAction testAction, int startSelection, int endSelection)
            {
                this.TestAction = testAction;
                this.StartSelection = startSelection;
                this.EndSelection = endSelection;
            }

            public static MyTestCaseData[] Cases = new MyTestCaseData[] {
                new MyTestCaseData(TestCaseAction.CutE, 1, 9),
                new MyTestCaseData(TestCaseAction.PasteE, 0, 0),
                new MyTestCaseData(TestCaseAction.CopyE, 1, 9),
                new MyTestCaseData(TestCaseAction.SelectAllE, 1, 9),
                new MyTestCaseData(TestCaseAction.DocumentEndEExtendDocumentHomeE, 1, 9),
                new MyTestCaseData(TestCaseAction.DocumentHomeEExtendDocumentEndE, 1, 9),
                new MyTestCaseData(TestCaseAction.DocumentEndEDocumentHomeE, 1, 9),
                new MyTestCaseData(TestCaseAction.NavigateWordForwardCommandExtendWordBackwardCommandE, 9, 1),
                new MyTestCaseData(TestCaseAction.NavigateWordBackwardCommandExtendWordForwardCommandE, 9, 1),
                new MyTestCaseData(TestCaseAction.CutK, 1, 9),
                new MyTestCaseData(TestCaseAction.PasteK, 0, 0),
                new MyTestCaseData(TestCaseAction.CopyK, 1, 9),
                new MyTestCaseData(TestCaseAction.DeleteK, 1, 20),
                new MyTestCaseData(TestCaseAction.SelectAllK, 1, 9),
                new MyTestCaseData(TestCaseAction.DocumentEndKExtendDocumentHomeK, 1, 9),
                new MyTestCaseData(TestCaseAction.DocumentHomeKExtendDocumentEndK, 1, 9),
                new MyTestCaseData(TestCaseAction.DocumentEndKDocumentHomeK, 1, 9),
                new MyTestCaseData(TestCaseAction.LineEndKExtendLineHomeK, 1, 9),
                new MyTestCaseData(TestCaseAction.LineHomeKExtendLineEndK, 1, 20),
                new MyTestCaseData(TestCaseAction.ShiftDeleteK, 1, 20),
                new MyTestCaseData(TestCaseAction.ShiftInsertK, 0, 0),
                new MyTestCaseData(TestCaseAction.CtrlInsertK, 1, 9),
                new MyTestCaseData(TestCaseAction.BoldE, 1, 9),
                new MyTestCaseData(TestCaseAction.ItalicE, 1, 9),
                new MyTestCaseData(TestCaseAction.UnderlineE, 1, 9),
                new MyTestCaseData(TestCaseAction.LeftJustifyE, 1, 9),
                new MyTestCaseData(TestCaseAction.CenterJustifyE, 1, 9),
                new MyTestCaseData(TestCaseAction.RightJustifyE, 1, 9),
                new MyTestCaseData(TestCaseAction.FullJustifyE, 1, 9),
                new MyTestCaseData(TestCaseAction.BoldK, 1, 9),
                new MyTestCaseData(TestCaseAction.ItalicK, 1, 9),
                new MyTestCaseData(TestCaseAction.UnderlineK, 1, 9),
                new MyTestCaseData(TestCaseAction.LeftJustifyK, 1, 9),
                new MyTestCaseData(TestCaseAction.CenterJustifyK, 1, 9),
                new MyTestCaseData(TestCaseAction.RightJustifyK, 1, 9),
                new MyTestCaseData(TestCaseAction.Regression_Bug32, 6, 10),
            };
        }

        #endregion struct

        #region Main flow.
        /// <summary>Runs a combination for the test case.</summary>
        protected override void DoRunCombination()
        {
            TestElement = _editableType.CreateInstance();
            _wrapper = new UIElementWrapper(TestElement);
            ((TextBoxBase)_wrapper.Element).Height = 100;
            _pasteString = "Hello World";
            _textBoxCount = 22; // Change this number if adding more command case for textBox            

            _testCaseIndex = 0;
            QueueDelegate(StartTest);
        }

        private void StartTest()
        {
            _dataContent = "This is a test1. This is a test2. This is a test3. This is a test4. This is a test5.";
            _wrapper.Text = _dataContent;
            MouseInput.MouseClick(_wrapper.Element);
            QueueDelegate(new SimpleHandler(PerformSelection));
        }

        private void PerformSelection()
        {
            _testCaseData = MyTestCaseData.Cases[_testCaseIndex];
            _wrapper.Select(_testCaseData.StartSelection, _testCaseData.EndSelection);
            
            if (_wrapper.Element is RichTextBox)
                Log("*******Running new test case:[" + _testCaseIndex + " out of " + MyTestCaseData.Cases.Length + "]");
            else
                Log("*******Running new test case:[" + _testCaseIndex + " out of " + _textBoxCount + "]");
            Log("Test Action:[" + _testCaseData.TestAction + "]");


            _expectedSelection = _wrapper.GetSelectedText(false, false);
            if (_wrapper.Element is RichTextBox)
            {
                _textRange = new TextRange(_wrapper.Start, _wrapper.End);
            }
            QueueDelegate(PerformAction);
        }

        private void PerformAction()
        {
            #region TestAction
            //Start testAction K for Keyboard key, E for Execute command api
            //EX: CutK=Ctrl+x, CutE=ApplicationCommands.Cut
            switch (_testCaseData.TestAction)
            {
                case TestCaseAction.CutK:
                    KeyboardInput.TypeString("^x");
                    break;
                case TestCaseAction.CutE:
                    RoutedCommand CutCommand = ApplicationCommands.Cut;
                    CutCommand.Execute(null, _wrapper.Element);
                    break;
                case TestCaseAction.CopyK:
                    KeyboardInput.TypeString("^c");
                    break;
                case TestCaseAction.CopyE:
                    RoutedCommand CopyCommand = ApplicationCommands.Copy;
                    CopyCommand.Execute(null, _wrapper.Element);
                    break;
                case TestCaseAction.PasteK:
                    Clipboard.SetDataObject(_pasteString);
                    KeyboardInput.TypeString("^v");
                    break;
                case TestCaseAction.PasteE:
                    Clipboard.SetDataObject(_pasteString);
                    RoutedCommand PasteCommand = ApplicationCommands.Paste;
                    PasteCommand.Execute(null, _wrapper.Element);
                    break;
                case TestCaseAction.DeleteK:
                    KeyboardInput.TypeString("{Delete}");
                    break;
                case TestCaseAction.DeleteE:
                    RoutedCommand DeleteCommand = ApplicationCommands.Delete;
                    DeleteCommand.Execute(null, _wrapper.Element);
                    break;
                case TestCaseAction.BoldK:
                    KeyboardInput.TypeString(EditingCommandData.ToggleBold.KeyboardShortcut);
                    break;
                case TestCaseAction.BoldE:
                    RoutedCommand BoldCommand = EditingCommands.ToggleBold;
                    BoldCommand.Execute(null, _wrapper.Element);
                    break;
                case TestCaseAction.ItalicK:
                    KeyboardInput.TypeString(EditingCommandData.ToggleItalic.KeyboardShortcut);
                    break;
                case TestCaseAction.ItalicE:
                    RoutedCommand ItalicCommand = EditingCommands.ToggleItalic;
                    ItalicCommand.Execute(null, _wrapper.Element);
                    break;
                case TestCaseAction.UnderlineK:
                    KeyboardInput.TypeString(EditingCommandData.ToggleUnderline.KeyboardShortcut);
                    break;
                case TestCaseAction.UnderlineE:
                    RoutedCommand UnderlineCommand = EditingCommands.ToggleUnderline;
                    UnderlineCommand.Execute(null, _wrapper.Element);
                    break;
                case TestCaseAction.LeftJustifyK:
                    KeyboardInput.TypeString("^l");
                    break;
                case TestCaseAction.LeftJustifyE:
                    RoutedCommand LeftJustifyCommand = EditingCommands.AlignLeft;
                    LeftJustifyCommand.Execute(null, _wrapper.Element);
                    break;
                case TestCaseAction.CenterJustifyK:
                    KeyboardInput.TypeString("^e");
                    break;
                case TestCaseAction.CenterJustifyE:
                    RoutedCommand CenterJustifyCommand = EditingCommands.AlignCenter;
                    CenterJustifyCommand.Execute(null, _wrapper.Element);
                    break;
                case TestCaseAction.RightJustifyK:
                    KeyboardInput.TypeString("^r");
                    break;
                case TestCaseAction.RightJustifyE:
                    RoutedCommand RightJustifyCommand = EditingCommands.AlignRight;
                    RightJustifyCommand.Execute(null, _wrapper.Element);
                    break;
                case TestCaseAction.FullJustifyE:
                    RoutedCommand FullJustifyCommand = EditingCommands.AlignJustify;
                    FullJustifyCommand.Execute(null, _wrapper.Element);
                    break;
                case TestCaseAction.LineEndKExtendLineHomeK: //test both End and Shift+Home
                    KeyboardInput.TypeString("{END}");
                    KeyboardInput.TypeString("+{HOME}");
                    break;
                case TestCaseAction.LineHomeKExtendLineEndK: //test both Home and Shift+End
                    KeyboardInput.TypeString("{HOME}");
                    KeyboardInput.TypeString("+{END}");
                    break;
                case TestCaseAction.DocumentEndKDocumentHomeK: //test both Ctrl+End and Ctrl+Home
                    KeyboardInput.TypeString("^{END}");
                    KeyboardInput.TypeString("^{HOME}");
                    break;
                case TestCaseAction.DocumentEndEDocumentHomeE:
                    RoutedCommand DocumentEndCommand = EditingCommands.MoveToDocumentEnd;
                    DocumentEndCommand.Execute(null, _wrapper.Element);
                    RoutedCommand DocumentHomeCommand = EditingCommands.MoveToDocumentStart;
                    DocumentHomeCommand.Execute(null, _wrapper.Element);
                    break;
                case TestCaseAction.DocumentEndKExtendDocumentHomeK: //test both End and Ctrl+Shift+Home
                    KeyboardInput.TypeString("^{END}");
                    KeyboardInput.TypeString("^+{HOME}");
                    break;
                case TestCaseAction.DocumentEndEExtendDocumentHomeE:
                    RoutedCommand DocumentEnd = EditingCommands.MoveToDocumentEnd;
                    DocumentEnd.Execute(null, _wrapper.Element);
                    RoutedCommand ExtendDocumentHomeCommand = EditingCommands.SelectToDocumentStart;
                    ExtendDocumentHomeCommand.Execute(null, _wrapper.Element);
                    break;
                case TestCaseAction.DocumentHomeKExtendDocumentEndK: //test both Home and Ctrl+Shift+End
                    KeyboardInput.TypeString("^{HOME}");
                    KeyboardInput.TypeString("^+{END}");

                    break;
                case TestCaseAction.DocumentHomeEExtendDocumentEndE:
                    RoutedCommand DocumentHome = EditingCommands.MoveToDocumentStart;
                    DocumentHome.Execute(null, _wrapper.Element);
                    RoutedCommand ExtendDocumentEndCommand = EditingCommands.SelectToDocumentEnd;
                    ExtendDocumentEndCommand.Execute(null, _wrapper.Element);
                    break;
                case TestCaseAction.ShiftDeleteK:
                    KeyboardInput.TypeString("+{DELETE}");
                    break;
                case TestCaseAction.ShiftInsertK:
                    Clipboard.SetDataObject(_pasteString);
                    KeyboardInput.TypeString("+{INSERT}");
                    break;
                case TestCaseAction.CtrlInsertK:
                    KeyboardInput.TypeString("^{INSERT}");
                    break;
                case TestCaseAction.SelectAllK:
                    KeyboardInput.TypeString("^a");
                    break;
                case TestCaseAction.SelectAllE:
                    RoutedCommand SelectAllCommand = ApplicationCommands.SelectAll;
                    SelectAllCommand.Execute(null, _wrapper.Element);
                    break;
                //Regression_Bug32 - bold, italic, underline are not toggled
                //when selection include white space is in front of character
                case TestCaseAction.Regression_Bug32:
                    KeyboardInput.TypeString(EditingCommandData.ToggleBold.KeyboardShortcut +
                        EditingCommandData.ToggleItalic.KeyboardShortcut +
                        EditingCommandData.ToggleUnderline.KeyboardShortcut);
                    break;
                //Ctrl+right arrow then ctrl+Shift+left
                case TestCaseAction.NavigateWordForwardCommandExtendWordBackwardCommandE:
                    RoutedCommand home1 = EditingCommands.MoveToDocumentStart;
                    home1.Execute(null, _wrapper.Element);
                    RoutedCommand NavigateWordForwardCommand = EditingCommands.MoveRightByWord;
                    NavigateWordForwardCommand.Execute(null, _wrapper.Element);
                    RoutedCommand ExtendWordBackwardCommand = EditingCommands.SelectLeftByWord;
                    ExtendWordBackwardCommand.Execute(null, _wrapper.Element);
                    break;
                //Ctrl+left arrow then ctrl+Shift+right
                case TestCaseAction.NavigateWordBackwardCommandExtendWordForwardCommandE:
                    RoutedCommand home2 = EditingCommands.MoveToDocumentStart;
                    home2.Execute(null, _wrapper.Element);
                    RoutedCommand NavigateWordBackwardCommand = EditingCommands.MoveLeftByWord;
                    NavigateWordBackwardCommand.Execute(null, _wrapper.Element);
                    RoutedCommand ExtendWordForwardCommand = EditingCommands.SelectRightByWord;
                    ExtendWordForwardCommand.Execute(null, _wrapper.Element);
                    break;
            }
            #endregion TestAction

            QueueDelegate(new SimpleHandler(Verify_TestAction));
        }

        private void Verify_TestAction()
        {
            _dataContent += (_wrapper.Element is RichTextBox) ? "\r\n" : "";
            switch (_testCaseData.TestAction)
            {
                case TestCaseAction.CutK:
                case TestCaseAction.CutE:
                case TestCaseAction.ShiftDeleteK:
                case TestCaseAction.CopyK:
                case TestCaseAction.CopyE:
                case TestCaseAction.CtrlInsertK:
                    VerifyResult(_expectedSelection, Clipboard.GetDataObject().GetData("UnicodeText").ToString());
                    break;

                case TestCaseAction.PasteK:
                case TestCaseAction.PasteE:
                case TestCaseAction.ShiftInsertK:
                    VerifyResult(_pasteString+_dataContent, _wrapper.Text);
                    break;

                case TestCaseAction.DeleteK:
                case TestCaseAction.DeleteE:
                    Verifier.Verify(_wrapper.Text != _dataContent,
                        "\nExpect: " + _dataContent + "\nActual: " + _wrapper.Text);
                    break;

                case TestCaseAction.BoldK:
                case TestCaseAction.BoldE:
                    Verifier.Verify(_wrapper.XamlText.Contains(" FontWeight=\"Bold\""),
                        "\nExpect: Xaml content should contain 'Bold'\nActual: " + _wrapper.XamlText);
                    //toggle bold for previous action
                    RoutedCommand BoldCommand = EditingCommands.ToggleBold;
                    BoldCommand.Execute(null, _wrapper.Element);
                    break;

                case TestCaseAction.ItalicK:
                case TestCaseAction.ItalicE:
                    Verifier.Verify(_wrapper.XamlText.Contains(" FontStyle=\"Italic\""),
                        "\nExpect: Xaml content should contain 'Italic'\nActual: " + _wrapper.XamlText);
                    //toggle italic for previous action
                    RoutedCommand ItalicCommand = EditingCommands.ToggleItalic;
                    ItalicCommand.Execute(null, _wrapper.Element);
                    break;

                case TestCaseAction.UnderlineK:
                case TestCaseAction.UnderlineE:
                    Verifier.Verify(_wrapper.XamlText.Contains("TextDecorations=\"Underline\""),
                        "\nExpect: Xaml content should contain TextDecorations=\"Underline\"\nActual: " + _wrapper.XamlText);
                    //toggle Underline for previous action
                    RoutedCommand UnderlineCommand = EditingCommands.ToggleUnderline;
                    UnderlineCommand.Execute(null, _wrapper.Element);
                    break;

                case TestCaseAction.LeftJustifyK:
                case TestCaseAction.LeftJustifyE:
                    Verifier.Verify(XamlUtils.TextRange_GetXml(_textRange).Contains("<Paragraph>"),
                        "\nExpect: Xaml content should contain '<Paragraph>'\nActual: " + XamlUtils.TextRange_GetXml(_textRange));
                    break;

                case TestCaseAction.CenterJustifyK:
                case TestCaseAction.CenterJustifyE:
                    Verifier.Verify(XamlUtils.TextRange_GetXml(_textRange).Contains("TextAlignment=\"Center\""),
                        "\nExpect: Xaml content should contain 'TextAlignment=Center'\nActual: " + XamlUtils.TextRange_GetXml(_textRange));
                    break;

                case TestCaseAction.RightJustifyK:
                case TestCaseAction.RightJustifyE:
                    Verifier.Verify(XamlUtils.TextRange_GetXml(_textRange).Contains("<Paragraph TextAlignment=\"Right\">"),
                        "\nExpect: Xaml content should contain 'TextAlignment=Right'\nActual: " + XamlUtils.TextRange_GetXml(_textRange));
                    break;

                case TestCaseAction.FullJustifyE:
                    Verifier.Verify(XamlUtils.TextRange_GetXml(_textRange).Contains("<Paragraph TextAlignment=\"Justify\">"),
                        "\nExpect: Xaml content should contain 'TextAlignment=Justify'\nActual: " + XamlUtils.TextRange_GetXml(_textRange));
                    break;

                case TestCaseAction.DocumentEndKDocumentHomeK:
                case TestCaseAction.DocumentEndEDocumentHomeE:
                    VerifyResult(string.Empty, _wrapper.GetSelectedText(false, false));
                    break;

                case TestCaseAction.DocumentEndKExtendDocumentHomeK:
                case TestCaseAction.DocumentEndEExtendDocumentHomeE:
                case TestCaseAction.LineEndKExtendLineHomeK:
                    _dataContent = _dataContent.Replace("\r\n", "");
                    VerifyResult(_dataContent, _wrapper.GetSelectedText(false, false));
                    break;

                case TestCaseAction.LineHomeKExtendLineEndK:// This verification will fail if 
                case TestCaseAction.DocumentHomeKExtendDocumentEndK:
                case TestCaseAction.DocumentHomeEExtendDocumentEndE:
                case TestCaseAction.SelectAllK:
                case TestCaseAction.SelectAllE:
                    VerifyResult(_dataContent, _wrapper.GetSelectedText(false, false));
                    break;

                case TestCaseAction.Regression_Bug32:
                    Verifier.Verify(
                        (_wrapper.XamlText).Contains("FontWeight=\"Bold\"") &&
                        (_wrapper.XamlText).Contains("FontStyle=\"Italic\"") &&
                        (_wrapper.XamlText).Contains("TextDecorations=\"Underline\""),
                        "\nExpect:[Xaml should container Bold, Italic, Underline]\nActual:[" + _wrapper.XamlText + "]");
                    //Toggle formated text
                    KeyboardInput.TypeString(EditingCommandData.ToggleBold.KeyboardShortcut +
                        EditingCommandData.ToggleItalic.KeyboardShortcut +
                        EditingCommandData.ToggleUnderline.KeyboardShortcut);
                    break;

                //Ctrl+right arrow then trl+Shift+left
                case TestCaseAction.NavigateWordForwardCommandExtendWordBackwardCommandE:
                //Ctrl+left arrow then Ctrl+Shift+right
                case TestCaseAction.NavigateWordBackwardCommandExtendWordForwardCommandE:
                        Verifier.Verify(_wrapper.GetSelectedText(false, false) == "This ",
                            "Expect: [This ]\nActual: " + "[" + _wrapper.GetSelectedText(false, false) + "]");
                    break;
            }
            QueueDelegate(new SimpleHandler(RunNextTestCase));
        }

        private void RunNextTestCase()
        {
            //Verify result for Regression_Bug32
            if (_testCaseData.TestAction == TestCaseAction.Regression_Bug32)
            {
                Verifier.Verify(
                        !(_wrapper.XamlText).Contains("FontWeight=\"Bold\"") &&
                        !(_wrapper.XamlText).Contains("FontStyle=\"Italic\"") &&
                        !(_wrapper.XamlText).Contains("TextDecorations=\"Underline\""),
                        "\nExpect:[Xaml should not container Bold, Italic, Underline]\nActual:[" + _wrapper.XamlText + "]");
            }

            _testCaseIndex++;
            if (_wrapper.Element is TextBox)
            {
                if (_testCaseIndex == _textBoxCount)
                {
                    QueueDelegate(NextCombination);
                }
                else
                {
                    Log("Restart next test case...");
                    StartTest();
                }
            }
            else
            {
                if (_testCaseIndex == MyTestCaseData.Cases.Length)
                {
                    QueueDelegate(NextCombination);
                }
                else
                {
                    Log("Restart next test case...");
                    StartTest();
                }
            }
        }

        private void VerifyResult(string expect, string actual)
        {
            Verifier.Verify(expect == actual, "\nExpect[" + expect + "]\nActual[" + actual + "]");
        }
        
        #endregion Main flow.

        #region Private fields.

        private UIElementWrapper _wrapper;
        private TextEditableType _editableType = null;
        private string _expectedSelection;
        private TextRange _textRange;
        private string _dataContent;
        private string _pasteString;
        private int _textBoxCount;
        private int _testCaseIndex;
        private MyTestCaseData _testCaseData;
        
        private enum TestCaseAction
        {
            CutE,
            PasteE,
            CopyE,
            DeleteE,
            SelectAllE,
            DocumentEndEExtendDocumentHomeE,
            DocumentHomeEExtendDocumentEndE,
            DocumentEndEDocumentHomeE,
            NavigateWordForwardCommandExtendWordBackwardCommandE,    //Ctrl+right arrow then trl+Shift+left arrow
            NavigateWordBackwardCommandExtendWordForwardCommandE,   //Ctrl+left arrow then Ctrl+Shift+right arrow
            CutK,
            PasteK,
            CopyK,
            DeleteK,
            SelectAllK,
            DocumentEndKExtendDocumentHomeK,
            DocumentHomeKExtendDocumentEndK,
            DocumentEndKDocumentHomeK,
            LineEndKExtendLineHomeK,
            LineHomeKExtendLineEndK,
            ShiftDeleteK,
            ShiftInsertK,
            CtrlInsertK,
            //Below commands are not valid in TextBox. They are for RichTextBox
            BoldE,
            ItalicE,
            UnderlineE,
            LeftJustifyE,
            CenterJustifyE,
            RightJustifyE,
            FullJustifyE,
            BoldK,
            ItalicK,
            UnderlineK,
            LeftJustifyK,
            CenterJustifyK,
            RightJustifyK,
            Regression_Bug32,
        }

        #endregion Private fields.
    }

    #endregion EditCommandText

    ///<summary>
    ///Runs data transfer test cases with different Unicode scripts.
    ///</summary>
    ///<remarks>
    ///The following execution modes are expected:
    ///Pri-1: EditingTest.exe /TestCaseType=DataTransferScripts (~16)
    ///</remarks>    
    [Test(2, "Editor", "DataTransferScripts", MethodParameters = "/TestCaseType=DataTransferScripts", Timeout=800)]
    [TestOwner("Microsoft"), TestWorkItem("24"), TestWorkItem("23"), TestTactics("274")]
    public class DataTransferScripts: ManagedCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            DockPanel panel;                    // Panel holding controls.
            FrameworkElement control;           // Source control for copying.
            FrameworkElement otherControl;      // A different instance to paste into.

            control = _editableType.CreateInstance();
            otherControl = _editableType.CreateInstance();
            Log("*************" + _editableType.XamlName + "**************");
            control.Width = otherControl.Width = 100;

            _source = new UIElementWrapper(control);
            _target = new UIElementWrapper(otherControl);

            panel = new DockPanel();
            panel.Children.Add(control);
            panel.Children.Add(otherControl);

            MainWindow.Content = panel;

            Clipboard.Clear();

            _scriptArray = TextScript.Values;
            _count = _scriptArray.Length;
            _count--;
            QueueDelegate(ExecuteTrigger);
        }

        private void ExecuteTrigger()
        {
            Log("Script Name: " + _scriptArray[_count].Name);
            _source.Text = _scriptArray[_count].Sample;
            QueueDelegate(CopyFromSource);
        }
        private void CopyFromSource()
        {
            _source.Element.Focus();
            QueueDelegate(CopyContent);
        }

        private void CopyContent()
        {
            KeyboardInput.TypeString("^a^c");
            QueueDelegate(PasteIntoTarget);
        }

        private void PasteIntoTarget()
        {
            _target.Element.Focus();
            KeyboardInput.TypeString("^a^v");
            QueueDelegate(VerifyContents);
        }

        private void VerifyContents()
        {
            Log("Source text: [" + _source.Text + "]");
            Log("Target text: [" + _target.Text + "]");

            if (_editableType.IsPassword)
            {
                Verifier.Verify(_target.Text == "",
                    "Target should be empty after a Paste with an empty clipboard.", false);
            }
            else
            {
                Verifier.Verify(_source.Text == _target.Text);
            }
            _count--;
            if (_count <0)
            {                
                QueueDelegate(NextCombination);
            }
            else
            {
                QueueDelegate(ExecuteTrigger);
            }
        }

        #endregion Main flow.

        #region Private fields.

        private UIElementWrapper _source;
        private UIElementWrapper _target;

        private TextScript[] _scriptArray;
        private int _count = 0;

        private TextEditableType _editableType= null;

        #endregion Private fields.
    }
}
