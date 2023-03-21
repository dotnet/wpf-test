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
using System.Windows.Ink;
using Microsoft.Test.Display;

namespace Avalon.Test.Annotations.Suites
{
    public class MenuSuite : AStickyNoteControlFunctionalSuite
    {
        #region BVT TESTS

        /// <summary>
        /// Copy/Paste.
        /// </summary>
        [OverrideClassTestDimensions]
        [TestDimension("fixed,flow")]
        [TestDimension("stickynote,inkstickynote")]
        [Priority(0)]
        [Keywords("MicroSuite")]
        protected void menu1()
        {
            CreateDefaultNote();
            SetContent(ContentKind.Standard_Small);
            Clipboard.Clear();
            CurrentlyAttachedStickyNote.SelectAll();
            CurrentlyAttachedStickyNote.Menu.Copy.Execute();
            VerifyContent(ContentKind.Standard_Small);
            VerifyClipboardContent(ContentKind.Standard_Small);
            printStatus("Verified: Menu->Copy");

            DeleteDefaultNote();
            VerifyNumberOfAttachedAnnotations(0);
            CreateDefaultNote();
            CurrentlyAttachedStickyNote.SelectAll();
            CurrentlyAttachedStickyNote.Menu.Paste.Execute();
            VerifyContent(ContentKind.Standard_Small);
            printStatus("Verified: Menu->Paste");

            passTest("Menu->Copy and Menu->Paste.");
        }

        /// <summary>
        /// Delete.
        /// Verify: note is deleted.
        /// </summary>
        [OverrideClassTestDimensions]
        [TestDimension("fixed,flow")]
        [TestDimension("stickynote,inkstickynote")]
        [Priority(0)]
        protected void menu2()
        {
            CreateDefaultNote();
            CurrentlyAttachedStickyNote.Menu.Delete.Execute();
            VerifyNumberOfAttachedAnnotations(0);
            passTest("Menu->Delete Note.");
        }

        /// <summary>
        /// Ink.
        /// Verify we can ink.
        /// </summary>
        [OverrideClassTestDimensions]
        [TestDimension("fixed,flow")]
        [TestDimension("inkstickynote")]
        [Priority(0)]
        protected void menu3()
        {
            EnsureInkMode();
            CreateDefaultNote();
            CurrentlyAttachedStickyNote.Menu.Erase.Execute();
            CurrentlyAttachedStickyNote.Menu.Ink.Execute();
            SetContent(ContentKind.Standard_Small);
            VerifyContent(ContentKind.Standard_Small);
            passTest("Menu->Ink.");
        }

        /// <summary>
        /// Select.
        /// Verify we can select ink.
        /// </summary>
        [DisabledTestCase()]
        [OverrideClassTestDimensions]
        [TestDimension("fixed,flow")]
        [TestDimension("inkstickynote")]
        [Priority(0)]
        protected void menu4()
        {
            EnsureInkMode();
            CreateDefaultNote();
            SetContent(ContentKind.Standard_Small);
            CurrentlyAttachedStickyNote.Menu.Select.Execute();

            // Do selection.
            Rect areaToSelection = InkBoundsToScreen(CurrentlyAttachedStickyNote.InkCanvas.Strokes.GetBounds());
            UIAutomationModule.MoveTo(areaToSelection.TopLeft);
            UIAutomationModule.LeftMouseDown();
            UIAutomationModule.MoveTo(areaToSelection.TopRight);
            UIAutomationModule.MoveTo(areaToSelection.BottomRight);
            UIAutomationModule.MoveTo(areaToSelection.BottomLeft);
            UIAutomationModule.LeftMouseUp();

            Assert("Verify strokes are selected.", CurrentlyAttachedStickyNote.InkCanvas.GetSelectedStrokes().Count > 0);
            passTest("Menu->Select.");
        }

        /// <summary>
        /// Erase
        /// Verify we can erase ink.
        /// </summary>
        [OverrideClassTestDimensions]
        [TestDimension("fixed,flow")]
        [TestDimension("inkstickynote")]
        [Priority(0)]
        protected void menu5()
        {
            EnsureInkMode();
            CreateDefaultNote();
            SetContent(ContentKind.Standard_Small);
            CurrentlyAttachedStickyNote.Menu.Erase.Execute();

            Assert("Verify there is ink to start with.", CurrentlyAttachedStickyNote.HasContent);

            // Delete each stroke.
            StrokeCollection strokes = CurrentlyAttachedStickyNote.InkCanvas.Strokes;
            Rect[] targets = new Rect[strokes.Count];
            // Erasing will modify the stroke collection so we can't be enumerating
            // over it at the same time.
            //
            for (int i = 0; i < strokes.Count; i++)
                targets[i] = InkBoundsToScreen(strokes[i].GetBounds());
            foreach (Rect target in targets)
            {
                UIAutomationModule.MoveTo(target.TopLeft);
                UIAutomationModule.LeftMouseDown();
                UIAutomationModule.MoveTo(target.BottomRight);
                UIAutomationModule.LeftMouseUp();
            }

            Assert("Verify strokes have been deleted.", !CurrentlyAttachedStickyNote.HasContent);
            passTest("Menu->Erase.");
        }

        #region Private Methods

        [TestCase_Helper()]
        private Rect InkBoundsToScreen(Rect relativeToNote)
        {
            Rect canvasBounds = UIAutomationModule.BoundingRectangle(CurrentlyAttachedStickyNote.InkCanvas);
            return new Rect(
                canvasBounds.X + Monitor.ConvertLogicalToScreen(Dimension.Width, relativeToNote.X),
                canvasBounds.Y + Monitor.ConvertLogicalToScreen(Dimension.Height, relativeToNote.Y),
                Monitor.ConvertLogicalToScreen(Dimension.Width, relativeToNote.Width),
                Monitor.ConvertLogicalToScreen(Dimension.Height, relativeToNote.Height));
        }

        #endregion

        #endregion BVT TESTS

        #region PRIORITY TESTS

        /// <summary>
        /// No content, nothing on clipboard.	
        /// Verify: copy disabled.
        /// </summary>
        [Priority(1)]
        protected void menu7() 
        {
            CreateDefaultNote();
            Clipboard.Clear();
            TestMenuItemEnablement(false, false);
        }

        /// <summary>
        /// Selected content, nothing on clipboard.	
        /// Verify: Copy enabled.
        /// </summary>
        [Priority(1)]
        protected void menu8()
        {
            CreateDefaultNote();
            SetContent(ContentKind.Standard_Small);
            CurrentlyAttachedStickyNote.SelectAll();
            Clipboard.Clear();
            TestMenuItemEnablement(true, false);
        }

        /// <summary>
        /// Content no selection.
        /// Verify: Copy disabled.
        /// </summary>
        [Priority(1)]
        protected void menu9()
        {
            CreateDefaultNote();
            SetContent(ContentKind.Standard_Small);
            Clipboard.Clear();
            TestMenuItemEnablement(false, false);
        }

        /// <summary>
        /// Content, selection in Viewer, no selection in note.
        /// Verify: Copy disabled.
        /// </summary>
        [Priority(1)]
        protected void menu10()
        {
            CreateDefaultNote();
            SetContent(ContentKind.Standard_Small);
            Clipboard.Clear();
            SetSelection(new SimpleSelectionData(0, 10, 100));
            TestMenuItemEnablement(false, false);
        }

        [TestCase_Helper()]
        private void TestMenuItemEnablement(bool copy, bool paste)
        {
            StickyNoteWrapper note = CurrentlyAttachedStickyNote;
            AssertEquals("Check enablement of 'Copy' command.", copy, note.Menu.Copy.IsEnabled);
            AssertEquals("Check enablement of 'Paste' command.", paste, note.Menu.Paste.IsEnabled);
            Assert("Check enablemtent of 'Delete' command.", note.Menu.Delete.IsEnabled);
            passTest("Command enablement verified.");
        }

        #endregion PRIORITY TESTS
    }
}	

