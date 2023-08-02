// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Columns Table DRT
//
//

using System;
using System.Threading;

using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace DRT
{
    // 
    // Table DRT's.
    // 
    internal sealed class TableColumnsSuite : FlowLayoutExtSuite
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        internal TableColumnsSuite() : base("TableColumns")
        {
            this.Contact = "Microsoft";
        }

        /// <summary>
        /// Creates array of callbacks
        /// </summary>
        /// <returns>Array of callbacks</returns>
        protected override DrtTest[] CreateTests()
        {

            return new DrtTest[]                
            {
                new DrtTest(Root),  new DrtTest(VerifyLayoutCreate),
                new DrtTest(AddColumn),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(AddColumn),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(ResetColumn),  new DrtTest(VerifyLayoutFinalize),
            };
        }

        /// <summary>
        /// Creates DRT's tree
        /// </summary>
        /// <returns>Root of the tree</returns>
        internal void Root()
        {
            Border border = new Border();
            border.Background = Brushes.White;

            FlowDocumentScrollViewer fdsv = new FlowDocumentScrollViewer();
            fdsv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            fdsv.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            fdsv.Document = new FlowDocument();
            fdsv.Document.TextAlignment = TextAlignment.Left;
            fdsv.Document.PagePadding = new Thickness(0);
            Table table = _root = new Table();

            CreateColumn(table, 140);

            TableRowGroup body = new TableRowGroup();
            table.RowGroups.Add(body);

            TableRow row;

            row = CreateRow(body);
            CreateCell(row, 1, 1);
            CreateCell(row, 1, 1);
            CreateCell(row, 1, 1);
            CreateCell(row, 1, 1);

            row = CreateRow(body);
            CreateCell(row, 1, 1);
            CreateCell(row, 2, 1);
            CreateCell(row, 1, 1);
            
            row = CreateRow(body);
            CreateCell(row, 1, 1);
            CreateCell(row, 1, 2);
            CreateCell(row, 1, 1);
            CreateCell(row, 1, 1);

            row = CreateRow(body);
            CreateCell(row, 1, 1);
            CreateCell(row, 2, 1);


            fdsv.Document.Blocks.Add(table);
            border.Child = fdsv;
            _contentRoot.Child = border;
        }

        private TableColumn CreateColumn(Table table, double width)
        { 
            TableColumn column = new TableColumn();
            table.Columns.Add(column);
            column.Width = new GridLength(width);
            return (column);
        }

        private TableRow CreateRow(TableRowGroup body)
        {
            TableRow row = new TableRow();
            body.Rows.Add(row);
            return (row);
        }

        private TableCell CreateCell(TableRow row, int columnSpan, int rowSpan)
        {
            TableCell cell = new TableCell(new Paragraph(new Run(
                    "Cell # " + _cellCount.ToString() + "; "
                +   "Column Span : " + columnSpan.ToString() + "; " 
                +   "Row Span : " + rowSpan.ToString() + "; " 
                +   " * * * * * * * * * * * * * * * * * * * * *"
                +   " * * * * * * * * * * * * * * * * * * * * *"
                +   " * * * * * * * * * * * * * * * * * * * * *"
            )));
            cell.ColumnSpan = columnSpan;
            row.Cells.Add(cell);
            _cellCount++;
            cell.RowSpan = rowSpan;
            cell.BorderThickness = new Thickness(1);
            cell.Padding = new Thickness(4);
            cell.BorderBrush = Brushes.Black;

            switch ((columnSpan + rowSpan) % 4)
            {
                case (0):
                    cell.Background = Brushes.Wheat;
                    break;
                case (1):
                    cell.Background = Brushes.Pink;
                    break;
                case (2):
                    cell.Background = Brushes.LightGreen;
                    break;
                case (3):
                    cell.Background = Brushes.LightBlue;
                    break;
            }
            return (cell);
        }

        private void SanityCheck()
        {
            foreach(TableRow r in _root.RowGroups[0].Rows)
            {
                if (r == null)
                    throw new Exception("Unexpected null row reference detected");

                foreach(TableCell c in r.Cells)
                {
                    if (c == null)
                        throw new Exception("Unexpected null cell reference detected");
                }
            }

            for (int i = _root.RowGroups[0].Rows.Count - 1; i >= 0; --i)
            {
                TableRow r = _root.RowGroups[0].Rows[i];
                if (r == null)
                    throw new Exception("Unexpected null row reference detected");

                for (int j = _root.RowGroups[0].Rows[i].Cells.Count - 1; j >= 0; --j)
                {
                    TableCell c = _root.RowGroups[0].Rows[i].Cells[j];
                    if (c == null)
                        throw new Exception("Unexpected null cell reference detected");
                }
            }
        }
        
        internal void AddColumn()  { CreateColumn(_root, 100); }
        internal void ResetColumn()  { _root.Columns[0].Width = new GridLength(1.0, GridUnitType.Auto); }

        private Table _root;
        private int _cellCount = 1;
    }
}
