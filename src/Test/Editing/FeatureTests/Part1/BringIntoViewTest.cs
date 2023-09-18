// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides coverage for BringIntoView() for FrameworkContentElement
//  (sampled: Paragraph) in RichTextBox

using System.Windows.Controls;
using System.Windows.Documents;
using Media = System.Windows.Media;

using Microsoft.Test;
using Microsoft.Test.Discovery;

using Test.Uis.Loggers;
using Test.Uis.TestTypes;
using Test.Uis.Wrappers;

namespace Test.Uis.TextEditing
{    
    /// <summary>
    /// This test case tests BringIntoView on FrameworkContentElements. Its tested on Paragraph element.
    /// Regression coverage for TFS Part1 Regression_Bug73
    /// </summary>
    [Test(2, "Controls", "BringIntoViewTest", MethodParameters = "/TestCaseType:BringIntoViewTest")]
    public class BringIntoViewTest : CustomTestCase
    {
        #region Main flow

        /// <summary>Starts the combination</summary>
        public override void RunTestCase()
        {
            _rtb = new RichTextBox();
            _rtb.Height = 100;
            _rtb.Width = 300;
            _rtb.FontSize = testFontSize;
            _rtb.FontFamily = new Media.FontFamily("Tahoma");
            _rtb.AcceptsReturn = true;
            _rtb.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;

            // Add content            
            _rtb.Document.Blocks.Clear();
            for (int i = 0; i < numberOfLines; i++)
            {
                _rtb.Document.Blocks.Add(new Paragraph(new Run("Line" + i.ToString())));
            }                        

            _panel = new StackPanel();
            _panel.Children.Add(_rtb);
            MainWindow.Content = _panel;            

            QueueDelegate(AfterLoad); 
        }

        private void AfterLoad()
        {
            _rtb.Focus();

            // Wait for focus
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();

            // Scroll to end so that we can call BringIntoView on the 1st paragraph
            _rtb.ScrollToEnd();
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();

            double initialVerticalOffset = _rtb.VerticalOffset;
            Log("Initial vertical offset: " + initialVerticalOffset);

            Paragraph paragraph1 = (Paragraph)_rtb.Document.Blocks.FirstBlock;
            paragraph1.BringIntoView();
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();

            double finalVerticalOffset = _rtb.VerticalOffset;
            Log("Final vertical offset: " + finalVerticalOffset);

            Verifier.Verify(finalVerticalOffset != initialVerticalOffset, "Vertical offset should change after calling BringIntoView()", true);
            Verifier.Verify(finalVerticalOffset == 0, "Vertical offset should be 0 after the 1st paragraph calls BringIntoView()", true);

            Logger.Current.ReportSuccess();
        }        

        #endregion

        #region Private fields
        
        private RichTextBox _rtb;        
        private StackPanel _panel;

        private const double testFontSize = 24;
        private const int numberOfLines = 10;

        #endregion
    }
}
