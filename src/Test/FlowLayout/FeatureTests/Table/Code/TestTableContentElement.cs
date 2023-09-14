// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Windows.Markup;

using Microsoft.Test.Layout;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.FlowLayout
{    
    [Test(2, "Table", "TableContentElement")]
    public class TableContentElement : AvalonTest
    {
        #region Test case members

        private Window _w;
        private string _inputXaml;
        private TableRow _newRow;
        private TableCell _newCell;

        #endregion
        
        #region Constructor

        [Variation("TableContentElements.xaml")]

        public TableContentElement(string testValue)
            : base()
        {
            _inputXaml = testValue;
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(DoTest);
        }

        #endregion

        #region Test Steps
   
        /// <summary>
        /// Initialize: Setup the test
        /// </summary>
        /// <returns>TestResult.Pass;</returns>
        private TestResult Initialize()
        {
            _w = new Window();
            FlowDocumentScrollViewer viewer = (FlowDocumentScrollViewer)XamlReader.Load(File.OpenRead(_inputXaml));
            _w.Content = viewer;
            _w.Width = 600;
            _w.Height = 900;
            _w.Top=0;
            _w.Left=0;
            _w.Show();
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _w.Close();
            return TestResult.Pass;
        }

        private TestResult DoTest()
        {
            TestResult tempResult = TestResult.Unknown;

            WaitForPriority(DispatcherPriority.ApplicationIdle);

            Status("DoTest");

            Table table = LogicalTreeHelper.FindLogicalNode(_w, "table") as Table;
            tempResult = DoHeaderTest(table);
            CommonFunctionality.FlushDispatcher();
            tempResult = DoBodyTest(table);
            CommonFunctionality.FlushDispatcher();
            tempResult = DoFooterTest(table);
            CommonFunctionality.FlushDispatcher();
            tempResult = DoFinalTest(table);

            return tempResult;
        }

        #endregion

        private TestResult DoHeaderTest(DependencyObject rootelement)
        {
            CommonFunctionality.FlushDispatcher();

            TableRowGroup header = LogicalTreeHelper.FindLogicalNode(rootelement, "header") as TableRowGroup;
            TableRowCollection header_rows = header.Rows;

            if (header_rows.Count < 1)
            {
                LogComment(header_rows.Count.ToString());
                LogComment("Header has no initial rows.");
                return TestResult.Fail;
            }

            _newRow = new TableRow();
            _newCell = new TableCell(new Paragraph(new Run("new header row")));

            _newRow.Cells.Add(_newCell);
            header_rows.Add(_newRow);
            if (header_rows.Count > 1)
            {
                Status("New row added to header.");
            }
            else
            {
                LogComment("New row was not added header.");
                return TestResult.Fail;
            }

            CommonFunctionality.FlushDispatcher();

            header_rows.Remove(_newRow);
            if (header_rows.Count != 1)
            {
                LogComment(header_rows.Count.ToString());
                LogComment("Could not remove new row, or removed too many rows.");
                return TestResult.Fail;
            }
            else
            {
                Status("New row was removed. Header test passed.");
            }

            return TestResult.Pass;
        }

        private TestResult DoBodyTest(DependencyObject rootelement)
        {
            CommonFunctionality.FlushDispatcher();

            TableRowGroup body = LogicalTreeHelper.FindLogicalNode(rootelement, "body") as TableRowGroup;
            TableRowCollection body_rows = body.Rows;

            if (body_rows.Count < 1)
            {
                LogComment(body_rows.Count.ToString());
                LogComment("Body has no initial rows.");
                return TestResult.Fail;
            }

            _newRow = new TableRow();
            _newCell = new TableCell(new Paragraph(new Run("new body row")));
            _newRow.Cells.Add(_newCell);

            body_rows.Add(_newRow);

            if (body_rows.Count > 1)
            {
                Status("New row added to body.");
            }
            else
            {
                LogComment("New row was not added body.");
                return TestResult.Fail;
            }

            CommonFunctionality.FlushDispatcher();
            body_rows.Remove(_newRow);

            if (body_rows.Count != 1)
            {
                LogComment(body_rows.Count.ToString());
                LogComment("Could not remove new row, or removed too many rows.");
                return TestResult.Fail;
            }
            else
            {
                Status("New row was removed. Body test passed.");
            }

            return TestResult.Pass;
        }

        private TestResult DoFooterTest(DependencyObject rootelement)
        {
            CommonFunctionality.FlushDispatcher();

            TableRowGroup footer = LogicalTreeHelper.FindLogicalNode(rootelement, "footer") as TableRowGroup;
            TableRowCollection footer_rows = footer.Rows;

            if (footer_rows.Count < 1)
            {
                LogComment(footer_rows.Count.ToString());
                LogComment("Footer has no initial rows.");
                return TestResult.Fail;
            }

            _newRow = new TableRow();
            _newCell = new TableCell(new Paragraph(new Run("new footer row")));
            _newRow.Cells.Add(_newCell);
            footer_rows.Add(_newRow);

            if (footer_rows.Count > 1)
            {
                Status("New row added to footer.");
            }
            else
            {
                LogComment("New row was not added footer.");
                return TestResult.Fail;
            }

            CommonFunctionality.FlushDispatcher();
            footer_rows.Remove(_newRow);
            if (footer_rows.Count != 1)
            {
                LogComment(footer_rows.Count.ToString());
                LogComment("Could not remove new row, or removed too many rows.");
                return TestResult.Fail;
            }
            else
            {
                Status("New row was removed. Footer test passed.");
            }

            return TestResult.Pass;
        }

        private TestResult DoFinalTest(DependencyObject rootelement)
        {
            // final test will change column width
            // change background on header row
            // change line height on body cell
            // change line stacking strategy on footer cell

            CommonFunctionality.FlushDispatcher();

            TableColumn column = LogicalTreeHelper.FindLogicalNode(rootelement, "column") as TableColumn;
            TableRow header_row = LogicalTreeHelper.FindLogicalNode(rootelement, "header_row") as TableRow;
            TableCell body_row_cell = LogicalTreeHelper.FindLogicalNode(rootelement, "body_row_cell") as TableCell;
            TableCell footer_row_cell = LogicalTreeHelper.FindLogicalNode(rootelement, "footer_row_cell") as TableCell;

            CommonFunctionality.FlushDispatcher();

            column.Width = new GridLength(1, GridUnitType.Auto);
            header_row.Background = Brushes.Snow;
            body_row_cell.LineHeight = 50;           

            CommonFunctionality.FlushDispatcher();
            if (column.Width == new GridLength(200))
            {
                LogComment("column width was not changed.");
                return TestResult.Fail;
            }
            else
            {
                Status("column width was changed.");
            }

            if (header_row.Background.ToString() == "#FF87CEEB")
            {
                LogComment("header row background was not changed.");
                return TestResult.Fail;
            }
            else
            {
                Status("Row background was changed");
            }

            if (body_row_cell.LineHeight != 50)
            {
                LogComment("line height of cell in body row was not changed.");
                return TestResult.Fail;
            }
            else
            {
                Status("cell line height was changed");
                Status("All tests are passed.");
                return TestResult.Pass;
            }
        }
    }
}
