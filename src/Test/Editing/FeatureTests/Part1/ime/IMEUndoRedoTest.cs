// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Verifies undo and redo operations in RichTextBox and TextBox

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
using System.Windows.Controls.Primitives;
using UisWin32 = Test.Uis.Wrappers.Win32;

namespace Test.Uis.TextEditing
{
    [Test(0, "IME", "IMEUndoRedoTest_Korean", MethodParameters = "/TestCaseType:IMEUndoRedoTest /locale=Korean", Timeout = 120, Keywords = "KoreanIME")]
    [Test(0, "IME", "IMEUndoRedoTest_Japanese", MethodParameters = "/TestCaseType:IMEUndoRedoTest /locale=Japanese", Timeout = 120, Keywords = "JapaneseIME")]
    [Test(0, "IME", "IMEUndoRedoTest_ChinesePinyin", MethodParameters = "/TestCaseType:IMEUndoRedoTest /locale=ChinesePinyin", Timeout = 120, Keywords = "ChinesePinyinIME")]
    [Test(1, "IME", "IMEUndoRedoTest_ChineseQuanPin", MethodParameters = "/TestCaseType:IMEUndoRedoTest /locale=ChineseQuanPin", Timeout = 120, Keywords = "ChineseQuanPinIME")]
    [Test(1, "IME", "IMEUndoRedoTest_ChineseNewPhonetic", MethodParameters = "/TestCaseType:IMEUndoRedoTest /locale=ChineseNewPhonetic", Timeout = 120, Keywords = "ChineseNewPhoneticIME")]
    public class IMEUndoRedoTest : ManagedCombinatorialTestCase
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
            Log("Load IME keyboard");
            _testTextBox.Focus();
            IMEHelper.SetUpIMEKeyboardLayout(_locale, _testTextBox, MainWindow);

            for (int count = 0; count < _testCaseData.GetLength(0); count++)
            {
                VerifyRedoUndo(_testCaseData[count, 0], _testCaseData[count, 1], int.Parse(_testCaseData[count, 2]));
                _isRichTextBox = false;
                VerifyRedoUndo(_testCaseData[count, 0], _testCaseData[count, 1], int.Parse(_testCaseData[count, 2]));
                _isRichTextBox = true;
                PrepareTestSetup();
            }

            NextCombination();
        }

        private void VerifyRedoUndo(string contentToTypeInIME, string composedStringByIME, int expectedUndoRedoCount)
        {
            int undoCount = 0;
            int redoCount = 0;
            TextBoxBase controlToTest;

            if (_isRichTextBox)
            {
                controlToTest = _rtb;
            }
            else
            {
                controlToTest = _tb;
            }

            controlToTest.Focus();
            KeyboardInput.TypeString(contentToTypeInIME);
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEHelper.CiceroWaitTimeMs);
            VerifyResult(composedStringByIME, true, false);

            while (controlToTest.CanUndo)
            {
                controlToTest.Undo();
                undoCount++;
            }
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();
            VerifyResult("", false, true);

            // Verify we do not crash calling undo again 
            controlToTest.Undo();
            VerifyResult("", false, true);

            while (controlToTest.CanRedo)
            {
                controlToTest.Redo();
                redoCount++;
            }
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();
            VerifyResult(composedStringByIME, true, false);

            // Verify we do not write the contents again
            controlToTest.Redo();
            VerifyResult(composedStringByIME, true, false);

            // Verify redo and undo count are equal 
            Verifier.Verify(undoCount == redoCount, "Verifying that Undo count = " + undoCount + " equals Redo count = " + redoCount);
            // Verify redo and undo count equals expected count
            Verifier.Verify(undoCount == expectedUndoRedoCount, "Verifying that Undo count = " + undoCount + " equals expected count = " + expectedUndoRedoCount, true);
            
        }

        private void VerifyResult(string expectedString, bool canUndo, bool canRedo)
        {
            if (_isRichTextBox)
            {
                if (expectedString.Equals(""))
                {
                    int blockCount = 1, inlineCount = 1;
                    blockCount = _rtb.Document.Blocks.Count;
                    if (blockCount > 0)
                    {
                        inlineCount = ((Paragraph)(_rtb.Document.Blocks.FirstBlock)).Inlines.Count;
                        Verifier.Verify(inlineCount == 0, "Verifying Inline Count : " + inlineCount, true);
                    }
                    else
                    {
                        Verifier.Verify(blockCount == 0, "Verifying Block Count : " + blockCount, true);
                    }
                }
                else
                {
                    Paragraph p = (Paragraph)_rtb.Document.Blocks.FirstBlock;
                    TextRange tr = new TextRange(p.ContentStart, p.ContentEnd);
                    Verifier.Verify(tr.Text == expectedString, "Verifying contents of richtextbox after typing: Actual[" +
                       tr.Text + "] Expected[" + expectedString + "]", true);
                }
                Verifier.Verify(_rtb.CanUndo == canUndo, "Verifying that undo can performed: Actual[" +
                  _rtb.CanUndo + "] Expected[" + canUndo + "]", false);
                Verifier.Verify(_rtb.CanRedo == canRedo, "Verifying that redo can be performed: Actual[" +
                  _rtb.CanRedo + "] Expected[" + canRedo + "]", false);
            }
            else
            {
                if (expectedString.Equals(""))
                {
                        Verifier.Verify(_tb.Text.Equals(""), "Verifying that textbox content is empty", true);
                }
                else
                {
                   
                    Verifier.Verify(_tb.Text == expectedString, "Verifying contents of textbox after typing: Actual[" +
                       _tb.Text + "] Expected[" + expectedString + "]", true);
                }
                Verifier.Verify(_tb.CanUndo == canUndo, "Verifying that undo can performed: Actual[" +
                  _rtb.CanUndo + "] Expected[" + canUndo + "]", false);
                Verifier.Verify(_tb.CanRedo == canRedo, "Verifying that redo can be performed: Actual[" +
                  _rtb.CanRedo + "] Expected[" + canRedo + "]", false);
            }
        }

        private void PrepareTestSetup()
        {
            _panel.Children.Remove(_rtb);
            _panel.Children.Remove(_tb);
            _rtb = new RichTextBox();
            _rtb.Height = 200;
            _rtb.FontSize = 24;
            _tb = new TextBox();
            _tb.Height = 100;
            _tb.FontSize = 24;
            _panel.Children.Add(_rtb);
            _panel.Children.Add(_tb);
        }

        private void SetTestVariables()
        {
            _testCaseData = null;
            switch (_locale)
            {
                case IMELocales.Korean:
                    _testCaseData = new string[,]{
                                      {"gksrmf{RIGHT}","한글","2"}, // Undo after finalize
                                      {"gksrmf","한글","2"}, // undo on an ongoing composition
                                      {"gksrmf{RIGHT}gksrmf{RIGHT}","한글한글","4"} // undo after finalizing composition
                    };
                    break;
                case IMELocales.Japanese:
                    _testCaseData = new string[,]{ 
                                       {"hiragana{ENTER}","ひらがな","1"},
                                       {"hiragana","ひらがな","1"},
                                       {"hiragana{ENTER}hiragana{ENTER}","ひらがなひらがな","2"},
                                       {"hiraganahiragana","ひらがなひらがな","1"},
                                       {"hiragana{ENTER}hiragana","ひらがなひらがな","2"},
                                       {"higana{ENTER}{HOME}{RIGHT}ra","ひらがな","2"},
                                       {"higana{HOME}{RIGHT}ra","ひらがな","1"},
                                       {"higana{ENTER}hiragana{Enter}{HOME}{RIGHT}ra","ひらがなひらがな","3"},
                                       {"higanahiragana{Enter}{HOME}{RIGHT}ra","ひらがなひらがな","2"}
                                     };

                    break;
                case IMELocales.ChinesePinyin:
                    _ver = Environment.OSVersion.Version;
                    if (_ver.Major > 6 || ((6 == _ver.Major) && _ver.Minor > 1))
                    {
                       _testCaseData = new string[,]{ 
                                       {"nihao{SPACE}","你好","1"},
                                       {"nihao{SPACE}nihao{SPACE}","你好你好","2"},
                                       {"nihao{SPACE}{HOME}nihao{SPACE}","你好你好","2"},
                                       {"nihao{SPACE}{HOME}{RIGHT 1}nihao{SPACE}","你你好好","2"},
                                       {"nihao{SPACE}nihao{SPACE}","你好你好","2"},
                                       {"nihao{SPACE}nihao{SPACE}{HOME}nihao{SPACE}","你好你好你好","3"},
                                       {"nihao{SPACE}nihao{SPACE}{LEFT 1}nihao{SPACE}","你好你你好好","3"},
                                     };
                    }
                    else{
                       _testCaseData = new string[,]{ 
                                       {"nihao{SPACE}{SPACE}","你好","1"},
                                       {"nihao{SPACE}","你好","1"},
                                       {"nihao{SPACE}nihao{SPACE}","你好你好","1"},
                                       {"nihao{SPACE}{HOME}nihao{SPACE}","你好你好","1"},
                                       {"nihao{SPACE}{HOME}{RIGHT 1}nihao{SPACE}","你你好好","1"},
                                       {"nihao{SPACE}{SPACE}nihao{SPACE}","你好你好","2"},
                                       {"nihao{SPACE}{SPACE}nihao{SPACE}{HOME}nihao{SPACE}","你好你好你好","2"},
                                       {"nihao{SPACE}{SPACE}nihao{SPACE}{LEFT 1}nihao{SPACE}","你好你你好好","2"},
                                     };
                    }
                    break;
                case IMELocales.ChineseQuanPin:
                    _testCaseData = new string[,]{ 
                                       {"nihao{SPACE}","你好","1"},
                                       {"nihao{SPACE}nihao{SPACE}","你好你好","2"},
                                       {"nihao{SPACE}{HOME}nihao{SPACE}","你好你好","2"},
                                       {"nihao{SPACE}{HOME}{RIGHT 1}nihao{SPACE}","你你好好","2"},
                                       {"nihao{SPACE}nihao{SPACE}{HOME}nihao{SPACE}","你好你好你好","3"},
                                       {"nihao{SPACE}nihao{SPACE}{LEFT 1}nihao{SPACE}","你好你你好好","3"},
                                     };
                    break;
                case IMELocales.ChineseNewPhonetic:
                    _testCaseData = new string[,]{ 
                                       {"su3cl3{ENTER}","你好","1"},
                                       {"su3cl3","你好","1"},
                                       {"su3cl3{ENTER}su3cl3{ENTER}","你好你好","2"},
                                       {"su3cl3su3cl3","你好你好","1"},
                                       {"su3cl3{ENTER}su3cl3","你好你好","2"},
                                       {"su3cl3{HOME}su3cl3","你好你好","1"},
                                       {"su3cl3{HOME}{RIGHT 1}su3cl3","你你好好","1"},
                                       {"su3cl3{ENTER}su3cl3{ENTER}{HOME}su3cl3{ENTER}","你好你好你好","3"},
                                       {"su3cl3{ENTER}su3cl3{ENTER}{LEFT 1}su3cl3{ENTER}","你好你你好好","3"},
                                     };
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
        private string[,] _testCaseData = null;
        private bool _isRichTextBox = true;

        Version _ver;

        #endregion Private fields
    }
}