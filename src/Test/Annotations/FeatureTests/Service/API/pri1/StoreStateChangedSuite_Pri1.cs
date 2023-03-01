// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Pri1's testing how AnnotationService handles StoreStateChanged events.


using Annotations.Test.Framework;				// TestSuite.
using System;
using System.Threading; using System.Windows.Threading;						// DispatcherOperationCallback.
using System.Windows;
using System.IO;							// File.
using Proxies.System.Windows.Annotations;
using Proxies.MS.Internal.Annotations.Anchoring;
using Proxies.MS.Internal.Annotations.Component;
using System.Windows.Annotations.Storage;
using System.Windows.Controls;
using System.Xml;

namespace Avalon.Test.Annotations.Pri1s
{
	/// <summary>
	/// NOTE: Each test case is broken into 1 or more context tasks, this is because
	/// state change events are processed asynchronously by the AnnotationService,
	/// therefore, we post a task after each state change to allow the service time
	/// to process these events.
	/// </summary>
	public class StoreStateChangedSuite_Pri1 : TestSuite
	{
		protected string ANNOTATION_STORE_FILE = "annotations.xml";

		protected override void RunTestCase()
		{
			Canvas root = AnnotationTestHelper.BuildMultiBranchTree();
			ServiceTestContext context = ServiceTestContext.SetupWithService(this, root);

			// Clean up after old tests.
			if (File.Exists(ANNOTATION_STORE_FILE))
				File.Delete(ANNOTATION_STORE_FILE);

			switch (CaseNumber)
			{
				case "storestatechanged_ready1": queueTask(new DispatcherOperationCallback(storestatechanged_ready1), context); break;
				case "storestatechanged_ready2": queueTask(new DispatcherOperationCallback(storestatechanged_ready2), context); break;

				case "storestatechanged_unspecified1": queueTask(new DispatcherOperationCallback(storestatechanged_unspecified1), context); break;
				case "storestatechanged_unspecified2": queueTask(new DispatcherOperationCallback(storestatechanged_unspecified2), context); break;

				default:
					failTest("TestSuite '" + this.ToString() + "' does not contain the given test case: '" + CaseNumber + "'.");
					break;
			}
		}

		// ----------------------------------------------------------------------------------
		//                                 TEST METHODS
		// ----------------------------------------------------------------------------------

		/// <summary>
		/// Empty store, change store state to Ready.  AddAnnotation, verify event. Modify 
		/// annotations anchor, verify event.
		/// </summary>
		public object storestatechanged_ready1(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			XmlStreamStore store = (XmlStreamStore) AnnotationTestHelper.CreateStore(null);
			AnnotationService.SetStore(context.enablementNode, store);

			//
			context.store = store;

			// Post task to let property change event propagate through.
			queueTask(new DispatcherOperationCallback(storestatechanged_ready1_stage1), context);
			return null;
		}

		public object storestatechanged_ready1_stage1(object inObj) 
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			Annotation anno = new Annotation(new XmlQualifiedName("simple", "Test"));
			AnnotationResource anchor = context.MakeAnchor("_dockpanel2");
			anno.Anchors.Add(anchor);
			context.store.AddAnnotation(anno);		// Load event.
			anchor.Name = "modified";				// Modified event.
			context.store.DeleteAnnotation(anno.Id);// Unload event.
			context.eventListener.VerifyEventCounts(1, 1, 1);

			passTest("Verified that service registers for events when store state changes to Ready.");
			return null;
		}

		/// <summary>
		/// Store with mix of partially, fully, and un-resolved annotations. Change state to Ready.  
		/// Verify that load event count and AttachedAnnotations.
		/// </summary>
		public object storestatechanged_ready2(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			XmlStreamStore store = (XmlStreamStore) AnnotationTestHelper.CreateStore(null);
			AnnotationService.SetStore(context.enablementNode, store);
			
			CreatePopulatedStoreFile(context);
			//
			context.store = store;
			queueTask(new DispatcherOperationCallback(storestatechanged_ready2_stage1), context);
			return null;
		}

		public object storestatechanged_ready2_stage1(object inObj) 
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			context.eventListener.VerifyEventCounts(2, 0, 0);
			assertEquals("Verify number of attached annotations.", 2, context.service.GetAttachedAnnotations().Count);

			passTest("Verified that service loads annotations on StoreStateChange to ready.");
			return null;
		}

		#region Disabled

		/// <summary>
		/// Empty store, Add annotation, modify, delete.  Verify events.  Change state to Unspecified.  
		/// Add, modify, delete annotation from store.  Verify no events.
		/// </summary>
		public object storestatechanged_unspecified1(object inObj)
		{
			failTest("Disabled");
//			ServiceTestContext context = (ServiceTestContext)inObj;
//			context.store = new MyTestStore(ANNOTATION_STORE_FILE);
//			AnnotationService.SetStore(context.enablementNode, context.store);
//			queueTask(new UIContextOperationCallback(storestatechanged_unspecified1_stage1), context);

			return null;
		}

		public object storestatechanged_unspecified1_stage1(object inObj)
		{
//			ServiceTestContext context = (ServiceTestContext)inObj;
//			assertEquals("Verify store is ready.", StoreState.Ready, context.store.StoreState);
//			printStatus("Store state = Ready");
//
//			AnnotationResource anchor = context.MakeAnchor("mainDockPanel");
//			Annotation anno = new Annotation(new XmlQualifiedName("simple", "Test"));
//			anno.Anchors.Add(anchor);
//
//			context.store.AddAnnotation(anno);
//			anchor.Name = "modified";
//			context.store.DeleteAnnotation(anno.Id);
//			context.eventListener.VerifyEventCounts(1, 1, 1);
//
//			context.eventListener.Reset();
//			((MyTestStore)context.store).ToggleState(StoreState.Unspecified);
//			queueTask(new UIContextOperationCallback(storestatechanged_unspecified1_stage2), context);

			return null;
		}

		public object storestatechanged_unspecified1_stage2(object inObj)
		{
//			ServiceTestContext context = (ServiceTestContext)inObj;
//			assertEquals("Verify store is Unspecified.", StoreState.Unspecified, context.store.StoreState);
//			printStatus("Store state = Unspecified");
//
//			AnnotationResource anchor = context.MakeAnchor("textbox1");
//			Annotation anno = new Annotation(new XmlQualifiedName("simple", "Test"));
//			anno.Anchors.Add(anchor);
//
//			context.store.AddAnnotation(anno);
//			anchor.Name = "modified";
//			context.store.DeleteAnnotation(anno.Id);
//			context.eventListener.VerifyEventCounts(0, 0, 0);
//
//			passTest("Verified that Service registers and unregisters for store events on state changes.");
			return null;
		}


		/// <summary>
		/// Store with annotations, change state to unspecified.  Verify unloaded event 
		/// count and no AttachedAnnotations.
		/// </summary>
		public object storestatechanged_unspecified2(object inObj)
		{
//			ServiceTestContext context = (ServiceTestContext)inObj;
//			CreatePopulatedStoreFile(context);
//			context.store = new MyTestStore(ANNOTATION_STORE_FILE);
//			AnnotationService.SetStore(context.enablementNode, context.store);
//
//			queueTask(new UIContextOperationCallback(storestatechanged_unspecified2_stage1), context);
			failTest("Disabled");

			return null;
		}

		public object storestatechanged_unspecified2_stage1(object inObj)
		{
//			ServiceTestContext context = (ServiceTestContext)inObj;
//			context.eventListener.VerifyEventCounts(2, 0, 0);
//			context.eventListener.Reset();
//			((MyTestStore)context.store).ToggleState(StoreState.Unspecified);
//			printStatus("Set store state to Unspecified.");
//			queueTask(new UIContextOperationCallback(storestatechanged_unspecified2_stage2), context);

			return null;
		}

		public object storestatechanged_unspecified2_stage2(object inObj)
		{
//			ServiceTestContext context = (ServiceTestContext)inObj;
//			context.eventListener.VerifyEventCounts(0, 2, 0);
//			passTest("Verified unloads when store state becomes Unspecified.");
			return null;
		}

//		/// <summary>
//		/// Add a method for explicitily changing the State of this store for
//		/// testing purposes.
//		/// </summary>
//		class MyTestStore : XmlStreamStore
//		{
//			public MyTestStore(string filename) : base(filename)
//			{
//				//empty.
//			}
//
//			/// <summary>
//			/// Method for toggling the state of an xmlfile store so that we can
//			/// test the behavior of an AnnotationService.
//			/// </summary>
//			/// <param name="state"></param>
//			public void ToggleState(StoreState state)
//			{
//				OnStoreStateChanged(state);
//			}
//		}

		#endregion Disabled

		// ----------------------------------------------------------------------------------
		//                                 PRIVATE METHODS
		// ----------------------------------------------------------------------------------

		/// <summary>
		/// Create an XmlStreamStore that contains 3 annotions, 1 fully, 1 partially, and 
		/// 1 unresolved. Writes to ANNOTATION_STORE_FILE.
		/// </summary>
		private void CreatePopulatedStoreFile(ServiceTestContext context)
		{
			if (File.Exists(ANNOTATION_STORE_FILE))
				File.Delete(ANNOTATION_STORE_FILE);
			FileStream stream = new FileStream(ANNOTATION_STORE_FILE, FileMode.OpenOrCreate);
			AnnotationStore storeWithContent = new XmlStreamStore(stream);

			// Add fully resolved annotation.
			Annotation resolvedAnno = context.MakeAnnotation("_dockpanel1");
			storeWithContent.AddAnnotation(resolvedAnno);

			// Add partially resolved annotation.
			Annotation partiallyResolvedAnno = context.MakePartiallyResolvedAnnotation("textbox1");
			storeWithContent.AddAnnotation(partiallyResolvedAnno);

			// Add unresolved annotation.
			Annotation unresolvedAnno = AnnotationTestHelper.makeSimpleAnnotation(context.service, new Canvas());
			storeWithContent.AddAnnotation(unresolvedAnno);

			storeWithContent.Flush();
			storeWithContent.Dispose();
			stream.Close();
		}
	}
}

