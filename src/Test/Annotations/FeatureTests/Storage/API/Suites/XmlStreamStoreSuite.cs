// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: XmlStreamStore specific API tests.


using System;
using System.Threading; 
using System.Windows.Threading;
using System.Windows;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Xml;
using Proxies.System.Windows.Annotations;
using System.Windows.Annotations.Storage;
using System.Windows.Annotations;
using Annotations.Test.Framework;

namespace Avalon.Test.Annotations.Suites
{
    [ExecutionGroupCompatible(false)]
    [TestDimension("-xmlstream")]
	public class XmlStreamStoreSuite_Pri1 : AnnotationStoreAPI
	{
		/// <summary>
		/// Open a file read only, open an XmlStreamStore on this stream, perform some queries, modify some annotations. 
		/// Flush the store, catch the FileIO exception, make sure that query operations performed still return valid results.
		/// </summary>
        [Priority(1)]
        public void xmlstreamstore1()
		{
			SetupStoreWithContent();
            FileStream fs = new FileStream(XmlStreamStoreWrapper.StoreUri, FileMode.Open, FileAccess.Read);
			Assert("Make sure we don't have write permission", !fs.CanWrite);
            AnnotationStore store = XmlStreamStoreWrapper.Create(fs);

			Assert("Check that store initially contains annotations.", store.GetAnnotations().Count > 0);

			Annotation anno1 = AnnotationStoreTestHelpers.MakeAnnotation1();
			store.AddAnnotation(anno1);

			Assert("Check that query for known annotation is not null.", store.GetAnnotation(anno1.Id) != null);

			try
			{
				store.Flush();
				failTest("Shouldn't have had write permission to this file.");
			}
			catch (UnauthorizedAccessException)
			{
				printStatus("UnauthorizedAccessException thrown when trying to write to read only file.");
			}

			Assert("Check that query for known annotation is not null after failed flush.", store.GetAnnotation(anno1.Id) != null);

			passTest("XmlStreamStore verfied.");
		}

		/// <summary>
		/// XmlStreamStore is passed a stream that has only read permission. 
		/// Perform some operations on the annotations, but do not Flush. This 
		/// should be fine.
		/// </summary>
        [Priority(1)]
        public void xmlstreamstore2()
		{
            CreateEmptyStore();
            CloseStore();
            FileStream fs = new FileStream(XmlStreamStoreWrapper.StoreUri, FileMode.Open, FileAccess.Read);
			Assert("Make sure we don't have write permission", !fs.CanWrite);
            AnnotationStore store = XmlStreamStoreWrapper.Create(fs);

			Annotation anno1 = AnnotationStoreTestHelpers.MakeAnnotation1();
			Annotation anno2 = AnnotationStoreTestHelpers.MakeAnnotation2();

			store.AddAnnotation(anno1);
			Assert("Delete does not fail.", store.DeleteAnnotation(anno1.Id) != null);
			store.AddAnnotation(anno1);
			anno1.Authors.Add("xmlstream2Author");
			store.AddAnnotation(anno2);

			verifyStoreContents(store, new Annotation[] { anno1, anno2 });
			passTest("XmlStreamStore verfied.");
		}

		/// <summary>
		/// Test that we can dispose and recreate the store.
		/// </summary>
        [Priority(1)]
        public void xmlstreamstore3()
		{
            FileStream fs = new FileStream(XmlStreamStoreWrapper.StoreUri, FileMode.Create);
            AnnotationStore store = XmlStreamStoreWrapper.Create(fs);
			store.Dispose();
            store = XmlStreamStoreWrapper.Create(fs);

			passTest("Store disposed and recreated.");
		}
	}
}

