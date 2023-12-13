// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Functional test cases for TextBox members that affect editing and 
//  are not covered elsewhere: AcceptsTab, AcceptsReturn, IsMultiline, Wrap. 

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Text/BVT/TextBoxOM/Editing.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
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
    using Microsoft.Test.Input;
    using System.Runtime.Serialization;
    using System.Runtime.InteropServices;
    #endregion Namespaces.

    #region enums.

    /// <summary> Content Choices </summary>
    enum ContentChoices
    {
        Empty,
        SingleLineText,
        SingleLineFollowedByEmptyLine,
        NewLine,
    }

    /// <summary> AppendTextChoices </summary>
    enum AppendTextChoices
    {
        Empty,
        Word,
        NewLine,
    }

    /// <summary> Cup Copy Paste possibilities </summary>
    enum CutCopyPasteOperations
    {
        CutPaste,
        CopyPaste,
        CutCopyPaste,
    }

    enum FlowDirectionInputs
    {
        Keyboard,
        Programmatically,
    }

    /// <summary> FormatChoices </summary>
    enum FormatChoices
    {
        Bold,
        Underline,
    }

    /// <summary> Locale Info </summary>
    enum InputStringChoices
    {
        Single,
        Multi,
    }

    /// <summary> Input String Type </summary>
    enum InputStringData
    {
        Empty,
        LTR,
        RTL,
        MixedLTR_RTL,
        MixedRTL_LTR,
        MultiLine,
        Surrogate,
        InvalidTest,
    }

    /// <summary> Locale Info </summary>
    enum LocaleChoices
    {
        English,
        Arabic,
    }

    enum OperationChoices
    {
        Append,
        Clear,
        Copy,
        CutPaste,
        UndoRedo,
        DragDrop,
        Text,
    }

    /// <summary> Text selection possibilities </summary>
    enum SelectTextOptions
    {
        SelectEmptyText,
        SelectAll,
        SelectMiddle,
        SelectEnd,
        NoSelection,
        PasswordPaste,
    }

    enum SetOptions
    {
        SetThroughProperty,
        SetThroughAccessor,
        SetThroughStyle,
    }

    #endregion enums.

    /// <summary>This one Test the autowordSelection property on TextBoxBase</summary>
    [Test(3, "TextBoxBase", "AutoWordSelectionPropertyTest", MethodParameters = "/TestCaseType:AutoWordSelectionPropertyTest")]
    [TestOwner("Microsoft"), TestTactics("627"), TestBugs(""), TestWorkItem("")]
    public class AutoWordSelectionPropertyTest : ManagedCombinatorialTestCase
    {
        /// <summary>
        /// Override the base to filter out the PasswordBox
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        protected override bool DoReadCombination(Hashtable values)
        {
            bool result = base.DoReadCombination(values);
            if (_editableType.IsPassword)
            {
                result = false;
            }
            return result;
        }

        /// <summary>P </summary>
        protected override void DoRunCombination()
        {
            bool enabled;
            TextBoxBase tBase;

            _wrapper = new UIElementWrapper(_editableType.CreateInstance());
            tBase = _wrapper.Element as TextBoxBase;

            //Filter out no textBoxBase Controls
            if (tBase == null)
            {
                NextCombination();
                return;
            }

            //choose a way to retrieve the value
            enabled = _useGetValue ? (bool)tBase.GetValue(TextBoxBase.AutoWordSelectionProperty) : tBase.AutoWordSelection;

            if (tBase is TextBox)
            {
                Verifier.Verify(tBase.AutoWordSelection == enabled, "Default value for TextBox.AutoWordSelection should be false. The actual value is true");
            }
            if (tBase is RichTextBox)
            {
                Verifier.Verify(tBase.AutoWordSelection == enabled, "Default value for RichTextBox.AutoWordSelection should be true. The actual value is false");
            }

            TestElement = _wrapper.Element as FrameworkElement;
            _wrapper.Text = "Auto word Selection Test";

            //choose a way to set the value
            if (_useSetValue)
            {
                tBase.SetValue(TextBoxBase.AutoWordSelectionProperty, _autoWordSelectionEnabled);
            }
            else
            {
                tBase.AutoWordSelection = _autoWordSelectionEnabled;
            }

            //choose a way to retrieve the value
            enabled = _useGetValue ? (bool)tBase.GetValue(TextBoxBase.AutoWordSelectionProperty) : tBase.AutoWordSelection;

            Verifier.Verify(enabled == _autoWordSelectionEnabled, "AutoWordSelection is not set correctly.");
            if (tBase is RichTextBox)
            {
                //This should be fully test somewhere else. 
                //Adding this code just verify the basic functionality of the Addreange method. 
                RichTextBox rBox = tBase as RichTextBox;
                List<TextElement> list = new List<TextElement>();
                list.Add(new Paragraph(new Run("a")));
                list.Add(new Paragraph(new Run("b")));
                rBox.Document.Blocks.Clear();
                rBox.Document.Blocks.AddRange(list);

                Verifier.Verify(_wrapper.Text == "a\r\nb\r\n", "wrong text in RichTextBox! Expected[a\r\nb\r\n], Actual[" + _wrapper.Text + "]");
            }

            NextCombination();
        }

        /// <summary>bool value to set</summary>
        private bool _autoWordSelectionEnabled = false;
        /// <summary>Element types</summary>
        private TextEditableType _editableType = null;
        /// <summary>Use getValue method?</summary>
        private bool _useGetValue = false;
        /// <summary>Use SetValue Method?</summary>
        private bool _useSetValue = false;
        /// <summary>Wrapper for the Control </summary>
        private UIElementWrapper _wrapper;
    }

    /// <summary>
    /// Verifies that AcceptsTab property work as expected in TextBox
    /// </summary>
    [Test(0, "TextBox", "TextBoxAcceptsTabTest", MethodParameters = "/TestCaseType=TextBoxAcceptsTabTest /InputMonitorEnabled:False")]
    [TestOwner("Microsoft"), TestTactics("625"), TestBugs("654"), TestWorkItem("116")]
    public class TextBoxAcceptsTabTest : CustomTestCase
    {
        #region PrivateMembers

        /// <summary>Combinatorial engine for values.</summary>
        private CombinatorialEngine _combinatorials;
        private string _selectionState;
        private bool _testValue;
        private string _textContent;

        private TextBox _tb;
        private Button _testButton1;
        private Button _testButton2;
        private UIElementWrapper _testWrapper;
        private StackPanel _panel;

        private string _stateBeforeTestOperation;
        private string _textBeforeCaret;
        private string _textAfterCaret;

        #endregion

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            Dimension[] dimensions; // Dimensions for combinations.
            dimensions = new Dimension[] {
                new Dimension("SelectionState", new object[]{"start", "middle", "end", "selectall"}),                
                new Dimension("Text", new Object[] {string.Empty, "Hello world"}),
                new Dimension("TestValue", new Object[] {true, false})
            };
            _combinatorials = CombinatorialEngine.FromDimensions(dimensions);

            _panel = new StackPanel();
            MainWindow.Content = _panel;

            QueueDelegate(RunNextTestCase);
        }

        private void RunNextTestCase()
        {
            Hashtable combinationValues;   // Values for combination.

            combinationValues = new Hashtable();
            if (_combinatorials.Next(combinationValues))
            {
                _selectionState = (string)combinationValues["SelectionState"];

                _tb = new TextBox();
                _testWrapper = new UIElementWrapper(_tb);
                _textContent = (string)combinationValues["Text"];
                _testValue = (bool)combinationValues["TestValue"];

                //Verifying the default value
                Verifier.Verify(_tb.AcceptsTab == false, "Verifying that AcceptsTab has a default value of false in TextBox");

                if (_testValue)
                {
                    _tb.AcceptsTab = true;
                }
                else
                {
                    _tb.AcceptsTab = false;
                }

                _tb.Height = 300;
                _tb.Width = 300;
                _tb.FontSize = 16;

                _testButton1 = new Button();
                _testButton2 = new Button();
                _testButton1.Width = _testButton2.Width = 300;
                _testButton1.Height = _testButton2.Height = 25;

                _panel.Children.Clear();
                _panel.Children.Add(_testButton1);
                _panel.Children.Add(_tb);
                _panel.Children.Add(_testButton2);

                Log("---- SelectionPosition: " + _selectionState +
                    " ---- Text: " + _textContent);

                QueueDelegate(SetFocus);
            }
            else
            {
                Logger.Current.ReportSuccess();
                return;
            }
        }

        private void SetFocus()
        {
            SetSelection();
            _tb.Focus();

            QueueDelegate(PerformTabAction);
        }

        private void SetSelection()
        {
            _testWrapper.Text = _textContent;

            if (_selectionState == "start")
            {
                _tb.SelectionStart = 0;
            }
            else if (_selectionState == "middle")
            {
                _tb.SelectionStart = _textContent.Length / 2;
            }
            else if (_selectionState == "end")
            {
                _tb.SelectionStart = _textContent.Length;
            }
            else if (_selectionState == "selectall")
            {
                _tb.SelectAll();
            }
        }

        private void PerformTabAction()
        {
            _stateBeforeTestOperation = _testWrapper.Text;
            _textBeforeCaret = _testWrapper.GetTextOutsideSelection(LogicalDirection.Backward);
            _textAfterCaret = _testWrapper.GetTextOutsideSelection(LogicalDirection.Forward);

            KeyboardInput.TypeString("{TAB}");

            QueueDelegate(VerifyTabResult);
        }

        private void VerifyTabResult()
        {
            if (_testValue == false)
            {
                Verifier.Verify(_testButton2.IsKeyboardFocused, "Verifying that focus shifted for Tab", true);
                Verifier.Verify(_testWrapper.Text == _stateBeforeTestOperation,
                    "Verifying that tab is not accepted when AcceptsTab is False", true);
            }
            else
            {
                Verifier.Verify(_tb.IsKeyboardFocused, "Verifying that focus did not shifted for Tab", true);
                if (_selectionState == "selectall")
                {
                    Verifier.Verify(_testWrapper.Text == "\t",
                    "Verifying that tab is accepted when AcceptsTab is True", true);
                }
                else
                {
                    Verifier.Verify(_testWrapper.Text == _textBeforeCaret + "\t" + _textAfterCaret,
                    "Verifying that tab is accepted when AcceptsTab is True", true);
                }
            }

            SetSelection();
            _tb.Focus();

            QueueDelegate(PerformShiftTabAction);
        }

        private void PerformShiftTabAction()
        {
            _stateBeforeTestOperation = _testWrapper.Text;

            KeyboardInput.TypeString("+{TAB}");
            QueueDelegate(VerifyShiftTabResult);
        }

        private void VerifyShiftTabResult()
        {
            if (_testValue == false)
            {
                Verifier.Verify(_testButton1.IsKeyboardFocused, "Verifying that focus shifted for ShiftTab", true);
                Verifier.Verify(_testWrapper.Text == _stateBeforeTestOperation,
                    "Verifying that ShiftTab is not accepted when AcceptsTab is False", true);
            }
            else
            {
                Verifier.Verify(_tb.IsKeyboardFocused, "Verifying that focus did not shifted for ShiftTab", true);
                if (_selectionState == "selectall")
                {
                    Verifier.Verify(_testWrapper.Text == "\t",
                    "Verifying that ShiftTab is accepted when AcceptsTab is True", true);
                }
                else
                {
                    Verifier.Verify(_testWrapper.Text == _textBeforeCaret + "\t" + _textAfterCaret,
                    "Verifying that ShiftTab is accepted when AcceptsTab is True", true);
                }
            }

            QueueDelegate(RunNextTestCase);
        }
    }

    /// <summary>
    /// Verifies that AcceptsTab property work as expected in RichTextBox
    /// </summary>
    [Test(2, "RichTextBox", "RTBAcceptsTabTest", MethodParameters = "/TestCaseType=RTBAcceptsTabTest /InputMonitorEnabled:False", Timeout = 200)]
    [TestOwner("Microsoft"), TestTactics("624"), TestBugs("654"), TestWorkItem("116")]
    public class RTBAcceptsTabTest : ManagedCombinatorialTestCase
    {
        #region PrivateMembers

        private bool _testValue = false;
        private bool _testSelectionNonEmpty = false;
        private string _xamlContent = string.Empty;
        private string _decreseIndentCommand = string.Empty;

        private RichTextBox _rtb;
        private Button _testButton1;
        private Button _testButton2;
        private UIElementWrapper _testWrapper;
        private StackPanel _panel;

        private double _beforeTextIndent,_afterTextIndent;
        private double _beforeLeftMargin,_afterLeftMargin;

        string _beforeState,_afterState;

        #endregion

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            if ((_testValue == false) && (_decreseIndentCommand == "{BACKSPACE}"))
            {
                //No need to run this combination
                Log("Skipping this combination");
                QueueDelegate(NextCombination);
                return;
            }

            if ((_testSelectionNonEmpty == true) && (_decreseIndentCommand == "{BACKSPACE}"))
            {
                //No need to run this combination
                Log("Skipping this combination");
                QueueDelegate(NextCombination);
                return;
            }

            _rtb = new RichTextBox();
            _rtb.Height = _rtb.Width = 300;
            _rtb.FontSize = 24;

            _testWrapper = new UIElementWrapper(_rtb);

            //Set content for RichTextBox            
            TextRange tr = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);
            XamlUtils.SetXamlContent(tr, _xamlContent);

            //Set selection            
            if (_testSelectionNonEmpty)
            {
                Run runElement = (Run)((Paragraph)_rtb.Document.Blocks.FirstBlock).Inlines.FirstInline;
                TextPointer tp1 = runElement.ContentStart.GetPositionAtOffset(runElement.Text.Length / 4);
                TextPointer tp2 = runElement.ContentStart.GetPositionAtOffset(runElement.Text.Length * (3 / 4));
                _rtb.Selection.Select(tp1, tp2);
            }

            Verifier.Verify(_rtb.AcceptsTab == false, "Verifying the default value of AcceptsTab");

            //Set AcceptsTab
            if (_testValue)
            {
                _rtb.AcceptsTab = true;
            }
            else
            {
                _rtb.AcceptsTab = false;
            }

            _testButton1 = new Button();
            _testButton2 = new Button();
            _testButton1.Width = _testButton2.Width = 300;
            _testButton1.Height = _testButton2.Height = 25;

            _panel = new StackPanel();
            _panel.Children.Add(_testButton1);
            _panel.Children.Add(_rtb);
            _panel.Children.Add(_testButton2);

            TestElement = _panel;
            QueueDelegate(SetFocus);
        }

        /// <summary>Control programmer</summary>
        private void SetFocus()
        {
            MouseInput.MouseClick(_rtb);
            KeyboardInput.TypeString("^{HOME}");
            QueueDelegate(ExecuteTrigger);
        }

        private void ExecuteTrigger()
        {
            _rtb.Focus();

            _beforeTextIndent = _rtb.Selection.Start.Paragraph.TextIndent;
            _beforeLeftMargin = _rtb.Selection.Start.Paragraph.Margin.Left;
            _beforeState = _testWrapper.Text;

            QueueDelegate(PerformFirstTab);
        }

        private void PerformFirstTab()
        {
            if (!_rtb.Selection.IsEmpty)
            {
                KeyboardInput.TypeString("{TAB}");
            }
            else
            {
                KeyboardInput.TypeString("{HOME}{TAB}");
            }

            QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromMilliseconds(250),
                new SimpleHandler(VerifyFirstTab), null);
        }

        private void VerifyFirstTab()
        {
            _afterTextIndent = _rtb.Selection.Start.Paragraph.TextIndent;
            _afterLeftMargin = _rtb.Selection.Start.Paragraph.Margin.Left;
            _afterState = _testWrapper.Text;

            VerifyIndentAndMarginOnTab("after 1st Tab operation");

            _beforeTextIndent = _rtb.Selection.Start.Paragraph.TextIndent;
            _beforeLeftMargin = _rtb.Selection.Start.Paragraph.Margin.Left;
            _beforeState = _testWrapper.Text;

            KeyboardInput.TypeString("{TAB}");
            QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromMilliseconds(250),
                new SimpleHandler(VerifySecondTab), null);
        }

        private void VerifySecondTab()
        {
            _afterTextIndent = _rtb.Selection.Start.Paragraph.TextIndent;
            _afterLeftMargin = _rtb.Selection.Start.Paragraph.Margin.Left;
            _afterState = _testWrapper.Text;

            VerifyIndentAndMarginOnTab("after 2nd Tab operation");

            _beforeTextIndent = _rtb.Selection.Start.Paragraph.TextIndent;
            _beforeLeftMargin = _rtb.Selection.Start.Paragraph.Margin.Left;
            _beforeState = _testWrapper.Text;

            KeyboardInput.TypeString("{TAB}");
            QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromMilliseconds(250),
                new SimpleHandler(VerifyThirdTab), null);
        }

        private void VerifyThirdTab()
        {
            _afterTextIndent = _rtb.Selection.Start.Paragraph.TextIndent;
            _afterLeftMargin = _rtb.Selection.Start.Paragraph.Margin.Left;
            _afterState = _testWrapper.Text;

            VerifyIndentAndMarginOnTab("after 3rd Tab operation");

            _beforeTextIndent = _rtb.Selection.Start.Paragraph.TextIndent;
            _beforeLeftMargin = _rtb.Selection.Start.Paragraph.Margin.Left;
            _beforeState = _testWrapper.Text;

            KeyboardInput.TypeString(_decreseIndentCommand);
            QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromMilliseconds(250),
                new SimpleHandler(VerifyFirstShiftTab), null);
        }

        private void VerifyFirstShiftTab()
        {
            _afterTextIndent = _rtb.Selection.Start.Paragraph.TextIndent;
            _afterLeftMargin = _rtb.Selection.Start.Paragraph.Margin.Left;
            _afterState = _testWrapper.Text;

            VerifyIndentAndMarginOnShiftTab("after 1st ShiftTab operation");

            _beforeTextIndent = _rtb.Selection.Start.Paragraph.TextIndent;
            _beforeLeftMargin = _rtb.Selection.Start.Paragraph.Margin.Left;
            _beforeState = _testWrapper.Text;

            KeyboardInput.TypeString(_decreseIndentCommand);
            QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromMilliseconds(250),
                new SimpleHandler(VerifySecondShiftTab), null);
        }

        private void VerifySecondShiftTab()
        {
            _afterTextIndent = _rtb.Selection.Start.Paragraph.TextIndent;
            _afterLeftMargin = _rtb.Selection.Start.Paragraph.Margin.Left;
            _afterState = _testWrapper.Text;

            VerifyIndentAndMarginOnShiftTab("after 2nd ShiftTab operation");

            _beforeTextIndent = _rtb.Selection.Start.Paragraph.TextIndent;
            _beforeLeftMargin = _rtb.Selection.Start.Paragraph.Margin.Left;
            _beforeState = _testWrapper.Text;

            KeyboardInput.TypeString(_decreseIndentCommand);
            QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromMilliseconds(250),
                new SimpleHandler(VerifyThirdShiftTab), null);
        }

        private void VerifyThirdShiftTab()
        {
            _afterTextIndent = _rtb.Selection.Start.Paragraph.TextIndent;
            _afterLeftMargin = _rtb.Selection.Start.Paragraph.Margin.Left;
            _afterState = _testWrapper.Text;

            VerifyIndentAndMarginOnShiftTab("after 3rd ShiftTab operation");

            QueueDelegate(NextCombination);
        }

        private void VerifyIndentAndMarginOnTab(string logString)
        {
            if (_testValue)
            {
                Verifier.Verify(_rtb.IsKeyboardFocused,
                    "Verifying that focus did not shifted " + logString, true);

                Verifier.Verify(_afterState == _beforeState,
                    "Verifying that tab is not accepted even when AcceptsTab is True " + logString, true);

                if (!_rtb.Selection.IsEmpty)
                {
                    Verifier.Verify(_beforeTextIndent == _afterTextIndent,
                        "Verify that TextIndent didnt change for Tab operation for non-empty selection " + logString, true);

                    //The default is Double.NaN
                    if (double.IsNaN(_beforeLeftMargin))
                    {
                        Verifier.Verify(_afterLeftMargin == 20, "Verifying that Margin = 20 " + logString, true);
                    }
                    else
                    {
                        Verifier.Verify(_afterLeftMargin == 20 + _beforeLeftMargin, "Verifying that Margin is increased by 20 " + logString, true);
                    }
                }
                else
                {
                    if (_beforeTextIndent < 0)
                    {
                        Verifier.Verify(_afterTextIndent == 0, "Verifying TextIndent " + logString +
                            " (beforeIndent<0)", true);

                        //The default is Double.NaN
                        if (double.IsNaN(_beforeLeftMargin))
                        {
                            Verifier.Verify(double.IsNaN(_afterLeftMargin), "Verifying LeftMargin " +
                                logString + " (beforeIndent<0)", true);
                        }
                        else
                        {
                            Verifier.Verify(_afterLeftMargin == _beforeLeftMargin,
                                "Verifying LeftMargin " + logString + " (beforeIndent<0)", true);
                        }
                    }
                    else if (_beforeTextIndent < 20)
                    {
                        Verifier.Verify(_afterTextIndent == 20, "Verifying TextIndent " + logString +
                            " (beforeIndent<20)", true);

                        //The default is Double.NaN
                        if (double.IsNaN(_beforeLeftMargin))
                        {
                            Verifier.Verify(double.IsNaN(_afterLeftMargin), "Verifying LeftMargin " +
                                logString + " (beforeIndent<20)", true);
                        }
                        else
                        {
                            Verifier.Verify(_afterLeftMargin == _beforeLeftMargin,
                                "Verifying LeftMargin " + logString + " (beforeIndent<20)", true);
                        }
                    }
                    else
                    {
                        Verifier.Verify(_afterTextIndent == _beforeTextIndent,
                            "Verifying TextIndent " + logString + " (beforeIndent>=20)", true);

                        //The default is Double.NaN
                        if (double.IsNaN(_beforeLeftMargin))
                        {
                            Verifier.Verify(_afterLeftMargin == 20, "Verifying LeftMargin " +
                                logString + " (beforeIndent>=20)", true);
                        }
                        else
                        {
                            Verifier.Verify(_afterLeftMargin == _beforeLeftMargin + 20,
                                "Verifying LeftMargin " + logString + " (beforeIndent>=20)", true);
                        }
                    }
                }
            }
            else
            {
                Verifier.Verify(_testButton2.IsKeyboardFocused,
                    "Verifying that focus shifted for Tab", true);
                Verifier.Verify(_afterState == _beforeState,
                    "Verifying that tab is not accepted when AcceptsTab is False", true);
                _rtb.Focus();
            }
        }

        private void VerifyIndentAndMarginOnShiftTab(string logString)
        {
            if (_testValue)
            {
                Verifier.Verify(_rtb.IsKeyboardFocused,
                    "Verifying that focus did not shifted " + logString, true);

                Verifier.Verify(_afterState == _beforeState,
                    "Verifying that shift+tab is not accepted even when AcceptsTab is True " + logString, true);

                if (!_rtb.Selection.IsEmpty)
                {
                    Verifier.Verify(_beforeTextIndent == _afterTextIndent,
                        "Verify that TextIndent didnt change for ShiftTab operation for non-empty selection " + logString, true);

                    if (_beforeLeftMargin >= 20)
                    {
                        Verifier.Verify(_afterLeftMargin == _beforeLeftMargin - 20,
                            "Verifying that Margin is decreased by 20 " + logString, true);
                    }
                    else
                    {
                        Verifier.Verify(_afterLeftMargin == 0, "Verifying that Margin = 0 " + logString, true);
                    }
                }
                else
                {
                    if (_beforeTextIndent > 20)
                    {
                        Verifier.Verify(_afterTextIndent == 20, "Verifying TextIndent " + logString +
                            " (beforeIndent>20)", true);

                        if (double.IsNaN(_beforeLeftMargin))
                        {
                            Verifier.Verify(double.IsNaN(_afterLeftMargin),
                                "Verifying that Margin didnt change if TextIndent is changed " + logString + " (beforeIndent>20)", true);
                        }
                        else
                        {
                            Verifier.Verify(_afterLeftMargin == _beforeLeftMargin,
                                "Verifying that Margin didnt change if TextIndent is changed " + logString + " (beforeIndent>20)", true);
                        }
                    }
                    else if (_beforeTextIndent > 0)
                    {
                        Verifier.Verify(_afterTextIndent == 0, "Verifying TextIndent " + logString +
                            " (beforeIndent>0)", true);

                        if (double.IsNaN(_beforeLeftMargin))
                        {
                            Verifier.Verify(double.IsNaN(_afterLeftMargin),
                                "Verifying that Margin didnt change if TextIndent is changed " + logString + " (beforeIndent20)", true);
                        }
                        else
                        {
                            Verifier.Verify(_afterLeftMargin == _beforeLeftMargin,
                                "Verifying that Margin didnt change if TextIndent is changed " + logString + " (beforeIndent20)", true);
                        }
                    }
                    else
                    {
                        Verifier.Verify(_afterTextIndent == 0, "Verifying TextIndent " + logString +
                            " (beforeIndent=0)", true);

                        if (_beforeLeftMargin >= 20)
                        {
                            Verifier.Verify(_afterLeftMargin == _beforeLeftMargin - 20,
                                "Verifying that Margin is decreased by 20 " + logString, true);
                        }
                        else
                        {
                            Verifier.Verify(_afterLeftMargin == 0, "Verifying that Margin = 0 " + logString, true);
                        }
                    }
                }
            }
            else
            {
                Verifier.Verify(_testButton1.IsKeyboardFocused,
                    "Verifying that focus shifted for ShiftTab", true);
                Verifier.Verify(_afterState == _beforeState,
                    "Verifying that ShiftTab is not accepted when AcceptsTab is False", true);
                _rtb.Focus();
            }
        }
    }

    /// <summary>
    /// Verifies that AcceptsReturn property work as expected
    /// </summary>
    [Test(0, "TextBox", "TestAcceptsReturn", MethodParameters = "/TestCaseType=TestAcceptsReturn")]
    [TestOwner("Microsoft"), TestTactics("623"), TestBugs(""), TestWorkItem("116")]
    public class TestAcceptsReturn : CustomTestCase
    {
        #region PrivateMembers
        /// <summary>Combinatorial engine for values.</summary>
        private CombinatorialEngine _combinatorials;
        private TextEditableType _textEditableType;
        private bool _testValue;
        private string _textContent;

        private Control _testElement;
        private UIElementWrapper _testWrapper;
        private StackPanel _panel;

        private string _stateBeforeTestOperation;
        private string _expState;
        #endregion

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            Dimension[] dimensions; // Dimensions for combinations.
            dimensions = new Dimension[] {
                new Dimension("TextEditableType", TextEditableType.Values),
                new Dimension("Text", new object[] {String.Empty,"abc"}),
                new Dimension("AcceptsReturn", new object[]{null,false,true})
            };
            _combinatorials = CombinatorialEngine.FromDimensions(dimensions);

            _panel = new StackPanel();
            MainWindow.Content = _panel;
            QueueDelegate(RunNextTestCase);
        }

        private void RunNextTestCase()
        {
            Hashtable combinationValues;   // Values for combination.

            combinationValues = new Hashtable();
            if (_combinatorials.Next(combinationValues))
            {
                _textEditableType = (TextEditableType)combinationValues["TextEditableType"];
                if (!typeof(TextBoxBase).IsAssignableFrom(_textEditableType.Type))
                {
                    RunNextTestCase();
                    return;
                }
                _testElement = (Control)_textEditableType.CreateInstance();
                _testWrapper = new UIElementWrapper(_testElement);
                _textContent = (string)combinationValues["Text"];

                if (combinationValues["AcceptsReturn"] != null)
                {
                    _testValue = (bool)combinationValues["AcceptsReturn"];
                    ((TextBoxBase)_testElement).AcceptsReturn = _testValue;
                }
                else
                {
                    //Verify with the default state for TextBox/RichTextBox.
                    _testValue = (_testElement is RichTextBox) ? true : false;

                    //For subclasses verify with the value it is customed to.
                    if (_textEditableType.IsSubClass)
                    {
                        _testValue = ((TextBoxBase)_testElement).AcceptsReturn;
                    }
                }

                if (_textContent != String.Empty)
                {
                    _testWrapper.Text = _textContent;
                }

                _testElement.Height = 300;
                _testElement.Width = 300;
                _testElement.FontSize = 16;

                _panel.Children.Clear();
                _panel.Children.Add(_testElement);

                Log("---- TextEditableType: " + _textEditableType.XamlName +
                    " ---- AcceptsReturn: " + _testValue +
                    " ---- Text: " + _textContent);

                QueueDelegate(PerformAction);
            }
            else
            {
                Logger.Current.ReportSuccess();
                return;
            }
        }

        private void PerformAction()
        {
            _testElement.Focus();
            _stateBeforeTestOperation = _testWrapper.Text;
            KeyboardInput.TypeString("{END}{ENTER}" + _textContent);
            QueueDelegate(VerifyResult);
        }

        private void VerifyResult()
        {
            if (_testValue == false)
            {
                if (_testElement is TextBox)
                {
                    Verifier.Verify((_testWrapper.Text == _stateBeforeTestOperation + _textContent),
                        "Verifying that Enter is not accepted in TextBox", true);
                }
                else
                {
                    _expState = "<Paragraph><Run>" + _stateBeforeTestOperation.Substring(0, _stateBeforeTestOperation.Length - 2) + _textContent + "</Run></Paragraph>";
                    Log("testWrapper.Text [" + _testWrapper.Text + "]\r\n ExpectedText [" + _stateBeforeTestOperation + _textContent + "] \r\n");
                    Log("Actual XAML[" + _testWrapper.GetPlainTextRepresentation(false) + "] \r\n ExpectedXAML [" + _expState + "] \r\n");
                    Verifier.Verify((_testWrapper.Text == _stateBeforeTestOperation + _textContent) ||
                        (_testWrapper.GetPlainTextRepresentation(false) == _expState),
                        "Verifying that Enter is not accepted in RichTextBox", true);
                }
            }
            else
            {
                if (_testElement is TextBox)
                {
                    Verifier.Verify(_testWrapper.Text == _stateBeforeTestOperation + "\r\n" + _textContent,
                        "Verify that Enter is accepted in TextBox", true);
                }
                else
                {
                    if (_textContent == "") // Empty paragraph doesn't have <Run>. See 
                    {
                        _expState = "<Paragraph>" + _stateBeforeTestOperation.Substring(0, _stateBeforeTestOperation.Length - 2)
                            + "</Paragraph><Paragraph>" + _textContent + "</Paragraph>";
                    }
                    else
                    {
                        _expState = "<Paragraph><Run>" + _stateBeforeTestOperation.Substring(0, _stateBeforeTestOperation.Length - 2)
                            + "</Run></Paragraph><Paragraph><Run>" + _textContent + "</Run></Paragraph>";
                    }
                    Verifier.Verify(_testWrapper.GetPlainTextRepresentation(false) == _expState,
                        "Verifying that Enter is accepted in RichTextBox", true);
                }
            }
            QueueDelegate(RunNextTestCase);
        }
    }

    /// <summary>
    /// Verifies that setting AcceptsReturn to false activates
    /// the default control.
    /// </summary>
    [Test(3, "TextBox", "TextBoxAcceptsReturnDefaultControl", MethodParameters = "/TestCaseType:TextBoxAcceptsReturnDefaultControl")]
    [TestOwner("Microsoft"), TestTactics("622"), TestWorkItem("116")]
    public class TextBoxAcceptsReturnDefaultControl : CustomTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            StackPanel panel;
            Button button;

            _childWindow = new Window();
            button = new Button();
            _control = new TextBox();
            panel = new StackPanel();

            button.Content = "Press Enter to close this window";
            button.IsDefault = true;
            button.Click += delegate { _childWindow.Close(); };

            panel.Children.Add(_control);
            panel.Children.Add(button);
            _childWindow.Content = panel;

            QueueDelegate(TypeText);
            _childWindow.ShowDialog();
        }

        private void TypeText()
        {
            InputMonitorManager.Current.IsEnabled = false;

            MouseInput.MouseClick(_control);
            KeyboardInput.TypeString("First line.{ENTER}Second line.");
            QueueDelegate(VerifyNewline);
        }

        private void VerifyNewline()
        {
            string text;
            int newlinePos;

            text = _control.Text;
            newlinePos = text.IndexOf("\n");
            Verifier.Verify(newlinePos == -1,
                "Newline not found; position = " + newlinePos, true);
            Verifier.Verify(!_childWindow.IsActive,
                "Dismissed window is no longer active.", true);

            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.

        #region Private fields.

        private Window _childWindow;
        private TextBox _control;

        #endregion Private fields.
    }

    /// <summary>
    /// Verifies that the text box Wrap property works correctly.
    /// </summary>
    /// <remarks>
    /// Note that an inverted scheme (light letters on dark background)
    /// will cause this test case to misbehave.
    /// </remarks>
    [Test(0, "TextBox", "TextBoxWrapVisual", MethodParameters = "/TestCaseType=TextBoxWrapVisual")]
    [TestOwner("Microsoft"), TestTactics("620")]
    public class TextBoxWrapVisual : TextBoxTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            TestTextBox.BorderThickness = new Thickness(0);
            TestTextBox.BorderBrush = System.Windows.Media.Brushes.Transparent;
            TestTextBox.TextWrapping = TextWrapping.NoWrap;
            Verifier.Verify(TestTextBox.TextWrapping == TextWrapping.NoWrap,
                "Wrap property is persisted correctly.", true);
            TestTextBox.FontSize = 30;
            QueueDelegate(DoInput);
        }

        private void DoInput()
        {
            TestTextBox.Focus();
            KeyboardInput.TypeString("AAAAAAAAAA BBBBBBBBBB CCCCCCCCCC DDDDDDDDDD EEEEEEEEEE FFFFFFFFFF");
            QueueDelegate(CheckTextUnwrapped);
        }

        private void CheckTextUnwrapped()
        {
            Verifier.Verify(TestTextBox.TextWrapping == TextWrapping.NoWrap,
                "Wrap property is persisted correctly.", true);
            Verifier.Verify(TestTextBox.Text.Length > 0,
                "Text has been typed.", true);
            Log("TextBox text: " + TestTextBox.Text);
            VerifySingleLine();

            TestTextBox.TextWrapping = TextWrapping.Wrap;
            Verifier.Verify(TestTextBox.TextWrapping == TextWrapping.Wrap,
                "Wrap property is persisted correctly.", true);
            QueueDelegate(CheckTextWrapped);
        }

        private void CheckTextWrapped()
        {
            VerifyMultipleLines();
            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.

        #region Verifications.

        private Bitmap CountLines(out int lineCount)
        {
            return TextBoxRenderTyping.CountLines(TestTextBox, out lineCount);
        }

        private void VerifySingleLine()
        {
            int lineCount;
            Bitmap b1 = BitmapCapture.CreateBitmapFromElement(TestTextBox);
            Bitmap b = BitmapUtils.CreateBorderlessBitmap(b1, 2);

            lineCount = BitmapUtils.CountTextLines(b);
            if (lineCount == 1)
                Log("A single line is found when unwrapped.");
            else
            {
                Logger.Current.LogImage(b, "single");
                throw new Exception(String.Format(
                    "{0} lines found when 1 was expected when unwrapped.",
                    lineCount));
            }
        }

        private void VerifyMultipleLines()
        {
            int lineCount;
            using (Bitmap b = CountLines(out lineCount))
            {
                if (lineCount > 1)
                    Log("Multiple lines are found when wrapped.");
                else
                {
                    Logger.Current.LogImage(b, "multiple");
                    throw new Exception(String.Format(
                        "{0} lines found when >1 was expected when wrapped.",
                        lineCount));
                }
            }
        }

        #endregion Verifications.
    }

    /// <summary>
    /// Verifies that MaxLength limits regular typing, overwriting,
    /// pasting, dropping, undoing and redoing, but not text
    /// assignment.
    /// </summary>
    [Test(0, "TextBox", "TextBoxMaxLengthTest", MethodParameters = "/TestCaseType:TextBoxMaxLengthTest")]
    [TestOwner("Microsoft"), TestTactics("621"), TestBugs("776,777, 778, 779"), TestWorkItem("115")]
    public class TextBoxMaxLengthTest : TextBoxTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            TestTextBox.Focus();
            Log("Setting TextBox.MaxLength to 3 characters...");
            TestTextBox.MaxLength = 3;

            Log("Typing 4 characters...");
            KeyboardInput.TypeString("1234");
            QueueDelegate(CheckSimpleType);
        }

        private void CheckSimpleType()
        {
            Verifier.Verify(TestTextBox.Text == "123",
                "3 characters found.", true);

            Log("Overwriting middle character...");
            KeyboardInput.TypeString("{LEFT 2}+{RIGHT}4");
            QueueDelegate(CheckOverwrite);
        }

        private void CheckOverwrite()
        {
            Verifier.Verify(TestTextBox.Text == "143",
                "3 characters found with middle one overwritten.", true);

            Log("Typing for overflow in middle character...");
            KeyboardInput.TypeString("5");
            QueueDelegate(CheckOverwriteOverflow);
        }

        private void CheckOverwriteOverflow()
        {
            Verifier.Verify(TestTextBox.Text == "143",
                "No changes found in text box.", true);

            Log("Pasting to partially overflow...");
            KeyboardInput.TypeString("{END}+{LEFT 2}^c{END}+{LEFT}^v");
            QueueDelegate(CheckPasteOverflow);
        }

        private void CheckPasteOverflow()
        {
            Verifier.Verify(TestTextBox.Text == "144", "Text is '144' as expected.", true);

            Log("Pasting to overflow at start...");
            KeyboardInput.TypeString("^a123{END}+{LEFT 2}^c{HOME}{DEL}^v");
            QueueDelegate(CheckOverflowAtStart);
        }

        private void CheckOverflowAtStart()
        {
            Verifier.Verify(TestTextBox.Text == "223",
               "Text is '223' as expected.", true);

            Log("Dropping text onto full TextBox...");
            TestTextBox.AllowDrop = true;
            TestTextBox.Text = "123";
            TestTextBoxAlt.Text = "abc";

            MouseInput.MouseClick(TestTextBoxAlt);
            KeyboardInput.TypeString("^a");
            ActionItemWrapper.MouseElementRelative(TestTextBoxAlt, "move 10 10");
            QueueDelegate(StartDrag);
        }

        private void StartDrag()
        {
            // Do this outside of the context of the input manager.
            InputMonitorManager.Current.IsEnabled = false;
            ActionItemWrapper.MouseElementRelative(TestTextBox, "pressdrag left 10 10");

            QueueDelegate(CheckDropOverflow);
        }

        private void CheckDropOverflow()
        {
            Verifier.Verify(TestTextBox.Text == "123",
                "Text has not changed, as expected.", true);

            // Re-enable the input manager.
            InputMonitorManager.Current.IsEnabled = true;

            Log("Checking that MaxLength does not prevent programmatic assignment...");
            TestTextBox.Text = "abcd";
            Verifier.Verify(TestTextBox.Text == "abcd",
                "TextBox can have 'abcd' value assigned.", true);

            Log("Checking that typing with invalid text does not modify text...");
            MouseInput.MouseClick(TestTextBox);
            KeyboardInput.TypeString("{HOME}a");

            QueueDelegate(CheckInvalidTextRemains);
        }

        private void CheckInvalidTextRemains()
        {
            Verifier.Verify(TestTextBox.Text == "abcd",
                "Text has not changed, as expected.", true);

            Log("Checking that Undo allows user to restore invalid text...");
            KeyboardInput.TypeString("^a{DEL}^z");

            QueueDelegate(CheckUndoRestoresInvalidText);
        }

        private void CheckUndoRestoresInvalidText()
        {
            string surrogatePair = StringData.SurrogatePair.Value;
            Verifier.Verify(TestTextBox.Text == "abcd",
                "Text was restored as expected.", true);

            TestTextBox.Text = "ab";
            TestTextBoxAlt.Text = surrogatePair;
            TestTextBoxAlt.SelectAll();
            TestTextBoxAlt.Copy();
            QueueDelegate(PasteMultiByteCharacter);
        }

        private void PasteMultiByteCharacter()
        {
            //paste a multi-byte character at the end of TestTextBox.
            KeyboardInput.TypeString("{END}^v");
            QueueDelegate(TestMultiByteCharacterOverFlowBehavior);
        }

        private void TestMultiByteCharacterOverFlowBehavior()
        {
            string highOrderSurrogatePair = StringData.SurrogatePair.Value.Substring(0, 1);
            //When pasting a surrogate pair on a textbox which has space
            //for just one more character (due to MaxLength constraint)
            //nothing will be pasted. WinForm's TextBox paste only the high-order byte
            Verifier.Verify(TestTextBox.Text == "ab",
                "Verifying behavior of overflow when pasting multi-byte character [" + TestTextBox.Text + "]", true);
            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.
    }

    /// <summary>
    /// Verifies that clicking with mouse can move the caret
    /// from end-of-line to beginning-of-line for the same
    /// position.
    /// </summary>
    [Test(3, "Editor", "TextBoxRepro656", MethodParameters = "/TestCaseType=TextBoxReproRegression_Bug656")]
    [TestOwner("Microsoft"), TestTactics("619"), TestBugs("656")]
    public class TextBoxReproRegression_Bug656 : TextBoxTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            SetTextBoxProperties(TestTextBox);
            TestWrapper.Text = "ab";
            TestTextBox.FontSize = 196 * (96.0 / 72.0);
            TestTextBox.TextWrapping = TextWrapping.Wrap;

            QueueDelegate(LayoutPassed);
        }

        private void LayoutPassed()
        {
            Rect r; // Rectangle for first character.

            Log("Clicking on the right-hand side of the first character...");
            r = TestWrapper.GetGlobalCharacterRect(TestWrapper.Start, LogicalDirection.Forward);
            Log("First character rectangle: " + r);
            MouseInput.MouseClick((int)(r.Left + r.Width * 0.7),
                (int)(r.Top + r.Height / 2));

            QueueDelegate(LineOneClicked);
        }

        private void LineOneClicked()
        {
            Rect r; // Rectangle for second character.

            Log("Capturing control bitmap...");
            _lineOneClick = BitmapCapture.CreateBitmapFromElement(TestControl);

            Log("Clicking on the left-hand side of the second character...");
            r = TestWrapper.GetGlobalCharacterRect(TestWrapper.Start.GetPositionAtOffset(2), LogicalDirection.Backward);
            Log("Second character rectangle: " + r);
            MouseInput.MouseClick((int)(r.Left + r.Width * 0.2),
                (int)(r.Top + r.Height / 2));

            QueueDelegate(LineTwoClicked);
        }

        private void LineTwoClicked()
        {
            Bitmap lineTwoClick;
            Bitmap delta;

            Log("Capturing control bitmap...");
            lineTwoClick = BitmapCapture.CreateBitmapFromElement(TestControl);

            Log("Verifying that rendering has changed in control...");
            if (ComparisonOperationUtils.AreBitmapsEqual(
                _lineOneClick, lineTwoClick, out delta))
            {
                Logger.Current.LogImage(_lineOneClick, "control");
                Log("Bitmaps are identical.");
                throw new Exception("Bitmaps are identical.");
            }
            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.

        #region Private fields.

        private Bitmap _lineOneClick;

        #endregion Private fields.
    }

    /// <summary>Test class for CutCopyPaste commands emulation </summary>
    [Test(0, "TextBoxBase", "TextBoxOMCutCopyPaste", MethodParameters = "/TestCaseType:TextBoxOMCutCopyPaste", Keywords = "Localization_Suite")]
    [TestOwner("Microsoft"), TestTactics("421"), TestWorkItem("65"), TestBugs("464")]
    public class TextBoxOMCutCopyPaste : ManagedCombinatorialTestCase
    {
        /// <summary>initialization of the run</summary>
        protected override void DoRunCombination()
        {
            _element = _editableType.CreateInstance();
            _ControlWrapper = new UIElementWrapper(_element);
            _ControlWrapper.Clear();
            if (_element is TextBoxBase)
            {
                ((TextBoxBase)_element).AcceptsReturn = true;
            }
            TestElement = _element;
            QueueDelegate(DoFocus);
        }

        /// <summary>Focus on element</summary>
        private void DoFocus()
        {
            _element.Focus();
            ExecuteTrigger();
        }

        /// <summary>Program Controller</summary>
        private void ExecuteTrigger()
        {
            SetText();
            Clipboard.Clear();
            switch (_selectTextOptionsSwitch)
            {
                case SelectTextOptions.SelectEmptyText:
                    _ControlWrapper.Text = String.Empty;
                    _initialString = _ControlWrapper.Text;
                    _ControlWrapper.SelectAll();
                    switch (_cutCopyPasteSwitch)
                    {
                        case CutCopyPasteOperations.CopyPaste:
                            SelectEmptyTextCopyPaste();
                            break;

                        case CutCopyPasteOperations.CutPaste:
                            SelectEmptyTextCutPaste();
                            break;

                        case CutCopyPasteOperations.CutCopyPaste:
                            SelectEmptyTextCutCopyPaste();
                            break;
                    }
                    break;

                case SelectTextOptions.SelectAll:
                    _initialString = _ControlWrapper.Text;
                    _ControlWrapper.SelectAll();
                    switch (_cutCopyPasteSwitch)
                    {
                        case CutCopyPasteOperations.CopyPaste:
                            SelectAllTextCopyPaste();
                            break;

                        case CutCopyPasteOperations.CutPaste:
                            SelectAllTextCutPaste();
                            break;

                        case CutCopyPasteOperations.CutCopyPaste:
                            SelectAllTextCutCopyPaste();
                            break;
                    }
                    break;

                case SelectTextOptions.SelectEnd:
                    _initialString = _ControlWrapper.Text;
                    _element.Focus();
                    KeyboardInput.TypeString("{RIGHT 4}^+{END}");
                    switch (_cutCopyPasteSwitch)
                    {
                        case CutCopyPasteOperations.CopyPaste:
                            QueueDelegate(TextEndCopyPaste);
                            break;

                        case CutCopyPasteOperations.CutPaste:
                            QueueDelegate(TextEndCutPaste);
                            break;

                        case CutCopyPasteOperations.CutCopyPaste:
                            QueueDelegate(TextEndCutCopyPaste);
                            break;
                    }
                    break;

                case SelectTextOptions.SelectMiddle:
                    _initialString = _ControlWrapper.Text;
                    _ControlWrapper.Select(30, 12);
                    switch (_cutCopyPasteSwitch)
                    {
                        case CutCopyPasteOperations.CopyPaste:
                            TextMiddleCopyPaste();
                            break;

                        case CutCopyPasteOperations.CutPaste:
                            TextMiddleCutPaste();
                            break;

                        case CutCopyPasteOperations.CutCopyPaste:
                            TextMiddleCutCopyPaste();
                            break;
                    }
                    break;

                case SelectTextOptions.NoSelection:
                    _initialString = _ControlWrapper.Text;
                    switch (_cutCopyPasteSwitch)
                    {
                        case CutCopyPasteOperations.CopyPaste:
                            TextNoSelectionCopyPaste();
                            break;

                        case CutCopyPasteOperations.CutPaste:
                            TextNoSelectionCutPaste();
                            break;

                        case CutCopyPasteOperations.CutCopyPaste:
                            TextNoSelectionCutCopyPaste();
                            break;
                    }
                    break;

                case SelectTextOptions.PasswordPaste:
                    if (_element is PasswordBox)
                    {
                        _initialString = _ControlWrapper.Text;
                        ((PasswordBox)_element).Password = _hello;
                        Clipboard.SetDataObject(_world);
                        PasswordBoxPaste();
                    }
                    NextCombination();
                    break;

                default:
                    break;
            }
        }

        /// <summary>Copy paste operation with no text and performing select all</summary>
        private void SelectEmptyTextCopyPaste()
        {
            PerformCopyOnSelectedText();
            QueueDelegate(PerformPasteAndVerifyForSelectEmptyTextAndSelectAll);
        }

        /// <summary>Cut paste operation with no text and performing select all</summary>
        private void SelectEmptyTextCutPaste()
        {
            PerformCutOnSelectedText();
            QueueDelegate(PerformPasteAndVerifyForSelectEmptyTextAndSelectAll);
        }

        /// <summary>Cut Copy paste operation with no text and performing select all</summary>
        private void SelectEmptyTextCutCopyPaste()
        {
            PerformCutOnSelectedText();
            PerformCopyOnSelectedText();
            QueueDelegate(PerformPasteAndVerifyForSelectEmptyTextAndSelectAll);
        }

        /// <summary>Verify operation with text/no text and performing select all</summary>
        private void PerformPasteAndVerifyForSelectEmptyTextAndSelectAll()
        {
            PerformPasteAtCurrentCaretLocation();
            _finalString = _ControlWrapper.Text;
            if (((_cutCopyPasteSwitch == CutCopyPasteOperations.CutPaste) || (_cutCopyPasteSwitch == CutCopyPasteOperations.CutCopyPaste))
                && (_element is RichTextBox))
            {
                //When you perform cut on all text \r\n is still left behind so pasting will paste text before the esisiting \r\n
                //  _initialString = (SelectTextOptionsSwitch == SelectTextOptions.SelectAll)? (_initialString + "\r\n"):_initialString;
                //NOW WE DO NOT HAVE AN EMPTY RUN!!!
                Verifier.Verify(_initialString == _finalString, "Strings should be equal. Expected[" + _initialString +
                                "] Actual String[" + _finalString + "]", true);
            }
            else
            {
                Verifier.Verify(_initialString == _finalString, "Strings should be equal. Expected[" + _initialString +
                "] Actual String[" + _finalString + "]", true);
            }
            NextCombination();
        }

        /// <summary>Copy paste operation with text and performing select all</summary>
        private void SelectAllTextCopyPaste()
        {
            SelectEmptyTextCopyPaste();
        }

        /// <summary>Cut paste operation with text and performing select all</summary>
        private void SelectAllTextCutPaste()
        {
            SelectEmptyTextCutPaste();
        }

        /// <summary>Cut Copy paste operation with text and performing select all</summary>
        private void SelectAllTextCutCopyPaste()
        {
            SelectEmptyTextCutCopyPaste();
        }

        /// <summary>Copy paste operation performing selection in middle</summary>
        private void TextMiddleCopyPaste()
        {
            PerformCopyOnSelectedText();
            QueueDelegate(PerformPasteAndVerifyTextMiddle);
        }

        /// <summary>Cut paste operation performing selection in middle</summary>
        private void TextMiddleCutPaste()
        {
            PerformCutOnSelectedText();
            QueueDelegate(PerformPasteAndVerifyTextMiddle);
        }

        /// <summary>Cut Copy paste operation performing selection in middle</summary>
        private void TextMiddleCutCopyPaste()
        {
            PerformCutOnSelectedText();
            PerformCopyOnSelectedText();
            QueueDelegate(PerformPasteAndVerifyTextMiddle);
        }

        /// <summary>Verify operation performing selection in middle</summary>
        private void PerformPasteAndVerifyTextMiddle()
        {
            PerformPasteAtCurrentCaretLocation();
            PerformPasteAtCurrentCaretLocation();
            _finalString = _ControlWrapper.Text;
            AdjustInitialStringForTwoPasteOperations();
            Verifier.Verify(_initialString == _finalString, "The strings are not equal. Expected [" + _initialString +
                "] Actual [" + _finalString + "]");
            NextCombination();

        }

        /// <summary>Modification of strings for comparison due to paragraphs in richtextbox</summary>
        private void AdjustInitialStringForTwoPasteOperations()
        {
            if (_element is RichTextBox)
            {
                //the \r\n account to 4 offsets since its in 2 different paras. 3 offsets if in same run
                _initialString = "This is a test for cut copy paste\r\nThipaste\r\nThis is the second run\r\n";
            }
            else if (_element is TextBox)
            {
                // \r\n accounts for 2 offsets
                _initialString = "This is a test for cut copy paste\r\nThis isste\r\nThis is the second run";
            }
        }

        /// <summary>Copy paste operation performing selection in end of doc</summary>
        private void TextEndCopyPaste()
        {
            PerformCopyOnSelectedText();
            QueueDelegate(PerformPasteAndVerifyTextEnd);
        }

        /// <summary>Cut paste operation performing selection in end of doc</summary>
        private void TextEndCutPaste()
        {
            PerformCutOnSelectedText();
            QueueDelegate(PerformPasteAndVerifyTextEnd);
        }

        /// <summary>Cut Copy paste operation performing selection in end of doc</summary>
        private void TextEndCutCopyPaste()
        {
            PerformCopyOnSelectedText();
            PerformCutOnSelectedText();
            QueueDelegate(PerformPasteAndVerifyTextEnd);
        }

        /// <summary>Verify operation performing selection in end of doc</summary>
        private void PerformPasteAndVerifyTextEnd()
        {
            PerformPasteAtCurrentCaretLocation();
            PerformPasteAtCurrentCaretLocation();
            _finalString = _ControlWrapper.Text;
            AdjustInitialStringForTwoPasteEndOperations();
            Verifier.Verify(_initialString == _finalString, "The strings are not equal. Expected [" + _initialString +
                "] Actual [" + _finalString + "]");
            NextCombination();
        }

        /// <summary>Modification of strings for comparison due to paragraphs in richtextbox</summary>
        private void AdjustInitialStringForTwoPasteEndOperations()
        {
            if (_element is RichTextBox)
            {
                if ((_cutCopyPasteSwitch == CutCopyPasteOperations.CutPaste || _cutCopyPasteSwitch == CutCopyPasteOperations.CutCopyPaste))
                {
                    //copying and pasting end of document places cursor before end
                    //cut and paste of end of document places cursor after end (\r\n) of doc 
                    //at the end there are 2 \r\n's ---> when u cut \r\n is saved but \r\n is created for existing line
                    //when u paste line \r\n is copied bacn and the existing \r\n is put at the end of doc with caret one position before doc end
                    _initialString = "This is a test for cut copy paste\r\nThis is the second run\r\n is a test for cut copy paste\r\nThis is the second run\r\n\r\n";
                }
                else
                {
                    //the \r\n account to 4 offsets since its in 2 different paras. 3 offsets if in same run
                    _initialString = "This is a test for cut copy paste\r\nThis is the second run is a test for cut copy paste\r\nThis is the second run\r\n\r\n";
                }
            }
            else if (_element is TextBox)
            {
                // \r\n accounts for 2 offsets
                _initialString = "This is a test for cut copy paste\r\nThis is the second run is a test for cut copy paste\r\nThis is the second run";
            }
        }

        /// <summary>Copy Paste operation with no selection</summary>
        private void TextNoSelectionCopyPaste()
        {
            PerformCopyOnSelectedText();
            QueueDelegate(PerformPasteAndVerifyTextNoSelection);
        }

        /// <summary>Cut Paste operation with no selection</summary>
        private void TextNoSelectionCutPaste()
        {
            PerformCutOnSelectedText();
            QueueDelegate(PerformPasteAndVerifyTextNoSelection);
        }

        /// <summary>Cut Copy Paste operation with no selection</summary>
        private void TextNoSelectionCutCopyPaste()
        {
            PerformCopyOnSelectedText();
            PerformCutOnSelectedText();
            QueueDelegate(PerformPasteAndVerifyTextNoSelection);
        }

        /// <summary>Verify operation with no selection</summary>
        private void PerformPasteAndVerifyTextNoSelection()
        {
            PerformPasteAtCurrentCaretLocation();
            _finalString = _ControlWrapper.Text;
            Verifier.Verify(_initialString == _finalString, "Strings should be equal. Expected[" + _initialString +
        "] Actual String[" + _finalString + "]", true);
            NextCombination();
        }

        private void PasswordBoxPaste()
        {
            string _expectedString = _world + _hello;
            ((PasswordBox)_element).Paste();
            _finalString = _ControlWrapper.Text;
            Verifier.Verify(_expectedString == _finalString, "after paste the strings are not equal. Expected[" +
                _expectedString + "] Actual[" + _finalString + "]", true);
        }

        #region helper functions.

        /// <summary>initialization of the text</summary>
        private void SetText()
        {
            if (_element is RichTextBox)
            {
                Run r1 = new Run();
                r1.FontFamily = new System.Windows.Media.FontFamily("Times New Roman");
                r1.FontWeight = FontWeights.Bold;
                r1.FontSize = 40;
                r1.Text = "This is a test for cut copy paste";
                ((RichTextBox)_element).Document = new FlowDocument(new Paragraph(r1));

                ((RichTextBox)_element).Document.Blocks.Add(new Paragraph(new Run("This is the second run")));
            }
            else
            {
                _ControlWrapper.Text = "This is a test for cut copy paste\r\nThis is the second run";
            }
        }

        /// <summary>Paste operation</summary>
        private void PerformPasteAtCurrentCaretLocation()
        {
            if (_element is TextBoxBase)
            {
                ((TextBoxBase)_element).Paste();
            }
            else if (_element is PasswordBox)
            {
                ((PasswordBox)_element).Paste();
            }
            else
            {
                throw UnSupportedException("Paste ");
            }
        }

        /// <summary>Copy Operation</summary>
        private void PerformCopyOnSelectedText()
        {
            if (_element is TextBoxBase)
            {
                ((TextBoxBase)_element).Copy();
            }
            else if (_element is PasswordBox)
            {
                KeyboardEditingData[] data = KeyboardEditingData.GetValues(KeyboardEditingTestValue.CopyCommandKeys);
                data[0].PerformAction(_ControlWrapper, null, true);
            }
            else
            {
                throw UnSupportedException("CopyOnSelectedText");
            }
        }

        /// <summary>Perform Cut</summary>
        private void PerformCutOnSelectedText()
        {
            if (_element is TextBoxBase)
            {
                ((TextBoxBase)_element).Cut();
            }
            else if (_element is PasswordBox)
            {
                KeyboardEditingData[] data = KeyboardEditingData.GetValues(KeyboardEditingTestValue.CutCommandKeys);
                data[0].PerformAction(_ControlWrapper, null, true);
            }
            else
            {
                throw UnSupportedException("CutOnSelectedText");
            }

        }

        /// <summary>Exception for the above</summary>
        private Exception UnSupportedException(string member)
        {
            return new NotSupportedException(member + " not supported for elements of type " + _ControlWrapper.TypeName);
        }

        #endregion helper functions.

        #region private data.

        private UIElementWrapper _ControlWrapper;
        private TextEditableType _editableType = null;
        private FrameworkElement _element;
        private SelectTextOptions _selectTextOptionsSwitch = 0;
        private CutCopyPasteOperations _cutCopyPasteSwitch = 0;
        private string _initialString;
        private string _finalString;
        private string _hello = "hello";
        private string _world = "world";

        #endregion private data.
    }

#if TESTBUILD_NET_ATLEAST_462 // TextBoxCutCopyFailureExceptionTest is only relevant on .NET 4.6.2+

    /// <summary>
    /// Tests to verify that an ExternalException is throw by TextBoxBase.Cut and TextBoxBase.Copy methods
    /// when they fail. 
    /// 
    /// Typically, such failure happens when the clipboard is held opened (see Win32 OpenClipboard function) 
    /// by another application. 
    /// </summary>
    [Test(priority: 2, 
          subArea: "TextBoxBase", 
          Name = nameof(TextBoxCutCopyFailureExceptionTest), 
          MethodParameters = "/TestCaseType:" + nameof(TextBoxCutCopyFailureExceptionTest), 
          Keywords = "Localization_Suite", 
          Versions = "4.6.2+")]
    public class TextBoxCutCopyFailureExceptionTest : CustomTestCase
    {
        private TextBox _textBox;

        public override void RunTestCase()
        {
            QueueDelegate(CreateTextBox);
            QueueDelegate(SelectAll);
            QueueDelegate(() => 
            {
                DoTest(_textBox.Copy, null);
            });
            QueueDelegate(() =>
            {
                DoTest(_textBox.Cut, Logger.Current.QueueSuccess);
            });
        }

        private void CreateTextBox()
        {
            _textBox = new TextBox();
            _textBox.Text = "Hello World!"; // Any text would do here

            Log("Created textbox");

            var stackPanel = new StackPanel();
            stackPanel.Children.Add(_textBox);

            MainWindow.Content = stackPanel;
            Log("\tAdded textbox to MainWindow");
        }

        private void SelectAll()
        {
            _textBox.SelectAll();
            Log("Selected all text");
        }

        private void DoTest(Action testAction, Action finallyAction)
        {
            Log($"Starting test for operation: {testAction.Method.Name}");

            bool success = false;

            try
            {
                ShouldThrowOnCopyOrCutFailure = true;
                using (new ClipboardLocker())
                {
                    testAction();
                }
            }
            catch (ClipboardNativeOperationException cnoe) // Unable to start the test
            {
                Verifier.Verify(false, $"\tFailed to execute test. {cnoe.ToString()}", logAlways: true);
            }
            catch (ExternalException) // Test succeeded
            {
                success = true;
            }
            finally
            {
                Verifier.Verify(success, "\tExternalException thrown by the operation as expected", logAlways: true);
                ShouldThrowOnCopyOrCutFailure = false;
                finallyAction?.Invoke();
            }
        }

        private bool ShouldThrowOnCopyOrCutFailure
        {
            get
            {
                return FrameworkCompatibilityPreferences.ShouldThrowOnCopyOrCutFailure;
            }

            set
            {
                try
                {
                    FrameworkCompatibilityPreferences.ShouldThrowOnCopyOrCutFailure = value;
                }
                catch (InvalidOperationException)
                {
                    // FrameworkCompatibilityPreferences.ShouldThrowOnCopyOrCutFailure has been sealed. Try again using reflection. 
                    typeof(FrameworkCompatibilityPreferences)
                        .GetField("_shouldThrowOnCopyOrCutFailure", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                            .SetValue(null, value);
                }
            }
        }

        private class ClipboardLocker: IDisposable
        {
            public ClipboardLocker()
            {
                if (!Win32.SafeOpenClipboard(Win32.HWND.NULL))
                {
                    _disposed = true;
                    Dispose();
                    throw new ClipboardNativeOperationException();
                }
            }

            #region IDisposable Support

            private bool _disposed = false; // To detect redundant calls

            protected virtual void Dispose(bool disposing)
            {
                if (!_disposed)
                {
                    try
                    {
                        if (!Win32.SafeCloseClipboard())
                        {
                            throw new ClipboardNativeOperationException();
                        }
                    }
                    finally
                    {
                        _disposed = true;
                    }
                }
            }

            ~ClipboardLocker()
            {
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                Dispose(false);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            #endregion
        }

        [Serializable]
        public class ClipboardNativeOperationException : Exception
        {
            public ClipboardNativeOperationException(): base($"Error Code: {Win32.GetLastError()}")
            {
            }

            public ClipboardNativeOperationException(string message) : base(message)
            {
            }

            public ClipboardNativeOperationException(string message, Exception innerException) : base(message, innerException)
            {
            }

            protected ClipboardNativeOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }
    }

#endif // TESTBUILD_NET_ATLEAST_462

    /// <summary>Tests whether clearvalue() clears all values </summary>
    [Test(2, "TextBox", "TextBoxClearValue", MethodParameters = "/TestCaseType:TextBoxClearValue")]
    [TestOwner("Microsoft"), TestTactics("615"), TestWorkItem("113,112")]
    public class TextBoxClearValue : ManagedCombinatorialTestCase
    {

        /// <summary> filter for combinations read</summary>
        protected override bool DoReadCombination(Hashtable values)
        {
            bool result = base.DoReadCombination(values);
            if (_editableType == TextEditableType.GetByName("TextBox"))
                return true;
            return false;
        }

        /// <summary>initialization of the run</summary>
        protected override void DoRunCombination()
        {
            _element = _editableType.CreateInstance();
            _ControlWrapper = new UIElementWrapper(_element);
            _changeCount = 0;
            _expectedCount = 0;
            TestElement = _element;
            QueueDelegate(DoFocus);
        }

        /// <summary>Focus on element</summary>
        private void DoFocus()
        {
            _element.Focus();
            ExecuteTrigger();
        }

        /// <summary>Program Controller</summary>
        private void ExecuteTrigger()
        {
            // SetSwitch = SetOptions.SetThroughStyle;
            ObtainValues(_initialPropValues);
            switch (_setSwitch)
            {
                case SetOptions.SetThroughProperty:
                    {
                        SetValuesThroughProperty();
                        break;
                    }
                case SetOptions.SetThroughAccessor:
                    {
                        SetValuesThroughAccessor();
                        break;
                    }

                case SetOptions.SetThroughStyle:
                    {
                        SetValuesThroughStyle();
                        break;
                    }
            }
            QueueDelegate(CheckSetValues);
        }

        /// <summary>Set values through the style</summary>
        private void SetValuesThroughStyle()
        {

            Style style;
            int i = 0;
            style = new Style(typeof(TextBox));
            style.Setters.Add(new Setter(TextBox.AcceptsReturnProperty, true)); i++;
            style.Setters.Add(new Setter(TextBox.AcceptsTabProperty, true)); i++;
            style.Setters.Add(new Setter(TextBox.AllowDropProperty, false)); i++;
            style.Setters.Add(new Setter(TextBox.BackgroundProperty, System.Windows.Media.Brushes.Beige)); i++;
            style.Setters.Add(new Setter(TextBox.BorderBrushProperty, System.Windows.Media.Brushes.Blue)); i++;
            style.Setters.Add(new Setter(TextBox.BorderThicknessProperty, new Thickness(4))); i++;
            style.Setters.Add(new Setter(TextBox.CharacterCasingProperty, CharacterCasing.Upper)); i++;
            style.Setters.Add(new Setter(TextBox.FlowDirectionProperty, FlowDirection.RightToLeft)); i++;
            style.Setters.Add(new Setter(TextBox.FocusableProperty, false)); i++;
            style.Setters.Add(new Setter(TextBox.FontFamilyProperty, new System.Windows.Media.FontFamily("comic"))); i++;
            style.Setters.Add(new Setter(TextBox.FontSizeProperty, (double)(20))); i++;
            style.Setters.Add(new Setter(TextBox.FontStretchProperty, FontStretches.ExtraExpanded)); i++;
            style.Setters.Add(new Setter(TextBox.FontStyleProperty, FontStyles.Italic)); i++;
            style.Setters.Add(new Setter(TextBox.FontWeightProperty, FontWeights.Bold)); i++;
            style.Setters.Add(new Setter(TextBox.ForegroundProperty, System.Windows.Media.Brushes.Brown)); i++;
            style.Setters.Add(new Setter(TextBox.HeightProperty, (double)(300))); i++;
            style.Setters.Add(new Setter(TextBox.HorizontalAlignmentProperty, HorizontalAlignment.Center)); i++;
            style.Setters.Add(new Setter(TextBox.HorizontalContentAlignmentProperty, HorizontalAlignment.Right)); i++;
            style.Setters.Add(new Setter(TextBox.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Visible)); i++;
            style.Setters.Add(new Setter(TextBox.IsEnabledProperty, false)); i++;
            style.Setters.Add(new Setter(TextBox.MaxHeightProperty, (double)(2000))); i++;
            style.Setters.Add(new Setter(TextBox.MaxLengthProperty, (int)(200))); i++;
            style.Setters.Add(new Setter(TextBox.MaxLinesProperty, (int)(300))); i++;
            style.Setters.Add(new Setter(TextBox.MaxWidthProperty, (double)(4000))); i++;
            style.Setters.Add(new Setter(TextBox.MinHeightProperty, (double)(1))); i++;
            style.Setters.Add(new Setter(TextBox.MinLinesProperty, (int)(2))); i++;
            // name property cannot be set on the style - BC
            // style.Setters.Add(new Setter(TextBox.NameProperty, "hello")); i++;
            style.Setters.Add(new Setter(TextBox.TextAlignmentProperty, TextAlignment.Right)); i++;
            style.Setters.Add(new Setter(TextBox.TextProperty, "sample text")); i++;
            style.Setters.Add(new Setter(TextBox.TextWrappingProperty, TextWrapping.Wrap)); i++;
            style.Setters.Add(new Setter(TextBox.ToolTipProperty, "tooltip")); i++;
            _expectedCount = --i;
            ((TextBox)_element).Style = style;
        }

        /// <summary>Set values through the SetValue property</summary>
        private void SetValuesThroughProperty()
        {
            int i = 0;
            ((TextBox)_element).SetValue(TextBox.AcceptsReturnProperty, true); i++;
            ((TextBox)_element).SetValue(TextBox.AcceptsTabProperty, true); i++;
            ((TextBox)_element).SetValue(TextBox.AllowDropProperty, false); i++;
            ((TextBox)_element).SetValue(TextBox.BackgroundProperty, System.Windows.Media.Brushes.Beige); i++;
            ((TextBox)_element).SetValue(TextBox.BorderBrushProperty, System.Windows.Media.Brushes.Blue); i++;
            ((TextBox)_element).SetValue(TextBox.BorderThicknessProperty, new Thickness(4)); i++;
            ((TextBox)_element).SetValue(TextBox.CharacterCasingProperty, CharacterCasing.Upper); i++;
            ((TextBox)_element).SetValue(TextBox.FlowDirectionProperty, FlowDirection.RightToLeft); i++;
            ((TextBox)_element).SetValue(TextBox.FocusableProperty, false); i++;
            ((TextBox)_element).SetValue(TextBox.FontFamilyProperty, new System.Windows.Media.FontFamily("comic")); i++;
            ((TextBox)_element).SetValue(TextBox.FontSizeProperty, (double)(20)); i++;
            ((TextBox)_element).SetValue(TextBox.FontStretchProperty, FontStretches.ExtraExpanded); i++;
            ((TextBox)_element).SetValue(TextBox.FontStyleProperty, FontStyles.Italic); i++;
            ((TextBox)_element).SetValue(TextBox.FontWeightProperty, FontWeights.Bold); i++;
            ((TextBox)_element).SetValue(TextBox.ForegroundProperty, System.Windows.Media.Brushes.Brown); i++;
            ((TextBox)_element).SetValue(TextBox.HeightProperty, (double)(300)); i++;
            ((TextBox)_element).SetValue(TextBox.HorizontalAlignmentProperty, HorizontalAlignment.Center); i++;
            ((TextBox)_element).SetValue(TextBox.HorizontalContentAlignmentProperty, HorizontalAlignment.Right); i++;
            ((TextBox)_element).SetValue(TextBox.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Visible); i++;
            ((TextBox)_element).SetValue(TextBox.IsEnabledProperty, false); i++;
            ((TextBox)_element).SetValue(TextBox.MaxHeightProperty, (double)(2000)); i++;
            ((TextBox)_element).SetValue(TextBox.MaxLengthProperty, (int)(200)); i++;
            ((TextBox)_element).SetValue(TextBox.MaxLinesProperty, (int)(300)); i++;
            ((TextBox)_element).SetValue(TextBox.MaxWidthProperty, (double)(4000)); i++;
            ((TextBox)_element).SetValue(TextBox.MinHeightProperty, (double)(1)); i++;
            ((TextBox)_element).SetValue(TextBox.MinLinesProperty, (int)(2)); i++;
            ((TextBox)_element).SetValue(TextBox.NameProperty, "hello"); i++;
            ((TextBox)_element).SetValue(TextBox.TextAlignmentProperty, TextAlignment.Right); i++;
            ((TextBox)_element).SetValue(TextBox.TextProperty, "sample text"); i++;
            ((TextBox)_element).SetValue(TextBox.TextWrappingProperty, TextWrapping.Wrap); i++;
            ((TextBox)_element).SetValue(TextBox.ToolTipProperty, "tooltip"); i++;
            _expectedCount = --i;
        }

        /// <summary>Set values through the accessor</summary>
        private void SetValuesThroughAccessor()
        {
            int i = 0;
            ((TextBox)_element).AcceptsReturn = true; i++;
            ((TextBox)_element).AcceptsTab = true; i++;
            ((TextBox)_element).AllowDrop = false; i++;
            ((TextBox)_element).Background = System.Windows.Media.Brushes.Beige; i++;
            ((TextBox)_element).BorderBrush = System.Windows.Media.Brushes.Blue; i++;
            ((TextBox)_element).BorderThickness = new Thickness(4); i++;
            ((TextBox)_element).CharacterCasing = CharacterCasing.Upper; i++;
            ((TextBox)_element).FlowDirection = FlowDirection.RightToLeft; i++;
            ((TextBox)_element).Focusable = false; i++;
            ((TextBox)_element).FontFamily = new System.Windows.Media.FontFamily("comic"); i++;
            ((TextBox)_element).FontSize = (double)(20); i++;
            ((TextBox)_element).FontStretch = FontStretches.ExtraExpanded; i++;
            ((TextBox)_element).FontStyle = FontStyles.Italic; i++;
            ((TextBox)_element).FontWeight = FontWeights.Bold; i++;
            ((TextBox)_element).Foreground = System.Windows.Media.Brushes.Brown; i++;
            ((TextBox)_element).Height = (double)(300); i++;
            ((TextBox)_element).HorizontalAlignment = HorizontalAlignment.Center; i++;
            ((TextBox)_element).HorizontalContentAlignment = HorizontalAlignment.Right; i++;
            ((TextBox)_element).HorizontalScrollBarVisibility = ScrollBarVisibility.Visible; i++;
            ((TextBox)_element).IsEnabled = false; i++;
            ((TextBox)_element).MaxHeight = (double)(2000); i++;
            ((TextBox)_element).MaxLength = (int)(200); i++;
            ((TextBox)_element).MaxLines = (int)(300); i++;
            ((TextBox)_element).MaxWidth = (double)(4000); i++;
            ((TextBox)_element).MinHeight = (double)(1); i++;
            ((TextBox)_element).MinLines = (int)(2); i++;
            ((TextBox)_element).Name = "hello"; i++;
            ((TextBox)_element).TextAlignment = TextAlignment.Right; i++;
            ((TextBox)_element).Text = "sample text"; i++;
            ((TextBox)_element).TextWrapping = TextWrapping.Wrap; i++;
            ((TextBox)_element).ToolTip = "tooltip"; i++;
            _expectedCount = --i;
        }

        /// <summary>CheckSetValues</summary>
        private void CheckSetValues()
        {
            ObtainValues(_afterChangePropValues);
            VerifySetValues(_initialPropValues, _afterChangePropValues);
            Verifier.Verify(_errorString != "", "The setting of values did occur" + _errorString, true);
            Verifier.Verify(_changeCount == _expectedCount, "The number of values set didnt match # of set textbox values Expected[" +
             _expectedCount.ToString() + "] Actual[" + _changeCount.ToString() + "]" + _errorString1, false);
            QueueDelegate(ResetValues);
        }

        /// <summary>ResetValues</summary>
        private void ResetValues()
        {
            if (_setSwitch == SetOptions.SetThroughStyle)
            {
                ((TextBox)_element).ClearValue(TextBox.StyleProperty);
            }
            else
            {
                ClearValues();
            }
            QueueDelegate(CheckResetValues);
        }

        /// <summary>CheckResetValues</summary>
        private void CheckResetValues()
        {
            _element.Focus();
            ObtainValues(_afterClearPropValues);
            VerifySetValues(_initialPropValues, _afterClearPropValues);
            Verifier.Verify(_errorString == "", "The resetting of values did occur\r\n" + _errorString, true);

            NextCombination();
        }

        /// <summary>VerifySetValues</summary>
        public void VerifySetValues(object[] _initialValues, object[] _changedValues)
        {
            int length = _length;
            _errorString1 = _errorString = "";
            for (int i = 0; i < length; i++)
            {
                if ((_initialValues[i] != null) &&
               (_changedValues[i] != null))
                {
                    _errorString1 += i.ToString() + "------" + _initialValues[i].ToString() + "-------------" + _changedValues[i].ToString() + "\r\n";
                }
                if ((_initialValues[i] != _changedValues[i]) &&
                     (_initialValues[i] != null) &&
                     (_changedValues[i] != null))
                {
                    _errorString1 += i.ToString() + "------" + _initialValues[i].ToString() + "-------------" + _changedValues[i].ToString() + "\r\n";
                    if (_initialValues[i].ToString() != _changedValues[i].ToString())
                    {
                        _changeCount++;
                        _errorString += i.ToString() + "------" + _initialValues[i].ToString() + "-------------" + _changedValues[i].ToString() + "\r\n";
                    }
                }
            }
        }

        #region Helpers.

        /// <summary>Obtain values</summary>
        public void ObtainValues(object[] _values)
        {
            int i = 0;

            _values[i++] = ((TextBox)_element).GetValue(TextBox.AcceptsReturnProperty);
            _values[i++] = ((TextBox)_element).GetValue(TextBox.AcceptsTabProperty);
            _values[i++] = ((TextBox)_element).GetValue(TextBox.AllowDropProperty);
            _values[i++] = ((TextBox)_element).GetValue(TextBox.BackgroundProperty);
            _values[i++] = ((TextBox)_element).GetValue(TextBox.BorderBrushProperty);
            _values[i++] = ((TextBox)_element).GetValue(TextBox.BorderThicknessProperty);
            _values[i++] = ((TextBox)_element).GetValue(TextBox.CharacterCasingProperty);
            _values[i++] = ((TextBox)_element).GetValue(TextBox.FlowDirectionProperty);
            _values[i++] = ((TextBox)_element).GetValue(TextBox.FocusableProperty);
            _values[i++] = ((TextBox)_element).GetValue(TextBox.FontFamilyProperty);
            _values[i++] = ((TextBox)_element).GetValue(TextBox.FontSizeProperty);
            _values[i++] = ((TextBox)_element).GetValue(TextBox.FontStretchProperty);
            _values[i++] = ((TextBox)_element).GetValue(TextBox.FontStyleProperty);
            _values[i++] = ((TextBox)_element).GetValue(TextBox.FontWeightProperty);
            _values[i++] = ((TextBox)_element).GetValue(TextBox.ForegroundProperty);
            _values[i++] = ((TextBox)_element).GetValue(TextBox.HeightProperty);
            _values[i++] = ((TextBox)_element).GetValue(TextBox.HorizontalAlignmentProperty);
            _values[i++] = ((TextBox)_element).GetValue(TextBox.HorizontalContentAlignmentProperty);
            _values[i++] = ((TextBox)_element).GetValue(TextBox.HorizontalScrollBarVisibilityProperty);
            _values[i++] = ((TextBox)_element).GetValue(TextBox.IsEnabledProperty);
            _values[i++] = ((TextBox)_element).GetValue(TextBox.MaxHeightProperty);
            _values[i++] = ((TextBox)_element).GetValue(TextBox.MaxLengthProperty);
            _values[i++] = ((TextBox)_element).GetValue(TextBox.MaxLinesProperty);
            _values[i++] = ((TextBox)_element).GetValue(TextBox.MaxWidthProperty);
            _values[i++] = ((TextBox)_element).GetValue(TextBox.MinHeightProperty);
            _values[i++] = ((TextBox)_element).GetValue(TextBox.MinLinesProperty);
            _values[i++] = ((TextBox)_element).GetValue(TextBox.NameProperty);
            _values[i++] = ((TextBox)_element).GetValue(TextBox.TextAlignmentProperty);
            _values[i++] = ((TextBox)_element).GetValue(TextBox.TextProperty);
            _values[i++] = ((TextBox)_element).GetValue(TextBox.TextWrappingProperty);
            _values[i++] = ((TextBox)_element).GetValue(TextBox.ToolTipProperty);
            _values[i++] = ((TextBox)_element).GetValue(TextBox.VisibilityProperty);
            _length = i;
        }

        /// <summary>clear values</summary>
        public void ClearValues()
        {
            ((TextBox)_element).ClearValue(TextBox.AcceptsReturnProperty);
            ((TextBox)_element).ClearValue(TextBox.AcceptsTabProperty);
            ((TextBox)_element).ClearValue(TextBox.AllowDropProperty);
            ((TextBox)_element).ClearValue(TextBox.BackgroundProperty);
            ((TextBox)_element).ClearValue(TextBox.BorderBrushProperty);
            ((TextBox)_element).ClearValue(TextBox.BorderThicknessProperty);
            ((TextBox)_element).ClearValue(TextBox.CharacterCasingProperty);
            ((TextBox)_element).ClearValue(TextBox.LanguageProperty);
            ((TextBox)_element).ClearValue(TextBox.FlowDirectionProperty);
            ((TextBox)_element).ClearValue(TextBox.FocusableProperty);
            ((TextBox)_element).ClearValue(TextBox.FontFamilyProperty);
            ((TextBox)_element).ClearValue(TextBox.FontSizeProperty);
            ((TextBox)_element).ClearValue(TextBox.FontStretchProperty);
            ((TextBox)_element).ClearValue(TextBox.FontStyleProperty);
            ((TextBox)_element).ClearValue(TextBox.FontWeightProperty);
            ((TextBox)_element).ClearValue(TextBox.ForegroundProperty);
            ((TextBox)_element).ClearValue(TextBox.HeightProperty);
            ((TextBox)_element).ClearValue(TextBox.HorizontalAlignmentProperty);
            ((TextBox)_element).ClearValue(TextBox.HorizontalContentAlignmentProperty);
            ((TextBox)_element).ClearValue(TextBox.HorizontalScrollBarVisibilityProperty);
            ((TextBox)_element).ClearValue(TextBox.IsEnabledProperty);
            ((TextBox)_element).ClearValue(TextBox.MaxHeightProperty);
            ((TextBox)_element).ClearValue(TextBox.MaxLengthProperty);
            ((TextBox)_element).ClearValue(TextBox.MaxLinesProperty);
            ((TextBox)_element).ClearValue(TextBox.MaxWidthProperty);
            ((TextBox)_element).ClearValue(TextBox.MinHeightProperty);
            ((TextBox)_element).ClearValue(TextBox.MinLinesProperty);
            ((TextBox)_element).ClearValue(TextBox.NameProperty);
            ((TextBox)_element).ClearValue(TextBox.TextAlignmentProperty);
            ((TextBox)_element).ClearValue(TextBox.TextProperty);
            ((TextBox)_element).ClearValue(TextBox.TextWrappingProperty);
            ((TextBox)_element).ClearValue(TextBox.ToolTipProperty);
            ((TextBox)_element).ClearValue(TextBox.VisibilityProperty);
        }

        #endregion Helpers.


        #region private data.

        private object[] _initialPropValues = new object[40];
        private object[] _afterChangePropValues = new object[40];
        private object[] _afterClearPropValues = new object[40];
        private string _errorString;
        private string _errorString1;
        private int _length = 40;
        private int _changeCount = 0;
        private int _expectedCount = 0;

        private UIElementWrapper _ControlWrapper;
        private TextEditableType _editableType = null;
        private FrameworkElement _element;
        private SetOptions _setSwitch = 0;

        #endregion private data.
    }

    /// <summary>
    /// Verifies that typing a large amount of text doesnt 
    /// alter text since it gets a string after typing each char
    /// </summary>
    [Test(3, "TextBox", "TextBoxTypeLongText", MethodParameters = "/TestCaseType:TextBoxTypeLongText", Timeout = 360)]
    [TestOwner("Microsoft"), TestTactics("616"), TestWorkItem("112")]
    public class TextBoxTypeLongText : CustomTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            Canvas canvasPanel = new Canvas();
            _control = new TextBox();
            _control.FontSize = 14;
            _control.Focus();
            _control.Width = 300;
            _control.Height = 400;

            _control.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            _control.TextWrapping = TextWrapping.Wrap;
            UIElementWrapper _controlWrapper = new UIElementWrapper(_control);
            _controlWrapper = new UIElementWrapper(_control);
            _controlWrapper.Clear();
            canvasPanel.Children.Add(_control);
            MainWindow.Content = canvasPanel;

            //covers exception condition 
            Verifier.Verify(-1 == _control.GetLastVisibleLineIndex(), " Expected Last line [-1] Actual [" +
                _control.GetLastVisibleLineIndex().ToString() + "]", true);
            Verifier.Verify(-1 == _control.GetFirstVisibleLineIndex(), " Expected First line [-1] Actual [" +
              _control.GetFirstVisibleLineIndex().ToString() + "]", true);
            QueueDelegate(DoFocus);
        }

        /// <summary>Focuses on element</summary>
        private void DoFocus()
        {
            MouseInput.MouseClick(_control);
            QueueDelegate(TypeText);
        }

        /// <summary>Types text</summary>
        private void TypeText()
        {
            _control.Focus();
            int num = _count + 50;
            _inputString = "";
            while (_count < num)
            {
                _inputString += _count.ToString();
                _count++;
            }
            _expectedString += _inputString;

            KeyboardInput.TypeString("^{END}" + _inputString);
            QueueDelegate(VerifyText);
        }

        /// <summary>Verifies text in a loop</summary>
        private void VerifyText()
        {
            Verifier.Verify(_control.Text == _expectedString, " Strings not equal. Expected [" + _expectedString + "]Actual[" +
                _control.Text + "]", false);
            if (_count < 1500)
            {
                TypeText();
            }
            else
            {
                Logger.Current.ReportSuccess();
            }
        }

        #endregion Main flow.

        #region Private fields.

        private TextBox _control;
        private int _count = 0;
        private string _expectedString = "";
        private string _inputString = "";

        #endregion Private fields.
    }

    /// <summary>Tests whether GetRectFromCharIndex returns proper rects</summary>
    [Test(0, "TextBox", "TextBoxGetRectFromCharIndex1", MethodParameters = "/TestCaseType:TextBoxGetRectFromCharIndex")]
    [Test(2, "PartialTrust", TestCaseSecurityLevel.FullTrust, "TextBoxGetRectFromCharIndex2", MethodParameters = "/TestCaseType:TextBoxGetRectFromCharIndex /XbapName=EditingTestDeploy")]
    [TestOwner("Microsoft"), TestTactics("617,618"), TestWorkItem("108")]
    public class TextBoxGetRectFromCharIndex : ManagedCombinatorialTestCase
    {
        /// <summary>initialization of the run</summary>
        protected override void DoRunCombination()
        {
            _element = _editableType.CreateInstance();
            if (_element is TextBox)
            {
                _ControlWrapper = new UIElementWrapper(_element);
                _tb = _element as TextBox;

                _tb.FontSize = 30;
                _tb.Width = 50;
                if (_localeSwitch == LocaleChoices.Arabic)
                {
                    _tb.Text = (_inputStringSwitch == InputStringChoices.Single) ? _ARtext : _ARtextWithNewLine;
                }
                else
                {
                    _tb.Text = (_inputStringSwitch == InputStringChoices.Single) ? _text : _textWithNewLine;
                }
                TestElement = _element;
                QueueDelegate(DoFocus);
            }
            else
            {
                NextCombination();
            }
        }

        /// <summary>Focus on element</summary>
        private void DoFocus()
        {
            _element.Focus();
            ExecuteTrigger();
        }

        /// <summary>Program controller</summary>
        private void ExecuteTrigger()
        {
            Rect rectwithIndex, rectwithEdge, savedRect;
            rectwithEdge = rectwithIndex = savedRect = new Rect(0, 0, 0, 0);
            int length = _tb.Text.Length;
            int count = 0;
            int prevCount = 0;
            while (count < length)
            {
                Log("CharacterIndex=" + count.ToString());
                rectwithIndex = _tb.GetRectFromCharacterIndex(count);
                savedRect = rectwithEdge;
                if (count != 0)
                {
                    prevCount = count - 1;
                }
                rectwithEdge = _tb.GetRectFromCharacterIndex(count, true);
                bool condition = false;
                // count ==0 because initially saved rect has wrong values
                // Behavior change in 4.0 (Look at Part1 
#if TESTBUILD_CLR20
                // rectwithIndex.Top > savedRect.Top because text may wrap and then saved rect will point to previous lines trailing edge
                condition = (count != 0) && (rectwithIndex.Top == savedRect.Top);
#endif
#if TESTBUILD_CLR40
                condition = (count != 0) && (tb.GetLineIndexFromCharacterIndex(count) == tb.GetLineIndexFromCharacterIndex(prevCount));
#endif
                if (condition)
                {
                    if (((count < _firstLineLength) && (count > _firstLineLength - 3) && (_inputStringSwitch == InputStringChoices.Multi)))
                    {
                        Verifier.Verify(Compare(rectwithIndex, rectwithEdge), "Rects are suposed to be equal Rectwithindex[" + rectwithIndex.ToString() +
                            "] rectwithEdge[" + rectwithEdge.ToString() + "] ---Processing Empty Line", true);
                    }
                    else
                    {
                        Verifier.Verify(Compare(rectwithIndex, savedRect), "Rects are suposed to be equal Rectwithindex[" + rectwithIndex.ToString() +
                            "] savedRect[" + savedRect.ToString() + "]", true);
                    }
                }
                else
                {
                    if (_localeSwitch == LocaleChoices.Arabic)
                    {
                        Verifier.Verify(rectwithIndex.Left > rectwithEdge.Left, "Rects are suppossed to be NOT equal Rectwithindex.Left[" + rectwithIndex.Left.ToString() +
                        "]> rectwithEdge.Left[" + rectwithEdge.Left.ToString() + "]", true);
                    }
                    else
                    {
                        Verifier.Verify(rectwithIndex.Left < rectwithEdge.Left, "Rects are suppossed to be NOT equal Rectwithindex.Left[" + rectwithIndex.Left.ToString() +
                        "]< rectwithEdge.Left[" + rectwithEdge.Left.ToString() + "]", true);
                    }
                }
                count++;
            }
            QueueDelegate(NextCombination);
        }

        /// <summary>Compare function</summary>
        public static bool Compare(Rect rect1, Rect rect2)
        {
            int top1 = (int)(rect1.Top + .001);
            int left1 = (int)(rect1.Left + .001);
            int right1 = (int)(rect1.Right + .001);
            int bottom1 = (int)(rect1.Bottom + .001);

            int top2 = (int)(rect2.Top + .001);
            int left2 = (int)(rect2.Left + .001);
            int right2 = (int)(rect2.Right + .001);
            int bottom2 = (int)(rect2.Bottom + .001);

            if ((top1 == top2) && (bottom1 == bottom2) && (left1 == left2) && (right1 == right2))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #region data.
        //"\x062e\x0635\x0645\x0639\x0644 \x0644\x0633\x0655\x0627\x0654" - old
        private string _text = "hello world";
        private int _firstLineLength = 7;
        private string _textWithNewLine = "hello" + Environment.NewLine + " world";
        private string _ARtext = "\x064a\x062a\x062d\x062f\x062f\x062b \x0628\x0644\x063a\x0629";
        private string _ARtextWithNewLine = "\x064a\x062a\x062d\x062f\x062f" + Environment.NewLine + "\x064a\x062a\x062d\x062f\x062f";

        private InputStringChoices _inputStringSwitch = 0;
        private UIElementWrapper _ControlWrapper;
        private TextEditableType _editableType = null;
        private FrameworkElement _element;
        private TextBox _tb;
        private LocaleChoices _localeSwitch = 0;

        #endregion data.
    }

    /// <summary>Tests GetRectFromCharIndex usign surrogates</summary>
    [Test(0, "TextBox", "TextBoxGetRectFromCharIndexSurrogate", MethodParameters = "/TestCaseType:TextBoxGetRectFromCharIndexSurrogate")]
    [TestOwner("Microsoft"), TestTactics("613"), TestWorkItem("108")]
    public class TextBoxGetRectFromCharIndexSurrogate : ManagedCombinatorialTestCase
    {
        /// <summary>initialization of the run</summary>
        protected override void DoRunCombination()
        {
            _element = _editableType.CreateInstance();
            if (_element is TextBox)
            {
                _ControlWrapper = new UIElementWrapper(_element);
                _tb = _element as TextBox;

                _tb.FontSize = 30;
                _tb.Width = 50;
                _tb.Text = "\xd800\xdc00\r\n\xd800\xdc00";
                TestElement = _element;
                QueueDelegate(DoFocus);
            }
            else
            {
                NextCombination();
            }
        }

        /// <summary>Focus on element</summary>
        private void DoFocus()
        {
            _element.Focus();
            ExecuteTrigger();
        }

        /// <summary>Program controller</summary>
        private void ExecuteTrigger()
        {
            Rect rectwithIndex, rectwithEdge, savedRect;
            rectwithEdge = rectwithIndex = savedRect = new Rect(0, 0, 0, 0);
            int index = 0;
            int lineLength = _tb.GetLineLength(index);
            int count = 0;
            while (count < lineLength)
            {
                rectwithIndex = _tb.GetRectFromCharacterIndex(count);
                rectwithEdge = _tb.GetRectFromCharacterIndex(count, true);
                if ((count == 0) || (count == 1))
                {
                    Verifier.Verify(rectwithIndex.Left < rectwithEdge.Left, "Rects are suppossed to be NOT equal Rectwithindex.Left[" + rectwithIndex.Left.ToString() +
                    "]< rectwithEdge.Left[" + rectwithEdge.Left.ToString() + "]", true);
                }
                else
                {
                    Verifier.Verify(TextBoxGetRectFromCharIndex.Compare(rectwithIndex, rectwithEdge), "Rects are suposed to be equal Rectwithindex[" + rectwithIndex.ToString() +
                            "] rectwithEdge[" + rectwithEdge.ToString() + "] ---Processing Empty Line", true);

                }
                count++;
                if ((count == lineLength) && (index < 1))
                {
                    count = 0;
                    index++;
                    lineLength = _tb.GetLineLength(index);
                }
            }
            QueueDelegate(NextCombination);
        }

        #region data.

        private UIElementWrapper _ControlWrapper;
        private TextEditableType _editableType = null;
        private FrameworkElement _element;
        private TextBox _tb;

        #endregion data.
    }

    /// <summary>Test TextBoxGetLineLength and GetLineText</summary>
    [Test(2, "TextBox", "TextBoxGetLineLengthAndText", MethodParameters = "/TestCaseType:TextBoxGetLineLengthAndText")]
    [TestOwner("Microsoft"), TestTactics("614"), TestWorkItem("107"), TestBugs("619")]
    public class TextBoxGetLineLengthAndText : ManagedCombinatorialTestCase
    {
        /// <summary>initialization of the run</summary>
        protected override void DoRunCombination()
        {
            _element = _editableType.CreateInstance();
            if (_element is TextBox)
            {
                _ControlWrapper = new UIElementWrapper(_element);
                _tb = _element as TextBox;
                _tb.FontSize = 30;
                TestElement = _element;
                //Boundary testing before adding to tree
                VerificationGetLineText(_tb.GetLineText(0), null);
                VerificationGetLineLength(_tb.GetLineLength(0), -1);
                QueueDelegate(DoFocus);
            }
            else
            {
                NextCombination();
            }
        }

        /// <summary>Focus on element</summary>
        private void DoFocus()
        {
            _element.Focus();
            ExecuteTrigger();
        }

        /// <summary>Program Controller</summary>
        private void ExecuteTrigger()
        {
            switch (_inputStringDataSwitch)
            {
                case InputStringData.Empty:
                    _tb.Text = _emptyText;
                    VerificationGetLineText(_tb.GetLineText(0), _emptyText);
                    VerificationGetLineLength(_tb.GetLineLength(0), _emptyText.Length);
                    break;

                case InputStringData.LTR:
                    _tb.Text = _ltrText;
                    VerificationGetLineText(_tb.GetLineText(0), _ltrText);
                    VerificationGetLineLength(_tb.GetLineLength(0), _ltrText.Length);
                    break;

                case InputStringData.MixedLTR_RTL:
                    _tb.Text = _ltrText + _rtlText;
                    string _temp = _ltrText + _rtlText;
                    VerificationGetLineText(_tb.GetLineText(0), _temp);
                    VerificationGetLineLength(_tb.GetLineLength(0), _temp.Length);
                    break;

                case InputStringData.MixedRTL_LTR:
                    _tb.Text = _rtlText + _ltrText;
                    _temp = _rtlText + _ltrText;
                    VerificationGetLineText(_tb.GetLineText(0), _temp);
                    VerificationGetLineLength(_tb.GetLineLength(0), _temp.Length);
                    break;

                case InputStringData.MultiLine:
                    _tb.AcceptsReturn = true;
                    _tb.Text = _ltrText + Environment.NewLine + _secLineString;
                    VerificationGetLineText(_tb.GetLineText(1), _secLineString);
                    VerificationGetLineLength(_tb.GetLineLength(1), _secLineString.Length);
                    break;

                case InputStringData.RTL:
                    _tb.Text = _rtlText;
                    VerificationGetLineText(_tb.GetLineText(0), _rtlText);
                    VerificationGetLineLength(_tb.GetLineLength(0), _rtlText.Length);
                    break;

                case InputStringData.Surrogate:
                    _tb.Text = _surrogateText;
                    VerificationGetLineText(_tb.GetLineText(0), _surrogateText);
                    VerificationGetLineLength(_tb.GetLineLength(0), _surrogateText.Length);
                    break;

                case InputStringData.InvalidTest:
                    try
                    {
                        if (_count++ == 0)
                        {
                            VerificationGetLineText(_tb.GetLineText(-1), "dont care");
                        }
                        VerificationGetLineLength(_tb.GetLineLength(10), 0);
                        throw new ApplicationException("Expected exception when index of string is -1");
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        Log("Exception thrown as expected when index of string is -1");
                    }
                    //   NextCombination();
                    break;

                default:
                    break;
            }
            NextCombination();
        }

        /// <summary>verification of GetLine</summary>
        private void VerificationGetLineText(string _tbText, string _assignedText)
        {
            Verifier.Verify(_tbText == _assignedText, "TextBox text is [" + _tbText +
                "] Assigned Text is [" + _assignedText + "]", true);
        }

        /// <summary>Verification of GetText</summary>
        private void VerificationGetLineLength(int _tbTextLength, int _assignedTextLength)
        {
            Verifier.Verify(_tbTextLength == _assignedTextLength, "TextBox text Length is [" + _tbTextLength +
                "] Assigned Text Length is [" + _assignedTextLength + "]", true);
        }

        #region data.

        private string _rtlText = "\x05d0\x05d1\x05ea\x05e9";
        private string _ltrText = "Abc";
        private string _surrogateText = "\x0302e\x0327\x0627\x0654\x0655"; //\x0302|e|\x0327|\x0627|\x0654|\x0655
        private string _emptyText = "";
        private string _secLineString = "defghi";
        private int _count = 0;

        private InputStringData _inputStringDataSwitch = 0;
        private UIElementWrapper _ControlWrapper;
        private TextEditableType _editableType = null;
        private FrameworkElement _element;
        private TextBox _tb;

        #endregion data.
    }

    /// <summary>
    /// Verifies the TextDecorations property on TextBox
    /// </summary>
    [Test(0, "TextBox", "TextBoxTextDecorationsTest", MethodParameters = "/TestCaseType:TextBoxTextDecorationsTest")]
    [TestOwner("Microsoft"), TestTactics("612"), TestWorkItem("106")]
    public class TextBoxTextDecorationsTest : CustomTestCase
    {
        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            Canvas canvasPanel = new Canvas();
            _textBox = new TextBox();
            _textBox.FontSize = 14;
            _textBox.Width = 300;
            _textBox.Height = 400;
            _textBox.FontSize = 30;
            _textBox.Text = _sampleString;

            canvasPanel.Children.Add(_textBox);
            MainWindow.Content = canvasPanel;
            QueueDelegate(GetInitialImage);
        }

        /// <summary>Focuses on element</summary>
        private void GetInitialImage()
        {
            _initialBitmap = BitmapCapture.CreateBitmapFromElement(_textBox);
            Logger.Current.LogImage(_initialBitmap, "InitialImage");
            QueueDelegate(UnderLineTextBox);
        }

        /// <summary>Underlines TextBox</summary>
        private void UnderLineTextBox()
        {
            _textBox.TextDecorations = TextDecorations.Underline;
            QueueDelegate(GetImageAfterUnderLine);
        }

        /// <summary>Get Image after Underline</summary>
        private void GetImageAfterUnderLine()
        {
            _bitmapWithUnderLine = BitmapCapture.CreateBitmapFromElement(_textBox);
            Logger.Current.LogImage(_bitmapWithUnderLine, "UnderlineImage");
            QueueDelegate(VerifyUnderLine);
        }

        /// <summary>Verify the underline of textbox</summary>
        private void VerifyUnderLine()
        {
            if (ComparisonOperationUtils.AreBitmapsEqual(_initialBitmap, _bitmapWithUnderLine, out _differencesBitmap) == false)
            {
                Logger.Current.ReportSuccess();
            }
            else
            {
                Logger.Current.LogImage(_differencesBitmap, "DiffUnderline");
                Verifier.Verify(false, "Figures are same. Underline Doesnt Work", false);
            }
        }

        #region Private fields.

        private TextBox _textBox;
        private string _sampleString = "Hello World";
        Bitmap _initialBitmap, _bitmapWithUnderLine, _differencesBitmap;

        #endregion Private fields.
    }

    /// <summary>Tests IME for readonly and non-readonly controls</summary>
    [Test(0, "Input", "TestForIME", MethodParameters = "/TestCaseType:TestForIME", Timeout = 420)]
    [TestOwner("Microsoft"), TestTactics("611"), TestWorkItem("2")]
    public class TestForIME : ManagedCombinatorialTestCase
    {
        /// <summary>initialization of the run</summary>
        protected override void DoRunCombination()
        {
            _criteria = new ComparisonCriteria();
            _criteria.MaxErrorProportion = 0.01f;

            _element = _editableType.CreateInstance();
            if ((_element is PasswordBox) || (KeyboardInput.IsBidiInputLanguageInstalled() == false) || (KeyboardInput.IsInputLanguageInstalled("jpn") == false))
            {
                NextCombination();
            }
            else
            {
                _controlWrapper = new UIElementWrapper(_element);
                _controlWrapper.Text = _surrogateString + "s";
                ((TextBoxBase)_element).FontSize = 30;
                TestElement = _element;
                KeyboardInput.SetActiveInputLocale(InputLocaleData.JapaneseMsIme2002.Identifier);
                KeyboardInput.EnableIME(_element);
                QueueDelegate(SetWindowFocus);
            }
        }

        /// <summary>Get initial image with text</summary>
        private void SetWindowFocus()
        {
            MouseInput.MouseClick(_element);
            KeyboardInput.TypeString("^{HOME}");
            QueueDelegate(GetInitialImage);
        }

        /// <summary>Get initial image with text</summary>
        private void GetInitialImage()
        {
            _element.Focus();
            _imageInitial = BitmapCapture.CreateBitmapFromElement(_element);
            Logger.Current.LogImage(_imageInitial, "imageInitial");
            KeyboardInput.TypeString("{End}{Backspace}s{SPACE}{SPACE}{HOME}");
            QueueDelegate(GetImageOfContextMenu);
        }

        /// <summary>Get image with context menu</summary>
        private void GetImageOfContextMenu()
        {
            _imageWithContextMenu = BitmapCapture.CreateBitmapFromElement(_element);
            Logger.Current.LogImage(_imageWithContextMenu, "imageWithContextMenu");
            KeyboardInput.TypeString("{DOWN}{DOWN}{ENTER}");
            QueueDelegate(VerifyIMEWorks);
        }

        /// <summary>Verification that IME context menu works</summary>
        private void VerifyIMEWorks()
        {
            string str = (_element is TextBox) ? "s" : "s\r\n";
            Verifier.Verify(ComparisonOperationUtils.AreBitmapsEqualUsingCriteria(_imageInitial, _imageWithContextMenu, out _imageDifferences, _criteria, true) == false,
                "Images are supposed to be different", true);
            Verifier.Verify(_controlWrapper.Text != (_surrogateString + str), "Strings are NOT supposed to be EQUAL Expected [" +
                _surrogateString + "*] Where  * is a char got from the Context Menu Actual[" + _controlWrapper.Text + "]", true);

            QueueDelegate(ResetValuesForInitialReadOnlyImage);
        }

        /// <summary>Reset values for Read Only case and bring up context menu
        /// This is needed just to get the initial image with curso non-blinking (wontfixbug)
        /// </summary>
        private void ResetValuesForInitialReadOnlyImage()
        {
            _textBeforeMakingReadOnly = _controlWrapper.Text;
            _controlWrapper.Text = _surrogateString;
            KeyboardInput.TypeString("{End}s{SPACE}{SPACE}{Space}{HOME}{ESC}"); //ESC ENSURES THAT THE CONTEXT MENU IS NOT PRESENT
            QueueDelegate(SetReadonlyForInitialImage);
        }

        /// <summary> Set readonly property  </summary>
        private void SetReadonlyForInitialImage()
        {
            ((TextBoxBase)(_element)).IsReadOnly = true;
            QueueDelegate(GetInitialImageWithReadonly);
        }

        /// <summary>gets the image of control with no context menu (explicitly closed)-- caret is fixed</summary>
        private void GetInitialImageWithReadonly()
        {
            _imageInitialReadOnly = BitmapCapture.CreateBitmapFromElement(_element);
            QueueDelegate(ResetValues);
        }

        /// <summary>Again reset values to test whether context m enu closes - no explicit closing</summary>
        private void ResetValues()
        {
            ((TextBoxBase)(_element)).IsReadOnly = false;
            _controlWrapper.Text = _surrogateString;
            KeyboardInput.TypeString("{End}s{SPACE}{SPACE}{HOME}"); //menu is opened
            QueueDelegate(MakeReadOnlyWithContextMenuOpened);
        }

        /// <summary>SET readonly property</summary>
        private void MakeReadOnlyWithContextMenuOpened()
        {
            _textBeforeMakingReadOnly = _controlWrapper.Text;
            ((TextBoxBase)(_element)).IsReadOnly = true;
            KeyboardInput.TypeString("{SPACE}"); //This should have no effect
            QueueDelegate(GetImageWithReadonlyContextmenu);
        }

        /// <summary>Get the image with readonly property set and with context menu opened (supposed to close) </summary>
        private void GetImageWithReadonlyContextmenu()
        {
            _imageWithContextMenuReadOnly = BitmapCapture.CreateBitmapFromElement(_element);
            QueueDelegate(SelectSomeWordIfContextMenuExists);
        }

        /// <summary>Perform keyboard operation to test if menu is still open</summary>
        private void SelectSomeWordIfContextMenuExists()
        {
            KeyboardInput.TypeString("{SPACE}{SPACE}{ENTER}"); //this changes the selected option on context menu if present
            QueueDelegate(VerifyReadOnlyWithContextMenuOpened);
        }

        /// <summary>Verifies whether menu is opened and if text is modified</summary>
        private void VerifyReadOnlyWithContextMenuOpened()
        {
            //ENUSRES THAT THE MENU CHOICES DO NOT WORK
            Verifier.Verify(_controlWrapper.Text == _textBeforeMakingReadOnly, "Strings are supposed to be EQUAL When in ReadOnly Expected [" +
                  _textBeforeMakingReadOnly + "] Actual[" + _controlWrapper.Text + "]", true);
            //ENSURES THAT THE MENU IS NOT OPENED

            Verifier.Verify(ComparisonOperationUtils.AreBitmapsEqualUsingCriteria(_imageInitialReadOnly, _imageWithContextMenuReadOnly, _criteria) == true,
                "Images are supposed to be EQUAL when in readonly", true);
            QueueDelegate(NextCombination);
        }

        #region data.

        private string _surrogateString = "hello";
        private UIElementWrapper _controlWrapper;
        private TextEditableType _editableType = null;
        private FrameworkElement _element;

        Bitmap _imageInitial, _imageWithContextMenu, _imageDifferences;
        Bitmap _imageInitialReadOnly, _imageWithContextMenuReadOnly;
        private string _textBeforeMakingReadOnly = "";

        private ComparisonCriteria _criteria;

        #endregion data.
    }

    /// <summary>Tests  ProgrammaticChangeInReadOnlyControl</summary>
    [Test(0, "TextBox", "TestForProgrammaticChangeInReadOnlyControl", MethodParameters = "/TestCaseType:TestForProgrammaticChangeInReadOnlyControl")]
    [TestOwner("Microsoft"), TestTactics("610"), TestWorkItem("111"), TestBugs("567")]
    public class TestForProgrammaticChangeInReadOnlyControl : ManagedCombinatorialTestCase
    {
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
                _controlWrapper.Text = _sampleData;
                ((TextBoxBase)_element).FontSize = 30;
                ((TextBoxBase)_element).IsReadOnly = true;
                _eventCount = 0;
                ((TextBoxBase)_element).TextChanged += new TextChangedEventHandler(element_TextChanged);
                TestElement = _element;
                QueueDelegate(ExecuteTrigger);
            }
        }

        /// <summary>Program Controller</summary>
        private void ExecuteTrigger()
        {
            string _newLine = (_element is RichTextBox) ? "\r\n" : "";
            Clipboard.Clear();

            switch (_operationSwitch)
            {
                case OperationChoices.Append:
                    ((TextBoxBase)(_element)).AppendText(_appendText);
                    VerificationHelperForComparison(_controlWrapper.Text, _sampleData + _appendText + _newLine, "---AppendText---");
                    VerificationHelperForTrigger(true, "---AppendText---");
                    NextCombination();
                    break;

                case OperationChoices.Clear:
                    _controlWrapper.Clear();
                    VerificationHelperForComparison(_controlWrapper.Text, _newLine, "---Clear---");
                    VerificationHelperForTrigger(true, "---Clear---");
                    NextCombination();
                    break;

                case OperationChoices.Copy:
                    string _clipBoardText = PerformCopyOperation();
                    VerificationHelperForComparison(_clipBoardText, _sampleData + _newLine, "---Copy---");
                    VerificationHelperForTrigger(false, "---Copy---");
                    NextCombination();
                    break;

                case OperationChoices.CutPaste:
                    PerformCutPasteAndVerification(_newLine);
                    NextCombination();
                    break;

                case OperationChoices.DragDrop:
                    _element.Focus();
                    KeyboardInput.TypeString("+{RIGHT 4}");
                    QueueDelegate(PerformDragDropOperation);
                    break;

                case OperationChoices.Text:
                    _controlWrapper.Text = _appendText;
                    //  _eventCount = (_element is TextBox) ? _eventCount : (_eventCount - 1); //this is done becuase RTB is first cleared then initialised
                    VerificationHelperForComparison(_controlWrapper.Text, _appendText + _newLine, "---Text Property ---");
                    VerificationHelperForTrigger(true, "---Text Property ---");
                    NextCombination();
                    break;

                case OperationChoices.UndoRedo:
                    PerformUndoRedoAndVerification(_newLine);
                    NextCombination();
                    break;

                default:
                    break;
            }
        }

        /// <summary>PerformCopyOperation</summary>
        private string PerformCopyOperation()
        {
            ((TextBoxBase)(_element)).SelectAll();
            ((TextBoxBase)(_element)).Copy();
            string _clipBoardText = "";
            if (Clipboard.ContainsText())
            {
                _clipBoardText = Clipboard.GetText();
            }
            return _clipBoardText;
        }

        /// <summary>PerformCutPasteAndVerification</summary>
        private void PerformCutPasteAndVerification(string _newLine)
        {
            ((TextBoxBase)(_element)).SelectAll();
            ((TextBoxBase)(_element)).Cut();
            string _clipBoardText = "";
            if (Clipboard.ContainsText())
            {
                _clipBoardText = Clipboard.GetText();
            }
            VerificationHelperForComparison(_clipBoardText, _sampleData + _newLine, "---Cut(Verification on ClipBoard)---");
            VerificationHelperForComparison(_controlWrapper.Text, "", "---Cut(verification on control)---");
            VerificationHelperForTrigger(true, "---Cut---");
            _eventCount = 0;
            ((TextBoxBase)(_element)).Paste();
            VerificationHelperForComparison(_controlWrapper.Text, _sampleData + _newLine, "---Paste---");
            VerificationHelperForTrigger(true, "---Paste---");

        }
        /// <summary>PerformDragDropOperation</summary>
        private void PerformDragDropOperation()
        {
            Rect rc = _controlWrapper.GetGlobalCharacterRect(1, LogicalDirection.Backward);

            System.Windows.Point _startPoint = new System.Windows.Point(rc.Left + rc.Width / 2,
                rc.Top + rc.Height / 2);
            System.Windows.Point _endPoint = new System.Windows.Point(rc.Left + (_element.ActualWidth / 2),
                rc.Top + rc.Height / 2);
            MouseInput.MouseDragInOtherThread(_startPoint, _endPoint, true,
                TimeSpan.FromMilliseconds(500), VerifyDragDrop, Dispatcher.CurrentDispatcher);
        }

        /// <summary>VerifyDragDrop</summary>
        private void VerifyDragDrop()
        {
            string _newLine = (_element is RichTextBox) ? "\r\n" : "";
            VerificationHelperForComparison(_controlWrapper.Text, _sampleData + _newLine, "---Drag Drop---");
            VerificationHelperForTrigger(false, "---Drag Drop---");
            NextCombination();
        }

        /// <summary>PerformUndoRedoAndVerification</summary>
        private void PerformUndoRedoAndVerification(string _newLine)
        {
            ((TextBoxBase)(_element)).AppendText(_appendText);
            ((TextBoxBase)(_element)).AppendText(_appendText);
            _eventCount = 0; //resetting it to facilitate verification since verification checks counts after setting text
            //Uncomment when Regression_Bug567 is fixed
            Verifier.Verify(((TextBoxBase)(_element)).CanUndo == true, "CanUndo should return true when control is ReadOnly and undo unit is available", false);
            ((TextBoxBase)(_element)).Undo();
            VerificationHelperForTrigger(true, "---Undo FirstTime---");
            _eventCount = 0;
            ((TextBoxBase)(_element)).Undo();
            VerificationHelperForTrigger(true, "---Undo Secondtime---");
            _eventCount = 0;
            VerificationHelperForComparison(_controlWrapper.Text, _sampleData + _newLine, "---Undo---");
            ((TextBoxBase)(_element)).Redo();
            VerificationHelperForComparison(_controlWrapper.Text, _sampleData + _appendText + _newLine, "---Redo---");
            VerificationHelperForTrigger(true, "---Redo---");
        }

        #region Helpers.

        /// <summary>VerificationHelperForComparison</summary>
        private void VerificationHelperForComparison(string actual, string expected, string operation)
        {
            Verifier.Verify(actual == expected, "Operation performed is:" + operation + " Actual Text is [" +
                actual + "] Expected Text is [" + expected + "]", true);
        }

        /// <summary>VerificationHelperForTrigger</summary>
        private void VerificationHelperForTrigger(bool IsTriggerred, string operation)
        {
            int _expected = (IsTriggerred) ? 1 : 0;
            Verifier.Verify(_eventCount == _expected, "Operation performed is:" + operation + " Actual event count is [" +
                _eventCount + "] Expected Text is [" + _expected + "]", true);
        }

        /// <summary>Text Changed event handler</summary>
        void element_TextChanged(object sender, TextChangedEventArgs e)
        {
            _eventCount++;
        }

        #endregion Helpers.

        #region data.

        private UIElementWrapper _controlWrapper;
        private TextEditableType _editableType = null;
        private FrameworkElement _element;
        private OperationChoices _operationSwitch = 0;

        private string _sampleData = "Hello";
        private string _appendText = "World";
        private int _eventCount = 0;

        #endregion data.
    }

    /// <summary>
    /// Verifies the TabIndex property works correctly 
    /// </summary>
    [Test(2, "TextBox", "TestTabIndexProperty", MethodParameters = "/TestCaseType:TestTabIndexProperty")]
    [TestOwner("Microsoft"), TestTactics("609"), TestWorkItem("110")]
    public class TestTabIndexProperty : CustomTestCase
    {
        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            StackPanel _stackPanel = new StackPanel();

            SetUpControls();

            _stackPanel.Children.Add(_richTextBox);
            _stackPanel.Children.Add(_rtbSub);
            _stackPanel.Children.Add(_textBox);
            _stackPanel.Children.Add(_tbSub);
            _stackPanel.Children.Add(_passwordBox);
            MainWindow.Content = _stackPanel;
            QueueDelegate(DoFocus);
        }

        /// <summary>Focus on the textbox.</summary>
        private void DoFocus()
        {
            _textBox.Focus();
            _count++;
            KeyboardInput.TypeString("{TAB}");
            QueueDelegate(ExecuteTabs);
        }

        /// <summary>Controller for tabbing.</summary>
        private void ExecuteTabs()
        {
            switch (_count)
            {
                case 0:
                    VerificationOfFocusedElement(_textBox.IsFocused, "TextBox");
                    break;

                case 1:
                    VerificationOfFocusedElement(_richTextBox.IsFocused, "RichTextBox");
                    break;

                case 2:
                    VerificationOfFocusedElement(_passwordBox.IsFocused, "PasswordBox");
                    break;

                case 3:
                    VerificationOfFocusedElement(_rtbSub.IsFocused, "RichTextBoxSubClass");
                    break;

                case 4:
                    VerificationOfFocusedElement(_tbSub.IsFocused, "TextBoxSubClass");
                    break;

                default:
                    break;
            }
        }

        /// <summary>Verification that element is focused.</summary>
        private void VerificationOfFocusedElement(bool IsFocused, string _controlType)
        {
            Verifier.Verify(IsFocused == true, "Control " + _controlType + " should be focused on tabbing " +
                _count + " time(s)", true);
            QueueDelegate(PressTab);
        }

        /// <summary>Press tabs.</summary>
        private void PressTab()
        {
            _count = ++_count % 5;
            if (_count == 1)
            {
                //  once all the elements are passed through - disable richtextbox and do the same again
                if (_operations++ == 0)
                {
                    Log("\r\n*****************************Disabling RTB ************************************\r\n");
                    _richTextBox.IsEnabled = false;
                    _count++; // done so that count now points to passwordbox
                }
                else
                {
                    Logger.Current.ReportSuccess();
                }
            }
            KeyboardInput.TypeString("{TAB}");
            QueueDelegate(ExecuteTabs);
        }

        #region Helpers.

        /// <summary>Initial setup.</summary>
        private void SetUpControls()
        {
            _textBox = new TextBox();
            _richTextBox = new RichTextBox();
            _passwordBox = new PasswordBox();
            _rtbSub = new RichTextBoxSubClass();
            _tbSub = new TextBoxSubClass();
            _textBox.TabIndex = 0;
            _richTextBox.TabIndex = 1;
            _passwordBox.TabIndex = 2;
            _rtbSub.TabIndex = 3;
            _tbSub.TabIndex = 4;
        }

        #endregion Helpers.

        #region data.

        private TextBox _textBox;
        private RichTextBox _richTextBox;
        private PasswordBox _passwordBox;
        private RichTextBoxSubClass _rtbSub;
        private TextBoxSubClass _tbSub;

        private int _operations = 0;
        private int _count = 0;

        #endregion data.
    }

    /// <summary>Tests  AppendText</summary>
    [Test(0, "TextBoxBase", "TestForAppendText", MethodParameters = "/TestCaseType=TestForAppendText", Timeout = 240)]
    [TestOwner("Microsoft"), TestTactics("608"), TestWorkItem("109"), TestBugs("620, 568")]
    public class TestForAppendText : ManagedCombinatorialTestCase
    {
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
                ((TextBoxBase)_element).FontSize = 30;
                ((TextBoxBase)_element).AcceptsReturn = true;
                TestElement = _element;
                InitializeControl();
                InitializeAppendText();
                QueueDelegate(ApplyFormattingOnInitialText);
            }
        }

        //Applies formatting
        private void ApplyFormattingOnInitialText()
        {
            if (_element is TextBox)
            {
                ApplyFormatToTextBoxControl();
            }
            else
            {
                ApplyFormatToRichTextBoxText();
            }
            QueueDelegate(AppendText);
        }

        //AppendText
        private void AppendText()
        {
            ((TextBoxBase)(_element)).AppendText(_appendText);
            QueueDelegate(VerifyAppendText);
        }

        //VerifyAppendText
        private void VerifyAppendText()
        {
            string _newLine = ((_element is RichTextBox)) ? "\r\n" : "";

            if ((_contentSwitch == ContentChoices.Empty) && ((_appendTextSwitch == AppendTextChoices.Empty) || (_appendTextSwitch == AppendTextChoices.NewLine)))
            {
                _newLine = ""; //corner condition - when initial text ----isnged is "" and appended text is "" -NO PARAGRAPH IS ADDED
            }
            Verifier.Verify(_controlWrapper.Text == (_initialText + _appendText + _newLine), "After Appending Text Expected [" +
            _initialText + _appendText + _newLine + "] Actual [" + _controlWrapper.Text + "]", true);
            VerifyXamlAndFormatting();
        }

        //VerifyXamlAndFormatting
        private void VerifyXamlAndFormatting()
        {
            if (_element is TextBox)
            {
                VerifyFormattingForTextBox();
                NextCombination();
            }
            else
            {
                VerifyRichTextBoxFormatting();
                NextCombination();
            }
        }

        #region Helpers.

        //ApplyFormatToRichTextBoxText
        private void ApplyFormatToRichTextBoxText()
        {
            _element.Focus();
            string _format = (_formatSwitch == FormatChoices.Bold) ? EditingCommandData.ToggleBold.KeyboardShortcut : EditingCommandData.ToggleUnderline.KeyboardShortcut;
            if ((_contentSwitch == ContentChoices.Empty) || (_contentSwitch == ContentChoices.NewLine))
            {
                KeyboardInput.TypeString("^{home}" + _format); //FoR SPRINGlOADING
            }
            else
            {
                KeyboardInput.TypeString("^A" + _format);
            }
        }

        //ApplyFormatToTextBoxControl
        private void ApplyFormatToTextBoxControl()
        {
            if (_formatSwitch == FormatChoices.Bold)
            {
                ((TextBox)(_element)).FontWeight = FontWeights.Bold;
            }
            else
            {
                ((TextBox)(_element)).TextDecorations = TextDecorations.Underline;
            }
        }

        //CountNumberOfParas
        private int CountNumberOfParas(string str, string find)
        {
            int _count = 0;
            int _index = 0;
            while (str.IndexOf(find, _index) > -1)
            {
                _index = str.IndexOf(find, _index);
                _index++;
                _count++;
            }
            return _count;
        }

        //InitializeControl
        private void InitializeControl()
        {
            switch (_contentSwitch)
            {
                case ContentChoices.Empty:
                    _initialText = _controlWrapper.Text = "";
                    break;

                case ContentChoices.SingleLineText:
                    _initialText = _controlWrapper.Text = _singleLineText;
                    break;

                case ContentChoices.SingleLineFollowedByEmptyLine:
                    _controlWrapper.Text = _singleLineFollowedByEmptyLine;
                    _initialText = (_element is RichTextBox) ? (_singleLineText + "\r\n") : _singleLineFollowedByEmptyLine;
                    break;

                case ContentChoices.NewLine:
                    _controlWrapper.Text = _newLine;
                    _initialText = (_element is RichTextBox) ? ("") : _newLine; //since initializing RTB to newline is the same as initiliasing it to ""
                    break;

                default:
                    break;
            }
        }

        //InitializeAppendText
        private void InitializeAppendText()
        {
            switch (_appendTextSwitch)
            {
                case AppendTextChoices.Empty:
                    _appendText = "";
                    break;

                case AppendTextChoices.NewLine:
                    _appendText = "\r\n";
                    break;

                case AppendTextChoices.Word:
                    _appendText = "EXTRA";
                    break;

                default:
                    break;
            }
        }

        //VerifyFormattingForTextBox
        private void VerifyFormattingForTextBox()
        {
            if (_formatSwitch == FormatChoices.Bold)
            {
                Verifier.Verify(((TextBox)(_element)).FontWeight == FontWeights.Bold, "Expected Bold Actual[" +
                    ((TextBox)(_element)).FontWeight.ToString() + "]", true);
            }
            else
            {
                Verifier.Verify(((TextBox)(_element)).TextDecorations == TextDecorations.Underline, "Expected Underline Actual[" +
                  ((TextBox)(_element)).TextDecorations.ToString() + "]", true);
            }
        }

        //VerifyRichTextBoxFormatting
        private void VerifyRichTextBoxFormatting()
        {
            RichTextBox _rtb = _element as RichTextBox;
            TextRange _tr = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);

            //remove this once Regression_Bug568 is resolve
            if ((_contentSwitch == ContentChoices.Empty) || (_contentSwitch == ContentChoices.NewLine))
            {
                Verifier.Verify(_tr.GetPropertyValue(RichTextBox.FontWeightProperty).ToString() == FontWeights.Normal.ToString(), "Expected NORMAL formatting Actual [" +
                         _tr.GetPropertyValue(RichTextBox.FontWeightProperty).ToString() + "]", true);
            }
            else
                if (_formatSwitch == FormatChoices.Bold)
                {
                    Verifier.Verify(_tr.GetPropertyValue(RichTextBox.FontWeightProperty).ToString() == FontWeights.Bold.ToString(), "Expected BOLD formatting Actual [" +
                        _tr.GetPropertyValue(RichTextBox.FontWeightProperty).ToString() + "]", true);
                }
                else
                {
                    Verifier.Verify(_tr.GetPropertyValue(Paragraph.TextDecorationsProperty).ToString() == TextDecorations.Underline.ToString(), "Expected UNDERLINE formatting Actual [" +
                       _tr.GetPropertyValue(Paragraph.TextDecorationsProperty).ToString() + "]", true);
                }

            //Verifies that \r\n is interpreted as NEW PARA in appendtext
            if (_appendTextSwitch == AppendTextChoices.NewLine)
            {
                VerifyAppendTextInsertsParagraphForNewLine();
            }
        }

        //VerifyAppendTextInsertsParagraphForNewLine
        private void VerifyAppendTextInsertsParagraphForNewLine()
        {
            int _numberOfParas = CountNumberOfParas(_controlWrapper.Text, _newLine);
            string _xamlStr = _controlWrapper.XamlText;
            int _actualNumberOfParas = CountNumberOfParas(_xamlStr, _paraTag);
            Verifier.Verify(_numberOfParas == _actualNumberOfParas, "Number of Paras Expected [" + _numberOfParas.ToString() +
                "] Actual [" + _actualNumberOfParas.ToString() + "]", true);
        }

        #endregion Helpers.

        #region data.

        private ContentChoices _contentSwitch = 0;
        private AppendTextChoices _appendTextSwitch = 0;
        private FormatChoices _formatSwitch = 0;
        private UIElementWrapper _controlWrapper = null;
        private TextEditableType _editableType = null;
        private FrameworkElement _element = null;

        private string _singleLineText = "Single Line";
        private string _singleLineFollowedByEmptyLine = "Single Line\r\n\r\n"; //This is because the initial \r\n is replaced by one of the \r\n
        private string _appendText = "";
        private string _initialText = "";
        private string _newLine = "\r\n";
        private string _paraTag = "<Paragraph";

        #endregion data.
    }

    /// <summary>Tests FlowDirection changes the alignment of text </summary>
    [Test(0, "RichEditing", "UIElementFlowDirectionAlignment", MethodParameters = "/TestCaseType=UIElementFlowDirectionAlignment")]
    [TestOwner("Microsoft"), TestTactics("607"), TestWorkItem("105")]
    public class UIElementFlowDirectionAlignment : ManagedCombinatorialTestCase
    {
        /// <summary>initialization of the run</summary>
        protected override void DoRunCombination()
        {
            _element = _editableType.CreateInstance();
            _element.Height = 100d;
            _element.Width = 200d;
            if (_element is PasswordBox)
            {
                NextCombination();
            }
            else
            {
                //Dont let border come into the bitmap capture of the elements
                ((TextBoxBase)_element).BorderThickness = new Thickness(1);
                _controlWrapper = new UIElementWrapper(_element);
                _controlWrapper.Clear();
                TestElement = _element;
                TestElement.FlowDirection = FlowDirection.LeftToRight;
                QueueDelegate(DoFocus);
            }
        }

        private void DoFocus()
        {
            _element.Focus();
            KeyboardInput.TypeString(_sampleString);

            QueueDelegate(DoFocusAway);
        }

        private void DoFocusAway()
        {
            //Remove the focus so that caret doesnt come in the bitmap capture
            _element.Focusable = false;
            QueueDelegate(GetInitialImage);
        }

        private void GetInitialImage()
        {
            _masterImage = BitmapCapture.CreateBitmapFromElement(_element);
            _masterImage = BitmapUtils.CreateBorderlessBitmap(_masterImage, 2);
            _element.Focusable = true;

            if (_element is PasswordBox)
            {
                QueueDelegate(ExecuteTrigger);
            }
            else
            {
                QueueDelegate(SetUpForAlignmentImage);
            }
        }

        private void SetUpForAlignmentImage()
        {
            if (_element is TextBox)
            {
                ((TextBox)_element).TextAlignment = TextAlignment.Right;
            }
            else
            {
                ((RichTextBox)_element).Document.TextAlignment = TextAlignment.Right;
            }

            QueueDelegate(GetAlignmentImage);
        }

        private void GetAlignmentImage()
        {
            _alignImage = BitmapCapture.CreateBitmapFromElement(_element);
            _alignImage = BitmapUtils.CreateBorderlessBitmap(_alignImage, 2);

            if (_element is TextBox)
            {
                ((TextBox)_element).TextAlignment = TextAlignment.Left;
            }
            else
            {
                ((RichTextBox)_element).Document.TextAlignment = TextAlignment.Left;
            }

            QueueDelegate(ExecuteTrigger);
        }

        /// <summary>Focus on element</summary>
        private void ExecuteTrigger()
        {
            KeyboardInput.AddInputLocale(InputLocaleData.ArabicSaudiArabia.Identifier);

            switch (_flowDirectionInputsSwitch)
            {
                case FlowDirectionInputs.Keyboard:
                    _element.Focus();
                    if (KeyboardInput.IsBidiInputLanguageInstalled())
                    {
                        KeyboardEditingData[] data = KeyboardEditingData.GetValues(KeyboardEditingTestValue.ControlShiftRight);
                        data[0].PerformAction(_controlWrapper, null);
                        if (_element.FlowDirection != FlowDirection.RightToLeft)
                        {
                            data[0].PerformAction(_controlWrapper, null);
                        }
                        if (_element.FlowDirection != FlowDirection.RightToLeft)
                        {
                            Input.SendKeyboardInput(System.Windows.Input.Key.RightCtrl, true);
                            Input.SendKeyboardInput(System.Windows.Input.Key.RightShift, true);
                            Input.SendKeyboardInput(System.Windows.Input.Key.RightCtrl, false);
                            Input.SendKeyboardInput(System.Windows.Input.Key.RightShift, false);
                        }
                        QueueDelegate(RemoveFocus);
                    }
                    else
                    {
                        NextCombination();
                    }

                    break;

                case FlowDirectionInputs.Programmatically:
                    _element.FlowDirection = FlowDirection.RightToLeft;
                    _element.UpdateLayout();
                    QueueDelegate(VerifyTextAlignmentChanged);
                    break;

                default:
                    break;
            }
        }

        private void RemoveFocus()
        {
            _element.Focusable = false;
            QueueDelegate(VerifyTextAlignmentChanged);
        }

        private void VerifyTextAlignmentChanged()
        {
            _secondaryImage = BitmapCapture.CreateBitmapFromElement(_element);
            _secondaryImage = BitmapUtils.CreateBorderlessBitmap(_secondaryImage, 2);
            _element.Focusable = true;

            _criteria = new ComparisonCriteria();
            _criteria.MaxErrorProportion = 0.02f;
            if (ComparisonOperationUtils.AreBitmapsEqualUsingCriteria(_masterImage, _secondaryImage, out _differenceImage, _criteria, false))
            {
                Logger.Current.LogImage(_masterImage, "Master");
                Logger.Current.LogImage(_secondaryImage, "Secondary");
                Logger.Current.LogImage(_differenceImage, "Difference");
                Verifier.Verify(false, "Images are not different after changing flowdirection", true);
            }
            else
            {
                Log("~~~~~~~~ IMAGES ARE DIFFERENT AFTER CHANGING FLOWDIRECTION ~~~~~~~");
            }
            //Subclass because gradient changes sides
            if ((_element is PasswordBox) || (_element is TextBoxSubClass) || (_flowDirectionInputsSwitch == FlowDirectionInputs.Programmatically))
            {
            }
            else
            {
                Verifier.Verify(ComparisonOperationUtils.AreBitmapsEqualUsingCriteria(_alignImage, _secondaryImage, 0.01f), "Images should be same with alignment=right and flowDirection=RTL uses criteria  0.01f", true);
            }
            QueueDelegate(NextCombination);
        }

        #region Private Data.

        private UIElementWrapper _controlWrapper;
        private FrameworkElement _element;
        private ComparisonCriteria _criteria;
        private TextEditableType _editableType = null;
        private FlowDirectionInputs _flowDirectionInputsSwitch = 0;

        private string _sampleString = "SAMPLE";
        private Bitmap _masterImage = null;
        private Bitmap _secondaryImage = null;
        private Bitmap _differenceImage = null;
        private Bitmap _alignImage = null;

        #endregion Private Data.
    }
    
    /// <summary>
    /// Editing: TextBox.GetLineLength() returns wrong value when the specified line is scrolled up out of the viewport.
    /// </summary>
    [Test(0, "TextBox", "RegressionTest_447", MethodParameters = "/TestCaseType:RegressionTest_447")]
    [TestOwner("Microsoft"), TestTactics("25"), TestBugs("447"), TestWorkItem("")]
    public class RegressionTest_447 : CombinedTestCase
    {
        TextBox _tbox;
        int _linelength = 5;
        int _lines = 100;

        /// <summary>Start to run the test</summary>
        public override void RunTestCase()
        {
            _tbox = new TextBox();
            _tbox.Height = 200;
            _tbox.Width = 100;
            _tbox.TextWrapping = TextWrapping.Wrap;
            MainWindow.Content = _tbox;
            _tbox.FontSize = 30;
            _tbox.FontFamily = new System.Windows.Media.FontFamily("Tahoma");
            for (int i = 0; i < _lines; i++)
            {
                _tbox.Text += "abcde";
            }
            QueueDelegate(SetFocus);
        }

        void SetFocus()
        {
            ((TextBox)MainWindow.Content).Focus();
            ((TextBox)MainWindow.Content).Select(250, 0);
            QueueDelegate(VerifyLineLength);
        }

        void VerifyLineLength()
        {
            TextBox tbox;
            tbox = (TextBox)MainWindow.Content;
            for (int i = 0; i < _lines; i++)
            {
                Verifier.Verify(tbox.GetLineLength(i) == _linelength, "Expected line length[" + _linelength.ToString() + "], actual value[" + tbox.GetLineLength(i).ToString() + "]");
            }
            EndTest();
        }
    }

    /// <summary>
    /// Regression test for Regression_Bug569
    /// Test backspace pn TextBox when content include \r\n\n
    /// </summary>
    [Test(0, "TextBox", "TestBackSpace", MethodParameters = "/TestCaseType:TestBackSpace")]
    [TestOwner("Microsoft"), TestTactics("606"), TestLastUpdatedOn("July 12,2006"), TestBugs("569")]
    public class TestBackSpace : CustomTestCase
    {
        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            StackPanel _stackPanel = new StackPanel();

            _textBox = new TextBox();

            _stackPanel.Children.Add(_textBox);
            MainWindow.Content = _stackPanel;
            QueueDelegate(SetContent);
        }

        private void SetContent()
        {
            _textBox.Text = _strArr[_count];
            _textBox.Focus();
            QueueDelegate(KeyboardAction);
        }

        /// <summary>Focus on the textbox.</summary>
        private void KeyboardAction()
        {
            KeyboardInput.TypeString("^{END}{BS}{BS}{BS}{BS}{BS}");
            QueueDelegate(VerifyBackSpace);
        }

        private void VerifyBackSpace()
        {
            bool val = (!_textBox.Text.Contains("\r") && !_textBox.Text.Contains("\n") && !_textBox.Text.Contains("\r\n"));
            Verifier.Verify(val == true, "TextBox still contains \r \r\n OR \n ", false);
            _count++;
            if (_count == _strArr.Length)
            {
                Logger.Current.ReportSuccess();
            }
            else
            {
                QueueDelegate(SetContent);
            }
        }

        #region data.

        private TextBox _textBox;
        private string[] _strArr = { "Test\r\n\n", "Test\r\n\r", "Test\r\n\r\r\r", "Test\r\n\n\n\n", "Test\r\r\n", "Test\n\r\n", "Test\r\n\r" };
        private int _count = 0;

        #endregion data.
    }

    /// <summary>
    /// Mainly regression coverage for Regression_Bug570
    /// </summary>
    [Test(0, "TextBox", "EditingWithLigatures", MethodParameters = "/TestCaseType:EditingWithLigatures")]
    [TestOwner("Microsoft"), TestTactics("605"), TestLastUpdatedOn("July 21, 2006"), TestBugs("570")]
    public class EditingWithLigatures : ManagedCombinatorialTestCase
    {
        #region Private fields

        private TextEditableType _editableType = null;
        private Control _control;
        private StackPanel _panel;
        private UIElementWrapper _wrapper;
        private string _initialString = string.Empty;
        private string _typeString = "abc";

        #endregion Private fields

        #region Main flow

        /// <summary>Starts the combination.</summary>
        protected override void DoRunCombination()
        {
            if (!KeyboardInput.IsBidiInputLanguageInstalled())
            {
                KeyboardInput.AddInputLocale(InputLocaleData.ArabicSaudiArabia.Identifier);
            }

            QueueDelegate(DoSetUp);
        }

        private void DoSetUp()
        {
            _control = (Control)_editableType.CreateInstance();
            _wrapper = new UIElementWrapper(_control);
            _wrapper.Text = _initialString;
            _control.HorizontalAlignment = HorizontalAlignment.Center;
            _control.FlowDirection = FlowDirection.RightToLeft;
            if (_control is TextBox)
            {
                ((TextBox)_control).TextWrapping = TextWrapping.Wrap;
            }

            _panel = new StackPanel();
            _panel.Children.Add(_control);

            TestElement = _panel;

            QueueDelegate(DoFocus);
        }

        private void DoFocus()
        {
            _control.Focus();

            KeyboardInput.TypeString(_typeString);

            QueueDelegate(VerifyContentChanged);
        }

        private void VerifyContentChanged()
        {
            Verifier.Verify(_wrapper.Text != _initialString, "Verifying that content has changed", true);
            QueueDelegate(NextCombination);
        }

        #endregion Main flow
    }
}
