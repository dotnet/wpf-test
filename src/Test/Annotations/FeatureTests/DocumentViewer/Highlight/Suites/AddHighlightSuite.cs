// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Pri1 cases that verify add behavior for Highlight annotations.

using System;
using System.Windows;
using Annotations.Test.Framework;
using Annotations.Test;
using System.Windows.Media;
using System.Collections;

namespace Avalon.Test.Annotations.Suites
{
    public class AddHighlightSuite : AHighlightSuite
    {
        #region PRIORITY TESTS

        /// <summary>
        /// Create highlight which contains an existing highlight. Result: 1 highlight.
        /// </summary>
        [Priority(1)]
        protected void add1_6()
        {
            if (!NonVisibleMode) PageDown();
            CreateAnnotation(new SimpleSelectionData(1, 105, 35));
            CreateAnnotation(new SimpleSelectionData(1, 100, 50));
            if (NonVisibleMode) PageDown();
            VerifyAnnotation(GetText(new SimpleSelectionData(1, 100, 50)));
            passTest("No error.");
        }

        /// <summary>
        /// Create highlight which is subset of existing: no-op.
        /// </summary>
        [Priority(1)]
        protected void add1_7()
        {
            if (!NonVisibleMode) PageDown();
            CreateAnnotation(new SimpleSelectionData(1, 100, 50));
            CreateAnnotation(new SimpleSelectionData(1, 105, 35));
            if (NonVisibleMode) PageDown();
            VerifyAnnotations(new string[] {
				GetText(new SimpleSelectionData(1, 100, 5)) +
				GetText(new SimpleSelectionData(1, 140, 10)),
				GetText(new SimpleSelectionData(1, 105, 35))
				});
            passTest("No error.");
        }

        /// <summary>
        /// Create adjacent highlight after existing highlight.  Verify they merge.
        /// </summary>
        [Priority(1)]
        protected void add1_8()
        {
            if (!NonVisibleMode) PageDown();
            CreateAnnotation(new SimpleSelectionData(1, 20, 10));
            CreateAnnotation(new SimpleSelectionData(1, 30, 10));
            if (NonVisibleMode) PageDown();
            VerifyAnnotations(new string[] {
				GetText(new SimpleSelectionData(1, 20, 10)),
				GetText(new SimpleSelectionData(1, 30, 10))
				});
            passTest("No error.");
        }

        /// <summary>
        /// Selection starts at beginning of existing highlight.  Create new extended highlight.
        /// </summary>
        [Priority(1)]
        protected void add1_9()
        {
            if (!NonVisibleMode) PageDown();
            CreateAnnotation(new SimpleSelectionData(1, 20, 10));
            CreateAnnotation(new SimpleSelectionData(1, 20, 20));
            if (NonVisibleMode) PageDown();
            VerifyAnnotation(GetText(new SimpleSelectionData(1, 20, 20)));
            passTest("No error.");
        }

        /// <summary>
        /// Partially overlapping highlights merged into one.
        /// </summary>
        [Priority(1)]
        protected void add1_11()
        {
            if (!NonVisibleMode) PageDown();
            CreateAnnotation(new SimpleSelectionData(1, 105, 35));
            CreateAnnotation(new SimpleSelectionData(1, 90, 40));
            if (NonVisibleMode) PageDown();
            VerifyAnnotations(new string[] {
			GetText(new SimpleSelectionData(1, 90, 40)),
			GetText(new SimpleSelectionData(1, 130, 10))
			});
            passTest("No error.");
        }

        /// <summary>
        /// Create adjacent highlight before existing highlight.  Verify they merge.
        /// </summary>
        [Priority(1)]
        protected void add1_13()
        {
            if (!NonVisibleMode) PageDown();
            CreateAnnotation(new SimpleSelectionData(1, 30, 10));
            CreateAnnotation(new SimpleSelectionData(1, 20, 10));
            if (NonVisibleMode) PageDown();
            VerifyAnnotations(new string[] {
				GetText(new SimpleSelectionData(1, 30, 10)),
				GetText(new SimpleSelectionData(1, 20, 10))
				});
            passTest("No error.");
        }

        /// <summary>
        /// Create overlapping highlight that spans a page break, verify highlights are merged.
        /// </summary>
        [Priority(1)]
        protected void add1_15()
        {
            if (!NonVisibleMode) GoToPageRange(1, 2);
            CreateAnnotation(new SimpleSelectionData(2, 10, 15));
            ISelectionData spanningSelection = new MultiPageSelectionData(1, PagePosition.End, -49, 2, PagePosition.Beginning, 51);
            CreateAnnotation(spanningSelection);
            if (NonVisibleMode) GoToPageRange(1, 2);
            VerifyAnnotation(GetText(spanningSelection));
            passTest("No error.");
        }

        /// <summary>
        /// Initial: ------------   -------------
        /// Add:				 ---
        /// Final:	 ----------------------------
        /// </summary>
        [Priority(1)]
        protected void add2_2()
        {
            if (!NonVisibleMode) PageDown();
            CreateAnnotation(new SimpleSelectionData(1, 20, 10));
            CreateAnnotation(new SimpleSelectionData(1, 35, 5));
            CreateAnnotation(new SimpleSelectionData(1, 30, 5));
            if (NonVisibleMode) PageDown();
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(1, 20, 10)),
                GetText(new SimpleSelectionData(1, 35, 5)),
                GetText(new SimpleSelectionData(1, 30, 5))
                });
            passTest("No error.");
        }

        /// <summary>
        /// Initial: ------------   -------------
        /// Add:			   -------
        /// Final:	 ----------------------------
        /// </summary>
        [Priority(1)]
        protected void add2_3()
        {
            if (!NonVisibleMode) PageDown();
            CreateAnnotation(new SimpleSelectionData(1, 20, 10));
            CreateAnnotation(new SimpleSelectionData(1, 35, 5));
            CreateAnnotation(new SimpleSelectionData(1, 25, 12));
            if (NonVisibleMode) PageDown();
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(1, 20, 5)),
                GetText(new SimpleSelectionData(1, 37, 3)),
                GetText(new SimpleSelectionData(1, 25, 12))
                });
            passTest("No error.");
        }

        /// <summary>
        /// Initial: ------------   -------------
        /// Add:	 ------------------
        /// Final:	 ----------------------------
        /// </summary>
        [Priority(1)]
        protected void add2_4()
        {
            if (!NonVisibleMode) PageDown();
            CreateAnnotation(new SimpleSelectionData(1, 20, 10));
            CreateAnnotation(new SimpleSelectionData(1, 35, 5));
            CreateAnnotation(new SimpleSelectionData(1, 20, 17));
            if (NonVisibleMode) PageDown();
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(1, 20, 17)),
                GetText(new SimpleSelectionData(1, 37, 3))
                });
            passTest("No error.");
        }

        /// <summary>
        /// Initial:    ------------   -------------
        /// Add:	 ---------------------
        /// Final:	 -------------------------------
        /// </summary>
        [Priority(1)]
        protected void add2_5()
        {
            if (!NonVisibleMode) PageDown();
            CreateAnnotation(new SimpleSelectionData(1, 25, 5));
            CreateAnnotation(new SimpleSelectionData(1, 35, 5));
            CreateAnnotation(new SimpleSelectionData(1, 20, 17));
            if (NonVisibleMode) PageDown();
            VerifyAnnotations(new string[] {
                GetText(new SimpleSelectionData(1, 20, 17)),
                GetText(new SimpleSelectionData(1, 37, 3))
                });
            passTest("No error.");
        }

        /// <summary>
        /// Annotations merged across pages.
        /// </summary>
        
        [DisabledTestCase()]
        [Priority(1)]
        protected void add2_7()
        {
            if (!NonVisibleMode) GoToPageRange(1, 2);
            CreateAnnotation(new MultiPageSelectionData(1, PagePosition.End, -50, 1, PagePosition.End, -25));
            CreateAnnotation(new SimpleSelectionData(2, PagePosition.Beginning, 49));
            CreateAnnotation(new SimpleSelectionData(1, PagePosition.End, -25));
            if (NonVisibleMode) GoToPageRange(1, 2);
            VerifyAnnotation(GetText(new MultiPageSelectionData(1, PagePosition.End, -50, 2, PagePosition.Beginning, 49)));
            passTest("No error.");
        }

        /// <summary>
        /// Mutliple highlights across pages merged into 1.
        /// </summary>
        [Priority(1)]
        protected void add2_9()
        {
            GoToPage(2);
            CreateAnnotation(new SimpleSelectionData(2, 10, 15));
            CreateAnnotation(new SimpleSelectionData(2, 1067, 194));
            CreateAnnotation(new SimpleSelectionData(2, PagePosition.End, -65));
            GoToPage(3);
            CreateAnnotation(new SimpleSelectionData(3, 8, 22));
            CreateAnnotation(new MultiPageSelectionData(1, PagePosition.End, -74, 3, PagePosition.Beginning, 93));
            printStatus("Begin verify.");
            VerifyAnnotation(GetText(new SimpleSelectionData(3, PagePosition.Beginning, 93)));
            printStatus("Verified segment on page 3.");
            GoToPage(2);
            VerifyAnnotation(GetText(new MultiPageSelectionData(2, PagePosition.Beginning, 0, 2, PagePosition.End, 0)));
            printStatus("Verified segment on page 2.");
            GoToPage(1);
            VerifyAnnotation(GetText(new SimpleSelectionData(1, PagePosition.End, -74)));
            printStatus("Verified segment on page 1.");
            passTest("No error.");
        }

        /// <summary>
        /// Verify default opacity respected.
        /// </summary>
        [OverrideClassTestDimensions]
        [TestDimension("fixed,flow")]
        [Priority(1)]
        protected void add_opacity1()
        {
            TestHighlightColor(Brushes.Red, Colors.Red);
        }

        /// <summary>
        /// Verify setting opacity in Color is respected.
        /// </summary>
        [OverrideClassTestDimensions]
        [TestDimension("fixed,flow")]
        [Priority(1)]
        protected void add_opacity2()
        {
            Color c = (Color)ColorConverter.ConvertFromString("#33FF00FF");
            TestHighlightColor(new SolidColorBrush(c), c);
        }

        /// <summary>
        /// Verify setting opacity on brush is preserved.
        /// </summary>
        [OverrideClassTestDimensions]
        [TestDimension("fixed,flow")]
        [Priority(1)]
        protected void add_opacity3()
        {
            SolidColorBrush b = Brushes.Black.CloneCurrentValue();
            b.Opacity = .5;
            TestHighlightColor(b, (Color)ColorConverter.ConvertFromString("#7F000000"));
        }

        /// <summary>
        /// Verify setting opacity on Color AND brush produces expected results.
        /// </summary>
        [OverrideClassTestDimensions]
        [TestDimension("fixed,flow")]
        [Priority(1)]
        protected void add_opacity4()
        {
            Color c = (Color)ColorConverter.ConvertFromString("#33FF00FF");
            SolidColorBrush b = new SolidColorBrush(c);
            b.Opacity = .5;
            TestHighlightColor(b, (Color)ColorConverter.ConvertFromString("#19FF00FF"));
        }

        private void TestHighlightColor(SolidColorBrush brush, Color expectedHighlightColor)
        {
            SetSelection(new SimpleSelectionData(0, 10, 50));
            System.Windows.Annotations.AnnotationService.CreateHighlightCommand.Execute(brush, TextControl);
            DispatcherHelper.DoEvents();
            VerifyNumberOfAttachedAnnotations(1);
            IList highlights = AnnotationComponentFinder.GetVisibleHighlightComponents(TextControl);
            AssertEquals("Num highlight components.", 1, highlights.Count);
            AssertEquals("Verify color", expectedHighlightColor, HighlightStateInfo.FromHighlightComponent(highlights[0]).HighlightBrushColor);
            passTest("Verified highlight opacity.");
        }

        #endregion PRIORITY TESTS
    }
}

