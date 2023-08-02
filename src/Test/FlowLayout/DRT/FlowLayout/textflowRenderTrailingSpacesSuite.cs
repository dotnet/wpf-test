// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Checks rendering of trailing spaces on TextFlow
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
    // Trailing spaces rendering suite for TextFlow
    // ----------------------------------------------------------------------
    internal class TextFlowRenderTrailingSpacesSuite : IContentHostSuite
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        internal TextFlowRenderTrailingSpacesSuite()
            : base("TextFlowRenderTrailingSpaces")
        {
            this.Contact = "Microsoft";
            _viewSize = new Size(700, 500);
        }

        // ------------------------------------------------------------------
        // Create initial element tree.
        // ------------------------------------------------------------------
        protected override UIElement CreateTree()
        {
            _fdsv = new FlowDocumentScrollViewer();
            _fdsv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            _fdsv.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            _fdsv.Document = new FlowDocument(new Paragraph());
            _fdsv.Document.TextAlignment = TextAlignment.Left;
            _fdsv.Document.PagePadding = new Thickness(0);

            _contentRoot = new Border();
            _contentRoot.Child = _fdsv;
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
            _fdsv.UpdateLayout();
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
            DRT.Assert(new TextRange(_fdsv.Document.ContentStart, _fdsv.Document.ContentEnd).Text == "\r\n", "Empty TextFlow expected!");
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
                _fdsv = System.Windows.Markup.XamlReader.Load(stream) as FlowDocumentScrollViewer;
            }
            finally
            {
                // done with the stream
                if (stream != null) { stream.Close(); }
            }
            DRT.Assert(_fdsv != null, this.TestName + ": Failed to load from xaml file.");
            _contentRoot.Child = _fdsv;
        }

        // ------------------------------------------------------------------
        // Verify rectangles.
        // ------------------------------------------------------------------

        private void VerifyRectangles()
        {
            IContentHost ich = LayoutSuite.GetIContentHost(_fdsv, DRT);

            Bold bold1 = (Bold)DRT.FindElementByID("bold1");
            DRT.Assert(bold1 != null);
            ReadOnlyCollection<Rect> rectangles = ich.GetRectangles(bold1);
            DRT.Assert(rectangles.Count == 1);
            VerifyRectangleCoordinates((Rect)rectangles[0], 413, 0, 136, 28);

            Bold bold2 = (Bold)DRT.FindElementByID("bold2");
            DRT.Assert(bold2 != null);
            rectangles = ich.GetRectangles(bold2);
            DRT.Assert(rectangles.Count == 1);
            VerifyRectangleCoordinates((Rect)rectangles[0], 396, 56, 61, 28);
        }

        private void LoadHitTest()
        {
            System.IO.Stream stream = null;
            try
            {
                stream = System.IO.File.OpenRead(this.DrtFilesDirectory + this.TestName + ".xaml");
                _fdsv = System.Windows.Markup.XamlReader.Load(stream) as FlowDocumentScrollViewer;
            }
            finally
            {
                // done with the stream
                if (stream != null) { stream.Close(); }
            }
            DRT.Assert(_fdsv != null, this.TestName + ": Failed to load from xaml file.");
            _contentRoot.Child = _fdsv;
        }

        private void VerifyHitTest()
        {
            Point pt = new Point(415, 15);
            IContentHost ich = LayoutSuite.GetIContentHost(_fdsv, DRT);

            IInputElement ie = ich.InputHitTest(pt);
            Run run1 = (Run)DRT.FindElementByID("run1");
            DRT.Assert(ie == run1);

            pt = new Point(200, 15);
            ie = ich.InputHitTest(pt);
            Run run2 = (Run)DRT.FindElementByID("run2");
            DRT.Assert(ie == run2);
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
        private FlowDocumentScrollViewer _fdsv;
        private Border _contentRoot;
        private Size _viewSize;
        private const string xamlFilePrefix = "TextFlowRenderTrailingSpaces";
    }
}