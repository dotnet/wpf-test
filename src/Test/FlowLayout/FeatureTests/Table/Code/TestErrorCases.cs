// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Regression test for Regression_Bug52
// 1.  Create a Table
// 2.  Add some content to a cell
// 2.  Remove all the content from the cell
//
// Expected: We should not get any exceptions.
// Created by: Microsoft

using System;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Controls;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Layout;
using Microsoft.Test.Logging;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>   
    /// Testing Table error cases.   
    /// </summary>
    [Test(2, "Table", "ErrorTest", MethodName="Run")]
    public class ErrorTest : AvalonTest
    {
        #region Test case members

        private Window _win;
        private Border _border;
        private BasicTable _table;
        private TableCell _cell1;
        private TableColumn _col2;
        private TableRow _row1;
        private TableRowGroup _body;

        #endregion

        #region Constructor

        public ErrorTest()
            : base()
        {
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(NegativeColumnWidth);
            RunSteps += new TestStep(NegativeColumnSpan);
            RunSteps += new TestStep(NegativeRowSpan);
            RunSteps += new TestStep(RemoveAtNullCell);
            RunSteps += new TestStep(RemoveAtNullRow);
            RunSteps += new TestStep(RemoveAtNullColumn);
        }

        #endregion

        #region Test Steps

        /// <summary>
        /// Initialize: Setup the test
        /// </summary>
        /// <returns>TestResult.Pass;</returns>
        private TestResult Initialize()
        {
            Status("Initialize");

            _win = new Window();
            _border = new Border();
            FlowDocumentScrollViewer eRoot = new FlowDocumentScrollViewer();
            eRoot.Document = new FlowDocument();
            eRoot.Width = 600;
            eRoot.Height = 400;
            _table = new BasicTable();
            _table.CellSpacing = 4;

            _table.CreateColumn(new GridLength(200), Brushes.Green);
            _col2 = _table.CreateColumn(new GridLength(200), Brushes.Gold);

            TableRow rowheader;
            TableRowGroup header = _table.CreateBody();

            rowheader = _table.CreateRow(header);
            _table.CreateCell(rowheader, 3, 1);

            TableRow row;
            _body = _table.CreateBody();

            _row1 = _table.CreateRow(_body);
            _table.CreateCell(_row1);
            _table.CreateCell(_row1);
            _table.CreateCell(_row1);
            row = _table.CreateRow(_body);
            _table.CreateCell(row);
            _table.CreateCell(row);
            _table.CreateCell(row);
            row = _table.CreateRow(_body);
            _cell1 = _table.CreateCell(row);
            _table.CreateCell(row);
            _table.CreateCell(row);

            TableRow rowfooter;
            TableRowGroup footer = _table.CreateBody();

            rowfooter = _table.CreateRow(footer);
            _table.CreateCell(rowfooter, 3, 1);
            eRoot.Document.Blocks.Add(_table.Tbl);
            _border.Child = eRoot;

            _win.Content = _border;
            _win.Width = 500;
            _win.Height = 500;
            _win.Top = 50;
            _win.Left = 50;
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _win.Close();
            return TestResult.Pass;
        }

        private TestResult NegativeColumnWidth()
        {
            SetExpectedErrorTypeInStep(typeof(ArgumentException));
            _col2.Width = new GridLength(-200);
            return TestResult.Fail;
        }

        private TestResult NegativeColumnSpan()
        {
            SetExpectedErrorTypeInStep(typeof(ArgumentException));
            _cell1.ColumnSpan = -1;
            return TestResult.Fail;
        }

        private TestResult NegativeRowSpan()
        {
            SetExpectedErrorTypeInStep(typeof(ArgumentException));
            _cell1.RowSpan = -1;
            return TestResult.Fail;
        }

        private TestResult RemoveAtNullCell()
        {
            SetExpectedErrorTypeInStep(typeof(ArgumentOutOfRangeException));
            int[] index = new int[3] { 2, 0, 2 };

            for (int i = 0; i < index.Length; i++)
            {
                _row1.Cells.RemoveAt(index[i]);
            }

            return TestResult.Fail;
        }

        private TestResult RemoveAtNullRow()
        {
            SetExpectedErrorTypeInStep(typeof(ArgumentOutOfRangeException));
            _body.Rows.RemoveAt(4);
            return TestResult.Fail;
        }

        private TestResult RemoveAtNullColumn()
        {
            SetExpectedErrorTypeInStep(typeof(ArgumentOutOfRangeException));
            _table.Tbl.Columns.RemoveAt(2);
            return TestResult.Fail;
        }
        #endregion
    }
}
