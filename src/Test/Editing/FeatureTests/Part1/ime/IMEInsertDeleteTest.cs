// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Validates that editing(insert,delete) opeartions on both finalized and
//  unfinalized input in a RichTextBox and TextBox

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
    [Test(0, "IME", "IMEInsertDeleteTest_Korean", MethodParameters = "/TestCaseType:IMEInsertDeleteTest /locale=Korean", Timeout = 120, Keywords = "KoreanIME")]
    [Test(0, "IME", "IMEInsertDeleteTest_Japanese", MethodParameters = "/TestCaseType:IMEInsertDeleteTest /locale=Japanese", Timeout = 120, Keywords = "JapaneseIME,MicroSuite")]
    [Test(2, "IME", "IMEInsertDeleteTest_ChinesePinyin", MethodParameters = "/TestCaseType:IMEInsertDeleteTest /locale=ChinesePinyin", Timeout = 120, Keywords = "ChinesePinyinIME")]
    [Test(1, "IME", "IMEInsertDeleteTest_ChineseNewPhonetic", MethodParameters = "/TestCaseType:IMEInsertDeleteTest /locale=ChineseNewPhonetic", Timeout = 120, Keywords = "ChineseNewPhoneticIME")]
    public class IMEInsertDeleteTest : ManagedCombinatorialTestCase
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
                _tb.Clear();
                Microsoft.Test.Threading.DispatcherHelper.DoEvents();
                _rtb.Document.Blocks.Clear();
                Microsoft.Test.Threading.DispatcherHelper.DoEvents();

                AddEventHandlers();
                SendInputAndVerifyResult(count);
                RemoveEventHandlers();                
            }

            NextCombination();         
        }

        private void SendInputAndVerifyResult(int index)
        {
            // RichTextBox            
            _rtb.Focus();
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();
            KeyboardInput.TypeString(_testCaseData[index, 0]);
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEHelper.CiceroWaitTimeMs);

            // Verify composed ime string and events
            Paragraph p = (Paragraph)_rtb.Document.Blocks.FirstBlock;
            TextRange tr = new TextRange(p.ContentStart, p.ContentEnd);
            // Verify content in the rich textbox
            Verifier.Verify(tr.Text == _testCaseData[index, 1], "Verifying contents of richtextbox : Actual[" +
               tr.Text + "] Expected[" + _testCaseData[index, 1] + "]", true);
            // Verify start, update and complete composition events
            Verifier.Verify(_startCompostionEventCountInRtb == int.Parse(_testCaseData[index, 2]), "Verifying start compostion event count in RTB: Actual[" +
           _startCompostionEventCountInRtb + "] Expected = " + _testCaseData[index, 2], false);
            Verifier.Verify(_updateCompostionEventCountInRtb > 0, "Verifying update compostion event count in RTB > 0, Actual = " + _updateCompostionEventCountInRtb, false);
            Verifier.Verify(_completeCompostionEventCountInRtb == int.Parse(_testCaseData[index, 3]), "Verifying complete compostion event count in RTB: Actual[" +
             _completeCompostionEventCountInRtb + "] Expected = " + _testCaseData[index, 3], false);           
            
            // TextBox
            _tb.Focus();
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();
            KeyboardInput.TypeString(_testCaseData[index, 0]);
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEHelper.CiceroWaitTimeMs);
            // Verify composed ime string and events
            Verifier.Verify(_tb.Text == _testCaseData[index, 1], "Verifying contents of TextBox : Actual[" +
              _tb.Text + "] Expected[" + _testCaseData[index, 1] + "]", true);
            // Verify start, update and complete composition events
            Verifier.Verify(_startCompostionEventCountInTb == int.Parse(_testCaseData[index, 2]), "Verifying start compostion event count in TB: Actual[" +
          _startCompostionEventCountInTb + "] Expected = " + _testCaseData[index, 2], false);
            Verifier.Verify(_updateCompostionEventCountInTb > 0, "Verifying update compostion event count in TB > 0, Actual = " + _updateCompostionEventCountInTb, false);
            Verifier.Verify(_completeCompostionEventCountInTb == int.Parse(_testCaseData[index, 3]), "Verifying complete compostion event count in TB: Actual[" +
             _completeCompostionEventCountInTb + "] Expected = " + _testCaseData[index, 3], false);           
        }        

        private void RemoveEventHandlers()
        {
            TextCompositionManager.RemovePreviewTextInputStartHandler(_rtb, RtbTextCompositionStartEventHandler);
            TextCompositionManager.RemovePreviewTextInputUpdateHandler(_rtb, RtbTextCompositionUpdateEventHandler);
            TextCompositionManager.RemovePreviewTextInputStartHandler(_tb, TbTextCompositionStartEventHandler);
            TextCompositionManager.RemovePreviewTextInputUpdateHandler(_tb, TbTextCompositionUpdateEventHandler);
            TextCompositionManager.RemovePreviewTextInputHandler(_rtb, RtbTextCompositionCompleteEventHandler);
            TextCompositionManager.RemovePreviewTextInputHandler(_tb, TbTextCompositionCompleteEventHandler);
        }

        private void AddEventHandlers()
        {
            TextCompositionManager.AddPreviewTextInputStartHandler(_rtb, RtbTextCompositionStartEventHandler);
            TextCompositionManager.AddPreviewTextInputUpdateHandler(_rtb, RtbTextCompositionUpdateEventHandler);
            TextCompositionManager.AddPreviewTextInputStartHandler(_tb, TbTextCompositionStartEventHandler);
            TextCompositionManager.AddPreviewTextInputUpdateHandler(_tb, TbTextCompositionUpdateEventHandler);
            TextCompositionManager.AddPreviewTextInputHandler(_rtb, RtbTextCompositionCompleteEventHandler);
            TextCompositionManager.AddPreviewTextInputHandler(_tb, TbTextCompositionCompleteEventHandler);
            _startCompostionEventCountInTb = 0;
            _updateCompostionEventCountInTb = 0;
            _startCompostionEventCountInRtb = 0;
            _updateCompostionEventCountInRtb = 0;
            _completeCompostionEventCountInRtb = 0;
            _completeCompostionEventCountInTb = 0;
        }

        void RtbTextCompositionStartEventHandler(object sender, TextCompositionEventArgs e)
        {
            _startCompostionEventCountInRtb++;
        }
        void RtbTextCompositionUpdateEventHandler(object sender, TextCompositionEventArgs e)
        {
            _updateCompostionEventCountInRtb++;
        }
        void RtbTextCompositionCompleteEventHandler(object sender, TextCompositionEventArgs e)
        {
            _completeCompostionEventCountInRtb++;
        }
        void TbTextCompositionStartEventHandler(object sender, TextCompositionEventArgs e)
        {
            _startCompostionEventCountInTb++;
        }
        void TbTextCompositionUpdateEventHandler(object sender, TextCompositionEventArgs e)
        {
            _updateCompostionEventCountInTb++;
        }
        void TbTextCompositionCompleteEventHandler(object sender, TextCompositionEventArgs e)
        {
            _completeCompostionEventCountInTb++;
        }

        private void SetTestVariables()
        {
            switch (_locale)
            {
                case IMELocales.Korean:
                    _testCaseData = new string[,]{ 
                                       {"gksrmf{RIGHT}","한글","2","2"}, // Verify Empty Insert
                                       {"gksrmf{HOME}rm","그한글","3","2"}, // Verify Insert at the begin
                                       {"gksrmf{LEFT}rm","한그글","3","2"}, // Verify INsert in the middle
                                       {"gksrmf{BACKSPACE}{RIGHT}","한그","2","2"}, // Verify backspace delete and finalize
                                     };
                    break;
                case IMELocales.Japanese:
                    _testCaseData = new string[,]{ 
                                       {"hiragana{ENTER}","ひらがな","1","1"}, // Verify Empty Insert
                                       {"hiragana{HOME}hiragana","ひらがなひらがな","1","0"}, // Insert at the beginning of an ongoing compostion
                                       {"hiragana{LEFT}hiragana","ひらがひらがなな","1","0"}, // Insert in the middle of an ongoing compostion
                                       {"hiragana{HOME}{RIGHT 4}hiragana","ひらがなひらがな","1","0"},// Insert at the end of an ongoing compostion
                                       {"hiragana{HOME}{END}hiragana","ひらがなひらがな","1","0"},// Insert at the end of an ongoing compostion
                                       {"hiraragana{LEFT 3}{DELETE}{RIGHT 2}hiragana","ひらがなひらがな","1","0"},// Delete in the middle of an ongoing compostion and type at the end
                                       {"higana{HOME}{RIGHT}ra","ひらがな","1","0"},// Insert in the middle of an ongoing compostion
                                       {"hiragana{HOME}{RIGHT 1}{BACKSPACE}","らがな","1","0"},// Delete in the middle of an ongoing compostion
                                       {"hiragana{LEFT 2}{BACKSPACE}","ひがな","1","0"},// Delete in the middle of an on going compostion
                                       {"hiragana{BACKSPACE}","ひらが","1","0"}, // Delete at the end of an ongoing compostion
                                       {"hiragana{HOME}{DELETE}","らがな","1","0"},// Delete first character of an ongoing compostion
                                       {"hiragana{ENTER}{BACKSPACE}","ひらが","1","1"}, // Delete last character after finalizing
                                       {"hiragana{ENTER}{HOME}{DELETE}","らがな","1","1"}, // Delete first character after finalizing
                                       {"hiragana{ENTER}{LEFT 2}{DELETE}","ひらな","1","1"}, // Delete some character in the middle after finalizing
                                       {"hiragana{ENTER}{LEFT 4}hiragana{ENTER}","ひらがなひらがな","2","2"}, // Insert before a finalized compostion
                                       {"hiragana{ENTER}{LEFT 2}hiragana{ENTER}","ひらひらがながな","2","2"}, // Insert in the middle of a finalized compostion
                                       {"hiragana{ENTER}hiragana{LEFT 5}hiragana","ひらがひらがななひらがな","3","2"} // Verify insert finalizes an unfinalized compostion when new compostion begins
                                     };
                    break;
                case IMELocales.ChinesePinyin:
                    _os = Environment.OSVersion;
                    _ver = _os.Version;
                    if (_ver.Major > 6 || ((6 == _ver.Major) && _ver.Minor > 1))
                    {
                        _testCaseData = new string[,]{ 
                                //{"nihao{SPACE}{SPACE}","你好 ","1","1"},
                                {"nihao{SPACE}","你好","1","1"},
                                {"nihao{SPACE}{HOME}nihao{SPACE}","你好你好","2","2"},
                                {"nihao{SPACE}{LEFT}nihao{SPACE}","你你好好","2","2"},
                                {"nihao{SPACE}{HOME}{RIGHT 1}nihao{SPACE}","你你好好","2","2"},
                                {"nihao{SPACE}{HOME}{END}nihao{SPACE}","你好你好","2","2"},
                                {"ninihao{SPACE}{LEFT 2}{DELETE}{RIGHT 1}nihao{SPACE}","你好你好","2","2"},
                                {"ninihao{SPACE}{HOME}{RIGHT 1}{BACKSPACE}","你好","1","1"},
                                {"nihao{SPACE}{BACKSPACE}","你","1","1"},
                                {"nihao{SPACE}{BACKSPACE}hao{SPACE}","你好","2","2"},
                                //{"nihao{SPACE}{BACKSPACE}hao{SPACE}{ENTER}","你好","2","2"},
                                {"nihao{SPACE}nihao{SPACE}{BACKSPACE}","你好你","2","2"},
                                {"nihao{SPACE}ni{SPACE}ni{SPACE}{BACKSPACE}hao{SPACE}","你好你好","4","4"},
                                {"nihao{SPACE}nihao{SPACE}{LEFT}nihao{SPACE}","你好你你好好","3","3"},
                                //{"nihao{SPACE}{SPACE}nihao{SPACE}{LEFT}nihao{SPACE}{ENTER}","你好 你你好好","3","3"},
                                {"nihao{HOME}nihao{ENTER}","nihaonihao","1","1"},
                                {"nihao{HOME}{RIGHT 2}nihao{ENTER}","ninihaohao","1","1"},
                                };
                    }
                    else
                    {
                        _testCaseData = new string[,]{ 
                                {"nihao{SPACE}{SPACE}","你好","1","1"},
                                {"nihao{SPACE}","你好","1","0"},
                                {"nihao{SPACE}{HOME}nihao{SPACE}","你好你好","1","0"},
                                {"nihao{SPACE}{LEFT}nihao{SPACE}","你你好好","1","0"},
                                {"nihao{SPACE}{HOME}{RIGHT 1}nihao{SPACE}","你你好好","1","0"},
                                {"nihao{SPACE}{HOME}{END}nihao{SPACE}","你好你好","1","0"},
                                {"ninihao{SPACE}{LEFT 2}{DELETE}{RIGHT 1}nihao{SPACE}","你好你好","1","0"},
                                {"ninihao{SPACE}{HOME}{RIGHT 1}{BACKSPACE}","你好","1","0"},
                                {"nihao{SPACE}{BACKSPACE}","你","1","0"},
                                {"nihao{SPACE}{BACKSPACE}hao{SPACE}","你好","1","0"},
                                {"nihao{SPACE}{BACKSPACE}hao{SPACE}{ENTER}","你好","1","1"},
                                {"nihao{SPACE}{SPACE}nihao{SPACE}{BACKSPACE}","你好你","2","1"},
                                {"nihao{SPACE}{SPACE}ni{SPACE}ni{SPACE}{BACKSPACE}hao{SPACE}{SPACE}","你好你好","2","2"},
                                {"nihao{SPACE}{SPACE}nihao{SPACE}{LEFT}nihao{SPACE}","你好你你好好","2","1"},
                                {"nihao{SPACE}{SPACE}nihao{SPACE}{LEFT}nihao{SPACE}{ENTER}","你好你你好好","2","2"},
                                {"nihao{HOME}nihao","nihaonihao","1","0"},
                                {"nihao{HOME}{RIGHT 2}nihao","ninihaohao","1","0"},
                                };
                    }
                    break;                
                case IMELocales.ChineseNewPhonetic:
                    _testCaseData = new string[,]{ 
                                {"su3cl3{ENTER}","你好","1","1"},
                                {"su3cl3","你好","1","0"},
                                {"su3cl3{ENTER}su3cl3{ENTER}","你好你好","2","2"},
                                {"su3cl3su3cl3","你好你好","1","0"},
                                {"su3cl3{ENTER}{HOME}su3cl3{ENTER}","你好你好","2","2"},
                                {"su3cl3{HOME}su3cl3","你好你好","1","0"},
                                {"su3cl3{ENTER}{HOME}{END}su3cl3{ENTER}","你好你好","2","2"},
                                {"su3cl3{HOME}{END}su3cl3","你好你好","1","0"},
                                {"su3cl3{ENTER}{LEFT}su3cl3{ENTER}","你你好好","2","2"},
                                {"su3cl3{LEFT}su3cl3","你你好好","1","0"},
                                {"su3cl3{ENTER}{HOME}{RIGHT 1}su3cl3{ENTER}","你你好好","2","2"},
                                {"su3cl3{HOME}{RIGHT 1}su3cl3","你你好好","1","0"},                                       
                                {"su3su3cl3{ENTER}{LEFT 2}{DELETE}{RIGHT 1}su3cl3{ENTER}","你好你好","2","2"},
                                {"su3su3cl3{LEFT 2}{DELETE}{RIGHT 1}su3cl3","你好你好","1","0"},
                                {"su3su3cl3{ENTER}{HOME}{RIGHT 1}{BACKSPACE}","你好","1","1"},
                                {"su3su3cl3{HOME}{RIGHT 1}{BACKSPACE}","你好","1","0"},
                                {"su3cl3{ENTER}{BACKSPACE}","你","1","1"},
                                {"su3cl3{BACKSPACE}{ENTER}","你","1","1"},    
                                {"su3cl3{ENTER}su3cl3{BACKSPACE 2}su3cl3{ENTER}","你好你好","3","3"},
                                {"su3cl3{ENTER}su3cl3{LEFT}su3cl3","你好你你好好","2","1"},
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
        private int _startCompostionEventCountInRtb = 0;
        private int _updateCompostionEventCountInRtb = 0;
        private int _completeCompostionEventCountInRtb = 0;
        private int _startCompostionEventCountInTb = 0;
        private int _updateCompostionEventCountInTb = 0;
        private int _completeCompostionEventCountInTb = 0;
        private const int expectedStartCompostionEventCount = 1;

        OperatingSystem _os;
        Version _ver;

        #endregion Private fields
    }
}