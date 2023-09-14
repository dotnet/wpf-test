using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Layout;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    ////////////////////////////////////////////////////////////////////////////////////////////
    // DISABLEDUNSTABLETEST:
    // TestName: DataGridColumnUserResizingStar2
    // Area: Controls   SubArea: DataGrid
    // Disable this case due to high fail rate, will enable after fix it.
    // to find all disabled tests in test tree, use: “findstr /snip DISABLEDUNSTABLETEST” 
    ////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// <description>
    /// Tests end-user column width resizing for star columns.  The action for this test case is as follows:
    /// 
    /// 1. resize column header left
    /// 2. resize column header left to min
    /// 3. resize column header back to starting point
    /// 4. resize column header to max
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridColumnUserResizingStar2", SecurityLevel = TestCaseSecurityLevel.FullTrust, Disabled = true)]
    public class DataGridColumnUserResizingStar2 : DataGridColumnUserResizingStarBase
    {
        #region Constructor

        public DataGridColumnUserResizingStar2()
            : this("DataGridColumnUserResizingStar.xaml")
        {
        }

        [Variation("DataGridColumnUserResizingStar.xaml")]
        [Variation("DataGridColumnUserResizingStar2.xaml")]
        [Variation("DataGridColumnUserResizingStar3.xaml")]
        [Variation("DataGridColumnUserResizingStar4.xaml")]
        //[Variation("DataGridColumnUserResizingStar5.xaml")]
        public DataGridColumnUserResizingStar2(string filename)
            : base(filename)
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestResizingColumnLeft);
        }

        #endregion

        #region Test Steps

        /// <summary>
        /// Initial Setup  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        public override TestResult Setup()
        {
            base.Setup();

            Status("Setup specific for DataGridColumnResizing");
                  

            LogComment("Setup for DataGridColumnResizing was successful");
            return TestResult.Pass;
        }                        

        /// <summary>
        /// Resize column 0 left to min width, then back to start point, then to max width.  Verifies each column width, desired,
        /// and actual values.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestResizingColumnLeft()
        {
            Status("TestResizingColumnLeft");

            LogComment("allow column resizing");
            double epsilon = 4;

            foreach (int colIndex in new[] { 0, 1, 2, 4, 6, 8 })
            {
                LogComment(string.Format("Begin test for column: {0}", colIndex));

                double startingPoint = 0.0;
                DataGridHelper.ColumnWidthData[] expectedColumns;

                LogComment("resize column header -10 units and verify");
                List<DataGridHelper.ColumnWidthData> prevColumns = GetAllColumnData();
                startingPoint = prevColumns[colIndex].displayWidth;
                double resizeDiff = 10;
                DataGridActionHelper.ResizeColumnHeader(MyDataGrid, colIndex, -(resizeDiff - epsilon));
                this.ComputeAndVerifyColumnWidthsOnResize(prevColumns, out expectedColumns, colIndex, resizeDiff, false, epsilon);
                                
                LogComment("resize column header -20 units and verify");
                prevColumns = expectedColumns.ToList<DataGridHelper.ColumnWidthData>();
                resizeDiff = 20;
                DataGridActionHelper.ClickOnColumnHeader(MyDataGrid, colIndex);
                DataGridActionHelper.ResizeColumnHeader(MyDataGrid, colIndex, -(resizeDiff - epsilon));

                this.ComputeAndVerifyColumnWidthsOnResize(prevColumns, out expectedColumns, colIndex, resizeDiff, false, epsilon);

                LogComment("resize column header to min and verify");
                prevColumns = expectedColumns.ToList<DataGridHelper.ColumnWidthData>();
                resizeDiff = prevColumns[colIndex].minWidth - 10;
                DataGridActionHelper.ClickOnColumnHeader(MyDataGrid, colIndex);
                DataGridActionHelper.ResizeColumnHeader(MyDataGrid, colIndex, -(resizeDiff - epsilon));
                this.ComputeAndVerifyColumnWidthsOnResize(prevColumns, out expectedColumns, colIndex, resizeDiff, false, epsilon);

                LogComment("resize column header back up to starting point and verify");
                prevColumns = expectedColumns.ToList<DataGridHelper.ColumnWidthData>();
                resizeDiff = startingPoint - prevColumns[colIndex].displayWidth;
                DataGridActionHelper.ClickOnColumnHeader(MyDataGrid, colIndex);
                DataGridActionHelper.ResizeColumnHeader(MyDataGrid, colIndex, resizeDiff + epsilon);
                this.ComputeAndVerifyColumnWidthsOnResize(prevColumns, out expectedColumns, colIndex, resizeDiff, true, epsilon);

                LogComment("resize column to max and verify");
                prevColumns = expectedColumns.ToList<DataGridHelper.ColumnWidthData>();
                if (Double.IsInfinity(prevColumns[colIndex].maxWidth))
                {
                    resizeDiff = 2000;
                    DataGridActionHelper.ClickOnColumnHeader(MyDataGrid, colIndex);
                    DataGridActionHelper.ResizeColumnHeader(MyDataGrid, colIndex, resizeDiff + epsilon);
                    this.ComputeAndVerifyColumnWidthsOnResize(prevColumns, out expectedColumns, colIndex, resizeDiff, true, epsilon);
                }
                else
                {
                    resizeDiff = prevColumns[colIndex].maxWidth - prevColumns[colIndex].displayWidth;
                    DataGridActionHelper.ClickOnColumnHeader(MyDataGrid, colIndex);
                    DataGridActionHelper.ResizeColumnHeader(MyDataGrid, colIndex, resizeDiff + epsilon);
                    this.ComputeAndVerifyColumnWidthsOnResize(prevColumns, out expectedColumns, colIndex, resizeDiff, true, epsilon);
                }

                LogComment("resize column header back down to starting point and verify");
                prevColumns = expectedColumns.ToList<DataGridHelper.ColumnWidthData>();
                resizeDiff = prevColumns[colIndex].displayWidth - startingPoint;
                if (DoubleUtil.GreaterThan(resizeDiff, 0.0))
                {
                    DataGridActionHelper.ClickOnColumnHeader(MyDataGrid, colIndex);
                    DataGridActionHelper.ResizeColumnHeader(MyDataGrid, colIndex, -(resizeDiff - epsilon));
                    this.ComputeAndVerifyColumnWidthsOnResize(prevColumns, out expectedColumns, colIndex, resizeDiff, false, epsilon);
                }

                LogComment(string.Format("End test for column: {0}", colIndex));
            }

            LogComment("TestResizingColumnLeft was successful");
            return TestResult.Pass;
        }

        #endregion Test Steps        
    }
}
