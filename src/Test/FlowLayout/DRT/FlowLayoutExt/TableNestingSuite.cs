// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Table nesting cases. 
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
    /// <summary>
    /// DoubleMeasureArrangeDecorator calls its child to measure TWO times. 
    /// </summary>
    internal sealed class DoubleMeasureArrangeDecorator : Decorator
    {
        internal DoubleMeasureArrangeDecorator()
        {
        }

        protected override Size MeasureOverride(Size constraint)
        {
            UIElement child = Child;

            if (child != null)
            {
                child.Measure(new Size(constraint.Width - 10, constraint.Height - 10));
                child.Measure(constraint);
            }

            return (constraint);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            UIElement child = Child;
            if (child != null)
            {
                child.Arrange(new Rect(child.DesiredSize));
            }
            return arrangeSize;
        }
        
        protected override void OnRender(DrawingContext dc)
        {
            Color backgroundColor = Color.FromArgb(
                (byte)(255), 
                (byte)(((int)RenderSize.Height) % 127 + 128), 
                (byte)(((int)RenderSize.Width) % 127 + 128), 
                (byte)(((int)RenderSize.Width - (int)RenderSize.Height) % 127 + 128));

            Brush backgroundBrush = new SolidColorBrush(backgroundColor);

            dc.DrawRectangle(
                backgroundBrush, 
                null, 
                new Rect(new Point(0.0, 0.0), RenderSize));
        }
    }

    // 
    // Table DRT's.
    // 
    internal sealed class TableNestingSuite : FlowLayoutExtSuite
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        internal TableNestingSuite() : base("TableNesting")
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
                new DrtTest(CreateCase01),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(ShrinkLayout),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(ExpandLayout),  new DrtTest(VerifyLayoutFinalize),
            };
        }

        /// <summary>
        /// Creates DRT's tree
        /// </summary>
        /// <returns>Root of the tree</returns>
        internal void Root()
        {
            Border border = _root = new Border();
            border.Background = Brushes.White;

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

        /// <summary>
        /// In this case table is nested inside "double measure" parent - DoubleMeasureArrangeDecorator. 
        /// DoubleMeasureArrangeDecorator calls its child to measure twice, then arrange. Table should 
        /// handle "measure - measure - arrange" case...
        /// </summary>
        private void CreateCase01()
        {
            DockPanel dockPanelA = new DockPanel();
            _root.Child = dockPanelA;

            DoubleMeasureArrangeDecorator decorator = new DoubleMeasureArrangeDecorator();
            dockPanelA.Children.Add(decorator);

            DockPanel dockPanelB = new DockPanel();
            decorator.Child = dockPanelB;
            dockPanelB.Width = 560;

            TableRowGroup body = new TableRowGroup();
            for (int i = 1; i <= 9; ++i)
            {
                TableRow row;

                row = CreateRow(body);
                CreateCell(row, "something " + i + ".1");
                CreateCell(row, "something " + i + ".2");
                CreateCell(row, "something " + i + ".3");
            }

            Table table = new Table();
            table.CellSpacing = 4;
            table.RowGroups.Add(body);

            FlowDocumentScrollViewer fdsv = new FlowDocumentScrollViewer();
            fdsv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            fdsv.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            fdsv.Document = new FlowDocument(table);
            fdsv.Document.TextAlignment = TextAlignment.Left;
            fdsv.Document.PagePadding = new Thickness(0);
            dockPanelB.Children.Add(fdsv);
        }

        internal void ShrinkLayout() { ((FrameworkElement)(_root.Child)).Width = 400; }
        internal void ExpandLayout() { ((FrameworkElement)(_root.Child)).Width = Double.NaN; }

        private Border _root;
    }
}
