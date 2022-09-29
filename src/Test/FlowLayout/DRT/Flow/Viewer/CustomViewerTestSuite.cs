// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: Test suite for CustomViewer basic scenarios.
//

using System;                               // string
using System.Windows;                       // Size, Rect
using System.Windows.Controls;              // FlowDocumentPageViewer
using System.Windows.Controls.Primitives;   // DocumentViewerBase
using System.Windows.Documents;             // TextElement
using System.Windows.Media;                 // Fonts

namespace DRT
{
    /// <summary>
    /// Test suite for CustomViewer basic scenarios.
    /// </summary>
    internal class CustomViewerTestSuite : ViewerTestSuite
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="suiteName">Name of the suite.</param>
        internal CustomViewerTestSuite():
            base("CustomViewer")
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
                new DrtTest(LoadCustomStyle2Pages),         new DrtTest(DumpAppend),
                new DrtTest(NextPageCommand),               new DrtTest(DumpAppend),
                new DrtTest(LoadDefaultStyle),              new DrtTest(DumpAppend),
                new DrtTest(CustomDocumentViewer),          new DrtTest(DumpAppend),
                new DrtTest(LoadCustomStyle1Page),          new DrtTest(DumpAppend),
                new DrtTest(LoadCustomPaginator),           new DrtTest(DumpAppend),
                new DrtTest(LoadCustomDynamicPaginator),    new DrtTest(DumpAppend),
                new DrtTest(NextPageCommand),               new DrtTest(DumpAppend),
                new DrtTest(GoToPage3),                     new DrtTest(DumpAppend),
                new DrtTest(NextPageCommand),               new DrtTest(DumpAppend),
                new DrtTest(FirstPageCommand),              new DrtTest(DumpFinalizeAndVerify),
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
            _viewer = new CustomFlowDocumentPageViewer();
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
            _document = LoadFromXaml("FlowDocumentPlain.xaml") as IDocumentPaginatorSource;
            DRT.Assert(_document != null, "Cannot load 'FlowDocumentPlain.xaml'.");
            _viewer.Document = _document;
        }

        /// <summary>
        /// Load custom style.
        /// </summary>
        private void LoadCustomStyle2Pages()
        {
            ResourceDictionary resources = LoadFromXaml("CustomStyles.xaml") as ResourceDictionary;
            Style style = resources["CTDocumentViewerTwoPagesWithBorderStyle"] as Style;
            _viewer.Style = style;
        }

        /// <summary>
        /// Load default style.
        /// </summary>
        private void LoadDefaultStyle()
        {
            _viewer.ClearValue(FrameworkElement.StyleProperty);
        }

        /// <summary>
        /// Creates CustomDocumentViewer with no document attached to it.
        /// </summary>
        private void CustomDocumentViewer()
        {
            _viewer = new CustomDocumentViewer();
            _document = null;
            this.Root.Child = _viewer;
        }

        /// <summary>
        /// Load custom style.
        /// </summary>
        private void LoadCustomStyle1Page()
        {
            ResourceDictionary resources = LoadFromXaml("CustomStyles.xaml") as ResourceDictionary;
            Style style = resources["CTDocumentViewerSinglePageWithBorderStyle"] as Style;
            _viewer.Style = style;
        }

        /// <summary>
        /// Load CustomPaginator.
        /// </summary>
        private void LoadCustomPaginator()
        {
            _document = new CustomDocumentPaginatorSource();
            _viewer.Document = _document;
        }

        /// <summary>
        /// Load CustomDynamicPaginator.
        /// </summary>
        private void LoadCustomDynamicPaginator()
        {
            _document = new CustomDynamicDocumentPaginatorSource();
            _viewer.Document = _document;
        }

        /// <summary>
        /// Go to page #3.
        /// </summary>
        private void GoToPage3()
        {
            GoToPageCommand(3);
        }

        private DocumentViewerBase _viewer;
        private IDocumentPaginatorSource _document;
    }
}
