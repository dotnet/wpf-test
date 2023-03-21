// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 

using System;
using System.Windows;
using Annotations.Test.Framework;
using System.Windows.Annotations;
using System.Xml;
using Annotations.Test.Reflection;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Avalon.Test.Annotations.Suites
{
    public class LocatorSuite : TestSuite
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
        public LocatorState1 handleState(LocatorState1 inState)
        {
            LocatorState1 returnState;
            if (inState == null)
                returnState = new LocatorState1(this);
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
        /// Return the first ContentLocatorBase from a Collection
        /// </summary>
        /// <param name="anno">Collection of Locators, of which we want the first.</param>
        public ContentLocatorBase GetFirstLocator(ICollection<ContentLocatorBase> locators)
        {
            ContentLocatorBase[] allLocs = new ContentLocatorBase[locators.Count];
            locators.CopyTo(allLocs, 0);
            return allLocs[0];
        }

        /// <summary>
        /// Verify that one Anchor has the correct set of Locators
        /// </summary>
        /// <param name="anno">Annotations containing some resources and locators.</param>
        /// <param name="locs">set of locators that represent the added locators to verify are in the Annotation.</param>
        protected void VerifyLocators(Annotation anno, ICollection<ContentLocatorBase> locs)
        {
            ContentLocatorBase[] verifyLocs = new ContentLocatorBase[locs.Count];
            locs.CopyTo(verifyLocs, 0);
            VerifyLocators(anno, verifyLocs);
        }

        /// <summary>
        /// Verify that one Anchor has the correct set of Locators
        /// </summary>
        /// <param name="anno">Annotations containing some resources and locators.</param>
        /// <param name="locs">set of locators that represent the added locators to verify are in the Annotation.</param>
        protected void VerifyLocators(Annotation anno, ContentLocatorBase[] locs)
        {
            // have we found a complete match
            bool matched = false;

            // try all the resources, match should be unique
            foreach (AnnotationResource res in anno.Anchors)
            {
                ICollection<ContentLocatorBase> annoLocs = res.ContentLocators;
                // If the two are the same length
                if (annoLocs.Count == locs.Length)
                {
                    // Did we find the match here
                    bool found = true;

                    // Match each of the locators
                    int i = 0;
                    IEnumerator<ContentLocatorBase> iter = annoLocs.GetEnumerator();
                    while (iter.MoveNext())
                    {
                        if (locs[i++] != iter.Current)
                        {
                            found = false;
                            break;
                        }
                    }
                    // if we found a match, make sure we only have one
                    if (found)
                        if (matched)
                            failTest("Duplicate AnnotationResource found in VerifyLocators.");
                        else
                            matched = true;
                }
            }
            Assert("Verify that LocatorPartSet exists.", matched);
            printStatus("Locators found as expected.");
        }

        #endregion Test Helper Methods

        #region Test Case Methods

        // Create a state to be passed around when test cases build on the results of other test cases.
        // Sets up an annotations and listeners for the Anchor changed events.
        public class LocatorState1
        {
            public Annotation anno;
            // ContentLocatorBase to use for cases where the same locator is added twice or added then deleted...
            public ContentLocatorBase basicLocator;
            public AnchorEventListener annoEventListener;

            public LocatorState1(TestSuite inSuite)
            {
                anno = AnnotationTestHelper.ResourceBlankAnnotation();
                basicLocator = AnnotationTestHelper.CreateLPS("Basic ContentLocatorBase", "NamespaceBasic", "90125");
                annoEventListener = new AnchorEventListener(inSuite, anno);
            }
        }

        /// <summary>
        /// -----------------------------------------------------------------------------------------------------
        ///| Operation	|	ContentLocatorBase	|	AnnotationResource	|	Results		|	Event				|	Status res1		|
        ///|------------|-----------|---------------|---------------|-----------------------|--------------------
        ///| Add		|	loc1	|	res1		|	true		|	Anno1->modify res1	|	loc1			|
        /// -----------------------------------------------------------------------------------------------------
        /// </summary>
        /// <param name="inState">State if this is not the final part, null if it is the final part.</param>
        [Priority(0)]
        public void case1_1(LocatorState1 inState)
        {
            bool final = (inState == null);

            LocatorState1 locState = handleState(inState);
            //			ContentLocatorBase loc = AnnotationTestHelper.CreateLPS("Captain Kirk", "Enterprise", "Spock me baby");
            foreach (AnnotationResource aRes in locState.anno.Anchors)
                aRes.ContentLocators.Add(locState.basicLocator);

            VerifyLocators(locState.anno, new ContentLocatorBase[] { locState.basicLocator });
            locState.annoEventListener.VerifyEventCount(0, 0, 1);
            if (final)
                passTest("Anchor added successfully.");
        }

        /// <summary>
        /// -----------------------------------------------------------------------------------------------------
        ///| Operation	|	ContentLocatorBase	|	AnnotationResource	|	Results		|	Event				|	Status res1		|
        ///|------------|-----------|---------------|---------------|-----------------------|--------------------
        ///| Add		|	loc1	|	res1		|	true		|	Anno1->modify res1	|	loc1			|
        /// -----------------------------------------------------------------------------------------------------
        ///| Add		|	loc1	|	res2		|	exception	|						|	loc1			|
        /// -----------------------------------------------------------------------------------------------------
        /// </summary>
        [Priority(0)]
        public void case1_3(LocatorState1 inState)
        {
            LocatorState1 locState = handleState(inState);

            bool final = (inState == null);

            case1_1(locState);

            AnnotationResource res2 = new AnnotationResource();
            locState.anno.Anchors.Add(res2);
            try
            {
                res2.ContentLocators.Add(locState.basicLocator);
            }
            catch (Exception e)
            {
                if (e is ArgumentException)
                {
                    printStatus("After exception caught:-");
                    VerifyLocators(locState.anno, new ContentLocatorBase[] { locState.basicLocator });
                    locState.annoEventListener.VerifyEventCount(1, 0, 1);
                    passTest("Expected exception occurred.");
                }
                failTest("Unexpected type of exception occurred: " + e.ToString());
            }
            failTest("Expected exception did not occur.");
        }

        /// <summary>
        /// -----------------------------------------------------------------------------------------------------
        ///| Operation	|	ContentLocatorBase	|	AnnotationResource	|	Results		|	Event				|	Status res1		|
        ///|------------|-----------|---------------|---------------|-----------------------|--------------------
        ///| Add		|	loc1	|	res1		|	true		|	Anno1->modify res1	|	loc1			|
        /// -----------------------------------------------------------------------------------------------------
        ///| Remove		|	loc1	|	res1		|	true		|	Anno1->modify res1	|					|
        /// -----------------------------------------------------------------------------------------------------
        /// </summary>
        [Priority(0)]
        public void case2_1(LocatorState1 inState)
        {
            LocatorState1 locState = handleState(inState);

            bool final = (inState == null);

            case1_1(locState);
            bool retSuccess = false;
            AnnotationResource aRes = GetFirstResource(locState.anno.Anchors);
            retSuccess = aRes.ContentLocators.Remove(locState.basicLocator);
            Assert("Removing a locator returns true.", retSuccess);// 1024595 
            VerifyLocators(locState.anno, new ContentLocatorBase[] { });
            locState.annoEventListener.VerifyEventCount(0, 0, 2);

            if (final)
                passTest("Anchor removed successfully.");
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
        public void case2_4(LocatorState1 inState)
        {
            LocatorState1 locState = handleState(inState);

            bool final = (inState == null);
            bool retSuccess = true;

            case2_1(locState);

            AnnotationResource aRes = GetFirstResource(locState.anno.Anchors);
            retSuccess = aRes.ContentLocators.Remove(locState.basicLocator);
            Assert("Removing a non-existant locator returns false.", !retSuccess);

            VerifyLocators(locState.anno, new ContentLocatorBase[] { });
            locState.annoEventListener.VerifyEventCount(0, 0, 2);
            passTest("Anchor removed successfully, then remove again returns false.");
        }

        /// <summary>
        /// -----------------------------------------------------------------------------------------------------
        ///| Operation	|	ContentLocatorBase	|	AnnotationResource	|	Results		|	Event				|	Status res1		|
        ///|------------|-----------|---------------|---------------|-----------------------|--------------------
        ///| Add		|	loc1..n	|	res1		|	true		|	Anno1->modify res1	|	loc1..n			|
        /// -----------------------------------------------------------------------------------------------------
        /// </summary>
        /// <param name="inState">State if this is not the final part, null if it is the final part.</param>
        [Priority(0)]
        public void case3_2(LocatorState1 inState)
        {
            bool final = (inState == null);

            LocatorState1 locState = handleState(inState);

            ContentLocatorBase loc;
            Collection<ContentLocatorBase> locCollect = new Collection<ContentLocatorBase>();
            for (int i = 0; i < 5; i++)
            {
                loc = AnnotationTestHelper.CreateLPS(i.ToString() + "Captain Kirk", "Enterprise" + i.ToString(), "Spock me baby");
                locCollect.Add(loc);
            }
            foreach (AnnotationResource aRes in locState.anno.Anchors)
                AnnotationTestHelper.AddLocators(aRes, locCollect);

            VerifyLocators(locState.anno, locCollect);
            locState.annoEventListener.VerifyEventCount(0, 0, 5);
            if (final)
                passTest("Anchor added successfully.");
        }

        /// <summary>
        /// ---------------------------------------------------------------------------------------------------------------------------------
        ///| Operation	|	ContentLocatorBase								|	AnnotationResource	|	Results		|	Event				|	Status res1		|
        ///|------------|---------------------------------------|---------------|---------------|-----------------------|--------------------
        ///| Add		|	loc1,null,loc2,null,null,loc3,null	|	res1		|	true		|	Anno1->modify res1	|	loc1,loc2,loc3	|
        /// ---------------------------------------------------------------------------------------------------------------------------------
        /// </summary>
        /// <param name="inState">State if this is not the final part, null if it is the final part.</param>
        [Priority(0)]
        public void case3_4(LocatorState1 inState)
        {
            bool final = (inState == null);

            LocatorState1 locState = handleState(inState);

            ContentLocatorBase loc;
            Collection<ContentLocatorBase> locCollect = new Collection<ContentLocatorBase>();
            Collection<ContentLocatorBase> verifyCollect = new Collection<ContentLocatorBase>();

            // set up the collection to add, including embedded nulls
            // set up a second collection without the nulls to verify the added locators, nulls do not get added
            loc = AnnotationTestHelper.CreateLPS("Captain Kirk", "Enterprise", "Spock me baby");
            locCollect.Add(loc); verifyCollect.Add(loc);
            locCollect.Add(null); verifyCollect.Add(null);
            loc = AnnotationTestHelper.CreateLPS("Boney", "Enterprise", "ouch");
            locCollect.Add(loc); verifyCollect.Add(loc);
            locCollect.Add(null); verifyCollect.Add(null);
            locCollect.Add(null); verifyCollect.Add(null);
            loc = AnnotationTestHelper.CreateLPS("Scotish git", "Enterprise", "Clunk");
            locCollect.Add(loc); verifyCollect.Add(loc);
            locCollect.Add(null); verifyCollect.Add(null);

            foreach (AnnotationResource aRes in locState.anno.Anchors)
                AnnotationTestHelper.AddLocators(aRes, locCollect);

            VerifyLocators(locState.anno, verifyCollect);
            locState.annoEventListener.VerifyEventCount(0, 0, 7);
            if (final)
                passTest("Anchor added successfully.");
        }

        /// <summary>
        /// ---------------------------------------------------------------------------------------------------------
        ///| Operation	|	ContentLocatorBase		|	AnnotationResource	|	Results		|	Event				|	Status res1		|
        ///|------------|---------------|---------------|---------------|-----------------------|--------------------
        ///| Add		|	loc1..n		|	res1		|	true		|	Anno1->modify res1	|	loc1..n			|
        ///|------------|---------------|---------------|---------------|-----------------------|--------------------
        ///| Add		|	loc3..n+1	|	res1		|	Exception	|	None				|	loc1..n			|
        /// ---------------------------------------------------------------------------------------------------------
        /// </summary>
        /// <param name="inState">State if this is not the final part, null if it is the final part.</param>
        [Priority(0)]
        public void case3_8(LocatorState1 inState)
        {
            bool final = (inState == null);

            LocatorState1 locState = handleState(inState);

            ContentLocatorBase loc;
            Collection<ContentLocatorBase> locCollect = new Collection<ContentLocatorBase>();
            for (int i = 0; i < 5; i++)
            {
                loc = AnnotationTestHelper.CreateLPS(i.ToString() + "Captain Kirk", "Enterprise" + i.ToString(), "Spock me baby");
                locCollect.Add(loc);
            }
            foreach (AnnotationResource aRes in locState.anno.Anchors)
                AnnotationTestHelper.AddLocators(aRes, locCollect);

            ContentLocatorBase[] verifyLocs = new ContentLocatorBase[locCollect.Count];
            locCollect.CopyTo(verifyLocs, 0);
            VerifyLocators(locState.anno, verifyLocs);
            locState.annoEventListener.VerifyEventCount(0, 0, 5);

            // alter some of the locators in the array then add them to a collection and do an Locators.Add
            verifyLocs[0] = AnnotationTestHelper.CreateLPS("Captain Krock", "Weanie", "And some other text");
            verifyLocs[3] = AnnotationTestHelper.CreateLPS("Yet another ContentLocatorBase bit", "Weanie", "42");
            Collection<ContentLocatorBase> newCollect = new Collection<ContentLocatorBase>();
            // make a new collection that is shorter and includes the new locators and two old ones
            for (int i = 0; i < 4; i++)
                newCollect.Add(verifyLocs[i]);
            AnnotationResource theRes = GetFirstResource(locState.anno.Anchors);
            try
            {
                AnnotationTestHelper.AddLocators(theRes, newCollect);
            }
            catch (Exception e)
            {
                printStatus("After exception caught:-");
                if (e is ArgumentException)
                {
                    VerifyLocators(locState.anno, locCollect);  // verify no locators changed
                    locState.annoEventListener.VerifyEventCount(0, 0, 5);  // and no events were fired
                    passTest("Expected exception occurred.");
                }
                failTest("Unexpected type of exception occurred: " + e.ToString());
            }
            failTest("Expected exception did not occur.");
        }

        #endregion Test Case Methods

        #endregion BVT TESTS

        #region PRIORITY TESTS

        #region Clone

        /// <summary>
		/// Parameters: ContentLocator with 1 ContentLocatorPart.  	
		/// Verify: Deep copy with 1 ContentLocatorPart.
		/// </summary>
        [Priority(1)]
        protected void locator_clone1()
		{
			VerifyClone(AnnotationTestHelper.CreateLocator(new string[] { "A" }));
			passTest("Verified clone with 1 ContentLocatorPart.");			
		}

		/// <summary>
		/// Parameters: ContentLocator with N ContentLocatorPart.  	
		/// Verify: Deep copy with N ContentLocatorPart.
		/// </summary>
        [Priority(1)]
        protected void locator_clone2()
		{
			VerifyClone(AnnotationTestHelper.CreateLocator(new string[] { "A", "B", "C", "D", "E" }));
			passTest("Verified clone with N ContentLocatorPart.");
		}

		/// <summary>
		/// Parameters: ContentLocator with empty ContentLocatorPart.  	
		/// Verify: Deep copy with empty ContentLocatorPart.
		/// </summary>
        [Priority(1)]
        protected void locator_clone3()
		{
			VerifyClone(AnnotationTestHelper.CreateLocator(new string[0]));
			passTest("Verified clone with empty ContentLocatorPart.");
		}

		private void VerifyClone(ContentLocator original)
		{
			ContentLocator clone = (ContentLocator)original.Clone();
			Assert("Verify clone is not the same object.", original != clone);
			Assert("Verify all elements are the same.", AnnotationTestHelper.LocatorPartListsEqual(original, clone));
		}

		#endregion

		#region DotProduct

		/// <summary>
		/// Parameters: Null	
		/// Verify: List containing ‘this’.
		/// </summary>
        [Priority(1)]
        protected void locator_dotproduct1()
		{
			ContentLocator loc = AnnotationTestHelper.CreateLocator(new string [] { "A" });
			IList<ContentLocatorBase> result = DoDotProduct(loc, null);
			AssertEquals("Verify result length.", 1, result.Count);
			Assert("Verify element is 'this'.", loc == result[0]);
			passTest("Verified DotProduct(null).");
		}

		/// <summary>
		/// Parameters: Empty list.
		/// Verify: List containing ‘this’.
		/// </summary>
        [Priority(1)]
        protected void locator_dotproduct2()
		{
			ContentLocator loc = AnnotationTestHelper.CreateLocator(new string[] { "A" });
			IList<ContentLocatorBase> result = DoDotProduct(loc, new List<ContentLocatorPart>());
			AssertEquals("Verify result length.", 1, result.Count);
			Assert("Verify element is 'this'.", loc == result[0]);
			passTest("Verified DotProduct(empty list).");
		}

		/// <summary>
		/// Parameters: N ContentLocatorParts	
		/// Verify: List containing N Locators
		/// </summary>
        [Priority(1)]
        protected void locator_dotproduct3()
		{
			int numLocatorParts = 10;
			IList<ContentLocatorPart> lpList = new List<ContentLocatorPart>();			
			for (int i=0; i < numLocatorParts; i++)
				lpList.Add(AnnotationTestHelper.CreateLocatorPart(i.ToString()));

			ContentLocator loc = AnnotationTestHelper.CreateLocator(new string[] { "A" });
			IList<ContentLocatorBase> result = DoDotProduct(loc, lpList);
			AssertEquals("Verify result length.", numLocatorParts, result.Count);
			for (int i = 0; i < numLocatorParts; i++)
			{
				ContentLocator current = (ContentLocator)result[i];
				ContentLocator expectedLocator = AnnotationTestHelper.CreateLocator(new string[] { "A", i.ToString() });
				Assert("Verify locator '" + i + "'.", AnnotationTestHelper.LocatorPartListsEqual(expectedLocator, current));
			}

			passTest("Verified DotProduct(List).");
		}

		[Priority(1)]private IList<ContentLocatorBase> DoDotProduct(ContentLocator locA, IList<ContentLocatorPart> lpList)
		{
			return (IList<ContentLocatorBase>)ReflectionHelper.InvokeMethod(locA, "DotProduct", new object[] { lpList });
		}

		#endregion

		#region GetSchema

        [Priority(1)]
        protected void locator_getschema1()
		{
			AssertNull("Verify null.", AnnotationTestHelper.CreateLocator(new string[] { "B" }).GetSchema());
			passTest("Schema is null.");
		}

		#endregion

		#region Merge

		/// <summary>
		/// Parameters: Other=Null	
		/// Verify: Return this.
		/// </summary>
        [Priority(1)]
        protected void locator_merge1()
		{
			ContentLocator loc = AnnotationTestHelper.CreateLocator(new string[] { "one" });
			ContentLocatorBase result = DoMerge(loc, null);
			Assert("Verify locator is 'this'.", loc == result);
			passTest("Verified Merge(null).");
		}

		/// <summary>
		/// Parameters: ContentLocator X.	
		/// Verify: X appended to each ContentLocator.
		/// </summary>
        [Priority(1)]
        protected void locator_merge2()
		{
			ContentLocator locA = AnnotationTestHelper.CreateLocator(new string[] { "A" });
			ContentLocator locB = AnnotationTestHelper.CreateLocator(new string[] { "B" });
			ContentLocatorBase result = DoMerge(locA, locB);
			Assert("Verify locator is 'this'.", locA == result);
			ContentLocator expectedLocators = AnnotationTestHelper.CreateLocator(new string[] { "A", "B" });
			Assert("Verify 'this' was changed.", AnnotationTestHelper.LocatorPartListsEqual(expectedLocators, locA));
			passTest("Verified Merge(ContentLocator).");
		}

		/// <summary>
		/// Parameters: Other=ContentLocatorGroup N locators	
		/// Verify: ContentLocatorGroup with N locators. 
		///			This.Parts contains ContentLocatorGroup[0].Parts.
		/// </summary>
        [Priority(1)]
        protected void locator_merge3()
		{
			ContentLocator locA = AnnotationTestHelper.CreateLocator(new string[] { "A" });
			ContentLocatorGroup group = AnnotationTestHelper.CreateLocatorGroup(10, 1, "B");
			ContentLocatorGroup result = (ContentLocatorGroup)DoMerge(locA, group);

			ContentLocator expectedLocators = AnnotationTestHelper.CreateLocator(new string[] { "A", "B" });
			AssertEquals("Verify number of locators.", 10, result.Locators.Count);
			foreach (ContentLocator loc in result.Locators)
				Assert("Verify equality.", AnnotationTestHelper.LocatorPartListsEqual(expectedLocators, loc));

			Assert("Verify 'this' was changed.", AnnotationTestHelper.LocatorPartListsEqual(expectedLocators, locA));
			passTest("Verified Merge(ContentLocatorGroup).");
		}

		private ContentLocatorBase DoMerge(ContentLocator locA, ContentLocatorBase locB)
		{
			return (ContentLocatorBase)ReflectionHelper.InvokeMethod(locA, "Merge", new object[] { locB });
		}

		#endregion
        
        #endregion PRIORITY TESTS
    }
}	

