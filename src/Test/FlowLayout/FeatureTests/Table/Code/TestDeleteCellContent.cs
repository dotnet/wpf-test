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
using Microsoft.Test.Logging;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>   
    /// Testing commands in FlowDocumentPageViewer.   
    /// </summary>
    [Test(3, "Table", "DeleteCellContent")]
    public class DeleteCellContent : AvalonTest
    {
        #region Test case members

        private Window _win;
        private TableCell _cell;
        
        #endregion

        #region Constructor

        public DeleteCellContent()
            : base()
        {
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTests);
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
                     
            FlowDocumentScrollViewer tp = new FlowDocumentScrollViewer();
            tp.Document = new FlowDocument();

            Table table = new Table();
            table.Background = Brushes.LightGreen;

            TableColumn col = new TableColumn();
            table.Columns.Add(col);

            TableRowGroup header = new TableRowGroup();
            table.RowGroups.Add(header);

            TableRow row = new TableRow();
            header.Rows.Add(row);

            _cell = new TableCell(new Paragraph(new Run("foo")));

            row.Cells.Add(_cell);

            tp.Document.Blocks.Add(table);

            _win.Content = tp;
            _win = new Window();
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

        /// <summary>
        /// RunTests: Runs a set of tests based on the input variation.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult RunTests()
        {            
            ((Paragraph)_cell.Blocks.FirstBlock).Inlines.Add(new InlineUIContainer(AppendMe()));
            ((Paragraph)_cell.Blocks.FirstBlock).Inlines.Add(new Run("header cell"));
            RemoveContent();

            if (Log.Result != TestResult.Fail)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }

        #endregion

        private void RemoveContent()
        {
            LogComment("Try to delete content of cell (after append)");
            TextRange range = new TextRange(_cell.ContentStart, _cell.ContentEnd);
            range.Text = String.Empty;
        }

        private FrameworkElement AppendMe()
        {
            TextBlock txt = new TextBlock();
            txt.Text = "foo";
            txt.FontSize = 25;
            return txt;
        }
    }
}
