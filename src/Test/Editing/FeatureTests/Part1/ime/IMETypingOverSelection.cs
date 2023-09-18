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
    /// This test adds regression coverage for TFS Part1 Regression_Bug356. In particular when typing over non-empty 
    /// selection with IME, certain TextElements like LineBreaks created a char count mismatch with IME.
    /// </summary>
    [Test(0, "IME", "IMETypingOverSelection_Korean", MethodParameters = "/TestCaseType:IMETypingOverSelection /locale=Korean", Timeout = 120, Keywords = "KoreanIME")]
    [Test(0, "IME", "IMETypingOverSelection_Japanese", MethodParameters = "/TestCaseType:IMETypingOverSelection /locale=Japanese", Timeout = 120, Keywords = "JapaneseIME")]
    [Test(0, "IME", "IMETypingOverSelection_ChinesePinyin", MethodParameters = "/TestCaseType:IMETypingOverSelection /locale=ChinesePinyin", Timeout = 120, Keywords = "ChinesePinyinIME")]    
    public class IMETypingOverSelection : ManagedCombinatorialTestCase
    {
        #region Main flow

        /// <summary>Starts the combination</summary>
        protected override void DoRunCombination()
        {
            if (_rtb == null)
            {
                _rtb = new RichTextBox();
                _rtb.Height = 200;
                _rtb.FontSize = 24;
            }
            if (_testTextBox == null)
            {
                _testTextBox = new TextBox();
                _testTextBox.Height = 100;
                _testTextBox.FontSize = 24;
            }

            if (_panel == null)
            {
                _panel = new StackPanel();
                _panel.Children.Add(_rtb);
                _panel.Children.Add(_testTextBox);
                MainWindow.Content = _panel;
            }

            InitializeTestVariables();

            QueueDelegate(PerformTestSetup);
        }

        private void PerformTestSetup()
        {            
            _rtb.Document.Blocks.Clear();
            _testTextBox.Text = string.Empty;

            TextRange tr = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);
            XamlUtils.SetXamlContent(tr, _xamlContent[_xamlContentIndex]);

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
            
            // Put the focus in the actual control where test is done
            _rtb.SelectAll();
            _rtb.Focus();
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();

            KeyboardInput.TypeString(_contentToTypeInIME);
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEHelper.CiceroWaitTimeMs);            

            VerifyContentAfterTyping();

            // Loop through all xamlContent values
            _xamlContentIndex++;
            if (_xamlContentIndex < _xamlContent.Length)
            {
                PerformTestSetup();
            }
            else
            {                
                NextCombination();
            }            
        }

        private void VerifyContentAfterTyping()
        {
            Paragraph p = (Paragraph)_rtb.Document.Blocks.FirstBlock;
            TextRange tr = new TextRange(p.ContentStart, p.ContentEnd);

            Verifier.Verify(tr.Text == _composedStringByIME, "Verifying contents after typing: Actual[" +
                tr.Text + "] Expected[" + _composedStringByIME + "]", true);
        }

        private void InitializeTestVariables()
        {
            _xamlContentIndex = 0;

            switch (_locale)
            {
                case IMELocales.Korean:
                    _contentToTypeInIME = koreanTypeSequence;
                    _composedStringByIME = koreanCompositedString;
                    break;
                case IMELocales.Japanese:
                    _contentToTypeInIME = japaneseTypeSequence;
                    _composedStringByIME = japaneseCompositedString;
                    break;
                case IMELocales.ChinesePinyin:
                    _contentToTypeInIME = chinesePinyinTypeSequence;
                    _composedStringByIME = chinesePinyinCompositedString;
                    break;                
            }                        
        }        

        #endregion

        #region Private fields

        // Combinatorial engine variables; set to default values
        private IMELocales _locale = IMELocales.Korean;        

        private StackPanel _panel;
        private RichTextBox _rtb;
        private TextBox _testTextBox; // Used just to set the appropriate Ime mode
        private bool _isIMESetupDone = false;

        private int _xamlContentIndex = 0;
        private string[] _xamlContent = new string[]{"<Paragraph><Run>ab</Run><LineBreak/><Run>cd</Run></Paragraph>"};

        private string _contentToTypeInIME = string.Empty;
        private string _composedStringByIME = string.Empty;

        private const string koreanTypeSequence = "qixms";
        private const string koreanCompositedString = "뱌튼";

        private const string japaneseTypeSequence = "ae{ENTER}";
        private const string japaneseCompositedString = "あえ";

        private const string chinesePinyinTypeSequence = "nihao{SPACE}{ENTER}";
        private const string chinesePinyinCompositedString = "你好";        

        #endregion
    }
}