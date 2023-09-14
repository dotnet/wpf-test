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
    internal sealed class TableBasicSuite : LayoutSuite
    {

        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        internal TableBasicSuite() : base("TableBasic")
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
                new DrtTest(CreateEmpty),       new DrtTest(VerifyLayoutCreate),
                new DrtTest(InsertDynamicRow),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(RemoveCell),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(RemoveCell),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertCell),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertCell),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(RemoveDynamicRow),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertDynamicRow),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(RemoveCell),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(RemoveCell),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertCell),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertCell),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(RemoveDynamicRow),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertDynamicRow),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(RemoveCell),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(RemoveCell),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertCell),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertCell),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(RemoveDynamicRow),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertDynamicRow),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(RemoveCell),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(RemoveCell),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertCell),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertCell),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(RemoveDynamicRow),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertDynamicRow),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(RemoveCell),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(RemoveCell),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertCell),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertCell),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(RemoveDynamicRow),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertDynamicRow),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(RemoveCell),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(RemoveCell),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertCell),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertCell),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(RemoveDynamicRow),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertDynamicRow),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(RemoveCell),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(RemoveCell),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertCell),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertCell),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(RemoveDynamicRow),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertDynamicRow),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(RemoveCell),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(RemoveCell),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertCell),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertCell),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(RemoveDynamicRow),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertDynamicRow),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(RemoveCell),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(RemoveCell),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertCell),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertCell),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(RemoveDynamicRow),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertDynamicRow),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(RemoveCell),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(RemoveCell),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertCell),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertCell),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(RemoveDynamicRow),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(RemoveHeadRow),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(RemoveHeadRow),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(RemoveHeadRow),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(RemoveHeadRow),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertTailRow),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertTailRow),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertTailRow),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertTailRow),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(RemoveTailRow),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(RemoveTailRow),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(RemoveTailRow),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(RemoveTailRow),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertHeadRow),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertHeadRow),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertHeadRow),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(InsertHeadRow),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(VerifyIList),  new DrtTest(VerifyLayoutFinalize),
            };
        }

        /// <summary>
        /// Creates DRT's tree
        /// </summary>
        /// <returns>Root of the tree</returns>
        private void CreateEmpty()
        {
            FlowDocumentScrollViewer fdsv = new FlowDocumentScrollViewer();
            fdsv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            fdsv.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            fdsv.Document = new FlowDocument();
            fdsv.Document.TextAlignment = TextAlignment.Left;
            fdsv.Document.PagePadding = new Thickness(0);
            Border border = new Border();
            border.Background = Brushes.White;
            
            Table table = _root = new Table();
            table.CellSpacing = 3;

            CreateColumn(table, Brushes.Pink);
            CreateColumn(table, Brushes.LightGreen);
            CreateColumn(table, Brushes.LightBlue);

            TableRowGroup body = new TableRowGroup();
            table.RowGroups.Add(body);
            
            TableRow row;
            TableCell cell;

            row = CreateRow(body);
            cell = CreateCell(row, "1.1");
            cell.Foreground = Brushes.Blue;
            cell.FontSize = 18.0/72.0*96.0;
            cell.FontWeight = FontWeights.Bold;

            cell = CreateCell(row, "1.2");
            cell.Foreground = Brushes.Blue;
            cell.FontSize = 18.0/72.0*96.0;
            cell.FontWeight = FontWeights.Bold;

            cell = CreateCell(row, "1.3");
            cell.Foreground = Brushes.Blue;
            cell.FontSize = 18.0/72.0*96.0;
            cell.FontWeight = FontWeights.Bold;

            row = CreateRow(body);

            CreateCell(row, 
                "Something mysterious is formed, born in the silent void. Waiting alone and unmoving, " + 
                "it is at once still and yet in constant motion. It is the source of all programs. I " + 
                "do not know its name, so I will call it the Tao of Programming.");

            CreateCell(row, 
                "The Tao gave birth to machine language. Machine language gave birth to the assembler.");

            CreateCell(row, 
                "In the beginning was the Tao. The Tao gave birth to Space and Time. Therefore Space " + 
                "and Time are Yin and Yang of programming.");

            row = CreateRow(body);

            CreateCell(row, 
                "If the Tao is great, then the operating system is great. If the operating system is " + 
                "great, then the compiler is great. If the compiler is great, then the application is " + 
                "great. The user is pleased and there is harmony in the world.");

            CreateCell(row, 
                "The assembler gave birth to the compiler. Now there are ten thousand languages.");

            CreateCell(row, 
                "Programmers that do not comprehend the Tao are always running out of time and space " + 
                "for their programs. Programmers that comprehend the Tao always have enough time and " + 
                "space to accomplish their goals.");

            row = CreateRow(body);

            CreateCell(row, 
                "The Tao of Programming flows far away and returns on the wind of morning.");

            CreateCell(row, 
                "Each language has its purpose, however humble. Each language expresses the Yin and Yang " + 
                "of software. Each language has its place within the Tao.   But do not program in COBOL if you can avoid it.");

            CreateCell(row, 
                "How could it be otherwise? ");

            _dynamicRow = CreateSpecialRow();


            fdsv.Document.Blocks.Add(table);
            border.Child = fdsv;
            _contentRoot.Child = border;
        }

        private TableRow CreateRow(TableRowGroup body)
        {
            TableRow row = new TableRow();
            body.Rows.Add(row);
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

        private TableRow CreateSpecialRow()
        {
            TableRow row;
            TableCell cell;

            row = new TableRow();
            row.Background = Brushes.Wheat;

            cell = CreateCell(row, "Y Y Y Y Y Y Y Y");
            cell.Foreground = Brushes.Green;
            cell.FontSize = 24.0/72.0*96.0;
            cell.FontWeight = FontWeights.Bold;

            cell = CreateCell(row, "Z Z Z Z Z Z Z Z");
            cell.Foreground = Brushes.Blue;
            cell.FontSize = 24.0/72.0*96.0;
            cell.FontWeight = FontWeights.Bold;

            cell = new TableCell(new Paragraph(new Run("X X X X X X X X")));
            cell.Foreground = Brushes.Red;
            cell.FontSize = 24.0*96.0/72.0;
            cell.FontWeight = FontWeights.Bold;
            cell.BorderThickness = new Thickness(1);
            cell.Padding = new Thickness(4);
            row.Cells.Insert(0, cell);

            return (row);
        }

        private void VerifyIList()
        {
            TableRowGroup body = _root.RowGroups[0];
            IList list = body.Rows;

            TableRow newRow = CreateRow(body);
            TableRow newRow2 = CreateRow(body);

            list.Remove(newRow);
            list.Remove(newRow2);

            list.Add(newRow);

            if(!list.Contains(newRow) || list.IndexOf(newRow) != list.Count - 1)
            {
                throw new Exception("Error in IList implementation");
            }

            list[list.Count - 1] = newRow2;

            if(!list.Contains(newRow2) || list.Contains(newRow) || list.IndexOf(newRow2) != list.Count - 1)
            {
                throw new Exception("Error in IList implementation");
            }

            list.RemoveAt(list.Count - 1);

            if(list.Contains(newRow2))
            {
                throw new Exception("Error in IList implementation");
            }
        }

        internal void InsertDynamicRow()
        {
            if (_dynamicRowInex == _root.RowGroups[0].Rows.Count)
                _root.RowGroups[0].Rows.Add(_dynamicRow);
            else
                _root.RowGroups[0].Rows.Insert(_dynamicRowInex, _dynamicRow);

            SanityCheck();
        }

        internal void RemoveDynamicRow()
        {
            if ((_dynamicRowInex % 2) == 0)
                _root.RowGroups[0].Rows.Remove(_dynamicRow);
            else
                _root.RowGroups[0].Rows.RemoveAt(_dynamicRowInex);

            SanityCheck();

            _dynamicRowInex = (_dynamicRowInex + 1) % (_root.RowGroups[0].Rows.Count + 1);
        }

        internal void RemoveHeadRow()
        {
            if (_root.RowGroups[0].Rows.Count > 0)
            {
                _rowStack.Push(_root.RowGroups[0].Rows[0]);
                _root.RowGroups[0].Rows.RemoveAt(0);
            }
        }

        internal void InsertHeadRow()
        {
            if (_rowStack.Count > 0)
            {
                _root.RowGroups[0].Rows.Insert(0, _rowStack.Pop() as TableRow);
            }
        }

        internal void RemoveTailRow()
        {
            if (_root.RowGroups[0].Rows.Count > 0)
            {
                _rowStack.Push(_root.RowGroups[0].Rows[_root.RowGroups[0].Rows.Count - 1]);
                _root.RowGroups[0].Rows.RemoveAt(_root.RowGroups[0].Rows.Count - 1);
            }
        }

        internal void InsertTailRow()
        {
            if (_rowStack.Count > 0)
            {
                _root.RowGroups[0].Rows.Add(_rowStack.Pop() as TableRow);
            }
        }

        internal void RemoveCell()
        {
            if (_dynamicRow.Cells.Count > 1)
            {
                int index = _head ? 0 : _dynamicRow.Cells.Count - 1;
                _cellStack.Push(_dynamicRow.Cells[index]);
                _dynamicRow.Cells.RemoveAt(index);

                if (_dynamicRow.Cells.Count == 1)
                    _head = !_head;
            }
        }

        internal void InsertCell()
        {
            if (_cellStack.Count > 0)
            {
                if (_head)
                {
                    _dynamicRow.Cells.Insert(0, _cellStack.Pop() as TableCell);
                }
                else
                {
                    _dynamicRow.Cells.Add(_cellStack.Pop() as TableCell);
                }
            }
        }

        private Table _root;
        private TableRow _dynamicRow;
        private int _dynamicRowInex = 0;
        private Stack _rowStack = new Stack();
        private bool _head = true;
        private Stack _cellStack = new Stack();
    }
}
