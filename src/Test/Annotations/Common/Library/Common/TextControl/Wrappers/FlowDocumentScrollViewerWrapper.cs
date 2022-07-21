// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Wrapper specific to 'FlowDocumentScrollViewer'.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Documents;
using Annotations.Test.Framework;

namespace Avalon.Test.Annotations
{
    public class FlowDocumentScrollViewerWrapper : ATextControlWrapper
	{
		#region Constructors

		public FlowDocumentScrollViewerWrapper() 
            : this(new FlowDocumentScrollViewer())
		{
            // Nothing.
		}

        public FlowDocumentScrollViewerWrapper(FlowDocumentScrollViewer fdsv)
            : base(fdsv)
        {
            SelectionModule = new FlowDocumentScrollViewerSelector(Viewer);
        }

		#endregion

		#region Public Methods

        public override void GoToStart()
        {
            ScrollViewer.ScrollToHome();
            DispatcherHelper.DoEvents();
        }
        public override void GoToEnd()
        {
            ScrollViewer.ScrollToBottom();
            DispatcherHelper.DoEvents();
        }
        public override void BringIntoView(ISelectionData selection)
        {
            throw new NotSupportedException();
        }

        public override void SetZoom(double zoomPercent)
        {
            Viewer.Zoom = zoomPercent;
            DispatcherHelper.DoEvents();
        }

        public override double GetZoom()
        {
            return Viewer.Zoom;
        }  

        public override void ZoomIn()
        {
            SyncCommand(NavigationCommands.IncreaseZoom, Viewer);
        }

        public override void ZoomOut()
        {
            SyncCommand(NavigationCommands.DecreaseZoom, Viewer);
        }

        public override void ScrollUp(int n)
        {
            for (int i = 0; i < n; i++)
            {
                ScrollViewer.LineUp();
                DispatcherHelper.DoEvents();
            }
        }

        public override void ScrollDown(int n)
        {
            for (int i = 0; i < n; i++)
            {
                ScrollViewer.LineDown();
                DispatcherHelper.DoEvents();
            }
        }

        public override Point PointerToScreenCoordinates(int page, int offset, System.Windows.Documents.LogicalDirection direction, ATextControlWrapper.HorizontalJustification horizontalJustification)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// ScrollViewer doesn't have pages so simulate this page just paging down n times.
        /// </summary>
        public override void GoToPage(int page)
        {
            PageDown(page);
        }

        #endregion

        #region Public Properties

        public FlowDocumentScrollViewer Viewer
		{
            get { return (FlowDocumentScrollViewer)Target; }
		}

        /// <summary>
        /// ScollViewer has no pages so always return 0.
        /// </summary>
        public override int FirstVisiblePage
        {
            get 
            {
                return 0;
            }
        }

        /// <summary>
        /// ScollViewer has no pages so always return int.MaxValue
        /// </summary>
        public override int LastVisiblePage
        {
            get
            {
                return int.MaxValue;
            }
        }

        /// <summary>
        /// ScrollViewer has no pages so always return int.MaxValue
        /// </summary>
        public override int PageCount
        {
            get
            {
                return int.MaxValue;
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
                DispatcherHelper.DoEvents();
            }
        }

		#endregion
    }
}
