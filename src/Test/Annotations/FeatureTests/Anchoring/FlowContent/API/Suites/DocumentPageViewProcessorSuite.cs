// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: API BVTs for Annotations with FlowContent.


using System;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Input;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Reflection;
using System.Xml;
using Proxies.System.Windows.Annotations;
using System.Windows.Annotations.Storage;
using Proxies.MS.Internal.Annotations.Anchoring;
using Annotations.Test.Framework;
using System.Collections.Generic;
using Annotations.Test.Reflection;
using System.Windows.Controls.Primitives;
using Proxies.MS.Internal.Annotations;
using System.Windows.Annotations;

namespace Avalon.Test.Annotations.Suites
{
	public class DocumentPageViewProcessorSuite : AFlowContentSuite
    {
        DocumentPageViewerProcessorTester _processorTester;

        [TestCase_Setup()]
        protected override void DoSetup()
        {
            base.DoSetup();
            _processorTester = new DocumentPageViewerProcessorTester(this, new TextViewSelectionProcessor());
        }

        #region BVT TESTS

        #region Constructors

        /// <summary>
        /// Verify: No exception.
        /// </summary>
        [Priority(0)]
        private void documentpage_ctor1()
        {
            TextViewSelectionProcessor processor = new TextViewSelectionProcessor();
            AssertNotNull("Verify processor is instantiated.", processor);
            passTest("Constructed TextViewSelectionProcessor without exception.");
        }

        #endregion Constructors

        #region GetSelectedNodes

        /// <summary>
        /// Parameters: Selection=null	
        /// Verify: ArgumentNullException.
        /// </summary>
        [Priority(0)]
        private void documentpage_getselectednodes1()
        {
            _processorTester.VerifyGetSelectedNodesFails(null, typeof(ArgumentNullException));
            passTest("GetSelectedNodes threw exception for null selection.");
        }

        /// <summary>
        /// Parameters: Selection=valid DocumentPageView
        /// Verify: The DocumentPageView.
        /// </summary>
        [Priority(0)]
        private void documentpage_getselectednodes2()
        {
            DocumentPageView pageview = ViewerBase.PageViews[0];
            _processorTester.VerifyGetSelectedNodes(pageview, new object[] { pageview });

            passTest("GetSelectedNodes succeeds for valid pageview");
        }

        #endregion GetSelectedNodes

        #region GenerateLocator

        /// <summary>
        /// Parameters: Selection=non-DocumentPageView object
        ///				StartNode=the DocumentPageView
        /// Verify: ArgumentException
        /// </summary>
        [Priority(0)]
        private void documentpage_generatelocatorparts3()
        {
            _processorTester.VerifyGenerateLocatorPartsFails(DocViewerWrapper.MakeTextRange(0, 10, 10), ViewerBase.PageViews[0], typeof(ArgumentException));
            passTest("Exception for non-DocumentPageView selection.");
        }

        /// <summary>
        /// Parameters: Page 0 is visible.
        ///				Selection=the DocumentPageView
        ///				StartNode=DocumentViewer
        /// Verify: DocumentGridLocatorPart for page 0.
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void documentpage_generatelocatorparts4()
        {
            VerifyPageIsVisible(0);
            // Subtracting two from the page offset because its calculated to be an insertion position but TextViewProcessor doesn't want it to be an insertion position
            _processorTester.VerifyGenerateLocatorParts(ViewerBase.PageViews[0], ViewerBase, DocViewerWrapper.SelectionModule.PageOffset(0) - 2, DocViewerWrapper.SelectionModule.PageLength(0));
            passTest("Locators generated for page 1.");
        }

        /// <summary>
        /// Parameters: Page 2 is visible.
        ///				Selection=DocumentPageView for page 3.
        ///				StartNode=DocumentViewer
        /// Verify: DocumentGridLocatorPart for page 2.
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void documentpage_generatelocatorparts6()
        {
            int page = 3;
            DocViewerWrapper.GoToPage(page);
            DocumentPageView pageView = ViewerBase.PageViews[0];
            AssertEquals("Verify page view is for page '" + page + "'.", page, pageView.PageNumber);
            DocViewerWrapper.PageUp();
            VerifyPageIsNotVisible(page);
            // Subtracting two from the page offset because its calculated to be an insertion position but TextViewProcessor doesn't want it to be an insertion position
            _processorTester.VerifyGenerateLocatorParts(pageView, ViewerBase, DocViewerWrapper.SelectionModule.PageOffset(2) - 2, DocViewerWrapper.SelectionModule.PageLength(2));
            passTest("Locators generated for non-visible page.");
        }

        #endregion GenerateLocator

        #region ResolveLocatorPart

        /// <summary>
        /// Parameters: ContentLocatorPart=valid LP
        ///				StartNode=DocumentPageView
        /// Verify: Null returned.
        /// </summary>
        [Priority(0)]
        private void documentpage_resolvelocatorpart4()
        {
            _processorTester.VerifyResolveLocatorPart(DocumentPageViewerProcessorTester.ValidLocatorPart(0, 10), ViewerBase.PageViews[0], null, AttachmentLevel.Unresolved);
            passTest("Null returned for valid parameters.");
        }

        #endregion ResolveLocatorPart

        #region MergeSelections

        /// <summary>
        /// Parameters: Selection1=DocumentPageView
        ///				Selection2=DocumentPageView
        /// Verify: False returned, newSelection=null
        /// </summary>
        [Priority(0)]
        private void documentpage_mergeselections2()
        {
            _processorTester.VerifyMergeSelections(ViewerBase.PageViews[0], ViewerBase.PageViews[0]);
            passTest("Returned false and null for valid selections.");
        }

        #endregion MergeSelections

        #region GetParent

        /// <summary>
        /// Parameters: Selection=DocumentPageView
        /// Verify: DocumentPageView
        /// </summary>
        [Priority(0)]
        private void documentpage_getparent3()
        {
            DocViewerWrapper.SetSelection(2, 100, 92);
            _processorTester.VerifyGetParent(ViewerBase.PageViews[0], ViewerBase.PageViews[0]);
            passTest("DocumentPageView returned for valid selection.");
        }

        #endregion GetParent

        #region GetAnchorPoint

        /// <summary>
        /// Parameters: Selection=DocumentPageView
        /// Verify: (0,0) returned
        /// </summary>
        [Priority(0)]
        private void documentpage_getanchorpoint2()
        {
            DocViewerWrapper.SetSelection(0, 10, 50);
            _processorTester.VerifyGetAnchorPoint(ViewerBase.PageViews[0], new Point(double.NaN, double.NaN));
            passTest("(NaN,NaN) returned for valid selection.");
        }

        #endregion GetAnchorPoint

        #endregion BVT TESTS

        #region PRIORITY TESTS

        #region GetSelectedNodes

        /// <summary>
        /// Parameters: Selection=not a DocumentPageView.	
        /// Verify: ArgumentException.
        /// </summary>
        [Priority(1)]
        private void documentpage_getselectednodes3()
        {
            _processorTester.VerifyGetSelectedNodesFails(new Grid(), typeof(ArgumentException));
            passTest("Exception for non-ITextRange.");
        }

        #endregion

        #region GenerateLocatorParts

        /// <summary>
        /// Parameters: Selection=null
        ///				startNode=DocumentPageView
        /// Verify: ArgumentNullException
        /// </summary>
        [Priority(1)]
        private void documentpage_generatelocatorparts1()
        {
            _processorTester.VerifyGenerateLocatorPartsFails(null, ViewerBase.PageViews[0], typeof(ArgumentNullException));
            passTest("Exception for null selection.");
        }

        /// <summary>
        /// Parameters: Selection=DocumentPageView
        ///				startNode=null
        /// Verify: ArgumentNullException
        /// </summary>
        [Priority(1)]
        private void documentpage_generatelocatorparts2()
        {
            _processorTester.VerifyGenerateLocatorPartsFails(ViewerBase.PageViews[0], null, typeof(ArgumentNullException));
            passTest("Exception for null startNode.");
        }

        /// <summary>
        /// Parameters: Page 3 is visible.
        ///				Selection= DocumentPageView 3
        ///				StartNode= DocumentViewer	
        /// Verify: DocumentPageViewLocatorPart for page 3.
        /// </summary>
        [Priority(1)]
        private void documentpage_generatelocatorparts5()
        {
            // Subtracting two because the page offset is calculated to be at an insertion position but TextViewSelectionProcessor doesn't do that
            int expectedStartOffset = DocViewerWrapper.SelectionModule.PageOffset(3) - 2;
            int expectedEndOffset = expectedStartOffset + DocViewerWrapper.SelectionModule.PageLength(3) + 2;

            DocViewerWrapper.GoToPage(3);
            DocumentPageView pageView = ViewerBase.PageViews[0];
            _processorTester.VerifyGenerateLocatorParts(pageView, ViewerBase, expectedStartOffset, expectedEndOffset);
            passTest("Locators generated for visible non-edge page.");
        }


        #endregion

        #region ResolveLocatorPart

        /// <summary>
        /// Parameters: ContentLocatorPart=null
        ///				StartNode=null	
        /// Verify: ArgumentNullException.
        /// </summary>
        [Priority(1)]
        private void documentpage_resolvelocatorpart1()
        {
            _processorTester.VerifyResolveLocatorPartFails(null, null, typeof(ArgumentNullException));
            passTest("ArgumentNullException for null parameters.");
        }

        /// <summary>
        /// Parameters: ContentLocatorPart=valid ContentLocatorPart
        ///             StartNode=non-DocumentPageView DependencyObject	
        /// Verify: Null returned.
        /// </summary>
        [Priority(1)]
        private void documentpage_resolvelocatorpart2()
        {
            _processorTester.VerifyResolveLocatorPart(DocumentPageViewerProcessorTester.ValidLocatorPart(1, 2), new FlowDocument(), null, AttachmentLevel.Unresolved);
            passTest("Null for non-DocumentPageView startNode.");
        }

        /// <summary>
        /// Parameters: ContentLocatorPart=invalid ContentLocatorPart
        ///				StartNode= DocumentPageView
        /// Verify: Null returned.
        /// </summary>
        [Priority(1)]
        private void documentpage_resolvelocatorpart3()
        {
            _processorTester.VerifyResolveLocatorPart(new ContentLocatorPart(new XmlQualifiedName("type", "nsp")), ViewerBase.PageViews[0], null, AttachmentLevel.Unresolved);
            passTest("Null for valid args.");
        }


        #endregion ResolveLocatorPart

        #region MergeSelections

        /// <summary>
        /// Parameters: Selection1=null
        ///				Selection2=null	
        /// Verify: False returned, newSelection=null
        /// </summary>
        [Priority(1)]
        private void documentpage_mergeselections1()
        {
            _processorTester.VerifyMergeSelections(null, null, false, null);
            passTest("Returned false for null selections.");
        }

        /// <summary>
        /// Parameters: Selection1=DocumentViewer
        ///             Selection2=ListBox
        /// Verify: False returned, newSelection=null
        /// </summary>
        [Priority(1)]
        private void documentpage_mergeselections3()
        {
            _processorTester.VerifyMergeSelections(ViewerBase, new ListBox());
            passTest("Returned false dv and ListBox.");
        }

        /// <summary>
        /// Parameters: Selection1=DocumentPageView
        ///             Selection2=non-DocumentPageView
        /// Verify: False returned, newSelection=null
        /// </summary>
        [Priority(1)]
        private void documentpage_mergeselections4()
        {
            _processorTester.VerifyMergeSelections(ViewerBase.PageViews[0], new FixedDocument());
            passTest("Returned false for DocumentPageView and FixedDocument.");
        }

        /// <summary>
        /// Parameters: Selection1=non-DocumentPageView
        ///             Selection2=DocumentPageView
        /// Verify: False returned, newSelection=null
        /// </summary>
        [Priority(1)]
        private void documentpage_mergeselections5()
        {
            _processorTester.VerifyMergeSelections(new Button(), ViewerBase.PageViews[0]);
            passTest("Returned false for Button and DocumentPageView.");
        }

        #endregion MergeSelections

        #region GetParent

        /// <summary>
        /// Parameters: Selection=null	
        /// Verify: ArgumentNullException
        /// </summary>
        [Priority(1)]
        private void documentpage_getparent1()
        {
            _processorTester.VerifyGetParentFails(null, typeof(ArgumentNullException));
            passTest("Exception for null selection.");
        }

        /// <summary>
        /// Parameters: Selection=string
        /// Verify: ArgumentException
        /// </summary>
        [Priority(1)]
        private void documentpage_getparent2()
        {
            _processorTester.VerifyGetParentFails("hello world", typeof(ArgumentException));
            passTest("Exception for non-DocumentPageView selection.");
        }

        #endregion GetParent

        #region GetAnchorPoint

        /// <summary>
        /// Parameters: Selection=null	
        /// Verify: ArgumentNullException
        /// </summary>
        [Priority(1)]
        private void documentpage_getanchorpoint1()
        {
            _processorTester.VerifyGetAnchorPointFails(null, typeof(ArgumentNullException));
            passTest("ArgumentNullException for null selection.");
        }

        /// <summary>
        /// Parameters: Selection=TextBox
        /// Verify: ArgumentException
        /// </summary>
        [Priority(1)]
        private void documentpage_getanchorpoint3()
        {
            _processorTester.VerifyGetAnchorPointFails(new TextBox(), typeof(ArgumentException));
            passTest("ArgumentException for TextBox.");
        }

        #endregion GetAnchorPoint

        #endregion PRIORITY TESTS

   	}
}

