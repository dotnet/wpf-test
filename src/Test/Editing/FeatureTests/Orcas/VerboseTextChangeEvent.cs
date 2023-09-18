// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides test cases for TextBoxBase properties introduced in Orcas.

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


namespace Test.Uis.TextEditing
{
    /// <summary>
    /// Tests TextChange value passed in the TextChanged event for TextBox
    /// </summary>
    [Test(0, "TextBoxOM", "TextBoxVerboseTextChangedEvent", MethodParameters = "/TestCaseType=TextBoxVerboseTextChangedEvent", Keywords = "Localization_Suite")]
    public class TextBoxVerboseTextChangedEvent : CustomTestCase
    {
        #region Public Members

        /// <summary>
        /// Entry point to test case
        /// </summary>
        public override void RunTestCase()
        {
            _textBox = new TextBox();
            TestWindow.Content = _textBox;
            _textBox.Text = s_initialText;
            _textBox.SpellCheck.IsEnabled = true;

            _textBox.TextChanged += new TextChangedEventHandler(textBox_TextChanged);
            DispatcherHelper.DoEvents();

            while (_currentOperation < 6)
            {
                _triggerCount = 0;
                _textBox.Focus();

                switch ((Operations) _currentOperation)
                {
                    case Operations.TypeInsert:
                        Log("------- Inside TypeInsert -------");
                        Input.SendUnicodeString(s_stringSample, 1, 1000);
                        DispatcherHelper.DoEvents();
                        TriggerCountVerifier(s_stringSample.Length);
                        break;

                    case Operations.Delete:
                        Log("------- Inside Delete -------");
                        _textBox.Select(0, s_selectedTextLength);
                        _textBox.Cut();
                        TriggerCountVerifier(1);
                        break;

                    case Operations.Paste:
                        Log("------- Inside Paste -------");
                        Clipboard.SetData(DataFormats.Text, s_stringSample);
                        _textBox.Paste();
                        TriggerCountVerifier(1);
                        break;

                    case Operations.Overwrite:
                        Log("------- Inside Overwrite -------");
                        _textBox.Select(0, s_selectedTextLength);
                        Input.SendUnicodeString("Z");
                        DispatcherHelper.DoEvents();
                        TriggerCountVerifier(1);
                        break;

                    case Operations.Speller:
                        Log("------- Inside Speller -------");
                        _textBox.TextChanged -= new TextChangedEventHandler(textBox_TextChanged);
                        _textBox.Text = s_initialText;
                        _textBox.TextChanged += new TextChangedEventHandler(textBox_TextChanged);
                        if (_textBox.GetSpellingError(0) != null)
                        {
                            _textBox.GetSpellingError(0).Correct(s_stringSample);
                            TriggerCountVerifier(1);
                        }
                        break;

                    case Operations.ChangeBlock:
                        Log("------- Inside ChangeBlock -------");
                        Clipboard.SetData(DataFormats.Text, s_stringSample);
                        using (_textBox.DeclareChangeBlock())
                        {
                            _textBox.Paste();
                            _textBox.Paste();
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

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _triggerCount++;

            Verifier.Verify(e.Changes.Count == 1, "In TextBox there is one change on every operation. Actual Count [" +
            e.Changes.Count.ToString() + "]", false);

            IEnumerator enumerator = e.Changes.GetEnumerator();
            enumerator.MoveNext();
            TextChange textChange = enumerator.Current as TextChange;

            CaretPositionVerifier(textChange);

            switch ((Operations)_currentOperation)
            {
                case Operations.TypeInsert:
                    //one char added for every insert char operation
                    TextChangeVerifier(1, 0, textChange);
                    break;

                case Operations.Delete:
                    //no char added in delete
                    TextChangeVerifier(0, s_selectedTextLength, textChange);
                    break;

                case Operations.Paste:
                    //pasted string length added
                    TextChangeVerifier(s_stringSample.Length, 0, textChange);
                    break;

                case Operations.Overwrite:
                    //selected text is deleted
                    TextChangeVerifier(1, s_selectedTextLength, textChange);
                    break;

                case Operations.Speller:
                    //first word is replaced by new word (old and new words have same length)
                    TextChangeVerifier(s_stringSample.Length, s_stringSample.Length, textChange);
                    break;

                case Operations.ChangeBlock:
                    TextChangeVerifier(s_stringSample.Length * 2, 0, textChange);
                    break;

                default:
                    break;
            }
        }

        private void CaretPositionVerifier(TextChange textChange)
        {
            Verifier.Verify(_textBox.CaretIndex == (textChange.Offset + textChange.AddedLength), "Caret Index should be Equal to the Offset + Added TextLength" +
                "CaretIndex [" + _textBox.CaretIndex + "] Offset [" + textChange.Offset + "+" + textChange.AddedLength + "]", true);
        }

        private void TextChangeVerifier(int addCount, int removeCount, TextChange textChange)
        {
            Verifier.Verify(addCount == textChange.AddedLength, "Expected Added Character Count [" + addCount + 
                           "] Actual[" + textChange.AddedLength + "]", true);
            Verifier.Verify(removeCount == textChange.RemovedLength, "Expected Deleted Character Count [" + removeCount + 
                           "] Actual[" + textChange.RemovedLength + "]", true);
        }

        private void TriggerCountVerifier(int expectedTriggerCount)
        {
            Verifier.Verify(_triggerCount == expectedTriggerCount, "Event should be triggered [" +
              expectedTriggerCount + "] time(s) Actual [" + _triggerCount + "]", true);
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
        
        private int _currentOperation = 0;
        private TextBox _textBox;
        private int _triggerCount = 0;
        
        static readonly string s_stringSample = "TEXT";
        static readonly string s_initialText = "INI1 TEXT1";
        static readonly int s_selectedTextLength = 2;

        #endregion
    }
}