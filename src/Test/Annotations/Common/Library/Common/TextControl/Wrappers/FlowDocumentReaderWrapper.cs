// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Wrapper specific to 'FlowDocumentReader'.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Documents;
using Annotations.Test.Reflection;
using Annotations.Test.Framework;

namespace Avalon.Test.Annotations
{
    public class FlowDocumentReaderWrapper : ATextControlWrapper
	{
		#region Constructors

		public FlowDocumentReaderWrapper() 
            : base(new FlowDocumentReader())
		{
                        
		}

		#endregion

        #region Public Methods

        public override void PageUp(int n)
        {
            DelegateWrapper.PageUp(n);
        }

        public override void PageDown(int n)
        {
            DelegateWrapper.PageDown(n);
        }

        public override void GoToStart()
        {
            DelegateWrapper.GoToStart();
        }

        public override void GoToEnd()
        {
            DelegateWrapper.GoToEnd();
        }

        public override void BringIntoView(ISelectionData selection)
        {
            DelegateWrapper.BringIntoView(selection);
        }

        public override void SetZoom(double zoomPercent)
        {
            DelegateWrapper.SetZoom(zoomPercent);
        }

        public override double GetZoom()
        {
            return DelegateWrapper.GetZoom();
        }  

        public override void ZoomIn()
        {
            Viewer.IncreaseZoom();
            DispatcherHelper.DoEvents();
        }

        public override void ZoomOut()
        {
            Viewer.DecreaseZoom();
            DispatcherHelper.DoEvents();
        }

        public override void ScrollUp(int n)
        {
            DelegateWrapper.ScrollUp(n);
        }

        public override void ScrollDown(int n)
        {
            DelegateWrapper.ScrollDown(n);
        }

        public override Point PointerToScreenCoordinates(int page, int offset, System.Windows.Documents.LogicalDirection direction, ATextControlWrapper.HorizontalJustification horizontalJustification)
        {
            throw new Exception("The method or operation is not implemented.");
        }  

		#endregion

		#region Public Properties

        public FlowDocumentReaderViewingMode ViewingMode
        {
            get
            {
                return Viewer.ViewingMode;
            }
            set
            {
                TestSuite.Current.printStatus("Changing Reader View... " + value.ToString());
                Viewer.ViewingMode = value;
                WaitForPaginationToComplete();
            }
        }

        public FlowDocumentReader Viewer
		{
            get { return (FlowDocumentReader)Target; }
		}

        public ATextControlWrapper DelegateWrapper
        {
            get
            {
                ATextControlWrapper delegateWrapper = null;

                switch (Viewer.ViewingMode)
                {
                    case FlowDocumentReaderViewingMode.Page:
                        delegateWrapper = PageWrapper;
                        break;
                    case FlowDocumentReaderViewingMode.TwoPage:
                        delegateWrapper = TwoPageWrapper;
                        break;
                    case FlowDocumentReaderViewingMode.Scroll:
                        delegateWrapper = ScrollWrapper;
                        break;
                    default:
                        throw new NotSupportedException();
                }

                return delegateWrapper;
            }
        }

        public override IDocumentPaginatorSource Document
        {
            get
            {
                return Viewer.Document;
            }
            set
            {
                Viewer.Document = (FlowDocument)value;
                WaitForPaginationToComplete();
            }
        }      

        public override SelectionModule SelectionModule
        {
            get
            {
                return DelegateWrapper.SelectionModule;
            }
        }

        public override int PageCount
        {
            get 
            {
                return DelegateWrapper.PageCount;
            }
        }

        public override int FirstVisiblePage
        {
            get
            {
                return Viewer.PageNumber - 1;
            }
        }

        public override int LastVisiblePage
        {
            get
            {
                if (ViewingMode == FlowDocumentReaderViewingMode.TwoPage)
                    return FirstVisiblePage + 1;
                return FirstVisiblePage;
            }
        }

		#endregion    

        #region Protected Methods

        /// <summary>
        /// Synchronously yield to the dispatcher until document is done paginating.
        /// </summary>
        protected void WaitForPaginationToComplete()
        {
            DispatcherHelper.DoEvents();
            // If Paginated content, wait for pagination to complete.
            if (Viewer.Document != null && Viewer.ViewingMode != FlowDocumentReaderViewingMode.Scroll)
            {
                while (!((IDocumentPaginatorSource)Viewer.Document).DocumentPaginator.IsPageCountValid)
                    DispatcherHelper.DoEvents();
            }
        }

        #endregion

        #region Protected Properties

        protected FlowDocumentScrollViewerWrapper ScrollWrapper
        {
            get
            {
                if (_scrollWrapper == null)
                {
                    FlowDocumentScrollViewer fdsv = (FlowDocumentScrollViewer)ReflectionHelper.GetField(Viewer, "_scrollViewer");
                    if (fdsv == null)
                        throw new Exception("Couldn't locate FlowDocumentReader's internal FlowDocumentScrollViewer, did variable name change?");

                    _scrollWrapper = new FlowDocumentScrollViewerWrapper(fdsv);
                }
                return _scrollWrapper;
            }
        }

        protected FlowDocumentPageViewerWrapper PageWrapper
        {
            get
            {
                if (_pageWrapper == null)
                {
                    FlowDocumentPageViewer fdpv = (FlowDocumentPageViewer)ReflectionHelper.GetField(Viewer, "_pageViewer");
                    if (fdpv == null)
                        throw new Exception("Couldn't locate FlowDocumentReader's internal single Page FlowDocumentPageViewer, did variable name change?");

                    _pageWrapper = new FlowDocumentPageViewerWrapper(fdpv);
                }
                return _pageWrapper;
            }
        }

        protected FlowDocumentPageViewerWrapper TwoPageWrapper
        {
            get
            {
                if (_twoPageWrapper == null)
                {
                    FlowDocumentPageViewer fdpv = (FlowDocumentPageViewer)ReflectionHelper.GetField(Viewer, "_twoPageViewer");
                    if (fdpv == null)
                        throw new Exception("Couldn't locate FlowDocumentReader's internal TwoPage FlowDocumentPageViewer, did variable name change?");

                    _twoPageWrapper = new FlowDocumentPageViewerWrapper(fdpv);
                }
                return _twoPageWrapper;
            }
        }

        #endregion

        #region Fields

        private FlowDocumentScrollViewerWrapper _scrollWrapper;
        private FlowDocumentPageViewerWrapper _pageWrapper;
        private FlowDocumentPageViewerWrapper _twoPageWrapper;

        #endregion
    }
}
