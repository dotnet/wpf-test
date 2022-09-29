// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: Test suite for BringIntoView functionality inside viewers.
//

using System;                               // string
using System.Windows;                       // Size, Rect
using System.Windows.Controls;              // FlowDocumentPageViewer
using System.Windows.Documents;             // FlowDocument
using System.Windows.Media;                 // Brushes

namespace DRT
{
    /// <summary>
    /// Test suite for BringIntoView functionality inside viewers.
    /// </summary>
    internal class ViewerBringIntoViewTestSuite : ViewerTestSuite
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="suiteName">Name of the suite.</param>
        internal ViewerBringIntoViewTestSuite() :
            base("ViewerBringIntoView")
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
                // Bring inner ContentElement into view.
                new DrtTest(PageViewerInPageViewer),        new DrtTest(DumpCreate),
                new DrtTest(BringInnerParaIntoView),        new DrtTest(DumpAppend),
                new DrtTest(ScrollViewerInPageViewer),      new DrtTest(DumpAppend),
                new DrtTest(BringInnerParaIntoView),        new DrtTest(DumpAppend),
                new DrtTest(ReaderViewerInPageViewer),      new DrtTest(DumpAppend),
                new DrtTest(BringInnerParaIntoView),        new DrtTest(DumpAppend),
                new DrtTest(PageViewerInScrollViewer),      new DrtTest(DumpAppend),
                new DrtTest(BringInnerParaIntoView),        new DrtTest(DumpAppend),
                new DrtTest(ScrollViewerInScrollViewer),    new DrtTest(DumpAppend),
                new DrtTest(BringInnerParaIntoView),        new DrtTest(DumpAppend),
                new DrtTest(ReaderViewerInScrollViewer),    new DrtTest(DumpAppend),
                new DrtTest(BringInnerParaIntoView),        new DrtTest(DumpAppend),
                new DrtTest(PageViewerInReaderViewer),      new DrtTest(DumpAppend),
                new DrtTest(BringInnerParaIntoView),        new DrtTest(DumpAppend),
                new DrtTest(ScrollViewerInReaderViewer),    new DrtTest(DumpAppend),
                new DrtTest(BringInnerParaIntoView),        new DrtTest(DumpAppend),
                new DrtTest(ReaderViewerInReaderViewer),    new DrtTest(DumpAppend),
                new DrtTest(BringInnerParaIntoView),        new DrtTest(DumpAppend),
                // Bring inner UIElement into view.
                new DrtTest(PageViewerInPageViewer),        new DrtTest(DumpAppend),
                new DrtTest(BringInnerBorderIntoView),      new DrtTest(DumpAppend),
                new DrtTest(ScrollViewerInPageViewer),      new DrtTest(DumpAppend),
                new DrtTest(BringInnerBorderIntoView),      new DrtTest(DumpAppend),
                new DrtTest(ReaderViewerInPageViewer),      new DrtTest(DumpAppend),
                new DrtTest(BringInnerBorderIntoView),      new DrtTest(DumpAppend),
                new DrtTest(PageViewerInScrollViewer),      new DrtTest(DumpAppend),
                new DrtTest(BringInnerBorderIntoView),      new DrtTest(DumpAppend),
                new DrtTest(ScrollViewerInScrollViewer),    new DrtTest(DumpAppend),
                new DrtTest(BringInnerBorderIntoView),      new DrtTest(DumpAppend),
                new DrtTest(ReaderViewerInScrollViewer),    new DrtTest(DumpAppend),
                new DrtTest(BringInnerBorderIntoView),      new DrtTest(DumpAppend),
                new DrtTest(PageViewerInReaderViewer),      new DrtTest(DumpAppend),
                new DrtTest(BringInnerBorderIntoView),      new DrtTest(DumpAppend),
                new DrtTest(ScrollViewerInReaderViewer),    new DrtTest(DumpAppend),
                new DrtTest(BringInnerBorderIntoView),      new DrtTest(DumpAppend),
                new DrtTest(ReaderViewerInReaderViewer),    new DrtTest(DumpAppend),
                new DrtTest(BringInnerBorderIntoView),      new DrtTest(DumpAppend),
                // Bring inner ContentElement into view (indirect).
                new DrtTest(PageViewerInPageViewer2),       new DrtTest(DumpAppend),
                new DrtTest(BringInnerParaIntoView),        new DrtTest(DumpAppend),
                new DrtTest(ScrollViewerInPageViewer2),     new DrtTest(DumpAppend),
                new DrtTest(BringInnerParaIntoView),        new DrtTest(DumpAppend),
                new DrtTest(ReaderViewerInPageViewer2),     new DrtTest(DumpAppend),
                new DrtTest(BringInnerParaIntoView),        new DrtTest(DumpAppend),
                new DrtTest(PageViewerInScrollViewer2),     new DrtTest(DumpAppend),
                new DrtTest(BringInnerParaIntoView),        new DrtTest(DumpAppend),
                new DrtTest(ScrollViewerInScrollViewer2),   new DrtTest(DumpAppend),
                new DrtTest(BringInnerParaIntoView),        new DrtTest(DumpAppend),
                new DrtTest(ReaderViewerInScrollViewer2),   new DrtTest(DumpAppend),
                new DrtTest(BringInnerParaIntoView),        new DrtTest(DumpAppend),
                new DrtTest(PageViewerInReaderViewer2),     new DrtTest(DumpAppend),
                new DrtTest(BringInnerParaIntoView),        new DrtTest(DumpAppend),
                new DrtTest(ScrollViewerInReaderViewer2),   new DrtTest(DumpAppend),
                new DrtTest(BringInnerParaIntoView),        new DrtTest(DumpAppend),
                new DrtTest(ReaderViewerInReaderViewer2),   new DrtTest(DumpAppend),
                new DrtTest(BringInnerParaIntoView),        new DrtTest(DumpFinalizeAndVerify),
            };
        }

        /// <summary>
        /// Document viewer instance.
        /// </summary>
        protected override Control Viewer { get { return _viewer; } }

        /// <summary>
        /// Creates PageViewer with nested PageViewer.
        /// </summary>
        private void PageViewerInPageViewer()
        {
            FlowDocumentPageViewer viewer = new FlowDocumentPageViewer();
            viewer.Document = CreateDocumentWithPageViewer(false);
            _viewer = viewer;
            SetViewerProperties();
            this.Root.Child = _viewer;
        }

        /// <summary>
        /// Creates PageViewer with nested PageViewer.
        /// </summary>
        private void PageViewerInPageViewer2()
        {
            FlowDocumentPageViewer viewer = new FlowDocumentPageViewer();
            viewer.Document = CreateDocumentWithPageViewer(true);
            _viewer = viewer;
            SetViewerProperties();
            this.Root.Child = _viewer;
        }

        /// <summary>
        /// Creates PageViewer with nested ScrollViewer.
        /// </summary>
        private void ScrollViewerInPageViewer()
        {
            FlowDocumentPageViewer viewer = new FlowDocumentPageViewer();
            viewer.Document = CreateDocumentWithScrollViewer(false);
            _viewer = viewer;
            SetViewerProperties();
            this.Root.Child = _viewer;
        }

        /// <summary>
        /// Creates PageViewer with nested ScrollViewer.
        /// </summary>
        private void ScrollViewerInPageViewer2()
        {
            FlowDocumentPageViewer viewer = new FlowDocumentPageViewer();
            viewer.Document = CreateDocumentWithScrollViewer(true);
            _viewer = viewer;
            SetViewerProperties();
            this.Root.Child = _viewer;
        }

        /// <summary>
        /// Creates PageViewer with nested ReaderViewer.
        /// </summary>
        private void ReaderViewerInPageViewer()
        {
            FlowDocumentPageViewer viewer = new FlowDocumentPageViewer();
            viewer.Document = CreateDocumentWithReaderViewer(false);
            _viewer = viewer;
            SetViewerProperties();
            this.Root.Child = _viewer;
        }

        /// <summary>
        /// Creates PageViewer with nested ReaderViewer.
        /// </summary>
        private void ReaderViewerInPageViewer2()
        {
            FlowDocumentPageViewer viewer = new FlowDocumentPageViewer();
            viewer.Document = CreateDocumentWithReaderViewer(true);
            _viewer = viewer;
            SetViewerProperties();
            this.Root.Child = _viewer;
        }

        /// <summary>
        /// Creates ScrollViewer with nested PageViewer.
        /// </summary>
        private void PageViewerInScrollViewer()
        {
            FlowDocumentScrollViewer viewer = new FlowDocumentScrollViewer();
            viewer.Document = CreateDocumentWithPageViewer(false);
            _viewer = viewer;
            SetViewerProperties();
            this.Root.Child = _viewer;
        }

        /// <summary>
        /// Creates ScrollViewer with nested PageViewer.
        /// </summary>
        private void PageViewerInScrollViewer2()
        {
            FlowDocumentScrollViewer viewer = new FlowDocumentScrollViewer();
            viewer.Document = CreateDocumentWithPageViewer(true);
            _viewer = viewer;
            SetViewerProperties();
            this.Root.Child = _viewer;
        }

        /// <summary>
        /// Creates ScrollViewer with nested ScrollViewer.
        /// </summary>
        private void ScrollViewerInScrollViewer()
        {
            FlowDocumentScrollViewer viewer = new FlowDocumentScrollViewer();
            viewer.Document = CreateDocumentWithScrollViewer(false);
            _viewer = viewer;
            SetViewerProperties();
            this.Root.Child = _viewer;
        }

        /// <summary>
        /// Creates ScrollViewer with nested ScrollViewer.
        /// </summary>
        private void ScrollViewerInScrollViewer2()
        {
            FlowDocumentScrollViewer viewer = new FlowDocumentScrollViewer();
            viewer.Document = CreateDocumentWithScrollViewer(true);
            _viewer = viewer;
            SetViewerProperties();
            this.Root.Child = _viewer;
        }

        /// <summary>
        /// Creates ScrollViewer with nested ReaderViewer.
        /// </summary>
        private void ReaderViewerInScrollViewer()
        {
            FlowDocumentScrollViewer viewer = new FlowDocumentScrollViewer();
            viewer.Document = CreateDocumentWithReaderViewer(false);
            _viewer = viewer;
            SetViewerProperties();
            this.Root.Child = _viewer;
        }

        /// <summary>
        /// Creates ScrollViewer with nested ReaderViewer.
        /// </summary>
        private void ReaderViewerInScrollViewer2()
        {
            FlowDocumentScrollViewer viewer = new FlowDocumentScrollViewer();
            viewer.Document = CreateDocumentWithReaderViewer(true);
            _viewer = viewer;
            SetViewerProperties();
            this.Root.Child = _viewer;
        }

        /// <summary>
        /// Creates ReaderViewer with nested PageViewer.
        /// </summary>
        private void PageViewerInReaderViewer()
        {
            FlowDocumentReader viewer = new FlowDocumentReader();
            viewer.Document = CreateDocumentWithPageViewer(false);
            _viewer = viewer;
            SetViewerProperties();
            this.Root.Child = _viewer;
        }

        /// <summary>
        /// Creates ReaderViewer with nested PageViewer.
        /// </summary>
        private void PageViewerInReaderViewer2()
        {
            FlowDocumentReader viewer = new FlowDocumentReader();
            viewer.Document = CreateDocumentWithPageViewer(true);
            _viewer = viewer;
            SetViewerProperties();
            this.Root.Child = _viewer;
        }

        /// <summary>
        /// Creates ReaderViewer with nested ScrollViewer.
        /// </summary>
        private void ScrollViewerInReaderViewer()
        {
            FlowDocumentReader viewer = new FlowDocumentReader();
            viewer.Document = CreateDocumentWithScrollViewer(false);
            _viewer = viewer;
            SetViewerProperties();
            this.Root.Child = _viewer;
        }

        /// <summary>
        /// Creates ReaderViewer with nested ScrollViewer.
        /// </summary>
        private void ScrollViewerInReaderViewer2()
        {
            FlowDocumentReader viewer = new FlowDocumentReader();
            viewer.Document = CreateDocumentWithScrollViewer(true);
            _viewer = viewer;
            SetViewerProperties();
            this.Root.Child = _viewer;
        }

        /// <summary>
        /// Creates ReaderViewer with nested ReaderViewer.
        /// </summary>
        private void ReaderViewerInReaderViewer()
        {
            FlowDocumentReader viewer = new FlowDocumentReader();
            viewer.Document = CreateDocumentWithReaderViewer(false);
            _viewer = viewer;
            SetViewerProperties();
            this.Root.Child = _viewer;
        }

        /// <summary>
        /// Creates ReaderViewer with nested ReaderViewer.
        /// </summary>
        private void ReaderViewerInReaderViewer2()
        {
            FlowDocumentReader viewer = new FlowDocumentReader();
            viewer.Document = CreateDocumentWithReaderViewer(true);
            _viewer = viewer;
            SetViewerProperties();
            this.Root.Child = _viewer;
        }

        /// <summary>
        /// Bring "InnerPara" element into view.
        /// </summary>
        private void BringInnerParaIntoView()
        {
            Paragraph para = DRT.FindElementByID("InnerPara", _viewer) as Paragraph;
            DRT.Assert(para != null, "Cannot find element with Name='InnerPara'.");
            para.BringIntoView();
        }

        /// <summary>
        /// Bring "InnerBorder" element into view.
        /// </summary>
        private void BringInnerBorderIntoView()
        {
            Border border = DRT.FindElementByID("InnerBorder", _viewer) as Border;
            DRT.Assert(border != null, "Cannot find element with Name='InnerBorder'.");
            border.BringIntoView();
        }

        /// <summary>
        /// Create FlowDocument with nested PageViewer.
        /// </summary>
        private FlowDocument CreateDocumentWithPageViewer(bool separateWithUI)
        {
            BlockUIContainer uiContainer;
            FlowDocument document = CreateDocument();
            FlowDocumentPageViewer viewer = new FlowDocumentPageViewer();
            viewer.Document = CreateNestedDocument();
            if (!separateWithUI)
            {
                uiContainer = new BlockUIContainer(viewer);
            }
            else
            {
                Border border = new Border();
                border.Child = viewer;
                uiContainer = new BlockUIContainer(border);
            }
            document.Blocks.Add(uiContainer);
            return document;
        }

        /// <summary>
        /// Create FlowDocument with nested ScrollViewer.
        /// </summary>
        private FlowDocument CreateDocumentWithScrollViewer(bool separateWithUI)
        {
            BlockUIContainer uiContainer;
            FlowDocument document = CreateDocument();
            FlowDocumentScrollViewer viewer = new FlowDocumentScrollViewer();
            viewer.Document = CreateNestedDocument();
            if (!separateWithUI)
            {
                uiContainer = new BlockUIContainer(viewer);
            }
            else
            {
                Border border = new Border();
                border.Child = viewer;
                uiContainer = new BlockUIContainer(border);
            }
            document.Blocks.Add(uiContainer);
            return document;
        }

        /// <summary>
        /// Create FlowDocument with nested ReaderViewer.
        /// </summary>
        private FlowDocument CreateDocumentWithReaderViewer(bool separateWithUI)
        {
            BlockUIContainer uiContainer;
            FlowDocument document = CreateDocument();
            FlowDocumentReader viewer = new FlowDocumentReader();
            viewer.Document = CreateNestedDocument();
            if (!separateWithUI)
            {
                uiContainer = new BlockUIContainer(viewer);
            }
            else
            {
                Border border = new Border();
                border.Child = viewer;
                uiContainer = new BlockUIContainer(border);
            }
            document.Blocks.Add(uiContainer);
            return document;
        }

        /// <summary>
        /// Create FlowDocument.
        /// </summary>
        private FlowDocument CreateDocument()
        {
            FlowDocument document = new FlowDocument();
            document.Blocks.Add(CreateParagraph());
            document.Blocks.Add(CreateParagraph());
            document.Blocks.Add(CreateParagraph());
            document.Blocks.Add(CreateParagraph());
            document.Blocks.Add(CreateParagraph());
            return document;
        }

        /// <summary>
        /// Create nested FlowDocument.
        /// </summary>
        private FlowDocument CreateNestedDocument()
        {
            FlowDocument document = new FlowDocument();
            Paragraph para = CreateParagraph();
            para.Background = Brushes.LightGreen;
            para.Name = "InnerPara";
            document.Blocks.Add(para);
            document.Blocks.Add(CreateBlockUIContainer());
            return document;
        }

        /// <summary>
        /// Create Paragraph.
        /// </summary>
        private Paragraph CreateParagraph()
        {
            Paragraph para = new Paragraph();
            para.Inlines.Add(new Run("Your application's user interface is an intermediary, situated between the user and the computer. To some extent, "));
            para.Inlines.Add(new Run("the user and the software rely on each other. A user cannot directly manipulate bits of data in memory or storage "));
            para.Inlines.Add(new Run("hardware, nor can the software's limited capacity for intelligence allow it to independently determine the desired "));
            para.Inlines.Add(new Run("result for any non-trivial task. In a way, the user and the software collaborate. This collaboration requires you "));
            para.Inlines.Add(new Run("to make a critical decision: which of the two will control the interaction—the user or the software?"));
            return para;
        }

        /// <summary>
        /// Create BlockUIContainer.
        /// </summary>
        private BlockUIContainer CreateBlockUIContainer()
        {
            Border brInner = new Border();
            brInner.BorderBrush = Brushes.Blue;
            brInner.BorderThickness = new Thickness(5);
            brInner.Background = Brushes.GreenYellow;
            brInner.Name = "InnerBorder";
            Border brOuter = new Border();
            brOuter.BorderBrush = Brushes.Green;
            brOuter.BorderThickness = new Thickness(5);
            brOuter.Child = brInner;
            return new BlockUIContainer(brOuter);
        }

        private void SetViewerProperties()
        {
            if (_viewer != null)
            {
                _viewer.FontSize = 11.0;
                _viewer.FontFamily = new FontFamily("Tahoma");
            }
        }

        private Control _viewer;
    }
}
