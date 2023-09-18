// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides coverage to verify 2 way binding
//  in adjacent bound run and an undbound run

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
    /// Test verifies 2 way binding between adjacent bound run and an undbound run
    ///  Flowdocument structure
    /// <paragraph><run1>SomeText</run1><run2>SomeText</run2></paragraph>
    /// <paragraph><run3>SomeText</run3><run4>SomeText</run4></paragraph>
    /// TextBlock1 structure <run5>SomeText</run5><run6>SomeText</run6>
    /// TextBlock2 structure <run7>SomeText</run7><run8>SomeText</run8>
    /// run6, run7, run2, run3 - bound to textbox
    /// run1, run4 - not bound  
    /// Multiple runs bound to one source
    /// </summary>
    [Test(0, "DataBoundRun", "TwoWayBoundUnboundRunCombinations", MethodParameters = "/TestCaseType:TwoWayBoundUnboundRunCombinations", Timeout = 200)]
    public class TwoWayBoundUnboundRunCombinations : CustomTestCase
    {
        #region Main flow

        /// <summary>Starts the combination              
        /// </summary>
        public override void RunTestCase()
        {
            _richTextbox = new RichTextBox();
            _richTextbox.Height = 100;
            _richTextbox.Width = 300;
            _textbox = new TextBox();
            _textblock1 = new TextBlock();
            _textblock2 = new TextBlock();
            _r6 = new Run();
            _r2 = new Run();
            _r3 = new Run();
            _r7 = new Run();
            StackPanel panel = new StackPanel();
            panel.Children.Add(_textbox);
            panel.Children.Add(_richTextbox);
            panel.Children.Add(_textblock1);
            panel.Children.Add(_textblock2);
            MainWindow.Content = panel;

            QueueDelegate(TestVariations);
        }

        private void TestVariations()
        {
            SetInitialContent();

            // Verify binding - Modifying text property of the textbox
            // Verify Binding in Flow Document and text blocks 
            PerformTestAction("Set textbox.text as Two.", "Two", null, false);
            VerifyBinding("OneTwo", "TwoFour", null);

            PerformTestAction("Set textbox.text as Three.", "Three", null, false);
            VerifyBinding("OneThree", "ThreeFour", null);

            // Verify binding - Typing variations
            _textbox.Text = "";
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();

            // Verify Binding in Flow Document and text blocks 
            PerformTestAction("Typing 2 at the beginning of textbox content.", "^{HOME}2", null, true);
            VerifyBinding("One2", "2Four", null);

            PerformTestAction("Typing 3 at the end of the textbox content.", "^{END}3", null, true);
            VerifyBinding("One23", "23Four", null);

            PerformTestAction("Typing 1 in between the textbox content.", "^{HOME}{RIGHT 1}1", null, true);
            VerifyBinding("One213", "213Four", null);
            
            // Verify binding - Modifying the run.text through property system
            _textbox.Text = "";
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();

            // Verify Binding in Flow Document, text blocks and textbox
            PerformTestAction("Set run.text in flowdocument as Update1", "Update1", _r2, false);
            VerifyBinding("OneUpdate1", "Update1Four", "Update1");

            PerformTestAction("Set run.text in flowdocument as Update2", "Update2", _r3, false);
            VerifyBinding("OneUpdate2", "Update2Four", "Update2");

            PerformTestAction("Set run.text in textblock1 as Update3", "Update3", _r6, false);
            VerifyBinding("OneUpdate3", "Update3Four", "Update3");

            PerformTestAction("Set run.text in textblock2 as Update4", "Update4", _r7, false);
            VerifyBinding("OneUpdate4", "Update4Four", "Update4");           

            Logger.Current.ReportSuccess();
        }

        #endregion

        #region Helpers

        private void PerformTestAction(string inputDescription, string inputString, Run run, bool performTypingVariation)
        {
            Log(inputDescription);
            // Typing variation
            if (performTypingVariation)
            {
                _textbox.Focus();
                Microsoft.Test.Threading.DispatcherHelper.DoEvents();
                KeyboardInput.TypeString(inputString);               
            }
            else
            { // Update text content of run
                if (run != null)
                {
                    run.Text = inputString;
                }
                else
                {
                    _textbox.Text = inputString;
                }
            }
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(delayTimeToBind);
        }        
       
        private void VerifyBinding(string expectedTextInTextBlock1, string expectedTextInTextBlock2, string expectedTextInTextbox)
        {
            // Verify Binding in Flow Document
            Paragraph paragraph1;
            string str;

            paragraph1 = (Paragraph)_richTextbox.Document.Blocks.FirstBlock;           
            str = ((Run)paragraph1.Inlines.LastInline).Text;
            Log("Text in Textbox [" + _textbox.Text + "]");
            Log("Text in last inline of first block [" + str + "]");
            Verifier.Verify(str.Equals(_textbox.Text), "Verifying that contents of textbox and bound run are same.", true);

            str = ((Run)paragraph1.Inlines.FirstInline).Text;
            Verifier.Verify(str.Equals("One"), "Verifying that contents of first inline of first block are not changed.", true);

            paragraph1 = (Paragraph)_richTextbox.Document.Blocks.LastBlock;            
            str = ((Run)paragraph1.Inlines.FirstInline).Text;
            Log("Text in Textbox [" + _textbox.Text + "]");
            Log("Text in first inline of last block [" + str + "]");
            Verifier.Verify(str.Equals(_textbox.Text), "Verifying that contents of textbox and bound run are same.", true);

            str = ((Run)paragraph1.Inlines.LastInline).Text;
            Verifier.Verify(str.Equals("Four"), "Verifying that contents of last inline of the last block are not changed.", true);

            // Verify binding in TextBlocks
            Verifier.Verify(expectedTextInTextBlock1.Equals(_textblock1.Text), "Verifying that contents of TextBlock [" + _textblock1.Text + "] are same as [" + expectedTextInTextBlock1 + "]", true);
            Verifier.Verify(expectedTextInTextBlock2.Equals(_textblock2.Text), "Verifying that contents of TextBlock [" + _textblock2.Text + "] are same as [" + expectedTextInTextBlock2 + "]", true);

            // Verify binding in Textbox
            if (expectedTextInTextbox != null)
            {
                Verifier.Verify(expectedTextInTextbox.Equals(_textbox.Text), "Verifying that contents of textbox Textbox [" + _textbox.Text + "] are same as [" + expectedTextInTextbox + "]", true);
            }
        }

        private void SetInitialContent()
        {            
            Run r1 = new Run();
            Run r4 = new Run();
            Run r5 = new Run();
            Run r8 = new Run();
            r1.Text = "One";
            r4.Text = "Four";
            r5.Text = "One";
            r8.Text = "Four";
            
            _textbox.Text = "One";
            _richTextbox.Document.Blocks.Clear();
            Paragraph para1 = new Paragraph();
            para1.Inlines.Add(r1);
            para1.Inlines.Add(_r2);
            _richTextbox.Document.Blocks.Add(para1);
            Paragraph para2 = new Paragraph();
            para2.Inlines.Add(_r3);
            para2.Inlines.Add(r4);
            _richTextbox.Document.Blocks.Add(para2);

            _textblock1.Inlines.Add(r5);
            _textblock1.Inlines.Add(_r6);
            _textblock2.Inlines.Add(_r7);
            _textblock2.Inlines.Add(r8);

            // Bind textbox text property to Flowdocument Run
            Binding binding = new Binding("Text");
            binding.Source = _textbox;
            binding.Mode = BindingMode.TwoWay;
            _r2.SetBinding(Run.TextProperty, binding);
            _r3.SetBinding(Run.TextProperty, binding);
            _r6.SetBinding(Run.TextProperty, binding);
            _r7.SetBinding(Run.TextProperty, binding);
        }

        #endregion Helpers

        #region Private fields

        private RichTextBox _richTextbox;
        private TextBox _textbox;
        private TextBlock _textblock1,_textblock2;
        private Run _r2,_r3,_r6,_r7;
        private const int delayTimeToBind = 2000;

        #endregion
    }
}