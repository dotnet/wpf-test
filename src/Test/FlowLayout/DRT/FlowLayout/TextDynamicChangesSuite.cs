// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Test suite for Text with dynamic content. 
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
    // Test suite for Text with dynamic content.
    // ----------------------------------------------------------------------
    internal sealed class TextDynamicChangesSuite : LayoutSuite
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        internal TextDynamicChangesSuite() : base("TextDynamicChanges")
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
                //new DrtTest(TextElementMouseEnter),     new DrtTest(VerifyLayoutFinalize),
                new DrtTest(TextElementRestore),        new DrtTest(VerifyLayoutAppend),
                new DrtTest(ITextHostSetup),            new DrtTest(VerifyLayoutAppend),
                new DrtTest(ITextHostTest),             new DrtTest(VerifyLayoutFinalize),
            };
        }

        // ------------------------------------------------------------------
        // Load simple content.
        // ------------------------------------------------------------------
        private void CreateEmpty()
        {
            _testName = "";

            _text = new TextBlock();
            _text.TextWrapping = TextWrapping.WrapWithOverflow;
            _text.FontSize = 11.0;
            _text.FontFamily = new FontFamily("Tahoma");
            _contentRoot.Child = _text;
            _obj = null;
            _inline = null;
        }

        // ------------------------------------------------------------------
        // AppendText
        // ------------------------------------------------------------------
        private void AppendText()
        {
            _text.Inlines.Add(new Run("Append some text. "));
        }

        // ------------------------------------------------------------------
        // RenderProps
        // ------------------------------------------------------------------
        private void RenderProps()
        {
            _text.Foreground = Brushes.RoyalBlue;
        }

        // ------------------------------------------------------------------
        // MeasureProps
        // ------------------------------------------------------------------
        private void MeasureProps()
        {
            _text.FontSize = 14*96.0/72.0;
        }

        // ------------------------------------------------------------------
        // AppendInlineObject
        // ------------------------------------------------------------------
        private void AppendInlineObject()
        {
            if (_obj == null)
            {
                _obj = new TextBlock();
                _obj.Inlines.Add(new Run("[ inline object"));
                _text.Inlines.Add(new InlineUIContainer(_obj));
            }
        }

        // ------------------------------------------------------------------
        // ModifyInlineObject
        // ------------------------------------------------------------------
        private void ModifyInlineObject()
        {
            if (_obj != null)
            {
                _obj.Inlines.Add(new Run(" ]"));
            }
        }

        // ------------------------------------------------------------------
        // InlineObjectRenderProps
        // ------------------------------------------------------------------
        private void InlineObjectRenderProps()
        {
            if (_obj != null)
            {
                _obj.TextDecorations = TextDecorations.Underline;
            }
        }

        // ------------------------------------------------------------------
        // InlineObjectMeasureProps
        // ------------------------------------------------------------------
        private void InlineObjectMeasureProps()
        {
            if (_obj != null)
            {
                _obj.FontSize = 16*96.0/72.0;
            }
        }

        // ------------------------------------------------------------------
        // AppendTextElement
        // ------------------------------------------------------------------
        private void AppendTextElement()
        {
            if (_inline == null)
            {
                _inline = new Run("{ Inline element", _text.ContentEnd);
                _inline.Foreground = Brushes.DarkBlue;
                if (_inline.Parent == null) 
                    throw new InvalidOperationException("_inline has lost its parent - 1");
            }
        }

        // ------------------------------------------------------------------
        // ModifyTextElement
        // ------------------------------------------------------------------
        private void ModifyTextElement()
        {
            if (_inline != null)
            {
                if (_inline.Parent == null)
                    throw new InvalidOperationException("_inline has lost its parent - 2");
                _inline.ContentEnd.InsertTextInRun(" } ");
                if (_inline.Parent == null)
                    throw new InvalidOperationException("_inline has lost its parent - 3");
            }
        }

        // ------------------------------------------------------------------
        // TextElementRenderProps
        // ------------------------------------------------------------------
        private void TextElementRenderProps()
        {
            if (_inline != null)
            {
                if (_inline.Parent == null)
                    throw new InvalidOperationException("_inline has lost its parent - 4");
                _inline.Foreground = Brushes.Red;
                if (_inline.Parent == null)
                    throw new InvalidOperationException("_inline has lost its parent - 5");
            }
        }

        // ------------------------------------------------------------------
        // TextElementMeasureProps
        // ------------------------------------------------------------------
        private void TextElementMeasureProps()
        {
            if (_inline != null)
            {
                if (_inline.Parent == null)
                    throw new InvalidOperationException("_inline has lost its parent - 6");
                _inline.FontWeight = FontWeights.Bold;
                if (_inline.Parent == null)
                    throw new InvalidOperationException("_inline has lost its parent - 7");
            }
        }

        // ------------------------------------------------------------------
        // TextElementMouseEnter
        // ------------------------------------------------------------------
        private void TextElementMouseEnter()
        {
            if (_inline != null)
            {
                if (_inline.Parent == null)
                    throw new InvalidOperationException("_inline has lost its parent - 8");

                // Get position of TextElement
                Rect rect = _inline.ContentStart.GetCharacterRect(LogicalDirection.Forward);
                Point pt = new Point(rect.Left + 20, rect.Top + 5);
                pt = PointUtil.ClientToScreen(pt, PresentationSource.FromVisual(_contentRoot));

                // The mouse needs to be in a well-defined place outside of text content.
                Input.MoveTo(new Point());

                // Add mouse enter handler
                _inline.MouseEnter += s_textElementMouseEnterHandler;

                // Move the mouse over the TextElement.
                Input.MoveTo(pt);

                if (_inline.Parent == null)
                    throw new InvalidOperationException("_inline has lost its parent - 9");
            }
        }

        // ------------------------------------------------------------------
        // TextElementMouseLeave
        // ------------------------------------------------------------------
        private void TextElementRestore()
        {
            if (_inline != null)
            {
                if (_inline.Parent == null)
                    throw new InvalidOperationException("_inline has lost its parent - 10");

                // Change TextElement properties
                _inline.ClearValue(Inline.FontSizeProperty);

                if (_inline.Parent == null)
                    throw new InvalidOperationException("_inline has lost its parent - 11");
            }
        }

        // ------------------------------------------------------------------
        // ITextHostSetup
        // ------------------------------------------------------------------
        private void ITextHostSetup()
        {
            _obj = null;
            _inline = null;

            _text.TextWrapping = TextWrapping.WrapWithOverflow;
            _text.Foreground = Brushes.Black;
            _text.FontSize = 16*96.0/72.0;
            _text.FontFamily = new FontFamily("Tahoma");
            _text.Inlines.Clear();
            _text.Inlines.Add(new Run("before inline object"));
            _text.Inlines.Add(new InlineUIContainer(_obj = new TextBlock()));
            _text.Inlines.Add(new Run("after inline object." + "Wrap to the next line." + "Wrap to the next line." + "Wrap to the next line."));
            _obj.Width = 200;
            _obj.Height = 200;
            _obj.Text = "IO";
            _obj.FontFamily = new FontFamily("Tahoma");
            _obj.FontSize = 24*96.0/72.0;
        }

        // ------------------------------------------------------------------
        // ITextHostTest
        // ------------------------------------------------------------------
        private void ITextHostTest()
        {
            _obj.FontSize = 36*96.0/72.0;
        }

        // ------------------------------------------------------------------
        // TextElementMouseEnterHandler
        // ------------------------------------------------------------------
        private void TextElementMouseEnterHandler(object sender, MouseEventArgs e)
        {
            if (_inline != null)
            {
                if (_inline.Parent == null)
                    throw new InvalidOperationException("_inline has lost its parent - 12");

                // Remove mouse enter handler
                _inline.MouseEnter -= s_textElementMouseEnterHandler;

                // The mouse needs to be in a well-defined place outside of text content.
                Input.MoveTo(new Point());

                // Change TextElement properties
                _inline.FontSize = 36 * 96.0 / 72.0;

                if (_inline.Parent == null)
                    throw new InvalidOperationException("_inline has lost its parent - 13");
            }
        }

        // ------------------------------------------------------------------
        // Private fields.
        // ------------------------------------------------------------------
        private TextBlock _text;
        private TextBlock _obj;
        private Run _inline;

        // ------------------------------------------------------------------
        // Mouse events handlers.
        // ------------------------------------------------------------------
        private static MouseEventHandler s_textElementMouseEnterHandler;
    }
}
