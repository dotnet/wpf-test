// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Test suite for MultiPageTextView. 
//
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;

namespace DRT
{
    // ----------------------------------------------------------------------
    // Test suite for MultiPageTextView.
    // ----------------------------------------------------------------------
    internal sealed class TextViewMultiPageSuite : TextViewSuite
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        internal TextViewMultiPageSuite() : base("TextViewMultiPage")
        {
            this.Contact = "Microsoft";
            _pageSize = new Size(300, 400);
            _viewSize = new Size(700, 500);
        }

        // ------------------------------------------------------------------
        // Initialize tests.
        // ------------------------------------------------------------------
        protected override DrtTest[] CreateTests()
        {
            // Return the lists of tests to run against the tree
            return new DrtTest[] {
                new DrtTest(LoadSimple),            new DrtTest(VerifySimple),
            };
        }

        // ------------------------------------------------------------------
        // CreateEmpty
        // ------------------------------------------------------------------
        private void CreateEmpty()
        {
            _testName = "Empty";

            _pageView1 = new DocumentPageView();
            _pageView1.PageNumber = 0;
            _pageView2 = new DocumentPageView();
            _pageView2.PageNumber = 1;

            _container = new Grid();
            _container.Width = _viewSize.Width;
            _container.Height = _viewSize.Height;
            _container.Background = Brushes.LightGreen;
            _container.Children.Add(_pageView1);
            _container.Children.Add(_pageView2);

            Grid.SetColumn(_pageView1, 0);
            _pageView1.HorizontalAlignment = HorizontalAlignment.Left;
            _pageView1.VerticalAlignment = VerticalAlignment.Top;

            Grid.SetColumn(_pageView2, 1);
            _pageView2.HorizontalAlignment = HorizontalAlignment.Right;
            _pageView2.VerticalAlignment = VerticalAlignment.Bottom;

            _contentRoot.Child = _container;
        }

        // ------------------------------------------------------------------
        // LoadSimple
        // ------------------------------------------------------------------
        private void LoadSimple()
        {
            CreateEmpty();
            _testName = "Simple";
            _paginator = ((IDocumentPaginatorSource)LoadFromXaml(this.TestName + ".xaml")).DocumentPaginator;
            DRT.Assert(_paginator.Source is FlowDocument, this.TestName + ": Expecting FlowDocument as content.");
            _paginator.PageSize = _pageSize;
            _pageView1.DocumentPaginator = _paginator;
            _pageView2.DocumentPaginator = _paginator;
        }

        // ------------------------------------------------------------------
        // VerifySimple
        // ------------------------------------------------------------------
        private void VerifySimple()
        {
            // Get TextContainer
            FlowDocument fd = _paginator.Source as FlowDocument;
            DRT.Assert(fd != null, this.TestName + ": Expecting FlowDocument as content.");
            TextContainer tc = TextContainer.FromTextPointer(fd.ContentStart);

            // Get MultiPageTextView
            object textViewInstance = _assembly.CreateInstance("MS.Internal.Documents.MultiPageTextView", false,
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance, null,
                new object[] { _container, tc.Instance }, System.Globalization.CultureInfo.InvariantCulture, null);
            DRT.Assert(textViewInstance != null, this.TestName + ": Failed to create MultiPageTextView.");
            TextView tv = new TextView(textViewInstance);
            List<DocumentPageView> pageViews = new List<DocumentPageView>(2);
            pageViews.Add(_pageView1);
            pageViews.Add(_pageView2);
            tv.SetActiveDocumentPageViews(new ReadOnlyCollection<DocumentPageView>(pageViews));

            // GetPositionFromPoint
            PositionFromPointTestDesc[] tests1 = new PositionFromPointTestDesc[]{
                new PositionFromPointTestDesc(new Point(5,5), true, LogicalDirection.Forward, 2),
                new PositionFromPointTestDesc(new Point(_viewSize.Width / 2, _viewSize.Height / 2), true, LogicalDirection.Backward, 677),
                new PositionFromPointTestDesc(new Point(_viewSize.Width - 10, _viewSize.Height - 10), true, LogicalDirection.Backward, 1764),
            };

            TextPointer pos = TextContainer.CreateTextPointer(tc.Start, 3, LogicalDirection.Forward);
            double lineHeight = tv.GetRectangleFromTextPosition(pos).Height;
            int cpLast = tc.Start.GetOffsetToPosition(tc.End);

            // GetRectangleFromTextPosition
            RectangleFromPositionTestDesc[] tests2 = new RectangleFromPositionTestDesc[]{
                new RectangleFromPositionTestDesc(0, LogicalDirection.Forward, Rect.Empty),
                new RectangleFromPositionTestDesc(1, LogicalDirection.Forward, new Rect(17,17,0,lineHeight)),
                new RectangleFromPositionTestDesc(1000, LogicalDirection.Backward, new Rect(543,428,0,lineHeight)),
                new RectangleFromPositionTestDesc(cpLast, LogicalDirection.Backward, Rect.Empty),
            };

            // GetPositionAtNextLine
            PositionAtNextLineTestDesc[] tests3 = new PositionAtNextLineTestDesc[]{
                new PositionAtNextLineTestDesc(1000, LogicalDirection.Forward, 522, -5, 821, LogicalDirection.Backward, 122, -5),
                new PositionAtNextLineTestDesc(1000, LogicalDirection.Forward, 522, -1, 951, LogicalDirection.Forward, 122, -1),
                new PositionAtNextLineTestDesc(951, LogicalDirection.Forward, 122, 50, 1737, LogicalDirection.Backward, 522, 24),
                new PositionAtNextLineTestDesc(821, LogicalDirection.Backward, 122, 5, 1000, LogicalDirection.Forward, 522, 5),
            };
        }

        // ------------------------------------------------------------------
        // Private fields.
        // ------------------------------------------------------------------
        private Grid _container;
        private DocumentPageView _pageView1;
        private DocumentPageView _pageView2;
        private DocumentPaginator _paginator;
        private Size _pageSize;
        private Size _viewSize;
    }
}
