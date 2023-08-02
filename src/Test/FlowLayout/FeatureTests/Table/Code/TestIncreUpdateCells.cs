// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Controls;
using Microsoft.Test.Layout.PropertyDump;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Layout;
using Microsoft.Test.Logging;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>   
    /// Testing Incremental update of cells in table.    
    /// </summary>
    [Test(2, "Table", "IncreUpdateCells")]
    public class IncreUpdateCells : AvalonTest
    {
        #region Test case members

        private Window _win;
        private int _testId;
        private Border _border;
        private BasicTable _table;
        private Table _root;
        private TableRow _row1;
        private TableRow _row2;
        private TableRow _row3;
        private TableRow _row4;
        private TableRow _row5;
        private TableRow _row6;
        private TableRow _row7;
        private TableRow _row8;
        private TableRow _row9;
        private TableCell _newcell;
        private string _inputString;       

        #endregion

        #region Constructor

        [Variation(1, "Test1")]
        [Variation(2, "Test2")]
        [Variation(3, "Test3")]
        [Variation(4, "Test4", Priority = 3)]
        [Variation(5, "Test5", Priority = 3)]
        [Variation(6, "Test6", Priority = 3)]
        public IncreUpdateCells(int testValue, string testString)
            : base()
        {
            _testId = testValue;
            _inputString = testString;
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
            _border.Background = Brushes.White;
            FlowDocumentScrollViewer eRoot = new FlowDocumentScrollViewer();
            eRoot.Document = new FlowDocument();            

            _table = new BasicTable();
            _root = _table.Tbl;

            _table.Background = Brushes.White;
            _table.CreateColumn(new GridLength(100), Brushes.LightBlue);
            _table.CreateColumn(new GridLength(100), Brushes.LightPink);
            _table.CreateColumn(Brushes.Yellow);
            _table.CreateColumn(new GridLength(100), Brushes.LightGreen);
            _table.CreateColumn(new GridLength(100), Brushes.LightGray);

            TableRowGroup body = _table.CreateBody();
            _row1 = _table.CreateRow(body);
            _table.CreateCell(_row1, 1, 1, "1");
            _table.CreateCell(_row1, 1, 1, "2");
            _table.CreateCell(_row1, 1, 1, "3");
            _table.CreateCell(_row1, 1, 1, "4");
            _table.CreateCell(_row1, 1, 1, "5");

            _row2 = _table.CreateRow(body);
            _table.CreateCell(_row2, 1, 1, "1");
            _table.CreateCell(_row2, 1, 1, "2");
            _table.CreateCell(_row2, 1, 1, "3");

            _row3 = _table.CreateRow(body);
            _table.CreateCell(_row3, 1, 1, "1");
            _table.CreateCell(_row3, 1, 1, "2");
            _table.CreateCell(_row3, 1, 1, "3");
            _table.CreateCell(_row3, 1, 1, "4");
            _table.CreateCell(_row3, 1, 1, "5");

            _row4 = _table.CreateRow(body);
            _table.CreateCell(_row4, 1, 1, "1");
            _table.CreateCell(_row4, 1, 1, "2");
            _table.CreateCell(_row4, 1, 1, "3");
            _table.CreateCell(_row4, 1, 1, "4");
            _table.CreateCell(_row4, 1, 1, "5");

            _row5 = _table.CreateRow(body);
            _table.CreateCell(_row5, 1, 1, "1");
            _table.CreateCell(_row5, 1, 1, "2");
            _table.CreateCell(_row5, 1, 1, "3");
            _table.CreateCell(_row5, 1, 1, "4");
            _table.CreateCell(_row5, 1, 1, "5");

            _row6 = _table.CreateRow(body);
            _table.CreateCell(_row6, 1, 1, "1");
            _table.CreateCell(_row6, 1, 1, "2");
            _table.CreateCell(_row6, 1, 1, "3");
            _table.CreateCell(_row6, 1, 1, "4");
            _table.CreateCell(_row6, 1, 1, "5");

            _row7 = _table.CreateRow(body);
            _table.CreateCell(_row7, 1, 1, "1");
            _table.CreateCell(_row7, 1, 1, "2");
            _table.CreateCell(_row7, 1, 1, "3");
            _table.CreateCell(_row7, 1, 1, "4");
            _table.CreateCell(_row7, 1, 1, "5");

            _row8 = _table.CreateRow(body);
            _table.CreateCell(_row8, 1, 1, "1");
            _table.CreateCell(_row8, 1, 1, "2");
            _table.CreateCell(_row8, 1, 1, "3");
            _table.CreateCell(_row8, 1, 1, "4");
            _table.CreateCell(_row8, 1, 1, "5");

            _row9 = _table.CreateRow(body);
            _table.CreateCell(_row9, 1, 1, "1");
            _table.CreateCell(_row9, 1, 1, "2");
            _table.CreateCell(_row9, 1, 1, "3");
            _table.CreateCell(_row9, 1, 1, "4");
            _table.CreateCell(_row9, 1, 1, "5");
            eRoot.Document.Blocks.Add(_table.Tbl);
            _border.Child = eRoot;

            _win.Content = _border;
            if (_win.Content is FrameworkElement)
            {
                // Setting Window.Content size to ensure same size of root element over all themes.  
                // Different themes have different sized window chrome which will cause property dump 
                // and vscan failures even though the rest of the content is the same.
                // 784x564 is the content size of a 800x600 window in Aero theme.
                ((FrameworkElement)_win.Content).Height = 564;
                ((FrameworkElement)_win.Content).Width = 784;
            }
            _win.Top = 0;
            _win.Left = 0;
            _win.SizeToContent = SizeToContent.WidthAndHeight;
            _win.Resources.MergedDictionaries.Add(GenericStyles.LoadAllStyles());
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
            switch (_testId)
            {
                case 1:
                    AddCellToFirstRowInRandomOrder();
                    break;
                case 2:
                    AddCellToRandomRow();
                    break;
                case 3:
                    AddCellWithColumnSpan();
                    break;
                case 4:
                    AddCellWithRowSpan();
                    break;
                case 5:
                    RemoveCellInRandomOrder();
                    break;
                case 6:
                    RemoveCellsFromRandomRow();
                    break;
                default:
                    LogComment("Invalid entry ......");
                    Log.Result = TestResult.Fail;
                    break;
            }

            WaitForPriority(DispatcherPriority.ApplicationIdle);
            if (Log.Result == TestResult.Fail)
            {
                return TestResult.Fail;
            }
            else
            {
                return TestResult.Pass;
            }
        }

        /// <summary>
        /// VerifyTest: Verify the test.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult VerifyTest()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            Status("Property Dump Verification");
            try
            {
                PropertyDumpHelper helper = new PropertyDumpHelper((Visual)_win.Content);
                if (helper.CompareLogShow(new Arguments(this)))
                {
                    return TestResult.Pass;
                }
                else
                {
                    return TestResult.Fail;
                }
            }
            catch (System.Xml.XmlException)
            {
                return TestResult.Ignore;
            }
        }

        #endregion

        private void AddCellToFirstRowInRandomOrder()
        {
            int[] indexOrder = new int[3] { 5, 3, 0 };

            for (int i = 0; i < indexOrder.Length; i++)
            {
                _newcell = new TableCell();
                _newcell.Background = Brushes.Red;
                _newcell.BorderBrush = Brushes.Black;
                _newcell.BorderThickness = new Thickness(1);
                _row1.Cells.Insert(indexOrder[i], _newcell);
            }
        }

        private void AddCellToRandomRow()
        {
            TableRow[] rowIndex = new TableRow[10] { _row3, _row4, _row1, _row9, _row1, _row7, _row5, _row6, _row8, _row2 };
            int z = 0;

            for (int i = 0; i < rowIndex.Length; i++)
            {
                _newcell = new TableCell();
                _newcell.Background = Brushes.Red;
                _newcell.BorderBrush = Brushes.Black;
                _newcell.BorderThickness = new Thickness(1);
                rowIndex[i].Cells.Insert(z, _newcell);
                z++;

                if (z >= 6)
                {
                    z = 0;
                }
            }
        }

        private void AddCellWithColumnSpan()
        {
            _newcell = new TableCell();
            _newcell.Background = Brushes.Red;
            _newcell.BorderBrush = Brushes.Black;
            _newcell.BorderThickness = new Thickness(1);
            _newcell.ColumnSpan = 3;
            _row4.Cells.Insert(3, _newcell);
        }

        private void AddCellWithRowSpan()
        {
            _newcell = new TableCell();
            _newcell.Background = Brushes.Red;
            _newcell.BorderBrush = Brushes.Black;
            _newcell.BorderThickness = new Thickness(1);
            _newcell.RowSpan = 3;
            _row8.Cells.Insert(4, _newcell);
        }

        private void RemoveCellInRandomOrder()
        {
            int[] indexOrder = new int[4] { 4, 1, 0, 1 };

            for (int i = 0; i < indexOrder.Length; i++)
            {
                _row1.Cells.RemoveAt(indexOrder[i]);
            }
        }

        private void RemoveCellsFromRandomRow()
        {
            TableRow[] rowIndex = new TableRow[10] { _row3, _row4, _row1, _row9, _row1, _row7, _row5, _row6, _row8, _row2 };
            int[] z = new int[10] { 4, 3, 0, 2, 3, 1, 4, 0, 2, 1 };
            int cnt = 0;
            for (int i = 0; i < rowIndex.Length; i++)
            {
                rowIndex[i].Cells.RemoveAt(z[cnt]);
                cnt++;
            }
        }
    }
}
