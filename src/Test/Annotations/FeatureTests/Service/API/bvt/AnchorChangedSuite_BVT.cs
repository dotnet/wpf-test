// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: BVTS testing the AnchorChanged eventing API for AnnotationService.


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
	public class AnchorChangedSuite_BVT : TestSuite
	{
		protected override void RunTestCase()
		{
			Canvas canvas = AnnotationTestHelper.BuildMultiBranchTree();
			ServiceTestContext context = ServiceTestContext.SetupWithServiceAndStore(this, canvas);

			switch (CaseNumber)
			{
				case "anchorchanged_added1": queueTask(new DispatcherOperationCallback(anchorchanged_added1), context); break;
				case "anchorchanged_removed1": queueTask(new DispatcherOperationCallback(anchorchanged_removed1), context); break;
				case "anchorchanged_modified1": queueTask(new DispatcherOperationCallback(anchorchanged_modified1), context); break;
				case "anchorchanged_modified5": queueTask(new DispatcherOperationCallback(anchorchanged_modified5), context); break;
				case "anchorchanged_modified8": queueTask(new DispatcherOperationCallback(anchorchanged_modified5), context); break;

				default:
					failTest("TestSuite '" + this.ToString() + "' does not contain the given test case: '" + CaseNumber + "'.");
					break;
			}
		}

		// ----------------------------------------------------------------------------------
		//                                 TEST METHODS
		// ----------------------------------------------------------------------------------

		/// <summary>
		/// Annotation with 1 resolving anchor.  Add resolving anchor. Verify AttachedAnnotations 
		/// and 1 Loaded event.
		/// </summary>
		public object anchorchanged_added1(object inObj) 
		{ 
			ServiceTestContext context = (ServiceTestContext) inObj;
			Annotation anno = context.MakeAnnotation("btnAddAnnot1");
			context.store.AddAnnotation(anno);

			AnnotationResource resolvingAnchor = context.MakeAnchor("_dockpanel2");
			anno.Anchors.Add(resolvingAnchor);

			assertEquals("Verify number of AttachedAnnotations.", 2, context.service.GetAttachedAnnotations().Count);
			context.eventListener.VerifyEventCounts(2, 0, 0);

			passTest("Simplest AnchorChanged -> Load event.");
			return null;
		}

		/// <summary>
		/// Annotation with 1 resolving anchor.  Remove anchor.  Verify AttachedAnnotations and 
		/// 1 Unloaded event.
		/// </summary>
		public object anchorchanged_removed1(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			Annotation anno = new Annotation(new XmlQualifiedName("SimpleAnnotation", "AnnotationTests"));
			AnnotationResource anchor = context.MakeAnchor("btnAddAnnot1");
			anno.Anchors.Add(anchor);
			context.store.AddAnnotation(anno);

			anno.Anchors.Remove(anchor); // Remove only anchor.

			assertEquals("Verify number of AttachedAnnotations.", 0, context.service.GetAttachedAnnotations().Count);
			context.eventListener.VerifyEventCounts(1, 1, 0);

			passTest("Simplest AnchorChanged -> Unload event.");
			return null;
		}

		/// <summary>
		/// AnchorModified: Annotation with 1 fully resolved anchor.  Modify anchor so it still 
		/// fully resolves.  1 event.
		/// </summary>
		/// <param name="inObj"></param>
		/// <returns></returns>
		public object anchorchanged_modified1(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			Annotation anno = new Annotation(new XmlQualifiedName("SimpleAnnotation", "AnnotationTests"));
			AnnotationResource anchor = context.MakeAnchor("btnAddAnnot1");
			anno.Anchors.Add(anchor);
			context.store.AddAnnotation(anno);

			anchor.Name = "I've_been_modified";

			context.eventListener.VerifyEventCounts(1, 0, 1);
			passTest("AnchorModified event confirmed.");
			return null;
		}

		/// <summary>
		/// Loaded: Annotation with 1 unresolved anchor.  Modify so it partially resolves. 1 event.
		/// </summary>
		public object anchorchanged_modified5(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			Annotation anno = new Annotation(new XmlQualifiedName("SimpleAnnotation", "AnnotationTests"));
			AnnotationResource unresolvedAnchor = context.MakeAnchor("mainCanvas");
			anno.Anchors.Add(unresolvedAnchor);
			context.store.AddAnnotation(anno);

			// Create locators that would cause an anchor to be partially resolved.
			TextBox node = (TextBox) LogicalTreeHelper.FindLogicalNode(context.enablementNode, "textbox1");			
			IList<ContentLocatorBase> partiallyResolvedLocators = context.service.LocatorManager.GenerateLocators(node);
			((DockPanel)LogicalTreeHelper.GetParent(node)).Children.Remove(node);

			AnnotationTestHelper.ReplaceAllLocators(unresolvedAnchor, partiallyResolvedLocators);

			context.eventListener.VerifyEventCounts(0, 0, 0);
			passTest("Unresolved -> partially resolved Loaded event confirmed.");
			return null;
		}

		/// <summary>
		/// Unloaded: Annotation with 1 fully resolved anchor.  Modify so it is unresolved. 1 event.
		/// </summary>
		public object anchorchanged_modified8(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;			
		
			// Create resolved annotation.
			Annotation anno = new Annotation(new XmlQualifiedName("SimpleAnnotation", "AnnotationTests"));
			AnnotationResource anchor = context.MakeAnchor("mainDockPanel");
			anno.Anchors.Add(anchor);
			context.store.AddAnnotation(anno);

			// Make anchor unresolved.
			IList<ContentLocatorBase> unresolvedLocators = context.GenerateLocators("mainCanvas");
			AnnotationTestHelper.ReplaceAllLocators(anchor, unresolvedLocators);

			context.eventListener.VerifyEventCounts(0, 1, 0);
			passTest("Resolved->Unresolved Unload event confirmed.");
			return null;
		}
	}
}

