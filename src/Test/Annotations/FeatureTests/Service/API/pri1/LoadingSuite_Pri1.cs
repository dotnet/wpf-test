// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Pri1s testing the Load and Unload annotations API for AnnotationService.


using Annotations.Test.Framework;				// TestSuite.
using System;
using System.Collections;
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


namespace Avalon.Test.Annotations.Pri1s
{
	public class LoadingSuite_Pri1 : ALoadingSuite
	{
		protected override void RunTestCase()
		{
			Canvas canvas = AnnotationTestHelper.BuildMultiBranchTree();
			ServiceTestContext context = ServiceTestContext.SetupWithServiceAndStore(this, canvas);

			// - DISABLED 10/29/04 - 
			//
			// Set our own SubTreeProcessor for this test because the behavior of LoadAnnotations
			// depends upon it.
			//context.service.RegisterSubTreeProcessor(new TestSubTreeProcessor(context.service.LocatorManager), "test");
			//AnnotationService.SetSubTreeProcessorId(context.enablementNode, "test");
			//
			// - DISABLED - 
			DataIdProcessor.SetFetchAnnotationsAsBatch(context.enablementNode, true);

			switch (CaseNumber)
			{
				case "loading2": queueTask(new DispatcherOperationCallback(loading2), context); break;
				case "loading4": queueTask(new DispatcherOperationCallback(loading4), context); break;
				case "loading5": queueTask(new DispatcherOperationCallback(loading5), context); break;
				case "loading6": queueTask(new DispatcherOperationCallback(loading6), context); break;
				case "loading7": queueTask(new DispatcherOperationCallback(loading7), context); break;
				case "loading8": queueTask(new DispatcherOperationCallback(loading8), context); break;
				case "loading9": queueTask(new DispatcherOperationCallback(loading9), context); break;
				case "loading10": queueTask(new DispatcherOperationCallback(loading10), context); break;
				case "loading11": queueTask(new DispatcherOperationCallback(loading11), context); break;
				case "loading12": queueTask(new DispatcherOperationCallback(loading12), context); break;

				default:
					failTest("TestSuite '" + this.ToString() + "' does not contain the given test case: '" + CaseNumber + "'.");
					break;
			}
		}

		/// <summary>
		/// Call UnloadAnnotations on a subtree.  Verify that only annotations above sub-tree 
		/// are still attached.
		/// </summary>
		public object loading2(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			Hashtable annotationMap = AnnotateMultiBranchTree(context.enablementNode);
			assertEquals("Initial number of attached annotations.", 7, context.service.GetAttachedAnnotations().Count);

			DependencyObject subtreeRoot = LogicalTreeHelper.FindLogicalNode(context.enablementNode, "_dockpanel1");
			context.service.UnloadAnnotations(subtreeRoot);
			assertEquals("Number of attached annotations after Unload.", 4, context.service.GetAttachedAnnotations().Count);
			VerifyAnnotationsAreNotAttached(context.service.GetAttachedAnnotations(), (Annotation []) annotationMap["_dockpanel1"]);

			passTest("Expected annotations unloaded.");
			return null;
		}

		/// <summary>
		/// Unload root.  Load subtree 1.  Verify AttachedAnnotations.
		/// </summary>
		public object loading4(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			Hashtable annotationMap = AnnotateMultiBranchTree(context.enablementNode);
			assertEquals("Initial number of attached annotations.", 7, context.service.GetAttachedAnnotations().Count);

			context.service.UnloadAnnotations(context.enablementNode);
			assertEquals("Number of attached annotations after Unload root.", 0, context.service.GetAttachedAnnotations().Count);

			DependencyObject subtreeRoot = LogicalTreeHelper.FindLogicalNode(context.enablementNode, "_dockpanel1");
			context.service.LoadAnnotations(subtreeRoot);
			assertEquals("Number of attached annotations after Load subtree.", 3, context.service.GetAttachedAnnotations().Count);
			VerifyAttachedAnnotations(context.service.GetAttachedAnnotations(), (Annotation[]) annotationMap["_dockpanel1"]);

			passTest("Expected attached annotations after Unload/Load.");
			return null;
		}

		/// <summary>
		/// Unload root.  Load subtree 2, Unload subtree 2.  Verify that all expected annotations 
		/// are attached.
		/// </summary>
		public object loading5(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			Hashtable annotationMap = AnnotateMultiBranchTree(context.enablementNode);			
			assertEquals("Initial number of attached annotations.", 7, context.service.GetAttachedAnnotations().Count);

			context.service.UnloadAnnotations(context.enablementNode);
			assertEquals("Number of attached annotations after Unload root.", 0, context.service.GetAttachedAnnotations().Count);

			DependencyObject subtreeRoot = LogicalTreeHelper.FindLogicalNode(context.enablementNode, "_dockpanel2");
			context.service.LoadAnnotations(subtreeRoot);
			assertEquals("Number of attached annotations after Load subtree.", 3, context.service.GetAttachedAnnotations().Count);

			context.service.UnloadAnnotations(subtreeRoot);
			assertEquals("Number of attached annotations after Unload subtree.", 0, context.service.GetAttachedAnnotations().Count);

			passTest("Expected number of attached annotations after unload/Load/unload.");
			return null;
		}

		/// <summary>
		/// Call Load root twice.  Verify no exception and AttachedAnnotations.  Unload a 
		/// subtree.  Verify AttachedAnnotations.
		/// </summary>
		public object loading6(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			Hashtable annotationMap = AnnotateMultiBranchTree(context.enablementNode);			

			context.service.LoadAnnotations(context.enablementNode);
			context.service.LoadAnnotations(context.enablementNode);
			assertEquals("Initial number of attached annotations.", 7, context.service.GetAttachedAnnotations().Count);

			DependencyObject subtreeRoot = LogicalTreeHelper.FindLogicalNode(context.enablementNode, "_dockpanel2");
			context.service.UnloadAnnotations(subtreeRoot);
			assertEquals("Number of attached annotations after Unload root.", 4, context.service.GetAttachedAnnotations().Count);
			VerifyAnnotationsAreNotAttached(context.service.GetAttachedAnnotations(), (Annotation[])annotationMap["_dockpanel2"]);

			passTest("Expected attached annotations after multiple unloads.");
			return null;
		}

		/// <summary>
		/// Call Unload on empty node.  Call Unload twice on root.  Verify no exception and no 
		/// AttachedAnnotations. Call Load subtree.  Verify AttachedAnnotations.
		/// </summary>
		public object loading7(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			Hashtable annotationMap = AnnotateMultiBranchTree(context.enablementNode);

			// Make one node have no annotations.
			Annotation toDelete = ((Annotation[])annotationMap["btnAddAnnot1"])[0];
			assert("Verify annotation exists.", toDelete != null);
			context.store.DeleteAnnotation(toDelete.Id);

			assertEquals("Initial number of attached annotations.", 6, context.service.GetAttachedAnnotations().Count);

			// Unload on empty node. Verify AttachedAnnotations don't change.
			DependencyObject button1 = LogicalTreeHelper.FindLogicalNode(context.enablementNode, "btnAddAnnot1");
			context.service.UnloadAnnotations(button1);
			assertEquals("Number of attached annotations after unload empty.", 6, context.service.GetAttachedAnnotations().Count);

			// Unload twice on root.
			context.service.UnloadAnnotations(context.enablementNode);
			context.service.UnloadAnnotations(context.enablementNode);
			assertEquals("Number of attached annotations after Unload root twice.", 0, context.service.GetAttachedAnnotations().Count);

			// Load Subtree.
			DependencyObject subtreeRoot = LogicalTreeHelper.FindLogicalNode(context.enablementNode, "_dockpanel1");
			context.service.LoadAnnotations(subtreeRoot);
			assertEquals("Number of attached annotations after Load subtree.", 2, context.service.GetAttachedAnnotations().Count);

			passTest("Expected attached annotations after multiple unloads and unload on empty.");
			return null;
		}

		/// <summary>
		/// Unload leaf. Verify AttachedAnnotations.  Load from root, verify original AttachedAnnotations.
		/// </summary>
		public object loading8(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			Hashtable annotationMap = AnnotateMultiBranchTree(context.enablementNode);

			// Unload on leaf. Verify AttachedAnnotations don't change.
			DependencyObject leaf = LogicalTreeHelper.FindLogicalNode(context.enablementNode, "dp1Canvas");
			context.service.UnloadAnnotations(leaf);
			assertEquals("Number of annotations after unload leaf.", 6, context.service.GetAttachedAnnotations().Count);
			VerifyAnnotationsAreNotAttached(context.service.GetAttachedAnnotations(), (Annotation[])annotationMap["dp1Canvas"]);

			// Load from root.
			context.service.LoadAnnotations(context.enablementNode);
			assertEquals("Number of attached annotations after Load root.", 7, context.service.GetAttachedAnnotations().Count);
			VerifyAttachedAnnotations(context.service.GetAttachedAnnotations(), (Annotation[])annotationMap["mainDockPanel"]);

			passTest("Expected attached annotations after unload leaf, reload from root.");
			return null;
		}

		/// <summary>
		/// Unload at root.  Add annotations.  Load.  Verify that new annotations exist.
		/// </summary>
		public object loading9(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			Hashtable annotationMap = AnnotateMultiBranchTree(context.enablementNode);

			// Unload at root.
			context.service.UnloadAnnotations(context.enablementNode);
			assertEquals("Number of attached annotations after Unload root.", 0, context.service.GetAttachedAnnotations().Count);

			// Add annotation while "Unloaded".
			Annotation newAnno = context.MakeAnnotation("_dockpanel1");
			context.store.AddAnnotation(newAnno);

			// Reload and verify "new" annotation is attached.
			context.service.LoadAnnotations(context.enablementNode);
			assertEquals("Final number of attached annotations.", 8, context.service.GetAttachedAnnotations().Count);
			VerifyAnnotationIsAttached(context.service.GetAttachedAnnotations(), newAnno);

			passTest("Add while 'unloaded' verified.");
			return null;
		}

		/// <summary>
		/// Unload, Delete Annotations, then Load. Original annotations minus deleted.
		/// </summary>
		public object loading10(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			Hashtable annotationMap = AnnotateMultiBranchTree(context.enablementNode);

			// Unload at root.
			context.service.UnloadAnnotations(context.enablementNode);
			assertEquals("Number of attached annotations after Unload root.", 0, context.service.GetAttachedAnnotations().Count);

			// Delete annotations while "Unloaded".
			Annotation[] toDelete = (Annotation[])annotationMap["_dockpanel1"];
			for (int i = 0; i < toDelete.Length; i++)
				assert("Verify delete successful: " + i + ".", context.store.DeleteAnnotation(toDelete[i].Id) != null);

			// Reload and verify deleted annotations do not exist.
			context.service.LoadAnnotations(context.enablementNode);
			assertEquals("Final number of attached annotations.", 4, context.service.GetAttachedAnnotations().Count);
			VerifyAnnotationsAreNotAttached(context.service.GetAttachedAnnotations(), toDelete);

			passTest("Delete while 'unloaded' verified.");
			return null;
		}

		/// <summary>
		/// Unload inner node and 2 leaf nodes.  Verify AttachedAnnotations.  Load on root, verify AttachedAnnotations.
		/// </summary>
		public object loading11(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			Hashtable annotationMap = AnnotateMultiBranchTree(context.enablementNode);

			// Unload on inner node.
			DependencyObject innerNode = LogicalTreeHelper.FindLogicalNode(context.enablementNode, "_dockpanel2");
			context.service.UnloadAnnotations(innerNode);

			// Unload on leaf 1.
			DependencyObject leaf1 = LogicalTreeHelper.FindLogicalNode(context.enablementNode, "dp1Canvas");
			context.service.UnloadAnnotations(leaf1);

			// Unload on leaf 2.
			DependencyObject leaf2 = LogicalTreeHelper.FindLogicalNode(context.enablementNode, "btnAddAnnot1");
			context.service.UnloadAnnotations(leaf2);
			
			assertEquals("Number of attached annotations after various Unloads.", 2, context.service.GetAttachedAnnotations().Count);

			// Reload root.
			context.service.LoadAnnotations(context.enablementNode);
			assertEquals("Final number of attached annotations.", 7, context.service.GetAttachedAnnotations().Count);

			passTest("Verified load propagates through entire tree.");
			return null;
		}

		/// <summary>
		/// Unload inner, 1 leaf.  Load leaf.  Unload 2 leaf.  Load inner.  Verify AttachedAnnotations.
		/// </summary>
		public object loading12(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			Hashtable annotationMap = AnnotateMultiBranchTree(context.enablementNode);

			// Unload on inner node.
			DependencyObject innerNode = LogicalTreeHelper.FindLogicalNode(context.enablementNode, "_dockpanel2");
			context.service.UnloadAnnotations(innerNode);

			// Unload/Reload on leaf 1.
			DependencyObject leaf1 = LogicalTreeHelper.FindLogicalNode(context.enablementNode, "dp1Canvas");
			context.service.UnloadAnnotations(leaf1);
			context.service.LoadAnnotations(leaf1);

			// Unload on leaf 2.
			DependencyObject leaf2 = LogicalTreeHelper.FindLogicalNode(context.enablementNode, "btnAddAnnot1");
			context.service.UnloadAnnotations(leaf2);

			// Reload inner node.
			context.service.LoadAnnotations(innerNode);

			assertEquals("Final number of attached annotations.", 6, context.service.GetAttachedAnnotations().Count);
			VerifyAnnotationsAreNotAttached(context.service.GetAttachedAnnotations(), (Annotation[])annotationMap["btnAddAnnot1"]);

			passTest("Verified un-symetric load/unloads.");
			return null;
		}
	}
}

