// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Test suite for pagination. 
//
//

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;

namespace DRT
{
    // ----------------------------------------------------------------------
    // Test suite for pagination.
    // ----------------------------------------------------------------------
    internal sealed class PaginationBasicSuite : LayoutSuite
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        internal PaginationBasicSuite() : base("PaginationBasic")
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
                new DrtTest(CreateEmpty),               new DrtTest(VerifyLayoutCreate),
                new DrtTest(SetContent),                new DrtTest(VerifyLayoutAppend),
                new DrtTest(AddFirstPara),              new DrtTest(VerifyLayoutAppend),
                new DrtTest(AddSecondPara),             new DrtTest(VerifyLayoutAppend),
                new DrtTest(ChangeColumnWidth),         new DrtTest(VerifyLayoutAppend),
                new DrtTest(RestoreColumnWidth),        new DrtTest(VerifyLayoutAppend),
                new DrtTest(ChangePageSize),            new DrtTest(VerifyLayoutAppend),
                new DrtTest(ChangePagePadding),         new DrtTest(VerifyLayoutAppend),
                new DrtTest(ChangeMinPageSize),         new DrtTest(VerifyLayoutAppend),
                new DrtTest(ChangeMaxPageSize),         new DrtTest(VerifyLayoutFinalize),
            };
        }

        // ------------------------------------------------------------------
        // Load simple content.
        // ------------------------------------------------------------------
        private void CreateEmpty()
        {
            _testName = "";

            _pageView1 = new DocumentPageView();
            _pageView1.PageNumber = 0;
            _pageView2 = new DocumentPageView();
            _pageView2.PageNumber = 1;

            Grid container = new Grid();
            container.Background = Brushes.LightGreen;
            container.Children.Add(_pageView1);
            container.Children.Add(_pageView2);
            Grid.SetColumn(_pageView1, 0);
            _pageView1.HorizontalAlignment = HorizontalAlignment.Left;
            _pageView1.VerticalAlignment = VerticalAlignment.Top;

            Grid.SetColumn(_pageView2, 1);
            _pageView2.HorizontalAlignment = HorizontalAlignment.Right;
            _pageView2.VerticalAlignment = VerticalAlignment.Bottom;

            _contentRoot.Child = container;
        }

        // ------------------------------------------------------------------
        // SetContent
        // ------------------------------------------------------------------
        private void SetContent()
        {
            FlowDocument document = new FlowDocument();
            document.Background = Brushes.LightBlue;
            document.FontSize = 16.0;
            document.FontFamily = new FontFamily("Georgia");
            _paginator = ((IDocumentPaginatorSource)document).DocumentPaginator as DynamicDocumentPaginator;
            _paginator.PageSize = new Size(350, 500);
            _pageView1.DocumentPaginator = _paginator;
            _pageView2.DocumentPaginator = _paginator;
        }

        // ------------------------------------------------------------------
        // AddFirstPara
        // ------------------------------------------------------------------
        private void AddFirstPara()
        {
            _para1 = new Paragraph();
            ((FlowDocument)_paginator.Source).Blocks.Add(_para1);
            _para1.Inlines.Add(new Run("The first step in writing Longhorn Applications  is to equip your computer with a recent version of  the platform, "
                + "and then install the necessary development tools. This document takes you through the necessary procedures."
                + "The Windows Client Platform (WCP), formerly called Avalon. The first step you need to take  "
                + "is thus to install a recent version.. If you can't dedicate a computer to "
                + "Longhorn, you can have WindowsXP and Longhorn coexist on the same computer by installing Longhorn and any related "
                + "applications on a separate partition. This ensures that you do not overwrite files that may be needed by the other system. You "
                + "choose which system to run at boot time, and you must reboot to switch from one to the other."));
        }

        // ------------------------------------------------------------------
        // AddSecondPara
        // ------------------------------------------------------------------
        private void AddSecondPara()
        {
            _para2 = new Paragraph();
            ((FlowDocument)_paginator.Source).Blocks.Add(_para2);
            _para2.Inlines.Add(new Run("To get the most recent WCP bits. The preferred way to install Longhorn is with "
                + "IBS. To install the latest Longhorn Professional build with IBS,. This will take you to a page with information on the build status, and a link that you can click to run "
                + "the installation application. The Clean Install option is currently the most reliable; upgrade installs may not work properly. For further "
                + "information on builds"));
            _para2.Inlines.Add(new LineBreak());
            _para2.Inlines.Add(new Run("Note: When you log in for the first time, WinFS needs about 30 minutes to initialize. You should wait until that process is complete "
                + "before attempting to use the machine. Installing software, in particular, may have unpredictable effects."));
            _para2.Inlines.Add(new LineBreak());
            _para2.Inlines.Add(new Run("If you have a dual-boot configuration, you can install Longhorn without disturbing your other operating system. When the setup "
                + "Wizard asks which partition to use, select the Longhorn partition that you created when you set up the dual-boot configuration, "
                + "typically the D: drive. If you have used that partition previously, you should have the installation wizard reformat it."));
        }

        // ------------------------------------------------------------------
        // ChangeColumnWidth
        // ------------------------------------------------------------------
        private void ChangeColumnWidth()
        {
            ((FlowDocument)_paginator.Source).ColumnWidth = 150;
            ((FlowDocument)_paginator.Source).ColumnRuleWidth = 2;
            ((FlowDocument)_paginator.Source).ColumnRuleBrush = Brushes.DarkBlue;
            ((FlowDocument)_paginator.Source).ColumnGap = 10;
        }

        // ------------------------------------------------------------------
        // RestoreColumnWidth
        // ------------------------------------------------------------------
        private void RestoreColumnWidth()
        {
            ((FlowDocument)_paginator.Source).ColumnWidth = double.NaN;
        }

        // ------------------------------------------------------------------
        // ChangePageSize
        // ------------------------------------------------------------------
        private void ChangePageSize()
        {
            _paginator.PageSize = new Size(600, 900);
            ((FlowDocument)_paginator.Source).PageWidth = 300;
            ((FlowDocument)_paginator.Source).PageHeight = 450;
        }

        // ------------------------------------------------------------------
        // ChangePagePadding
        // ------------------------------------------------------------------
        private void ChangePagePadding()
        {
            _paginator.PageSize = new Size(30, 45);
            ((FlowDocument)_paginator.Source).PagePadding = new Thickness(25);
        }

        // ------------------------------------------------------------------
        // ChangeMinPageSize
        // ------------------------------------------------------------------
        private void ChangeMinPageSize()
        {
            ((FlowDocument)_paginator.Source).PageWidth = double.NaN;
            ((FlowDocument)_paginator.Source).PageHeight = double.NaN;
            ((FlowDocument)_paginator.Source).MinPageWidth = 250;
            ((FlowDocument)_paginator.Source).MinPageHeight = 400;
        }

        // ------------------------------------------------------------------
        // ChangeMaxPageSize
        // ------------------------------------------------------------------
        private void ChangeMaxPageSize()
        {
            _paginator.PageSize = new Size(600, 900);
            ((FlowDocument)_paginator.Source).MinPageWidth = 0;
            ((FlowDocument)_paginator.Source).MinPageHeight = 0;
            ((FlowDocument)_paginator.Source).MaxPageWidth = 300;
            ((FlowDocument)_paginator.Source).MaxPageHeight = 450;
        }

        // ------------------------------------------------------------------
        // Private fields.
        // ------------------------------------------------------------------
        private DynamicDocumentPaginator _paginator;
        private DocumentPageView _pageView1;
        private DocumentPageView _pageView2;
        private Paragraph _para1, _para2;
    }
}
