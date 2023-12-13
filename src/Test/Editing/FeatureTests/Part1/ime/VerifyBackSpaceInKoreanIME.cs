// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Validates that backspace deletes only the last symbol for an ongoing
//  composition and not the complete character. Also verify that backspace
//  deletes complete character once input is finalized.

using System.Windows.Controls;

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
    [Test(0, "IME", "VerifyBackSpaceInKoreanIME", MethodParameters = "/TestCaseType:VerifyBackSpaceInKoreanIME", Timeout = 120, Keywords = "KoreanIME")]
    public class VerifyBackSpaceInKoreanIME : CustomTestCase
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
            int blockCount;

            Log("Load IME keyboard");
            _testTextBox.Focus();
            IMEHelper.SetUpIMEKeyboardLayout(IMELocales.Korean, _testTextBox, MainWindow);

            // Verify backspace in rich text box
            _rtb.Focus();
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();
            KeyboardInput.TypeString("gksrmf");
            KeyboardInput.TypeString("{BACKSPACE 3}");
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEHelper.CiceroWaitTimeMs);

            Paragraph p = (Paragraph)_rtb.Document.Blocks.FirstBlock;
            TextRange tr = new TextRange(p.ContentStart, p.ContentEnd);
            Verifier.Verify(tr.Text.Equals("한"), "Verifying contents of richtextbox after typing: Actual[" +
               tr.Text + "] Expected[ 한 ]", true);
            _rtb.Focus();
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();
            KeyboardInput.TypeString("{BACKSPACE}");

            blockCount = _rtb.Document.Blocks.Count;
            if (blockCount > 0)
            {
                int inlineCount = ((Paragraph)(_rtb.Document.Blocks.FirstBlock)).Inlines.Count;
                Verifier.Verify(inlineCount == 1, "Verifying Inline Count : " + inlineCount, true);
            }
            else
            {
                Verifier.Verify(blockCount == 0, "Verifying Block Count : " + blockCount, true);
            }

            // Verify backspace behavour in text box
            _tb.Focus();
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();
            KeyboardInput.TypeString("gksrmf");           
            KeyboardInput.TypeString("{BACKSPACE 3}");
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEHelper.CiceroWaitTimeMs);

            Verifier.Verify(_tb.Text.Equals("한"), "Verifying contents of textbox after typing: Actual[" +
               _tb.Text + "] Expected[ 한 ]", true);
            KeyboardInput.TypeString("{BACKSPACE}");
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEHelper.CiceroWaitTimeMs);
            Verifier.Verify(_tb.Text.Equals("") || _tb.Text == null, "Verifying contents of textbox after typing: Actual[" +
              _tb.Text + "] Expected[]", true);

            Logger.Current.ReportSuccess();
        }

        #endregion Main flow

        #region Private fields

        private StackPanel _panel = null;
        private RichTextBox _rtb = null;
        private TextBox _tb = null;
        private TextBox _testTextBox = null; // Used just to set the appropriate IME mode

        #endregion
    }
}