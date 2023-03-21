// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Tests for CAF Commands.
using System;
using System.Windows;
using Annotations.Test.Framework;
using System.Windows.Annotations;
using System.Windows.Media;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Avalon.Test.Annotations.Suites
{
    public class CommandSuite : ACommandSuite
    {
        #region BVT TESTS

        #region CreateHighlightCommand

        /// <summary>
        /// Parameters: None	
        /// Verify: Highlight created on current selection with default color.
        /// </summary>
        [Keywords("Setup_SanitySuite")]
        [Priority(0)]
        public void highlight_create1()
        {
            ISelectionData selection = new SimpleSelectionData(0, 910, -42);
            SetSelection(selection);
            DoSyncCommand(AnnotationService.CreateHighlightCommand, ViewerBase);
            VerifyAnnotation(GetText(selection));
            passTest("Verified CreateHighlightCommand w/o parameter.");
        }

        /// <summary>
        /// Parameters: Colors.Red	
        /// Verify: Red Highlight created on current selection.
        /// </summary>
        [Priority(0)]
        public void highlight_create2()
        {
            ISelectionData selection = new SimpleSelectionData(0, 910, -42);
            SetSelection(selection);
            DoSyncCommand(AnnotationService.CreateHighlightCommand, ViewerBase, Brushes.Red);
            VerifyAnnotation(GetText(selection));
            AssertEquals("Verify brush color", Colors.Red, ((HighlightStateInfo)GetCurrentState()[0]).HighlightBrushColor);
            passTest("Verified CreateHighlightCommand w/ color parameter.");
        }

        #endregion

        #region CreatStickyNoteXXXCommand

        /// <summary>
        /// Parameters: None	
        /// Verify: SN created on current selection.
        /// </summary>
        [Priority(0)]
        protected void sn_create1_text() { sn_create1(true); }
        [Priority(0)]
        protected void sn_create1_ink() { sn_create1(false); }
        protected void sn_create1(bool text)
        {
            ISelectionData selection = new SimpleSelectionData(0, 100, 900);
            SetSelection(selection);
            DoSyncCommand((text) ? AnnotationService.CreateTextStickyNoteCommand : AnnotationService.CreateInkStickyNoteCommand, ViewerBase);
            VerifyAnnotation(GetText(selection));
            passTest("Verified CreateSN command.");
        }

        #endregion

        #region ClearHighlightsCommand

        /// <summary>
        /// Parameters: None, 
        ///				Highlight is selected	
        /// Verify: Selected highlight is deleted
        /// </summary>
        [Priority(0)]
        protected void highlight_clear1()
        {
            ISelectionData selection = new SimpleSelectionData(0, 910, -42);
            new HighlightDefinition(selection, Colors.Yellow).Create(DocViewerWrapper);
            VerifyNumberOfAttachedAnnotations(1);
            SetSelection(selection);
            DoSyncCommand(AnnotationService.ClearHighlightsCommand, ViewerBase);
            VerifyNumberOfAttachedAnnotations(0);
            passTest("Verified ClearHighlightCommand.");
        }

        #endregion

        #region DeleteStickyNotesCommand

        /// <summary>
        /// Parameters: None,
        ///				Portion of text SN is selected.	
        /// Verify: SN deleted.
        /// </summary>
        [Priority(0)]
        protected void sn_delete1_text() { sn_delete1(true); }
        [Priority(0)]
        protected void sn_delete1_ink() { sn_delete1(false); }
        protected void sn_delete1(bool text)
        {
            ISelectionData selection = new SimpleSelectionData(0, 910, -42);
            new StickyNoteDefinition(selection, AnnotationMode.StickyNote).Create(DocViewerWrapper);
            VerifyNumberOfAttachedAnnotations(1);
            SetSelection(selection);
            DoSyncCommand(AnnotationService.DeleteStickyNotesCommand, ViewerBase);
            VerifyNumberOfAttachedAnnotations(0);
            passTest("Verified DeleteStickyNotesCommand.");
        }

        #endregion

        #region DeleteAnnotationsCommand

        /// <summary>
        /// Parameters: None, 
        ///				selected portion of highlight, text, and ink SNs.	
        /// Verify: Both SN�s deleted, selected portion of highlight cleared.
        /// </summary>
        [Priority(0)]
        protected void annot_delete1()
        {
            ISelectionData selection = new SimpleSelectionData(0, 250, 500);
            new HighlightDefinition(selection, Colors.Beige).Create(DocViewerWrapper);
            new StickyNoteDefinition(selection, AnnotationMode.StickyNote).Create(DocViewerWrapper);
            new StickyNoteDefinition(selection, AnnotationMode.InkStickyNote).Create(DocViewerWrapper);
            VerifyNumberOfAttachedAnnotations(3);
            SetSelection(selection);
            DoSyncCommand(AnnotationService.DeleteAnnotationsCommand, ViewerBase);
            VerifyNumberOfAttachedAnnotations(0);
            passTest("Verified DeleteAnnotationsCommand.");
        }

        #endregion

        #endregion BVT TESTS

        #region PRIOROTY TESTS

        #region InkCommand

        [Priority(1)]
        protected void ink1()
        {
            TestInkCommand(InkCanvasEditingMode.EraseByPoint);
        }

        [Priority(1)]
        protected void ink2()
        {
            TestInkCommand(InkCanvasEditingMode.EraseByStroke);            
        }

        [Priority(1)]
        protected void ink3()
        {
            TestInkCommand(InkCanvasEditingMode.GestureOnly);
        }

        [Priority(1)]
        protected void ink4()
        {
            TestInkCommand(InkCanvasEditingMode.Ink);
        }

        [Priority(1)]
        protected void ink5()
        {
            TestInkCommand(InkCanvasEditingMode.InkAndGesture);
        }

        [Priority(1)]
        protected void ink6()
        {
            TestInkCommand(InkCanvasEditingMode.None);
        }

        [Priority(1)]
        protected void ink7()
        {
            TestInkCommand(InkCanvasEditingMode.Select);
        }

        private void TestInkCommand(InkCanvasEditingMode mode)
        {
            new StickyNoteDefinition(new SimpleSelectionData(0, 100, 100), AnnotationMode.InkStickyNote).Create(DocViewerWrapper);
            StickyNoteWrapper wrapper = CurrentlyAttachedStickyNote;
            DoSyncCommand(StickyNoteControl.InkCommand, wrapper.Target, mode);
            AssertEquals("Verify editing mode is correct.", mode, wrapper.InkCanvas.ActiveEditingMode);
            passTest("Verified ink command.");
        }

        /// <summary>
        /// Verify that exception is thrown if Brush is not of type SolidColorBrush.
        /// </summary>
        [Priority(1)]
        private void highlight1()
        {
            try
            {
                SetSelection(new SimpleSelectionData(0, 1, 1)); // Must have a selection for command to be executed.
                DoSyncCommand(AnnotationService.CreateHighlightCommand, ViewerBase, new VisualBrush());
            }
            catch (ArgumentException e)
            {
                printStatus("Expected exception - " + e.Message);
                passTest("Verified exception for non-SolidColorBrush");
            }
            failTest("Expected exception for non-SolidColorBrush in V1.");
        }

        /// <summary>
        /// Verify that exception is thrown if Brush is not of type SolidColorBrush.
        /// </summary>
        [Priority(1)]
        private void highlight2()
        {
            try
            {
                SetSelection(new SimpleSelectionData(0, 1, 1)); // Must have a selection for command to be executed.
                AnnotationHelper.CreateHighlightForSelection((AnnotationService)Service.Delegate, "foo", new VisualBrush());
            }
            catch (ArgumentException e)
            {
                printStatus("Expected exception - " + e.Message);
                passTest("Verified exception for non-SolidColorBrush");
            }
            failTest("Expected exception for non-SolidColorBrush in V1.");
        }        

        #endregion

        #endregion PRIOROTY TESTS
    }
}	

