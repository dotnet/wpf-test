// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides coverage to verify 1 way binding in single bound run

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
    /// Test verifies 1 way binding in single bound run
    /// FlowDocument Structure 
    /// <paragraph><Run2>Two</Run2></paragraph>
    /// TextBlock structure <Run1>ONE</Run1>
    /// Run2 bound to textbox2
    /// Run1 bound to textbox1
    /// One source bound to single run
    /// </summary>
    [Test(0, "DataBoundRun", "OneWayBindingInSingleBoundRun", MethodParameters = "/TestCaseType:OneWayBindingInSingleBoundRun", Timeout = 200)]
    public class OneWayBindingInSingleBoundRun : CustomTestCase
    {
        #region Main flow

        /// <summary>Starts the combination</summary>
        public override void RunTestCase()
        {
            StackPanel panel = new StackPanel();
            _textblock = new TextBlock();
            _textbox1 = new TextBox();
            _textbox2 = new TextBox();
            _richTextbox = new RichTextBox();
            panel.Children.Add(_textbox1);
            panel.Children.Add(_textbox2);
            panel.Children.Add(_textblock);
            panel.Children.Add(_richTextbox);
            MainWindow.Content = panel;

            QueueDelegate(TestVariations);
        }

        private void TestVariations()
        {
            // Set initial content 
            SetInitialContent();

            // Verify Binding - Modifying the textbox content
            // Verify binding in Textblock
            PerformTestAction("Set textbox1 content as Update1.", "Update1", _textbox1, false);
            VerifyBinding("Update1");

            // Update the text in the textbox2
            // Verify binding in FlowDocument
            PerformTestAction("Set textbox2 content as Update2.", "Update2", _textbox2, false);
            VerifyBinding(null);

            // Verify Binding - Typing Variations
            // Verify binding in FlowDocument
            _textbox2.Text = "";
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();

            PerformTestAction("Typing 22 at the begining in textbox2", "^{HOME}22", _textbox2, true);
            VerifyBinding(null);

            PerformTestAction("Typing 33 at the end in textbox2", "^{END}33", _textbox2, true);
            VerifyBinding(null);

            PerformTestAction("Typing 44 in between content of textbox2", "^{END}{LEFT 1}44", _textbox2, true);
            VerifyBinding(null);
            
            _textbox1.Text = "";
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(2000);

            // Verify binding in TextBlock
            PerformTestAction("Typing 55 at the begining in textbox1", "^{HOME}55", _textbox1, true);
            VerifyBinding("55");

            PerformTestAction("Typing 66 at the end in textbox2", "^{END}66", _textbox1, true);
            VerifyBinding("5566");

            PerformTestAction("Typing 77 in between content of textbox1", "^{HOME}{RIGHT 1}77", _textbox1, true);
            VerifyBinding("577566");           

            Logger.Current.ReportSuccess();
        }

        #endregion

        #region Helpers

        private void PerformTestAction(string inputDescription, string inputString, TextBoxBase inputTarget, bool performTypingVariation)
        {
            Log(inputDescription);
            // Typing variation
            if (performTypingVariation)
            {
                inputTarget.Focus();
                Microsoft.Test.Threading.DispatcherHelper.DoEvents();
                KeyboardInput.TypeString(inputString);
            }
            else
            {
                TextBox tb = (TextBox)inputTarget;
                tb.Text = inputString;
            }
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(delayTimeToBind);
        }   

        private void VerifyBinding(string expectedTextInTextBlock)
        {
            // Verify Binding in Textblock
            if (expectedTextInTextBlock != null)
            {
                Verifier.Verify(expectedTextInTextBlock.Equals(_textblock.Text), "Verifying that contents of TextBlock [" + _textblock.Text + "] are same as [" + expectedTextInTextBlock + "]", true);             
            }
            else
            { 
                // Verify binding in FlowDocument
                Paragraph paragraph1;
                string str;

                paragraph1 = (Paragraph)_richTextbox.Document.Blocks.FirstBlock;
                str = ((Run)paragraph1.Inlines.FirstInline).Text;
                Verifier.Verify(str.Equals(_textbox2.Text), "Verifying that contents of textbox [" + _textbox2.Text + "] and bound run [" + str + "] are same.", true);                
            }
        }

        private void SetInitialContent()
        {

            Run r1 = new Run();
            Run r2 = new Run();
            _textbox1.Text = "One";
            _textbox2.Text = "Two";

            _richTextbox.Height = 100;
            _richTextbox.Width = 300;
            _richTextbox.Document.Blocks.Clear();
            Paragraph para1 = new Paragraph();
            para1.Inlines.Add(r2);
            _richTextbox.Document.Blocks.Add(para1);

            _textblock.Inlines.Add(r1);

            // Bind textbox1 text property to Run1
            Binding binding1 = new Binding("Text");
            binding1.Source = _textbox1;
            binding1.Mode = BindingMode.OneWay;
            r1.SetBinding(Run.TextProperty, binding1);

            // Bind textbox1 text property to Run2
            Binding binding2 = new Binding("Text");
            binding2.Source = _textbox2;
            binding2.Mode = BindingMode.OneWay;
            r2.SetBinding(Run.TextProperty, binding2);
        }

        #endregion Helpers

        #region Private fields

        private RichTextBox _richTextbox;
        private TextBlock _textblock;
        private TextBox _textbox1,_textbox2;
        private const int delayTimeToBind = 2000;

        #endregion
    }
}