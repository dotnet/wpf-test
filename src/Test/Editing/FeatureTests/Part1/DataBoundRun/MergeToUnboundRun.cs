// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides coverage to verify binding while merging runs

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
    /// Test verifies if bound run on the right side is merged with an adjacent unbound run on the left side to form a single unbound run.
    /// FlowDocument Structure
    /// <paragraph><RUN1>UNBOUND</RUN1><Run2>BOUND</Run2></paragraph>
    /// RUN1 - Unbound Run
    /// RUN2 - Bound Run
    /// </summary>
    [Test(1, "DataBoundRun", "MergeToUnboundRun", MethodParameters = "/TestCaseType:MergeToUnboundRun", Timeout = 200)]
    public class MergeToUnboundRun : CustomTestCase
    {
        #region Main flow
        
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

            QueueDelegate(MergeVariations);
        }

        private void MergeVariations()
        {
            // UNBOUNDBOUND -> UNBOUNBOUND
            RunVariation(_richTextbox, "^{HOME}{RIGHT 6}{DELETE}", "Deleting 'D' from \"UNBOUND\" in flowdocument to merge the runs.");
            // UNBOUNDBOUND -> UNBOUNDOUND
            RunVariation(_richTextbox, "^{HOME}{RIGHT 7}{DELETE}", "Deleting 'B' from \"BOUND\" in flowdocument to merge the runs.");
            // UNBOUNDBOUND -> UNBOUNOUND
            RunVariation(_richTextbox, "^{HOME}{RIGHT 6}{DELETE 2}", "Deleting 'D' from \"UNBOUND\" and 'B' from \"BOUND\" in flowdocument to merge the runs.");
            // UNBOUNDBOUND -> UNBOUUND
            RunVariation(_richTextbox, "^{HOME}{RIGHT 5}{DELETE 4}", "Deleting 'ND' from \"UNBOUND\" and 'BO' from \"BOUND\" in flowdocument to merge the runs.");
            // UNBOUNDBOUND -> UNBOU
            RunVariation(_richTextbox, "^{HOME}{RIGHT 5}{DELETE 7}", "Deleting 'ND' from \"UNBOUND\" and 'BOUND' from \"BOUND\" in flowdocument to merge the runs.");

            Logger.Current.ReportSuccess();
        }

        #endregion Main flow

        #region Helpers

        private void RunVariation(TextBoxBase inputTarget, string inputString, string inputDescription)
        { 
            SetInitialContent();
            SendInput(inputTarget, inputString, inputDescription);
            VerifyInlineCount();
            SendInputAndVerifyBinding();
        }

        private void SendInputAndVerifyBinding()
        {
            Paragraph paragraph1;
            string str;

            SendInput(_textbox, "^{END}123", "Typing 123 in the textbox");
            paragraph1 = (Paragraph)_richTextbox.Document.Blocks.FirstBlock;
            str = ((Run)paragraph1.Inlines.FirstInline).Text;
            VerifyInlineCount();
            Verifier.Verify(str.Contains("123") == false, "Verifying if the contents of textbox and flowdocument are not same.", true);

            SendInput(_textbox, "^{HOME}456", "Typing 456 in the textbox");
            paragraph1 = (Paragraph)_richTextbox.Document.Blocks.FirstBlock;
            str = ((Run)paragraph1.Inlines.FirstInline).Text;
            VerifyInlineCount();
            Verifier.Verify(str.Contains("456") == false, "Verifying if the contents of textbox and flowdocument are not same.", true);

            SendInput(_richTextbox, "^{HOME}Y", "Typing Y at the beginning of the merged run.");
            VerifyInlineCount();
            Verifier.Verify((_textbox.Text).Contains("Y") == false, "Verifying if the contents of textbox are not same.", true);

            SendInput(_richTextbox, "^{END}X", "Typing X at the end of the merged run.");
            VerifyInlineCount();
            Verifier.Verify((_textbox.Text).Contains("X") == false, "Verifying if the contents of textbox are not same.", true);
        }      

        private void SendInput(TextBoxBase textboxBase, string inputString, string inputDescription)
        {
            Log(inputDescription);
            textboxBase.Focus();
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();
            KeyboardInput.TypeString(inputString);
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(delayTimeToBind);
        }

        private void VerifyInlineCount()
        {
            Paragraph paragraph1;
            paragraph1 = (Paragraph)_richTextbox.Document.Blocks.FirstBlock;
            Log("Actual number of Inlines = " + paragraph1.Inlines.Count);
            Verifier.Verify(paragraph1.Inlines.Count == 1, "Verifying if number of Inlines in Paragraph = 1." , true);
        }

        private void SetInitialContent()
        {
            Run r1 = new Run();
            Run r2 = new Run();
            r2.Text = "UNBOUND";
            _textbox.Text = "BOUND";
            _richTextbox.Document.Blocks.Clear();
            Paragraph para1 = new Paragraph();
            para1.Inlines.Add(r2);
            para1.Inlines.Add(r1);
            _richTextbox.Document.Blocks.Add(para1);
            // Bind textbox text property to Flowdocument Run
            Binding binding = new Binding("Text");
            binding.Source = _textbox;
            r1.SetBinding(Run.TextProperty, binding);
        }
        #endregion Helpers

        #region Private fields

        private RichTextBox _richTextbox;
        private TextBox _textbox;
        private const int delayTimeToBind = 2000;

        #endregion
    }
}