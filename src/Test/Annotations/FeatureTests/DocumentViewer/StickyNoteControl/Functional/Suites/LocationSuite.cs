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
    public class LocationSuite : ALocationSuite
    {
        #region BVT TESTS

        /// <summary>
        /// Create Note.  	
        /// Verify: Initial position.
        /// </summary>
        [TestDimension("/zoom=100")]
        [Priority(0)]
        protected void location1()
        {
            CreateAnnotation(new SimpleSelectionData(0, 50, 100));
            ChangeZoom();
            AssertEquals(
                "Verify note location.",
                DocViewerWrapper.PointerToScreenCoordinates(0, 50, LogicalDirection.Forward, ADocumentViewerBaseWrapper.HorizontalJustification.Bottom),
                CurrentlyAttachedStickyNote.Location,
                1e-3);
            passTest("Verified initial location.");
        }

        /// <summary>
        /// Drag to new position on page.
        /// Page up.
        /// Page Down.	
        /// Verify: Position.
        /// </summary>
        [Priority(0)]
        protected void location2()
        {
            Vector offset = new Vector(-50, 25);
            ChangeZoom();
            WholePageLayout();
            CreateAnnotation(new SimpleSelectionData(0, 50, 100));
            CurrentlyAttachedStickyNote.Drag(offset);
            Point initialLocation = CurrentlyAttachedStickyNote.Location;
            PageDown();
            PageUp();
            AssertEquals(
                "Verify note location after dragging and navigation.",
                initialLocation,
                CurrentlyAttachedStickyNote.Location,
                1e-3);
            passTest("Verified drag position preserved during navigation.");
        }

        #endregion BVT TESTS

        #region PRIORITY TESTS

        /// <summary>
        /// Create Note A.
        /// Create Note B.
        /// Click in Note A.	
        /// Verify: Final positions.
        /// </summary>
        [Priority(1)]
        protected void location6()
        {
            CreateAnnotation(new SimpleSelectionData(0, 901, 19), "A");
            CreateAnnotation(new SimpleSelectionData(0, 10, 21), "B");
            StickyNoteWrapper[] wrappers = GetStickyNotesByAuthor(new string[] { "A", "B" });
            
            wrappers[0].ClickIn();                         
            AssertEquals("Verify focus.", StickyNoteState.Active_Focused, wrappers[0].State);
            printStatus("Note 'B' focused.");

            VerifyNotePosition(wrappers[0], 0, 901);
            VerifyNotePosition(wrappers[1], 0, 10);
            passTest("Verified location after focus change.");
        }

        [TestCase_Helper()]
        protected void VerifyNotePosition(StickyNoteWrapper wrapper, int page, int offset)
        {
            Point expectedAnchorPoint = DocViewerWrapper.PointerToScreenCoordinates(page, offset, System.Windows.Documents.LogicalDirection.Forward, ADocumentViewerBaseWrapper.HorizontalJustification.Bottom);
            AssertEquals("Verify note position.", expectedAnchorPoint, wrapper.Location, 1e-3);
        }

        #endregion PRIORITY TESTS
    }
}	

