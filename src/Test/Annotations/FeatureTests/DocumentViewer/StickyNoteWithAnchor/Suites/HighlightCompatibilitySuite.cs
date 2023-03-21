// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 

using System;
using System.Windows;
using Annotations.Test.Framework;
using System.Windows.Documents;
using System.Reflection;
using System.Windows.Automation;
using System.Windows.Annotations;
using System.Windows.Controls;
using System.Collections;
using System.Collections.Generic;
using Annotations.Test;
using System.Windows.Media;

namespace Avalon.Test.Annotations.Suites
{
    public class HighlightCompatibilitySuite : AStickyNoteWithAnchorSuite
    {
        #region BVT TESTS

        /// <summary>
        /// 1. Create SNwA.
        /// 2. Create overlapping Highlight. 
        /// Verify: resulting appearance is as expected.
        /// </summary>
        [DisabledTestCase()]
        protected void highlight1()
        {
            SetZoom(200);
            CreateStickyNoteWithAnchor(new SimpleSelectionData(0, 1, 200), "");
            CreateHighlight(new SimpleSelectionData(0, 1, 200));

            CompareToMaster("highlight1_master.bmp", "highlight1_tolerance.xml");
            passTest("Highlight/SNwA appeared as expected.");
        }

        /// <summary>
        /// 1. Create SNwA
        /// 2. Create overlapping Highlight.
        /// 3. Call DeleteAnnotationForSelection(Highlight).
        /// Verify: only SNwA remains, and it is selected.
        /// </summary>
        protected void highlight2()
        {
            CreateStickyNoteWithAnchor(new SimpleSelectionData(0, 200, 50), null);
            CreateHighlight(new SimpleSelectionData(0, 200, 50));
            VerifyNumberOfAttachedAnnotations(2);

            ViewerBase.Focus();
            DeleteHighlight(new SimpleSelectionData(0, 200, 50));
            VerifyNumberOfAttachedAnnotations(1);
            IList<StickyNoteControl> sns = AnnotationComponentFinder.GetVisibleStickyNotes(ViewerBase);
            AssertEquals("Verify that there is 1 SNwA.", 1, sns.Count);
            AssertEquals("Verify SNwA is inactive.", StickyNoteState.Inactive, new StickyNoteWrapper(sns[0], "nan1").State);

            passTest("Highlight annotation only was deleted.");
        }

        /// <summary>
        /// 1. Create SNwA
        /// 2. Create overlapping Highlight.
        /// 3. Call DeleteAnnotationForSelection(SNwA).
        /// Verify: only highlight remains.
        /// </summary>
        protected void highlight3()
        {
            CreateStickyNoteWithAnchor(new SimpleSelectionData(0, 900, 25), null);
            CreateHighlight(new SimpleSelectionData(0, 900, 50));
            VerifyNumberOfAttachedAnnotations(2);

            DeleteStickyNoteWithAnchor(new SimpleSelectionData(0, 900, 75));
            VerifyNumberOfAttachedAnnotations(1);
            IList<StickyNoteControl> sns = AnnotationComponentFinder.GetVisibleStickyNotes(ViewerBase);
            AssertEquals("Verify that there is 1 SNwA.", 0, sns.Count);

            passTest("SNwA only was deleted.");
        }

        /// <summary>
        /// 1. Create SNwA
        /// 2. Create Highlight
        /// 3. Un-Highlight part of Highlight
        /// 4. Delete SNwA
        /// </summary>
        protected void highlight9()
        {
            CreateStickyNoteWithAnchor(new SimpleSelectionData(0, 55, 235), null);
            CreateHighlight(new SimpleSelectionData(0, 99, 99));
            VerifyNumberOfAttachedAnnotations(2);
            DeleteHighlight(new SimpleSelectionData(0, 99, 40));
            DeleteStickyNoteWithAnchor(new SimpleSelectionData(0, 55, 235));
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(0, 139, 59))
            });
            passTest("Deleted SNwA and portion of Highlight.");
        }

        /// <summary>
        /// 1. Create SNwA
        /// 2. Create Highlight
        /// 3. Un-Highlight part of Highlight
        /// 4. Delete SNwA
        /// </summary>
        protected void highlight_multicolor1()
        {
            new HighlightDefinition(new SimpleSelectionData(0, 113, 202), Colors.Yellow).Create(DocViewerWrapper);
            new HighlightDefinition(new SimpleSelectionData(0, 178, 60), Colors.Purple).Create(DocViewerWrapper);
            CreateStickyNoteWithAnchor(new SimpleSelectionData(0, 168, 80), null);
            DeleteStickyNoteWithAnchor(new SimpleSelectionData(0, 168, 80));
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(0, 113, 65)) + GetText(new SimpleSelectionData(0, 238, 77)),
                GetText(new SimpleSelectionData(0, 178, 60)),                
            });
            passTest("Deleted SNwA no affect on highlights.");
        }

        /// <summary>
        /// 1. Create SNwA
        /// 2. Create Highlight
        /// 3. Un-Highlight part of Highlight
        /// 4. Delete SNwA
        /// </summary>
        protected void highlight_multicolor2()
        {
            CreateStickyNoteWithAnchor(new SimpleSelectionData(0, 77, 222), null);
            new HighlightDefinition(new SimpleSelectionData(0, 33, 299), Colors.Yellow).Create(DocViewerWrapper);
            new HighlightDefinition(new SimpleSelectionData(0, 99, 178), Colors.Purple).Create(DocViewerWrapper);
            DeleteStickyNoteWithAnchor(new SimpleSelectionData(0, 77, 222));
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(0, 33, 66)) + GetText(new SimpleSelectionData(0, 277, 55)),
                GetText(new SimpleSelectionData(0, 99, 178)),                
            });
            passTest("Deleted SNwA no affect on highlights.");
        }


        #endregion BVT TESTS

        #region PRIORITY TESTS

        /// <summary>
        /// 1. Create Highlight.
        /// 2. Create overlapping SNwA.
        /// Verify: resulting appearance is same as Highlight1
        /// </summary>
        [DisabledTestCase()]
        protected void highlight4()
        {
            CreateHighlight(new SimpleSelectionData(0, 60, 200));
            CreateStickyNoteWithAnchor(new SimpleSelectionData(0, 60, 200), "");

            CompareToMaster("highlight1_master.bmp");
            passTest("Highlight/SNwA appeared as expected.");
        }

		/// <summary>
		/// 1. Create SNwA
		/// 2. Create Highlight behind.
		/// 3. Delete highlight behind.
		/// 4. Delete SNwA.
		/// Verify: no annotations.
		/// </summary>
		protected void highlight5()
		{
			ISelectionData selection = new SimpleSelectionData(0, 60, 200);
			CreateStickyNoteWithAnchor(selection, "");
			CreateHighlight(selection);

			DeleteHighlight(selection);
			VerifyNumberOfAttachedAnnotations(1);
			AssertEquals("Verify SNwA.", 1, AnnotationComponentFinder.GetVisibleStickyNotes(ViewerBase).Count);

			DeleteStickyNoteWithAnchor(selection);
			VerifyNumberOfAttachedAnnotations(0);

			passTest("Verified creating and deleting overlapping highlight/SNwAs.");
		}

		/// <summary>
		/// 1. Create SNwA.
		/// 2. Create Highlight behind.
		/// 3. Focus SNwA.
		/// Verify: anchor color.
		/// </summary>
		protected void highlight6()
		{
			SetZoom(100); // zoom in for fixed.
			StickyNoteWrapper sn = CreateStickyNoteWithAnchor(new SimpleSelectionData(0, 0, 500), "");
			CreateHighlight(new SimpleSelectionData(0, 0, 500));
			sn.ClickIn();
			AssertEquals("Verify sn state.", StickyNoteState.Active_Focused, sn.State);

			passTest("Verified active SN overlaying highlight.");
		}

		/// <summary>
		/// 1. Create SNwA
		/// 2. Create Highlight behind.
		/// 3. Select anchor.
		/// Verfiy: anchor color
		/// </summary>
		protected void highlight7()
		{
			StickyNoteWrapper sn = CreateStickyNoteWithAnchor(new SimpleSelectionData(0, 0, 500), "");
			CreateHighlight(new SimpleSelectionData(0, 0, 500));
			ViewerBase.Focus();
			DocViewerWrapper.SelectionModule.SetSelection(0, 250, 0);
			AssertEquals("Verify sn state.", StickyNoteState.Active_Selected, sn.State);

			passTest("Verified active anchor overlaying highlight.");
		}

        /// <summary>
        /// 1. Create SNwA
        /// 2. Create highlight behind.
        /// 3. Page down.
        /// 4. Page Up.
        /// Verify: SNwA is still infront.
        /// </summary>
        [DisabledTestCase()]
        protected void highlight8_1()
        {
            WholePageLayout();
            PageDown();
            SimpleSelectionData selection = new SimpleSelectionData(1, 100, 500);
            StickyNoteWrapper sn = CreateStickyNoteWithAnchor(selection, "");
            CreateHighlight(selection);
            PageDown();
            PageUp();
            CompareToMaster("highlight8_master.bmp");
            passTest("Verified z order after down/up navigation.");
        }

        /// <summary>
        /// 1. Create SNwA
        /// 2. Create highlight behind.
        /// 3. Page up.
        /// 4. Page down.
        /// Verify: SNwA is still infront.
        /// </summary>
        [DisabledTestCase()]
        protected void highlight8_2()
        {
            WholePageLayout();
            PageDown();
            SimpleSelectionData selection = new SimpleSelectionData(1, 100, 500);
            StickyNoteWrapper sn = CreateStickyNoteWithAnchor(selection, "");
            CreateHighlight(selection);
            PageUp();
            PageDown();
            CompareToMaster("highlight8_master.bmp");
            passTest("Verified z order after up/down navigation.");
        }

        /// <summary>
        /// 1. ----{------}{------}----- adjacent SNwAs
        /// 2. ----{------}{------}----- highlight under 1
        /// 3. ----{------}{------}----- highlight under 2
        /// 4. ----{------}----------- delete right SNwA
        /// 5. --------------------- delete left SNwA � single highlight.
        /// </summary>
        [DisabledTestCase()]
        protected void highlight11()
        {
            ISelectionData selection1 = new SimpleSelectionData(0, 100, 50);
            ISelectionData selection2 = new SimpleSelectionData(0, 150, 50);
            CreateStickyNoteWithAnchor(selection1, null);
            CreateStickyNoteWithAnchor(selection2, null);
            VerifyNumberOfAttachedAnnotations("2 SNwAs.", 2);
            CreateHighlight(selection1);
            CreateHighlight(selection2); // will be merged so count will not increase for this create.
            VerifyNumberOfAttachedAnnotations("+2 highlight.", 4);
            DeleteStickyNoteWithAnchor(selection2);
            DeleteStickyNoteWithAnchor(selection1);
            VerifyNumberOfAttachedAnnotations("only highlights.", 2);

            CompareToMaster("highlight11_master.bmp");
            passTest("Verified merge behind StickyNotes.");
        }

        /// <summary>
        /// 1. Create SNwA
        /// 2. Create Highlight
        /// 3. Un-Highlight to split highlight
        /// 4. Delete SNwA
        /// </summary>
        protected void highlight10()
        {
            GoToPage(2);
            CreateStickyNoteWithAnchor(new SimpleSelectionData(2, 101, 599), null);
            CreateHighlight(new SimpleSelectionData(2, 350, 200));
            VerifyNumberOfAttachedAnnotations(2);
            DeleteHighlight(new SimpleSelectionData(2, 417, 109));
            DeleteStickyNoteWithAnchor(new SimpleSelectionData(2, 101, 599));
            VerifyAnnotation(
                GetText(new SimpleSelectionData(2, 350, 67)) +
                GetText(new SimpleSelectionData(2, 526, 24))
            );
            passTest("Deleted SNwA and split Highlight.");
        }

        /// <summary>
        /// 1. Create SNwA
        /// 2. Create multiple non overlapping highlight
        /// 3. create one overlapping highlight
        /// 4. Delete SNwA Verify 1 highlight.
        /// </summary>
        protected void highlight_multicolor3()
        {
            CreateStickyNoteWithAnchor(new SimpleSelectionData(0, 101, 404), null);
            new HighlightDefinition(new SimpleSelectionData(0, 101, 100), Colors.Pink).Create(DocViewerWrapper);
            new HighlightDefinition(new SimpleSelectionData(0, 405, 100), Colors.Yellow).Create(DocViewerWrapper);
            new HighlightDefinition(new SimpleSelectionData(0, 250, 100), Colors.Pink).Create(DocViewerWrapper);
            VerifyNumberOfAttachedAnnotations(4);
            new HighlightDefinition(new SimpleSelectionData(0, 101, 404), Colors.Pink).Create(DocViewerWrapper);
            DeleteStickyNoteWithAnchor(new SimpleSelectionData(0, 101, 404));
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(0, 101, 404))
            });
            passTest("Deleted SNwA One Highlight.");
        }

        /// <summary>
        /// 1. Create SNwA
        /// 2. Create highlight
        /// 3. create one overlapping highlight that splits first highlight
        /// 4. Delete SNwA Verify 3 highlight.
        /// </summary>
        protected void highlight_multicolor4()
        {
            GoToPage(2);

            CreateStickyNoteWithAnchor(new SimpleSelectionData(2, 550, 433), null);
            new HighlightDefinition(new SimpleSelectionData(2, 550, 433), Colors.RoyalBlue).Create(DocViewerWrapper);
            VerifyNumberOfAttachedAnnotations(2);
            new HighlightDefinition(new SimpleSelectionData(2, 601, 78), Colors.Orange).Create(DocViewerWrapper);
            DeleteStickyNoteWithAnchor(new SimpleSelectionData(2, 550, 433));
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(2, 550, 51)) + GetText(new SimpleSelectionData(2, 679, 304)),
                GetText(new SimpleSelectionData(2, 601, 78))
            });
            passTest("Deleted SNwA Three Highlight.");
        }

        #endregion PRIORITY TESTS
    }
}	

