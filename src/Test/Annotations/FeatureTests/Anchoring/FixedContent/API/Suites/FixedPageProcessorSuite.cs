// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.IO;
using System.Windows.Input;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Reflection;


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
using Proxies.MS.Internal.Annotations.Anchoring;

using Annotations.Test.Framework;
using System.Xml;
using Annotations.Test;

/// <summary>
/// FixedPageProcessorSuite provides the implementation for the API tests for 
/// FixedPageProcessor's public methods.
/// </summary>
namespace Avalon.Test.Annotations.Suites
{
	public class FixedPageProcessorSuite : AFixedPageProcessorSuite
    {
        #region BVT TESTS

        #region Constructors

        /// <summary>
        /// Parameters: manager = valid, non-null LocatorManager
        /// Verify: No exceptions thrown.
        /// </summary>
        [Priority(0)]
        private void fixedpage_ctor1()
        {
            try
            {
                FixedPageProcessor fp = new FixedPageProcessor(this.LocatorManager);
            }
            catch (Exception exp)
            {
                failTest("Unexpected exception thrown in ctor1:  \n" + exp.ToString());
            }
            passTest("No exceptions expected.");
        }

        #endregion Constructors

        #region GenerateLocator

        /// <summary>
        /// Parameters: node = PathNode generated from FixedPage (visible)	
        /// Verify: ContentLocator with one ContentLocatorPart is generated continueGenerating = true
        /// </summary>
        [Priority(0)]
        private void fixedpage_generatelocator3()
        {
            bool continueGenerating;
            ContentLocator result = fixedPageProcessor.GenerateLocator(new PathNode(GetDocumentPageView(0)), out continueGenerating);

            AssertNotNull("Verify locators returned.", result);
            AssertEquals("Verify 1 ContentLocatorPart.", 1, result.Parts.Count);
            Assert("Out parameter should be true.", continueGenerating);

            passTest("Verified result of GenerateLocator");
        }

        #endregion GenerateLocator

        #region ResolveLocatorPart

        /// <summary>
        /// Parameters: locatorPart = valid ContentLocatorPart startNode = visible 
        /// Verify: FixedPage described by locatorPart	Node described by locatorPart continueResolving = true
        /// </summary>
        [Priority(0)]
        private void fixedpage_resolvelocatorpart6()
        {
            DocumentPageView page0 = GetDocumentPageView(0);

            // Create valid ContentLocatorPart for page 0.
            bool continueGenerating;
            ContentLocator locatorSequence = fixedPageProcessor.GenerateLocator(new PathNode(page0), out continueGenerating);
            Assert("Verify ContentLocatorPart generated successfully.", locatorSequence != null && locatorSequence.Parts.Count == 1);

            bool continueResolving;
            DependencyObject result = fixedPageProcessor.ResolveLocatorPart(locatorSequence.Parts[0], page0, out continueResolving);
            AssertNotNull("Verify ContentLocatorPart resolved.", result);
            Assert("Verify ContentLocatorPart resolved to correct location.", result == GetFixedPage(0));
            Assert("Out parameter should be true.", continueResolving);

            passTest("Verified that ResolveLocatorPart works for simple positive case.");
        }

        #endregion ResolveLocatorPart

        #endregion BVT TESTS

        #region PRIORITY TESTS

        FixedPageProcessorTester processorTester;

        [TestCase_Setup()]
        protected override void DoSetup()
        {
            base.DoSetup();

            // Setup DocumentViewer so that first 2 pages are visible.
            DocViewerWrapper.SetZoom(50);
            processorTester = new FixedPageProcessorTester(this, fixedPageProcessor);
        }

        #region Constructor Tests

        /// <summary>
        /// Parameters: manager = null	
        /// Verify: ArgumentNullException thrown
        /// </summary>
        [Priority(1)]
        private void fixedpage_ctor2()
        {
            try
            {
                FixedPageProcessor fp = new FixedPageProcessor(null);
            }
            catch (System.ArgumentException)
            {
                passTest("ArgumentNullException correctly thrown.");
            }
            catch (Exception exp)
            {
                failTest("ArgumentNullException expected, not:  \n" + exp.ToString());
            }
            failTest("ArgumentNullException expected but not thrown");
        }

        #endregion Constructor Tests

        #region GenerateLocator Tests

        /// <summary>
        /// Parameters: node = null	
        /// Verify: ArgumentNullException thrown
        /// </summary>
        [Priority(1)]
        private void fixedpage_generatelocator1()
        {
            processorTester.VerifyGenerateLocatorFails(null, typeof(ArgumentNullException));
            passTest("Exception for null node.");
        }

        /// <summary>
        /// Parameters: node = not generated from a FixedPage (i.e. Canvas)	
        /// Verify: Null  is returned, continueGenerating = true
        /// </summary>
        [Priority(1)]
        private void fixedpage_generatelocator2()
        {
            processorTester.VerifyGenerateLocator(new PathNode(new Canvas()), true, -1);
            passTest("Null correctly returned");
        }

        /// <summary>
        /// Parameters: node = PathNode generated from FixedPage that was visible and is now non-visible.
        ///	Verify: continueGenerating=true
        /// </summary>
        [Priority(1)]
        private void fixedpage_generatelocator3_2()
        {
            DocViewerWrapper.SetZoom(100);
            DocViewerWrapper.WholePageLayout();
            VerifyPageIsVisible(0);
            DocumentPageView pageView = GetDocumentPageView(0);
            DocViewerWrapper.PageDown(2);
            VerifyPageIsNotVisible(0);
            processorTester.VerifyGenerateLocator(new PathNode(pageView), true);
            passTest("ArgumentException for non-visible page.");
        }

        /// <summary>
        /// Parameters: node = PathNode generated from visible FixedPage not at start of document.
        /// Verify: ContentLocator with one ContentLocatorPart for page is generated, continueGenerating=true
        /// </summary>
        [Priority(1)]
        private void fixedpage_generatelocator4()
        {
            DocViewerWrapper.SetZoom(100);
            DocViewerWrapper.WholePageLayout();
            DocViewerWrapper.PageDown();
            VerifyPageIsVisible(1);
            processorTester.VerifyGenerateLocator(new PathNode(GetDocumentPageView(1)), true, 1);
            passTest("No exception for valid DPV");
        }

        /// <summary>
        /// Parameters: node = PathNode generated from visible FixedPage at the start of the document.
        /// Verify: ContentLocator with one ContentLocatorPart for page is generated, continueGenerating=true
        /// </summary>
        [Priority(1)]
        private void fixedpage_generatelocator5()
        {
            processorTester.VerifyGenerateLocator(new PathNode(GetDocumentPageView(0)), true, 0);
            passTest("Verified number of ContentLocatorParts for visible page at beginning of document.");
        }

        /// <summary>
        /// Parameters: node = PathNode generated from visible FixedPage at the end of the document.
        /// Verify: ContentLocator with one ContentLocatorPart for page is generated, continueGenerating=true
        /// </summary>
        [Priority(1)]
        private void fixedpage_generatelocator6()
        {
            DocViewerWrapper.SetZoom(100);
            DocViewerWrapper.WholePageLayout();
            DocViewerWrapper.PageDown(5);
            VerifyPageIsVisible(4);
            processorTester.VerifyGenerateLocator(new PathNode(GetDocumentPageView(4)), true, 4);
            passTest("GenerateLocator for last page.");
        }

        #endregion GenerateLocator Tests

        #region ResolveLocatorPart Tests

        /// <summary>
        /// Parameters: locatorPart = null
        ///				startNode = valid visible FixedPage	
        /// Verify: ArgumentNullException thrown
        /// </summary>
        [Priority(1)]
        private void fixedpage_resolvelocatorpart1()
        {
            Assert("Verify page 0 is visible.", DocViewerWrapper.LastVisiblePage >= 0);
            processorTester.VerifyResolveLocatorPartFails(null, GetDocumentPageView(0), typeof(ArgumentNullException));
            passTest("Exception for null locator part.");
        }

        /// <summary>
        /// Parameters: locatorPart = valid ContentLocatorPart
        ///				startNode = null	
        /// Verify: ArgumentNullException thrown
        /// </summary>
        [Priority(1)]
        private void fixedpage_resolvelocatorpart2()
        {
            ContentLocatorPart goodLP = new ContentLocatorPart(new XmlQualifiedName("PageNumber", "http://schemas.microsoft.com/windows/annotations/2003/11/base"));
            goodLP.NameValuePairs.Add("Value", "0");

            processorTester.VerifyResolveLocatorPartFails(goodLP, null, typeof(ArgumentNullException));
            passTest("Exception for null StartNode.");
        }

        /// <summary>
        /// Parameters: locatorPart = incorrect type (type is not PageNumber or namespace is not base schema namespace) 
        ///				startNode = valid visible FixedPageProxy	
        /// Verify: ArgumentException thrown
        /// </summary>
        [Priority(1)]
        private void fixedpage_resolvelocatorpart3()
        {
            ContentLocatorPart badLP = new ContentLocatorPart(new XmlQualifiedName("badType", "http://schemas.microsoft.com/windows/annotations/2003/11/base"));
            badLP.NameValuePairs.Add("Value", "1");

            DocViewerWrapper.SetZoom(100);
            DocViewerWrapper.WholePageLayout();
            DocViewerWrapper.PageDown();
            VerifyPageIsVisible(1);
            processorTester.VerifyResolveLocatorPartFails(badLP, GetDocumentPageView(1), typeof(ArgumentException));
            passTest("Exception for bad LP.");
        }

        /// <summary>
        /// Parameters: locatorPart = valid ContentLocatorPart for page N
        ///				startNode = FixedPage for page N+1
        /// Verify: Null returned, continueResolving = false
        /// </summary>
        [Priority(1)]
        private void fixedpage_resolvelocatorpart4()
        {
            DocViewerWrapper.SetZoom(100);
            DocViewerWrapper.WholePageLayout();
            DocViewerWrapper.PageDown();
            VerifyPageIsVisible(1);
            processorTester.VerifyResolveLocatorPart(0, GetDocumentPageView(1), null, false);
            passTest("Null returned mismatched LP and StartNode.");
        }

        /// <summary>
        /// Parameters: locatorPart = valid ContentLocatorPart
        ///				startNode = valid DependencyObject located higher up in the app tree	
        /// Verify: Null returned, continueResolving = true
        /// </summary>
        [Priority(1)]
        private void fixedpage_resolvelocatorpart5()
        {
            processorTester.VerifyResolveLocatorPart(0, ViewerBase, null, true);
            passTest("Verified behavior for non-FixedPage StartNode.");
        }

        /// <summary>
        /// 
        /// Parameters: locatorPart = valid ContentLocatorPart
        ///				startNode = non-visible, loaded FixedPage described by ContentLocatorPart	
        /// Verify: Null returned, continueResolving = true
        /// </summary>
        [Priority(1)]
        private void fixedpage_resolvelocatorpart6_1()
        {
            ContentLocatorPart goodLP = new ContentLocatorPart(new XmlQualifiedName("PageNumber", "http://schemas.microsoft.com/windows/annotations/2003/11/base"));
            goodLP.NameValuePairs.Add("Value", "1");

            DocViewerWrapper.SetZoom(100);
            DocViewerWrapper.WholePageLayout();
            DocViewerWrapper.PageDown();
            VerifyPageIsVisible(1);
            DocumentPageView page1 = GetDocumentPageView(1);
            DocViewerWrapper.PageDown();
            VerifyPageIsNotVisible(1);
            processorTester.VerifyResolveLocatorPart(goodLP, page1, true, null);
            passTest("Exception for non-visible page.");
        }

        /// <summary>
        /// Parameters: locatorPart = valid ContentLocatorPart
        ///				startNode = visible FixedPageProxy described by locatorPart	
        /// Verify: Null returned, continueResolving = true
        /// </summary>
        [Priority(1)]
        private void fixedpage_resolvelocatorpart8()
        {
            processorTester.VerifyResolveLocatorPart(0, new FixedPageProxy(ViewerBase, 0), null, true);
            passTest("Verified behavior for StartNode=FixedPageProxy.");
        }

        /// <summary>
        /// Parameters: locatorPart = valid ContentLocatorPart (for selection at start of document)
        ///				startNode = visible FixedPage described by ContentLocatorPart	
        /// Verify: StartNode returned, continueResolving = true
        /// </summary>
        [Priority(1)]
        private void fixedpage_resolvelocatorpart9()
        {
            processorTester.VerifyResolveLocatorPart(0, GetDocumentPageView(0), GetFixedPage(0), true);
            passTest("Verified behavior for first page.");
        }

        /// <summary>
        /// Parameters: locatorPart = valid ContentLocatorPart (for selection at end of document)
        ///				startNode = visible FixedPage described by locatorPart	
        /// Verify: StartNode returned, continueResolving = true
        /// </summary>
        [Priority(1)]
        private void fixedpage_resolvelocatorpart10()
        {
            DocViewerWrapper.SetZoom(100);
            DocViewerWrapper.WholePageLayout();
            DocViewerWrapper.GoToEnd();
            processorTester.VerifyResolveLocatorPart(4, GetDocumentPageView(4), GetFixedPage(4), true);
            passTest("ResolveLocatorPart works for last page.");
        }

        #endregion ResolveLocatorPart Tests

        #region PreProcessNode Tests

        /// <summary>
        /// Parameters: node = null	
        /// Verify: ArgumentNullException is thrown
        /// </summary>
        [Priority(1)]
        private void fixedpage_preprocessnode1()
        {
            processorTester.VerifyPreProcessNodeFails(null, typeof(ArgumentNullException));
            passTest("ArgumentNullException correctly thrown.");
        }

        /// <summary>
        /// Parameters: node = FixedPage with N annotations set on it	
        /// Verify: List with all annotations (count = N) set on the page is loaded, calledProcessAnnotations = true
        /// </summary>
        [Priority(1)]
        private void fixedpage_preprocessnode2()
        {
            DocViewerWrapper.PageDown();
            Assert("Create annotation 1.", MakeAnnotation(1, 0, 10));		// Annotation at beginning.
            Assert("Create annotation 2.", MakeAnnotation(1, 3894, 115));	// Annotation at end.
            Assert("Create annotation 3.", MakeAnnotation(1, 16, 7));
            Assert("Create annotation 4.", MakeAnnotation(1, 2, 2));
            Assert("Create annotation 5.", MakeAnnotation(1, 0, 4009));		// Whole page.

            processorTester.VerifyPreProcessNode(GetDocumentPageView(1), true, 5);

            passTest("Preprocessnode for N annotations.");
        }

        /// <summary>
        /// Parameters: node = FixedPage with no annotations set on it	
        /// Verify: 0 AttachedAnnotations returned, calledProcessAnnotations = true
        /// </summary>
        [Priority(1)]
        private void fixedpage_preprocessnode3()
        {
            processorTester.VerifyPreProcessNode(GetDocumentPageView(0), true, 0);
            passTest("Preprocessnode for 0 annotations.");
        }

        /// <summary>
        /// Parameters: node = non-FixedPage node	
        /// Verify: Null is returned, calledProcessAnnotations = false
        /// </summary>
        [Priority(1)]
        private void fixedpage_preprocessnode4()
        {
            processorTester.VerifyPreProcessNode(ViewerBase, false, -1);
            passTest("Preprocessnode for non-FixedPage node.");
        }

        /// <summary>
        /// Parameters: node = FixedPage with 1 annotation set on it	
        /// Verify: List with the annotation (count = 1) set on the page is loaded, calledProcessAnnotations = true
        /// </summary>
        [Priority(1)]
        private void fixedpage_preprocessnode5()
        {
            Assert("Make annotation 1.", MakeAnnotation(0, 16, 7));
            processorTester.VerifyPreProcessNode(GetDocumentPageView(0), true, 1);
            passTest("Preprocessnode for page with 1 annotation.");
        }

        /// <summary>
        /// Parameters: node = FixedPageProxy for a FixedPage with annotations set on it	
        /// Verify: Null is returned, calledProcessAnnotations = false.
        /// </summary>
        [Priority(1)]
        private void fixedpage_preprocessnode6()
        {
            MakeAnnotation(0, 50, 25);
            processorTester.VerifyPreProcessNode(new FixedPageProxy(ViewerBase, 0), false, -1);
            passTest("Preprocessnode for FixedPageProxy node.");
        }

        /// <summary>
        /// Parameters: Annotation spans pages N and N+1 node=FixedPage N	
        /// Verify: 1 AttachedAnnotation returned, calledProcessAnnotations=true
        /// </summary>
        [Priority(1)]
        private void fixedpage_preprocessnode7()
        {
            DocViewerWrapper.SetZoom(10.0);
            VerifyPageIsVisible(1);
            VerifyPageIsVisible(2);
            MakeAnnotation(1, 3900, 100); // Annotation spans pages 1 and 2.            
            processorTester.VerifyPreProcessNode(GetDocumentPageView(1), true, 1);
            passTest("1st page of multipage.");
        }

        /// <summary>
        /// Parameters: Annotation spans pages N and N+1 node=FixedPage N+1
        /// Verify: 1 AttachedAnnotation returned, calledProcessAnnotations=true
        /// </summary>
        [Priority(1)]
        private void fixedpage_preprocessnode7_1()
        {
            DocViewerWrapper.SetZoom(10.0);
            VerifyPageIsVisible(1);
            VerifyPageIsVisible(2);
            MakeAnnotation(1, 3900, 500); // Annotation spans pages 1 and 2.            
            processorTester.VerifyPreProcessNode(GetDocumentPageView(2), true, 1);
            passTest("2nd page of multipage.");
        }

        /// <summary>
        /// Parameters: Node=non-visible FixedPage with 1 annotation.	
        /// Verify: 0 AttachedAnnotations returned.
        /// </summary>
        private void fixedpage_preprocessnode8()
        {
            DocViewerWrapper.WholePageLayout();
            DocViewerWrapper.PageDown();
            DocumentPageView page1 = GetDocumentPageView(1);
            CreateAnnotation(1, 500, 45);
            DocViewerWrapper.PageUp();
            VerifyPageIsNotVisible(1);
            processorTester.VerifyPreProcessNode(page1, false, -1);
            passTest("Exception for non-visible page.");
        }

        /// <summary>
        /// Parameters: Node=last FixedPage in document with N annotations.	
        /// Verify: N AttachedAnnotations returned, calledProcessAnnotations=true
        /// </summary>
        [Priority(1)]
        private void fixedpage_preprocessnode9()
        {
            AsyncTestScript script = new AsyncTestScript();
            DocViewerWrapper.PageLayout(5);
            VerifyPageIsVisible(4);

            CreateAnnotation(4, 0, 100);		// Annotate start of page.
            CreateAnnotation(4, 1150, 75);	// Annotate end of page.
            CreateAnnotation(4, 500, 12);	// Annotate middle of page.
            processorTester.VerifyPreProcessNode(GetDocumentPageView(4), true, 3);
            passTest("All pages visible.");
        }

        /// <summary>
        /// Parameters: Annotations on page N, Node=FixedPage N+1	
        /// Verify: Null returned, calledProcessAnnotations=true
        /// </summary>
        /// <returns></returns>
        [Priority(1)]
        private void fixedpage_preprocessnode10()
        {
            Assert("Annotate page 1.", MakeAnnotation(1, 1500, 200));
            processorTester.VerifyPreProcessNode(GetDocumentPageView(0), true, 0);
            passTest("Preprecessnode for page N with annotation on N+1.");
        }

        #endregion PreProcessNode Tests

        #region GetLocatorPartTypes Tests

        [Priority(1)]
        private void fixedpage_getlocatorparttypes1()
        {
            processorTester.VerifyGetLocatorPartTypes(new XmlQualifiedName[] { new XmlQualifiedName("PageNumber", "http://schemas.microsoft.com/windows/annotations/2003/11/base") });
            passTest("Verified ContentLocatorPart types.");
        }

        #endregion GetLocatorPartTypes Tests

        class FixedPageProcessorTester : SubtreeProcessorTester
        {
            public FixedPageProcessorTester(TestSuite suite, SubTreeProcessor processor)
                : base(suite, processor) { }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="node"></param>
            /// <param name="expectedContinueGenerating"></param>
            /// <param name="pageNumber">-1 if null is expected.</param>
            public void VerifyGenerateLocator(PathNode node, bool expectedContinueGenerating, int pageNumber)
            {
                bool continueGenerating;
                ContentLocator lps = processor.GenerateLocator(node, out continueGenerating);
                suite.AssertEquals("Verify out parameter 'continueGenerating'.", expectedContinueGenerating, continueGenerating);

                if (pageNumber == -1)
                    suite.AssertNull("Verify return value.", lps);
                else
                {
                    suite.AssertNotNull("Verify return value not null.", lps);
                    suite.AssertEquals("Verify only 1 of ContentLocatorPart.", 1, lps.Parts.Count);
                    suite.AssertEquals("Verify ContentLocatorPart PageNumber.", pageNumber.ToString(), AnnotationTestHelper.GetOnlyLocatorPart(lps.Parts).NameValuePairs["Value"]);
                }
            }

            public void VerifyResolveLocatorPart(int locatorPartPageNum, DependencyObject startNode, DependencyObject expectedResult, bool expectedContinueResolving)
            {
                ContentLocatorPart lp = new ContentLocatorPart(new XmlQualifiedName("PageNumber", "http://schemas.microsoft.com/windows/annotations/2003/11/base"));
                lp.NameValuePairs.Add("Value", locatorPartPageNum.ToString());
                VerifyResolveLocatorPart(lp, startNode, expectedContinueResolving, expectedResult);
            }
        }

        #endregion PRIORITY TESTS

    }
}	

