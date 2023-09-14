using Avalon.Test.ComponentModel;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.Windows.Controls;
using System;

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
    [Test(0, "DataGrid", "DataGridColumnStarWidthChanges", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords = "Localization_Suite")]
    public class DataGridColumnStarWidthChanges : DataGridColumnUserResizingStarBase
    {
        #region Constructor

        public DataGridColumnStarWidthChanges()
            : this("DataGridColumnStarWidthChanges.xaml")
        {
        }

        [Variation("DataGridColumnStarWidthChanges.xaml")]
        public DataGridColumnStarWidthChanges(string filename)
            : base(filename)
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestUpdateWidthFromStarToNonStarAndStarColumnsExist);
            RunSteps += new TestStep(TestUpdateWidthFromNonStarToNonStarAndStarColumnsExist);
            RunSteps += new TestStep(TestUpdateWidthFromNonStarToStarAndStarColumnsExist);
            RunSteps += new TestStep(TestUpdateWidthFromStarToStar);
            RunSteps += new TestStep(TestUpdateWidthFromStarToNonStarAndNoMoreStarColumnsExist);
            RunSteps += new TestStep(TestUpdateWidthFromNonStarToNonStarAndNoStarColumnsExist);
            RunSteps += new TestStep(TestUpdateWidthFromNonStarToStarAndStarColumnsDidNotExist);
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
        /// 
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestUpdateWidthFromStarToNonStarAndStarColumnsExist()
        {
            Status("TestUpdateWidthFromStarToNonStarAndStarColumnsExist");

            Assert.AssertTrue("Star columns must exist initially in this step.", HasStarColumns());

            // get the current column widths
            DataGridHelper.ColumnWidthData[] expectedColumns = GetAllColumnData().ToArray();

            // find a star column 
            DataGridColumn columnToUpdate = null;
            int i = 0;
            foreach (DataGridColumn column in MyDataGrid.Columns)
            {
                if (column.Width.IsStar)
                {
                    columnToUpdate = column;
                    break;
                }
                i++;
            }
            Assert.AssertTrue("columnToUpdate must exist to continue step", columnToUpdate != null);

            // capture the old width
            DataGridLength oldWidth = columnToUpdate.Width;
            DataGridHelper.ColumnWidthData oldWidthData = new DataGridHelper.ColumnWidthData
            {
                unitType = oldWidth.UnitType,
                width = oldWidth.Value,
                desiredWidth = oldWidth.DesiredValue,
                displayWidth = oldWidth.DisplayValue,
                minWidth = columnToUpdate.MinWidth,
                maxWidth = columnToUpdate.MaxWidth,
                CanUserResize = columnToUpdate.CanUserResize
            };

            // set the new width to non-star
            DataGridLength newWidth = new DataGridLength(100.0);
            DataGridHelper.ColumnWidthData newWidthData = new DataGridHelper.ColumnWidthData
            {
                unitType = newWidth.UnitType,
                width = newWidth.Value,
                desiredWidth = newWidth.DesiredValue,
                displayWidth = newWidth.DisplayValue,
                minWidth = columnToUpdate.MinWidth,
                maxWidth = columnToUpdate.MaxWidth,
                CanUserResize = columnToUpdate.CanUserResize
            };
            expectedColumns[i] = newWidthData;

            // update the column
            columnToUpdate.Width = newWidth;
            QueueHelper.WaitTillQueueItemsProcessed();

            // then verify all columns after change
            double epsilon = 0.0;
            ComputeAndVerifyOnColumnWidthChange(ref expectedColumns, i, oldWidthData, epsilon);

            LogComment("TestUpdateWidthFromStarToNonStarAndStarColumnsExist was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestUpdateWidthFromNonStarToNonStarAndStarColumnsExist()
        {
            Status("TestUpdateWidthFromNonStarToNonStarAndStarColumnsExist");

            Assert.AssertTrue("Star columns must exist initially in this step.", HasStarColumns());

            // get the current column widths
            DataGridHelper.ColumnWidthData[] expectedColumns = GetAllColumnData().ToArray();

            // find a non-star column 
            DataGridColumn columnToUpdate = null;
            int i = 0;
            foreach (DataGridColumn column in MyDataGrid.Columns)
            {
                if (column.Width.IsAbsolute)
                {
                    columnToUpdate = column;
                    break;
                }
                i++;
            }
            Assert.AssertTrue("columnToUpdate must exist to continue step", columnToUpdate != null);

            // capture the old width
            DataGridLength oldWidth = columnToUpdate.Width;
            DataGridHelper.ColumnWidthData oldWidthData = new DataGridHelper.ColumnWidthData
            {
                unitType = oldWidth.UnitType,
                width = oldWidth.Value,
                desiredWidth = oldWidth.DesiredValue,
                displayWidth = oldWidth.DisplayValue,
                minWidth = columnToUpdate.MinWidth,
                maxWidth = columnToUpdate.MaxWidth,
                CanUserResize = columnToUpdate.CanUserResize
            };

            // update the column to non-star
            columnToUpdate.Width = DataGridLength.Auto;
            QueueHelper.WaitTillQueueItemsProcessed();

            // record the new expected width 
            DataGridLength newWidth = columnToUpdate.Width;
            DataGridHelper.ColumnWidthData newWidthData = new DataGridHelper.ColumnWidthData
            {
                unitType = newWidth.UnitType,
                width = newWidth.Value,
                desiredWidth = newWidth.DesiredValue,
                displayWidth = newWidth.DisplayValue,
                minWidth = columnToUpdate.MinWidth,
                maxWidth = columnToUpdate.MaxWidth,
                CanUserResize = columnToUpdate.CanUserResize
            };
            expectedColumns[i] = newWidthData;

            // then verify all columns after change
            double epsilon = 0.0;
            ComputeAndVerifyOnColumnWidthChange(ref expectedColumns, i, oldWidthData, epsilon);

            LogComment("TestUpdateWidthFromNonStarToNonStarAndStarColumnsExist was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestUpdateWidthFromNonStarToStarAndStarColumnsExist()
        {
            Status("TestUpdateWidthFromNonStarToStarAndStarColumnsExist");

            Assert.AssertTrue("Star columns must exist initially in this step.", HasStarColumns());

            // get the current column widths
            DataGridHelper.ColumnWidthData[] expectedColumns = GetAllColumnData().ToArray();

            // find a non-star column
            DataGridColumn columnToUpdate = null;
            int i = 0;
            foreach (DataGridColumn column in MyDataGrid.Columns)
            {
                if (column.Width.IsSizeToCells)
                {
                    columnToUpdate = column;
                    break;
                }
                i++;
            }
            Assert.AssertTrue("columnToUpdate must exist to continue step", columnToUpdate != null);

            // capture the old width
            DataGridLength oldWidth = columnToUpdate.Width;
            DataGridHelper.ColumnWidthData oldWidthData = new DataGridHelper.ColumnWidthData
            {
                unitType = oldWidth.UnitType,
                width = oldWidth.Value,
                desiredWidth = oldWidth.DesiredValue,
                displayWidth = oldWidth.DisplayValue,
                minWidth = columnToUpdate.MinWidth,
                maxWidth = columnToUpdate.MaxWidth,
                CanUserResize = columnToUpdate.CanUserResize
            };

            // record the new expected width 
            DataGridLength newWidth = new DataGridLength(1.0, DataGridLengthUnitType.Star);
            DataGridHelper.ColumnWidthData newWidthData = new DataGridHelper.ColumnWidthData
            {
                unitType = newWidth.UnitType,
                width = newWidth.Value,
                desiredWidth = newWidth.DesiredValue,
                displayWidth = newWidth.DisplayValue,
                minWidth = columnToUpdate.MinWidth,
                maxWidth = columnToUpdate.MaxWidth,
                CanUserResize = columnToUpdate.CanUserResize
            };
            expectedColumns[i] = newWidthData;

            // update the column to star
            columnToUpdate.Width = newWidth;
            QueueHelper.WaitTillQueueItemsProcessed();

            double epsilon = 0.0;
            ComputeAndVerifyOnColumnWidthChange(ref expectedColumns, i, oldWidthData, epsilon);

            LogComment("TestUpdateWidthFromNonStarToStarAndStarColumnsExist was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestUpdateWidthFromStarToStar()
        {
            Status("TestUpdateWidthFromStarToStar");

            Assert.AssertTrue("Star columns must exist initially in this step.", HasStarColumns());

            // get the current column widths
            DataGridHelper.ColumnWidthData[] expectedColumns = GetAllColumnData().ToArray();

            // find a star column
            DataGridColumn columnToUpdate = null;
            int i = 0;
            foreach (DataGridColumn column in MyDataGrid.Columns)
            {
                if (column.Width.IsStar)
                {
                    columnToUpdate = column;
                    break;
                }
                i++;
            }
            Assert.AssertTrue("columnToUpdate must exist to continue step", columnToUpdate != null);

            // capture the old width
            DataGridLength oldWidth = columnToUpdate.Width;
            DataGridHelper.ColumnWidthData oldWidthData = new DataGridHelper.ColumnWidthData
            {
                unitType = oldWidth.UnitType,
                width = oldWidth.Value,
                desiredWidth = oldWidth.DesiredValue,
                displayWidth = oldWidth.DisplayValue,
                minWidth = columnToUpdate.MinWidth,
                maxWidth = columnToUpdate.MaxWidth,
                CanUserResize = columnToUpdate.CanUserResize
            };

            // record the new expected width which is a different star
            DataGridLength newWidth = new DataGridLength(3.0, DataGridLengthUnitType.Star);
            DataGridHelper.ColumnWidthData newWidthData = new DataGridHelper.ColumnWidthData
            {
                unitType = newWidth.UnitType,
                width = newWidth.Value,
                desiredWidth = newWidth.DesiredValue,
                displayWidth = newWidth.DisplayValue,
                minWidth = columnToUpdate.MinWidth,
                maxWidth = columnToUpdate.MaxWidth,
                CanUserResize = columnToUpdate.CanUserResize
            };
            expectedColumns[i] = newWidthData;

            // update the column to star
            columnToUpdate.Width = newWidth;
            QueueHelper.WaitTillQueueItemsProcessed();

            double epsilon = 0.0;
            ComputeAndVerifyOnColumnWidthChange(ref expectedColumns, i, oldWidthData, epsilon);

            LogComment("TestUpdateWidthFromStarToStar was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestUpdateWidthFromStarToNonStarAndNoMoreStarColumnsExist()
        {
            Status("TestUpdateWidthFromStarToNonStarAndNoMoreStarColumnsExist");

            Assert.AssertTrue("Star columns must exist initially in this step.", HasStarColumns());

            bool starColumnsExist = true;
            while (starColumnsExist)
            {
                // get the current column widths
                DataGridHelper.ColumnWidthData[] expectedColumns = GetAllColumnData().ToArray();

                // find a star column
                DataGridColumn columnToUpdate = null;
                int i = 0;
                foreach (DataGridColumn column in MyDataGrid.Columns)
                {
                    if (column.Width.IsStar)
                    {
                        columnToUpdate = column;
                        break;
                    }
                    i++;
                }

                if (columnToUpdate == null)
                {
                    starColumnsExist = false;
                    break;
                }

                LogComment(string.Format("Begin updating star column to non-star at index: {0}", i));

                // capture the old width
                DataGridLength oldWidth = columnToUpdate.Width;
                DataGridHelper.ColumnWidthData oldWidthData = new DataGridHelper.ColumnWidthData
                {
                    unitType = oldWidth.UnitType,
                    width = oldWidth.Value,
                    desiredWidth = oldWidth.DesiredValue,
                    displayWidth = oldWidth.DisplayValue,
                    minWidth = columnToUpdate.MinWidth,
                    maxWidth = columnToUpdate.MaxWidth,
                    CanUserResize = columnToUpdate.CanUserResize
                };

                // update the column to non-star
                columnToUpdate.Width = 100.0;
                QueueHelper.WaitTillQueueItemsProcessed();

                // record the new expected width 
                DataGridLength newWidth = columnToUpdate.Width;
                DataGridHelper.ColumnWidthData newWidthData = new DataGridHelper.ColumnWidthData
                {
                    unitType = newWidth.UnitType,
                    width = newWidth.Value,
                    desiredWidth = newWidth.DesiredValue,
                    displayWidth = newWidth.DisplayValue,
                    minWidth = columnToUpdate.MinWidth,
                    maxWidth = columnToUpdate.MaxWidth,
                    CanUserResize = columnToUpdate.CanUserResize
                };
                expectedColumns[i] = newWidthData;

                double epsilon = 4.0;
                ComputeAndVerifyOnColumnWidthChange(ref expectedColumns, i, oldWidthData, epsilon);

                LogComment(string.Format("End updating star column to non-star at index: {0}", i));
            }

            LogComment("TestUpdateWidthFromStarToNonStarAndNoMoreStarColumnsExist was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestUpdateWidthFromNonStarToNonStarAndNoStarColumnsExist()
        {
            Status("TestUpdateWidthFromNonStarToNonStarAndNoStarColumnsExist");

            Assert.AssertTrue("Star columns must NOT exist initially in this step.", !HasStarColumns());

            // get the current column widths
            DataGridHelper.ColumnWidthData[] expectedColumns = GetAllColumnData().ToArray();

            // find a non-star column 
            DataGridColumn columnToUpdate = null;
            int i = 0;
            foreach (DataGridColumn column in MyDataGrid.Columns)
            {
                if (column.Width.IsAuto)
                {
                    columnToUpdate = column;
                    break;
                }
                i++;
            }
            Assert.AssertTrue("columnToUpdate must exist to continue step", columnToUpdate != null);

            // capture the old width
            DataGridLength oldWidth = columnToUpdate.Width;
            DataGridHelper.ColumnWidthData oldWidthData = new DataGridHelper.ColumnWidthData
            {
                unitType = oldWidth.UnitType,
                width = oldWidth.Value,
                desiredWidth = oldWidth.DesiredValue,
                displayWidth = oldWidth.DisplayValue,
                minWidth = columnToUpdate.MinWidth,
                maxWidth = columnToUpdate.MaxWidth,
                CanUserResize = columnToUpdate.CanUserResize
            };

            DataGridLength newWidth = new DataGridLength(400.0);
            DataGridHelper.ColumnWidthData newWidthData = new DataGridHelper.ColumnWidthData
            {
                unitType = newWidth.UnitType,
                width = newWidth.Value,
                desiredWidth = newWidth.DesiredValue,
                displayWidth = newWidth.DisplayValue,
                minWidth = columnToUpdate.MinWidth,
                maxWidth = columnToUpdate.MaxWidth,
                CanUserResize = columnToUpdate.CanUserResize
            };
            expectedColumns[i] = newWidthData;

            // update the column
            columnToUpdate.Width = newWidth;
            QueueHelper.WaitTillQueueItemsProcessed();

            // then verify all columns after change
            double epsilon = 0.0;
            ComputeAndVerifyOnColumnWidthChange(ref expectedColumns, i, oldWidthData, epsilon);

            LogComment("TestUpdateWidthFromNonStarToNonStarAndNoStarColumnsExist was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestUpdateWidthFromNonStarToStarAndStarColumnsDidNotExist()
        {
            Status("TestUpdateWidthFromNonStarToStarAndStarColumnsDidNotExist");

            Assert.AssertTrue("Star columns must NOT exist initially in this step.", !HasStarColumns());

            // get the current column widths
            DataGridHelper.ColumnWidthData[] expectedColumns = GetAllColumnData().ToArray();

            // find a non-star column
            DataGridColumn columnToUpdate = null;
            int i = 0;
            foreach (DataGridColumn column in MyDataGrid.Columns)
            {
                if (column.Width.IsAuto)
                {
                    columnToUpdate = column;
                    break;
                }
                i++;
            }
            Assert.AssertTrue("columnToUpdate must exist to continue step", columnToUpdate != null);

            // capture the old width
            DataGridLength oldWidth = columnToUpdate.Width;
            DataGridHelper.ColumnWidthData oldWidthData = new DataGridHelper.ColumnWidthData
            {
                unitType = oldWidth.UnitType,
                width = oldWidth.Value,
                desiredWidth = oldWidth.DesiredValue,
                displayWidth = oldWidth.DisplayValue,
                minWidth = columnToUpdate.MinWidth,
                maxWidth = columnToUpdate.MaxWidth,
                CanUserResize = columnToUpdate.CanUserResize
            };

            // record the new expected width 
            DataGridLength newWidth = new DataGridLength(1.0, DataGridLengthUnitType.Star);
            DataGridHelper.ColumnWidthData newWidthData = new DataGridHelper.ColumnWidthData
            {
                unitType = newWidth.UnitType,
                width = newWidth.Value,
                desiredWidth = newWidth.DesiredValue,
                displayWidth = newWidth.DisplayValue,
                minWidth = columnToUpdate.MinWidth,
                maxWidth = columnToUpdate.MaxWidth,
                CanUserResize = columnToUpdate.CanUserResize
            };
            expectedColumns[i] = newWidthData;

            // update the column to star
            columnToUpdate.Width = newWidth;
            QueueHelper.WaitTillQueueItemsProcessed();

            double epsilon = 0.0;
            ComputeAndVerifyOnColumnWidthChange(ref expectedColumns, i, oldWidthData, epsilon);

            LogComment("TestUpdateWidthFromNonStarToStarAndStarColumnsDidNotExist was successful");
            return TestResult.Pass;
        }

        #endregion Test Steps
    }
}
