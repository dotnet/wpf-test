// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Wrapper specific to 'DocumentViewer'.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Reflection;
using System.Collections;
using System.Windows.Annotations;
using Annotations.Test.Reflection;
using System.Windows.Automation;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using Annotations.Test.Framework;
using Microsoft.Test.Display;

namespace Avalon.Test.Annotations
{
    public class DocumentViewerWrapper : ADocumentViewerBaseWrapper
	{
		#region Constructors

		public DocumentViewerWrapper() 
            : base(new DocumentViewer())
		{
            SelectionModule = new DocumentViewerSelector(Viewer);
		}

		#endregion

		#region Public Methods	

        public override void SetZoom(double zoomPercent)
        {
            SyncCommand(NavigationCommands.Zoom, zoomPercent);
        }

        public override double GetZoom()
        {
            return Viewer.Zoom;
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

		/// <summary>
		/// Synchronously, without adjusting zoom or window size, try and position the scrollviewer so that this range of
		/// pages is visible.  If it is not possible to make all of them visible, just make as many as possible starting 
		/// from the lowest page (e.g. if asked for 1-3 and only 2 pages can be shown, pages 1-2 will be visible).
		/// </summary>
		public override void GoToPageRange(int firstPage, int secondPage)
		{
			ValidatePageNumber(firstPage);
			ValidatePageNumber(secondPage);
			if (secondPage < firstPage)
				throw new ArgumentException("Second page '" + secondPage + "' must be greater than or equal to first page '" + firstPage + "'.");

			GoToPage(firstPage);
			while (FirstVisiblePage <= firstPage && LastVisiblePage < secondPage)
				ScrollDown(1);
			EnsurePageIsVisible(firstPage);
		}

		public override void WholePageLayout()
		{
            SyncCommand(DocumentViewer.FitToHeightCommand);
		}

		public override void PageWidthLayout() 
		{
            SyncCommand(DocumentViewer.FitToWidthCommand);
		}

		public override void PageLayout(int numPages)
		{
            SyncCommand(DocumentViewer.FitToMaxPagesAcrossCommand, numPages);
		}

		public override void ScrollUp(int n)
		{
            SyncCommand(n, ComponentCommands.MoveUp);
		}

		public override void ScrollDown(int n)
		{
            SyncCommand(n, ComponentCommands.MoveDown);
		}

		/// <summary>
		/// Disable all the stuff that might change and break us.
		/// -remove scroll bars.
		/// -hide toolbar.
		/// </summary>
		public void DisableScrollAndToolbars()
		{
			ScrollViewer scrollView = ReflectionHelper.GetField(Viewer, "_scrollViewer") as ScrollViewer;
			scrollView.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
			scrollView.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            //DocumentViewer Breaking Change: ToggleToolbar (to be cut with task 40450)
            //SyncCommand(DocumentViewer.ToggleToolBar);
		}

        /// <summary>
        /// Compute position relative to ScrollViewer control.
        /// </summary>
        public override Point PointerToScreenCoordinates(int page, int offset, LogicalDirection direction, ATextControlWrapper.HorizontalJustification horizontalJustification)
        {
            ValidatePageNumber(page);

            ScrollViewer scrollviewer = ReflectionHelper.GetField(ViewerBase, "_scrollViewer") as ScrollViewer;
            Debug.Assert(scrollviewer != null);
            Rect scrollViewerRect = UIAutomationModule.BoundingRectangle(scrollviewer);

            Debug.Assert(!scrollViewerRect.Equals(Rect.Empty), "Since page was visible we should have found its PageView.");
            Rect charRect = SelectionModule.CharacterRect(page, offset, direction);
            
            // Set justification relative to y axis of character rect.
            Vector justification = new Vector(0, 0);
            if (horizontalJustification == HorizontalJustification.Middle)
                justification = new Vector(0, charRect.Height / 2);
            else if (horizontalJustification == HorizontalJustification.Bottom)
                justification = new Vector(0, charRect.Height);

            return Point.Add(new Point(scrollViewerRect.Left + Monitor.ConvertLogicalToScreen(Dimension.Width, charRect.Left), scrollViewerRect.Top + Monitor.ConvertLogicalToScreen(Dimension.Height, charRect.Top)), new Vector(Monitor.ConvertLogicalToScreen(Dimension.Width, justification.X), Monitor.ConvertLogicalToScreen(Dimension.Height, justification.Y)));
        }

		#endregion

		#region Public Properties

		public DocumentViewer Viewer
		{
			get { return (DocumentViewer)ViewerBase; }
		}

		#endregion              
    }
}
