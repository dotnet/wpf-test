// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: BVTS testing the StoreContentChanged eventing API for AnnotationService.


using Annotations.Test.Framework;				// TestSuite.
using System;
using System.Threading; using System.Windows.Threading;						// DispatcherOperationCallback.
using System.Windows;
using Proxies.System.Windows.Annotations;
using Proxies.MS.Internal.Annotations.Anchoring;
using Proxies.MS.Internal.Annotations.Component;
using System.Windows.Annotations.Storage;
using System.Windows.Controls;
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

namespace Avalon.Test.Annotations.BVTs
{
	public class StoreContentChangedSuite_BVT : TestSuite
	{
		protected override void RunTestCase()
		{
			Canvas root = AnnotationTestHelper.BuildMultiBranchTree();
			ServiceTestContext context = ServiceTestContext.SetupWithServiceAndStore(this, root);

			switch (CaseNumber)
			{
				case "storecontentchanged_added1": queueTask(new DispatcherOperationCallback(storecontentchanged_added1), context); break;
				case "storecontentchanged_deleted1": queueTask(new DispatcherOperationCallback(storecontentchanged_deleted1), context); break;

				default:
					failTest("TestSuite '" + this.ToString() + "' does not contain the given test case: '" + CaseNumber + "'.");
					break;
			}
		}

		// ----------------------------------------------------------------------------------
		//                                 TEST METHODS
		// ----------------------------------------------------------------------------------

		/// <summary>
		/// AddAnnotation with single resolving anchor.  Verify 1 AttachedAnnotation and 1 
		/// Loaded event.
		/// </summary>
		public object storecontentchanged_added1(object inObj) 
		{
			ServiceTestContext context = (ServiceTestContext)inObj;

			Annotation resolvingAnnotation = context.MakeAnnotation("textbox1");
			context.store.AddAnnotation(resolvingAnnotation);

			context.eventListener.VerifyEventCounts(1, 0, 0);
			assertEquals("Number of attached annotations.", 1, context.service.GetAttachedAnnotations().Count);
			passTest("Verified simplest Load event.");
			return null;
		}

		/// <summary>
		/// DeleteAnnotation with single resolving anchor.  Verify AttachedAnnotation deleted 
		/// and 1 Unloaded event.
		/// </summary>
		public object storecontentchanged_deleted1(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;

			Annotation resolvingAnnotation = context.MakeAnnotation("textbox1");
			context.store.AddAnnotation(resolvingAnnotation);
			
			assert("Verify delete returns true.", context.store.DeleteAnnotation(resolvingAnnotation.Id) != null);
			context.eventListener.VerifyEventCounts(1, 1, 0);
			assertEquals("Number of attached annotations.", 0, context.service.GetAttachedAnnotations().Count);

			passTest("Verified simplest Unload event.");
			return null;
		}
	}
}

