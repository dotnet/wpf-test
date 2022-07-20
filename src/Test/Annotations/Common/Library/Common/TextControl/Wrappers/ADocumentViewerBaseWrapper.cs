// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Base class for wrappers that contain DocumentViewerBase instances.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Reflection;
using System.Collections;
using System.Windows.Annotations;
using Annotations.Test.Reflection;
using System.Windows.Automation;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Annotations.Test.Framework;
using System.Collections.Generic;
using System.Security.Permissions;

namespace Avalon.Test.Annotations
{
	public abstract class ADocumentViewerBaseWrapper : ATextControlWrapper
    {
        #region Constructor

        protected ADocumentViewerBaseWrapper(DocumentViewerBase target)
            : base(target)
        {
            // Empty.
        }

        #endregion       

        #region Public Methods

        // Layout Actions.
        //
        abstract public void WholePageLayout();
        abstract public void PageWidthLayout();
        abstract public void PageLayout(int numPages);

        /// <summary>
        /// Change view so that these two pages are visible.
        /// </summary>
        abstract public void GoToPageRange(int firstPage, int secondPage);

        public override void GoToStart()
        {
            while (!PageIsVisible(0)) PageUp(1);
        }

        public override void GoToEnd()
        {
            while (!PageIsVisible(PageCount - 1)) PageDown(1);
        }        
        
        public override void BringIntoView(ISelectionData selection)
        {
            TextRange range = selection.GetSelectionAsTextRange(SelectionModule);
            GoToSelection(range);
        }

        /// <summary>
        /// Synchronously, without adjusting zoom or window size, try and position make the start and end page
        /// of this selection visible.  If it is not possible to view both, just make the first page visible.
        /// </summary>
        /// <param name="selection">Should be of internal type ITextRange.</param>
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        public void GoToSelection(object selection)
        {
            Assembly presentationFramework = typeof(TextPointer).Assembly;
            Type ITextRangeType = presentationFramework.GetType("System.Windows.Documents.ITextRange");
            ContentPosition startPosition = ITextRangeType.GetProperty("Start").GetValue(selection, null) as ContentPosition;
            ContentPosition endPosition = ITextRangeType.GetProperty("End").GetValue(selection, null) as ContentPosition;
            int startPage = (int)ReflectionHelper.InvokeMethod(ViewerBase.Document.DocumentPaginator, "GetPageNumber", new object[] { startPosition });
            int endPage = (int)ReflectionHelper.InvokeMethod(ViewerBase.Document.DocumentPaginator, "GetPageNumber", new object[] { endPosition });
            GoToPageRange(startPage, endPage);
        }

        /// <summary>
        /// Uses UIAutomation to get the bounds of all the visible pages.  Sorts by ascending x then y position.
        /// </summary>
        /// <returns>List of Rect objects that are the bounds of all visible pages ordered from small to large, x to y.</returns>
        public Rect[] GetBoundsOfVisiblePages()
        {
            Rect[] bounds = new Rect[ViewerBase.PageCount];
            IEnumerator<DocumentPageView> pageEnumerator = ViewerBase.PageViews.GetEnumerator();
            while (pageEnumerator.MoveNext())
            {
                bounds[pageEnumerator.Current.PageNumber] = UIAutomationModule.BoundingRectangle(pageEnumerator.Current);
            }
            return bounds;
        }

        // Helper methods that route directly to SelectionModule.
        //
        public TextRange SetSelection(int pageNumber, int startIdx, int length) { return SelectionModule.SetSelection(pageNumber, startIdx, length); }
        public TextRange SetSelection(int startPageNum, int startOffset, int endPageNum, int endOffset) { return SelectionModule.SetSelection(startPageNum, startOffset, endPageNum, endOffset); }
        public TextRange SetSelection(int startPageNum, PagePosition startPagePos, int endPageNum, PagePosition endPagePos) { return SelectionModule.SetSelection(startPageNum, startPagePos, endPageNum, endPagePos); }
        public TextRange SetSelection(int startPageNum, PagePosition startPagePos, int startOffset, int endPageNum, PagePosition endPagePos, int endOffset) { return SelectionModule.SetSelection(startPageNum, startPagePos, startOffset, endPageNum, endPagePos, endOffset); }
        public TextRange SetSelection(int pageNum, PagePosition startPos, int offset) { return SelectionModule.SetSelection(pageNum, startPos, offset); }
        public TextRange MakeTextRange(int pageNumber, int startIdx, int length) { return SelectionModule.MakeTextRange(pageNumber, startIdx, length); }
        public object MakeTextAnchor(int pageNumber, PagePosition startPos, int offset) { return MakeTextAnchor((TextRange)SelectionModule.SetSelection(pageNumber, startPos, offset)); }
        public object MakeTextAnchor(int pageNumber, int startIdx, int length) { return MakeTextAnchor((TextRange)SelectionModule.MakeTextRange(pageNumber, startIdx, length)); }

        public int PageLength(int page) { return SelectionModule.PageLength(page); }

        #endregion

        #region Protected Methods

        protected void WaitForFinalPageCount()
        {
            DispatcherHelper.DoEvents();
            while (ViewerBase.Document != null && !ViewerBase.Document.DocumentPaginator.IsPageCountValid)
                DispatcherHelper.DoEvents();
        }

        #endregion

        #region Properties

        public override IDocumentPaginatorSource Document
        {
            get
            {
                return ViewerBase.Document;
            }
            set
            {
                ViewerBase.Document = value;
                WaitForFinalPageCount();
            }
        }

        public DocumentViewerBase ViewerBase
        {
            get
            {
                return Target as DocumentViewerBase;
            }
        }

        public override int FirstVisiblePage
        {
            get
            {
                return ViewerBase.PageViews[0].PageNumber;
            }
        }

        public override int LastVisiblePage
        {
            get
            {
                return ViewerBase.PageViews[ViewerBase.PageViews.Count - 1].PageNumber;
            }
        }

        public override int PageCount
        {
            get
            {
                if (!ViewerBase.Document.DocumentPaginator.IsPageCountValid)
                    throw new Exception("Querying for number of pages but page count is not final.");
                return ViewerBase.Document.DocumentPaginator.PageCount;
            }
        }

        public new ADocumentViewerBaseSelector SelectionModule
        {
            get
            {
                return base.SelectionModule as ADocumentViewerBaseSelector; 
            }
            set { base.SelectionModule = value; }
        }

        #endregion
    }
}
