// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Customized DocumentPaginator. 
//

using System;
using System.Windows;               // FrameworkContentElement
using System.Windows.Controls;      // Border
using System.Windows.Documents;     // DocumentPaginator
using System.Windows.Media;         // FontFamily

namespace DRT
{
    /// <summary>
    /// Customized IDocumentPaginatorSource.
    /// </summary>
    internal sealed class CustomDocumentPaginatorSource : FrameworkContentElement, IDocumentPaginatorSource
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        internal CustomDocumentPaginatorSource()
        {
            _paginator = new CustomDocumentPaginator(this);
        }

        /// <summary>
        /// An object which paginates content.
        /// </summary>
        public DocumentPaginator DocumentPaginator
        {
            get { return _paginator; }
        }

        /// <summary>
        /// An object which paginates content.
        /// </summary>
        private CustomDocumentPaginator _paginator;
    }

    /// <summary>
    /// Customized DocumentPaginator.
    /// </summary>
    internal sealed class CustomDocumentPaginator : DocumentPaginator
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        internal CustomDocumentPaginator(IDocumentPaginatorSource source)
        {
            _source = source;
            _pageCount = 10;
        }

        /// <summary>
        /// <see cref="System.Windows.Documents.DocumentPaginator.GetPage"/>
        /// </summary>
        public override DocumentPage GetPage(int pageNumber)
        {
            if (pageNumber >= _pageCount)
            {
                return DocumentPage.Missing;
            }

            pageNumber++;

            TextBlock textBlock = new TextBlock();
            textBlock.Text = pageNumber.ToString();
            textBlock.FontSize = 100;
            textBlock.FontFamily = new FontFamily("Lucida Handwriting");
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            textBlock.HorizontalAlignment = HorizontalAlignment.Center;

            Border border = new Border();
            border.Background = Brushes.LightGreen;
            border.Width = PageSize.Width;
            border.Height = PageSize.Height;
            border.Child = textBlock;

            border.Measure(PageSize);
            border.Arrange(new Rect(PageSize));

            return new DocumentPage(border);
        }

        /// <summary>
        /// <see cref="System.Windows.Documents.DocumentPaginator.IsPageCountValid"/>
        /// </summary>
        public override bool IsPageCountValid { get { return true; } }

        /// <summary>
        /// <see cref="System.Windows.Documents.DocumentPaginator.PageCount"/>
        /// </summary>
        public override int PageCount { get { return _pageCount;  } }

        /// <summary>
        /// <see cref="System.Windows.Documents.DocumentPaginator.PageSize"/>
        /// </summary>
        public override Size PageSize
        {
            get { return new Size(200, 400);  }
            set { }
        }

        /// <summary>
        /// <see cref="System.Windows.Documents.DocumentPaginator.Source"/>
        /// </summary>
        public override IDocumentPaginatorSource Source { get { return _source; } }

        private IDocumentPaginatorSource _source;
        private int _pageCount;
    }

    /// <summary>
    /// Customized IDocumentPaginatorSource.
    /// </summary>
    internal sealed class CustomDynamicDocumentPaginatorSource : FrameworkContentElement, IDocumentPaginatorSource
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        internal CustomDynamicDocumentPaginatorSource()
        {
            _paginator = new CustomDynamicDocumentPaginator(this);
        }

        /// <summary>
        /// An object which paginates content.
        /// </summary>
        public DocumentPaginator DocumentPaginator
        {
            get { return _paginator; }
        }

        /// <summary>
        /// An object which paginates content.
        /// </summary>
        private CustomDynamicDocumentPaginator _paginator;
    }

    /// <summary>
    /// Customized DocumentPaginator.
    /// </summary>
    internal sealed class CustomDynamicDocumentPaginator : DynamicDocumentPaginator
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        internal CustomDynamicDocumentPaginator(IDocumentPaginatorSource source)
        {
            _source = source;
            _pageCount = 4;
            _lastPage = -1;
        }

        /// <summary>
        /// <see cref="System.Windows.Documents.DocumentPaginator.GetPage"/>
        /// </summary>
        public override DocumentPage GetPage(int pageNumber)
        {
            if (pageNumber >= _pageCount)
            {
                return DocumentPage.Missing;
            }

            if (pageNumber > _lastPage)
            {
                _lastPage = pageNumber;
                OnPaginationProgress(new PaginationProgressEventArgs(pageNumber, 1));
            }
            if (IsPageCountValid)
            {
                OnPaginationCompleted(EventArgs.Empty);
            }
            pageNumber++;

            TextBlock textBlock = new TextBlock();
            textBlock.Text = pageNumber.ToString();
            textBlock.FontSize = 100;
            textBlock.FontFamily = new FontFamily("Lucida Handwriting");
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            textBlock.HorizontalAlignment = HorizontalAlignment.Center;

            Border border = new Border();
            border.Background = Brushes.LawnGreen;
            border.Width = PageSize.Width;
            border.Height = PageSize.Height;
            border.Child = textBlock;

            border.Measure(PageSize);
            border.Arrange(new Rect(PageSize));

            return new CustomDocumentPage(border, pageNumber-1);
        }

        /// <summary>
        /// <see cref="System.Windows.Documents.DynamicDocumentPaginator.GetPageNumber"/>
        /// </summary>
        public override int GetPageNumber(ContentPosition contentPosition)
        {
            if (!(contentPosition is CustomContentPosition))
            {
                throw new ArgumentException("Expecting CustomContentPosition.", "contentPosition");
            }
            return ((CustomContentPosition)contentPosition).PageNumber;
        }

        /// <summary>
        /// <see cref="System.Windows.Documents.DynamicDocumentPaginator.GetPagePosition"/>
        /// </summary>
        public override ContentPosition GetPagePosition(DocumentPage page)
        {
            if (!(page is CustomDocumentPage))
            {
                throw new ArgumentException("Expecting CustomDocumentPage.", "page");
            }
            return new CustomContentPosition(((CustomDocumentPage)page).PageNumber);
        }

        /// <summary>
        /// <see cref="System.Windows.Documents.DynamicDocumentPaginator.GetObjectPosition"/>
        /// </summary>
        public override ContentPosition GetObjectPosition(Object o)
        {
            return ContentPosition.Missing;
        }

        /// <summary>
        /// <see cref="System.Windows.Documents.DocumentPaginator.IsPageCountValid"/>
        /// </summary>
        public override bool IsPageCountValid
        {
            get { return (_lastPage == _pageCount - 1); }
        }

        /// <summary>
        /// <see cref="System.Windows.Documents.DocumentPaginator.PageCount"/>
        /// </summary>
        public override int PageCount { get { return _lastPage + 1; } }

        /// <summary>
        /// <see cref="System.Windows.Documents.DocumentPaginator.PageSize"/>
        /// </summary>
        public override Size PageSize
        {
            get { return new Size(200, 400);  }
            set { }
        }

        /// <summary>
        /// <see cref="System.Windows.Documents.DocumentPaginator.Source"/>
        /// </summary>
        public override IDocumentPaginatorSource Source { get { return _source; } }

        private IDocumentPaginatorSource _source;
        private int _pageCount;
        private int _lastPage;

        private class CustomDocumentPage : DocumentPage
        {
            internal CustomDocumentPage(Visual visual, int pageNumber)
                : base(visual)
            {
                _pageNumber = pageNumber;
            }
            internal int PageNumber { get { return _pageNumber; } }
            private int _pageNumber;
        }

        private class CustomContentPosition : ContentPosition
        {
            internal CustomContentPosition(int pageNumber)
            {
                _pageNumber = pageNumber;
            }
            internal int PageNumber { get { return _pageNumber; } }
            private int _pageNumber;
        }
    }
}
