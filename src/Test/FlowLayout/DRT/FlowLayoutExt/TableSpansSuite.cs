// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Spanned cells Table DRT
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
    internal sealed class TableSpansSuite : FlowLayoutExtSuite
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        internal TableSpansSuite() : base("TableSpans")
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
                new DrtTest(Root),             new DrtTest(VerifyLayoutCreate),
                new DrtTest(IncRowSpan91),     new DrtTest(VerifyLayoutAppend),
                new DrtTest(DecRowSpan91),     new DrtTest(VerifyLayoutAppend),
                new DrtTest(IncColumnSpan11),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(IncColumnSpan11),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(IncColumnSpan11),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(DecColumnSpan11),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(DecColumnSpan11),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(DecColumnSpan12),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(IncColumnSpan12),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(IncColumnSpan22),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(DecColumnSpan22),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(IncColumnSpan93),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(IncColumnSpan93),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(DecColumnSpan93),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(DecColumnSpan93),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(IncRowSpan11),     new DrtTest(VerifyLayoutAppend),
                new DrtTest(IncRowSpan11),     new DrtTest(VerifyLayoutAppend),
                new DrtTest(IncRowSpan11),     new DrtTest(VerifyLayoutAppend),
                new DrtTest(DecRowSpan11),     new DrtTest(VerifyLayoutAppend),
                new DrtTest(IncRowSpan22),     new DrtTest(VerifyLayoutAppend),
                new DrtTest(IncRowSpan22),     new DrtTest(VerifyLayoutAppend),
                new DrtTest(IncRowSpan22),     new DrtTest(VerifyLayoutAppend),
                new DrtTest(IncRowSpan22),     new DrtTest(VerifyLayoutAppend),
                new DrtTest(DecRowSpan22),     new DrtTest(VerifyLayoutAppend),
                new DrtTest(DecRowSpan22),     new DrtTest(VerifyLayoutAppend),
                new DrtTest(IncRowSpan21),     new DrtTest(VerifyLayoutAppend),
                new DrtTest(IncRowSpan21),     new DrtTest(VerifyLayoutAppend),
                new DrtTest(IncRowSpan21),     new DrtTest(VerifyLayoutAppend),
                new DrtTest(IncRowSpan21),     new DrtTest(VerifyLayoutAppend),
                new DrtTest(IncRowSpan21),     new DrtTest(VerifyLayoutAppend),
                new DrtTest(IncRowSpan21),     new DrtTest(VerifyLayoutAppend),
                new DrtTest(IncColumnSpan22),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(DecColumnSpan22),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(DecRowSpan11),     new DrtTest(VerifyLayoutAppend),
                new DrtTest(DecRowSpan11),     new DrtTest(VerifyLayoutFinalize),
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

            TableRowGroup rs = new TableRowGroup();
            table.RowGroups.Add(rs);
            
            TableRow row;

            row = CreateRow(rs);
            _cell11 =
            CreateCell(row, 1, 1);
            _cell12 = 
            CreateCell(row, 2, 1);
            CreateCell(row, 1, 1);

            row = CreateRow(rs);

            _cell21 = 
            CreateCell(row, 1, 1);
            _cell22 = 
            CreateCell(row, 1, 2);
            CreateCell(row, 1, 1);
            CreateCell(row, 1, 1);
            _cell25 = 
            CreateCell(row, 1, 1);

            row = CreateRow(rs);

            CreateCell(row, 1, 1);
            CreateCell(row, 1, 1);
            CreateCell(row, 1, 1);

            row = CreateRow(rs);

            CreateCell(row, 1, 1);
            CreateCell(row, 1, 1);
            CreateCell(row, 1, 1);

            row = CreateRow(rs);
            
            CreateCell(row, 1, 1);
            CreateCell(row, 1, 1);
            CreateCell(row, 1, 1);
            
            row = CreateRow(rs);

            CreateCell(row, 1, 1);
            CreateCell(row, 1, 1);
            CreateCell(row, 1, 1);

            row = CreateRow(rs);
            
            CreateCell(row, 1, 1);
            CreateCell(row, 1, 1);
            CreateCell(row, 1, 1);
            CreateCell(row, 1, 1);
            CreateCell(row, 1, 1);
            CreateCell(row, 1, 1);

            row = CreateRow (rs);

            _cell91 = 
            CreateCell(row, 1, 1);
            CreateCell(row, 1, 1);
            _cell93 = 
            CreateCell(row, 1, 1);


            fdsv.Document.Blocks.Add(table);
            border.Child = fdsv;
            _contentRoot.Child = border;
        }

        private TableRow CreateRow(TableRowGroup rs)
        {
            TableRow row = new TableRow();
            rs.Rows.Add(row);
            return (row);
        }

        private TableCell CreateCell(TableRow row, int columnSpan, int rowSpan)
        {
            TableCell cell = new TableCell(new Paragraph(new Run(". . .   Cell # " + _cellCount.ToString() + ";   . . .")));
            cell.ColumnSpan = columnSpan;
            cell.RowSpan = rowSpan;
            cell.BorderThickness = new Thickness(1);
            cell.Padding = new Thickness(4);
            cell.BorderBrush = Brushes.Black;
            SetCellBackground(cell, columnSpan + rowSpan);
            row.Cells.Add(cell);
            _cellCount++;
            return (cell);
        }

        private void SetCellBackground(TableCell cell, int key)
        {
            if (key == 2)
            {
                cell.Background = Brushes.LightGray;
            }
            else
            {
                switch (key % 5)
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
                    case (4):
                        cell.Background = Brushes.Gold;
                        break;
                }
            }
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
        
        internal void IncColumnSpan11()  { _cell11.ColumnSpan = _cell11.ColumnSpan + 1; SetCellBackground(_cell11, _cell11.ColumnSpan + _cell11.RowSpan); }
        internal void DecColumnSpan11()  { _cell11.ColumnSpan = _cell11.ColumnSpan - 1; SetCellBackground(_cell11, _cell11.ColumnSpan + _cell11.RowSpan); }
        internal void IncColumnSpan12()  { _cell12.ColumnSpan = _cell12.ColumnSpan + 1; SetCellBackground(_cell12, _cell12.ColumnSpan + _cell12.RowSpan); }
        internal void DecColumnSpan12()  { _cell12.ColumnSpan = _cell12.ColumnSpan - 1; SetCellBackground(_cell12, _cell12.ColumnSpan + _cell12.RowSpan); }
        internal void IncColumnSpan22()  { _cell22.ColumnSpan = _cell22.ColumnSpan + 1; SetCellBackground(_cell22, _cell22.ColumnSpan + _cell22.RowSpan); }
        internal void DecColumnSpan22()  { _cell22.ColumnSpan = _cell22.ColumnSpan - 1; SetCellBackground(_cell22, _cell22.ColumnSpan + _cell22.RowSpan); }
        internal void IncColumnSpan93()  { _cell93.ColumnSpan = _cell93.ColumnSpan + 2; SetCellBackground(_cell93, _cell93.ColumnSpan + _cell93.RowSpan); }
        internal void DecColumnSpan93()  { _cell93.ColumnSpan = _cell93.ColumnSpan - 2; SetCellBackground(_cell93, _cell93.ColumnSpan + _cell93.RowSpan); }
        internal void IncRowSpan11()     { _cell11.RowSpan = _cell11.RowSpan + 1; SetCellBackground(_cell11, _cell11.ColumnSpan + _cell11.RowSpan); }
        internal void DecRowSpan11()     { _cell11.RowSpan = _cell11.RowSpan - 1; SetCellBackground(_cell11, _cell11.ColumnSpan + _cell11.RowSpan); }
        internal void IncRowSpan21()     { _cell21.RowSpan = _cell21.RowSpan + 1; SetCellBackground(_cell21, _cell21.ColumnSpan + _cell21.RowSpan); }
        internal void DecRowSpan21()     { _cell21.RowSpan = _cell21.RowSpan - 1; SetCellBackground(_cell21, _cell21.ColumnSpan + _cell21.RowSpan); }
        internal void IncRowSpan22()     { _cell22.RowSpan = _cell22.RowSpan + 2; SetCellBackground(_cell22, _cell22.ColumnSpan + _cell22.RowSpan); }
        internal void DecRowSpan22()     { _cell22.RowSpan = _cell22.RowSpan - 2; SetCellBackground(_cell22, _cell22.ColumnSpan + _cell22.RowSpan); }
        internal void IncRowSpan91()     { _cell91.RowSpan = 153; SetCellBackground(_cell91, _cell91.ColumnSpan + _cell91.RowSpan); }
        internal void DecRowSpan91()     { _cell91.RowSpan = 1; SetCellBackground(_cell91, _cell91.ColumnSpan + _cell91.RowSpan); }

        private Table _root;

        private int _cellCount = 1;
        private TableCell _cell11, _cell12, _cell21, _cell22, _cell25, _cell91, _cell93;
    }
}
