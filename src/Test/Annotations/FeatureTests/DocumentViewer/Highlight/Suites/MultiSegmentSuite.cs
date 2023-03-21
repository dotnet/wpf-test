// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Pri1 cases that verify add behavior for Highlight annotations.

using System;
using System.Windows;
using Annotations.Test.Framework;
using Annotations.Test;
using System.Windows.Media;

namespace Avalon.Test.Annotations.Suites
{
    public class MultiSegmentSuite : AHighlightSuite
    {
        #region BVT TESTS

        /// <summary>
        /// 1 annotation 2 segments.
        /// +-----1-----+
        /// |           |
        /// --1--   --2--
        ///         --d--
        /// 
        /// clear highlight
        /// end 1 annotation, 1 segment.
        /// 
        /// </summary>
        [Priority(0)]
        protected void multisegment1_3()
        {
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 10, 389), Colors.Green));
            DeleteAnnotation(new SimpleSelectionData(0, 20, 379));
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(0, 10, 10))
            });
            passTest("MultiSegment : [delete] 1 segment.");
        }

        /// <summary>
        /// 1 annotation 2 segments.
        /// +-----1-----+
        /// |           |
        /// --1--   --2--
        ///          -d-
        /// 
        /// end 1 annotation, 3 segment.
        /// 
        /// </summary>
        [Priority(0)]
        protected void multisegment2_1()
        {
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 200, 400), Colors.Green));
            DeleteAnnotation(new SimpleSelectionData(0, 300, 100));
            VerifyNumberOfAttachedAnnotations(1);
            DeleteAnnotation(new SimpleSelectionData(0, 450, 50));
            VerifyNumberOfAttachedAnnotations(1);
            //should now have 3 segments.
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(0, 200, 100)) + 
                GetText(new SimpleSelectionData(0, 400, 50)) + 
                GetText(new SimpleSelectionData(0, 500, 100)) 
            });
            passTest("MultiSegment : 1 annotation, 3 segments.");
        }

        /// <summary>
        /// 1 annotation 2 segments.
        /// +-----1-----+
        /// |           |
        /// --1--   --2--
        ///          -d-
        /// 
        /// end 1 annotation, 3 segment.
        /// 
        /// </summary>
        [Priority(0)]
        protected void multisegment2_2()
        {
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 200, 400), Colors.Green));
            DeleteAnnotation(new SimpleSelectionData(0, 300, 100));
            VerifyNumberOfAttachedAnnotations(1);
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 450, 100), Colors.Blue));
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(0, 200, 100)) + 
                GetText(new SimpleSelectionData(0, 400, 50)) +
                GetText(new SimpleSelectionData(0, 550, 50)) ,
                GetText(new SimpleSelectionData(0, 450, 100)) 
            });
            passTest("MultiSegment : 1 annotation, 3 segments + 1 annotation, 1 segment.");
        }

        /// <summary>
        /// 1 annotation 2 segments.
        /// +-----1-----+
        /// |           |
        /// --1--   --2--
        ///           --3--
        /// 
        /// end 2 annotations
        /// 
        /// </summary>
        [Priority(0)]
        protected void multisegment3()
        {
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 10, 600), Colors.Green));
            DeleteAnnotation(new SimpleSelectionData(0, 300, 100));
            VerifyNumberOfAttachedAnnotations(1);
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 575, 75), Colors.Green));
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(0, 10, 290)) + 
                GetText(new SimpleSelectionData(0, 400, 175)),
                GetText(new SimpleSelectionData(0, 575, 75)) 
            });
            passTest("MultiSegment : 2 annotations.");
        }

        /// <summary>
        /// 1 annotation 2 segments.
        /// +-----1-----+
        /// |           |
        /// --1--   --2--
        ///    ---d---
        /// 
        /// 1st & 2nd segment modified.
        /// 
        /// </summary>
        [Priority(0)]
        protected void multisegment7_1()
        {
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 555, 400), Colors.Green));
            DeleteAnnotation(new SimpleSelectionData(0, 710, 90));
            VerifyNumberOfAttachedAnnotations(1);
            DeleteAnnotation(new SimpleSelectionData(0, 600, 210));
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(0, 555, 45)) + 
                GetText(new SimpleSelectionData(0, 810, 145))
            });
            passTest("MultiSegment : 1 annotation, 2 modified segments.");
        }

        /// <summary>
        /// 4 annotations.
        /// 
        ///   --1--|--2--|--3--|--4--
        /// ------------new------------
        /// 
        /// 1 highlight.
        /// 
        /// </summary>
        [Priority(0)]
        protected void multisegment9()
        {
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 520, 110), Colors.Orange));
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 630, 110), Colors.Orange));
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 740, 110), Colors.Orange));
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 850, 110), Colors.Orange));
            VerifyNumberOfAttachedAnnotations(4);
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 500, 480), Colors.Yellow));
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(0, 500, 480))
            });
            passTest("MultiSegment : 1 annotation.");
        }

        /// <summary>
        /// 1 annotation with segments on different pages.
        /// 
        ///                 ---1---
        /// ----------page----------
        ///  ---2---
        /// 
        /// 1 highlight.
        /// 
        /// </summary>
        [Priority(0)]
        protected void multisegment12_1()
        {
            CreateAnnotation(new HighlightDefinition(new MultiPageSelectionData(0, PagePosition.End, -300, 1, PagePosition.Beginning, 300), Colors.Orange));
            DeleteAnnotation(new MultiPageSelectionData(0, PagePosition.End, -100, 1, PagePosition.Beginning, 100));
            GoToPage(0);
            VerifyAnnotations(new string[] {
                GetText(new MultiPageSelectionData(0, PagePosition.End, -300, 0, PagePosition.End, -100))
                });
            passTest("MultiSegment : 1 annotation, 2 segments, 2 pages.");
        }

        /// <summary>
        /// 1 annotation with segments on different pages.
        /// 
        ///                 ---1---
        /// ----------page----------
        ///  ---2---
        /// 
        /// 1 highlight.
        /// 
        /// </summary>
        [Priority(0)]
        protected void multisegment12_2()
        {
            CreateAnnotation(new HighlightDefinition(new MultiPageSelectionData(0, PagePosition.End, -300, 1, PagePosition.Beginning, 300), Colors.Orange));
            DeleteAnnotation(new MultiPageSelectionData(0, PagePosition.End, -100, 1, PagePosition.Beginning, 100));
            GoToPage(1);
            VerifyAnnotations(new string[] {
                GetText(new MultiPageSelectionData(1, PagePosition.Beginning, 100, 1, PagePosition.Beginning, 300))
                });
            passTest("MultiSegment : 1 annotation, 2 segments, 2 pages.");
        }

        /// <summary>
        /// 1 annotation with segments on different pages.
        /// 
        ///                 ---1---
        /// ----------page----------
        ///  ---2---
        /// 
        /// 1 highlight.
        /// 
        /// </summary>
        [Priority(0)]
        [Keywords("MicroSuite")]
        protected void multisegment12_4()
        {
            GoToPage(0);
            CreateAnnotation(new HighlightDefinition(new MultiPageSelectionData(0, PagePosition.End, -300, 1, PagePosition.Beginning, 300), Colors.Orange));
            DeleteAnnotation(new MultiPageSelectionData(0, PagePosition.End, -100, 1, PagePosition.Beginning, 100));
            VerifyNumberOfAttachedAnnotations(1);
            DeleteAnnotation(new MultiPageSelectionData(1, PagePosition.Beginning, 175, 1, PagePosition.Beginning, 225));
            ViewAsTwoPages();
            VerifyAnnotation(
                GetText(new MultiPageSelectionData(0, PagePosition.End, -300, 0, PagePosition.End, -100)) +
                GetText(new MultiPageSelectionData(1, PagePosition.Beginning, 100, 1, PagePosition.Beginning, 175)) +
                GetText(new MultiPageSelectionData(1, PagePosition.Beginning, 225, 1, PagePosition.Beginning, 300))
            );
            passTest("MultiSegment : 1 annotation, 3 segments, 2 pages.");
        }

        #endregion BVT TESTS

        #region PRIORITY TESTS

        /// <summary>
        /// 1 annotation 2 segments.
        /// +-----1-----+
        /// |           |
        /// --1--   --2--
        ///         -----
        /// 
        /// highlight with green.
        /// end 2 annotations, 1 segment each.
        /// 
        /// </summary>
        [Priority(1)]
        protected void multisegment1_1()
        {
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 100, 400), Colors.Yellow));
            DeleteAnnotation(new SimpleSelectionData(0, 199, 50));
            VerifyNumberOfAttachedAnnotations(1);
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 249, 500), Colors.Green));
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(0, 100, 99)),
                GetText(new SimpleSelectionData(0, 249, 500))
            });
            passTest("MultiSegment : 2 annotations, 1 segment each.");
        }

        /// <summary>
        /// 1 annotation 2 segments.
        /// +-----1-----+
        /// |           |
        /// --1--   --2--
        ///         -----
        /// 
        /// highlight with blue.
        /// end 2 annotations, 1 segment each.
        /// 
        /// </summary>
        [Priority(1)]
        protected void multisegment1_2()
        {
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 10, 389), Colors.Yellow));
            DeleteAnnotation(new SimpleSelectionData(0, 20, 79));
            VerifyNumberOfAttachedAnnotations(1);
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 99, 300), Colors.Blue));
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(0, 10, 10)),
                GetText(new SimpleSelectionData(0, 99, 300))
            });
            passTest("MultiSegment : 2 annotations, 1 segment each.");
        }

        /// <summary>
        /// 1 annotation 2 segments.
        /// +-----1-----+
        /// |           |
        /// --1--   --2--
        ///       --d--
        /// 
        /// 2nd segment modified.
        /// 
        /// </summary>
        [Priority(1)]
        protected void multisegment4()
        {
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 10, 600), Colors.Green));
            DeleteAnnotation(new SimpleSelectionData(0, 300, 100));
            VerifyNumberOfAttachedAnnotations(1);
            DeleteAnnotation(new SimpleSelectionData(0, 375, 150));
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(0, 10, 290)) + 
                GetText(new SimpleSelectionData(0, 525, 85))
            });
            passTest("MultiSegment : 1 annotation, 2nd segment modified.");
        }

        /// <summary>
        /// 1 annotation 2 segments.
        /// +-----1-----+
        /// |           |
        /// --1--   --2--
        ///   --d--   
        /// 
        /// 1st segment modified.
        /// 
        /// </summary>
        [Priority(1)]
        protected void multisegment5()
        {
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 555, 400), Colors.Green));
            DeleteAnnotation(new SimpleSelectionData(0, 710, 90));
            VerifyNumberOfAttachedAnnotations(1);
            DeleteAnnotation(new SimpleSelectionData(0, 610, 100));
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(0, 555, 55)) + 
                GetText(new SimpleSelectionData(0, 800, 155))
            });
            passTest("MultiSegment : 1 annotation, 1st segment modified.");
        }

        /// <summary>
        /// 1 annotation 2 segments.
        /// +-----1-----+
        /// |           |
        /// --1--   --2--
        /// -d-   
        /// 
        /// 1st segment modified.
        /// 
        /// </summary>
        [Priority(1)]
        protected void multisegment6()
        {
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 555, 400), Colors.Green));
            DeleteAnnotation(new SimpleSelectionData(0, 710, 90));
            VerifyNumberOfAttachedAnnotations(1);
            DeleteAnnotation(new SimpleSelectionData(0, 555, 105));
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(0, 660, 50)) + 
                GetText(new SimpleSelectionData(0, 800, 155))
            });
            passTest("MultiSegment : 1 annotation, 1st segment modified.");
        }

        /// <summary>
        /// 1 annotation 2 segments.
        /// +-----1-----+
        /// |           |
        /// --1--   --2--
        ///    ---3---
        /// 
        /// New Highlight
        /// 1st & 2nd segment modified.
        /// 
        /// end 1 annotation with 2 segements, and 1 annotation with 1 segment.
        /// 
        /// </summary>
        [Priority(1)]
        protected void multisegment7_2()
        {
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 555, 400), Colors.Green));
            DeleteAnnotation(new SimpleSelectionData(0, 710, 90));
            VerifyNumberOfAttachedAnnotations(1);
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 600, 210), Colors.Green));
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(0, 555, 45)) + 
                GetText(new SimpleSelectionData(0, 810, 145)),
                GetText(new SimpleSelectionData(0, 600, 210))
            });
            passTest("MultiSegment : 1 annotation, 2 modified segments.");
        }

        /// <summary>
        /// 1 annotation 3 segments.
        /// +---------1---------+
        /// |                   |
        /// --1--   --2--   --3--
        ///       ----d----
        /// 
        /// 2nd segment cleared.
        /// 
        /// </summary>
        [Priority(1)]
        protected void multisegment8_1()
        {
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 100, 900), Colors.Green));
            DeleteAnnotation(new SimpleSelectionData(0, 350, 100));
            DeleteAnnotation(new SimpleSelectionData(0, 550, 100));
            VerifyNumberOfAttachedAnnotations(1);
            DeleteAnnotation(new SimpleSelectionData(0, 425, 150));
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(0, 100, 250)) + 
                GetText(new SimpleSelectionData(0, 650, 350))
            });
            passTest("MultiSegment : 1 annotation, 3 segments. 1 cleared.");
        }

        /// <summary>
        /// 1 annotation 3 segments.
        /// +---------1---------+
        /// |                   |
        /// --1--   --2--   --3--
        ///       ----2----
        /// 
        /// new annotation
        /// 
        /// end 1 annotation has 2 segments, the other 1 segment
        /// 
        /// </summary>
        [Priority(1)]
        protected void multisegment8_2()
        {
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 100, 900), Colors.Green));
            DeleteAnnotation(new SimpleSelectionData(0, 350, 100));
            DeleteAnnotation(new SimpleSelectionData(0, 550, 100));
            VerifyNumberOfAttachedAnnotations(1);
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 425, 150), Colors.Blue));
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(0, 100, 250)) + 
                GetText(new SimpleSelectionData(0, 650, 350)),
                GetText(new SimpleSelectionData(0, 425, 150))
            });
            passTest("MultiSegment : 1 annotation, 3 segments. 1 cleared.");
        }

        /// <summary>
        /// 1 annotation 3 segments.
        /// +---------1---------+
        /// |                   |
        /// --1--   --2--   --3--
        /// ----------2----------
        /// 
        /// 1 new annotation
        /// 
        /// </summary>
        [Priority(1)]
        protected void multisegment8_3()
        {
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 100, 900), Colors.Green));
            DeleteAnnotation(new SimpleSelectionData(0, 350, 100));
            DeleteAnnotation(new SimpleSelectionData(0, 550, 100));
            VerifyNumberOfAttachedAnnotations(1);
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 50, 1000), Colors.Green));
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(0, 50, 1000))
            });
            passTest("MultiSegment : 1 annotation.");
        }

        /// <summary>
        /// 4 annotations.
        /// 
        /// --1--|--2--|--3--|--4--
        /// ----------new----------
        /// 
        /// 1 highlight.
        /// 
        /// </summary>
        [Priority(1)]
        protected void multisegment10()
        {
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 520, 110), Colors.Orange));
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 630, 110), Colors.Orange));
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 740, 110), Colors.Orange));
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 850, 110), Colors.Orange));
            VerifyNumberOfAttachedAnnotations(4);
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 520, 440), Colors.Yellow));
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(0, 520, 440))
            });
            passTest("MultiSegment : 1 annotation.");
        }

        /// <summary>
        /// 4 annotations.
        /// 
        /// --1--  --2--  --3--  --4--
        /// ------------new------------
        /// 
        /// 1 highlight.
        /// 
        /// </summary>
        [Priority(1)]
        protected void multisegment11()
        {
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 520, 110), Colors.Orange));
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 650, 110), Colors.Orange));
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 780, 110), Colors.Orange));
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 910, 110), Colors.Orange));
            VerifyNumberOfAttachedAnnotations(4);
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 510, 520), Colors.Yellow));
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(0, 510, 520))
            });
            passTest("MultiSegment : 1 annotation.");
        }

        /// <summary>
        /// 1 annotation with segments on different pages.
        /// 
        ///                 ---1---
        /// ----------page----------
        ///  ---2---
        /// 
        /// 1 highlight.
        /// 
        /// </summary>
        [Priority(1)]
        protected void multisegment12_3()
        {
            CreateAnnotation(new HighlightDefinition(new MultiPageSelectionData(1, PagePosition.End, -300, 2, PagePosition.Beginning, 300), Colors.Orange));
            DeleteAnnotation(new MultiPageSelectionData(1, PagePosition.End, -100, 2, PagePosition.Beginning, 100));
            ViewAsTwoPages();
            GoToPageRange(1, 2);
            VerifyAnnotations(new string[] {
                GetText(new MultiPageSelectionData(1, PagePosition.End, -300, 1, PagePosition.End, -100)) +
                GetText(new MultiPageSelectionData(2, PagePosition.Beginning, 100, 2, PagePosition.Beginning, 300))
                });
            passTest("MultiSegment : 1 annotation, 2 segments, 2 pages.");
        }

        /// <summary>
        /// 1 annotation with segments on 3 different pages.
        /// 
        ///                 ---1---
        /// ----------page----------
        /// ----------page----------
        /// ----------page----------
        /// ---2---
        /// 
        /// 1 highlight.
        /// 
        /// </summary>
        [Priority(1)]
        protected void multisegment13()
        {
            CreateAnnotation(new HighlightDefinition(new MultiPageSelectionData(0, PagePosition.End, -1000, 2, PagePosition.Beginning, 1000), Colors.Orange));
            VerifyNumberOfAttachedAnnotations(1);
            DeleteAnnotation(new MultiPageSelectionData(1, PagePosition.Beginning, 0, 1, PagePosition.End, 0));            
            string expectedText = 
                GetText(new MultiPageSelectionData(0, PagePosition.End, -1000, 0, PagePosition.End, 0)) +
                GetText(new MultiPageSelectionData(2, PagePosition.Beginning, 0, 2, PagePosition.Beginning, 1000));
            SetWindowWidth(1011);
            SetWindowHeight(776);
            PageLayout(6);
            SetZoom(50);
            VerifyNumberOfAttachedAnnotations(1);
            VerifyAnnotation(expectedText);
            passTest("MultiSegment : 1 annotation, 2 segments, 3 pages.");
        }

        /// <summary>
        /// 1 annotations, 3 segments.
        /// 
        /// --1--     --2--     --3--
        ///       -1-       -2-
        /// ------------1------------
        /// 
        /// fill splits with 2 new annotations
        /// 
        /// end 1 highlight
        /// 
        /// </summary>
        [Priority(1)]
        protected void multisegment14_1()
        {
            GoToPage(1);
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(1, 153, 1531), Colors.Red));
            DeleteAnnotation(new SimpleSelectionData(1, 306, 351));
            DeleteAnnotation(new SimpleSelectionData(1, 1027, 351));
            VerifyNumberOfAttachedAnnotations(1);
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(1, 405, 153), Colors.Green));
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(1, 711, 153), Colors.Green));
            VerifyNumberOfAttachedAnnotations(3);
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(1, 10, 2100), Colors.Red));
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(1, 10, 2100))
            });
            passTest("MultiSegment : 1 annotation.");
        }

        /// <summary>
        /// 1 annotations, 3 segments.
        /// 
        /// -1-    -2-   -3-
        ///    -1-    -2-
        /// 
        /// fill splits with 2 new annotations
        /// 
        /// </summary>
        
        [Priority(1)]
        protected void multisegment14_2()
        {
	        GoToPage(2);
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 500, 1500), Colors.Red));
            DeleteAnnotation(new SimpleSelectionData(2, 600, 100));
            DeleteAnnotation(new SimpleSelectionData(2, 1300, 100));
            VerifyNumberOfAttachedAnnotations(1);
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 600, 100), Colors.Green));
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 1300, 100), Colors.Green));
            VerifyNumberOfAttachedAnnotations(3);
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 100, 2500), Colors.Green));
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(2, 100, 2500))
            });
            passTest("MultiSegment : 1 annotation.");
        }

        /// <summary>
        /// 1 annotations, 3 segments.
        /// 
        /// -1-         -1-
        ///    -2-   -2-
        ///       -3-
        /// fill splits with 2 new annotations
        /// 
        /// </summary>
        [Priority(1)]
        protected void multisegment15()
        {
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 101, 500), Colors.Red));
            DeleteAnnotation(new SimpleSelectionData(0, 201, 300));
            VerifyNumberOfAttachedAnnotations(1);
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 201, 300), Colors.Green));
            VerifyNumberOfAttachedAnnotations(2);
            DeleteAnnotation(new SimpleSelectionData(0, 301, 100));
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 301, 100), Colors.Blue));
            VerifyNumberOfAttachedAnnotations(3);
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 50, 701), Colors.Blue));
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(0, 50, 701))
            });
            passTest("MultiSegment : 1 annotation.");
        }

        #endregion PRIORITY TESTS
    }
}

