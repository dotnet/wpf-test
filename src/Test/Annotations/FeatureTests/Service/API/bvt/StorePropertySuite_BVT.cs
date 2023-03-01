// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: BVTS testing the Store DependencyProperty for AnnotationService.


using Annotations.Test.Framework;				// TestSuite.
using System;
using System.Threading; using System.Windows.Threading;						// DispatcherOperationCallback.
using System.Windows;
using Proxies.System.Windows.Annotations;
using Proxies.MS.Internal.Annotations.Anchoring;
using Proxies.MS.Internal.Annotations.Component;
using System.Windows.Annotations.Storage;
using System.Windows.Controls;

namespace Avalon.Test.Annotations.BVTs
{
	public class StorePropertySuite_BVT : TestSuite
	{
		protected override void RunTestCase()
		{
			switch (CaseNumber)
			{
				case "storeproperty1": storeproperty1(); break;
				case "storeproperty2": storeproperty2(); break;
				case "storepropertyinvalidated2": storepropertyinvalidated2(); break;

				default:
					failTest("TestSuite '" + this.ToString() + "' does not contain the given test case: '" + CaseNumber + "'.");
					break;
			}
		}

		// ----------------------------------------------------------------------------------
		//                                 TEST METHODS
		// ----------------------------------------------------------------------------------

		/// <summary>
		/// SetStore, Enable on same node. Verify that GetStore returns expected store.
		/// </summary>
		public void storeproperty1()
		{
			ServiceTestContext context = new ServiceTestContext();
			context.enablementNode = AnnotationTestHelper.BuildSingleBranchTree();
			context.store = AnnotationTestHelper.CreateStore(null);

			AnnotationService.SetStore(context.enablementNode, context.store);
			context.service = EnableService(context.enablementNode);
			queueTask(new DispatcherOperationCallback(VerifyStorePropertyIdentityTask), context);
		}

		/// <summary>
		/// Enable, SetStore on same node. Verify that GetStore returns expected store.
		/// </summary>
		public void storeproperty2()
		{
			ServiceTestContext context = new ServiceTestContext();
			Canvas root = AnnotationTestHelper.BuildMultiBranchTree();
			context.enablementNode = LogicalTreeHelper.FindLogicalNode(root, "btnAddAnnot1");
			context.store = AnnotationTestHelper.CreateStore(null);

			context.service = EnableService(context.enablementNode);
			AnnotationService.SetStore(context.enablementNode, context.store);
			queueTask(new DispatcherOperationCallback(VerifyStorePropertyIdentityTask), context);
		}

		/// <summary>
		/// Set AnnotationStore DP to another store.  Verify number of Unloaded and Loaded events.  
		/// Verify that only annotations from the new store exist as AttachedAnnotations.  Verify 
		/// that Service.GetStore is the 2nd store.
		/// </summary>
		public void storepropertyinvalidated2()
		{
			ServiceTestContext context = new ServiceTestContext();
			context.enablementNode = AnnotationTestHelper.BuildMultiBranchTree();
			context.store = AnnotationTestHelper.CreateStore("annotations.xml");

			AnnotationService.SetStore(context.enablementNode, context.store);
			context.service = EnableService(context.enablementNode);

			queueTask(new DispatcherOperationCallback(storepropertyinvalidated2_stage1), context);
		}

		/// <summary>
		/// Set AnnotationStore DP to another store.  Verify number of Unloaded and Loaded events.  
		/// Verify that only annotations from the new store exist as AttachedAnnotations.  Verify 
		/// that Service.GetStore is the 2nd store.
		/// </summary>
		public object storepropertyinvalidated2_stage1(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			context.eventListener = new ServiceEventListener(this, context.service);

			// Add 3 annotations to current store.
			context.store.AddAnnotation(context.MakeAnnotation("textbox1"));
			context.store.AddAnnotation(context.MakeAnnotation("_dockpanel1"));
			context.store.AddAnnotation(context.MakeAnnotation("dp1Canvas"));

			// Change the store DP to another store which contains different annotations.
			AnnotationStore newStore = AnnotationTestHelper.CreateStore("annotations2.xml");
			newStore.AddAnnotation(context.MakeAnnotation("_dockpanel2"));
			newStore.AddAnnotation(context.MakeAnnotation("btnAddAnnot1"));
			AnnotationService.SetStore(context.enablementNode, newStore);
			context.store = newStore;

			// Post task so that Service has time to receive invalidation event
			// before we check the final AttachedAnnotation set.
			queueTask(new DispatcherOperationCallback(storepropertyinvalidated2_stage2), context);
			return null;
		}

		/// <summary>
		/// PASS if number of load and unload events is expected, the number of attached annotations
		/// is expected, and service.Store's identity is confirmed.
		/// </summary>
		public object storepropertyinvalidated2_stage2(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			
			context.eventListener.VerifyUnloadEventCount(3);
			context.eventListener.VerifyLoadEventCount(5);
			assertEquals("Verify no AttachedAnnotations.", 2, context.service.GetAttachedAnnotations().Count);
			assert("Verify store identity.", context.store == context.service.Store);

			passTest("Verified that Service handles StoreDataProperty invalidation events correctly.");
			return null;
		}

		// ----------------------------------------------------------------------------------
		//									  TASKS
		// ----------------------------------------------------------------------------------

		public AnnotationService EnableService(DependencyObject dpO)
		{
			AnnotationService.Enable(dpO);
			return AnnotationService.GetService(dpO);
		}

		/// <summary>
		/// PASS if Service.Store == context.store.
		/// </summary>
		/// <param name="inObj">ServiceTestContext</param>
		public object VerifyStorePropertyIdentityTask(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			assert("Verify store property identity.", context.store == context.service.Store);

			passTest("Verified that Service.Store was expected.");
			return null;
		}
	}
}

