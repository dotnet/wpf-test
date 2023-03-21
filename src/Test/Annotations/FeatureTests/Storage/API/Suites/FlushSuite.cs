// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Tests of AnnotationStore flush api.


using System;
using System.Windows;
using System.Windows.Annotations.Storage;
using System.Windows.Annotations;
using System.Xml;
using Annotations.Test.Framework;

namespace Avalon.Test.Annotations.Suites
{
    [ExecutionGroupCompatible(false)]
    [TestDimension("-xmlstream")]
	public class FlushSuite_Pri1 : AnnotationStoreAPI
	{
		/// <summary>
		/// Add annotations, do not flush, close. Annotations not in the file.
		/// </summary>
		/// <param name="frmk"></param>
        [Priority(1)]
        public void flush1()
		{
			AnnotationStore store = CreateStore();
			Annotation annotation = new Annotation(new XmlQualifiedName("myAnno", "StoreFlushRevert1"));
			store.AddAnnotation(annotation);
            CloseStore();

            Assert("Verify store no content.", !StoreHasContent());
			passTest("Add then Revert did not persist to disk.");
		}

		/// <summary>
		/// Auto-flush on, add annotations and flush. Annotations should be in the file.
		/// </summary>
        [Priority(1)]
        public void flush4()
		{
			AnnotationStore store = CreateStore();
			Annotation annotation = new Annotation(new XmlQualifiedName("myAnno", "StoreFlushRevert4"));
			store.AutoFlush = true;
			store.AddAnnotation(annotation);
			CloseStore();

            verifyStoreContents(new Annotation[] { annotation });
            passTest("Annotation saved.");
		}

		/// <summary>
		/// Open store, close. Should be no changes to the file.
		/// </summary>
		/// <param name="frmk"></param>
        [Priority(1)]
        public void flush5()
		{
            AnnotationStore store = CreateStore();
            CloseStore();
            Assert("Verify no contents.", !StoreHasContent());
            passTest("No content written for autoflush = false.");
		}

		/// <summary>
		/// Open store, auto-flush on, close. Should be no changes to the file.
		/// </summary>
		/// <param name="frmk"></param>
        [Priority(1)]
        public void flush6()
		{
            AnnotationStore store = CreateStore();
            store.AutoFlush = true;
            CloseStore();
            Assert("Verify no contents.", !StoreHasContent());
            passTest("No content written for autoflush = false.");
		}

		/// <summary>
		/// Open store and add some annotations, auto-flush on then add some more annotations then close. 
		/// File should contain all of the added annotations.
		/// </summary>
		/// <param name="frmk"></param>
        [Priority(1)]
        public void flush7()
		{			
			AnnotationStore store = CreateStore();

			Annotation anno1 = AnnotationStoreTestHelpers.MakeAnnotation1();			
			Annotation anno2 = AnnotationStoreTestHelpers.MakeAnnotation2();

			store.AddAnnotation(anno1);
			store.AddAnnotation(anno2);

			// Enable autoflush after adding annotations.
			store.AutoFlush = true;
			CloseStore();

            verifyStoreContents(new Annotation[] { anno1, anno2 });
			passTest("Verified all annotations loaded from file.");
		}

		/// <summary>
		/// Open store and add some annotations, auto-flush on then add some more annotations then 
		/// close. File should contain all of the added annotations.
		/// </summary>
        [Priority(1)]
        public void flush8()
		{			
			AnnotationStore store = CreateStore();

			Annotation anno1 = AnnotationStoreTestHelpers.MakeAnnotation1();
			Annotation anno2 = AnnotationStoreTestHelpers.MakeAnnotation2();

			store.AddAnnotation(anno1);
			store.AddAnnotation(anno2);

			// Enable autoflush after adding annotations.
			store.AutoFlush = true;

			Annotation anno3 = AnnotationStoreTestHelpers.MakeAnnotation1();
			store.AddAnnotation(anno3);
            CloseStore();

            verifyStoreContents(new Annotation[] { anno1, anno2, anno3 });
			passTest("Verified all annotations loaded from file.");
		}

		/// <summary>
		/// Open store, flush. Valid file with no annotations.
		/// </summary>
        [Priority(1)]
        public void flush9()
		{			
			AnnotationStore store = CreateStore();
			store.Flush();
            Assert("Verify no content.", !StoreHasContent());
            passTest("No content flushed.");
		}

		/// <summary>
		/// Open store, auto-flush on. Valid file with no annotations.
		/// </summary>
        [Priority(1)]
        public void flush10()
		{			
			AnnotationStore store = CreateStore();
			store.AutoFlush = true;
            Assert("Verify no content.", !StoreHasContent());
            passTest("No content flushed.");
		}

		/// <summary>
		/// Open store, auto-flush on, flush. Valid file with no annotations.
		/// </summary>
        [Priority(1)]
        public void flush11()
		{			
			AnnotationStore store = CreateStore();
			store.AutoFlush = true;
			store.Flush();
            Assert("Verify no content.", !StoreHasContent());
            passTest("No content flushed.");
		}

		/// <summary>
		/// Open store, flush twice. Valid file with no annotations.
		/// </summary>
        [Priority(1)]
        public void flush12()
		{			
			AnnotationStore store = CreateStore();
			store.Flush();
			store.Flush();
            Assert("Verify no content.", !StoreHasContent());
            passTest("No content flushed.");
		}

		/// <summary>
		/// Create and add annotations, then flush twice, then close. Valid file should be left with annotations.
		/// </summary>
        [Priority(1)]
        public void flush13()
		{			
			AnnotationStore store = CreateStore();

			Annotation anno1 = new Annotation(new XmlQualifiedName("flush12", "StoreTests"));
			store.AddAnnotation(anno1);
			store.Flush();
			store.Flush();
            CloseStore();

            verifyStoreContents(new Annotation[] { anno1 });
            passTest("Annotation flushed.");
		}
	}
}

