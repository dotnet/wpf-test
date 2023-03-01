// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: AnnotationStore API eventing tests.


using System;
using System.Windows;
using System.Windows.Annotations.Storage;
using System.Windows.Annotations;
using System.Xml;
using Annotations.Test.Framework;

namespace Avalon.Test.Annotations.Suites
{
    [ExecutionGroupCompatible(false)]
    //[TestDimension("-xmlstream,-sql")]
    [TestDimension("-xmlstream")]
    public class EventingSuite_Pri1 : AnnotationStoreAPI
	{
		/// <summary>
		/// Create empty store, add a single annotation, check for event.  
		/// Verify that annotation in event is annotation and ref that 
		/// was added (check content and object id).
		/// </summary>
        [Priority(1)]
        public void eventing3()
		{
			
			AnnotationStore store = CreateStore();
			StoreEventCounter eventCounter = new StoreEventCounter(this, store);

			Annotation anno1 = AnnotationStoreTestHelpers.MakeAnnotation1();
			store.AddAnnotation(anno1);

			eventCounter.verifyContentEventCounts(1, 0);
			eventCounter.verifyLastAddedAnnotation(anno1);

			passTest("Add event and identity verified.");
		}

		/// <summary>
		/// Load a store, try add an existing annotation catch ArgumentException.  
		/// Add new annotation twice, catch ArgumentException.
		/// </summary>
        [Priority(1)]
        public void eventing4()
		{
			SetupStoreWithContent();
			AnnotationStore store = CreateStore();
			StoreEventCounter eventCounter = new StoreEventCounter(this, store);

			Assert("Verify that store initially populated.", store.GetAnnotations().Count > 0);
			Annotation anno = store.GetAnnotations()[0];

			try
			{
				store.AddAnnotation(anno);
				failTest("Second add of existing annotation should not have succeeded.");
			}
			catch (ArgumentException)
			{
				eventCounter.verifyContentEventCounts(0, 0);
			}

			Annotation anno1 = AnnotationStoreTestHelpers.MakeAnnotation1();
			store.AddAnnotation(anno1);
			try
			{
				store.AddAnnotation(anno1);
				failTest("Second add of new annotation should not have succeeded.");
			}
			catch (ArgumentException)
			{
				eventCounter.verifyContentEventCounts(1, 0);
			}

			passTest("Add event counts with illegal adds verified.");
		}

		/// <summary>
		/// Load a store, delete an annotation.  Verify that annotation 
		/// in event is annotation and ref that was deleted.
		/// </summary>
        [Priority(1)]
        public void eventing6()
		{
			SetupStoreWithContent();
			AnnotationStore store = CreateStore();
			StoreEventCounter eventCounter = new StoreEventCounter(this, store);
			Annotation anno = store.GetAnnotations()[0];

			Assert("Verify that delete succeeds.", store.DeleteAnnotation(anno.Id) != null);
			eventCounter.verifyContentEventCounts(0, 1);

			passTest("Delete event received.");
		}

		/// <summary>
		/// Load store, delete twice.  Test that only 1 delete event occurs.
		/// </summary>
        [Priority(1)]
        public void eventing7()
		{
			SetupStoreWithContent();
			AnnotationStore store = CreateStore();
			StoreEventCounter eventCounter = new StoreEventCounter(this, store);
			Annotation anno = store.GetAnnotations()[0];

			Assert("Verify that delete succeeds.", store.DeleteAnnotation(anno.Id) != null);
			Assert("Verify that 2nd delete fails.", store.DeleteAnnotation(anno.Id) == null);
			eventCounter.verifyContentEventCounts(0, 1);

			passTest("Single delete event received.");
		}

		/// <summary>
		/// Delete non-existent annotation doesnt throw an event.
		/// </summary>
        [Priority(1)]
        public void eventing8()
		{
			SetupStoreWithContent();
			AnnotationStore store = CreateStore();
			StoreEventCounter eventCounter = new StoreEventCounter(this, store);
			Annotation anno = AnnotationStoreTestHelpers.MakeAnnotation1();

			Assert("Verify that deleting non-existent annoation fails.", store.DeleteAnnotation(anno.Id) == null);
			eventCounter.verifyContentEventCounts(0, 0);

			passTest("No Delete event received.");
		}

		/// <summary>
		/// Create new store, add then delete new annotation.
		/// </summary>
        [Priority(1)]
        public void eventing10()
		{
			SetupStoreWithContent();
			AnnotationStore store = CreateStore();
			StoreEventCounter eventCounter = new StoreEventCounter(this, store);
			
			Annotation anno = AnnotationStoreTestHelpers.MakeAnnotation1();
			store.AddAnnotation(anno);
			Assert("Delete new annotation.", store.DeleteAnnotation(anno.Id) != null);
			eventCounter.verifyContentEventCounts(1, 1);

			passTest("Delete event for new annotation.");
		}

		/// <summary>
		/// Create new store, add then delete, then add then delete one annotation.
		/// </summary>
        [Priority(1)]
        public void eventing11()
		{

			AnnotationStore store = CreateStore();
			StoreEventCounter eventCounter = new StoreEventCounter(this, store);

			Annotation anno = AnnotationStoreTestHelpers.MakeAnnotation1();
			store.AddAnnotation(anno);
			store.DeleteAnnotation(anno.Id);
			store.AddAnnotation(anno);
			store.DeleteAnnotation(anno.Id);

			eventCounter.verifyContentEventCounts(2, 2);

			passTest("Delete event for new annotation.");
		}

		/// <summary>
		/// Create new store, add then delete new annotation that contains an empty resource. 
		/// </summary>
        [Priority(1)]
        public void eventing12()
		{

			AnnotationStore store = CreateStore();
			StoreEventCounter eventCounter = new StoreEventCounter(this, store);

			Annotation anno = new Annotation(new XmlQualifiedName("eventing12", "store tests"));
			store.AddAnnotation(anno);
			store.DeleteAnnotation(anno.Id);
			
			eventCounter.verifyContentEventCounts(1, 1);

			passTest("Add and delete events for empty resource.");
		}

		/// <summary>
		/// Load store.  Delete existing. Re-add existing. Add new.
		/// </summary>
        [Priority(1)]
        public void eventing13()
		{
			SetupStoreWithContent();
			AnnotationStore store = CreateStore();
			StoreEventCounter eventCounter = new StoreEventCounter(this, store);

			Annotation anno = store.GetAnnotations()[0];
			store.DeleteAnnotation(anno.Id);
			store.AddAnnotation(anno);

			Annotation anno1 = AnnotationStoreTestHelpers.MakeAnnotation1();
			store.AddAnnotation(anno1);

			eventCounter.verifyContentEventCounts(2, 1);

			passTest("Delete, re-add, and new add succeeded.");
		}

		/// <summary>
		/// Load store.  Delete all, then add some.  Check that events still work.
		/// </summary>
        [Priority(1)]
        public void eventing14()
		{
			SetupStoreWithContent();
			AnnotationStore store = CreateStore();
			StoreEventCounter eventCounter = new StoreEventCounter(this, store);

			AssertEquals("Verify initial number of annotations.", 3, store.GetAnnotations().Count);
			while (store.GetAnnotations().Count > 0)
			{
				store.DeleteAnnotation(store.GetAnnotations()[0].Id);
			}

			Annotation anno = AnnotationStoreTestHelpers.MakeAnnotation1();
			Annotation anno1 = AnnotationStoreTestHelpers.MakeAnnotation2();
			store.AddAnnotation(anno);
			store.AddAnnotation(anno1);

			eventCounter.verifyContentEventCounts(2, 3);

			passTest("Delete all, then add works.");
		}

		/// <summary>
		/// Delete on empty store. Add one.  Check that add event occurs.
		/// </summary>
        [Priority(1)]
        public void eventing15()
		{
			AnnotationStore store = CreateStore();
			StoreEventCounter eventCounter = new StoreEventCounter(this, store);

			Annotation anno = AnnotationStoreTestHelpers.MakeAnnotation1();
			Assert("Delete on empty store failed.", store.DeleteAnnotation(anno.Id) == null);
			store.AddAnnotation(anno);

			eventCounter.verifyContentEventCounts(1, 0);

			passTest("Delete on empty, then add.");
		}

		/// <summary>
		/// Create store. Add/modify/delete annotations. Add/delete annotations.  Check content event counts.
		/// </summary>
        [Priority(1)]
        public void eventing16()
		{
			AnnotationStore store = CreateStore();
			StoreEventCounter eventCounter = new StoreEventCounter(this, store);

			Annotation anno = AnnotationStoreTestHelpers.MakeAnnotation1();
			Annotation anno1 = AnnotationStoreTestHelpers.MakeAnnotation2();

			store.AutoFlush = true;

			store.AddAnnotation(anno1);
			anno1.Cargos.Add(AnnotationStoreTestHelpers.createCargo("eventing15", "StoreTests", "Some content"));
			store.AddAnnotation(anno);
			store.DeleteAnnotation(anno1.Id);
			store.AddAnnotation(anno1);

			eventCounter.verifyContentEventCounts(3, 1);

			store.DeleteAnnotation(anno.Id);
			store.AddAnnotation(anno);

			eventCounter.verifyContentEventCounts(4, 2);

			passTest("Verified event count for string of content changes.");
		}

		/// <summary>
		/// Set autoflush on. Add/delete.  Check events.
		/// </summary>
        [Priority(1)]
        public void eventing17()
		{
			AnnotationStore store = CreateStore();
			StoreEventCounter eventCounter = new StoreEventCounter(this, store);

			store.AutoFlush = true;

			Annotation anno = AnnotationStoreTestHelpers.MakeAnnotation1();
			store.AddAnnotation(anno);
			store.DeleteAnnotation(anno.Id);

			eventCounter.verifyContentEventCounts(1, 1);

			passTest("Eventing still works with autoflush.");
		}
	}
}

