// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 

using System;
using System.Windows;
using Annotations.Test.Framework;

namespace Avalon.Test.Annotations.Suites
{
	public class AddStickyNoteSuite : AStickyNoteWithAnchorSuite
    {
        #region BVT TESTS

        /// <summary>
        /// Create SNwA.
        /// VScan: verify visuals.
        /// </summary>
        [DisabledTestCase()]
        protected void add1()
        {
            SetZoom(200);
            CreateStickyNoteWithAnchor(new SimpleSelectionData(0, 1, 500), "");
            VerifyNumberOfAttachedAnnotations(1);
            CompareToMaster("add_master.bmp");
            passTest("Verified simple add.");
        }

        #endregion BVT TESTS

        #region PRIORITY TESTS

        [TestCase_Setup("add2.*")]
		protected void  Setup2()
		{
            base.DoSetup();
			PageDown();
			CreateStickyNoteWithAnchor(new SimpleSelectionData(1, 200, 500), "");
			ViewerBase.Focus();
			VerifyNumberOfAttachedAnnotations(1);
		}

        /// <summary>
        /// Create SNwA.
        /// Page Down.
        /// Page Up.
        /// VScan: verify visuals.
        /// </summary>
        [DisabledTestCase()]
        protected void add2_1()
        {
            PageDown();
            PageUp();
            VerifyNumberOfAttachedAnnotations(1);
            AssertEquals("Verify final state.", StickyNoteState.Active_Selected, CurrentlyAttachedStickyNote.State);
            CompareToMaster("add_master.bmp");
            passTest("Verified anchor after page down/up.");
        }

        /// <summary>
        /// Create SNwA.
        /// Page up.
        /// Page down.
        /// VScan: Verify visuals.
        /// </summary>
        [DisabledTestCase()]
        protected void add2_2()
        {
            PageUp();
            PageDown();
            VerifyNumberOfAttachedAnnotations(1);
            AssertEquals("Verify final state.", StickyNoteState.Active_Selected, CurrentlyAttachedStickyNote.State);
            CompareToMaster("add_master.bmp");
            passTest("Verified anchor after page up/down.");
        }

        /// <summary>
        /// Create SNwA.
        /// Unload content.
        /// Reload content.
        /// VScan: Verify visuals.
        /// </summary>
        [DisabledTestCase()]
        protected void add2_3()
        {
            ClearDocumentViewerContent();
            SetDocumentViewerContent();
            WholePageLayout();
            PageDown();
            VerifyNumberOfAttachedAnnotations(1);
            AssertEquals("Verify final state.", StickyNoteState.Inactive, CurrentlyAttachedStickyNote.State);
            CompareToMaster("add_master.bmp");
            passTest("Verified anchor after resetting content.");
        }

        [TestCase_Setup("add3.*")]
		private void setupAdd3()
		{
            base.DoSetup();
			PageDown();
			DocViewerWrapper.SelectionModule.SetSelection(1, 200, 500);
		}

        /// <summary>
        /// Create selection.
        /// Page Down.
        /// Create SNwA.
        /// Page Up.
        /// VScan: Verfiy Visuals
        /// </summary>
        [DisabledTestCase()]
        protected void add3_1()
        {
            PageDown();
            CreateStickyNoteWithAnchor(null, "");
            PageUp();
            VerifyNumberOfAttachedAnnotations(1);
            AssertEquals("Verify final state.", StickyNoteState.Active_Focused, CurrentlyAttachedStickyNote.State);
            CompareToMaster("add_master.bmp");
            passTest("Verified anchor created on non-visible page.");
        }

        /// <summary>
        /// Create selection.
        /// Page up.
        /// Create SNwA.
        /// Page Down.
        /// VScan: Verfiy Visuals
        /// </summary>
        [DisabledTestCase()]
        protected void add3_2()
        {
            PageUp();
            CreateStickyNoteWithAnchor(null, "");
            PageDown();
            VerifyNumberOfAttachedAnnotations(1);
            AssertEquals("Verify final state.", StickyNoteState.Active_Focused, CurrentlyAttachedStickyNote.State);
            CompareToMaster("add_master.bmp");
            passTest("Verified anchor created on non-visible page.");
        }

        #endregion PRIORITY TESTS
    }
}	

