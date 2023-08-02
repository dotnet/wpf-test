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
using Microsoft.Test.Layout.PropertyDump;
using Microsoft.Test.Logging;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>    
    /// Testing commands in FlowDocumentPageViewer.   
    /// </summary>
    [Test(2, "Table", "IncreColumnSpan", MethodName="Run")]
    public class IncreColumnSpan : AvalonTest
    {
        #region Test case members

        private Window _win;
        private int _testId;
        private Border _border;
        private BasicTable _table;                
        private Table _root;
        private TableCell _cell11,_cell22,_cell34,_cell43,_cell55,_cell62,_cell83;
        private string _inputString;

        #endregion
        [Variation(1, "Test1")]
        [Variation(2, "Test2" )]
        [Variation(3, "Test3", Priority = 3)]
        [Variation(4, "Test4", Priority = 3)]
        [Variation(5, "Test5", Priority = 3)]
        [Variation(6, "Test6")]
        [Variation(7, "Test7")]
        [Variation(8, "Test8")]
        [Variation(9, "Test9")]
        [Variation(10, "Test10")]
        [Variation(11, "Test11")]
        [Variation(12, "Test12")]
        [Variation(13, "Test13")]
        [Variation(14, "Test14")]
        [Variation(15, "Test15")]
        [Variation(16, "Test16", Priority = 3)]
        [Variation(17, "Test17", Priority = 3)]
        [Variation(18, "Test18", Priority = 3)]
        [Variation(19, "Test19")]
        [Variation(20, "Test20")]
        [Variation(21, "Test21")]
        [Variation(22, "Test22")]
        [Variation(23, "Test23", Priority = 3)]
        [Variation(24, "Test24", Priority = 3)]
        [Variation(25, "Test25")]
        [Variation(26, "Test26")]

        #region Constructor

        public IncreColumnSpan(int testValue, string testString)
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
            TableRow row;

            row = _table.CreateRow(body);
            _cell11 = _table.CreateCell(row, 2, 1, "1");
            _table.CreateCell(row, 1, 1, "2");
            _table.CreateCell(row, 1, 1, "3");
            _table.CreateCell(row, 1, 1, "4");
            _table.CreateCell(row, 1, 1, "5");

            row = _table.CreateRow(body);
            _table.CreateCell(row, 1, 1, "1");
            _cell22 = _table.CreateCell(row, 3, 2, "2");
            _table.CreateCell(row, 1, 1, "3");

            row = _table.CreateRow(body);
            _table.CreateCell(row, 1, 1, "1");
            _table.CreateCell(row, 1, 1, "2");
            _table.CreateCell(row, 1, 1, "3");
            _cell34 = _table.CreateCell(row, 1, 3, "4");
            _table.CreateCell(row, 1, 1, "5");

            row = _table.CreateRow(body);
            _table.CreateCell(row, 1, 1, "1");
            _table.CreateCell(row, 1, 1, "2");
            _cell43 = _table.CreateCell(row, 2, 2, "3");
            _table.CreateCell(row, 1, 1, "4");
            _table.CreateCell(row, 1, 1, "5");

            row = _table.CreateRow(body);
            _table.CreateCell(row, 1, 1, "1");
            _table.CreateCell(row, 1, 1, "2");
            _table.CreateCell(row, 1, 1, "3");
            _table.CreateCell(row, 1, 1, "4");
            _cell55 = _table.CreateCell(row, 1, 1, "5");

            row = _table.CreateRow(body);
            _table.CreateCell(row, 1, 1, "1");
            _cell62 = _table.CreateCell(row, 1, 3, "2");
            _table.CreateCell(row, 1, 1, "3");
            _table.CreateCell(row, 1, 1, "4");
            _table.CreateCell(row, 1, 1, "5");

            row = _table.CreateRow(body);
            _table.CreateCell(row, 1, 1, "1");
            _table.CreateCell(row, 1, 1, "2");
            _table.CreateCell(row, 1, 1, "3");
            _table.CreateCell(row, 1, 1, "4");
            _table.CreateCell(row, 1, 1, "5");

            row = _table.CreateRow(body);
            _table.CreateCell(row, 1, 1, "1");
            _table.CreateCell(row, 1, 1, "2");
            _cell83 = _table.CreateCell(row, 1, 2, "3");
            _table.CreateCell(row, 1, 1, "4");
            _table.CreateCell(row, 1, 1, "5");

            row = _table.CreateRow(body);
            _table.CreateCell(row, 1, 1, "1");
            _table.CreateCell(row, 1, 1, "2");
            _table.CreateCell(row, 1, 1, "3");
            _table.CreateCell(row, 1, 1, "4");
            _table.CreateCell(row, 1, 1, "5");

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
                    IncreColumnSpan11();
                    break;
                case 2:
                    IncreColumnSpan22();
                    break;
                case 3:
                    IncreColumnSpan34();
                    break;
                case 4:
                    IncreColumnSpan43();
                    break;
                case 5:
                    IncreColumnSpan55();
                    break;
                case 6:
                    IncreColumnSpan62();
                    break;
                case 7:
                    IncreColumnSpan83();
                    break;
                case 8:
                    IncreColumnSpan11();
                    IncreColumnSpan22();
                    IncreColumnSpan34();
                    IncreColumnSpan43();
                    IncreColumnSpan55();
                    IncreColumnSpan62();
                    IncreColumnSpan83();
                    break;
                case 9:
                    DecColumnSpan11();
                    break;
                case 10:
                    DecColumnSpan22();
                    break;
                case 11:
                    DecColumnSpan43();
                    break;
                case 12:
                    DecColumnSpan11();
                    DecColumnSpan22();
                    DecColumnSpan43();
                    break;
                case 13:
                    IncreRowSpan11();
                    break;
                case 14:
                    IncreRowSpan22();
                    break;
                case 15:
                    IncreRowSpan34();
                    break;
                case 16:
                    IncreRowSpan43();
                    break;
                case 17:
                    IncreRowSpan55();
                    break;
                case 18:
                    IncreRowSpan62();
                    break;
                case 19:
                    IncreRowSpan83();
                    break;
                case 20:
                    IncreRowSpan11();
                    IncreRowSpan22();
                    IncreRowSpan34();
                    IncreRowSpan43();
                    IncreRowSpan55();
                    IncreRowSpan62();
                    IncreRowSpan83();
                    break;
                case 21:
                    DecRowSpan22();
                    break;
                case 22:
                    DecRowSpan34();
                    break;
                case 23:
                    DecRowSpan43();
                    break;
                case 24:
                    DecRowSpan62();
                    break;
                case 25:
                    DecRowSpan83();
                    break;
                case 26:
                    DecRowSpan22();
                    DecRowSpan34();
                    DecRowSpan43();
                    DecRowSpan62();
                    DecRowSpan83();
                    break;
                default:
                    LogComment("Invalid entry....");
                    Log.Result = TestResult.Fail;
                    break;
            }

            if (Log.Result == TestResult.Fail)
            {
                return TestResult.Fail;
            }
            else
            {
                return TestResult.Pass;
            }
        }

        private void IncreColumnSpan11() { _cell11.ColumnSpan = _cell11.ColumnSpan + 2; }
        private void IncreColumnSpan22() { _cell22.ColumnSpan = _cell22.ColumnSpan + 1; }
        private void IncreColumnSpan34() { _cell34.ColumnSpan = _cell34.ColumnSpan + 3; }
        private void IncreColumnSpan43() { _cell43.ColumnSpan = _cell43.ColumnSpan + 1; }
        private void IncreColumnSpan55() { _cell55.ColumnSpan = _cell55.ColumnSpan + 2; }
        private void IncreColumnSpan62() { _cell62.ColumnSpan = _cell62.ColumnSpan + 1; }
        private void IncreColumnSpan83() { _cell83.ColumnSpan = _cell83.ColumnSpan + 3; }
        private void DecColumnSpan22() { _cell22.ColumnSpan = _cell22.ColumnSpan - 2; }
        private void DecColumnSpan11() { _cell11.ColumnSpan = _cell11.ColumnSpan - 1; }
        private void DecColumnSpan43() { _cell43.ColumnSpan = _cell43.ColumnSpan - 1; }
        private void IncreRowSpan11() { _cell11.RowSpan = _cell11.RowSpan + 10; }
        private void IncreRowSpan22() { _cell22.RowSpan = _cell22.RowSpan + 2; }
        private void IncreRowSpan34() { _cell34.RowSpan = _cell34.RowSpan + 1; }
        private void IncreRowSpan43() { _cell43.RowSpan = _cell43.RowSpan + 4; }
        private void IncreRowSpan55() { _cell55.RowSpan = _cell55.RowSpan + 2; }
        private void IncreRowSpan62() { _cell62.RowSpan = _cell62.RowSpan + 3; }
        private void IncreRowSpan83() { _cell83.RowSpan = _cell83.RowSpan + 2; }
        private void DecRowSpan22() { _cell22.RowSpan = _cell22.RowSpan - 1; }
        private void DecRowSpan34() { _cell34.RowSpan = _cell34.RowSpan - 2; }
        private void DecRowSpan43() { _cell43.RowSpan = _cell43.RowSpan - 1; }
        private void DecRowSpan62() { _cell62.RowSpan = _cell62.RowSpan - 2; }
        private void DecRowSpan83() { _cell83.RowSpan = _cell83.RowSpan - 1; }
      
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
    }
}
