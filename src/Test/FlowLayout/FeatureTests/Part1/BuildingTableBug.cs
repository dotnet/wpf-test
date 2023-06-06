// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;

using Microsoft.Test.Discovery;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Logging;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>
    /// <area>Part1.RegressionTest</area>
    /// <owner>Microsoft</owner>
    /// <priority>1</priority>
    /// <description>
    /// Regression coverage for Part1 Regression_Bug47 where adding a TableRow to a Table that spans two pages causes an exception.
    /// </description>
    /// </summary>
    [Test(1, "Part1.RegressionTests", "BuildingTableBug", MethodName = "Run")]
    public class BuildingTableBug : AvalonTest
    {
        private TableRowGroup _tableRowGroup;
        private FlowDocumentReader _flowDocumentReader;
        private const int minExpectedDocumentPageCount = 3;
        private const int numTableRowsToAdd = 40;
        private Window _testWin;

        public BuildingTableBug()
        {
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTest);
            RunSteps += new TestStep(VerifyTest);
        }

        /// <summary>
        /// Creates content for the test.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult Initialize()
        {
            _tableRowGroup = new TableRowGroup();
            _tableRowGroup.Rows.Add(CreateTableRow());

            Table table = new Table();
            table.RowGroups.Add(_tableRowGroup);

            FlowDocument flowDocument = new FlowDocument();
            flowDocument.Blocks.Add(table);

            _flowDocumentReader = new FlowDocumentReader();
            _flowDocumentReader.Document = flowDocument;
            _flowDocumentReader.Width = 400;
            _flowDocumentReader.Height = 400;

            _testWin = new Window();
            _testWin.Height = 450;
            _testWin.Width = 450;
            _testWin.Content = _flowDocumentReader;
            _testWin.Show();

            WaitForPriority(DispatcherPriority.ApplicationIdle);

            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _testWin.Close();
            return TestResult.Pass;
        }

        /// <summary>
        /// Add several TableRows to the Table so that it extends a few pages.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult RunTest()
        {
            for (int i = 0; i < numTableRowsToAdd; i++)
            {
                _tableRowGroup.Rows.Add(CreateTableRow());
                WaitForPriority(DispatcherPriority.ApplicationIdle);
            }

            return TestResult.Pass;
        }

        /// <summary>
        /// Verify that the document is at least 3 pages. 
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult VerifyTest()
        {
            if (_flowDocumentReader.PageCount < minExpectedDocumentPageCount)
            {
                TestLog.Current.LogEvidence(string.Format("There was some problem adding Rows to the Table.  The document is only {0} pages long.  Expected at least {1}.", _flowDocumentReader.PageCount, minExpectedDocumentPageCount));
                return TestResult.Fail;
            }
            else
            {
                return TestResult.Pass;
            }
        }

        /// <summary>
        /// Creates a generic TableRow.
        /// </summary>
        /// <returns>TableRow</returns>
        private TableRow CreateTableRow()
        {
            Paragraph paragraph = new Paragraph(new Run("A new TableRow."));
            TableCell tableCell = new TableCell(paragraph);
            TableRow tableRow = new TableRow();
            tableRow.Cells.Add(tableCell);
            return tableRow;
        }
    }
}
