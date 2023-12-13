// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Threading;

using Test.Uis.Loggers;
using Test.Uis.TestTypes;
using Test.Uis.Utils;

namespace Test.Uis.TextEditing
{
    /// <summary>
    /// This test case provides regression coverage for TFS Part1 Regression_Bug77. 
    /// Scenario involves Copy->Paste->Undo of TextElement with some properties 
    /// having an expression as a value. The bug was in the copy and extraction code.
    /// </summary>
    [Test(1, "TextOM", "TextElementExtractUndoWithExpressions", MethodParameters = "/TestCaseType:TextElementExtractUndoWithExpressions")]
    public class TextElementExtractUndoWithExpressions : CustomTestCase
    {
        #region Main flow

        /// <summary>Starts the combination</summary>
        public override void RunTestCase()
        {
            _rtb = new RichTextBox();
            _rtb.Height = 100;
            _rtb.Width = 300;
            _rtb.FontSize = 12;

            ResourceDictionary rd = MainWindow.Resources;
            rd.Add("ColorBrush1", new SolidColorBrush(Colors.Red));
            rd.Add("ColorBrush2", new SolidColorBrush(Colors.Green));

            _tr = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);
            XamlUtils.SetXamlContent(_tr, _xamlContent);

            _initialContents = _tr.Text;

            _panel = new StackPanel();
            _panel.Children.Add(_rtb);
            MainWindow.Content = _panel;

            QueueDelegate(AfterLoad); 
        }

        // Performs  the repro steps here.
        private void AfterLoad()
        {            
            // Make a selection such that it contains some text from first second run elements.            
            Paragraph p = (Paragraph)_rtb.Document.Blocks.FirstBlock;
            Run r1 = (Run)p.Inlines.FirstInline;
            Run r2 = (Run)p.Inlines.LastInline;
            TextPointer tp1 = r1.ContentEnd.GetPositionAtOffset(-2);
            TextPointer tp2 = r2.ContentStart.GetPositionAtOffset(2);
            _rtb.Selection.Select(tp1, tp2);
            
            _rtb.Copy();

            // Collapse the selection
            _rtb.Selection.Select(tp2, tp2);

            _rtb.Paste();
            
            Verifier.Verify(_tr.Text != _initialContents, "Verifying that the contents have changed after paste operation", true);
            DispatcherHelper.DoEvents();
            
            _rtb.Undo();

            QueueDelegate(DoVerification);
        }

        private void DoVerification()
        {
            Verifier.Verify(_tr.Text == _initialContents, "Verifying that the contents get back to intial state after undo operation", true);
            Logger.Current.ReportSuccess();
        }

        #endregion

        #region Private fields

        private RichTextBox _rtb = null;
        private StackPanel _panel = null;
        private TextRange _tr = null;
        private string _initialContents = string.Empty;
        private string _finalContents = string.Empty;
        private string _xamlContent = "<Paragraph>" +            
            "<Run Foreground='{DynamicResource ColorBrush1}'>Jumped Over The Moon</Run>" +
            "<Run Foreground='{DynamicResource ColorBrush2}'>Jumped Over The Moon</Run>" +
            "</Paragraph>";

        #endregion
    }
}
