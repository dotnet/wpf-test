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


namespace Test.Uis.TextEditing
{
    /// <summary>
    /// Tests TextChange value passed in the TextChanged event for richTextBox
    /// </summary>
    [Test(0, "RichTextBox", "RichTextBoxTextChangedEvent", MethodParameters = "/TestCaseType=RichTextBoxTextChangedEvent")]
    public class RichTextBoxTextChangedEvent : CustomTestCase
    {
        #region Public Members

        /// <summary>
        /// Entry point to test case
        /// </summary>
        public override void RunTestCase()
        {
            _richTextBox = new RichTextBox();
            UIElementWrapper rtbWrapper = new UIElementWrapper(_richTextBox);
            TestWindow.Content = _richTextBox;
            rtbWrapper.Text = s_initialText;
            _richTextBox.SpellCheck.IsEnabled = true;

            _richTextBox.TextChanged += new TextChangedEventHandler(richTextBox_TextChanged);
            DispatcherHelper.DoEvents();

            while (_currentOperation < Enum.GetValues(typeof(Operations)).Length)
            {
                _triggerCount = 0;
                _richTextBox.Focus();
                _intialTagTextCount = CountTagsAndText();
                
                switch ((Operations) _currentOperation)
                {
                    case Operations.TypeInsert:
                        Log("------- Inside TypeInsert -------");
                        KeyboardInput.TypeString("^{HOME}{RIGHT 4}"+ EditingCommandData.ToggleBold.KeyboardShortcut);
                        DispatcherHelper.DoEvents(500);
                        Input.SendUnicodeString(s_stringSample, 1, 500);
                        DispatcherHelper.DoEvents();
                        TriggerCountVerifier(s_stringSample.Length);
                        break;

                    case Operations.Delete:
                        Log("------- Inside Delete -------");
                        //offset is +2 because there is a document and paragraph tag at the beginning
                        _richTextBox.Selection.Select(_richTextBox.Document.ContentStart,_richTextBox.Document.ContentStart.GetPositionAtOffset(2+s_selectedTextLength));
                        _richTextBox.Cut();
                        TriggerCountVerifier(1);
                        break;

                    case Operations.Paste:
                        Log("------- Inside Paste -------");
                        Clipboard.SetData(DataFormats.Text, s_pasteString);
                        _richTextBox.Paste();
                        TriggerCountVerifier(1);
                        break;

                    case Operations.Overwrite:
                        Log("------- Inside Overwrite -------");
                        _richTextBox.Selection.Select(_richTextBox.Document.ContentStart, _richTextBox.Document.ContentStart.GetPositionAtOffset(2 + s_selectedTextLength));
                        Input.SendUnicodeString("Z");
                        DispatcherHelper.DoEvents();
                        TriggerCountVerifier(1);
                        break;

                    case Operations.Speller:
                        Log("------- Inside Speller -------");
                        //Handler is unhooked so that the event doesnt need to handle the case when text is initialized
                        //on the next line. Prevents a conditional in the event.
                        _richTextBox.TextChanged -= new TextChangedEventHandler(richTextBox_TextChanged);
                        rtbWrapper.Text = s_initialText;
                        _richTextBox.TextChanged += new TextChangedEventHandler(richTextBox_TextChanged);
                        //This should work ... In case it doesnt we still want to continue.. the speller cases will catch the error
                        if (_richTextBox.GetSpellingError(_richTextBox.Document.ContentStart) != null)
                        {
                            _richTextBox.GetSpellingError(_richTextBox.Document.ContentStart).Correct(s_stringSample);
                            TriggerCountVerifier(1);
                        }
                        break;

                    case Operations.ChangeBlock:
                        Log("------- Inside ChangeBlock -------");
                        Clipboard.SetData(DataFormats.Text, s_pasteString);
                        using (_richTextBox.DeclareChangeBlock())
                        {
                            _richTextBox.Paste();
                            _richTextBox.Paste();
                        }
                        TriggerCountVerifier(1);
                        break;

                    default:
                        break;
                }
                _currentOperation++;
            }

            Logger.Current.ReportSuccess();
        }

        #endregion

        #region private Members

        private void richTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _triggerCount++;

            Verifier.Verify(e.Changes.Count >= 1, "In richTextBox there is atleast one change on every operation. Actual Count [" +
            e.Changes.Count.ToString() + "]", false);

            string rangeText = GetOffsetRangeText(e);
            //netsymbols added is calculated in getoffsetrangetext above
            VerifyTagsSplitMerge(_netSymbolsAdded);
            bool rangeIsValid = false;

            switch ((Operations)_currentOperation)
            {
                case Operations.TypeInsert:
                    foreach (char ch in s_stringSample)
                    {
                        if (rangeText.Contains(ch.ToString()))
                        {
                            rangeIsValid = true;
                            break;
                        }
                    }
                    if (!rangeIsValid)
                    {
                        Verifier.Verify(false, "The min max range obtained through TextChange offsets didn't contain the " +
                            "inserted character", false);
                    }
                    break;

                case Operations.Delete:
                    Verifier.Verify(!rangeText.Contains(s_initialText.Substring(0, s_selectedTextLength)), "The  min max range obtained" +
                        "through TextChange offsets should not contain deleted text", false);
                    break;

                case Operations.Paste:
                    Verifier.Verify(rangeText.Contains(s_pasteString), "The  min max range obtained" +
                        "through TextChange offsets should contain pasted text", false);
                    break;

                case Operations.Overwrite:
                    Verifier.Verify(rangeText.Contains("Z"), "The  min max range obtained" +
                        "through TextChange offsets should contain overwritten text", false);
                    break;

                case Operations.Speller:
                    Verifier.Verify(rangeText.Contains(s_stringSample), "The  min max range obtained" +
                        "through TextChange offsets should contain corrected text", false);
                    break;

                case Operations.ChangeBlock:
                    Verifier.Verify(rangeText.Contains(s_pasteString+s_pasteString), "The  min max range obtained" +
                        "through TextChange offsets should contain pasted text done twice in a change block", false);
                    break;

                default:
                    break;
            }
        }

        private string GetOffsetRangeText(TextChangedEventArgs e)
        {
            //Here we are getting the minoffset and max offset for a change
            //min max is used because the changes need not be placed in order
            int minOffset = Int32.MaxValue;
            int maxOffset = Int32.MinValue;
            _netSymbolsAdded = 0;
            foreach (TextChange textChanges in e.Changes)
            {
                _netSymbolsAdded += textChanges.AddedLength;
                _netSymbolsAdded -= textChanges.RemovedLength;
                if (textChanges.Offset < minOffset)
                {
                    minOffset = textChanges.Offset;
                }
                if (textChanges.Offset + textChanges.AddedLength > maxOffset)
                {
                    maxOffset = textChanges.Offset + textChanges.AddedLength;
                }
            }
            TextPointer changeStartPointer = _richTextBox.Document.ContentStart.GetPositionAtOffset(minOffset);
            TextPointer changeEndPointer = _richTextBox.Document.ContentStart.GetPositionAtOffset(maxOffset);
            string rangeText = new TextRange(changeStartPointer, changeEndPointer).Text;
            return rangeText;
        }

        private void VerifyTagsSplitMerge(int netSymbolsAdded)
        {
            int currentTagTextCount = CountTagsAndText();
            Verifier.Verify(netSymbolsAdded == (currentTagTextCount - _intialTagTextCount), "Expected Tag Count [" +
                (currentTagTextCount - _intialTagTextCount).ToString() + "] Actual Tag Count [" + netSymbolsAdded.ToString() + "]", true);
            _intialTagTextCount = currentTagTextCount;
        }

        private int CountTagsAndText()
        {
            //Here we count the number of opening and closing tags including the # of text characters
            TextRange range = new TextRange(_richTextBox.Document.ContentStart, _richTextBox.Document.ContentEnd);
            string xaml = XamlUtils.TextRange_GetXml(range);
            int count = FindInstanceCount(xaml, "<");
            count += range.Text.Length;
            return count;
        }

        private int FindInstanceCount(string xaml, string searchString)
        {
            int count=0;
            for (int index = 0; index >= 0; index = xaml.IndexOf(searchString, index+1))
            {
                count++;
            }
            return count;
        }

        private void TriggerCountVerifier(int expectedTriggerCount)
        {
            Verifier.Verify(_triggerCount == expectedTriggerCount, "Event should be triggered [" +
              expectedTriggerCount + "] time(s) Actual [" + _triggerCount + "]", false);
        }

        #endregion

        #region Private Data

        enum Operations
        {
            TypeInsert=0,
            Delete,
            Paste,
            Overwrite,
            Speller,
            ChangeBlock
        };

        private int _intialTagTextCount = 0;
        private int _currentOperation = 0;
        private RichTextBox _richTextBox;
        private int _triggerCount = 0;
        private int _netSymbolsAdded = 0;
        
        static readonly string s_stringSample = "ABCD";
        static readonly string s_initialText = "INI1 TEXT1";
        static readonly string s_pasteString = "MSFT";
        static readonly int s_selectedTextLength = 2;

        #endregion
    }
}