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
    internal sealed class TableBugsSuite : FlowLayoutExtSuite
    {

        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        internal TableBugsSuite() : base("TableBugs")
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
                new DrtTest(Root),                                  new DrtTest(VerifyLayoutCreate),
                new DrtTest(Regression_Bug23),                             new DrtTest(VerifyLayoutAppend),
                new DrtTest(Regression_Bug24),                             new DrtTest(VerifyLayoutAppend),
                new DrtTest(Regression_Bug24_RemoveFirstBodyRow),          new DrtTest(VerifyLayoutAppend),
                new DrtTest(Regression_Bug24_RemoveFirstBodyRow),          new DrtTest(VerifyLayoutAppend),
                new DrtTest(Regression_Bug25),                             new DrtTest(VerifyLayoutAppend),
                new DrtTest(Regression_Bug25_RemoveLastBodyRow),           new DrtTest(VerifyLayoutAppend),
                new DrtTest(Regression_Bug25_RemoveLastBodyRow),           new DrtTest(VerifyLayoutAppend),
                new DrtTest(Regression_Bug22),                        new DrtTest(VerifyLayoutAppend),
                new DrtTest(Regression_Bug22_UpdateTableBackground),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(Regression_Bug22_UpdateColumnBackground), new DrtTest(VerifyLayoutAppend),
                new DrtTest(Regression_Bug22_UpdateRowBackground),    new DrtTest(VerifyLayoutAppend),
                new DrtTest(Regression_Bug22_UpdateCellBackground),   new DrtTest(VerifyLayoutAppend),
                new DrtTest(Regression_Bug26),                             new DrtTest(VerifyLayoutFinalize),
            };

        }

        private void Root()
        {
            _root = new Border();
            _contentRoot.Child = _root;
        }
        
        //
        //  Regression_Bug23 "Extra line added when TextFlow used in Cell."
        //
        internal void Regression_Bug23()
        {
            _root.Child = (UIElement)LoadFromXaml("TableRegression_Bug23.xaml");
        }

        //
        //  Regression_Bug24 "Table: Try to delete all the Rows in Table, it throws Assert when deleting the Last Row"
        //
        internal void Regression_Bug24()
        {
            _root.Child = (UIElement)LoadFromXaml("TableRegression_Bug24.xaml");
        }

        internal void Regression_Bug24_RemoveFirstBodyRow()
        {
            Table table = GetTableFromTextPanel(_root.Child);
            table.RowGroups[0].Rows.Remove(table.RowGroups[0].Rows[0]);
        }

        //
        //  Regression_Bug25 "Table :: Deleting Rows from Body adjust the gap by shifting rows but Footer doesn't positioned Appropriatly, Result is a gap between Body and Footer"
        //
        internal void Regression_Bug25()
        {
            _root.Child = (UIElement)LoadFromXaml("TableRegression_Bug25.xaml");
        }

        internal void Regression_Bug25_RemoveLastBodyRow()
        {
            Table table = GetTableFromTextPanel(_root.Child);
            table.RowGroups[0].Rows.Remove(table.RowGroups[0].Rows[table.RowGroups[0].Rows.Count - 1]);
        }

        //
        //  "Changing background property on table / column / row / cell does not update rendering"
        //
        internal void Regression_Bug22()
        {
            _root.Child = (UIElement)LoadFromXaml("TableRegression_Bug22.xaml");
        }

        internal void Regression_Bug22_UpdateTableBackground()
        {
            Table table = GetTableFromTextPanel(_root.Child);
            table.Background = Brushes.White;
        }

        internal void Regression_Bug22_UpdateColumnBackground()
        {
            Table table = GetTableFromTextPanel(_root.Child);
            table.Columns[0].Background = Brushes.Black;
        }

        internal void Regression_Bug22_UpdateRowBackground()
        {
            Table table = GetTableFromTextPanel(_root.Child);
            table.RowGroups[0].Rows[0].Background = Brushes.Gray;
        }

        internal void Regression_Bug22_UpdateCellBackground()
        {
            Table table = GetTableFromTextPanel(_root.Child);
            table.RowGroups[0].Rows[0].Cells[0].Background = Brushes.Red;
        }

        internal Table GetTableFromTextPanel(UIElement uiElt)
        {
            if(!(uiElt is FlowDocumentScrollViewer))
                return null;

            FlowDocumentScrollViewer fdsv = (FlowDocumentScrollViewer) uiElt;
            IEnumerator e = LogicalTreeHelper.GetChildren(fdsv.Document).GetEnumerator();
            e.MoveNext();

            return (Table) e.Current;
        }

        //
        //  Regression_Bug26 "The content from the cell in the adjacent row appears outside the pageElement while paginating in between cells of 2 rows."
        //  
        internal void Regression_Bug26()
        {
            IDocumentPaginatorSource document = (IDocumentPaginatorSource)LoadFromXaml("TableRegression_Bug26.xaml");
            DocumentPaginator paginator = document.DocumentPaginator;
            paginator.PageSize = new Size(365, 365);

            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;

            for (int i = 0; i < 2; ++i)
            {
                DocumentPageView pageView = new DocumentPageView();
                pageView.DocumentPaginator = paginator;
                pageView.PageNumber = i;
                pageView.Height = 300;
                pageView.Width = 300;

                Border border = new Border();
                border.BorderBrush = Brushes.Gold;
                border.BorderThickness = new Thickness(2);
                border.Background = Brushes.Gray;
                border.Margin = new Thickness(3);
                border.Child = pageView;

                stackPanel.Children.Add(border);
            }

            _root.Child = stackPanel;
        }

        private Border _root;
    }
}
