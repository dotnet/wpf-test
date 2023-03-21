// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Verify deleting annotations in paginated content.

using System;
using System.Windows;
using Annotations.Test.Framework;

namespace Avalon.Test.Annotations.BVTs
{
    public class Scenario1_5Suite_BVT : AScenario1_5Suite
    {
        #region Test Cases

        protected void scenario1_5_3()
        {
            GoToPage(3);
            RunScenario(new SimpleSelectionData(3, PagePosition.Beginning, 385));
            passTest("Deleted at start of non-edge page.");
        }
        protected void scenario1_5_4()
        {
            GoToPage(2);
            RunScenario(new SimpleSelectionData(2, PagePosition.End, -38));
            passTest("Deleted at end of non-edge page.");
        }
        protected void scenario1_5_5()
        {
            GoToPage(1);
            RunScenario(new SimpleSelectionData(1, PagePosition.Middle, 291));
            passTest("Deleted at middle of non-edge page.");
        }
        protected void scenario1_5_6()
        {
            GoToPageRange(0, 1);
            RunScenario(new MultiPageSelectionData(0, PagePosition.End, -341, 1, PagePosition.Beginning, 934));
            passTest("Deleted spanning 1 page break.");
        }
        protected void scenario1_5_7()
        {
            RunScenario(new MultiPageSelectionData(0, PagePosition.End, -341, 4, PagePosition.Beginning, 934));
            for (int i = 1; i < 5; i++)
            {
                GoToPage(i);
                VerifyNumberOfAttachedAnnotations(0);
            }
            passTest("Deleted spanning N page break.");
        }
        protected void scenario1_5_12()
        {
            RunScenario(
                new SimpleSelectionData(0, 10, 393), 
                new SimpleSelectionData(0, 901, -91), 
                new MultiPageSelectionData(0, PagePosition.Beginning, 0, 0, PagePosition.End, 0));
            passTest("Deleted multiple annotations with one call.");
        }
        protected void scenario1_5_13()
        {
            GoToPageRange(2, 3);
            RunScenario(
                new SimpleSelectionData(2, PagePosition.Middle, 19),
                new SimpleSelectionData(3, PagePosition.Middle, -45),          
                new MultiPageSelectionData(2, PagePosition.Beginning, 0, 3, PagePosition.End, 0));
            passTest("Deleted multiple annotations across pages with one call.");
        }      
        protected void scenario1_5_17()
        {
            ISelectionData selection = new SimpleSelectionData(4, 500, 100);
            GoToPage(4);
            CreateAnnotation(selection);
            VerifyNumberOfAttachedAnnotations(1);
            GoToPage(0);
            DeleteAnnotation(selection);
            GoToPage(4);
            VerifyNumberOfAttachedAnnotations(0);
            passTest("Deleted non-visible annotation on 1 page.");
        }
        protected void scenario1_5_18a()
        {
            RunScenario(
                new MultiPageSelectionData(3, PagePosition.End, -41, 4, PagePosition.Beginning, 91), 
                new int[] { 3, 4 },
                new int[] { 2, 3 },
                new int[] { 3, 4 }
            );
            passTest("Deleted non-visible annotation spanning partially non-visible page break.");
        }
        protected void scenario1_5_18c()
        {
            RunScenario(
                new MultiPageSelectionData(0, PagePosition.End, -41, 1, PagePosition.Beginning, 91),
                new int[] { 0, 1 },
                new int[] { 3, 4 },
                new int[] { 0, 1 }
            );
            passTest("Deleted non-visible annotation spanning non-visible page break.");
        }

        #endregion
    }
}

