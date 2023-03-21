// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 


using Annotations.Test.Framework; // TestSuite.

using System;
using System.Threading;
using System.Windows.Threading;
using System.Windows;

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
	public class ContentBVTs : TestSuite
    {
        #region BVT TESTS

        #region Constants

		public const string TEST_TYPENAME = "testType";
		public const string TEST_NAMESPACE = "testNamespace";

        #endregion Constants
       
        #region Test Helper Methods

		/// <summary>
		/// Make a new state if required
		/// </summary>
		/// <param name="anno">State that was passed in to a method.</param>
		public ContentState1 handleState(ContentState1 inState)
		{
			ContentState1 returnState;
			if (inState == null)
				returnState = new ContentState1(this);
			else
				returnState = inState;
			return returnState;
		}

		/// <summary>
		/// Return the first AnnotationResource from a Collection
		/// </summary>
		/// <param name="anno">Collection of Resources, of which we want the first.</param>
		public AnnotationResource GetFirstResource(ICollection<AnnotationResource> resources)
		{
			AnnotationResource[] allRes = new AnnotationResource[resources.Count];
			resources.CopyTo(allRes, 0);
			return allRes[0];
		}

		/// <summary>
		/// Return the first Content from a Collection
		/// </summary>
		/// <param name="anno">Collection of Contents, of which we want the first.</param>
		public object GetFirstContent(ICollection contents)
		{
			object[] allConts = new object[contents.Count];
			contents.CopyTo(allConts, 0);
			return allConts[0];
		}

		/// <summary>
		/// Verify that one Anchor has the correct set of Locators
		/// </summary>
		/// <param name="anno">Annotations containing some resources and locators.</param>
		/// <param name="locs">set of locators that represent the added locators to verify are in the Annotation.</param>
		protected void VerifyContents(Annotation anno, ICollection conts)
		{
			object[] verifyConts = new object[conts.Count];
			conts.CopyTo(verifyConts, 0);
			VerifyContents(anno, verifyConts);
		}

		/// <summary>
		/// Verify that one Anchor has the correct set of Locators
		/// </summary>
		/// <param name="anno">Annotations containing some resources and locators.</param>
		/// <param name="locs">set of locators that represent the added locators to verify are in the Annotation.</param>
		protected void VerifyContents(Annotation anno, object[] conts)
		{
			// have we found a complete match
			bool matched = false;

			// try all the resources, match should be unique
			foreach (AnnotationResource res in anno.Anchors)
			{
				ICollection annoConts = res.Contents;
				// If the two are the same length
				if (annoConts.Count == conts.Length)
				{
					// Did we find the match here
					bool found = true;

					// Match each of the locators
					int i = 0;
					IEnumerator iter = annoConts.GetEnumerator();
					while (iter.MoveNext())
					{
						if (conts[i++] != iter.Current)
						{
							found = false;
							break;
						}
					}
					// if we found a match, make sure we only have one
					if (found)
						if (matched)
							failTest("Duplicate AnnotationResource found in VerifyContents.");
						else
							matched = true;
				}
			}
			Assert("Verify that Content exists.", matched);
			printStatus("Contents found as expected.");
		}

        #endregion Test Helper Methods

        #region Test Case Methods

		// Create a state to be passed around when test cases build on the results of other test cases.
		// Sets up an annotations and listeners for the Anchor changed events.
		public class ContentState1
		{
			public Annotation anno;
			// ContentLocatorBase to use for cases where the same locator is added twice or added then deleted...
			public ContentLocatorBase basicLocator;
			public XmlElement basicContent;
			public AnchorEventListener annoEventListener;

			public ContentState1(TestSuite inSuite)
			{
				anno = AnnotationTestHelper.ResourceBlankAnnotation();
				basicLocator = AnnotationTestHelper.CreateLPS("Basic ContentLocatorBase", "NamespaceBasic", "90125");
				XmlDocument doc = new XmlDocument();
				XmlElement el = doc.CreateElement("Annotation_Element", "Annotation_test_namespace");
				el.InnerXml = "This is a pretend content to test that we can have a content in the various places that we should have a content.";
				basicContent = el;
				annoEventListener = new AnchorEventListener(inSuite, anno);
			}
		}

		/// <summary>
		/// -----------------------------------------------------------------------------------------------------
		///| Operation	|	Content	|	AnnotationResource	|	Results		|	Event				|	Status res1		|
		///|------------|-----------|---------------|---------------|-----------------------|--------------------
		///| Add		|	cont1	|	res1		|	true		|	Anno1->modify cont1	|	cont1			|
		/// -----------------------------------------------------------------------------------------------------
		/// </summary>
		/// <param name="inState">State if this is not the final part, null if it is the final part.</param>
        [Priority(0)]
        public void case1_1(ContentState1 inState)
		{
			bool final = (inState == null);

			ContentState1 locState = handleState(inState);
//			ContentLocatorBase loc = AnnotationTestHelper.CreateLPS("Captain Kirk", "Enterprise", "Spock me baby");
			foreach (AnnotationResource aRes in locState.anno.Anchors)
				aRes.Contents.Add(locState.basicContent);
			

			VerifyContents(locState.anno, new object[] { locState.basicContent });
			locState.annoEventListener.VerifyEventCount(0, 0, 1);
			if (final)
				passTest("Content added successfully.");
		}

		/// <summary>
		/// -----------------------------------------------------------------------------------------------------
		///| Operation	|	Content	|	AnnotationResource	|	Results		|	Event				|	Status res1		|
		///|------------|-----------|---------------|---------------|-----------------------|--------------------
		///| Add		|	cont1	|	res1		|	true		|	Anno1->modify res1	|	cont1			|
		/// -----------------------------------------------------------------------------------------------------
		///| Add		|	cont1	|	res2		|	exception	|						|	cont1			|
		/// -----------------------------------------------------------------------------------------------------
		/// </summary>
        [Priority(0)]
        public void case1_3(ContentState1 inState)
		{
			ContentState1 locState = handleState(inState);

			bool final = (inState == null);

			case1_1(locState);

            AnnotationResource aRes = GetFirstResource(locState.anno.Anchors);
			try
			{
				aRes.Contents.Add(locState.basicContent);
			}
			catch (Exception e)
			{
				if (e is ArgumentException)
				{
					printStatus("After exception caught:-");
					VerifyContents(locState.anno, new object[] { locState.basicContent });
					locState.annoEventListener.VerifyEventCount(0, 0, 1);
					passTest("Expected exception occurred.");
				}
				failTest("Unexpected type of exception occurred: " + e.ToString());
			}
			failTest("Expected exception did not occur.");
		}

		/// <summary>
		/// -----------------------------------------------------------------------------------------------------
		///| Operation	|	Content	|	AnnotationResource	|	Results		|	Event				|	Status res1		|
		///|------------|-----------|---------------|---------------|-----------------------|--------------------
		///| Add		|	cont1	|	res1		|	true		|	Anno1->modify res1	|	loc1			|
		/// -----------------------------------------------------------------------------------------------------
		///| Remove		|	cont1	|	res1		|	true		|	Anno1->modify res1	|					|
		/// -----------------------------------------------------------------------------------------------------
		/// </summary>
        [Priority(0)]
        public void case2_1(ContentState1 inState)
		{
			ContentState1 locState = handleState(inState);

			bool final = (inState == null);

			case1_1(locState);
			bool retSuccess = false;
			AnnotationResource aRes = GetFirstResource(locState.anno.Anchors);
			retSuccess = aRes.Contents.Remove(locState.basicContent);
			Assert("Removing a locator returns true.", retSuccess);
			VerifyContents(locState.anno, new object[] {});
			locState.annoEventListener.VerifyEventCount(0, 0, 2);

			if (final)
				passTest("Content removed successfully.");
		}

		/// <summary>
		/// -----------------------------------------------------------------------------------------------------
		///| Operation	|	ContentLocatorBase	|	AnnotationResource	|	Results		|	Event				|	Status res1		|
		///|------------|-----------|---------------|---------------|-----------------------|--------------------
		///| Add		|	loc1	|	res1		|	true		|	Anno1->modify res1	|	loc1			|
		/// -----------------------------------------------------------------------------------------------------
		///| Remove		|	loc1	|	res1		|	true		|	Anno1->modify res1	|					|
		/// -----------------------------------------------------------------------------------------------------
		///| Remove		|	loc1	|	res1		|	false		|	Anno1->modify res1	|					|
		/// -----------------------------------------------------------------------------------------------------
		/// </summary>
        [Priority(0)]
        public void case2_4(ContentState1 inState)
		{
			ContentState1 locState = handleState(inState);

			bool final = (inState == null);
			bool retSuccess = true;

			case2_1(locState);

			AnnotationResource aRes = GetFirstResource(locState.anno.Anchors);
			retSuccess = aRes.Contents.Remove(locState.basicContent);
			Assert("Removing a non-existant locator returns false.", !retSuccess);

			VerifyContents(locState.anno, new object[] { });
			locState.annoEventListener.VerifyEventCount(0, 0, 2);
			passTest("Content removed successfully, then remove again returns false.");
		}

        #endregion Test Case Methods

        #endregion BVT TESTS

        #region PRIORITY TESTS
        #endregion PRIORITY TESTS
    }
}


