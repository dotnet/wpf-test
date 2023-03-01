// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 



using System;
using System.Windows;
using Annotations.Test.Framework;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Documents;
using System.Reflection;
using System.Windows.Controls.Primitives;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Avalon.Test.Annotations.Suites
{
    public class FixedSuite : AStickyNoteAccessibilitySuite
    {
        protected override TestMode DetermineTestMode(string[] args)
        {
            return TestMode.Fixed;
        }

        /// <summary>
        /// Multiple Notes on current page.
        /// Viewport is smaller than page and does not include any notes.
        /// Verify that we can tab to each note and that each not is brought into view when it is 
        /// tabbed to.
        /// </summary>
        [Priority(0)]
        protected void fixed1()
        {
            MainWindow.PreviewMouseRightButtonDown += DumpStats;
            PrintViewPortStats();
            SetZoom(75);
            PrintViewPortStats();
            Rect pageBounds = PageBounds(0);
            CreateStickyNote(new SimpleSelectionData(0, 1, 10), "A");
            CreateStickyNote(new SimpleSelectionData(0, 150, 10), "B");
            CreateStickyNote(new SimpleSelectionData(0, 250, 10), "C");
            GetStickyNoteWithAuthor("C").Drag(pageBounds.BottomRight);
            GetStickyNoteWithAuthor("B").Drag(new Vector(-pageBounds.Width, 0));
            GetStickyNoteWithAuthor("A").Drag(new Vector(pageBounds.Width, 50));
            SetZoom(200);

            TextControl.Focus();
            PrintViewPortStats();
            TabHelper.TabToAnnotationGroup();
            VerifyNoteViewportVisibility("A", true);
            TabHelper.MoveToNextAnnotation();
            VerifyNoteViewportVisibility("B", true);
            TabHelper.MoveToNextAnnotation();
            VerifyNoteViewportVisibility("C", true);
            passTest("Verified notes brought into view during tabbing.");
        }   

        [TestCase_Helper()]
        void DumpStats(object sender, MouseEventArgs e)
        {
            PrintViewPortStats();
            VerifyNoteViewportVisibility("A", true);
            VerifyNoteViewportVisibility("B", true);
            VerifyNoteViewportVisibility("C", true);
        }

        [TestCase_Helper()]
        void PrintViewPortStats()        
        {
            ScrollViewer scrollViewer = DocViewerWrapper.ScrollViewer;
            printStatus("Viewport: Offset=(" + scrollViewer.HorizontalOffset + ", " + scrollViewer.VerticalOffset + ") Size=(" + scrollViewer.ViewportWidth + ", " + scrollViewer.ViewportHeight + ")");
        }
    }    
}	

