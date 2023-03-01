// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 


using System;
using System.Windows;
using Annotations.Test.Framework;
using System.Windows.Media;

namespace Avalon.Test.Annotations.Suites
{
    public class HighlightSuite : AStickyNoteAccessibilitySuite
    {
        #region BVT TESTS

        /// <summary>
        /// Variations	            Initial State	                Action.
        ///                         1 Note and a highlight.	        Can only tab to Note.
        /// </summary>
        [Priority(0)]
        protected void highlight1()
        {
            CreateDefaultNote();
            TabHelper.TabToAnnotationGroup();
            TabHelper.MoveToNextAnnotation();
            IInputElement tabStopAfterNote = TabHelper.CurrentlyFocusedElement;

            // Create highlight that starts after the note.
            new HighlightDefinition(new SimpleSelectionData(DEFAULT_PAGE, DEFAULT_OFFSET + DEFAULT_LENGTH + 10, 100), Colors.Red).Create(DocViewerWrapper);

            TabHelper.TabToAnnotationGroup();
            Assert("Verify stickynote is focused.", TabHelper.IsCurrentElementAStickyNote);
            TabHelper.MoveToNextAnnotation();
            Assert("Verify highlight did not affect tab order.", tabStopAfterNote == TabHelper.CurrentlyFocusedElement);
            passTest("Highlight ignored during tabbing.");
        }

        #endregion BVT TESTS

        #region PRIORITY TESTS

        /// <summary>
        /// Variations      Initial State                               Action
        ///                 Only highlights are visible.	            No items in Annotations TabGroup.
        /// </summary>
        [Priority(1)]
        protected void highlight2()
        {
            new HighlightDefinition(new SimpleSelectionData(0, 9, 25), Colors.Red).Create(DocViewerWrapper);
            new HighlightDefinition(new SimpleSelectionData(0, PagePosition.Middle, -241), Colors.Green).Create(DocViewerWrapper);
            new HighlightDefinition(new SimpleSelectionData(0, PagePosition.End, -67), Colors.Blue).Create(DocViewerWrapper);
            VerifyNumberOfAttachedAnnotations(3);
            ViewerBase.Focus();
            TabHelper.TabToAnnotationGroup();
            Assert("Verify that annotation is not focused.", !TabHelper.IsCurrentElementAStickyNote);
            passTest("Cannot tab to highlights.");
        }

        #endregion PRIORITY TESTS
    }
}	

