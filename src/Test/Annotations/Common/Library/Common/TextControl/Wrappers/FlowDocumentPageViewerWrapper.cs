// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Wrapper specific to 'FlowDocumentPageViewer'.

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
using System.IO;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Diagnostics;
using Annotations.Test.Framework;
using Microsoft.Test.Display;

namespace Avalon.Test.Annotations
{
    public class FlowDocumentPageViewerWrapper : ADocumentViewerBaseWrapper
	{
		#region Constructors

        public FlowDocumentPageViewerWrapper(FlowDocumentPageViewer fdpv)
            : base(fdpv)
        {
            SelectionModule = new FlowDocumentPageViewerSelector(Viewer);

            DefaultStyle = Viewer.Style;	// Save default style.
        }

		public FlowDocumentPageViewerWrapper() 
            : this(new FlowDocumentPageViewer())
		{
            // Nothing.
		}



		#endregion

		#region Public Methods

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

        public override double GetZoom()
        {
            return Viewer.Zoom;
        }  

		/// <summary>
		/// In order to make a page break visible, we will need to change the page layout to 2 pages.  Then navigate until
		/// the desired pages are visible.
		/// </summary>
		/// <remarks>
		/// If firstPage != secondPage then this will change the pageLayout.
		/// </remarks>
		public override void GoToPageRange(int firstPage, int secondPage)
		{
			ValidatePageNumber(firstPage);
			ValidatePageNumber(secondPage);
			if (secondPage < firstPage)
				throw new ArgumentException("Second page '" + secondPage + "' must be greater than or equal to first page '" + firstPage + "'.");

			if (firstPage != secondPage)
				PageLayout(2);

			GoToPage(firstPage);
			if (!PageIsVisible(secondPage))
				PageDown();
			EnsurePageIsVisible(firstPage);
			EnsurePageIsVisible(secondPage);
		}

		public override void WholePageLayout()
		{
			PageLayout(1);
		}

		public override void PageWidthLayout()
		{
			PageLayout(1);
		}

		public override void PageLayout(int numPages)
		{
			Style style = null;
			switch (numPages)
			{
				case 1: style = DefaultStyle; break;
				case 2: style = LoadStyle(TWO_PAGE_STYLE); break;
				case 6: style = LoadStyle(SIX_PAGE_STYLE); break;
				default:
					throw new ArgumentException("No style for viewing '" + numPages.ToString() + "' pages.");		
			}

			ViewerBase.Style = style;
			WaitForFinalPageCount();		
		}

		public override void SetZoom(double zoomPercent)
		{
			Viewer.Zoom = zoomPercent;
			WaitForFinalPageCount();
		}

		public override void ScrollUp(int n)
		{
            throw new NotSupportedException("FlowDocumentPageViewer does not support scrolling.");
		}

		public override void ScrollDown(int n)
		{
            throw new NotSupportedException("FlowDocumentPageViewer does not support scrolling.");
		}

        /// <summary>
        /// Compute position relative to corresponding DocumentPageView.
        /// </summary>
        public override Point PointerToScreenCoordinates(int page, int offset, LogicalDirection direction, ATextControlWrapper.HorizontalJustification horizontalJustification)
        {
            ValidatePageNumber(page);

            Rect targetPageRect = Rect.Empty;
            IEnumerator<DocumentPageView> pageEnumerator = ViewerBase.PageViews.GetEnumerator();
            while (pageEnumerator.MoveNext())
            {
                if (pageEnumerator.Current.PageNumber == page)
                {
                    targetPageRect = UIAutomationModule.BoundingRectangle(pageEnumerator.Current);
                    break;
                }
            }

            Debug.Assert(!targetPageRect.Equals(Rect.Empty), "Since page was visible we should have found its PageView.");
            Rect charRect = SelectionModule.CharacterRect(page, offset, direction);

            // Set justification relative to y axis of character rect.
            Vector justification = new Vector(0, 0);
            if (horizontalJustification == HorizontalJustification.Middle)
                justification = new Vector(0, charRect.Height / 2);
            else if (horizontalJustification == HorizontalJustification.Bottom)
                justification = new Vector(0, charRect.Height);

            return Point.Add(new Point(targetPageRect.Left + Monitor.ConvertLogicalToScreen(Dimension.Width, charRect.Left), targetPageRect.Top + Monitor.ConvertLogicalToScreen(Dimension.Height, charRect.Top)), new Vector(Monitor.ConvertLogicalToScreen(Dimension.Width, justification.X), Monitor.ConvertLogicalToScreen(Dimension.Height, justification.Y)));
        }

		#endregion

		#region Public Properties

        public FlowDocumentPageViewer Viewer
		{
            get { return (FlowDocumentPageViewer)ViewerBase; }
		}

		#endregion

		#region Protected Methods

		protected Style LoadStyle(string styleName)
		{
			// Lazily load custom styles the first time they are used.
			if (_customStyles == null)
			{
				_customStyles = AnnotationTestHelper.LoadXaml(STYLE_FILENAME) as ResourceDictionary;
			}

			Style style = _customStyles[styleName] as Style;
			if (style == null)
				throw new ArgumentException("No style named '" + styleName + "'.");
			return style;
		}

		#endregion

		#region Private Variables

		const string STYLE_FILENAME = "CustomFlowStyles.xaml";
		const string ONE_PAGE_STYLE = "onePageStyle";
		const string TWO_PAGE_STYLE = "twoPageStyle";
		const string SIX_PAGE_STYLE = "sixPageStyle";
		ResourceDictionary _customStyles;
		Style DefaultStyle;

		#endregion
	}
}
