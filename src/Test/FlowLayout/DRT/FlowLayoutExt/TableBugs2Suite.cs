// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Misc Table bugs regression control
//
//

using System;
using System.Threading;

using System.Collections;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Markup;

namespace DRT
{
    // 
    // Table DRT's.
    // 
    internal sealed class TableBugs2Suite : FlowLayoutExtSuite
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        internal TableBugs2Suite() : base("TableBugs2")
        {
            this.Contact = "Microsoft";
        }

        /// <summary>
        /// Creates array of callbacks
        /// </summary>
        /// <returns>Array of callbacks</returns>
        protected override DrtTest[] CreateTests()
        {

            return new DrtTest[] {
                new DrtTest(Root),                      new DrtTest(VerifyLayoutCreate),
                new DrtTest(Regression_Bug14),                 new DrtTest(VerifyLayoutAppend),
                new DrtTest(Regression_Bug14_DynamicChange),   new DrtTest(VerifyLayoutAppend),
                new DrtTest(Regression_Bug15),                 new DrtTest(VerifyLayoutAppend),
                new DrtTest(Regression_Bug15_DynamicChange),   new DrtTest(VerifyLayoutAppend),
                new DrtTest(Regression_Bug16),                 new DrtTest(VerifyLayoutAppend),
                new DrtTest(Regression_Bug16_DynamicChange),   new DrtTest(VerifyLayoutAppend),
                new DrtTest(Regression_Bug17),                 new DrtTest(VerifyLayoutAppend),
                new DrtTest(Regression_Bug17_DynamicChange),   new DrtTest(VerifyLayoutFinalize),
            };

        }

        private void Root()
        {
            _root = new Border();
            _contentRoot.Child = _root;
        }


        //
        //  Regression_Bug21 "The cell with auto column width is not placed correctly when colspan is changed dynamically."
        //  
        internal void Regression_Bug14()
        {
            FlowDocumentScrollViewer fdsv = new FlowDocumentScrollViewer();
            fdsv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            fdsv.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            fdsv.Document = new FlowDocument();
            fdsv.Document.TextAlignment = TextAlignment.Left;
            fdsv.Document.PagePadding = new Thickness(0);
            Table table = new Table();

            fdsv.Width = 750;
            fdsv.Height = 500;
            table.Background = Brushes.White;

            CreateColumn(table, 100, Brushes.LightBlue);
            CreateColumn(table, 100, Brushes.LightPink);
            CreateColumn(table, Brushes.Yellow);
            CreateColumn(table, 100, Brushes.LightGreen);
            CreateColumn(table, 100, Brushes.LightGray);

            TableRowGroup body = new TableRowGroup();

            table.RowGroups.Add(body);

            TableRow row;

            row = CreateRow(body);
            CreateCells(row, 2, 1, "1");
            CreateCells(row, 1, 1, "2");
            CreateCells(row, 1, 1, "3");
            CreateCells(row, 1, 1, "4");
            CreateCells(row, 1, 1, "5");
            
            row = CreateRow(body);
            CreateCells(row, 1, 1, "1");
            _targetCell = CreateCells(row, 3, 2, "2");
            CreateCells(row, 1, 1, "3");
            
            row = CreateRow(body);
            CreateCells(row, 1, 1, "1");
            CreateCells(row, 1, 1, "2");
            CreateCells(row, 1, 1, "3");
            CreateCells(row, 1, 3, "4");
            CreateCells(row, 1, 1, "5");

            fdsv.Document.Blocks.Add(table);
            _root.Child = fdsv;
        }

        internal void Regression_Bug14_DynamicChange()
        {
            _targetCell.ColumnSpan = _targetCell.ColumnSpan + 1;
            _targetCell = null;
        }

        //
        //  Bug Regression_Bug15 "App crashes when the colspan on the cell with auto column width is removed."
        //  
        internal void Regression_Bug15()
        {
            FlowDocumentScrollViewer fdsv = new FlowDocumentScrollViewer();
            fdsv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            fdsv.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            fdsv.Document = new FlowDocument();
            fdsv.Document.TextAlignment = TextAlignment.Left;
            fdsv.Document.PagePadding = new Thickness(0);
            Table table = new Table();

            fdsv.Width = 750;
            fdsv.Height = 500;
            table.Background = Brushes.White;

            CreateColumn(table, 100, Brushes.LightBlue);
            CreateColumn(table, 100, Brushes.LightPink);
            CreateColumn(table, Brushes.Yellow);
            CreateColumn(table, 100, Brushes.LightGreen);
            CreateColumn(table, 100, Brushes.LightGray);

            TableRowGroup body = new TableRowGroup();

            table.RowGroups.Add(body);

            TableRow row;

            row = CreateRow(body);
            CreateCells(row, 2, 1, "1");
            CreateCells(row, 1, 1, "2");
            CreateCells(row, 1, 1, "3");
            CreateCells(row, 1, 1, "4");
            CreateCells(row, 1, 1, "5");
            
            row = CreateRow(body);
            CreateCells(row, 1, 1, "1");
            CreateCells(row, 3, 2, "2");
            CreateCells(row, 1, 1, "3");
            
            row = CreateRow(body);
            CreateCells(row, 1, 1, "1");
            CreateCells(row, 1, 1, "2");
            CreateCells(row, 1, 1, "3");
            CreateCells(row, 1, 3, "4");
            CreateCells(row, 1, 1, "5");
            
            
            row = CreateRow(body);
            CreateCells(row, 1, 1, "1");
            CreateCells(row, 1, 1, "2");
            _targetCell = CreateCells(row, 2, 1, "3");
            CreateCells(row, 1, 1, "4");
            CreateCells(row, 1, 1, "5");

            fdsv.Document.Blocks.Add(table);
            _root.Child = fdsv;
        }

        internal void Regression_Bug15_DynamicChange()
        {
            _targetCell.ColumnSpan = _targetCell.ColumnSpan - 1;
            _targetCell = null;
        }

        //
        //  Regression_Bug19 "App crashes when rowspan is incremented on the cell ."
        //  
        internal void Regression_Bug16()
        {
            FlowDocumentScrollViewer fdsv = new FlowDocumentScrollViewer();
            fdsv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            fdsv.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            fdsv.Document = new FlowDocument();
            fdsv.Document.TextAlignment = TextAlignment.Left;
            fdsv.Document.PagePadding = new Thickness(0);
            Table table = new Table();

            fdsv.Width = 750;
            fdsv.Height = 500;
            table.Background = Brushes.White;

            CreateColumn(table, 100, Brushes.LightBlue);
            CreateColumn(table, 100, Brushes.LightPink);
            CreateColumn(table, 100, Brushes.Yellow);
            CreateColumn(table, 100, Brushes.LightGreen);
            CreateColumn(table, 100, Brushes.LightGray);

            TableRowGroup body = new TableRowGroup();

            table.RowGroups.Add(body);

            TableRow row;

            
            row = CreateRow(body);
            CreateCells(row, 1, 1, "1");
            CreateCells(row, 3, 2,"2");

            row = CreateRow(body);
            CreateCells(row, 1, 1, "1");
            CreateCells(row, 1, 1, "2");
            CreateCells(row, 1, 1, "3");
            _targetCell = CreateCells(row, 1, 1, "4");
            CreateCells(row, 1, 1, "5");

            fdsv.Document.Blocks.Add(table);
            _root.Child = fdsv;
        }

        internal void Regression_Bug16_DynamicChange()
        {
            _targetCell.RowSpan = _targetCell.RowSpan + 2;
            _targetCell = null;
        }

        //
        //  Regression_Bug20 "The cells in the following rows don't remain in their position when new cells are added to the intended row."
        //  
        internal void Regression_Bug17()
        {
            FlowDocumentScrollViewer fdsv = new FlowDocumentScrollViewer();
            fdsv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            fdsv.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            fdsv.Document = new FlowDocument();
            fdsv.Document.TextAlignment = TextAlignment.Left;
            fdsv.Document.PagePadding = new Thickness(0);
            Table table = new Table();

            fdsv.Width = 750;
            fdsv.Height = 500;
            table.Background = Brushes.White;

            CreateColumn(table, 100, Brushes.LightBlue);
            CreateColumn(table, 100, Brushes.LightPink);
            CreateColumn(table, Brushes.Yellow);
            CreateColumn(table, 100, Brushes.LightGreen);
            CreateColumn(table, 100, Brushes.LightGray);

            TableRowGroup body = new TableRowGroup();

            table.RowGroups.Add(body);

            _targetRow = CreateRow(body);
            CreateCells(_targetRow, 1, 1, "1");
            CreateCells(_targetRow, 1, 1, "2");
            CreateCells(_targetRow, 1, 1, "3");
            CreateCells(_targetRow, 1, 1, "4");
            CreateCells(_targetRow, 1, 1, "5");

            TableRow row;
            row = CreateRow(body);
            CreateCells(row, 1, 1, "1");
            CreateCells(row, 1, 1, "2");
            CreateCells(row, 1, 1, "3");

            row = CreateRow(body);
            CreateCells(row, 1, 1, "1");
            CreateCells(row, 1, 1, "2");
            CreateCells(row, 1, 1, "3");
            CreateCells(row, 1, 1, "4");
            CreateCells(row, 1, 1, "5");

            fdsv.Document.Blocks.Add(table);
            _root.Child = fdsv;
        }

        internal void Regression_Bug17_DynamicChange()
        {
            TableCell cell = new TableCell(new Paragraph(new Run()));
            cell.Background = Brushes.Red;
            cell.BorderBrush = Brushes.Black;
            cell.BorderThickness = new Thickness(1);
            _targetRow.Cells.Insert(3, cell);
            _targetRow = null;
        }

        private TableColumn CreateColumn(Table table, Brush background)
        {
            TableColumn col = new TableColumn();

            table.Columns.Add(col);
            col.Background = background;
            return (col);
        }

        private TableColumn CreateColumn(Table table, double width, Brush background)
        {
            TableColumn col = new TableColumn();

            table.Columns.Add(col);
            col.Width = new GridLength(width);
            col.Background = background;
            return (col);
        }

        private TableRow CreateRow(TableRowGroup rowgroup)
        {
            TableRow row = new TableRow();

            rowgroup.Rows.Add(row);
            return (row);
        }

        private TableCell CreateCells(TableRow row, int colspan, int rowspan, string text)
        {
            TableCell cell = new TableCell(new Paragraph(new Run("[" + text + "]")));
            cell.ColumnSpan = colspan;
            cell.RowSpan = rowspan;
            cell.BorderBrush = Brushes.Black;
            cell.BorderThickness = new Thickness(1);

            row.Cells.Add(cell);
            return (cell);
        }

        private Border _root;
        private TableCell _targetCell;
        private TableRow _targetRow;
    }
}
