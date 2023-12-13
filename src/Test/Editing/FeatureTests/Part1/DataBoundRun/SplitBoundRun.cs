// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides coverage to verify binding while spliting a bound run

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
    /// Test verifies if the left portion of the run is still bound to the textbox text property after splitting the run
    /// FlowDocument Structure 
    /// <paragraph><Run>DISCOVER</Run></paragraph>
    /// Run bound to textbox
    /// </summary>
    [Test(1, "DataBoundRun", "SplitBoundRun", MethodParameters = "/TestCaseType:SplitBoundRun", Timeout = 200)]
    public class SplitBoundRun : CustomTestCase
    {
        #region Main flow

        /// <summary>Starts the combination</summary>
        public override void RunTestCase()
        {
            StackPanel panel = new StackPanel();
            _richTextbox = new RichTextBox();
            _textbox = new TextBox();
            _richTextbox.Height = 100;
            _richTextbox.Width = 300;            
            panel.Children.Add(_textbox);
            panel.Children.Add(_richTextbox);
            MainWindow.Content = panel;

            QueueDelegate(SplitRunUsingReturnKey);
        }

        private void SplitRunUsingReturnKey()
        {   
            // DISCOVER -> DIS(bound) COVER(not bound)
            RunVariation(_richTextbox, "^{HOME}{RIGHT 3}{ENTER}", "Typing Return key after DIS");
            // DISCOVER -> D(bound) ISCOVER(not bound)
            RunVariation(_richTextbox, "^{HOME}{RIGHT 1}{ENTER}", "Typing Return key after D in DISCOVER");
            // DISCOVER -> DISCOVE(bound) R(not bound)
            RunVariation(_richTextbox, "^{END}{LEFT 1}{ENTER}", "Typing Return key after E in DISCOVER");

            QueueDelegate(SplitRunChangingFormattingProperties);
        }

        private void SplitRunChangingFormattingProperties()
        {
            // DISCOVER -> D(bound) I(unbound)SCOVER(not bound)
            RunVariation(2, 3, "Making I in DISCOVER to bold", 3);
            // DISCOVER -> DIS(bound) COVER(not bound)
            RunVariation(4, 9, "Making COVER in DISCOVER to bold", 2);
            // DISCOVER -> DISCOVE(bound) R(not bound)
            RunVariation(8, 9, "Making R in DISCOVER to bold", 2);
            // DISCOVER -> D(bound) ISCOVER(not bound)
            RunVariation(1, 2, "Making D in DISCOVER to bold", 2);

            Logger.Current.ReportSuccess();
        }
        
        #endregion

        #region Helpers

        private void RunVariation(TextBoxBase inputTarget, string inputString, string inputDescription)
        {
            Log(inputDescription);
            SetInitialContent();
            SendInput(inputTarget, inputString, inputDescription);
            VerifyBlockCount();
            SendInputToVerifyBinding();            
        }

        private void RunVariation(int selectionStartOffset, int selectionEndOffset, string inputDescription, int expectedInlineCount)
        {
            TextPointer textPointer1, textPointer2, textPointer3;
            TextRange textRange;           

            System.Windows.DependencyProperty formattingProperty = TextElement.FontWeightProperty;
            object value = System.Windows.FontWeights.Bold;

            SetInitialContent();
            Log(inputDescription);
            textPointer1 = _richTextbox.Document.Blocks.FirstBlock.ContentStart;
            textPointer2 = textPointer1.GetPositionAtOffset(selectionStartOffset, LogicalDirection.Forward);
            textPointer3 = textPointer1.GetPositionAtOffset(selectionEndOffset, LogicalDirection.Forward);

            textRange = new TextRange(textPointer2, textPointer3);
            textRange.ApplyPropertyValue(formattingProperty,value);

            VerifyInlineCount(expectedInlineCount);
            SendInputToVerifyBinding();
        }

        private void SendInputToVerifyBinding()
        {
            SendInput(_richTextbox, "^{HOME}123", "Typing 123 at beginning of the left portion of split RUN to verify if binding is preserved.");
            VerifyInlineText();

            SendInput(_textbox, "^{HOME}A", "Typing A at the begining of the textbox");
            VerifyInlineText();

            SendInput(_textbox, "^{END}Z", "Typing Z at end of the textbox");
            VerifyInlineText();

            SendInput(_textbox, "^{HOME}{RIGHT 3}abc", "Typing abc in the middle of the textbox");
            VerifyInlineText();
        }       

        private void SendInput(TextBoxBase sourceName, string inputString, string inputDescription)
        {
            Log(inputDescription);
            sourceName.Focus();
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();
            KeyboardInput.TypeString(inputString);
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(delayTimeToBind);
        }

        private void VerifyBlockCount()
        {
            int count;
            count = _richTextbox.Document.Blocks.Count;
            Log("Actual number of blocks in the FlowDocument : " + count);
            Verifier.Verify(count == 2, "Verifying that the number of blocks in the flowdocument = 2.", true);          
        }

        private void VerifyInlineCount(int expectedInlineCount)
        {
            Paragraph paragraph1;
            int count;
            paragraph1 = (Paragraph)_richTextbox.Document.Blocks.FirstBlock;
            count = paragraph1.Inlines.Count;
            Log("Actual number of inlines in the FlowDocument : " + count);
            Verifier.Verify(count == expectedInlineCount, "Verifying that the number of inlines in the flowdocument = " + expectedInlineCount, true);           
        }

        private void VerifyInlineText()
        {
            Paragraph paragraph1;
            string str;
            paragraph1 = (Paragraph)_richTextbox.Document.Blocks.FirstBlock;
            str = ((Run)paragraph1.Inlines.FirstInline).Text;           
            Verifier.Verify(str == _textbox.Text, "Verifying that contents of textbox and bound run are same. Actual :: Text in textbox ["+_textbox.Text+"] Expected :: Text in FlowDocument : " + str, true);
        }

        private void SetInitialContent()
        {
            Run r = new Run();
            // Add Content
            _textbox.Text = "Discover";                            
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
        private const int delayTimeToBind = 2000;

        #endregion
    }
}