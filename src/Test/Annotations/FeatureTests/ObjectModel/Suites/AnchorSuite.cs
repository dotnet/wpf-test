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
	public class AnchorBVTs : TestSuite
    {

        #region BVT TESTS

        #region Constants

        public const string TEST_TYPENAME = "testType";
        public const string TEST_NAMESPACE = "testNamespace";

        #endregion Constants

        #region Test Helper Methods

        protected void VerifyAnchors(Annotation anno, AnnotationResource[] anchors)
        {
            AssertEquals("Verify number of anchors.", anchors.Length, anno.Anchors.Count);

            // Annotation.Anchors returns an ICollection, which is unordered.
            // So don't assume order in search.
            for (int i = 0; i < anchors.Length; i++)
            {
                bool found = false;
                IEnumerator<AnnotationResource> iter = anno.Anchors.GetEnumerator();
                while (iter.MoveNext())
                {
                    if (anchors[i] == iter.Current)
                    {
                        found = true;
                        break;
                    }
                }
                Assert("Verify that anchor '" + anchors[i].ToString() + "' exists.", found);
            }
            printStatus("Annotation.Anchors are same as expected.");
        }

        #endregion Test Helper Methods

        #region TEST METHODS

        /// <summary>
        /// Operation	  Result	Author's State
        /// Add anchor1		True	anchor1
        /// </summary>
        [Priority(0)]
        public void case1_1()
        {
            Annotation anno = AnnotationTestHelper.UnanchoredAnnotation();
            AnchorEventListener annoEventListener = new AnchorEventListener(this, anno);
            AnnotationResource anchor1 = AnnotationTestHelper.CreateAnchor("Captain Kirk", "Enterprise");
            anno.Anchors.Add(anchor1);

            VerifyAnchors(anno, new AnnotationResource[] { anchor1 });
            annoEventListener.VerifyEventCount(1, 0, 0);
            passTest("Anchor added successfully.");
        }

        /// <summary>
        /// Operation	Result	Author's State
        /// Add anchor1		True	anchor1
        /// Remove anchor1	True
        /// </summary>
        [Priority(0)]
        public void case1_2()
        {
            Annotation anno = AnnotationTestHelper.UnanchoredAnnotation();
            AnchorEventListener annoEventListener = new AnchorEventListener(this, anno);
            AnnotationResource anchor1 = AnnotationTestHelper.CreateAnchor("Uhluru", "Land of Oz");
            anno.Anchors.Add(anchor1);
            VerifyAnchors(anno, new AnnotationResource[] { anchor1 });
            annoEventListener.VerifyEventCount(1, 0, 0);

            anno.Anchors.Remove(anchor1);
            VerifyAnchors(anno, new AnnotationResource[] { });
            annoEventListener.VerifyEventCount(1, 1, 0);

            passTest("Author added and removed successfully.");
        }

        /// <summary>
        /// Operation	Result	Anchor's State
        /// Add anchor1		true	anchor1
        /// Remove anchor2	false	anchor1
        /// </summary>
        [Priority(0)]
        public void case1_4()
        {
            Annotation anno = AnnotationTestHelper.UnanchoredAnnotation();
            AnchorEventListener annoEventListener = new AnchorEventListener(this, anno);
            AnnotationResource anchor1 = AnnotationTestHelper.CreateAnchor("Bones", "Sick bay");
            AnnotationResource anchor2 = AnnotationTestHelper.CreateAnchor("Spock", "Volcano");

            anno.Anchors.Add(anchor1);
            AssertEquals("Remove non-existant anchor.", false, anno.Anchors.Remove(anchor2));
            VerifyAnchors(anno, new AnnotationResource[] { anchor1 });
            annoEventListener.VerifyEventCount(1, 0, 0);

            passTest("Remove non-existant anchor failed.");
        }

        /// <summary>
        /// Operation	Result	Anchor's State
        /// Remove anchor1	false
        /// Add anchor2		true	anchor2
        /// </summary>
        [Priority(0)]
        public void case1_7()
        {
            Annotation anno = AnnotationTestHelper.UnanchoredAnnotation();
            AnchorEventListener annoEventListener = new AnchorEventListener(this, anno);
            AnnotationResource anchor1 = AnnotationTestHelper.CreateAnchor("Tribble", "Grain Store");
            AnnotationResource anchor2 = AnnotationTestHelper.CreateAnchor("Spock", "Pointy Eared People");

            AssertEquals("Remove non-existant anchor.", false, anno.Anchors.Remove(anchor1));
            anno.Anchors.Add(anchor2);
            VerifyAnchors(anno, new AnnotationResource[] { anchor2 });
            annoEventListener.VerifyEventCount(1, 0, 0);

            passTest("Remove non-existant anchor then add succeeded.");
        }

        /// <summary>
        /// Operation	Result		Anchor's State
        /// Add anchor1		true		anchor1
        /// Add anchor1		Exception	anchor1
        /// </summary>
        [Priority(0)]
        public void case1_8()
        {
            Annotation anno = AnnotationTestHelper.UnanchoredAnnotation();
            AnchorEventListener annoEventListener = new AnchorEventListener(this, anno);
            AnnotationResource anchor1 = AnnotationTestHelper.CreateAnchor("Captain Cook", "Endeavour");

            anno.Anchors.Add(anchor1);
            try
            {
                anno.Anchors.Add(anchor1);
            }
            catch (Exception e)
            {
                if (e is ArgumentException)
                {
                    VerifyAnchors(anno, new AnnotationResource[] { anchor1 });
                    annoEventListener.VerifyEventCount(1, 0, 0);
                    passTest("Expected exception occurred.");
                }
                failTest("Unexpected type of exception occurred: " + e.ToString());
            }
            failTest("Expected exception did not occur.");
        }

        /// <summary>
        /// Two anchors a1, a2 and a3.
        /// Two annotations anno1 and anno2.
        /// </summary>
        public class State2Context
        {
            public Annotation anno1;
            public Annotation anno2;
            public AnnotationResource anchor1;
            public AnnotationResource anchor2;
            public AnnotationResource anchor3;

            public AnchorEventListener anno1EventListener;
            public AnchorEventListener anno2EventListener;

            public State2Context(TestSuite suite)
            {
                anno1 = AnnotationTestHelper.UnanchoredAnnotation();
                anno2 = AnnotationTestHelper.UnanchoredAnnotation();

                anchor1 = AnnotationTestHelper.CreateAnchor("Fred", "Space One");
                anchor2 = AnnotationTestHelper.CreateAnchor("Bill", "Other space", "12");
                anchor3 = AnnotationTestHelper.CreateAnchor("john", "Other space");

                anno1EventListener = new AnchorEventListener(suite, anno1);
                anno2EventListener = new AnchorEventListener(suite, anno2);
            }
        }

        /// <summary>
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Operation	|	Anchor	|	Annotation	|	Results		|	Event				|	Status Anno1	|	Status Anno2	|
        ///|------------|-----------|---------------|---------------|-----------------------|----------------------------------------
        ///| Add		|	anchor1	|	anno1		|	true		|	Anno1->add anchor1	|	anchor1			|					|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Add		|	anchor1	|	anno2		|	Exception	|	Anno1->add anchor1	|	anchor1			|					|
        /// -------------------------------------------------------------------------------------------------------------------------
        /// </summary>
        /// <param name="inState">State if this is not the final part, null if it is the final part.</param>
        [Priority(0)]
        public void case2_2(State2Context inState)
        {
            State2Context context;
            bool exceptThrown = false;
            // is this the final test on the call stack
            bool final = (inState == null);

            if (final)
                context = new State2Context(this);
            else
                context = inState;

            context.anno1.Anchors.Add(context.anchor1);
            try
            {
                context.anno2.Anchors.Add(context.anchor1);
            }
            catch (Exception e)
            {
                exceptThrown = true;
                if (e is ArgumentException)
                {
                    VerifyAnchors(context.anno1, new AnnotationResource[] { context.anchor1 });
                    context.anno1EventListener.VerifyEventCount(1, 0, 0);
                    context.anno2EventListener.VerifyEventCount(0, 0, 0);
                    if (final)
                        passTest("Expected exception occurred.");
                }
                else
                    failTest("Unexpected type of exception occurred: " + e.ToString());
            }
            if (!exceptThrown)
                failTest("Expected exception did not occur.");
        }

        /// <summary>
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Operation	|	Anchor	|	Annotation	|	Results		|	Event				|	Status Anno1	|	Status Anno2	|
        ///|------------|-----------|---------------|---------------|-----------------------|----------------------------------------
        ///| Add		|	anchor1	|	anno1		|	true		|	Anno1->add anchor1	|	anchor1			|					|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Add		|	anchor1	|	anno2		|	Exception	|						|	anchor1			|					|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Remove		|	anchor2	|	anno1		|	False		|						|	anchor1			|					|
        /// -------------------------------------------------------------------------------------------------------------------------
        /// </summary>
        /// <param name="inState">State if this is not the final part, null if it is the final part.</param>
        [Priority(0)]
        public void case2_6update(State2Context inState)
        {
            State2Context context;
            bool final = (inState == null);

            if (final)
                context = new State2Context(this);
            else
                context = inState;

            case2_2(context);

            // bring the state up to date for test case 2.6
            context.anno1.Anchors.Remove(context.anchor2);
            context.anno2.Anchors.Add(context.anchor2);
            AnnotationTestHelper.ModifyAnchor(context.anchor1);
        }

        /// <summary>
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Operation	|	Anchor	|	Annotation	|	Results		|	Event				|	Status Anno1	|	Status Anno2	|
        ///|------------|-----------|---------------|---------------|-----------------------|----------------------------------------
        ///| Add		|	anchor1	|	anno1		|	true		|	Anno1->add anchor1	|	anchor1			|					|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Add		|	anchor1	|	anno2		|	Exception	|						|	anchor1			|					|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Remove		|	anchor2	|	anno1		|	False		|						|	anchor1			|					|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Add		|	anchor2	|	anno2		|	True		|	Anno1->add anchor2	|	anchor1			|	anchor2			|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Modify		|	anchor1	|				|				|	Anno1->mod anchor1	|	anchor1			|	anchor2			|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Add		|	anchor3	|	anno2		|	True		|	Anno2->add anchor3	|	anchor1			|	anchor2,anchor3	|
        /// -------------------------------------------------------------------------------------------------------------------------
        /// </summary>
        /// <param name="inState">State if this is not the final part, null if it is the final part.</param>
        [Priority(0)]
        public void case2_6(State2Context inState)
        {
            State2Context context;
            bool final = (inState == null);

            if (final)
                context = new State2Context(this);
            else
                context = inState;

            case2_6update(context);

            context.anno2.Anchors.Add(context.anchor3);

            context.anno1EventListener.VerifyEventCount(1, 0, 1);
            context.anno2EventListener.VerifyEventCount(2, 0, 0);
            context.anno2EventListener.VerifyLastChangedAnchor(context.anchor3);

            if (final)
                passTest("Anchor changed events ok.");
        }

        /// <summary>
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Operation	|	Anchor	|	Annotation	|	Results		|	Event				|	Status Anno1	|	Status Anno2	|
        ///|------------|-----------|---------------|---------------|-----------------------|----------------------------------------
        ///| Add		|	anchor1	|	anno1		|	true		|	Anno1->add anchor1	|	anchor1			|					|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Add		|	anchor1	|	anno2		|	Exception	|						|	anchor1			|					|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Remove		|	anchor2	|	anno1		|	False		|						|	anchor1			|					|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Add		|	anchor2	|	anno2		|	True		|	Anno1->add anchor2	|	anchor1			|	anchor2			|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Modify		|	anchor1	|				|				|	Anno1->mod anchor1	|	anchor1			|	anchor2			|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Add		|	anchor3	|	anno2		|	True		|	Anno2->add anchor3	|	anchor1			|	anchor2,anchor3	|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Modify		|	anchor3	|				|				|	Anno2->mod anchor3	|	anchor1			|	anchor2,anchor3	|
        /// -------------------------------------------------------------------------------------------------------------------------
        /// </summary>
        /// <param name="inState">State if this is not the final part, null if it is the final part.</param>
        [Priority(0)]
        public void case2_7(State2Context inState)
        {
            State2Context context;
            bool final = (inState == null);

            if (final)
                context = new State2Context(this);
            else
                context = inState;

            case2_6(context);

            AnnotationTestHelper.ModifyAnchor(context.anchor3);

            context.anno1EventListener.VerifyEventCount(1, 0, 1);
            context.anno2EventListener.VerifyEventCount(2, 0, 1);
            context.anno2EventListener.VerifyLastChangedAnchor(context.anchor3);

            if (final)
                passTest("Anchor changed events ok.");
        }

        /// <summary>
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Operation	|	Anchor	|	Annotation	|	Results		|	Event				|	Status Anno1	|	Status Anno2	|
        ///|------------|-----------|---------------|---------------|-----------------------|----------------------------------------
        ///| Add		|	anchor1	|	anno1		|	true		|	Anno1->add anchor1	|	anchor1			|					|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Add		|	anchor1	|	anno2		|	Exception	|						|	anchor1			|					|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Remove		|	anchor2	|	anno1		|	False		|						|	anchor1			|					|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Add		|	anchor2	|	anno2		|	True		|	Anno1->add anchor2	|	anchor1			|	anchor2			|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Modify		|	anchor1	|				|				|	Anno1->mod anchor1	|	anchor1			|	anchor2			|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Add		|	anchor3	|	anno2		|	True		|	Anno2->add anchor3	|	anchor1			|	anchor2,anchor3	|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Modify		|	anchor3	|				|				|	Anno2->mod anchor3	|	anchor1			|	anchor2,anchor3	|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Remove		|	anchor3	|	anno2		|	True		|	Anno2->del anchor3	|	anchor1			|	anchor2			|
        /// -------------------------------------------------------------------------------------------------------------------------
        /// </summary>
        /// <param name="inState">State if this is not the final part, null if it is the final part.</param>
        [Priority(0)]
        public void case2_8(State2Context inState)
        {
            State2Context context;
            bool final = (inState == null);

            if (final)
                context = new State2Context(this);
            else
                context = inState;

            case2_7(context);

            context.anno2.Anchors.Remove(context.anchor3);

            context.anno1EventListener.VerifyEventCount(1, 0, 1);
            context.anno2EventListener.VerifyEventCount(2, 1, 1);
            context.anno2EventListener.VerifyLastChangedAnchor(context.anchor3);

            if (final)
                passTest("Anchor changed events ok.");
        }

        /// <summary>
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Operation	|	Anchor	|	Annotation	|	Results		|	Event				|	Status Anno1	|	Status Anno2	|
        ///|------------|-----------|---------------|---------------|-----------------------|----------------------------------------
        ///| Add		|	anchor1	|	anno1		|	true		|	Anno1->add anchor1	|	anchor1			|					|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Add		|	anchor1	|	anno2		|	Exception	|						|	anchor1			|					|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Remove		|	anchor2	|	anno1		|	False		|						|	anchor1			|					|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Add		|	anchor2	|	anno2		|	True		|	Anno1->add anchor2	|	anchor1			|	anchor2			|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Modify		|	anchor1	|				|				|	Anno1->mod anchor1	|	anchor1			|	anchor2			|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Add		|	anchor3	|	anno2		|	True		|	Anno2->add anchor3	|	anchor1			|	anchor2,anchor3	|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Modify		|	anchor3	|				|				|	Anno2->mod anchor3	|	anchor1			|	anchor2,anchor3	|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Remove		|	anchor3	|	anno2		|	True		|	Anno2->del anchor3	|	anchor1			|	anchor2			|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Add		|	anchor3	|	anno1		|	True		|	Anno1->add anchor3	|	anchor1,anchor3	|	anchor2			|
        /// -------------------------------------------------------------------------------------------------------------------------
        /// </summary>
        /// <param name="inState">State if this is not the final part, null if it is the final part.</param>
        [Priority(0)]
        public void case2_10update(State2Context inState)
        {
            State2Context context;
            bool final = (inState == null);

            if (final)
                context = new State2Context(this);
            else
                context = inState;

            case2_8(context);

            context.anno1.Anchors.Add(context.anchor3);

        }

        /// <summary>
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Operation	|	Anchor	|	Annotation	|	Results		|	Event				|	Status Anno1	|	Status Anno2	|
        ///|------------|-----------|---------------|---------------|-----------------------|----------------------------------------
        ///| Add		|	anchor1	|	anno1		|	true		|	Anno1->add anchor1	|	anchor1			|					|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Add		|	anchor1	|	anno2		|	Exception	|						|	anchor1			|					|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Remove		|	anchor2	|	anno1		|	False		|						|	anchor1			|					|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Add		|	anchor2	|	anno2		|	True		|	Anno1->add anchor2	|	anchor1			|	anchor2			|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Modify		|	anchor1	|				|				|	Anno1->mod anchor1	|	anchor1			|	anchor2			|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Add		|	anchor3	|	anno2		|	True		|	Anno2->add anchor3	|	anchor1			|	anchor2,anchor3	|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Modify		|	anchor3	|				|				|	Anno2->mod anchor3	|	anchor1			|	anchor2,anchor3	|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Remove		|	anchor3	|	anno2		|	True		|	Anno2->del anchor3	|	anchor1			|	anchor2			|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Add		|	anchor3	|	anno1		|	True		|	Anno1->add anchor3	|	anchor1,anchor3	|	anchor2			|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Modify		|	anchor3	|				|				|	Anno1->mod anchor3	|	anchor1,anchor3	|	anchor2			|
        /// -------------------------------------------------------------------------------------------------------------------------
        /// </summary>
        /// <param name="inState">State if this is not the final part, null if it is the final part.</param>
        [Priority(0)]
        public void case2_10(State2Context inState)
        {
            State2Context context;
            bool final = (inState == null);

            if (final)
                context = new State2Context(this);
            else
                context = inState;

            case2_10update(context);

            AnnotationTestHelper.ModifyAnchor(context.anchor3);

            context.anno1EventListener.VerifyEventCount(2, 0, 2);
            context.anno2EventListener.VerifyEventCount(2, 1, 1);
            context.anno1EventListener.VerifyLastChangedAnchor(context.anchor3);

            if (final)
                passTest("Anchor changed events ok.");
        }

        /// <summary>
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Operation	|	Anchor	|	Annotation	|	Results		|	Event				|	Status Anno1	|	Status Anno2	|
        ///|------------|-----------|---------------|---------------|-----------------------|----------------------------------------
        ///| Add		|	anchor1	|	anno1		|	true		|	Anno1->add anchor1	|	anchor1			|					|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Add		|	anchor1	|	anno2		|	Exception	|						|	anchor1			|					|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Remove		|	anchor2	|	anno1		|	False		|						|	anchor1			|					|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Add		|	anchor2	|	anno2		|	True		|	Anno1->add anchor2	|	anchor1			|	anchor2			|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Modify		|	anchor1	|				|				|	Anno1->mod anchor1	|	anchor1			|	anchor2			|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Add		|	anchor3	|	anno2		|	True		|	Anno2->add anchor3	|	anchor1			|	anchor2,anchor3	|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Modify		|	anchor3	|				|				|	Anno2->mod anchor3	|	anchor1			|	anchor2,anchor3	|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Remove		|	anchor3	|	anno2		|	True		|	Anno2->del anchor3	|	anchor1			|	anchor2			|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Add		|	anchor3	|	anno1		|	True		|	Anno1->add anchor3	|	anchor1,anchor3	|	anchor2			|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Modify		|	anchor3	|				|				|	Anno1->mod anchor3	|	anchor1,anchor3	|	anchor2			|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Add		|	anchor3	|	anno2		|	Exception	|						|	anchor1,anchor3	|	anchor2			|
        /// -------------------------------------------------------------------------------------------------------------------------
        /// </summary>
        /// <param name="inState">State if this is not the final part, null if it is the final part.</param>
        [Priority(0)]
        public void case2_11(State2Context inState)
        {
            State2Context context;
            bool exceptThrown = false;
            bool final = (inState == null);

            if (final)
                context = new State2Context(this);
            else
                context = inState;

            case2_10(context);
            try
            {
                context.anno2.Anchors.Add(context.anchor1);
            }
            catch (Exception e)
            {
                exceptThrown = true;
                if (e is ArgumentException)
                {
                    VerifyAnchors(context.anno1, new AnnotationResource[] { context.anchor1, context.anchor3 });
                    VerifyAnchors(context.anno2, new AnnotationResource[] { context.anchor2 });
                    context.anno1EventListener.VerifyEventCount(2, 0, 2);
                    context.anno2EventListener.VerifyEventCount(2, 1, 1);
                    if (final)
                        passTest("Expected exception occurred.");
                }
                else
                    failTest("Unexpected type of exception occurred: " + e.ToString());
            }
            if (!exceptThrown)
                failTest("Expected exception did not occur.");
        }

        /// <summary>
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Operation	|	Anchor	|	Annotation	|	Results		|	Event				|	Status Anno1	|	Status Anno2	|
        ///|------------|-----------|---------------|---------------|-----------------------|----------------------------------------
        ///| Add		|	anchor1	|	anno1		|	true		|	Anno1->add anchor1	|	anchor1			|					|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Add		|	anchor1	|	anno2		|	Exception	|						|	anchor1			|					|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Remove		|	anchor2	|	anno1		|	False		|						|	anchor1			|					|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Add		|	anchor2	|	anno2		|	True		|	Anno1->add anchor2	|	anchor1			|	anchor2			|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Modify		|	anchor1	|				|				|	Anno1->mod anchor1	|	anchor1			|	anchor2			|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Add		|	anchor3	|	anno2		|	True		|	Anno2->add anchor3	|	anchor1			|	anchor2,anchor3	|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Modify		|	anchor3	|				|				|	Anno2->mod anchor3	|	anchor1			|	anchor2,anchor3	|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Remove		|	anchor3	|	anno2		|	True		|	Anno2->del anchor3	|	anchor1			|	anchor2			|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Add		|	anchor3	|	anno1		|	True		|	Anno1->add anchor3	|	anchor1,anchor3	|	anchor2			|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Modify		|	anchor3	|				|				|	Anno1->mod anchor3	|	anchor1,anchor3	|	anchor2			|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Add		|	anchor3	|	anno2		|	Exception	|						|	anchor1,anchor3	|	anchor2			|
        /// -------------------------------------------------------------------------------------------------------------------------
        ///| Modify		|	anchor3	|				|				|	Anno1->mod anchor3	|	anchor1,anchor3	|	anchor2			|
        /// -------------------------------------------------------------------------------------------------------------------------
        /// </summary>
        /// <param name="inState">State if this is not the final part, null if it is the final part.</param>
        [Priority(0)]
        public void case2_12(State2Context inState)
        {
            State2Context context;
            bool final = (inState == null);

            if (final)
                context = new State2Context(this);
            else
                context = inState;

            case2_11(context);

            AnnotationTestHelper.ModifyAnchor(context.anchor3);

            context.anno1EventListener.VerifyEventCount(2, 0, 3);
            context.anno2EventListener.VerifyEventCount(2, 1, 1);
            context.anno1EventListener.VerifyLastChangedAnchor(context.anchor3);

            if (final)
                passTest("Anchor changed events ok.");
        }

        #endregion TEST METHODS

        #endregion BVT TESTS

        #region PRIORITY TESTS
        #endregion PRIORITY TESTS

    }
}


