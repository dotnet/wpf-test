// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 

using System;
using System.Windows;
using Annotations.Test.Framework;
using Avalon.Test.Annotations;
using System.Windows.Media;
using Microsoft.Test.Display;

namespace Avalon.Test.Annotations.Suites
{
    public class FdsvBasicSuite : AFlowDocumentScrollViewerSuite
    {
        #region Automation [BVT] Tests

        /// <summary>
        /// Verify AbsoluteSelectionData works.
        /// </summary>
        [Priority(0)]
        protected void verify1()
        {
            printStatus("Offsets: '" + GetText(new AbsoluteSelectionData(0, 100)) + "'.");
            printStatus("Relative to end: '" + GetText(new AbsoluteSelectionData(PagePosition.End, -100)) + "'.");
            printStatus("Relative to Middle: '" + GetText(new AbsoluteSelectionData(PagePosition.Middle, 65)) + "'.");
            printStatus("Range: '" + GetText(new AbsoluteSelectionData(PagePosition.Beginning, 10, PagePosition.Middle, -200)) + "'.");
            passTest("No exception for different selections.");
        }

        /// <summary>
        /// Verify Visiblity APIs: rectangle.
        /// </summary>
        [Priority(0)]
        protected void verify2_1()
        {
            PageDown();
            VerifyAnchorVisibility(new AbsoluteSelectionData(0, 10), false);
            PageUp();
            VerifyAnchorVisibility(new AbsoluteSelectionData(0, 10), true);
            VerifyAnchorVisibility(new AbsoluteSelectionData(PagePosition.End, -10), false);
            VerifyAnchorVisibility(new AbsoluteSelectionData(PagePosition.Beginning, 0, PagePosition.End, 0), true);
            passTest("Verified testing viewport visiblity of various anchors.");
        }

        ///// <summary>
        ///// Verify Visiblity APIs: StickyNote.
        ///// </summary>
        //[Priority(0)]
        //protected void verify2_2()
        //{
        //    SetWindowHeight(WindowSize.Height - 50);

        //    CreateAnnotation(new AbsoluteSelectionData(0, 10), "A");
        //    PageDown();
        //    VerifyNoteViewportVisibility("A", false);
        //    PageUp();
        //    VerifyNoteViewportVisibility("A", true);
        //    CreateAnnotation(new AbsoluteSelectionData(PagePosition.End, -10), "B");
        //    VerifyNoteViewportVisibility("B", true);
        //    VerifyNoteViewportVisibility("A", false);
        //    GoToStart();
        //    VerifyNoteViewportVisibility("B", false);
        //    VerifyNoteViewportVisibility("A", true);
        //    CreateAnnotation(new AbsoluteSelectionData(PagePosition.Beginning, 0, PagePosition.End, 0), "C");
        //    VerifyNoteViewportVisibility("C", true);
            
        //    // Test viewport bounds.
        //    CreateAnnotation(new AbsoluteSelectionData(1900, 2001), "D");
        //    CreateAnnotation(new AbsoluteSelectionData(1800, 2001), "E");
        //    PageUp();
           
        //    // Disabling verification b/c validation method is unreliable due to theme differences.
        //    //VerifyNoteViewportVisibility("D", false);
        //    VerifyNoteViewportVisibility("E", true);
        //    PageDown();
        //    VerifyNoteViewportVisibility("D", true);
        //    VerifyNoteViewportVisibility("E", true);
        //    passTest("Verified testing viewport visiblity of various Notes.");
        //}

        #endregion

        #region BVT TESTS

        /// <summary>
        /// Variations      Description
        /// Nonvisible	    Create and delete annotation at middle of document.
        /// </summary>
        protected void DoBasic3(bool visible)
        {
            ISelectionData selection = new AbsoluteSelectionData(PagePosition.Middle, 25);
            PageDown(3);
            TestAddDelete(selection, true, visible);
        }
        [TestDimension("stickynote,highlight")]
        [Priority(0)]
        protected void basic3_visible() { DoBasic3(true); }
        [TestDimension("stickynote,highlight")]
        [Priority(0)]
        protected void basic3_nonvisible() { DoBasic3(false); }

        /// <summary>
        /// Variations      Description
        /// Nonvisible	    1. Create highlight.
        ///                 2. Clear portion.
        ///                 Verify: 1 highlight with 2 segments.
        /// </summary>
        protected void DoBasic4(bool visible)
        {
            if (!visible) PageDown();
            CreateAnnotation(new AbsoluteSelectionData(10, 100), AnnotationMode.Highlight);
            DeleteAnnotation(new AbsoluteSelectionData(50, 60), AnnotationMode.Highlight);
            if (!visible) PageUp();
            VerifyAnnotation(GetText(new AbsoluteSelectionData(10, 50)) + GetText(new AbsoluteSelectionData(60, 100)));
            passTest("Verified splitting highlight.");
        }
        [Priority(0)]
        protected void basic4_visible() { DoBasic4(true); }
        [Priority(0)]
        protected void basic4_nonvisible() { DoBasic4(false); }

        /// <summary>
        /// Variations      Description
        /// Nonvisible	    1. Create green highlight.
        ///                 2. Create red highlight over top.
        ///                 Verify: 1 red highlight.
        /// </summary>
        protected void DoBasic5(bool visible)
        {
            if (!visible) PageDown();
            ISelectionData selection = new AbsoluteSelectionData(50, 65);
            CreateAnnotation(new HighlightDefinition(selection, Colors.Green));
            CreateAnnotation(new HighlightDefinition(selection, Colors.Red));
            if (!visible) PageUp();
            VerifyAnnotation(GetText(selection));
            passTest("Verified merging highlights.");
        }
        [Priority(0)]
        protected void basic5_visible() { DoBasic5(true); }
        [Priority(0)]
        protected void basic5_nonvisible() { DoBasic5(false); }

        /// <summary>
        /// Create N annotations throughout document.
        /// Toggle AnnotationService.
        /// Verify annoations restored.
        /// </summary>
        [Priority(0)]
        protected void basic6()
        {
            CreateAnnotation(new StickyNoteDefinition(new AbsoluteSelectionData(PagePosition.Middle, -400), AnnotationMode.InkStickyNote, true, "A"));
            CreateAnnotation(new HighlightDefinition(new AbsoluteSelectionData(PagePosition.Beginning, 500), Colors.Blue));
            CreateAnnotation(new StickyNoteDefinition(new AbsoluteSelectionData(PagePosition.Middle, 300, PagePosition.Middle, 400), AnnotationMode.StickyNote, false, "B"));
            CreateAnnotation(new HighlightDefinition(new AbsoluteSelectionData(PagePosition.Beginning, 400, PagePosition.Beginning, 600), Colors.Purple));
            CreateAnnotation(new StickyNoteDefinition(new AbsoluteSelectionData(PagePosition.End, -2, PagePosition.End, -91), AnnotationMode.StickyNote, true, "C"));
            VerifyNumberOfAttachedAnnotations(5);
            DisableAnnotationService();
            SetupAnnotationService();
            VerifyNumberOfAttachedAnnotations(5);
            passTest("Verified annotations persisted through disable/re-enable");
        }

        /// <summary>
        /// Non-Visible annotation.
        /// Change width so that annotation is visible.
        /// </summary>
        [TestDimension("stickynote,highlight")]
        [Priority(0)]
        protected void basic7()
        {
            string author = "A";
            ISelectionData selection = new AbsoluteSelectionData(800, 900);
            CreateAnnotation(selection, author);
            ResizeWindow(new Size(450, 300));
            VerifyAnnotationVisibility(author, selection, false);
            ResizeWindow(new Size(900, 600));
            VerifyAnnotationVisibility(author, selection, true);
            passTest("Non-visible annotation became visible: resize.");
        }

        /// <summary>
        /// Non-visible annotation.
        /// Change zoom so that annotation is visible.
        /// </summary>
        [TestDimension("stickynote,highlight")]
        [Priority(0)]
        protected void basic8()
        {
            string author = "A";
            ISelectionData selection = new AbsoluteSelectionData(800, 900);
            CreateAnnotation(selection, author);
            SetZoom(500);
            VerifyAnnotationVisibility(author, selection, false);
            SetZoom(50);
            VerifyAnnotationVisibility(author, selection, true);
            passTest("Non-visible annotation became visible: zoom.");
        }

        /// <summary>
        /// Create multiple annotations.
        /// Zoom in to 300%.
        /// </summary>
        [Priority(0)]
        protected void basic9_1()
        {
            SetZoom(50);
            SetupBasic9();
            SetZoom(300);
            passTest("Verified no crash.");
        }
        /// <summary>
        /// Create multiple annotations. 
        /// Zoom out to 25%.
        /// </summary>
        [Priority(0)]
        protected void basic9_2()
        {
            SetupBasic9();
            SetZoom(25);
            passTest("Verified no crash.");
        }
        [TestCase_Helper()]
        protected void SetupBasic9()
        {
            CreateAnnotation(new StickyNoteDefinition(new AbsoluteSelectionData(100, 150), AnnotationMode.InkStickyNote, true, "A"));
            CreateAnnotation(new HighlightDefinition(new AbsoluteSelectionData(PagePosition.Beginning, 500), Colors.Blue));
            CreateAnnotation(new StickyNoteDefinition(new AbsoluteSelectionData(PagePosition.Beginning, 690, PagePosition.Middle, 0), AnnotationMode.StickyNote, false, "B"));
            CreateAnnotation(new HighlightDefinition(new AbsoluteSelectionData(450, 800), Colors.Purple));
            CreateAnnotation(new StickyNoteDefinition(new AbsoluteSelectionData(PagePosition.End, -2, PagePosition.End, -91), AnnotationMode.StickyNote, true, "C"));
        }

        /// <summary>
        /// 1. Create annotations.
        /// 2. Set Document to null.
        /// 3. Set Document back to content.
        /// Verify: annotations restored.
        /// </summary>
        [Priority(0)]
        protected void basic15()
        {
            SetupBasic9();
            SetContent(null);
            VerifyNumberOfAttachedAnnotations(0);
            SetContent(LoadContent(Simple));
            VerifyNumberOfAttachedAnnotations(5);
            passTest("Verified clearing/restoring content.");
        }

        /// <summary>
        /// In Paginated viewer:
        /// Create annotation on each page.
        /// Create some annotations across page breaks.
        /// Load this store into FDSV, verify annotations.
        /// </summary>
        [Priority(0)]
        protected void basic16()
        {
            SwitchTextControl(AnnotatableTextControlTypes.FlowDocumentPageViewer);
            CreateAnnotation(new StickyNoteDefinition(new SimpleSelectionData(0, 10, 100), AnnotationMode.InkStickyNote, false, "A"));
            CreateAnnotation(new StickyNoteDefinition(new MultiPageSelectionData(2, PagePosition.End, -61, 3, PagePosition.Beginning, 94), AnnotationMode.StickyNote, true, "Cross-Page"));
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(4, 500, 100), Colors.Blue));
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(4, 550, 25), Colors.Red));
            SwitchTextControl(AnnotatableTextControlTypes.FlowDocumentScrollViewer);
            VerifyNumberOfAttachedAnnotations(4);
            passTest("Paginated to Scroll view.");
        }

        /// <summary>
        /// In Scroll Viewer:
        /// Create multiple annotations of various types throughout the document.
        /// Load this store into FDPV, verify annotations.
        /// </summary>
        [Priority(0)]
        protected void basic17()
        {
            CreateAnnotation(new HighlightDefinition(new AbsoluteSelectionData(PagePosition.End, -500), Colors.Blue));
            CreateAnnotation(new HighlightDefinition(new AbsoluteSelectionData(PagePosition.End, -250), Colors.Yellow));
            CreateAnnotation(new StickyNoteDefinition(new AbsoluteSelectionData(PagePosition.Middle, 0, PagePosition.End, 0), AnnotationMode.StickyNote, true, "B"));
            CreateAnnotation(new StickyNoteDefinition(new AbsoluteSelectionData(PagePosition.Beginning, 0, PagePosition.End, 0), AnnotationMode.InkStickyNote, true, "A"));
            SwitchTextControl(AnnotatableTextControlTypes.FlowDocumentPageViewer);
            ADocumentViewerBaseWrapper docViewerWrapper = (ADocumentViewerBaseWrapper)TextControlWrapper;
            docViewerWrapper.GoToPage(0);
            VerifyNumberOfAttachedAnnotations(1);
            docViewerWrapper.GoToPage(4);
            VerifyNumberOfAttachedAnnotations(2);
            docViewerWrapper.GoToEnd();
            VerifyNumberOfAttachedAnnotations(4);
            passTest("Scroll view to Paginated.");
        }

        #endregion BVT TESTS

        #region PRIORITY TESTS

        /// <summary>
        /// Variations      Description
        /// Nonvisible	    Create and delete annotation at start of document.
        /// </summary>         
        protected void DoBasic1(bool visible)
        {
            TestAddDelete(new AbsoluteSelectionData(PagePosition.Beginning, 100), false, visible);
        }
        [Priority(1)]
        protected void basic1_visible() { DoBasic1(true); }
        [Priority(1)]
        protected void basic1_nonvisible() { DoBasic1(false); }

        /// <summary>
        /// Variations      Description
        /// Nonvisible	    Create and delete annotation at end of document.
        /// </summary>
        protected void DoBasic2(bool visible)
        {
            GoToEnd();
            TestAddDelete(new AbsoluteSelectionData(PagePosition.End, -1, PagePosition.End, -100), true, visible);
        }
        [Priority(1)]
        protected void basic2_visible() { DoBasic2(true); }
        [Priority(1)]
        protected void basic2_nonvisible() { DoBasic2(false); }

        /// <summary>
        /// Create StickyNote.
        /// Scroll.
        /// Verify Iconify/restore.
        /// </summary>
        [Priority(1)]
        protected void basic11()
        {
            CreateAnnotation(new AbsoluteSelectionData(100, 200));
            TextControlWrapper.ScrollDown(3);
            CurrentlyAttachedStickyNote.MinimizeWithMouse();
            Assert("Verify note is minimized.", !CurrentlyAttachedStickyNote.Expanded);
            CurrentlyAttachedStickyNote.ClickIn();
            Assert("Verify note is restored.", CurrentlyAttachedStickyNote.Expanded);
            passTest("Verified iconify/restore.");
        }

        /// <summary>
        /// Create StickyNote.
        /// Scroll.
        /// Verify Resize.
        /// </summary>
        [Priority(1)]
        protected void basic12()
        {
            CreateAnnotation(new AbsoluteSelectionData(100, 200));
            TextControlWrapper.ScrollDown(5);
            CurrentlyAttachedStickyNote.ResizeWithMouse(new Vector(-50, 50));
            CurrentlyAttachedStickyNote.ResizeWithMouse(new Vector(100, 100));
            passTest("Verified resizing note.");
        }

        /// <summary>
        /// Create StickyNote at top of document.
        /// Drag Note away from anchor.
        /// Resize viewer so that anchor is visible but not Note.        
        /// </summary>
        [Priority(1)]
        protected void basic13()
        {
            CreateAnnotation(new AbsoluteSelectionData(100, 200), "A");
            CurrentlyAttachedStickyNote.Drag(new Vector(Monitor.ConvertLogicalToScreen(Dimension.Width, 0), Monitor.ConvertLogicalToScreen(Dimension.Height, 300)));
            VerifyNoteViewportVisibility("A", true);
            SetWindowHeight(366);
            VerifyNoteViewportVisibility("A", false);
            passTest("Verified anchor visible while note is not.");
        }

        /// <summary>
        /// Create StickyNote at top of document.
        /// Drag Note out of visible area.      
        /// </summary>
        [Priority(1)]
        protected void basic14()
        {
            CreateAnnotation(new AbsoluteSelectionData(10, 20), "A");
            CurrentlyAttachedStickyNote.Move(new Vector(0, 1000));
            VerifyNoteViewportVisibility("A", false);
            passTest("Verified note dragged outside visible area.");
        }

        /// <summary>
        /// Create N annotations throught document.
        /// Select All.
        /// Delete annotations.
        /// </summary>
        [Priority(1)]
        protected void basic18()
        {
            SetWindowFocus();
            CreateAnnotation(new AbsoluteSelectionData(10, 100));
            CreateAnnotation(new AbsoluteSelectionData(PagePosition.Middle, -1000, PagePosition.Middle, -10));
            CreateAnnotation(new AbsoluteSelectionData(PagePosition.Middle, 1911, PagePosition.Middle, 2001));
            CreateAnnotation(new AbsoluteSelectionData(PagePosition.Middle, 0, PagePosition.Middle, 1950));
            CreateAnnotation(new AbsoluteSelectionData(PagePosition.End, -921));
            VerifyNumberOfAttachedAnnotations(5);
            TextControlWrapper.Target.Focus();
            UIAutomationModule.Ctrl(System.Windows.Input.Key.A);
            Assert("Verify selection is not empty.", !TextControlWrapper.SelectionModule.Selection.IsEmpty);
            DeleteAnnotation();
            VerifyNumberOfAttachedAnnotations(0);
            passTest("Select All + Delete");
        }

        #endregion Priority Tests
    }
}

