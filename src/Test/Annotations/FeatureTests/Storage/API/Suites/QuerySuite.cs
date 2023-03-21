// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Tests AnnotationStore query api.


using System;
using System.Windows;
using System.Windows.Annotations.Storage;
using System.Windows.Annotations;
using Annotations.Test.Framework;

namespace Avalon.Test.Annotations.Suites
{
    [ExecutionGroupCompatible(false)]
    [TestDimension("-xmlstream")]
	public class QuerySuite_Pri1 : AnnotationStoreAPI
	{
		/// <summary>
		/// Add multiple annotations and check that GetAnnotation and GetAnnoations 
		/// return correct results.
		/// </summary>
        [Priority(1)]
        public void query2()
		{
			AnnotationStore store = CreateStore();
			Annotation anno1 = AnnotationStoreTestHelpers.MakeAnnotation1();
			Annotation anno2 = AnnotationStoreTestHelpers.MakeAnnotation2();
			Annotation anno3 = AnnotationStoreTestHelpers.MakeAnnotation1();

			store.AddAnnotation(anno1);
			store.AddAnnotation(anno2);
			store.AddAnnotation(anno3);

			Assert("GetAnnotation: verify first annotation.", anno1 == store.GetAnnotation(anno1.Id));
			Assert("GetAnnotations: verify first annotation", anno1 == store.GetAnnotations()[0]);

			Assert("GetAnnotation: verify second annotation.", anno2 == store.GetAnnotation(anno2.Id));
			Assert("GetAnnotations: verify seconf annotation", anno2 == store.GetAnnotations()[1]);

			store.DeleteAnnotation(anno2.Id);

			Assert("GetAnnotation: verify third annotation.", anno3 == store.GetAnnotation(anno3.Id));
			Assert("GetAnnotations: verify third annotation", anno3 == store.GetAnnotations()[1]);

			passTest("Verified get given multiple annotations and modifications.");
		}
		
		/// <summary>
		/// Autoflush off, add then modify anchor, author, and cargo.  Test that 
		/// get returns the modified annotation.
		/// </summary>
        [Priority(1)]
        public void query3()
		{
			AnnotationStore store = CreateStore();
			Annotation anno1 = AnnotationStoreTestHelpers.MakeAnnotation1();
			store.AddAnnotation(anno1);

			anno1.Authors.Add("query2Author");
			anno1.Anchors.Add(AnnotationStoreTestHelpers.createAnchor("query2", "AnchorStoreTests", 2));
			anno1.Cargos.Add(AnnotationStoreTestHelpers.createCargo("query2", "CargoStoreTests", "mycontent"));

			weakCompareAnnotations(anno1, store.GetAnnotation(anno1.Id));
			weakCompareAnnotations(anno1, store.GetAnnotations()[0]);

			passTest("Verified query preserves modified annotations.");
		}
	}
}

