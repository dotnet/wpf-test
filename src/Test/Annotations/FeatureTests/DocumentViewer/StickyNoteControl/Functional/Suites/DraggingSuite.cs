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
using System.Collections;

namespace Avalon.Test.Annotations.Suites
{
    public class DraggingSuite : ALocationSuite
    {
        #region Overrides

        protected override void DoExtendedSetup()
        {
            // Make window bigger in Fixed so that we can see more of the pages at 100% zoom.
            if (ContentMode == TestMode.Fixed) 
            {
                MainWindow.Width = 1125;
                MainWindow.Height = 964;                
            }
        }

        #endregion

        #region Tests

        /// <summary>
        /// Drag to upper left corner.
        /// Single page view.
        /// </summary>
        [Priority(0)]
        protected void dragging1_1()
        {
            MainWindow.Left = 100;
            CreateDefaultNote();
            ChangeZoom();
            CurrentlyAttachedStickyNote.Drag(new Vector(-1000, -1000));
            AssertEquals("Verify final location.", Point.Add(PageBounds(0).TopLeft, new Vector(LeftMargin - Note.ActualWidth, TopMargin)), CurrentlyAttachedStickyNote.Location, tolerance);
            passTest("Verified drag position 1.");
        }

        /// <summary>
        /// Drag to upper left corner.
        /// Multipage view.
        /// </summary>
        [Priority(0)]
        protected void dragging1_2()
        {
            MainWindow.Left = 100;
            PageLayout(2);
            if (FixedContent != null) SetZoom(100);
            CreateAnnotation(new SimpleSelectionData(1, PagePosition.Middle, 1));         
            CurrentlyAttachedStickyNote.Drag(Point.Add(PageBounds(0).TopLeft, new Vector(0, -20)));
            AssertEquals("Verify final location.", Point.Add(PageBounds(1).TopLeft, new Vector(LeftMargin - Note.ActualWidth, TopMargin)), CurrentlyAttachedStickyNote.Location, tolerance);
            passTest("Verified drag position 1.");
        }

        /// <summary>
        /// Drag to upper right corner.
        /// Single page view.
        /// </summary>
        [Priority(0)]
        protected void dragging3_1()
        {
            CreateDefaultNote();
            ChangeZoom();
            CurrentlyAttachedStickyNote.Drag(new Vector(1000, -1000));
            AssertEquals("Verify final location.", Point.Add(PageBounds(0).TopRight, new Vector(-RightMargin, TopMargin)), CurrentlyAttachedStickyNote.Location, tolerance);
            passTest("Verified drag position 3.");
        }

        /// <summary>
        /// Drag to bottom right corner.
        /// Single page view.
        /// </summary>
        [Priority(0)]
        protected void dragging5_1()
        {
            CreateAnnotation(new SimpleSelectionData(0, PagePosition.End, -15));
            ChangeZoom();
            if (ContentMode == TestMode.Fixed)
                GoToPageRange(0, 1);
            CurrentlyAttachedStickyNote.Drag(new Vector(1000, 1000));
            AssertEquals("Verify final location.", Point.Add(PageBounds(0).BottomRight, new Vector(-RightMargin, -BottomMargin)), CurrentlyAttachedStickyNote.Location, tolerance);
            passTest("Verified drag position 5.");
        }

        /// <summary>
        /// Drag to bottom left corner.
        /// Single page view.
        /// </summary>
        [Priority(0)]
        protected void dragging7_1()
        {
            CreateAnnotation(new SimpleSelectionData(0, PagePosition.End, -15));            
            ChangeZoom();
            if (ContentMode == TestMode.Fixed)
                GoToPageRange(0, 1);
            else
                MainWindow.Left = 100;
            CurrentlyAttachedStickyNote.Drag(Point.Add(PageBounds(0).BottomLeft, new Vector(-200, 200)));
            AssertEquals("Verify final location.", Point.Add(PageBounds(0).BottomLeft, new Vector(LeftMargin - Note.ActualWidth, -BottomMargin)), CurrentlyAttachedStickyNote.Location, tolerance);
            passTest("Verified drag position 7.");
        }

        #endregion

        #region Fields 

        const int LeftMargin = 45;
        const int RightMargin = 20;
        const int BottomMargin = 20;
        const int TopMargin = 0;

        double tolerance = 1;

        #endregion
    }
}	

