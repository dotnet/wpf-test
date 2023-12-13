// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Verifies text conversion from hiragana to katakana in Japanese

using System.Windows.Controls;

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
    [Test(0, "IME", "VerifyHiraganaToKatakana", MethodParameters = "/TestCaseType:VerifyHiraganaToKatakana", Timeout = 120, Keywords = "JapaneseIME")]
    public class VerifyHiraganaToKatakana : CustomTestCase
    {
        #region Main flow

        /// <summary>Starts the combination</summary>
        public override void RunTestCase()
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

            QueueDelegate(PerformTestActions);
        }

        private void PerformTestActions()
        {
            Log("Load IME keyboard");
            _testTextBox.Focus();
            IMEHelper.SetUpIMEKeyboardLayout(IMELocales.Japanese, _testTextBox, MainWindow);

            for (int count = 0; count < _testCaseData.GetLength(0); count++)
            {
                _rtb.Document.Blocks.Clear();
                _tb.Clear();

                VerifyTextConversionInRTB(count);
                VerifyTextConversionInTB(count);
            }

            Logger.Current.ReportSuccess();
        }

        private void VerifyTextConversionInRTB(int index)
        {
            Paragraph p;
            TextRange tr;

            _rtb.Focus();
            KeyboardInput.TypeString(_testCaseData[index, 0]);
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEHelper.CiceroWaitTimeMs);
            
            p = (Paragraph)_rtb.Document.Blocks.FirstBlock;
            tr = new TextRange(p.ContentStart, p.ContentEnd);
            Verifier.Verify(tr.Text == _testCaseData[index, 1], "Verifying contents of richtextbox after typing: Actual[" +
               tr.Text + "] Expected[" + _testCaseData[index, 1] + "]", false);

            KeyboardInput.TypeString("{SPACE}{ENTER}");
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEHelper.CiceroWaitTimeMs);

            p = (Paragraph)_rtb.Document.Blocks.FirstBlock;
            tr = new TextRange(p.ContentStart, p.ContentEnd);
            Verifier.Verify(tr.Text == _testCaseData[index, 2], "Verifying contents of richtextbox after typing: Actual[" +
               tr.Text + "] Expected[" + _testCaseData[index, 2] + "]", false);
        }

        private void VerifyTextConversionInTB(int index)
        {           
            _tb.Focus();
            KeyboardInput.TypeString(_testCaseData[index, 0]);
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEHelper.CiceroWaitTimeMs);

            Verifier.Verify(_tb.Text == _testCaseData[index, 1], "Verifying contents of textbox after typing: Actual[" +
               _tb.Text + "] Expected[" + _testCaseData[index, 1] + "]", false);

            KeyboardInput.TypeString("{SPACE}{ENTER}");
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEHelper.CiceroWaitTimeMs);

            Verifier.Verify(_tb.Text == _testCaseData[index, 2], "Verifying contents of textbox after typing: Actual[" +
              _tb.Text + "] Expected[" + _testCaseData[index, 2] + "]", false);            
        }              
        #endregion Main flow

        #region Private fields
        
        private StackPanel _panel = null;
        private RichTextBox _rtb = null;
        private TextBox _tb = null;
        private TextBox _testTextBox = null; // Used just to set the appropriate IME mode
        private string[,] _testCaseData = new string[,]{ 
                                       {"aisu","あいす","アイス"},
                                       {"tesuto","てすと","テスト"},
                                       {"miruku","みるく","ミルク"},
                                       {"hoteru","ほてる","ホテル"},
                                       {"terebi","てれび","テレビ"},
                                       };  

        #endregion Private fields
    }
}