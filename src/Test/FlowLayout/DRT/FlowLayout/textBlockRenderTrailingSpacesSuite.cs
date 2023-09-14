// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Checks rendering of trailing spaces on TextBlock
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;

namespace DRT
{
    // ----------------------------------------------------------------------
    // Trailing spaces rendering suite for TextBlock
    // ----------------------------------------------------------------------
    internal class TextBlockRenderTrailingSpacesSuite : IContentHostSuite
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        internal TextBlockRenderTrailingSpacesSuite()
            : base("TextBlockRenderTrailingSpaces")
        {
            this.Contact = "Microsoft";
            _viewSize = new Size(700, 500);
        }

        // ------------------------------------------------------------------
        // Create initial element tree.
        // ------------------------------------------------------------------
        protected override UIElement CreateTree()
        {
            _textBlock = new TextBlock();

            _contentRoot = new Border();
            _contentRoot.Child = _textBlock;
            _contentRoot.Width = _viewSize.Width;
            _contentRoot.Height = _viewSize.Height;

            Border root = new Border();
            root.Background = Brushes.White;
            root.Child = _contentRoot;
            return root;
        }

        // ------------------------------------------------------------------
        // Initialize tests.
        // ------------------------------------------------------------------

        protected override void InitializeTests()
        {
            _textBlock.UpdateLayout();
            _tests = new DrtTestDetails[]{
                new DrtTestDetails("Empty", null, new DrtTestDetails.RunDelegate(VerifyEmpty)),
                new DrtTestDetails("Rectangles", new DrtTestDetails.LoadDelegate(LoadRectangles), new DrtTestDetails.RunDelegate(VerifyRectangles)),
                new DrtTestDetails("HitTest", new DrtTestDetails.LoadDelegate(LoadHitTest), new DrtTestDetails.RunDelegate(VerifyHitTest)),
            };
            _currentTest = 0;
        }

        // ------------------------------------------------------------------
        // Verify empty Text content.
        // ------------------------------------------------------------------
        private void VerifyEmpty()
        {
            DRT.Assert(_textBlock.Text == null || _textBlock.Text == "", "Empty Text element expected!");
        }

        // ------------------------------------------------------------------
        // Load Rectangles.
        // ------------------------------------------------------------------

        private void LoadRectangles()
        {
            System.IO.Stream stream = null;
            try
            {
                stream = System.IO.File.OpenRead(this.DrtFilesDirectory + this.TestName + ".xaml");
                _textBlock = System.Windows.Markup.XamlReader.Load(stream) as TextBlock;
            }
            finally
            {
                // done with the stream
                if (stream != null) { stream.Close(); }
            }
            DRT.Assert(_textBlock != null, this.TestName + ": Failed to load from xaml file.");
            _contentRoot.Child = _textBlock;
            _textBlock.TextWrapping = TextWrapping.WrapWithOverflow;
        }

        // ------------------------------------------------------------------
        // Verify rectangles.
        // ------------------------------------------------------------------

        private void VerifyRectangles()
        {
            // NOTE: the assetions about the number of rectangles are based on the hard-coded input in
            // DrtFiles/TextIContentHostRectangles.xaml and for this window size. If there are any changes to
            // the text text or the window size we will have to recalculate
            Span para1 = (Span)DRT.FindElementByID("para1");
            DRT.Assert(para1 != null);
            ReadOnlyCollection<Rect> rectangles = ((IContentHost)_textBlock).GetRectangles(para1);
            DRT.Assert(rectangles.Count == 2);
            VerifyRectangleCoordinates((Rect)rectangles[0], 188, 30, 511, 30);

            Span para2 = (Span)DRT.FindElementByID("para2");
            DRT.Assert(para2 != null);
            rectangles = ((IContentHost)_textBlock).GetRectangles(para2);
            DRT.Assert(rectangles.Count == 2);
            VerifyRectangleCoordinates((Rect)rectangles[0], 152, 90, 547, 30);
            VerifyRectangleCoordinates((Rect)rectangles[1], 700, 120, 0, 30);

            LineBreak lb1 = (LineBreak)DRT.FindElementByID("lb1");
            DRT.Assert(lb1 != null);
            rectangles = ((IContentHost)_textBlock).GetRectangles(lb1);
            DRT.Assert(rectangles.Count == 1);
            VerifyRectangleCoordinates((Rect)rectangles[0], 700, 90, 0, 30); 
        }

        private void LoadHitTest()
        {
            System.IO.Stream stream = null;
            try
            {
                stream = System.IO.File.OpenRead(this.DrtFilesDirectory + this.TestName + ".xaml");
                _textBlock = System.Windows.Markup.XamlReader.Load(stream) as TextBlock;
            }
            finally
            {
                // done with the stream
                if (stream != null) { stream.Close(); }
            }
            DRT.Assert(_textBlock != null, this.TestName + ": Failed to load from xaml file.");
            _contentRoot.Child = _textBlock;
            _textBlock.TextWrapping = TextWrapping.WrapWithOverflow;
        }

        private void VerifyHitTest()
        {
            Point pt = new Point(350, 35);
            IInputElement ie = ((IContentHost)_textBlock).InputHitTest(pt);
            Run run1 = (Run)DRT.FindElementByID("run1");
            DRT.Assert(ie == run1);
        }

        // Checks coordinates of rectangle
        // NOTE: this check is imprecise. It will truncate the double values to integers. For more exact
        // precision examine the Rect values themselves
        private void VerifyRectangleCoordinates(Rect rect, int x, int y, int width, int height)
        {
            DRT.Assert((int)rect.X == x);
            DRT.Assert((int)rect.Y == y);
            DRT.Assert((int)rect.Width == width);
            DRT.Assert((int)rect.Height == height);
        }

        // ------------------------------------------------------------------
        // Private fields.
        // ------------------------------------------------------------------
        private TextBlock _textBlock;
        private Border _contentRoot;
        private Size _viewSize;
        private const string xamlFilePrefix = "TextBlockRenderTrailingSpaces";
    }
}