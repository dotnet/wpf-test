// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Pri1s testing the StoreContentChanged eventing API for AnnotationService.


using Annotations.Test.Framework;				// TestSuite.
using System;
using System.Threading; using System.Windows.Threading;						// DispatcherOperationCallback.
using System.Windows;
using Proxies.System.Windows.Annotations;
using Proxies.MS.Internal.Annotations.Anchoring;
using Proxies.MS.Internal.Annotations.Component;
using System.Windows.Annotations.Storage;
using System.Windows.Controls;
using System.Xml;
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

namespace Avalon.Test.Annotations.Pri1s
{
	public class StoreContentChangedSuite_Pri1 : TestSuite
	{
		protected override void RunTestCase()
		{
			Canvas root = AnnotationTestHelper.BuildMultiBranchTree();
			ServiceTestContext context = ServiceTestContext.SetupWithServiceAndStore(this, root);

			switch (CaseNumber)
			{
				case "storecontentchanged_added2": queueTask(new DispatcherOperationCallback(storecontentchanged_added2), context); break;
				case "storecontentchanged_added3": queueTask(new DispatcherOperationCallback(storecontentchanged_added3), context); break;
				case "storecontentchanged_added4": queueTask(new DispatcherOperationCallback(storecontentchanged_added4), context); break;
				case "storecontentchanged_added5": queueTask(new DispatcherOperationCallback(storecontentchanged_added5), context); break;

				case "storecontentchanged_deleted2": queueTask(new DispatcherOperationCallback(storecontentchanged_deleted2), context); break;
				case "storecontentchanged_deleted3": queueTask(new DispatcherOperationCallback(storecontentchanged_deleted3), context); break;
				case "storecontentchanged_deleted4": queueTask(new DispatcherOperationCallback(storecontentchanged_deleted4), context); break;
				case "storecontentchanged_deleted5": queueTask(new DispatcherOperationCallback(storecontentchanged_deleted5), context); break;

				default:
					failTest("TestSuite '" + this.ToString() + "' does not contain the given test case: '" + CaseNumber + "'.");
					break;
			}
		}

		// ----------------------------------------------------------------------------------
		//                                 TEST METHODS
		// ----------------------------------------------------------------------------------

		/// <summary>
		/// AddAnnotation with 2 resolving anchors.  Verify 2 AttachedAnnotations and 2 Loaded events.
		/// </summary>
		public object storecontentchanged_added2(object inObj) 
		{
			ServiceTestContext context = (ServiceTestContext)inObj;

			Annotation resolvingAnnotation1 = context.MakeAnnotation("textbox1");
			context.store.AddAnnotation(resolvingAnnotation1);

			Annotation resolvingAnnotation2 = context.MakeAnnotation("_dockpanel1");
			context.store.AddAnnotation(resolvingAnnotation2);

			context.eventListener.VerifyEventCounts(2, 0, 0);
			assertEquals("Number of attached annotations.", 2, context.service.GetAttachedAnnotations().Count);
			passTest("Verified multiple Load events.");
			return null;
		}

		/// <summary>
		/// AddAnnotation with 1 partially resolving anchor.  Verify 1 AttachedAnnotation and 1 Loaded event.
		/// </summary>
		public object storecontentchanged_added3(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			Annotation partiallyResolvedAnnotation = context.MakePartiallyResolvedAnnotation("textbox1");
			context.store.AddAnnotation(partiallyResolvedAnnotation);
			context.eventListener.VerifyEventCounts(1, 0, 0);
			assertEquals("Number of attached annotations.", 1, context.service.GetAttachedAnnotations().Count);
			passTest("Verified loading partially resolved annotation.");
			return null;
		}

		/// <summary>
		/// AddAnnotation with no resolving anchors.  No events fired.
		/// </summary>
		public object storecontentchanged_added4(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;

			// Create annotation with unresolving anchor.
			Annotation unresolvingAnnotation = new Annotation(new XmlQualifiedName("simple", "Test"));
			unresolvingAnnotation.Anchors.Add(AnnotationTestHelper.makeAnchor(context.service, new Button()));
			context.store.AddAnnotation(unresolvingAnnotation);

			context.eventListener.VerifyEventCounts(0, 0, 0);
			assertEquals("Number of attached annotations.", 0, context.service.GetAttachedAnnotations().Count);
			passTest("Verified events for unresolved annotation.");
			return null;
		}

		/// <summary>
		/// AddAnnotation with no anchors.  No events fired.
		/// </summary>
		public object storecontentchanged_added5(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			context.store.AddAnnotation(new Annotation(new XmlQualifiedName("simple", "Tests")));

			context.eventListener.VerifyEventCounts(0, 0, 0);
			assertEquals("Number of attached annotations.", 0, context.service.GetAttachedAnnotations().Count);
			passTest("Verified no events for annotation with no anchors.");
			return null;
		}

		/// <summary>
		/// DeleteAnnotation with 2 resolving anchors.  Verify 2 AttachedAnnotations deleted 
		/// and 2 Unloaded events.
		/// </summary>
		public object storecontentchanged_deleted2(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;

			// Annotation with 2 resolving anchors.
			Annotation anno = new Annotation(new XmlQualifiedName("simple", "Test"));			
			anno.Anchors.Add(context.MakeAnchor("mainDockPanel")); // Resolving anchor.
			anno.Anchors.Add(context.MakeAnchor("btnAddAnnot1"));  // Resolving anchor.
			context.store.AddAnnotation(anno);
			assertEquals("Initial number of attached annotations.", 2, context.service.GetAttachedAnnotations().Count);

			// Clear event counts.
			context.eventListener.Reset();

			// Remove annotation with 2 resolving anchors.
			context.store.DeleteAnnotation(anno.Id);
			context.eventListener.VerifyEventCounts(0, 2, 0);
			assertEquals("Number of attached annotations.", 0, context.service.GetAttachedAnnotations().Count);

			passTest("Verified Unload event for each resolving anchor.");
			return null;
		}

		/// <summary>
		/// DeleteAnnotation with 1 resolving and 1 un-resolving anchor.  Verify 2 AttachedAnnotations 
		/// deleted and 2 Unloaded events.
		/// </summary>
		public object storecontentchanged_deleted3(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;

			// Annotation with 1 resolving and 1 un-resolving anchor.
			Annotation anno = new Annotation(new XmlQualifiedName("simple", "Test"));
			anno.Anchors.Add(context.MakeAnchor("mainDockPanel"));							// Resolving anchor.
			anno.Anchors.Add(AnnotationTestHelper.makeAnchor(context.service, new Button())); // Unresolving anchor.
			context.store.AddAnnotation(anno);
			assertEquals("Initial number of attached annotations.", 1, context.service.GetAttachedAnnotations().Count);

			// Clear event counts.
			context.eventListener.Reset();

			// Remove annotation.
			context.store.DeleteAnnotation(anno.Id);
			context.eventListener.VerifyEventCounts(0, 1, 0);
			assertEquals("Number of attached annotations.", 0, context.service.GetAttachedAnnotations().Count);

			passTest("Verified Unload event for each resolving anchor with no event for unresolved anchors.");
			return null;
		}

		/// <summary>
		/// DeleteAnnotation with no anchors.  No events fired.
		/// </summary>
		public object storecontentchanged_deleted4(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;

			// Annotation with no anchors.
			Annotation anno = new Annotation(new XmlQualifiedName("simple", "Test"));
			context.store.AddAnnotation(anno);

			// Remove annotation.
			context.store.DeleteAnnotation(anno.Id);
			context.eventListener.VerifyEventCounts(0, 0, 0);
			assertEquals("Number of attached annotations.", 0, context.service.GetAttachedAnnotations().Count);

			passTest("Verified no events if no anchors.");
			return null;
		}

		/// <summary>
		/// DeleteAnnotation with only unresolved anchors.  No events fired.
		/// </summary>
		public object storecontentchanged_deleted5(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;

			// Annotation with only unresolved anchors.
			Annotation anno = new Annotation(new XmlQualifiedName("simple", "Test"));
			anno.Anchors.Add(AnnotationTestHelper.makeAnchor(context.service, new Button()));
			anno.Anchors.Add(AnnotationTestHelper.makeAnchor(context.service, new TextBox()));
			context.store.AddAnnotation(anno);

			// Remove annotation.
			context.store.DeleteAnnotation(anno.Id);
			context.eventListener.VerifyEventCounts(0, 0, 0);
			assertEquals("Number of attached annotations.", 0, context.service.GetAttachedAnnotations().Count);

			passTest("Verified no events for unresolved anchors.");
			return null;
		}
	}
}

