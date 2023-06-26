// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides coverage to verify 1 way binding
//  between adjacent bound run and an undbound run

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
    /// Test verifies 1 way binding between adjacent bound run and an undbound run
    /// Flowdocument structure
    /// <paragraph><run1>sometext</run1><run2>sometext</run2></paragraph>
    /// <paragraph><run3>sometext</run3><run4>sometext</run4></paragraph>
    /// TextBlock1 structure <run5>sometext</run5><run6>sometext</run6>
    /// TextBlock2 structure <run7>sometext</run7><run8>sometext</run8>
    /// run6, run7, run2, run3- bound to textbox
    /// run1, run4, run5, run8 - not bound  
    /// Multiple runs bound to the same sorce
    /// </summary>
    [Test(0, "DataBoundRun", "OneWayBoundUnboundRunCombinations", MethodParameters = "/TestCaseType:OneWayBoundUnboundRunCombinations", Timeout = 200)]
    public class OneWayBoundUnboundRunCombinations : CustomTestCase
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

            // Verify Binding - Modifying the textbox content
            // Verify Binding in FlowDocument and TextBlocks
            PerformTestAction("Setting textbox content as Two.", "Two", false);
            VerifyBinding("OneTwo", "TwoFour");

            PerformTestAction("Setting textbox content as Three.", "Three", false);
            VerifyBinding("OneThree", "ThreeFour");

            // Verify Binding - Typing variations
            _textbox.Text = "";
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();

            // Verify Binding in FlowDocument and TextBlocks
            PerformTestAction("Typing 2 at the beginning in textbox.", "^{HOME}2", true);
            VerifyBinding("One2", "2Four");

            PerformTestAction("Typing 3 at the end of the textbox.", "^{END}3", true);
            VerifyBinding("One23", "23Four");

            PerformTestAction("Typing 1 in between the contents of the textbox.", "^{HOME}{RIGHT 1}1", true);
            VerifyBinding("One213", "213Four");

            Logger.Current.ReportSuccess();
        }

        #endregion

        #region Helpers

        private void PerformTestAction(string inputDescription, string inputString, bool performTypingVariation)
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
            { 
               _textbox.Text = inputString;
            }
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(delayTimeToBind);
        }

        private void VerifyBinding(string expectedTextInTextBlock1, string expectedTextInTextBlock2)
        {
            // Verify binding in flow document
            Paragraph paragraph1;
            string str;

            paragraph1 = (Paragraph)_richTextbox.Document.Blocks.FirstBlock;           
            str = ((Run)paragraph1.Inlines.LastInline).Text;
            Log("Text in Textbox [" + _textbox.Text + "]");
            Log("Text in LastInline of first block [" + str + "]");
            Verifier.Verify(str.Equals(_textbox.Text), "Verifying that contents of textbox and bound run are same.", true);

            str = ((Run)paragraph1.Inlines.FirstInline).Text;
            Verifier.Verify(str.Equals("One"), "Verifying that contents of the firstinline in first block are not changed.", true);

            paragraph1 = (Paragraph)_richTextbox.Document.Blocks.LastBlock;            
            str = ((Run)paragraph1.Inlines.FirstInline).Text;
            Log("Text in Textbox [" + _textbox.Text + "]");
            Log("Text in FirstInline of last block [" + str + "]");
            Verifier.Verify(str.Equals(_textbox.Text), "Verifying that contents of textbox and bound run are same.", true);

            str = ((Run)paragraph1.Inlines.LastInline).Text;
            Verifier.Verify(str.Equals("Four"), "Verifying that contents of the lastinline of the lastblock are not changed.", true);

            //Verify binding in TextBlocks
            Verifier.Verify(expectedTextInTextBlock1.Equals(_textblock1.Text), "Verifying that contents of TextBlock [" + _textblock1.Text + "] are same as [" + expectedTextInTextBlock1 + "]", true);
            Verifier.Verify(expectedTextInTextBlock2.Equals(_textblock2.Text), "Verifying that contents of TextBlock [" + _textblock2.Text + "] are same as [" + expectedTextInTextBlock2 + "]", true);
        }

        private void SetInitialContent()
        {
            Run r1 = new Run();
            Run r2 = new Run();
            Run r3 = new Run();
            Run r4 = new Run();
            Run r5 = new Run();
            Run r6 = new Run();
            Run r7 = new Run();
            Run r8 = new Run();
            r1.Text = "One";
            r4.Text = "Four";
            r5.Text = "One";
            r8.Text = "Four";
            
            _textbox.Text = "One";
            _richTextbox.Document.Blocks.Clear();
            Paragraph para1 = new Paragraph();
            para1.Inlines.Add(r1);
            para1.Inlines.Add(r2);
            _richTextbox.Document.Blocks.Add(para1);
            Paragraph para2 = new Paragraph();
            para2.Inlines.Add(r3);
            para2.Inlines.Add(r4);
            _richTextbox.Document.Blocks.Add(para2);

            _textblock1.Inlines.Add(r5);
            _textblock1.Inlines.Add(r6);
            _textblock2.Inlines.Add(r7);
            _textblock2.Inlines.Add(r8);
            // Bind textbox text property to Flowdocument Run
            Binding binding = new Binding("Text");
            binding.Mode = BindingMode.OneWay;
            binding.Source = _textbox;
            r2.SetBinding(Run.TextProperty, binding);
            r3.SetBinding(Run.TextProperty, binding);
            r6.SetBinding(Run.TextProperty, binding);
            r7.SetBinding(Run.TextProperty, binding);
        }

        #endregion Helpers

        #region Private fields

        private RichTextBox _richTextbox;
        private TextBox _textbox;
        private TextBlock _textblock1,_textblock2;
        private const int delayTimeToBind = 2000;

        #endregion
    }
}