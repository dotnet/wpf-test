// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Pri1s testing annotations compatibility with repaginiating flow content.

using Annotations.Test;
using Annotations.Test.Framework;
using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;

namespace Avalon.Test.Annotations.Pri1s
{
    public class FlowPaginationSuite : AFlowPaginationSuite
    {
        #region Basic Behavior Tests


        /// <summary>
        /// Action:                                 Verify:
        /// 1. Annotation at start of document. 	 
        /// 2. Change pagination.  	                Annotation still at start of document.
        /// </summary>
        /// <param name="Script"></param>
        private void flow_basic0()
        {
            Script.Add("CreateAnnotation", SelectionData(SelectionType.StartOfDocument));
            Script.Add("VerifyAnnotation", new object[] { ExpectedSelectedText(SelectionType.StartOfDocument) });
            Script.Add("ResizeWindow", new object[] { new Size(900, 250) });
            Script.Add("SetZoom", new object[] { 25.0 });
            EnsureWholePage(ref Script);
            Script.Add("VerifyAnnotation", new object[] { ExpectedSelectedText(SelectionType.StartOfDocument) });
            RunScript(Script);
        }

        /// <summary>
        /// Action:                                 Verify:
        /// 1. Annotation at end of document. 	
        /// 2. Change pagination.  	                Annotation still at end of document.
        /// </summary>
        /// <param name="Script"></param>
        private void flow_basic1()
        {
            Script.Add("GoToLastPage");
            Script.Add("CreateAnnotation", SelectionData(SelectionType.EndOfDocDocument));
            Script.Add("VerifyAnnotation", new object[] { ExpectedSelectedText(SelectionType.EndOfDocDocument) });
            Script.Add("ResizeWindow", new object[] { new Size(500, 1000) });
            Script.Add("SetZoom", new object[] { 65.0 });
            Script.Add("GoToLastPage");
            Script.Add("VerifyAnnotation", new object[] { ExpectedSelectedText(SelectionType.EndOfDocDocument) });
            RunScript(Script);
        }

        /// <summary>
        /// Action:                                                         Verify:
        /// 1. Annotation at start of non-edge page. 	
        /// 2. Change pagination so that annotation is at end of page.	    Verify anchor.
        /// </summary>
        /// <param name="Script"></param>
        private void flow_basic2()
        {            
            ISelectionData selection = new SimpleSelectionData(3, 0, 47);            
            SetZoom(100);
            GoToPage(3);
            string expectedAnchor = GetText(selection); 
            CreateAnnotation(selection);
            VerifyAnnotation(expectedAnchor);
            ResizeWindow(new Size(930, 666));
            VerifyAnnotation(expectedAnchor);
            passTest("verified reflow from start to end of page.");
        }

        /// <summary>
        /// Action:                                                         Verify:
        /// 1. Annotation at start of non-edge page.	
        /// 2. Change pagination so that annotation is in middle of page.	Verify anchor.
        /// </summary>
        /// <param name="Script"></param>
        private void flow_basic3()
        {
            Script.Add("GoToPage", new object[] { 3 });
            Script.Add("CreateAnnotation", SelectionData(SelectionType.Page3_0_to_53));
            Script.Add("VerifyAnnotation", new object[] { ExpectedSelectedText(SelectionType.Page3_0_to_53) });
            Script.Add("SetZoom", new object[] { 75.0 });
            Script.Add("VerifyAnnotation", new object[] { ExpectedSelectedText(SelectionType.Page3_0_to_53) });
            RunScript(Script);
        }

        /// <summary>
        /// Action:                                                         Verify:
        /// 1. Annotation at middle of non-edge page. 	
        /// 2. Change pagination so annotation is at start of page.	        Verify anchor.
        /// </summary>
        /// <param name="Script"></param>
        private void flow_basic4()
        {
            Script.Add("GoToPage", new object[] { 2 });
            Script.Add("CreateAnnotation", SelectionData(SelectionType.Page2_1067_to_1261));
            Script.Add("VerifyAnnotation", new object[] { ExpectedSelectedText(SelectionType.Page2_1067_to_1261) });
            Script.Add("SetWindowHeight", new object[] { 200 });
            Script.Add("GoToSelection");
            Script.Add("VerifyAnnotation", new object[] { ExpectedSelectedText(SelectionType.Page2_1067_to_1261) });
            RunScript(Script);
        }

        /// <summary>
        /// Action:                                                         Verify:
        /// 1. Annotation at middle of non-edge page.	
        /// 2. Change pagination so annotation is at end of page.	        Verify anchor.
        /// </summary>
        /// <param name="Script"></param>
        private void flow_basic5()
        {
            Script.Add("GoToPage", new object[] { 2 });
            Script.Add("CreateAnnotation", SelectionData(SelectionType.Page2_1067_to_1261));
            Script.Add("VerifyAnnotation", new object[] { ExpectedSelectedText(SelectionType.Page2_1067_to_1261) });
            Script.Add("SetWindowHeight", new object[] { 200 });
            Script.Add("GoToSelection");
            Script.Add("VerifyAnnotation", new object[] { ExpectedSelectedText(SelectionType.Page2_1067_to_1261) });
            RunScript(Script);
        }

        /// <summary>
        /// Action	                                                                            Verify
        /// 1. Annotation that is anchored to entire document and displayed on 1 giant page.	
        /// 2. Change pagination so that annotation is anchored across multiple pages.	        Verify annotation.
        /// </summary>
        private void flow_basic13()
        {
            ViewerBase.Document = SinglePageFlowDocument();
            DispatcherHelper.DoEvents();

            Script.Add("CreateAnnotation", new object[] { 0, PagePosition.Beginning, 0, 0, PagePosition.End, 0 });
            Script.Add("VerifyAnnotation", new object[] { SinglePageText() });
            Script.Add("SetZoom", new object[] { 150 });
			Script.Add("PageLayout", new object[] { 6 });
			Script.Add("VerifyPagesAreVisible", new object[] { new int[] { 0, 1, 2, 3 } });
            Script.Add("VerifyAnnotation", new object[] { SinglePageText() });
            RunScript(Script);
        }

        #endregion Basic Behavior Tests

        #region Repagination Mode Tests

        /// <summary>
        /// Action	                            Verify
        /// 1. Annotate original pagination	    Verify annotations are correct.
        /// 2. Change to “Two Pages”.	        Verify annotations are correct.
        /// </summary>
        private void flow_mode1()
        {
            Script.Add("CreateAnnotation", SelectionData(SelectionType.Page0_1200_to_1215));
			Script.Add("GoToPage", new object[] { 1 });
            Script.Add("CreateAnnotation", SelectionData(SelectionType.Page1_90_to_140));
			Script.Add("CreateAnnotation", SelectionData(SelectionType.Page1_to_Page2));
			Script.Add("ViewAsTwoPages");
			Script.Add("GoToPage", new object[] { 0 });
			Script.Add("VerifyAnnotations", new object[] {  new string [] { ExpectedSelectedText(SelectionType.Page0_1200_to_1215),
			                                                                ExpectedSelectedText(SelectionType.Page1_90_to_140),
			                                                                ExpectedSelectedText(SelectionType.Page1_End) }
			                                              });
			Script.Add("PageDown");
			Script.Add("PageDown");
			Script.Add("VerifyAnnotations", new object[] { new string[] { ExpectedSelectedText(SelectionType.Page2_Start) } });
            RunScript(Script);
        }

        /// <summary>
        /// Action	                            Verify
        /// 1. Annotate original pagination	    Verify annotations are correct.
        /// 2. Change to “Page Width”.  	    Verify annotations are correct.
        /// </summary>
        /// <param name="Script"></param>
        private void flow_mode2()
        {
			string[] page2Anchors = new string[] {  ExpectedSelectedText(SelectionType.Page2_Start),
                                                                            ExpectedSelectedText(SelectionType.Page2_1067_to_1261),
                                                                            ExpectedSelectedText(SelectionType.Page2_End) };
			string[] page3Anchors = new string[] { ExpectedSelectedText(SelectionType.Page3_Start) };

			Script.Add("GoToPage", new object[] { 2 });            
            Script.Add("CreateAnnotation", SelectionData(SelectionType.Page2_Start));
            Script.Add("CreateAnnotation", SelectionData(SelectionType.Page2_1067_to_1261));
            Script.Add("CreateAnnotation", SelectionData(SelectionType.Page2_to_Page3));
			Script.Add("VerifyAnnotations", new object[] { page2Anchors });
			Script.Add("PageWidthLayout");
			Script.Add("VerifyAnnotations", new object[] { page2Anchors });
            Script.Add("PageDown");
            Script.Add("VerifyAnnotations", new object[] { page3Anchors });
            RunScript(Script);
        }

        /// <summary>
        /// Action	                            Verify
        /// 1. Annotate original pagination	    Verify annotations are correct.
        /// 2. Change to “Two Pages”.	        Verify annotations are correct.
        /// 3. Change to “Page Width”.  	    Verify annotations are correct.
        /// 4. Change to “Whole page”.	        Verify annotations are correct.
        /// </summary>
        /// <param name="Script"></param>
        private void flow_mode4()
        {
            string[] page1Anchors;
            string[] page2Anchors;
            string[] allAnchors;

            // Different results for highlight because overlapping annotations are combined.
            if (AnnotationType == AnnotationMode.Highlight)
            {
                page1Anchors = new string[] { ExpectedSelectedText(SelectionType.Page1_100_to_150) };
                page2Anchors = new string[] { ExpectedSelectedText(SelectionType.Page2_Start), ExpectedSelectedText(SelectionType.Page2_1067_to_1261), ExpectedSelectedText(SelectionType.Page2_End) };
                allAnchors = new string[] { page1Anchors[0], page2Anchors[0], page2Anchors[1], page2Anchors[2] };
            }
            else
            {
                page1Anchors = new string[] { ExpectedSelectedText(SelectionType.Page1_110_to_125), ExpectedSelectedText(SelectionType.Page1_100_to_150) };
                page2Anchors = new string[] { ExpectedSelectedText(SelectionType.Page2_Start), ExpectedSelectedText(SelectionType.Page2_1067_to_1261), ExpectedSelectedText(SelectionType.Page2_End) };
                allAnchors = new string[] { page1Anchors[0], page1Anchors[1], page2Anchors[0], page2Anchors[1], page2Anchors[2] };
            }

            Script.AddStatus("Setup:");
            Script.Add("PageDown");
            Script.Add("CreateAnnotation", SelectionData(SelectionType.Page1_110_to_125));
            Script.Add("CreateAnnotation", SelectionData(SelectionType.Page1_100_to_150));
            Script.Add("VerifyAnnotations", new object[] { page1Anchors });
            Script.Add("PageDown");
            Script.Add("CreateAnnotation", SelectionData(SelectionType.Page2_Start));
            Script.Add("CreateAnnotation", SelectionData(SelectionType.Page2_1067_to_1261));
            Script.Add("CreateAnnotation", SelectionData(SelectionType.Page2_End));
            Script.Add("VerifyAnnotations", new object[] { page2Anchors });
            Script.Add("PageUp");

            Script.AddStatus("Two Page Layout:");
            Script.Add("PageLayout", new object[] { 2 });
            Script.Add("SetZoom", new object[] { 25 });
            Script.Add("VerifyAnnotations", new object[] { page1Anchors });
            Script.Add("PageDown", new object[] { 2 });
            Script.Add("VerifyAnnotations", new object[] { page2Anchors });

            Script.AddStatus("Page Width Layout:");
            Script.Add("PageWidthLayout");
            Script.Add("SetZoom", new object[] { 100 });
            Script.Add("GoToPage", new object[] { 1 });
            Script.Add("VerifyAnnotations", new object[] { page1Anchors });
            Script.Add("PageDown");
            Script.Add("VerifyAnnotations", new object[] { page2Anchors });

            Script.AddStatus("Whole Page Layout:");
            Script.Add("WholePageLayout");            
            Script.Add("GoToPage", new object[] { 2 });
            Script.Add("VerifyAnnotations", new object[] { page2Anchors });
            Script.Add("PageUp");
            Script.Add("VerifyAnnotations", new object[] { page1Anchors });

            RunScript(Script);
        }

        #endregion
    }
}


