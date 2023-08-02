using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Avalon.Test.ComponentModel;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;


namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Tests MaxWidth changes for star columns.  
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridColumnStarMaxWidthChanges", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords = "Localization_Suite")]
    public class DataGridColumnStarMaxWidthChanges : DataGridColumnUserResizingStarBase
    {
        #region Constructor

        public DataGridColumnStarMaxWidthChanges()
            : this("DataGridColumnStarMaxWidthChanges.xaml")
        {
        }

        [Variation("DataGridColumnStarMaxWidthChanges.xaml")]
        public DataGridColumnStarMaxWidthChanges(string filename)
            : base(filename)
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestPrecedenceofMaxColumnWidthAndMaxWidth);
            RunSteps += new TestStep(TestPrecedenceofMaxColumnWidthAndMaxWidth2);
            RunSteps += new TestStep(TestUpdateMaxWidthFromNonStarToBeLessThanDisplay);
            RunSteps += new TestStep(TestUpdateMaxWidthFromNonStarToBeLessThanDisplay2);
            RunSteps += new TestStep(TestUpdateMaxWidthFromStarToBeLessThanDisplay);
            RunSteps += new TestStep(TestUpdateMaxWidthFromStarToBeLessThanDisplay2);
            RunSteps += new TestStep(TestUpdateMaxColumnWidthToBeLessThanViewportWidth);            
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
        /// Verify precedence when settings MaxColumnWidth and MaxWidth on initial load
        /// 
        /// -MaxColumnWidth is set to default
        /// -Column0 MaxWidth set explicitly to 150
        /// -Column4 MaxWidth set explicitly to 150
        /// -Column6 MaxWidth set explicitly to 150
        /// -Column8 MaxWidth set explicitly to 75
        /// -all other columns take MaxColumnWidth value
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestPrecedenceofMaxColumnWidthAndMaxWidth()
        {
            Status("TestPrecedenceofMaxColumnWidthAndMaxWidth");

            if (!Double.IsPositiveInfinity(MyDataGrid.MaxColumnWidth))
            {
                throw new TestValidationException(string.Format(
                    "Expected MaxColumnWidth: {0}, Actual: {1}",
                    Double.PositiveInfinity,
                    MyDataGrid.MaxColumnWidth));
            }

            int i = 0;
            foreach (DataGridColumn column in MyDataGrid.Columns)
            {
                if (i == 0)
                {
                    if (column.MaxWidth != 150.0)
                    {
                        throw new TestValidationException(string.Format(
                            "Expected column: {0}, MaxWidth: {1}, Actual: {2}",
                            i,
                            150.0,
                            column.MaxWidth));
                    }
                }
                else if (i == 4)
                {
                    if (column.MaxWidth != 150.0)
                    {
                        throw new TestValidationException(string.Format(
                            "Expected column: {0}, MaxWidth: {1}, Actual: {2}",
                            i,
                            150.0,
                            column.MaxWidth));
                    }
                }
                else if (i == 6)
                {
                    if (column.MaxWidth != 150.0)
                    {
                        throw new TestValidationException(string.Format(
                            "Expected column: {0}, MaxWidth: {1}, Actual: {2}",
                            i,
                            150.0,
                            column.MaxWidth));
                    }
                }
                else if (i == 8)
                {
                    if (column.MaxWidth != 75.0)
                    {
                        throw new TestValidationException(string.Format(
                            "Expected column: {0}, MaxWidth: {1}, Actual: {2}",
                            i,
                            75.0,
                            column.MaxWidth));
                    }
                }
                else
                {
                    if(column.Width.IsStar && column.MaxWidth != 10000.0)
                    {
                        throw new TestValidationException(string.Format(
                            "Expected column: {0}, MaxWidth: {1}, Actual: {2}",
                            i,
                            10000.0,
                            column.MaxWidth));
                    }
                    else if (!column.Width.IsStar && !Double.IsPositiveInfinity(column.MaxWidth))                    
                    {
                        throw new TestValidationException(string.Format(
                            "Expected column: {0}, MaxWidth: {1}, Actual: {2}",
                            i,
                            Double.PositiveInfinity,
                            column.MaxWidth));
                    }
                }

                i++;
            }

            LogComment("TestPrecedenceofMaxColumnWidthAndMaxWidth was successful");
            return TestResult.Pass;
        }                

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestUpdateMaxWidthFromNonStarToBeLessThanDisplay()
        {
            Status("TestUpdateMaxWidthFromNonStarToBeLessThanDisplay");

            // get the current column widths
            DataGridHelper.ColumnWidthData[] prevColumns = GetAllColumnData().ToArray();
            DataGridHelper.ColumnWidthData[] expectedColumns = GetAllColumnData().ToArray();

            // find a non-star column 
            DataGridColumn columnToUpdate = null;
            int i = 0;
            foreach (DataGridColumn column in MyDataGrid.Columns)
            {
                if (column.Width.IsAuto && 
                    DoubleUtil.LessThan(column.Width.DesiredValue, column.MaxWidth) &&
                    DoubleUtil.GreaterThan(column.Width.DesiredValue, column.MinWidth))
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

            // update expected data
            double maxWidth = columnToUpdate.ActualWidth - 10.0;
            maxWidth = DoubleUtil.GreaterThan(maxWidth, columnToUpdate.MinWidth) ? maxWidth : columnToUpdate.MinWidth + 1;
            expectedColumns[i].maxWidth = maxWidth;

            LogComment(string.Format("update MaxWidth: {0}", maxWidth));
            columnToUpdate.MaxWidth = maxWidth;
            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("verify on max width change");
            double epsilon = 0.0;
            ComputeAndVerifyOnColumnMaxWidthChange(ref expectedColumns, i, oldWidthData, epsilon);

            LogComment(string.Format("update MaxWidth back to original: {0}", oldWidthData.maxWidth));
            columnToUpdate.MaxWidth = oldWidthData.maxWidth;
            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("verify columns go back to original");
            VerifyColumnWidths(prevColumns, epsilon);       

            LogComment("TestUpdateMaxWidthFromNonStarToBeLessThanDisplay was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Update MaxWidth so new MaxWidth is greater than old MaxWidth and old MaxWidth 
        /// was less than width.DesiredValue.  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestUpdateMaxWidthFromNonStarToBeLessThanDisplay2()
        {
            Status("TestUpdateMaxWidthFromNonStarToBeLessThanDisplay2");

            // get the current column widths
            DataGridHelper.ColumnWidthData[] expectedColumns = GetAllColumnData().ToArray();

            // find a non-star column 
            DataGridColumn columnToUpdate = null;
            int i = 0;
            foreach (DataGridColumn column in MyDataGrid.Columns)
            {
                if (column.Width.IsAuto &&
                    DoubleUtil.LessThan(column.Width.DesiredValue, column.MaxWidth) &&
                    DoubleUtil.GreaterThan(column.Width.DesiredValue, column.MinWidth))
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

            // update expected data
            double maxWidth = columnToUpdate.ActualWidth - 10.0;
            maxWidth = DoubleUtil.GreaterThan(maxWidth, columnToUpdate.MinWidth) ? maxWidth : columnToUpdate.MinWidth + 1;
            expectedColumns[i].maxWidth = maxWidth;

            LogComment("update MaxWidth");
            columnToUpdate.MaxWidth = maxWidth;
            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("verify on max width change");            
            double epsilon = 0.0;
            ComputeAndVerifyOnColumnMaxWidthChange(ref expectedColumns, i, oldWidthData, epsilon);

            expectedColumns = GetAllColumnData().ToArray();

            // capture the old width
            DataGridLength oldWidth2 = columnToUpdate.Width;
            DataGridHelper.ColumnWidthData oldWidthData2 = new DataGridHelper.ColumnWidthData
            {
                unitType = oldWidth.UnitType,
                width = oldWidth.Value,
                desiredWidth = oldWidth.DesiredValue,
                displayWidth = oldWidth.DisplayValue,
                minWidth = columnToUpdate.MinWidth,
                maxWidth = columnToUpdate.MaxWidth,
                CanUserResize = columnToUpdate.CanUserResize
            };

            LogComment("update MaxWidth to be less than current MaxWidth");
            double updatedMaxWidth = columnToUpdate.MaxWidth - 10;
            updatedMaxWidth = DoubleUtil.GreaterThan(updatedMaxWidth, columnToUpdate.MinWidth) ? updatedMaxWidth : columnToUpdate.MinWidth + 1;
            expectedColumns[i].maxWidth = updatedMaxWidth;

            LogComment("update MaxWidth again");            
            columnToUpdate.MaxWidth = updatedMaxWidth;

            LogComment("verify on max width change again");                        
            ComputeAndVerifyOnColumnMaxWidthChange(ref expectedColumns, i, oldWidthData2, epsilon);

            LogComment("TestUpdateMaxWidthFromNonStarToBeLessThanDisplay2 was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestUpdateMaxWidthFromStarToBeLessThanDisplay()
        {
            Status("TestUpdateMaxWidthFromStarToBeLessThanDisplay");

            // get the current column widths
            DataGridHelper.ColumnWidthData[] prevColumns = GetAllColumnData().ToArray();
            DataGridHelper.ColumnWidthData[] expectedColumns = GetAllColumnData().ToArray();

            // find a non-star column 
            DataGridColumn columnToUpdate = null;
            int i = 0;
            foreach (DataGridColumn column in MyDataGrid.Columns)
            {
                if (column.Width.IsStar &&
                    DoubleUtil.LessThan(column.Width.DesiredValue, column.MaxWidth) &&
                    DoubleUtil.GreaterThan(column.Width.DesiredValue, column.MinWidth))
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

            // update expected data
            double maxWidth = columnToUpdate.ActualWidth - 10.0;
            expectedColumns[i].maxWidth = maxWidth;

            LogComment("update MaxWidth");
            columnToUpdate.MaxWidth = maxWidth;
            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("verify on max width change");            
            double epsilon = 0.0;
            ComputeAndVerifyOnColumnMaxWidthChange(ref expectedColumns, i, oldWidthData, epsilon);

            LogComment("update MaxWidth back to original");
            columnToUpdate.MaxWidth = oldWidthData.maxWidth;
            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("verify columns go back to original");
            VerifyColumnWidths(prevColumns, epsilon);

            LogComment("TestUpdateMaxWidthFromStarToBeLessThanDisplay was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Update MaxWidth so new MaxWidth is greater than old MaxWidth and old MaxWidth 
        /// was less than width.DesiredValue.  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestUpdateMaxWidthFromStarToBeLessThanDisplay2()
        {
            Status("TestUpdateMaxWidthFromStarToBeLessThanDisplay2");

            // get the current column widths
            DataGridHelper.ColumnWidthData[] expectedColumns = GetAllColumnData().ToArray();

            // find a non-star column 
            DataGridColumn columnToUpdate = null;
            int i = 0;
            foreach (DataGridColumn column in MyDataGrid.Columns)
            {
                if (column.Width.IsStar &&
                    DoubleUtil.LessThan(column.Width.DesiredValue, column.MaxWidth) &&
                    DoubleUtil.GreaterThan(column.Width.DesiredValue, column.MinWidth))
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

            // update expected data
            double maxWidth = columnToUpdate.ActualWidth - 10.0;
            expectedColumns[i].maxWidth = maxWidth;

            LogComment("update MaxWidth");
            columnToUpdate.MaxWidth = maxWidth;
            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("verify on max width change");
            double epsilon = 0.0;
            ComputeAndVerifyOnColumnMaxWidthChange(ref expectedColumns, i, oldWidthData, epsilon);

            expectedColumns = GetAllColumnData().ToArray();

            // capture the old width
            DataGridLength oldWidth2 = columnToUpdate.Width;
            DataGridHelper.ColumnWidthData oldWidthData2 = new DataGridHelper.ColumnWidthData
            {
                unitType = oldWidth.UnitType,
                width = oldWidth.Value,
                desiredWidth = oldWidth.DesiredValue,
                displayWidth = oldWidth.DisplayValue,
                minWidth = columnToUpdate.MinWidth,
                maxWidth = columnToUpdate.MaxWidth,
                CanUserResize = columnToUpdate.CanUserResize
            };

            LogComment("update MaxWidth to be less than current MaxWidth");
            double updatedMaxWidth = columnToUpdate.Width.DesiredValue - columnToUpdate.MaxWidth + 10;
            expectedColumns[i].maxWidth = updatedMaxWidth;

            LogComment("update MaxWidth again");
            columnToUpdate.MaxWidth = updatedMaxWidth;

            LogComment("verify on max width change again");
            ComputeAndVerifyOnColumnMaxWidthChange(ref expectedColumns, i, oldWidthData2, epsilon);

            LogComment("TestUpdateMaxWidthFromStarToBeLessThanDisplay2 was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestUpdateMaxColumnWidthToBeLessThanViewportWidth()
        {
            Status("TestUpdateMaxColumnWidthToBeLessThanViewportWidth");
            
            // get the current column widths
            double prevMaxColumnWidth = MyDataGrid.MaxColumnWidth;
            DataGridHelper.ColumnWidthData[] prevColumns = GetAllColumnData().ToArray();            
            DataGridHelper.ColumnWidthData[] expectedColumns = GetAllColumnData().ToArray();                        

            // update expected data
            double totalMaxWidth = GetViewportWidthForColumns() - 7.0;
            double maxWidth = totalMaxWidth / expectedColumns.Length;
            for (int i = 0; i < expectedColumns.Length; i++)
            {
                // the default MaxWidth set on the columns is PositiveInfinity.  If that is used
                // then update accordingly, otherwise it will keep its explicitly
                // set value
                if (Double.IsPositiveInfinity(expectedColumns[i].maxWidth))
                {
                    expectedColumns[i].maxWidth = maxWidth;
                    ComputeExpectedWidthsOnColumnMaxWidthChange(ref expectedColumns, i, prevColumns[i]);
                }
            }

            LogComment(string.Format("update dataGrid.MaxColumnWidth: {0}", maxWidth));
            MyDataGrid.MaxColumnWidth = maxWidth;
            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("verify on MaxColumnWidth change");            
            double epsilon = 0.0;
            VerifyColumnWidths(expectedColumns, epsilon);

            LogComment("update MaxColumnWidth to be its previous value");
            MyDataGrid.MaxColumnWidth = prevMaxColumnWidth;
            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("verify on min width change again");
            VerifyColumnWidths(prevColumns, epsilon);

            LogComment("TestUpdateMaxColumnWidthToBeLessThanViewportWidth was successful");
            return TestResult.Pass;
        }                        

        /// <summary>
        /// Verify precedence when settings MaxColumnWidth and MaxWidth after initial load
        /// 
        /// -MaxColumnWidth is set to specific value
        /// -Column0 MaxWidth set explicitly to 150
        /// -Column4 MaxWidth set explicitly to 150
        /// -Column6 MaxWidth set explicitly to 150
        /// -Column8 MaxWidth set explicitly to 75
        /// -all other columns take MaxColumnWidth value
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestPrecedenceofMaxColumnWidthAndMaxWidth2()
        {
            Status("TestPrecedenceofMaxColumnWidthAndMaxWidth2");

            double origMaxColumnWidth = MyDataGrid.MaxColumnWidth;

            // set the MaxColumnWidth
            MyDataGrid.MaxColumnWidth = 50.0;

            int i = 0;
            foreach (DataGridColumn column in MyDataGrid.Columns)
            {
                if (i == 0)
                {
                    if (column.MaxWidth != 150.0)
                    {
                        throw new TestValidationException(string.Format(
                            "Expected column: {0}, MaxWidth: {1}, Actual: {2}",
                            i,
                            150.0,
                            column.MaxWidth));
                    }
                }
                else if (i == 4)
                {
                    if (column.MaxWidth != 150.0)
                    {
                        throw new TestValidationException(string.Format(
                            "Expected column: {0}, MaxWidth: {1}, Actual: {2}",
                            i,
                            150.0,
                            column.MaxWidth));
                    }
                }
                else if (i == 6)
                {
                    if (column.MaxWidth != 150.0)
                    {
                        throw new TestValidationException(string.Format(
                            "Expected column: {0}, MaxWidth: {1}, Actual: {2}",
                            i,
                            150.0,
                            column.MaxWidth));
                    }
                }
                else if (i == 8)
                {
                    if (column.MaxWidth != 75.0)
                    {
                        throw new TestValidationException(string.Format(
                            "Expected column: {0}, MaxWidth: {1}, Actual: {2}",
                            i,
                            75.0,
                            column.MaxWidth));
                    }
                }
                else
                {
                    if (column.MaxWidth != 50.0)
                    {
                        throw new TestValidationException(string.Format(
                            "Expected column: {0}, MaxWidth: {1}, Actual: {2}",
                            i,
                            50.0,
                            column.MaxWidth));
                    }
                }

                i++;
            }

            // set back to original value
            MyDataGrid.MaxColumnWidth = origMaxColumnWidth;
            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("TestPrecedenceofMaxColumnWidthAndMaxWidth2 was successful");
            return TestResult.Pass;
        }        

        #endregion Test Steps
    }
}
