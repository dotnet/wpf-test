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
    /// Testing get cell property in table.  
    /// </summary>
    [Test(0, "Table", "GetCellProperties")]
    public class GetCellProperties : AvalonTest
    {
        #region Test case members

        private Window _win;
        private Border _border;
        private BasicTable _table;        
        private TableRow _row1,_row2,_row3;
        private TableCell _cell11,_cell22,_cell33,_cell44;
        private System.Windows.TextAlignment _cell11TextHorizontal,_cell33TextHorizontal;

        #endregion
        
        #region Constructor

        public GetCellProperties()
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
        /// <returns>TestResult.Pass;</returns>
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
            _cell11.ContentEnd.InsertTextInRun("That takes care of the aesthetics. Butaway from that, the real impact came with Farhats resolve to ride over his second nature.");
            _cell22 = _table.CreateCell(_row1, 1, 2);
            _cell22.ContentEnd.InsertTextInRun("A naturally attacking player, he buried those instincts deep and pulled out a rather sedate approach");
            _table.CreateCell(_row1);

            _row2 = _table.CreateRow(body);
            _table.CreateCell(_row2);
            _cell33 = _table.CreateCell(_row2, 1, 2);
            _cell33.ContentEnd.InsertTextInRun("HELLO");

            _row3 = _table.CreateRow(body);
            _table.CreateCell(_row3);
            _cell44 = _table.CreateCell(_row3);
            _cell44.ContentEnd.InsertTextInRun("GOOD BYE");

            eRoot.Document.Blocks.Add(_table.Tbl);
            _border.Child = eRoot;

            _win.Content = _border;
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
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            LogComment("Runtest started ....");
            _cell11.TextAlignment = System.Windows.TextAlignment.Justify;
            _cell33.TextAlignment = System.Windows.TextAlignment.Center;
            _cell11TextHorizontal = _cell11.TextAlignment;
            _cell33TextHorizontal = _cell33.TextAlignment;
            return TestResult.Pass;                   
        }

        /// <summary>
        /// VerifyTest: Verify the test.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult VerifyTest()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            LogComment("VerifyTest started ....");
            if ((_cell11TextHorizontal == System.Windows.TextAlignment.Justify) && (_cell33TextHorizontal == System.Windows.TextAlignment.Center))
            {
                LogComment("TextAlignment value and set and got correctly");
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
