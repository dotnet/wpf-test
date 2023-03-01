// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 

using System;
using System.Windows;
using Annotations.Test.Framework;
using System.Windows.Automation;
using System.Collections.Generic;
using Proxies.System.Windows.Controls;
using System.Windows.Documents;
using Microsoft.Test.Display;

namespace Avalon.Test.Annotations.Suites
{
    public class StateSuite : AStateSuite
    {
        #region BVT TESTS

        [TestCase_Setup()]
        protected override void DoSetup()
        {
            base.DoSetup();

            AnnotationTestHelper.BringToFront(MainWindow);
        }

        #region Single SNwA Tests

        /// <summary>
        /// Adjacent caret R, forward.
        /// </summary>
        [TestDimension(",minimized")]
        protected void state_single1_1()
        {
            TestState(new SimpleSelectionData(0, 100, 10), new SimpleSelectionData(0, 110, LogicalDirection.Forward), StickyNoteState.Inactive);
            passTest("Inactive: Adjacent caret R, forwards.");
        }

        /// <summary>
        /// Adjacent caret R, backward.
        /// </summary>
        protected void state_single1_2()
        {
            TestState(new SimpleSelectionData(0, 100, 10), new SimpleSelectionData(0, 110, LogicalDirection.Backward), StickyNoteState.Active_Selected);
            passTest("Active: Adjacent caret R, backwards.");
        }

        /// <summary>
        /// Adjacent caret L, backward
        /// </summary>
        protected void state_single5_1()
        {
            state_single5_X(LogicalDirection.Backward, StickyNoteState.Inactive);
            passTest("Active: Adjacent caret L, backwards.");
        }

        /// <summary>
        /// Adjacent caret L, forward
        /// </summary>
        [TestDimension(",minimized")]
        protected void state_single5_2()
        {
            state_single5_X(LogicalDirection.Forward, StickyNoteState.Active_Selected);
            passTest("Active: Adjacent caret L, backwards.");
        }
        [TestCase_Helper()]
        private void state_single5_X(LogicalDirection dir, StickyNoteState state)
        {
            TestState(new SimpleSelectionData(0, 100, 10), new SimpleSelectionData(0, 100, dir), state);
        }

        /// <summary>
        /// Caret in middle.
        /// </summary>
        [TestDimension(",minimized")]
        protected void state_single6()
        {
            TestState(new SimpleSelectionData(0, 100, 10), new SimpleSelectionData(0, 105, 0), StickyNoteState.Active_Selected);
            passTest("Active: Caret in middle.");
        }

        /// <summary>
        /// Full selection overlap.
        /// </summary>
        [TestDimension(",minimized")]
        protected void state_single7()
        {
            TestState(new SimpleSelectionData(0, 100, 10), new SimpleSelectionData(0, 100, 10), StickyNoteState.Active_Selected);
            passTest("Active: Full selection overlap.");
        }

        /// <summary>
        /// Page down.
        /// Page up. Verify still selected.
        /// </summary>
        protected void state_single17()
        {
            TestState(new SimpleSelectionData(0, 100, 10), new SimpleSelectionData(0, 100, 10), StickyNoteState.Active_Selected);
            PageDown();
            PageUp();
            AssertEquals("Verify final state.", StickyNoteState.Active_Selected, CurrentlyAttachedStickyNote.State);
            passTest("Verified state after navigation.");
        }

        #endregion

        #region Multiple SnwA Tests

        /// <summary>
        /// Shared Start: -----------{{I-}---}--------
        /// A inactive, B inactive.
        /// </summary>
        protected void state_multi1()
        {
            TestState(
                new SimpleSelectionData(0, 400, 10), // anchorA
                new SimpleSelectionData(0, 400, 20), // anchorB
                new SimpleSelectionData(0, 400, LogicalDirection.Backward),  // selection.
                StickyNoteState.Inactive,        // state A.
                StickyNoteState.Inactive);		 // state B.
            passTest("Both StickyNotes selected.");
        }

        /// <summary>
        /// Shared end: -----------{----{-}}I>--------
        /// A inactive, B inactive.
        /// </summary>
        protected void state_multi2()
        {
            TestState(
                new SimpleSelectionData(0, 300, 50), // anchorA
                new SimpleSelectionData(0, 250, 100), // anchorB
                new SimpleSelectionData(0, 350, LogicalDirection.Forward),  // selection.
                StickyNoteState.Inactive,        // state A.
                StickyNoteState.Inactive);		 // state B.
            passTest("Both StickyNotes unselected.");
        }

        #endregion

        #region Single Transition Tests

        /// <summary>
        /// Create new SNwA, selected: -----{I------}-------
        /// </summary>
        protected void state_transition_single1()
        {
            PageDown();
            DocViewerWrapper.SelectionModule.SetSelection(1, 100, 50);
            CreateStickyNoteWithAnchor();
            AssertEquals("Verify State.", StickyNoteState.Active_Focused, CurrentlyAttachedStickyNote.State);
            passTest("Selected focused created.");
        }

        /// <summary>
        /// 1. -----{----I--}------- Selected SNwA 
        /// 2. -----{------}----I--- Click elsewhere on same page, not selected : 
        /// </summary>
        protected void state_transition_single2()
        {
            DocViewerWrapper.SelectionModule.SetSelection(0, 400, 19);
            StickyNoteWrapper sn = CreateStickyNoteWithAnchor();
            DocViewerWrapper.SelectionModule.SetSelection(0, 410, 0);
            ViewerBase.Focus();
            AssertEquals("Verify initial State.", StickyNoteState.Active_Selected, sn.State);
            DocViewerWrapper.SelectionModule.SetSelection(0, 500, 0);
            AssertEquals("Verify final State.", StickyNoteState.Inactive, sn.State);
            passTest("Selected->Unselected.");
        }

        #endregion

        #region Multiple Transition Tests

        /// <summary>
        /// 1. Two SNwAs, annot A is selected:   -----{----I--}-------{--------}---------
        /// 2. Click in annot B, B is now selected: -----{-------}-------{--I-----}---------
        /// </summary>
        [TestDimension(",minimized")]
        protected void state_transition_multiple1()
        {
            StickyNoteWrapper[] wrappers = TestState(
                new SimpleSelectionData(0, 10, 25),	// anchorA
                new SimpleSelectionData(0, 40, 25),	// anchorB
                new SimpleSelectionData(0, 20, 0),	// selection.
                StickyNoteState.Active_Selected,	// state A.
                StickyNoteState.Inactive);			// state B.

            DocViewerWrapper.SelectionModule.SetSelection(0, 50, 0);
            AssertEquals("Verify final State A.", StickyNoteState.Inactive, wrappers[0].State);
            AssertEquals("Verify final State B.", StickyNoteState.Active_Selected, wrappers[1].State);

            passTest("Verified state transition between multiple annots.");
        }

        #endregion

        #region Active Tests

        /// <summary>
        /// Create new SNwA.  Focused.
        /// </summary>
        protected void state_active1()
        {
            DocViewerWrapper.SelectionModule.SetSelection(0, 0, 10);
            CreateStickyNoteWithAnchor();
            AssertEquals("Verify state.", StickyNoteState.Active_Focused, CurrentlyAttachedStickyNote.State);
            passTest("SN Focused by default.");
        }

        /// <summary>
        /// Create new SNwA, click in it, now active.
        /// </summary>
        protected void state_active2()
        {
            SetZoom(100); // Zoom in for fixed.
            DocViewerWrapper.SelectionModule.SetSelection(0, 0, 10);
            StickyNoteWrapper sn = CreateStickyNoteWithAnchor();
            sn.ClickIn();
            AssertEquals("Verify State.", StickyNoteState.Active_Focused, sn.State);
            passTest("SN active after mouse click.");
        }

        /// <summary>
        /// -----------{--[-}--]-------- 
        /// Focus SN A. Verify: anchor color.
        /// Focus SN B. Verify: anchor color.
        /// </summary>
        [DisabledTestCase()]
        protected void state_active5()
        {
            if (ContentMode == TestMode.Flow) SetZoom(150);
            else PageWidthLayout();

            StickyNoteWrapper[] wrappers = new StickyNoteWrapper[2];
            wrappers[0] = CreateStickyNoteWithAnchor(new SimpleSelectionData(0, 0, 1000), "A");
            wrappers[1] = CreateStickyNoteWithAnchor(new SimpleSelectionData(0, 500, 1000), "B");

            wrappers[0].ClickIn();
            CompareToMaster("active5A_master.bmp", "active5_tolerance.xml");

            wrappers[1].ClickIn();
            CompareToMaster("active5B_master.bmp", "active5_tolerance.xml");

            passTest("Verified transition between states.");
        }

        /// <summary>
        /// ----{-----{-I-}---}--- both anchors are active.
        /// Click in sn A.  SN A is active, anchor B is inactive.
        /// Focus DV, both anchors are active.
        /// </summary>
        protected void state_active13()
        {
            StickyNoteWrapper[] wrappers = new StickyNoteWrapper[2];
            wrappers[0] = CreateStickyNoteWithAnchor(new SimpleSelectionData(0, 100, 100), "A");
            wrappers[1] = CreateStickyNoteWithAnchor(new SimpleSelectionData(0, 150, 100), "B");

            ViewerBase.Focus();
            DocViewerWrapper.SelectionModule.SetSelection(0, 175, 0);
            VerifyStates("Both selected", wrappers, new StickyNoteState[] { StickyNoteState.Active_Selected, StickyNoteState.Active_Selected });

            wrappers[0].ClickIn();
            VerifyStates("A Focused", wrappers, new StickyNoteState[] { StickyNoteState.Active_Focused, StickyNoteState.Inactive });

            ViewerBase.Focus();
            VerifyStates("DV focused: both selected", wrappers, new StickyNoteState[] { StickyNoteState.Active_Selected, StickyNoteState.Active_Selected });

            passTest("Verified transition between focus activation and selection activation.");
        }

        #endregion

        #endregion BVT TESTS

        #region PRIORITY TESTS

        #region Single SNwA Tests

        /// <summary>
		/// Adjacent selection R, forwards.
		/// </summary>
		protected void state_single2_1()
		{
			state_single2_X(LogicalDirection.Forward, StickyNoteState.Inactive);
			passTest("Verified inactive anchor.");
		}

		/// <summary>
		/// Adjacent selection R, backwards.
		/// </summary>
		protected void state_single2_2()
		{
			state_single2_X(LogicalDirection.Backward, StickyNoteState.Active_Selected);
			passTest("Verified active anchor.");
		}

		private void state_single2_X(LogicalDirection dir, StickyNoteState state)
		{
			TestState(new SimpleSelectionData(0, 100, 10), new SimpleSelectionData(0, 110, dir), state);
		}

		/// <summary>
		/// Caret 1 char to L.
		/// </summary>
		protected void state_single3()
		{
			TestState(new SimpleSelectionData(0, 100, 10), new SimpleSelectionData(0, 100, LogicalDirection.Backward), StickyNoteState.Inactive);
			passTest("Verified inactive anchor.");
		}		

		/// <summary>
		/// 1 character long, R 1 char.
		/// </summary>
		protected void state_single4_X(LogicalDirection dir, StickyNoteState state)
		{
			TestState(new SimpleSelectionData(0, 100, 1), new SimpleSelectionData(0, 101, dir), state);
			passTest("Verified anchor:" + state.ToString());
		}
		protected void state_single4_1() { state_single4_X(LogicalDirection.Forward, StickyNoteState.Inactive); }
		protected void state_single4_2() { state_single4_X(LogicalDirection.Backward, StickyNoteState.Active_Selected); }

		/// <summary>
		/// 1 char overlap R.
		/// </summary>
		protected void state_single8()
		{
			TestState(new SimpleSelectionData(0, 100, 10), new SimpleSelectionData(0, 109, 0), StickyNoteState.Active_Selected);
			passTest("Verified active anchor.");
		}

		/// <summary>
		/// Adjacent selection L.
		/// </summary>
		protected void state_single9()
		{
			TestState(new SimpleSelectionData(0, 100, 10), new SimpleSelectionData(0, 100, -25), StickyNoteState.Inactive);
			passTest("Verified active anchor.");
		}

		/// <summary>
		/// Encompassing.
		/// </summary>
		protected void state_single10()
		{
			TestState(new SimpleSelectionData(0, 100, 10), new SimpleSelectionData(0, 90, 25), StickyNoteState.Active_Selected);
			passTest("Verified active anchor.");
		}

		/// <summary>
		/// Whole page.
		/// </summary>
		protected void state_single11()
		{
			TestState(new SimpleSelectionData(0, 100, 10), new MultiPageSelectionData(0, PagePosition.Beginning, 0, 0, PagePosition.End, 0), StickyNoteState.Active_Selected);
			passTest("Verified active anchor.");
		}

		/// <summary>
		/// 1st page of multiple pages (both pages visible).
		/// </summary>
		private void state_single12_1_X(LogicalDirection dir, StickyNoteState state)
		{
			if (ContentMode == TestMode.Flow) ViewAsTwoPages();
			GoToPageRange(1, 2);
			TestState(new MultiPageSelectionData(1, PagePosition.End, -20, 2, PagePosition.Beginning, 20), new SimpleSelectionData(1, PagePosition.End, dir), state);
			passTest("Verified anchor: " + state.ToString());
		}
		
		[DisabledTestCase()]
        protected void state_single12_1_1() { state_single12_1_X(LogicalDirection.Forward, StickyNoteState.Active_Selected); }
		protected void state_single12_1_2() { state_single12_1_X(LogicalDirection.Backward, StickyNoteState.Active_Selected); }

		/// <summary>
		/// 1st page of multiple pages (1st page visible).
		/// </summary>
		protected void state_single12_2_X(LogicalDirection dir, StickyNoteState state)
		{
			PageDown();
			TestState(new MultiPageSelectionData(1, PagePosition.End, -20, 2, PagePosition.Beginning, 20), new SimpleSelectionData(1, PagePosition.End, dir), state);
			passTest("Verified active anchor.");
		}
        
        [DisabledTestCase()]
        protected void state_single12_2_1() { state_single12_2_X(LogicalDirection.Forward, StickyNoteState.Active_Selected); }
		protected void state_single12_2_2() { state_single12_2_X(LogicalDirection.Backward, StickyNoteState.Active_Selected); }

		/// <summary>
		/// 1st page of multiple pages (2nd page visible).
		/// </summary>
		protected void state_single12_3_X(LogicalDirection dir, StickyNoteState state)
		{
			GoToPage(2);
			TestState(new MultiPageSelectionData(1, PagePosition.End, -20, 2, PagePosition.Beginning, 20), new SimpleSelectionData(1, PagePosition.End, dir), state);
			passTest("Verified active anchor.");
		}
        
		[DisabledTestCase()]
        protected void state_single12_3_1() { state_single12_3_X(LogicalDirection.Forward, StickyNoteState.Active_Selected); }
		protected void state_single12_3_2() { state_single12_3_X(LogicalDirection.Backward, StickyNoteState.Active_Selected); }

		/// <summary>
		/// 2nd page of multiple pages (both pages visible).
		/// </summary>
		protected void state_single13_1_X(LogicalDirection dir, StickyNoteState state)
		{
			if (ContentMode == TestMode.Flow) ViewAsTwoPages();
			GoToPageRange(1, 2);
			TestState(new MultiPageSelectionData(1, PagePosition.End, -20, 2, PagePosition.Beginning, 20), new SimpleSelectionData(2, PagePosition.Beginning, dir), state);
			passTest("Verified active anchor.");
		}
        
		[DisabledTestCase()]
        protected void state_single13_1_1() { state_single13_1_X(LogicalDirection.Backward, StickyNoteState.Active_Selected); }
		protected void state_single13_1_2() { state_single13_1_X(LogicalDirection.Forward, StickyNoteState.Active_Selected); }

		/// <summary>
		/// 2nd page of multiple pages (2nd page visible).
		/// </summary>
		protected void state_single13_2_X(LogicalDirection dir, StickyNoteState state)
		{
			GoToPage(2);
			TestState(new MultiPageSelectionData(1, PagePosition.End, -20, 2, PagePosition.Beginning, 20), new SimpleSelectionData(2, PagePosition.Beginning, dir), state);
			passTest("Verified active anchor.");
		}
        
        [DisabledTestCase()]
        protected void state_single13_2_1() { state_single13_2_X(LogicalDirection.Backward, StickyNoteState.Active_Selected); }
		protected void state_single13_2_2() { state_single13_2_X(LogicalDirection.Forward, StickyNoteState.Active_Selected); }

		/// <summary>
		/// 2nd page of multiple pages (1st page visible).
		/// </summary>
		protected void state_single13_3_X(LogicalDirection dir, StickyNoteState state)
		{
			GoToPage(1);
			TestState(new MultiPageSelectionData(1, PagePosition.End, -20, 2, PagePosition.Beginning, 20), new SimpleSelectionData(2, PagePosition.Beginning, dir), state);
			passTest("Verified active anchor.");
		}
        
		[DisabledTestCase()]
        protected void state_single13_3_1() { state_single13_3_X(LogicalDirection.Backward, StickyNoteState.Active_Selected); }
		protected void state_single13_3_2() { state_single13_3_X(LogicalDirection.Forward, StickyNoteState.Active_Selected); }


		/// <summary>
		/// Caret 1 char inside R.
		/// </summary>
		protected void state_single14()
		{
			TestState(new SimpleSelectionData(0, 100, 10), new SimpleSelectionData(0, 109, 0), StickyNoteState.Active_Selected);
			passTest("Verified active anchor.");
		}

		/// <summary>
		/// 1 character long anchor.
		/// </summary>
		protected void state_single15_X(SimpleSelectionData pos, StickyNoteState state)
		{
			TestState(new SimpleSelectionData(0, 100, 1), pos, state);
			passTest("Verified active anchor.");
		}
		protected void state_single15_1() { state_single15_X(new SimpleSelectionData(0, 100, LogicalDirection.Backward), StickyNoteState.Inactive); }
		protected void state_single15_2() { state_single15_X(new SimpleSelectionData(0, 100, LogicalDirection.Forward), StickyNoteState.Active_Selected); }
		protected void state_single15_3() { state_single15_X(new SimpleSelectionData(0, 101, LogicalDirection.Forward), StickyNoteState.Inactive); }
		protected void state_single15_4() { state_single15_X(new SimpleSelectionData(0, 101, LogicalDirection.Backward), StickyNoteState.Active_Selected); }

		/// <summary>
		/// 1 character long, selected, active.
		/// </summary>
		protected void state_single16()
		{
			TestState(new SimpleSelectionData(0, 100, 1), new SimpleSelectionData(0, 100, 1), StickyNoteState.Active_Selected);
			passTest("Verified active anchor.");
		}

		/// <summary>
		/// Anchor at end of page. Caret adjacent R.
		/// </summary>
		protected void state_single18_1()
		{
			GoToPage(1);
			TestState(new SimpleSelectionData(1, PagePosition.End, -100), new SimpleSelectionData(1, PagePosition.End, 0), StickyNoteState.Active_Selected);
			passTest("Verified selected anchor.");
		}

		/// <summary>
		/// Anchor at end of page. Caret adjacent R.
		/// Part of next page is visible.
		/// </summary>
		protected void state_single18_2()
		{
			if (ContentMode == TestMode.Flow) ViewAsTwoPages();
			GoToPageRange(1, 2);
			TestState(new SimpleSelectionData(1, PagePosition.End, -100), new SimpleSelectionData(1, PagePosition.End, 0), StickyNoteState.Active_Selected);
			passTest("Verified selected anchor.");
		}

		/// <summary>
		/// Anchor at end of document. Caret adjacent R.
		/// </summary>
		protected void state_single19()
		{
			int lastPage = DocViewerWrapper.PageCount-1;
			GoToLastPage();
			TestState(new SimpleSelectionData(lastPage, PagePosition.End, -100), new SimpleSelectionData(lastPage, PagePosition.End, 0), StickyNoteState.Active_Selected);
			passTest("Verified selected anchor.");
		}

		/// <summary>
		/// Anchor at start of page. Caret adjacent L. 1 page visible.
		/// </summary>
		protected void state_single20_1()
		{
			GoToPage(1);
			TestState(new SimpleSelectionData(1, PagePosition.Beginning, 100), new SimpleSelectionData(1, PagePosition.Beginning, 0), StickyNoteState.Active_Selected);
			passTest("Verified selected anchor.");
		}

		/// <summary>
		/// Anchor at start of page. Caret adjacent L. 1 page visible.
		/// Part of previous page is visible.
		/// </summary>
		protected void state_single20_2()
		{
			if (ContentMode == TestMode.Flow) ViewAsTwoPages();
			GoToPageRange(0, 1);
			TestState(new SimpleSelectionData(1, PagePosition.Beginning, 100), new SimpleSelectionData(1, PagePosition.Beginning, 0), StickyNoteState.Active_Selected);
			passTest("Verified selected anchor.");
		}

		/// <summary>
		/// Anchor at start of document. Caret adjacent L.
		/// </summary>
		protected void state_single21()
		{
			TestState(new SimpleSelectionData(0, PagePosition.Beginning, 100), new SimpleSelectionData(0, PagePosition.Beginning, 0), StickyNoteState.Active_Selected);
			passTest("Verified selected anchor.");
		}

		/// <summary>
		/// Zoom in. Verify still selected.
		/// </summary>
		protected void state_single22_1()
		{
			TestState(new SimpleSelectionData(0, 10, 100), new SimpleSelectionData(0, 50, 0), StickyNoteState.Active_Selected);
			SetZoom(120);
			AssertEquals("Verify final state.", StickyNoteState.Active_Selected, CurrentlyAttachedStickyNote.State);
			passTest("Verified active anchor after zoom.");
		}
		/// <summary>
		/// Resize window. Verify still selected.
		/// </summary>
		protected void state_single22_2()
		{
			TestState(new SimpleSelectionData(0, 10, 100), new SimpleSelectionData(0, 50, 0), StickyNoteState.Active_Selected);
			ChangeWindowWidth(-50);
			ChangeWindowHeight(+50);		
			AssertEquals("Verify final state.", StickyNoteState.Active_Selected, CurrentlyAttachedStickyNote.State);
			passTest("Verified active anchor after window resize.");
		}

		#endregion

		#region Multiple SNwA Tests

		/// <summary>
		/// Caret between.
		/// </summary>
		protected void state_multi3()
		{
			TestState(
				new SimpleSelectionData(0, 400, 5),		// anchorA
				new SimpleSelectionData(0, 450, 5),		// anchorB
				new SimpleSelectionData(0, 425, 0),		// selection.
				StickyNoteState.Inactive,				// state A.
				StickyNoteState.Inactive);				// state B.
			passTest("Both anchors inactive.");
		}

		/// <summary>
		/// Caret in one, other is inactive
		/// </summary>
		protected void state_multi4()
		{
			TestState(
				new SimpleSelectionData(0, 400, 5),		// anchorA
				new SimpleSelectionData(0, 450, 5),		// anchorB
				new SimpleSelectionData(0, 402, 0),		// selection.
				StickyNoteState.Active_Selected,		// state A.
				StickyNoteState.Inactive);				// state B.
			passTest("Only 1 anchor active.");
		}

		/// <summary>
		/// -----------{---{-}-I-}--------
		/// A inactive, B active.
		/// </summary>
		protected void state_multi5()
		{
			TestState(
				new SimpleSelectionData(0, 400, 50),	// anchorA
				new SimpleSelectionData(0, 449, 50),	// anchorB
				new SimpleSelectionData(0, 475, 0),		// selection.
				StickyNoteState.Inactive,				// state A.
				StickyNoteState.Active_Selected);		// state B.
			passTest("Anchor B active.");
		}

		/// <summary>
		/// -----------{-I-{-}- -}--------
		/// A active, B inactive.
		/// </summary>
		protected void state_multi6()
		{
			TestState(
				new SimpleSelectionData(0, 400, 50),	// anchorA
				new SimpleSelectionData(0, 449, 50),	// anchorB
				new SimpleSelectionData(0, 425, 0),		// selection.
				StickyNoteState.Active_Selected,		// state A.
				StickyNoteState.Inactive);				// state B.
			passTest("Anchor A active.");
		}

		/// <summary>
		/// -----------{--{-I}- -}--------
		/// A inactive, B active.
		/// </summary>
		protected void state_multi7()
		{
			TestState(
				new SimpleSelectionData(0, 400, 50),	// anchorA
				new SimpleSelectionData(0, 449, 50),	// anchorB
				new SimpleSelectionData(0, 450, LogicalDirection.Forward),		// selection.
				StickyNoteState.Inactive,				// state A.
				StickyNoteState.Active_Selected);		// state B.
			passTest("Anchor B active.");
		}

		/// <summary>
		/// -----------{--{I-}- -}--------
		/// A active, B inactive
		/// </summary>
		protected void state_multi8()
		{
			TestState(
				new SimpleSelectionData(0, 400, 50),	// anchorA
				new SimpleSelectionData(0, 449, 50),	// anchorB
				new SimpleSelectionData(0, 449, LogicalDirection.Backward),		// selection.
				StickyNoteState.Active_Selected,		// state A.
				StickyNoteState.Inactive);				// state B.
			passTest("Both active.");
		}

		/// <summary>
		/// -----------{{-I}---}--------
		/// A inactive, B active.
		/// </summary>
		protected void state_multi9()
		{
			TestState(
				new SimpleSelectionData(0, 400, 1),		// anchorA
				new SimpleSelectionData(0, 400, 50),	// anchorB
				new SimpleSelectionData(0, 401, LogicalDirection.Forward),		// selection.
				StickyNoteState.Inactive,				// state A.
				StickyNoteState.Active_Selected);		// state B.
			passTest("Anchor B active.");
		}

		/// <summary>
		/// -----------{{------I}}--------
		/// A inactive, B inactive.
		/// </summary>
		protected void state_multi10()
		{
			TestState(
				new SimpleSelectionData(0, 400, 50),	// anchorA
				new SimpleSelectionData(0, 400, 50),	// anchorB
				new SimpleSelectionData(0, 450, LogicalDirection.Forward),		// selection.
				StickyNoteState.Inactive,				// state A.
				StickyNoteState.Inactive);				// state B.
			passTest("Both inactive.");
		}

		/// <summary>
		/// -----------{{---I---}}--------
		/// A active, B active.
		/// </summary>
		protected void state_multi11()
		{
			TestState(
				new SimpleSelectionData(0, 400, 50),	// anchorA
				new SimpleSelectionData(0, 400, 50),	// anchorB
				new SimpleSelectionData(0, 425, 0),		// selection.
				StickyNoteState.Active_Selected,		// state A.
				StickyNoteState.Active_Selected);		// state B.
			passTest("Both active.");
		}

		/// <summary>
		/// ----------{{-------}}--------
		/// Encompassing selection.
		/// </summary>
		protected void state_multi12()
		{
			TestState(
				new SimpleSelectionData(0, 400, 50),	// anchorA
				new SimpleSelectionData(0, 400, 50),	// anchorB
				new SimpleSelectionData(0, 350, 200),	// selection.
				StickyNoteState.Active_Selected,		// state A.
				StickyNoteState.Active_Selected);		// state B.
			passTest("Both active.");
		}

		/// <summary>
		/// ----{----}------{----}------
		/// Partial overlapping selection.
		/// </summary>
		protected void state_multi13()
		{
			TestState(
				new SimpleSelectionData(0, 500, 10),	// anchorA
				new SimpleSelectionData(0, 520, 10),	// anchorB
				new SimpleSelectionData(0, 505, 20),	// selection.
				StickyNoteState.Active_Selected,		// state A.
				StickyNoteState.Active_Selected);		// state B.
			passTest("Both active.");
		}

		/// <summary>
		/// --------{----}I{----}------
		/// A inactive, B inactive.
		/// </summary>
		protected void state_multi14_X(LogicalDirection dir, StickyNoteState stateA, StickyNoteState stateB)
		{
			TestState(
				new SimpleSelectionData(0, 500, 10),	// anchorA
				new SimpleSelectionData(0, 510, 10),	// anchorB
				new SimpleSelectionData(0, 510, dir),	// selection.
				stateA,				// state A.
				stateB);				// state B.
			passTest("Anchor A is '" + stateA.ToString() + "' B is '" + stateB.ToString() + "'.");
		}
		protected void state_multi14_1() { state_multi14_X(LogicalDirection.Backward, StickyNoteState.Active_Selected, StickyNoteState.Inactive); }
		protected void state_multi14_2() { state_multi14_X(LogicalDirection.Forward, StickyNoteState.Inactive, StickyNoteState.Active_Selected); }

		/// <summary>
		/// --------{----}ssssss{----}------
		/// A inactive, B inactive.
		/// </summary>
		protected void state_multi15()
		{
			TestState(
				new SimpleSelectionData(0, 300, 50),	// anchorA
				new SimpleSelectionData(0, 370, 50),	// anchorB
				new SimpleSelectionData(0, 350, 20),	// selection.
				StickyNoteState.Inactive,				// state A.
				StickyNoteState.Inactive);				// state B.
			passTest("Anchor B active.");
		}

		/// <summary>
		/// --------{----}------{----}------
		/// Encompassing selection.
		/// </summary>
		protected void state_multi16()
		{
			TestState(
				new SimpleSelectionData(0, 300, 50),	// anchorA
				new SimpleSelectionData(0, 370, 50),	// anchorB
				new SimpleSelectionData(0, 250, 500),	// selection.
				StickyNoteState.Active_Selected,		// state A.
				StickyNoteState.Active_Selected);		// state B.
			passTest("Both active.");
		}

		/// <summary>
		/// -----------{---{---}I ---}------
		/// A active, B inactive.
		/// </summary>
		protected void state_multi17()
		{
			TestState(
				new SimpleSelectionData(0, 300, 50),	// anchorA
				new SimpleSelectionData(0, 310, 30),	// anchorB
				new SimpleSelectionData(0, 340, LogicalDirection.Forward),		// selection.
				StickyNoteState.Active_Selected,		// state A.
				StickyNoteState.Inactive);				// state B.
			passTest("Anchor A active.");
		}

		/// <summary>
		/// -----------{---I{---} ---}------
		/// A active, B inactive.
		/// </summary>
		protected void state_multi18()
		{
			TestState(
				new SimpleSelectionData(0, 300, 50),	// anchorA
				new SimpleSelectionData(0, 310, 30),	// anchorB
				new SimpleSelectionData(0, 310, LogicalDirection.Backward),		// selection.
				StickyNoteState.Active_Selected,		// state A.
				StickyNoteState.Inactive);				// state B.
			passTest("Anchor A active.");
		}

		/// <summary>
		/// ---{{----I}|---}
		/// Both pages visible
		/// A active, B inactive.
		/// </summary>
        
		[DisabledTestCase()]
        protected void state_multi21()
		{
			if (ContentMode == TestMode.Flow) ViewAsTwoPages();
			GoToPageRange(1,2);
			TestState(
				new MultiPageSelectionData(1, PagePosition.End, -10, 2, PagePosition.Beginning, 10),	// anchorA
				new SimpleSelectionData(1, PagePosition.End, -10),										// anchorB
				new SimpleSelectionData(1, PagePosition.End, LogicalDirection.Forward),					// selection.
				StickyNoteState.Active_Selected,		// state A.
				StickyNoteState.Inactive);				// state B.
			passTest("Anchor A active.");
		}

		/// <summary>
		/// ---{--|{I----}}
		/// Both pages visible
		/// A active, B inactive.
		/// </summary>
        
		[DisabledTestCase()]
        protected void state_multi22()
		{
			if (ContentMode == TestMode.Flow) ViewAsTwoPages();
			GoToPageRange(1, 2);
			TestState(
				new MultiPageSelectionData(1, PagePosition.End, -10, 2, PagePosition.Beginning, 10),	// anchorA
				new SimpleSelectionData(2, PagePosition.Beginning, 10),									// anchorB
				new SimpleSelectionData(2, PagePosition.Beginning, LogicalDirection.Backward),			// selection.
				StickyNoteState.Active_Selected,		// state A.
				StickyNoteState.Inactive);				// state B.
			passTest("Anchor A active.");
		}	

		#endregion

		#region Single Transition Tests

		/// <summary>
		/// 1. -----{----I--}------- Selected SNwA
		/// 2. Click on other page, not selected.
		/// </summary>
		protected void state_transition_single3()
		{
			if (ContentMode == TestMode.Flow) ViewAsTwoPages();

			GoToPageRange(1, 2);
			DocViewerWrapper.SelectionModule.SetSelection(1, PagePosition.End, -10);
			StickyNoteWrapper sn = CreateStickyNoteWithAnchor();
			ViewerBase.Focus();
			DocViewerWrapper.SelectionModule.SetSelection(1, PagePosition.End, -10, 1, PagePosition.End, -5);
			AssertEquals("Verify Initial State.", StickyNoteState.Active_Selected, sn.State);
			DocViewerWrapper.SelectionModule.SetSelection(2, PagePosition.Beginning, 0);
			AssertEquals("Verify final State.", StickyNoteState.Inactive, sn.State);
			passTest("Unselected after cursor moves to another page.");
		}

		/// <summary>
		/// a. -----{----I--}------- 
		/// b. -----{-I-----}------- select elsewhere
		/// </summary>
		protected void state_transition_single4()
		{
			DocViewerWrapper.SelectionModule.SetSelection(0, 0, 52);
			StickyNoteWrapper sn = CreateStickyNoteWithAnchor();
			ViewerBase.Focus();
			DocViewerWrapper.SelectionModule.SetSelection(0, 10, 0);
			AssertEquals("Verify Initial State.", StickyNoteState.Active_Selected, sn.State);
			DocViewerWrapper.SelectionModule.SetSelection(0, 50, 0);
			AssertEquals("Verify final State.", StickyNoteState.Active_Selected, sn.State);
			passTest("Still selected after moving caret.");
		}

		/// <summary>
		/// Cursor on first page of selection, move cursor to second page of selection, SNwA is still Selected.
		/// Both pages visible.
		/// </summary>
		protected void state_transition_single5_1()
		{
			if (ContentMode == TestMode.Flow) ViewAsTwoPages();
			GoToPageRange(1, 2);
			do_state_transition_single5(true);			
		}

		/// <summary>
		/// Cursor on first page of selection, move cursor to second page of selection, SNwA is still Selected.
		/// Only 1st page is visible.
		/// </summary>
		protected void state_transition_single5_2()
		{
			GoToPage(1);
			do_state_transition_single5(true);
		}

		/// <summary>
		/// Cursor on first page of selection, move cursor to second page of selection, SNwA is still Selected.
		/// Only 2nd page visible.
		/// </summary>
		protected void state_transition_single5_3()
		{
			GoToPage(2);
			do_state_transition_single5(false);
		}

		private void do_state_transition_single5(bool doPageUp)
		{
			DocViewerWrapper.SelectionModule.SetSelection(1, PagePosition.End, -10, 2, PagePosition.Beginning, 10);
			CreateStickyNoteWithAnchor();
			ViewerBase.Focus();
			DocViewerWrapper.SelectionModule.SetSelection(1, PagePosition.End, -5, 1, PagePosition.End, -5);
			AssertEquals("Verify Initial State.", StickyNoteState.Active_Selected, CurrentlyAttachedStickyNote.State);
			printStatus("Verified Status on first page.");
            if (doPageUp)
                PageDown();            
			DocViewerWrapper.SelectionModule.SetSelection(2, PagePosition.Beginning, 5);
			DispatcherHelper.DoEvents(); // 
			VerifyNumberOfAttachedAnnotations(1);
			AssertEquals("Verify final State.", StickyNoteState.Active_Selected, CurrentlyAttachedStickyNote.State);
			passTest("Still selected after switching pages.");
		}

		#endregion

		#region Multiple Transition Tests

		/// <summary>
		/// 1. Two SnwAs, both selected:          -----{-------}-------{--------}---------
		/// 2. Click elsewhere, both unselected:  -I---{-------}-------{--------}--------
		/// </summary>
		protected void state_transition_multiple2()
		{
			StickyNoteWrapper[] wrappers = TestState(
				new SimpleSelectionData(0, 10, 25),	// anchorA
				new SimpleSelectionData(0, 50, 25),	// anchorB
				new SimpleSelectionData(0, 0, 100),		// selection.
				StickyNoteState.Active_Selected,			// state A.
				StickyNoteState.Active_Selected);			// state B.

			DocViewerWrapper.SelectionModule.SetSelection(0, 1, 0);
			AssertEquals("Verify final State A", StickyNoteState.Inactive, wrappers[0].State);
			AssertEquals("Verify final State B", StickyNoteState.Inactive, wrappers[1].State);
			
			passTest("Verified multiple selected to none selected.");
		}

        /// <summary>
        /// 1. -----------{-I-{-}--}-------- A selected
        /// 2. -----------{--{-}-I-}-------- B selected
        /// Verify appearance.
        /// </summary>
        [DisabledTestCase()]
        protected void state_transition_multiple3()
        {
            StickyNoteWrapper[] wrappers = TestState(
                new SimpleSelectionData(0, 10, 15),		// anchorA
                new SimpleSelectionData(0, 20, 15),		// anchorB
                new SimpleSelectionData(0, 15, 0),		// selection.
                StickyNoteState.Active_Selected,		// state A.
                StickyNoteState.Inactive);				// state B.

            DocViewerWrapper.SelectionModule.SetSelection(0, 30, 0);
            AssertEquals("Verify final State A", StickyNoteState.Inactive, wrappers[0].State);
            AssertEquals("Verify final State B", StickyNoteState.Active_Selected, wrappers[1].State);

            CompareToMaster("state_transition_multiple3_master.bmp");
            passTest("Verified final state.");
        }

        /// <summary>
        /// 1. -----{---I--[---}-(--]--------)------
        /// 2. -----{-----[---}I-(--]--------)------
        /// 3. -----{-----[---}-(--]-I------)------
        /// Verify appearance.
        /// </summary>
        [DisabledTestCase()]
        protected void state_transition_multiple4()
        {
            StickyNoteWrapper[] wrappers = new StickyNoteWrapper[3];
            wrappers[0] = CreateStickyNoteWithAnchor(new SimpleSelectionData(0, 10, 50), "A");
            wrappers[1] = CreateStickyNoteWithAnchor(new SimpleSelectionData(0, 50, 50), "B"); ;
            wrappers[2] = CreateStickyNoteWithAnchor(new SimpleSelectionData(0, 90, 50), "C");

            ViewerBase.Focus(); // unfocus SN.

            DocViewerWrapper.SelectionModule.SetSelection(0, 25, 0);
            VerifyStates("Phase 1", wrappers, new StickyNoteState[] { StickyNoteState.Active_Selected, StickyNoteState.Inactive, StickyNoteState.Inactive });
            CompareToMaster("state_transition_multiple4_A_master.bmp");

            DocViewerWrapper.SelectionModule.SetSelection(0, 80, 0);
            VerifyStates("Phase 2", wrappers, new StickyNoteState[] { StickyNoteState.Inactive, StickyNoteState.Active_Selected, StickyNoteState.Inactive });
            CompareToMaster("state_transition_multiple4_B_master.bmp");

            DocViewerWrapper.SelectionModule.SetSelection(0, 120, 0);
            VerifyStates("Phase 3", wrappers, new StickyNoteState[] { StickyNoteState.Inactive, StickyNoteState.Inactive, StickyNoteState.Active_Selected });
            CompareToMaster("state_transition_multiple4_C_master.bmp");

            passTest("Verified state transition between 3 overlapping SNs.");
        }

		#endregion

		#region Active Tests

		/// <summary>
		/// Two SNwA: A, B. A is active.  Click in B.  B is now only one active.
		/// </summary>
		protected void state_active3()
		{
			SetZoom(100);
			DocViewerWrapper.SelectionModule.SetSelection(0, 0, 100);
			StickyNoteWrapper snA = CreateStickyNoteWithAnchor();
			DocViewerWrapper.SelectionModule.SetSelection(0, 750, 200);
			StickyNoteWrapper snB = CreateStickyNoteWithAnchor();

			snA.ClickIn();
			AssertEquals("Verify intial state A.", StickyNoteState.Active_Focused, snA.State);
			AssertEquals("Verify intial state B.", StickyNoteState.Inactive, snB.State);

			snB.ClickIn();
			AssertEquals("Verify final state A.", StickyNoteState.Inactive, snA.State);
			AssertEquals("Verify final state B.", StickyNoteState.Active_Focused, snB.State);

			passTest("Verified transition of active state from 1 SN to another.");
		}

		/// <summary>
		/// Two SNwA: A, B. A is active.  Click elsewhere in text.  Neither is active.
		/// </summary>
		protected void state_active4()
		{			
			StickyNoteWrapper[] wrappers = new StickyNoteWrapper[2];
			wrappers[0] = CreateStickyNoteWithAnchor(new SimpleSelectionData(0, 0, 100), "A");
			wrappers[1] = CreateStickyNoteWithAnchor(new SimpleSelectionData(0, 600, 100), "B");

			wrappers[0].ClickIn();
			VerifyStates("Verify A focused.", wrappers, new StickyNoteState[] { StickyNoteState.Active_Focused, StickyNoteState.Inactive});

			Point snCorner = wrappers[0].Location;
            Point clickTarget = new Point(snCorner.X+wrappers[0].BoundingRect.Width+25, snCorner.Y+100);
            UIAutomationModule.MoveToAndClick(new Point(Monitor.ConvertLogicalToScreen(Dimension.Width, clickTarget.X), Monitor.ConvertLogicalToScreen(Dimension.Height, clickTarget.Y)));
			DispatcherHelper.DoEvents();
			VerifyStates("Verify none focused.", wrappers, new StickyNoteState[] { StickyNoteState.Inactive, StickyNoteState.Inactive });

			passTest("Verified transition of active state from 1 SN to none.");
		}

        /// <summary>
        /// -----------{{------}}--------
        /// Focus SN A. Verify: anchor color.
        /// Focus SN B. Verify: anchor color.
        /// </summary>
        [DisabledTestCase()]
        protected void state_active6()
        {
            StickyNoteWrapper[] wrappers = new StickyNoteWrapper[2];
            wrappers[0] = CreateStickyNoteWithAnchor(new SimpleSelectionData(0, 10, 500), "A");
            wrappers[1] = CreateStickyNoteWithAnchor(new SimpleSelectionData(0, 10, 500), "B");

            wrappers[1].Move(new Vector(200, 0)); //Move top SN so that they do not overlap exactly.	
            wrappers[0].ClickIn();
            VerifyStates("Verify A Focused", wrappers, new StickyNoteState[] { StickyNoteState.Active_Focused, StickyNoteState.Inactive });
            CompareToMaster("state_active6_B_master.bmp");

            wrappers[1].ClickIn();
            VerifyStates("Verify B Focused", wrappers, new StickyNoteState[] { StickyNoteState.Inactive, StickyNoteState.Active_Focused });
            CompareToMaster("state_active6_B_master.bmp");

            passTest("Verified changing focus between SNs.");
        }

        /// <summary>
        /// -----{------[---}-(--]--------)------
        /// Focus {}. Verify anchor color.
        /// Focus []. Verify anchor color.
        /// Focus (). Verify anchor color.
        /// </summary>
        [DisabledTestCase()]
        protected void state_active7()
        {
            GoToPage(1);
            StickyNoteWrapper[] wrappers = new StickyNoteWrapper[3];
            wrappers[0] = CreateStickyNoteWithAnchor(new SimpleSelectionData(1, 0, 500), "A");
            wrappers[1] = CreateStickyNoteWithAnchor(new SimpleSelectionData(1, 300, 600), "B");
            wrappers[2] = CreateStickyNoteWithAnchor(new SimpleSelectionData(1, 800, 500), "C");

            // Ensure they aren't overlapping.
            wrappers[1].Move(new Vector(200, 0));
            wrappers[2].Move(new Vector(0, 50));

            wrappers[0].ClickIn();
            VerifyStates("A Focused", wrappers, new StickyNoteState[] { StickyNoteState.Active_Focused, StickyNoteState.Inactive, StickyNoteState.Inactive });
            CompareToMaster("state_active7_A_master.bmp");
            wrappers[1].ClickIn();
            VerifyStates("B Focused", wrappers, new StickyNoteState[] { StickyNoteState.Inactive, StickyNoteState.Active_Focused, StickyNoteState.Inactive });
            CompareToMaster("state_active7_B_master.bmp");
            wrappers[2].ClickIn();
            VerifyStates("C Focused", wrappers, new StickyNoteState[] { StickyNoteState.Inactive, StickyNoteState.Inactive, StickyNoteState.Active_Focused });
            CompareToMaster("state_active7_C_master.bmp");

            passTest("Verified multiple SN focus transitions.");
        }

        /// <summary>
        /// ---{------}{------}--
        /// Focus Black: Verify black is SNActive.
        /// Focus Red: Verify red is SNActive.
        /// </summary>
        [DisabledTestCase()]
        protected void state_active11()
        {
            GoToPage(1);
            StickyNoteWrapper[] wrappers = new StickyNoteWrapper[2];
            wrappers[0] = CreateStickyNoteWithAnchor(new SimpleSelectionData(1, 500, 500), "A");
            wrappers[1] = CreateStickyNoteWithAnchor(new SimpleSelectionData(1, 1000, 500), "B");

            // Ensure they aren't overlapping.
            if (ContentMode == TestMode.Flow)
            {
                wrappers[0].Move(new Vector(0, -100));
            }

            wrappers[0].ClickIn();
            VerifyStates("A Focused", wrappers, new StickyNoteState[] { StickyNoteState.Active_Focused, StickyNoteState.Inactive });
            CompareToMaster("state_active11_A_master.bmp");
            wrappers[1].ClickIn();
            VerifyStates("B Focused", wrappers, new StickyNoteState[] { StickyNoteState.Inactive, StickyNoteState.Active_Focused });
            CompareToMaster("state_active11_B_master.bmp");

            passTest("Verified anchors of focused SNs.");
        }

        /// <summary>
        /// ----{-----I---}----- anchor is active.
        /// Click in SN, SN is Active.
        /// Return focus to DV.  Anchor is active.
        /// </summary>
        [DisabledTestCase()]
        protected void state_active12()
        {
            StickyNoteWrapper wrapper = CreateStickyNoteWithAnchor(new SimpleSelectionData(0, 500, 500), "");
            ViewerBase.Focus();
            DocViewerWrapper.SelectionModule.SetSelection(0, 750, 0);
            AssertEquals("Verify anchor initially selected.", StickyNoteState.Active_Selected, wrapper.State);
            CompareToMaster("state_active12_A_master.bmp");

            wrapper.ClickIn();
            AssertEquals("Verify SN has focus.", StickyNoteState.Active_Focused, wrapper.State);
            CompareToMaster("state_active12_B_master.bmp");

            ViewerBase.Focus();
            AssertEquals("Verify anchor finally selected.", StickyNoteState.Active_Selected, wrapper.State);
            CompareToMaster("state_active12_C_master.bmp");

            passTest("Verified transition through all SN states.");
        }

		/// <summary>
		/// ----{-----{-I-}---}--- both anchors are active.
		/// Click in sn A.  SN A is active, anchor B is inactive.
		/// Focus DV, both anchors are active.
		/// </summary>
		protected void state_active14()
		{
			GoToPage(3);
			StickyNoteWrapper[] wrappers = new StickyNoteWrapper[2];
			wrappers[0] = CreateStickyNoteWithAnchor(new SimpleSelectionData(3, 100, 100), "A");
			wrappers[1] = CreateStickyNoteWithAnchor(new SimpleSelectionData(3, 150, 100), "B");

			ViewerBase.Focus();
			DocViewerWrapper.SelectionModule.SetSelection(3, 175, 0);
			VerifyStates("Both selected", wrappers, new StickyNoteState[] { StickyNoteState.Active_Selected, StickyNoteState.Active_Selected });
			wrappers[1].ClickIn();
			VerifyStates("B Focused", wrappers, new StickyNoteState[] { StickyNoteState.Inactive, StickyNoteState.Active_Focused });
			DocViewerWrapper.ViewerBase.Focus();
			VerifyStates("DV focused: both selected", wrappers, new StickyNoteState[] { StickyNoteState.Active_Selected, StickyNoteState.Active_Selected });

			passTest("Verified transition between focus activation and selection activation.");
		}

		/// <summary>
		/// ----{{-I-}---}--- both anchors are active.
		/// Click in sn A.  SN A is active, anchor B is inactive.
		/// Click in sn B. Anchor A is inactive, SN B is active.
		/// Focus DV, both anchors are active.
		/// </summary>
		protected void state_active15()
		{
			StickyNoteWrapper[] wrappers = new StickyNoteWrapper[2];
			wrappers[0] = CreateStickyNoteWithAnchor(new SimpleSelectionData(0, 100, 100), "A");
			wrappers[1] = CreateStickyNoteWithAnchor(new SimpleSelectionData(0, 100, 50), "B");

			wrappers[1].Move(new Vector(200, 50)); // Make sure sns are not overlapping.

			ViewerBase.Focus();
			DocViewerWrapper.SelectionModule.SetSelection(0, 125, 0);
			VerifyStates("Both selected", wrappers, new StickyNoteState[] { StickyNoteState.Active_Selected, StickyNoteState.Active_Selected });
			
			wrappers[0].ClickIn();
			VerifyStates("A Focused", wrappers, new StickyNoteState[] { StickyNoteState.Active_Focused, StickyNoteState.Inactive });
			wrappers[1].ClickIn();
			VerifyStates("B Focused", wrappers, new StickyNoteState[] { StickyNoteState.Inactive, StickyNoteState.Active_Focused });

			DocViewerWrapper.ViewerBase.Focus();
			VerifyStates("DV focused: both selected", wrappers, new StickyNoteState[] { StickyNoteState.Active_Selected, StickyNoteState.Active_Selected });

			passTest("Verified transition between focus activation and selection activation.");
		}

		/// <summary>
		/// Crosspage anchor.
		/// Click in SN.  SN is active.
		/// </summary>
		protected void state_active16_1()
		{
			SetZoom(100);
			if (ContentMode == TestMode.Flow) ViewAsTwoPages();			
			GoToPageRange(2, 3);
			StickyNoteWrapper wrapper = CreateStickyNoteWithAnchor(new MultiPageSelectionData(2, PagePosition.End, -50, 3, PagePosition.Beginning, 100), "A");
			wrapper.ClickIn();
			AssertEquals("Verify state.", StickyNoteState.Active_Focused, wrapper.State);

			passTest("Verified active SN across page break.");
		}

		/// <summary>
		/// Crosspage anchor.
		/// Both pages visible. 
		/// Click in SN.  SN is active.
		/// Page down.  SN is no longer active.
		/// </summary>
		protected void state_active16_2()
		{
			SetZoom(100);
			if (ContentMode == TestMode.Flow) ViewAsTwoPages();
			GoToPageRange(2, 3);
			CreateStickyNoteWithAnchor(new MultiPageSelectionData(2, PagePosition.End, -50, 3, PagePosition.Beginning, 100), "A");
			AssertEquals("Verify intial state.", StickyNoteState.Active_Focused, CurrentlyAttachedStickyNote.State);
			PageDown();
			AssertEquals("Verify final state.", StickyNoteState.Inactive, CurrentlyAttachedStickyNote.State);

			passTest("Verified active SN across page break.");
		}


		/// <summary>
		/// Crosspage anchors, both are active. {{--I---|--}---}
		/// Click in sn A. SN A is active, B is inactive.
		/// Focus DV, both are active.
		/// </summary>
		protected void state_active17()
		{
			if (ContentMode == TestMode.Flow) ViewAsTwoPages();
			GoToPageRange(1, 2);
			StickyNoteWrapper[] wrappers = new StickyNoteWrapper[2];
			wrappers[0] = CreateStickyNoteWithAnchor(new MultiPageSelectionData(1, PagePosition.End, -50, 2, PagePosition.Beginning, 100), "A");
			wrappers[1] = CreateStickyNoteWithAnchor(new MultiPageSelectionData(1, PagePosition.End, -50, 2, PagePosition.Beginning, 50), "B");

            wrappers[1].Drag(new Vector(-250, 0)); // Make sure they are not overlapping.	

			ViewerBase.Focus();
			DocViewerWrapper.SelectionModule.SetSelection(1, PagePosition.End, -25, 1, PagePosition.End, -25);
			VerifyStates("Both selected", wrappers, new StickyNoteState[] { StickyNoteState.Active_Selected, StickyNoteState.Active_Selected });

			wrappers[0].ClickIn();
			VerifyStates("A Focused", wrappers, new StickyNoteState[] { StickyNoteState.Active_Focused, StickyNoteState.Inactive });			

			DocViewerWrapper.ViewerBase.Focus();
			VerifyStates("DV focused: both selected", wrappers, new StickyNoteState[] { StickyNoteState.Active_Selected, StickyNoteState.Active_Selected });

			passTest("Verified transition between focus activation and selection activation.");
		}


		#endregion

        #endregion PRIORITY TESTS
    }
}	

