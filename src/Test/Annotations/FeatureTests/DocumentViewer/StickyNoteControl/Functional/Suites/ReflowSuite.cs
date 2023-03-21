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
    /// <summary>
    /// Suite only runs for Flow content, reflow is not possible for fixed documents.
    /// </summary>
    [OverrideClassTestDimensions()]
    public class ReflowSuite_BVT : AStickyNoteControlFunctionalSuite
    {
        #region BVT TESTS

        #region Overrides

        protected override TestMode DetermineTestMode(string[] args)
        {
            return TestMode.Flow;
        }

        #endregion

        #region Tests

        /// <summary>
        /// Note off right edge.  After reflow note still on right edge.
        /// </summary>
        [Priority(0)]
        protected void reflow1()
        {
            double widthChange = -250;
            CreateDefaultNote();
            CurrentlyAttachedStickyNote.Drag(new Vector(1000, 0));
            Point initialLocation = CurrentlyAttachedStickyNote.Location;
            ChangeWindowWidth(widthChange);
            AssertEquals("Verify position after X reflow.", Point.Add(initialLocation, new Vector(widthChange, 0)), CurrentlyAttachedStickyNote.Location, 1e-3);
            passTest("Right edge reflow verified.");
        }

        /// <summary>
        /// Note off right edge.  After reflow note still on right edge.
        /// </summary>
        [Priority(0)]
        protected void reflow2()
        {
            double delta = -250;
            CreateDefaultNote();
            CurrentlyAttachedStickyNote.Drag(new Vector(0, 1000));
            Point initialLocation = CurrentlyAttachedStickyNote.Location;
            ChangeWindowHeight(delta);
            AssertEquals("Verify position after Y reflow.", Point.Add(initialLocation, new Vector(0, delta)), CurrentlyAttachedStickyNote.Location, 1e-3);
            passTest("Bottom edge reflow verified.");
        }

        /// <summary>
        /// Note on page, reflow so that right edge pushes note to new position.
        /// </summary>
        [Priority(0)]
        protected void reflow4()
        {
            CreateDefaultNote();
            ChangeWindowWidth(-463);
            AssertEquals("Verify location of note's right edge.", PageBounds(0).Right, CurrentlyAttachedStickyNote.BoundingRect.Right, 1e-3);
            passTest("Note moved horizontally by page edge.");
        }

        /// <summary>
        /// Note on page, reflow so that bottom edge pushes note to new position.
        /// </summary>
        [Priority(0)]
        protected void reflow5()
        {
            CreateDefaultNote();
            ChangeWindowHeight(-500);
            AssertEquals("Verify location of note's bottom edge.", PageBounds(0).Top, CurrentlyAttachedStickyNote.BoundingRect.Top);
            passTest("Note moved vertically by page edge.");
        }

        #endregion

        #endregion BVT TESTS
    }
}	

