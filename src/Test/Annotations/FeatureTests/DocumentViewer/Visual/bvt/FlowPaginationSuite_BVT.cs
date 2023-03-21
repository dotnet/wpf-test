// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 

using Annotations.Test;
using Annotations.Test.Framework;
using System;
using System.Windows;
using System.Windows.Documents;

namespace Avalon.Test.Annotations.BVTs
{
	/// <summary>
	/// Pagination tests for Flow content.
	/// </summary>
	public class FlowPagintationSuite_BVT : AFlowPaginationSuite
	{
        #region Basic Behavior Tests

        /// <summary>
        /// Action	                                        Verify
        /// 1. Annotation with anchor that spans 2 pages.	
        /// 2. Change pagination so anchor is on 1 page.	Verify Annotation.
        /// </summary>
        private void flow_basic9()
        {
            ISelectionData selection = new MultiPageSelectionData(2, PagePosition.End, -52, 3, PagePosition.Beginning, 100);
            ViewAsTwoPages();
            Script.Add("GoToPageRange", new object[] { 2, 3 });
            Script.Add("CreateAnnotation", new object[] { selection });
            Script.Add("VerifyAnnotation", new object[] { GetText(selection) });
            Script.Add("SetZoom", new object[] { 50 });
            Script.Add("GoToSelection");
            Script.Add("VerifyAnnotation", new object[] { GetText(selection) });
            RunScript(Script);
        }

        /// <summary>
        /// Action	                                                            Verify
        /// 1. Annotation is completely contained on 1 page.	
        /// 2. Change pagination so that annotation now spans a page break.	    Verify annotation.
        /// </summary>
        private void flow_basic15()
        {            
            ViewerBase.Document = SinglePageFlowDocument();
            DispatcherHelper.DoEvents();

            string expectedAnchor = SinglePageText().Substring(200, SinglePageText().Length - 400);
            Script.Add("CreateAnnotation", new object[] { 0, PagePosition.Beginning, 200, 0, PagePosition.End, -200 });
            Script.Add("VerifyAnnotation", new object[] { expectedAnchor });
            Script.Add("SetZoom", new object[] { 175 });
            Script.Add("PageLayout", new object[] { 2 }); // 
            Script.Add("PageLayout", new object[] { 2 });
            Script.Add("VerifyPagesAreVisible", new object[] { new int[] { 0, 1 } });
            Script.Add("VerifyAnnotation", new object[] { expectedAnchor });
            RunScript(Script);
        }

        /// <summary>
        /// Action	                                                            Verify
        /// 1. Annotation is completely contained on 1 page.
        /// 2.  Incrementally zoom in until annotation spans a page break.      No Crash.
        /// </summary>
        private void flow_basic16()
        {
            SetZoom(180);
            CreateAnnotation(new MultiPageSelectionData(0, PagePosition.Middle, 0, 0, PagePosition.End, -10));
            for (int i = 0; i < 3; i++)
                DocViewerWrapper.ZoomIn();
            GoToFirstPage();
            VerifyNumberOfAttachedAnnotations(1);
            GoToPage(1);
            VerifyNumberOfAttachedAnnotations(1);
            passTest("Verified reflowing annotation onto 2 pages using incremental zoom.");
        }

        /// <summary>
        /// Action	                                                            Verify
        /// 1. Annotation is across page break.
        /// 2. Incrementally zoom out until annotation is on single page.      No Crash.
        /// </summary>
        private void flow_basic17()
        {
            GoToPage(2);
            CreateAnnotation(new MultiPageSelectionData(2, PagePosition.End, -50, 3, PagePosition.Beginning, 50));
            for (int i = 0; i < 3; i++)
                DocViewerWrapper.ZoomOut();
            GoToPage(2);
            VerifyNumberOfAttachedAnnotations(1);
            PageDown();
            VerifyNumberOfAttachedAnnotations(0);
            passTest("Verified reflowing annotation from 2 pages using incremental zoom.");
        }
                             
        #endregion Basic Behavior Tests

        #region Visibility Tests

        /// <summary>
        /// Action	                                                        Verify
        /// 1. Annotate page that is non-visible. 	
        /// 2. Change pagination so that annotation moves to visible page.	Verify annotation is correct.
        /// </summary>
        /// <param name="Script"></param>
        private void flow_visibility1()
        {
            Script.Add("GoToPage", new object[] { 3 });
            Script.Add(DocViewerWrapper, "SetSelection", SelectionData(SelectionType.Page3_0_to_53));
            Script.Add("PageUp");
			Script.Add("CreateAnnotation", new object[] { true });
			Script.Add("VerifyAnnotations", new object[] { 0, null }); // Verify no annotations.
			Script.Add("SetZoom", new object[] { 50 });
			EnsureWholePage(ref Script);
			Script.Add("GoToSelection");
			Script.Add("VerifyAnnotation", new object[] { ExpectedSelectedText(SelectionType.Page3_0_to_53) });
			RunScript(Script);
        }

        /// <summary>
        /// Action	                                                                    Verify
        /// 1. Annotation for selection from page N to N+1, where N+1 is non-visible.	Verify annotation for page N.
        /// 2. Change pagination so that both pages are visible.	                    Verify annotation for pages N and N+1
        /// 3. Change pagination so that only page N is visible.	                    Verify annotation for page N.
        /// </summary>
        /// <param name="Script"></param>
        private void flow_visibility5()
        {
            Script.Add("GoToPage", new object[] { 2 });            
            Script.Add(DocViewerWrapper, "SetSelection", SelectionData(SelectionType.Page2_to_Page3));
            Script.Add("CreateAnnotation", new object[] { true });
            Script.Add("VerifyAnnotation", new object[] { ExpectedSelectedText(SelectionType.Page2_End) }); 
            Script.Add("ChangeWindowHeight", new object[] { 100 });
            EnsureWholePage(ref Script);
            Script.Add("VerifyAnnotation", new object[] { ExpectedSelectedText(SelectionType.Page2_to_Page3) });
            Script.Add("ChangeWindowHeight", new object[] { -100 });
            EnsureWholePage(ref Script);
            Script.Add("VerifyAnnotation", new object[] { ExpectedSelectedText(SelectionType.Page2_End) });
            RunScript(Script);
        }

        #endregion Visibility Tests

        #region Repagination Mode Tests

        /// <summary>
        /// Action	                                        Verify
        /// 1. Set number of pages displayed to 1.	        Should have 1 big page.
        /// 2. Annotate visible page in multiple places.	Verify annotations created.
        /// 3. Set number of pages displayed to 5.	        Should have original content spread across multiple pages.  Verify annotations are displayed correctly.
        /// </summary>
        /// <param name="Script"></param>
        private void flow_mode11()
        {
            string[] anchors = new string[] { 
                                                ExpectedSelectedText(SelectionType.Page0_805_to_825),                                
                                                ExpectedSelectedText(SelectionType.Page0_10_to_30),
                                                ExpectedSelectedText(SelectionType.Page0_574_to_599),
                                                ExpectedSelectedText(SelectionType.Page0_250_to_215),
                                                ExpectedSelectedText(SelectionType.Page0_1200_to_1215)};

            Script.AddStatus("Setup:");
            Script.Add("CreateAnnotation", SelectionData(SelectionType.Page0_805_to_825));
            Script.Add("CreateAnnotation", SelectionData(SelectionType.Page0_10_to_30));            
            Script.Add("CreateAnnotation", SelectionData(SelectionType.Page0_574_to_599));
            Script.Add("CreateAnnotation", SelectionData(SelectionType.Page0_250_to_215));            
            Script.Add("CreateAnnotation", SelectionData(SelectionType.Page0_1200_to_1215));
            Script.Add("VerifyAnnotations", new object[] { anchors });

            Script.AddStatus("Re-paginate to 5 pages:");
            Script.Add("SetZoom", new object[] { 165 });
            Script.Add("PageLayout", new object[] { 6 });
            Script.Add("VerifyAnnotations", new object[] { anchors });

            RunScript(Script);
        }

        /// <summary>
        /// Action	                                        Verify
        /// 1. Set number of pages displayed to 5.	        5 pages visible.
        /// 2. Create 1 annotation per visible page.	    All annotations created successfully.
        /// 3. Set number of pages displayed to 1.	        Verify that annotations are relocated correctly.  
        /// 4. Scroll down.	                                Verify that annotations that were pushed to non-visible pages are correct.
        /// </summary>
        /// <param name="Script"></param>
        private void flow_mode12()
        {
            string[] anchors = new string[] { 
                                                ExpectedSelectedText(SelectionType.Page0_805_to_825),                                
                                                ExpectedSelectedText(SelectionType.Page0_10_to_30),
                                                ExpectedSelectedText(SelectionType.Page0_574_to_599),
                                                ExpectedSelectedText(SelectionType.Page0_250_to_215),
                                                ExpectedSelectedText(SelectionType.Page0_1200_to_1215)};
            
            Script.AddStatus("Setup:");
            Script.Add("SetZoom", new object[] { 165 });
            Script.Add("PageLayout", new object[] { 6 });
            Script.Add("CreateAnnotation", SelectionData(SelectionType.Page0_805_to_825));
            Script.Add("CreateAnnotation", SelectionData(SelectionType.Page0_10_to_30));
            Script.Add("CreateAnnotation", SelectionData(SelectionType.Page0_574_to_599));
            Script.Add("CreateAnnotation", SelectionData(SelectionType.Page0_250_to_215));
            Script.Add("CreateAnnotation", SelectionData(SelectionType.Page0_1200_to_1215));
            Script.Add("VerifyAnnotations", new object[] { anchors });

            Script.AddStatus("Re-paginate to 1 page:");
            Script.Add("SetZoom", new object[] { 100 });
            Script.Add("PageLayout", new object[] { 1 });
            Script.Add("VerifyAnnotations", new object[] { anchors });

            RunScript(Script);
        }

        /// <summary>
        /// 1. Set number of pages displayed to 2.	
        /// 2. Create Annotation on whole document.	
        /// Verify: No exception.
        /// </summary>
        private void flow_mode13()
        {
            ViewerBase.Document = SinglePageFlowDocument();
            DispatcherHelper.DoEvents();

            SetZoom(50);
            PageLayout(2);
            CreateAnnotation(new MultiPageSelectionData(0, PagePosition.Beginning, 0, DocViewerWrapper.PageCount - 1, PagePosition.End, 0));
            VerifyNumberOfAttachedAnnotations(1);
            passTest("Verified.");
        }

        /// <summary>
        /// 1. Create annotation on page.	
        /// 2. Create Annotation on whole document.	
        /// Verify: No exception.
        /// </summary>
        private void flow_mode14()
        {
            ViewerBase.Document = SinglePageFlowDocument();
            DispatcherHelper.DoEvents();

            SetZoom(50);            
            CreateAnnotation(new MultiPageSelectionData(0, PagePosition.Beginning, 0, 0, PagePosition.End, 0));
            PageLayout(2);
            VerifyNumberOfAttachedAnnotations(1);
            passTest("Verified.");
        }

        #endregion Repagination Mode Tests
    }
}	

