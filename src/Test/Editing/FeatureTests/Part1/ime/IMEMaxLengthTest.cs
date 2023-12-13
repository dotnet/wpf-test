// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Validates IME input in TextBox in conjuction with MaxLength constraint

using System.Windows.Controls;

using System;
using System.Windows;

using Microsoft.Test;
using Microsoft.Test.Discovery;

using Test.Uis.Data;
using Test.Uis.Loggers;
using Test.Uis.TestTypes;
using Test.Uis.Utils;

namespace Test.Uis.TextEditing
{
    [Test(0, "IME", "IMEMaxLengthTest_Korean", MethodParameters = "/TestCaseType:IMEMaxLengthTest /locale=Korean", Timeout = 120, Keywords = "KoreanIME")]
    [Test(2, "IME", "IMEMaxLengthTest_Japanese", MethodParameters = "/TestCaseType:IMEMaxLengthTest /locale=Japanese", Timeout = 120, Keywords = "JapaneseIME")]
    [Test(0, "IME", "IMEMaxLengthTest_ChinesePinyin", MethodParameters = "/TestCaseType:IMEMaxLengthTest /locale=ChinesePinyin", Timeout = 120, Keywords = "ChinesePinyinIME")]
    [Test(1, "IME", "IMEMaxLengthTest_ChineseQuanPin", MethodParameters = "/TestCaseType:IMEMaxLengthTest /locale=ChineseQuanPin", Timeout = 120, Keywords = "ChineseQuanPinIME")]    
    public class IMEMaxLengthTest : ManagedCombinatorialTestCase
    {
        #region Main flow

        /// <summary>Starts the combination</summary>
        protected override void DoRunCombination()
        {                        
            // Filter out the uninteresting combinations
            if ( ((_maxLength == 0)&&(_existingContentLength == 0))||
                ((_maxLength == 1)&&(_existingContentLength == 0))||
                ((_maxLength == 1)&&(_existingContentLength == 1))||
                ((_maxLength == 4)&&(_existingContentLength == 0))||
                ((_maxLength == 4)&&(_existingContentLength == 4))||
                ((_maxLength == 4)&&(_existingContentLength == 5)) )
            {                
                if (_textBox == null)
                {
                    _textBox = new TextBox();
                    _textBox.Height = 200;                
                    _textBox.FontSize = 24;                
                }
                if (_testTextBox == null)
                {            
                    _testTextBox = new TextBox();
                    _testTextBox.Height = 100;
                    _testTextBox.FontSize = 24;                
                }
                                                
                if(_panel == null)
                {
                    _panel = new StackPanel();            
                    _panel.Children.Add(_textBox);
                    _panel.Children.Add(_testTextBox);
                    MainWindow.Content = _panel;
                }
                                                                            
                SetTestVariables();
                            
                QueueDelegate(PerformTestActions);
            }
            else
            {
                Log("Skipping this combination as it is not interesting");
                NextCombination();                                
            }
        }

        private void PerformTestActions()
        {
            if (!_isIMESetupDone)
            {
                Log("Load IME keyboard");
                IMEHelper.SetUpIMEKeyboardLayout(_locale, _testTextBox, MainWindow);
                _isIMESetupDone = true;
            }            

            // Put the caret at the end of the contents
            _textBox.Select(_textBox.Text.Length, 0);
            // Put the focus in the actual TextBox where test is done                        
            _textBox.Focus();
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();

            KeyboardInput.TypeString(_contentToTypeInIME);
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEHelper.CiceroWaitTimeMs);

            VerifyContentAfterTyping();

            NextCombination();
        }

        private void SetTestVariables()
        {        
            GlobalStringData koreanStringData = new GlobalStringData("Korean");
            GlobalStringData japaneseStringData = new GlobalStringData("Japanese");
            GlobalStringData chinesepinyinStringData = new GlobalStringData("ChinesePinyin");
            GlobalStringData chinesequanpinStringData = new GlobalStringData("ChineseQuanPin");

            _ver = Environment.OSVersion.Version;

            switch(_locale)
            {
                case IMELocales.Korean:
                    _contentToTypeInIME = koreanStringData.StringTypeSequence;
                    _composedStringByIME = koreanStringData.CompositedString;
                    break;
                case IMELocales.Japanese:
                    _contentToTypeInIME = japaneseStringData.StringTypeSequence;
                    _composedStringByIME = japaneseStringData.CompositedString;
                    break;
                case IMELocales.ChinesePinyin:
                    //chinesePinyinTypeSequence = "nihao{SPACE}{SPACE}nihao{SPACE}{SPACE}nihao{SPACE}{SPACE}1234567890";
                    //chineseQuanPinTypeSequence = "nihao{SPACE}nihao{SPACE}nihao{SPACE}1234567890";
                    //The composited string is all "??????1234567890"
                    //The input by using chinese Pinyin SimpleFast is as same as chineseQuanPin
                    if (_ver.Major > 6 || ((6 == _ver.Major) && _ver.Minor > 1))
                    {
                        _contentToTypeInIME = chinesequanpinStringData.StringTypeSequence;
                    }
                    else
                    {
                        _contentToTypeInIME = chinesepinyinStringData.StringTypeSequence;
                    }
                    _composedStringByIME = chinesepinyinStringData.CompositedString;
                    break;
                case IMELocales.ChineseQuanPin:
                    _contentToTypeInIME = chinesequanpinStringData.StringTypeSequence;
                    _composedStringByIME = chinesequanpinStringData.CompositedString;
                    break;
            }

            _textBox.Text = _testTextBox.Text = string.Empty; // Clean up from previous combinations
            _textBox.IsUndoEnabled = _isUndoEnabled;
            _textBox.MaxLength = _maxLength;
            _existingContent = string.Empty;
            if (_existingContentLength > 0)
            {
                _existingContent = _composedStringByIME.Substring(0, _existingContentLength);
                _textBox.Text = _existingContent;
            }
        }        

        private void VerifyContentAfterTyping()
        {
            string actualContent, expectedContent;
            actualContent = _textBox.Text;
            if (_maxLength == 0)
            {
                expectedContent = string.Concat(_existingContent, _composedStringByIME);                
            }
            else if (_maxLength < _existingContentLength)
            {                
                if ( (System.Environment.OSVersion.Version.Major < 6) && (_locale == IMELocales.Japanese) )
                {
                    // TFS Part1 
                    expectedContent = _existingContent.Substring(0, _maxLength);
                }                
                else
                {
                    expectedContent = _existingContent;
                }
            }
            else
            {
                expectedContent = string.Concat(_existingContent, _composedStringByIME.Substring(0, _maxLength - _existingContentLength));
            }

            Verifier.Verify(actualContent == expectedContent, "Verifying contents after typing: Actual[" + 
                actualContent + "] Expected[" + expectedContent + "]", true);            
        }

        #endregion

        #region Private fields

        // Combinatorial engine variables; set to default values
        private IMELocales _locale = IMELocales.Korean;
        private int _maxLength = 0;
        private int _existingContentLength = 0;
        private bool _isUndoEnabled = true;

        private StackPanel _panel = null;
        private TextBox _textBox = null;
        private TextBox _testTextBox = null; // Used just to set the appropriate IME mode
        private bool _isIMESetupDone = false;

        private string _contentToTypeInIME = string.Empty;
        private string _composedStringByIME = string.Empty;       
        private string _existingContent = string.Empty;

        Version _ver;

        #endregion
    }
}
