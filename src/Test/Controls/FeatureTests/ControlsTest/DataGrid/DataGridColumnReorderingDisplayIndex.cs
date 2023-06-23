using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;


namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Tests for adding, removing, moving, replacing, and clearing with DisplayIndex.
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridColumnReorderingDisplayIndex", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords = "Localization_Suite")]    
    public class DataGridColumnReorderingDisplayIndex : DataGridTest
    {
        #region Constructor

        public DataGridColumnReorderingDisplayIndex()
            : base(@"DataGridEditing.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestInitialDisplayOrder);
            RunSteps += new TestStep(TestInsertingIntoListBasic);
            RunSteps += new TestStep(TestInsertingIntoListPermutation);
            RunSteps += new TestStep(TestRemovingFromList);
            RunSteps += new TestStep(TestInsertingExistingBackIntoList);
            RunSteps += new TestStep(TestInsertIntoInvalidDisplayIndex);            
            RunSteps += new TestStep(TestReplacingAColumn);
            RunSteps += new TestStep(TestMovingColumns);
            RunSteps += new TestStep(TestClearingAllColumns);
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

            Status("Setup specific for DataGridColumnReorderingDisplayIndex");

            this.SetupDataSource();

            LogComment("Setup for DataGridColumnReorderingDisplayIndex was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify initial display order matches the column.DisplayIndex order
        /// </summary>
        private TestResult TestInitialDisplayOrder()
        {
            Status("TestInitialDisplayOrder");

            DataGridVerificationHelper.VerifyDisplayIndices(MyDataGrid);

            LogComment("TestInitialDisplayOrder was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify basic inserting scenarios
        /// </summary>
        private TestResult TestInsertingIntoListBasic()
        {
            Status("TestInsertingIntoListBasic");

            // insert at first index
            ObservableCollection<DataGridColumn> prevColumns = DataGridHelper.GetDisplayColumnList(MyDataGrid);            
            int insertIndex = 0;
            DataGridColumn newColumn = new DataGridTextColumn();
            newColumn.Header = "1st, DI=0";

            MyDataGrid.Columns.Insert(insertIndex, newColumn);
            this.VerifyInsertReorder(prevColumns, insertIndex, -1, newColumn);

            // insert at last index
            prevColumns = DataGridHelper.GetDisplayColumnList(MyDataGrid); 
            insertIndex = MyDataGrid.Columns.Count - 1;
            newColumn = new DataGridTextColumn();
            newColumn.Header = "Last, DI=last";

            MyDataGrid.Columns.Insert(insertIndex, newColumn);
            this.VerifyInsertReorder(prevColumns, insertIndex, -1, newColumn);

            // insert in the middle
            prevColumns = DataGridHelper.GetDisplayColumnList(MyDataGrid); 
            insertIndex = MyDataGrid.Columns.Count / 2;
            newColumn = new DataGridTextColumn();
            newColumn.Header = "Mid, DI=mid";

            MyDataGrid.Columns.Insert(insertIndex, newColumn);
            this.VerifyInsertReorder(prevColumns, insertIndex, -1, newColumn);

            LogComment("TestInsertingIntoListBasic was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify a matrix of insert scenarios
        /// </summary>
        private TestResult TestInsertingIntoListPermutation()
        {
            Status("TestInsertingIntoListPermutation");

            int[] insertIndexArr = { 0, 1, MyDataGrid.Columns.Count / 2, MyDataGrid.Columns.Count - 1 };
            int[] displayIndexArr = { 0, 1, MyDataGrid.Columns.Count / 2, MyDataGrid.Columns.Count };

            foreach (int insertIndex in insertIndexArr)
            {
                foreach (int displayIndex in displayIndexArr)
                {
                    LogComment(string.Format("Insert into colIdx = {0} with displayIndex = {1}", insertIndex, displayIndex));

                    ObservableCollection<DataGridColumn> prevColumns = DataGridHelper.GetDisplayColumnList(MyDataGrid);  

                    // create a new column with the particular display index
                    DataGridColumn newColumn = new DataGridTextColumn();
                    newColumn.Header = string.Format("ColIdx={0}, DI={1}", insertIndex, displayIndex);
                    newColumn.DisplayIndex = displayIndex;

                    // insert the new column at the particular index
                    MyDataGrid.Columns.Insert(insertIndex, newColumn);

                    // verify order
                    this.VerifyInsertReorder(prevColumns, insertIndex, displayIndex, newColumn);                    

                    LogComment(string.Format("successfully verified at colIdx = {0} with displayIndex = {1}", insertIndex, displayIndex));
                }
            }                        

            LogComment("TestInsertingIntoListPermutation was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify removing scenarios
        /// </summary>
        private TestResult TestRemovingFromList()
        {
            Status("TestRemovingFromList");

            // remove at 0
            ObservableCollection<DataGridColumn> prevColumns = DataGridHelper.GetDisplayColumnList(MyDataGrid);
            int removeIndex = 0;
            int displayRemoveIndex = MyDataGrid.Columns[removeIndex].DisplayIndex;

            MyDataGrid.Columns.RemoveAt(removeIndex);
            this.VerifyRemoveReorder(prevColumns, displayRemoveIndex);

            // remove at last index
            prevColumns = DataGridHelper.GetDisplayColumnList(MyDataGrid);
            removeIndex = MyDataGrid.Columns.Count - 1;
            displayRemoveIndex = MyDataGrid.Columns[removeIndex].DisplayIndex;

            MyDataGrid.Columns.RemoveAt(removeIndex);
            this.VerifyRemoveReorder(prevColumns, displayRemoveIndex);

            // remove at mid index
            prevColumns = DataGridHelper.GetDisplayColumnList(MyDataGrid);
            removeIndex = MyDataGrid.Columns.Count / 2;
            displayRemoveIndex = MyDataGrid.Columns[removeIndex].DisplayIndex;            

            MyDataGrid.Columns.RemoveAt(removeIndex);
            this.VerifyRemoveReorder(prevColumns, displayRemoveIndex);
            
            // remove consecutive
            prevColumns = DataGridHelper.GetDisplayColumnList(MyDataGrid);
            removeIndex = 1;
            displayRemoveIndex = MyDataGrid.Columns[removeIndex].DisplayIndex;                        

            MyDataGrid.Columns.RemoveAt(removeIndex);
            this.VerifyRemoveReorder(prevColumns, displayRemoveIndex);

            prevColumns = DataGridHelper.GetDisplayColumnList(MyDataGrid);
            removeIndex = 2;
            displayRemoveIndex = MyDataGrid.Columns[removeIndex].DisplayIndex;                                    

            MyDataGrid.Columns.RemoveAt(removeIndex);
            this.VerifyRemoveReorder(prevColumns, displayRemoveIndex);

            LogComment("TestRemovingFromList was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify removing and inserting back into the list
        /// </summary>
        private TestResult TestInsertingExistingBackIntoList()
        {
            Status("TestInsertingExistingBackIntoList");

            // setup
            ObservableCollection<DataGridColumn> prevColumns = DataGridHelper.GetDisplayColumnList(MyDataGrid);
            int removeIndex = 0;
            int displayRemoveIndex = MyDataGrid.Columns[removeIndex].DisplayIndex;
            DataGridColumn column = MyDataGrid.Columns[0];

            // remove to insert back later
            MyDataGrid.Columns.RemoveAt(removeIndex);
            this.VerifyRemoveReorder(prevColumns, displayRemoveIndex);

            // setup
            prevColumns = DataGridHelper.GetDisplayColumnList(MyDataGrid);
            int insertIndex = 3;

            // insert into a new position where display index has already been set on column
            MyDataGrid.Columns.Insert(insertIndex, column);
            this.VerifyInsertReorder(prevColumns, insertIndex, column.DisplayIndex, column);

            LogComment("TestInsertingExistingBackIntoList was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Invalid cases for DisplayIndex
        /// </summary>
        private TestResult TestInsertIntoInvalidDisplayIndex()
        {
            Status("TestInsertIntoInvalidDisplayIndex");

            string message = "The DisplayIndex for the DataGridcolumn with Header '" + MyDataGrid.Columns[0].Header + "' is out of range.  DisplayIndex must be greater than or equal to 0 and less than Columns.Count.";
            ExceptionHelper.ExpectException(
                () =>
                {
                    MyDataGrid.Columns[0].DisplayIndex = -2;
                },
                new ArgumentOutOfRangeException("displayIndex", -2, message));


            ExceptionHelper.ExpectException(
                () =>
                {
                    MyDataGrid.Columns[0].DisplayIndex = MyDataGrid.Columns.Count;
                },
                new ArgumentOutOfRangeException("displayIndex", MyDataGrid.Columns.Count, message));

            ExceptionHelper.ExpectException(
                () =>
                {
                    MyDataGrid.Columns[0].DisplayIndex = -1;
                },
                new ArgumentOutOfRangeException("displayIndex", -1, message));

            ExceptionHelper.ExpectException(
                () =>
                {
                    DataGridColumn newColumn = new DataGridTextColumn();
                    newColumn.DisplayIndex = -1;
                    MyDataGrid.Columns.Insert(0, newColumn);
                },
                new ArgumentOutOfRangeException("displayIndex", -1, message));

            ExceptionHelper.ExpectException(
               () =>
               {
                   DataGridColumn newColumn = new DataGridTextColumn();
                   newColumn.DisplayIndex = MyDataGrid.Columns.Count + 1;
                   MyDataGrid.Columns.Insert(0, newColumn);
               },
               new ArgumentOutOfRangeException("displayIndex", MyDataGrid.Columns.Count + 1, message));            

            LogComment("TestInsertIntoInvalidDisplayIndex was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify replacing columns scenarios
        /// </summary>
        private TestResult TestReplacingAColumn()
        {
            Status("TestReplacingAColumn");

            // setup
            ObservableCollection<DataGridColumn> prevColumns = DataGridHelper.GetDisplayColumnList(MyDataGrid);   
            int replaceIndex = MyDataGrid.Columns[0].DisplayIndex;
            DataGridColumn newColumn = new DataGridTextColumn();

            // replace column 0 with new column and default display index
            MyDataGrid.Columns[0] = newColumn;
            this.VerifyReplaceReorder(prevColumns, replaceIndex, 0, newColumn);

            // setup
            prevColumns = DataGridHelper.GetDisplayColumnList(MyDataGrid);
            replaceIndex = MyDataGrid.Columns[2].DisplayIndex;
            DataGridColumn newColumn2 = new DataGridTextColumn();
            newColumn2.DisplayIndex = 0;

            // replace column 2 with new column and displayindex = 0
            MyDataGrid.Columns[2] = newColumn2;
            this.VerifyReplaceReorder(prevColumns, replaceIndex, newColumn2.DisplayIndex, newColumn2);

            LogComment("TestReplacingAColumn was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify moving columns scenarios
        /// </summary>
        private TestResult TestMovingColumns()
        {
            Status("TestMovingColumns");

            ObservableCollection<DataGridColumn> prevColumns = DataGridHelper.GetDisplayColumnList(MyDataGrid);   

            MyDataGrid.Columns.Move(0, 2);
            DataGridVerificationHelper.VerifyDisplayOrder(MyDataGrid, prevColumns);

            MyDataGrid.Columns.Move(0, MyDataGrid.Columns.Count - 1);
            DataGridVerificationHelper.VerifyDisplayOrder(MyDataGrid, prevColumns);

            MyDataGrid.Columns.Move(MyDataGrid.Columns.Count - 1, MyDataGrid.Columns.Count / 2);
            DataGridVerificationHelper.VerifyDisplayOrder(MyDataGrid, prevColumns);

            MyDataGrid.Columns.Move(MyDataGrid.Columns.Count - 1, 0);
            DataGridVerificationHelper.VerifyDisplayOrder(MyDataGrid, prevColumns);

            LogComment("TestMovingColumns was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify clearing the columns
        /// </summary>
        private TestResult TestClearingAllColumns()
        {
            Status("TestClearingAllColumns");

            MyDataGrid.Columns.Clear();

            DataGridVerificationHelper.VerifyDisplayIndices(MyDataGrid);

            LogComment("TestInsertMultiple was successful");
            return TestResult.Pass;
        }

        #endregion Test Steps

        #region Helpers

        private void VerifyInsertReorder(ObservableCollection<DataGridColumn> prevColumns, int insertIndex, int displayIndex, DataGridColumn column)
        {
            // case for default displayIndex, takes whatever the insertIndex is
            if (displayIndex == -1)
            {
                displayIndex = insertIndex;
            }

            // create the expected order of columns, for inserting, order is just like a normal list
            prevColumns.Insert(displayIndex, column);

            DataGridVerificationHelper.VerifyDisplayOrder(MyDataGrid, prevColumns);
        }

        private void VerifyRemoveReorder(ObservableCollection<DataGridColumn> prevColumns, int displayRemoveIndex)
        {
            // create the expected column order by removing at the display index
            prevColumns.RemoveAt(displayRemoveIndex);

            DataGridVerificationHelper.VerifyDisplayOrder(MyDataGrid, prevColumns);
        }

        private void VerifyReplaceReorder(ObservableCollection<DataGridColumn> prevColumns, int replaceIndex, int newDisplayIndex, DataGridColumn column)
        {
            // create the expected order of columns by removing at the prev display index and inserting into the new one
            prevColumns.RemoveAt(replaceIndex);
            prevColumns.Insert(newDisplayIndex, column);

            DataGridVerificationHelper.VerifyDisplayOrder(MyDataGrid, prevColumns);
        }

        #endregion Helpers
    }
}
