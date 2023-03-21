// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Pri1s that verify delete behavior of Highlight annotations.

using System;
using System.Windows;
using Annotations.Test.Framework;
using Annotations.Test;

namespace Avalon.Test.Annotations.Suites
{
	public class DeleteHighlightSuite : AHighlightSuite
    {
        #region PRIORITY TESTS

        /// <summary>
		/// Annotation:    ------------
		/// Selection:  ------
		/// Result:		      ---------
		/// </summary>
        [Priority(1)]
        protected void delete1_2()
		{            
			CreateAnnotation(0, 100, 50);
			VerifyNumberOfAttachedAnnotations(1);
            if (NonVisibleMode) GoToPage(1);
			DeleteAnnotation(0, 90, 20);
            if (NonVisibleMode) GoToPage(0);
			VerifyAnnotation(GetText(new SimpleSelectionData(0, 110, 40)));
			passTest("Annotation partially deleted.");
		}

		/// <summary>
		/// Annotation:  ------------
		/// Selection:      ------
		/// Result:      ---      ---
		/// </summary>
        [Priority(1)]
        protected void delete1_3()
		{
			CreateAnnotation(0, 100, 50);
			VerifyNumberOfAttachedAnnotations(1);
            if (NonVisibleMode) GoToPage(1);
			DeleteAnnotation(0, 120, 20);
            if (NonVisibleMode) GoToPage(0);
			VerifyAnnotations(new string[] { 
				GetText(new SimpleSelectionData(0, 100, 20))+ // Fragment 1.
				GetText(new SimpleSelectionData(0, 140, 10)) // Fragment 2.
				});
			passTest("Annotation split.");
		}

		/// <summary>
		/// Annotation:  ------
		/// Selection:   ------
		/// Result: deleted.
		/// </summary>
        [Priority(1)]
        protected void delete1_5()
		{
			CreateAnnotation(0, 100, 50);
			VerifyNumberOfAttachedAnnotations(1);
            if (NonVisibleMode) GoToPage(1);
			DeleteAnnotation(0, 100, 50);
            if (NonVisibleMode) GoToPage(0);
			VerifyNumberOfAttachedAnnotations(0);
			passTest("Annotation deleted.");
		}

		/// <summary>
		/// Annotation:  ------
		/// Selection:         ------
		/// Result: no change.
		/// </summary>
        [Priority(1)]
        protected void delete1_7()
		{
			CreateAnnotation(0, 100, 50);
			VerifyNumberOfAttachedAnnotations(1);
            if (NonVisibleMode) GoToPage(1);
			DeleteAnnotation(0, 150, 50);
            if (NonVisibleMode) GoToPage(0);
			VerifyNumberOfAttachedAnnotations(1);
			passTest("Annotation not deleted.");
		}

		/// <summary>
		/// Selection on next page, no change.
		/// </summary>
        [Priority(1)]
        protected void delete1_8()
		{
			CreateAnnotation(0, PagePosition.End, -50);
			VerifyNumberOfAttachedAnnotations(1);
            if (NonVisibleMode) GoToPage(1);
			DeleteAnnotation(1, PagePosition.Beginning, 50);
            if (NonVisibleMode) GoToPage(0);
			VerifyNumberOfAttachedAnnotations(1);
			passTest("Annotation not deleted.");
		}

		/// <summary>
		/// Annotation:  ------ -----
		/// Selection:     ------- 
		/// Result:      --       ---
		/// </summary>
        [Priority(1)]
        protected void delete3_1()
		{
			CreateAnnotation(0, 19, 25);
			CreateAnnotation(0, 50, 19);
			VerifyNumberOfAttachedAnnotations(2);
            if (NonVisibleMode) GoToPage(1);
			DeleteAnnotation(0, 25, 30);
            if (NonVisibleMode) GoToPage(0);
			VerifyAnnotations(new string[] {
				GetText(new SimpleSelectionData(0, 19, 6)),
				GetText(new SimpleSelectionData(0, 55, 14)),
			});
			passTest("Annotations partially deleted.");
		}

		/// <summary>
		/// Annotation:  ------   -----
		/// Selection:     ------- 
		/// Result:		 --       -----
		/// </summary>
        [Priority(1)]
        protected void delete3_3()
		{
			ISelectionData highlightA = new SimpleSelectionData(1, 20, 7);
			ISelectionData highlightB = new SimpleSelectionData(1, 30, 10);
			ISelectionData selectionToDelete = new SimpleSelectionData(1, 25, 5);
			ISelectionData fragmentA = new SimpleSelectionData(1, 20, 5);

			GoToPage(1);
			CreateAnnotation(highlightA);
			CreateAnnotation(highlightB);
			VerifyNumberOfAttachedAnnotations(2);
            if (NonVisibleMode) GoToPage(3);
			DeleteAnnotation(new SimpleSelectionData(1, 25, 5));
            if (NonVisibleMode) GoToPage(1);
			VerifyAnnotations(new string[] { GetText(fragmentA), GetText(highlightB) });
			passTest("Portion of 1 highlight deleted.");			
		}

		/// <summary>
		/// Annotations on seperate pages. Selection spans both.  Both deleted.
		/// </summary>
        [Priority(1)]
        protected void delete3_4()
		{
			CreateAnnotation(0, 500, 100);
			PageDown();
			CreateAnnotation(1, 90, 15);
			DeleteAnnotation(0, PagePosition.Beginning, 500, 1, PagePosition.Beginning, 105);
			VerifyNumberOfAttachedAnnotations(0);
			printStatus("Highlight on page 2 deleted.");
			PageUp();
			VerifyNumberOfAttachedAnnotations(0);
			printStatus("Highlight on page 1 deleted.");
			passTest("Highlights across page break deleted.");
		}

		/// <summary>
		/// Annotations on same page.  Selection spans whole page.  Both deleted.
		/// </summary>
        [Priority(1)]
        protected void delete3_6()
		{
			CreateAnnotation(0, 500, 100);	
			CreateAnnotation(0, 9, 61);
            if (NonVisibleMode) GoToPage(1);
			DeleteAnnotation(0, PagePosition.Beginning, 0, 0, PagePosition.End, 0);
            if (NonVisibleMode) GoToPage(0);
			VerifyNumberOfAttachedAnnotations(0);
			passTest("Both highlights deleted.");
        }

        #endregion PRIORITY TESTS
    }
}	

