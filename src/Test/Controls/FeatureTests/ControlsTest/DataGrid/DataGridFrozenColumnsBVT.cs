using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using Avalon.Test.ComponentModel;
using Avalon.Test.ComponentModel.DataSources;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Verification;



namespace Microsoft.Test.Controls
{
    /// <summary>
    /// The BVT tests for Frozen Columns feature
    ///
    /// Factors to consider in testing:
    /// 1. Validate DataGrid.FrozenColumnCount - int validations
    /// 2. Validate DataGridColumn.IsFrozen - the first n columns are frozen when n=DataGrid.FrozenColumnCount
    /// 3. Scrolling Tests: only those not frozen can be scrolled
    /// 4. Add/remove a column at the DisplayIndex less than n, or at greater than n
    /// 5. Add a new row - all first n columns should have IsFrozen=true
    ///
    /// NOTES:
    /// 1. All DP tests will be added into the DataGridDependencyTests.cs
    /// 2. The IsFrozen value set/reset will be done via the column's OnDataGridFrozenColumnCountChanged, so given
    ///      the change to the FrozenColumnCount, the IsFrozen value tests on selected columns should be sufficient.
    /// 3. Scrolling tests are the same
    /// </summary>
    [Test(0, "DataGrid", "DataGridFrozenColumnsBVT", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords = "Localization_Suite",Versions="4.5+,4.5Client+")] 
    public class DataGridFrozenColumnsBVT : XamlTest
    {
        #region Private Fields

        DataGrid dataGrid;
        NewPeople people;
        Page page;
        ScrollViewer scrollViewer;

        #endregion

        #region Constructor

        public DataGridFrozenColumnsBVT()
            : base(@"DataGridFrozenColumnsBVT.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(TestInvalidInput);
            RunSteps += new TestStep(TestBasics);                       
            RunSteps += new TestStep(TestAddRemoveColumn);               
            RunSteps += new TestStep(TestAddRow);
            RunSteps += new TestStep(VerifyFrozenColumnByDisplayIndex);
            RunSteps += new TestStep(VerifyFrozenColumnSorting);         
            RunSteps += new TestStep(VerifyFrozenColumnReordering);     // can be only reordered within their group
            //RunSteps += new TestStep(VerifyFrozenColumnResizing);      
        }

        #endregion

        #region Test Steps

        /// <summary>
        /// initial setup
        /// </summary>
        /// <returns></returns>
        TestResult Setup()
        {
            Status("Setup");
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            // locate the DataGrid being tested
            dataGrid = (DataGrid)RootElement.FindName("DataGrid_Standard");
            Assert.AssertTrue("Can not find the DataGrid in the xaml file!", dataGrid != null);

            page = (Page)this.Window.Content;
            people = (NewPeople)(page.FindResource("people"));
            Assert.AssertTrue("Can not find the data source in the xaml file!", people != null);

            scrollViewer = DataGridHelper.FindVisualChild<ScrollViewer>(dataGrid);
            Assert.AssertTrue("Can not find the DataGrid in the xaml file!", scrollViewer != null);

            LogComment("Setup was successful");
            return TestResult.Pass;
        }

        TestResult CleanUp()
        {
            dataGrid = null;
            page = null;
	     people = null;
            scrollViewer = null;
            return TestResult.Pass;
        }

        /// <summary>
        /// For FrozenColumnCount:
        /// 1. set to Negative value should throw
        /// 2. set to positive out of range value should be coerced
        /// </summary>
        /// <returns></returns>
        TestResult TestInvalidInput()
        {
            Status("TestInvalidInput");

            // 1. Verify the exception thrown w/ negative value
            ExceptionHelper.ExpectException(() =>
                {
                    dataGrid.FrozenColumnCount = -1;
                },
                new ArgumentException());

            // 2. Verify that outofrange value, the value coerced.
            dataGrid.FrozenColumnCount = 20;
            Assert.AssertTrue("Positive outofrange value should be coerced.",
                (Int32)(dataGrid.GetValue(DataGrid.FrozenColumnCountProperty)) == dataGrid.Columns.Count);

            // Reset
            dataGrid.SetValue(DataGrid.FrozenColumnCountProperty, 0);

            LogComment("TestInvalidInput was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 1. when FrozenColumnCount=0: no column is frozen
        /// 2. change to FrozenColumnCount=1: the first column is frozen
        ///      and can't be scrolled
        /// </summary>
        /// <returns></returns>
        TestResult TestBasics() 
        {
            Status("TestBasics");

            // 1: no cell is frozen
            foreach (DataGridColumn col in dataGrid.Columns)
            {
                if (col.IsFrozen == true)
                {
                    return TestResult.Fail;
                }
            }

            // 2. change the FrozenColumnCount = 1
            dataGrid.FrozenColumnCount = 1;
            DataGridColumnHeader header0 = DataGridHelper.GetColumnHeader(dataGrid, 0);
            Assert.AssertTrue("Column at index 0 should be frozen", header0.IsFrozen == true);

            // record the initial tranform values before the scrolling
            DataGridColumnHeader header5 = DataGridHelper.GetColumnHeader(dataGrid, 5);
            Assert.AssertTrue("Column at index 5 should not be frozen", header5.IsFrozen == false);

            GeneralTransform transform0Prev = header0.TransformToAncestor(dataGrid);
            GeneralTransform transform5Prev = header5.TransformToAncestor(dataGrid);

            // do horizontal scrolling
            scrollViewer.PageRight();
            WaitForPriority(DispatcherPriority.Background);

            // verify the results
            GeneralTransform transform0After = header0.TransformToAncestor(dataGrid);
            GeneralTransform transform5After = header5.TransformToAncestor(dataGrid);

            Assert.AssertTrue("Transforms For header0 should be equal", transform0Prev.ToString() == transform0After.ToString());
            // Verify scrollViewer.PageRight() have effected
            Assert.AssertTrue("Transforms for header5 should be equal", transform5Prev.ToString() != transform5After.ToString());

            LogComment("TestBasics was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 1. set FrozenColumnCount=2: validate col1.IsFrozen=true
        /// 2. add a new column at index=1: validate the new column is frozen
        /// 3. add the new column at index=4: validate the new column is not frozen
        /// 4. remove the newly added column: validate the column at index 4 is not frozen
        /// </summary>
        /// <returns></returns>
        TestResult TestAddRemoveColumn() 
        {
            Status("TestAddRemoveColumn");

            // step 1 -
            dataGrid.FrozenColumnCount = 2;
            WaitForPriority(DispatcherPriority.Background);
            Assert.AssertTrue("The column at index 1 should be frozen", ((DataGridColumnHeader)(DataGridHelper.GetColumnHeader(dataGrid, 1))).IsFrozen == true);

            // step 2 - insert a new column in range
            //cache the column
            DataGridColumn oldColumn = dataGrid.ColumnFromDisplayIndex(1);
            int columnIndex = dataGrid.Columns.IndexOf(oldColumn);

            DataGridTextColumn textColumn = new DataGridTextColumn();
            textColumn.Width = 100;
            textColumn.DisplayIndex = 1;
            dataGrid.Columns.Add(textColumn);
            WaitForPriority(DispatcherPriority.Render);
            // validate
            Assert.AssertTrue("The new column should be frozen", textColumn.IsFrozen == true);

            // what about the old column at orig index=1?
            oldColumn = dataGrid.Columns[columnIndex];
            Assert.AssertTrue("The oldColumn should be un-frozen", oldColumn.IsFrozen == false);

            // step 3 -
            textColumn.DisplayIndex = 4;
            WaitForPriority(DispatcherPriority.Background);
            Assert.AssertTrue("The new column should not be frozen", textColumn.IsFrozen == false);

            // step 3+ -
            textColumn.DisplayIndex = 1;
            WaitForPriority(DispatcherPriority.Background);
            Assert.AssertTrue("The new column should be back to frozen", textColumn.IsFrozen == true);

            // step 4 -
            dataGrid.Columns.Remove(textColumn);
            WaitForPriority(DispatcherPriority.Background);
            DataGridColumn column4 = (DataGridColumn)DataGridHelper.GetColumn(dataGrid, 4);
            Assert.AssertTrue("The column 4 should not be frozen", column4.IsFrozen == false);

            // again - what about the old column at orig index=1?
            oldColumn = dataGrid.Columns[columnIndex];
            Assert.AssertTrue("The oldColumn should be back to frozen", oldColumn.IsFrozen == true);

            LogComment("TestAddRemoveColumn was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 1. set FrozenColumnCount=3, validate column at index 2 is frozen
        /// 2. add a new row, validate the first 3 columns is frozen, and column 5 is not.
        /// </summary>
        /// <returns></returns>
        TestResult TestAddRow()
        {
            Status("TestAddRow");

            // 1 - set the new value and verify the newly frozen column
            dataGrid.FrozenColumnCount = 3;
            Assert.AssertTrue("The 3rd column should be frozen",((DataGridColumnHeader)(DataGridHelper.GetColumnHeader(dataGrid, 2))).IsFrozen == true);

            // 2 - add a new row
            people.Insert(0, new Person("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXX", "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"));
            WaitForPriority(DispatcherPriority.Background);

            // 3. verify the first 3 columns of row 0 are frozen
            DataGridColumn col0 = ((DataGridCell)DataGridHelper.GetCell(dataGrid, 0, 0)).Column;
            DataGridColumn col1 = ((DataGridCell)DataGridHelper.GetCell(dataGrid, 0, 1)).Column;
            DataGridColumn col2 = ((DataGridCell)DataGridHelper.GetCell(dataGrid, 0, 2)).Column;
            DataGridColumn col4 = ((DataGridCell)DataGridHelper.GetCell(dataGrid, 0, 4)).Column;

            Assert.AssertTrue("The first column should be frozen", col0.IsFrozen == true);
            Assert.AssertTrue("The 2nd column should be frozen", col1.IsFrozen == true);
            Assert.AssertTrue("The 3rd column should be frozen", col2.IsFrozen == true);
            Assert.AssertTrue("The 5th column should not be frozen", col4.IsFrozen == false);

            LogComment("TestAddRow was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Alternatively, to verify if a column is frozen, DisplayIndex on columns can be used
        /// For now, the FrozenColumnCount = 3, so all columns with DisplayIndex less than 3 should be frozen
        /// </summary>
        /// <returns></returns>
        TestResult VerifyFrozenColumnByDisplayIndex()
        {
            Status("VerifyFrozenColumnByDisplayIndex");

            ObservableCollection<DataGridColumn> columns = DataGridHelper.GetDisplayColumnList(dataGrid);
            for(int i = 0; i < columns.Count; i++)
            {
                if (i < dataGrid.FrozenColumnCount)
                {
                    Assert.AssertTrue(string.Format("The column {0} should be frozen", i),
                        columns[i].IsFrozen == true);
                }
                else
                {
                     Assert.AssertTrue(string.Format("The column {0} should not be frozen", i),
                         columns[i].IsFrozen == false);
                }
            }

            LogComment("VerifyFrozenColumnByDisplayIndex was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify that the Frozen Columns can be sorted
        ///
        /// We have seperate column sorting tests, here we only focus on the basics
        /// for frozen columns:
        ///     1. sort on column 1 - DataGridTextColumn w/ string values
        ///     2. verify the column was sorted
        /// </summary>
        /// <returns></returns>
        TestResult VerifyFrozenColumnSorting()
        {
            Status("VerifyFrozenColumnSorting");

            // make sure we can sort the column
            DataGridColumn column1 = (DataGridColumn)DataGridHelper.GetColumn(dataGrid, 1);
            column1.CanUserSort = true;

            // 1. sorting the column
            DataGridActionHelper.ClickOnColumnHeader(dataGrid, 1);
            WaitForPriority(DispatcherPriority.Background);

            // 2. which direction are we sorting the column?  Asc / Desc?
            ListSortDirection? sortDirection = column1.SortDirection;
            Assert.AssertTrue("The sortDirection should not be null", sortDirection != null);

            // 3. verify the sorting happened for this column
            if (sortDirection == ListSortDirection.Ascending)
            {
                Assert.AssertTrue("The first column should be sorted ASC", CheckIfSortedOnFirstName(ListSortDirection.Ascending) == true);
            }
            else
            {
                Assert.AssertTrue("The first column should be sorted DESC", CheckIfSortedOnFirstName(ListSortDirection.Descending) == true);
            }

            LogComment("VerifyFrozenColumnSorting was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify that the Frozen Columns can be reordered within the frozen group range using DisplayIndex
        ///
        /// Other column reorder feature tests are covered in Column Reordering
        /// </summary>
        /// <returns></returns>
        TestResult VerifyFrozenColumnReordering()
        {
            Status("VerifyFrozenColumnReordering");

            // Preconditions: DataGrid.CanUserReorderColumns=true and DataGridColumn.CanUserReorder=true
            dataGrid.CanUserResizeColumns = true;

            // 1. make sure the DataGrid.CanUserReorderColumns=true
            DataGridColumn column0 = (DataGridColumn)DataGridHelper.GetColumn(dataGrid, 0);
            column0.CanUserReorder = true;

            // set the value
            column0.SetValue(DataGridColumn.DisplayIndexProperty, 1);
            // verify the set
            Assert.AssertTrue("The DisplayIndex of the column should be 1 now.", column0.DisplayIndex == 1);

            LogComment("VerifyFrozenColumnReordering was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify that the Frozen Columns can't be resized
        /// </summary>
        /// <returns></returns>
        TestResult VerifyFrozenColumnResizing()
        {
            Status("VerifyFrozenColumnResizing");

            // Precondition: make sure the DataGrid.CanUserResizeColumns=true
            dataGrid.CanUserResizeColumns = true;

            LogComment("Resize each column right and verify");
            for (int i = 0; i < dataGrid.FrozenColumnCount; i++)
            {
                List<double> headerWidths = DataGridHelper.GetColumnHeaderWidths(dataGrid);

                // get the gripper and resize column header
                DataGridActionHelper.ResizeColumnHeader(dataGrid, i, 120);

                // verify columns are resized correctly
                DataGridVerificationHelper.VerifyColumnResizing(dataGrid, headerWidths, i, 120);
            }

            LogComment("VerifyFrozenColumnResizing was successful");
            return TestResult.Pass;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Checks if all the visible columns are in specfic sorted order on firstname
        /// </summary>
        /// <param name="sortDirection">The SortDirection of the column</param>
        /// <returns></returns>
        private bool CheckIfSortedOnFirstName(ListSortDirection sortDirection)
        {
            int index = 1;
            DataGridRow row1 = (DataGridRow)DataGridHelper.GetRow(dataGrid, 0);
            DataGridRow row2 = (DataGridRow)DataGridHelper.GetRow(dataGrid, 1);
            while (index < people.Count)
            {
                if (row2 == null)
                {
                    break;
                }
                Person person1 = row1.Item as Person;
                Person person2 = row2.Item as Person;
                Assert.AssertTrue("person1 should not be null", person1 != null);
                Assert.AssertTrue("person2 should not be null", person2 != null);

                if (sortDirection == ListSortDirection.Ascending)
                {
                    if (person1.FirstName.CompareTo(person2.FirstName) > 0)
                    {
                        return false;
                    }
                }
                else
                {
                    if (person1.FirstName.CompareTo(person2.FirstName) < 0)
                    {
                        return false;
                    }
                }

                row1 = row2;
                ++index;
                row2 = (DataGridRow)DataGridHelper.GetRow(dataGrid, index);
            }
            return true;
        }

        #endregion
    }
}
