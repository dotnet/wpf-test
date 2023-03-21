// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: BVT test cases for the GenerateAndResolveSuite test suite.
//  
//  The GenerateAndResolve test suite consists of testing the following
//  API and functionality:
//
//  - ResolveLocator
//  - GenerateLocators

using System;
using System.Windows;
using System.Windows.Controls;

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
using System.Windows.Documents;

namespace Avalon.Test.Annotations.Suites
{
    [ExecutionGroupCompatible(false)]
    public class GenerateAndResolveSuite_BVT : AGenerateAndResolveSuite
    {
        [Priority(0)]
        protected void basicgeneratelocators1()
        {
            BuildSimpleTree_BasicGenerateLocators();
            locList = manager.GenerateLocators(b);
            VerifyLocatorList(locList, 0);
        }

        /// <summary>
        /// Leaf (with TextBlock sibling) of a tree with root using TextFingerprintProcessor
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        protected void basicgeneratelocators2()
        {
            BuildComplexTree_BasicGenerateLocators();
            LocatorManager.SetSubTreeProcessorId(a, TextFingerprintProcessor.Id);
            locList = manager.GenerateLocators(i);
            VerifyLocatorList(locList, 0);
        }

        [Priority(0)]
        protected void basicgeneratelocators3()
        {
            BuildComplexTree_BasicGenerateLocators();
            LocatorManager.SetSubTreeProcessorId(a, DataIdProcessor.Id);
            locList = manager.GenerateLocators(i);
            VerifyLocatorList(locList, 0);
        }

        [DisabledTestCase()]
        [Priority(0)]
        protected void generatelocators1()
        {
            try
            {
                locList = manager.GenerateLocators(null);
            }
            catch (System.ArgumentNullException)
            {
                passTest("ArgumentNullException correctly thrown");
            }
            catch (Exception exp)
            {
                failTest("ArgumentNullException should have been thrown, not " + exp.ToString());
            }
            failTest("ArgumentNullException should have been thrown");
        }

        /// <summary>
        /// selection = new object() (not a simple LogicalTreeNode), no selection processor registered for this type
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        protected void generatelocators2()
        {
            try
            {
                locList = manager.GenerateLocators(new object());
            }
            catch (System.ArgumentException)
            {
                passTest("ArgumentException correctly thrown");
            }
            catch (Exception exp)
            {
                failTest("ArgumentException should have been thrown, not " + exp.ToString());
            }
            failTest("ArgumentException should have been thrown");
        }

        /// <summary>
        /// selection = new DockPanel()
        /// </summary>
        [TestId("generatelocators2.1")]
        [Priority(0)]
        protected void generatelocators2_1()
        {
            DockPanel dpTree = new DockPanel();
            locList = manager.GenerateLocators(dpTree);
            VerifyLocatorList(locList, 0);
        }

        [DisabledTestCase()]
        [Priority(0)]
        protected void resolvelocator1()
        {
            try
            {
                resolvedLocator = manager.ResolveLocator(locator, 0, null, out type);
            }
            catch (System.ArgumentNullException)
            {
                passTest("ArgumentNullException correctly thrown");
            }
            catch (Exception exp)
            {
                failTest("ArgumentNullException should have been thrown, not " + exp.ToString());
            }
            failTest("ArgumentNullException should have been thrown");
        }

        /// <summary>
        /// Null locator
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        protected void resolvelocator2()
        {
            try
            {
                resolvedLocator = manager.ResolveLocator(null, 0, new DockPanel(), out type);
            }
            catch (System.ArgumentNullException)
            {
                passTest("ArgumentNullException correctly thrown");
            }
            catch (Exception exp)
            {
                failTest("ArgumentNullException should have been thrown, not " + exp.ToString());
            }
            failTest("ArgumentNullException should have been thrown");
        }

        /// <summary>
        /// Non null locator and startNode, offset = 0
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        protected void resolvelocator3()
        {
            try
            {
                resolvedLocator = manager.ResolveLocator(locator, 0, new DockPanel(), out type);
            }
            catch (System.ArgumentException)
            {
                passTest("ArgumentException correctly thrown");
            }
            catch (Exception exp)
            {
                failTest("ArgumentException should have been thrown, not " + exp.ToString());
            }
            failTest("ArgumentException should have been thrown");
        }

        /// <summary>
        /// Negative offset value
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        protected void resolvelocator4()
        {
            try
            {
                resolvedLocator = manager.ResolveLocator(locator, -1, new DockPanel(), out type);
            }
            catch (System.ArgumentException)
            {
                passTest("ArgumentException correctly thrown");
            }
            catch (Exception exp)
            {
                failTest("ArgumentException should have been thrown, not " + exp.ToString());
            }
            failTest("ArgumentException should have been thrown");
        }

        [Priority(0)]
        protected void singlepassgeneratelocators1()
        {
            BuildSimpleTree_SinglePassGenerateLocators();
            anchor = b;

            if (GenerateAndCheckLocators(anchor, numExpectedLocators, numExpectedLocatorParts, out locator))
                if (ResolveAndCheckLocators(null, locator, 1, a, AttachmentLevel.Unresolved))
                    passTest("ContentLocatorBase resolved to correct anchor and attachment level");
        }

        /// <summary>
        /// Generate locators for leaf selection.  Resolve starting from root, L[0], offset = 0
        /// Verify generated locator resolves to original anchor, anchorType is Full
        /// </summary>
        [Priority(0)]
        protected void singlepassgeneratelocators2()
        {
            BuildSimpleTree_SinglePassGenerateLocators();
            anchor = b;

            if (GenerateAndCheckLocators(anchor, numExpectedLocators, numExpectedLocatorParts, out locator))
                if (ResolveAndCheckLocators(anchor, locator, 0, a, AttachmentLevel.Full))
                    passTest("ContentLocatorBase resolved to correct anchor and attachment level");
        }

        /// <summary>
        /// Generate locators for leaf selection.  Resolve from the leaf, L[0] and offset = 0
        /// Verify generated locator is null, anchorType is Unresolved
        /// </summary>
        [Priority(0)]
        protected void singlepassgeneratelocators3()
        {
            BuildSimpleTree_SinglePassGenerateLocators();
            anchor = b;

            if (GenerateAndCheckLocators(anchor, numExpectedLocators, numExpectedLocatorParts, out locator))
                if (ResolveAndCheckLocators(null, locator, 0, b, AttachmentLevel.Unresolved))
                    passTest("ContentLocatorBase resolved to correct anchor and attachment level");
        }

        /// <summary>
        /// Generate locators for leaf selection.  Resolve starting from the root, L[0], offset = 10
        /// Check if ArgumentException is thrown
        /// </summary>
        [Priority(0)]
        protected void singlepassgeneratelocators4()
        {
            BuildSimpleTree_SinglePassGenerateLocators();
            anchor = b;
            try
            {
                if (GenerateAndCheckLocators(anchor, numExpectedLocators, numExpectedLocatorParts, out locator))
                    if (ResolveAndCheckLocators(anchor, locator, 10, a, anchorType))
                        failTest("ArgumentException should have been thrown");
            }
            catch (System.ArgumentException)
            {
                passTest("ArgumentException correctly thrown");
            }
            catch (Exception exp)
            {
                failTest("ArgumentException should have been thrown, not " + exp.ToString());
            }
        }

        /// <summary>
        /// Generate locators for leaf selection.  Resolve starting from the root, L[0], offset = 0
        /// Verify generated locator is the root, anchorType is Partial
        /// </summary>
        [Priority(0)]
        protected void singlepassgeneratelocators5()
        {
            BuildSimpleTree_SinglePassGenerateLocators();
            anchor = b;

            if (GenerateAndCheckLocators(anchor, numExpectedLocators, numExpectedLocatorParts, out locator))
            {
                // Remove the anchor and resolve locator
                a.Children.Remove(b);
                if (ResolveAndCheckLocators(a, locator, 0, a, AttachmentLevel.Incomplete))
                    passTest("ContentLocatorBase resolved to correct anchor and attachment level");
            }
        }

        /// <summary>
        /// Generate locators for leaf selection.  Resolve starting from the root, L[0], offset = 1
        /// Verify generated locator is null, anchorType is Unresolved
        /// </summary>
        [Priority(0)]
        protected void singlepassgeneratelocators6()
        {
            BuildSimpleTree_SinglePassGenerateLocators();
            anchor = b;

            if (GenerateAndCheckLocators(anchor, numExpectedLocators, numExpectedLocatorParts, out locator))
            {
                // Remove the anchor and resolve locator
                a.Children.Remove(b);
                if (ResolveAndCheckLocators(null, locator, 1, a, AttachmentLevel.Unresolved))
                    passTest("ContentLocatorBase resolved to correct anchor and attachment level");
            }
        }

        /// <summary>
        /// Generate locators for leaf selection.  Resolve an element not in tree, L[0], offset = 0
        /// Verify that generated locator is null, anchorType is Unresolved
        /// </summary>
        [Priority(0)]
        protected void singlepassgeneratelocators7()
        {
            BuildSimpleTree_SinglePassGenerateLocators();
            anchor = b;

            if (GenerateAndCheckLocators(anchor, numExpectedLocators, numExpectedLocatorParts, out locator))
                if (ResolveAndCheckLocators(null, locator, 0, new Canvas(), AttachmentLevel.Unresolved))
                    passTest("ContentLocatorBase resolved to correct anchor and attachment level");
        }

        /// <summary>
        /// Generate locators for leaf TextSelection.  Resolve starting from the root, L[0], offset = 1
        /// Verify that generated locator is null, anchorType is Unresolved
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        protected void singlepassgeneratelocators8()
        {
            BuildDualLevelTree_SinglePassGenerateLocators();
            anchor = AnchoringAPIHelpers.SubstringTextRange(txt1, 5, 15);

            if (GenerateAndCheckLocators(anchor, numExpectedLocators, numExpectedLocatorParts, out locator))
            {
                //DEBUG
                AnchoringAPIHelpers.ExportLocatorParts(locator.Parts.GetEnumerator());
                if (ResolveAndCheckLocators(null, locator, 1, a, anchorType))
                    passTest("ContentLocatorBase resolved to correct anchor and attachment level");
            }
        }

        [DisabledTestCase()]
        [Priority(0)]
        protected void singlepassgeneratelocators9()
        {
            BuildDualLevelTree_SinglePassGenerateLocators();
            anchor = AnchoringAPIHelpers.SubstringTextRange(txt1, 5, 15);

            if (GenerateAndCheckLocators(anchor, numExpectedLocators, numExpectedLocatorParts, out locator))
            {
                //DEBUG
                AnchoringAPIHelpers.ExportLocatorParts(locator.Parts.GetEnumerator());

                if (ResolveAndCheckLocators(anchor, locator, 0, a, AttachmentLevel.Full))
                    passTest("ContentLocatorBase resolved to correct anchor and attachment level");
            }
        }

        /// <summary>
        /// TO BE IMPLEMENTED
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        protected void singlepassgeneratelocators10()
        {
            BuildDualLevelTree_SinglePassGenerateLocators();
        }

        /// <summary>
        /// Generate locators for leaf TextSelection.  Resolve starting from root, with L[0] and offset = 0
        /// Verify generated locator is the root, anchorType is partial
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        protected void singlepassgeneratelocators11()
        {
            BuildMultiLevelTree_SinglePassGenerateLocators();
            anchor = AnchoringAPIHelpers.SubstringTextRange(txt1, 5, 15);

            if (GenerateAndCheckLocators(anchor, numExpectedLocators, numExpectedLocatorParts, out locator))
            {
                //DEBUG
                AnchoringAPIHelpers.ExportLocatorParts(locator.Parts.GetEnumerator());

                b.Children.Remove(d);
                a.Children.Add(d);
                a.Children.Remove(b);

                if (ResolveAndCheckLocators(anchor, locator, 0, a, AttachmentLevel.Incomplete))
                    passTest("ContentLocatorBase resolved to correct anchor and attachment level");
            }
        }

        [Priority(0)]
        protected void multipassgeneratelocators1()
        {
            BuildSimpleTree_SinglePassGenerateLocators();
            object selection1 = b;

            if (GenerateAndCheckLocators(selection1, numExpectedLocators, numExpectedLocatorParts, out locator1))
                if (ResolveAndCheckLocators(b, locator1, 0, a, anchorType, out selection2))
                    if (GenerateAndCheckLocators(selection2, numExpectedLocators, numExpectedLocatorParts, out locator2))
                        VerifyLocatorsAreEqual(locator1, locator2);
        }

        /// <summary>
        /// Generate locators for leaf.  Resolve starting from the leaf, L[0], offset = 1
        /// </summary>
        [Priority(0)]
        protected void multipassgeneratelocators2()
        {
            BuildSimpleTree_SinglePassGenerateLocators();
            object selection1 = b;

            if (GenerateAndCheckLocators(selection1, numExpectedLocators, numExpectedLocatorParts, out locator1))
                if (ResolveAndCheckLocators(selection1, locator1, 1, b, anchorType, out selection2))
                    if (GenerateAndCheckLocators(selection2, numExpectedLocators, numExpectedLocatorParts, out locator2))
                        VerifyLocatorsAreEqual(locator1, locator2);
        }

        /// <summary>
        /// Generate locators for TextBlock leaf.  Resolve starting from the root, L[0], offset = 0
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        protected void multipassgeneratelocators3()
        {
            BuildDualLevelTree_SinglePassGenerateLocators();
            TextRange selection1 = AnchoringAPIHelpers.SubstringTextRange(txt1, 5, 15);

            if (GenerateAndCheckLocators(selection1, numExpectedLocators, numExpectedLocatorParts, out locator1))
            {
                // DEBUG
                AnchoringAPIHelpers.ExportLocatorParts(locator1.Parts.GetEnumerator());

                if (ResolveAndCheckLocators(selection1, locator1, 0, a, anchorType, out selection2))
                    if (GenerateAndCheckLocators(selection2, numExpectedLocators, numExpectedLocatorParts, out locator2))
                        VerifyLocatorsAreEqual(locator1, locator2);
            }
        }

        /// <summary>
        /// Generate locators for TextBlock leaf with a parent using TextFingerprintProcessor.
        /// Resolve starting from the root, L[0] and offset = 0
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        protected void multipassgeneratelocators4()
        {
            BuildDualLevelTree_SinglePassGenerateLocators();
            TextRange selection1 = AnchoringAPIHelpers.SubstringTextRange(txt1, 5, 15);

            if (GenerateAndCheckLocators(selection1, numExpectedLocators, numExpectedLocatorParts, out locator1))
            {
                // DEBUG
                AnchoringAPIHelpers.ExportLocatorParts(locator1.Parts.GetEnumerator());

                d = new Canvas();
                a.Children.Add(d);
                a.Children.Remove(b);
                d.Children.Add(b);

                if (ResolveAndCheckLocators(selection1, locator1, 0, a, anchorType, out selection2))
                    if (GenerateAndCheckLocators(selection2, numExpectedLocators, numExpectedLocatorParts, out locator2))
                        VerifyLocatorsAreEqual(locator1, locator2);
            }
        }

        [Priority(0)]
        protected void multipassgeneratelocators5()
        {
            BuildComplexTree_BasicGenerateLocators();
            DataIdProcessor.SetDataId(a, "a");
            DataIdProcessor.SetDataId(e, "e");
            DataIdProcessor.SetDataId(f, "f");
            DataIdProcessor.SetDataId(i, "i");

            object selection1 = i;
            if (GenerateAndCheckLocators(selection1, numExpectedLocators, 3, out locator1))
            {
                // DEBUG
                // AnchoringAPIHelpers.ExportLocatorParts(locator1.GetEnumerator());

                if (ResolveAndCheckLocators(selection1, locator1, 0, a, anchorType, out selection2))
                    if (GenerateAndCheckLocators(selection2, numExpectedLocators, 3, out locator2))
                        VerifyLocatorsAreEqual(locator1, locator2);
            }
        }

        /// <summary>
        /// Generate locators for 4th level TextBlock leaf.  Resolve starting from the root, L[0], offset = 0
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        protected void multipassgeneratelocators6()
        {
            BuildMultiLevelTree_MultiPassGenerateLocators();
            TextRange selection1 = AnchoringAPIHelpers.SubstringTextRange(txt1, 5, 15);

            if (GenerateAndCheckLocators(selection1, numExpectedLocators, numExpectedLocatorParts, out locator1))
            {
                // DEBUG
                AnchoringAPIHelpers.ExportLocatorParts(locator1.Parts.GetEnumerator());

                if (ResolveAndCheckLocators(selection1, locator1, 0, a, anchorType, out selection2))
                    if (GenerateAndCheckLocators(selection2, numExpectedLocators, numExpectedLocatorParts, out locator2))
                        VerifyLocatorsAreEqual(locator1, locator2);
            }
        }

        /// <summary>
        /// Generate locators for 4th level TextBlock leaf.  Resolve starting from the root, L[0], offset = 0
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        protected void multipassgeneratelocators6_1()
        {
            BuildMultiLevelTree_MultiPassGenerateLocators();
            LocatorManager.SetSubTreeProcessorId(a, TextFingerprintProcessor.Id);
            TextRange selection1 = AnchoringAPIHelpers.SubstringTextRange(txt1, 5, 15);

            if (GenerateAndCheckLocators(selection1, numExpectedLocators, 3, out locator1))
            {
                // DEBUG
                //AnchoringAPIHelpers.ExportLocatorParts(locator1.GetEnumerator());

                if (ResolveAndCheckLocators(selection1, locator1, 0, a, anchorType, out selection2))
                    if (GenerateAndCheckLocators(selection2, numExpectedLocators, 3, out locator2))
                        VerifyLocatorsAreEqual(locator1, locator2);
            }
        }

        /// <summary>
        /// Generate locators for 4th level TextBlock leaf (empty string).  Resolve starting from the root, L[0], offset = 0
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        protected void multipassgeneratelocators6_2()
        {
            BuildMultiLevelTree_MultiPassGenerateLocators();
            LocatorManager.SetSubTreeProcessorId(a, TextFingerprintProcessor.Id);
            txt1.Text = String.Empty;
            TextRange selection1 = new TextRange(txt1.ContentStart, txt1.ContentEnd);

            if (GenerateAndCheckLocators(selection1, numExpectedLocators, 3, out locator1))
            {
                // DEBUG
                //AnchoringAPIHelpers.ExportLocatorParts(locator1.GetEnumerator());

                if (ResolveAndCheckLocators(selection1, locator1, 0, a, anchorType, out selection2))
                    if (GenerateAndCheckLocators(selection2, numExpectedLocators, 3, out locator2))
                        VerifyLocatorsAreEqual(locator1, locator2);
            }
        }

        /// <summary>
        /// Generate locators for 4th level TextBlock leaf, with tree root using TextFingerprintProcessor.  
        /// Resolve starting from the root (FP), L[0], offset = 0
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        protected void multipassgeneratelocators7()
        {
            BuildComplexTree_BasicGenerateLocators();
            g.Children.Remove(i);
            txt2 = new TextBlock();
            g.Children.Add(txt2);
            LocatorManager.SetSubTreeProcessorId(a, TextFingerprintProcessor.Id);
            TextRange selection1 = AnchoringAPIHelpers.SubstringTextRange(txt1, 5, 15);

            if (GenerateAndCheckLocators(selection1, numExpectedLocators, 2, out locator1))
            {
                // DEBUG
                //AnchoringAPIHelpers.ExportLocatorParts(locator1.GetEnumerator());

                if (ResolveAndCheckLocators(selection1, locator1, 0, a, anchorType, out selection2))
                    if (GenerateAndCheckLocators(selection2, numExpectedLocators, 2, out locator2))
                        VerifyLocatorsAreEqual(locator1, locator2);
            }
        }

        /// <summary>
        /// Generate locators for 3rd level TextBlock leaf, with tree root using TextFingerprintProcessor. 
        /// Resolve starting from the root (FP), L[0], offset = 0
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        protected void multipassgeneratelocators8()
        {
            BuildUnnamedMultiLevelTree_MultiPassGenerateLocators();
            e.Children.Remove(f);
            txt2 = new TextBlock();
            e.Children.Add(txt2);
            LocatorManager.SetSubTreeProcessorId(a, TextFingerprintProcessor.Id);
            LocatorManager.SetSubTreeProcessorId(e, TextFingerprintProcessor.Id);
            TextRange selection1 = AnchoringAPIHelpers.SubstringTextRange(txt1, 5, 15);

            if (GenerateAndCheckLocators(selection1, numExpectedLocators, 2, out locator1))
            {
                // DEBUG
                //AnchoringAPIHelpers.ExportLocatorParts(locator1.GetEnumerator());

                if (ResolveAndCheckLocators(selection1, locator1, 0, a, anchorType, out selection2))
                    if (GenerateAndCheckLocators(selection2, numExpectedLocators, 2, out locator2))
                        VerifyLocatorsAreEqual(locator1, locator2);
            }
        }

        /// <summary>
        /// Generate locators for 3rd level leaf, with tree root using TextFingerprintProcessor
        /// Resolve starting from the root (FP), L[0], offset = 0
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        protected void multipassgeneratelocators9()
        {
            BuildUnnamedMultiLevelTree_MultiPassGenerateLocators();
            LocatorManager.SetSubTreeProcessorId(a, TextFingerprintProcessor.Id);
            LocatorManager.SetSubTreeProcessorId(e, DataIdProcessor.Id);
            DataIdProcessor.SetDataId(f, "f");
            object selection1 = f;

            if (GenerateAndCheckLocators(selection1, numExpectedLocators, 1, out locator1))
            {
                // DEBUG
                //AnchoringAPIHelpers.ExportLocatorParts(locator1.GetEnumerator());

                if (ResolveAndCheckLocators(selection1, locator1, 0, a, anchorType, out selection2))
                    if (GenerateAndCheckLocators(selection2, numExpectedLocators, 1, out locator2))
                        VerifyLocatorsAreEqual(locator1, locator2);
            }
        }

        /// <summary>
        /// Generate locators for 3rd level TextBlock leaf, with tree root using TextFingerprintProcessor
        /// Resolve starting from the root (FP), L[0], offset = 0
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        protected void multipassgeneratelocators10()
        {
            BuildNamedMultiLevelTree_MultiPassGenerateLocators();
            LocatorManager.SetSubTreeProcessorId(a, TextFingerprintProcessor.Id);
            LocatorManager.SetSubTreeProcessorId(c, DataIdProcessor.Id);
            LocatorManager.SetSubTreeProcessorId(e, TextFingerprintProcessor.Id);
            TextRange selection1 = AnchoringAPIHelpers.SubstringTextRange(txt1, 5, 15);

            if (GenerateAndCheckLocators(selection1, numExpectedLocators, 3, out locator1))
            {
                // DEBUG
                //AnchoringAPIHelpers.ExportLocatorParts(locator1.GetEnumerator());

                if (ResolveAndCheckLocators(selection1, locator1, 0, a, anchorType, out selection2))
                    if (GenerateAndCheckLocators(selection2, numExpectedLocators, 3, out locator2))
                        VerifyLocatorsAreEqual(locator1, locator2);
            }
        }

    }
}

