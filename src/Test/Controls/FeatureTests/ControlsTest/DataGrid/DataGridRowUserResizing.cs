using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Avalon.Test.ComponentModel;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Input;
using Microsoft.Test.Layout;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;



namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Tests end-user row height resizing.            
    /// 
    /// Row Height Verifications:
    /// Row.Height = CellsPresenter.ActualHeight
    /// RowHeader.Height = RowHeader.ActualHeight - RowDetails.ActualHeight
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridRowUserResizing", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords = "MicroSuite", Disabled=true)]              

    public class DataGridRowUserResizing : DataGridTest
    {
        #region Private Fields

        private double defaultRowHeightValue;
        private int[] defaultRowsToTest;
        private List<int[]> defaultMaxRowsToTest;
        private List<int[]> defaultMinRowsToTest;
        private DataGridRowDetailsVisibilityMode[] defaultDetailVisibilityModes = new[] { DataGridRowDetailsVisibilityMode.Visible,
                                                                                          DataGridRowDetailsVisibilityMode.VisibleWhenSelected, 
                                                                                          DataGridRowDetailsVisibilityMode.Collapsed     };

        #endregion Private Fields

        #region Constructor

        public DataGridRowUserResizing()
            : base(@"DataGridBasicRowColumnHeaderSizing.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestCanUserResizeRows);
            RunSteps += new TestStep(TestDraggingRowSeparatorDown);
            RunSteps += new TestStep(TestDraggingRowSeparatorUp);
            RunSteps += new TestStep(TestDraggingRowSeparatorDownPastMaxHeight);
            RunSteps += new TestStep(TestDraggingRowSeparatorUpPastMinHeight);
            RunSteps += new TestStep(TestDoubleClickOnRowSeparator);
            RunSteps += new TestStep(TestRowLoadingEvent);
            RunSteps += new TestStep(TestRowUnloadingEvent);
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

            Status("Setup specific for DataGridRowUserResizing");

            this.SetupDataSource();            

            this.defaultRowsToTest = new int[] { 0, 1, 5, MyDataGrid.Items.Count / 2, MyDataGrid.Items.Count - 1 };

            this.defaultMinRowsToTest = new List<int[]>();
            defaultMinRowsToTest.Add(new int[] { (MyDataGrid.Items.Count / 2) + 1, MyDataGrid.Items.Count - 3 });
            defaultMinRowsToTest.Add(new int[] { 0, 1, 2 });
            defaultMinRowsToTest.Add(new int[] { MyDataGrid.Items.Count - 4, MyDataGrid.Items.Count - 5 });

            this.defaultMaxRowsToTest = new List<int[]>();
            defaultMaxRowsToTest.Add(new int[] { (MyDataGrid.Items.Count / 2) - 1, MyDataGrid.Items.Count - 5 });
            defaultMaxRowsToTest.Add(new int[] { 5, 6, 7 });
            defaultMaxRowsToTest.Add(new int[] { MyDataGrid.Items.Count - 1, MyDataGrid.Items.Count - 2 });

            DataGridRow row = DataGridHelper.GetRow(MyDataGrid, 0);
            DataGridCellsPresenter cp = DataGridHelper.GetCellsPresenter(row);
            this.defaultRowHeightValue = cp.ActualHeight;            

            LogComment("Setup for DataGridRowUserResizing was successful");
            return TestResult.Pass;
        }

        /// <summary>
        ///  Verify when CanUserResizeRows set to false, user cannot use separators 
        ///  to resize columns.  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestCanUserResizeRows()
        {
            Status("TestCanUserResizeRows");

            // disallow row resizing
            MyDataGrid.CanUserResizeRows = false;
            double resizeDiff = 0;

            this.RunConfigMatrix((detailsVisibilityMode, rowIndex) =>
            {
                RowHeightValues prevHeightValues = this.GetRowHeightValues(rowIndex);

                // get the gripper and resize column header
                DataGridActionHelper.ResizeRowHeader(MyDataGrid, rowIndex, resizeDiff);
                this.WaitForPriority(DispatcherPriority.ApplicationIdle);

                // verify row is resized
                VerifyRowResizing(prevHeightValues, rowIndex, resizeDiff, detailsVisibilityMode);
            });

            LogComment("TestCanUserResizeRows was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify dragging row separator down increases the row height.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestDraggingRowSeparatorDown()
        {
            Status("TestDraggingRowSeparatorDown");

            // allow row resizing
            MyDataGrid.CanUserResizeRows = true;
            double resizeDiff = 20;

            this.RunConfigMatrix((detailsVisibilityMode, rowIndex) =>
                {
                    RowHeightValues prevHeightValues = this.GetRowHeightValues(rowIndex);

                    // get the gripper and resize column header
                    DataGridActionHelper.ResizeRowHeader(MyDataGrid, rowIndex, resizeDiff);
                    this.WaitForPriority(DispatcherPriority.ApplicationIdle);

                    // verify row is resized
                    VerifyRowResizing(prevHeightValues, rowIndex, resizeDiff, detailsVisibilityMode);
                });

            LogComment("TestDraggingRowSeparatorDown was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify dragging row separator up shrinks the row height
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestDraggingRowSeparatorUp()
        {
            Status("TestDraggingRowSeparatorUp");

            // allow row resizing
            MyDataGrid.CanUserResizeRows = true;
            double resizeDiff = -5;

            this.RunConfigMatrix((detailsVisibilityMode, rowIndex) =>
                {
                    RowHeightValues prevHeightValues = this.GetRowHeightValues(rowIndex);

                    // get the gripper and resize column header
                    DataGridActionHelper.ResizeRowHeader(MyDataGrid, rowIndex, resizeDiff);
                    this.WaitForPriority(DispatcherPriority.ApplicationIdle);

                    // verify row is resized
                    VerifyRowResizing(prevHeightValues, rowIndex, resizeDiff, detailsVisibilityMode);
                });

            LogComment("TestDraggingRowSeparatorUp was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify dragging row separator past MaxHeight increases the row 
        /// height to at most MaxHeight.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestDraggingRowSeparatorDownPastMaxHeight()
        {
            Status("TestDraggingRowSeparatorDownPastMaxHeight");

            // allow row resizing
            MyDataGrid.CanUserResizeRows = true;

            // set past max
            DataGridRow row = DataGridHelper.GetRow(MyDataGrid, 0);
            double resizeDiff = row.MaxHeight + 50;

            this.RunConfigMatrix(
                this.defaultDetailVisibilityModes,
                this.defaultMaxRowsToTest,
                (detailsVisibilityMode, rowIndex) =>
                {
                    RowHeightValues prevHeightValues = this.GetRowHeightValues(rowIndex);

                    // get the gripper and resize column header
                    DataGridActionHelper.ResizeRowHeader(MyDataGrid, rowIndex, resizeDiff);
                    this.WaitForPriority(DispatcherPriority.ApplicationIdle);

                    // verify row is resized
                    VerifyRowResizing(prevHeightValues, rowIndex, resizeDiff, detailsVisibilityMode);
                });

            LogComment("TestDraggingRowSeparatorDownPastMaxHeight was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify dragging row separator up past MinHeight shrinks 
        /// the row height no less than MinHeight.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestDraggingRowSeparatorUpPastMinHeight()
        {
            Status("TestDraggingRowSeparatorUpPastMinHeight");

            // allow row resizing
            MyDataGrid.CanUserResizeRows = true;

            // set past min
            DataGridRow row = DataGridHelper.GetRow(MyDataGrid, 0);
            double resizeDiff = -400;

            this.RunConfigMatrix(
                this.defaultDetailVisibilityModes,
                this.defaultMinRowsToTest,
                (detailsVisibilityMode, rowIndex) =>
                {
                    RowHeightValues prevHeightValues = this.GetRowHeightValues(rowIndex);

                    // get the gripper and resize column header
                    DataGridActionHelper.ResizeRowHeader(MyDataGrid, rowIndex, resizeDiff);
                    this.WaitForPriority(DispatcherPriority.ApplicationIdle);

                    // verify row is resized
                    VerifyRowResizing(prevHeightValues, rowIndex, resizeDiff, detailsVisibilityMode);
                });

            LogComment("TestDraggingRowSeparatorUpPastMinHeight was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify double-click behavior with row separators.  On double-click,
        /// height should reset back to its original value.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestDoubleClickOnRowSeparator()
        {
            Status("TestDoubleClickOnRowSeparator");

            // allow row resizing
            MyDataGrid.CanUserResizeRows = true;
            List<int[]> allRowsToTest = new List<int[]>();
            allRowsToTest.Add(new int[] { 0, 1, 2 });
            allRowsToTest.Add(this.defaultMaxRowsToTest[0]);
            allRowsToTest.Add(this.defaultMinRowsToTest[0]);

            this.RunConfigMatrix(
                this.defaultDetailVisibilityModes,
                allRowsToTest,
                (detailsVisibilityMode, rowIndex) =>
                {
                    RowHeightValues prevRowHeights = this.GetRowHeightValues(rowIndex);

                    // get the gripper and resize column header
                    DataGridActionHelper.DoubleClickRowHeaderGripper(MyDataGrid, rowIndex);
                    this.WaitForPriority(DispatcherPriority.ApplicationIdle);

                    // Calculate the expected heights
                    RowHeightValues expectedRowHeights = new RowHeightValues();
                    expectedRowHeights.rowHeight = prevRowHeights.rowHeight;
                    expectedRowHeights.rowMinHeight = prevRowHeights.rowMinHeight;
                    expectedRowHeights.rowMaxHeight = prevRowHeights.rowMaxHeight;
                    expectedRowHeights.rowHeaderHeight = prevRowHeights.rowHeaderHeight;
                    expectedRowHeights.rowDetailsActualHeight = prevRowHeights.rowDetailsActualHeight;
                    expectedRowHeights.rowActualHeight = this.defaultRowHeightValue + prevRowHeights.rowDetailsActualHeight;
                    expectedRowHeights.rowHeaderActualHeight = this.defaultRowHeightValue + prevRowHeights.rowDetailsActualHeight;
                    expectedRowHeights.cellsPresenterActualHeight = this.defaultRowHeightValue;

                    // get the actual values
                    RowHeightValues actualRowHeights = GetRowHeightValues(rowIndex);

                    // verify 
                    this.VerifyRowHeights(expectedRowHeights, actualRowHeights);
                });

            LogComment("TestDoubleClickOnRowSeparator was successful");
            return TestResult.Pass;
        }

        /// <summary>
        ///  Verify LoadingRow is called for each row.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestRowLoadingEvent()
        {
            Status("TestRowLoadingEvent");

            LogComment("clear the DataGrid");
            MyDataGrid.ItemsSource = null;
            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("attach to the event");
            List<object> loadedItems = new List<object>();
            int eventCounter = 0;
            EventHandler<DataGridRowEventArgs> LoadingRow = (s, e) =>
                {
                    eventCounter++;
                    loadedItems.Add(e.Row.Item);
                };
            MyDataGrid.LoadingRow += LoadingRow;

            LogComment("repopulate the DataGrid and navigate to each row");
            MyDataGrid.ItemsSource = new People();
            MyDataGrid.UpdateLayout();
            QueueHelper.WaitTillQueueItemsProcessed();

            foreach (object item in MyDataGrid.Items)
            {
                MyDataGrid.ScrollIntoView(item);
                QueueHelper.WaitTillQueueItemsProcessed();
            }

            LogComment("verify LoadingRow was called for each row");
            if (eventCounter < MyDataGrid.Items.Count)
            {
                throw new TestValidationException(string.Format(
                    "LoadingRow event was not fired the correct number of times. Expected: {0}, Actual: {1}",
                    MyDataGrid.Items.Count,
                    eventCounter));
            }

            Dictionary<int, int> visitedRows = new Dictionary<int, int>(MyDataGrid.Items.Count);
            for (int i = 0; i < MyDataGrid.Items.Count; i++)
                visitedRows.Add(i, 0);

            foreach (object loadedItem in loadedItems)
            {
                int rowIndex = MyDataGrid.Items.IndexOf(loadedItem);
                if (rowIndex == -1)
                {
                    throw new TestValidationException(string.Format("item does not exist in current DataGrid."));
                }
                visitedRows[rowIndex]++;
            }

            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<int, int> kvp in visitedRows)
            {
                if (kvp.Value == 0)
                    sb.Append(string.Format("Row: {0}, was not passed in to the LoadingRow event{1}", kvp.Key, Environment.NewLine));
            }

            if (sb.Length > 0)
            {
                throw new TestValidationException(sb.ToString());
            }

            // clean up
            MyDataGrid.LoadingRow -= LoadingRow;

            LogComment("TestRowLoadingEvent was successful");
            return TestResult.Pass;
        }

        /// <summary>
        ///  Verify when CanUserResizeRows set to false, user cannot use separators 
        ///  to resize columns.  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestRowUnloadingEvent()
        {
            Status("TestRowUnloadingEvent");

            MyDataGrid.ItemsSource = null;
            QueueHelper.WaitTillQueueItemsProcessed();
            MyDataGrid.ItemsSource = new People();
            MyDataGrid.UpdateLayout();
            QueueHelper.WaitTillQueueItemsProcessed();

            List<object> dgItems = new List<object>();
            foreach (object item in MyDataGrid.Items)
            {
                if (item != CollectionView.NewItemPlaceholder)
                    dgItems.Add(item);
            }
            Dictionary<int, int> visitedRows = new Dictionary<int, int>(MyDataGrid.Items.Count - 1);
            for (int i = 0; i < MyDataGrid.Items.Count - 1; i++)
            {
                visitedRows.Add(i, 0);
            }

            LogComment("attach to the event");
            List<object> unloadedItems = new List<object>();
            int eventCounter = 0;
            EventHandler<DataGridRowEventArgs> UnloadingRow = (s, e) =>
                {
                    eventCounter++;
                    unloadedItems.Add(e.Row.Item);
                };
            MyDataGrid.UnloadingRow += UnloadingRow;

            LogComment("delete all items in the DataGrid");
            DataGridActionHelper.SelectRow(MyDataGrid, 0, false, false);
            int count = MyDataGrid.Items.Count;
            while (count > 0)
            {
                UserInput.KeyPress(System.Windows.Input.Key.Delete, true);
                QueueHelper.WaitTillQueueItemsProcessed();
                count--;
            }

            LogComment("verify LoadingRow was called for each row");
            if (eventCounter < dgItems.Count)
            {
                throw new TestValidationException(string.Format(
                    "LoadingRow event was not fired the correct number of times. Expected: {0}, Actual: {1}",
                    dgItems.Count,
                    eventCounter));
            }

            foreach (object unloadedItem in unloadedItems)
            {
                int rowIndex = dgItems.IndexOf(unloadedItem);
                if (rowIndex == -1)
                {
                    throw new TestValidationException(string.Format("item does not exist in current DataGrid."));
                }
                visitedRows[rowIndex]++;
            }

            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<int, int> kvp in visitedRows)
            {
                if (kvp.Value == 0)
                    sb.Append(string.Format("Row: {0}, was not passed in to the LoadingRow event{1}", kvp.Key, Environment.NewLine));
            }

            if (sb.Length > 0)
            {
                throw new TestValidationException(sb.ToString());
            }

            // clean up
            MyDataGrid.UnloadingRow -= UnloadingRow;

            LogComment("TestRowUnloadingEvent was successful");
            return TestResult.Pass;
        }
        #endregion Test Steps

        #region Helpers

        private void RunConfigMatrix(Action<DataGridRowDetailsVisibilityMode, int> action)
        {
            RunConfigMatrix(this.defaultDetailVisibilityModes, this.defaultRowsToTest, action);
        }

        private void RunConfigMatrix(int[] rowsToTest, Action<DataGridRowDetailsVisibilityMode, int> action)
        {
            RunConfigMatrix(this.defaultDetailVisibilityModes, rowsToTest, action);
        }

        private void RunConfigMatrix(DataGridRowDetailsVisibilityMode[] detailsVisibleModes, int[] rowsToTest, Action<DataGridRowDetailsVisibilityMode, int> action)
        {
            List<int[]> allRows = new List<int[]>();
            allRows.Add(rowsToTest);
            RunConfigMatrix(detailsVisibleModes, allRows, action);
        }

        private void RunConfigMatrix(DataGridRowDetailsVisibilityMode[] detailsVisibleModes, List<int[]> allRowsToTest, Action<DataGridRowDetailsVisibilityMode, int> action)
        {
            int i = 0;
            foreach (DataGridRowDetailsVisibilityMode detailsVisibilityMode in defaultDetailVisibilityModes)
            {
                LogComment(string.Format("Begin testing with detailsVisibilityMode: {0}", detailsVisibilityMode));

                // set the details mode
                MyDataGrid.RowDetailsVisibilityMode = detailsVisibilityMode;
                QueueHelper.WaitTillQueueItemsProcessed();
                this.WaitFor(500);

                foreach (int rowIndex in allRowsToTest[i])
                {
                    LogComment(string.Format("Begin testing with rowIndex: {0}", rowIndex));

                    MyDataGrid.ScrollIntoView(MyDataGrid.Items[rowIndex]);
                    QueueHelper.WaitTillQueueItemsProcessed();
                    this.WaitFor(500);

                    action(detailsVisibilityMode, rowIndex);

                    MyDataGrid.UnselectAll();
                    QueueHelper.WaitTillQueueItemsProcessed();

                    LogComment(string.Format("End testing with rowIndex: {0}", rowIndex));
                }

                if (i + 1 < allRowsToTest.Count) i++;
                LogComment(string.Format("End testing with detailsVisibilityMode: {0}", detailsVisibilityMode));
            }
        }

        public struct RowHeightValues
        {
            public double rowActualHeight;
            public double rowHeight;
            public double rowMinHeight;
            public double rowMaxHeight;

            public double rowHeaderActualHeight;
            public double rowHeaderHeight;

            public double cellsPresenterActualHeight;
            public double rowDetailsActualHeight;
        }

        private RowHeightValues GetRowHeightValues(int rowIndex)
        {
            RowHeightValues rowHeights = new RowHeightValues();

            DataGridRow row = DataGridHelper.GetRow(MyDataGrid, rowIndex);
            DataGridRowHeader rowHeader = DataGridHelper.GetRowHeader(row);
            DataGridCellsPresenter cellsPresenter = DataGridHelper.GetCellsPresenter(row);
            DataGridDetailsPresenter detailsPresenter = DataGridHelper.FindVisualChild<DataGridDetailsPresenter>(row);

            rowHeights.rowActualHeight = row.ActualHeight;
            rowHeights.rowHeight = row.Height;
            rowHeights.rowMaxHeight = row.MaxHeight;
            rowHeights.rowMinHeight = cellsPresenter.MinHeight;

            rowHeights.rowHeaderActualHeight = rowHeader.ActualHeight;
            rowHeights.rowHeaderHeight = rowHeader.Height;

            rowHeights.cellsPresenterActualHeight = cellsPresenter.ActualHeight;
            rowHeights.rowDetailsActualHeight = detailsPresenter.ActualHeight;

            return rowHeights;
        }

        private void VerifyRowResizing(RowHeightValues prevHeights, int rowToResize, double resizeDiff, DataGridRowDetailsVisibilityMode detailsVisibleMode)
        {
            // Calculate the expected heights
            RowHeightValues expectedRowHeights = new RowHeightValues();
            expectedRowHeights.rowHeight = prevHeights.rowHeight;
            expectedRowHeights.rowMinHeight = prevHeights.rowMinHeight;
            expectedRowHeights.rowMaxHeight = prevHeights.rowMaxHeight;
            expectedRowHeights.rowHeaderHeight = prevHeights.rowHeaderHeight;
            expectedRowHeights.rowDetailsActualHeight = prevHeights.rowDetailsActualHeight;
            expectedRowHeights.rowActualHeight = prevHeights.rowActualHeight;
            expectedRowHeights.rowHeaderActualHeight = prevHeights.rowActualHeight;
            expectedRowHeights.cellsPresenterActualHeight = prevHeights.rowActualHeight;

            // acount for resize
            double epsilon = 4.0;
            double rowActualHeight = prevHeights.rowActualHeight + resizeDiff - ((!DoubleUtil.IsZero(resizeDiff)) ? epsilon : 0.0);
            rowActualHeight = rowActualHeight < prevHeights.rowMinHeight ? prevHeights.rowMinHeight : rowActualHeight;
            rowActualHeight = rowActualHeight > prevHeights.rowMaxHeight ? prevHeights.rowMaxHeight : rowActualHeight;

            expectedRowHeights.rowActualHeight = rowActualHeight;
            expectedRowHeights.rowHeaderActualHeight = rowActualHeight;
            expectedRowHeights.cellsPresenterActualHeight = rowActualHeight;

            // get the actual values
            RowHeightValues actualRowHeights = GetRowHeightValues(rowToResize);

            // account for details visibility 
            if (detailsVisibleMode == DataGridRowDetailsVisibilityMode.VisibleWhenSelected)
            {
                expectedRowHeights.rowDetailsActualHeight = actualRowHeights.rowDetailsActualHeight;
                double updatedExpectedHeight = expectedRowHeights.rowActualHeight + expectedRowHeights.rowDetailsActualHeight;
                double updatedExpectedHeaderHeight = expectedRowHeights.rowHeaderActualHeight + expectedRowHeights.rowDetailsActualHeight;
                expectedRowHeights.rowActualHeight = updatedExpectedHeight;
                expectedRowHeights.rowHeaderActualHeight = updatedExpectedHeaderHeight;
            }
            else if (detailsVisibleMode == DataGridRowDetailsVisibilityMode.Visible)
            {
                if (DoubleUtil.AreClose(expectedRowHeights.rowActualHeight, expectedRowHeights.rowMinHeight))
                {
                    double updatedExpectedHeight = expectedRowHeights.rowActualHeight + expectedRowHeights.rowDetailsActualHeight;
                    double updatedExpectedHeaderHeight = expectedRowHeights.rowHeaderActualHeight + expectedRowHeights.rowDetailsActualHeight;
                    expectedRowHeights.rowActualHeight = updatedExpectedHeight;
                    expectedRowHeights.rowHeaderActualHeight = updatedExpectedHeaderHeight;
                }
                expectedRowHeights.cellsPresenterActualHeight = expectedRowHeights.rowActualHeight - expectedRowHeights.rowDetailsActualHeight;
            }

            // verify
            VerifyRowHeights(expectedRowHeights, actualRowHeights);
        }

        private void VerifyRowHeights(RowHeightValues expected, RowHeightValues actual)
        {
            double epsilon = 3.2;
            bool areEqual = true;
            StringBuilder sb = new StringBuilder();

            if (!DataGridHelper.AreClose(expected.rowActualHeight, actual.rowActualHeight, epsilon))
            {
                areEqual = false;
                sb.Append(string.Format(
                    "expected.rowActualHeight: {0}, does not match actual: {1}{2}",
                    expected.rowActualHeight,
                    actual.rowActualHeight,
                    Environment.NewLine));
            }

            if (!DataGridHelper.AreClose(expected.rowHeaderActualHeight, actual.rowHeaderActualHeight, epsilon))
            {
                areEqual = false;
                sb.Append(string.Format(
                    "expected.rowHeaderActualHeight: {0}, does not match actual: {1}{2}",
                    expected.rowHeaderActualHeight,
                    actual.rowHeaderActualHeight,
                    Environment.NewLine));
            }

            if (!DataGridHelper.AreClose(expected.rowDetailsActualHeight, actual.rowDetailsActualHeight, epsilon))
            {
                areEqual = false;
                sb.Append(string.Format(
                    "expected.rowDetailsActualHeight: {0}, does not match actual: {1}{2}",
                    expected.rowDetailsActualHeight,
                    actual.rowDetailsActualHeight,
                    Environment.NewLine));
            }

            if ((Double.IsNaN(expected.rowHeight) && !Double.IsNaN(actual.rowHeight)) ||
                (!Double.IsNaN(expected.rowHeight) && Double.IsNaN(actual.rowHeight)))
            {
                areEqual = false;
                sb.Append(string.Format(
                    "one value is NaN while the other is not. expected.rowHeight: {0}, does not match actual: {1}{2}",
                    expected.rowHeight,
                    actual.rowHeight,
                    Environment.NewLine));
            }
            else if (!Double.IsNaN(expected.rowHeight) && !DataGridHelper.AreClose(expected.rowHeight, actual.rowHeight, epsilon))
            {
                areEqual = false;
                sb.Append(string.Format(
                    "expected.rowHeight: {0}, does not match actual: {1}{2}",
                    expected.rowHeight,
                    actual.rowHeight,
                    Environment.NewLine));
            }

            if ((Double.IsNaN(expected.rowHeaderHeight) && !Double.IsNaN(actual.rowHeaderHeight)) ||
                (!Double.IsNaN(expected.rowHeaderHeight) && Double.IsNaN(actual.rowHeaderHeight)))
            {
                areEqual = false;
                sb.Append(string.Format(
                    "one value is NaN while the other is not. expected.rowHeaderHeight: {0}, does not match actual: {1}{2}",
                    expected.rowHeaderHeight,
                    actual.rowHeaderHeight,
                    Environment.NewLine));
            }
            else if (!Double.IsNaN(expected.rowHeaderHeight) && !DataGridHelper.AreClose(expected.rowHeaderHeight, actual.rowHeaderHeight, epsilon))
            {
                areEqual = false;
                sb.Append(string.Format(
                    "expected.rowHeaderHeight: {0}, does not match actual: {1}{2}",
                    expected.rowHeaderHeight,
                    actual.rowHeaderHeight,
                    Environment.NewLine));
            }

            if (!DataGridHelper.AreClose(expected.rowMinHeight, actual.rowMinHeight, epsilon))
            {
                areEqual = false;
                sb.Append(string.Format(
                    "expected.rowMinHeight: {0}, does not match actual: {1}{2}",
                    expected.rowMinHeight,
                    actual.rowMinHeight,
                    Environment.NewLine));
            }

            if (!DataGridHelper.AreClose(expected.rowMaxHeight, actual.rowMaxHeight, epsilon))
            {
                areEqual = false;
                sb.Append(string.Format(
                    "expected.rowMaxHeight: {0}, does not match actual: {1}{2}",
                    expected.rowMaxHeight,
                    actual.rowMaxHeight,
                    Environment.NewLine));
            }

            if (!DataGridHelper.AreClose(expected.cellsPresenterActualHeight, actual.cellsPresenterActualHeight, epsilon))
            {
                // it's possible that it is a MaxHeight case where actual.cellsPresenterHeight is equal to rowMaxHeight
                if (!DataGridHelper.AreClose(expected.rowMaxHeight, actual.cellsPresenterActualHeight, epsilon))
                {
                    areEqual = false;
                    sb.Append(string.Format(
                        "expected.cellsPresenterActualHeight: {0}, does not match actual: {1}{2}",
                        expected.cellsPresenterActualHeight,
                        actual.cellsPresenterActualHeight,
                        Environment.NewLine));
                }
            }

            if (!areEqual)
            {
                throw new TestValidationException(sb.ToString());
            }
        }

        #endregion Helpers
    }
}
