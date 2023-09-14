using Avalon.Test.ComponentModel;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.Windows.Controls;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Tests MinWidth changes for star columns.  
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridColumnStarMinWidthChanges", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords = "Localization_Suite")]
    public class DataGridColumnStarMinWidthChanges : DataGridColumnUserResizingStarBase
    {
        #region Constructor

        public DataGridColumnStarMinWidthChanges()
            : this("DataGridColumnStarWidthChanges.xaml")
        {
        }

        [Variation("DataGridColumnStarWidthChanges.xaml")]
        public DataGridColumnStarMinWidthChanges(string filename)
            : base(filename)
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestPrecedenceofMinColumnWidthAndMinWidth);
            RunSteps += new TestStep(TestUpdateMinColumnWidthToBeGreaterThanViewportWidth);
            RunSteps += new TestStep(TestUpdateMinWidthFromNonStarToBeGreaterThanDisplay);
            RunSteps += new TestStep(TestUpdateMinWidthFromNonStarToBeGreaterThanDisplay2);
            RunSteps += new TestStep(TestUpdateMinWidthFromStarToBeGreaterThanDisplay);
            RunSteps += new TestStep(TestUpdateMinWidthFromStarToBeGreaterThanDisplay2);
            RunSteps += new TestStep(TestPrecedenceofMinColumnWidthAndMinWidth2);
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
        /// Verify precedence when settings MinColumnWidth and MinWidth on initial load
        /// 
        /// -MinColumnWidth is set to default
        /// -Column0 MinWidth set explicitly to 100
        /// -Column2 MinWidth set explicitly to 100
        /// -Column3 MinWidth set explicitly to 200
        /// -Column8 MinWidth set explicitly to 40
        /// -all other columns take MinColumnWidth value
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestPrecedenceofMinColumnWidthAndMinWidth()
        {
            Status("TestPrecedenceofMinColumnWidthAndMinWidth");

            if (MyDataGrid.MinColumnWidth != 20.0)
            {
                throw new TestValidationException(string.Format(
                    "Expected MinColumnWidth: {0}, Actual: {1}",
                    20.0,
                    MyDataGrid.MinColumnWidth));
            }

            int i = 0;
            foreach (DataGridColumn column in MyDataGrid.Columns)
            {
                if (i == 0)
                {
                    if (column.MinWidth != 100.0)
                    {
                        throw new TestValidationException(string.Format(
                            "Expected column: {0}, MinWidth: {1}, Actual: {2}",
                            i,
                            100.0,
                            column.MinWidth));
                    }
                }
                else if (i == 2)
                {
                    if (column.MinWidth != 100.0)
                    {
                        throw new TestValidationException(string.Format(
                            "Expected column: {0}, MinWidth: {1}, Actual: {2}",
                            i,
                            100.0,
                            column.MinWidth));
                    }
                }
                else if (i == 3)
                {
                    if (column.MinWidth != 200.0)
                    {
                        throw new TestValidationException(string.Format(
                            "Expected column: {0}, MinWidth: {1}, Actual: {2}",
                            i,
                            200.0,
                            column.MinWidth));
                    }
                }
                else if (i == 8)
                {
                    if (column.MinWidth != 40.0)
                    {
                        throw new TestValidationException(string.Format(
                            "Expected column: {0}, MinWidth: {1}, Actual: {2}",
                            i,
                            40.0,
                            column.MinWidth));
                    }
                }
                else
                {
                    if (column.MinWidth != 20.0)
                    {
                        throw new TestValidationException(string.Format(
                            "Expected column: {0}, MinWidth: {1}, Actual: {2}",
                            i,
                            20.0,
                            column.MinWidth));
                    }
                }

                i++;
            }

            LogComment("TestPrecedenceofMinColumnWidthAndMinWidth was successful");
            return TestResult.Pass;
        }                

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestUpdateMinWidthFromNonStarToBeGreaterThanDisplay()
        {
            Status("TestUpdateMinWidthFromNonStarToBeGreaterThanDisplay");

            // get the current column widths
            DataGridHelper.ColumnWidthData[] prevColumns = GetAllColumnData().ToArray();
            DataGridHelper.ColumnWidthData[] expectedColumns = GetAllColumnData().ToArray();

            // find a non-star column 
            DataGridColumn columnToUpdate = null;
            int i = 0;
            foreach (DataGridColumn column in MyDataGrid.Columns)
            {
                if (column.Width.IsAuto && (DoubleUtil.GreaterThan(column.ActualWidth, column.MinWidth)))
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
            double minWidth = columnToUpdate.ActualWidth + 100.0;
            expectedColumns[i].minWidth = minWidth;

            LogComment("update MinWidth");
            columnToUpdate.MinWidth = minWidth;
            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("verify on min width change");
            double epsilon = 0.0;
            ComputeAndVerifyOnColumnMinWidthChange(ref expectedColumns, i, oldWidthData, epsilon);

            LogComment("update MinWidth back to original");
            columnToUpdate.MinWidth = oldWidthData.minWidth;
            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("verify columns go back to original");
            VerifyColumnWidths(prevColumns, epsilon);            

            LogComment("TestUpdateMinWidthFromNonStarToBeGreaterThanDisplay was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Update MinWidth so new MinWidth is less than old MinWidth and old MinWidth 
        /// was greater than width.DesiredValue.  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestUpdateMinWidthFromNonStarToBeGreaterThanDisplay2()
        {
            Status("TestUpdateMinWidthFromNonStarToBeGreaterThanDisplay2");

            // get the current column widths
            DataGridHelper.ColumnWidthData[] expectedColumns = GetAllColumnData().ToArray();

            // find a non-star column 
            DataGridColumn columnToUpdate = null;
            int i = 0;
            foreach (DataGridColumn column in MyDataGrid.Columns)
            {
                if (column.Width.IsAbsolute && (DoubleUtil.GreaterThan(column.ActualWidth, column.MinWidth)))
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
            double minWidth = columnToUpdate.Width.DesiredValue + 100.0;
            minWidth = DoubleUtil.LessThan(minWidth, columnToUpdate.MaxWidth) ? minWidth : columnToUpdate.MaxWidth - 1;
            expectedColumns[i].minWidth = minWidth;

            LogComment(string.Format("update MinWidth to: {0}", minWidth));
            columnToUpdate.MinWidth = minWidth;
            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("verify on min width change");            
            double epsilon = 0.0;
            ComputeAndVerifyOnColumnMinWidthChange(ref expectedColumns, i, oldWidthData, epsilon);

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

            LogComment("update MinWidth to be less than current MinWidth");
            double updatedMinWidth = columnToUpdate.Width.DesiredValue + 10;
            updatedMinWidth = DoubleUtil.LessThan(updatedMinWidth, columnToUpdate.MaxWidth) ? updatedMinWidth : columnToUpdate.MaxWidth - 1;
            expectedColumns[i].minWidth = updatedMinWidth;

            LogComment(string.Format("update MinWidth again to: {0}", updatedMinWidth));            
            columnToUpdate.MinWidth = updatedMinWidth;

            LogComment("verify on min width change again");                        
            ComputeAndVerifyOnColumnMinWidthChange(ref expectedColumns, i, oldWidthData2, epsilon);            

            LogComment("TestUpdateMinWidthFromNonStarToBeGreaterThanDisplay2 was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Update MinWidth so new MinWidth is greater than width.DesiredValue.  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestUpdateMinWidthFromStarToBeGreaterThanDisplay()
        {
            Status("TestUpdateMinWidthFromStarToBeGreaterThanDisplay");

            // get the current column widths
            DataGridHelper.ColumnWidthData[] prevColumns = GetAllColumnData().ToArray();
            DataGridHelper.ColumnWidthData[] expectedColumns = GetAllColumnData().ToArray();

            // find a star column 
            DataGridColumn columnToUpdate = null;
            int i = 0;
            foreach (DataGridColumn column in MyDataGrid.Columns)
            {
                if (column.Width.IsStar && (DoubleUtil.GreaterThan(column.ActualWidth, column.MinWidth)))
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
            double minWidth = columnToUpdate.ActualWidth + 100.0;
            expectedColumns[i].minWidth = minWidth;

            LogComment("update MinWidth");
            columnToUpdate.MinWidth = minWidth;
            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("verify on min width change");            
            double epsilon = 0.0;
            ComputeAndVerifyOnColumnMinWidthChange(ref expectedColumns, i, oldWidthData, epsilon);

            LogComment("update MinWidth back to original");            
            columnToUpdate.MinWidth = oldWidthData.minWidth;
            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("verify columns go back to original");            
            VerifyColumnWidths(prevColumns, epsilon);

            LogComment("TestUpdateMinWidthFromStarToBeGreaterThanDisplay was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Update MinWidth so new MinWidth is less than old MinWidth and old MinWidth 
        /// was greater than width.DesiredValue.  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestUpdateMinWidthFromStarToBeGreaterThanDisplay2()
        {
            Status("TestUpdateMinWidthFromStarToBeGreaterThanDisplay2");

            // get the current column widths
            DataGridHelper.ColumnWidthData[] expectedColumns = GetAllColumnData().ToArray();

            // find a non-star column 
            DataGridColumn columnToUpdate = null;
            int i = 0;
            foreach (DataGridColumn column in MyDataGrid.Columns)
            {
                if (column.Width.IsStar && (DoubleUtil.GreaterThan(column.ActualWidth, column.MinWidth)))
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
            double minWidth = columnToUpdate.ActualWidth + 100.0;
            expectedColumns[i].minWidth = minWidth;

            LogComment("update MinWidth");
            columnToUpdate.MinWidth = minWidth;
            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("verify on min width change");
            double epsilon = 5.0;
            ComputeAndVerifyOnColumnMinWidthChange(ref expectedColumns, i, oldWidthData, epsilon);

            expectedColumns = GetAllColumnData().ToArray();

            // capture the old width
            DataGridLength oldWidth2 = columnToUpdate.Width;
            DataGridHelper.ColumnWidthData oldWidthData2 = new DataGridHelper.ColumnWidthData
            {
                unitType = oldWidth.UnitType,
                width = columnToUpdate.Width.Value,
                desiredWidth = columnToUpdate.Width.DesiredValue,
                displayWidth = columnToUpdate.Width.DisplayValue,
                minWidth = columnToUpdate.MinWidth,
                maxWidth = columnToUpdate.MaxWidth,
                CanUserResize = columnToUpdate.CanUserResize
            };

            LogComment("update MinWidth to be less than current MinWidth");
            double updatedMinWidth = columnToUpdate.MinWidth - 5;
            expectedColumns[i].minWidth = updatedMinWidth;

            LogComment("update MinWidth again");
            columnToUpdate.MinWidth = updatedMinWidth;

            LogComment("verify on min width change again");
            ComputeAndVerifyOnColumnMinWidthChange(ref expectedColumns, i, oldWidthData2, epsilon); 

            LogComment("TestUpdateMinWidthFromStarToBeGreaterThanDisplay2 was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestUpdateMinColumnWidthToBeGreaterThanViewportWidth()
        {
            Status("TestUpdateMinColumnWidthToBeGreaterThanViewportWidth");

            double viewportWidth = GetViewportWidthForColumns();
            double tempTotalMinWidth = 0.0;
            foreach(DataGridColumn column in MyDataGrid.Columns)
            {
                tempTotalMinWidth += column.MinWidth;
            }
            Assert.AssertTrue("Total MinWidths should be less than viewport width at the beginning of this step", DoubleUtil.LessThan(tempTotalMinWidth, viewportWidth));

            // get the current column widths
            double prevMinColumnWidth = MyDataGrid.MinColumnWidth;
            DataGridHelper.ColumnWidthData[] prevColumns = GetAllColumnData().ToArray();            
            DataGridHelper.ColumnWidthData[] expectedColumns = GetAllColumnData().ToArray();                        

            // update expected data
            double totalMinWidth = GetViewportWidthForColumns() + 50.0;
            double minWidth = totalMinWidth / expectedColumns.Length;
            for (int i = 0; i < expectedColumns.Length; i++)
            {
                // the default MinWidth set on the columns is 20.  If that is used
                // then update accordingly, otherwise it will keep its explicitly
                // set value
                if (expectedColumns[i].minWidth == 20.0)
                {
                    expectedColumns[i].minWidth = minWidth;
                    ComputeExpectedWidthsOnColumnMinWidthChange(ref expectedColumns, i, prevColumns[i]);
                }
            }

            LogComment(string.Format("update dataGrid.MinColumnWidth: {0}", minWidth));
            MyDataGrid.MinColumnWidth = minWidth;
            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("verify on MinColumnWidth change");            
            double epsilon = 0.0;
            VerifyColumnWidths(expectedColumns, epsilon);

            LogComment("update MinColumnWidth to be its previous value");
            MyDataGrid.MinColumnWidth = prevMinColumnWidth;
            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("verify on min width change again");
            VerifyColumnWidths(prevColumns, epsilon);

            LogComment("TestUpdateMinColumnWidthToBeGreaterThanViewportWidth was successful");
            return TestResult.Pass;
        }                        

        /// <summary>
        /// Verify precedence when settings MinColumnWidth and MinWidth after initial load
        /// 
        /// -MinColumnWidth is set to new value
        /// -Column0 MinWidth set explicitly to 100
        /// -Column2 MinWidth set explicitly to 100
        /// -Column3 MinWidth set explicitly to 200
        /// -Column8 MinWidth set explicitly to 40
        /// -all other columns take MinColumnWidth value
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestPrecedenceofMinColumnWidthAndMinWidth2()
        {
            Status("TestPrecedenceofMinColumnWidthAndMinWidth2");

            MyDataGrid.Columns.Clear();

            DataGridTextColumn tempColumn = new DataGridTextColumn();
            tempColumn.MinWidth = 100;
            MyDataGrid.Columns.Add(tempColumn);
            tempColumn = new DataGridTextColumn();
            MyDataGrid.Columns.Add(tempColumn);
            tempColumn = new DataGridTextColumn();
            tempColumn.MinWidth = 100;
            MyDataGrid.Columns.Add(tempColumn); 
            tempColumn = new DataGridTextColumn();
            tempColumn.MinWidth = 200;
            MyDataGrid.Columns.Add(tempColumn);
            tempColumn = new DataGridTextColumn();
            MyDataGrid.Columns.Add(tempColumn);
            tempColumn = new DataGridTextColumn();
            MyDataGrid.Columns.Add(tempColumn);
            tempColumn = new DataGridTextColumn();
            MyDataGrid.Columns.Add(tempColumn);
            tempColumn = new DataGridTextColumn();
            MyDataGrid.Columns.Add(tempColumn);
            tempColumn = new DataGridTextColumn();
            tempColumn.MinWidth = 40;
            MyDataGrid.Columns.Add(tempColumn);

            // set the MinColumnWidth
            MyDataGrid.MinColumnWidth = 400.0;
            MyDataGrid.UpdateLayout();
            QueueHelper.WaitTillQueueItemsProcessed();

            int i = 0;
            foreach (DataGridColumn column in MyDataGrid.Columns)
            {
                if (i == 0)
                {
                    if (column.MinWidth != 100.0)
                    {
                        throw new TestValidationException(string.Format(
                            "Expected column: {0}, MinWidth: {1}, Actual: {2}",
                            i,
                            100.0,
                            column.MinWidth));
                    }
                }
                else if (i == 2)
                {
                    if (column.MinWidth != 100.0)
                    {
                        throw new TestValidationException(string.Format(
                            "Expected column: {0}, MinWidth: {1}, Actual: {2}",
                            i,
                            100.0,
                            column.MinWidth));
                    }
                }
                else if (i == 3)
                {
                    if (column.MinWidth != 200.0)
                    {
                        throw new TestValidationException(string.Format(
                            "Expected column: {0}, MinWidth: {1}, Actual: {2}",
                            i,
                            200.0,
                            column.MinWidth));
                    }
                }
                else if (i == 8)
                {
                    if (column.MinWidth != 40.0)
                    {
                        throw new TestValidationException(string.Format(
                            "Expected column: {0}, MinWidth: {1}, Actual: {2}",
                            i,
                            40.0,
                            column.MinWidth));
                    }
                }
                else
                {
                    if (column.MinWidth != 400.0)
                    {
                        throw new TestValidationException(string.Format(
                            "Expected column: {0}, MinWidth: {1}, Actual: {2}",
                            i,
                            400.0,
                            column.MinWidth));
                    }
                }

                i++;
            }

            LogComment("TestPrecedenceofMinColumnWidthAndMinWidth2 was successful");
            return TestResult.Pass;
        }        

        #endregion Test Steps
    }
}
