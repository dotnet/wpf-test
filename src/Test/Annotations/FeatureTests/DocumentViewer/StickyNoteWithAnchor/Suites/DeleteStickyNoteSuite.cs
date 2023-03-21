// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 

using System;
using System.Windows;
using Annotations.Test.Framework;
using System.Windows.Documents;

namespace Avalon.Test.Annotations.Suites
{
	public class DeleteStickyNoteSuite : AStickyNoteWithAnchorSuite
    {
        #region BVT TESTS
        #endregion BVT TESTS

        #region PRIORITY TESTS

        /// <summary>
        /// Create SNwA.
		/// Select anchor.
		/// Page down/up
		/// Delete.
		/// Page Up/down
		/// VScan: Verfiy no annotation.
		/// </summary>
        protected void delete2_X(bool upMode)
		{
			PageDown();
			ISelectionData selection = new SimpleSelectionData(1, 900, 32);
			CreateStickyNoteWithAnchor(selection, "");
			selection.SetSelection(DocViewerWrapper.SelectionModule);
			if (upMode)
				PageUp();
			else
				PageDown();
			DeleteStickyNoteWithAnchor(null);
			if (upMode) 
				PageDown();
			else
				PageUp();
			CompareToMaster("delete_master.bmp");
			passTest("Verified delete of no-visible SNwA.");
		}
        [DisabledTestCase()]
        [Priority(1)]
        protected void delete2_1() { delete2_X(false); }
        [DisabledTestCase()]
        [Priority(1)]
        protected void delete2_2() { delete2_X(false); }

        [TestCase_Helper()]
		protected void TestEdgeDelete(ISelectionData selection, bool result)
		{
			PageDown();
			CreateStickyNoteWithAnchor(new SimpleSelectionData(1, 200, 50), "");
			DeleteStickyNoteWithAnchor(selection);
			if (!result) VerifyNumberOfAttachedAnnotations(1);
			else VerifyNumberOfAttachedAnnotations(0);
			passTest("Verified that annotation was deleted: " + result);
		}
        
		protected void delete3_X(LogicalDirection dir, bool result)
		{
			TestEdgeDelete(new SimpleSelectionData(1, 250, dir), result);	
		}
		/// <summary>
		/// ****{***}I->*** not deleted.
		/// </summary>		
        [Priority(1)]
        protected void delete3_1() { delete3_X(LogicalDirection.Forward, false); }
		/// <summary>
		/// ****{***}<-I*** deleted.
		/// </summary>
        [Priority(1)]
        protected void delete3_2() { delete3_X(LogicalDirection.Backward, true); }

		protected void delete4_X(LogicalDirection dir, bool result)
		{
			TestEdgeDelete(new SimpleSelectionData(1, 200, dir), result);		
		}
		/// <summary>
		/// ****I->{***}*** deleted.
		/// </summary>
        [Priority(1)]
        protected void delete4_1() { delete4_X(LogicalDirection.Forward, true); }
		/// <summary>
		/// ****<-I{***}*** deleted.
		/// </summary>
        [Priority(1)]
        protected void delete4_2() { delete4_X(LogicalDirection.Backward, false); }

        #endregion PRIORITY TESTS
    }
}	

