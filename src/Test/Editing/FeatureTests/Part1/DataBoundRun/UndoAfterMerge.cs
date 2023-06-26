// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides coverage to verify that any undo after a merge does not crash

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
    /// Test verifies if undo operation after a merge operation does not crash
    /// FlowDocument Structure
    /// <paragraph><RUN1>Some Text</RUN1><RUN2> Some Text</RUN2></paragraph>
    /// Run1 or Run2 is bound depending on the test setting
    /// </summary>
    [Test(2, "DataBoundRun", "UndoAfterMerge", MethodParameters = "/TestCaseType:UndoAfterMerge")]
    public class UndoAfterMerge : CustomTestCase
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
            // ONETWO -> ONTWO  
            // ONE - Bound TWO - Unbound
            SetInitialContent(true);
            RunVariation(_richTextbox, "^{HOME}{RIGHT 2}{DELETE}", "Deleting 'E' from \"ONE\" in flowdocument to merge the runs.");

            // TWOONE -> TWONE 
            // ONE - Bound TWO - Unbound
            SetInitialContent(false);
            RunVariation(_richTextbox, "^{HOME}{RIGHT 2}{DELETE}", "Deleting 'O' from \"TWO\" in flowdocument to merge the runs.");

            // ONETWO -> ONEWO  
            // ONE - Bound TWO - Unbound
            SetInitialContent(true);
            RunVariation(_richTextbox, "^{HOME}{RIGHT 3}{DELETE}", "Deleting 'T' from \"TWO\" in flowdocument to merge the runs.");

            // TWOONE -> TWONE   
            // ONE - Bound TWO - Unbound
            SetInitialContent(false);
            RunVariation(_richTextbox, "^{HOME}{RIGHT 3}{DELETE}", "Deleting 'O' from \"ONE\" in flowdocument to merge the runs.");

            Logger.Current.ReportSuccess();
        }

        #endregion

        #region Helpers

        private void RunVariation(TextBoxBase inputTarget, string inputString, string inputDescription)
        {
            Log(inputDescription);
            SendInput(inputTarget, inputString, inputDescription);
            VerifyInlineCount();       
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

        private void VerifyInlineText(string inputString)
        {
            Paragraph paragraph1;
            string str;
            paragraph1 = (Paragraph)_richTextbox.Document.Blocks.FirstBlock;
            str = ((Run)paragraph1.Inlines.FirstInline).Text;
            Verifier.Verify(str.Contains(inputString) == false, "Verifying if contents of textbox are not updated in the flowdocument.", true);
        }

        private void VerifyInlineCount()
        {
            Paragraph paragraph1;
            paragraph1 = (Paragraph)_richTextbox.Document.Blocks.FirstBlock;
            Log("Actual number of Inlines = " + paragraph1.Inlines.Count);
            Verifier.Verify(paragraph1.Inlines.Count == 1, "Verifying if number of Inlines in Paragraph = 1 .", true);
        }

        private void SendInput(TextBoxBase textboxBase, string inputString, string inputDescription)
        {
            Log(inputDescription);
            textboxBase.Focus();
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();
            KeyboardInput.TypeString(inputString);
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(_delayTimeToBind);
        }

        private void SetInitialContent(bool setContentToMergeAsBound)
        {
            Run r1 = new Run();
            Run r2 = new Run();
            r2.Text = "TWO";
            _textbox.Text = "ONE";
            _richTextbox.Document.Blocks.Clear();
            Paragraph para1 = new Paragraph();

            if (setContentToMergeAsBound)
            {
                para1.Inlines.Add(r1);
                para1.Inlines.Add(r2);
            }
            else
            {
                para1.Inlines.Add(r2);
                para1.Inlines.Add(r1);                
            }

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
        private int _delayTimeToBind = 2000;

        #endregion
    }
}