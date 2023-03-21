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
using System.Windows.Markup;

namespace Avalon.Test.Annotations.Suites
{
    public class ResizeSuite : AStickyNoteControlFunctionalSuite
    {
        #region BVT TESTS
        /// <summary>
        /// Empty note.
        /// Make bigger.	
        /// Verify: Size changed.
        /// </summary>
        //[Priority(0)]
        //protected void resize1()
        //{
        //    CreateDefaultNote();
        //    TestMakeNoteLarger();
        //    passTest("Verified note can be made larger.");
        //}

        /// <summary>
        /// Empty note.
        /// Make smaller.	
        /// Verify: Size changed.
        /// </summary>
        //[Priority(0)]
        //protected void resize2()
        //{
        //    CreateDefaultNote();
        //    TestMakeNoteSmaller();
        //    passTest("Verified note can be made larger.");
        //}

        /// <summary>
        /// Note with content.
        /// Make Bigger.	
        /// Verify: note still has content.
        /// </summary>
        //[Priority(0)]
        //protected void resize3()
        //{
        //    CreateDefaultNote();
        //    SetContent(ContentKind.Standard_Small);
        //    TestMakeNoteLarger();
        //    VerifyContent(ContentKind.Standard_Small);
        //    passTest("Simple verification of resize with content.");
        //}

        /// <summary>
        /// Note with content.
        /// Make smaller.	
        /// Verify: note still has content.
        /// </summary>
        //[Priority(0)]
        //protected void resize4()
        //{
        //    CreateDefaultNote();
        //    SetContent(ContentKind.Standard_Small);
        //    TestMakeNoteSmaller();
        //    VerifyContent(ContentKind.Standard_Small);
        //    passTest("Simple verification of resize with content.");
        //}

        /// <summary>
        /// Empty note.
        /// Resize.
        /// Iconify
        /// Restore note.	
        /// Verify: size is same after restore.
        /// </summary>
        [Priority(0)]
        protected void resize7()
        {
            CreateDefaultNote();
            CurrentlyAttachedStickyNote.ResizeWithMouse(new Vector(100, -25));
            double currentWidth = Note.ActualWidth;
            double currentHeight = Note.ActualHeight;
            CurrentlyAttachedStickyNote.MinimizeWithMouse();
            CurrentlyAttachedStickyNote.ClickIn();
            AssertEquals("Verify width after restore.", currentWidth, Note.ActualWidth, 1e-6);
            AssertEquals("Verify height after restore.", currentHeight, Note.ActualHeight, 1e-6);
            passTest("Verified size presisted through iconification.");
        }

        #region Private Methods

        [TestCase_Helper()]
        private void TestMakeNoteLarger()
        {
            double initialHeight = Note.ActualHeight;
            double initialWidth = Note.ActualWidth;
            CurrentlyAttachedStickyNote.ResizeWithMouse(new Vector(50, 50));
            DispatcherHelper.DoEvents();
            Assert("Verify note is wider.", initialWidth < Note.ActualWidth);
            Assert("Verify note is taller.", initialHeight < Note.ActualHeight);
        }

        [TestCase_Helper()]
        private void TestMakeNoteSmaller()
        {
            double initialHeight = Note.ActualHeight;
            double initialWidth = Note.ActualWidth;
            CurrentlyAttachedStickyNote.ResizeWithMouse(new Vector(-50, -50));
            DispatcherHelper.DoEvents();
            Assert("Verify note is less wide.", initialWidth > Note.ActualWidth);
            Assert("Verify note is less tall.", initialHeight > Note.ActualHeight);
        }

        #endregion

        #endregion BVT TESTS

        #region PRIORITY TESTS

        /// <summary>
        /// Create note near page edge, drag it to the middle of the page and then resize.
        /// Verify: that resizing the note doesn't change its position.
        /// </summary>
        [OverrideClassTestDimensions]
        [TestDimension("flow")]
        [TestDimension("stickynote,inkstickynote")]
        [Priority(1)]
        protected void resize_regression_issue1()        {
            SetZoom(150);
            CreateAnnotation(new MultiPageSelectionData(0, PagePosition.End, -20, 0, PagePosition.End, -10));
            CurrentlyAttachedStickyNote.Drag(new Vector(-100, 0));
            Point initialLocation = CurrentlyAttachedStickyNote.Location;
            CurrentlyAttachedStickyNote.ResizeWithMouse(new Vector(20, 20));
            AssertEquals("Verify position after resize.", initialLocation, CurrentlyAttachedStickyNote.Location);
            passTest("Verified area has not regressed.");
        }

        /// <summary>
        /// Create note near page edge, resize it.
        /// Verify: location before resize is the same as after.
        /// </summary>
        [OverrideClassTestDimensions]
        [TestDimension("flow")]
        [TestDimension("stickynote,inkstickynote")]
        [Priority(1)]
        protected void resize_regression_issue2()
        {
            SetZoom(150);
            CreateAnnotation(new MultiPageSelectionData(0, PagePosition.End, -20, 0, PagePosition.End, -10));
            Point initialLocation = CurrentlyAttachedStickyNote.Location;
            CurrentlyAttachedStickyNote.ResizeWithMouse(new Vector(20, 20));
            AssertEquals("Verify position after resize.", initialLocation, CurrentlyAttachedStickyNote.Location);
            passTest("Verified area has not regressed.");
        }

        #endregion PRIORITY TESTS
    }
}	

