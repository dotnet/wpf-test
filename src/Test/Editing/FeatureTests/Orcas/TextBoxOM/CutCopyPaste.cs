// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides test cases for richTextBoxBase properties introduced in Orcas.

using System;
using System.Collections.Generic;
using System.Text;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Test.Uis.TestTypes;
using Microsoft.Test.Threading;
using Microsoft.Test.Input;
using Test.Uis.Loggers;
using System.Collections;
using Test.Uis.Wrappers;
using Test.Uis.Utils;
using Test.Uis.Data;
using Test.Uis.Management;


namespace Test.Uis.TextEditing
{
    /// <summary> Text selection possibilities </summary>
    enum SelectTextOption
    {
        SelectEmptyText,
        SelectAll,
        SelectMiddle,
        SelectEnd,
        NoSelection,
    }

    /// <summary> Cup Copy Paste possibilities </summary>
    enum CutCopyPasteOperation
    {
        CutPaste,
        CopyPaste,
        CutCopyPaste,
    }

    /// <summary>Test class for CutCopyPaste commands emulation </summary>
    [Test(0, "TextBoxBase", "IndicCutCopyPaste", MethodParameters = "/TestCaseType:IndicCutCopyPaste")]
    [TestOwner("Microsoft"), TestTactics("421"), TestWorkItem("65"), TestBugs("464")]
    public class IndicCutCopyPaste : ManagedCombinatorialTestCase
    {
        /// <summary> Returns hashtable values if not password </summary>
        protected override bool DoReadCombination(Hashtable values)
        {
            bool result = base.DoReadCombination(values);
            if (_editableType.IsPassword)
            {
                result = false;
            }
            return result; 
        }

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
            switch (_selectTextOptionSwitch)
            {
                case SelectTextOption.SelectEmptyText:
                    _ControlWrapper.Text = String.Empty;
                    _initialString = _ControlWrapper.Text;
                    _ControlWrapper.SelectAll();
                    switch (_cutCopyPasteSwitch)
                    {
                        case CutCopyPasteOperation.CopyPaste:
                            SelectEmptyTextCopyPaste();
                            break;

                        case CutCopyPasteOperation.CutPaste:
                            SelectEmptyTextCutPaste();
                            break;

                        case CutCopyPasteOperation.CutCopyPaste:
                            SelectEmptyTextCutCopyPaste();
                            break;
                    }
                    break;

                case SelectTextOption.SelectAll:
                    _initialString = _ControlWrapper.Text;
                    _ControlWrapper.SelectAll();
                    switch (_cutCopyPasteSwitch)
                    {
                        case CutCopyPasteOperation.CopyPaste:
                            SelectAllTextCopyPaste();
                            break;

                        case CutCopyPasteOperation.CutPaste:
                            SelectAllTextCutPaste();
                            break;

                        case CutCopyPasteOperation.CutCopyPaste:
                            SelectAllTextCutCopyPaste();
                            break;
                    }
                    break;

                case SelectTextOption.SelectEnd:
                    _initialString = _ControlWrapper.Text;
                    _element.Focus();
                    KeyboardInput.TypeString("{RIGHT 4}^+{END}");
                    switch (_cutCopyPasteSwitch)
                    {
                        case CutCopyPasteOperation.CopyPaste:
                            QueueDelegate(TextEndCopyPaste);
                            break;

                        case CutCopyPasteOperation.CutPaste:
                            QueueDelegate(TextEndCutPaste);
                            break;

                        case CutCopyPasteOperation.CutCopyPaste:
                            QueueDelegate(TextEndCutCopyPaste);
                            break;
                    }
                    break;

                case SelectTextOption.SelectMiddle:
                    _initialString = _ControlWrapper.Text;
                    _ControlWrapper.Select(3, 14);
                    switch (_cutCopyPasteSwitch)
                    {
                        case CutCopyPasteOperation.CopyPaste:
                            TextMiddleCopyPaste();
                            break;

                        case CutCopyPasteOperation.CutPaste:
                            TextMiddleCutPaste();
                            break;

                        case CutCopyPasteOperation.CutCopyPaste:
                            TextMiddleCutCopyPaste();
                            break;
                    }
                    break;

                case SelectTextOption.NoSelection:
                    _initialString = _ControlWrapper.Text;
                    switch (_cutCopyPasteSwitch)
                    {
                        case CutCopyPasteOperation.CopyPaste:
                            TextNoSelectionCopyPaste();
                            break;

                        case CutCopyPasteOperation.CutPaste:
                            TextNoSelectionCutPaste();
                            break;

                        case CutCopyPasteOperation.CutCopyPaste:
                            TextNoSelectionCutCopyPaste();
                            break;
                    }
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
            if (((_cutCopyPasteSwitch == CutCopyPasteOperation.CutPaste) || (_cutCopyPasteSwitch == CutCopyPasteOperation.CutCopyPaste))
                && (_element is RichTextBox))
            {
                //When you perform cut on all text \r\n is still left behind so pasting will paste text before the esisiting \r\n
                //  _initialString = (SelectTextOptionSwitch == SelectTextOption.SelectAll)? (_initialString + "\r\n"):_initialString;
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
            _finalString = _ControlWrapper.Text;
            Verifier.Verify(_initialString == _finalString, "The strings are not equal. Expected [" + _initialString +
                "] Actual [" + _finalString + "]");
            NextCombination();

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
                if ((_cutCopyPasteSwitch == CutCopyPasteOperation.CutPaste || _cutCopyPasteSwitch == CutCopyPasteOperation.CutCopyPaste))
                {
                    //copying and pasting end of document places cursor before end
                    //cut and paste of end of document places cursor after end (\r\n) of doc 
                    //at the end there are 2 \r\n's ---> when u cut \r\n is saved but \r\n is created for existing line
                    //when u paste line \r\n is copied bacn and the existing \r\n is put at the end of doc with caret one position before doc end
                    _initialString += "\r\n";
                }
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
            ((TextBoxBase)_element).FontFamily = new FontFamily("Tunga");
            if (_element is RichTextBox)
            {
                Run r1 = new Run();
                r1.FontWeight = FontWeights.Bold;
                r1.FontSize = 40;
                r1.Text = TextScript.FindByName("Kannada").Sample;
                
                ((RichTextBox)_element).Document = new FlowDocument(new Paragraph(r1));

                ((RichTextBox)_element).Document.Blocks.Add(new Paragraph(new Run("\x0C80\x0C90 \x0CA0\x0CB0")));
            }
            else
            {
                _ControlWrapper.Text = TextScript.FindByName("Kannada").Sample + "\r\n\x0C80\x0C90 \x0CA0\x0CB0"; 
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
        private SelectTextOption _selectTextOptionSwitch = 0;
        private CutCopyPasteOperation _cutCopyPasteSwitch = 0;
        private string _initialString;
        private string _finalString;
        private string _hello = "hello";
        private string _world = "world";

        #endregion private data.
    }
}
