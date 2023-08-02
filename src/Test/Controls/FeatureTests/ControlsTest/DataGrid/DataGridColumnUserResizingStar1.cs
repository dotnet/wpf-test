using System;
using System.Collections.Generic;
using System.Linq;
using Avalon.Test.ComponentModel;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.Windows.Controls;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Tests end-user column width resizing for star columns.  The action for this test case is as follows:
    /// 
    /// 1. resize column header right
    /// 2. resize column header right to max
    /// 3. resize column header back to starting point
    /// 4. resize column header to min
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridColumnUserResizingStar1", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DataGridColumnUserResizingStar1 : DataGridColumnUserResizingStarBase
    {
        #region Constructor

        public DataGridColumnUserResizingStar1()
            : this("DataGridColumnUserResizingStar5.xaml")
        {
        }

        [Variation("DataGridColumnUserResizingStar.xaml")]
        [Variation("DataGridColumnUserResizingStar2.xaml")]
        [Variation("DataGridColumnUserResizingStar3.xaml")]
        [Variation("DataGridColumnUserResizingStar4.xaml")]
        //[Variation("DataGridColumnUserResizingStar5.xaml")]
        public DataGridColumnUserResizingStar1(string filename)
            : base(filename)
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestResizingColumnRight);
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
        /// Verifies each column width, desired, and actual values.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestResizingColumnRight()
        {
            Status("TestResizingColumnRight");

            double epsilon = 4;

            foreach (int colIndex in new[] { 0, 1, 2, 4, 6, 8 })
            {
                LogComment(string.Format("Begin test for column: {0}", colIndex));

                double startingPoint = 0.0;
                DataGridHelper.ColumnWidthData[] expectedColumns;

                List<DataGridHelper.ColumnWidthData> prevColumns = GetAllColumnData();
                double unusedSpace = GetUnusedSpaceFromDataGrid(prevColumns.ToArray(), 0.0, false);
                if (DataGridHelper.AreClose(unusedSpace, 0.0, 0.01) && colIndex == MyDataGrid.Columns.Count - 1)
                {
                    continue;
                }

                LogComment("resize column header 10 units and verify");
                startingPoint = prevColumns[colIndex].displayWidth;
                double resizeDiff = 10;
                DataGridActionHelper.ResizeColumnHeader(MyDataGrid, colIndex, resizeDiff + epsilon);
                this.WaitFor(250);
                this.ComputeAndVerifyColumnWidthsOnResize(prevColumns, out expectedColumns, colIndex, resizeDiff, true, epsilon);

                // for the last column and no unused space,
                // does not do anymore resizing right as clicking on the gripper will have a side effect of changing the 
                // width to auto.  verification is enough from the first resize above
                unusedSpace = GetUnusedSpaceFromDataGrid(expectedColumns, 0.0, false);
                if (DataGridHelper.AreClose(unusedSpace, 0.0, 0.01) && colIndex == MyDataGrid.Columns.Count - 1)
                {
                    continue;
                }

                LogComment("resize column header 20 units and verify");
                prevColumns = expectedColumns.ToList<DataGridHelper.ColumnWidthData>();
                resizeDiff = 20;
                DataGridActionHelper.ResizeColumnHeader(MyDataGrid, colIndex, resizeDiff + epsilon);
                this.WaitFor(250);
                this.ComputeAndVerifyColumnWidthsOnResize(prevColumns, out expectedColumns, colIndex, resizeDiff, true, epsilon);

                LogComment("resize column header to max and verify");
                prevColumns = expectedColumns.ToList<DataGridHelper.ColumnWidthData>();
                if (Double.IsInfinity(prevColumns[colIndex].maxWidth))
                {
                    resizeDiff = 1000;
                    DataGridActionHelper.ResizeColumnHeader(MyDataGrid, colIndex, resizeDiff + epsilon);
                    this.WaitFor(250);                  
                    this.ComputeAndVerifyColumnWidthsOnResize(prevColumns, out expectedColumns, colIndex, resizeDiff, true, epsilon);
                }
                else
                {
                    resizeDiff = prevColumns[colIndex].maxWidth + 10;
                    DataGridActionHelper.ResizeColumnHeader(MyDataGrid, colIndex, resizeDiff + epsilon);
                    this.WaitFor(250);
                    this.ComputeAndVerifyColumnWidthsOnResize(prevColumns, out expectedColumns, colIndex, resizeDiff, true, epsilon);
                }

                prevColumns = GetAllColumnData();
                resizeDiff = prevColumns[colIndex].displayWidth - startingPoint;
                LogComment(string.Format("resize column header back down to starting point and verify.  resizeDiff: {0}", resizeDiff));
                DataGridActionHelper.ResizeColumnHeader(MyDataGrid, colIndex, -(resizeDiff - epsilon));
                this.WaitFor(250);
                this.ComputeAndVerifyColumnWidthsOnResize(prevColumns, out expectedColumns, colIndex, resizeDiff, false, epsilon);

                prevColumns = GetAllColumnData();
                resizeDiff = prevColumns[colIndex].displayWidth - prevColumns[colIndex].minWidth;
                LogComment(string.Format("resize column to min and verify. resizeDiff: {0}", resizeDiff));
                DataGridActionHelper.ResizeColumnHeader(MyDataGrid, colIndex, -(resizeDiff - epsilon));
                this.WaitFor(250);
                this.ComputeAndVerifyColumnWidthsOnResize(prevColumns, out expectedColumns, colIndex, resizeDiff, false, epsilon);

                LogComment(string.Format("End test for column: {0}", colIndex));
            }

            LogComment("TestResizingColumnRight was successful");
            return TestResult.Pass;
        }

        #endregion Test Steps
    }
}
