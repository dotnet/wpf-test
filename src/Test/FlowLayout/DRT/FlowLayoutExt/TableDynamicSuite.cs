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
    internal sealed class TableDynamicSuite : FlowLayoutExtSuite
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        internal TableDynamicSuite() : base("TableDynamicSuite")
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

                new DrtTest(RemoveFirstBodyRow),  
                new DrtTest(InsertFirstBodyRow),  
                new DrtTest(RemoveLastBodyRow),  
                new DrtTest(InsertLastBodyRow),  
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
            table.CellSpacing = 3;

            table.RowGroups.Add(new TableRowGroup());
            
            for (int i = 0; i < 5; ++i)
            {
                CreateRow(table.RowGroups[0], Brushes.Wheat, 5);
            }

            fdsv.Document.Blocks.Add(table);
            border.Child = fdsv;
            _contentRoot.Child = border;
        }

        private TableRow CreateRow(TableRowGroup rowGroup, Brush background, int cCells)
        {
            TableRow row = new TableRow();
            rowGroup.Rows.Add(row);

            row.Background = background;

            string rowGroupName = "Body";

            for (int i = 1; i <= cCells; ++i)
            {
                CreateCell(row, rowGroupName + " : " + rowGroup.Rows.Count + " : " + i);
            }
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

        internal void RemoveFirstBodyRow()  { _bodyRow = _root.RowGroups[0].Rows[0]; _root.RowGroups[0].Rows.Remove(_bodyRow); }
        internal void RemoveLastBodyRow()  { _bodyRow = _root.RowGroups[0].Rows[_root.RowGroups[0].Rows.Count - 1]; _root.RowGroups[0].Rows.Remove(_bodyRow); }
        internal void InsertFirstBodyRow()  { _root.RowGroups[0].Rows.Insert(0, _bodyRow); }
        internal void InsertLastBodyRow()  { _root.RowGroups[0].Rows.Insert(_root.RowGroups[0].Rows.Count, _bodyRow); }

        private Table _root;
        private TableRow _bodyRow;
    }
}
