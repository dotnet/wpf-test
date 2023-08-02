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
    /// Testing Inflate row capacity in table.   
    /// </summary>
    [Test(2, "Table", "InFlateRowCapacity", MethodName="Run", Variables="VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]
    public class InFlateRowCapacity : AvalonTest
    {
        #region Test case members

        private Window _win;
        private Border _border;
        private BasicTable _table;        
        private TableRow _row1,_row2,_row3,_row4,_row5,_row;
        private TableCell _newcell;
        private int _X = 0;

        #endregion

        #region Constructor       

        public InFlateRowCapacity()
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
            _table.CellSpacing = 4;
            _table.CreateColumn(new GridLength(100), Brushes.Green);
            _table.CreateColumn(new GridLength(100), Brushes.Gold);
            _table.CreateColumn(new GridLength(100), Brushes.Crimson);
            _table.CreateColumn(new GridLength(100), Brushes.Green);
            _table.CreateColumn(new GridLength(100), Brushes.Gold);
            _table.CreateColumn(new GridLength(100), Brushes.Crimson);

            TableRowGroup body = _table.CreateBody();
            _row1 = _table.CreateRow(body);
            _table.CreateCell(_row1);
            _table.CreateCell(_row1);
            _table.CreateCell(_row1);
            _table.CreateCell(_row1);
            _table.CreateCell(_row1);
            _table.CreateCell(_row1);

            _row = _table.CreateRow(body);
            _table.CreateCell(_row);
            _table.CreateCell(_row);
            _table.CreateCell(_row);
            _table.CreateCell(_row);
            _table.CreateCell(_row);
            _table.CreateCell(_row);

            _row2 = _table.CreateRow(body);
            _table.CreateCell(_row2);
            _table.CreateCell(_row2);
            _table.CreateCell(_row2);
            _table.CreateCell(_row2);
            _table.CreateCell(_row2);
            _table.CreateCell(_row2);

            _row = _table.CreateRow(body);
            _table.CreateCell(_row);
            _table.CreateCell(_row);
            _table.CreateCell(_row);
            _table.CreateCell(_row);
            _table.CreateCell(_row);
            _table.CreateCell(_row);

            _row = _table.CreateRow(body);
            _table.CreateCell(_row);
            _table.CreateCell(_row);
            _table.CreateCell(_row);
            _table.CreateCell(_row);
            _table.CreateCell(_row);
            _table.CreateCell(_row);

            _row3 = _table.CreateRow(body);
            _table.CreateCell(_row3);
            _table.CreateCell(_row3);
            _table.CreateCell(_row3);
            _table.CreateCell(_row3);
            _table.CreateCell(_row3);
            _table.CreateCell(_row3);

            _row = _table.CreateRow(body);
            _table.CreateCell(_row);
            _table.CreateCell(_row);
            _table.CreateCell(_row);
            _table.CreateCell(_row);
            _table.CreateCell(_row);
            _table.CreateCell(_row);

            _row4 = _table.CreateRow(body);
            _table.CreateCell(_row4);
            _table.CreateCell(_row4);
            _table.CreateCell(_row4);
            _table.CreateCell(_row4);
            _table.CreateCell(_row4);
            _table.CreateCell(_row4);

            _row = _table.CreateRow(body);
            _table.CreateCell(_row);
            _table.CreateCell(_row);
            _table.CreateCell(_row);
            _table.CreateCell(_row);
            _table.CreateCell(_row);
            _table.CreateCell(_row);

            _row = _table.CreateRow(body);
            _table.CreateCell(_row);
            _table.CreateCell(_row);
            _table.CreateCell(_row);
            _table.CreateCell(_row);
            _table.CreateCell(_row);
            _table.CreateCell(_row);

            _row5 = _table.CreateRow(body);
            _table.CreateCell(_row5);
            _table.CreateCell(_row5);
            _table.CreateCell(_row5);
            _table.CreateCell(_row5);
            _table.CreateCell(_row5);
            _table.CreateCell(_row5);

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
            LogComment("Runtest started ....");
            InsertCell();
            InsertCell();
            InsertCell();
            InsertCell();
            InsertCell();

            return TestResult.Pass;
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

        private void InsertCell()
        {
            TableRow[] rowIndex = new TableRow[5] { _row1, _row2, _row3, _row4, _row5 };
            int z = 0;

            _newcell = new TableCell();
            _newcell.Background = Brushes.LightBlue;
            _newcell.BorderBrush = Brushes.Black;
            _newcell.BorderThickness = new Thickness(1);
            rowIndex[_X].Cells.Insert(z, _newcell);
            z++; _X++;

            if (z > 4)
            {
                z = 0;
            }
        }
    }
}
