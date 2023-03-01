// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 


using Size = System.Windows.Size;

using System;
using System.Windows;
using Annotations.Test;
using Annotations.Test.Framework;
using System.Windows.Media;
using System.Drawing;

namespace Avalon.Test.Annotations.Pri1s
{
	public class TableSuite : ATableSuite
	{
        #region Simple Table

        /// <summary>
        ///  Annotate across table.
        /// </summary>
        protected void table_simple1()
        {
            ITableSegment selection = new TableSpan(-25, 13);
            CreateAnnotation(TableSelectionFactory.Create(selection));
            VerifyAnnotation(GetText(selection));
            passTest("Annotation across table.");
        }

        /// <summary>
        ///  Annotate multiple rows.
        /// </summary>
        protected void table_simple5()
        {
            TestTableSelection(new CellSpan(2, 1, 3, 6));
        }

        /// <summary>
        ///  Annotate multiple columns.
        /// </summary>
        protected void table_simple7()
        {
            TestTableSelection(new CellSpan(1, 3, 4, 4));
        }

        /// <summary>
        ///  Annotate middle of table.
        /// </summary>
        protected void table_simple8()
        {
            TestTableSelection(new CellSpan(2, 2, 3, 4));
        }

        /// <summary>
        /// Annotate column across page boundary: 1st page visible.
        /// </summary>
        protected void table_simple13_1()
        {
            simpletable13(true, true);
        }
        /// <summary>
        /// Annotate column across page boundary: 2nd page visible.
        /// </summary>
        protected void table_simple13_2()
        {
            simpletable13(true, false);
        }        
        protected void simpletable13(bool createBeforeReflow, bool firstPageVisible)
        {
            TestCrossPageTableSelection(
                200,
                new Size(913, 376),
                new CellSpan(1, 2, 4, 2),
                new ITableSegment[] { new CellSpan(1, 2, 2, 2) },
                new ITableSegment[] { new CellSpan(3, 2, 4, 2) },
                createBeforeReflow,
                firstPageVisible);
        }

        /// <summary>
        /// Annotate partial cell split across page boundary: annotate before cell is split..
        /// </summary>
        protected void table_simple15_1()
        {
            DoSimpletable15(false, true);
        }
        /// <summary>
        /// Annotate partial cell split across page boundary: 1st page visible.
        /// </summary>
        protected void table_simple15_2()
        {
            DoSimpletable15(true, true);
        }
        /// <summary>
        /// Annotate partial cell split across page boundary: 2nd page visible.
        /// </summary>
        protected void table_simple15_3()
        {
            DoSimpletable15(true, false);
        }
        protected void DoSimpletable15(bool createBeforeReflow, bool firstPageVisible)
        {
            TestCrossPageTableSelection(
                200,
                new Size(493, 461),
                new CellSubset(2,3, PagePosition.Beginning, 2, PagePosition.End, -2),
                new ITableSegment[] { new CellSubset(2, 3, PagePosition.Beginning, 2, PagePosition.Beginning, 6) },
                new ITableSegment[] { new CellSubset(2, 3, PagePosition.End, -3, PagePosition.End, -2) },
                createBeforeReflow,
                firstPageVisible);
        }

        #endregion

        #region Spanning Table

        /// <summary>
        /// Annotate 1st and 2nd column containing multi-column span.
        /// </summary>
        protected void table_spanning2()
        {
            TestTableSelection(new CellSpan(3, 3, 5, 4));
        }

        /// <summary>
        /// Annotate whole table.
        /// </summary>
        protected void table_spanning4()
        {
            TestTableSelection(new TableSpan());
        }

        /// <summary>
        /// Multi-column cell split across page boundary.
        /// </summary>
        protected void table_spanning5()
        {
            TestCrossPageTableSelection(
            100,
            new Size(375,468),
            new CellSpan(4, 3),
            new ITableSegment[] { new CellStart(4, 3, 41) },
            new ITableSegment[] { new CellSubset(4, 3, PagePosition.End, -1, PagePosition.End, -27) },
            false,
            true);
        }

        /// <summary>
        /// Annotate cell with RowSpan=2.
        /// </summary>
        protected void table_spanning6()
        {
            TestTableSelection(new SingleCell(6, 2));
        }

        /// <summary>
        /// Annotate above and including cell with RowSpan=2.
        /// </summary>
        protected void table_spanning9()
        {
            TestTableSelection(new CellSpan(5, 2, 7, 3));
        }

        /// <summary>
        /// Annotate column containing cell with RowSpan=2.
        /// </summary>
        protected void table_spanning11()
        {
            TestTableSelection(new CellSpan(1, 2, 10, 2));
        }

        /// <summary>
        /// Multi-row cell split across page boundary.
        /// </summary>
        protected void table_spanning12()
        {
            TestCrossPageTableSelection(
                DocViewerWrapper.GetZoom(),
                new Size(913, 400),
                new CellSpan(6, 2),
                new ITableSegment[] { new CellStart(6,2,25) },
                new ITableSegment[] { new CellSubset(6,2, PagePosition.End, -1, PagePosition.End, -25) },
                true,
                true);
        }

        /// <summary>
        /// Annotate across row that spans full table width.
        /// </summary>
        protected void table_spanning14()
        {
            TestTableSelection(new CellSpan(8, 1, 10, 4));
        }

        #endregion

        #region Nested Table

        /// <summary>
        /// Annotate cell containing nested table.
        /// </summary>
        protected void table_nested2()
        {
            TestTableSelection(new SingleCell(2, 2), new ITableSegment[] { new CellStart(2, 2, 37), new CellSubset(2, 2, PagePosition.Beginning, 38, PagePosition.End, -37), new CellSubset(2, 2, PagePosition.End, -1, PagePosition.End, -36) });
        }

        /// <summary>
        /// Annotate row containing nested table.
        /// </summary>
        protected void table_nested6()
        {
            TestTableSelection(new RowSpan(2), new ITableSegment[] { new RowSpan(2), new CellStart(2, 2, 37), new CellSubset(2, 2, PagePosition.Beginning, 38, PagePosition.End, -37), new CellSubset(2, 2, PagePosition.End, -1, PagePosition.End, -36) });
        }

        /// <summary>
        /// Annotate into nested table.
        /// </summary>
        protected void table_nested7()
        {
            this.TableSelectionFactory = new TableSelectionFactory(ViewerTestConstants.TableTests.InnerNestedTablename);
            TestTableSelection(new TableStart(-25, 2), new ITableSegment[] { new TableStart(-25,-1), new CellSpan(1,1, 2, 2) });
        }

        /// <summary>
        /// Annotate across nested table split on page boundary.
        /// </summary>
        protected void table_nested8()
        {
            // Hardcoded anchors because its just way easier than trying to define this case programmatically.
            string firstPage = "Row0.1	Row0.2	Row0.3	Row0.4\r\n" + "Row1.1	This is right before a nested table.\r\n" + "one	two\r\n" + "Three	4\r\n" + "Five	Six Seven\r\n" + "Eight	Nine\r\n" + "This is right after a nested table.\r\n" + "Row1.3\r\n" + "Row2	Row2.1\r\n" + "Row3	Row3.1\r\n" + "Row4	Row4.1\r\n";
            string secondPage = "Row5.1\t" + "Five	Six Seven\r\n" + "Eight	Nine\r\n" + "This is right after a nested table.\t" + "Row5.2	Row5.3	Row5.4";

            SetWindowHeight(576);
            SetWindowWidth(913);
            PageDown();
            CreateAnnotation(TableSelectionFactory.Create(new TableSpan()));
            VerifyAnnotation(secondPage);
            PageUp();
            VerifyAnnotation(firstPage);
            passTest("Nested table split across page boundary.");
        }

        #endregion

        #region Highlight Tests

        /// <summary>
        /// Clear start of middle cell.
        /// </summary>
        protected void table_simple_highlight4()
        {
            Bitmap master = GetBitmapOfView();
            CreateHighlight(new RowSpan(2), Colors.Yellow);
            ClearHighlight(new CellStart(2, 2, 3));                        
            VerifyTableAnnotation(master, new ITableSegment[] { new CellSpan(2, 1), new CellEnd(2, 2, -1), new CellSubset(2, 2, PagePosition.Beginning, 3, PagePosition.End, 0), new CellSpan(2, 3, 2, 6) } );
            passTest("Verified splitting highlight at start of cell.");
        }

        /// <summary>
        /// Clear end of middle cell.
        /// </summary>
        protected void table_simple_highlight5()
        {
            Bitmap master = GetBitmapOfView();
            CreateHighlight(new RowSpan(2), Colors.Yellow);
            ClearHighlight(new CellEnd(2, 2, -3));
            VerifyTableAnnotation(master, new ITableSegment[] { new CellSpan(2, 1), new CellEnd(2, 2, -1), new CellSubset(2, 2, PagePosition.Beginning, 0, PagePosition.End, -3), new CellSpan(2, 3, 2, 6) } );
            passTest("Verified splitting highlight at end of cell.");
        }

        /// <summary>
        /// Highlight middle of block with different color.
        /// </summary>
        protected void table_simple_highlight8_2()
        {
            CreateHighlight(new CellSpan(1,1, 3,3), Colors.Green);
            CreateHighlight(new CellSpan(2, 2), Colors.Red);
            VerifyNumberOfAttachedAnnotations(2);
            VerifyAnnotationWithAnchorExists(GetText(new ITableSegment[] { new CellSpan(1,1, 1,3), new CellSpan(2,1), new CellEnd(2, 2, -1), new CellSpan(2,3), new CellSpan(3,1,3,3) }));
            VerifyAnnotationWithAnchorExists(GetText(new SingleCell(2,2)));            
            passTest("Verified splitting highlight with differnt color.");
        }

        /// <summary>
        /// Clear middle row of highlighted block.
        /// </summary>
        protected void table_simple_highlight8_3()
        {
            Bitmap master = GetBitmapOfView();
            CreateHighlight(new CellSpan(1, 1, 3, 3), Colors.Green);            
            ClearHighlight(new RowSpan(2));
            VerifyTableAnnotation(master, new ITableSegment[] { new CellSpan(1,1, 1,3), new CellSpan(3,1, 3,3) } );
            passTest("Verified clearing row of highlighted block.");
        }

        /// <summary>
        /// Clear column of highlighted block.
        /// </summary>
        protected void table_simple_highlight8_4()
        {
            Bitmap master = GetBitmapOfView();
            CreateHighlight(new CellSpan(1, 1, 3, 3), Colors.Green);
            ClearHighlight(new CellSpan(1,2, 3,2));
            VerifyTableAnnotation(master, new ITableSegment[] { 
                new CellSpan(1,1), new CellSpan(1,3),
                new CellSpan(2,1), new CellSpan(2,3),
                new CellSpan(3,1), new CellSpan(3,3)            
            });
            passTest("Verified clearing column of highlighted block.");
        }

        /// <summary>
        /// Split middle cell of highlighted block.
        /// </summary>
        protected void table_simple_highlight8_6()
        {
            CreateHighlight(new CellSpan(1, 2, 3, 4), Colors.Green);
            Bitmap master = GetBitmapOfView();
            ITableSegment[] greenAnchor = new ITableSegment[] { new CellSpan(1,2,1,4), new CellSpan(2, 2), new CellStart(2, 3, 3), new CellEnd(2, 3, -3), new CellSpan(2, 4), new CellSpan(3,2,3,4) };
            ITableSegment redAnchor = new CellSubset(2, 3, PagePosition.Beginning, 3, PagePosition.End, -3);
            CreateHighlight(redAnchor, Colors.Red);
            VerifyNumberOfAttachedAnnotations(2);            
            VerifyAnnotationWithAnchorExists(GetText(greenAnchor));
            VerifyAnnotationWithAnchorExists(GetText(redAnchor));
            VerifyThatExpectRegionWasUnchanged(master, new ITableSegment[] { redAnchor });
            passTest("Verified clearing column of highlighted block.");
        }

        /// <summary>
        /// Different columns are different colors, unhighlight table.
        /// </summary>
        protected void table_simple_highlight16_2()
        {
            Bitmap master = GetBitmapOfView();
            CreateHighlight(new CellSpan(1, 1, 4, 2), Colors.Green);
            CreateHighlight(new CellSpan(1, 3, 4, 3), Colors.Red);
            CreateHighlight(new CellSpan(1, 4, 4, 5), Colors.Blue);
            CreateHighlight(new CellSpan(1, 6, 4, 6), Colors.Yellow);            
            ClearHighlight(new TableSpan());

            VerifyNumberOfAttachedAnnotations(0);
            CompareBitmaps(master, GetBitmapOfView(), true);
            passTest("Verified clearing table multi multi-color columns.");
        }

        /// <summary>
        /// Each row is a different color, clear table.
        /// </summary>
        protected void table_simple_highlight17_2()
        {
            Bitmap master = GetBitmapOfView();
            CreateHighlight(new RowSpan(1), Colors.Green);
            CreateHighlight(new RowSpan(2), Colors.Red);
            CreateHighlight(new RowSpan(3), Colors.Blue);
            CreateHighlight(new RowSpan(4), Colors.Yellow);            
            ClearHighlight(new TableSpan());

            VerifyNumberOfAttachedAnnotations(0);
            CompareBitmaps(master, GetBitmapOfView(), true);
            passTest("Verified clearing table across multi-color rows.");
        }

        /// <summary>
        /// 2 Green cells, highlight over one with part of 2 red cell highlight.
        /// </summary>
        protected void table_simple_highlight18()
        {
            Bitmap master = GetBitmapOfView();
            CreateHighlight(new CellSpan(3,3, 3,4), Colors.Green);
            CreateHighlight(new CellSpan(3, 2, 3, 3), Colors.Red);
            VerifyNumberOfAttachedAnnotations(2);
            VerifyAnnotationWithAnchorExists(GetText(new CellSpan(3, 4)));
            VerifyAnnotationWithAnchorExists(GetText(new CellSpan(3, 2, 3, 3)));
            passTest("Verified partially overlapping highlights.");
        }

        /// <summary>
        /// 1. Annotation across table.
        /// 2. Clear table portion.
        /// </summary>
        protected void table_simple_highlight19_1()
        {
            CreateHighlight(new TableSpan(-50, 50), Colors.Red);
            ClearHighlight(new TableSpan());
            VerifyAnnotation(GetText(new ITableSegment[] { new TableStart(-50, -1), new TableEnd(-1, 50) }));
            passTest("Verified clearing table.");
        }

        /// <summary>
        /// 1. Annotation across table.
        /// 2. Clear row.
        /// </summary>
        protected void table_simple_highlight19_2()
        {
            Bitmap master = GetBitmapOfView();
            CreateHighlight(new TableSpan(-50, 50), Colors.Red);
            ClearHighlight(new RowSpan(3));
            VerifyTableAnnotation(master,
                new ITableSegment[] { new TableStart(-50, 2), new TableEnd(4, 50) },
                new ITableSegment[] { new TableStart(-50, -1), new CellSpan(1, 1, 2, 6), new RowSpan(4), new TableEnd(4, 50) });
            passTest("Verified clearing row.");
        }

        /// <summary>
        /// 1. Annotation across table.
        /// 2. Clear column.
        /// </summary>
        protected void table_simple_highlight19_3()
        {
            Bitmap master = GetBitmapOfView();
            CreateHighlight(new TableSpan(-50, 50), Colors.Red);
            ClearHighlight(new CellSpan(1,4, 4,4));
            VerifyTableAnnotation(master,
                new ITableSegment[] { 
                    new TableStart(-50, -1), 
                    new CellSpan(1,1, 1,3), new CellSpan(1,5, 1,6), 
                    new CellSpan(2,1, 2,3), new CellSpan(2,5, 2,6), 
                    new CellSpan(3,1, 3,3), new CellSpan(3,5, 3,6), 
                    new CellSpan(4,1, 4,3), new CellSpan(4,5, 4,6), 
                    new TableEnd(-1, 50) },
                new ITableSegment[] { 
                    new TableStart(-50, -1),
                    new CellSpan(1, 1, 4, 3), new CellSpan(1, 5, 4, 6) ,
                    new TableEnd(-1, 50)
                });
            passTest("Verified clearing column.");
        }

        /// <summary>
        /// 1. Annotation across table.
        /// 2. Clear end of highlight.
        /// </summary>
        protected void table_simple_highlight19_4()
        {
            Bitmap master = GetBitmapOfView();
            CreateHighlight(new TableSpan(-50, 50), Colors.Red);
            ClearHighlight(new TableEnd(3, 50));
            VerifyTableAnnotation(master, new ITableSegment[] { new TableStart(-50, 2) }, new ITableSegment[] { new TableStart(-50, -1), new CellSpan(1, 1, 2, 6) });
            passTest("Verified clearing end of highlight.");
        }

        /// <summary>
        /// 1. Annotation across table.
        /// 2. Clear start of highlight.
        /// </summary>
        protected void table_simple_highlight19_5()
        {
            Bitmap master = GetBitmapOfView();
            CreateHighlight(new TableSpan(-50, 50), Colors.Red);
            ClearHighlight(new TableStart(-50, 2));
            VerifyTableAnnotation(master, new ITableSegment[] { new TableEnd(3, 50) }, new ITableSegment[] { new CellSpan(3, 1, 4, 6), new TableEnd(-1, 50) });
            passTest("Verified clearing start of table.");
        }

        /// <summary>
        /// Unhighlight entire inner table.
        /// </summary>
        protected void table_nested_highlight1()
        {
            Bitmap master = GetBitmapOfView();
            CreateHighlight(new CellSpan(2,2), Colors.Green);
            this.TableSelectionFactory = new TableSelectionFactory(ViewerTestConstants.TableTests.InnerNestedTablename);
            ClearHighlight(new TableSpan());
            this.TableSelectionFactory = new TableSelectionFactory(ViewerTestConstants.TableTests.OuterNestedTableName);
            VerifyTableAnnotation(master, new ITableSegment[] { new CellStart(2, 2, 37), new LineBreak(), new CellSubset(2, 2, PagePosition.End, -1, PagePosition.End, -36) });
            passTest("Verified clearing inner table.");
        }

        /// <summary>
        /// Unhighlight entire inner table.
        /// </summary>
        protected void table_nested_highlight2()
        {
            Bitmap master = GetBitmapOfView();
            CreateHighlight(new TableSpan(), Colors.Red);
            this.TableSelectionFactory = new TableSelectionFactory(ViewerTestConstants.TableTests.InnerNestedTablename);
            ClearHighlight(new TableSpan());
            this.TableSelectionFactory = new TableSelectionFactory(ViewerTestConstants.TableTests.OuterNestedTableName);
            VerifyTableAnnotation(master, new ITableSegment[] { 
                new RowSpan(1), 
                new CellSpan(2,1), new CellStart(2,2,37), new LineBreak(), new CellSubset(2, 2, PagePosition.End, -1, PagePosition.End, -36), new LineBreak(), new CellSpan(2,11, 2, 11, true),
                new CellSpan(7,1, 10,4)
            });
            passTest("Verified clearing inner table in whole table selection.");
        }

        /// <summary>
        /// Unhighlight column of inner table.
        /// </summary>
        protected void table_nested_highlight3()
        {
            CreateHighlight(new CellSpan(2, 2), Colors.Yellow);
            string anchorStart = GetText(new CellStart(2, 2, 37));
            string anchorEnd = GetText(new CellSubset(2, 2, PagePosition.End, -1, PagePosition.End, -36));
            this.TableSelectionFactory = new TableSelectionFactory(ViewerTestConstants.TableTests.InnerNestedTablename);                        
            ClearHighlight(new CellSpan(1, 2, 4, 2));
            string anchorMiddle = GetText(new ITableSegment[] { new CellSpan(1, 1), new CellSpan(2, 1), new CellSpan(3, 1), new CellSpan(4, 1) });
            VerifyAnnotation(anchorStart + "\r\n" + anchorMiddle + anchorEnd);
            passTest("Verified clearing column within inner table.");
        }

        /// <summary>
        /// Unhighlight row of inner table.
        /// </summary>
        protected void table_nested_highlight4()
        {
            Bitmap master = GetBitmapOfView();
            CreateHighlight(new CellSpan(2, 2), Colors.Blue);            
            this.TableSelectionFactory = new TableSelectionFactory(ViewerTestConstants.TableTests.InnerNestedTablename);
            ClearHighlight(new RowSpan(2));
            VerifyTableAnnotation(master,
                new ITableSegment[] { new TableStart(-37, 1), new TableEnd(3, 36) },
                new ITableSegment[] { new TableStart(-37, -1), new RowSpan(1), new CellSpan(3,1, 4,2), new TableEnd(-1, 37) }
                );
            passTest("Verified clearing row within inner table.");
        }

        #endregion
    }
}	

