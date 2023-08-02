// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Test suite for pagination APIs. 
//
//

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;

namespace DRT
{
    // ----------------------------------------------------------------------
    // Test suite for pagination.
    // ----------------------------------------------------------------------
    internal sealed class PaginationApiSuite : LayoutSuite
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        internal PaginationApiSuite() : base("PaginationApi")
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
                new DrtTest(NoContent),                 new DrtTest(VerifyNoContent),
                new DrtTest(SmallContent),              new DrtTest(VerifySmallContent),
                new DrtTest(LargeContent),              new DrtTest(VerifyLargeContent),
                new DrtTest(ModifyPage0),               new DrtTest(VerifyModifyPage0),
                new DrtTest(ModifyPage2),               new DrtTest(VerifyModifyPage2),
                new DrtTest(ModifyPage3),               new DrtTest(VerifyModifyPage3),
                new DrtTest(NavigateToLastPage),        new DrtTest(VerifyNavigateToLastPage),
                new DrtTest(NavigateToFirstPageAsync),  new DrtTest(VerifyNavigateToFirstPageAsync),
                new DrtTest(NavigateRestorePage1),      new DrtTest(VerifyNavigateRestorePage1),
                new DrtTest(RemoveFromPage2),           new DrtTest(VerifyRemoveFromPage2),
                new DrtTest(DeletePages),               new DrtTest(VerifyDeletePages),
            };
        }

        // ------------------------------------------------------------------
        // NoContent
        // ------------------------------------------------------------------
        private void NoContent()
        {
            _testName = "NoContent";

            _pageView1 = new DocumentPageView();
            _pageView1.PageNumber = 1;
            _pageView2 = new DocumentPageView();
            _pageView2.PageNumber = 2;

            Grid container = new Grid();
            container.Background = Brushes.LightGreen;
            container.Children.Add(_pageView1);
            container.Children.Add(_pageView2);
            Grid.SetColumn(_pageView1, 0);

            _pageView1.HorizontalAlignment = HorizontalAlignment.Left;
            _pageView1.VerticalAlignment = VerticalAlignment.Top;

            Grid.SetColumn(_pageView2, 1);

            _pageView1.HorizontalAlignment = HorizontalAlignment.Right;
            _pageView1.VerticalAlignment = VerticalAlignment.Bottom;

            _contentRoot.Child = container;
        }

        // ------------------------------------------------------------------
        // VerifyNoContent
        // ------------------------------------------------------------------
        private void VerifyNoContent()
        {
            DRT.Assert(_pageView1.DocumentPage == DocumentPage.Missing, "{0}: Failed: Expecting DocumentPage.Missing for the first page.", this.TestName);
            DRT.Assert(_pageView2.DocumentPage == DocumentPage.Missing, "{0}: Failed: Expecting DocumentPage.Missing for the second page.", this.TestName);
        }

        // ------------------------------------------------------------------
        // SmallContent
        // ------------------------------------------------------------------
        private void SmallContent()
        {
            _testName = "SmallContent";
            ResetState();

            FlowDocument document = new FlowDocument();
            _paginator = ((IDocumentPaginatorSource)document).DocumentPaginator as DynamicDocumentPaginator;
            _paginator.PageSize = new Size(350, 500);
            _paginator.GetPageCompleted += new GetPageCompletedEventHandler(OnGetPageCompleted);
            _paginator.GetPageNumberCompleted += new GetPageNumberCompletedEventHandler(OnGetPageNumberCompleted);
            _paginator.PaginationCompleted += new EventHandler(OnPaginationCompleted);
            _paginator.PaginationProgress += new PaginationProgressEventHandler(OnPaginationProgress);
            _paginator.PagesChanged += new PagesChangedEventHandler(OnPagesChanged);
            _pageView1.DocumentPaginator = _paginator;
            _pageView2.DocumentPaginator = _paginator;

            // Add content.
            BlockCollection blocks = (document).Blocks;
            AppendSimpleParagraph(blocks, "Para1");
            AppendSimpleParagraph(blocks, "Para2");
            AppendSimpleParagraph(blocks, "Para3");
            AppendSimpleParagraph(blocks, "Para4");
            AppendSimpleParagraph(blocks, "Para5");
        }

        // ------------------------------------------------------------------
        // VerifySmallContent
        // ------------------------------------------------------------------
        private void VerifySmallContent()
        {
            DRT.Assert(_pageView1.DocumentPage != DocumentPage.Missing, "{0}: Failed: Expecting valid DocumentPage for PN={1}.", this.TestName, 1);
            DRT.Assert(_pageView2.DocumentPage != DocumentPage.Missing, "{0}: Failed: Expecting valid DocumentPage for PN={1}.", this.TestName, 2);
            // Events
            DRT.Assert(_paginationCompleted, "{0}: Failed: Should receive PaginationCompleted event.", this.TestName);
            DRT.Assert(!_getPageNumberCompleted, "{0}: Failed: Should not receive GetPageNumberCompleted event.", this.TestName);
            // Page 0 events
            DRT.Assert(_pageState[0].GetPageCompleted == 0, "{0}: Failed: PN={1}: GetPageCompleted event: Expecting {2}, got {3}.",     this.TestName, 0, 0, _pageState[0].GetPageCompleted);
            DRT.Assert(_pageState[0].PaginationProgress == 1, "{0}: Failed: PN={1}: PaginationProgress event: Expecting {2}, got {3}.", this.TestName, 0, 1, _pageState[0].PaginationProgress);
            DRT.Assert(_pageState[0].PagesChanged == 0, "{0}: Failed: PN={1}: PagesChanged event: Expecting {2}, got {3}.",             this.TestName, 0, 0, _pageState[0].PagesChanged);
            // Page 1 events
            DRT.Assert(_pageState[1].GetPageCompleted == 1, "{0}: Failed: PN={1}: GetPageCompleted event: Expecting {2}, got {3}.",     this.TestName, 1, 1, _pageState[1].GetPageCompleted);
            DRT.Assert(_pageState[1].PaginationProgress == 1, "{0}: Failed: PN={1}: PaginationProgress event: Expecting {2}, got {3}.", this.TestName, 1, 1, _pageState[1].PaginationProgress);
            DRT.Assert(_pageState[1].PagesChanged == 0, "{0}: Failed: PN={1}: PagesChanged event: Expecting {2}, got {3}.",             this.TestName, 1, 0, _pageState[1].PagesChanged);
            // Page 2 events
            DRT.Assert(_pageState[2].GetPageCompleted == 1, "{0}: Failed: PN={1}: GetPageCompleted event: Expecting {2}, got {3}.",     this.TestName, 2, 1, _pageState[2].GetPageCompleted);
            DRT.Assert(_pageState[2].PaginationProgress == 1, "{0}: Failed: PN={1}: PaginationProgress event: Expecting {2}, got {3}.", this.TestName, 2, 1, _pageState[2].PaginationProgress);
            DRT.Assert(_pageState[2].PagesChanged == 0, "{0}: Failed: PN={1}: PagesChanged event: Expecting {2}, got {3}.",             this.TestName, 2, 0, _pageState[2].PagesChanged);
            // Page 2 events
            DRT.Assert(_pageState[3].GetPageCompleted == 0, "{0}: Failed: PN={1}: GetPageCompleted event: Expecting {2}, got {3}.",     this.TestName, 3, 0, _pageState[3].GetPageCompleted);
            DRT.Assert(_pageState[3].PaginationProgress == 1, "{0}: Failed: PN={1}: PaginationProgress event: Expecting {2}, got {3}.", this.TestName, 3, 1, _pageState[3].PaginationProgress);
            DRT.Assert(_pageState[3].PagesChanged == 0, "{0}: Failed: PN={1}: PagesChanged event: Expecting {2}, got {3}.",             this.TestName, 3, 0, _pageState[3].PagesChanged);
        }

        // ------------------------------------------------------------------
        // LargeContent
        // ------------------------------------------------------------------
        private void LargeContent()
        {
            _testName = "LargeContent";
            ResetState();

            // Add content.
            BlockCollection blocks = ((FlowDocument)_paginator.Source).Blocks;
            AppendSimpleParagraph(blocks, "Para4");
            AppendSimpleParagraph(blocks, "Para5");
            AppendSimpleParagraph(blocks, "Para6");
            AppendSimpleParagraph(blocks, "Para7");
        }

        // ------------------------------------------------------------------
        // VerifyLargeContent
        // ------------------------------------------------------------------
        private void VerifyLargeContent()
        {
            DRT.Assert(_pageView1.DocumentPage != DocumentPage.Missing, "{0}: Failed: Expecting valid DocumentPage for PN={1}.", this.TestName, 1);
            DRT.Assert(_pageView2.DocumentPage != DocumentPage.Missing, "{0}: Failed: Expecting valid DocumentPage for PN={1}.", this.TestName, 2);
            // Events
            DRT.Assert(_paginationCompleted, "{0}: Failed: Should receive PaginationCompleted event.", this.TestName);
            DRT.Assert(!_getPageNumberCompleted, "{0}: Failed: Should not receive GetPageNumberCompleted event.", this.TestName);
            // Page 0 events
            DRT.Assert(_pageState[0].GetPageCompleted == 0, "{0}: Failed: PN={1}: GetPageCompleted event: Expecting {2}, got {3}.",     this.TestName, 0, 0, _pageState[0].GetPageCompleted);
            DRT.Assert(_pageState[0].PaginationProgress == 0, "{0}: Failed: PN={1}: PaginationProgress event: Expecting {2}, got {3}.", this.TestName, 0, 0, _pageState[0].PaginationProgress);
            DRT.Assert(_pageState[0].PagesChanged == 0, "{0}: Failed: PN={1}: PagesChanged event: Expecting {2}, got {3}.",             this.TestName, 0, 0, _pageState[0].PagesChanged);
            // Page 1 events
            DRT.Assert(_pageState[1].GetPageCompleted == 0, "{0}: Failed: PN={1}: GetPageCompleted event: Expecting {2}, got {3}.",     this.TestName, 1, 0, _pageState[1].GetPageCompleted);
            DRT.Assert(_pageState[1].PaginationProgress == 0, "{0}: Failed: PN={1}: PaginationProgress event: Expecting {2}, got {3}.", this.TestName, 1, 0, _pageState[1].PaginationProgress);
            DRT.Assert(_pageState[1].PagesChanged == 0, "{0}: Failed: PN={1}: PagesChanged event: Expecting {2}, got {3}.",             this.TestName, 1, 0, _pageState[1].PagesChanged);
            // Page 2 events
            DRT.Assert(_pageState[2].GetPageCompleted == 0, "{0}: Failed: PN={1}: GetPageCompleted event: Expecting {2}, got {3}.",     this.TestName, 2, 0, _pageState[2].GetPageCompleted);
            DRT.Assert(_pageState[2].PaginationProgress == 0, "{0}: Failed: PN={1}: PaginationProgress event: Expecting {2}, got {3}.", this.TestName, 2, 0, _pageState[2].PaginationProgress);
            DRT.Assert(_pageState[2].PagesChanged == 0, "{0}: Failed: PN={1}: PagesChanged event: Expecting {2}, got {3}.",             this.TestName, 2, 0, _pageState[2].PagesChanged);
            // Page 2 events
            DRT.Assert(_pageState[3].GetPageCompleted == 0, "{0}: Failed: PN={1}: GetPageCompleted event: Expecting {2}, got {3}.",     this.TestName, 3, 0, _pageState[3].GetPageCompleted);
            DRT.Assert(_pageState[3].PaginationProgress == 0, "{0}: Failed: PN={1}: PaginationProgress event: Expecting {2}, got {3}.", this.TestName, 3, 0, _pageState[3].PaginationProgress);
            DRT.Assert(_pageState[3].PagesChanged == 0, "{0}: Failed: PN={1}: PagesChanged event: Expecting {2}, got {3}.",             this.TestName, 3, 0, _pageState[3].PagesChanged);
        }

        // ------------------------------------------------------------------
        // ModifyPage0
        // ------------------------------------------------------------------
        private void ModifyPage0()
        {
            _testName = "ModifyPage0";
            ResetState();

            // Add content.
            Paragraph para = (Paragraph)DRT.FindElementByID("Para1", (DependencyObject)_paginator.Source);
            AppendBoldToParagraph(para, "Bold1");
        }

        // ------------------------------------------------------------------
        // VerifyModifyPage0
        // ------------------------------------------------------------------
        private void VerifyModifyPage0()
        {
            DRT.Assert(_pageView1.DocumentPage != DocumentPage.Missing, "{0}: Failed: Expecting valid DocumentPage for PN={1}.", this.TestName, 1);
            DRT.Assert(_pageView2.DocumentPage != DocumentPage.Missing, "{0}: Failed: Expecting valid DocumentPage for PN={1}.", this.TestName, 2);
            // Events
            DRT.Assert(_paginationCompleted, "{0}: Failed: Should receive PaginationCompleted event.", this.TestName);
            DRT.Assert(!_getPageNumberCompleted, "{0}: Failed: Should not receive GetPageNumberCompleted event.", this.TestName);
            // Page 0 events
            DRT.Assert(_pageState[0].GetPageCompleted == 0, "{0}: Failed: PN={1}: GetPageCompleted event: Expecting {2}, got {3}.",     this.TestName, 0, 0, _pageState[0].GetPageCompleted);
            DRT.Assert(_pageState[0].PaginationProgress == 1, "{0}: Failed: PN={1}: PaginationProgress event: Expecting {2}, got {3}.", this.TestName, 0, 1, _pageState[0].PaginationProgress);
            DRT.Assert(_pageState[0].PagesChanged == 1, "{0}: Failed: PN={1}: PagesChanged event: Expecting {2}, got {3}.",             this.TestName, 0, 1, _pageState[0].PagesChanged);
            // Page 1 events
            DRT.Assert(_pageState[1].GetPageCompleted == 1, "{0}: Failed: PN={1}: GetPageCompleted event: Expecting {2}, got {3}.",     this.TestName, 1, 1, _pageState[1].GetPageCompleted);
            DRT.Assert(_pageState[1].PaginationProgress == 1, "{0}: Failed: PN={1}: PaginationProgress event: Expecting {2}, got {3}.", this.TestName, 1, 1, _pageState[1].PaginationProgress);
            DRT.Assert(_pageState[1].PagesChanged == 1, "{0}: Failed: PN={1}: PagesChanged event: Expecting {2}, got {3}.",             this.TestName, 1, 1, _pageState[1].PagesChanged);
            // Page 2 events
            DRT.Assert(_pageState[2].GetPageCompleted == 1, "{0}: Failed: PN={1}: GetPageCompleted event: Expecting {2}, got {3}.",     this.TestName, 2, 1, _pageState[2].GetPageCompleted);
            DRT.Assert(_pageState[2].PaginationProgress == 1, "{0}: Failed: PN={1}: PaginationProgress event: Expecting {2}, got {3}.", this.TestName, 2, 1, _pageState[2].PaginationProgress);
            DRT.Assert(_pageState[2].PagesChanged == 1, "{0}: Failed: PN={1}: PagesChanged event: Expecting {2}, got {3}.",             this.TestName, 2, 1, _pageState[2].PagesChanged);
            // Page 2 events
            DRT.Assert(_pageState[3].GetPageCompleted == 0, "{0}: Failed: PN={1}: GetPageCompleted event: Expecting {2}, got {3}.",     this.TestName, 3, 0, _pageState[3].GetPageCompleted);
            DRT.Assert(_pageState[3].PaginationProgress == 1, "{0}: Failed: PN={1}: PaginationProgress event: Expecting {2}, got {3}.", this.TestName, 3, 0, _pageState[3].PaginationProgress);
            DRT.Assert(_pageState[3].PagesChanged == 1, "{0}: Failed: PN={1}: PagesChanged event: Expecting {2}, got {3}.",             this.TestName, 3, 1, _pageState[3].PagesChanged);
        }

        // ------------------------------------------------------------------
        // ModifyPage2
        // ------------------------------------------------------------------
        private void ModifyPage2()
        {
            _testName = "ModifyPage2";
            ResetState();

            // Add content.
            Paragraph para = (Paragraph)DRT.FindElementByID("Para6", (DependencyObject)_paginator.Source);
            AppendBoldToParagraph(para, "Bold6");
        }

        // ------------------------------------------------------------------
        // VerifyModifyPage2
        // ------------------------------------------------------------------
        private void VerifyModifyPage2()
        {
            DRT.Assert(_pageView1.DocumentPage != DocumentPage.Missing, "{0}: Failed: Expecting valid DocumentPage for PN={1}.", this.TestName, 1);
            DRT.Assert(_pageView2.DocumentPage != DocumentPage.Missing, "{0}: Failed: Expecting valid DocumentPage for PN={1}.", this.TestName, 2);
            // Events
            DRT.Assert(_paginationCompleted, "{0}: Failed: Should receive PaginationCompleted event.", this.TestName);
            DRT.Assert(!_getPageNumberCompleted, "{0}: Failed: Should not receive GetPageNumberCompleted event.", this.TestName);
            // Page 0 events
            DRT.Assert(_pageState[0].GetPageCompleted == 0, "{0}: Failed: PN={1}: GetPageCompleted event: Expecting {2}, got {3}.",     this.TestName, 0, 0, _pageState[0].GetPageCompleted);
            DRT.Assert(_pageState[0].PaginationProgress == 0, "{0}: Failed: PN={1}: PaginationProgress event: Expecting {2}, got {3}.", this.TestName, 0, 0, _pageState[0].PaginationProgress);
            DRT.Assert(_pageState[0].PagesChanged == 0, "{0}: Failed: PN={1}: PagesChanged event: Expecting {2}, got {3}.",             this.TestName, 0, 0, _pageState[0].PagesChanged);
            // Page 1 events
            DRT.Assert(_pageState[1].GetPageCompleted == 0, "{0}: Failed: PN={1}: GetPageCompleted event: Expecting {2}, got {3}.",     this.TestName, 1, 0, _pageState[1].GetPageCompleted);
            DRT.Assert(_pageState[1].PaginationProgress == 0, "{0}: Failed: PN={1}: PaginationProgress event: Expecting {2}, got {3}.", this.TestName, 1, 0, _pageState[1].PaginationProgress);
            DRT.Assert(_pageState[1].PagesChanged == 0, "{0}: Failed: PN={1}: PagesChanged event: Expecting {2}, got {3}.",             this.TestName, 1, 0, _pageState[1].PagesChanged);
            // Page 2 events
            DRT.Assert(_pageState[2].GetPageCompleted == 0, "{0}: Failed: PN={1}: GetPageCompleted event: Expecting {2}, got {3}.",     this.TestName, 2, 1, _pageState[2].GetPageCompleted);
            DRT.Assert(_pageState[2].PaginationProgress == 0, "{0}: Failed: PN={1}: PaginationProgress event: Expecting {2}, got {3}.", this.TestName, 2, 1, _pageState[2].PaginationProgress);
            DRT.Assert(_pageState[2].PagesChanged == 0, "{0}: Failed: PN={1}: PagesChanged event: Expecting {2}, got {3}.",             this.TestName, 2, 1, _pageState[2].PagesChanged);
            // Page 2 events
            DRT.Assert(_pageState[3].GetPageCompleted == 0, "{0}: Failed: PN={1}: GetPageCompleted event: Expecting {2}, got {3}.",     this.TestName, 3, 0, _pageState[3].GetPageCompleted);
            DRT.Assert(_pageState[3].PaginationProgress == 0, "{0}: Failed: PN={1}: PaginationProgress event: Expecting {2}, got {3}.", this.TestName, 3, 0, _pageState[3].PaginationProgress);
            DRT.Assert(_pageState[3].PagesChanged == 0, "{0}: Failed: PN={1}: PagesChanged event: Expecting {2}, got {3}.",             this.TestName, 3, 1, _pageState[3].PagesChanged);
        }

        // ------------------------------------------------------------------
        // ModifyPage3
        // ------------------------------------------------------------------
        private void ModifyPage3()
        {
            _testName = "ModifyPage3";
            ResetState();

            // Add content.
            Paragraph para = (Paragraph)DRT.FindElementByID("Para7", (DependencyObject)_paginator.Source);
            AppendBoldToParagraph(para, "Bold7");
        }

        // ------------------------------------------------------------------
        // VerifyModifyPage3
        // ------------------------------------------------------------------
        private void VerifyModifyPage3()
        {
            DRT.Assert(_pageView1.DocumentPage != DocumentPage.Missing, "{0}: Failed: Expecting valid DocumentPage for PN={1}.", this.TestName, 1);
            DRT.Assert(_pageView2.DocumentPage != DocumentPage.Missing, "{0}: Failed: Expecting valid DocumentPage for PN={1}.", this.TestName, 2);
            // Events
            DRT.Assert(_paginationCompleted, "{0}: Failed: Should receive PaginationCompleted event.", this.TestName);
            DRT.Assert(!_getPageNumberCompleted, "{0}: Failed: Should not receive GetPageNumberCompleted event.", this.TestName);
            // Page 0 events
            DRT.Assert(_pageState[0].GetPageCompleted == 0, "{0}: Failed: PN={1}: GetPageCompleted event: Expecting {2}, got {3}.",     this.TestName, 0, 0, _pageState[0].GetPageCompleted);
            DRT.Assert(_pageState[0].PaginationProgress == 0, "{0}: Failed: PN={1}: PaginationProgress event: Expecting {2}, got {3}.", this.TestName, 0, 0, _pageState[0].PaginationProgress);
            DRT.Assert(_pageState[0].PagesChanged == 0, "{0}: Failed: PN={1}: PagesChanged event: Expecting {2}, got {3}.",             this.TestName, 0, 0, _pageState[0].PagesChanged);
            // Page 1 events
            DRT.Assert(_pageState[1].GetPageCompleted == 0, "{0}: Failed: PN={1}: GetPageCompleted event: Expecting {2}, got {3}.",     this.TestName, 1, 0, _pageState[1].GetPageCompleted);
            DRT.Assert(_pageState[1].PaginationProgress == 0, "{0}: Failed: PN={1}: PaginationProgress event: Expecting {2}, got {3}.", this.TestName, 1, 0, _pageState[1].PaginationProgress);
            DRT.Assert(_pageState[1].PagesChanged == 0, "{0}: Failed: PN={1}: PagesChanged event: Expecting {2}, got {3}.",             this.TestName, 1, 0, _pageState[1].PagesChanged);
            // Page 2 events
            DRT.Assert(_pageState[2].GetPageCompleted == 0, "{0}: Failed: PN={1}: GetPageCompleted event: Expecting {2}, got {3}.",     this.TestName, 2, 0, _pageState[2].GetPageCompleted);
            DRT.Assert(_pageState[2].PaginationProgress == 0, "{0}: Failed: PN={1}: PaginationProgress event: Expecting {2}, got {3}.", this.TestName, 2, 0, _pageState[2].PaginationProgress);
            DRT.Assert(_pageState[2].PagesChanged == 0, "{0}: Failed: PN={1}: PagesChanged event: Expecting {2}, got {3}.",             this.TestName, 2, 0, _pageState[2].PagesChanged);
            // Page 2 events
            DRT.Assert(_pageState[3].GetPageCompleted == 0, "{0}: Failed: PN={1}: GetPageCompleted event: Expecting {2}, got {3}.",     this.TestName, 3, 0, _pageState[3].GetPageCompleted);
            DRT.Assert(_pageState[3].PaginationProgress == 0, "{0}: Failed: PN={1}: PaginationProgress event: Expecting {2}, got {3}.", this.TestName, 3, 0, _pageState[3].PaginationProgress);
            DRT.Assert(_pageState[3].PagesChanged == 0, "{0}: Failed: PN={1}: PagesChanged event: Expecting {2}, got {3}.",             this.TestName, 3, 0, _pageState[3].PagesChanged);
        }

        // ------------------------------------------------------------------
        // NavigateToLastPage
        // ------------------------------------------------------------------
        private void NavigateToLastPage()
        {
            _testName = "NavigateToLastPage";
            ResetState();

            // Store the current ContentPosition for page #1
            _page1ContentPosition = _paginator.GetPagePosition(_pageView1.DocumentPage);

            // Get ContentPosition for elemetn with ID=Bold7
            Bold bold = (Bold)DRT.FindElementByID("Bold7", (DependencyObject)_paginator.Source);
            ContentPosition contentPosition = _paginator.GetObjectPosition(bold);

            // Get page number for elemetn with ID=Bold7
            int pageNumber = _paginator.GetPageNumber(contentPosition);

            // Navigate to page
            _pageView1.PageNumber = pageNumber;
        }

        // ------------------------------------------------------------------
        // VerifyNavigateToLastPage
        // ------------------------------------------------------------------
        private void VerifyNavigateToLastPage()
        {
            DRT.Assert(_pageView1.PageNumber == 7, "{0}: Failed: Expecting PN={1}, got PN={2}.", this.TestName, 7, _pageView1.PageNumber);
            DRT.Assert(_pageView1.DocumentPage != DocumentPage.Missing, "{0}: Failed: Expecting valid DocumentPage for PN={1}.", this.TestName, 3);
            DRT.Assert(_pageView2.DocumentPage != DocumentPage.Missing, "{0}: Failed: Expecting valid DocumentPage for PN={1}.", this.TestName, 2);
            // Events
            DRT.Assert(!_paginationCompleted, "{0}: Failed: Should not receive PaginationCompleted event.", this.TestName);
            DRT.Assert(!_getPageNumberCompleted, "{0}: Failed: Should not receive GetPageNumberCompleted event.", this.TestName);
            // Page 0 events
            DRT.Assert(_pageState[0].GetPageCompleted == 0, "{0}: Failed: PN={1}: GetPageCompleted event: Expecting {2}, got {3}.",     this.TestName, 0, 0, _pageState[0].GetPageCompleted);
            DRT.Assert(_pageState[0].PaginationProgress == 0, "{0}: Failed: PN={1}: PaginationProgress event: Expecting {2}, got {3}.", this.TestName, 0, 0, _pageState[0].PaginationProgress);
            DRT.Assert(_pageState[0].PagesChanged == 0, "{0}: Failed: PN={1}: PagesChanged event: Expecting {2}, got {3}.",             this.TestName, 0, 0, _pageState[0].PagesChanged);
            // Page 1 events
            DRT.Assert(_pageState[1].GetPageCompleted == 0, "{0}: Failed: PN={1}: GetPageCompleted event: Expecting {2}, got {3}.",     this.TestName, 1, 0, _pageState[1].GetPageCompleted);
            DRT.Assert(_pageState[1].PaginationProgress == 0, "{0}: Failed: PN={1}: PaginationProgress event: Expecting {2}, got {3}.", this.TestName, 1, 0, _pageState[1].PaginationProgress);
            DRT.Assert(_pageState[1].PagesChanged == 0, "{0}: Failed: PN={1}: PagesChanged event: Expecting {2}, got {3}.",             this.TestName, 1, 0, _pageState[1].PagesChanged);
            // Page 2 events
            DRT.Assert(_pageState[2].GetPageCompleted == 0, "{0}: Failed: PN={1}: GetPageCompleted event: Expecting {2}, got {3}.",     this.TestName, 2, 0, _pageState[2].GetPageCompleted);
            DRT.Assert(_pageState[2].PaginationProgress == 0, "{0}: Failed: PN={1}: PaginationProgress event: Expecting {2}, got {3}.", this.TestName, 2, 0, _pageState[2].PaginationProgress);
            DRT.Assert(_pageState[2].PagesChanged == 0, "{0}: Failed: PN={1}: PagesChanged event: Expecting {2}, got {3}.",             this.TestName, 2, 0, _pageState[2].PagesChanged);
            // Page 2 events
            DRT.Assert(_pageState[3].GetPageCompleted == 0, "{0}: Failed: PN={1}: GetPageCompleted event: Expecting {2}, got {3}.",     this.TestName, 3, 1, _pageState[3].GetPageCompleted);
            DRT.Assert(_pageState[3].PaginationProgress == 0, "{0}: Failed: PN={1}: PaginationProgress event: Expecting {2}, got {3}.", this.TestName, 3, 1, _pageState[3].PaginationProgress);
            DRT.Assert(_pageState[3].PagesChanged == 0, "{0}: Failed: PN={1}: PagesChanged event: Expecting {2}, got {3}.",             this.TestName, 3, 0, _pageState[3].PagesChanged);
        }

        // ------------------------------------------------------------------
        // NavigateToFirstPageAsync
        // ------------------------------------------------------------------
        private void NavigateToFirstPageAsync()
        {
            _testName = "NavigateToFirstPageAsync";
            ResetState();

            // Get ContentPosition for elemetn with ID=Bold1
            Bold bold = (Bold)DRT.FindElementByID("Bold1", (DependencyObject)_paginator.Source);
            ContentPosition contentPosition = _paginator.GetObjectPosition(bold);

            // Get page number for elemetn with ID=Bold1
            _paginator.GetPageNumberAsync(contentPosition, this);

            // Navigate to page
            _pageView1.PageNumber = _pageNumberForContentPosition;
        }

        // ------------------------------------------------------------------
        // VerifyNavigateToFirstPageAsync
        // ------------------------------------------------------------------
        private void VerifyNavigateToFirstPageAsync()
        {
            DRT.Assert(_pageView1.PageNumber == 0, "{0}: Failed: Expecting PN={1}, got PN={2}.", this.TestName, 0, _pageView1.PageNumber);
            DRT.Assert(_pageView1.DocumentPage != DocumentPage.Missing, "{0}: Failed: Expecting valid DocumentPage for PN={1}.", this.TestName, 0);
            DRT.Assert(_pageView2.DocumentPage != DocumentPage.Missing, "{0}: Failed: Expecting valid DocumentPage for PN={1}.", this.TestName, 2);
            // Events
            DRT.Assert(!_paginationCompleted, "{0}: Failed: Should not receive PaginationCompleted event.", this.TestName);
            DRT.Assert(_getPageNumberCompleted, "{0}: Failed: Should receive GetPageNumberCompleted event.", this.TestName);
            // Page 0 events
            DRT.Assert(_pageState[0].GetPageCompleted == 1, "{0}: Failed: PN={1}: GetPageCompleted event: Expecting {2}, got {3}.",     this.TestName, 0, 1, _pageState[0].GetPageCompleted);
            DRT.Assert(_pageState[0].PaginationProgress == 0, "{0}: Failed: PN={1}: PaginationProgress event: Expecting {2}, got {3}.", this.TestName, 0, 0, _pageState[0].PaginationProgress);
            DRT.Assert(_pageState[0].PagesChanged == 0, "{0}: Failed: PN={1}: PagesChanged event: Expecting {2}, got {3}.",             this.TestName, 0, 0, _pageState[0].PagesChanged);
            // Page 1 events
            DRT.Assert(_pageState[1].GetPageCompleted == 0, "{0}: Failed: PN={1}: GetPageCompleted event: Expecting {2}, got {3}.",     this.TestName, 1, 0, _pageState[1].GetPageCompleted);
            DRT.Assert(_pageState[1].PaginationProgress == 0, "{0}: Failed: PN={1}: PaginationProgress event: Expecting {2}, got {3}.", this.TestName, 1, 0, _pageState[1].PaginationProgress);
            DRT.Assert(_pageState[1].PagesChanged == 0, "{0}: Failed: PN={1}: PagesChanged event: Expecting {2}, got {3}.",             this.TestName, 1, 0, _pageState[1].PagesChanged);
            // Page 2 events
            DRT.Assert(_pageState[2].GetPageCompleted == 0, "{0}: Failed: PN={1}: GetPageCompleted event: Expecting {2}, got {3}.",     this.TestName, 2, 0, _pageState[2].GetPageCompleted);
            DRT.Assert(_pageState[2].PaginationProgress == 0, "{0}: Failed: PN={1}: PaginationProgress event: Expecting {2}, got {3}.", this.TestName, 2, 0, _pageState[2].PaginationProgress);
            DRT.Assert(_pageState[2].PagesChanged == 0, "{0}: Failed: PN={1}: PagesChanged event: Expecting {2}, got {3}.",             this.TestName, 2, 0, _pageState[2].PagesChanged);
            // Page 2 events
            DRT.Assert(_pageState[3].GetPageCompleted == 0, "{0}: Failed: PN={1}: GetPageCompleted event: Expecting {2}, got {3}.",     this.TestName, 3, 0, _pageState[3].GetPageCompleted);
            DRT.Assert(_pageState[3].PaginationProgress == 0, "{0}: Failed: PN={1}: PaginationProgress event: Expecting {2}, got {3}.", this.TestName, 3, 0, _pageState[3].PaginationProgress);
            DRT.Assert(_pageState[3].PagesChanged == 0, "{0}: Failed: PN={1}: PagesChanged event: Expecting {2}, got {3}.",             this.TestName, 3, 0, _pageState[3].PagesChanged);
        }

        // ------------------------------------------------------------------
        // NavigateRestorePage1
        // ------------------------------------------------------------------
        private void NavigateRestorePage1()
        {
            _testName = "NavigateRestorePage1";
            ResetState();

            // Get page number for previously stored ContentPosition
            int pageNumber = _paginator.GetPageNumber(_page1ContentPosition);

            // Navigate to page
            _pageView1.PageNumber = pageNumber;
        }

        // ------------------------------------------------------------------
        // VerifyNavigateRestorePage1
        // ------------------------------------------------------------------
        private void VerifyNavigateRestorePage1()
        {
            DRT.Assert(_pageView1.PageNumber == 1, "{0}: Failed: Expecting PN={1}, got PN={2}.", this.TestName, 1, _pageView1.PageNumber);
            DRT.Assert(_pageView1.DocumentPage != DocumentPage.Missing, "{0}: Failed: Expecting valid DocumentPage for PN={1}.", this.TestName, 0);
            DRT.Assert(_pageView2.DocumentPage != DocumentPage.Missing, "{0}: Failed: Expecting valid DocumentPage for PN={1}.", this.TestName, 2);
            // Events
            DRT.Assert(!_paginationCompleted, "{0}: Failed: Should not receive PaginationCompleted event.", this.TestName);
            DRT.Assert(!_getPageNumberCompleted, "{0}: Failed: Should receive GetPageNumberCompleted event.", this.TestName);
            // Page 0 events
            DRT.Assert(_pageState[0].GetPageCompleted == 0, "{0}: Failed: PN={1}: GetPageCompleted event: Expecting {2}, got {3}.",     this.TestName, 0, 0, _pageState[0].GetPageCompleted);
            DRT.Assert(_pageState[0].PaginationProgress == 0, "{0}: Failed: PN={1}: PaginationProgress event: Expecting {2}, got {3}.", this.TestName, 0, 0, _pageState[0].PaginationProgress);
            DRT.Assert(_pageState[0].PagesChanged == 0, "{0}: Failed: PN={1}: PagesChanged event: Expecting {2}, got {3}.",             this.TestName, 0, 0, _pageState[0].PagesChanged);
            // Page 1 events
            DRT.Assert(_pageState[1].GetPageCompleted == 1, "{0}: Failed: PN={1}: GetPageCompleted event: Expecting {2}, got {3}.",     this.TestName, 1, 1, _pageState[1].GetPageCompleted);
            DRT.Assert(_pageState[1].PaginationProgress == 0, "{0}: Failed: PN={1}: PaginationProgress event: Expecting {2}, got {3}.", this.TestName, 1, 0, _pageState[1].PaginationProgress);
            DRT.Assert(_pageState[1].PagesChanged == 0, "{0}: Failed: PN={1}: PagesChanged event: Expecting {2}, got {3}.",             this.TestName, 1, 0, _pageState[1].PagesChanged);
            // Page 2 events
            DRT.Assert(_pageState[2].GetPageCompleted == 0, "{0}: Failed: PN={1}: GetPageCompleted event: Expecting {2}, got {3}.",     this.TestName, 2, 0, _pageState[2].GetPageCompleted);
            DRT.Assert(_pageState[2].PaginationProgress == 0, "{0}: Failed: PN={1}: PaginationProgress event: Expecting {2}, got {3}.", this.TestName, 2, 0, _pageState[2].PaginationProgress);
            DRT.Assert(_pageState[2].PagesChanged == 0, "{0}: Failed: PN={1}: PagesChanged event: Expecting {2}, got {3}.",             this.TestName, 2, 0, _pageState[2].PagesChanged);
            // Page 2 events
            DRT.Assert(_pageState[3].GetPageCompleted == 0, "{0}: Failed: PN={1}: GetPageCompleted event: Expecting {2}, got {3}.",     this.TestName, 3, 0, _pageState[3].GetPageCompleted);
            DRT.Assert(_pageState[3].PaginationProgress == 0, "{0}: Failed: PN={1}: PaginationProgress event: Expecting {2}, got {3}.", this.TestName, 3, 0, _pageState[3].PaginationProgress);
            DRT.Assert(_pageState[3].PagesChanged == 0, "{0}: Failed: PN={1}: PagesChanged event: Expecting {2}, got {3}.",             this.TestName, 3, 0, _pageState[3].PagesChanged);
        }

        // ------------------------------------------------------------------
        // RemoveFromPage2
        // ------------------------------------------------------------------
        private void RemoveFromPage2()
        {
            _testName = "RemoveFromPage2";
            ResetState();

            // Add content.
            Bold bold = (Bold)DRT.FindElementByID("Bold6", (DependencyObject)_paginator.Source);
            bold.SiblingInlines.Remove(bold);
        }

        // ------------------------------------------------------------------
        // VerifyRemoveFromPage2
        // ------------------------------------------------------------------
        private void VerifyRemoveFromPage2()
        {
            DRT.Assert(_pageView1.DocumentPage != DocumentPage.Missing, "{0}: Failed: Expecting valid DocumentPage for PN={1}.", this.TestName, 1);
            DRT.Assert(_pageView2.DocumentPage != DocumentPage.Missing, "{0}: Failed: Expecting valid DocumentPage for PN={1}.", this.TestName, 2);
            // Events
            DRT.Assert(_paginationCompleted, "{0}: Failed: Should receive PaginationCompleted event.", this.TestName);
            DRT.Assert(!_getPageNumberCompleted, "{0}: Failed: Should not receive GetPageNumberCompleted event.", this.TestName);
            // Page 0 events
            DRT.Assert(_pageState[0].GetPageCompleted == 0, "{0}: Failed: PN={1}: GetPageCompleted event: Expecting {2}, got {3}.",     this.TestName, 0, 0, _pageState[0].GetPageCompleted);
            DRT.Assert(_pageState[0].PaginationProgress == 0, "{0}: Failed: PN={1}: PaginationProgress event: Expecting {2}, got {3}.", this.TestName, 0, 0, _pageState[0].PaginationProgress);
            DRT.Assert(_pageState[0].PagesChanged == 0, "{0}: Failed: PN={1}: PagesChanged event: Expecting {2}, got {3}.",             this.TestName, 0, 0, _pageState[0].PagesChanged);
            // Page 1 events
            DRT.Assert(_pageState[1].GetPageCompleted == 0, "{0}: Failed: PN={1}: GetPageCompleted event: Expecting {2}, got {3}.",     this.TestName, 1, 0, _pageState[1].GetPageCompleted);
            DRT.Assert(_pageState[1].PaginationProgress == 0, "{0}: Failed: PN={1}: PaginationProgress event: Expecting {2}, got {3}.", this.TestName, 1, 0, _pageState[1].PaginationProgress);
            DRT.Assert(_pageState[1].PagesChanged == 0, "{0}: Failed: PN={1}: PagesChanged event: Expecting {2}, got {3}.",             this.TestName, 1, 0, _pageState[1].PagesChanged);
            // Page 2 events
            DRT.Assert(_pageState[2].GetPageCompleted == 0, "{0}: Failed: PN={1}: GetPageCompleted event: Expecting {2}, got {3}.",     this.TestName, 2, 1, _pageState[2].GetPageCompleted);
            DRT.Assert(_pageState[2].PaginationProgress == 0, "{0}: Failed: PN={1}: PaginationProgress event: Expecting {2}, got {3}.", this.TestName, 2, 1, _pageState[2].PaginationProgress);
            DRT.Assert(_pageState[2].PagesChanged == 0, "{0}: Failed: PN={1}: PagesChanged event: Expecting {2}, got {3}.",             this.TestName, 2, 1, _pageState[2].PagesChanged);
            // Page 2 events
            DRT.Assert(_pageState[3].GetPageCompleted == 0, "{0}: Failed: PN={1}: GetPageCompleted event: Expecting {2}, got {3}.",     this.TestName, 3, 0, _pageState[3].GetPageCompleted);
            DRT.Assert(_pageState[3].PaginationProgress == 0, "{0}: Failed: PN={1}: PaginationProgress event: Expecting {2}, got {3}.", this.TestName, 3, 0, _pageState[3].PaginationProgress);
            DRT.Assert(_pageState[3].PagesChanged == 0, "{0}: Failed: PN={1}: PagesChanged event: Expecting {2}, got {3}.",             this.TestName, 3, 1, _pageState[3].PagesChanged);
        }

        // ------------------------------------------------------------------
        // DeletePages
        // ------------------------------------------------------------------
        private void DeletePages()
        {
            _testName = "DeletePages";
            ResetState();

            // Delete content.
            Paragraph para;
            para = (Paragraph)DRT.FindElementByID("Para5", (DependencyObject)_paginator.Source);
            para.SiblingBlocks.Remove(para);
            para = (Paragraph)DRT.FindElementByID("Para7", (DependencyObject)_paginator.Source);
            para.SiblingBlocks.Remove(para);
            para = (Paragraph)DRT.FindElementByID("Para6", (DependencyObject)_paginator.Source);
            para.SiblingBlocks.Remove(para);
        }

        // ------------------------------------------------------------------
        // VerifyDeletePages
        // ------------------------------------------------------------------
        private void VerifyDeletePages()
        {
            DRT.Assert(_pageView1.DocumentPage != DocumentPage.Missing, "{0}: Failed: Expecting valid DocumentPage for PN={1}.", this.TestName, 1);
            DRT.Assert(_pageView2.DocumentPage != DocumentPage.Missing, "{0}: Failed: Expecting valid DocumentPage for PN={1}.", this.TestName, 2);
            // Events
            DRT.Assert(_paginationCompleted, "{0}: Failed: Should receive PaginationCompleted event.", this.TestName);
            DRT.Assert(!_getPageNumberCompleted, "{0}: Failed: Should not receive GetPageNumberCompleted event.", this.TestName);
            // Page 0 events
            DRT.Assert(_pageState[0].GetPageCompleted == 0, "{0}: Failed: PN={1}: GetPageCompleted event: Expecting {2}, got {3}.",     this.TestName, 0, 0, _pageState[0].GetPageCompleted);
            DRT.Assert(_pageState[0].PaginationProgress == 0, "{0}: Failed: PN={1}: PaginationProgress event: Expecting {2}, got {3}.", this.TestName, 0, 0, _pageState[0].PaginationProgress);
            DRT.Assert(_pageState[0].PagesChanged == 0, "{0}: Failed: PN={1}: PagesChanged event: Expecting {2}, got {3}.",             this.TestName, 0, 0, _pageState[0].PagesChanged);
            // Page 1 events
            DRT.Assert(_pageState[1].GetPageCompleted == 0, "{0}: Failed: PN={1}: GetPageCompleted event: Expecting {2}, got {3}.",     this.TestName, 1, 0, _pageState[1].GetPageCompleted);
            DRT.Assert(_pageState[1].PaginationProgress == 0, "{0}: Failed: PN={1}: PaginationProgress event: Expecting {2}, got {3}.", this.TestName, 1, 0, _pageState[1].PaginationProgress);
            DRT.Assert(_pageState[1].PagesChanged == 0, "{0}: Failed: PN={1}: PagesChanged event: Expecting {2}, got {3}.",             this.TestName, 1, 0, _pageState[1].PagesChanged);
            // Page 2 events
            DRT.Assert(_pageState[2].GetPageCompleted == 0, "{0}: Failed: PN={1}: GetPageCompleted event: Expecting {2}, got {3}.",     this.TestName, 2, 0, _pageState[2].GetPageCompleted);
            DRT.Assert(_pageState[2].PaginationProgress == 0, "{0}: Failed: PN={1}: PaginationProgress event: Expecting {2}, got {3}.", this.TestName, 2, 0, _pageState[2].PaginationProgress);
            DRT.Assert(_pageState[2].PagesChanged == 0, "{0}: Failed: PN={1}: PagesChanged event: Expecting {2}, got {3}.",             this.TestName, 2, 0, _pageState[2].PagesChanged);
            // Page 2 events
            DRT.Assert(_pageState[3].GetPageCompleted == 0, "{0}: Failed: PN={1}: GetPageCompleted event: Expecting {2}, got {3}.",     this.TestName, 3, 0, _pageState[3].GetPageCompleted);
            DRT.Assert(_pageState[3].PaginationProgress == 1, "{0}: Failed: PN={1}: PaginationProgress event: Expecting {2}, got {3}.", this.TestName, 3, 1, _pageState[3].PaginationProgress);
            DRT.Assert(_pageState[3].PagesChanged == 1, "{0}: Failed: PN={1}: PagesChanged event: Expecting {2}, got {3}.",             this.TestName, 3, 1, _pageState[3].PagesChanged);
        }

        // ------------------------------------------------------------------
        // AppendSimpleParagraph
        // ------------------------------------------------------------------
        private void AppendSimpleParagraph(BlockCollection blocks, string id)
        {
            Paragraph para = new Paragraph();
            para.Name = id;
            para.Margin = new Thickness(0, 0, 0, 10);
            blocks.Add(para);
            para.Inlines.Add(new Run("The first step in writing Longhorn Applications  is to equip your computer with a recent version of  the platform, "
                + "and then install the necessary development tools. This document takes you through the necessary procedures."
                + "The Windows Client Platform (WCP), formerly called Avalon. The first step you "
                + "need to take is thus to install a recent version. If you can't dedicate a computer to "
                + "Longhorn, you can have WindowsXP and Longhorn coexist on the same computer by installing Longhorn and any related "
                + "applications on a separate partition. This ensures that you do not overwrite files that may be needed by the other system. You "
                + "choose which system to run at boot time, and you must reboot to switch from one to the other."));
        }

        // ------------------------------------------------------------------
        // AppendBoldToParagraph
        // ------------------------------------------------------------------
        private void AppendBoldToParagraph(Paragraph para, string id)
        {
            Bold bold = new Bold(new Run("[]"));
            bold.Name = id;
            para.Inlines.Add(bold);
        }

        // ------------------------------------------------------------------
        // OnGetPageCompleted
        // ------------------------------------------------------------------
        private void OnGetPageCompleted(object sender, GetPageCompletedEventArgs e)
        {
            DRT.Assert(e.Error == null, "{0}: Failed: GetPageCompleted failed.", this.TestName);
            DRT.Assert(!e.Cancelled, "{0}: Failed: GetPageCompleted has been canceled.", this.TestName);
            DRT.Assert(e.UserState == _pageView1 || e.UserState == _pageView2, "{0}: Failed: GetPageCompleted returned unknown UserState.", this.TestName);
            _pageState[e.PageNumber].GetPageCompleted = _pageState[e.PageNumber].GetPageCompleted + 1;
        }

        // ------------------------------------------------------------------
        // OnGetPageNumberCompleted
        // ------------------------------------------------------------------
        private void OnGetPageNumberCompleted(object sender, GetPageNumberCompletedEventArgs e)
        {
            DRT.Assert(!_getPageNumberCompleted, "{0}: Failed: GetPageNumberCompleted has been already received.", this.TestName);
            _getPageNumberCompleted = true;
            _pageNumberForContentPosition = e.PageNumber;
        }

        // ------------------------------------------------------------------
        // OnPaginationCompleted
        // ------------------------------------------------------------------
        private void OnPaginationCompleted(object sender, EventArgs e)
        {
            DRT.Assert(!_paginationCompleted, "{0}: Failed: PaginationCompleted has been already received.", this.TestName);
            _paginationCompleted = true;
        }

        // ------------------------------------------------------------------
        // OnPaginationProgress
        // ------------------------------------------------------------------
        private void OnPaginationProgress(object sender, PaginationProgressEventArgs e)
        {
            DRT.Assert(e.Start >= 0 && (e.Start + e.Count) <=12, "{0}: Failed: PaginationProgress received for unexpected page range: Start={1}, Count={2}.", this.TestName, e.Start, e.Count);
            for (int i = e.Start; i < (e.Start + e.Count); i++)
            {
                _pageState[i].PaginationProgress = _pageState[i].PaginationProgress + 1;
            }
        }

        // ------------------------------------------------------------------
        // OnPagesChanged
        // ------------------------------------------------------------------
        private void OnPagesChanged(object sender, PagesChangedEventArgs e)
        {
            DRT.Assert(e.Start >= 0 && e.Start < 12, "{0}: Failed: PagesChanged received for unexpected page range: Start={1}, Count={2}.", this.TestName, e.Start, e.Count);
            int end = Math.Min(4, e.Start + e.Count);
            for (int i = e.Start; i < end; i++)
            {
                _pageState[i].PagesChanged = _pageState[i].PagesChanged + 1;
            }
        }

        // ------------------------------------------------------------------
        // ResetState
        // ------------------------------------------------------------------
        private void ResetState()
        {
            _pageState = new PageState[12];
            _getPageNumberCompleted = false;
            _paginationCompleted = false;
        }

        // ------------------------------------------------------------------
        // Private fields.
        // ------------------------------------------------------------------
        private DynamicDocumentPaginator _paginator;
        private DocumentPageView _pageView1;
        private DocumentPageView _pageView2;
        private PageState[] _pageState;
        private bool _getPageNumberCompleted;
        private bool _paginationCompleted;
        private ContentPosition _page1ContentPosition;
        private int _pageNumberForContentPosition;

        // ------------------------------------------------------------------
        // Private types.
        // ------------------------------------------------------------------
        private struct PageState
        {
            public int GetPageCompleted;
            public int PaginationProgress;
            public int PagesChanged;
        }
    }
}
