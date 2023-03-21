// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Pri1s testing the Store DependencyProperty API for AnnotationService.


using Annotations.Test.Framework;				// TestSuite.
using System;
using System.IO;							// Stream.
using System.Collections;
using System.Threading; using System.Windows.Threading;						// DispatcherOperationCallback.
using System.Windows;
using Proxies.System.Windows.Annotations;
using Proxies.MS.Internal.Annotations.Anchoring;
using Proxies.MS.Internal.Annotations.Component;
using System.Windows.Annotations.Storage;
using System.Windows.Controls;
using System.Xml;

namespace Avalon.Test.Annotations.Pri1s
{
	public class StorePropertySuite_Pri1 : TestSuite
	{
		protected override void RunTestCase()
		{
			switch (CaseNumber)
			{
				case "storeproperty3": storeproperty3(); break;
				case "storeproperty4": storeproperty4(); break;
				case "storeproperty5": storeproperty5(); break;
				case "storeproperty6": storeproperty6(); break;
				case "storeproperty7": storeproperty7(); break;
				case "storeproperty8": storeproperty8(); break;
				case "storeproperty9": storeproperty9(); break;
				case "storeproperty10": storeproperty10(); break;

				case "storepropertyinvalidated1": storepropertyinvalidated1(); break;
				case "storepropertyinvalidated3": storepropertyinvalidated3(); break;
				case "storepropertyinvalidated4": storepropertyinvalidated4(); break;

				default:
					failTest("TestSuite '" + this.ToString() + "' does not contain the given test case: '" + CaseNumber + "'.");
					break;
			}
		}

		/// <summary>
		/// Enable on some node in tree. SetStore on each node in tree, no exceptions. 
		/// Verify that Service.Store returns expected store.
		/// </summary>
		public void storeproperty3()
		{
			Canvas root = AnnotationTestHelper.BuildMultiBranchTree();
			ServiceTestContext context = ServiceTestContext.SetupWithServiceAndStore(this, LogicalTreeHelper.FindLogicalNode(root, "_dockpanel2"));
		
			RecursivelySetStore(root, context.enablementNode);
			assert("Verify Service.Store identity.", context.service.Store == context.store);
			passTest("Verified that Service uses correct Store DP.");
		}

		/// <summary>
		/// SetStore on each node in the tree except one that the Service is Enabled on.  
		/// Verify that Service.GetStore returns null.
		/// </summary>
		public void storeproperty4()
		{
			Canvas root = AnnotationTestHelper.BuildMultiBranchTree();
			ServiceTestContext context = new ServiceTestContext();
			context.enablementNode = LogicalTreeHelper.FindLogicalNode(root, "_dockpanel1");
			AnnotationService.Enable(context.enablementNode);

			RecursivelySetStore(root, context.enablementNode);
			assert("Verify Service.Store is null.", AnnotationService.GetService(context.enablementNode).Store == null);
			passTest("Verified that Service uses correct Store DP.");
		}

		/// <summary>
		/// SetStore(null, X) throws NullArgumentException.
		/// </summary>
		public void storeproperty5()
		{
			try
			{
				AnnotationService.SetStore(null, AnnotationTestHelper.CreateStore(null));
			}
			catch (Exception e)
			{
				if (e is ArgumentNullException)
					passTest("Expected exception occurred.");
				failTest("Received unexpected exception: " + e.ToString());
			}
			failTest("Should have been an exception.");
		}

		/// <summary>
		/// SetStore(X, null) throws Null ArgumentException.
		/// </summary>
		public void storeproperty6()
		{
			Canvas root = AnnotationTestHelper.BuildMultiBranchTree();
			try
			{
				AnnotationService.SetStore(root, null);
			}
			catch (Exception e)
			{
				if (e is ArgumentNullException)
					passTest("Expected exception occurred.");
				failTest("Received unexpected exception: " + e.ToString());
			}
			failTest("Should have been an exception.");
		}

		/// <summary>
		/// SetStore on non-root. Verify that GetStore on children and parent return null.
		/// </summary>
		public void storeproperty7()
		{
			Canvas root = AnnotationTestHelper.BuildMultiBranchTree();
			DependencyObject nonRoot = LogicalTreeHelper.FindLogicalNode(root, "_dockpanel2");
			DependencyObject child = LogicalTreeHelper.FindLogicalNode(nonRoot, "textbox1");

			AnnotationService.SetStore(nonRoot, AnnotationTestHelper.CreateStore(null));
			assert("Store of parent is null.", AnnotationService.GetStore(root) == null);
			assert("Store of child is null.", AnnotationService.GetStore(child) == null);
			
			passTest("Verified non-inheritence of Store DP.");
		}

		/// <summary>
		/// SetStore twice on Service root.  Verify that GetStore returns second store.
		/// </summary>
		public void storeproperty8()
		{
			Canvas root = AnnotationTestHelper.BuildMultiBranchTree();
			AnnotationStore store1 = AnnotationTestHelper.CreateStore(null);
			AnnotationStore store2 = AnnotationTestHelper.CreateStore(null);

			AnnotationService.SetStore(root, store1);
			AnnotationService.SetStore(root, store2);

			assert("Verify store identity.", AnnotationService.GetStore(root) == store2);

			passTest("Verified changing store DP.");
		}

		/// <summary>
		/// Two services on sibling nodes. SetStore with unique Store for each.  Verify store property.
		/// </summary>
		public void storeproperty9()
		{
			Canvas root = AnnotationTestHelper.BuildMultiBranchTree();
			DependencyObject tree1 = LogicalTreeHelper.FindLogicalNode(root, "_dockpanel1");
			DependencyObject tree2 = LogicalTreeHelper.FindLogicalNode(root, "_dockpanel2");

			ServiceTestContext context1 = ServiceTestContext.SetupWithServiceAndStore(this, tree1, "anno1.xml");
			ServiceTestContext context2 = ServiceTestContext.SetupWithServiceAndStore(this, tree2, "anno2.xml");

			assert("Verify service 1's store.", context1.service.Store == context1.store);
			assert("Verify service 2's store.", context2.service.Store == context2.store);

			passTest("Verified Store DP for parallel services.");
		}

		/// <summary>
		/// Two services on sibling nodes.  SetStore with same Store.  Verify that store property is the same.
		/// </summary>
		public void storeproperty10()
		{
			Canvas root = AnnotationTestHelper.BuildMultiBranchTree();
			DependencyObject tree1 = LogicalTreeHelper.FindLogicalNode(root, "_dockpanel1");
			DependencyObject tree2 = LogicalTreeHelper.FindLogicalNode(root, "_dockpanel2");
			
			AnnotationService.Enable(tree1);
			AnnotationService.Enable(tree2);

			AnnotationStore store = AnnotationTestHelper.CreateStore(null);
			AnnotationService.SetStore(tree1, store);
			AnnotationService.SetStore(tree2, store);

			assert("Verify service 1's store.", AnnotationService.GetService(tree1).Store == store);
			assert("Verify service 2's store.", AnnotationService.GetService(tree2).Store == store);

			passTest("Verified shared Store for parallel services.");
		}

		/// <summary>
		/// Set AnnotationStore DP to null.  Verify that ArgumentNullException is thrown.
		/// </summary>
		public void storepropertyinvalidated1()
		{
			Canvas root = AnnotationTestHelper.BuildMultiBranchTree();
			try
			{
				AnnotationService.SetStore(root, null);
			}
			catch (ArgumentNullException)
			{
				passTest("Expected exception occurred for SetStore(x, null).");
			}
			catch (Exception e)
			{
				failTest("Unexpected exception occurred for SetStore(x, null): " + e.Message + ".");
			}
			failTest("Excepted exception for SetStore(x, null).");
		}

		/// <summary>
		/// Set AnnotationStore DP to an empty store.  Verify unload events, and no attached annotations.  
		/// Set DP back to original store.  Verify load events and the number of attached annotations is 
		/// the same as before.
		/// </summary>
		public void storepropertyinvalidated3()
		{
			Canvas root = AnnotationTestHelper.BuildMultiBranchTree();
			ServiceTestContext context = ServiceTestContext.SetupWithServiceAndStore(this, root);
			DataIdProcessor.SetFetchAnnotationsAsBatch(root, true);
			queueTask(new DispatcherOperationCallback(storepropertyinvalidated3_stage1), context);
		}

		public object storepropertyinvalidated3_stage1(object inobj)
		{
			ServiceTestContext context = (ServiceTestContext)inobj;
			Hashtable annotationMap = AnnotationTestHelper.AnnotateMultiBranchTree(context.enablementNode);
			assertEquals("Verify initial number of annotations.", 8, context.service.GetAttachedAnnotations().Count);

			// Reset counts.
			context.eventListener.Reset();

			// Set AnnotationStore DP to an empty store.
			AnnotationService.SetStore(context.enablementNode, new XmlStreamStore(new MemoryStream()));
			queueTask(new DispatcherOperationCallback(storepropertyinvalidated3_stage2), context);
			return null;
		}

		public object storepropertyinvalidated3_stage2(object inobj)
		{
			ServiceTestContext context = (ServiceTestContext)inobj;
			assertEquals("Verify no attached annotations.", 0, context.service.GetAttachedAnnotations().Count);
			context.eventListener.VerifyUnloadEventCount(8);

			// Set AnnotationStore DP back.
			AnnotationService.SetStore(context.enablementNode, context.store);
			queueTask(new DispatcherOperationCallback(storepropertyinvalidated3_stage3), context);
			return null;
		}

		public object storepropertyinvalidated3_stage3(object inobj)
		{
			ServiceTestContext context = (ServiceTestContext)inobj;
			assertEquals("Verify attached annotations.", 8, context.service.GetAttachedAnnotations().Count);
			context.eventListener.VerifyLoadEventCount(8);

			passTest("Verified setting store property to empty store and back.");
			return null;
		}

		/// <summary>
		/// Set AnnotationStore DP to the same store.  Event but no changes to Service.
		/// </summary>
		public void storepropertyinvalidated4()
		{
			Canvas root = AnnotationTestHelper.BuildMultiBranchTree();
			ServiceTestContext context = ServiceTestContext.SetupWithServiceAndStore(this, root);
			queueTask(new DispatcherOperationCallback(storepropertyinvalidated4_stage1), context);
		}

		public object storepropertyinvalidated4_stage1(object inobj)
		{
			ServiceTestContext context = (ServiceTestContext)inobj;
			Hashtable annotationMap = AnnotationTestHelper.AnnotateMultiBranchTree(context.enablementNode);
			assertEquals("Verify initial number of annotations.", 8, context.service.GetAttachedAnnotations().Count);

			// Reset counts.
			context.eventListener.Reset();

			// Set AnnotationStore DP to same store
			AnnotationService.SetStore(context.enablementNode, context.store);
			queueTask(new DispatcherOperationCallback(storepropertyinvalidated4_stage2), context);
			return null;
		}

		public object storepropertyinvalidated4_stage2(object inobj)
		{
			ServiceTestContext context = (ServiceTestContext)inobj;
			context.eventListener.VerifyEventCounts(0, 0, 0);

			passTest("Verified no action when SetStore to same value.");
			return null;
		}


		// ----------------------------------------------------------------------------------
		//									  TASKS
		// ----------------------------------------------------------------------------------



		// ----------------------------------------------------------------------------------
		//									  PRIVATE METHODS
		// ----------------------------------------------------------------------------------

		/// <summary>
		/// Recursively call SetStore on this node and all of its children.  One exception,
		/// do not set store on the 'excludeNode'.  If 'excludeNode' is null, then this method
		/// will set store on all nodes below 'node'.
		/// </summary>
		private void RecursivelySetStore(DependencyObject node, DependencyObject excludeNode)
		{
			if (node != excludeNode)
				AnnotationService.SetStore(node, AnnotationTestHelper.CreateStore(null));

			IEnumerator children = LogicalTreeHelper.GetChildren(node).GetEnumerator();
			while (children.MoveNext())
			{
				if (children.Current is DependencyObject)
					RecursivelySetStore((DependencyObject)children.Current, excludeNode);
			}
		}

	}
}

