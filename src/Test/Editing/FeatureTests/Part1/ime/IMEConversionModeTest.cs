// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

using Microsoft.Test.Discovery;

using Test.Uis.Data;
using Test.Uis.Loggers;
using Test.Uis.TestTypes;
using Test.Uis.Utils;

namespace Test.Uis.TextEditing
{
    /// <summary>
    /// This test adds regression coverage for TFS Part1 Regression_Bug355. In particular its basically test coverage for 
    /// InputMethod.SetPreferredImeConversionMode() API. Input is tested against TextBoxes with different ImeConversionMode values.    
    /// </summary>
    [Test(0, "IME", "IMEConversionModeTest_Korean", MethodParameters = "/TestCaseType:IMEConversionModeTest /locale=Korean", Timeout = 120, Keywords = "KoreanIME")]
    [Test(0, "IME", "IMEConversionModeTest_Japanese", MethodParameters = "/TestCaseType:IMEConversionModeTest /locale=Japanese", Timeout = 120, Keywords = "JapaneseIME")]
    [Test(0, "IME", "IMEConversionModeTest_ChinesePinyin", MethodParameters = "/TestCaseType:IMEConversionModeTest /locale=ChinesePinyin", Timeout = 120, Keywords = "ChinesePinyinIME")]    
    public class IMEConversionModeTest : ManagedCombinatorialTestCase
    {
        #region Main flow

        /// <summary>Starts the combination</summary>
        protected override void DoRunCombination()
        {
            if (_textBoxes == null)
            {
                _textBoxes = new TextBox[5];

                for (int i = 0; i < _textBoxes.Length; i++)
                {
                    _textBoxes[i] = new TextBox();
                    _textBoxes[i].Height = 50;
                    _textBoxes[i].FontSize = 24;
                    InputMethod.SetPreferredImeState(_textBoxes[i], InputMethodState.On);
                }

                // Set the interesting ImeConversionMode values to each individual textbox
                InputMethod.SetPreferredImeConversionMode(_textBoxes[0], ImeConversionModeValues.Native | ImeConversionModeValues.FullShape);
                InputMethod.SetPreferredImeConversionMode(_textBoxes[1], ImeConversionModeValues.Native | ImeConversionModeValues.Katakana);
                InputMethod.SetPreferredImeConversionMode(_textBoxes[2], ImeConversionModeValues.Native | ImeConversionModeValues.Katakana | ImeConversionModeValues.FullShape);
                InputMethod.SetPreferredImeConversionMode(_textBoxes[3], ImeConversionModeValues.Alphanumeric);
                InputMethod.SetPreferredImeConversionMode(_textBoxes[4], ImeConversionModeValues.Alphanumeric | ImeConversionModeValues.FullShape);
            }

            if (_testTextBox == null)
            {
                _testTextBox = new TextBox();
                _testTextBox.Height = 50;
                _testTextBox.FontSize = 24;
            }            

            if (_panel == null)
            {
                _panel = new StackPanel();
                for (int i = 0; i < _textBoxes.Length; i++)
                {
                    _panel.Children.Add(_textBoxes[i]);
                }                
                _panel.Children.Add(_testTextBox);
                MainWindow.Content = _panel;
            }

            for (int i = 0; i < _textBoxes.Length; i++)
            {
                _textBoxes[i].Text = string.Empty;
            }
            _testTextBox.Text = string.Empty;
            _conversionModeIndex = 0;

            QueueDelegate(PerformTestActions);
        }

        private void PerformTestActions()
        {                        
            if (!_isIMESetupDone)
            {
                Log("Load IME keyboard");
                IMEHelper.SetUpIMEKeyboardLayout(_locale, _testTextBox, MainWindow);
                _isIMESetupDone = true;
            }

            SetInputData();

            _textBoxes[_conversionModeIndex].Focus();
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();

            KeyboardInput.TypeString(_contentToTypeInIME);
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEHelper.CiceroWaitTimeMs);            

            VerifyContentAfterTyping();

            // Loop through all textBoxes
            _conversionModeIndex++;
            if (_conversionModeIndex < _textBoxes.Length)
            {
                PerformTestActions();
            }
            else
            {
                NextCombination();
            }           
        }

        private void VerifyContentAfterTyping()
        {
            Verifier.Verify(_textBoxes[_conversionModeIndex].Text == _composedStringByIME, 
                "Verifying contents after typing in TextBox " + _conversionModeIndex + ": Actual[" +
                _textBoxes[_conversionModeIndex].Text + "] Expected[" + _composedStringByIME + "]", true);
        }

        private void SetInputData()
        {            
            switch (_locale)
            {
                case IMELocales.Korean:
                    _contentToTypeInIME = koreanTypeSequence;
                    _composedStringByIME = _koreanCompositedString[_conversionModeIndex];
                    break;
                case IMELocales.Japanese:
                    _contentToTypeInIME = japaneseTypeSequence;
                    _composedStringByIME = _japaneseCompositedString[_conversionModeIndex];
                    break;
                case IMELocales.ChinesePinyin:
                    _contentToTypeInIME = chinesePinyinTypeSequence;
                    _composedStringByIME = _chinesePinyinCompositedString[_conversionModeIndex];
                    break;                
            }                        
        }        

        #endregion

        #region Private fields

        // Combinatorial engine variables; set to default values
        private IMELocales _locale = IMELocales.Korean;        

        private StackPanel _panel = null;
        private TextBox[] _textBoxes = null;
        private TextBox _testTextBox = null; // Used just to set the appropriate Ime mode
        private bool _isIMESetupDone = false;
        private int _conversionModeIndex = 0;

        private string _contentToTypeInIME = string.Empty;
        private string _composedStringByIME = string.Empty;

        private const string koreanTypeSequence = "qixms ";
        private string[] _koreanCompositedString = new string[] { "뱌튼　", "뱌튼 ", "뱌튼　", "qixms ", "ｑｉｘｍｓ　" };

        private const string japaneseTypeSequence = "a{ENTER}";
        private string[] _japaneseCompositedString = new string[] { "あ", "ｱ", "ア", "a", "ａ" };

        private const string chinesePinyinTypeSequence = "nihao{SPACE}{ENTER}";
        private string[] _chinesePinyinCompositedString = new string[] { "你好", "你好", "你好", "nihao ", "ｎｉｈａｏ　" };        

        #endregion
    }
}