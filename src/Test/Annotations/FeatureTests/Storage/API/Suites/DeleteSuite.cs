// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 


using System;
using System.Windows;
using System.Windows.Annotations.Storage;
using System.Windows.Annotations;
using System.Collections.Generic;
using Annotations.Test.Framework;

namespace Avalon.Test.Annotations.Suites
{
    [ExecutionGroupCompatible(false)]
    [TestDimension("-xmlstream")]
	public class DeleteSuite_Pri1 : AnnotationStoreAPI
	{
		/// <summary>
		/// Add an annotation, flush the store, delete the annotation, 
		/// add another annotation and flush. Make sure that only the second annotation is stored.
		/// </summary>
		/// <param name="frmk"></param>
        [Priority(1)]
        public void delete3()
		{

			AnnotationStore store = CreateStore();

			Annotation anno1 = AnnotationStoreTestHelpers.MakeAnnotation1();
			Annotation anno2 = AnnotationStoreTestHelpers.MakeAnnotation1();

			store.AddAnnotation(anno1);
			store.Flush();

			store.DeleteAnnotation(anno1.Id);
			store.AddAnnotation(anno2);
			store.Flush();

			verifyStoreContents(store, new Annotation[] { anno2 });
			passTest("Store contents verified.");
		}

		/// <summary>
		/// Create a store, try to delete an annotation,then add an annotation, and flush. 
		/// Make sure first returns null and the second annotation is correct in the store.
		/// </summary>
		/// <param name="frmk"></param>
        [Priority(1)]
        public void delete4()
		{

			AnnotationStore store = CreateStore();

			Annotation anno1 = AnnotationStoreTestHelpers.MakeAnnotation1();
			Assert("Check invalid delete return value is null.", store.DeleteAnnotation(anno1.Id) == null);

			store.AddAnnotation(anno1);

			verifyStoreContents(store, new Annotation[] { anno1 });
			passTest("Store contents verified.");
		}

		/// <summary>
		/// Add an annotation, add another, delete the first annotation twice, make sure the second call returns null. 
		/// Add another annotation. Close the store and make sure there are two annotations in the store.
		/// </summary>
		/// <param name="frmk"></param>
        [Priority(1)]
        public void delete5()
		{

			AnnotationStore store = CreateStore();

			Annotation anno1 = AnnotationStoreTestHelpers.MakeAnnotation1();
			Annotation anno2 = AnnotationStoreTestHelpers.MakeAnnotation2();

			store.AddAnnotation(anno1);
			store.AddAnnotation(anno2);

			Assert("Make sure first delete succeeds.", store.DeleteAnnotation(anno1.Id) != null);
			Assert("Make sure second delete fails.", store.DeleteAnnotation(anno1.Id) == null);

			Annotation anno3 = AnnotationStoreTestHelpers.MakeAnnotation2();
			store.AddAnnotation(anno3);

			verifyStoreContents(store, new Annotation[] { anno2, anno3 });
			passTest("Store contents verified.");
		}

		/// <summary>
		/// Add an annotation a1, try to add the same annotation a1, should fail. Delete a1, add a1 again. 
		/// This should succeed. Make sure only one is present in the store.
		/// </summary>
		/// <param name="frmk"></param>
        [Priority(1)]
        public void delete6()
		{

			AnnotationStore store = CreateStore();

			Annotation anno1 = AnnotationStoreTestHelpers.MakeAnnotation1();
			store.AddAnnotation(anno1);
			try
			{
				store.AddAnnotation(anno1);
				failTest("Adding annotation twice should not have succeeded.");
			}
			catch (ArgumentException)
			{
				printStatus("ArgumentException caught for second add on single annotation.");
			}

			store.DeleteAnnotation(anno1.Id);
			store.AddAnnotation(anno1);

			verifyStoreContents(store, new Annotation[] { anno1 });
			passTest("Store contents verified.");
		}

		/// <summary>
		/// Delete all annotations makes size of GetAnnotations is zero. 
		/// Check that GetAnnotation(Guid) returns null.
		/// </summary>
		/// <param name="frmk"></param>
        [Priority(1)]
        public void delete7()
		{
            SetupStoreWithContent();
			AnnotationStore store = CreateStore();

			Assert("Check that there are initially annotations in the store.", store.GetAnnotations().Count > 0);

			Annotation anno1 = store.GetAnnotations()[0];

			while (store.GetAnnotations().Count > 0)
				store.DeleteAnnotation(store.GetAnnotations()[0].Id);

			Assert("Make sure getAnnotation on empty store returns null.", store.GetAnnotation(anno1.Id) == null);

			passTest("Delete all successful.");
		}

		/// <summary>
		/// Load a store, delete an annotation. Check that delete returned the correct annotation.
		/// </summary>
		/// <param name="StoreType"></param>
        [Priority(1)]
        public void delete8()
		{
			SetupStoreWithContent();
			AnnotationStore store = CreateStore();

			IList<Annotation> annotations = store.GetAnnotations();
			int initialSize = annotations.Count;
			Assert("Verify that store contains annotations", initialSize > 0);

			Annotation toDelete = annotations[0];
			Annotation deleted = store.DeleteAnnotation(toDelete.Id);

			Assert("Check that delete returns the same annotation instance.", toDelete == deleted);
			AssertEquals("Check that size decreased by one.", initialSize - 1, store.GetAnnotations().Count);

			passTest("Size decreased by one and return value verified.");
		}
	}
}

