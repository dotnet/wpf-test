// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Tests of the Annotations V1 public api.


using Annotations.Test.Framework;				// TestSuite.
using Annotations.Test.Reflection;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Navigation;
using System.Xml;
using System.Threading;
using System.Windows.Threading;

using System.Windows.Annotations;
using System.Windows.Controls.Primitives;
using System.Security.Permissions;
using System.Windows.Annotations.Storage;

namespace Avalon.Test.Annotations.Suites
{
	/// <summary>
	/// Base class for all V1 public api TestSuites.
	/// </summary>
	public abstract class ADocumentViewerSuite : TestSuite
	{
		/// <summary>
		/// Perform a strong check on whether or not an AnnotationService has been correctly
		/// enabled on the given DocumentViewer.
		/// 
		/// Verify using both public and internal APIs.
		/// </summary>
		public void VerifyServiceEnabled(DocumentViewerBase docView)
		{
			AnnotationService service = DocumentViewerAPIHelpers.GetService(docView);
			// Verify service through public api.
			Assert("Verify serivce is enabled.", service.IsEnabled);

			// Verify service through internal api.
			AnnotationService service2 = DocumentViewerAPIHelpers.GetService(docView);
			AssertNotNull("Verify service exists.", service2);
			AssertNotNull("Verify that AnnoationService.LocatorManager is not null.", ReflectionHelper.GetProperty(service, "LocatorManager"));
		}

		/// <summary>
		/// Enable service on a DocumentViewer.
		/// </summary>
		public void EnableService(DocumentViewerBase documentViewer, Stream stream)
		{
			AnnotationService service = DocumentViewerAPIHelpers.GetService(documentViewer);
			if (service == null)
				service = new AnnotationService(documentViewer);
            AnnotationStore store = new XmlStreamStore(stream);
            store.AutoFlush = true;
            service.Enable(store); 
			AssertNotNull("Verify service exists.", service);
			printStatus("Service enabled and exists.");
		}

		/// <summary>
		/// Call AnnotationService.Disable and verify that it worked.
		/// </summary>
		/// <param name="documentViewer"></param>
		public void DisableServiceAndVerify(DocumentViewerBase documentViewer)
		{
			AnnotationService service = DocumentViewerAPIHelpers.GetService(documentViewer);
            service.Disable();
			printStatus("Service Disabled.");

			printStatus("Verifying service disabled using public API.");
			Assert("Verify service is not ready.", !service.IsEnabled);

			printStatus("Verifying service disabled using internal API.");
			service = DocumentViewerAPIHelpers.GetService(documentViewer);
			AssertNull("Verify service does not exists.", service);
		}

		/// <summary>
		/// Call AnnotationHelper.CreateXXXForSelection with the given parameters.
		/// </summary>
		public void CreateAnnotation(DocumentViewerBase documentViewer, AnnotationMode type)
		{
			switch (type) {
				case AnnotationMode.StickyNote:
					AnnotationHelper.CreateTextStickyNoteForSelection(DocumentViewerAPIHelpers.GetService(documentViewer), null);
					break;
				case AnnotationMode.InkStickyNote:
					AnnotationHelper.CreateInkStickyNoteForSelection(DocumentViewerAPIHelpers.GetService(documentViewer), null);
					break;
				case AnnotationMode.Highlight:
					AnnotationHelper.CreateHighlightForSelection(DocumentViewerAPIHelpers.GetService(documentViewer), null, null);
					break;
				default:
					throw new NotSupportedException("unknown annotation type '" + type + "'.");
			}
        }

		/// <summary>
		/// Call AnnotationHelper.CreateXXXForSelection with the given parameters.
		/// </summary>
		public void VerifyCreateAnnotationFails(DocumentViewerBase documentViewer, AnnotationMode type, Type expectedExceptionType)
		{
			bool exceptionOccurred = false;
			try
			{
				CreateAnnotation(documentViewer, type);
			}
			catch (Exception e)
			{
				AssertEquals("Verify exception type.", expectedExceptionType, e.GetType());
				exceptionOccurred = true;
			}
			Assert("Verify that exception occurred.", exceptionOccurred);
		}
	}
}

