// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 

using System;
using System.Windows;
using Annotations.Test;
using Annotations.Test.Framework;

namespace Avalon.Test.Annotations.Suites
{
	public class MultiColortSuite_BVT : AHighlightSuite
	{
		#region BVT TESTS

		/// <summary>
		/// Create two Highlight annotations in the middle fo the first page, then another Highlight that overlaps both.
		/// Three annotations created.
		/// </summary>
        [Priority(0)]
        private void multicolor1_1()
		{
            if (!NonVisibleMode) GoToPage(1);
			CreateHighlight(new SimpleSelectionData(1, 120, 7), 0);
			CreateHighlight(new SimpleSelectionData(1, 130, 10), 1);
			CreateHighlight(new SimpleSelectionData(1, 125, 12), 2);
            if (NonVisibleMode) GoToPage(1);
			VerifyAnnotations(new string[] { new SimpleSelectionData(1, 120, 5).GetSelection(DocViewerWrapper.SelectionModule), new SimpleSelectionData(1, 125, 12).GetSelection(DocViewerWrapper.SelectionModule), new SimpleSelectionData(1, 137, 3).GetSelection(DocViewerWrapper.SelectionModule) });
			passTest("Multicolor with overlap 1");
		}

		/// <summary>
		/// Create two Highlight annotations in the middle fo the first page, then another Highlight that overlaps both.
		/// Three annotations created.
		/// </summary>
        [Priority(0)]
        private void multicolor1_2()
		{
            if (!NonVisibleMode) GoToPage(1);
			CreateHighlight(new SimpleSelectionData(1, 120, 7), 1);
			CreateHighlight(new SimpleSelectionData(1, 130, 10), 2);
			CreateHighlight(new SimpleSelectionData(1, 125, 12), 1);
            if (NonVisibleMode) GoToPage(1);
			VerifyAnnotations(new string[] { 
                new SimpleSelectionData(1, 120, 5).GetSelection(DocViewerWrapper.SelectionModule), 
                new SimpleSelectionData(1, 125, 12).GetSelection(DocViewerWrapper.SelectionModule), 
                new SimpleSelectionData(1, 137, 3).GetSelection(DocViewerWrapper.SelectionModule) });
			passTest("Multicolor with overlap 2");
		}

		/// <summary>
		/// Create two Highlight annotations in the middle fo the first page, then another Highlight that meets
		/// one and overlaps the other.
		/// Three annotations created.
		/// </summary>
        [Priority(0)]
        private void multicolor1_4()
		{
            if (!NonVisibleMode) GoToPage(2);
			CreateHighlight(new SimpleSelectionData(2, 231, 12), 1);
			CreateHighlight(new SimpleSelectionData(2, 260, 20), 2);
			CreateHighlight(new SimpleSelectionData(2, 243, 22), 0);
            if (NonVisibleMode) GoToPage(2);
			VerifyAnnotations(new string[] { new SimpleSelectionData(2, 231, 12).GetSelection(DocViewerWrapper.SelectionModule), new SimpleSelectionData(2, 243, 22).GetSelection(DocViewerWrapper.SelectionModule), new SimpleSelectionData(2, 265, 15).GetSelection(DocViewerWrapper.SelectionModule) });
			passTest("Multicolor with meet and overlap 4");
		}

		/// <summary>
		/// Create two Highlight annotations in the middle fo the first page, then another Highlight of the colour of the first
		/// Highthat meets one and overlaps the other.
		/// Two annotations created.
		/// </summary>
        [Priority(0)]
        private void multicolor1_6()
		{
            if (!NonVisibleMode) GoToPage(2);
			CreateHighlight(new SimpleSelectionData(2, 231, 12), 1);
			CreateHighlight(new SimpleSelectionData(2, 260, 20), 1);
			CreateHighlight(new SimpleSelectionData(2, 243, 22), 2);
            if (NonVisibleMode) GoToPage(2);
			VerifyAnnotations(new string[] { new SimpleSelectionData(2, 231, 12).GetSelection(DocViewerWrapper.SelectionModule), new SimpleSelectionData(2, 243, 22).GetSelection(DocViewerWrapper.SelectionModule), new SimpleSelectionData(2, 265, 15).GetSelection(DocViewerWrapper.SelectionModule) });
			passTest("Multicolor with meet and overlap 6");
		}

		/// <summary>
		/// Create two Highlight annotations in the middle of the first page, then another of a different colour
		/// Highlight that meets both, filling the gap between
		/// Three annotations created.
		/// </summary>
        [Priority(0)]
        private void multicolor1_7()
		{
            if (!NonVisibleMode) GoToPage(2);
			CreateHighlight(new SimpleSelectionData(2, 572, 9), 1);
			CreateHighlight(new SimpleSelectionData(2, 604, 23), 3);
			CreateHighlight(new SimpleSelectionData(2, 581, 23), 2);
            if (NonVisibleMode) GoToPage(2);
			VerifyAnnotations(new string[] { new SimpleSelectionData(2, 572, 9).GetSelection(DocViewerWrapper.SelectionModule), new SimpleSelectionData(2, 604, 23).GetSelection(DocViewerWrapper.SelectionModule), new SimpleSelectionData(2, 581, 23).GetSelection(DocViewerWrapper.SelectionModule) });
			passTest("Multicolor two meets 4");
		}

		/// <summary>
		/// Create two Highlight annotations in the middle of the first page, then another
		/// Highlight that overwrites one
		/// Two annotations.
		/// </summary>
        [Priority(0)]
        private void multicolor3_1()
		{
            if (!NonVisibleMode) GoToPage(3);
			CreateHighlight(new SimpleSelectionData(3, 172, 19), 1);
			CreateHighlight(new SimpleSelectionData(3, 194, 23), 3);
			CreateHighlight(new SimpleSelectionData(3, 194, 32), 1);
            if (NonVisibleMode) GoToPage(3);
			VerifyAnnotations(new string[] { new SimpleSelectionData(3, 172, 19).GetSelection(DocViewerWrapper.SelectionModule), new SimpleSelectionData(3, 194, 32).GetSelection(DocViewerWrapper.SelectionModule) });
			passTest("Multicolor disjoint 1");
		}

		/// <summary>
		/// Create two Highlight annotations in the middle of the first page, then another
		/// Highlight that overwrites one and meets the other, same colour as the one it meets,
		/// should merge.
		/// One annotation.
		/// </summary>
        [Priority(0)]
        private void multicolor4_1()
		{
            if (NonVisibleMode) GoToPage(1);
			CreateHighlight(new SimpleSelectionData(0, 172, 3), 1);
			CreateHighlight(new SimpleSelectionData(0, 194, 23), 2);
			CreateHighlight(new SimpleSelectionData(0, 169, 25), 2);
            if (NonVisibleMode) GoToPage(0);
			VerifyAnnotations(new string[] { 
                new SimpleSelectionData(0, 169, 25).GetSelection(DocViewerWrapper.SelectionModule) ,
                new SimpleSelectionData(0, 194, 23).GetSelection(DocViewerWrapper.SelectionModule)
            });
			passTest("Multicolor disjoint 1");
		}

		/// <summary>
		/// Create two Highlight annotations in the middle of the first page, then another
		/// Highlight that overwrites one and meets the other, different colour from the one it meets,
		/// same colour as the one it replaces.
		/// One annotations.
		/// </summary>
        [Priority(0)]
        private void multicolor4_2()
		{
            if (NonVisibleMode) GoToPage(1);
			CreateHighlight(new SimpleSelectionData(0, 172, 3), 1);
			CreateHighlight(new SimpleSelectionData(0, 194, 23), 2);
			CreateHighlight(new SimpleSelectionData(0, 169, 25), 1);
            if (NonVisibleMode) GoToPage(0);
			VerifyAnnotations(new string[] { new SimpleSelectionData(0, 169, 25).GetSelection(DocViewerWrapper.SelectionModule), new SimpleSelectionData(0, 194, 23).GetSelection(DocViewerWrapper.SelectionModule) });
			passTest("Multicolor disjoint 1");
		}

		/// <summary>
		/// Create two Highlight annotations in the middle of the first page, then another
		/// Highlight that overwrites one and overlaps the other. Each a different colour.
		/// One annotations.
		/// </summary>
        [Priority(0)]
        private void multicolor4_4()
		{
            if (!NonVisibleMode) GoToPage(1);
			CreateHighlight(new SimpleSelectionData(1, 721, 23), 1);
			CreateHighlight(new SimpleSelectionData(1, 754, 15), 0);
			CreateHighlight(new SimpleSelectionData(1, 732, 42), 2);
            if (NonVisibleMode) GoToPage(1);
            VerifyAnnotations(new string[] { new SimpleSelectionData(1, 721, 11).GetSelection(DocViewerWrapper.SelectionModule), new SimpleSelectionData(1, 732, 42).GetSelection(DocViewerWrapper.SelectionModule) });
			passTest("Multicolor disjoint 1");
		}

		/// <summary>
		/// Create two Highlight annotations in the middle of the first page, then select between
		/// them and delete. No change.
		/// </summary>
        [Priority(0)]
        private void multicolordelete1_2()
		{
            if (!NonVisibleMode) GoToPage(1);
			CreateHighlight(new SimpleSelectionData(1, 211, 12), 1);
			CreateHighlight(new SimpleSelectionData(1, 243, 15), 0);
			DeleteHighlight(DocViewerWrapper, new SimpleSelectionData(1, 225, 12));
            if (NonVisibleMode) GoToPage(1);
            VerifyAnnotations(new string[] { new SimpleSelectionData(1, 211, 12).GetSelection(DocViewerWrapper.SelectionModule), new SimpleSelectionData(1, 243, 15).GetSelection(DocViewerWrapper.SelectionModule) });
			passTest("Multicolor disjoint 1");
		}

		/// <summary>
		/// Create two Highlight annotations that meet, delete a selection that contains the first and meets the
		/// second. First deleted.
		/// </summary>
		private void multicolordelete2_1()
		{
            if (!NonVisibleMode) GoToPage(1);
			CreateHighlight(new SimpleSelectionData(1, 288, 9), 1);
			CreateHighlight(new SimpleSelectionData(1, 297, 5), 0);
			DeleteHighlight(DocViewerWrapper, new SimpleSelectionData(1, 285, 12));
            if (NonVisibleMode) GoToPage(1);
            VerifyAnnotations(new string[] { new SimpleSelectionData(1, 297, 5).GetSelection(DocViewerWrapper.SelectionModule) });
			passTest("Multicolor disjoint 1");
		}

		/// <summary>
		/// Create two Highlight annotations that meet, delete a selection is contained by the first and meets the
		/// second. First truncated.
		/// </summary>
        [Priority(0)]
        private void multicolordelete2_2()
		{
            if (!NonVisibleMode) GoToPage(1);
			CreateHighlight(new SimpleSelectionData(1, 288, 9), 1);
			CreateHighlight(new SimpleSelectionData(1, 297, 5), 0);
			DeleteHighlight(DocViewerWrapper, new SimpleSelectionData(1, 290, 7));
            if (NonVisibleMode) GoToPage(1);
            VerifyAnnotations(new string[] { new SimpleSelectionData(1, 288, 2).GetSelection(DocViewerWrapper.SelectionModule), new SimpleSelectionData(1, 297, 5).GetSelection(DocViewerWrapper.SelectionModule) });
			passTest("Multicolor disjoint 1");
		}

		/// <summary>
		/// Create two Highlight annotations that meet, delete a selection is contained by the first and meets the
		/// second. First truncated.
		/// </summary>
        [Priority(0)]
        private void multicolordelete2_5()
		{
            if (!NonVisibleMode) GoToPage(1);
			CreateHighlight(new SimpleSelectionData(1, 288, 9), 1);
			CreateHighlight(new SimpleSelectionData(1, 297, 5), 0);
			DeleteHighlight(DocViewerWrapper, new SimpleSelectionData(1, 293, 7));
            if (NonVisibleMode) GoToPage(1);
			VerifyAnnotations(new string[] { new SimpleSelectionData(1, 288, 5).GetSelection(DocViewerWrapper.SelectionModule), new SimpleSelectionData(1, 300, 2).GetSelection(DocViewerWrapper.SelectionModule) });
			passTest("Multicolor disjoint 1");
        }

        #endregion BVT TESTS
    }
}	

