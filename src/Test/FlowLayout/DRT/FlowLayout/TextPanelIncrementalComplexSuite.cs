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
    internal sealed class TextPanelIncrementalComplexSuite : LayoutSuite
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        internal TextPanelIncrementalComplexSuite() : base("TextPanelIncrementalComplex")
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
                new DrtTest(CreateEmpty),       new DrtTest(VerifyLayoutCreate),
                new DrtTest(AddText),           new DrtTest(VerifyLayoutAppend),
                new DrtTest(ReplaceText),       new DrtTest(VerifyLayoutAppend),
                new DrtTest(ModifyTrack),       new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertTextLines),   new DrtTest(VerifyLayoutAppend),
                new DrtTest(DeleteTextLines),   new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertFloaters),    new DrtTest(VerifyLayoutAppend),
                new DrtTest(ReplaceFloaters),   new DrtTest(VerifyLayoutAppend),
                new DrtTest(DeleteParas),       new DrtTest(VerifyLayoutAppend),
                new DrtTest(DeleteFloaters),    new DrtTest(VerifyLayoutAppend),
                new DrtTest(DeleteNestedPara),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(ReInit),            new DrtTest(VerifyLayoutAppend),
                new DrtTest(AddText),           new DrtTest(VerifyLayoutAppend),
                new DrtTest(ReplaceText),       new DrtTest(VerifyLayoutAppend),
                new DrtTest(ModifyTrack),       new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertTextLines),   new DrtTest(VerifyLayoutAppend),
                new DrtTest(DeleteTextLines),   new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertFloaters),    new DrtTest(VerifyLayoutAppend),
                new DrtTest(ReplaceFloaters),   new DrtTest(VerifyLayoutAppend),
                new DrtTest(DeleteParas),       new DrtTest(VerifyLayoutAppend),
                new DrtTest(DeleteFloaters),    new DrtTest(VerifyLayoutAppend),
                new DrtTest(DeleteNestedPara),  new DrtTest(VerifyLayoutFinalize),
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
            _fdsv.Document = new FlowDocument();
            _fdsv.Document.TextAlignment = TextAlignment.Left;
            _fdsv.Document.PagePadding = new Thickness(0);
            s_section1 = new Section();
            _fdsv.Document.Blocks.Add(s_section1);
            s_section2 = new Section();
            _fdsv.Document.Blocks.Add(s_section2);
            s_section3 = new Section();
            _fdsv.Document.Blocks.Add(s_section3);
            s_section4 = new Section();
            _fdsv.Document.Blocks.Add(s_section4);
            _contentRoot.Child = _fdsv;
        }

        // ------------------------------------------------------------------
        // AddText
        // ------------------------------------------------------------------
        private void AddText()
        {
            s_section1.Blocks.Add(new Paragraph(new Run("The 6th ")));
            s_section2.Blocks.Add(new Paragraph(new Run("A man discovers he has been illegally cloned. A man discovers he has been illegally cloned. A man discovers he has")));
            s_section3.Blocks.Add(new Paragraph(new Run("No information available. No information available. No information available. No information available. No information available. No information avail")));
            s_section4.Blocks.Add(new Paragraph(new Run("7:30 PM - 9:20 PM.  7:30 PM - 9:20 PM.  7:30 PM - 9:20 PM.  7:30 PM - 9:20 PM.  7:30 PM - 9:20 PM.  7:30 PM")));
        }

        // ------------------------------------------------------------------
        // ReplaceText
        // ------------------------------------------------------------------
        private void ReplaceText()
        {
            ((Run)((Paragraph)s_section1.Blocks.FirstBlock).Inlines.FirstInline).Text = "Jeopardy ";
            ((Run)((Paragraph)s_section2.Blocks.FirstBlock).Inlines.FirstInline).Text = "Television week preview. Television week preview. Television week preview. Television we";
            ((Run)((Paragraph)s_section3.Blocks.FirstBlock).Inlines.FirstInline).Text = "Only some information available. Only some information available. Only some information available. Only so";
            ((Run)((Paragraph)s_section4.Blocks.FirstBlock).Inlines.FirstInline).Text = "10:12 AM - 2:30 PM. 10:12 AM - 2:30 PM. 10:12 AM - 2:30 PM. 10:12 AM - 2:30 PM. 10";
        }

        // ------------------------------------------------------------------
        // ModifyTrack
        // ------------------------------------------------------------------
        private void ModifyTrack()
        {
            // Replace _section2
            Section section = new Section();
            s_section2.SiblingBlocks.InsertAfter(s_section2, section);
            _fdsv.Document.Blocks.Remove(s_section2);
            s_section2 = section;

            s_section2.Background = Brushes.LightGreen;

            Paragraph paragraph = new Paragraph();
            s_section2.Blocks.Add(paragraph);

            paragraph.Inlines.Add(new Run("TextParagraph with background set on it."));
            paragraph.Inlines.Add(new LineBreak());
            paragraph.Inlines.Add(new Run("New line."));
            paragraph.Inlines.Add(new LineBreak());
            paragraph.Inlines.Add(new Run("new line."));
            paragraph.Inlines.Add(new LineBreak());
            paragraph.Inlines.Add(new Run("And inline element."));
            paragraph.Inlines.Add(new LineBreak());

            // Add anonymous text paragraph after _section2.
            s_section2.Blocks.Add(new Paragraph(new Run("Anonymous text paragraph.")));
        }

        // ------------------------------------------------------------------
        // InsertTextLines
        // ------------------------------------------------------------------
        private void InsertTextLines()
        {
            LineBreak lineBreak = ((Paragraph)s_section2.Blocks.FirstBlock).Inlines.FirstInline.NextInline.NextInline.NextInline.NextInline as LineBreak;
            if (lineBreak == null)
            {
                throw new Exception("TextPanelIncrementComplexSuite.InsertTextLines: LineBreak is expected");
            }

            // Insert one more linebreak and text "And " after this LineBreak
            lineBreak.SiblingInlines.InsertAfter(lineBreak, new Run("And "));
            lineBreak.SiblingInlines.InsertAfter(lineBreak, new LineBreak());

            // Remove this lineBreak
            lineBreak.SiblingInlines.Remove(lineBreak);
        }

        // ------------------------------------------------------------------
        // DeleteTextLines
        // ------------------------------------------------------------------
        private void DeleteTextLines()
        {
            // Remove content from _section2
            TextPointer tn = s_section2.ContentStart;
            tn = tn.GetNextContextPosition(LogicalDirection.Forward); // skip text
            tn = tn.GetNextContextPosition(LogicalDirection.Forward); // skip LineBreak Begin
            tn = tn.GetNextContextPosition(LogicalDirection.Forward); // skip LineBreak End
            tn = tn.GetNextContextPosition(LogicalDirection.Forward); // skip text
            new TextRange(tn, s_section2.ContentEnd).Text = "";
        }

        // ------------------------------------------------------------------
        // InsertFloaters
        // ------------------------------------------------------------------
        private void InsertFloaters()
        {
            // Create a floater1
            Border floater1 = new Border();
            floater1.Background = Brushes.Yellow;
            floater1.Width = 30;
            floater1.Height = 60;
            //TextFlow.SetFlowBehavior(floater1, FlowBehavior.Floating);
            //TextFlow.SetFloatingDirection(floater1, System.Windows.HorizontalAlignment.Left);

            // Create a floater2
            Border floater2 = new Border();
            floater2.Background = Brushes.DarkGreen;
            floater2.Width = 40;
            floater2.Height = 40;
            //TextFlow.SetFlowBehavior(floater2, FlowBehavior.Floating);
            //TextFlow.SetFloatingDirection(floater2, System.Windows.HorizontalAlignment.Center);

            // Insert floater1 to _section2
            /*
            TextPointer tn = _section2.ContentStart.GetNextContextPosition(LogicalDirection.Forward); // skip text
            tn.InsertUIElement(floater1);
            */

            // Append text and floater2 to _section2
            s_section2.ContentEnd.InsertTextInRun("Text Paragraph with some floaters. ");
            s_section2.Blocks.Add(new Paragraph(new InlineUIContainer(floater2)));
            s_section2.ContentEnd.InsertTextInRun("Text Paragraph with some floaters. ");
            s_section2.ContentEnd.InsertTextInRun("Text Paragraph with some floaters. ");
            s_section2.ContentEnd.InsertTextInRun("Text Paragraph with some floaters. ");
            s_section2.ContentEnd.InsertTextInRun("Text Paragraph with some floaters. ");
            s_section2.ContentEnd.InsertTextInRun("Text Paragraph with some floaters. ");
            s_section2.ContentEnd.InsertTextInRun("Text Paragraph with some floaters. ");
            s_section2.ContentEnd.InsertTextInRun("Text Paragraph with some floaters. ");
            s_section2.ContentEnd.InsertTextInRun("Text Paragraph with some floaters. ");
        }

        // ------------------------------------------------------------------
        // ReplaceFloaters
        // ------------------------------------------------------------------
        private void ReplaceFloaters()
        {
            // Create a floater
            Border floater = new Border();
            floater.Background = Brushes.RoyalBlue;
            floater.Width = 100;
            floater.Height = 20;
            //TextFlow.SetFlowBehavior(floater, FlowBehavior.Floating);
            //TextFlow.SetFloatingDirection(floater, System.Windows.HorizontalAlignment.Right);
            // Replace existing floater with new one
            TextPointer tn = s_section2.ContentStart;
            tn = tn.GetNextContextPosition(LogicalDirection.Forward); // skip text
            tn = tn.GetNextContextPosition(LogicalDirection.Forward); // skip Floater
            tn = tn.GetNextContextPosition(LogicalDirection.Forward); // skip LineBreak Begin
            tn = tn.GetNextContextPosition(LogicalDirection.Forward); // skip LineBreak End
            tn = tn.GetNextContextPosition(LogicalDirection.Forward); // skip text
            //TextPointer tn2 = tn;
            //tn2 = tn2.GetPositionAtOffset(+1);
            //new TextRange(tn, tn2).Text = "";
            //tn.InsertUIElement(floater);
            tn.InsertTextInRun("");
            s_section2.Blocks.Add(new Paragraph(new InlineUIContainer(floater)));
        }

        // ------------------------------------------------------------------
        // DeleteParas
        // ------------------------------------------------------------------
        private void DeleteParas()
        {
            // Delete last 2 paragraphs
            s_section4.SiblingBlocks.Remove(s_section4);
            s_section4 = null;
            s_section3.SiblingBlocks.Remove(s_section3);
            s_section3 = null;
            // Shift floaters by removing LineBreak
            TextPointer tn = s_section2.ContentStart;
            tn = tn.GetNextContextPosition(LogicalDirection.Forward); // skip text
            tn = tn.GetNextContextPosition(LogicalDirection.Forward); // skip Floater
            tn = tn.GetNextContextPosition(LogicalDirection.Forward); // skip LineBreak Begin
            // 



            s_section3 = new Section(new Paragraph(new Run()));
            s_section2.Blocks.Add(s_section3);
            s_section3.Background = Brushes.Gray;
            s_section3.Padding = new Thickness(5);
            s_section3.Blocks.FirstBlock.ContentEnd.InsertTextInRun("Nested block element.");
        }

        // ------------------------------------------------------------------
        // DeleteFloaters
        // ------------------------------------------------------------------
        private void DeleteFloaters()
        {
            TextPointer tn = s_section2.ContentStart.GetNextContextPosition(LogicalDirection.Forward); // skip text
            TextPointer tn2 = tn.GetPositionAtOffset(+1);
            new TextRange(tn, tn2).Text = "";
            tn = tn.GetNextContextPosition(LogicalDirection.Forward); // skip text
            tn2 = tn.GetPositionAtOffset(+1);
            new TextRange(tn, tn2).Text = "";
        }

        // ------------------------------------------------------------------
        // DeleteNestedPara
        // ------------------------------------------------------------------
        private void DeleteNestedPara()
        {
            s_section3.SiblingBlocks.Remove(s_section3);
            s_section3 = null;
        }

        // ------------------------------------------------------------------
        // ReInit
        // ------------------------------------------------------------------
        private void ReInit()
        {
            _fdsv.Document.Blocks.Clear();
            s_section1 = new Section();
            s_section1.Blocks.Add(new Paragraph(new Run()));
            s_section2 = new Section(new Paragraph(new Run()));
            _fdsv.Document.Blocks.Add(s_section2);
            s_section3 = new Section(new Paragraph(new Run()));
            _fdsv.Document.Blocks.Add(s_section3);
            s_section4 = new Section(new Paragraph(new Run()));
            _fdsv.Document.Blocks.Add(s_section4);
        }

        // ------------------------------------------------------------------
        // Private fields.
        // ------------------------------------------------------------------
        private FlowDocumentScrollViewer _fdsv;
        private static Section s_section1,s_section2,s_section3,s_section4;
    }
}
