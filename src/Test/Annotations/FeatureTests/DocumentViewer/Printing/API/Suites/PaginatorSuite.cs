// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 

using System;
using System.Windows;
using Annotations.Test.Framework;
using System.IO;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Annotations;
using System.Collections.Generic;
using System.Windows.Markup;

namespace Avalon.Test.Annotations.Suites
{
    [ExecutionGroupCompatible(false)]
    public class PaginatorSuite : APrintingSuite
    {
        #region BVT TESTS

        #region GetPage

        /// <summary>
        /// Parameters: Annotation on first page.
        ///				pageNumber = 0
        ///	Verify: DocumentPage contains Annotation in correct location.
        /// </summary>
        [Priority(0)]
        protected void adp_getpage1()
        {
            DocumentStateInfo docState = SetupDocumentState(new SimpleSelectionData(0, 100, 100));
            DocViewerWrapper.GoToPage(0);
            DocumentPage result = GetPage(CreateAnnotationDocumentPaginator(), 0);
            VerifyPrintedAnnotations(docState.AnnotationState(0), result);
            passTest("Verified single annotation.");
        }

        /// <summary>
        /// Parameters: Non-overlapping StickyNote and Highlight on page N.
        ///				PageNumber=N	
        /// Verify : DocumentPage contains StickyNote and Highlight in correct location.
        /// </summary>
        [Priority(0)]
        [Keywords("MicroSuite")]
        protected void adp_getpage2()
        {
            DocumentStateInfo docState = SetupDocumentState(new AnnotationDefinition[] { 
				new StickyNoteDefinition(new SimpleSelectionData(0, PagePosition.End, -900), true),
				new HighlightDefinition(new SimpleSelectionData(0, PagePosition.Beginning, 50), Colors.Blue) });
            DocumentPaginator adp = CreateAnnotationDocumentPaginator();
            VerifyPrintedAnnotations(docState.AnnotationState(0), GetPage(adp, 0));
            passTest("Verified non-overlapping SN and highlight on same page.");
        }

        /// <summary>
        /// Parameters: Overlapping StickyNote and Highlight on page N.
        ///				PageNumber=N	
        /// Verify : DocumentPage contains StickyNote and Highlight in correct location.
        /// </summary>
        [Priority(0)]
        protected void adp_getpage3()
        {
            DocumentStateInfo docState = SetupDocumentState(new AnnotationDefinition[] { 
				new StickyNoteDefinition(new SimpleSelectionData(0, PagePosition.Middle, 500), true),
				new HighlightDefinition(new SimpleSelectionData(0, PagePosition.Middle, 600), Colors.Red) });
            DocumentPaginator adp = CreateAnnotationDocumentPaginator();
            VerifyPrintedAnnotations(docState.AnnotationState(0), GetPage(adp, 0));
            passTest("Verified overlapping SN and highlight on same page.");
        }

        /// <summary>
        /// Parameters: StickyNote spanning pages A and B. 
        ///				PageNumber=A then PageNumber=B
        /// Verify: DocumentPage contains first portion of SN.
        /// </summary>
        [Priority(0)]
        protected void adp_getpage6()
        {
            DocumentStateInfo docState = SetupDocumentState(new MultiPageSelectionData(1, PagePosition.End, -100, 2, PagePosition.Beginning, 100));
            DocumentPaginator adp = CreateAnnotationDocumentPaginator();
            printStatus("Verifying page 1");
            VerifyPrintedAnnotations(docState.AnnotationState(1), GetPage(adp, 1));
            printStatus("Verifying page 2");
            VerifyPrintedAnnotations(docState.AnnotationState(2), GetPage(adp, 2));
            passTest("Verified annotation on 1st page of multipage anchor.");
        }

        /// <summary>
        /// Parameters: Minimized SN and maximized SN on page N.
        ///				PageNumber=N
        ///	Verify: DocumentPage contains minimized SN in correct location.
        /// </summary>
        [Priority(0)]
        protected void adp_getpage7()
        {
            DocumentStateInfo docState = SetupDocumentState(new AnnotationDefinition[] { new StickyNoteDefinition(new SimpleSelectionData(1, 1000, 231), false) });
            VerifyPrintedAnnotations(docState.AnnotationState(1), GetPage(CreateAnnotationDocumentPaginator(), 1));
            passTest("Verified minimized SN.");
        }

        #endregion

        #region PageSize

        [Priority(0)]
        protected void adp_pagesize1()
        {
            if (ContentMode == TestMode.Fixed)
                testPageSizeFixed(true);
            else
                testPageSizeFlow(true);
        }

        [Priority(0)]
        protected void adp_pagesize2()
        {
            if (ContentMode == TestMode.Fixed)
                testPageSizeFixed(false);
            else
                testPageSizeFlow(false);
        }

        /// <summary>
        /// Parameters: PageSize > window/DocumentPage size.
        /// Verify: GetPage returns DP with annotations reflowed onto larger pages.
        /// </summary>
        protected void testPageSizeFixed(bool larger)
        {
            DocumentStateInfo docState = SetupDocumentState(new ISelectionData[] { 
				new SimpleSelectionData(2, 100, 200), 
				new SimpleSelectionData(2, PagePosition.Middle, 101), 
				new SimpleSelectionData(3, PagePosition.Beginning, 51), 
				new SimpleSelectionData(3, PagePosition.Middle, 83) });
            DocumentPaginator adp = CreateAnnotationDocumentPaginator();
            SetPageSize(larger, adp);

            printStatus("Verifying page 2");
            VerifyPrintedAnnotations(docState.AnnotationState(2), GetPage(adp, 2));
            printStatus("Verifying page 3");
            VerifyPrintedAnnotations(docState.AnnotationState(3), GetPage(adp, 3));
            passTest("Verified PageSize changes nothing.");
        }

        /// <summary>
        /// Parameters: PageSize > window/DocumentPage size.
        /// Verify: GetPage returns DP with annotations reflowed onto larger pages.
        /// </summary>
        protected void testPageSizeFlow(bool larger)
        {
            SetupDocumentState(new ISelectionData[] { 
				new SimpleSelectionData(4, 100, 200), 
				new SimpleSelectionData(4, PagePosition.Middle, 101), 
				new MultiPageSelectionData(4, PagePosition.End, -200, 5, PagePosition.Beginning, 51), 
				new SimpleSelectionData(5, PagePosition.Middle, 83) });
            DocumentPaginator adp = CreateAnnotationDocumentPaginator();
            SetPageSize(larger, adp);

            if (larger)
            {
                // Reflow so that all annotations are on same page.
                VerifyNumberOfPrintedAnnotations(GetPage(adp, 3), 4);
            }
            else
            {
                // Reflow so page sizes are much smaller.
                VerifyNumberOfPrintedAnnotations(GetPage(adp, 14), 1);
                VerifyNumberOfPrintedAnnotations(GetPage(adp, 16), 1);
                VerifyNumberOfPrintedAnnotations(GetPage(adp, 18), 2);
            }
            passTest("Verified PageSize causes reflow.");
        }

        private void SetPageSize(bool larger, DocumentPaginator idp)
        {
            Size newSize;
            if (larger)
                newSize = new Size(2000, 2000); // Verify large.
            else
                newSize = new Size(idp.PageSize.Width / 2, idp.PageSize.Height / 2); // 1/2 original size.
            idp.PageSize = newSize;

            while (!idp.IsPageCountValid)
                DispatcherHelper.DoEvents(); // Wait for page count computation.
        }

        #endregion

        #endregion BVT TESTS

        #region PRIORITY TESTS

        #region GetPage

        /// <summary>
		/// Parameters: N annotations on firstPage.
		///				PageNumber=secondPage	
		/// Verify: DocumentPage contains no annotations.
		/// </summary>
        [Priority(1)]
        protected void adp_getpage4()
		{
			DocumentStateInfo docState = SetupDocumentState(new ISelectionData[] { new SimpleSelectionData(0, 0, 100), new SimpleSelectionData(0, PagePosition.Middle, 301), new SimpleSelectionData(0, PagePosition.End, -99) });
			VerifyNumberOfPrintedAnnotations(GetPage(CreateAnnotationDocumentPaginator(), 1), 0);
			passTest("No annotations expected.");
		}

        [TestCase_Helper()]
        private void EnsureStickyNoteMode()
		{
			if (AnnotationType == AnnotationMode.Highlight)
				throw new ArgumentException("Test case is StickyNote only.");
		}

		/// <summary>
		/// Parameters: N overlapping StickyNotes on firstPage. 
		///				PageNumber = firstPage	
		/// Verify: DocumentPage contains StickyNotes in correct locations.
		/// </summary>
        [Priority(1)]
        protected void adp_getpage5_1()
		{
			EnsureStickyNoteMode();
			DocumentStateInfo docState = SetupDocumentState(new ISelectionData[] { 
				new SimpleSelectionData(0, 200, 250), 
				new SimpleSelectionData(0, 200, 250), 
				new SimpleSelectionData(0, 150, 500) });
			VerifyPrintedAnnotations(docState.AnnotationState(0), GetPage(CreateAnnotationDocumentPaginator(), 0));
			passTest("Verified printing overlapping SNs maintains default Z order.");
		}

		/// <summary>
		/// Parameters: N overlapping StickyNotes on firstPage with non-default Z order.
		///				PageNumber = firstPage	
		/// Verify: DocumentPage contains StickyNotes in correct locations.
		/// </summary>
        [Priority(1)]
        protected void adp_getpage5_2()
		{
			EnsureStickyNoteMode();
			// Create StickyNotes in the opposite Z-order than is default.
			DocumentStateInfo docState = SetupDocumentState(new AnnotationDefinition[] { 
				new StickyNoteDefinition(new SimpleSelectionData(0, 200, 250), true, true), 
				new StickyNoteDefinition(new SimpleSelectionData(0, 200, 250), true, true), 
				new StickyNoteDefinition(new SimpleSelectionData(0, 150, 500), true, true) });
			VerifyPrintedAnnotations(docState.AnnotationState(0), GetPage(CreateAnnotationDocumentPaginator(), 0));
			passTest("Verified printing overlapping SNs maintains non-default Z order.");
		}

		/// <summary>
		/// Parameters: Minimized SN and maximized SN on page N.
		///				PageNumber=N	
		/// Verify: Document contains 1 min and 1 max SN.
		/// </summary>
        [Priority(1)]
        protected void adp_getpage8()
		{
			DocumentStateInfo docState = SetupDocumentState(new AnnotationDefinition[] { 
				new StickyNoteDefinition(new SimpleSelectionData(0, 71, 29), false),
				new StickyNoteDefinition(new SimpleSelectionData(0, 250, 68), true)});
			VerifyPrintedAnnotations(docState.AnnotationState(0), GetPage(CreateAnnotationDocumentPaginator(), 0));
			passTest("Verified min/max SN on same page.");
		}

		/// <summary>
		/// Parameters: PageNumber=negative	Throw 
		/// Verify: ArgumentOutOfRangeException
		/// </summary>
        [Priority(1)]
        protected void adp_getpage9()
		{
			bool expectedException = false;
			try
			{
				GetPage(CreateAnnotationDocumentPaginator(), -1);
			}
			//catch (ArgumentOutOfRangeException e)
            catch (ArgumentOutOfRangeException)
			{
				expectedException = true;
			}
			Assert("Verify exception occurred.", expectedException);
			passTest("Exception for negative page number.");
		}

		#endregion

		#region Async

		/// <summary>
		/// Parameters: N page document.
		///				Call GetPageAsync for all pages.	
		/// Verify: Verify that all DocumentPages are received and annotations are correct.
		/// </summary>
        [Priority(1)]
        protected void adp_async1()
		{
			DocumentStateInfo docState = SetupDocumentState(new ISelectionData[] {
				new SimpleSelectionData(0, 900, -500),
				new SimpleSelectionData(1, 1001, 45),
				new SimpleSelectionData(2, PagePosition.End, -291),
				new SimpleSelectionData(3, PagePosition.Beginning, 200)
			});
			int [] pages = new int[] { 0, 1, 2, 3, 4 };
			VerifyPrintedAnnotations(docState, new AsyncOperationHelper().GetPages(CreateAnnotationDocumentPaginator(), pages), pages);
			passTest("Verified async get for all pages.");
		}

		/// <summary>
		/// Parameters: Call with PageNumber=N 3 times.	
		/// Verify: Verify 1 event.
		/// </summary>
        [Priority(1)]
        protected void adp_async2()
		{
			DocumentStateInfo docState = SetupDocumentState(new ISelectionData[] {
				new SimpleSelectionData(1, 1600, -261)				
			});
			int[] pages = new int[] { 1, 1, 1 };
			VerifyPrintedAnnotations(docState.AnnotationState(1), new AsyncOperationHelper().GetPages(CreateAnnotationDocumentPaginator(), pages)[1]);
			passTest("Verified async 3 times.");
		}

		/// <summary>
		/// Parameters: PageNumber=N+1 then PageNumber=N	
		/// Verify: Verify events for 6 and 7 (no order).
		/// </summary>
        [Priority(1)]
        protected void adp_async3()
		{
			DocumentStateInfo docState = SetupDocumentState(new ISelectionData[] {
				new SimpleSelectionData(2, 900, -500),
				new SimpleSelectionData(3, 1001, 45)
			});
			int[] pages = new int[] { 3, 2 };
			VerifyPrintedAnnotations(docState, new AsyncOperationHelper().GetPages(CreateAnnotationDocumentPaginator(), pages), pages);
			passTest("Verified out of order async get.");
		}

		/// <summary>
		/// Parameters: PageNumber=N
		///				CancelAsync 	
		/// Verify: Verify no event.
		/// </summary>
        [Priority(1)]
        protected void adp_async4()
		{
			DocumentStateInfo docState = SetupDocumentState(new ISelectionData[] {
				new SimpleSelectionData(2, PagePosition.Middle, 250)
			});
			AsyncOperationHelper asyncHelper = new AsyncOperationHelper();
			asyncHelper.Start_GetPage(CreateAnnotationDocumentPaginator(), 2);
			AssertEquals("Verify number of pages returned.", 0, asyncHelper.Cancel_GetPages(new int[] { 2 }).Count);
			passTest("Verified cancel works.");
		}

		/// <summary>
		/// Parameters: PageNumber=N
		///				CancelAsync twice.
		/// Verify: No Exception.
		/// </summary>
        [Priority(1)]
        protected void adp_async5()
		{
			DocumentStateInfo docState = SetupDocumentState(new ISelectionData[] {
				new SimpleSelectionData(2, PagePosition.Middle, 250)
			});
			AsyncOperationHelper asyncHelper = new AsyncOperationHelper();
			asyncHelper.Start_GetPage(CreateAnnotationDocumentPaginator(), 2);
			asyncHelper.Cancel_GetPages(new int[] { 2 });
		    asyncHelper.Cancel_GetPages(new int[] { 2 });
			passTest("No exception for calling cancel twice.");
		}

		/// <summary>
		/// Parameters: PageNumber=0
		///				Delete annotation on page 0
		///	Verify: Verify annotation not deleted
		/// </summary>
        [Priority(1)]
        [DisabledTestCase()]
        protected void adp_async8_stream()
		{
			adp_async8(true);	
		}

		/// <summary>
		/// Parameters: PageNumber=0
		///				Delete annotation on page 0
		///	Verify: Verify annotation deleted
		/// </summary>
        [Priority(1)]
        [DisabledTestCase()]
        protected void adp_async8_store()
		{
			adp_async8(false);
		}

        [DisabledTestCase()]
		private void adp_async8(bool asStream)
		{
			AnnotationDefinition annot = new StickyNoteDefinition(new SimpleSelectionData(0, PagePosition.Middle, -100), true);
			DocumentStateInfo docState = SetupDocumentState(new AnnotationDefinition[] { annot });
			AsyncOperationHelper asyncHelper = new AsyncOperationHelper();

			DocumentPaginator idp = null;
			if (asStream)
				idp = CreateAnnotationDocumentPaginator(CurrentAnnotationStream);
			else
				idp = CreateAnnotationDocumentPaginator(CurrentAnnotationStore);

			asyncHelper.Start_GetPages(idp, new int[] { 0 });
			annot.Delete(DocViewerWrapper);

			VerifyPrintedAnnotations(docState.AnnotationState(0), asyncHelper.WaitForEnd_GetPages()[0]);
			passTest("Verified GetPageAsync while deleting annotations.");
		}

		/// <summary>
		/// Call multiple times then call CancelAsync(null).	
		/// Verify no events.
		/// </summary>
        [Priority(1)]
        protected void adp_async11()
		{
			DocumentStateInfo docState = SetupDocumentState(new ISelectionData[] { });
			AsyncOperationHelper asyncHelper = new AsyncOperationHelper();
			asyncHelper.Start_GetPages(CreateAnnotationDocumentPaginator(), new int[] { 0, 1, 2, 3, 4 });
			AssertEquals("Verify number of pages returned.", 0, asyncHelper.CancelAll_GetPages().Count);
			passTest("Verified cancel all.");
		}

		#endregion

		#region PageCount

		/// <summary>
		/// N Page Document	N.
		/// </summary>
        [Priority(1)]
        protected void adp_pagecount1()
		{
			DocumentPaginator adp = CreateAnnotationDocumentPaginator();
			AssertEquals("Verify num pages.", ViewerBase.PageCount, adp.PageCount);
			passTest("Verified number of pages.");
		}

		/// <summary>
		/// Increase PageSize, call ComputePageCount	
		/// Flow: Verify PageCount decreased.	
		/// Fixed: No change
		/// </summary>
        [Priority(1)]
        protected void adp_pagecount2()
		{
			TestPageCount(true);
			passTest("Verified number of pages.");
		}

		/// <summary>
		/// Decrease PageSize, call ComputePageCount	
		/// Flow: Verify PageCount increased.	
		/// Fixed: No change
		/// </summary>
        [Priority(1)]
        protected void adp_pagecount3()
		{
			TestPageCount(false);
			passTest("Verified number of pages.");
		}

		private void TestPageCount(bool larger)
		{
			DocumentPaginator adp = CreateAnnotationDocumentPaginator();
			int initialPageCount = adp.PageCount;
			double scale = (larger) ? 2 : .5;
			adp.PageSize = new Size(adp.PageSize.Width * scale, adp.PageSize.Height * scale);
			while (!adp.IsPageCountValid)
				DispatcherHelper.DoEvents();

			if (ContentMode == TestMode.Fixed)
			{
				// Page size should not change for fixed content.
				AssertEquals("Verify page count is unchanged.", initialPageCount, adp.PageCount);
			}
			else
			{
				if (larger)
					Assert("Verify num pages has decreased.", adp.PageCount < initialPageCount);
				else
					Assert("Verify num pages has increased.", adp.PageCount > initialPageCount);
			}
			passTest("Verfied PageCount behavior.");
		}

		#endregion

        #region Serialization

        /// <summary>
        /// Test serializing annotations on current document.
        /// </summary>
        [TestCase_Helper()]
        public void TestSerialization()
        {
            Assert("Verify some annotation exist.", TextControlWrapper.Service.Store.GetAnnotations().Count > 0);
            DocumentPaginator adp = CreateAnnotationDocumentPaginator();
            VerifySerializingToPrinter(adp);
            VerifySerializingToXps(adp);
            passTest("No exception serializing document with annotations");
        }

        #endregion

        #endregion PRIORITY TESTS
    }
}	

