// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Test suite for TextFlow with dynamic content. 
//
//

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using MS.Internal;

namespace DRT
{
    // ----------------------------------------------------------------------
    // Test suite for TextFlow with dynamic content.
    // ----------------------------------------------------------------------
    internal sealed class TextPanelDynamicChangesSuite : LayoutSuite
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        internal TextPanelDynamicChangesSuite() : base("TextPanelDynamicChanges")
        {
            this.Contact = "Microsoft";
            s_textElementMouseEnterHandler = new MouseEventHandler(TextElementMouseEnterHandler);
        }

        // ------------------------------------------------------------------
        // Initialize tests.
        // ------------------------------------------------------------------
        protected override DrtTest[] CreateTests()
        {
            // Return the lists of tests to run against the tree
            return new DrtTest[] {
                new DrtTest(CreateEmpty),               new DrtTest(VerifyLayoutCreate),
                new DrtTest(AppendText),                new DrtTest(VerifyLayoutAppend),
                new DrtTest(AppendInlineObject),        new DrtTest(VerifyLayoutAppend),
                new DrtTest(AppendTextElement),         new DrtTest(VerifyLayoutAppend),
                new DrtTest(AppendText),                new DrtTest(VerifyLayoutAppend),
                new DrtTest(ModifyInlineObject),        new DrtTest(VerifyLayoutAppend),
                new DrtTest(ModifyTextElement),         new DrtTest(VerifyLayoutAppend),
                new DrtTest(RenderProps),               new DrtTest(VerifyLayoutAppend),
                new DrtTest(MeasureProps),              new DrtTest(VerifyLayoutAppend),
                new DrtTest(InlineObjectRenderProps),   new DrtTest(VerifyLayoutAppend),
                new DrtTest(InlineObjectMeasureProps),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(TextElementRenderProps),    new DrtTest(VerifyLayoutAppend),
                new DrtTest(TextElementMeasureProps),   new DrtTest(VerifyLayoutAppend),
                //new DrtTest(TextElementMouseEnter1),    new DrtTest(VerifyLayoutAppend),
                new DrtTest(TextElementRestore),        new DrtTest(VerifyLayoutAppend),
                new DrtTest(ReplaceContent),            new DrtTest(VerifyLayoutAppend),
                new DrtTest(ModifyFirstPara),           new DrtTest(VerifyLayoutAppend),
                new DrtTest(AppendParas),               new DrtTest(VerifyLayoutAppend),
                new DrtTest(ModifySecondPara),          new DrtTest(VerifyLayoutAppend),
                new DrtTest(AppendLastPara),            new DrtTest(VerifyLayoutFinalize),
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
            _obj = null;
            _inline = null;
            _italic = null;
            _inline2 = null;
            _para1 = _para3 = null;
        }

        // ------------------------------------------------------------------
        // AppendText
        // ------------------------------------------------------------------
        private void AppendText()
        {
            ((Paragraph)_fdsv.Document.Blocks.LastBlock).Inlines.Add(new Run("Append some text. "));
        }

        // ------------------------------------------------------------------
        // RenderProps
        // ------------------------------------------------------------------
        private void RenderProps()
        {
            _fdsv.Document.Foreground = Brushes.RoyalBlue;
        }

        // ------------------------------------------------------------------
        // MeasureProps
        // ------------------------------------------------------------------
        private void MeasureProps()
        {
            _fdsv.Document.FontSize = 14*96.0/72.0;
        }

        // ------------------------------------------------------------------
        // AppendInlineObject
        // ------------------------------------------------------------------
        private void AppendInlineObject()
        {
            _obj = new TextBlock(new Run("[ inline object"));
            ((Paragraph)_fdsv.Document.Blocks.LastBlock).Inlines.Add(new InlineUIContainer(_obj));
        }

        // ------------------------------------------------------------------
        // ModifyInlineObject
        // ------------------------------------------------------------------
        private void ModifyInlineObject()
        {
            _obj.Inlines.LastInline.ContentEnd.InsertTextInRun(" ]");
        }

        // ------------------------------------------------------------------
        // InlineObjectRenderProps
        // ------------------------------------------------------------------
        private void InlineObjectRenderProps()
        {
            _obj.TextDecorations = TextDecorations.Underline;
        }

        // ------------------------------------------------------------------
        // InlineObjectMeasureProps
        // ------------------------------------------------------------------
        private void InlineObjectMeasureProps()
        {
            _obj.FontSize = 16*96.0/72.0;
        }

        // ------------------------------------------------------------------
        // AppendTextElement
        // ------------------------------------------------------------------
        private void AppendTextElement()
        {
            _inline = new Run("{ Inline element");
            _inline.Foreground = Brushes.DarkBlue;
            ((Paragraph)_fdsv.Document.Blocks.LastBlock).Inlines.Add(_inline);
        }

        // ------------------------------------------------------------------
        // ModifyTextElement
        // ------------------------------------------------------------------
        private void ModifyTextElement()
        {
            _inline.ContentEnd.InsertTextInRun(" } ");
        }

        // ------------------------------------------------------------------
        // TextElementRenderProps
        // ------------------------------------------------------------------
        private void TextElementRenderProps()
        {
            _inline.Foreground = Brushes.Red;
        }

        // ------------------------------------------------------------------
        // TextElementMeasureProps
        // ------------------------------------------------------------------
        private void TextElementMeasureProps()
        {
            _inline.FontWeight = FontWeights.Bold;
        }

        // ------------------------------------------------------------------
        // TextElementMouseEnter1
        // ------------------------------------------------------------------
        private void TextElementMouseEnter1()
        {
            // Get position of TextElement
            Rect rect = _inline.ContentStart.GetCharacterRect(LogicalDirection.Forward);
            Point pt = new Point(rect.Left + 10, rect.Top + 10);
            pt = PointUtil.ClientToScreen(pt, PresentationSource.FromVisual(_contentRoot));

            // The mouse needs to be in a well-defined place outside of text content.
            Input.MoveTo(new Point());

            // Add mouse enter handler
            _inline.MouseEnter += s_textElementMouseEnterHandler;

            // Move the mouse over the TextElement.
            Input.MoveTo(pt);
        }

        // ------------------------------------------------------------------
        // TextElementMouseLeave
        // ------------------------------------------------------------------
        private void TextElementRestore()
        {
            _inline.ClearValue(Inline.FontSizeProperty);
        }

        // ------------------------------------------------------------------
        // ReplaceContent
        // ------------------------------------------------------------------
        private void ReplaceContent()
        {
            _obj = null;
            _inline = null;

            _fdsv.Document.Blocks.Clear();
            _fdsv.Document.ClearValue(FlowDocument.FontSizeProperty);
            _fdsv.Document.ClearValue(FlowDocument.ForegroundProperty);

            _para1 = new Paragraph(new Run("The next release of the Microsoft Windows operating system, "
                + "code-named 'Longhorn,' is an essential milestone for many reasons. It is the "
                + "first operating system built with managed code and the first to host a new "
                + "storage subsystem (code-named 'WinFS') that revolutionizes the concept of a file "
                + "system. It is also the first operating system to support a natural search technology "
                + "(Natural UI) that automatically resolves many of the ambiguities inherent in query text."));

            _inline2 = new Run("[Inline ");
            _inline2.Foreground = Brushes.DarkGreen;
            _para1.Inlines.Add(_inline2);
            _para1.Inlines.Add(new Run("In addition, Longhorn is the first operating system designed from the ground "
                + "up with security and trustworthy computing at the core. These and other features "
                + "suggest that Longhorn will change the way applications are builtnot something that "
                + "happens every day. Since the advent of Windows, I remember two similar milestonesthe "
                + "move to 32-bit Windows and the dawn of the managed environment of the Microsoft .NET Framework."));

            _fdsv.Document.Blocks.Add(_para1);
        }

        // ------------------------------------------------------------------
        // ModifyFirstPara
        // ------------------------------------------------------------------
        private void ModifyFirstPara()
        {
            _inline2.ContentEnd.InsertTextInRun("text element]");
        }

        // ------------------------------------------------------------------
        // AppendParas
        // ------------------------------------------------------------------
        private void AppendParas()
        {
            Paragraph paragraph = new Paragraph();
            _fdsv.Document.Blocks.Add(paragraph);
            paragraph.Inlines.Add(new Run("One of the most important changes in Longhorn is that this operating "
                + "system makes it possible to write an application one time and use it in "
                + "multiple deployment scenarios. To achieve this ambitious goal, Longhorn-based "
                + "applications are completely object-oriented and based on a central Application "
                + "object that provides all key services that are needed for running the application. "));

            _italic = new Italic(new Run("[Inline text element] "));
            paragraph.Inlines.Add(_italic);
            paragraph.Inlines.Add(new Run("In this article, I'll examine the Longhorn application model in some deapth "
                + "and apply it to a few basic scenarios, including the classic Hello World application."));

            _para3 = new Paragraph();
            _fdsv.Document.Blocks.Add(_para3);
            _para3.Inlines.Add(new Run("The Application object is the heart of the Longhorn application model. Through its set of properties, "
                + "methods, and events, the object enables you to arrange a collection of markup pagesa sort of enhanced "
                + "version of HTMLinto a coherent and classic Windows-based application. The Application object is the "
                + "root application object available in Longhorn. It provides basic application support and will typically "
                + "be used by those applications that need low overhead and don't use page navigation and state management. "
                + "More complex Longhorn applications will use the closely related NavigationApplication object, which "
                + "inherits from Application but adds support for navigation."
                + "The Application object is the heart of the Longhorn application model. Through its set of properties, "
                + "methods, and events, the object enables you to arrange a collection of markup pagesa sort of enhanced "
                + "version of HTMLinto a coherent and classic Windows-based application. The Application object is the "
                + "root application object available in Longhorn. It provides basic application support and will typically "
                + "be used by those applications that need low overhead and don't use page navigation and state management. "
                + "More complex Longhorn applications will use the closely related NavigationApplication object, which "
                + "inherits from Application but adds support for navigation."));

            _inline2.ContentEnd.InsertTextInRun(" ");
        }

        // ------------------------------------------------------------------
        // ModifySecondPara
        // ------------------------------------------------------------------
        private void ModifySecondPara()
        {
            _italic.Foreground = Brushes.DarkGreen;
        }

        // ------------------------------------------------------------------
        // AppendLastPara
        // ------------------------------------------------------------------
        private void AppendLastPara()
        {
            _fdsv.Document.Blocks.Add(new Paragraph(new Run("Code Name Longhorn.")));
        }

        // ------------------------------------------------------------------
        // TextElementMouseEnterHandler
        // ------------------------------------------------------------------
        private void TextElementMouseEnterHandler(object sender, MouseEventArgs e)
        {
            // Remove mouse enter handler
            _inline.MouseEnter -= s_textElementMouseEnterHandler;

            // The mouse needs to be in a well-defined place outside of text content.
            Input.MoveTo(new Point());

            // Change TextElement properties
            _inline.FontSize = 36*96.0/72.0;
        }

        // ------------------------------------------------------------------
        // Private fields.
        // ------------------------------------------------------------------
        private FlowDocumentScrollViewer _fdsv;
        private TextBlock _obj;
        private Run _inline;
        private Italic _italic;
        private Run _inline2;
        private Paragraph _para1, _para3;

        // ------------------------------------------------------------------
        // Mouse events handlers.
        // ------------------------------------------------------------------
        private static MouseEventHandler s_textElementMouseEnterHandler;
    }
}
