// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: BVTS testing the AttachedAnnotationChanged eventing API for AnnotationService.


using Annotations.Test.Framework;				// TestSuite.
using System;
using System.Collections.Generic;
using System.Threading; using System.Windows.Threading;						// DispatcherOperationCallback.
using System.Windows;
using Proxies.System.Windows.Annotations;
using Proxies.MS.Internal.Annotations.Anchoring;
using Proxies.MS.Internal.Annotations.Component;
using System.Windows.Annotations.Storage;
using System.Windows.Controls;
using System.Xml;
using Proxies.MS.Internal.Annotations;
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
	public class AttachedAnnotationChangedSuite_BVT : TestSuite
	{
		protected override void RunTestCase()
		{
			Canvas canvas = AnnotationTestHelper.BuildMultiBranchTree();
			ServiceTestContext context = ServiceTestContext.SetupWithServiceAndStore(this, canvas);

			switch (CaseNumber)
			{
				case "aachanged10": queueTask(new DispatcherOperationCallback(aachanged10), context); break;
				case "aachanged11": queueTask(new DispatcherOperationCallback(aachanged11), context); break;

				default:
					failTest("TestSuite '" + this.ToString() + "' does not contain the given test case: '" + CaseNumber + "'.");
					break;
			}
		}

		// ----------------------------------------------------------------------------------
		//                                 TEST METHODS
		// ----------------------------------------------------------------------------------

		/// <summary>
		/// AttachedAnnotation: Unload annotation. Test identity.
		/// </summary>
		public object aachanged10(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			
			// Add single annotation.
			context.store.AddAnnotation(context.MakeAnnotation("textbox1"));
			IAttachedAnnotation expectedAA = AnnotationTestHelper.GetOnlyAttachedAnnotation(context.service);

			context.service.UnloadAnnotations(context.enablementNode);
			context.eventListener.VerifyEventCounts(1, 1, 0);
			context.eventListener.VerifyLastAttachedAnnotationIdentity(expectedAA);

			passTest("Verified AttachedAnnotation arg for 'Unload'.");
			return null;
		}

		/// <summary>
		/// AttachedAnnotation: Modify annotation anchor. Test identity.
		/// </summary>
		public object aachanged11(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;

			Annotation anno = new Annotation(new XmlQualifiedName("foo", "bar"));
			AnnotationResource anchor = context.MakeAnchor("textbox1");
			anno.Anchors.Add(anchor);
			context.store.AddAnnotation(anno);

			anchor.Name = "modified";

			context.eventListener.VerifyEventCounts(1, 0, 1);
			context.eventListener.VerifyLastAttachedAnnotationIdentity(AnnotationTestHelper.GetOnlyAttachedAnnotation(context.service));

			passTest("Verified AttachedAnnotation arg for 'AnchorModified'.");
			return null;
		}
	}
}

