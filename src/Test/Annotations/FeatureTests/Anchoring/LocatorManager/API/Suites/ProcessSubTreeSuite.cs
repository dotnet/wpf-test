// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: BVT test cases for the ProcessSubTreeSuite test suite.
//  
//  The ProcessSubTree test suite consists of testing the following API and 
//  functionality:
//
//  - ProcessSubTree

using System;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Documents;
using System.Collections;

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
using System.Collections.Generic;

namespace Avalon.Test.Annotations.Suites
{
    public class ProcessSubTreeSuite_BVT : AProcessSubTreeSuite
    {
        // -----------------------------------------------------------------------------------------------
        //								PROCESSSUBTREE TEST CASES
        //
        //  These tests create annotations on nodes in the tree then call ProcessSubTree and check 
        //  the list of attached annotations returned to see if we received back the annotations 
        //  originally set (and no extra ones)
        // -----------------------------------------------------------------------------------------------

        /// <summary>
        /// Create annotations on FrameworkElement and Text leaves of the tree with
        /// TextFingerprintProcessor set on the parent of Text leaves and DataIdProcessor
        /// set on the root.  Start searching starting from the root, verify that all
        /// annotations are found.
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void processsubtree3()
        {
            BuildTree_ProcessSubTree3();
            TextRange selection1 = AnchoringAPIHelpers.SubstringTextBox(txt1, 5, 15);
            TextRange selection2 = AnchoringAPIHelpers.SubstringTextBox(txt2, 5, 15);

            toFind.Add(AnchoringAPIHelpers.CreateBasicAnnotation(c, manager, annotationStore));
            toFind.Add(AnchoringAPIHelpers.CreateBasicAnnotation(selection1, manager, annotationStore));
            toFind.Add(AnchoringAPIHelpers.CreateBasicAnnotation(selection2, manager, annotationStore));

            // Call ProcessSubtree on A, check if IList contains annotation_c, annotation_txt1, annotation_txt2
            processResult = ProcessSubTree(a);
            SearchProcessedAnnotations(processResult, toFind, null);
        }

        // -----------------------------------------------------------------------------------------------
        //								PROCESSANNOTATIONS TEST CASES
        //
        //  ProcessAnnotations is called on a subtree that needs to have annotations processor for it.
        //  Usually called by processors so these tests just check for valid/invalid input
        // -----------------------------------------------------------------------------------------------

        /// <summary>
        /// Null subtree
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void processannotations1()
        {
            try
            {
                manager.ProcessAnnotations(null);
                failTest("ArgumentNullException should have been thrown");
            }
            catch (System.ArgumentNullException)
            {
                passTest("ArgumentNullException correctly thrown");
            }
            catch (Exception exp)
            {
                failTest("ArgumentNullException should have been thrown, not " + exp.ToString());
            }
        }

        /// <summary>
        /// Subtree is new object derived from DependencyObject
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void processannotations2()
        {
            DockPanel dp = new DockPanel();
            EnableService(dp, annotationStore);
            IList<IAttachedAnnotation> retList = manager.ProcessAnnotations(dp);

            if (retList.Count == 0)
                passTest("Empty list expected.  Empty list returned");
            else if (retList == null)
                failTest("Empty list expected.  Null returned");
            else
                failTest("Empty list expected.  List contains " + retList.Count + " elements");
        }

        // -----------------------------------------------------------------------------------------------
        //								PROCESSSUBTREE TEST CASES
        //
        //  These tests create annotations on nodes in the tree then call ProcessSubTree and check 
        //  the list of attached annotations returned to see if we received back the annotations 
        //  originally set (and no extra ones)
        // -----------------------------------------------------------------------------------------------

        /// <summary>
        /// Create and find annotation on single node tree
        /// </summary>
        [DisabledTestCase()]
        [Priority(1)]
        private void processsubtree1()
        {
            BuildRoot();
            toFind.Add(AnchoringAPIHelpers.CreateBasicAnnotation(a, manager, annotationStore));
            processResult = ProcessSubTree(a);
            SearchProcessedAnnotations(processResult, toFind, null);
        }

        /// <summary>
        /// Create annotations on each node in a simple tree.  Only the annotations
        /// set on the leaves should be found.
        /// </summary>
        [DisabledTestCase()]
        [Priority(1)]
        private void processsubtree2()
        {
            BuildSingleLevelTree();

            // Create and store annotations for nodes A, B, C
            doNotFind.Add(AnchoringAPIHelpers.CreateBasicAnnotation(a, manager, annotationStore));
            toFind.Add(AnchoringAPIHelpers.CreateBasicAnnotation(b, manager, annotationStore));
            toFind.Add(AnchoringAPIHelpers.CreateBasicAnnotation(c, manager, annotationStore));

            // Call ProcessSubtree on A, check to see if IList returned contains annotation_b and annotation_c
            processResult = ProcessSubTree(a);
            SearchProcessedAnnotations(processResult, toFind, doNotFind);
        }

        /// <summary>
        /// Create annotations on all FrameworkElement and Text leaves of the tree with
        /// TextFingerprintProcessor set on the parent of the Text leaves.  Start searching
        /// starting from this parent, verify that only the Text annotations are found
        /// </summary>
        [DisabledTestCase()]
        [Priority(1)]
        private void processsubtree4()
        {
            BuildTree_ProcessSubTree4And5();

            TextRange selection1 = AnchoringAPIHelpers.SubstringTextBox(txt1, 5, 15);
            TextRange selection2 = AnchoringAPIHelpers.SubstringTextBox(txt2, 5, 15);

            doNotFind.Add(AnchoringAPIHelpers.CreateBasicAnnotation(c, manager, annotationStore));
            toFind.Add(AnchoringAPIHelpers.CreateBasicAnnotation(selection1, manager, annotationStore));
            toFind.Add(AnchoringAPIHelpers.CreateBasicAnnotation(selection2, manager, annotationStore));

            // Call ProcessSubtree on A, check to see if IList returned contains annotation_b and annotation_c
            processResult = ProcessSubTree(b);
            SearchProcessedAnnotations(processResult, toFind, doNotFind);
        }

        /// <summary>
        /// Create annotations on all FrameworkElement and Text leaves of the tree with
        /// TextFingerprintProcessor set on the parent of the Text leaves.  Start searching
        /// starting from the root, verify that all annotations are found
        /// </summary>
        [DisabledTestCase()]
        [Priority(1)]
        private void processsubtree5()
        {
            BuildTree_ProcessSubTree4And5();

            TextRange selection1 = AnchoringAPIHelpers.SubstringTextBox(txt1, 5, 15);
            TextRange selection2 = AnchoringAPIHelpers.SubstringTextBox(txt2, 5, 15);

            toFind.Add(AnchoringAPIHelpers.CreateBasicAnnotation(c, manager, annotationStore));
            toFind.Add(AnchoringAPIHelpers.CreateBasicAnnotation(selection1, manager, annotationStore));
            toFind.Add(AnchoringAPIHelpers.CreateBasicAnnotation(selection2, manager, annotationStore));

            // Call ProcessSubtree on A, check to see if IList returned contains annotation_b and annotation_c
            processResult = ProcessSubTree(a);
            SearchProcessedAnnotations(processResult, toFind, null);
        }

        /// <summary>
        /// Create annotations on all FrameworkElement and Text leaves of the tree with 
        /// TextFingerprintProcessor set on the parent of the Text leaves.  Start searching
        /// starting from this parent, verify that only Text annotations are found 
        /// </summary>
        [DisabledTestCase()]
        [Priority(1)]
        private void processsubtree6()
        {
            BuildTree_ProcessSubTree6();

            TextRange selection1 = AnchoringAPIHelpers.SubstringTextBox(txt1, 5, 15);
            TextRange selection2 = AnchoringAPIHelpers.SubstringTextBox(txt2, 5, 15);

            doNotFind.Add(AnchoringAPIHelpers.CreateBasicAnnotation(c, manager, annotationStore));
            toFind.Add(AnchoringAPIHelpers.CreateBasicAnnotation(selection1, manager, annotationStore));
            toFind.Add(AnchoringAPIHelpers.CreateBasicAnnotation(selection2, manager, annotationStore));

            // Call ProcessSubtree on A, check to see if IList returned contains annotation_b and annotation_c
            processResult = ProcessSubTree(b);
            SearchProcessedAnnotations(processResult, toFind, doNotFind);
        }

        /// <summary>
        /// Create annotations on 2 FrameworkElement leaves of the tree with 
        /// DataIdProcessor set on the root.  Start searching starting from
        /// the root, verify that all the leaf annotations are found 
        /// </summary>
        [DisabledTestCase()]
        [Priority(1)]
        private void processsubtree7()
        {
            BuildTree_ProcessSubTree7();

            toFind.Add(AnchoringAPIHelpers.CreateBasicAnnotation(f, manager, annotationStore));
            toFind.Add(AnchoringAPIHelpers.CreateBasicAnnotation(i, manager, annotationStore));

            // Call ProcessSubtree on A, check to see if IList returned contains annotation_f and annotation_i
            processResult = ProcessSubTree(a);
            SearchProcessedAnnotations(processResult, toFind, null);
        }

        /// <summary>
        /// Create annotations on 2 FrameworkElement leaves of the tree with
        /// DataIdProcessor set on the root (by default).  Start searching 
        /// starting from their parent, verify that only annotations set on
        /// the FrameworkElement with a DataId are found 
        /// </summary>
        [DisabledTestCase()]
        [Priority(1)]
        private void processsubtree8()
        {
            BuildTree_ProcessSubTree8();

            // Create and store annotations for nodes H, I
            doNotFind.Add(AnchoringAPIHelpers.CreateBasicAnnotation(h, manager, annotationStore));
            toFind.Add(AnchoringAPIHelpers.CreateBasicAnnotation(i, manager, annotationStore));

            // Call ProcessSubtree on G, check to see if IList returned contains annotation_i
            processResult = ProcessSubTree(g);
            SearchProcessedAnnotations(processResult, toFind, doNotFind);
        }

        /// <summary>
        /// Create annotations on FrameworkElement and Text leaves of a tree with 
        /// TextFingerprintProcessor set on an ancestor.  Start searching starting
        /// from the root, verify that only Text annotations are found
        /// </summary>
        [DisabledTestCase()]
        [Priority(1)]
        private void processsubtree9()
        {
            BuildTree_ProcessSubTree9();
            TextRange selection1 = AnchoringAPIHelpers.SubstringTextBox(txt1, 5, 15);
            TextRange selection2 = AnchoringAPIHelpers.SubstringTextBox(txt2, 5, 15);

            // Create and store annotations for nodes F, H, I
            toFind.Add(AnchoringAPIHelpers.CreateBasicAnnotation(selection1, manager, annotationStore));
            // Next line commented out since TextFingerprintProcessor doesn't work with TextBox
            //			doNotFind.Add(AnchoringAPIHelpers.CreateBasicAnnotation(h, _manager, _annotationStore));
            toFind.Add(AnchoringAPIHelpers.CreateBasicAnnotation(selection2, manager, annotationStore));

            // Call ProcessSubtree on A, check to see if IList returned contains annotation_f, annotation_i
            processResult = ProcessSubTree(a);
            SearchProcessedAnnotations(processResult, toFind, doNotFind);
        }

        /// <summary>
        /// Create annotations on FrameworkElement and Text leaves of the tree with
        /// DataIdProcessor set on the parent of a FrameworkElement leaf and 
        /// TextFingerprintProcessor set on the parent of a FrameworkElement and
        /// Text leaf.  Start searching starting from the root, verify that 
        /// the annotations belonging to the FE w/ the DataIdProcessor and
        /// the Text with the TextFingerprintProceesor are found
        /// </summary>
        [DisabledTestCase()]
        [Priority(1)]
        private void processsubtree10()
        {
            BuildTree_ProcessSubTree10();
            TextRange selection1 = AnchoringAPIHelpers.SubstringTextBox(txt1, 5, 15);

            // Create and store annotations for nodes D, F, G
            toFind.Add(AnchoringAPIHelpers.CreateBasicAnnotation(d, manager, annotationStore));
            // Next line commented out since TextFingerprintProcessor doesn't work with TextBox
            //			doNotFind.Add(AnchoringAPIHelpers.CreateBasicAnnotation(f, _manager, _annotationStore));
            toFind.Add(AnchoringAPIHelpers.CreateBasicAnnotation(selection1, manager, annotationStore));

            // Call ProcessSubtree on A, check to see if IList returned contains annotation_d, annotation_g
            processResult = ProcessSubTree(a);
            SearchProcessedAnnotations(processResult, toFind, doNotFind);
        }
    }
}

