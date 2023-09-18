// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Test that undo stack are created correctly ind different situations


namespace Test.Uis.TextEditing
{
    #region Namespaces.
    using System;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.TestTypes;
    using Test.Uis.Management;
    using Test.Uis.Wrappers;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows;
    using System.Collections;
    using Test.Uis.Data;
    using Microsoft.Test;
    using Microsoft.Test.Discovery;
    #endregion Namespaces.

    /// <summary>
    /// Test UndoDelimiter in TextBox and RichTextBox.
    ///</summary>
     [TestOwner("Microsoft"), TestBugs("528, 771, 772, 539"), TestTactics("370"), TestWorkItem("28")]
    public class KeyboardUndoDelimiterTest : ManagedCombinatorialTestCase
    {
        /// <summary>Element Wraper for Text Controls</summary>
        private UIElementWrapper _wrapper;
        /// <summary>Final string value after all actions. It is for verification.</summary>
        private string _finalExpectedString = null;
        /// <summary>Initial text in the control</summary>
        private string _startText=string.Empty;
        /// <summary> The control to be tested</summary>
        private string _textControl=string.Empty;
        /// <summary>specify if the control accpets return.</summary>
        bool _acceptsReturn= false;
        /// <summary>Keyboard editing data for Undo boundary.</summary>
        private KeyboardEditingData _delimiter = null ; 
     
        /// <summary>
        /// start to run the combinataion.
        /// </summary>
        protected override void DoRunCombination()
        {
             _wrapper = new UIElementWrapper(TextEditableType.GetByName(_textControl).CreateInstance());
             MainWindow.Content = _wrapper.Element;
             ((System.Windows.Controls.Primitives.TextBoxBase)(_wrapper.Element)).AcceptsReturn = _acceptsReturn;
             ((System.Windows.Controls.Primitives.TextBoxBase)(_wrapper.Element)).Width = 200;
             QueueDelegate(SetFocus);
        }

        /// <summary>Set focus to the TextControl</summary>
        void SetFocus()
        {
            _wrapper.Element.Focus();
            QueueDelegate(PerfromAction);
        }

        /// <summary>Keyboard input for each delimiters and calculate the expected values </summary>
 
        void PerfromAction()
        {
            string StartString = _startText ;
            string EndString = "xy";

           //After Regression_Bug310 is fixed we will fixed the if statement.
            if(
              _delimiter.TestValue == KeyboardEditingTestValue.PageUpControl ||
              _delimiter.TestValue == KeyboardEditingTestValue.DeleteControl ||
              _delimiter.TestValue == KeyboardEditingTestValue.PageDownControl
             )
            {
                _finalExpectedString = "";
            }
            else if (_wrapper.IsElementRichText)
            {
                switch (_delimiter.TestValue)
                {
                    case KeyboardEditingTestValue.Backspace:
                    case KeyboardEditingTestValue.BackspaceControl:
                    case KeyboardEditingTestValue.BackspaceShift:
                        _finalExpectedString = StartString.Substring(0, StartString.Length > 0 ? StartString.Length - 1 : 0);
                        break;
                    case KeyboardEditingTestValue.CenterJustifyCommandKeys:
                    case KeyboardEditingTestValue.LeftJustifyCommandKeys:
                    case KeyboardEditingTestValue.RightJustifyCommandKeys:
                        _finalExpectedString = StartString + "\r\n";
                        break;
                    case KeyboardEditingTestValue.PasteCommandKeys:
                        System.Windows.Clipboard.SetDataObject("");
                        _finalExpectedString = StartString;
                        break;
                    case KeyboardEditingTestValue.Enter:
                        if (_acceptsReturn)
                            _finalExpectedString = StartString + "\r\n\r\n";
                        else
                            _finalExpectedString = "";
                        break;
                    case KeyboardEditingTestValue.EnterShift:
                        if (_acceptsReturn)
                            _finalExpectedString = StartString + "\r\n";
                        else
                            _finalExpectedString = "";
                        break;
                    case KeyboardEditingTestValue.UndoCommandKeys:
                        _finalExpectedString = "";
                        break;
                    default: _finalExpectedString = StartString;
                        break;
                }
            }
            else
            {
                switch (_delimiter.TestValue)
                {
                    //TextBox won't support formating
                    case KeyboardEditingTestValue.BoldCommandKeys:
                    case KeyboardEditingTestValue.UnderlineCommandKeys:
                    case KeyboardEditingTestValue.ItalicCommandKeys:
                    case KeyboardEditingTestValue.CenterJustifyCommandKeys:
                    case KeyboardEditingTestValue.LeftJustifyCommandKeys:
                    case KeyboardEditingTestValue.RightJustifyCommandKeys:
                        _finalExpectedString = "";
                        break;
                    case KeyboardEditingTestValue.Backspace:
                    case KeyboardEditingTestValue.BackspaceShift:
                        _finalExpectedString = StartString.Substring(0, StartString.Length > 0 ? StartString.Length - 1 : 0);
                        break;
                    case KeyboardEditingTestValue.PasteCommandKeys:
                        System.Windows.Clipboard.SetDataObject("");
                        _finalExpectedString = StartString;
                        break;
                    case KeyboardEditingTestValue.Enter:
                    case KeyboardEditingTestValue.EnterShift:
                        if (_acceptsReturn)
                            _finalExpectedString = StartString + "\r\n";
                        else
                            _finalExpectedString = "";
                        break;
                    case KeyboardEditingTestValue.UndoCommandKeys:
                        _finalExpectedString = "";
                        break;
                    default: _finalExpectedString = StartString;
                        break;
                }
            }
            //keyboard input
            KeyboardInput.TypeString(StartString);
            _delimiter.PerformAction(_wrapper, null);
            KeyboardInput.TypeString(EndString + "^z");

            QueueDelegate(EvaluateResult);
        }
       
        /// <summary> evaluate the result for each delimiter</summary>
        void EvaluateResult()
        {
            Verifier.Verify(_wrapper.Text ==_finalExpectedString, "Expected text[" + _finalExpectedString + "], Actual text[" + _wrapper.Text + "]");
            QueueDelegate(NextCombination);
        }
    }

    /// <summary>
    /// This class test some delimiters that are not keyboard typing.
    /// </summary>
    [Test(0, "UndoRedo", "FocusUndoDelimiterTest", MethodParameters = "/TestCaseType=FocusUndoDelimiterTest /Priority=0", Timeout=250)]
    [TestOwner("Microsoft"), TestBugs("650"), TestTactics("369"), TestWorkItem("28")]
    public class FocusUndoDelimiterTest : ManagedCombinatorialTestCase
    {
        /// <summary>
        /// CurrentWrapper is the wrapper for typing text and perfrom Undo
        /// SupportWrapper is the wrapper for helping to change focus.
        /// </summary>
        private UIElementWrapper _currentWrapper,_supportWrapper;
        /// <summary>specify how the foucs is losted.</summary>
        private string _loseFocus=string.Empty;
        /// <summary>Control name</summary>
        private string _textControl=string.Empty;
        /// <summary>Text for the control for the top frame of the undo stack.</summary>
        private string _textForFirstUndoUnit=string.Empty;

        /// <summary>perfrom the initialization</summary>
        private void SetInitValue()
        {
            RichTextBox rb = new RichTextBox();
            rb.Width = 200;
            rb.Background = Brushes.Wheat;
            TextBox tb = new TextBox();
            tb.Width = 200;

            if (_textControl == "TextBox")
            {
                _currentWrapper = new UIElementWrapper(tb);
                _supportWrapper = new UIElementWrapper(rb);
            }
            else
            {
                _currentWrapper = new UIElementWrapper(rb);
                _supportWrapper = new UIElementWrapper(tb);
            }
            Canvas canvas = new Canvas();
            Canvas.SetTop(rb, 0);
            Canvas.SetLeft(rb, 0);
            Canvas.SetTop(tb, 300);
            Canvas.SetLeft(tb, 300);
            canvas.Children.Add(rb);
            canvas.Children.Add(tb);
            MainWindow.Content = canvas;
        }

        /// <summary>
        /// Start to run the combination.
        /// </summary>
        protected override void DoRunCombination()
        {
            SetInitValue();
            
            QueueDelegate(SetFocus);
        }

        /// <summary>Set the focus to the working box</summary>
        private void SetFocus()
        {
            _currentWrapper.Element.Focus();
            QueueDelegate(CreateFirstUndoUnit);
        }

        /// <summary>Type to create the first undoUnit</summary>
        private void CreateFirstUndoUnit()
        {
            KeyboardInput.TypeString(_textForFirstUndoUnit);
            QueueDelegate(LostFocus);
        }
       
        /// <summary>working box lose focus</summary>
        void LostFocus()
        {
            if (_loseFocus == "ControlOutOfFocus")
            {
                _supportWrapper.Element.Focus();
            }
            else if (_loseFocus == "AppOutOfFocus")
            {
                System.Diagnostics.Process ps = Avalon.Test.Win32.Interop.LaunchAProcess("Notepad.exe", "");
                System.Threading.Thread.Sleep(500);
                ps.Kill();
            }
            else if (_loseFocus == "MouseClickOnTestControl")
            {
                MouseInput.MouseClick(_currentWrapper.Element);
            }
            else
            {
                MouseInput.MouseClick(_supportWrapper.Element);
            }
            QueueDelegate(GainFocus);
        }

        /// <summary>get the focus back to the working box.</summary>
        private void GainFocus()
        {
            if (_loseFocus == "ControlOutOfFocus")
            {
                _currentWrapper.Element.Focus();
            }
            if (_loseFocus == "MouseClickOutOfTestControl")
            {
                MouseInput.MouseClick(_currentWrapper.Element);
            }
            QueueDelegate(CreateSecondUndoUnit);
        }

        /// <summary>Try to create the second Undo unit on the top of the stack</summary>
        private void CreateSecondUndoUnit()
        {
            KeyboardInput.TypeString("1234");
            QueueDelegate(PerformUndo);
        }
        
        /// <summary>Perform undo</summary>
        private void PerformUndo()
        {
            KeyboardInput.TypeString("^z");
            QueueDelegate(VerifyResult);
        }
        
        /// <summary>check the result</summary>
        void VerifyResult()
        {            
            string text = _textForFirstUndoUnit;
            //if ((string)values["TextControl"] == "RichTextBox" && text!="")
            if (_textControl == "RichTextBox")
                text += "\r\n";
            bool pass = _currentWrapper.Text == text; 
            
            //When Regression_Bug308 is fixed, the case will fail and we will remove the if statement.            
            if (_loseFocus == "AppOutOfFocus" && _textForFirstUndoUnit != "")
            {
              pass = !pass;
              Log("When Regression_Bug308 is fixed, the case will fail and we will remove the if statement.");
            }
            Verifier.Verify(pass, "Failed: expected Text[" + _textForFirstUndoUnit + "], Actual Text[" + _currentWrapper.Text + "]");
            QueueDelegate(NextCombination);
        }
    }
}
