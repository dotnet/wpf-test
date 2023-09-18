// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides coverage to verify that undo operation does not crash after
//  splitting a bound run

using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Data;
using Microsoft.Test.Discovery;
using Test.Uis.Loggers;
using Test.Uis.TestTypes;
using Test.Uis.Utils;

namespace Microsoft.Test.DataBoundRun
{
    /// <summary>
    /// Test verifies that undo operation after splitting a bound run does not crash
    /// FlowDocument Structure 
    /// <paragraph><Run>DISCOVER</Run></paragraph>
    /// Run - bound to textbox
    /// </summary>
    [Test(2, "DataBoundRun", "UndoAfterSplit", MethodParameters = "/TestCaseType:UndoAfterSplit", Timeout = 200)]
    public class UndoAfterSplit : CustomTestCase
    {
        #region Main flow

        /// <summary>Starts the combination</summary>
        public override void RunTestCase()
        {
            _richTextbox = new RichTextBox();
            _richTextbox.Height = 100;
            _richTextbox.Width = 300;
            _textbox = new TextBox();
            StackPanel panel = new StackPanel();
            panel.Children.Add(_textbox);
            panel.Children.Add(_richTextbox);
            MainWindow.Content = panel;

            QueueDelegate(SplitVariations);
        }

        private void SplitVariations()
        {
            // DISCOVER -> DIS(bound) COVER(not bound)
            RunVariation(_richTextbox, "^{HOME}{RIGHT 3}{ENTER}", "Typing Return key after DIS");
            // DISCOVER -> D(bound) ISCOVER(not bound)
            RunVariation(_richTextbox, "^{HOME}{RIGHT 1}{ENTER}", "Typing Return key after D in DISCOVER");
            // DISCOVER -> DISCOVE(bound) R(not bound)
            RunVariation(_richTextbox, "^{END}{LEFT 1}{ENTER}", "Typing Return key after E in DISCOVER");

            Logger.Current.ReportSuccess();
        }

        #endregion

        #region Helpers

        private void RunVariation(TextBoxBase inputTarget, string inputString, string inputDescription)
        {
            SetInitialContent();
            SendInput(inputTarget, inputString, inputDescription);
            VerifyBlockCount();
            _richTextbox.Undo();
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();
            SendInputAndVerifyIfBindingIsLost();
        }

        private void SendInputAndVerifyIfBindingIsLost()
        {
            SendInput(_textbox, "^{HOME}33", "Typing 33 at the begining of the textbox");
            VerifyInlineText("33");

            SendInput(_textbox, "^{END}44", "Typing 44 at end of the textbox");
            VerifyInlineText("44");

            SendInput(_textbox, "^{HOME}{RIGHT 3}55", "Typing 55 in the middle of the textbox");
            VerifyInlineText("55");
        }

        private void SendInput(TextBoxBase textboxBase, string inputString, string inputDescription)
        {
            Log(inputDescription);
            textboxBase.Focus();
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();
            KeyboardInput.TypeString(inputString);
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(_delayTimeToBind);
        }

        private void VerifyInlineText(string inputString)
        {
            Paragraph paragraph1;
            string str;
            paragraph1 = (Paragraph)_richTextbox.Document.Blocks.FirstBlock;
            str = ((Run)paragraph1.Inlines.FirstInline).Text;
            Verifier.Verify(str.Contains(inputString) == false, "Verifying if contents of textbox are not updated in the flowdocument.", true);
        }

        private void VerifyBlockCount()
        {
            int count;
            count = _richTextbox.Document.Blocks.Count;
            Log("Actual number of blocks in the FlowDocument : " + count);
            Verifier.Verify(count == 2, "Verifying that the number of blocks in the flowdocument = 2", true);
        }

        private void SetInitialContent()
        {
            Run r = new Run();
            _textbox.Text = "DISCOVER";
            _richTextbox.Document.Blocks.Clear();
            Paragraph para1 = new Paragraph();
            para1.Inlines.Add(r);
            _richTextbox.Document.Blocks.Add(para1);
            // Bind textbox text property to Flowdocument Run
            Binding binding = new Binding("Text");
            binding.Source = _textbox;
            r.SetBinding(Run.TextProperty, binding);
        }

        #endregion Helpers

        #region Private fields

        private RichTextBox _richTextbox;
        private TextBox _textbox;
        private int _delayTimeToBind = 2000;

        #endregion
    }
}