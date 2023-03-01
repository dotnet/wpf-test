// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Verify deleting annotations in paginated content.

using System;
using System.Windows;
using Annotations.Test.Framework;

namespace Avalon.Test.Annotations.Pri1s
{
    [OverrideClassTestDimensions]
    [TestDimension("fixed,flow")]
    [TestDimension("stickynote")]
    public class Scenario1_5Suite : AScenario1_5Suite
    {

        #region Test Cases

        protected void scenario1_5_1()
        {
            RunScenario(new SimpleSelectionData(0, PagePosition.Beginning, 291));
            passTest("Delete at start of document.");
        }
        protected void scenario1_5_2()
        {
            GoToLastPage();
            RunScenario(new SimpleSelectionData(4, PagePosition.End, -900));
            passTest("Deleted at end of document.");
        }
        protected void scenario1_5_14()
        {
            EnsureStickyNote();
            ISelectionData anchor = new SimpleSelectionData(0, PagePosition.End, -629);
            RunScenario(anchor, anchor, anchor);
            passTest("Deleted overlapping annotations with one call.");
        }
        protected void scenario1_5_15()
        {
            EnsureStickyNote();
            RunScenario(
                new SimpleSelectionData(0, 600, 100),
                new SimpleSelectionData(0, 500, 200),
                new MultiPageSelectionData(0, PagePosition.Beginning, 0, 0, PagePosition.End, 0));
            passTest("Deleted overlapping annotations with one call.");
        }
        protected void scenario1_5_16()
        {
            EnsureStickyNote();
            RunScenario(
                new SimpleSelectionData(0, 500, 100),
                new SimpleSelectionData(0, 550, 100),
                new MultiPageSelectionData(0, PagePosition.Beginning, 0, 0, PagePosition.End, 0));
            passTest("Deleted partially overlapping annotations with one call.");
        }
        protected void scenario1_5_18b()
        {
            RunScenario(
                new MultiPageSelectionData(0, PagePosition.End, -41, 1, PagePosition.Beginning, 91),
                new int[] { 0, 1 },
                new int[] { 1, 2 },
                new int[] { 0, 1 }
            );
            passTest("Deleted non-visible annotation spanning partially non-visible page break.");
        }

        #endregion
    }
}

