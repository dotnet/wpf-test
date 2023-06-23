using System.Windows.Input;
using Avalon.Test.ComponentModel;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

using System.Windows.Data;
using System.Windows.Controls;
using System.Windows;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Tests for adding a new row.
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridDeleteRowScenarios", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords = "Localization_Suite")]
    public class DataGridDeleteRowScenarios : DataGridEditing
    {
        #region Private Fields


        #endregion Private Fields

        #region Constructor

        public DataGridDeleteRowScenarios()
            : this("Microsoft.Test.Controls.DataSources.EditablePeople, ControlsCommon", "Microsoft.Test.Controls.DataSources.EditablePerson, ControlsCommon")
        {
        }

        [Variation("Microsoft.Test.Controls.DataSources.EditablePeople, ControlsCommon", "Microsoft.Test.Controls.DataSources.EditablePerson, ControlsCommon")]
        public DataGridDeleteRowScenarios(string dataSourceName, string dataTypeName)
            : base(@"DataGridEditing.xaml")
        {
            this.DataSourceTypeName = dataSourceName;
            this.TypeNameFromDataSource = dataTypeName;
            this.CreateDataSource();

            InitializeSteps += new TestStep(Setup);
            RunSteps += CreateTestStepFromGeneric(typeof(DataGridDeleteRowScenarios), "TestDeletingARow");
            RunSteps += CreateTestStepFromGeneric(typeof(DataGridDeleteRowScenarios), "TestDeletingMultipleRow");
            RunSteps += CreateTestStepFromGeneric(typeof(DataGridDeleteRowScenarios), "TestDeletingMultipleRowAndVirtualization");
            RunSteps += CreateTestStepFromGeneric(typeof(DataGridDeleteRowScenarios), "TestDeletingRowsThroughBulkSelection");
            RunSteps += CreateTestStepFromGeneric(typeof(DataGridDeleteRowScenarios), "TestDeletingAllRows");
            RunSteps += CreateTestStepFromGeneric(typeof(DataGridDeleteRowScenarios), "TestDeletingWhileInEditMode");

            RunSteps += CreateTestStepFromGeneric(typeof(DataGridDeleteRowScenarios), "TestDeletingWithNullCurrentCell");





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

            Status("Setup specific for DataGridDeleteRowScenarios");

            Assert.AssertTrue("Items in ItemsSource must be large enough to run these tests.", (MyDataGrid.Items.Count > 20));

            LogComment("Setup for DataGridDeleteRowScenarios was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Select a row and delete it.
        /// Verify SelectedItems and the data source are updated accordingly.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestDeletingARow<DT>() where DT : IDataGridDataType, new()
        {
            Status("TestDeletingARow");

            InitialSetup();

            int prevItemCount = MyDataGrid.Items.Count;
            DT itemToDelete = (DT)MyDataGrid.Items[0];

            DataGridActionHelper.DeleteRow(MyDataGrid, 0);

            // verify the count decreased
            if (MyDataGrid.Items.Count != prevItemCount - 1)
            {
                throw new TestValidationException(string.Format(
                    "The row was not deleted.  Expected count: {0}, Actual count: {1}",
                    prevItemCount - 1,
                    MyDataGrid.Items.Count));
            }

            // verify the item was removed
            int index = MyDataGrid.Items.IndexOf(itemToDelete);
            if (index != -1)
            {
                throw new TestValidationException(string.Format(
                    "The row item was not deleted.  Item exists at index: {0}",
                    index));
            }

            //

            LogComment("TestDeletingARow was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Select multiple rows and delete all at once.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestDeletingMultipleRow<DT>() where DT : IDataGridDataType, new()
        {
            Status("TestDeletingMultipleRow");

            InitialSetup();

            int prevItemCount = MyDataGrid.Items.Count;
            DT[] itemsToDelete = new DT[] {
                (DT)MyDataGrid.Items[0],
                (DT)MyDataGrid.Items[1],
                (DT)MyDataGrid.Items[2],
                (DT)MyDataGrid.Items[4],
                (DT)MyDataGrid.Items[7] };

            DataGridActionHelper.DeleteRows(MyDataGrid, new int[] { 0, 1, 2, 4, 7 });

            // verify the count decreased
            if (MyDataGrid.Items.Count != prevItemCount - itemsToDelete.Length)
            {
                throw new TestValidationException(string.Format(
                    "The row was not deleted.  Expected count: {0}, Actual count: {1}",
                    prevItemCount - itemsToDelete.Length,
                    MyDataGrid.Items.Count));
            }

            // verify the items were all removed
            foreach (DT itemToDelete in itemsToDelete)
            {
                int index = MyDataGrid.Items.IndexOf(itemToDelete);
                if (index != -1)
                {
                    throw new TestValidationException(string.Format(
                        "The row item was not deleted.  Item exists at index: {0}",
                        index));
                }
            }

            //

            LogComment("TestDeletingMultipleRow was successful");
            return TestResult.Pass;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestDeletingMultipleRowAndVirtualization<DT>() where DT : IDataGridDataType, new()
        {
            Status("TestDeletingMultipleRowAndVirtualization");

            InitialSetup();

            int prevItemCount = MyDataGrid.Items.Count;
            DT[] itemsToDelete = new DT[] {
                (DT)MyDataGrid.Items[0],
                (DT)MyDataGrid.Items[1],
                (DT)MyDataGrid.Items[2],
                (DT)MyDataGrid.Items[4],
                (DT)MyDataGrid.Items[7] };

            // select the rows
            foreach (int row in new int[] { 0, 1, 2, 4, 7 })
            {
                DataGridActionHelper.SelectRow(MyDataGrid, row, true, false);
            }

            // navigate to the bottom of the list
            MyDataGrid.ScrollIntoView(MyDataGrid.Items[MyDataGrid.Items.Count - 1]);
            QueueHelper.WaitTillQueueItemsProcessed();

            // then delete
            UserInput.KeyPress(System.Windows.Input.Key.Delete, true);
            QueueHelper.WaitTillQueueItemsProcessed();

            // verify the count decreased
            if (MyDataGrid.Items.Count != prevItemCount - itemsToDelete.Length)
            {
                throw new TestValidationException(string.Format(
                    "The row was not deleted.  Expected count: {0}, Actual count: {1}",
                    prevItemCount - itemsToDelete.Length,
                    MyDataGrid.Items.Count));
            }

            // verify the items were all removed
            foreach (DT itemToDelete in itemsToDelete)
            {
                int index = MyDataGrid.Items.IndexOf(itemToDelete);
                if (index != -1)
                {
                    throw new TestValidationException(string.Format(
                        "The row item was not deleted.  Item exists at index: {0}",
                        index));
                }
            }

            //

            LogComment("TestDeletingMultipleRowAndVirtualization was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Select all and delete
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestDeletingRowsThroughBulkSelection<DT>() where DT : IDataGridDataType, new()
        {
            Status("TestDeletingRowsThroughBulkSelection");

            InitialSetup();

            // select
            MyDataGrid.SelectAll();
            QueueHelper.WaitTillQueueItemsProcessed();

            // delete
            UserInput.KeyPress(System.Windows.Input.Key.Delete, true);
            QueueHelper.WaitTillQueueItemsProcessed();

            // verify the count is cleared (NewItemPlaceholder is still there)
            if (MyDataGrid.Items.Count != 1)
            {
                throw new TestValidationException(string.Format(
                    "Not all rows were deleted.  Expected count: 1, Actual count: {0}",
                    MyDataGrid.Items.Count));
            }

            LogComment("TestDeletingRowsThroughBulkSelection was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Delete all rows one by one.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestDeletingAllRows<DT>() where DT : IDataGridDataType, new()
        {
            Status("TestDeletingAllRows");

            InitialSetup();

            DataGridActionHelper.SelectRow(MyDataGrid, 0, false, false);

            int count = MyDataGrid.Items.Count;
            while (count > 0)
            {
                int prevItemCount = MyDataGrid.Items.Count;
                object itemToDelete;
                if (prevItemCount == 1)
                {
                    itemToDelete = MyDataGrid.Items[0];
                }
                else
                {
                    itemToDelete = (DT)MyDataGrid.Items[0];
                }

                UserInput.KeyPress(System.Windows.Input.Key.Delete, true);
                QueueHelper.WaitTillQueueItemsProcessed();

                if (prevItemCount == 1)
                {
                    // verify the count does not decrease
                    if (MyDataGrid.Items.Count != prevItemCount)
                    {
                        throw new TestValidationException(string.Format(
                            "The row was still deleted.  Expected count: {0}, Actual count: {1}",
                            prevItemCount,
                            MyDataGrid.Items.Count));
                    }
                }
                else
                {
                    // verify the count decreased
                    if (MyDataGrid.Items.Count != prevItemCount - 1)
                    {
                        throw new TestValidationException(string.Format(
                            "The row was not deleted.  Expected count: {0}, Actual count: {1}",
                            prevItemCount - 1,
                            MyDataGrid.Items.Count));
                    }

                    // verify the item was removed
                    int index = MyDataGrid.Items.IndexOf(itemToDelete);
                    if (index != -1)
                    {
                        throw new TestValidationException(string.Format(
                            "The row item was not deleted.  Item exists at index: {0}",
                            index));
                    }
                }

                count--;
            }

            LogComment("TestDeletingAllRows was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// While in Edit mode of a CheckBox column press delete.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestDeletingWhileInEditMode<DT>() where DT : IDataGridDataType, new()
        {
            Status("TestDeletingWhileInEditMode");

            int testRow = 0;
            int testCol = DataGridHelper.FindFirstColumnTypeIndex(MyDataGrid, DataGridHelper.ColumnTypes.DataGridCheckBoxColumn);

            EditingStepInfo<DT>[] steps = new EditingStepInfo<DT>[]
                {
                    new EditingStepInfo<DT> {
                        debugComments = "open the cell for edit",
                        row = testRow,
                        col = testCol,
                        DoAction = this.DoBeginEditing<DT>,
                        VerifyAfterAction = this.VerifyAfterBegin<DT>,
                        beginAction = DataGridCommandHelper.BeginEditAction.F2 },

                    new EditingStepInfo<DT> {
                        debugComments = "press delete while in cell edit mode",
                        row = testRow,
                        col = testCol,
                        DoAction = this.DoDelete<DT>,
                        VerifyAfterAction = null /* nothing should happen and no exceptions should occur */},

                    new EditingStepInfo<DT> {
                        debugComments = "begin editing",
                        row = testRow,
                        col = testCol,
                        DoAction = this.DoEditCell<DT> },

                    new EditingStepInfo<DT> {
                        debugComments = "cancel the cell edit",
                        row = testRow,
                        col = testCol,
                        DoAction = this.DoCancelEditing<DT>,
                        VerifyAfterAction = this.VerifyAfterCancelCell<DT>,
                        cancelAction = DataGridCommandHelper.CancelEditAction.Esc },

                    new EditingStepInfo<DT> {
                        debugComments = "press delete while in row edit mode",
                        row = testRow,
                        col = testCol,
                        DoAction = this.DoDelete<DT>,
                        VerifyAfterAction = this.VerifyAfterDelete<DT> },
                };

            this.DoEditingSteps<DT>(steps);

            LogComment("TestDeletingWhileInEditMode was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 1. Set the CurrentCell to null
        /// 2. Select all rows
        /// 3. Delete the rows through the DeleteCommand
        ///
        /// Verify rows are deleted
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestDeletingWithNullCurrentCell<DT>() where DT : IDataGridDataType, new()
        {
            Status("TestDeletingWithNullCurrentCell");

            InitialSetup();
            MyDataGrid.Focus();

            LogComment("Set the CurrentCell to null");
            MyDataGrid.CurrentCell = new DataGridCellInfo(DependencyProperty.UnsetValue, MyDataGrid.Columns[0]);
            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("Select all rows");
            MyDataGrid.SelectAll();
            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("press delete");
            UserInput.KeyPress(System.Windows.Input.Key.Delete, true);
            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("verify the count is cleared (NewItemPlaceholder is still there)");
            if (MyDataGrid.Items.Count != 1)
            {
                throw new TestValidationException(string.Format(
                    "Not all rows were deleted.  Expected count: 1, Actual count: {0}",
                    MyDataGrid.Items.Count));
            }

            LogComment("TestDeletingWithNullCurrentCell was successful");
            return TestResult.Pass;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestDeletingAndSorting()
        {
            Status("TestDeletingAndSorting");


            LogComment("TestDeletingAndSorting was successful");
            return TestResult.Pass;
        }

        #endregion Test Steps

        #region Helpers

        private void InitialSetup()
        {
            if (MyDataGrid.Items.Count <= 1)
            {
                // recreate the DataSource and set ItemsSource
                this.CreateDataSource();
                this.SetupDataSource();
            }

            // clear all selection
            MyDataGrid.UnselectAll();
            QueueHelper.WaitTillQueueItemsProcessed();
        }

        protected virtual void DoDelete<T>(EditingStepInfo<T> editingStepInfo, ref EditingData<T> expectedData)
        {
            UserInput.KeyPress(System.Windows.Input.Key.Delete, true);
            QueueHelper.WaitTillQueueItemsProcessed();
        }

        protected virtual void VerifyAfterDelete<T>(EditingStepInfo<T> editingStepInfo, ref EditingData<T> expectedData) where T : IDataGridDataType
        {
            foreach (object dgItem in MyDataGrid.Items)
            {
                if (!(dgItem is T) && dgItem == CollectionView.NewItemPlaceholder)
                    continue;

                T item = (T)dgItem;
                if (expectedData.prevRowData.CustomEquals(item))
                {
                    throw new TestValidationException(string.Format("Row is expected to be deleted but exists in the DataGrid"));
                }
            }

            // verify the current row is not in edit mode
            DataGridVerificationHelper.VerifyCurrentCellEditMode(
                MyDataGrid,
                editingStepInfo.row,
                editingStepInfo.col,
                false   /* expected IsEditing value */,
                false   /* do not verify the CurrentCell info */,
                -1      /* the new current row */,
                -1      /* the new current col */);
            DataGridVerificationHelper.VerifyCurrentRowEditMode(
                MyDataGrid,
                editingStepInfo.row,
                false);
        }

        #endregion Helpers
    }
}
