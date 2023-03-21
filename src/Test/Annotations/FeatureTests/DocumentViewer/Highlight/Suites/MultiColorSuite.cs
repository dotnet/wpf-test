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
    public class MultiColorSuite : AHighlightSuite
    {
        #region PRIORITY TESTS

        /// <summary>
        /// three highlight.
        /// ---1---
        ///             ---2---
        ///     -----3------
        /// merge 2 and 3 and truncate 1
        /// --1-|-----3----2----
        /// 
        /// create 2 highlight sections.  merge the third with your second highlight and truncate your first highlight with it.
        /// 
        /// </summary>
        [Priority(1)]
        protected void multicolor6_3()
        {
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 100, 100), Colors.Red));
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 250, 100), Colors.Blue));
            VerifyNumberOfAttachedAnnotations(2);
            if (NonVisibleMode) GoToPage(2);
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 120, 200), Colors.Blue));
            if (NonVisibleMode) GoToPage(0);
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(0, 100, 20)),
                GetText(new SimpleSelectionData(0, 320, 30)),
                GetText(new SimpleSelectionData(0, 120, 200))
            });
            passTest("Highlight : merged [2][3] and truncated [1].");
        }

        /// <summary>
        /// three highlights.
        /// ---1---
        ///             ---2---
        ///     -----3------
        /// 
        /// merge 1 and 3 and truncate 2
        /// -----1----3----|-2-
        /// 
        /// create 2 highlight sections.   merge the third highlight with your first highlight and truncate the second highlight with it.
        /// 
        /// </summary>
        [Priority(1)]
        protected void multicolor6_5()
        {
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 87, 44), Colors.Yellow));
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 187, 213), Colors.Blue));
            VerifyNumberOfAttachedAnnotations(2);
            if (NonVisibleMode) GoToPage(2);
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 97, 256), Colors.Yellow));
            if (NonVisibleMode) GoToPage(0);
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(0, 87, 10)),
                GetText(new SimpleSelectionData(0, 97, 256)),
                GetText(new SimpleSelectionData(0, 353, 47))
            });
            passTest("Highlight : merged [1][3] and truncated [2].");
        }

        /// <summary>
        /// three highlights.
        /// --1--
        ///           --2--
        ///      --3--
        /// 
        /// merge 2 and 3
        /// --1--|--3----2--
        /// 
        /// create two highlight sections.  merge the third highlight with the second highlight.
        /// 
        /// </summary>
        [Priority(1)]
        protected void multicolor6_8()
        {
            GoToPage(2);
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 11, 81), Colors.Orange));
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 140, 99), Colors.RoyalBlue));
            VerifyNumberOfAttachedAnnotations(2);
            if (NonVisibleMode) GoToPage(0);
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 92, 55), Colors.RoyalBlue));
            if (NonVisibleMode) GoToPage(2);
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(2, 11, 81)),
                GetText(new SimpleSelectionData(2, 147, 92)),
                GetText(new SimpleSelectionData(2, 92, 55))
            });
            passTest("Highlight : merged [2][3]");
        }

        /// <summary>
        /// three highlights.
        /// --1--
        ///           --2--
        ///      --3--
        /// 
        /// merge 1 and 3
        /// --1----3--|--2--
        /// 
        /// create two highlight sections.  merge the third highlight with the first highlight.
        /// 
        /// </summary>
        [Priority(1)]
        protected void multicolor6_9()
        {
            GoToPage(2);

            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 799, -101), Colors.Pink));
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 1201, 399), Colors.Purple));
            VerifyNumberOfAttachedAnnotations(2);
            if (NonVisibleMode) GoToPage(0);
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 1200, -431), Colors.Pink));
            if (NonVisibleMode) GoToPage(2);
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(2, 698, 71)),
                GetText(new SimpleSelectionData(2, 769, 431)),
                GetText(new SimpleSelectionData(2, 1201, 399))
            });
            passTest("Highlight : merged [1][3]");
        }

        /// <summary>
        /// three highlights.
        /// --1--
        ///           --2--
        /// ----3-----
        /// 
        /// merge 2 and 3 and replace 1
        /// ---3------2----
        /// 
        /// create two highlight sections.  merge the third highlight with the second highlight and replace the first.
        /// 
        /// </summary>
        [Priority(1)]
        protected void multicolor9_1()
        {
            GoToPage(1);

            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(1, 444, 77), Colors.SpringGreen));
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(1, 909, 606), Colors.HotPink));
            VerifyNumberOfAttachedAnnotations(2);
            if (NonVisibleMode) GoToPage(0);
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(1, 909, -500), Colors.HotPink));
            if (NonVisibleMode) GoToPage(1);
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(1, 409, 500)),
                GetText(new SimpleSelectionData(1, 909, 606)),
            });
            passTest("Highlight : merged [2][3] and replaced [1]");
        }

        /// <summary>
        /// three highlights.
        /// --1--
        ///           --2--
        ///    ----3----
        /// 
        /// extend 2 and truncate 1
        /// -1-|---3----2----
        /// 
        /// create two highlight sections.  merge the third highlight with the second,extending the second and truncating the first.
        ///  
        /// </summary>
        
        [Priority(1)]
        protected void multicolor9_3()
        {
            GoToPage(2);

            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 1501, 633), Colors.IndianRed));
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 2999, 26), Colors.RoyalBlue));
            VerifyNumberOfAttachedAnnotations(2);
            if (NonVisibleMode) GoToPage(0);
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 3101, -1500), Colors.RoyalBlue));
            if (NonVisibleMode) GoToPage(2);
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(2, 1501, 100)),
                GetText(new SimpleSelectionData(2, 1601, 1500))
            });
            passTest("Highlight : merged [2][3], extendings [2] and truncated [1]");
        }

        /// <summary>
        /// three highlights.
        /// --1--
        ///           --2--
        /// -------3--------
        /// 
        /// merge 2 and 3 and replace 1
        /// ------3-----2---
        /// 
        /// create two highlight sections.  merge the third highlight with the second highlight and replace the first.
        /// 
        /// </summary>
        
        [Priority(1)]
        protected void multicolor9_5()
        {
            GoToPage(2);

            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 9, 777), Colors.IndianRed));
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 2009, 801), Colors.RoyalBlue));
            VerifyNumberOfAttachedAnnotations(2);
            if (NonVisibleMode) GoToPage(0);
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 9, 2801), Colors.RoyalBlue));
            if (NonVisibleMode) GoToPage(2);
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(2, 9, 2801))
            });
            passTest("Highlight : merged [2][3], replaced [1]");
        }

        /// <summary>
        /// two highlights.
        /// --1--      --2--
        ///       |   | 
        ///       [del]
        /// 
        /// no change.
        /// --1--       --2--
        /// 
        /// create two highlight sections.  delete section in between, no change should occur.
        /// 
        /// </summary>
        [Priority(1)]
        protected void multicolor11_1()
        {
            GoToPage(2);

            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 57, 600), Colors.IndianRed));
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 1057, 300), Colors.RoyalBlue));
            VerifyNumberOfAttachedAnnotations(2);
            if (NonVisibleMode) GoToPage(0);
            DeleteAnnotation(new SimpleSelectionData(2, 657, 400));
            if (NonVisibleMode) GoToPage(2);
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(2, 57, 600)),
                GetText(new SimpleSelectionData(2, 1057, 300))
            });
            passTest("Delete Highlight : delete selection with no change in current highlights");
        }

        /// <summary>
        /// two highlights.
        /// ---1--- ---2---
        ///        |   |   
        ///        [del]
        /// 
        /// 2 truncated.
        /// ---1---     -2-
        /// 
        /// create two highlight sections.  delete section truncating highlight 2 only.
        /// 
        /// </summary>
        [Priority(1)]
        protected void multicolor11_3()
        {
            GoToPage(2);

            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 157, -100), Colors.IndianRed));
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 257, 100), Colors.RoyalBlue));
            VerifyNumberOfAttachedAnnotations(2);
            if (NonVisibleMode) GoToPage(0);
            DeleteAnnotation(new SimpleSelectionData(2, 157, 150));
            if (NonVisibleMode) GoToPage(2);
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(2, 57, 100)),
                GetText(new SimpleSelectionData(2, 307, 50))
            });
            passTest("Delete Highlight : deleted selection that truncates highlight [2]");
        }

        /// <summary>
        /// two highlights.
        /// ---1--- ---2---
        /// |      |   
        /// [ del  ]
        /// 
        /// 1 deleted.
        ///         ---2---
        /// 
        /// create two highlight sections.  delete section that deletes highlight 1.
        /// 
        /// </summary>
        
        [Priority(1)]
        protected void multicolor11_4()
        {
            GoToPage(2);

            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 1111, 555), Colors.IndianRed));
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 2222, 777), Colors.RoyalBlue));
            VerifyNumberOfAttachedAnnotations(2);
            if (NonVisibleMode) GoToPage(0);
            DeleteAnnotation(new SimpleSelectionData(2, 1111, 1111));
            if (NonVisibleMode) GoToPage(2);
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(2, 2222, 777))
            });
            passTest("Delete Highlight : deleted selection deleted highlight [1]");
        }

        /// <summary>
        /// two highlights.
        ///    --1--|--2--
        /// 
        /// --3--
        /// merge with one.
        /// --3--1--|--2--
        /// 
        /// create two highlight sections.  merge third highlight with first.
        /// 
        /// </summary>
        [Priority(1)]
        protected void multicolor10_3()
        {
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 99, 151), Colors.IndianRed));
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 250, 99), Colors.RoyalBlue));
            VerifyNumberOfAttachedAnnotations(2);
            if (NonVisibleMode) GoToPage(2);
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 44, 60), Colors.IndianRed));
            if (NonVisibleMode) GoToPage(0);
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(0,44,60)),
                GetText(new SimpleSelectionData(0,104,146)),
                GetText(new SimpleSelectionData(0,250, 99))
            });
            passTest("Highlight : merged [3] with [1] with no affect on [2].");
        }

        /// <summary>
        /// two highlights.
        ///    --1--|--2--
        /// 
        /// ---3----
        /// merge with one.
        /// ---3----|--2--
        /// 
        /// create two highlight sections.  third highlight replaces first.
        /// 
        /// </summary>
        
        [Priority(1)]
        protected void multicolor10_6()
        {
            GoToPage(2);

            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 117, 1717), Colors.IndianRed));
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 1834, 999), Colors.RoyalBlue));
            VerifyNumberOfAttachedAnnotations(2);
            if (NonVisibleMode) GoToPage(0);
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 0, 1834), Colors.IndianRed));
            if (NonVisibleMode) GoToPage(2);
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(2, 0 ,1834)),
                GetText(new SimpleSelectionData(2, 1834, 999))
            });
            passTest("Highlight : [3] replaced [1], no affect on [2].");
        }

        /// <summary>
        /// two highlights.
        /// ---1---|---2---
        /// 
        /// -3-
        /// 
        /// third higlight has no affect.
        /// ---1---|---2---
        /// 
        /// create two highlight sections.  third highlight is created over portion of first highlight with no affect.
        /// 
        /// </summary>
        [Priority(1)]
        protected void multicolor10_7()
        {
            GoToPage(2);

            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 101, 101), Colors.LimeGreen));
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 202, 101), Colors.HotPink));
            VerifyNumberOfAttachedAnnotations(2);
            if (NonVisibleMode) GoToPage(0);
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 101, 75), Colors.LimeGreen));
            if (NonVisibleMode) GoToPage(2);
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(2, 101, 75)),
                GetText(new SimpleSelectionData(2, 176, 26)),
                GetText(new SimpleSelectionData(2, 202, 101))
            });
            passTest("Highlight : [3] created over portion of [1], no affect on [1]&[2].");
        }

        /// <summary>
        /// two highlights.
        /// ---1---|---2---
        /// 
        /// -3-
        /// 
        /// third higlight over 1.
        /// -3-1---|---2---
        /// 
        /// create two highlight sections.  third highlight replaces portion of first highlight.
        /// 
        /// </summary>
        [Priority(1)]
        protected void multicolor10_8()
        {
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 121, 121), Colors.RoyalBlue));
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 242, 121), Colors.Yellow));
            VerifyNumberOfAttachedAnnotations(2);
            if (NonVisibleMode) GoToPage(2);
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(0, 121, 25), Colors.Yellow));
            if (NonVisibleMode) GoToPage(0);
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(0, 121,25)),
                GetText(new SimpleSelectionData(0, 146,96)),
                GetText(new SimpleSelectionData(0, 242, 121))
            });
            passTest("Highlight : [3] replaced portion of [1].");
        }

        /// <summary>
        /// two highlights.
        /// ---1---|---2---
        /// 
        ///   -3-
        /// 
        /// third higlight has no affect.
        /// -1-3---|---2---
        /// 
        /// create two highlight sections.  third highlight made over portion of first highlight, no affect.
        /// 
        /// </summary>
        [Priority(1)]
        protected void multicolor10_10()
        {
            GoToPage(2);

            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 99, 411), Colors.Orange));
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 510, 999), Colors.Salmon));
            VerifyNumberOfAttachedAnnotations(2);
            if (NonVisibleMode) GoToPage(0);
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 221, 199), Colors.Orange));
            if (NonVisibleMode) GoToPage(2);
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(2, 99,122))+
                GetText(new SimpleSelectionData(2, 420,90)),
                GetText(new SimpleSelectionData(2, 221,199)),
                GetText(new SimpleSelectionData(2, 510,999))
            });
            passTest("Highlight : [3] created over portion of [1], no affect on [1]&[2].");
        }


        /// <summary>
        /// two highlights.
        /// ---1---|---2---
        /// 
        ///     -3-
        /// 
        /// third higlight has no affect.
        /// ---1-3-|---2---
        /// 
        /// create two highlight sections.  third highlight made over portion of first highlight, no affect.
        /// 
        /// </summary>
        [Priority(1)]
        protected void multicolor10_12()
        {
            GoToPage(2);

            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 7, 21), Colors.Orange));
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 28, 35), Colors.Salmon));
            VerifyNumberOfAttachedAnnotations(2);
            if (NonVisibleMode) GoToPage(0);
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 21, 7), Colors.Orange));
            if (NonVisibleMode) GoToPage(2);
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(2, 7,14)),
                GetText(new SimpleSelectionData(2, 21,7)),
                GetText(new SimpleSelectionData(2, 28,35))
            });
            passTest("Highlight : [3] created over portion of [1], no affect on [1]&[2].");
        }

        /// <summary>
        /// two highlights.
        /// ---1---|---2---
        /// 
        ///     -3-
        /// 
        /// third truncates 1.
        /// -1-|-3-|---2---
        /// 
        /// create two highlight sections.  third highlight made over portion of first highlight truncating it..
        /// 
        /// </summary>
        
        [Priority(1)]
        protected void multicolor10_13()
        {
            GoToPage(2);

            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 500, 1000), Colors.IndianRed));
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 1500, 1000), Colors.Gray));
            VerifyNumberOfAttachedAnnotations(2);
            if (NonVisibleMode) GoToPage(0);
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 700, 800), Colors.Gold));
            if (NonVisibleMode) GoToPage(2);
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(2, 500, 200)),
                GetText(new SimpleSelectionData(2, 700, 800)),
                GetText(new SimpleSelectionData(2, 1500, 1000))
            });
            passTest("Highlight : [3] created over portion of [1], truncated [1].");
        }

        /// <summary>
        /// two highlights.
        /// ---1---|---2---
        /// 
        /// ---3---
        /// 
        /// third replaces 1.
        /// ---3---|---2---
        /// 
        /// create two highlight sections.  third highlight replaces one..
        /// 
        /// </summary>
        [Priority(1)]
        protected void multicolor10_14()
        {
            GoToPage(2);

            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 333, 700), Colors.DodgerBlue));
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 1033, 497), Colors.DarkGreen));
            VerifyNumberOfAttachedAnnotations(2);
            if (NonVisibleMode) GoToPage(0);
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 333, 700), Colors.LightYellow));
            if (NonVisibleMode) GoToPage(2);
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(2, 333, 700)),
                GetText(new SimpleSelectionData(2, 1033, 497))
            });
            passTest("Highlight : Replace [1] with [3].");
        }

        /// <summary>
        /// two highlights.
        /// ---1---|---2---
        /// 
        /// ---3---
        /// 
        /// third no effect on 1.
        /// ---1---|---2---
        /// 
        /// create two highlight sections.  third highlight has no effect on one and two..
        /// 
        /// </summary>
        
        [Priority(1)]
        protected void multicolor10_15()
        {
            GoToPage(2);

            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 1333, 501), Colors.DodgerBlue));
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 1834, 1534), Colors.DarkGreen));
            VerifyNumberOfAttachedAnnotations(2);
            if (NonVisibleMode) GoToPage(0);
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 1333, 501), Colors.DodgerBlue));
            if (NonVisibleMode) GoToPage(2);
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(2, 1333, 501)),
                GetText(new SimpleSelectionData(2, 1834, 1534))
            });
            passTest("Highlight : [3] has no affect on [1].");
        }

        /// <summary>
        /// two highlights.
        /// ---1---|---2---
        /// 
        /// ----3----
        /// 
        /// merge 3and2 replace 1.
        /// ---3----|--2---
        /// 
        /// create two highlight sections.  merge second and third and replace one..
        /// 
        /// </summary>
        [Priority(1)]
        protected void multicolor10_16()
        {
            GoToPage(2);

            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 33, 248), Colors.DodgerBlue));
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 281, 1099), Colors.Fuchsia));
            VerifyNumberOfAttachedAnnotations(2);
            if (NonVisibleMode) GoToPage(0);
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 33, 501), Colors.Fuchsia));
            if (NonVisibleMode) GoToPage(2);
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(2, 33, 501)),
                GetText(new SimpleSelectionData(2, 534, 846))
            });
            passTest("Highlight : Merged [3] and [2], replaced [1].");
        }

        /// <summary>
        /// two highlights.
        /// ---1---|---2---
        /// 
        /// -------3-------
        /// 
        /// 3 replaces 1 and 2.
        /// -------3-------
        /// 
        /// create two highlight sections.  third highlight will replace 1 and 2..
        /// 
        /// </summary>
        
        [Priority(1)]
        protected void multicolor10_18()
        {
            GoToPage(2);

            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 777, 777), Colors.DodgerBlue));
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 1554, 1001), Colors.Fuchsia));
            VerifyNumberOfAttachedAnnotations(2);
            if (NonVisibleMode) GoToPage(0);
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 33, 2522), Colors.Yellow));
            if (NonVisibleMode) GoToPage(2);
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(2, 33, 2522))
            });
            passTest("Highlight : [3] replaced [1] and [2].");
        }

        /// <summary>
        /// two highlights.
        /// ---1---|---2---
        /// 
        /// ----3----
        /// 
        /// merge 3and2 replace 1.
        /// ---3----|--2---
        /// 
        /// create two highlight sections.  merge second and third and replace one..
        /// 
        /// </summary>
        
        [Priority(1)]
        protected void multicolor10_19()
        {
            GoToPage(2);

            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 107, 701), Colors.Crimson));
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 808, 1616), Colors.DarkOrange));
            VerifyNumberOfAttachedAnnotations(2);
            if (NonVisibleMode) GoToPage(0);
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 107, 2317), Colors.DarkOrange));
            if (NonVisibleMode) GoToPage(2);
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(2, 107, 2317))
            });
            passTest("Highlight : Merged [3] and [2], replaced [1].");
        }

        /// <summary>
        /// two highlights.
        /// ---1---|---2---
        ///  |   |   
        ///  [del]
        /// 
        /// 1 deleted.
        /// -1    -|---2---
        /// 
        /// create two highlight sections.  delete portion of highlight 1.
        /// 
        /// </summary>
        [Priority(1)]
        protected void multicolor12_3()
        {
            GoToPage(2);

            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 505, 501), Colors.IndianRed));
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 1006, 777), Colors.RoyalBlue));
            VerifyNumberOfAttachedAnnotations(2);
            if (NonVisibleMode) GoToPage(0);
            DeleteAnnotation(new SimpleSelectionData(2, 700, 155));
            if (NonVisibleMode) GoToPage(2);
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(2, 505, 195))+
                GetText(new SimpleSelectionData(2, 855, 151)),
                GetText(new SimpleSelectionData(2, 1006, 777))
            });
            passTest("Delete Highlight : deleted portion of highlight [1]");
        }

        /// <summary>
        /// two highlights.
        /// ---1---|---2---
        /// |          |   
        /// [    del   ]
        /// 
        /// 1 deleted, 2 truncated.
        /// -1    -|---2---
        /// 
        /// create two highlight sections.  delete 1 and truncate 2.
        /// 
        /// </summary>
        
        [Priority(1)]
        protected void multicolor12_4()
        {
            GoToPage(2);

            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 321, 1234), Colors.CornflowerBlue));
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 1555, 999), Colors.Pink));
            VerifyNumberOfAttachedAnnotations(2);
            if (NonVisibleMode) GoToPage(0);
            DeleteAnnotation(new SimpleSelectionData(2, 201, 2002));
            if (NonVisibleMode) GoToPage(2);
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(2, 2203, 351))
            });
            passTest("Delete Highlight : deleted [1], truncated [2]");
        }

        /// <summary>
        /// two highlights.
        /// ---1---|---2---
        ///        |       |   
        ///        [  del  ]
        /// 
        /// 2 deleted.
        /// ---1---
        /// 
        /// create two highlight sections.  delete 2.
        /// 
        /// </summary>
        
        [Priority(1)]
        protected void multicolor12_6()
        {
            GoToPage(2);

            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 666, 1299), Colors.SkyBlue));
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 1965, 1265), Colors.OrangeRed));
            VerifyNumberOfAttachedAnnotations(2);
            if (NonVisibleMode) GoToPage(0);
            DeleteAnnotation(new SimpleSelectionData(2, 1965, 1265));
            if (NonVisibleMode) GoToPage(2);
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(2, 666, 1299))
            });
            passTest("Delete Highlight : deleted [2].");
        }

        /// <summary>
        /// two highlights.
        /// ---1---|---2---
        ///            |   |   
        ///            [del]
        /// 
        /// 2 deleted.
        /// ---1---|-2-
        /// 
        /// create two highlight sections.  truncate 2.
        /// 
        /// </summary>
        
        [Priority(1)]
        protected void multicolor12_7()
        {
            GoToPage(2);

            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 341, 709), Colors.SkyBlue));
            CreateAnnotation(new HighlightDefinition(new SimpleSelectionData(2, 1050, 1509), Colors.OrangeRed));
            VerifyNumberOfAttachedAnnotations(2);
            if (NonVisibleMode) GoToPage(0);
            DeleteAnnotation(new SimpleSelectionData(2, 2232, 327));
            if (NonVisibleMode) GoToPage(2);
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(2, 341, 709)),
                GetText(new SimpleSelectionData(2, 1050, 1182))
            });
            passTest("Delete Highlight : truncated [2].");
        }

        #endregion PRIORITY TESTS
    }
}

