// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Verify that unfinalized input gets finalized automatically with change
//  in focus.

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
    [Test(0, "IME", "IMEVerifyAutoFinalization_Korean", MethodParameters = "/TestCaseType:IMEVerifyAutoFinalization /locale=Korean", Timeout = 120, Keywords = "KoreanIME")]
    [Test(0, "IME", "IMEVerifyAutoFinalization_Japanese", MethodParameters = "/TestCaseType:IMEVerifyAutoFinalization /locale=Japanese", Timeout = 120, Keywords = "JapaneseIME")]
    [Test(0, "IME", "IMEVerifyAutoFinalization_ChinesePinyin", MethodParameters = "/TestCaseType:IMEVerifyAutoFinalization /locale=ChinesePinyin", Timeout = 120, Keywords = "ChinesePinyinIME")]
    [Test(0, "IME", "IMEVerifyAutoFinalization_ChineseNewPhonetic", MethodParameters = "/TestCaseType:IMEVerifyAutoFinalization /locale=ChineseNewPhonetic", Timeout = 120, Keywords = "ChineseNewPhoneticIME")]
    [Test(0, "IME", "IMEVerifyAutoFinalization_ChineseQuanPin", MethodParameters = "/TestCaseType:IMEVerifyAutoFinalization /locale=ChineseQuanPin", Timeout = 120, Keywords = "ChineseQuanPinIME")]
    public class IMEVerifyAutoFinalization : ManagedCombinatorialTestCase
    {
        #region Main flow

        /// <summary>Starts the combination</summary>
        protected override void DoRunCombination()
        {
            if (_locale == IMELocales.ChineseQuanPin)
            {
                Log("Skipping this combination as it is not interesting");
                NextCombination(); 
            }
            else
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
        }

        private void PerformTestActions()
        {
            Log("Load IME keyboard");
            _testTextBox.Focus();
            IMEHelper.SetUpIMEKeyboardLayout(_locale, _testTextBox, MainWindow);

            _rtb.Document.Blocks.Clear();
            _tb.Clear();

            AddEventHandlers();
            SendInputAndVerifyResult();
            RemoveEventHandlers();

            NextCombination();
        }

        private void SendInputAndVerifyResult()
        {
            // Send Input            
            _rtb.Focus();
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();
            KeyboardInput.TypeString(_contentToTypeInIME);
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEHelper.CiceroWaitTimeMs);

            // Verify composed ime string and events
            Paragraph p = (Paragraph)_rtb.Document.Blocks.FirstBlock;
            TextRange tr = new TextRange(p.ContentStart, p.ContentEnd);
            // Verify content in the rich textbox
            Verifier.Verify(tr.Text == _composedStringByIME, "Verifying contents of richtextbox : Actual[" +
               tr.Text + "] Expected[" + _composedStringByIME + "]", true);
            // Verify start, update and complete composition events
            Verifier.Verify(_startCompostionEventCountInRtb == 1, "Verifying start compostion event count in RTB: Actual[" +
           _startCompostionEventCountInRtb + "]", false);
            Verifier.Verify(_updateCompostionEventCountInRtb > 0, "Verifying update compostion event count in RTB > 0, Actual = " + _updateCompostionEventCountInRtb, false);

            //It is finished keydown "你好" in the textbox or richtextbox when chinese pinyin simplefast type "nihao{SPACE}" 
            _ver = Environment.OSVersion.Version;
            if ((_ver.Major > 6 || ((6 == _ver.Major) && _ver.Minor > 1)) && (_locale == IMELocales.ChinesePinyin))
            {
                Verifier.Verify(_completeCompostionEventCountInRtb == 1, "Verifying complete compostion event count before focus change in RTB: Actual[" +
                     _completeCompostionEventCountInRtb + "] Expected = 1", false);
            }
            else
            {
                Verifier.Verify(_completeCompostionEventCountInRtb == 0, "Verifying complete compostion event count before focus change in RTB: Actual[" +
                 _completeCompostionEventCountInRtb + "] Expected = 0", false);
            }

            // Change focus to textbox and verify complete event
            _tb.Focus();
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();
            Verifier.Verify(_completeCompostionEventCountInRtb == 1, "Verifying complete compostion event count after focus change in RTB: Actual[" +
                _completeCompostionEventCountInRtb + "] Expected = 1", false);

            KeyboardInput.TypeString(_contentToTypeInIME);
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEHelper.CiceroWaitTimeMs);
            // Verify composed ime string and events
            Verifier.Verify(_tb.Text == _composedStringByIME, "Verifying contents of TextBox : Actual[" +
              _tb.Text + "] Expected[" + _composedStringByIME + "]", true);
            // Verify start, update and complete composition events
        
            Verifier.Verify(_startCompostionEventCountInTb == 1, "Verifying start compostion event count in TB: Actual[" +
          _startCompostionEventCountInTb + "]", false);
            Verifier.Verify(_updateCompostionEventCountInTb > 0, "Verifying update compostion event count in TB > 0, Actual = " + _updateCompostionEventCountInTb, false);

            //It is finished keydown "你好" in the textbox or richtextbox when chinese pinyin simplefast type "nihao{SPACE}" 
            if ((_ver.Major > 6 || ((6 == _ver.Major) && _ver.Minor > 1)) && (_locale == IMELocales.ChinesePinyin))
            {                
                    Verifier.Verify(_completeCompostionEventCountInTb == 1, "Verifying complete compostion event count before focus change in TB: Actual[" +
                     _completeCompostionEventCountInTb + "] Expected = 1", false);                
            }
            else
            {
                Verifier.Verify(_completeCompostionEventCountInTb == 0, "Verifying complete compostion event count before focus change in TB: Actual[" +
                 _completeCompostionEventCountInTb + "] Expected = 0", false);
            }

            // Change focus to richTextbox and verify complete event
            _rtb.Focus();
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();
            Verifier.Verify(_completeCompostionEventCountInTb == 1, "Verifying complete compostion event count after focus change in TB: Actual[" +
                _completeCompostionEventCountInTb + "] Expected = 1", true);
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
                    _contentToTypeInIME = "rk";
                    _composedStringByIME = "가";
                    break;
                case IMELocales.Japanese:
                    _contentToTypeInIME = "hiragana{SPACE}";
                    _composedStringByIME = "ひらがな";
                    break;
                case IMELocales.ChinesePinyin:
                    _contentToTypeInIME = "nihao{SPACE}";
                    _composedStringByIME = "你好";
                    break;
                case IMELocales.ChineseNewPhonetic:
                    _contentToTypeInIME = "su3cl3a87";
                    _composedStringByIME = "你好嗎";
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
        private int _startCompostionEventCountInRtb = 0;
        private int _updateCompostionEventCountInRtb = 0;
        private int _completeCompostionEventCountInRtb = 0;
        private int _startCompostionEventCountInTb = 0;
        private int _updateCompostionEventCountInTb = 0;
        private int _completeCompostionEventCountInTb = 0;
        private string _contentToTypeInIME = string.Empty;
        private string _composedStringByIME = string.Empty;

        Version _ver;

        #endregion Private fields
    }
}