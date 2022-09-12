// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: Test suite for FlowDocumentPageViewer basic scenarios.
//

using System;                               // string
using System.Windows;                       // Size, Rect
using System.Windows.Controls;              // FlowDocumentPageViewer
using System.Windows.Documents;             // TextElement
using System.Windows.Media;                 // Fonts

namespace DRT
{
    /// <summary>
    /// Test suite for FlowDocumentPageViewer basic scenarios.
    /// </summary>
    internal class PageViewerTestSuite : ViewerTestSuite
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="suiteName">Name of the suite.</param>
        internal PageViewerTestSuite():
            base("PageViewer")
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
                new DrtTest(LoadFlowDocument1),             new DrtTest(DumpAppend),
                new DrtTest(NextPageCommand),               new DrtTest(DumpAppend),
                new DrtTest(GoToPageCommand),               new DrtTest(DumpAppend),
                new DrtTest(LastPageCommand),               new DrtTest(DumpAppend),
                new DrtTest(PreviousPageCommand),           new DrtTest(DumpAppend),
                new DrtTest(FirstPageCommand),              new DrtTest(DumpAppend),
                new DrtTest(BringIntoView1),                new DrtTest(DumpAppend),
                new DrtTest(BringIntoView2),                new DrtTest(DumpAppend),
                new DrtTest(BringIntoView3),                new DrtTest(DumpAppend),
                new DrtTest(BringIntoView4),                new DrtTest(DumpAppend),
                new DrtTest(UnloadDocument),                new DrtTest(DumpAppend),
                new DrtTest(LoadFlowDocument2),             new DrtTest(DumpFinalizeAndVerify),
            };
        }

        /// <summary>
        /// Document viewer instance.
        /// </summary>
        protected override Control Viewer { get { return _viewer; } }

        /// <summary>
        /// Creates FlowDocumentPageViewer with no document attached to it.
        /// </summary>
        private void NoDocument()
        {
            _viewer = new FlowDocumentPageViewer();
            _viewer.FontSize = 11.0;
            _viewer.FontFamily = new FontFamily("Tahoma");
            _document = null;
            this.Root.Child = _viewer;
        }

        /// <summary>
        /// Load FlowDocument.
        /// </summary>
        private void LoadFlowDocument1()
        {
            _document = LoadFromXaml("FlowDocumentComplex.xaml") as FlowDocument;
            DRT.Assert(_document != null, "Cannot load 'FlowDocumentComplex.xaml'.");
            _viewer.Document = _document;
        }

        /// <summary>
        /// Go to page number 5.
        /// </summary>
        private void GoToPageCommand()
        {
            GoToPageCommand(5);
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

        /// <summary>
        /// Load FlowDocument.
        /// </summary>
        private void LoadFlowDocument2()
        {
            _document = LoadFromXaml("FlowDocumentPlain.xaml") as FlowDocument;
            DRT.Assert(_document != null, "Cannot load 'FlowDocumentPlain.xaml'.");
            ((DynamicDocumentPaginator)((IDocumentPaginatorSource)_document).DocumentPaginator).IsBackgroundPaginationEnabled = false;
            _viewer.Document = _document;
        }

        private FlowDocumentPageViewer _viewer;
        private FlowDocument _document;
    }
}
