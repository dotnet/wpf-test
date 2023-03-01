// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 


using System;
using System.Windows;
using Annotations.Test.Framework;
using System.Windows.Input;

namespace Avalon.Test.Annotations.Suites
{
    public class SingleNoteSuite : AStickyNoteAccessibilitySuite
    {
        #region BVT TESTS

        /// <summary>
        /// Variations      Initial State                       Action
        /// Iconified	    1 Note.                             Can tab into Note.
        ///                 Focus in Document, no selection	
        /// </summary>
        [OverrideClassTestDimensions]
        [TestDimension("flow,fixed)")]
        [TestDimension(",/expanded=false")]
        [Priority(0)]
        protected void singlenote1_1()
        {
            CreateDefaultNote();
            PageDown();
            PageUp();
            TextControl.Focus();
            TestCanTabToAnnotationGroup();
        }

        /// <summary>
        /// Variations      Initial State                       Action
        /// Ink,Iconified	1 Note.                             Can tab into Note.
        /// backwards       Focus in Document, selection before anchor
        /// </summary>
        [OverrideClassTestDimensions]
        [TestDimension("flow,fixed)")]
        [TestDimension(",/expanded=false")]
        [TestDimension(",inkstickynote")]
        [TestDimension(",/tabdirection=backward")]
        [Priority(0)]
        protected void singlenote1_2()
        {
            CreateDefaultNote();
            SetSelection(new SimpleSelectionData(DEFAULT_PAGE, DEFAULT_OFFSET - 10, 1));
            TestCanTabToAnnotationGroup();
        }

        /// <summary>
        /// Variations      Initial State                       Action
        /// ink, Iconified	1 Note.                             Can tab into Note.
        ///                 Window is focused.
        /// </summary>
        [OverrideClassTestDimensions]
        [TestDimension("flow,fixed)")]
        [TestDimension(",/expanded=false")]
        [TestDimension(",inkstickynote")]
        [Priority(0)]
        protected void singlenote1_5()
        {
            CreateDefaultNote();
            MainWindow.Focus();
            TestCanTabToAnnotationGroup();
        }

        /// <summary>
        /// Variations      Initial State                       Action
        /// ink, Iconified, 1 Note.                             Can tab out of AnnotationGroup
        /// backwards       Focus in note content.
        /// </summary>
        [OverrideClassTestDimensions]
        [TestDimension("flow,fixed)")]
        [TestDimension(",/expanded=false")]
        [TestDimension(",inkstickynote")]
        [Priority(0)]
        protected void singlenote2()
        {
            CreateDefaultNote();
            DispatcherHelper.DoEvents();
            CurrentlyAttachedStickyNote.InnerControl.Focus();
            DispatcherHelper.DoEvents();
            TabHelper.MoveToNextAnnotation();
            DispatcherHelper.DoEvents();
            Assert("Verify not on annotation anymore.", !TabHelper.IsCurrentElementAStickyNote);
            passTest("Can tab out of annotations.");
        }

        /// <summary>
        /// Variations      Initial State                       Action
        /// Ink, Iconified	1 Note.                             Tab out of annotation TabGroup, can Tab backwards into group.
        ///                 Focus in Note content.	
        /// </summary>
        [OverrideClassTestDimensions]
        [TestDimension("flow,fixed)")]
        [TestDimension(",/expanded=false")]
        [TestDimension(",inkstickynote")]
        [Priority(0)]
        protected void singlenote3()
        {
            CreateDefaultNote();
            CurrentlyAttachedStickyNote.InnerControl.Focus();
            TabHelper.MoveToNextAnnotation();
            Assert("Verify we Tab'd out of note.", !TabHelper.IsCurrentElementAStickyNote);
            TabHelper.MoveToPreviousAnnotation();
            Assert("Verify we Tab'd back to note.", TabHelper.IsCurrentElementAStickyNote);
            passTest("Can move in and out of group.");
        }


        /// <summary>
        /// Variations      Initial State               Action
        /// Ink, backwards	1 Note.                     Can Tab into Note and through note group: content, minimize button, tools menu.
        ///                 Focus in Document.	
        /// </summary>
        [OverrideClassTestDimensions]
        [TestDimension("flow,fixed)")]
        [TestDimension(",inkstickynote")]
        [TestDimension(",/tabdirection=backward")]
        [Priority(0)]
        protected void singlenote5()
        {
            CreateDefaultNote();
            ViewerBase.Focus();
            TabHelper.TabToAnnotationGroup();
            StickyNoteWrapper wrapper = CurrentlyAttachedStickyNote;
            TabHelper.LogFocusedElement();
            Assert("Verify in note InnerControl is focused.", wrapper.InnerControl == TabHelper.CurrentlyFocusedElement);
            TabHelper.Tab();
            TabHelper.LogFocusedElement();
            Assert("Verify tools menu is focused.", wrapper.Menu.Target == TabHelper.CurrentlyFocusedElement);
            TabHelper.Tab();
            TabHelper.LogFocusedElement();
            Assert("Verify minimize button is focused.", wrapper.CloseButton == TabHelper.CurrentlyFocusedElement);            
            TabHelper.Tab();
            TabHelper.LogFocusedElement();
            Assert("Verify in note InnerControl is focused again.", wrapper.InnerControl == TabHelper.CurrentlyFocusedElement);
            
            passTest("Verified tabbing through all note controls.");
        }

        /// <summary>
        /// Variations      Initial State               Action
        /// Ink	            1 Note.                     Tab to Note’s minimize button.
        ///                 Focus in Document.	        ‘Enter’ causes Note to be iconified.
        /// </summary>
        [OverrideClassTestDimensions]
        [TestDimension("flow,fixed)")]
        [TestDimension(",inkstickynote")]
        [Priority(0)]
        protected void singlenote7()
        {
            CreateDefaultNote();
            CurrentlyAttachedStickyNote.InnerControl.Focus();
            TabToCloseButton();
            UIAutomationModule.PressKey(Key.Enter);
            Assert("Verify note is iconified.", !CurrentlyAttachedStickyNote.Expanded);
            passTest("Note iconified with keyboard.");
        }

        /// <summary>
        /// Variations      Initial State               Action
        /// 1 Note.         Focus in Document.	        Can enter content.      
        ///                 Tab to Note’s content.
        /// </summary>
        [OverrideClassTestDimensions]
        [TestDimension("flow,fixed)")]
        [Priority(0)]
        protected void singlenote8()
        {
            CreateDefaultNote();
            ViewerBase.Focus();
            TabHelper.TabToAnnotationGroup();
            InsertContent(ContentKind.Standard_Small);
            VerifyContent(ContentKind.Standard_Small);
            passTest("Verified insert content in keyboard accessed note.");
        }

        /// <summary>
        /// Variations      Initial State               Action
        /// 1 Note.         Focus in Document.	        Tab to Note’s tools menu.
        ///                                             ‘Enter’ opens menu.
        ///                                             Can select ‘Delete’ using down arrow.
        /// </summary>
        [OverrideClassTestDimensions]
        [TestDimension("flow,fixed)")]
        [Priority(0)]
        protected void singlenote9()
        {
            CreateDefaultNote();
            TabToToolsMenu();
            UIAutomationModule.PressKey(Key.Enter);
            UIAutomationModule.PressKey(Key.Down);
            UIAutomationModule.PressKey(Key.Enter);
            VerifyNumberOfAttachedAnnotations(0);
            passTest("Verified note deleted with keyboard.");
        }

        /// <summary>
        /// Variations      Initial State               Action
        ///                 Iconified Note.             Tab to note.  
        ///                 Focus in Document.	        ‘Enter’ Expands note.
        ///                                             Can enter content.
        /// </summary>
        [OverrideClassTestDimensions]
        [TestDimension("flow,fixed)")]
        [TestDimension(",/tabdirection=backward")]
        [Priority(0)]
        protected void singlenote12()
        {
            CreateDefaultNote();
            CurrentlyAttachedStickyNote.Expanded = false;
            Assert("Verify note is NOT expanded.", !CurrentlyAttachedStickyNote.Expanded);
            ViewerBase.Focus();
            MoveMouseToCharacterOffset(DEFAULT_PAGE, 0); // Ensure mouse is not over anchor.
            printStatus("Verify initial icon size.");
            VerifyIconSize(StickyNoteWrapper.IconDefaultSize);
            TabHelper.TabToAnnotationGroup();
            printStatus("Verify focused icon size.");
            VerifyIconSize(StickyNoteWrapper.IconHoverSize);
            UIAutomationModule.PressKey(Key.Enter);
            Assert("Verify note is expanded.", CurrentlyAttachedStickyNote.Expanded);
            passTest("Verified restoring iconified note with keyboard.");
        }


        /// <summary>
        /// Variations      Initial State                           Action
        /// Backwards	    1 Note, Start of anchor is visible.     Can tab to Note.
        ///                 Focus in Document.	
        /// </summary>
        [OverrideClassTestDimensions]
        [TestDimension("flow,fixed)")]
        [TestDimension(",/tabdirection=backward")]
        [Priority(0)]
        protected void singlenote14()
        {
            CreateAnnotation(new MultiPageSelectionData(0, PagePosition.Middle, 0, 1, PagePosition.Middle, 0));
            ViewerBase.Focus();
            TabHelper.TabToAnnotationGroup();
            Assert("Verify note is focused.", TabHelper.IsCurrentElementAStickyNote);
            passTest("Verified tabbing to note with cross page anchor.");
        }

        /// <summary>
        /// Variations      Initial State                               Action
        /// Backwards	    1 Note, only end of anchor is visible.      Cannot tab to Note.
        ///                 Focus in Document.	
        /// </summary>
        [OverrideClassTestDimensions]
        [TestDimension("flow,fixed)")]
        [TestDimension(",/tabdirection=backward")]
        [Priority(0)]
        protected void singlenote15_2()
        {
            CreateAnnotation(new MultiPageSelectionData(0, PagePosition.Middle, 0, 1, PagePosition.Middle, 0));
            GoToPage(1);
            ViewerBase.Focus();
            TabHelper.TabToAnnotationGroup();
            Assert("Verify that annotation is note focused.", !TabHelper.IsCurrentElementAStickyNote);
            passTest("Cannot tab to note with only end of anchor visible.");
        }

        /// <summary>
        /// Variations      Initial State               Action
        ///                 1 iconified note.           Tab away from note.
        ///                 Note focused.	            Note changes from hover to small state.
        /// </summary>
        [OverrideClassTestDimensions]
        [TestDimension("flow,fixed)")]
        [Priority(0)]
        protected void singlenote16()
        {
            CreateAnnotation(new SimpleSelectionData(0, 54, 10));
            CurrentlyAttachedStickyNote.Expanded = false;
            CurrentlyAttachedStickyNote.InnerControl.Focus();

            MoveMouseToCharacterOffset(0, 0);
            printStatus("Verify initial icon size.");
            VerifyIconSize(StickyNoteWrapper.IconHoverSize);
            TabHelper.MoveToNextAnnotation();
            printStatus("Verify final icon size.");
            VerifyIconSize(StickyNoteWrapper.IconDefaultSize);

            passTest("Verified icon size change.");
        }

        #endregion BVT TESTS

        #region PRIORITY TESTS

        /// <summary>
        /// Variations      Initial State                       Action
        /// Iconified	    1 Note.                             Can tab into Note.
        ///                 Focus in Document, selection after anchor
        /// </summary>
        [Priority(1)]
        protected void singlenote1_3()
        {
            CreateDefaultNote();
            SetSelection(new SimpleSelectionData(DEFAULT_PAGE, DEFAULT_OFFSET + DEFAULT_LENGTH + 10, 1));
            TestCanTabToAnnotationGroup();
        }

        /// <summary>
        /// Variations      Initial State                       Action
        /// Iconified	    1 Note.                             Can tab into Note.
        ///                 Focus in Document, whole document selected.
        /// </summary>
        [Priority(1)]
        protected void singlenote1_4()
        {
            CreateDefaultNote();
            SetSelection(new MultiPageSelectionData(0, PagePosition.Beginning, 0, ViewerBase.Document.DocumentPaginator.PageCount - 1, PagePosition.End, 0));
            TestCanTabToAnnotationGroup();
        }

        /// <summary>
        /// Variations          Initial State               Action
        /// Ink, Iconified,     1 Note.                     Can Tab into Note and then out of annotation TabGroup.
        /// backwards	        Focus in Document.	
        /// </summary>
        [Priority(1)]
        protected void singlenote4()
        {
            CreateDefaultNote();
            ViewerBase.Focus();
            TabHelper.TabToAnnotationGroup();
            Assert("Verify in annotation group.", TabHelper.IsCurrentElementAStickyNote);
            TabHelper.MoveToNextAnnotation();
            Assert("Verify no longer in annotation group.", !TabHelper.IsCurrentElementAStickyNote);
            passTest("Can tab out of annotation group.");
        }

        /// <summary>
        /// Variations          Initial State               Action
        /// Ink	                Focus in content of Note	Can Tab out of annotation’s TabGroup
        /// </summary>
        [Priority(1)]
        protected void singlenote6_1()
        {
            CreateDefaultNote();
            CurrentlyAttachedStickyNote.InnerControl.Focus();
            TestCanTabOutOfAnnotationGroup();
        }

        /// <summary>
        /// Variations          Initial State                       Action
        /// Ink	                Focus on minimize icon of Note	    Can Tab out of annotation’s TabGroup
        /// </summary>
        [Priority(1)]
        protected void singlenote6_2()
        {
            InitiallyExpanded = false;
            CreateDefaultNote();
            DispatcherHelper.DoEvents();
            CurrentlyAttachedStickyNote.InnerControl.Focus();
            DispatcherHelper.DoEvents();
            TestCanTabOutOfAnnotationGroup();
            DispatcherHelper.DoEvents();
        }

        /// <summary>
        /// Variations          Initial State                   Action
        /// Ink	                Focus on Tools menu of Note     Can Tab out of annotation’s TabGroup
        /// </summary>
        [Priority(1)]
        protected void singlenote6_3()
        {
            CreateDefaultNote();
            TabToToolsMenu();
            TestCanTabOutOfAnnotationGroup();
        }

        /// <summary>
        /// Variations          Initial State                   Action
        ///                     Focus in Note content.	        Insert content.
        ///                                                     Tab through not controls (e.g. back to content).
        ///                                                     Verify inserting more content.
        /// </summary>
        [OverrideClassTestDimensions]
        [TestDimension("flow, fixed")]
        [Priority(1)]
        protected void singlenote11()
        {
            CreateDefaultNote();
            CurrentlyAttachedStickyNote.InnerControl.Focus();
            UIAutomationModule.TypeString("Hello");
            TabHelper.Tab(3);
            UIAutomationModule.TypeString("World");
            AssertEquals("Verify Text content of RTB.", "HelloWorld\r\n", CurrentlyAttachedStickyNote.Content);
            passTest("Inserting text after tabbing.");
        }

        /// <summary>
        /// Variations          Initial State                   Action
        /// Ink	                1 Note.                         Tab to minimize button.            
        ///                     Focus in Note.	                ‘Enter’, note is iconified.
        ///                                                     Tab back to note.
        ///                                                     ‘Enter’, note is restored.
        ///                                                     Can insert content.
        /// </summary>
        [Priority(1)]
        protected void singlenote13()
        {
            CreateDefaultNote();
            CurrentlyAttachedStickyNote.InnerControl.Focus();
            TabHelper.Tab(2);
            UIAutomationModule.PressKey(Key.Enter);
            AssertEquals("Verify note is iconified.", false, Note.IsExpanded);
            TabHelper.TabToAnnotationGroup();
            UIAutomationModule.PressKey(Key.Enter);
            AssertEquals("Verify note is restored.", true, Note.IsExpanded);
            InsertContent(ContentKind.Standard_Small);
            VerifyContent(ContentKind.Standard_Small);
            passTest("Verified iconify/restore followed by inserting text.");
        }

        /// <summary>
        /// Variations      Initial State                               Action
        /// Backwards	    1 Note, only middle of anchor is visible.   Cannot tab to Note.
        ///                 Focus in Document.	
        /// </summary>
        [Priority(1)]
        protected void singlenote15_1()
        {
            CreateAnnotation(new MultiPageSelectionData(0, PagePosition.Middle, 0, 2, PagePosition.Middle, 0));
            GoToPage(1);
            ViewerBase.Focus();
            TabHelper.TabToAnnotationGroup();
            Assert("Verify that annotation is not focused.", !TabHelper.IsCurrentElementAStickyNote);
            passTest("Cannot tab to note with only middle of anchor visible.");
        }

        /// <summary>
        /// Variations      Initial State                               Action
        /// Backwards	    1 Note, only middle of anchor is visible.   Cannot tab to Note.
        ///                 Focus in Document.	
        /// </summary>
        [Priority(1)]
        protected void singlenote15_3()
        {
            CreateAnnotation(new SimpleSelectionData(1, PagePosition.Middle, 45));
            ViewerBase.Focus();
            TabHelper.TabToAnnotationGroup();
            Assert("Verify that annotation is not focused.", !TabHelper.IsCurrentElementAStickyNote);
            passTest("Cannot tab to non-visible note.");
        }

        /// <summary>
        /// Variations      Initial State               Action
        /// Backwards.	    1 iconified note.           Tab into note. 
        ///                 Focus in Document.          Icon remains in hover state.
        ///                 Mouse over anchor	
        /// </summary>
        [Priority(1)]
        protected void singlenote17()
        {
            CreateAnnotation(new SimpleSelectionData(0, 100, 1000));
            CurrentlyAttachedStickyNote.Expanded = false;
            ViewerBase.Focus();
            MoveMouseToCharacterOffset(0, 500);
            printStatus("Verify initial icon size.");
            VerifyIconSize(StickyNoteWrapper.IconHoverSize);
            TabHelper.TabToAnnotationGroup();
            printStatus("Verify final icon size.");
            VerifyIconSize(StickyNoteWrapper.IconHoverSize);
            passTest("Mouse over anchor doesn't affect focused icon size.");
        }

        /// <summary>
        /// Variations      Initial State                           Action
        ///                 1 iconified note.                       Mouse off anchor
        ///                 Focus in note.	Mouse over anchor       Click elsewhere in document.
        ///                                                         Icon remains in hover state.
        /// </summary>
        [OverrideClassTestDimensions]
        [TestDimension("flow, fixed")]
        [Priority(1)]
        protected void singlenote18()
        {
            CreateAnnotation(new SimpleSelectionData(0, 100, 200));
            CurrentlyAttachedStickyNote.Expanded = false;
            TabHelper.TabToAnnotationGroup();
            MoveMouseToCharacterOffset(0, 200);
            printStatus("Verify initial icon size.");
            VerifyIconSize(StickyNoteWrapper.IconHoverSize);
            MoveMouseToCharacterOffset(0, 320);
            printStatus("Verify icon size after mouse move.");
            VerifyIconSize(StickyNoteWrapper.IconHoverSize);
            UIAutomationModule.LeftMouseClick();
            printStatus("Verify final icon size.");
            VerifyIconSize(StickyNoteWrapper.IconDefaultSize);
            passTest("Focused icon size verified.");
        }

        #endregion PRIORITY TESTS

        #region Helper Methods

        /// <summary>
        /// PASS if MoveToNextAnnotation moves focus out of annotation TabGroup.
        /// </summary>
        [TestCase_Helper()]
        private void TestCanTabOutOfAnnotationGroup()
        {
            TabHelper.MoveToNextAnnotation();
            Assert("Verify no longer in annotation group.", !TabHelper.IsCurrentElementAStickyNote);
            passTest("Can tab out of annotation group.");
        }

        #endregion
    }
}	

