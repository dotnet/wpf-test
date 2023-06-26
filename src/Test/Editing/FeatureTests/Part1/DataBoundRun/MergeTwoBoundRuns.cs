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
    /// Test verifies if Left bound run is merged with an adjacent right bound run to form a single bound run
    /// FlowDocument Structure
    /// <paragraph><RUN1>LEFT</RUN1><Run2>RIGHT</Run2></paragraph>
    /// Run1 - bound to textbox1
    /// Run2 - bound to textbox2
    /// </summary>
    [Test(2, "DataBoundRun", "MergeTwoBoundRuns", MethodParameters = "/TestCaseType:MergeTwoBoundRuns", Timeout = 200)]
    public class MergeTwoBoundRuns : CustomTestCase
    {
        #region Main flow

        /// <summary>Starts the combination</summary>
        public override void RunTestCase()
        {
            _richTextbox = new RichTextBox();
            _richTextbox.Height = 100;
            _richTextbox.Width = 300;
            _textbox1 = new TextBox();
            _textbox2 = new TextBox();
            StackPanel panel = new StackPanel();
            panel.Children.Add(_textbox1);
            panel.Children.Add(_textbox2);
            panel.Children.Add(_richTextbox);
            MainWindow.Content = panel;

            QueueDelegate(MergeVariations);
        }

        private void MergeVariations()
        {
            //Verify adjacent bound runs merge into a single bound run
            // LEFTRIGHT -> LEFRIGHT
            RunVariation(_richTextbox, "^{HOME}{RIGHT 3}{DELETE}", "Deleting 'T' from \"LEFT\" in flowdocument to merge the runs.");
            // LEFTRIGHT -> LEFTIGHT
            RunVariation(_richTextbox, "^{HOME}{RIGHT 4}{DELETE}", "Deleting 'R' from \"RIGHT\" in flowdocument to merge the runs.");
            // LEFTRIGHT -> LEFIGHT
            RunVariation(_richTextbox, "^{HOME}{RIGHT 3}{DELETE 2}", "Deleting 'T' from \"LEFT\" and 'R' from \"RIGHT\" in flowdocument to merge the runs.");
            // LEFTRIGHT -> LEIGHT
            RunVariation(_richTextbox, "^{HOME}{RIGHT 2}{DELETE 3}", "Deleting 'FT' from \"LEFT\" and 'R' from \"RIGHT\" in flowdocument to merge the runs.");
            // LEFTRIGHT -> LEGHT
            RunVariation(_richTextbox, "^{HOME}{RIGHT 2}{DELETE 4}", "Deleting 'FT' from \"LEFT\" and 'RI' from \"RIGHT\" in flowdocument to merge the runs.");
            // LEFTRIGHT -> LEF
            RunVariation(_richTextbox, "^{HOME}{RIGHT 3}{DELETE 6}", "Deleting 'T' from \"LEFT\" and 'RIGHT' from \"RIGHT\" in flowdocument to merge the runs.");
            // LEFTRIGHT -> LET
            RunVariation(_richTextbox, "^{HOME}{RIGHT 2}{DELETE 6}", "Deleting 'FT' from \"LEFT\" and 'RIGH' from \"RIGHT\" in flowdocument to merge the runs.");

            Logger.Current.ReportSuccess();
        }

        #endregion

        #region Helpers

        private void RunVariation(TextBoxBase inputTarget, string inputString, string inputDescription)
        {
            SetInitialContent();
            SendInput(inputTarget, inputString, inputDescription);
            VerifyInlineCountAndText();
            SendInputAndVerifyBinding();
        }

        private void SendInputAndVerifyBinding()
        {
            SendInput(_textbox1, "^{HOME}A", "Typing A at the begining of the textbox");
            VerifyInlineCountAndText();

            SendInput(_textbox1, "^{END}Z", "Typing Z at end of the textbox");
            VerifyInlineCountAndText();

            SendInput(_textbox1, "^{HOME}{RIGHT 3},", "Typing , in the middle of the textbox");
            VerifyInlineCountAndText();

            SendInput(_richTextbox, "^{HOME}123", "Typing 123 at the beginning of the merged run.");
            VerifyInlineCountAndText();

            SendInput(_richTextbox, "^{END}456", "Typing 456 at the end of the merged run.");
            VerifyInlineCountAndText();
            
        }   
          
        private void SendInput(TextBoxBase textboxBase, string inputString, string inputDescription)
        {
            Log(inputDescription);
            textboxBase.Focus();
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();
            KeyboardInput.TypeString(inputString);
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(_delayTimeToBind);
        }

        private void VerifyInlineCountAndText()
        {
            Paragraph paragraph1;
            string str;
            paragraph1 = (Paragraph)_richTextbox.Document.Blocks.FirstBlock;
            Log("Actual number of Inlines = " + paragraph1.Inlines.Count);
            Verifier.Verify(paragraph1.Inlines.Count == 1, "Verifying if number of Inlines in Paragraph = 1.", true);

            str = ((Run)paragraph1.Inlines.FirstInline).Text;            
            Log("Actual :: Text in textbox1 [" + _textbox1.Text +"]");
            Log("Expected :: Text in FlowDocument [" + str +"]");
            Verifier.Verify(str == _textbox1.Text, "Verifying if contents of textbox1 and flowdocument are same.", true);            
        }

        private void SetInitialContent()
        {
            Run r1 = new Run();
            Run r2 = new Run();
            _textbox1.Text = "LEFT";
            _textbox2.Text = "RIGHT";

            _richTextbox.Document.Blocks.Clear();
            Paragraph para1 = new Paragraph();
            para1.Inlines.Add(r1);
            para1.Inlines.Add(r2);
            _richTextbox.Document.Blocks.Add(para1);
            // Bind textbox1 text property to Flowdocument Run1
            Binding binding1 = new Binding("Text");
            binding1.Source = _textbox1;
            r1.SetBinding(Run.TextProperty, binding1);

            // Bind textbox2 text property to Flowdocument Run2
            Binding binding2 = new Binding("Text");
            binding2.Source = _textbox2;
            r2.SetBinding(Run.TextProperty, binding2);
        }

        #endregion Helpers

        #region Private fields

        private RichTextBox _richTextbox;
        private TextBox _textbox1;
        private TextBox _textbox2;
        private int _delayTimeToBind = 2000;

        #endregion
    }
}