// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description:

using System;
using System.Windows;
using Annotations.Test.Framework;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;
using Microsoft.Test.Display;

namespace Avalon.Test.Annotations.Suites
{
    public class IconificationSuite : AIconificationSuite
    {
        #region BVT TESTS

        /// <summary>
        /// “Click” on Minimize Button.	Properly iconified
        /// </summary>
        //[Priority(0)]
        //protected void iconification1()
        //{
        //    CreateAnnotation(new SimpleSelectionData(0, 100, 200));
        //    CurrentlyAttachedStickyNote.MinimizeWithMouse();
        //    AssertEquals("Verify icon location.", DocViewerWrapper.PointerToScreenCoordinates(0, 100, LogicalDirection.Forward, ADocumentViewerBaseWrapper.HorizontalJustification.Bottom), CurrentlyAttachedStickyNote.Location, 1e-3);

        //    // Move cursor so that it isn't over the anchor.
        //    UIAutomationModule.MoveTo(DocViewerWrapper.PointerToScreenCoordinates(0, 350, LogicalDirection.Forward, ADocumentViewerBaseWrapper.HorizontalJustification.Bottom));
        //    VerifyIconSize(StickyNoteWrapper.IconDefaultSize);
        //    passTest("Verified iconified note.");
        //}

        /// <summary>
        /// “Click” on Minimize Button.	
        /// Restored.
        /// Verify: propertly restored.
        /// </summary>
        [Priority(0)]
        protected void iconification8()
        {
            CreateDefaultNote();
            Size originalSize = CurrentlyAttachedStickyNote.BoundingRect.Size;
            Point originalLocation = CurrentlyAttachedStickyNote.Location;

            CurrentlyAttachedStickyNote.MinimizeWithMouse();
            CurrentlyAttachedStickyNote.ClickIn();

            AssertEquals("Verify size.", originalSize, CurrentlyAttachedStickyNote.BoundingRect.Size);
            AssertEquals("Verify location.", originalLocation, CurrentlyAttachedStickyNote.Location);
            passTest("Verified note properly restored after iconified.");
        }

        /// <summary>
        /// Create Note A.
        /// Minimize A.
        /// Create Note B. 
        /// Minimize B.	
        /// Verify: Both are minimized.
        /// </summary>
        //[Priority(0)]
        //protected void iconification13()
        //{
        //    CreateAnnotation(new SimpleSelectionData(0, 103, 45), "A");
        //    CurrentlyAttachedStickyNote.MinimizeWithMouse();
        //    CreateAnnotation(new SimpleSelectionData(0, 412, 15), "B");
        //    GetStickyNotesByAuthor(new string[] { "B" })[0].MinimizeWithMouse();

        //    StickyNoteWrapper[] wrappers = GetStickyNotesByAuthor(new string[] { "A", "B" });
        //    AssertEquals("Verify state A.", false, wrappers[0].Expanded);
        //    AssertEquals("Verify state B.", false, wrappers[0].Expanded);

        //    passTest("Multiple notes minimized.");
        //}   

        #endregion BVT TESTS

        #region PRIORITY TESTS

        /// <summary>
        /// Mouse over icon.	
        /// Verify icon changes.
        /// </summary>
        //[Priority(1)]
        //protected void iconification2()
        //{
        //    if (DisplayConfiguration.GetAppearance() != DesktopAppearance.WindowsClassic)
        //    {
        //        CreateDefaultNote();
        //        CurrentlyAttachedStickyNote.MinimizeWithMouse();
        //        // Mouse over.
        //        CurrentlyAttachedStickyNote.MoveTo();
        //        VerifyIconSize(StickyNoteWrapper.IconHoverSize);
        //    }
        //    passTest("Verified enlarged iconified note.");
        //}      

        /// <summary>
        /// Iconify.
        /// Change zoom.
        /// Verify icon position.
        /// </summary>
        [Priority(1)]
        protected void iconification3()
        {
            CreateDefaultNote();
            CurrentlyAttachedStickyNote.MinimizeWithMouse();
            ChangeZoom();
            VerifyNoteLocation();
            passTest("Position is correct after zoom.");
        }

        /// <summary>
        /// Iconify, resize window.	
        /// Verify icon position.
        /// </summary>
        [Priority(1)]
        protected void iconification5()
        {
            CreateDefaultNote();
            CurrentlyAttachedStickyNote.MinimizeWithMouse();
            ChangeWindowHeight(-200);
            ChangeWindowWidth(-500);
            VerifyNoteLocation();
            passTest("Position is correct after window resize.");
        }

        /// <summary>
        /// Iconify,page down, page up.	
        /// Verify icon position.
        /// </summary>
        [Priority(1)]
        protected void iconification6()
        {
            WholePageLayout();
            CreateDefaultNote();
            CurrentlyAttachedStickyNote.MinimizeWithMouse();
            PageDown();
            PageUp();
            VerifyNoteLocation();
            passTest("Position is correct after naviation.");
        }

        /// <summary>
        /// Drag SN from default position.
        /// Iconify.	
        /// Verify icon position.
        /// </summary>
        //[Priority(1)]
        //protected void iconification7()
        //{
        //    CreateDefaultNote();
        //    CurrentlyAttachedStickyNote.Drag(new Vector(50, -50));
        //    CurrentlyAttachedStickyNote.MinimizeWithMouse();
        //    VerifyNoteLocation();
        //    passTest("Icon position correct after drag.");
        //}

        /// <summary>
        /// Drag SN from default position.
        /// Iconify.	
        /// Click on icon.
        /// Verify restored.
        /// </summary>
        //[Priority(1)]
        //protected void iconification9()
        //{            
        //    CreateDefaultNote();
        //    CurrentlyAttachedStickyNote.Drag(new Vector(50, -50));
        //    VerifyIconifyAndRestore(true);            
        //    passTest("Restored position is correct after drag.");
        //}

        /// <summary>
        /// Drag note off right page edge.
        /// Iconify.
        /// Restore, verify location.
        /// </summary>
        [Priority(1)]
        protected void iconification10()
        {
            SetZoom(75);
            CreateDefaultNote();
            Point dest = new Point(PageBounds(0).Right, CurrentlyAttachedStickyNote.Location.Y);
            CurrentlyAttachedStickyNote.Drag(dest);
            VerifyIconifyAndRestore(false);
            passTest("Icon position correct after drag.");
        }

        /// <summary>
        /// Drag note off bottom page edge.
        /// Iconify.
        /// Restore, verify location.
        /// </summary>
        [Priority(1)]
        protected void iconification11()
        {
            GoToPage(2);
            if (ContentMode == TestMode.Fixed)
                GoToPageRange(2, 3);
            CreateAnnotation(new SimpleSelectionData(2, PagePosition.End, -500));
            Point dest = new Point(CurrentlyAttachedStickyNote.Location.X-50, PageBounds(2).Bottom);
            CurrentlyAttachedStickyNote.Drag(dest);
            VerifyIconifyAndRestore(true);
            passTest("Icon position correct after drag.");
        }

        /// <summary>
        /// Verify that iconifying and restoring the currently attached note will restore it
        /// to the correct location.
        /// </summary>
        [TestCase_Helper()]
        private void VerifyIconifyAndRestore(bool withMouse)
        {
            Point expectedLocation = CurrentlyAttachedStickyNote.Location;
            if (withMouse)
                CurrentlyAttachedStickyNote.MinimizeWithMouse();
            else
                CurrentlyAttachedStickyNote.Expanded = false;

            Assert("Verify minimized.", !CurrentlyAttachedStickyNote.Expanded);
            CurrentlyAttachedStickyNote.ClickIn();
            Assert("Verify maximized.", CurrentlyAttachedStickyNote.Expanded);

            Point actualLocation = CurrentlyAttachedStickyNote.Location;
            AssertEquals("Verify position after note is restored.", expectedLocation, actualLocation, 1e-3);
        }

        [TestCase_Helper()]
        private void VerifyNoteLocation()
        {
            AssertEquals(
                "Verify Icon Location.", 
                DocViewerWrapper.PointerToScreenCoordinates(DEFAULT_PAGE, DEFAULT_OFFSET, System.Windows.Documents.LogicalDirection.Forward, ADocumentViewerBaseWrapper.HorizontalJustification.Bottom), 
                CurrentlyAttachedStickyNote.Location,
                1e-3);
        }

        #endregion PRIORITY TESTS
    }
}	

