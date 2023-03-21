// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Pri1 test cases for the LocatorSetSuite test suite.
//  
//  The ContentLocatorGroup test suite consists of testing the following API and 
//  functionality:
//
//  - GenerateLocators
//  - ResolveLocator
//  - FindAttachedAnchor

using System;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Documents;
using System.Collections;
using System.Collections.Generic;

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
using Proxies.MS.Internal.Annotations.Anchoring;
using Annotations.Test.Framework;
using Proxies.MS.Internal.Annotations;

namespace Avalon.Test.Annotations.Suites
{
    [ExecutionGroupCompatible(false)]
    public class LocatorSetSuite_Pri1 : ALocatorSetSuite
    {

        /// <summary>
        /// GenerateLocators:  txtSelection is all of P1, P2, P3.
        /// ResolveLocator gets back txtSelection
        /// </summary>
        [DisabledTestCase()]
        [Priority(1)]
        private void locatorset_generateandresolvelocators1()
        {
            expectedLocators = 3;

            // If test has not failed before this point, it counts as a pass
            if (GenerateAndResolveLocatorSets(p1.ContentStart, p3.ContentEnd,
                moveStart, moveEnd, expectedLocators, expectedLocatorParts, a, AttachmentLevel.Full))
                passTest("Produced the correct number of locators & locator parts and resolved to the correct anchor");
        }


        /// <summary>
        /// 
        /// </summary>
        [DisabledTestCase()]
        [Priority(1)]
        private void locatorset_generateandresolvelocators1_1()
        {
        }


        /// <summary>
        /// Try to resolve a non-contiguous ContentLocatorGroup back to original anchors
        /// This should throw InvalidOperationException in Merge (private fcn)
        /// </summary>
        [DisabledTestCase()]
        [Priority(1)]
        private void locatorset_generateandresolvelocators1_2()
        {
            try
            {
                // Create non-contiguous text selections
                IList<ContentLocatorBase> p2Locator = manager.GenerateLocators(new TextRange(p2.ContentStart, p2.ContentEnd));
                IList<ContentLocatorBase> p4Locator = manager.GenerateLocators(new TextRange(p4.ContentStart, p4.ContentEnd));

                // Append both sets of Locators generated together in one ContentLocatorGroup
                ContentLocatorGroup ls = new ContentLocatorGroup();

                foreach (ContentLocatorBase l in p2Locator)
                    ls.Locators.Add((ContentLocator)l);
                foreach (ContentLocatorBase l in p4Locator)
                    ls.Locators.Add((ContentLocator)l);

                AttachmentLevel attachmtType = AttachmentLevel.Unresolved;
                Object resolveAnchor = manager.ResolveLocator(ls, 0, a, out attachmtType);

                // Should throw an InvalidOperationException
                failTest("InvalidOperationException should have been thrown");
            }
            catch (System.InvalidOperationException)
            {
                passTest("InvalidOperationException correctly thrown");
            }
            catch (Exception exp)
            {
                failTest("InvalidOperationException expected, not: " + exp.ToString());
            }
        }


        /// <summary>
        /// GenerateLocators:  txtSelection is last line of P1, all of P2, first line of P3.
        /// ResolveLocator gets back txtSelection
        /// </summary>
        [DisabledTestCase()]
        [Priority(1)]
        private void locatorset_generateandresolvelocators2()
        {
            moveStart = -22;
            moveEnd = 20;
            expectedLocators = 3;

            // If test has not failed before this point, it counts as a pass
            if (GenerateAndResolveLocatorSets(p1.ContentEnd, p3.ContentStart,
                moveStart, moveEnd, expectedLocators, expectedLocatorParts, a, AttachmentLevel.Full))
                passTest("Produced the correct number of locators & locator parts and resolved to the correct anchor");
        }


        /// <summary>
        /// GenerateLocators:  txtSelection is last character of P2, first character of P3.
        /// ResolveLocator gets back txtSelection
        /// </summary>
        [DisabledTestCase()]
        [Priority(1)]
        private void locatorset_generateandresolvelocators3()
        {
            moveStart = -1;
            moveEnd = 1;
            expectedLocators = 2;

            // If test has not failed before this point, it counts as a pass
            if (GenerateAndResolveLocatorSets((p2.ContentEnd), (p3.ContentStart),
                moveStart, moveEnd, expectedLocators, expectedLocatorParts, a, AttachmentLevel.Full))
                passTest("Produced the correct number of locators & locator parts and resolved to the correct anchor");
        }


        /// <summary>
        /// GenerateLocators:  txtSelection is last half of P1, first 2 characters of P2.
        /// ResolveLocator on ContentLocatorGroup gets back txtSelection
        /// </summary>
        [DisabledTestCase()]
        [Priority(1)]
        private void locatorset_generateandresolvelocators4()
        {
            moveStart = -44;
            moveEnd = 2;
            expectedLocators = 2;

            // If test has not failed before this point, it counts as a pass
            if (GenerateAndResolveLocatorSets((p1.ContentEnd), (p2.ContentStart),
                moveStart, moveEnd, expectedLocators, expectedLocatorParts, a, AttachmentLevel.Full))
                passTest("Produced the correct number of locators & locator parts and resolved to the correct anchor");
        }


        /// <summary>
        /// GenerateLocators:  txtSelection is last character of P2, all of P3.
        /// ResolveLocator on ContentLocatorGroup gets back txtSelection
        /// </summary>
        [DisabledTestCase()]
        [Priority(1)]
        private void locatorset_generateandresolvelocators5()
        {
            moveStart = -1;
            expectedLocators = 2;

            // If test has not failed before this point, it counts as a pass
            if (GenerateAndResolveLocatorSets((p2.ContentEnd), (p3.ContentEnd),
                moveStart, moveEnd, expectedLocators, expectedLocatorParts, a, AttachmentLevel.Full))
                passTest("Produced the correct number of locators & locator parts and resolved to the correct anchor");
        }


        /// <summary>
        /// GenerateLocators:  txtSelection is last character of P1, all of P2, first character of P3.
        /// ResolveLocator on ContentLocatorGroup gets back txtSelection
        /// </summary>
        [DisabledTestCase()]
        [Priority(1)]
        private void locatorset_generateandresolvelocators6()
        {
            moveStart = -1;
            moveEnd = 1;
            expectedLocators = 3;

            // If test has not failed before this point, it counts as a pass
            if (GenerateAndResolveLocatorSets((p1.ContentEnd), (p3.ContentStart),
                moveStart, moveEnd, expectedLocators, expectedLocatorParts, a, AttachmentLevel.Full))
                passTest("Produced the correct number of locators & locator parts and resolved to the correct anchor");
        }


        /// <summary>
        /// GenerateLocators:  txtSelection is all of P1, P2, P3, P4.
        /// ResolveLocator on ContentLocatorGroup gets back txtSelection
        /// </summary>
        [DisabledTestCase()]
        [Priority(1)]
        private void locatorset_generateandresolvelocators7()
        {
            expectedLocators = 5;

            // If test has not failed before this point, it counts as a pass
            if (GenerateAndResolveLocatorSets((p1.ContentStart), (p4.ContentEnd),
                moveStart, moveEnd, expectedLocators, expectedLocatorParts, a, AttachmentLevel.Full))
                passTest("Produced the correct number of locators & locator parts and resolved to the correct anchor");
        }


        /// <summary>
        /// GenerateLocators:  txtSelection is last 2 characters of P1, all of P2 and P3, 
        /// empty_paragraph, empty_image, first character of P4.  ResolveLocator on ContentLocatorGroup
        /// gets back txtSelection
        /// </summary>
        [DisabledTestCase()]
        [Priority(1)]
        private void locatorset_generateandresolvelocators8()
        {
            moveStart = -2;
            moveEnd = 1;
            expectedLocators = 5;

            // If test has not failed before this point, it counts as a pass
            if (GenerateAndResolveLocatorSets(p1.ContentEnd, p4.ContentStart,
                moveStart, moveEnd, expectedLocators, expectedLocatorParts, a, AttachmentLevel.Full))
                passTest("Produced the correct number of locators & locator parts and resolved to the correct anchor");
        }


        /// <summary>
        /// GenerateLocators:  txtSelection is last line of P1, all of P2 and P3, empty Paragraph.
        /// ResolveLocator on ContentLocatorGroup gets back txtSelection
        /// </summary>
        [DisabledTestCase()]
        [Priority(1)]
        private void locatorset_generateandresolvelocators9()
        {
            moveStart = -22;
            expectedLocators = 3;

            // If test has not failed before this point, it counts as a pass
            if (GenerateAndResolveLocatorSets((p1.ContentEnd), (empty_Paragraph.ContentEnd),
                moveStart, moveEnd, expectedLocators, expectedLocatorParts, a, AttachmentLevel.Full))
                passTest("Produced the correct number of locators & locator parts and resolved to the correct anchor");
        }


        /// <summary>
        /// GenerateLocators:  txtSelection is last character of P2, all of P3, empty Paragraph and empty Image
        /// ResolveLocator on ContentLocatorGroup gets back txtSelection
        /// </summary>
        [DisabledTestCase()]
        [Priority(1)]
        private void locatorset_generateandresolvelocators10()
        {
            moveStart = -1;
            moveEnd = -1;
            expectedLocators = 3;

            // If test has not failed before this point, it counts as a pass
            if (GenerateAndResolveLocatorSets((p2.ContentEnd), (p4.ContentStart),
                moveStart, moveEnd, expectedLocators, expectedLocatorParts, a, AttachmentLevel.Full))
                passTest("Produced the correct number of locators & locator parts and resolved to the correct anchor");
        }


        /// <summary>
        /// GenerateLocators:  txtSelection is last line of P1, all of P2 and P3, 
        /// empty Paragraph, empty Image and first line of P4.  ResolveLocator on ContentLocatorGroup gets back
        /// txtSelection
        /// </summary>
        [DisabledTestCase()]
        [Priority(1)]
        private void locatorset_generateandresolvelocators11()
        {
            moveStart = -22;
            moveEnd = 37;
            expectedLocators = 5;

            // If test has not failed before this point, it counts as a pass
            if (GenerateAndResolveLocatorSets((p1.ContentEnd), (p4.ContentStart),
                moveStart, moveEnd, expectedLocators, expectedLocatorParts, a, AttachmentLevel.Full))
                passTest("Produced the correct number of locators & locator parts and resolved to the correct anchor");
        }


        /// <summary>
        /// GenerateLocators:  txtSelection is last character of P2, all of P3
        /// empty Paragraph, empty Image and P4.  ResolveLocator on ContentLocatorGroup gets back txtSelection
        /// </summary>
        [DisabledTestCase()]
        [Priority(1)]
        private void locatorset_generateandresolvelocators12()
        {
            moveStart = -1;
            expectedLocators = 4;

            // If test has not failed before this point, it counts as a pass
            if (GenerateAndResolveLocatorSets((p2.ContentEnd), (p4.ContentEnd),
                moveStart, moveEnd, expectedLocators, expectedLocatorParts, a, AttachmentLevel.Full))
                passTest("Produced the correct number of locators & locator parts and resolved to the correct anchor");
        }


        // -----------------------------------------------------------------------------------------------
        //								FINDATTACHEDANCHOR TEST CASES
        // -----------------------------------------------------------------------------------------------

        /// <summary>
        /// FindAttachedAnchor:  startNode, prefix = A, locator = ContentLocatorGroup for text selection
        /// </summary>
        [DisabledTestCase()]
        [Priority(1)]
        private void locatorset_findattachedanchor1()
        {
            ArrayList args = new ArrayList();
            TextRange txtSelection = new TextRange((p1.ContentStart), (p4.ContentEnd));

            args.Add(a);
            args.Add(txtSelection);
            queueTask(doTestCase_FindAttachedAnchor1, args);
        }


        /// <summary>
        /// FindAttachedAnchor:  startNode, prefix = TextPanel, locator = ContentLocatorGroup for text selection
        /// </summary>
        [DisabledTestCase()]
        [Priority(1)]
        private void locatorset_findattachedanchor2()
        {
            ArrayList args = new ArrayList();
            TextRange txtSelection = new TextRange((p1.ContentStart), (p4.ContentEnd));

            args.Add(txtPanel);
            args.Add(txtSelection);
            queueTask(doTestCase_FindAttachedAnchor2, args);
        }


        /// <summary>
        /// FindAttachedAnchor:  startNode, prefix = empty Image, locator = ContentLocatorGroup for text selection
        /// </summary>
        [DisabledTestCase()]
        [Priority(1)]
        private void locatorset_findattachedanchor3()
        {
            ArrayList args = new ArrayList();
            TextRange txtSelection = new TextRange((p1.ContentStart), (p4.ContentEnd));

            args.Add(empty_image);
            args.Add(txtSelection);
            queueTask(doTestCase_FindAttachedAnchor3, args);
        }

        [TestCase_Helper()]
        public object doTestCase_FindAttachedAnchor1(object inObj)
        {
            // Extract arguments 
            ArrayList args = (ArrayList)inObj;
            Canvas c1 = args[0] as Canvas;
            TextRange selection = args[1] as TextRange;
            object returnedAnchor = null;

            try
            {
                returnedAnchor = FindAttachedAnchor(c1, selection, out anchorType);
            }
            catch (Exception exp)
            {
                failTest("Unexpected exception in doTestCase_FindAttachedAnchor1:  " + exp.ToString());
            }

            VerifyAnchorIsFullyResolved(returnedAnchor, selection, anchorType);
            return null;
        }
        [TestCase_Helper()]
        public object doTestCase_FindAttachedAnchor2(object inObj)
        {
            // Extract arguments
            ArrayList args = (ArrayList)inObj;
            FlowDocumentScrollViewer panel = args[0] as FlowDocumentScrollViewer;
            TextRange selection = args[1] as TextRange;
            object returnedAnchor = null;

            try
            {
                returnedAnchor = FindAttachedAnchor(panel, selection, out anchorType);
            }
            catch (Exception exp)
            {
                failTest("Unexpected exception in doTestCase_FindAttachedAnchor2:  " + exp.ToString());
            }

            VerifyAnchorIsFullyResolved(returnedAnchor, selection, anchorType);
            return null;
        }
        [TestCase_Helper()]
        public object doTestCase_FindAttachedAnchor3(object inObj)
        {
            // Extract arguments
            ArrayList args = (ArrayList)inObj;
            Image emptyImg = args[0] as Image;
            TextRange selection = args[1] as TextRange;
            object returnedAnchor = null;

            try
            {
                returnedAnchor = FindAttachedAnchor(emptyImg, selection, out anchorType);
            }
            catch (Exception exp)
            {
                failTest("Unexpected exception in doTestCase_FindAttachedAnchor3:  " + exp.ToString());
            }

            VerifyAnchorIsFullyResolved(returnedAnchor, selection, anchorType);
            return null;
        }
    }		// end of LocatorSetSuite_Pri1 class
}

