// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Pri1s testing the AnchorChanged eventing API for AnnotationService.


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
using System.Windows.Annotations;							// XmlNode.

namespace Avalon.Test.Annotations.Pri1s
{
	public class AnchorChangedSuite_Pri1 : TestSuite
	{		
		protected override void RunTestCase()
		{
			Canvas canvas = AnnotationTestHelper.BuildMultiBranchTree();
			ServiceTestContext context = ServiceTestContext.SetupWithServiceAndStore(this, canvas);

			switch (CaseNumber)
			{
				case "anchorchanged_added2": queueTask(new DispatcherOperationCallback(anchorchanged_added2), context); break;
				case "anchorchanged_added3": queueTask(new DispatcherOperationCallback(anchorchanged_added3), context); break;
				case "anchorchanged_added4": queueTask(new DispatcherOperationCallback(anchorchanged_added4), context); break;

				case "anchorchanged_removed2": queueTask(new DispatcherOperationCallback(anchorchanged_removed2), context); break;
				case "anchorchanged_removed3": queueTask(new DispatcherOperationCallback(anchorchanged_removed3), context); break;
				case "anchorchanged_removed4": queueTask(new DispatcherOperationCallback(anchorchanged_removed4), context); break;
				case "anchorchanged_removed5": queueTask(new DispatcherOperationCallback(anchorchanged_removed5), context); break;

				case "anchorchanged_modified2": queueTask(new DispatcherOperationCallback(anchorchanged_modified2), context); break;
				case "anchorchanged_modified3": queueTask(new DispatcherOperationCallback(anchorchanged_modified3), context); break;
				case "anchorchanged_modified4": queueTask(new DispatcherOperationCallback(anchorchanged_modified4), context); break;
				case "anchorchanged_modified6": queueTask(new DispatcherOperationCallback(anchorchanged_modified6), context); break;
				case "anchorchanged_modified7": queueTask(new DispatcherOperationCallback(anchorchanged_modified7), context); break;
				case "anchorchanged_modified9": queueTask(new DispatcherOperationCallback(anchorchanged_modified9), context); break;
				case "anchorchanged_modified10a": queueTask(new DispatcherOperationCallback(anchorchanged_modified10a), context); break;
				case "anchorchanged_modified10b": queueTask(new DispatcherOperationCallback(anchorchanged_modified10b), context); break;
				case "anchorchanged_modified10c": queueTask(new DispatcherOperationCallback(anchorchanged_modified10c), context); break;
				case "anchorchanged_modified10d": queueTask(new DispatcherOperationCallback(anchorchanged_modified10d), context); break;
				case "anchorchanged_modified11": queueTask(new DispatcherOperationCallback(anchorchanged_modified11), context); break;
				case "anchorchanged_modified12": queueTask(new DispatcherOperationCallback(anchorchanged_modified12), context); break;
				case "anchorchanged_modified13": queueTask(new DispatcherOperationCallback(anchorchanged_modified13), context); break;
				case "anchorchanged_modified14": queueTask(new DispatcherOperationCallback(anchorchanged_modified14), context); break;
				case "anchorchanged_modified15": queueTask(new DispatcherOperationCallback(anchorchanged_modified15), context); break;
				case "anchorchanged_modified16": queueTask(new DispatcherOperationCallback(anchorchanged_modified16), context); break;
				case "anchorchanged_modified17": queueTask(new DispatcherOperationCallback(anchorchanged_modified17), context); break;

				default:
					failTest("TestSuite '" + this.ToString() + "' does not contain the given test case: '" + CaseNumber + "'.");
					break;
			}
		}

		// ----------------------------------------------------------------------------------
		//                                 TEST METHODS
		// ----------------------------------------------------------------------------------

		/// <summary>
		/// Annotation with no anchors.  Add resolving anchor.  Verify AttachedAnnotations and 1 Loaded event.
		/// </summary>
		public object anchorchanged_added2(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;			

			// Add annotation with no anchors.
			Annotation anno = new Annotation(new XmlQualifiedName("simple", "Test"));
			context.store.AddAnnotation(anno);

			// Add Resolving anchor.
			anno.Anchors.Add(context.MakeAnchor("btnAddAnnot1"));

			assertEquals("Verify number of AttachedAnnotations.", 1, context.service.GetAttachedAnnotations().Count);
			context.eventListener.VerifyEventCounts(1, 0, 0);

			passTest("Verified load event for resolved anchor added to non-attached annotation.");
			return null;
		}

		/// <summary>
		/// Annotation with no anchors.  Add partially resolving anchor.  Verify AttachedAnnotations 
		/// and 1 Loaded event.
		/// </summary>
		public object anchorchanged_added3(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;

			// Add annotation with no anchors.
			Annotation anno = new Annotation(new XmlQualifiedName("simple", "Test"));
			context.store.AddAnnotation(anno);

			// Add partially resolving anchor.
			anno.Anchors.Add(context.MakePartiallyResolvedAnchor("btnAddAnnot1"));

			assertEquals("Verify number of AttachedAnnotations.", 1, context.service.GetAttachedAnnotations().Count);
			context.eventListener.VerifyEventCounts(1, 0, 0);

			passTest("Verified load event for partial anchor added to non-attached annotation.");
			return null;
		}

		/// <summary>
		/// Annotation with 2 resolving anchors.  Add un-resolving anchor.  Verify AttachedAnnotations 
		/// and no events fired.
		/// </summary>
		public object anchorchanged_added4(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;

			// Add annotation with 2 resolving anchors.
			Annotation anno = new Annotation(new XmlQualifiedName("simple", "Test"));
			anno.Anchors.Add(context.MakeAnchor("btnAddAnnot1")); // Resolving anchor.
			anno.Anchors.Add(context.MakeAnchor("dp1Canvas"));	// Resolving anchor.
			context.store.AddAnnotation(anno);

			context.eventListener.Reset(); // Reset event counts.

			// Add partially resolving anchor.
			AnnotationResource unresolvedAnchor = AnnotationTestHelper.makeAnchor(context.service, new Canvas());
			anno.Anchors.Add(unresolvedAnchor);

			context.eventListener.VerifyEventCounts(0, 0, 0);
			passTest("Verified no event for adding unresolved anchor to resolving annotation.");
			return null;
		}

		/// <summary>
		/// Annotation with 2 resolving anchors.  Remove 1 anchor.  Verify AttachedAnnotations 
		/// and 1 Unloaded event.
		/// </summary>
		public object anchorchanged_removed2(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;

			// Setup: annotation with 2 resolving anchors.
			AnnotationResource resolvingAnchor1 = context.MakeAnchor("btnAddAnnot1");
			AnnotationResource resolvingAnchor2 = context.MakeAnchor("_dockpanel2");
			Annotation anno = new Annotation(new XmlQualifiedName("simple", "Test"));
			anno.Anchors.Add(resolvingAnchor1);
			anno.Anchors.Add(resolvingAnchor2);
			context.store.AddAnnotation(anno);

			// Remove 1 anchor.
			anno.Anchors.Remove(resolvingAnchor2);

			assertEquals("Verify number of AttachedAnnotations.", 1, context.service.GetAttachedAnnotations().Count);
			context.eventListener.VerifyEventCounts(2, 1, 0);

			passTest("Verified unload event for removing resolving anchor.");
			return null;
		}

		/// <summary>
		/// Annotation with 1 fully resolved and 1 partially resolved anchor.  Remove both. 
		/// Verify AttachedAnnotations and 2 Unloaded events.
		/// </summary>
		public object anchorchanged_removed3(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;

			// Setup: annotation with 1 resolved and 1 partially resolved anchor.
			Annotation anno = new Annotation(new XmlQualifiedName("simple", "Test"));

			AnnotationResource resolvingAnchor = context.MakeAnchor("btnAddAnnot1");
			AnnotationResource partialResolvingAnchor = context.MakePartiallyResolvedAnchor("textbox1");

			anno.Anchors.Add(resolvingAnchor);
			anno.Anchors.Add(partialResolvingAnchor);
			context.store.AddAnnotation(anno);
		
			// Test: Remove both anchors
			anno.Anchors.Remove(resolvingAnchor);
			anno.Anchors.Remove(partialResolvingAnchor);

			assertEquals("Verify number of AttachedAnnotations.", 0, context.service.GetAttachedAnnotations().Count);
			context.eventListener.VerifyEventCounts(2, 2, 0);

			passTest("Verified unload event for removing partial and fully resolved anchors.");
			return null;
		}

		/// <summary>
		/// Annotation with 1 resolving and 1 un-resolving anchor.  Remove un-resolving anchor.  
		/// Verify AttachedAnnotations didnt change and no events were fired.
		/// </summary>
		public object anchorchanged_removed4(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;

			// Setup: annotation with 1 resolved and 1 unresolved anchor.
			Annotation anno = new Annotation(new XmlQualifiedName("simple", "Test"));
			AnnotationResource resolvingAnchor = context.MakeAnchor("btnAddAnnot1");
			AnnotationResource unresolvedAnchor = AnnotationTestHelper.makeAnchor(context.service, new Button());
			anno.Anchors.Add(resolvingAnchor);
			anno.Anchors.Add(unresolvedAnchor);
			context.store.AddAnnotation(anno);

			// Test: Remove unresolved anchor.
			anno.Anchors.Remove(unresolvedAnchor);

			assertEquals("Verify number of AttachedAnnotations.", 1, context.service.GetAttachedAnnotations().Count);
			context.eventListener.VerifyEventCounts(1, 0, 0);

			passTest("Verified no unload event for removing unresolved anchor.");
			return null;
		}

		/// <summary>
		/// Annotation with no anchors.  Add anchor.  Delete anchor.  Verify AttachedAnnotations 
		/// and 1 Unloaded event.
		/// </summary>
		public object anchorchanged_removed5(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;

			// Setup: annotation with no anchors.
			Annotation anno = new Annotation(new XmlQualifiedName("simple", "Test"));			
			context.store.AddAnnotation(anno);

			// Test: Add anchor, delete anchor.
			AnnotationResource resolvingAnchor = context.MakeAnchor("btnAddAnnot1");
			anno.Anchors.Add(resolvingAnchor);
			anno.Anchors.Remove(resolvingAnchor);

			assertEquals("Verify number of AttachedAnnotations.", 0, context.service.GetAttachedAnnotations().Count);
			context.eventListener.VerifyEventCounts(1, 1, 0);

			passTest("Verified load and unload event for adding and removing single anchor.");
			return null;
		}

		/// <summary>
		/// AnchorModified: Annotation with 1 fully resolved anchor.  Modify anchor so it is partially 
		/// resolved. 1 event.
		/// </summary>
		public object anchorchanged_modified2(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;

			// Setup: Annotation with 1 fully resolved anchor.
			Annotation anno = new Annotation(new XmlQualifiedName("simple", "Test"));		

			// Test: Modify anchor so it is partially resolved.
			DockPanel parent = (DockPanel)LogicalTreeHelper.FindLogicalNode(context.enablementNode, "_dockpanel2");
			DependencyObject node = LogicalTreeHelper.FindLogicalNode(context.enablementNode, "textbox1");			
			AnnotationResource anchor = AnnotationTestHelper.makeAnchor(context.service, node);
			ContentLocator locator = ((ContentLocator)anchor.ContentLocators[0]);
			ContentLocatorPart lastPart = locator.Parts[locator.Parts.Count - 1];
			locator.Parts.Remove(lastPart);
			parent.Children.Remove((UIElement)node); // Remove node from tree.

			anno.Anchors.Add(anchor); // Fully resolved.
			context.store.AddAnnotation(anno);

			locator.Parts.Add(lastPart); // Make partially resolve.

			context.eventListener.VerifyEventCounts(1, 0, 1);
			passTest("Verified modify event for transition from resolved to partially resolved.");
			return null;
		}

		/// <summary>
		/// AnchorModified: Annotation with 1 partially resolved anchor.  Modify so it is still partially 
		/// resolved. 1 event.
		/// </summary>
		public object anchorchanged_modified3(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;

			// Setup: Annotation with 1 partially resolved anchor.
			Annotation anno = new Annotation(new XmlQualifiedName("simple", "Test"));
			AnnotationResource partiallyResolvedAnchor = context.MakePartiallyResolvedAnchor("textbox1");
			anno.Anchors.Add(partiallyResolvedAnchor);
			context.store.AddAnnotation(anno);

			// Test: Modify anchor.
			partiallyResolvedAnchor.Name = "Modified";

			context.eventListener.VerifyEventCounts(1, 0, 1);
			passTest("Verified modify event when partially resolved.");
			return null;
		}

		/// <summary>
		/// AnchorModified: Annotation with 1 partially resolved anchor.  Modify so it is fully resolved. 
		/// 1 event.
		/// </summary>
		public object anchorchanged_modified4(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;

			// Setup: Annotation with 1 partially resolved anchor.
			Annotation anno = new Annotation(new XmlQualifiedName("simple", "Test"));
			AnnotationResource partiallyResolvedAnchor = context.MakePartiallyResolvedAnchor("dp1Canvas");
			anno.Anchors.Add(partiallyResolvedAnchor);
			context.store.AddAnnotation(anno);

			// Test: Modify anchor so it is fully resolved.
			AnnotationTestHelper.ReplaceAllLocators(partiallyResolvedAnchor, context.GenerateLocators("btnAddAnnot1"));

			context.eventListener.VerifyEventCounts(1, 0, 1);
			passTest("Verified modify event when transitioning from partially resolved to resolved.");
			return null;
		}

		/// <summary>
		/// Loaded: Annotation with 2 unresolved anchor.  Modify 1 so it fully resolves.  1 event.
		/// </summary>
		public object anchorchanged_modified6(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;

			// Setup: Annotation with 2 unresolved anchors			
			AnnotationResource unresolvedAnchor1 = AnnotationTestHelper.makeAnchor(context.service, new TextBox());
			AnnotationResource unresolvedAnchor2 = AnnotationTestHelper.makeAnchor(context.service, new DockPanel());
			
			Annotation anno = new Annotation(new XmlQualifiedName("simple", "Test"));
			anno.Anchors.Add(unresolvedAnchor1);
			anno.Anchors.Add(unresolvedAnchor2);
			context.store.AddAnnotation(anno);

			// Test: Modify 1 anchor so it fully resolves.
			AnnotationTestHelper.ReplaceAllLocators(unresolvedAnchor2, context.GenerateLocators("btnAddAnnot1"));

			context.eventListener.VerifyEventCounts(1, 0, 0);
			passTest("Verified load event for anchor transition from unresolved to resolved.");
			return null;
		}

		/// <summary>
		/// Loaded: Annotation with 1 unresolved anchor.  Modify so it is still unresolved.  Modify 
		/// again so it fully resolves.  1 event.
		/// </summary>
		public object anchorchanged_modified7(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;

			// Setup: Annotation with 1 unresolved anchors			
			AnnotationResource unresolvedAnchor1 = AnnotationTestHelper.makeAnchor(context.service, new TextBox());

			Annotation anno = new Annotation(new XmlQualifiedName("simple", "Test"));
			anno.Anchors.Add(unresolvedAnchor1);
			context.store.AddAnnotation(anno);

			// Test1: Modify anchor so it is still unresolved.
			IList<ContentLocatorBase> unresolvedLocators = context.service.LocatorManager.GenerateLocators(new DockPanel());
			AnnotationTestHelper.ReplaceAllLocators(unresolvedAnchor1, unresolvedLocators);
			context.eventListener.VerifyEventCounts(0, 0, 0);

			// Test2: Modify so it fully resolves.
			AnnotationTestHelper.ReplaceAllLocators(unresolvedAnchor1, context.GenerateLocators("btnAddAnnot1"));
			context.eventListener.VerifyEventCounts(1, 0, 0);

			passTest("Verified load event for anchor transition from long time unresolved to resolved.");
			return null;
		}

		/// <summary>
		/// Unloaded: Annotation with 2 partially resolved anchors.  Modify so 1 does not resolve.  1 event.
		/// </summary>
		public object anchorchanged_modified9(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;

			// Setup: Annotation with 2 partially resolved anchors
			AnnotationResource partiallyResolvedAnchor1 = context.MakePartiallyResolvedAnchor("btnAddAnnot1");
			AnnotationResource partiallyResolvedAnchor2 = context.MakePartiallyResolvedAnchor("dp1Canvas");

			Annotation anno = new Annotation(new XmlQualifiedName("simple", "Test"));
			anno.Anchors.Add(partiallyResolvedAnchor1);
			anno.Anchors.Add(partiallyResolvedAnchor2);
			context.store.AddAnnotation(anno);

			// Test: Modify 1 anchor so it doesn't unresolved.
			IList<ContentLocatorBase> unresolvedLocators = context.service.LocatorManager.GenerateLocators(new DockPanel());
			AnnotationTestHelper.ReplaceAllLocators(partiallyResolvedAnchor1, unresolvedLocators);
			context.eventListener.VerifyEventCounts(2, 1, 0);

			passTest("Verified unload event for anchor transition from partially resolved to unresolved.");
			return null;
		}

		/// <summary>
		/// Verify that changing an anchor's Name causes an AnchorModified event.
		/// </summary>
		public object anchorchanged_modified10a(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			Annotation anno1 = new Annotation(new XmlQualifiedName("TestCase73a", "EventingTests"));
			AnnotationResource anchor = context.MakeAnchor("_dockpanel2");
			anno1.Anchors.Add(anchor);
			context.store.AddAnnotation(anno1);

			// - - - - Modify name - - - - //
			anchor.Name = "foo";
			context.eventListener.VerifyEventCounts(1, 0, 1);
			passTest("Verified AnchorModifiedEvent for name change.");
			return null;
		}

		/// <summary>
		/// Verify that changing adding locators to an anchor causes an AnchorModified event.
		/// </summary>
		public object anchorchanged_modified10b(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			Annotation anno1 = new Annotation(new XmlQualifiedName("TestCase73b", "EventingTests"));
			AnnotationResource anchor = context.MakeAnchor("_dockpanel1");
			anno1.Anchors.Add(anchor);
			context.store.AddAnnotation(anno1);

			// - - - - AddLocators - - - - //
			IList<ContentLocatorBase> locs = context.GenerateLocators("btnAddAnnot1");
			AnnotationTestHelper.AddLocators(anchor, locs);
			context.eventListener.VerifyEventCounts(1, 0, 1);
			passTest("Verified AnchorModifiedEvent for 'AddLocators'.");
			return null;
		}

		/// <summary>
		/// Verify that Locators.Add and Locators.Remove api causes an AnchorModified event.
		/// </summary>
		public object anchorchanged_modified10c(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			Annotation anno1 = new Annotation(new XmlQualifiedName("TestCase73c", "EventingTests"));
			AnnotationResource anchor = context.MakeAnchor("mainDockPanel");
			anno1.Anchors.Add(anchor);
			context.store.AddAnnotation(anno1);

			// - - - - Locators.Add - - - - //
			IList<ContentLocatorBase> locs1 = context.GenerateLocators("btnAddAnnot1");
			anchor.ContentLocators.Add(locs1[0]);
			context.eventListener.VerifyEventCounts(1, 0, 1);
			printStatus("Verified AnchorModifiedEvent for 'Locators.Add'.");

			// - - - - Locators.Remove - - - - //
			anchor.ContentLocators.Remove(locs1[0]);
			context.eventListener.VerifyEventCounts(1, 0, 2);
			passTest("Verified AnchorModifiedEvent for 'Locators.Remove'.");
			return null;
		}

		/// <summary>
		/// Verify that modifying an anchor's content will fire a AnchorModified event.
		/// </summary>
		public object anchorchanged_modified10d(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			Annotation anno1 = new Annotation(new XmlQualifiedName("TestCase73d", "EventingTests"));
			AnnotationResource anchor = context.MakeAnchor("textbox1");
			anno1.Anchors.Add(anchor);
			context.store.AddAnnotation(anno1);

			XmlElement cargo1 = MakeContent("Cargo1");
			XmlElement cargo2 = MakeContent("Cargo2");
			AnnotationResource anchor1 = context.MakeAnchor("_dockpanel1");
			anchor1.Contents.Add(cargo2);

			// - - - - Contents.Add - - - - //
			anchor.Contents.Add(cargo1);
			context.eventListener.VerifyEventCounts(1, 0, 1);
			printStatus("Verified AnchorModifiedEvent for 'Contents.Add'.");

			// - - - - Replace Content - - - - //		
			anchor.Contents[0] = cargo2;
			context.eventListener.VerifyEventCounts(1, 0, 2);
			printStatus("Verified AnchorModifiedEvent for 'Contents[0] = X'.");

			// - - - - Contents.Remove - - - - //
			anchor.Contents.Remove(cargo2);
			context.eventListener.VerifyEventCounts(1, 0, 3);
			printStatus("Verified AnchorModifiedEvent for 'Contents.Remove'.");

			passTest("Correct number of AnchorModified events received.");
			return null;
		}

		/// <summary>
		/// N resolved anchors, modify 1, receive 1 event. Verify correct AttachedAnnotation.
		/// </summary>
		public object anchorchanged_modified11(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			Annotation anno1 = new Annotation(new XmlQualifiedName("Testcase75", "EventingTests"));
			AnnotationResource[] resolvingAnchors = CreateAndAddNAnchors(context, anno1, "_dockpanel1", 10);

			context.store.AddAnnotation(anno1); // Load annotation.
			ICollection<IAttachedAnnotation> attachedAnnos = context.service.GetAttachedAnnotations();
			assertEquals("Verify number of attached annotations.", 10, attachedAnnos.Count);

			// Note: this is an order dependent test, so if the order ever changes it will break.
			IAttachedAnnotation aa = null;
			IEnumerator<IAttachedAnnotation> aaEnum = attachedAnnos.GetEnumerator();
			for (int i = 0; i < 2; i++)
				aaEnum.MoveNext();				
			aa = aaEnum.Current;

			resolvingAnchors[1].ContentLocators.Add(context.GenerateLocators("btnAddAnnot1")[0]); // Modify 1 anchor.

			context.eventListener.VerifyEventCounts(10, 0, 1);
			context.eventListener.VerifyLastAttachedAnnotationIdentity(aa);

			passTest("Correct number of AnchorModified events received.");
			return null;
		}

		/// <summary>
		/// N resolved anchors, M unresolved anchors, modify 1 resolving anchor.  
		/// Receive 1 event.
		/// </summary>
		public object anchorchanged_modified12(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			Annotation anno1 = new Annotation(new XmlQualifiedName("TestCase76", "EventingTests"));
			AnnotationResource[] resolvingAnchors = CreateAndAddNAnchors(context, anno1, "_dockpanel1", 5);
			AnnotationResource[] unresolvingAnchors = CreateAndAddNAnchors(context, anno1, "mainCanvas", 7);

			context.store.AddAnnotation(anno1); // Load annotation.
			resolvingAnchors[1].ContentLocators.Add(context.GenerateLocators("btnAddAnnot1")[0]); // Modify 1 resolved anchor.

			context.eventListener.VerifyEventCounts(5, 0, 1);

			passTest("Correct number of AnchorModified events received.");
			return null;
		}
	
		/// <summary>
		/// N resolved anchors, modify M resolved anchors, receive M events.
		/// </summary>
		public object anchorchanged_modified13(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			Annotation anno1 = new Annotation(new XmlQualifiedName("TestCase77", "EventingTests"));
			AnnotationResource[] resolvingAnchors = CreateAndAddNAnchors(context, anno1, "_dockpanel1", 7); // Create N anchors.

			context.store.AddAnnotation(anno1); // Load annotation.

			// - - - - Modify M Anchors - - - - //
			XmlDocument xdoc = new XmlDocument();
			for (int i = 0; i < 5; i++)
				resolvingAnchors[i].Contents.Add((XmlElement)xdoc.CreateNode(XmlNodeType.Element, "content" + i, "EventingTests"));

			context.eventListener.VerifyEventCounts(7, 0, 5); // Expect M modify events.

			passTest("Correct number of AnchorModified events received.");
			return null;
		}
	
		/// <summary>
		///	Unresolved anchor, modify, do not receive event.
		/// </summary>
		public object anchorchanged_modified14(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			Annotation anno1 = new Annotation(new XmlQualifiedName("TestCase78", "EventingTests"));
			AnnotationResource resolvingAnchor = context.MakeAnchor("_dockpanel2");
			AnnotationResource unresolvingAnchor = context.MakeAnchor("mainCanvas");

			anno1.Anchors.Add(unresolvingAnchor); // No event.
			anno1.Anchors.Add(resolvingAnchor);   // Load event.
			context.store.AddAnnotation(anno1);

			context.eventListener.VerifyEventCounts(1, 0, 0);

			unresolvingAnchor.Name = "Modified"; // No event.
			context.eventListener.VerifyEventCounts(1, 0, 0);

			passTest("Correct number of AnchorModified events received.");
			return null;
		}

		/// <summary>
		///	Resolved anchor, modify, event.  Unload annotation, 
		/// modify, no AnchorModified event. Load annotation, modify, event.
		/// </summary>
		public object anchorchanged_modified15(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			Annotation anno1 = new Annotation(new XmlQualifiedName("TestCase79", "EventingTests"));// AnnotationTestHelper.makeSimpleAnnotation(context.service, context.dockP1));
			AnnotationResource anchor = new AnnotationResource();
			IList<ContentLocatorBase> resolvingLocs = context.GenerateLocators("_dockpanel2");
			IList<ContentLocatorBase> unresolvingLocs = context.GenerateLocators("mainCanvas");

			AnnotationTestHelper.AddLocators(anchor, resolvingLocs);
			anno1.Anchors.Add(anchor);
			context.store.AddAnnotation(anno1);						  // Load Event.			

			anchor.Contents.Add(MakeContent("mod1")); // Modify event.
			context.eventListener.VerifyEventCounts(1, 0, 1);

			AnnotationTestHelper.ReplaceAllLocators(anchor, unresolvingLocs);				  // Unload event.
			anchor.Contents.Add(MakeContent("mod2")); // No event.
			context.eventListener.VerifyEventCounts(1, 1, 1);

			AnnotationTestHelper.ReplaceAllLocators(anchor, resolvingLocs);				  // Load event.
			anchor.Contents.Add(MakeContent("mod3")); // Modify event.
			context.eventListener.VerifyEventCounts(2, 1, 2);

			passTest("Correct number of AnchorModified events received.");
			return null;
		}

		/// <summary>
		/// Resolved anchor, remove anchor, modify, no event.
		/// </summary>
		public object anchorchanged_modified16(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			Annotation anno1 = new Annotation(new XmlQualifiedName("TestCase79", "EventingTests"));
			AnnotationResource anchor = context.MakeAnchor("_dockpanel2");	  // Resolves.

			context.store.AddAnnotation(anno1);
			anno1.Anchors.Add(anchor);								  // Load event.

			anchor.Contents.Add(MakeContent("mod1")); // Modify event.
			context.eventListener.VerifyEventCounts(1, 0, 1);

			anno1.Anchors.Remove(anchor);								  // Unload event.
			anchor.Contents.Add(MakeContent("mod2")); // No event.
			context.eventListener.VerifyEventCounts(1, 1, 1);

			anno1.Anchors.Add(anchor);								  // Load event.
			anchor.Contents.Add(MakeContent("mod3")); // Modify event.
			context.eventListener.VerifyEventCounts(2, 1, 2);

			passTest("Correct number of AnchorModified events received.");
			return null;
		}

		/// <summary>
		/// Resolved anchor, remove annotation, modify, no event.
		/// </summary>
		public object anchorchanged_modified17(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			Annotation anno1 = new Annotation(new XmlQualifiedName("TestCase81", "EventingTests"));
			AnnotationResource anchor = context.MakeAnchor("_dockpanel1");	  // Resolves.

			anno1.Anchors.Add(anchor);
			context.store.AddAnnotation(anno1);                       // Load event.
			context.eventListener.VerifyEventCounts(1, 0, 0);

			context.store.DeleteAnnotation(anno1.Id);                 // Unload event.
			anchor.Contents.Add(MakeContent("mod1")); // No event.

			context.eventListener.VerifyEventCounts(1, 1, 0);
			passTest("Correct number of AnchorModified events received.");
			return null;
		}

		// ----------------------------------------------------------------------------------
		//                                 PRIVATE METHODS
		// ----------------------------------------------------------------------------------

		private XmlDocument xdoc = new XmlDocument();

		private XmlElement MakeContent(string content)
		{			
			return xdoc.CreateNode(XmlNodeType.Element, content, "ServiceAPITests") as XmlElement;
		}


		private AnnotationResource[] CreateAndAddNAnchors(ServiceTestContext context, Annotation anno, string nodeId, int nAnchors)
		{
			DependencyObject node = LogicalTreeHelper.FindLogicalNode(context.enablementNode, nodeId);
			assert("Given nodeId must be valid.", node != null);

			AnnotationResource[] anchors = new AnnotationResource[nAnchors];
			for (int i = 0; i < nAnchors; i++)
			{
				anchors[i] = AnnotationTestHelper.makeAnchor(context.service, node);
				anno.Anchors.Add(anchors[i]);
			}
			return anchors;
		}
	}
}

