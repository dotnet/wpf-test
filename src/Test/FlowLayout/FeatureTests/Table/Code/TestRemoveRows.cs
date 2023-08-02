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
    [Test(2, "Table", "RemoveRow", MethodName = "Run")]
    public class RemoveRow : AvalonTest
    {
        #region Test case members

        private Window _win;
        private int _testId;
        private Border _border;
        private BasicTable _table;
        private TableRowGroup _header;
        private TableRowGroup _body;
        private TableRowGroup _footer;
        private Table _root;
        private TableRow _header_row;
        private TableRow _bodyRow;
        private TableRow _footerRow;
        private string _inputString;        

        #endregion

        #region Constructor

        [Variation(1, "Test1")]
        [Variation(2, "Test2")]
        [Variation(3, "Test3", Priority = 3)]
        [Variation(4, "Test4")]
        [Variation(5, "Test5", Priority = 3)]
        [Variation(6, "Test6")]
        [Variation(7, "Test7", Priority = 3)]
        [Variation(8, "Test8")]
        [Variation(9, "Test9")]
        [Variation(10, "Test10")]
        [Variation(11, "Test11")]
        [Variation(12, "Test12")]
        [Variation(13, "Test13", Priority = 3)]

        public RemoveRow(int testValue, string testString)
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

            _table.CellSpacing = 3;
            _root.Name = "MyTable";
            for (int i = 0; i <= 5; ++i)
            {
                _table.CreateColumn(Brushes.LightPink);
            }

            _header = _table.CreateBody();
            _body = _table.CreateBody();
            _footer = _table.CreateBody();
            for (int i = 0; i < 5; ++i)
            {
                if ((i == 0) || (i == 4))
                {
                    _table.CreateRow(_header, Brushes.Red, 5);


                }
                else
                {
                    _table.CreateRow(_header, Brushes.Gold, 5);
                }
            }

            for (int i = 0; i < 5; ++i)
            {
                if ((i == 0) || (i == 2) || (i == 4))
                {
                    _table.CreateRow(_body, Brushes.Blue, 5);
                }
                else
                {
                    _table.CreateRow(_body, Brushes.Wheat, 5);
                }
            }

            for (int i = 0; i < 5; ++i)
            {
                if ((i == 0) || (i == 4))
                {
                    _table.CreateRow(_footer, Brushes.Salmon, 5);
                }
                else
                {
                    _table.CreateRow(_footer, Brushes.LightGreen, 5);
                }
            }
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
            _win.Resources.MergedDictionaries.Add(GenericStyles.LoadAllStyles());
            _win.SizeToContent = SizeToContent.WidthAndHeight;
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
            Status("Starting RunTests ....");

            switch (_testId)
            {
                case 1:
                    DeleteAllRows();
                    break;
                case 2:
                    DeleteAllheader_rowsRandomly();
                    break;
                case 3:
                    DeleteAllheader_rows();
                    break;
                case 4:
                    DeleteAllBodyRow();
                    break;
                case 5:
                    DeleteAllFooterRow();
                    break;
                case 6:
                    DeleteBodyRowRandomly();
                    break;
                case 7:
                    DeleteFooterRowRandomly();
                    break;
                case 8:
                    DeleteFirstheader_row();
                    break;
                case 9:
                    DeleteLastheader_row();
                    break;
                case 10:
                    DeleteFirstFooterRow();
                    break;
                case 11:
                    DeleteLastFooterRow();                    
                    break;
                case 12:
                    DeleteMiddleBodyRow();
                    break;
                case 13:
                    DeleteFirstheader_row();
                    DeleteLastheader_row();
                    DeleteFirstheader_row();
                    DeleteLastheader_row();
                    break;
                default:
                    LogComment("Invalid entry ....");
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

        private void DeleteAllRows()
        {
            for (int j = _root.RowGroups.Count - 1; j >= 0; --j)
            {
                for (int i = _root.RowGroups[j].Rows.Count - 1; i >= 0; --i)
                {
                    _root.RowGroups[j].Rows.RemoveAt(i);
                }
            }
        }

        private void DeleteAllheader_rowsRandomly()
        {
            int[] indexOrders = new int[5] { 0, 1, 2, 1, 3 };

            for (int i = _header.Rows.Count - 1; i >= 0; --i)
            {
                _header.Rows.RemoveAt(indexOrders[i]);
            }
        }

        private void DeleteAllheader_rows()
        {
            for (int i = _header.Rows.Count - 1; i >= 0; --i)
            {
                _header.Rows.RemoveAt(i);
                LogComment("Header Row " + i + " deleted");
            }
        }

        private void DeleteAllBodyRow()
        {

            for (int i = _body.Rows.Count - 1; i >= 0; --i)
            {
                _body.Rows.RemoveAt(i);
                LogComment("Body Row " + i + " deleted");
            }

        }

        private void DeleteAllFooterRow()
        {
            for (int i = _footer.Rows.Count - 1; i >= 0; --i)
            {
                _footer.Rows.RemoveAt(i);
                LogComment("Footer Row " + i + " deleted");
            }
        }

        private void DeleteBodyRowRandomly()
        {
            int[] indexOrders = new int[5] { 0, 1, 0, 1, 0 };

            for (int i = _body.Rows.Count - 1; i >= 0; --i)
            {
                _body.Rows.RemoveAt(indexOrders[i]);
            }
        }

        private void DeleteFooterRowRandomly()
        {
            int[] indexOrders = new int[5] { 0, 0, 1, 2, 1 };

            for (int i = _footer.Rows.Count - 1; i >= 0; --i)
            {
                _footer.Rows.RemoveAt(indexOrders[i]);
            }
        }

        private void DeleteFirstheader_row()
        {
            _header_row = _header.Rows[0];
            _header.Rows.Remove(_header_row);
        }

        private void DeleteLastheader_row()
        {
            _header_row = _header.Rows[_header.Rows.Count - 1];
            _header.Rows.Remove(_header_row);
        }

        private void DeleteFirstFooterRow()
        {
            _footerRow = _footer.Rows[0];
            _footer.Rows.Remove(_footerRow);
        }

        private void DeleteLastFooterRow()
        {
            _footerRow = _footer.Rows[_footer.Rows.Count - 1];
            _footer.Rows.Remove(_footerRow);
        }

        private void DeleteMiddleBodyRow()
        {            
            _bodyRow = _root.RowGroups[0].Rows[(_body.Rows.Count + 1) / 2];
            _root.RowGroups[0].Rows.Remove(_bodyRow);
        }
    }
}
