// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 23 $ $Source: //depot/vbl_wcp_avalon_dev/windowstest/client/wcptests/uis/Text/BVT/RichText/FigureAndFloaterTest.cs $")]
namespace Test.Uis.TextEditing
{
    #region Namespaces.
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Input;
    using System.Windows.Threading;

    using Test.Uis.Data;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;
    #endregion Namespaces.

    /// <summary>Figure And Floater testing</summary>
    [TestOwner("Microsoft"), TestWorkItem("72,1,2,75"), TestTactics("698"), TestBugs("187,188")]
    public class FigureAndFloaterTest : ManagedCombinatorialTestCase
    {
        #region Main flow.

        ///<summary>Prepare RichTextBox content.</summary>
        protected override void DoRunCombination()
        {
            _control = (RichTextBox)_editableType.CreateInstance();
            _wrapper = new UIElementWrapper(_control);
            TestElement = _control;
            _control.SpellCheck.IsEnabled = true;
            
            string str;
            if(_figureOrFloater == "Figure")
                str = "<Figure Background='orange' Height='50' Width='100' HorizontalAnchor='ContentRight' VerticalAnchor='ContentTop' VerticalOffset='10' WrapDirection='both'><Paragraph>" + _misspellWord + "</Paragraph></Figure>";
            else
                str = "<Floater Background='orange' HorizontalAlignment='Stretch' Width='200'><Paragraph>" + _misspellWord + "</Paragraph></Floater>";
            string content = "<Paragraph Background='yellow'>1</Paragraph><Paragraph>" + str + "</Paragraph><Paragraph Background='yellow'>2</Paragraph>";
            
            Log("RichTextBox xaml [" + content + "]");
            _wrapper.XamlText = content;
            
            QueueDelegate(SpellCheckTest);
        }

        #region 72: SpellCheck

        /// <summary>Testing SpellCheck inside figure or floater (work item 72)</summary>
        private void SpellCheckTest()
        {
            Log("----------Action: Mouse right click after 'Mi' in 'Misspellword'");
            _tp = _wrapper.Start.GetInsertionPosition(LogicalDirection.Forward);
            _tp = _tp.GetPositionAtOffset(9, LogicalDirection.Forward);
            _point = MouseEditingTestHelper.FindPoint(_tp, _wrapper);            
            MouseInput.RightMouseDown(_point);
            MouseInput.RightMouseUp();

            //Add some delay for context menu to comeup.
            QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromMilliseconds(500), new SimpleHandler(CorrectWord), null);
        }

        private void CorrectWord()
        {
            Log("----------Action: {DOWN}{ENTER} to select available suggestion");
            KeyboardInput.TypeString("{DOWN}{ENTER}");
            QueueDelegate(VerifySpellCheck);
        }

        private void VerifySpellCheck()
        {
            Verifier.Verify(_wrapper.Text.Contains(_correctWord), "SpellCheck in " + _figureOrFloater + " works." +
                "\nExpect["+_correctWord+"] Actual[" + _wrapper.Text + "]");

            Log("----------Action: do ^z");
            KeyboardInput.TypeString("^z");
            QueueDelegate(MouseSelectionTest);
        }

        #endregion 72: SpellCheck

        #region 1: Mouse selection test

        /// <summary>
        /// Testing mouse selection inside figure and floater (work item 1)
        /// Regression_Bug187 -When floater is selected, text after the floater is selected
        /// </summary>
        private void MouseSelectionTest()
        {
            Log("----------Action: Mouse down after 'Mi' and drag down to below " + _figureOrFloater);
            Point end = _point;
            end.Y = _point.Y + 50;
            MouseInput.MouseDown(_point);
            MouseInput.MouseMove(end);
            MouseInput.MouseUp();
            
            QueueDelegate(VerifyMouseDrag);
        }

        private void VerifyMouseDrag()
        {
            Verifier.Verify(_wrapper.SelectionInstance.Text == "\r\n" + _misspellWord + "\r\n\r\n2\r\n", "Mouse Drag works." +
                "\nExpect[" + "\r\n" + _misspellWord + "\r\n\r\n2\r\n" + "] Actual[" + _wrapper.SelectionInstance.Text + "]");

            Log("----------Action: Mouse double click after 'Mi'"); // (
            MouseInput.MouseClick(_point);
            MouseInput.MouseClick(_point);
            QueueDelegate(VerifyMouseDoubleClick);
        }

        private void VerifyMouseDoubleClick()
        {
            Verifier.Verify(_wrapper.SelectionInstance.Text == _misspellWord, "Mouse double click works."+
                "\nExpect[" + _misspellWord + "] Actual[" + _wrapper.SelectionInstance.Text + "]");

            QueueDelegate(TypingTest);
        }

        #endregion 1: Mouse selection test

        #region 2: typing test

        /// <summary>
        /// Typing over selection and Typing around the Figure, Floater (work item 2)
        /// Regression_Bug188 - type 'k' to right of figure causes ArgumentNullException.
        /// </summary>
        private void TypingTest()
        {
            Log("----------Action: Type 'abc' over the existing selection");
            KeyboardInput.TypeString("abc");
            QueueDelegate(VerifyTypingOverSelection);
        }

        private void VerifyTypingOverSelection()
        {
            Verifier.Verify(_wrapper.Text.Contains("abc"), "abc is typed."+
                "\nExpect to contain [abc] Actual[" + _wrapper.Text + "]");
            Verifier.Verify(!_wrapper.Text.Contains(_misspellWord), "Selection is delete."+
                "\nExpect not contain [" + _misspellWord + "] Actual[" + _wrapper.Text + "]");

            Log("----------Action: {RIGHT}k+{LEFT}");
            KeyboardInput.TypeString("{RIGHT}k+{LEFT}");
            QueueDelegate(VerifyTyping);
        }

        private void VerifyTyping()
        {
            Verifier.Verify(_wrapper.SelectionInstance.Text == "k", "k is typed"+
                "\nExpect [k] Actual[" + _wrapper.SelectionInstance.Text + "]");

            Log("----------Action: {LEFT 3} to move caret away from selection");
            KeyboardInput.TypeString("{LEFT 3}");
            QueueDelegate(KeyboardSelectionTest);
        }

        #endregion 2: typing test

        #region 3: Keyboard selection test

        /// <summary>
        /// Keyboard selection (+right, +left, +up, +down) around the Figure, Floater (work item 3)
        /// Regression_Bug189 - Unable to unselect when selection includes a floater along with text inside RichTextBox
        /// </summary>
        private void KeyboardSelectionTest()
        {
            Log("----------Action: +{RIGHT 5} to select include Figure/Floater");
            KeyboardInput.TypeString("+{RIGHT 5}");
            QueueDelegate(VerifyShiftRight);
        }

        private void VerifyShiftRight()
        {
            Verifier.Verify(_wrapper.SelectionInstance.Text == "\r\nabc\r\nk\r\n2", "ShiftRight works."+
                "\nExpect [\r\nabc\r\nk\r\n2] Actual[" + _wrapper.SelectionInstance.Text + "]");

            Log("----------Action: +{LEFT 12} to deselect and re-select");
            KeyboardInput.TypeString("+{LEFT 12}");
            QueueDelegate(VerifyShiftLeft);
        }

        private void VerifyShiftLeft()
        {
            Verifier.Verify(_wrapper.SelectionInstance.Text == "1\r\n\r\nabc\r\n", "ShiftLeft works." +
                "\nExpect [1\r\n\r\nabc\r\n] Actual[" + _wrapper.SelectionInstance.Text + "]");

            Log("----------Action: +{DOWN 4} to deselect then select");
            KeyboardInput.TypeString("+{DOWN 4}");
            QueueDelegate(VerifyShiftDown);
        }

        private void VerifyShiftDown()
        {
            Verifier.Verify(_wrapper.SelectionInstance.Text == "\r\nabc\r\nk\r\n2\r\n", "ShiftDown works." +
                "\nExpect [\r\nabc\r\nk\r\n2\r\n] Actual[" + _wrapper.SelectionInstance.Text + "]");

            Log("----------Action: +{UP 4} to deselect then select");
            KeyboardInput.TypeString("+{UP 4}");
            QueueDelegate(VerifyShiftUp);
        }

        private void VerifyShiftUp()
        {
            Verifier.Verify(_wrapper.SelectionInstance.Text == "1\r\n\r\nabc\r\n", "ShiftUp works." +
                "\nExpect [1\r\n\r\nabc\r\n] Actual[" + _wrapper.SelectionInstance.Text + "]");

            QueueDelegate(KeyboardNavigationTest);
        }

        #endregion 3: Keyboard selection test
        
        #region 5: Keyboard navigation

        /// <summary>
        /// Keyboard navigation (right, left, up, down) around the Figure, Floater (work item 5)
        /// </summary>
        private void KeyboardNavigationTest()
        {
            Log("----------Action: {RIGHT 10} to navigate to end of doc.");
            KeyboardInput.TypeString("{RIGHT 10}");
            QueueDelegate(VerifyRight);
        }

        private void VerifyRight()
        {
            _tp = _wrapper.SelectionInstance.Start;
            string temp = _tp.GetTextInRun(LogicalDirection.Backward);
            Verifier.Verify(temp == "2", "Right works.\nExpect [2] Actual[" + temp + "]");

            Log("----------Action: {LEFT 10} to navigate to begining of doc.");
            KeyboardInput.TypeString("{LEFT 10}");
            QueueDelegate(VerifyLeft);
        }

        private void VerifyLeft()
        {
            _tp = _wrapper.SelectionInstance.Start;
            string temp = _tp.GetTextInRun(LogicalDirection.Forward);
            Verifier.Verify(temp == "1", "Left works.\nExpect [1] Actual[" + temp + "]");

            Log("----------Action: {DOWN 3} to navigate to end of doc.");
            KeyboardInput.TypeString("{DOWN 3}");
            QueueDelegate(VerifyDown);
        }

        private void VerifyDown()
        {
            _tp = _wrapper.SelectionInstance.Start;
            string temp = _tp.GetTextInRun(LogicalDirection.Forward);
            Verifier.Verify(temp == "2", "Down works.\nExpect [2] Actual[" + temp + "]");

            Log("----------Action: {UP 3} to navigate to begining of doc.");
            KeyboardInput.TypeString("{UP 3}");
            QueueDelegate(VerifyUp);
        }

        private void VerifyUp()
        {
            _tp = _wrapper.SelectionInstance.Start;
            string temp = _tp.GetTextInRun(LogicalDirection.Forward);
            Verifier.Verify(temp == "1", "Up works.\nExpect [1] Actual[" + temp + "]");
            
            QueueDelegate(ApplyingFormattingTest);
        }

        #endregion 5: Keyboard navigation
        
        #region 75: Applying formatting

        /// <summary>
        /// Applying formatting in Figure, Floater (work item 75).
        /// Bold, Italic, Underline, TextAlignment (center, right, left), FlowDirections (RTL), Grow/Shrink font
        /// </summary>
        private void ApplyingFormattingTest()
        {
            Log("----------Action: {RIGHT 3}+{RIGHT 3}^b");
            KeyboardInput.TypeString("{RIGHT 3}+{RIGHT 3}^b");
            QueueDelegate(VerifyBold);
        }

        private void VerifyBold()
        {
            string temp = XamlUtils.TextRange_GetXml(new TextRange(_wrapper.SelectionInstance.Start, _wrapper.SelectionInstance.End));
            Verifier.Verify(temp.Contains("Bold"), "Bold works.\nExpect to contain [Bold] Actual[" + temp + "]");

            Log("----------Action: ^z^i");
            KeyboardInput.TypeString("^z^i");
            QueueDelegate(VerifyItalic);
        }

        private void VerifyItalic()
        {
            string temp = XamlUtils.TextRange_GetXml(new TextRange(_wrapper.SelectionInstance.Start, _wrapper.SelectionInstance.End));
            Verifier.Verify(temp.Contains("Italic"), "Italic works.\nExpect to contain [Italic] Actual[" + temp + "]");
            Verifier.Verify(!temp.Contains("Bold"), "Shouldn't contain Bold.");

            Log("----------Action: ^z^u");
            KeyboardInput.TypeString("^z^u");
            QueueDelegate(VerifyUnderline);
        }

        private void VerifyUnderline()
        {
            string temp = XamlUtils.TextRange_GetXml(new TextRange(_wrapper.SelectionInstance.Start, _wrapper.SelectionInstance.End));
            Verifier.Verify(temp.Contains("Underline"), "Underline works.\nExpect to contain [Underline] Actual[" + temp + "]");
            Verifier.Verify(!temp.Contains("Italic"), "Shouldn't contain Italic.");

            Log("----------Action: ^z^e");
            KeyboardInput.TypeString("^z^e");
            QueueDelegate(VerifyTextAlignmentCenter);
        }

        private void VerifyTextAlignmentCenter()
        {
            string temp = XamlUtils.TextRange_GetXml(new TextRange(_wrapper.Start, _wrapper.End));
            Verifier.Verify(temp.Contains("TextAlignment=\"Center\""), "Center TextAlignment works.\nExpect to contain [Center] Actual[" + temp + "]");
            Verifier.Verify(!temp.Contains("Underline"), "Shouldn't contain Underline.");

            Log("----------Action: ^r");
            KeyboardInput.TypeString("^r");
            QueueDelegate(VerifyTextAlignmentRight);
        }

        private void VerifyTextAlignmentRight()
        {
            string temp = XamlUtils.TextRange_GetXml(new TextRange(_wrapper.Start, _wrapper.End));
            Verifier.Verify(temp.Contains("TextAlignment=\"Right\""), "Right TextAlignment works.\nExpect to contain [Right] Actual[" + temp + "]");
            Verifier.Verify(!temp.Contains("TextAlignment=\"Center\""), "Shouldn't contain Center.");

            Log("----------Action: ^l");
            KeyboardInput.TypeString("^l");
            QueueDelegate(VerifyTextAlignmentLeft);
        }

        private void VerifyTextAlignmentLeft()
        {
            string temp = XamlUtils.TextRange_GetXml(new TextRange(_wrapper.Start, _wrapper.End));
            Verifier.Verify(temp.Contains("TextAlignment=\"Left\""), "Left TextAlignment works.\nExpect to contain [Left] Actual[" + temp + "]");
            Verifier.Verify(!temp.Contains("TextAlignment=\"Right\""), "Shouldn't contain Right.");

            if (!KeyboardInput.IsBidiInputLanguageInstalled())
            {
                Log("Adding ArabicSaudiArabia keyboard...");
                KeyboardInput.AddInputLocale(InputLocaleData.ArabicSaudiArabia.Identifier);
            }

            QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromMilliseconds(500), new SimpleHandler(DoControlRightShift), null);
        }

        private void DoControlRightShift()
        {
            Log("----------Action: {right control right shift} of change FlowDirection to righttoleft");
            KeyboardInput.PressVirtualKey(Test.Uis.Wrappers.Win32.VK_RCONTROL);
            KeyboardInput.PressVirtualKey(Test.Uis.Wrappers.Win32.VK_RSHIFT);            
            KeyboardInput.ReleaseVirtualKey(Test.Uis.Wrappers.Win32.VK_RSHIFT);
            KeyboardInput.ReleaseVirtualKey(Test.Uis.Wrappers.Win32.VK_RCONTROL);
            QueueDelegate(VerifyRTLFlowDirection);
        }

        private void VerifyRTLFlowDirection()
        {
            string temp = XamlUtils.TextRange_GetXml(new TextRange(_wrapper.Start, _wrapper.End));
            Verifier.Verify(temp.Contains("Paragraph FlowDirection=\"RightToLeft\""), " FlowDirection works.\nExpect to contain [Paragraph FlowDirection=\"RightToLeft\"] Actual[" + temp + "]");

            Log("----------Action: ^]^]^]^]^]^] for grow font");
            KeyboardInput.TypeString("^]^]^]^]^]^]");
            QueueDelegate(VerifyGrowFont);
        }

        private void VerifyGrowFont()
        {
            string temp = XamlUtils.TextRange_GetXml(new TextRange(_wrapper.Start, _wrapper.End));
            Verifier.Verify(temp.Contains("<Run FlowDirection=\"LeftToRight\" FontSize=\"15.5\">abc</Run>"), " Grow font works.\nExpect to contain FontSize [15.5] Actual[" + temp + "]");

            Log("----------Action: ^[^[^[^[^[ for shrink font");
            KeyboardInput.TypeString("^[^[^[^[^[");
            QueueDelegate(VerifyShrinkFont);
        }

        private void VerifyShrinkFont()
        {
            string temp = XamlUtils.TextRange_GetXml(new TextRange(_wrapper.Start, _wrapper.End));
            Verifier.Verify(temp.Contains("<Run FlowDirection=\"LeftToRight\" FontSize=\"11.75\">abc</Run>"), " Shrink font works.\nExpect to contain FontSize [11.75] Actual[" + temp + "]");
            QueueDelegate(NextCombination);
        }

        #endregion 75: Applying formatting

        #endregion Main flow.

        #region Private fields.
        private RichTextBox _control;
        private UIElementWrapper _wrapper;
        private Point _point;
        private TextPointer _tp;
        private string _misspellWord = "Misspellword";
        private string _correctWord = "Misspell word";
        private TextEditableType _editableType = null;
        private string _figureOrFloater = string.Empty;
        #endregion Private fields.
    }
}
