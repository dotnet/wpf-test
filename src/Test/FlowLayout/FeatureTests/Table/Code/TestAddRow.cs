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
    /// Testingadd rows to table.   
    /// </summary>
    [Test(0, "Table", "AddRow")]
    public class AddRow : AvalonTest
    {
        #region Test case members

        private Window _win;
        private int _testId;
        private Border _border;
        private BasicTable _table;
        private FlowDocumentScrollViewer _eRoot;
        private TableRowGroup _header;
        private TableRowGroup _body;
        private TableRowGroup _footer;
        private Table _root;
        private TableRow _header_row;
        private TableRow _bodyRow;
        private TableRow _footerRow;
        private int _X = 0;
        private int _Y = 0;        
        private string _inputString;

        #endregion

        [Variation(1, "Test1", Priority = 3)]
        [Variation(2, "Test2", Priority = 3)]
        [Variation(3, "Test3")]
        [Variation(4, "Test4")]
        [Variation(5, "Test5")]
        [Variation(6 , "Test6")]
        #region Constructor
      
        public AddRow(int testValue, string testString)
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
            Status("Initialize");
            _win = new Window();            
            Paragraph para = new Paragraph();
            _border = new Border();
            _eRoot = new FlowDocumentScrollViewer();
            _eRoot.Document = new FlowDocument();
            _border.Background = Brushes.White;
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
            for (int i = 0; i < 3; ++i)
            {
                _table.CreateRow(_header, Brushes.Gold, 4);
            }

            for (int i = 0; i < 3; ++i)
            {
                _table.CreateRow(_body, Brushes.Wheat, 4);
            }

            for (int i = 0; i < 3; ++i)
            {
                _table.CreateRow(_footer, Brushes.LightGreen, 4);
            }

            _eRoot.Document.Blocks.Add(_table.Tbl);
            _border.Child = _eRoot;
            _win.Content = _border;
            _win.SizeToContent = SizeToContent.WidthAndHeight;
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
            LogComment("RunTests begins ....");
            switch (_testId)
            {
                case 1:
                    InsertFirstheader_row();
                    break;
                case 2:
                    InsertLastheader_row();
                    break;
                case 3:
                    InsertMiddleBodyRow();
                    break;
                case 4:
                    InsertFirstFooterRow();
                    break;
                case 5:
                    InsertLastFooterRow();
                    break;
                case 6:
                    InsertBodyRow();
                    RemoveBodyRow();
                    InsertBodyRow();
                    RemoveBodyRow();
                    InsertBodyRow();
                    RemoveBodyRow();
                    break;
                default:
                    LogComment("Invalid entry....");
                    Log.Result = TestResult.Fail;
                    break;
            }

            if (Log.Result != TestResult.Fail)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }
       
        /// <summary>
        /// VerifyTest: Verifies the test result
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult VerifyTest()
        {
            bool tempResult = false;

            WaitForPriority(DispatcherPriority.ApplicationIdle);            
            LogComment("Starting property dump verification...");
            try
            {
                PropertyDumpHelper helper = new PropertyDumpHelper((Visual)_win.Content);
                tempResult = helper.CompareLogShow(new Arguments(this));
            }
            catch (System.Xml.XmlException)
            {
                return TestResult.Ignore;
            }

            switch (_testId)
            {
                case 1:
                    if (_header.Rows.Count == 4)
                    {
                        LogComment("Pass, The first header row was inserted ");
                        Log.Result = TestResult.Pass;
                    }
                    else
                    {
                        LogComment("Fail, The first header row was not inserted properly");
                        Log.Result = TestResult.Fail;
                    }
                    break;
                case 2:
                    if (_header.Rows.Count == 4)
                    {
                        LogComment("Pass, The last header row was inserted ");
                        Log.Result = TestResult.Pass;
                    }
                    else
                    {
                        LogComment("Fail, The last header row was not inserted properly");
                        Log.Result = TestResult.Fail;
                    }
                    break;
                case 3:
                    if (_body.Rows.Count == 4)
                    {
                        LogComment("Pass, The middle body row was inserted ");
                        Log.Result = TestResult.Pass;
                    }
                    else
                    {
                        LogComment("Fail, The middle body row was not inserted properly");
                        Log.Result = TestResult.Fail;
                    }
                    break;
                case 4:
                    if (_footer.Rows.Count == 4)
                    {
                        LogComment("Pass, The first footer row was inserted ");
                        Log.Result = TestResult.Pass;
                    }
                    else
                    {
                        LogComment("Fail, The first footer row was not inserted properly");
                        Log.Result = TestResult.Fail;
                    }
                    break;

                case 5:
                    if (_footer.Rows.Count == 4)
                    {
                        LogComment("Pass, The last footer row was inserted ");
                        Log.Result = TestResult.Pass;
                    }
                    else
                    {
                        LogComment("Fail, The last footer row was not inserted properly");
                        Log.Result = TestResult.Fail;
                    }
                    break;

                case 6:
                    if (_body.Rows.Count == 3)
                    {
                        LogComment("Pass, The body row was inserted and removed properly");
                        Log.Result = TestResult.Pass;
                    }
                    else
                    {
                        LogComment("Fail, The body row was not inserted and removed properly");
                        Log.Result = TestResult.Fail;
                    }
                    break;

                default:
                    LogComment("Invalid entry....");
                    break;
            }

            if (tempResult)
            {
                return Log.Result;
            }
            else
            {
                return TestResult.Fail;
            }
        }

        #endregion

        #region Methods
        private void InsertFirstheader_row()
        {
            _header_row = new TableRow();
            _header_row.Background = Brushes.Transparent;
            for (int i = 1; i <= 2; ++i)
            {
                _table.CreateCell(_header_row);
            }
            _header.Rows.Insert(0, _header_row);
        }

        private void InsertLastheader_row()
        {
            _header_row = new TableRow();
            _header_row.Background = Brushes.Turquoise;
            for (int i = 1; i <= 2; ++i)
            {
                _table.CreateCell(_header_row);
            }
            _header.Rows.Insert(_header.Rows.Count, _header_row);
        }

        private void InsertMiddleBodyRow()
        {
            _bodyRow = new TableRow();
            _bodyRow.Background = Brushes.Turquoise;
            for (int i = 1; i <= 2; i++)
            {
                _table.CreateCell(_bodyRow);
            }
            _body.Rows.Insert((_body.Rows.Count) % 2, _bodyRow);
        }

        private void InsertFirstFooterRow()
        {
            _footerRow = new TableRow();
            _footerRow.Background = Brushes.Transparent;
            for (int i = 1; i <= 2; ++i)
            {
                _table.CreateCell(_footerRow);
            }
            _footer.Rows.Insert(0, _footerRow);
        }

        private void InsertLastFooterRow()
        {
            _footerRow = new TableRow();
            _footerRow.Background = Brushes.Turquoise;
            for (int i = 1; i <= 2; ++i)
            {
                _table.CreateCell(_footerRow);
            }
            _footer.Rows.Insert(_footer.Rows.Count, _footerRow);
        }

        private void InsertBodyRow()
        {
            int[] indexOrder = new int[3] { 0, 2, 1 };

            _bodyRow = new TableRow();
            _bodyRow.Background = Brushes.Purple;
            for (int i = 0; i <= 3; i++)
            {
                _table.CreateCell(_bodyRow);
            }

            _body.Rows.Insert(indexOrder[_X], _bodyRow);
            _X++;
        }

        private void RemoveBodyRow()
        {
            int[] indexOrder = new int[3] { 2, 3, 2 };

            _body.Rows.RemoveAt(indexOrder[_Y]);
            _Y++;
        }

        #endregion
    }
}
