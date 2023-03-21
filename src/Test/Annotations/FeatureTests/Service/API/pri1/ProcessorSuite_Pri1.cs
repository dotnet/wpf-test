// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Pri1s testing Selection and Subtree processor API for AnnotationService.


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
	public class ProcessorSuite_Pri1 : AProcessorSuite
	{
		protected override void RunTestCase()
		{
			Canvas root = AnnotationTestHelper.BuildMultiBranchTree();
			ServiceTestContext context = ServiceTestContext.SetupWithServiceAndStore(this, root);

			switch (CaseNumber)
			{
				case "processor2": queueTask(new DispatcherOperationCallback(processor2), context); break;
				case "processor4": queueTask(new DispatcherOperationCallback(processor4), context); break;
				case "processor5": queueTask(new DispatcherOperationCallback(processor5), context); break;
				case "processor6": queueTask(new DispatcherOperationCallback(processor6), context); break;
				case "processor7": queueTask(new DispatcherOperationCallback(processor7), context); break;

				default:
					failTest("TestSuite '" + this.ToString() + "' does not contain the given test case: '" + CaseNumber + "'.");
					break;
			}
		}

		/// <summary>
		/// SetSubTreeProcessor on node A, set a different SubTreeProcessor on node B which is a child of 
		/// node A.  Verify that above B is As processor, and below B is Bs processor
		/// </summary>
		public object processor2(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;

			CountingSubTreeProcessor processor1 = new CountingSubTreeProcessor(context.service.LocatorManager);
			context.service.LocatorManager.RegisterSubTreeProcessor(processor1, "p1");

			CountingSubTreeProcessor processor2 = new CountingSubTreeProcessor(context.service.LocatorManager);
			context.service.LocatorManager.RegisterSubTreeProcessor(processor2, "p2");

			// Set SubTreeProcessor on node A.
			DependencyObject nodeA = context.enablementNode;
			AnnotationService.SetSubTreeProcessorId(nodeA, "p1");

			// Set SubTreeProcessor on node B.
			DependencyObject nodeB = LogicalTreeHelper.FindLogicalNode(nodeA, "_dockpanel1");
			AnnotationService.SetSubTreeProcessorId(nodeB, "p2");			

			// Verify that SubTreeProcessor property does not affect node it is set on.
			context.service.LocatorManager.GenerateLocators(nodeB);
			assert("Verify that expected processor was called for nodeB.", processor1.CallCount() > 0);
			assertEquals("Verify that unexpected processor was not called.", 0, processor2.CallCount());

			// Verify SubTreeProcessor of children of set node.
			DependencyObject childB = LogicalTreeHelper.FindLogicalNode(nodeB, "dp1Canvas");
			context.service.LocatorManager.GenerateLocators(childB);
			assert("Verify that expected processor was called for childB.", processor2.CallCount() > 0);

			passTest("Verified SubTreeProcessor inheritance.");
			return null;
		}

		/// <summary>
		/// RegisterSelectionProcessor P1 for type A, then RegisterSelectionProcessor P2, for type A.  
		/// Verify that P2 is now used for processing type A selections.
		/// </summary>
		public object processor4(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			CountingSelectionProcessor processor1 = new CountingSelectionProcessor(context.service.LocatorManager);
			CountingSelectionProcessor processor2 = new CountingSelectionProcessor(context.service.LocatorManager);

			context.service.LocatorManager.RegisterSelectionProcessor(processor1, new TextBox().GetType());
			context.service.LocatorManager.RegisterSelectionProcessor(processor2, new TextBox().GetType());

			context.service.LocatorManager.GenerateLocators(new TextBox());
			assert("Check that 1st SelectionProcessor was not used.", processor1.CallCount == 0);
			assert("Check that 2nd SelectionProcessor was used.", processor2.CallCount > 0);

			passTest("Verified re-registering SelectionProcessors.");
			return null;
		}

		/// <summary>
		/// Root of tree has no Id. Set DataIdProcessor.FetchAnnotationAsBatch on root. 
		/// Call Load\Unload.  Verify AttachedAnnotations.
		/// </summary>
		public object processor5(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			((Canvas)context.enablementNode).Name = null;

			Annotation innerAnno = context.MakeAnnotation("_dockpanel1");
			Annotation leafAnno = context.MakeAnnotation("textbox1");

			context.store.AddAnnotation(innerAnno);
			context.store.AddAnnotation(leafAnno);

			assertEquals("Verify initial AttachedAnnotation count.", 2, context.service.GetAttachedAnnotations().Count);

			context.service.UnloadAnnotations(context.enablementNode);
			assertEquals("Verify 0 AttachedAnnotations after unload.", 0, context.service.GetAttachedAnnotations().Count);
			context.service.LoadAnnotations(context.enablementNode);

			assertEquals("Verify final AttachedAnnotation count.", 2, context.service.GetAttachedAnnotations().Count);
			passTest("Verified setting FetchAsBatch on node with no Id.");
			return null;
		}

		/// <summary>
		/// RegisterSelectionProcessor P1 for type A, then RegisterSelectionProcessor P2, for type B.  
		/// Verify that P1 used for processing type A selections, and P2 for type B.
		/// </summary>
		public object processor6(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			
			CountingSelectionProcessor processor1 = new CountingSelectionProcessor(context.service.LocatorManager);
			CountingSelectionProcessor processor2 = new CountingSelectionProcessor(context.service.LocatorManager);

			context.service.LocatorManager.RegisterSelectionProcessor(processor1, new TextBox().GetType());
			context.service.LocatorManager.RegisterSelectionProcessor(processor2, new Canvas().GetType());

			context.service.LocatorManager.GenerateLocators(new TextBox());
			assertEquals("Check that 1st SelectionProcessor was used for TextBox.", 1, processor1.CallCount);
			assertEquals("Check that 2nd SelectionProcessor was not used for TextBox.", 0, processor2.CallCount);

			context.service.LocatorManager.GenerateLocators(new Canvas());
			assertEquals("Check that 2nd SelectionProcessor was used for Canvas.", 1, processor2.CallCount);
			assertEquals("Check that 1st SelectionProcessor was not used for Canvas.", 1, processor1.CallCount);

			passTest("Verified registering multiple SelectionProcessors.");
			return null;
		}

		/// <summary>
		/// Annotation with 1 anchor with 2 locators.  1 locator has an invalid processor, 1 locator
		/// is valid and resolves.  Add annotation, verify 1 attached annotation.
		/// </summary>
		public object processor7(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;

			Annotation anno = AnnotationTestHelper.MakeAnnotation2();
			DependencyObject node = LogicalTreeHelper.FindLogicalNode(context.enablementNode, "_dockpanel1");
			IEnumerator<AnnotationResource> enumer = anno.Anchors.GetEnumerator();
			enumer.MoveNext();
			AnnotationTestHelper.AddLocators(enumer.Current, context.service.LocatorManager.GenerateLocators(node));

			context.store.AddAnnotation(anno);

			assertEquals("Verify the one of two anchors got resolved.", 1, context.service.GetAttachedAnnotations().Count);
			passTest("Verified multiple anchors with missing processor.");
			return null;
		}

		// ----------------------------------------------------------------------------------
		//									  PRIVATE METHODS
		// ----------------------------------------------------------------------------------

		// 

		/// <summary>
		/// Recursively call SetStore on this node and all of its children.  One exception,
		/// do not set store on the 'excludeNode'.  If 'excludeNode' is null, then this method
		/// will set store on all nodes below 'node'.
		/// </summary>
		//private void RecursivelySetStore(DependencyObject node, DependencyObject excludeNode)
		//{
		//    if (node != excludeNode)
		//        AnnotationService.SetStore(node, AnnotationTestHelper.CreateStore(null));

		//    IEnumerator children = LogicalTreeHelper.GetChildren(node).GetEnumerator();
		//    while (children.MoveNext())
		//    {
		//        if (children.Current is DependencyObject)
		//            RecursivelySetStore((DependencyObject)children.Current, excludeNode);
		//    }
		//}

	}
}

