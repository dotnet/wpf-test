// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Test suite for TextFlow with incremental content changes. 
//
//

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace DRT
{
    // ----------------------------------------------------------------------
    // Test suite for TextFlow with incremental content changes.
    // ----------------------------------------------------------------------
    internal sealed class TextPanelIncrementalSimpleSuite : LayoutSuite
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        internal TextPanelIncrementalSimpleSuite() : base("TextPanelIncrementalSimple")
        {
            this.Contact = "Microsoft";
        }

        // ------------------------------------------------------------------
        // Initialize tests.
        // ------------------------------------------------------------------
        protected override DrtTest[] CreateTests()
        {
            // Return the lists of tests to run against the tree
            return new DrtTest[] {
                new DrtTest(CreateEmpty),                   new DrtTest(VerifyLayoutCreate),
                new DrtTest(AddText),                       new DrtTest(VerifyLayoutAppend),
                new DrtTest(AppendText),                    new DrtTest(VerifyLayoutAppend),
                new DrtTest(ReplaceText),                   new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertTextBefore),              new DrtTest(VerifyLayoutAppend),
                new DrtTest(RemoveInline),                  new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertInline),                  new DrtTest(VerifyLayoutAppend),
                new DrtTest(ChangeLastTextPara),            new DrtTest(VerifyLayoutAppend),
                new DrtTest(ChangeSection),                 new DrtTest(VerifyLayoutAppend),
                new DrtTest(ChangeTextParas),               new DrtTest(VerifyLayoutAppend),
                new DrtTest(AddControl),                    new DrtTest(VerifyLayoutAppend),
                new DrtTest(ModifyControl),                 new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertLineBreak),               new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertLineBreakAndShift),       new DrtTest(VerifyLayoutAppend),
                new DrtTest(ClearAll),                      new DrtTest(VerifyLayoutAppend),
                new DrtTest(Case1AddTextAndInlineObject),    new DrtTest(VerifyLayoutAppend),
                new DrtTest(Case1InsertContainerBetween),    new DrtTest(VerifyLayoutAppend),
                new DrtTest(ClearAll),                      new DrtTest(VerifyLayoutAppend),
                new DrtTest(Case2AddTextAndUIPara),          new DrtTest(VerifyLayoutAppend),
                new DrtTest(Case2InsertContainerBetween),    new DrtTest(VerifyLayoutAppend),
                new DrtTest(ClearAll),                      new DrtTest(VerifyLayoutAppend),
                new DrtTest(Case3ReplaceFirstAndLast),       new DrtTest(VerifyLayoutAppend),
                new DrtTest(ClearAll),                      new DrtTest(VerifyLayoutAppend),
                new DrtTest(Case4AddEmptyAndTextPara),       new DrtTest(VerifyLayoutAppend),
                new DrtTest(Case4InsertEmptyParas),          new DrtTest(VerifyLayoutAppend),
                new DrtTest(Case4InsertEmptyParas),          new DrtTest(VerifyLayoutAppend),
                new DrtTest(Case4RemoveEmptyParas),          new DrtTest(VerifyLayoutAppend),
                new DrtTest(Case4RemoveEmptyParas),          new DrtTest(VerifyLayoutFinalize),
            };
        }

        // ------------------------------------------------------------------
        // Load simple content.
        // ------------------------------------------------------------------
        private void CreateEmpty()
        {
            _testName = "";

            _fdsv = new FlowDocumentScrollViewer();
            _fdsv.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            _fdsv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            _fdsv.Document = new FlowDocument(new Paragraph(new Run()));
            _fdsv.Document.TextAlignment = TextAlignment.Left;
            _fdsv.Document.PagePadding = new Thickness(0);
            _contentRoot.Child = _fdsv;
            _block1 = null;
            _block2 = null;
            _inline1 = null;
            _text = null;
            _border = null;
        }

        // ------------------------------------------------------------------
        // AddText
        // ------------------------------------------------------------------
        private void AddText()
        {
            _fdsv.Document.Blocks.LastBlock.ContentEnd.InsertTextInRun("Text paragraph");
        }

        // ------------------------------------------------------------------
        // AppendText
        // ------------------------------------------------------------------
        private void AppendText()
        {
            _fdsv.Document.Blocks.LastBlock.ContentEnd.InsertTextInRun(" with some additinal content.");
        }

        // ------------------------------------------------------------------
        // ReplaceText
        // ------------------------------------------------------------------
        private void ReplaceText()
        {
            new TextRange(_fdsv.Document.ContentStart, _fdsv.Document.ContentEnd).Text = "";
            _fdsv.Document.Blocks.LastBlock.ContentEnd.InsertTextInRun("Another text paragraph in the place of old one.");
        }

        // ------------------------------------------------------------------
        // InsertTextBefore
        // ------------------------------------------------------------------
        private void InsertTextBefore()
        {
            TextPointer position = _fdsv.Document.ContentStart.GetPositionAtOffset(0, LogicalDirection.Forward);

            // Insert text at the beginning
            position.InsertTextInRun("Note: ");
            if (position != null)
            {
                throw new Exception("TextPanelIncrementSimpleSuite.InsertTextBefore: Cannot be here as previous line must throw an exception");
            }

            // Insert element after just inserted text.
            _inline1 = new Run("This is cool! ", position);
            _inline1.Foreground = Brushes.DarkRed;
            // Insert blank inline element.
            new Run(String.Empty, position);
        }

        // ------------------------------------------------------------------
        // RemoveInline
        // ------------------------------------------------------------------
        private void RemoveInline()
        {
            _inline1.SiblingInlines.Remove(_inline1);
            _inline1 = null;
        }

        // ------------------------------------------------------------------
        // InsertInline
        // ------------------------------------------------------------------
        private void InsertInline()
        {
            _inline1 = new Run(String.Empty, _fdsv.Document.Blocks.FirstBlock.ContentStart);
            _inline1.ContentEnd.InsertTextInRun("\"Our mission is to enable people and businesses throughout the world to ");
            _inline1.ContentEnd.InsertTextInRun("realize their full potential. But our mission is not just about building ");
            _inline1.ContentEnd.InsertTextInRun("great technology. It's also about who we are as a company and as individuals, ");
            _inline1.ContentEnd.InsertTextInRun("how we manage our business internally, and how we think about and work with ");
            _inline1.ContentEnd.InsertTextInRun("partners and customers,\" Ballmer writes in his all-company e-mail.");
            _inline1.Foreground = Brushes.DarkBlue;
            _inline1.Background = Brushes.Beige;
        }

        // ------------------------------------------------------------------
        // ChangeLastTextPara
        // ------------------------------------------------------------------
        private void ChangeLastTextPara()
        {
            _block1 = _fdsv.Document.Blocks.LastBlock;
            new TextRange(_block1.ElementEnd, _fdsv.Document.ContentEnd).Text = "";
            _fdsv.Document.Blocks.LastBlock.ContentEnd.InsertTextInRun("Everyone at Microsoft should be running the latest versions of Office 2000 or ");
            _fdsv.Document.Blocks.LastBlock.ContentEnd.InsertTextInRun("Office XP. Beginning Monday, anyone not running at least the latest version of ");
            _fdsv.Document.Blocks.LastBlock.ContentEnd.InsertTextInRun("Outlook 2000 or Outlook 2002 will be unable to connect to their corporate e-mail");
        }

        // ------------------------------------------------------------------
        // ChangeSection
        // ------------------------------------------------------------------
        private void ChangeSection()
        {
            _block1.ContentStart.InsertTextInRun("[");
            _block1.ContentEnd.InsertTextInRun("]");
        }

        // ------------------------------------------------------------------
        // ChangeTextParas
        // ------------------------------------------------------------------
        private void ChangeTextParas()
        {
            _fdsv.Document.Blocks.FirstBlock.ContentStart.InsertTextInRun("#");
            _fdsv.Document.Blocks.LastBlock.ContentEnd.InsertTextInRun("]");
        }

        // ------------------------------------------------------------------
        // AddControl
        // ------------------------------------------------------------------
        private void AddControl()
        {
            _border = new Border();
            _fdsv.Document.Blocks.Add(new Paragraph(new InlineUIContainer(_border)));
            _border.Background = Brushes.LightGreen;
            //TextFlow.SetFlowBehavior(_border, FlowBehavior.Block);
            _border.Child = _text = new TextBlock();
            _text.Text = "Nested Text";
        }

        // ------------------------------------------------------------------
        // ModifyControl
        // ------------------------------------------------------------------
        private void ModifyControl()
        {
            _text.Text += " has been just changed.";
        }

        // ------------------------------------------------------------------
        // InsertLineBreak
        // ------------------------------------------------------------------
        private void InsertLineBreak()
        {
            new LineBreak(_block1.ContentStart);
        }

        // ------------------------------------------------------------------
        // InsertLineBreakAndShift
        // ------------------------------------------------------------------
        private void InsertLineBreakAndShift()
        {
            new LineBreak(_block1.ContentStart);
            _block1.ContentStart.InsertTextInRun("...");
        }

        // ------------------------------------------------------------------
        // ClearAll
        // ------------------------------------------------------------------
        private void ClearAll()
        {
            _block1 = null;
            _block2 = null;
            _inline1 = null;
            _text = null;
            _border = null;
            new TextRange(_fdsv.Document.ContentStart, _fdsv.Document.ContentEnd).Text = "";
        }

        // ------------------------------------------------------------------
        // Case1AddTextAndInlineObject
        // ------------------------------------------------------------------
        private void Case1AddTextAndInlineObject()
        {
            _block1 = new Section();
            _fdsv.Document.Blocks.Add(_block1);
            _inline1 = new Run("inline", _block1.ContentEnd);
            ((Section)_block1).Blocks.Add(new Paragraph(new InlineUIContainer(_border = new Border())));

            _border.Background = Brushes.DarkGreen;
            _border.Width = 20;
            _border.Height = 20;
        }

        // ------------------------------------------------------------------
        // Case1InsertContainerBetween
        // ------------------------------------------------------------------
        private void Case1InsertContainerBetween()
        {
#if false 
            // Find the text position right before _border
            // Insert Section before Border
            _block2 = new Section(new Paragraph(new Run("block")));
            _inline1.SiblingInlines.InsertAfter(_inline1, _block2);
#endif
        }

        // ------------------------------------------------------------------
        // Case2AddTextAndUIPara
        // ------------------------------------------------------------------
        private void Case2AddTextAndUIPara()
        {
#if false 
            _block1 = new Section(new Paragraph());
            _fdsv.Document.Blocks.Add(_block1);
            _inline1 = new Run("inline");
            ((Section)_block1).Blocks.Adds(_inline1);
            _border = new Border();
            ((Section)_block1).Blocks.Add(new Paragraph(new InlineUIContainer(_border)));

            _border.Background = Brushes.DarkGreen;
            _border.Width = 20;
            _border.Height = 20;
           // TextFlow.SetFlowBehavior(_border, FlowBehavior.Block);
#endif
        }

        // ------------------------------------------------------------------
        // Case2InsertContainerBetween
        // ------------------------------------------------------------------
        private void Case2InsertContainerBetween()
        {
            Case1InsertContainerBetween();
        }

        // ------------------------------------------------------------------
        // Case3ReplaceFirstAndLast
        // ------------------------------------------------------------------
        private void Case3ReplaceFirstAndLast()
        {
            _block1.SiblingBlocks.Remove(_block1);
            _block1 = new Section(new Paragraph(new Run("New 1st paragraph.")));
            _fdsv.Document.Blocks.FirstBlock.SiblingBlocks.InsertBefore(_fdsv.Document.Blocks.FirstBlock, _block1);

            _block2.SiblingBlocks.Remove(_block2);
            _block2 = new Section(new Paragraph(new Run("New 3rd paragraph.")));
            _fdsv.Document.Blocks.Add(_block2);
        }

        // ------------------------------------------------------------------
        // Case4AddEmptyAndTextPara
        // ------------------------------------------------------------------
        private void Case4AddEmptyAndTextPara()
        {
            _block1 = new Section(new Paragraph(new Run()));
            _fdsv.Document.Blocks.Add(_block1);
            _block2 = new Section(new Paragraph(new Run("Text paragraph.")));
            _fdsv.Document.Blocks.Add(_block2);
        }

        // ------------------------------------------------------------------
        // Case4InsertEmptyParas
        // ------------------------------------------------------------------
        private void Case4InsertEmptyParas()
        {
            _fdsv.Document.Blocks.Add(new Section(new Paragraph(new Run())));
            _fdsv.Document.Blocks.Add(new Section(new Paragraph(new Run())));
        }

        // ------------------------------------------------------------------
        // Case4RemoveEmptyParas
        // ------------------------------------------------------------------
        private void Case4RemoveEmptyParas()
        {
            TextPointer position = _fdsv.Document.ContentStart.GetNextContextPosition(LogicalDirection.Forward);
            ((Paragraph)position.Parent).SiblingBlocks.Remove((Paragraph)position.Parent);

            position = position.GetNextContextPosition(LogicalDirection.Forward);
            ((Paragraph)position.Parent).SiblingBlocks.Remove((Paragraph)position.Parent);
        }

        // ------------------------------------------------------------------
        // Private fields.
        // ------------------------------------------------------------------
        private FlowDocumentScrollViewer _fdsv;
        private Block _block1, _block2;
        private Run _inline1;
        private TextBlock _text;
        private Border _border;
    }
}
