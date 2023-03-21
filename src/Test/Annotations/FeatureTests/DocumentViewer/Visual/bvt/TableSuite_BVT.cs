// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 

using System;
using System.Windows;
using System.Drawing;
using Annotations.Test;
using Annotations.Test.Framework;
using System.Windows.Documents;
using System.Collections.Generic;
using Proxies.System.Windows.Annotations;
using Proxies.MS.Internal.Annotations;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using System.Windows.Media;

using Size = System.Windows.Size;
using Color = System.Windows.Media.Color;

namespace Avalon.Test.Annotations.BVTs
{
    [TestDimension("stickynote,highlight")]
	public class TableSuite_BVT : ATableSuite
	{
        #region Automation Tests

        /// <summary>
        /// Test the tests, verify that selection definitions work as expected.
        /// </summary>
        [OverrideClassTestDimensions()]
        protected void table_simple_verify1()
        {
            // Test row selection.
            AssertEquals("Verify select 1st row.", "Name\tAge\tOccupation\tId\tSex\tHeight\r\n", GetText(new RowSpan(1)));
            AssertEquals("Verify select last row.", "Suzanne\t23\tEngineer\t1923\tFemale\t6'1\r\n", GetText(new RowSpan(4)));
            AssertEquals("Verify select multiple rows.", "Name\tAge\tOccupation\tId\tSex\tHeight\r\nJohn\t17\tStudent\t444\tMalee\t5'10\r\n", GetText(new CellSpan(1, 1, 2, 6)));
            // Test column selection.
            AssertEquals("Verify single column.", "Age\t17\t35\t23\t", GetText(new CellSpan(1, 2, 4, 2)));
            AssertEquals("Verify multiple columns.", "Id\tSex\tHeight\r\n444\tMalee\t5'10\r\n918\tFemale\tTall\r\n1923\tFemale\t6'1\r\n", GetText(new CellSpan(1, 4, 4, 6)));
            // Test cell selection.
            AssertEquals("Verify first cell.", "Name\t", GetText(new CellSpan(1, 1)));
            AssertEquals("Verify middle cell.", "Student\t", GetText(new CellSpan(2, 3)));
            AssertEquals("Verify last cell.", "6'1\t", GetText(new CellSpan(4, 6)));
            AssertEquals("Verify multiple cells.", "35\tDoctor\t918	", GetText(new CellSpan(3, 2, 3, 4)));
            // Test sub-cell selection.
            AssertEquals("Verify sub-cell selection.", "am", GetText(new CellSubset(1, 1, PagePosition.Beginning, 2, PagePosition.End, -2)));
            AssertEquals("Verify start of cell selection.", "Na", GetText(new CellStart(1, 1, 3)));
            AssertEquals("Verify end of cell selection.", "me\t", GetText(new CellEnd(1, 1, -3)));
            // Test table selection.
            string tableText = "Name\tAge\tOccupation\tId\tSex\tHeight\r\n" +
                                "John\t17\tStudent\t444\tMalee\t5'10\r\n" +
                                "Jane\t35\tDoctor\t918\tFemale\tTall\r\n" +
                                "Suzanne\t23\tEngineer\t1923\tFemale\t6'1\r\n";
            AssertEquals("Verify full table selection.", tableText, GetText(new TableSpan()));
            AssertEquals("Verify cross table selection.", "columns and rows are the same size.\r\n" + tableText + "Spanning Table:", GetText(new TableSpan(-36, 16)));
            assertEquals("Verify portion of table and portion of surrounding text 1.",
                "columns and rows are the same size.\r\n" +
                "Name\tAge\tOccupation\tId\tSex\tHeight\r\n" + "John\t17\tStudent\t444\tMalee\t5'10\r\n", // first two rows.
                GetText(new TableStart(-36, 2)));
            assertEquals("Verify portion of table and portion of surrounding text 2.",
               "Jane\t35\tDoctor\t918\tFemale\tTall\r\n" + "Suzanne\t23\tEngineer\t1923\tFemale\t6'1\r\n" // last 2 rows.
               + "Spanning Table:",
               GetText(new TableEnd(3, 16)));

            passTest("Verified that table selection automation works as expected.");
        }

        [OverrideClassTestDimensions()]
        protected void table_simple_testimaging()
        {
            // Test Simple Table.
            GetTableBitmap().Save("Table1.bmp");
            GetTableBitmap(new RowSpan(2)).Save("Row2.bmp");
            GetTableBitmap(new CellSpan(2, 3)).Save("Cell.bmp");
            GetTableBitmap(new CellSpan(1, 3, 4, 3)).Save("Column.bmp");

            // Test Nested Table.
            this.TableSelectionFactory = new TableSelectionFactory(ViewerTestConstants.TableTests.InnerNestedTablename);
            BlackOutSegment(GetBitmapOfView(), new TableSpan()).Save("NestedTable.bmp");

            passTest("Generated all sub-bitmaps");
        }

        [OverrideClassTestDimensions()]
        protected void table_simple_colordiff()
        {
            Bitmap master = GetTableBitmap();
            Color[] colorsToTest = new Color[] { Colors.Yellow, Colors.Red, Colors.Purple, Colors.Blue, Colors.Orange };
            ITableSegment selection = new CellSubset(1, 1, PagePosition.Beginning, 2, PagePosition.Beginning, 3);
            foreach (Color color in colorsToTest)
            {
                printStatus("Testing '" + color + "':");
                CreateHighlight(selection, color);
                CompareBitmaps(master, GetTableBitmap(), false);
            }
            passTest("Verified that 1 character of highlight is detected by imaging comparison.");
        }

        #endregion

        #region Simple Table

        /// <summary>
        /// Annotate whole table.
        /// </summary>
        protected void table_simple2()
        {
            TestTableSelection(new TableSpan());
        }

        /// <summary>
        /// Annotate single cell.
        /// </summary>
        protected void table_simple3()
        {
            TestTableSelection(new SingleCell(2, 2));
        }

        /// <summary>
        /// Annotate first row.
        /// </summary>
        protected void table_simple4()
        {
            TestTableSelection(new RowSpan(1));
        }

        /// <summary>
        /// Annotate single column
        /// </summary>
        protected void table_simple6()
        {
            TestTableSelection(new CellSpan(1, 2, 4, 2));
        }

        /// <summary>
        /// Annotate portion of single cell.
        /// </summary>
        protected void table_simple9()
        {
            TestTableSelection(new CellSubset(2, 3, PagePosition.Beginning, 3, PagePosition.End, -3));
        }

        /// <summary>
        /// Annotate starting before table and ending inside.
        /// </summary>
        protected void table_simple10()
        {
            TestTableSelection(new TableStart(-50, 2), new ITableSegment[] { new TableStart(-50, -1), new CellSpan(1, 1, 2, 6) });
        }

        /// <summary>
        /// Annotate starting inside table and ending after.
        /// </summary>
        protected void table_simple11()
        {
            TestTableSelection(new TableEnd(3, 50), new ITableSegment[] { new CellSpan(3, 1, 4, 6), new TableEnd(-1, 50) });
        }

        /// <summary>
        /// Annotate table spanning page boundary: create annotation before reflow.
        /// </summary>        
        protected void table_simple12_1()
        {
            simpletable12(true, true);
        }
        /// <summary>
        /// Annotate table spanning page boundary: create annotation after reflow, first page is visible.
        /// </summary>
        protected void table_simple12_2()
        {
            simpletable12(false, true);
        }
        /// <summary>
        /// Annotate table spanning page boundary: create annotation after reflow, second page is visible.
        /// </summary>
        protected void table_simple12_3()
        {
            simpletable12(false, false);
        }
        protected void simpletable12(bool createBeforeReflow, bool firstPageVisible)
        {
            TestCrossPageTableSelection(
                200,
                new Size(913, 373),
                new TableSpan(),
                new ITableSegment[] { new CellSpan(1, 1, 2, 6) },
                new ITableSegment[] { new CellSpan(3, 1, 4, 6) },
                createBeforeReflow,
                firstPageVisible);
        }

        /// <summary>
        /// Annotate table spanning page boundary: create annotation after reflow.
        /// </summary>
        protected void table_simple12_4()
        {
            ViewAsTwoPages();
            SetWindowWidth(925);
            SetWindowHeight(451);
            SetZoom(180);
            TestTableSelection(new TableSpan(), new ITableSegment[] { new CellSpan(1, 1, 2, 6), new CellSpan(3, 1, 4, 6) } );
        }        

        /// <summary>
        /// Annotate table, cell split on page boundary.
        /// </summary>
        [OverrideClassTestDimensions()]
        [TestDimension("stickynote")]
        protected void table_simple14()
        {            
            ITableSegment[] firstPage = new ITableSegment[] { new RowSpan(1), new CellSpan(2, 1, 2, 2), new CellStart(2, 3, 5), new CellStart(2, 4, 5), new CellStart(2,5, 5), new CellSpan(2,6, 2,6, true) };
            ITableSegment[] secondPage = new ITableSegment[] { new CellEnd(2, 3, -4, true), new CellEnd(2, 5, -2, true), new RowSpan(3), new RowSpan(4) };
            TestCrossPageTableSelection(
                200, 
                new Size(435, 517),
                new TableSpan(),
                firstPage,
                secondPage,
                true, true);
        }        

        #endregion

        #region Spanning Table

        /// <summary>
        /// Annotate cell with columnspan = 2.
        /// </summary>
        protected void table_spanning1()
        {
            TestTableSelection(new SingleCell(4, 3));
        }

        /// <summary>
        /// Column containing mutli span cell.
        /// </summary>
        protected void table_spanning3()
        {
            TestTableSelection(new CellSpan(2, 3, 5, 3));
        }

        /// <summary>
        /// Annotate top row where selection contains row with RowSpan=2.
        /// </summary>
        protected void table_spanning7()
        {
            TestTableSelection(new RowSpan(6), new ITableSegment[] { new RowSpan(6), new CellSpan(6, 2) });
        }

        /// <summary>
        /// Annotate bottom row where selection contains row with RowSpan=2.
        /// </summary>
        protected void table_spanning8()
        {
            TestTableSelection(new RowSpan(7), new ITableSegment[] { new CellSpan(7, 1), new CellSpan(7,3,7,4) });
        }

        #endregion

        #region Nested Table

        /// <summary>
        /// Annotate whole table.
        /// </summary>
        protected void table_nested1()
        {
            TestTableSelection(new TableSpan());
        }

        /// <summary>
        /// Annotate inner table.
        /// </summary>
        protected void table_nested3()
        {
            this.TableSelectionFactory = new TableSelectionFactory(ViewerTestConstants.TableTests.InnerNestedTablename);
            TestTableSelection(new TableSpan());
        }

        /// <summary>
        /// Annotate cell within inner table.
        /// </summary>
        protected void table_nested4()
        {
            this.TableSelectionFactory = new TableSelectionFactory(ViewerTestConstants.TableTests.InnerNestedTablename);
            TestTableSelection(new SingleCell(2,1));
        }

        /// <summary>
        /// Annotate portion of cell within inner table.
        /// </summary>
        protected void table_nested5()
        {
            this.TableSelectionFactory = new TableSelectionFactory(ViewerTestConstants.TableTests.InnerNestedTablename);
            TestTableSelection(new CellSubset(3, 2, PagePosition.Beginning, 3, PagePosition.End, -3));
        }

        /// <summary>
        /// Annotate inner table split on page boundary. Create before reflow.  
        /// </summary>
        [OverrideClassTestDimensions()]
        [TestDimension("stickynote,highlight")]
        protected void table_nested9_1()
        {
            TestTableNested9(true, true);
        }
        /// <summary>
        /// Annotate inner table split on page boundary. Create after reflow, 1st page visible.
        /// </summary>
        [OverrideClassTestDimensions()]
        [TestDimension("stickynote,highlight")]
        protected void table_nested9_2()
        {
            TestTableNested9(false, true);
        }
        /// <summary>
        /// Annotate inner table split on page boundary. Create after reflow, 2nd page visible.
        /// </summary>
        protected void table_nested9_3()
        {
            TestTableNested9(false, false);
        }

        protected void TestTableNested9(bool createBeforeReflow, bool firstPageVisible)
        {
            this.TableSelectionFactory = new TableSelectionFactory(ViewerTestConstants.TableTests.InnerNestedTablename);
            TestCrossPageTableSelection(
                DocViewerWrapper.GetZoom(),
                new Size(913, 589),
                new TableSpan(),
                new ITableSegment[] { new CellSpan(1, 1, 2, 2) },
                new ITableSegment[] { new CellSpan(3, 1, 4, 2) },
                createBeforeReflow,
                firstPageVisible);
        }

        /// <summary>
        /// Annotate across page boundary.	Select all delete.
        /// </summary>
        /// <param name="createBeforeReflow"></param>
        /// <param name="firstPageVisible"></param>
        protected void table_nested10()
        {
            this.TableSelectionFactory = new TableSelectionFactory(ViewerTestConstants.TableTests.InnerNestedTablename);
            SetWindowWidth(913);
            SetWindowHeight(589);
            CreateAnnotation(TableSelectionFactory.Create(new TableSpan()));
            VerifyAnnotation(GetText(new CellSpan(1, 1, 2, 2)));
            ViewerBase.Focus();
            UIAutomationModule.Ctrl(System.Windows.Input.Key.A);
            DeleteAnnotation();
            VerifyNumberOfAttachedAnnotations(0);
            passTest("Select-all delete");
        }

        #endregion

        #region Embedded Table

        /// <summary>
        /// Annotate image inside table.
        /// </summary>
        protected void table_embedded1()
        {
            PageDown();
            TestTableSelection(new TableSpan(), new ITableSegment[] { new RowSpan(1), new CellSpan(1, 2) });
        }

        #endregion

        #region Highlight Tests

        /// <summary>
        /// Split single cell highlight, 1 annotation with 2 segments.
        /// </summary>
        [OverrideClassTestDimensions()]
        [TestDimension("highlight")]
        protected void table_simple_highlight1()
        {
            Bitmap master = GetBitmapOfView();
            CreateHighlight(new CellSpan(1, 3), Colors.Yellow);
            ClearHighlight(new CellSubset(1, 3, PagePosition.Beginning, 3, PagePosition.End, -3));
            VerifyTableAnnotation(master,
                new ITableSegment[] { 
                    new CellSubset(1, 3, PagePosition.Beginning, 1, PagePosition.Beginning, 3), 
                    new CellSubset(1, 3, PagePosition.End, -3, PagePosition.End, -1) });
            passTest("Verified splitting highlight in cell.");
        }

        /// <summary>
        /// 2 segment green annotation, 1 segment red annotation.
        /// </summary>
        [OverrideClassTestDimensions()]
        [TestDimension("highlight")]
        protected void table_simple_highlight2()
        {
            CreateHighlight(new CellSpan(1, 3), Colors.Green);
            CreateHighlight(new CellSubset(1, 3, PagePosition.Beginning, 3, PagePosition.End, -3), Colors.Red);

            VerifyNumberOfAttachedAnnotations(2);
            VerifyAnnotationWithAnchorExists(GetText( new ITableSegment[] { 
                    new CellSubset(1, 3, PagePosition.Beginning, 1, PagePosition.Beginning, 3), 
                    new CellSubset(1, 3, PagePosition.End, -3, PagePosition.End, -1) }));
            VerifyAnnotationWithAnchorExists(GetText(new CellSubset(1, 3, PagePosition.Beginning, 3, PagePosition.End, -3)));
            passTest("Verified splitting highlight in cell.");
        }

        /// <summary>
        /// Create new highlight over existing single cell highlight.
        /// </summary>
        [OverrideClassTestDimensions()]
        [TestDimension("highlight")]
        protected void table_simple_highlight3_1()
        {
            ITableSegment cell = new SingleCell(2, 1);

            Color c1 = (Color)System.Windows.Media.ColorConverter.ConvertFromString("#54008000");
            Color c2 = (Color)System.Windows.Media.ColorConverter.ConvertFromString("#54FF0000");

            CreateHighlight(cell, c1);
            CreateHighlight(cell, c2);

            VerifyAnnotation(GetText(cell));
            HighlightStateInfo highlight = HighlightStateInfo.FromHighlightComponent(AnnotationComponentFinder.GetVisibleHighlightComponents(ViewerBase)[0]);
            assertEquals("Verify highlight color.", c2, highlight.HighlightBrushColor);
            passTest("Verified highlighting over highlighted cell.");
        }

        /// <summary>
        /// Create new highlight across exisiting single cell highlight.
        /// </summary>
        [OverrideClassTestDimensions()]
        [TestDimension("highlight")]
        protected void table_simple_highlight3_2()
        {
            ITableSegment cell = new CellSpan(2, 1);
            ITableSegment row = new RowSpan(2);

            Color c1 = (Color)System.Windows.Media.ColorConverter.ConvertFromString("#54008000");
            Color c2 = (Color)System.Windows.Media.ColorConverter.ConvertFromString("#54FF0000");

            CreateHighlight(cell, c1);
            CreateHighlight(row, c2);

            VerifyAnnotation(GetText(row));
            HighlightStateInfo highlight = HighlightStateInfo.FromHighlightComponent(AnnotationComponentFinder.GetVisibleHighlightComponents(ViewerBase)[0]);
            assertEquals("Verify highlight color.", c2, highlight.HighlightBrushColor);
            passTest("Verified highlighting over highlighted cell.");
        }

        /// <summary>
        /// Split middle cell of multi-cell highlight.
        /// </summary>
        [OverrideClassTestDimensions()]
        [TestDimension("highlight")]
        protected void table_simple_highlight6()
        {
            Bitmap master = GetBitmapOfView();
            CreateHighlight(new RowSpan(3), Colors.Green);
            ClearHighlight(new CellSubset(3, 3, PagePosition.Beginning, 3, PagePosition.End, -3));
            VerifyTableAnnotation(master,
                new ITableSegment[] { 
                    new CellSpan(3, 1, 3, 2),
                    new CellStart(3, 3, 3), 
                    new CellEnd(3, 3, -3),
                    new CellSpan(3, 4, 3, 6),});
            passTest("Verified splitting middle cell of multi-cell highlight.");
        }

        /// <summary>
        /// Clear middle cell of multi-cell highlight.
        /// </summary>
        [OverrideClassTestDimensions()]
        [TestDimension("highlight")]
        protected void table_simple_highlight7()
        {
            Bitmap master = GetBitmapOfView();
            CreateHighlight(new RowSpan(3), Colors.Green);
            ClearHighlight(new CellSpan(3, 5));
            VerifyTableAnnotation(master,
                new ITableSegment[] { 
                    new CellSpan(3, 1, 3, 4), new CellEnd(3,5,-1),
                    new CellSpan(3, 6, 3, 6, true) });
            passTest("Verified clearing middle cell of multi-cell highlight.");
        }

        /// <summary>
        /// Clear middle cell of highlighted block.
        /// </summary>
        [OverrideClassTestDimensions()]
        [TestDimension("highlight")]
        protected void table_simple_highlight8_1()
        {
            Bitmap master = GetBitmapOfView();
            CreateHighlight(new CellSpan(1, 1, 3, 4), Colors.Green);
            ClearHighlight(new CellSpan(2, 2));
            VerifyTableAnnotation(master,
                new ITableSegment[] { 
                    new CellSpan(1,1, 1,4),
                    new CellSpan(2, 1), new CellEnd(2, 2, -1), new CellSpan(2, 3, 2, 4),
                    new CellSpan(3,1, 3,4) });
            passTest("Verified clearing middle cell of multi-cell highlight.");
        }

        /// <summary>
        /// Split middle cell of highlighted block.
        /// </summary>
        [OverrideClassTestDimensions()]
        [TestDimension("highlight")]
        protected void table_simple_highlight8_5()
        {
            Bitmap master = GetBitmapOfView();
            CreateHighlight(new CellSpan(2, 1, 4, 4), Colors.Green);
            ClearHighlight(new CellSubset(3, 3, PagePosition.Beginning, 5, PagePosition.End, -2));
            VerifyTableAnnotation(master,
                new ITableSegment[] { 
                    new CellSpan(2, 1, 2, 4),
                    new CellSpan(3, 1, 3, 2), new CellStart(3, 3, 5), new CellEnd(3, 3, -2), new CellSpan(3, 4),
                    new CellSpan(4, 1, 4, 4) });
            passTest("Verified splitting middle cell of multi-cell highlight.");
        }

        /// <summary>
        /// Different columns are different colors, unhighlight row.
        /// </summary>
        [OverrideClassTestDimensions()]
        [TestDimension("highlight")]
        protected void table_simple_highlight16_1()
        {
            Bitmap master = GetBitmapOfView();
            CreateHighlight(new CellSpan(1, 1, 4, 2), Colors.Green);
            CreateHighlight(new CellSpan(1, 3, 4, 3), Colors.Red);
            CreateHighlight(new CellSpan(1, 4, 4, 5), Colors.Blue);
            CreateHighlight(new CellSpan(1, 6, 4, 6), Colors.Yellow);            
            ClearHighlight(new RowSpan(3));

            VerifyNumberOfAttachedAnnotations(4);
            VerifyAnnotationWithAnchorExists(GetText(new ITableSegment[] { new CellSpan(1, 1, 2, 2), new CellSpan(4, 1, 4, 2) }));
            VerifyAnnotationWithAnchorExists(GetText(new ITableSegment[] { new CellSpan(1, 3, 2, 3), new CellSpan(4, 3) }));
            VerifyAnnotationWithAnchorExists(GetText(new ITableSegment[] { new CellSpan(1, 4, 2, 5), new CellSpan(4, 4, 4, 5) }));
            VerifyAnnotationWithAnchorExists(GetText(new ITableSegment[] { new CellSpan(1, 6, 2, 6), new CellSpan(4, 6, 4, 6, true) }));
            VerifyThatExpectRegionWasUnchanged(master, new ITableSegment[] { new CellSpan(1,1,2,6), new RowSpan(4), /* Work around for text rendering issue */ new CellSubset(3,6,PagePosition.Beginning, 1, PagePosition.End, -1) });
            passTest("Verified clearing row across multi-color columns.");
        }

        /// <summary>
        /// Each row is a different color, clear column.
        /// </summary>
        [OverrideClassTestDimensions()]
        [TestDimension("highlight")]
        protected void table_simple_highlight17_1()
        {
            Bitmap master = GetBitmapOfView();
            CreateHighlight(new RowSpan(1), Colors.Green);
            CreateHighlight(new RowSpan(2), Colors.Red);
            CreateHighlight(new RowSpan(3), Colors.Blue);
            CreateHighlight(new RowSpan(4), Colors.Yellow);            
            ClearHighlight(new CellSpan(1, 2, 4, 2)); // Column 2.

            VerifyNumberOfAttachedAnnotations(4);
            VerifyAnnotationWithAnchorExists(GetText(new ITableSegment[] { new CellSpan(1, 1), new CellSpan(1, 3, 1, 6) }));
            VerifyAnnotationWithAnchorExists(GetText(new ITableSegment[] { new CellSpan(2, 1), new CellSpan(2, 3, 2, 6) }));
            VerifyAnnotationWithAnchorExists(GetText(new ITableSegment[] { new CellSpan(3, 1), new CellSpan(3, 3, 3, 6) }));
            VerifyAnnotationWithAnchorExists(GetText(new ITableSegment[] { new CellSpan(4, 1), new CellSpan(4, 3, 4, 6) }));
            VerifyThatExpectRegionWasUnchanged(master, new ITableSegment[] { new CellSpan(1, 1, 4, 1), new CellSpan(1,3,4,6) });
            passTest("Verified clearing column across multi-color rows.");
        }

        /// <summary>
        /// Delete middle cell of column highlight
        /// </summary>
        [OverrideClassTestDimensions()]
        [TestDimension("highlight")]
        protected void table_simple_highlight21()
        {
            Bitmap master = GetBitmapOfView();
            CreateHighlight(new CellSpan(1, 2, 3, 2), Colors.Green);            
            ClearHighlight(new CellSpan(2, 2));
            VerifyTableAnnotation(master, new ITableSegment[] { new CellSpan(1, 2), new CellEnd(2, 2, -1), new CellSpan(3, 2) });
            passTest("Verified clearing middle cell of column.");
        }

        /// <summary>
        /// Single highlight split across page boundary.  Clear portion of highlight.
        /// </summary>
        [OverrideClassTestDimensions()]
        [TestDimension("highlight")]
        protected void table_simple_highlight22()
        {         
            ReflowDocument(200, new Size(913, 370));
            Bitmap masterA, masterB;
            CreateCrosspageMasters(out masterA, out masterB);
            CreateHighlight(new CellSpan(1, 2, 4, 3), Colors.Green); // Columns 1 and 2.                       
            ClearHighlight(new CellSpan(2, 2, 3, 3));
            VerifyCrosspageAnnotation(masterA, new ITableSegment[] { new CellSpan(1,2,1,3) }, masterB, new ITableSegment[] { new CellSpan(4,2,4,3) });
            passTest("Verified clearing middle of cross page highlight.");
        }

        /// <summary>
        /// Split highlight in cell that is split across page boundary.
        /// </summary>
        [OverrideClassTestDimensions()]
        [TestDimension("highlight")]
        protected void table_simple_highlight23()
        {
            ReflowDocument(200, new Size(450, 517));
            Bitmap masterA, masterB;
            CreateCrosspageMasters(out masterA, out masterB);
            CreateHighlight(new CellSpan(2, 3), Colors.Purple);
            ClearHighlight(new CellSubset(2, 3, PagePosition.Beginning, 3, PagePosition.End, -2));
            VerifyCrosspageAnnotation(
                masterA, new ITableSegment[] { new CellStart(2, 3, 3) },
                masterB, new ITableSegment[] { new CellSubset(2, 3, PagePosition.End, -1, PagePosition.End, -2) }
            );
            passTest("Verified splitting highlight in cell across page boundary.");
        }

        /// <summary>
        /// Split single table annotation multiple times.
        /// </summary>
        [OverrideClassTestDimensions()]
        [TestDimension("highlight")]
        protected void table_simple_highlight24()
        {
            Bitmap master = GetBitmapOfView();

            CreateHighlight(new TableSpan(), Colors.Green);
            ClearHighlight(new CellStart(1, 1, 3));
            ClearHighlight(new CellSpan(2, 4));
            ClearHighlight(new CellSubset(3, 5, PagePosition.Beginning, 2, PagePosition.End, -2));
            ClearHighlight(new CellSpan(3, 1, 3, 2));
            ClearHighlight(new CellEnd(4, 3, -6));

            VerifyTableAnnotation(master, new ITableSegment[] { 
                new CellEnd(1,1,-3), new CellSpan(1,2, 1,6),
                new CellSpan(2,1, 2,3), new CellEnd(2, 4, -1), new CellSpan(2,5, 2,6),
                new CellSpan(3,3, 3,4), new CellStart(3,5,2), new CellEnd(3,5,-2), new CellSpan(3,6, 3, 6, true),
                new CellSpan(4,1, 4,2), new CellStart(4,3,4), new CellEnd(4,3,-1), new CellSpan(4,4, 4,6)
            });
            
            passTest("Verified splitting highlight in cell across page boundary.");
        }

        /// <summary>
        /// Annotate inner table.
        /// </summary>
        [OverrideClassTestDimensions()]
        [TestDimension("highlight")]
        protected void table_nested_highlight5()
        {
            this.TableSelectionFactory = new TableSelectionFactory(ViewerTestConstants.TableTests.InnerNestedTablename);
            Bitmap masterA = GetBitmapOfView();
            CreateHighlight(new TableSpan(), Colors.Red);
            Bitmap masterB = GetBitmapOfView();
            ITableSegment segementToClear = new CellSubset(2, 1, PagePosition.Beginning, 2, PagePosition.End, -3);
            ClearHighlight(segementToClear);

            ITableSegment [] expectedAnchor = new ITableSegment[] { 
                new RowSpan(1),
                new CellStart(2,1,2), new CellEnd(2,1,-3), new CellSpan(2,2, 2, 2, true),
                new CellSpan(3,1, 4,2)
            };
            VerifyTableAnnotation(masterA, expectedAnchor); // Verify unhighlighted region.
            VerifyThatExpectRegionWasUnchanged(masterB, new ITableSegment[] { segementToClear }); // Verify highlighted region.
            passTest("Verified splitting highlight within nested table.");
        }

        #endregion
    }
}	

