// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: Test suite for FlowDocumentScrollViewer basic scenarios.
//

using System;                               // string
using System.Windows;                       // Size, Rect
using System.Windows.Controls;              // FlowDocumentScrollViewer
using System.Windows.Documents;             // TextElement
using System.Windows.Media;                 // Fonts

namespace DRT
{
    /// <summary>
    /// Test suite for FlowDocumentScrollViewer basic scenarios.
    /// </summary>
    internal class ScrollViewerTestSuite : ViewerTestSuite
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="suiteName">Name of the suite.</param>
        internal ScrollViewerTestSuite():
            base("ScrollViewer")
        {
            this.Contact = "Microsoft";
        }

        /// <summary>
        /// Returns collection of tests.
        /// </summary>
        protected override DrtTest[] CreateTests()
        {
            // Return the lists of tests to run against the tree
            return new DrtTest[] {
                new DrtTest(NoDocument),                    new DrtTest(DumpCreate),
                new DrtTest(LoadFlowDocument),              new DrtTest(DumpAppend),
                new DrtTest(IncreaseZoomCommand),           new DrtTest(DumpAppend),
                new DrtTest(DecreaseZoomCommand),           new DrtTest(DumpAppend),
                new DrtTest(ZoomInBy50),                    new DrtTest(DumpAppend),
                new DrtTest(ZoomInBy50),                    new DrtTest(DumpAppend),
                new DrtTest(ZoomInBy50),                    new DrtTest(DumpAppend),
                new DrtTest(ZoomOutBy50),                   new DrtTest(DumpAppend),
                new DrtTest(ZoomOutBy50),                   new DrtTest(DumpAppend),
                new DrtTest(ZoomToMax),                     new DrtTest(DumpAppend),
                new DrtTest(IncreaseMaxZoom),               new DrtTest(DumpAppend),
                new DrtTest(ZoomToMin),                     new DrtTest(DumpAppend),
                new DrtTest(DecreaseMinZoom),               new DrtTest(DumpAppend),
                new DrtTest(IncreaseMinZoom),               new DrtTest(DumpAppend),
                new DrtTest(ZoomGreaterThanMax),            new DrtTest(DumpAppend),
                new DrtTest(IncreaseMaxZoom),               new DrtTest(DumpAppend),
                new DrtTest(ZoomLessThanMin),               new DrtTest(DumpAppend),
                new DrtTest(DecreaseMinZoom),               new DrtTest(DumpAppend),
                new DrtTest(ResetZoom),                     new DrtTest(DumpAppend),
                new DrtTest(BringIntoView1),                new DrtTest(DumpAppend),
                new DrtTest(BringIntoView2),                new DrtTest(DumpAppend),
                new DrtTest(BringIntoView3),                new DrtTest(DumpAppend),
                new DrtTest(BringIntoView4),                new DrtTest(DumpAppend),
                new DrtTest(UnloadDocument),                new DrtTest(DumpFinalizeAndVerify),
            };
        }

        /// <summary>
        /// Document viewer instance.
        /// </summary>
        protected override Control Viewer { get { return _viewer; } }

        /// <summary>
        /// Creates FlowDocumentScrollViewer with no document attached to it.
        /// </summary>
        private void NoDocument()
        {
            _viewer = new FlowDocumentScrollViewer();
            _viewer.FontSize = 11.0;
            _viewer.FontFamily = new FontFamily("Tahoma");
            _document = null;
            this.Root.Child = _viewer;
        }

        /// <summary>
        /// Load FlowDocument.
        /// </summary>
        private void LoadFlowDocument()
        {
            _document = LoadFromXaml("FlowDocumentComplex.xaml") as FlowDocument;
            DRT.Assert(_document != null, "Cannot load 'FlowDocumentComplex.xaml'.");
            _viewer.Document = _document;
        }

        /// <summary>
        /// ZoomInBy50.
        /// </summary>
        private void ZoomInBy50()
        {
            _viewer.Zoom += 50;
        }

        /// <summary>
        /// ZoomOutBy50.
        /// </summary>
        private void ZoomOutBy50()
        {
            _viewer.Zoom -= 50;
        }

        /// <summary>
        /// ZoomDefault.
        /// </summary>
        private void ZoomDefault()
        {
            _viewer.ClearValue(FlowDocumentScrollViewer.ZoomProperty);
        }

        /// <summary>
        /// ZoomToMax.
        /// </summary>
        private void ZoomToMax()
        {
            _viewer.Zoom = _viewer.MaxZoom;
        }

        /// <summary>
        /// IncreaseMaxZoom.
        /// </summary>
        private void IncreaseMaxZoom()
        {
            _viewer.MaxZoom += 10;
        }

        /// <summary>
        /// ZoomToMin.
        /// </summary>
        private void ZoomToMin()
        {
            _viewer.Zoom = _viewer.MinZoom;
        }

        /// <summary>
        /// DecreaseMinZoom.
        /// </summary>
        private void DecreaseMinZoom()
        {
            _viewer.MinZoom -= 10;
        }

        /// <summary>
        /// IncreaseMinZoom.
        /// </summary>
        private void IncreaseMinZoom()
        {
            _viewer.MinZoom += 20;
        }

        /// <summary>
        /// ZoomGreaterThanMax.
        /// </summary>
        private void ZoomGreaterThanMax()
        {
            _viewer.Zoom = _viewer.MaxZoom + 10;
        }

        /// <summary>
        /// ZoomLessThanMin.
        /// </summary>
        private void ZoomLessThanMin()
        {
            _viewer.Zoom = _viewer.MinZoom - 5;
        }

        /// <summary>
        /// ResetZoom.
        /// </summary>
        private void ResetZoom()
        {
            _viewer.ClearValue(FlowDocumentScrollViewer.ZoomProperty);
            _viewer.ClearValue(FlowDocumentScrollViewer.MinZoomProperty);
            _viewer.ClearValue(FlowDocumentScrollViewer.MaxZoomProperty);
        }

        /// <summary>
        /// Bring paragraph into view.
        /// </summary>
        private void BringIntoView1()
        {
            TextElement element = DRT.FindElementByID("sixteenth", _viewer) as TextElement;
            DRT.Assert(element != null, "Cannot find element with Name='sixteenth'.");
            element.BringIntoView();
        }

        /// <summary>
        /// Bring outer UIElement into view.
        /// </summary>
        private void BringIntoView2()
        {
            FrameworkElement element = DRT.FindElementByID("outer", _viewer) as FrameworkElement;
            DRT.Assert(element != null, "Cannot find element with Name='outer'.");
            element.BringIntoView();
        }

        /// <summary>
        /// Bring paragraph into view.
        /// </summary>
        private void BringIntoView3()
        {
            TextElement element = DRT.FindElementByID("nineteenth", _viewer) as TextElement;
            DRT.Assert(element != null, "Cannot find element with Name='nineteenth'.");
            element.BringIntoView();
        }

        /// <summary>
        /// Bring inner UIElement into view.
        /// </summary>
        private void BringIntoView4()
        {
            FrameworkElement element = DRT.FindElementByID("inner", _viewer) as FrameworkElement;
            DRT.Assert(element != null, "Cannot find element with Name='inner'.");
            element.BringIntoView();
        }

        /// <summary>
        /// Unload document.
        /// </summary>
        private void UnloadDocument()
        {
            _document = null;
            _viewer.Document = null;
        }

        private FlowDocumentScrollViewer _viewer;
        private FlowDocument _document;
    }
}
