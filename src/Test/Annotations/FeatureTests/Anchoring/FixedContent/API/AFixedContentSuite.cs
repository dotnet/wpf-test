// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

using System;
using System.Collections;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Reflection;

using Annotations.Test.Framework;

using Annotation = System.Windows.Annotations.Annotation;
using ContentLocatorBase = System.Windows.Annotations.ContentLocatorBase;
using ContentLocatorPart = System.Windows.Annotations.ContentLocatorPart;
using ContentLocator = System.Windows.Annotations.ContentLocator;
using ContentLocatorGroup = System.Windows.Annotations.ContentLocatorGroup;
using AnnotationResource = System.Windows.Annotations.AnnotationResource;
using AnnotationResourceChangedEventArgs = System.Windows.Annotations.AnnotationResourceChangedEventArgs;
using AnnotationAuthorChangedEventArgs = System.Windows.Annotations.AnnotationAuthorChangedEventArgs;
using AnnotationResourceChangedEventHandler = System.Windows.Annotations.AnnotationResourceChangedEventHandler;
using AnnotationAuthorChangedEventHandler = System.Windows.Annotations.AnnotationAuthorChangedEventHandler;
using AnnotationAction = System.Windows.Annotations.AnnotationAction;

using Proxies.System.Windows.Annotations;
using Proxies.MS.Internal.Annotations.Anchoring;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

/// <summary>
/// AFixedContentSuite provides all the general, common functionality needed in the 
/// test suite for annotating fixed paginated content.  
/// Abstract base class for FixedContent test suite (API tests).
/// </summary>
namespace Avalon.Test.Annotations
{
	public abstract class AFixedContentSuite : ADocumentViewerSuite
	{
		#region Protected Methods

        [TestCase_Setup()]
        protected virtual void DoSetup() 
        {
            SetupTestWindow();

            ViewerBase.Document = LoadContent("fixed_simple.xaml");
            DispatcherHelper.DoEvents();
        }

		/// <summary>
		/// Get FixedPage object for the given page number.
		/// </summary>
		/// <returns>FixedPage for given page number or null if none exists.</returns>
		protected FixedPage GetFixedPage(DocumentViewerBase viewer, int pageNum)
		{
			FixedDocument fixedDoc = viewer.Document as FixedDocument;

			if (fixedDoc == null)
				throw new InvalidOperationException("Root element of DocumentViewer is not FixedDocument, therefore a FixedPage cannot be retrieved.");

			DocumentPage fixedDocumentPage = ((IDocumentPaginatorSource)fixedDoc).DocumentPaginator.GetPage(pageNum);

			FixedPage fixedPage = fixedDocumentPage.GetType().GetProperty(
				"FixedPage",
				BindingFlags.NonPublic | BindingFlags.Instance,
				null,
				typeof(FixedPage),
				Type.EmptyTypes,
				null).GetValue(fixedDocumentPage, null) as FixedPage;
			AssertNotNull("Ensure FixedPage.", fixedPage);

			return fixedPage;
		}

		protected FixedPage GetFixedPage(int pageNum)
		{
            return GetFixedPage(ViewerBase, pageNum);
		}

        protected DocumentPageView GetDocumentPageView(int pageNum)
        {
            DocumentPageView page = null;
            foreach (DocumentPageView view in ViewerBase.PageViews)
            {
                if (view.PageNumber == pageNum)
                {
                    page = view;
                    break;
                }
            }

            if (page == null)
                throw new ArgumentException("Cannot locate DocumentPageView for page '" + pageNum + "', make sure page is visible.");
            return page;
        }

		protected UIElement GetDocumentPageHost(int pageNum)
		{
			DocumentPageView pageView = GetDocumentPageView(pageNum);

			// DocumentPageHost is directly parented by the DocumentPageView.
			return VisualTreeHelper.GetChild(pageView, 0) as UIElement;
		}

		protected TextRange MakeTextRange(int pageNumber, int startIdx, int length)
		{
			return DocViewerWrapper.MakeTextRange(pageNumber, startIdx, length);
		}

		protected TextRange SetSelection(int pageNumber, int startIdx, int length)
		{
			return DocViewerWrapper.SetSelection(pageNumber, startIdx, length);
		}

		/// <summary>
		/// Create an annotation on the given page, and TextRange of ViewerBase.
		/// </summary>
		/// <returns>True if annotation created.</returns>
		protected bool MakeAnnotation(int pageNumber, int startIdx, int length)
		{
			SetSelection(pageNumber, startIdx, length);
			return CreateAnnotation();
		}

		#endregion Protected Methods

		#region Protected Fields

		protected LocatorManager LocatorManager
		{
			get { return AnnotationService.GetService(ViewerBase).LocatorManager; }
		}

		#endregion Protected Fields
	}
}


