// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 

using System;
using System.Windows;
using System.Windows.Controls;
using Annotations.Test.Framework;

namespace Avalon.Test.Annotations.Suites
{
	/// <summary>
	/// See Scenario1 in ScenarioDefinitions.cs.
	/// </summary>
	public class Highlight1Suite_BVT : AHighlightSuite
	{
		#region BVT TESTS

		/// <summary>
		/// Create Highlight annotation in the middle fo the first page.
		/// </summary>
        [Priority(0)]
        private void highlight1_1()
		{
            ISelectionData selection = new SimpleSelectionData(0, 10, 30);
            if (NonVisibleMode) GoToPage(1);
            CreateAnnotation(selection);
            if (NonVisibleMode) GoToPage(0);
            VerifyAnnotation(GetText(selection));
			passTest("simple highlight.");
		}

		/// <summary>
		/// Create a Highlight annotation on the first page, page down and create on on the second page.
		/// </summary>
        [Priority(0)]
        private void highlight1_4()
		{
            ISelectionData selection1 = new SimpleSelectionData(0, 10, 30);
            CreateAnnotation(selection1);
            VerifyAnnotation(GetText(selection1));
            GoToPage(1);
            ISelectionData selection2 = new SimpleSelectionData(1, 90, 140);
            CreateAnnotation(selection2);
            VerifyAnnotation(GetText(selection2));
            GoToPage(0);
            VerifyAnnotation(GetText(selection1));
            passTest("Verified creating highlights across multiple pages.");
		}

		/// <summary>
		/// Create a Highlight annotation on the first page, no page down and create on on the second page. Page down to verify.
		/// </summary>
        [Priority(0)]
        private void highlight1_4a()
		{
            ISelectionData selection1 = new SimpleSelectionData(0, 10, 30);
            ISelectionData selection2 = new SimpleSelectionData(1, 90, 140);
            CreateAnnotation(selection1);
            CreateAnnotation(selection2);
            VerifyAnnotation(GetText(selection1));
            GoToPage(1);
            VerifyAnnotation(GetText(selection2));
            passTest("Verifed highlight creation.");
		}

		/// <summary>
		/// Create annotation in the middle of the page, then create an enclosing annotation.
		/// Expected: only one annotation, new replaces old.
		/// </summary>
        [Priority(0)]
        private void highlight1_6()
		{
            if (!NonVisibleMode) GoToPage(1);
            CreateAnnotation(new SimpleSelectionData(1, 100, 50));
            CreateAnnotation(new SimpleSelectionData(1, 110, 15));
            if (NonVisibleMode) GoToPage(1);
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(1, 100, 10)) +
                GetText(new SimpleSelectionData(1, 125, 25)) ,
                GetText(new SimpleSelectionData(1, 110, 15))
                });
            passTest("Verified highlight merge.");
		}

        /// <summary>
        ///  Create annotation then create an adjacent annotation.
        /// Expected: merged into one annotation.
        /// </summary>
        [Priority(0)]
        private void highlight1_10()
        {
            if (!NonVisibleMode) GoToPage(1);
            CreateAnnotation(new SimpleSelectionData(1, 105, 35));
            CreateAnnotation(new SimpleSelectionData(1, 90, 30));
            if (NonVisibleMode) GoToPage(1);
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(1, 120, 20)),
                GetText(new SimpleSelectionData(1, 90, 30))
                });
            passTest("Verified highlight merge.");
        }
        
        /// <summary>
		///  Create annotation then create an adjacent annotation.
		/// Expected: merged into one annotation.
		/// </summary>
        [Priority(0)]
        private void highlight1_12()
		{
            if (!NonVisibleMode) GoToPage(1);
            CreateAnnotation(new SimpleSelectionData(1, 110, 15));
            CreateAnnotation(new SimpleSelectionData(1, 120, 10));
            if (NonVisibleMode) GoToPage(1);
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(1, 110, 10)),
                GetText(new SimpleSelectionData(1, 120, 10))
                });
            passTest("Verified highlight merge.");
		}

        /// <summary>
        /// Create annotation at the end of one page. Select start of the next page, create annotation.
        /// Expected: merge into one annotation.
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void highlight1_14()
        {
            if (!NonVisibleMode) GoToPageRange(1, 2);
            CreateAnnotation(new SimpleSelectionData(1, PagePosition.End, -92));
            CreateAnnotation(new SimpleSelectionData(2, PagePosition.Beginning, 129));
            if (NonVisibleMode) GoToPageRange(1, 2);
            VerifyAnnotation(GetText(new MultiPageSelectionData(1, PagePosition.End, -92, 2, PagePosition.Beginning, 129)));
            passTest("Verified crosspage merge.");
        }

		/// <summary>
		/// Two annotations, make enclosing annotation and it becomes one annotation.
		/// </summary>
        [Priority(0)]
        private void highlight2_1()
		{
            if (!NonVisibleMode) GoToPage(1);
            CreateAnnotation(new SimpleSelectionData(1, 20, 10));
            CreateAnnotation(new SimpleSelectionData(1, 35, 5));
            CreateAnnotation(new SimpleSelectionData(1, 19, 23));
            if (NonVisibleMode) GoToPage(1);
            VerifyAnnotation(GetText(new SimpleSelectionData(1, 19, 23)));            
            passTest("Verified highlight merge.");
		}

        /// <summary>
        /// Create annotation near the end of one page. Create another near the start of the next page. Select
        /// containing both annotatoins, create annotation.
        /// Expected: merge into one annotation.
        /// </summary>
        [Priority(0)]
        private void highlight2_6()
        {
            if (!NonVisibleMode) GoToPageRange(1, 2);
            CreateAnnotation(new MultiPageSelectionData(1, PagePosition.End, -62, 1, PagePosition.End, -41));
            CreateAnnotation(new SimpleSelectionData(2, 10, 15));
            ISelectionData spanningSelection = new MultiPageSelectionData(1, PagePosition.End, -90, 2, PagePosition.Beginning, 129);
            CreateAnnotation(spanningSelection);
            if (NonVisibleMode) GoToPageRange(1, 2);
            VerifyAnnotation(GetText(spanningSelection));
            passTest("Verified crosspage merge.");			
        }
        
        /// <summary>
		/// Add a Highlight annotation, then delete overlapping range.
		/// </summary>
        [Priority(0)]
        private void highlight3_1()
		{
            if (!NonVisibleMode) GoToPage(1);
            CreateAnnotation(new SimpleSelectionData(1, 20, 10));
            DeleteAnnotation(new SimpleSelectionData(1, 25, 15));
            if (NonVisibleMode) GoToPage(1);
            VerifyAnnotation(GetText(new SimpleSelectionData(1, 20, 5)));
            passTest("Verified highlight delete.");
		}

		/// <summary>
		/// Add a Highlight annotation, then delete with containing range.
		/// </summary>
        [Priority(0)]
        private void highlight3_4()
		{
            if (!NonVisibleMode) GoToPage(1);
            CreateAnnotation(new SimpleSelectionData(1, 120, 15));
            DeleteAnnotation(new SimpleSelectionData(1, 100, 50));
            if (NonVisibleMode) GoToPage(1);
            VerifyNumberOfAttachedAnnotations(0);
            passTest("Verified highlight delete.");
		}

        /// <summary>
        /// Add a Highlight annotation, then delete a separate (non-overlapping) range.
        /// Highlight is not deleted
        /// </summary>
        [Priority(0)]
        private void highlight3_6()
        {
            ISelectionData anchor = new SimpleSelectionData(1, 120, 15);
            if (!NonVisibleMode) GoToPage(1);     
            CreateAnnotation(anchor);
            DeleteAnnotation(new SimpleSelectionData(1, 25, 40));
            if (NonVisibleMode) GoToPage(1);
            VerifyAnnotation(GetText(anchor));
            passTest("Verified highlight delete.");
        }

        /// <summary>
        /// Add a Highlight annotation, then delete with an insertion point inside the Highlight.
        /// </summary>
        [Priority(0)]
        private void highlight4_1()
        {
			CreateAnnotation(0, 10, 1000);
			try {
                if (NonVisibleMode) GoToPage(1);
				DeleteAnnotation(0, 500, 0);
			} 
            //catch (InvalidOperationException e) 
            catch (InvalidOperationException) 
            {
				passTest("Verify that Clear for zero length selection throws.");
			}
			failTest("Expected exception for Clear with zero length selection.");
        }

        /// <summary>
        /// Create annotation at the end of one page. Create another near the start of the next page.
        /// Select from before the first annotation to the end of the second annotation.
        /// Delete, should be no annotations.
        /// Expected: zero annotations.
        /// </summary>
        [Priority(0)]
        private void highlight5_2()
        {
            GoToPageRange(2, 3);
            CreateAnnotation(new SimpleSelectionData(3, 8, 22));
            CreateAnnotation(new SimpleSelectionData(2, PagePosition.End, -56));
            if (NonVisibleMode) GoToPageRange(0, 1);
            ISelectionData spanningSelection = new MultiPageSelectionData(2, PagePosition.End, -140, 3, PagePosition.Beginning, 30);
            DeleteAnnotation(spanningSelection);
            if (NonVisibleMode) GoToPageRange(2, 3);
            VerifyNumberOfAttachedAnnotations(0);
            passTest("Verified crosspage delete.");	
        }

        /// <summary>
        /// Create two annotations inside one page. Create another across the endof
        /// this page and into the start of the next page.
        /// Select from after the first annotation but before the second to the end of the
        /// page, delete.
        /// Expected: only the first annotation should remain.
        /// </summary>
        [Priority(0)]
        private void highlight5_5()
        {
            GoToPageRange(1, 2);
            ISelectionData anchor1 = new SimpleSelectionData(1, 90, 50);
            ISelectionData anchor2 = new MultiPageSelectionData(1, PagePosition.End, -50, 1, PagePosition.End, -100);
            CreateAnnotation(anchor1);
            CreateAnnotation(anchor2);
            ISelectionData spanningSelection = new MultiPageSelectionData(1, PagePosition.End, -40, 2, PagePosition.Beginning, 45);
            CreateAnnotation(spanningSelection);
            VerifyAnnotations(new string[] { GetText(anchor1),  GetText(anchor2), GetText(spanningSelection) });
            if (NonVisibleMode) GoToPageRange(3, 4); 
            DeleteAnnotation(new MultiPageSelectionData(1, PagePosition.End, -500, 2, PagePosition.Beginning, 400));
            if (NonVisibleMode) GoToPageRange(1, 2);
            VerifyAnnotation(GetText(anchor1));
            passTest("Verified crosspage merge/delete.");
        }

        #endregion BVT TESTS
    }
}	

