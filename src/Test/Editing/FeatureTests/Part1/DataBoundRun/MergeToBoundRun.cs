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
    /// Test verifies if left bound run is merged with an adjacent right unbound run to form a single bound run  
    /// FlowDocument Structure
    /// <paragraph><RUN1>LEFT</RUN1><Run2>RIGHT</Run2></paragraph>
    /// RUN1 - Bound Run
    /// RUN2 - Unbound Run
    /// </summary>
    [Test(1, "DataBoundRun", "MergeToBoundRun", MethodParameters = "/TestCaseType:MergeToBoundRun", Timeout = 200)]
    public class MergeToBoundRun : CustomTestCase
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

            QueueDelegate(MergeVariations);
        }

        private void MergeVariations()
        {   
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
            SendInput(_richTextbox, "^{HOME}123", "Typing 123 at the beginning left run.");
            VerifyInlineCountAndText();

            SendInput(_richTextbox, "^{END}456", "Typing 456 at the end left run.");
            VerifyInlineCountAndText();

            SendInput(_textbox, "^{HOME}*", "Typing * at the begining of the textbox");
            VerifyInlineCountAndText();

            SendInput(_textbox, "^{END}-", "Typing - at end of the textbox");
            VerifyInlineCountAndText();

            SendInput(_textbox, "^{HOME}{RIGHT 3},", "Typing , in the middle of the textbox");
            VerifyInlineCountAndText();
        }
        
        private void SendInput(TextBoxBase textboxBase, string inputString, string inputDescription)
        {
            Log(inputDescription);
            textboxBase.Focus();
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();
            KeyboardInput.TypeString(inputString);
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(delayTimeToBind);
        }

        private void VerifyInlineCountAndText()
        {
            Paragraph paragraph1;
            string str;

            paragraph1 = (Paragraph)_richTextbox.Document.Blocks.FirstBlock;
            Log("Actual number of Inlines = " + paragraph1.Inlines.Count);
            Verifier.Verify(paragraph1.Inlines.Count == 1, "Verifying if number of Inlines in Paragraph = 1.", true);           
            
            str = ((Run)paragraph1.Inlines.FirstInline).Text;
            Log("Actual :: Text in textbox :[" + _textbox.Text + "]");
            Log("Expected :: Text in FlowDocument : [" + str + "]");
            Verifier.Verify(str == _textbox.Text, "Verifying if contents of textbox and flowdocument are same.", true);
        }

        private void SetInitialContent()
        {
            Run r1 = new Run();
            Run r2 = new Run();
            r2.Text = "RIGHT";
            // Add Content
            _textbox.Text = "LEFT";
            _richTextbox.Document.Blocks.Clear();
            Paragraph para1 = new Paragraph();
            para1.Inlines.Add(r1);
            para1.Inlines.Add(r2);
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