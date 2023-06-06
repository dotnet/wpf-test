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
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Markup;

namespace DRT
{
    // 
    // Table DRT's.
    // 
    internal sealed class TableTasksSuite : FlowLayoutExtSuite
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        internal TableTasksSuite() : base("TableTasks")
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
                new DrtTest(Root),                              new DrtTest(VerifyLayoutCreate),
                new DrtTest(Task1_Load),                    new DrtTest(VerifyLayoutAppend),
                new DrtTest(Task1_2_Load),                  new DrtTest(VerifyLayoutAppend),
                new DrtTest(Task1_2_AddBodyEmptyRowBegin),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(Task1_2_AddBodyEmptyRowEnd),    new DrtTest(VerifyLayoutAppend),
                new DrtTest(Task1_2_BodyClearRows),         new DrtTest(VerifyLayoutAppend),
                new DrtTest(Task1_2_AddBodyEmptyRowBegin),  new DrtTest(VerifyLayoutAppend),
                new DrtTest(Task1_2_AddBodyEmptyRowEnd),    new DrtTest(VerifyLayoutFinalize),
            };
        }

        /// <summary>
        /// Creates DRT's tree
        /// </summary>
        /// <returns>Root of the tree</returns>
        internal void Root()
        {
            _root = new Border();
            _contentRoot.Child = _root;
        }

        //
        //  Task1 "Typographic Table: Allow for empty rows in tables"
        //
        internal void Task1_Load()
        {
            _root.Child = (UIElement)LoadFromXaml("TableTask1.xaml");
        }

        internal void Task1_2_Load()
        {
            FlowDocumentScrollViewer fdsv = new FlowDocumentScrollViewer();
            fdsv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            fdsv.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            fdsv.Document = new FlowDocument();
            fdsv.Document.TextAlignment = TextAlignment.Left;
            fdsv.Document.PagePadding = new Thickness(0);
            _table = new Table();

            _table.RowGroups.Add(new TableRowGroup());

            fdsv.Document.Blocks.Add(_table);
            _root.Child = fdsv;
        }

        internal void Task1_2_AddBodyEmptyRowBegin()
        {
            Task1_2_AddRowGroupEmptyRowBegin(_table.RowGroups[0]);
        }

        internal void Task1_2_AddBodyEmptyRowEnd()
        {
            Task1_2_AddRowGroupEmptyRowEnd(_table.RowGroups[0]);
        }

        internal void Task1_2_BodyClearRows()
        {
            _table.RowGroups[0].Rows.Clear();
            UpdateBackground();
        }

        private void Task1_2_AddRowGroupEmptyRowBegin(TableRowGroup rowGroup)
        {
            rowGroup.Rows.Insert(0, new TableRow());
            UpdateBackground();
        }

        private void Task1_2_AddRowGroupEmptyRowEnd(TableRowGroup rowGroup)
        {
            rowGroup.Rows.Add(new TableRow());
            UpdateBackground();
        }

        private void UpdateBackground()
        {
            _root.Background = new SolidColorBrush(Color.FromArgb(0xff, 00, 00, (byte)(_bg + _bg * 16)));
            _bg = (_bg + 1) % 16;
        }

        private Table _table;
        private Border _root;
        private int _bg = 1;
    }
}
