// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: AnnotationStore API bvts.


using System;
using System.Threading; 
using System.Windows.Threading;
using System.Windows;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Windows.Annotations;
using System.Windows.Annotations.Storage;
using Annotations.Test.Framework;

namespace Avalon.Test.Annotations.Suites
{
	/// <summary>
	/// All annotation store test cases.
	/// </summary>
    [ExecutionGroupCompatible(false)]
    [TestDimension("-xmlstream")]
	public class AnnotationStoreAPI_BVT : AnnotationStoreAPI
	{	
		#region Flush Tests

        /// <summary>
        /// Add annot, flush.  Dete annot, close store.  Verify annotation still exists.
        /// </summary>
        [Priority(1)]
        public void flush1_1()
        {
            AnnotationStore store = CreateStore();
            Annotation annot = AnnotationStoreTestHelpers.MakeAnnotation1();
            store.AddAnnotation(annot);
            store.Flush();
            store.DeleteAnnotation(annot.Id);
            CloseStore();
            verifyStoreContents(new Annotation[] { annot });
            passTest("Verified that delete is not applied if no flush occurs.");
        }

		/// <summary>
		/// Auto-flush on, add annotations, do not flush. Annotations in the file.
		/// </summary>
        [Priority(1)]
        public void flush2()
		{			
            AnnotationStore store = CreateStore();
			store.AutoFlush = true;

			Annotation annotation = new Annotation(new XmlQualifiedName("flush2", "http://StoreTests"));
			store.AddAnnotation(annotation);
            CloseStore();

            verifyStoreContents(new Annotation[] { annotation });
            passTest("Contents verified.");
		}

		/// <summary>
		/// Add annotations and flush. Annotations should be in the file.
		/// </summary>
        [Priority(1)]
        public void flush3()
		{
            AnnotationStore store = CreateStore();

			Annotation annotation = new Annotation(new XmlQualifiedName("flush2", "http://StoreTests"));
			store.AddAnnotation(annotation);
			store.Flush();
			CloseStore();

            verifyStoreContents(new Annotation[] { annotation });
            passTest("Contents verified.");
		}

		#endregion Flush Tests

		#region Delete Tests

		/// <summary>
		/// Add an annotation, delete the annotation, add another annotation flush and then close. 
		/// Make sure we have the second annotation only in the store and that it is correct.
		/// </summary>
        [Priority(1)]
        public void delete1()
		{
			AnnotationStore store = CreateStore();

			Annotation anno1 = AnnotationStoreTestHelpers.MakeAnnotation1();
			Annotation anno2 = AnnotationStoreTestHelpers.MakeAnnotation1();

			store.AddAnnotation(anno1);
			store.DeleteAnnotation(anno1.Id);

			store.AddAnnotation(anno2);
			store.Flush();

			CloseStore();

			verifyStoreContents(new Annotation[] { anno2 });
			passTest("Store contents verified.");
		}

		/// <summary>
		/// Auto-flush on, add an annotation, make sure annotation is in the store, delete the annotation, 
		/// add another annotation flush and then close. Make sure we have the second annotation only in 
		/// the store and that it is correct.
		/// </summary>
        [Priority(1)]
        public void delete2()
		{
			AnnotationStore store = CreateStore();

			Annotation anno1 = AnnotationStoreTestHelpers.MakeAnnotation1();
			Annotation anno2 = AnnotationStoreTestHelpers.MakeAnnotation1();

			store.AddAnnotation(anno1);
			verifyStoreContents(store, new Annotation[] { anno1 });

			store.DeleteAnnotation(anno1.Id);
			store.AddAnnotation(anno2);
			store.Flush();
			CloseStore();

            verifyStoreContents(new Annotation[] { anno2 });
			passTest("Store contents verified.");
		}

        [Priority(1)]
        public void delete3()
        {
            AnnotationStore store = CreateStore();
            Annotation annot = AnnotationStoreTestHelpers.MakeAnnotation1();
            store.AddAnnotation(annot);
            verifyStoreContents(store, new Annotation[] { annot });
            store.DeleteAnnotation(annot.Id);
            AssertEquals("Verify number of annotations.", 0, store.GetAnnotations().Count);
            store.AddAnnotation(annot);
            verifyStoreContents(store, new Annotation[] { annot });
            passTest("Can delete and re-add annotation.");
        }

		#endregion Delete Tests

		#region Query Tests

		/// <summary>
		/// Create empty store. Add annotation. Check that GetAnnotation and 
		/// GetAnnotations return references to the original annotation.
		/// </summary>
        [Priority(1)]
        public void query1()
		{
			AnnotationStore store = CreateStore();
			Annotation anno1 = AnnotationStoreTestHelpers.MakeAnnotation1();
			store.AddAnnotation(anno1);

			Assert("Check that GetAnnotation returns reference to original annotation.", anno1 == store.GetAnnotation(anno1.Id));
			Assert("Check that GetAnnotations returns reference to original annotation.", anno1 == store.GetAnnotations()[0]);
			CloseStore();

			passTest("Get methods return values verified.");
		}

		#endregion Query Tests

		#region Lifecycle Tests

		/// <summary>
		/// Open store.  Add annotations.  Flush store.  
		/// Close store.  Create new store on same stream/file. Flush store.
		/// </summary>
        [Priority(1)]
        public void lifecycle4()
		{			
			AnnotationStore store = CreateStore();
			Annotation anno1 = AnnotationStoreTestHelpers.MakeAnnotation1();			
			store.AddAnnotation(anno1);
			store.Flush();
			CloseStore();

			store = CreateStore();
			Annotation anno2 = AnnotationStoreTestHelpers.MakeAnnotation2();
			store.AddAnnotation(anno2);
			store.Flush();

			verifyStoreContents(store, new Annotation[] { anno1, anno2 });
			CloseStore();

			passTest("Verified flush after store re-opened.");
		}


		#endregion Lifecycle Tests

		#region Eventing Tests

		/// <summary>
		/// Add several different new annotations.  Check for events.
		/// </summary>
        [Priority(1)]
        public void eventing5()
		{
			AnnotationStore store = CreateStore();
			StoreEventCounter eventCounter = new StoreEventCounter(this, store);

			Annotation anno1 = AnnotationStoreTestHelpers.MakeAnnotation1();
			Annotation anno2 = AnnotationStoreTestHelpers.MakeAnnotation2();
			Annotation anno3 = new Annotation(new XmlQualifiedName("eventing5", "StoreTests"));
			anno3.Authors.Add("eventing5Author");

			store.AddAnnotation(anno1);
			store.AddAnnotation(anno2);
			store.AddAnnotation(anno3);

			eventCounter.verifyContentEventCounts(3, 0);
            CloseStore();

			passTest("Add events accounted for.");
		}

		/// <summary>
		/// Load store, delete multiple annotations.
		/// </summary>
        [Priority(1)]
        public void eventing9()
		{
            SetupStoreWithContent();

			AnnotationStore store = CreateStore();
			StoreEventCounter eventCounter = new StoreEventCounter(this, store);
            
            IList<Annotation> annotations = store.GetAnnotations();
            AssertEquals("Verify annotation count.", 3, annotations.Count);

			Assert("1st delete doesn't fail", store.DeleteAnnotation(annotations[0].Id) != null);
            Assert("2nd delete doesn't fail", store.DeleteAnnotation(annotations[1].Id) != null);

			eventCounter.verifyContentEventCounts(0, 2);
            CloseStore();

			passTest("All delete events accounted for.");
		}

		/// <summary>
		/// Add/delete annotations.  Call flush. Add/delete more annotations. Check events.
		/// </summary>
        [Priority(1)]
        public void eventing18()
		{			
			AnnotationStore store = CreateStore();
			StoreEventCounter eventCounter = new StoreEventCounter(this, store);

			Annotation anno = AnnotationStoreTestHelpers.MakeAnnotation1();
			store.AddAnnotation(anno);
			store.DeleteAnnotation(anno.Id);

			store.Flush();

			Annotation anno1 = AnnotationStoreTestHelpers.MakeAnnotation2();
			store.AddAnnotation(anno1);
			store.DeleteAnnotation(anno1.Id);

			eventCounter.verifyContentEventCounts(2, 2);
            CloseStore();

			passTest("Eventing still works after flush.");
		}

		#endregion Eventing Tests
	}
}

