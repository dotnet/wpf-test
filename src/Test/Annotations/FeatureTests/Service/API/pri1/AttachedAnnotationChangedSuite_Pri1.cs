// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Pri1s testing the AttachedAnnotationChanged eventing API for AnnotationService.


using Annotations.Test.Framework;				// TestSuite.
using System;
using System.Collections.Generic;
using System.Threading; 
using System.Windows.Threading;						
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

namespace Avalon.Test.Annotations.Pri1s
{
	public class AttachedAnnotationChangedSuite_Pri1 : TestSuite
	{
		protected override void RunTestCase()
		{
			Canvas canvas = AnnotationTestHelper.BuildMultiBranchTree();
			ServiceTestContext context = ServiceTestContext.SetupWithServiceAndStore(this, canvas);

			switch (CaseNumber)
			{
				case "aachanged1": queueTask(new DispatcherOperationCallback(aachanged1), context); break;
				case "aachanged2": queueTask(new DispatcherOperationCallback(aachanged2), context); break;
				case "aachanged3": queueTask(new DispatcherOperationCallback(aachanged3), context); break;
				case "aachanged4": queueTask(new DispatcherOperationCallback(aachanged4), context); break;
				case "aachanged5": queueTask(new DispatcherOperationCallback(aachanged5), context); break;
				case "aachanged6": queueTask(new DispatcherOperationCallback(aachanged6), context); break;
				case "aachanged7": queueTask(new DispatcherOperationCallback(aachanged7), context); break;
				case "aachanged8": queueTask(new DispatcherOperationCallback(aachanged8), context); break;
				case "aachanged9": queueTask(new DispatcherOperationCallback(aachanged9), context); break;

				default:
					failTest("TestSuite '" + this.ToString() + "' does not contain the given test case: '" + CaseNumber + "'.");
					break;
			}
		}

		// ----------------------------------------------------------------------------------
		//                                 TEST METHODS
		// ----------------------------------------------------------------------------------

		/// <summary>
		/// PreviousAttachedAnchor: Load annotation, should be null.
		/// </summary>
		public object aachanged1(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			context.store.AddAnnotation(context.MakeAnnotation("_dockpanel1"));
			context.eventListener.VerifyLastPreviousAttachedAnchorIdentity(null);
			passTest("PreviousAttachmentLevel verified.");
			return null;
		}

		/// <summary>
		/// PreviousAttachedAnchor: Unload annotation, should be null.
		/// </summary>
		public object aachanged2(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			Annotation anno = context.MakeAnnotation("_dockpanel1");
			context.store.AddAnnotation(anno);
			context.store.DeleteAnnotation(anno.Id);
			context.eventListener.VerifyLastPreviousAttachedAnchorIdentity(null);
			passTest("PreviousAttachmentLevel verified.");
			return null;
		}

		/// <summary>
		/// PreviousAttachedAnchor: Do 3 modifications to resolving anchor and 
		/// test that PreviousAttachedAnchor is set correctly.
		/// </summary>
		public object aachanged3(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			Annotation anno1 = new Annotation(new XmlQualifiedName("TestCase84", "EventingTests"));
			DependencyObject node0 = LogicalTreeHelper.FindLogicalNode(context.enablementNode, "textbox1");
			AnnotationResource anchor = AnnotationTestHelper.makeAnchor(context.service, node0);

			anno1.Anchors.Add(anchor);
			context.store.AddAnnotation(anno1);

			DependencyObject node1 = LogicalTreeHelper.FindLogicalNode(context.enablementNode, "_dockpanel2");
			anchor.ContentLocators[0] = context.service.LocatorManager.GenerateLocators(node1)[0];
			context.eventListener.VerifyLastPreviousAttachedAnchorIdentity(node0);

			DependencyObject node2 = LogicalTreeHelper.FindLogicalNode(context.enablementNode, "dp1Canvas");
			anchor.ContentLocators[0] = context.service.LocatorManager.GenerateLocators(node2)[0];
			context.eventListener.VerifyLastPreviousAttachedAnchorIdentity(node1);

			anchor.Name = "modified";
			context.eventListener.VerifyLastPreviousAttachedAnchorIdentity(node2);

			passTest("PreviousAttachmentAnchor verified.");
			return null;
		}

		/// <summary>
		/// PreviousAttachmentLevel: Load annotation, should be Unresolved.
		/// </summary>
		public object aachanged4(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			context.store.AddAnnotation(context.MakeAnnotation("_dockpanel1"));
			context.eventListener.VerifyLastPreviousAttachmentLevel(AttachmentLevel.Unresolved);
			passTest("PreviousAttachmentLevel verified.");
			return null;
		}

		/// <summary>
		/// PreviousAttachmentLevel: Unload annotation, should be Unresolved.
		/// </summary>
		public object aachanged5(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			Annotation anno1 = context.MakeAnnotation("dp1Canvas");
			context.store.AddAnnotation(anno1);
			context.store.DeleteAnnotation(anno1.Id);

			context.eventListener.VerifyLastPreviousAttachmentLevel(AttachmentLevel.Unresolved);
			passTest("PreviousAttachmentLevel verified.");
			return null;
		}

		/// <summary>
		/// PreviousAttachmentLevel: Annotation fully attached. Modify anchor 
		/// so still fully attached.  Should be FULL.
		/// </summary>
		public object aachanged6(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			Annotation anno1 = new Annotation(new XmlQualifiedName("TestCase87", "EventingTests"));
			AnnotationResource anchor = context.MakeAnchor("textbox1");

			anno1.Anchors.Add(anchor);
			context.store.AddAnnotation(anno1);
			anchor.Name = "modified";

			context.eventListener.VerifyLastPreviousAttachmentLevel(AttachmentLevel.Full);
			passTest("PreviousAttachmentLevel verified.");
			return null;
		}

		/// <summary>
		/// PreviousAttachmentLevel: Annotation partially attached. 
		/// Modify anchor so still partially attached. Should be PARTIAL.
		/// </summary>
		public object aachanged7(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			Annotation anno1 = new Annotation(new XmlQualifiedName("TestCase88", "EventingTests"));

			DependencyObject button = LogicalTreeHelper.FindLogicalNode(context.enablementNode, "btnAddAnnot1");
			AnnotationResource anchor = AnnotationTestHelper.makeAnchor(context.service, button);
			anno1.Anchors.Add(anchor);
			DockPanel parent = (DockPanel)LogicalTreeHelper.FindLogicalNode(context.enablementNode, "_dockpanel1");
			parent.Children.Remove((UIElement)button); // Make annotation partially resolve.
			context.store.AddAnnotation(anno1);

			assertEquals("Check that there is 1 attached annotation.", 1, context.service.GetAttachedAnnotations().Count);
			assertEquals("Check AttachmentLevel.", AttachmentLevel.Incomplete, AnnotationTestHelper.GetOnlyAttachedAnnotation(context.service).AttachmentLevel);

			anchor.Name = "Hello World";

			context.eventListener.VerifyLastPreviousAttachmentLevel(AttachmentLevel.Incomplete);
			passTest("PreviousAttachmentLevel verified.");
			return null;
		}

		/// <summary>
		/// PreviousAttachmentLevel: Annotation fully attached. Modify 
		/// anchor so it is partially attached.  Should be FULL.
		/// </summary>
		public object aachanged8(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			Annotation anno1 = new Annotation(new XmlQualifiedName("TestCase89", "EventingTests"));

			DockPanel parent = (DockPanel)LogicalTreeHelper.FindLogicalNode(context.enablementNode, "_dockpanel1");
			DependencyObject node = LogicalTreeHelper.FindLogicalNode(context.enablementNode, "btnAddAnnot1");
			AnnotationResource anchor = AnnotationTestHelper.makeAnchor(context.service, node);
			ContentLocator locator = ((ContentLocator)anchor.ContentLocators[0]);
			ContentLocatorPart lastPart = locator.Parts[locator.Parts.Count - 1];
			locator.Parts.Remove(lastPart);			
			parent.Children.Remove((UIElement)node); // Remove node from tree.

			anno1.Anchors.Add(anchor);	// Should fully resolve.
			context.store.AddAnnotation(anno1);

			locator.Parts.Add(lastPart); // Partially resolves.

			assertEquals("Check that there is 1 attached annotation.", 1, context.service.GetAttachedAnnotations().Count);
			assertEquals("Check AttachmentLevel.", AttachmentLevel.Incomplete, AnnotationTestHelper.GetOnlyAttachedAnnotation(context.service).AttachmentLevel);
			context.eventListener.VerifyLastPreviousAttachmentLevel(AttachmentLevel.Full);

			passTest("PreviousAttachmentLevel verified.");
			return null;
		}

		/// <summary>
		/// PreviousAttachmentLevel: Annotation partially attached.  
		/// Modify anchor so it is fully attached. Should be PARTIAL.
		/// </summary>
		public object aachanged9(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			Annotation anno1 = new Annotation(new XmlQualifiedName("TestCase88", "EventingTests"));

			AnnotationResource anchor = context.MakePartiallyResolvedAnchor("dp1Canvas");
			anno1.Anchors.Add(anchor);
			context.store.AddAnnotation(anno1);

			AnnotationTestHelper.ReplaceAllLocators(anchor, context.GenerateLocators("_dockpanel1")); // Make fully resolved.

			assertEquals("Check that there is 1 attached annotation.", 1, context.service.GetAttachedAnnotations().Count);
			assert("Check that attached annotation is only partially attached.", AnnotationTestHelper.GetOnlyAttachedAnnotation(context.service).AttachmentLevel == AttachmentLevel.Full);
			context.eventListener.VerifyLastPreviousAttachmentLevel(AttachmentLevel.Incomplete);

			passTest("PreviousAttachmentLevel verified.");
			return null;
		}
	}
}

