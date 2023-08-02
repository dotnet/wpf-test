// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Basic Table DRT
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
    internal sealed class TableOMSuite : FlowLayoutExtSuite
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        internal TableOMSuite() : base("TableOM")
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
                new DrtTest(Root),
                new DrtTest(VerifyLayoutCreateAndFinalize),
            };
        }

        /// <summary>
        /// Creates DRT's tree
        /// </summary>
        /// <returns>Root of the tree</returns>
        internal void Root()
        {
            TableRow row;
            TableCell cell1, cell2, cell3;

            Border border = new Border();
            border.Background = Brushes.White;

            FlowDocumentScrollViewer fdsv = new FlowDocumentScrollViewer();
            fdsv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            fdsv.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            fdsv.Document = new FlowDocument();
            fdsv.Document.TextAlignment = TextAlignment.Left;
            fdsv.Document.PagePadding = new Thickness(0);
            Table table = _root = new Table();
            table.CellSpacing = 3;

            CreateColumn(table, Brushes.Pink);
            CreateColumn(table, Brushes.LightGreen);
            CreateColumn(table, Brushes.LightBlue);

            //
            //  BODY
            //
            TableRowGroup body = new TableRowGroup();
            table.RowGroups.Add(body);

            row = CreateRow(body);
            table.RowGroups.Clear();                      //  <- should not crash
            table.RowGroups.Add(body);

            cell1 = CreateCell(row, "Body 1");
            cell2 = CreateCell(row, "Body 2");
            cell3 = CreateCell(row, "Body 3");

            table.RowGroups[0].Rows.Clear();                //  <- should not crash
            body.Rows.Add(row);

            table.RowGroups[0].Rows[0].Cells.Clear();       //  <- should not crash
            row.Cells.Add(cell1); 
            row.Cells.Add(cell2); 
            row.Cells.Add(cell3);

            fdsv.Document.Blocks.Add(table);
            border.Child = fdsv;

            _contentRoot.Child = border;
        }

        private TableRow CreateRow(TableRowGroup rowGroup)
        {
            TableRow row = new TableRow();
            rowGroup.Rows.Add(row);
            return (row);
        }

        private TableCell CreateCell(TableRow row, string text)
        {
            TableCell cell = new TableCell(new Paragraph(new Run(text)));
            cell.BorderThickness = new Thickness(1);
            cell.Padding = new Thickness(4);
            cell.BorderBrush = Brushes.Black;
            row.Cells.Add(cell);
            return (cell);
        }

        private TableColumn CreateColumn(Table table, Brush background)
        {
            TableColumn column = new TableColumn();
            column.Background = background;
            table.Columns.Add(column);
            return (column);
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

        private Table _root;
    }
}
