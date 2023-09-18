// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Validates IME input in combination with editing opeations in RichTextBox
//  with BUIC and IUIC

using System.Windows.Controls;
using System;
using System.Windows;

using Microsoft.Test;
using Microsoft.Test.Discovery;

using Test.Uis.Data;
using Test.Uis.Loggers;
using Test.Uis.TestTypes;
using Test.Uis.Utils;
using System.Windows.Markup;
using System.Windows.Documents;
using System.Windows.Input;
using UisWin32 = Test.Uis.Wrappers.Win32;

namespace Test.Uis.TextEditing
{
    [Test(1, "IME", "IMEInputOverMixedContent_Korean", MethodParameters = "/TestCaseType:IMEInputOverMixedContent /locale=Korean",  Keywords = "KoreanIME")]
    [Test(1, "IME", "IMEInputOverMixedContent_Japanese", MethodParameters = "/TestCaseType:IMEInputOverMixedContent /locale=Japanese",  Keywords = "JapaneseIME")]
    [Test(1, "IME", "IMEInputOverMixedContent_ChinesePinyin", MethodParameters = "/TestCaseType:IMEInputOverMixedContent /locale=ChinesePinyin",  Keywords = "ChinesePinyinIME")]
    [Test(1, "IME", "IMEInputOverMixedContent_ChineseQuanPin", MethodParameters = "/TestCaseType:IMEInputOverMixedContent /locale=ChineseQuanPin", Keywords = "ChineseQuanPinIME")]
    [Test(1, "IME", "IMEInputOverMixedContent_ChineseNewPhonetic", MethodParameters = "/TestCaseType:IMEInputOverMixedContent /locale=ChineseNewPhonetic", Timeout = 120, Keywords = "ChineseNewPhoneticIME")]
    public class IMEInputOverMixedContent : ManagedCombinatorialTestCase
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
            if (_tb == null)
            {
                _tb = new TextBox();
                _tb.Height = 100;
                _tb.FontSize = 24;
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
                _panel.Children.Add(_tb);
                _panel.Children.Add(_testTextBox);
                MainWindow.Content = _panel;
            }
            SetTestVariables();
            QueueDelegate(PerformTestActions);
        }

        private void PerformTestActions()
        {            
            // Set the ime keyboard
            Log("Load IME keyboard");
            _testTextBox.Focus();
            IMEHelper.SetUpIMEKeyboardLayout(_locale, _testTextBox, MainWindow);

            for(int count=0;count<_contentTypes.Length;count++)
            {
                _contentType=_contentTypes[count];
                // Verify normal typing
                SendInputAndVerifyTyping(false);
                // Verify typing over selection
                SendInputAndVerifyTyping(true);
            }                        
            NextCombination();
        }
       
        private void SendInputAndVerifyTyping(bool verifyOverTyping)
        {
            string actualContent, expectedContent;
            
            for (int count = 0; count < 2; count++)
            {
                // Add Content
                AddMixedContent();

                _rtb.Focus();
                Microsoft.Test.Threading.DispatcherHelper.DoEvents();
                // Perform test action
                if (verifyOverTyping)
                {
                    // Make selection
                    if (count == 1)
                    {
                        expectedContent = "B" + _composedImeString + "fter";
                        _rtb.Selection.Select(_r1.ContentStart.GetPositionAtOffset(1), _r2.ContentStart.GetPositionAtOffset(1));
                    }
                    else
                    {
                        expectedContent = _composedImeString;
                        _rtb.SelectAll();
                    }
                }
                else
                {
                    expectedContent = "Before" + _composedImeString + "After";
                    // Move caret 
                    KeyboardInput.TypeString(_typingVariations[count]);
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(2000);
                }
                // Type content
                KeyboardInput.TypeString(_contentToTypeInIME);
                Microsoft.Test.Threading.DispatcherHelper.DoEvents(2000);
                // Verify Content
                actualContent = ContentInRichTextBox();
                Verifier.Verify(actualContent.Equals(expectedContent), "Verifying text : Actual[" + actualContent + "] Expected[" + expectedContent + "]", true);
                // Verify we do not crash with redo and undo
                _rtb.Undo();
                _rtb.Redo();
            }
        }
        
        private void AddMixedContent()
        {
            _rtb.Document.Blocks.Clear();
            _rtb.Document.IsEnabled = true;

            _containerTextbox = new TextBox();

            _r1 = new Run("Before");
            _r2 = new Run("After");

            _p1 = new Paragraph();
            _p1.Inlines.Add(_r1);

            switch (_contentType)
            {
                case "IUIC":
                    _iuic = new InlineUIContainer(_containerTextbox);
                    _p1.Inlines.Add(_iuic);
                    _p1.Inlines.Add(_r2);
                    _rtb.Document.Blocks.Add(_p1);
                    _typingVariations = new string[] { "{HOME}{RIGHT 6}", "{END}{LEFT 5}" };
                    break;
                case "BUIC": 
                    _p2 = new Paragraph();
                    _p2.Inlines.Add(_r2);
                    _buic = new BlockUIContainer(_containerTextbox);
                    _rtb.Document.Blocks.Add(_p1);
                    _rtb.Document.Blocks.Add(_buic);
                    _rtb.Document.Blocks.Add(_p2);
                    _typingVariations = new string[] { "{HOME}{DOWN}", "{DOWN}{END}" };
                    break;
            }               
        }

        private string ContentInRichTextBox()
        {
            TextRange textRange;
            string documentContent;

            textRange = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);
            documentContent = (textRange.Text).Trim();
            documentContent = documentContent.Replace("\r", "");
            documentContent = documentContent.Replace("\n", "");
            documentContent = documentContent.Replace(" ", "");

            return documentContent.Trim();
        }   

        private void SetTestVariables()
        {
            switch (_locale)
            {
                case IMELocales.Korean:
                    _contentToTypeInIME = "gksrmf{RIGHT}";
                    _composedImeString = "한글";
                    break;
                case IMELocales.Japanese:
                    _contentToTypeInIME = "hiragana{ENTER}";
                    _composedImeString = "ひらがな";
                    break;
                case IMELocales.ChinesePinyin:
                    _contentToTypeInIME = "nihao{SPACE}{SPACE}";
                    _composedImeString = "你好";
                    break;
                case IMELocales.ChineseQuanPin:
                    _os = Environment.OSVersion;
                    _ver = _os.Version;
                    if (_ver.Major > 6 || ((6 == _ver.Major) && _ver.Minor > 1))
                    {
                        _contentToTypeInIME = "nihao{SPACE}";
                    }
                    else
                    {
                        _contentToTypeInIME = "nihao{ENTER}";                        
                    }
                    _composedImeString = "你好";
                    break;
                case IMELocales.ChineseNewPhonetic:
                    _contentToTypeInIME = "su3cl3{ENTER}";
                    _composedImeString = "你好";
                    break;
            }
        }

        #endregion Main flow

        #region Private fields

        // Combinatorial engine variables; set to default values
        private IMELocales _locale = IMELocales.Korean;

        private StackPanel _panel = null;
        private RichTextBox _rtb = null;
        private TextBox _tb = null;
        private TextBox _testTextBox = null; // Used just to set the appropriate IME mode
        Paragraph _p1,_p2;
        Run _r1,_r2; 
        TextBox _containerTextbox;
        BlockUIContainer _buic;
        InlineUIContainer _iuic;
        private string _contentToTypeInIME = string.Empty;
        private string _composedImeString = string.Empty;
        private readonly string[] _contentTypes = new string[] { "IUIC", "BUIC" };
        private string[] _typingVariations;
        private string _contentType = null;

        OperatingSystem _os;
        Version _ver;
        
        #endregion Private fields
    }
}