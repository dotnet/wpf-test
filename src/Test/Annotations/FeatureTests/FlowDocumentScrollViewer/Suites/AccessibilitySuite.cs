// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 

using System;
using System.Windows;
using Annotations.Test.Framework;
using System.Windows.Media;
using System.Windows.Input;

namespace Avalon.Test.Annotations.Suites
{
    public class AccessibilitySuite : AFlowDocumentScrollViewerSuite
    {
        #region BVT TESTS

        ///// <summary>
        ///// Anchor not visible. Tab to annotation, verify it is scrolled into view.
        ///// </summary>
        //[Priority(0)]
        //protected void accessibility1()
        //{
        //    SetWindowFocus();
        //    CreateAnnotation(new StickyNoteDefinition(new AbsoluteSelectionData(PagePosition.End, -200, PagePosition.End, -100), AnnotationType, true, "A"));
        //    GoToStart();
        //    VerifyNoteViewportVisibility("A", false);
        //    TestCanTabTo("A");
        //}

        ///// <summary>
        ///// Whole document annotate, start of anchor in view, note not in view.
        ///// Tab brings note into view.
        ///// </summary>
        //[Priority(0)]
        //protected void accessibility5()
        //{
        //    SetWindowFocus();
        //    CreateAnnotation(new StickyNoteDefinition(new AbsoluteSelectionData(PagePosition.Beginning, 0, PagePosition.End, 0), AnnotationType, true, "A"));
        //    CurrentlyAttachedStickyNote.Drag(new Vector(0, 2000));
        //    SetZoom(125);
        //    GoToStart();
        //    VerifyNoteViewportVisibility("A", false);
        //    TestCanTabTo("A");
        //}

        #endregion

        #region PRIORITY TESTS

        /// <summary>
        /// Anchor not visible but note is. Tab to Note.
        /// </summary>
        [Priority(1)]
        protected void accessibility2()
        {
            SetWindowFocus();
            GoToEnd();
            CreateAnnotation(new StickyNoteDefinition(new AbsoluteSelectionData(PagePosition.End, -200, PagePosition.End, -100), AnnotationType, true, "A"));
            CurrentlyAttachedStickyNote.Drag(new Vector(0, -1000));
            TextControlWrapper.ScrollUp(10);
            VerifyNoteViewportVisibility("A", true);
            TestCanTabTo("A");
        }

        /// <summary>
        /// Note partially in view. Can tab to it.
        /// </summary>
        [Priority(1)]
        protected void accessibility3()
        {
            SetWindowFocus();
            CreateAnnotation(new StickyNoteDefinition(new AbsoluteSelectionData(PagePosition.Beginning, 100), AnnotationType, true, "A"));
            CurrentlyAttachedStickyNote.Drag(new Vector(0, TextControlWrapper.ScrollViewer.ViewportHeight - 50));
            VerifyNoteViewportVisibility("A", true);
            TestCanTabTo("A");
        }

        ///// <summary>
        ///// Two notes, only one can be visible at the same time.  Verify tabbing
        ///// between causes viewport to scroll.
        ///// </summary>
        //[Priority(1)]
        //protected void accessibility4()
        //{
        //    SetWindowFocus();
        //    string [] authors = new string[] { "A", "B" };
        //    CreateAnnotation(new StickyNoteDefinition(new AbsoluteSelectionData(100, 200), AnnotationType, true, authors[0]));
        //    CreateAnnotation(new StickyNoteDefinition(new AbsoluteSelectionData(PagePosition.Middle, 100), AnnotationType, true, authors[1]));

        //    TextControlWrapper.Target.Focus();
        //    TabHelper.TabToAnnotationGroup();
        //    Assert("Verify StickyNote is focused.", TabHelper.IsCurrentElementAStickyNote);
        //    VerifyVisiblity(authors, new bool[] { true, false });
        //    TabHelper.CtrlTab();
        //    VerifyVisiblity(authors, new bool[] { false, true });
        //    TabHelper.CtrlShiftTab();
        //    VerifyVisiblity(authors, new bool[] { true, false });
        //    passTest("Verified tabbing between notes changes viewport.");
        //}

        [Priority(1)]
        private void VerifyVisiblity(string[] authors, bool[] isVisible)
        {
            printStatus("VerifyVisiblity...");
            AssertEquals("Verify authors.Length == isVisible.Length", authors.Length, isVisible.Length);
            for (int i = 0; i < authors.Length; i++)
            {
                printStatus("\t" + authors[i] + "...");
                VerifyNoteViewportVisibility(authors[i], isVisible[i]);
            }

        }

        #endregion Priority Tests
    }
}	

