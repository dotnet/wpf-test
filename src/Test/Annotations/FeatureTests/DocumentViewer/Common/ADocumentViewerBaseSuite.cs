// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Common base class that can be shared across all TestSuites


using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.IO;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Reflection;
using Annotations.Test.Framework;
using Annotations.Test;
using Annotations.Test.Reflection;
using System.Xml;
using System.Text.RegularExpressions;
using System.Windows.Controls.Primitives;
using Proxies.System.Windows.Annotations;
using System.Windows.Annotations.Storage;
using Proxies.MS.Internal.Annotations;

namespace Avalon.Test.Annotations
{
    public abstract class ADocumentViewerSuite : ATextControlTestSuite
    {
        #region General Methods

        protected void RunScript(AsyncTestScript script)
        {
            AsyncTestScriptRunner runner = new AsyncTestScriptRunner(this);
            runner.Run(script, true);
        }

        #endregion

        #region Setup Methods

        /// <summary>
        /// Generate default window layout: 
        /// -control for performing simple selections and annotation actions.
        /// -TextControl specific to content type.
        /// </summary>
        /// <returns>Root element of window contents that can be passed to Window.Content.</returns>
        protected override object CreateWindowContents()
        {
            PageSelectionControl selectionControl = new PageSelectionControl(DocViewerWrapper);
            UIElement dvsControl = selectionControl.Create();
            DockPanel.SetDock(dvsControl, Dock.Top);            

            DockPanel mainPanel = new DockPanel();
            mainPanel.Children.Add(dvsControl);
            mainPanel.Children.Add(TextControl);

            return mainPanel;
        }

        #endregion

        #region Navigation Actions

        protected void GoToSelection()
        {
            DocViewerWrapper.GoToSelection(DocViewerWrapper.Selection);
        }

        public void ScrollToPreviousPage()
        {
            int targetPage = DocViewerWrapper.FirstVisiblePage - 1;
            while (DocViewerWrapper.FirstVisiblePage > targetPage && DocViewerWrapper.FirstVisiblePage > 0)
                DocViewerWrapper.ScrollUp(1);
        }

        public void ScrollToNextPage()
        {
            int targetPage = DocViewerWrapper.LastVisiblePage + 1;
            while (DocViewerWrapper.LastVisiblePage < targetPage && DocViewerWrapper.LastVisiblePage < DocViewerWrapper.PageCount)
                DocViewerWrapper.ScrollDown(1);
        }

        protected void VerifyPagesAreVisible(int[] pages)
        {
            for (int i = 0; i < pages.Length; i++)
                VerifyPageIsVisible(pages[i]);
        }

        protected void VerifyPageIsVisible(int pageNum)
        {
            Assert(
                "Verify page '" + pageNum + "' is visible. Pages " + DocViewerWrapper.FirstVisiblePage + " to " + DocViewerWrapper.LastVisiblePage + " are visible.",
                DocViewerWrapper.PageIsVisible(pageNum));
        }

        protected void VerifyPageIsNotVisible(int pageNum)
        {
            Assert(
                "Verify page '" + pageNum + "' is NOT visible. Pages " + DocViewerWrapper.FirstVisiblePage + " to " + DocViewerWrapper.LastVisiblePage + " are visible.",
                !DocViewerWrapper.PageIsVisible(pageNum));
        }
        protected void VerifyPagesAreNotVisible(int[] pages)
        {
            for (int i = 0; i < pages.Length; i++)
                VerifyPageIsNotVisible(pages[i]);
        }

        protected void VerifyVisiblePages(int startPage, int endPage)
        {
            PrintVisiblePages();
            AssertEquals("Verify FirstVisiblePage.", startPage, DocViewerWrapper.FirstVisiblePage);
            AssertEquals("Verify LastVisiblePage.", endPage, DocViewerWrapper.LastVisiblePage);
        }

        protected void PrintVisiblePages()
        {
            printStatus("Pages " + DocViewerWrapper.FirstVisiblePage + " to " + DocViewerWrapper.LastVisiblePage + " are visible.");
        }

        #endregion Navigation Actions

        #region Annotation Actions

        #region Create Overloads

        protected void CreateAnnotation(int pageNum, int startPosition, int offset)
        {
            DoCreateAnnotation(DocViewerWrapper.SetSelection(pageNum, startPosition, offset));
        }

        protected void CreateAnnotation(int pageNum, PagePosition startPos, int offset)
        {
            DoCreateAnnotation(DocViewerWrapper.SetSelection(pageNum, startPos, offset));
        }

        protected void CreateAnnotation(int startPageNum, PagePosition startPos, int startOffset, int endPageNum, PagePosition endPos, int endOffset)
        {
            DoCreateAnnotation(DocViewerWrapper.SetSelection(startPageNum, startPos, startOffset, endPageNum, endPos, endOffset));
        }

        #endregion

        #region Delete Overloads

        protected void DeleteAnnotation(int pageNum, int startPosition, int offset)
        {
            DoDeleteAnnotation(DocViewerWrapper.SetSelection(pageNum, startPosition, offset));
        }

        protected void DeleteAnnotation(int pageNum, PagePosition startPos, int offset)
        {
            DoDeleteAnnotation(DocViewerWrapper.SetSelection(pageNum, startPos, offset));
        }

        protected void DeleteAnnotation(int startPageNum, PagePosition startPos, int startOffset, int endPageNum, PagePosition endPos, int endOffset)
        {
            DoDeleteAnnotation(DocViewerWrapper.SetSelection(startPageNum, startPos, startOffset, endPageNum, endPos, endOffset));
        }

        #endregion

        protected object MakeTextAnchor(int pageNumber, int startIdx, int length)
        {
            return DocViewerWrapper.MakeTextAnchor(pageNumber, startIdx, length);
        }

        protected object MakeTextAnchor(int pageNumber, PagePosition startPos, int offset)
        {
            return DocViewerWrapper.MakeTextAnchor(pageNumber, startPos, offset);
        }

        protected object MakeTextAnchor(TextRange textRange)
        {
            return DocViewerWrapper.MakeTextAnchor(textRange);
        }

        #endregion

        #region Text Actions

        /// <summary>
        /// For paginated content, if the anchor spans multiple pages we need to break it
        /// into page segments and then concatenate the text otherwise we may get undesired
        /// characters that appear between pages and are not included in annotation anchors.
        /// </summary>
        public override string GetText(ISelectionData selection)
        {
            MultiPageSelectionData multiPageSelection = selection as MultiPageSelectionData;
            if (multiPageSelection != null)
            {
                string retStr = "";
                ISelectionData[] selectionParts = multiPageSelection.GetPageBasedSelection();
                foreach (ISelectionData part in selectionParts)
                {
                    retStr += part.GetSelection(TextControlWrapper.SelectionModule);
                }
                return retStr;
            }

            return selection.GetSelection(TextControlWrapper.SelectionModule);
        }

        /// <summary>
        /// Special method for getting the text that is associated with the
        /// first page of a multipage selection.  This is needed because of
        /// differences between fixed and flow's algorithms.  Fixed will contain
        /// a '\r\n' at the end of the first page's anchor, whereas flow will not.
        /// </summary>
        public string GetFirstPageText(ISelectionData selection)
        {
            string text = GetText(selection);
            if (ContentMode == TestMode.Fixed)
                text += "\r\n";
            return text;
        }

        #endregion

        #region Wrapper Delegates

        protected void WholePageLayout()
        {
            printStatus("WholePageLayout()");
            DocViewerWrapper.WholePageLayout();
        }
        protected void PageWidthLayout()
        {
            printStatus("PageWidthLayout()");
            DocViewerWrapper.PageWidthLayout();
        }
        protected void PageLayout(int numPages)
        {
            printStatus("PageLayout(" + numPages + ")");
            DocViewerWrapper.PageLayout(numPages);
        }
        new protected void SetZoom(double zoomPercent)
        {
            printStatus("SetZoom(" + zoomPercent + ")");
            DocViewerWrapper.SetZoom(zoomPercent);
        }        
        public void GoToLastPage()
        {
            printStatus("GoToLastPage()");
            DocViewerWrapper.GoToEnd();
        }
        public void GoToFirstPage()
        {
            printStatus("GoToFirstPage()");
            DocViewerWrapper.GoToStart();
        }
        public void GoToPage(object selection)
        {
            printStatus("GoToPageForSelection");
            DocViewerWrapper.GoToSelection(selection);
        }
        new public void GoToPage(int n)
        {
            printStatus("GoToPage(" + n + ")");
            ((ATextControlWrapper)DocViewerWrapper).GoToPage(n);
        }

        public void ScrollUp() { DocViewerWrapper.ScrollUp(1); }
        public void ScrollUp(int n) { DocViewerWrapper.ScrollUp(n); }
        public void ScrollDown() { DocViewerWrapper.ScrollDown(1); }
        public void ScrollDown(int n) { DocViewerWrapper.ScrollDown(n); }
        public bool PageIsVisible(int n) { return DocViewerWrapper.PageIsVisible(n); }

        #endregion

        #region Private Methods

        /// <summary>
        /// Ensure that page number is > 0 and less than the number of pages.
        /// </summary>
        private void ValidatePageNumber(int page)
        {
            if (page < 0 || page >= DocViewerWrapper.PageCount)
                throw new ArgumentException("Page number '" + page + "' is invalid, must be between 0 and " + (DocViewerWrapper.PageCount - 1) + ".");
        }

        private void DoCreateAnnotation(TextRange selection)
        {
            printStatus("Selection for Annotation is '" + SelectionModule.PrintFriendlySelection(selection.Text) + "'.");
            CreateAnnotation();
        }

        private void DoDeleteAnnotation(TextRange selection)
        {
            printStatus("Selection for Deleting Annotation is '" + SelectionModule.PrintFriendlySelection(selection.Text) + "'.");
            DeleteAnnotation();
        }

        #endregion	

        #region Properties

        public ADocumentViewerBaseWrapper DocViewerWrapper
        {
            get
            {
                return TextControlWrapper as ADocumentViewerBaseWrapper;
            }
        }

        public DocumentViewerBase ViewerBase
        {
            get
            {
                return TextControl as DocumentViewerBase;
            }
        }

        #endregion
    }
}

