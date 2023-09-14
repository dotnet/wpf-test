// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
    /// Testing Incremental changes in table.    
    /// </summary>
    [Test(0, "Table", "IncrementalChange")]
    public class IncrementalChange : AvalonTest
    {
        #region Test case members

        private Window _win;
        private Border _border;
        private BasicTable _table;
        private TableRow _row1,_row2,_row3;
        private TableCell _cell11,_cell22,_cell33,_cell44;

        #endregion

        #region Constructor

        public IncrementalChange()
            : base()
        {
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTests);
            RunSteps += new TestStep(VerifyTest);
        }

        #endregion

        #region Test Steps
      
        /// <summary>
        /// Initialize: Setup the test
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult Initialize()
        {
            _win = new Window();
                      
            Status("Initialize");
            _border = new Border();
            FlowDocumentScrollViewer eRoot = new FlowDocumentScrollViewer();
            eRoot.Document = new FlowDocument();
            _table = new BasicTable();
            _table.CreateColumn(new GridLength(100), Brushes.LightBlue);
            _table.CreateColumn(new GridLength(100), Brushes.LightBlue);
            _table.CreateColumn(new GridLength(100), Brushes.LightBlue);
            _table.CreateColumn(new GridLength(100), Brushes.LightBlue);
            _table.CreateColumn(new GridLength(100), Brushes.LightBlue);

            TableRowGroup body = _table.CreateBody();

            _row1 = _table.CreateRow(body);
            _table.CreateCell(_row1);
            _cell11 = _table.CreateCell(_row1, 1, 3);
            _cell22 = _table.CreateCell(_row1, 1, 2);
            _table.CreateCell(_row1);
            _row2 = _table.CreateRow(body);
            _table.CreateCell(_row2);
            _cell33 = _table.CreateCell(_row2, 1, 2);
            _row3 = _table.CreateRow(body);
            _table.CreateCell(_row3);
            _cell44 = _table.CreateCell(_row3);

            eRoot.Document.Blocks.Add(_table.Tbl);

            _border.Child = eRoot;

            _win.Content = _border;
            _win.Width = 500;
            _win.Height = 500;
            _win.Top = 50;
            _win.Left = 50;
            _win.Show();

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
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            LogComment("Runtest started ....");
            _cell11.RowSpan = _cell11.RowSpan - 1;
            _cell22.RowSpan = _cell22.RowSpan + 1;
            _cell33.RowSpan = _cell33.RowSpan - 1;
            _cell44.RowSpan = _cell44.RowSpan + 1;

            return TestResult.Pass;
        }
     
        /// <summary>
        /// VerifyTest: Verify the test.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult VerifyTest()
        {
            if ((_cell11.RowSpan == 2) && (_cell22.RowSpan == 3) && (_cell33.RowSpan == 1) && (_cell44.RowSpan == 2))
            {
                LogComment("TestCase Passed. RowSpan was returned correctly");
                return TestResult.Pass;
            }
            else
            {
                LogComment("Test case failed");
                return TestResult.Fail;
            }                      
        }
        #endregion
    }
}
