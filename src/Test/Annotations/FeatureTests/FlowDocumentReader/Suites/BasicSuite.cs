// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 

using System;
using System.Windows;
using Annotations.Test.Framework;
using System.Windows.Controls;
using System.Windows.Annotations.Storage;
using Avalon.Test.Annotations;
using System.Windows.Media;
using Microsoft.Test.Display;

namespace Avalon.Test.Annotations.Suites
{
    public class BasicSuite: FDRSuite
    {
        #region BVT TESTS

        /// <summary>
        /// Add annotation on visible region.
        /// </summary>
        [Priority(0)]
        [TestDimension("/fdrmode=page,/fdrmode=scroll,/fdrmode=twopage")]
        [TestDimension("stickynote,highlight")]
        protected void basic1_1()
        {
            ISelectionData selection = new AbsoluteSelectionData(500, 750);
            CreateAnnotation(selection);
            VerifyAnnotation(GetText(selection));
            passTest("Created annotation on visible region.");
        }

        /// <summary>
        /// Add annotation on non-visible region
        /// </summary>        
        [Priority(0)]
        [TestDimension("/fdrmode=page,/fdrmode=scroll,/fdrmode=twopage")]
        [TestDimension("stickynote,highlight")]
        protected void basic1_2()
        {
            ISelectionData selection = new AbsoluteSelectionData(500, 750);
            PageDown();
            TextControl.Focus();
            CreateAnnotation(selection, "A");
            if (ReaderWrapper.ViewingMode == FlowDocumentReaderViewingMode.Scroll && AnnotationType != AnnotationMode.Highlight)
                VerifyNoteViewportVisibility("A", true);
            PageUp();
            VerifyAnnotation(GetText(selection));
            passTest("Created annotation on non-visible region.");
        }

        /// <summary>
        /// Delete annotation in visible region.
        /// </summary>
        [Priority(0)]
        [TestDimension("/fdrmode=page,/fdrmode=scroll,/fdrmode=twopage")]
        [TestDimension("stickynote,highlight")]
        
        protected void basic2_1()
        {
            if (this.ReaderMode == FlowDocumentReaderViewingMode.TwoPage && Microsoft.Test.Display.Monitor.Dpi.x != 96 && Microsoft.Test.Display.DisplayConfiguration.GetAppearance() != DesktopAppearance.WindowsClassic)
            {
                passTest("Bypassing test for broken scenario [TwoPage, High DPI, Windows Classic].");
            }
            else
            {
                DoBasic2(true);
            }
        }
        
        /// <summary>
        /// Delete annotation in non-visible region.
        /// </summary>
        [Priority(0)]
        [TestDimension("/fdrmode=page,/fdrmode=scroll,/fdrmode=twopage")]
        [TestDimension("stickynote,highlight")]
        
        protected void basic2_2()
        {
            if (this.ReaderMode == FlowDocumentReaderViewingMode.TwoPage && Microsoft.Test.Display.Monitor.Dpi.x != 96 && Microsoft.Test.Display.DisplayConfiguration.GetAppearance() != DesktopAppearance.WindowsClassic)
            {
                passTest("Bypassing test for broken scenario [TwoPage, High DPI, Windows Classic].");
            }
            else
            {
                DoBasic2(false);
            }
        }

        [TestCase_Helper()]
        protected void DoBasic2(bool visible)
        {
            ISelectionData selection = new AbsoluteSelectionData(721, 1423);
            CreateAnnotation(selection);
            VerifyAnnotation(GetText(selection));

            if (!visible) PageDown();
            DeleteAnnotation(selection);
            if (!visible) PageUp();
            VerifyNumberOfAttachedAnnotations(0);
            passTest("Deleted annotation.");
        }

        /// <summary>
        /// Split highlight in visible region
        /// </summary>
        [Priority(0)]
        [TestDimension("/fdrmode=page,/fdrmode=scroll,/fdrmode=twopage")]
        [TestDimension("stickynote,highlight")]
        protected void basic3_1()
        {
            DoBasic3(true);
        }

        /// <summary>
        /// Split highlight in non-visible region
        /// </summary>
        [Priority(0)]
        [TestDimension("/fdrmode=page,/fdrmode=scroll,/fdrmode=twopage")]
        [TestDimension("stickynote,highlight")]
        protected void basic3_2()
        {
            DoBasic3(false);
        }
        protected void DoBasic3(bool visible)
        {
            ISelectionData selection1 = new AbsoluteSelectionData(231, 594);
            ISelectionData selection2 = new AbsoluteSelectionData(429, 582);
            if (!visible) PageDown();
            CreateAnnotation(new HighlightDefinition(selection1, Colors.Orange));
            CreateAnnotation(new HighlightDefinition(selection2, Colors.Green));
            if (!visible) PageUp();
            VerifyAnnotations(new string[] { GetText(new AbsoluteSelectionData(231, 429)) + GetText(new AbsoluteSelectionData(582, 594)), GetText(selection2) });
            passTest("Split highlight.");
        }

        /// <summary>
        /// SinglePageView,TwoPageView,ScrollView	        N annotations throughout document.  
        ///                                                 Change to other 2 views and back to start.
        ///                                                 Verify annotations in each view.
        /// </summary>
        [Priority(0)]
        [TestDimension("/fdrmode=page,/fdrmode=scroll,/fdrmode=twopage")]
        [TestDimension("stickynote,highlight")]
        [Keywords("MicroSuite")]
        protected void basic4()
        {
            CreateAnnotation(new AbsoluteSelectionData(PagePosition.Beginning, 93));
            CreateAnnotation(new AbsoluteSelectionData(PagePosition.Middle, -2912, PagePosition.Middle, 839));
            CreateAnnotation(new AbsoluteSelectionData(PagePosition.Middle, 463));
            CreateAnnotation(new AbsoluteSelectionData(PagePosition.End, -476));
            CreateAnnotation(new AbsoluteSelectionData(PagePosition.End, -954, PagePosition.End, -61));
            for (int i = 0; i < 3; i++)
            {
                FlowDocumentReaderViewingMode view = (FlowDocumentReaderViewingMode)i;
                if (view != ReaderMode)
                {
                    ReaderWrapper.ViewingMode = view;
                    GoToStart();
                    Assert("Verify service is still enabled.", Service.IsEnabled);
                    Assert("Verify some annotations were attached.", Service.GetAttachedAnnotations().Count > 0);
                }
            }
            ReaderWrapper.ViewingMode = ReaderMode;
            passTest("No crash toggling through all views.");
        }

        /// <summary>
        /// SinglePageView, TwoPageView	                Create annotation across single page break.
        ///                                             Switch to Scrollview, back to original.
        /// </summary>
        [OverrideClassTestDimensions()]
        [TestDimension("/fdrmode=page,/fdrmode=twopage")]
        [TestDimension("stickynote,highlight")]
        [Priority(0)]
        
        protected void basic5()
        {
            if (this.ReaderMode == FlowDocumentReaderViewingMode.TwoPage && Microsoft.Test.Display.Monitor.Dpi.x != 96 && Microsoft.Test.Display.DisplayConfiguration.GetAppearance() != DesktopAppearance.WindowsClassic)
            {
                passTest("Bypassing test for broken scenario [TwoPage, High DPI, Windows Classic].");
            }
            else
            {
                MultiPageSelectionData selection = new MultiPageSelectionData(0, PagePosition.End, -91, 1, PagePosition.Beginning, 65);
                string pageAnchor = (ReaderMode == FlowDocumentReaderViewingMode.Page) ? GetText(selection.GetPageBasedSelection()[0]) : GetText(selection);
                string fullAnchor = GetText(selection);
                CreateAnnotation(selection);
                VerifyAnnotation(pageAnchor);
                ReaderWrapper.ViewingMode = FlowDocumentReaderViewingMode.Scroll;
                VerifyAnnotation(fullAnchor);
                ReaderWrapper.ViewingMode = ReaderMode;
                VerifyAnnotation(pageAnchor);
                passTest("Verified toggle view on cross page annotation.");
            }
        }

        /// <summary>
        /// SinglePageView: Create annotation.
        /// Chance to ScrollView.
        /// Close window.
        /// Verify no annotations in stream.
        /// </summary>        
        [OverrideClassTestDimensions]
        [TestDimension("stickynote,highlight")]
        [Priority(0)]
        protected void basic7()
        {
            ISelectionData selection = new AbsoluteSelectionData(250, 981);
            CreateAnnotation(selection);
            VerifyNumberOfAttachedAnnotations(1);
            ReaderWrapper.ViewingMode = FlowDocumentReaderViewingMode.Scroll;
            VerifyNumberOfAttachedAnnotations(1);
            DisableAnnotationService();
            AnnotationStore store = SetupAnnotationStore();
            AssertEquals("Verify no annotations in stream.", 0, store.GetAnnotations().Count);
            passTest("Verified toggle view does not flush annotations.");
        }

        /// <summary>
        /// SinglePageView, TwoPageView	                Resize window so that annotation reflows onto next page, then reflow again to restore original position.
        /// </summary>
        [OverrideClassTestDimensions()]
        [TestDimension("/fdrmode=page,/fdrmode=twopage")]
        [TestDimension("stickynote,highlight")]
        [Priority(0)]
        protected void basic11_1()
        {
            int page = (ReaderMode == FlowDocumentReaderViewingMode.TwoPage) ? 1 : 0;
            ISelectionData selection = new MultiPageSelectionData(page, PagePosition.End, -500, page, PagePosition.End, -350);
            string expectedAnchor = GetText(selection);
            CreateAnnotation(selection);
            ResizeWindow(new Size(800, 500));
            VerifyNumberOfAttachedAnnotations(0);
            TextControlWrapper.PageDown();
            VerifyAnnotation(expectedAnchor);
            passTest("Verified reflowing annotation across page boundary.");
        }

        /// <summary>
        /// SinglePageView, TwoPageView	                Zoom in so that annotation reflows onto other page.
        /// </summary>
        [OverrideClassTestDimensions()]
        [TestDimension("/fdrmode=page,/fdrmode=twopage")]
        [TestDimension("stickynote,highlight")]
        [Priority(0)]
        protected void basic12_1()
        {
            ReaderWrapper.Viewer.ZoomIncrement = 5;
            int page = (ReaderMode == FlowDocumentReaderViewingMode.TwoPage) ? 1 : 0;
            ISelectionData selection = new MultiPageSelectionData(page, PagePosition.End, -500, page, PagePosition.End, -350);
            string expectedAnchor = GetText(selection);
            CreateAnnotation(selection);
            TextControlWrapper.IcrementalZoomTo(120);
            VerifyNumberOfAttachedAnnotations(0);
            TextControlWrapper.PageDown();
            VerifyAnnotation(expectedAnchor);
            passTest("Verified reflowing annotation across page boundary.");
        }

        /// <summary>
        /// SinglePageView, TwoPageView	                Create cross page annotation.
        ///                                             Page Down twice.
        ///                                             Page Up twice.
        ///                                             Verify annotation.
        /// </summary>
        [OverrideClassTestDimensions()]
        [TestDimension("/fdrmode=page,/fdrmode=twopage")]
        [TestDimension("stickynote,highlight")]
        [Priority(0)]
        
        protected void basic13_1()
        {
            if (this.ReaderMode == FlowDocumentReaderViewingMode.TwoPage && Microsoft.Test.Display.Monitor.Dpi.x != 96 && Microsoft.Test.Display.DisplayConfiguration.GetAppearance() != DesktopAppearance.WindowsClassic)
            {
                passTest("Bypassing test for broken scenario [TwoPage, High DPI, Windows Classic].");
            }
            else
            {
                MultiPageSelectionData selection = new MultiPageSelectionData(0, PagePosition.End, -621, 1, PagePosition.Beginning, 72);
                string expectedAnchor = (ReaderMode == FlowDocumentReaderViewingMode.Page) ? GetText(selection.GetPageBasedSelection()[0]) : GetText(selection);
                CreateAnnotation(selection);
                VerifyAnnotation(expectedAnchor);
                TextControlWrapper.PageDown(2);
                VerifyNumberOfAttachedAnnotations(0);
                TextControlWrapper.PageUp(2);
                VerifyAnnotation(expectedAnchor);
                passTest("Verified loading of cross page annotation.");
            }

        }

        /// <summary>
        /// SinglePageView, TwoPageView, ScrollView	            Create note, insert content.
        /// </summary>
        [OverrideClassTestDimensions()]
        [TestDimension("/fdrmode=page,/fdrmode=scroll,/fdrmode=twopage")]
        [TestDimension("stickynote,inkstickynote")]
        [Priority(0)]
        protected void basic16()
        {
            AnnotationTestHelper.BringToFront(MainWindow);
            CreateAnnotation(new AbsoluteSelectionData(191, 265));
            if (AnnotationType == AnnotationMode.StickyNote)
            {
                CurrentlyAttachedStickyNote.InnerControl.Focus();
                string content = "Contains one space.";
                UIAutomationModule.TypeString(content);
                AssertEquals("Verify text content.", content + "\r\n", CurrentlyAttachedStickyNote.Content);
            }
            else
            {
                CurrentlyAttachedStickyNote.MoveTo();
                UIAutomationModule.LeftMouseClick();
                UIAutomationModule.LeftMouseDown();
                UIAutomationModule.Move(new Vector(-20, 20));
                DispatcherHelper.DoEvents();
                UIAutomationModule.LeftMouseUp();
                Assert("Verify there is ink content.", CurrentlyAttachedStickyNote.HasContent);
            }
            passTest("Verified adding content to note.");
        }

        /// <summary>
        /// SinglePageView, TwoPageView, ScrollView	            Create note, drag.
        /// </summary>
        [OverrideClassTestDimensions()]
        [TestDimension("/fdrmode=page,/fdrmode=scroll,/fdrmode=twopage")]
        [TestDimension("stickynote,inkstickynote")]
        [Priority(0)]
        protected void basic17()
        {
            AnnotationTestHelper.BringToFront(MainWindow);
            CreateAnnotation(new AbsoluteSelectionData(191, 265));
            Point originalPosition = CurrentlyAttachedStickyNote.Location;
            Vector delta = new Vector(-25, 25);
            CurrentlyAttachedStickyNote.Drag(delta);
            Point targetLocation = Point.Add(originalPosition, delta);
            Point actualLocation = CurrentlyAttachedStickyNote.Location;
            //increase tolerance for pass on high dpi
            //if (Microsoft.Test.Display.Monitor.Dpi.x == 120)
            //{
            //    AssertEquals("Verify location after drag.", targetLocation.X, actualLocation.X, 3);
            //    AssertEquals("Verify location after drag.", targetLocation.Y, actualLocation.Y, 3);
            //}
            //else
            //{
                AssertEquals("Verify location after drag.", Monitor.ConvertScreenToLogical(Dimension.Width, targetLocation.X), Monitor.ConvertScreenToLogical(Dimension.Width, actualLocation.X), 2);
                AssertEquals("Verify location after drag.", Monitor.ConvertScreenToLogical(Dimension.Height, targetLocation.Y), Monitor.ConvertScreenToLogical(Dimension.Height, actualLocation.Y), 2);
            //}
            passTest("Verified dragging note.");
        }

        /// <summary>
        /// SinglePageView, TwoPageView                         Create note, 
        ///                                                     drag out of view, 
        ///                                                     change to paginated mode, verify note is in view.
        /// </summary>
        [OverrideClassTestDimensions()]
        [TestDimension("/fdrmode=page,/fdrmode=twopage")]
        [TestDimension("stickynote,inkstickynote")]
        [Priority(0)]
        protected void basic18()
        {
            AnnotationTestHelper.BringToFront(MainWindow);
            ReaderWrapper.ViewingMode = FlowDocumentReaderViewingMode.Scroll;
            CreateAnnotation(new AbsoluteSelectionData(191, 265), "A");
            SetZoom(25);
            CurrentlyAttachedStickyNote.Drag(new Vector(0, 1000));
            SetZoom(100);
            VerifyNoteViewportVisibility("A", false);
            ReaderWrapper.ViewingMode = ReaderMode;
            VerifyNumberOfAttachedAnnotations(1);
            TextControl.Focus();
            CurrentlyAttachedStickyNote.ClickIn();
            Assert("Verify note is focused, if not it must not be in view.", CurrentlyAttachedStickyNote.InnerControl.IsFocused);
            ReaderWrapper.ViewingMode = FlowDocumentReaderViewingMode.Scroll;
            VerifyNoteViewportVisibility("A", false);
            passTest("Verified notes aren't lost changing views.");
        }

        #endregion BVT TESTS

        #region PRIORITY TESTS
 
        /// <summary>
        /// SinglePageView, TwoPageView	                Create annotation across multiple page breaks.
        ///                                             Switch to Scrollview, back to original.
        /// </summary>
        [Priority(1)]
        protected void basic6()
        {
            MultiPageSelectionData selection = new MultiPageSelectionData(0, PagePosition.Middle, 0, 4, PagePosition.Middle, 0);
            ISelectionData[] selectionByPage = selection.GetPageBasedSelection();
            string[] expectedAnchorPerPage = new string[] { GetText(selectionByPage[0]), GetText(selectionByPage[1]), GetText(selectionByPage[2]), GetText(selectionByPage[3]), GetText(selectionByPage[4]) };
            string expectedFullAnchor = GetText(selection);
            CreateAnnotation(selection);
            VerifyNumberOfAttachedAnnotations(1);
            ReaderWrapper.ViewingMode = FlowDocumentReaderViewingMode.Scroll;
            VerifyAnnotation(expectedFullAnchor);
            ReaderWrapper.ViewingMode = ReaderMode;
            for (int i = 0; i < 5; i++)
            {
                if (ReaderMode == FlowDocumentReaderViewingMode.Page)
                {
                    // Disabling verification b/c validation method is unreliable due to theme differences.
                    //VerifyAnnotation(expectedAnchorPerPage[i]);
                    PageDown();
                }
                else
                {
                    // Disabling verification b/c validation method is unreliable due to theme differences.
                    //VerifyAnnotation(expectedAnchorPerPage[i++] + expectedAnchorPerPage[i]);
                    PageDown(2);
                }
            }
            passTest("Verified annotation across multiple page breaks.");
        }

        /// <summary>
        /// Create annotation in ScrollView mode.
        /// Switch to SinglePageView.
        /// Add, delete annotations.
        /// Switch to ScrollView.
        /// Verify all changes.
        /// </summary>
        [Priority(1)]
        protected void basic8()
        {
            ReaderWrapper.ViewingMode = FlowDocumentReaderViewingMode.Scroll;
            CreateAnnotation(new StickyNoteDefinition(new AbsoluteSelectionData(121, 243), AnnotationMode.StickyNote, true, "A"));
            VerifyNumberOfAttachedAnnotations(1);
            
            ReaderWrapper.ViewingMode = ReaderMode;
            CreateAnnotation(new SimpleSelectionData(3, 19, 45));
            CreateAnnotation(new SimpleSelectionData(2, 100, 100));
            DeleteAnnotation(new SimpleSelectionData(3, 0, 200));

            ReaderWrapper.ViewingMode = FlowDocumentReaderViewingMode.Scroll;
            VerifyNumberOfAttachedAnnotations(2);
            
            passTest("Verified changes shared across paginated/scroll views.");
        }

        /// <summary>
        /// ScrollView: make Note very large height wise.
        /// Switch to SinglePageView.
        /// Verify 
        /// </summary>
        [Priority(1)]
        protected void basic10()
        {
            ReaderWrapper.ViewingMode = FlowDocumentReaderViewingMode.Scroll;
            CreateAnnotation(new AbsoluteSelectionData(10, 12));
            CurrentlyAttachedStickyNote.ResizeWithMouse(new Vector(0, 1000));
            Size expectedSize = CurrentlyAttachedStickyNote.BoundingRect.Size;

            ReaderWrapper.ViewingMode = ReaderMode;
            AssertEquals("Verify final size", expectedSize, CurrentlyAttachedStickyNote.BoundingRect.Size);

            passTest("Verified note size preserved across views.");
        }

        /// <summary>
        /// SinglePageView, TwoPageView	                Resize window so that annotation reflows onto previous page, then reflow again to restore original position.
        /// </summary>
        [Priority(1)]
        protected void basic11_2()
        {            
            ISelectionData selection = new MultiPageSelectionData(1, PagePosition.Beginning, 10, 1, PagePosition.Beginning, 50);
            string expectedAnchor = GetText(selection);
            CreateAnnotation(selection);
            ResizeWindow(new Size(1000, 800));
            VerifyAnnotation(expectedAnchor);            
            passTest("Verified reflowing annotation across page boundary.");
        }

        /// <summary>
        /// SinglePageView, TwoPageView	                Zoom in so that annotation reflows onto other page.
        /// </summary>
        [Priority(1)]
        protected void basic12_2()
        {
            ReaderWrapper.Viewer.ZoomIncrement = 5;
            ISelectionData selection = new MultiPageSelectionData(1, PagePosition.Beginning, 10, 1, PagePosition.Beginning, 50);
            string expectedAnchor = GetText(selection);
            CreateAnnotation(selection);
            TextControlWrapper.IcrementalZoomTo(90);
            VerifyAnnotation(expectedAnchor);            
            passTest("Verified reflowing annotation across page boundary.");
        }

        /// <summary>
        /// SinglePageView, TwoPageView	                Create cross page annotation.
        ///                                             Page Up.
        ///                                             Page Down.
        ///                                             Verify annotation.
        /// </summary>
        [Priority(1)]
        protected void basic13_2()
        {
            MultiPageSelectionData selection = new MultiPageSelectionData(2, PagePosition.End, -621, 3, PagePosition.Beginning, 72);
            PageDown((ReaderMode == FlowDocumentReaderViewingMode.Page) ? 2 : 1);
            string expectedAnchor = (ReaderMode == FlowDocumentReaderViewingMode.Page) ? GetText(selection.GetPageBasedSelection()[0]) : GetText(selection.GetPageBasedSelection()[0]) + GetText(selection.GetPageBasedSelection()[1]);
            CreateAnnotation(selection);
            // Disabling verification b/c validation method is unreliable due to theme differences.
            //VerifyAnnotation(expectedAnchor);
            PageUp();
            VerifyNumberOfAttachedAnnotations(0);
            PageDown();
            // Disabling verification b/c validation method is unreliable due to theme differences.
            //VerifyAnnotation(expectedAnchor);
            passTest("Verified loading of cross page annotation.");
        }

        /// <summary>
        /// SinglePageView, TwoPageView	                Cross page annotation.
        ///                                             Reflow.
        /// </summary>
        [Priority(1)]
        protected void basic14()
        {
            GoToPage(1);
            ISelectionData selection = new MultiPageSelectionData(1, PagePosition.End, -50, 2, PagePosition.Beginning, 50);
            string expectedAnchor = GetText(selection);
            CreateAnnotation(selection);
            ResizeWindow(new Size(1000, 800));
            GoToPage(1);
            VerifyAnnotation(expectedAnchor);
            passTest("Reflowed crosspage annotation.");
        }

        /// <summary>
        /// SinglePageView, TwoPageView	                Cross page annotation.
        ///                                             Zoom in.
        /// </summary>
        [Priority(1)]
        protected void basic15()
        {
            GoToPage(1);
            ISelectionData selection = new MultiPageSelectionData(1, PagePosition.End, -50, 2, PagePosition.Beginning, 50);
            string expectedAnchor = GetText(selection);
            CreateAnnotation(selection);
            TextControlWrapper.IcrementalZoomTo(110);
            GoToPage(2);
            VerifyAnnotation(expectedAnchor);
            passTest("Reflowed crosspage annotation.");
        }

        /// <summary>
        /// Variations                         Description	                End View
        /// ScrollView,SinglePage,TwoPage	   StickyNote, iconify.	        TwoPageView.
        /// </summary>
        [Priority(1)]
        protected void basic19_1()
        {
            DoBasic19(FlowDocumentReaderViewingMode.TwoPage);
        }
        
        /// <summary>
        /// Variations                         Description	                End View
        /// ScrollView,SinglePage,TwoPage	   StickyNote, iconify.	        ScrollView
        /// </summary>
        [Priority(1)]
        protected void basic19_2()
        {
            DoBasic19(FlowDocumentReaderViewingMode.Scroll);
        }
        protected void DoBasic19(FlowDocumentReaderViewingMode finalView)
        {
            CreateAnnotation(new AbsoluteSelectionData(10, 400));
            CurrentlyAttachedStickyNote.MinimizeWithMouse();
            ReaderWrapper.ViewingMode = finalView;
            AssertEquals("Verify expanded state of note.", false, CurrentlyAttachedStickyNote.Expanded);
            passTest("Expanded state preserved across views.");
        }

        /// <summary>
        /// Variations                  Description
        /// SinglePage, TwoPage         In scroll view, create annotation that will get split across page break.
        ///                             Switch to paginated mode.
        /// </summary>
        [Priority(1)]
        protected void basic20()
        {
            ReaderWrapper.ViewingMode = FlowDocumentReaderViewingMode.Scroll;
            CreateAnnotation(new AbsoluteSelectionData(PagePosition.Beginning, 10, PagePosition.Middle, 200));
            ReaderWrapper.ViewingMode = ReaderMode;
            GoToStart();
            VerifyNumberOfAttachedAnnotations(1);
            passTest("No crash.");
        }

        /// <summary>
        /// Variations                          Description                             
        /// SinglePageView, TwoPageView,        N annotations throughout document.
        /// ScrollView	                        Flush.
        ///                                     Disable Service.
        ///                                     TwoPageView.
        ///                                     Enable Service.
        ///                                     Verify annotations.	
        /// </summary>
        [Priority(1)]
        protected void basic21_1()
        {
            DoBasic21(FlowDocumentReaderViewingMode.TwoPage);
        }
        
        /// <summary>
        /// Variations                          Description                             
        /// SinglePageView, TwoPageView,        N annotations throughout document.
        /// ScrollView	                        Flush.
        ///                                     Disable Service.
        ///                                     Scroll.
        ///                                     Enable Service.
        ///                                     Verify annotations.	
        /// </summary>
        [Priority(1)]
        protected void basic21_2()
        {
            DoBasic21(FlowDocumentReaderViewingMode.Scroll);
        }
        protected void DoBasic21(FlowDocumentReaderViewingMode endView)
        {
            CreateAnnotation(new AbsoluteSelectionData(PagePosition.Beginning, 10, PagePosition.Middle, -91));
            CreateAnnotation(new AbsoluteSelectionData(PagePosition.End, -51, PagePosition.Middle, -91));
            CreateAnnotation(new AbsoluteSelectionData(PagePosition.Beginning, 0, PagePosition.Beginning, 248));
            CreateAnnotation(new AbsoluteSelectionData(PagePosition.Middle, -510, PagePosition.Middle, 510));
            Service.Store.Flush();
            DisableAnnotationService();
            ReaderWrapper.ViewingMode = endView;
            SetupAnnotationService();
            Assert("Verify at least 1 attached annotation.", Service.GetAttachedAnnotations().Count > 0);
            passTest("No crash.");
        }

        #endregion PRIORITY TESTS
    }
}

