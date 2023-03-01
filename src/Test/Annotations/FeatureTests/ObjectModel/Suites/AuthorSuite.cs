// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 


using Annotations.Test.Framework; // TestSuite.

using System;
using System.Threading; using System.Windows.Threading;
using System.Windows;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

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

using Proxies.System.Windows.Annotations;
using System.Windows.Annotations.Storage;



namespace Avalon.Test.Annotations.Suites
{
    [ExecutionGroupCompatible(false)]
	public class AuthorBVTs : TestSuite
	{
        #region BVT TESTS

        #region Constants

        public const string TEST_TYPENAME = "testType";
		public const string TEST_NAMESPACE = "testNamespace";

        #endregion Constants

        #region Test Helper Methods

        protected void VerifyAuthors(Annotation anno, object[] authors)
		{
			AssertEquals("Verify number of authors.", authors.Length, anno.Authors.Count);
			
			// Annotation.Authors returns an ICollection, which is unordered.
			// So don't assume order in search.
			for (int i = 0; i < authors.Length; i++)
			{
				bool found = false;
				IEnumerator iter = anno.Authors.GetEnumerator();
				while (iter.MoveNext())
				{
					if (authors[i] == iter.Current)
					{
						found = true;
						break;
					}
				}
				Assert("Verify that author '" + authors[i].ToString() + "' exists.", found);
			}			
			printStatus("Annotation.Authors are same as expected.");
        }

        #endregion Test Helper Methods

        #region Test Methods

        #region basic tests

        /// <summary>
		/// Operation	Result	Author's State
		/// Add author1		True	author1
		/// </summary>
        [Priority(0)]
        public void case1_1() 
		{
			Annotation anno = AnnotationTestHelper.UnauthoredAnnotation();
			string author1 = "Captain Kirk";
			anno.Authors.Add(author1);

			VerifyAuthors(anno, new object [] { author1 });
			passTest("Author added successfully.");
		}

		/// <summary>
		/// Operation	Result	Author's State
		/// Add author1		True
		/// Remove author1	True
		/// </summary>
        [Priority(0)]
        public void case1_2() 
		{
			Annotation anno = AnnotationTestHelper.UnauthoredAnnotation();
			string author1 = "Captain Kirk";
			anno.Authors.Add(author1);
			VerifyAuthors(anno, new object[] { author1 });

			anno.Authors.Remove(author1);
			VerifyAuthors(anno, new object[] { });

			passTest("Author added and removed successfully.");
		}

		/// <summary>
		/// Operation	Result	Author's State
		/// Add author1		true	author1
		/// Remove author2	false	a
		/// </summary>
        [Priority(0)]
        public void case1_4() 
		{
			Annotation anno = AnnotationTestHelper.UnauthoredAnnotation();
			string author1 = "Captain Kirk";
			string author2 = "Spock";

			anno.Authors.Add(author1);
			AssertEquals("Remove non-existant author.", false, anno.Authors.Remove(author2));
			VerifyAuthors(anno, new object[] { author1 });

			passTest("Remove non-existant author failed.");
		}

		/// <summary>
		/// Operation	Result	Author's State
		/// Remove author1	false
		/// Add author2		true	author2
		/// </summary>
        [Priority(0)]
        public void case1_7() 
		{
			Annotation anno = AnnotationTestHelper.UnauthoredAnnotation();
			string author1 = "Captain Kirk";
			string author2 = "Spock";
			
			AssertEquals("Remove non-existant author.", false, anno.Authors.Remove(author1));
			anno.Authors.Add(author2);
			VerifyAuthors(anno, new object[] { author2 });

			passTest("Remove non-existant author then add succeeded.");
		}

		/// <summary>
		/// Operation	Result	Author's State
		/// Add author1		true
		/// Add author1		no exception
		/// </summary>
        [Priority(0)]
        public void case1_8() 
		{
			Annotation anno = AnnotationTestHelper.UnauthoredAnnotation();
			string author1 = "Captain Kirk";

			anno.Authors.Add(author1);
			anno.Authors.Add(author1);			
			passTest("No exception occurred for duplicate authors.");
		}

		#endregion basic tests

		#region state 1 tests

		/// <summary>
		/// 1 Document with 3 authors author1, author2, author3.
		/// Two annotations anno1 and anno2.
		///   anno1 -> author1
		///   anno2 -> author2.
		/// </summary>
		class State1Context
		{
			public XmlDocument doc;

			public Annotation anno1;
			public Annotation anno2;
			public string author1;
			public string author2;
			public string author3;

			public AuthorEventListener anno1EventListener;
			public AuthorEventListener anno2EventListener;

			public State1Context(TestSuite suite)
			{
				doc = new XmlDocument();

				anno1 = AnnotationTestHelper.UnauthoredAnnotation();
				anno2 = AnnotationTestHelper.UnauthoredAnnotation();

				author1 = "Ben Kenobi";
				author2 = "Leia Organa";
				author3 = "Anakin Skywalker";

				anno1.Authors.Add(author1);
				anno2.Authors.Add(author2);

				anno1EventListener = new AuthorEventListener(suite, anno1);
				anno2EventListener = new AuthorEventListener(suite, anno2);
			}
		}

		/// <summary>
		/// ----------------------------------------------------------------------------------------
		///| Operation	|	Author	|	Result Anno1	|	Results Anno2	|	Event				|
		///|------------|-----------|-------------------|-------------------|-----------------------|
		///| Change		|	author1	|	author1			|	author2			|	Anno1->Delete/Add author1	|
		/// ----------------------------------------------------------------------------------------
		/// </summary>
        [Priority(0)]
        public void case2_1() 
		{
			State1Context context = new State1Context(this);

			context.anno1.Authors[0] = "Obiwan Kenobi";

			context.anno1EventListener.VerifyEventCount(1, 1, 0);			
			context.anno2EventListener.VerifyEventCount(0, 0, 0);

			passTest("Author changed events ok.");
		}

		/// <summary>
		/// --------------------------------------------------------------------------------------------
		///| Operation		|	Author	|	Result Anno1	|	Results Anno2	|	Event				|
		///|----------------|-----------|-------------------|-------------------|-----------------------|
		///| 1-Add to Anno1	|	author3	|	author1,author3	|	author2			|	Anno1->add			|
		///| 2-Change		|	author3	|	author1,author3 |	author2			|	Anno1->del/add  	|
		/// --------------------------------------------------------------------------------------------
		/// </summary>
        [Priority(0)]
        public void case2_7() 
		{
			State1Context context = new State1Context(this);
			context.anno1.Authors.Add(context.author3);
			
			context.author3 = "Darth Vader";
			context.anno1.Authors[1] = context.author3;

			VerifyAuthors(context.anno1, new object[] { context.author1, context.author3 });
			VerifyAuthors(context.anno2, new object[] { context.author2 });

			context.anno1EventListener.VerifyEventCount(2, 1, 0);
			context.anno2EventListener.VerifyEventCount(0, 0, 0);

			passTest("Author changed events ok.");
		}

		#endregion state 1 tests

		#region state 2 tests

		/// <summary>
		/// 2 documents docA & docB with 3 authors each:
		///    A -> author1, author2, author3
		///    B -> b1, b2, b3
		/// Two annotations anno1 and anno2.
		/// </summary>
		class State2Context
		{
			public XmlDocument docA;
			public XmlDocument docB;

			public Annotation anno1;
			public Annotation anno2;

			public string authorA1;
			public string authorA2;
			//public string authorA3;

			public string authorB1;
			public string authorB2;
			//public string authorB3;

			public AuthorEventListener anno1EventListener;
			public AuthorEventListener anno2EventListener;

			public State2Context(TestSuite suite)
			{
				docA = new XmlDocument();
				docB = new XmlDocument();

				anno1 = AnnotationTestHelper.UnauthoredAnnotation();
				anno2 = AnnotationTestHelper.UnauthoredAnnotation();

				authorA1 = "Ben Kenobi";
				authorA2 = "Leia Organa";
				//authorA3 = "Anakin Skywalker";

				authorB1 = "Han Solo";
				authorB2 = "C3PO";
				//authorB3 = "Chancellor Palpatine";

				anno1EventListener = new AuthorEventListener(suite, anno1);
				anno2EventListener = new AuthorEventListener(suite, anno2);
			}
		}

		/// <summary>
		/// ----------------------------------------------------------------------------------------------------------------
		///| Operation		|	Annotation	|	Author		|	Result Anno1	|	Results Anno2	|	Event				|
		///|----------------|---------------|---------------|-------------------|-------------------|-----------------------|
		///| 3.1-Add		|	anno1		|	authorA1	|	authorA1		|					|	Anno1->add			|
		///| 3.2-Add		|	anno2		|	authorB2	|	authorA1		|	authorB2		|	Anno2->add			|
		///| 3.3-Add		|	anno2		|	authorA2	|	authorA1		|	authB2,authA2	|	Anno2->add authA2	|
		/// ----------------------------------------------------------------------------------------------------------------
		/// </summary>
        [Priority(0)]
        public void case3_3() 
		{
			State2Context context = new State2Context(this);
			context.anno1.Authors.Add(context.authorA1);
			context.anno2.Authors.Add(context.authorB2);
			context.anno2.Authors.Add(context.authorA2);

			VerifyAuthors(context.anno1, new object[] { context.authorA1 });
			VerifyAuthors(context.anno2, new object[] { context.authorB2, context.authorA2 });

			context.anno1EventListener.VerifyEventCount(1, 0, 0);
			context.anno2EventListener.VerifyEventCount(2, 0, 0);
			context.anno2EventListener.VerifyLastChangedAuthor(context.authorA2);

			passTest("Author changed events ok.");
		}

		/// <summary>
		/// ----------------------------------------------------------------------------------------------------------------
		///| Operation		|	Annotation	|	Author		|	Result Anno1	|	Results Anno2	|	Event				|
		///|----------------|---------------|---------------|-------------------|-------------------|-----------------------|
		///| 3.1-Add		|	anno1		|	authorA1	|	authorA1		|					|	Anno1->add			|
		///| 3.2-Add		|	anno2		|	authorB2	|	authorA1		|	authorB2		|	Anno2->add			|
		///| 3.3-Add		|	anno2		|	authorA2	|	authorA1		|	authorB2,authA2	|	Anno2->add			|
		///| 3.4-Add		|	anno1		|	authorB2	|	authA1,authB2	|	authorB2,authA2	|	Anno1->add			|
		///| 3.5-Del		|	anno1		|	authorB1	|	authA1,authB2	|	authorB2,authA2	|	none				|
		/// ----------------------------------------------------------------------------------------------------------------
		/// </summary>
        [Priority(0)]
        public void case3_5() 
		{
			State2Context context = new State2Context(this);
			context.anno1.Authors.Add(context.authorA1);
			context.anno2.Authors.Add(context.authorB2);
			context.anno2.Authors.Add(context.authorA2);
			context.anno1.Authors.Add(context.authorB2);
			context.anno1.Authors.Remove(context.authorB1); // non-existent author.

			VerifyAuthors(context.anno1, new object[] { context.authorA1, context.authorB2 });
			VerifyAuthors(context.anno2, new object[] { context.authorB2, context.authorA2 });

			context.anno1EventListener.VerifyEventCount(2, 0, 0);
			context.anno2EventListener.VerifyEventCount(2, 0, 0);

			passTest("Author changed events ok.");
		}

		/// <summary>
		/// ----------------------------------------------------------------------------------------------------------------
		///| Operation		|	Annotation	|	Author		|	Result Anno1	|	Results Anno2	|	Event				|
		///|----------------|---------------|---------------|-------------------|-------------------|-----------------------|
		///| 3.1-Add		|	anno1		|	authorA1	|	authorA1		|					|	Anno1->add			|
		///| 3.2-Add		|	anno2		|	authorB2	|	authorA1		|	authorB2		|	Anno2->add			|
		///| 3.3-Add		|	anno2		|	authorA2	|	authorA1		|	authorB2,authA2	|	Anno2->add			|
		///| 3.4-Add		|	anno1		|	authorB2	|	authA1,authB2	|	authorB2,authA2	|	Anno1->add			|
		///| 3.5-Del		|	anno1		|	authorB1	|	authA1,authB2	|	authorB2,authA2	|	none				|
		///| 3.6-change		|				|	authorA1	|	authA1,authB2	|	authorB2,authA2	|	Anno1->mod			| 
		///| 3.7-change		|				|	authorB2	|	authA1,authB2	|	authorB2,authA2	|	Anno1/2->mod		| 
		///| 3.8-Del		|	anno2		|	authorB2	|	authA1,authB2	|	authA2			|	Anno2->del			|
		///| 3.9-Del		|	anno2		|	authorB2	|	authA1,authB2	|	authA2			|	none				|
		/// ----------------------------------------------------------------------------------------------------------------
		/// </summary>
        [Priority(0)]
        public void case3_9() 
		{
			State2Context context = new State2Context(this);
			context.anno1.Authors.Add(context.authorA1);
			context.anno2.Authors.Add(context.authorB2);
			context.anno2.Authors.Add(context.authorA2);
			context.anno1.Authors.Add(context.authorB2);
			context.anno1.Authors.Remove(context.authorB1); // non-existent author.

			context.anno2.Authors.Remove(context.authorB2);
			context.anno2.Authors.Remove(context.authorB2);

			VerifyAuthors(context.anno1, new object[] { context.authorA1, context.authorB2 });
			VerifyAuthors(context.anno2, new object[] { context.authorA2 });

			context.anno1EventListener.VerifyEventCount(2, 0, 0);
			context.anno2EventListener.VerifyEventCount(2, 1, 0);

			passTest("Author changed events ok.");
		}

		#endregion state 2 tests

        #endregion Test Methods

        #endregion BVT TESTS

        #region PRIORITY TESTS
        #endregion PRIORITY TESTS
    }
}


