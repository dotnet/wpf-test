// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Verfies typing over selection in both RichTextBox(merge runs,paragraphs)
//  and TextBox.

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
    [Test(1, "IME", "IMETextSelectionTest_Korean", MethodParameters = "/TestCaseType:IMETextSelectionTest /locale=Korean", Timeout = 120, Keywords = "KoreanIME")]
    [Test(1, "IME", "IMETextSelectionTest_Japanese", MethodParameters = "/TestCaseType:IMETextSelectionTest /locale=Japanese", Timeout = 120, Keywords = "JapaneseIME")]
    [Test(1, "IME", "IMETextSelectionTest_ChinesePinyin", MethodParameters = "/TestCaseType:IMETextSelectionTest /locale=ChinesePinyin", Timeout = 120, Keywords = "ChinesePinyinIME")]
    [Test(1, "IME", "IMETextSelectionTest_ChineseQuanPin", MethodParameters = "/TestCaseType:IMETextSelectionTest /locale=ChineseQuanPin", Timeout = 120, Keywords = "ChineseQuanPinIME")]
    [Test(1, "IME", "IMETextSelectionTest_ChineseNewPhonetic", MethodParameters = "/TestCaseType:IMETextSelectionTest /locale=ChineseNewPhonetic", Timeout = 120, Keywords = "ChineseNewPhoneticIME")]
    public class IMETextSelectionTest : ManagedCombinatorialTestCase
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
            QueueDelegate(SetTestVariables);
            QueueDelegate(BeginTests);
        }


        private void BeginTests()
        {
            // Set the ime keyboard
            Log("Load IME keyboard");
            _testTextBox.Focus();
            IMEHelper.SetUpIMEKeyboardLayout(_locale, _testTextBox, MainWindow);

            // Perform tests in Rich TextBox            
            RichTextBoxVariations();
            // Perform tests in TextBox
            TextBoxVariations();
            NextCombination();            
        }

        private void TextBoxVariations()
        {
            // Add Content in textbox
            AddContent(false);
            // Select all "Hello123" -> "??"
            PerformTestActionAndVerifyResult(0, _tb.Text.Length, _composedImeString);
            // Select partial content "Hello123" -> "??23"
            PerformTestActionAndVerifyResult(0, 5, _composedImeString+"123");
            // Select partial content "Hello123" -> "Hello??"
            PerformTestActionAndVerifyResult(5, 3, "Hello" + _composedImeString);
            // Select partial content "Hello123" -> "He??123"
            PerformTestActionAndVerifyResult(2, 3, "He" + _composedImeString + "123");
        }

        private void RichTextBoxVariations()
        {
            // Add content in the test window
            AddContent(true);

            // Typing over - Select All "<p>run11run12run13</p><p>run21run22run23</p><p>run31run32run33</p>" ------> "<p>?????<p>"
            PerformTestActionAndVerifyResult(_r1.ContentStart.GetPositionAtOffset(0), _r9.ContentEnd.GetPositionAtOffset(0), _composedImeString);

            // Merge 2 runs in same para1  "<p>run11run12run13</p>" -------> "<p>r????2run13</p>" - partial selection of content in the 2 runs
            PerformTestActionAndVerifyResult(_r1.ContentStart.GetPositionAtOffset(1), _r2.ContentEnd.GetPositionAtOffset(-1), ("r" + _composedImeString + "2run13run21run22run23run31run32run33"));

            // Merge 2 runs in same para1  "<p>run11run12run13</p>" -------> "<p>????run13</p>" - complete selction of content in the 2 runs
            PerformTestActionAndVerifyResult(_r1.ContentStart.GetPositionAtOffset(0), _r2.ContentEnd.GetPositionAtOffset(0), (_composedImeString + "run13run21run22run23run31run32run33"));

            // Typing over selection(selection complete content in one run)  "<p>run11run12run13</p>" -------> "<p>????run12run13</p>" 
            PerformTestActionAndVerifyResult(_r1.ContentStart.GetPositionAtOffset(0), _r1.ContentEnd.GetPositionAtOffset(0), (_composedImeString + "run12run13run21run22run23run31run32run33"));

            // Typing over selection(selection partial content in one run)  "<p>run11run12run13</p>" -------> "<p>r???1run12run13</p>" 
            PerformTestActionAndVerifyResult(_r1.ContentStart.GetPositionAtOffset(1), _r1.ContentEnd.GetPositionAtOffset(-1), ("r" + _composedImeString + "1run12run13run21run22run23run31run32run33"));

            // Typing over selection(selection partial content in one run)  "<p>run11run12run13</p>" -------> "<p>r???run12run13</p>" 
            PerformTestActionAndVerifyResult(_r1.ContentStart.GetPositionAtOffset(1), _r1.ContentEnd.GetPositionAtOffset(0), ("r" + _composedImeString + "run12run13run21run22run23run31run32run33"));

            // Typing over selection(selection partial content in one run)  "<p>run11run12run13</p>" -------> "<p>???1run12run13</p>" 
            PerformTestActionAndVerifyResult(_r1.ContentStart.GetPositionAtOffset(0), _r1.ContentEnd.GetPositionAtOffset(-1), (_composedImeString + "1run12run13run21run22run23run31run32run33"));

            // Merge 3 runs in same para1 "<p>run11run12run13</p>" -------> "<p>r????3</p>"
            PerformTestActionAndVerifyResult(_r1.ContentStart.GetPositionAtOffset(1), _r3.ContentEnd.GetPositionAtOffset(-1), ("r" + _composedImeString + "3run21run22run23run31run32run33"));

            // Merge 3 runs in same para1 "<p>run11run12run13</p>" -------> "<p>run1????un13</p>"
            PerformTestActionAndVerifyResult(_r1.ContentStart.GetPositionAtOffset(4), _r3.ContentStart.GetPositionAtOffset(1), ("run1" + _composedImeString + "un13run21run22run23run31run32run33"));

            // Merge 2 paragraphs - Complete Selection of para1,para2 "<p>run11run12run13</p><p>run21run22run23</p>" -------> "<p>????</p>"
            PerformTestActionAndVerifyResult(_r1.ContentStart.GetPositionAtOffset(0), _r6.ContentEnd.GetPositionAtOffset(0), (_composedImeString + "run31run32run33"));

            // Merge 2 paragraphs - partial selection of para1,para2 "<p>run11run12run13</p><p>run21run22run23</p>" -------> "<p>run11run12run1???un21run22run23</p>"
            PerformTestActionAndVerifyResult(_r3.ContentEnd.GetPositionAtOffset(-1), _r4.ContentStart.GetPositionAtOffset(1), ("run11run12run1" + _composedImeString + "un21run22run23run31run32run33"));

            // Merge 2 paragraphs - complete selection of para2,para3 "<p>run21run22run23</p><p>run31run32run33</p>" -------> "<p>?????</p>"
            PerformTestActionAndVerifyResult(_r4.ContentStart.GetPositionAtOffset(0), _r9.ContentEnd.GetPositionAtOffset(0), ("run11run12run13" + _composedImeString));

            // Merge 2 paragraphs - partial selection of para2,para3  "<p>run21run22run23</p><p>run31run32run33</p>" -------> "<p>run21run22????run32run33</p>"
            PerformTestActionAndVerifyResult(_r6.ContentStart.GetPositionAtOffset(0), _r7.ContentEnd.GetPositionAtOffset(0), ("run11run12run13run21run22" + _composedImeString + "run32run33"));

            // Merge 3 paragraphs - partial selection of para1,para2,para3  "<p>run21run22run23</p><p>run31run32run33</p>" -------> "<p>run21run22????run32run33</p>"
            PerformTestActionAndVerifyResult(_r3.ContentStart.GetPositionAtOffset(0), _r7.ContentEnd.GetPositionAtOffset(0), ("run11run12" + _composedImeString + "run32run33"));
        }

        private void PerformTestActionAndVerifyResult(int selectionStartIndex, int selectionLength, string expectedContent)
        {
            // Make selection
            _tb.Select(selectionStartIndex, selectionLength);
            // Type content
            _tb.Focus();
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();
            KeyboardInput.TypeString(_contentToTypeInIME);
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEHelper.CiceroWaitTimeMs);
            // Verify content
            Verifier.Verify((_tb.Text).Equals(expectedContent), "Verifying text in run: Actual[" + _tb.Text + "] Expected[" + expectedContent + "]", true);
            // cleanup
            AddContent(false);
        }

        private void PerformTestActionAndVerifyResult(TextPointer startSelection, TextPointer endSelection, string expectedContent)
        {
            string actualContent;
            
            // Make Selection
            _rtb.Selection.Select(startSelection, endSelection);
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();
            // Type using IME keyboard over the Selection
            _rtb.Focus();
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();
            KeyboardInput.TypeString(_contentToTypeInIME);
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEHelper.CiceroWaitTimeMs);
            // Verify Paragraph content
            actualContent = ContentInRichTextBox();
            Verifier.Verify(actualContent.Equals(expectedContent), "Verifying text in run: Actual[" + actualContent + "] Expected[" + expectedContent + "]", true);
            
            _rtb.Document.Blocks.Clear();
            // Add text to richtextbox
            AddContent(true);
           
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
       
        private void AddContent(bool richTextContent)
        {
            if (richTextContent)
            {
                _rtb.Document.Blocks.Clear();

                _p1 = new Paragraph();
                _p1.Language = XmlLanguage.GetLanguage(_language);
                _r1 = new Run("run11");
                _r2 = new Run("run12");
                _r3 = new Run("run13");

                _p2 = new Paragraph();
                _p2.Language = XmlLanguage.GetLanguage(_language);
                _r4 = new Run("run21");
                _r5 = new Run("run22");
                _r6 = new Run("run23");

                _p3 = new Paragraph();
                _p3.Language = XmlLanguage.GetLanguage(_language);
                _r7 = new Run("run31");
                _r8 = new Run("run32");
                _r9 = new Run("run33");

                _p1.Inlines.Add(_r1);
                _p1.Inlines.Add(_r2);
                _p1.Inlines.Add(_r3);

                _p2.Inlines.Add(_r4);
                _p2.Inlines.Add(_r5);
                _p2.Inlines.Add(_r6);

                _p3.Inlines.Add(_r7);
                _p3.Inlines.Add(_r8);
                _p3.Inlines.Add(_r9);

                _rtb.Document.Blocks.Add(_p1);
                _rtb.Document.Blocks.Add(_p2);
                _rtb.Document.Blocks.Add(_p3);
            }
            else
            {
                _tb.Text = "Hello123";
            }
        }

        private void SetTestVariables()
        {
            _ver = Environment.OSVersion.Version;
            switch(_locale)
            {
                case IMELocales.Korean:
                    _contentToTypeInIME = "gksrmf{RIGHT}";
                    _composedImeString = "한글";
                    _language="ko-kr";
                    break;
                case IMELocales.Japanese:
                    _contentToTypeInIME = "hiragana{ENTER}";
                    _composedImeString = "ひらがな";
                    _language = "ja-jp";
                    break;
                case IMELocales.ChinesePinyin:
                    if (_ver.Major > 6 || ((6 == _ver.Major) && _ver.Minor > 1))
                    {
                        _contentToTypeInIME = "nihao{SPACE}";
                    }
                    else
                    {
                        _contentToTypeInIME = "nihao{SPACE}{SPACE}";
                    }
                    _composedImeString = "你好";
                    _language = "zh-CN";
                    break;
                case IMELocales.ChineseQuanPin:
                    if (_ver.Major > 6 || ((6 == _ver.Major) && _ver.Minor > 1))
                    {
                        _contentToTypeInIME = "nihao{SPACE}";
                    }
                    else
                    {
                        _contentToTypeInIME = "nihao{ENTER}";
                    }
                    _composedImeString = "你好";
                    _language = "zh-CN";
                    break;
                case IMELocales.ChineseNewPhonetic:
                    _contentToTypeInIME = "su3cl3a87{ENTER}";
                    _composedImeString = "你好嗎";
                    _language = "zh-TW";
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
        Paragraph _p1,_p2,_p3;
        Run _r1,_r2,_r3,_r4,_r5,_r6,_r7,_r8,_r9;
        private string _contentToTypeInIME = string.Empty;
        private string _composedImeString = string.Empty;
        private string _language = string.Empty;

        Version _ver;

        #endregion Private fields
    }
}