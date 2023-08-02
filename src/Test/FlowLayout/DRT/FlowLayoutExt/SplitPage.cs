// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Simple space partitioning UIElement. 
//
//

using System;
using System.Threading;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;

namespace DRT
{
    // ----------------------------------------------------------------------
    // Simple paginator. Creates 2 pages and reflows its child inside them.
    // ----------------------------------------------------------------------
    internal sealed class SplitPage : FrameworkElement
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        internal SplitPage()
        {
            Init();
        }

        // ------------------------------------------------------------------
        // Content measurement.
        //
        //      constraint - Constraint size.
        //
        // Returns: Computed desired size.
        // ------------------------------------------------------------------
        protected override Size MeasureOverride(Size constraint)
        {
            if (_content != null)
            {
                Size pageSize = new Size(Math.Max(0.0, (constraint.Width - 30.0) / 2), Math.Max(0.0, constraint.Height - 20.0));
                if (Math.Abs(pageSize.Width - _content.DocumentPaginator.PageSize.Width) >= 1 ||
                    Math.Abs(pageSize.Height - _content.DocumentPaginator.PageSize.Height) >= 1)
                {
                    _content.DocumentPaginator.PageSize = pageSize;
                }
                _page1.Measure(pageSize);
                _page2.Measure(pageSize);
            }
            return constraint;
        }

        // ------------------------------------------------------------------
        // Content arrangement.
        // ------------------------------------------------------------------
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            _children.Clear();

            if (_content != null)
            {
                Size pageSize = new Size(Math.Max(0.0, (arrangeSize.Width - 30.0) / 2), Math.Max(0.0, arrangeSize.Height - 20.0));
                Rect pageRect = new Rect(0, 0, pageSize.Width, pageSize.Height);
                _page1.Arrange(pageRect);
                _page2.Arrange(pageRect);

                if (_page1.DocumentPage != DocumentPage.Missing)
                {
                    _children.Add(_page1);
                    //Transform is defined in the class MyDocumentPageView
                    _page1.Transform = new TranslateTransform(10, 10);
                }
                if (_page2.DocumentPage != DocumentPage.Missing)
                {
                    _children.Add(_page2);
                    _page2.Transform = new TranslateTransform(20 + pageSize.Width, 10);
                }
            }
            return arrangeSize;
        }

        // ------------------------------------------------------------------
        // Render control's content.
        //
        //      ctx - Drawing context.
        // ------------------------------------------------------------------
        protected override void OnRender(DrawingContext ctx)
        {
            // Background
            ctx.DrawRectangle(Brushes.DarkGray, null, new Rect(new Point(), RenderSize));

            // Page frames
            Size pageSize = new Size(Math.Max(0.0, (RenderSize.Width - 30.0) / 2), Math.Max(0.0, RenderSize.Height - 20.0));
            ctx.DrawRectangle(_page1.DocumentPage != DocumentPage.Missing ? Brushes.White : Brushes.LightGray, new Pen(Brushes.Black, 1), new Rect(10, 10, pageSize.Width, pageSize.Height));
            ctx.DrawRectangle(_page2.DocumentPage != DocumentPage.Missing ? Brushes.White : Brushes.LightGray, new Pen(Brushes.Black, 1), new Rect(20 + pageSize.Width, 10, pageSize.Width, pageSize.Height));
        }

        /// <summary>
        ///   Derived class must implement to support Visual children. The method must return
        ///    the child at the specified index. Index must be between 0 and GetVisualChildrenCount-1.
        ///
        ///    By default a Visual does not have any children.
        ///
        ///  Remark: 
        ///       During this virtual call it is not valid to modify the Visual tree. 
        /// </summary>
        protected override Visual GetVisualChild(int index)
        {            
            // _children is initialized by constructor so it will never be null
            if(index < 0 || index >= _children.Count)
            {
                throw new ArgumentOutOfRangeException("index is out of range");
            }

            return _children[index];
        }
        
        /// <summary>
        ///  Derived classes override this property to enable the Visual code to enumerate 
        ///  the Visual children. Derived classes need to return the number of children
        ///  from this method.
        ///
        ///    By default a Visual does not have any children.
        ///
        ///  Remark: During this virtual method the Visual tree must not be modified.
        /// </summary>        
        protected override int VisualChildrenCount
        {           
            get 
            { 
                // _children is initialized by constructor so it will never be null        
                return _children.Count; 
            }
        }


        // ------------------------------------------------------------------
        // Returns new array of document pages.
        // ------------------------------------------------------------------
        internal DocumentPage[] GetPages()
        {
            DocumentPage[] pages = null;
            if (_page1.DocumentPage != DocumentPage.Missing)
            {
                if (_page2.DocumentPage != DocumentPage.Missing)
                {
                    pages = new DocumentPage[2];
                    pages[0] = _page1.DocumentPage;
                    pages[1] = _page2.DocumentPage;
                }
                else
                {
                    pages = new DocumentPage[1];
                    pages[0] = _page1.DocumentPage;
                }
            }
            return pages;
        }

        // ------------------------------------------------------------------
        // Initialize split page.
        // ------------------------------------------------------------------
        private void Init()
        {
            _children = new VisualCollection(this);                
            _page1 = new MyDocumentPageView();
            _page1.PageNumber = 0;
            _page2 = new MyDocumentPageView();
            _page2.PageNumber = 1;
        }

        // ------------------------------------------------------------------
        // Content document formatter.
        // ------------------------------------------------------------------
        internal IDocumentPaginatorSource Content
        {
            set
            {
                if (_content != value)
                {
                    _content = value;
                    _page1.DocumentPaginator = _content.DocumentPaginator;
                    _page2.DocumentPaginator = _content.DocumentPaginator;
                }
            }
        }

        private IDocumentPaginatorSource _content;
        private VisualCollection _children;

        // ------------------------------------------------------------------
        // Document pages for the content.
        // ------------------------------------------------------------------
 //change it to MyDocumentPageView. 
        private MyDocumentPageView _page1, _page2;
    }
}

