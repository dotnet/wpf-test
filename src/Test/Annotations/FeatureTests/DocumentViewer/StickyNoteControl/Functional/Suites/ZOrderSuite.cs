// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 

using System;
using System.Windows;
using Annotations.Test.Framework;
using Proxies.System.Windows.Annotations;

namespace Avalon.Test.Annotations.Suites
{
    public class ZOrderSuite : AStickyNoteControlFunctionalSuite
    {
        #region BVT TESTS

        /// <summary>
        /// 1. Create SN A.
        /// 2. Create SN B.
        /// 3. Verify that SN B is on top.
        /// </summary>
        [Priority(0)]
        public void zorder1()
        {
            CreateAnnotation(new SimpleSelectionData(0, 100, 100), "A");
            CreateAnnotation(new SimpleSelectionData(0, 100, 100), "B");
            VerifyZOrders(GetStickyNotesByAuthor(new string[] { "B", "A" }));
            passTest("Verified Z order after create.");
        }

        /// <summary>
        /// 1. Create SN A.
        /// 2. Create SN B.
        /// 3. Make SN A on top of SN B.
        /// 4. Page up, then page down.
        /// 5. Verify that SN A is still on top.
        /// </summary>
        [Priority(0)]
        public void zorder2_1()
        {
            GoToPage(2);
            CreateAnnotation(new SimpleSelectionData(2, 500, 10), "A");
            CreateAnnotation(new SimpleSelectionData(2, 1000, 10), "B");
            StickyNoteWrapper[] wrappers = GetStickyNotesByAuthor(new string[] { "A", "B" });
            wrappers[0].ClickIn();
            VerifyZOrders(wrappers);
            PageUp();
            PageDown();
            VerifyZOrders(GetStickyNotesByAuthor(new string[] { "A", "B" }));
            passTest("Verfied z-order after page up/down.");
        }

        /// <summary>
        /// Create A.
        /// Create B.
        /// Minimize B.
        /// Focus A.
        /// Maximize B.	
        /// Verify B is on top of A.
        /// </summary>
        [Priority(0)]
        public void zorder9()
        {
            CreateAnnotation(new SimpleSelectionData(0, 1, 10), "A");
            CreateAnnotation(new SimpleSelectionData(0, 500, 10), "B");
            GetStickyNotesByAuthor(new string[] { "B" })[0].MinimizeWithMouse();
            GetStickyNotesByAuthor(new string[] { "A" })[0].ClickIn();
            GetStickyNotesByAuthor(new string[] { "B" })[0].ClickIn();
            VerifyZOrders(GetStickyNotesByAuthor(new string[] { "B", "A" }));
            passTest("Verified Z order min/max/focus change");
        }

        #endregion BVT TESTS

        #region PRIORITY TESTS

        /// <summary>
		/// 1. Create SN A.
		/// 2. Create SN B.
		/// 3. Make SN A on top of SN B.
		/// 4. Disable, re-enable service.
		/// 5. Verify that SN A is still on top.
		/// </summary>
        [Priority(1)]
        public void zorder2_3()
		{
			CreateAnnotation(new SimpleSelectionData(0, 61, 231), "A");
            CreateAnnotation(new SimpleSelectionData(0, 521, 19), "B");
            StickyNoteWrapper[] wrappers = GetStickyNotesByAuthor(new string[] { "A", "B" });
			wrappers[0].ClickIn();
			VerifyZOrders(wrappers);
			AnnotationService service = Service;
			service.Disable();
			service.Enable(service.Store);
			DispatcherHelper.DoEvents(); // Wait for visual Update.
            VerifyZOrders(GetStickyNotesByAuthor(new string[] { "A", "B" }));
			passTest("Verified z-order after disable/re-enable.");
		}

		/// <summary>
		/// 1. Create SN A.
		/// 2. Create SN B.
		/// 3. Create SN C.
		/// 4. Delete SN B.
		/// 5. Verify C in front of A.
		/// </summary>
        [Priority(1)]
        public void zorder3()
		{
            CreateAnnotation(new SimpleSelectionData(0, 2, 65), "A");
            CreateAnnotation(new SimpleSelectionData(0, 91, 12), "B");
            CreateAnnotation(new SimpleSelectionData(0, PagePosition.End, -51), "C");
			DeleteAnnotation(new SimpleSelectionData(0, 100, 0));
			VerifyZOrders(GetStickyNotesByAuthor(new string [] { "C", "A"}));
			passTest("Verified z-order after delete.");
		}

		/// <summary>
		/// 1. Create SN A.
		/// 2. Create SN B.
		/// 3. Make A on top.
		/// 4. Create SN C.
		/// 5. Verify order from front to back is: C, A, B
		/// </summary>
        [Priority(1)]
        public void zorder4()
		{
			CreateAnnotation(new SimpleSelectionData(0, 2, 65), "A");
            StickyNoteWrapper wrapperA = CurrentlyAttachedStickyNote;
            CreateAnnotation(new SimpleSelectionData(0, 91, 12), "B");
			wrapperA.ClickIn();
            CreateAnnotation(new SimpleSelectionData(0, PagePosition.End, -51), "C");			
			VerifyZOrders(GetStickyNotesByAuthor(new string[] { "C", "A", "B" }));
			passTest("Verified z-order after add/click sequence.");
        }

        #endregion PRIORITY TESTS
    }
}	

