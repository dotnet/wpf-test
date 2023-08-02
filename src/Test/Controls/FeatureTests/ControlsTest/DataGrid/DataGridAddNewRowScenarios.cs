using System;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Avalon.Test.ComponentModel;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;


namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Tests for adding a new row.  
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridAddNewRowScenarios", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords = "Localization_Suite")]  
    public class DataGridAddNewRowScenarios : DataGridEditingRowEditingScenarios
    {
        #region Private Fields

        private bool isNewItemInitialized = false;

        #endregion Private Fields

        #region Constructor

        public DataGridAddNewRowScenarios()
            //: this("Microsoft.Test.Controls.DataSources.People, ControlsCommon", "Microsoft.Test.Controls.DataSources.Person, ControlsCommon")
            : this("Microsoft.Test.Controls.DataSources.EditablePeople, ControlsCommon", "Microsoft.Test.Controls.DataSources.EditablePerson, ControlsCommon")
        {
        }

        [Variation("Microsoft.Test.Controls.DataSources.People, ControlsCommon", "Microsoft.Test.Controls.DataSources.Person, ControlsCommon")]
        [Variation("Microsoft.Test.Controls.DataSources.EditablePeople, ControlsCommon", "Microsoft.Test.Controls.DataSources.EditablePerson, ControlsCommon")]
        [Variation("System.Collections.ObjectModel.ObservableCollection`1", "Microsoft.Test.Controls.DataSources.EditablePerson, ControlsCommon")]
        public DataGridAddNewRowScenarios(string dataSourceName, string dataTypeName)
            : base(dataSourceName, dataTypeName)
        {
            RunSteps += CreateTestStepFromGeneric(typeof(DataGridAddNewRowScenarios), "TestUndoingRowAfterOpenningForEdit");
            RunSteps += CreateTestStepFromGeneric(typeof(DataGridAddNewRowScenarios), "TestSortingThenAddingNewRow");
            RunSteps += CreateTestStepFromGeneric(typeof(DataGridAddNewRowScenarios), "TestCustomizingNewItemInit");

            //RunSteps += CreateTestStepFromGeneric(typeof(DataGridAddNewRowScenarios), "TestChangesToItemsSource");
            //RunSteps += CreateTestStepFromGeneric(typeof(DataGridAddNewRowScenarios), "TestChangesToReadOnly");
            //RunSteps += CreateTestStepFromGeneric(typeof(DataGridAddNewRowScenarios), "TestChangesToNewItemsPlaceholderPosition");

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

            Status("Setup specific for DataGridAddNewRowScenarios");


            LogComment("Setup for DataGridAddNewRowScenarios was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Edit multiple cells using: {tab} and commit row.  Verify all cells are committed.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        protected override TestResult TestEditingMultipleRowCellsWithTab<DT>()
        {
            Status("TestEditingMultipleRowCellsWithTab");

            this.ValidateInitialStep();

            // set a row to begin with
            int testRow = MyDataGrid.Items.Count - 1;
            int testCol = 0;

            this.TestEditingMultipleRowCellsWithTabHelper<DT>(testRow, testCol);

            LogComment("TestEditingMultipleRowCellsWithTab was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Edit multiple cells using: {mouse clicks on different cells of same row} and commit row.  Verify all cells are committed.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        protected override TestResult TestEditingMultipleRowCellsWithMouseClicks<DT>()
        {
            Status("TestEditingMultipleRowCellsWithMouseClicks");

            this.ValidateInitialStep();

            // set a row to begin with
            int testRow = MyDataGrid.Items.Count - 1;
            int testCol = 0;

            this.TestEditingMultipleRowCellsWithMouseClicksHelper<DT>(testRow, testCol);

            LogComment("TestEditingMultipleRowCellsWithMouseClicks was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// While in row editing mode, edit several cells.  Press ESC twice.  
        /// Verify on first escape, current cell is reverted.  Verify on second escape, the entire row is reverted.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        protected override TestResult TestUndoingRowEdits<DT>()
        {
            Status("TestUndoingRowEdits");

            this.ValidateInitialStep();

            // set a row to begin with
            int testRow = MyDataGrid.Items.Count - 1;
            int testCol = 0;

            this.TestUndoingRowEditsHelper<DT>(testRow, testCol);

            LogComment("TestUndoingRowEdits was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// While in row editing mode, edit several cells.  Press ESC.  Do a commit.  
        /// Verify on escape, current cell is reverted.  Verify on commit, the other cells are commited.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        protected override TestResult TestUndoingCellEditButCommitingRest<DT>()
        {
            Status("TestUndoingCellEditButCommitingRest");

            this.ValidateInitialStep();

            // set a row to begin with
            int testRow = MyDataGrid.Items.Count - 1;
            int testCol = 0;

            this.TestUndoingCellEditButCommitingRestHelper<DT>(testRow, testCol);

            LogComment("TestUndoingCellEditButCommitingRest was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// While in row editing mode, edit several cells.  Press ESC.  Edit more cells.  Do a commit.  
        /// Verify on escape, current cell is reverted.  Verify on commit, all edited cells are commited.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        protected override TestResult TestUndoingCellEditsAndReEditing<DT>()
        {
            Status("TestUndoingCellEditsAndReEditing");

            this.ValidateInitialStep();

            // set a row to begin with
            int testRow = MyDataGrid.Items.Count - 1;
            int testCol = 0;

            this.TestUndoingCellEditsAndReEditingHelper<DT>(testRow, testCol);

            LogComment("TestUndoingCellEditsAndReEditing was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// While in row editing mode, edit several cells.  Do a commit.  Press ESC twice.  
        /// Verify on first escape, nothing happens.  Verify on seconde escape, nothing happens.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        protected override TestResult TestUndoingCellEditsAfterCommiting<DT>()
        {
            Status("TestUndoingCellEditsAfterCommiting");

            this.ValidateInitialStep();

            // set a row to begin with
            int testRow = MyDataGrid.Items.Count - 1;
            int testCol = 0;

            this.TestUndoingCellEditsAfterCommitingHelper<DT>(testRow, testCol);

            LogComment("TestUndoingCellEditsAfterCommiting was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// While in row editing mode, edit several cells.  Do a commit by pressing on a column header.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        protected override TestResult TestCommitingRowAndSorting<DT>()
        {
            Status("TestCommitingRowAndSorting");

            this.ValidateInitialStep();

            // set a row to begin with
            int testRow = MyDataGrid.Items.Count - 1;
            int testCol = 0;

            this.TestCommitingRowAndSortingHelper<DT>(testRow, testCol);

            // clean up any sorting
            DataGridHelper.ClearAnySortingDescrptions(MyDataGrid);

            LogComment("TestCommitingRowAndSorting was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Open the new row for edit then press ESC to remove the new item.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestUndoingRowAfterOpenningForEdit<DT>() where DT : IDataGridDataType, new()
        {
            Status("TestUndoingCellEditsAfterCommiting");

            // additonal config testing with selectionmode and selection 
            DataGridSelectionMode previousSelectionMode = MyDataGrid.SelectionMode;
            DataGridSelectionUnit previousSelectionUnit = MyDataGrid.SelectionUnit;

            foreach (DataGridSelectionMode selectionMode in Enum.GetValues(typeof(DataGridSelectionMode)))
            {
                foreach (DataGridSelectionUnit selectionUnit in Enum.GetValues(typeof(DataGridSelectionUnit)))
                {
                    this.ValidateInitialStep();

                    // set a row to begin with
                    int testRow = MyDataGrid.Items.Count - 1;
                    int testCol = 0;
                    MyDataGrid.SelectionUnit = selectionUnit;
                    MyDataGrid.SelectionMode = selectionMode;
                    QueueHelper.WaitTillQueueItemsProcessed();

                    EditingStepInfo<DT>[] steps = new EditingStepInfo<DT>[] 
                        {
                            new EditingStepInfo<DT> {
                                debugComments = "open first cell for edit",
                                row = testRow, 
                                col = testCol, 
                                DoAction = this.DoBeginEditing<DT>, 
                                VerifyAfterAction = this.VerifyAfterBegin<DT>, 
                                beginAction = DataGridCommandHelper.BeginEditAction.F2 },

                            new EditingStepInfo<DT> { 
                                debugComments = "begin editing",
                                row = testRow, 
                                col = testCol, 
                                DoAction = this.DoEditCell<DT> },                    

                            new EditingStepInfo<DT> { 
                                debugComments = "cancel the edit",
                                row = testRow, 
                                col = testCol, 
                                DoAction = this.DoCancelEditing<DT>, 
                                VerifyAfterAction = this.VerifyAfterCancelCell<DT>,  
                                cancelAction = DataGridCommandHelper.CancelEditAction.Esc },

                            new EditingStepInfo<DT> { 
                                debugComments = "cancel again",
                                row = testRow, 
                                col = testCol, 
                                DoAction = this.DoCancelEditing<DT>, 
                                VerifyAfterAction = this.VerifyAfterCancelRow<DT>, 
                                cancelAction = DataGridCommandHelper.CancelEditAction.Esc }
                        };

                    this.DoEditingSteps<DT>(steps);
                }
            }

            MyDataGrid.SelectionMode = previousSelectionMode;
            MyDataGrid.SelectionUnit = previousSelectionUnit;

            LogComment("TestUndoingCellEditsAfterCommiting was successful");
            return TestResult.Pass;
        }
        
        /// <summary>
        /// Sort the items then try to add a new row
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestSortingThenAddingNewRow<DT>() where DT : IDataGridDataType, new()
        {
            Status("TestSortingThenAddingNewRow");

            this.ValidateInitialStep();

            // set a row to begin with
            int testRow = MyDataGrid.Items.Count - 1;
            int testCol = 0;

            // do an auto-sort
            DataGridActionHelper.ClickOnColumnHeader(MyDataGrid, 0);
            QueueHelper.WaitTillQueueItemsProcessed();

            // verify NewItemPlaceholder is still at the bottom
            if (MyDataGrid.Items[MyDataGrid.Items.Count - 1] != CollectionView.NewItemPlaceholder)
            {
                throw new TestValidationException(string.Format("Expects the last item to be the NewItemPlaceholder.  Actual: {0}", MyDataGrid.Items[MyDataGrid.Items.Count - 1]));
            }

            EditingStepInfo<DT>[] steps = new EditingStepInfo<DT>[] 
                {
                    new EditingStepInfo<DT> {
                        debugComments = "open first cell for edit",
                        row = testRow, 
                        col = testCol, 
                        DoAction = this.DoBeginEditing<DT>, 
                        VerifyAfterAction = this.VerifyAfterBegin<DT>, 
                        beginAction = DataGridCommandHelper.BeginEditAction.F2 },

                    new EditingStepInfo<DT> { 
                        debugComments = "begin editing",
                        row = testRow, 
                        col = testCol, 
                        DoAction = this.DoEditCell<DT> },

                    new EditingStepInfo<DT> { 
                        debugComments = "tab to the next cell",
                        row = testRow, 
                        col = testCol, 
                        DoAction = this.DoCommitEditing<DT>, 
                        VerifyAfterAction = this.VerifyAfterCommitCell<DT>, 
                        commitAction = DataGridCommandHelper.CommitEditAction.Tab},

                    new EditingStepInfo<DT> { 
                        debugComments = "begin editing",
                        row = testRow, 
                        col = ++testCol, 
                        DoAction = this.DoEditCell<DT> },                                              

                    new EditingStepInfo<DT> { 
                        debugComments = "commit the entire row",
                        row = testRow, 
                        col = testCol, 
                        columnIsSorted = true,
                        DoAction = this.DoCommitEditing<DT>, 
                        VerifyAfterAction = this.VerifyAfterCommitRow<DT>, 
                        commitAction = DataGridCommandHelper.CommitEditAction.Enter }
                };

            this.DoEditingSteps<DT>(steps);

            // clean up sorting
            DataGridHelper.ClearAnySortingDescrptions(MyDataGrid);

            LogComment("TestUndoingCellEditsAfterCommiting was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Adds default values to the DataItem before it is added.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestCustomizingNewItemInit<DT>() where DT : IDataGridDataType, new()
        {
            Status("TestCustomizingNewItemInitialization");

            this.ValidateInitialStep();

            // set a row to begin with
            int testRow = MyDataGrid.Items.Count - 1;
            int testCol = 0;

            EditingStepInfo<DT>[] steps = new EditingStepInfo<DT>[] 
                {
                    new EditingStepInfo<DT> {
                        debugComments = "open first cell for edit",
                        row = testRow, 
                        col = testCol, 
                        DoAction = this.DoCustomBeginEditing<DT>, 
                        VerifyAfterAction = this.VerifyAfterBegin<DT>, 
                        beginAction = DataGridCommandHelper.BeginEditAction.F2 },                                                                

                    new EditingStepInfo<DT> { 
                        debugComments = "commit the entire row",
                        row = testRow, 
                        col = testCol, 
                        DoAction = this.DoCommitEditing<DT>, 
                        VerifyAfterAction = this.VerifyAfterCommitRow<DT>, 
                        commitAction = DataGridCommandHelper.CommitEditAction.Enter }
                };

            this.DoEditingSteps<DT>(steps);

            LogComment("TestCustomizingNewItemInitialization was successful");
            return TestResult.Pass;
        }

        #endregion Test Steps

        #region Helpers

        private void ValidateInitialStep()
        {
            // verify the NewItemPlaceholder is present
            Assert.AssertTrue("DataGrid.CanUserAddRows must be true to run test.", MyDataGrid.CanUserAddRows);
            if (MyDataGrid.Items[MyDataGrid.Items.Count - 1] != CollectionView.NewItemPlaceholder)
            {
                throw new TestValidationException(string.Format("Expects the last item to be the NewItemPlaceholder.  Actual: {0}", MyDataGrid.Items[MyDataGrid.Items.Count - 1]));
            }

            // set the init new item flag to false;
            this.isNewItemInitialized = false;
        }

        protected override void DoBeginEditing<T>(EditingStepInfo<T> editingStepInfo, ref EditingData<T> expectedData)
        {
            // listen to event to get the expected row data
            T newItem = default(T);
            EventHelper.ExpectEvent<InitializingNewItemEventArgs>(
                () =>
                {
                    this.DoBeginEditing(editingStepInfo.row, editingStepInfo.col, editingStepInfo.beginAction);
                },
                MyDataGrid,
                "InitializingNewItem",
                (sender, args) =>
                {
                    newItem = (T)args.NewItem;
                });

            if (!isNewItemInitialized)
            {
                isNewItemInitialized = true;
                expectedData.rowData = newItem;
                expectedData.prevRowData = newItem;
            }
        }

        protected virtual void DoCustomBeginEditing<T>(EditingStepInfo<T> editingStepInfo, ref EditingData<T> expectedData)
        {
            // listen to event to get the expected row data
            T newItem = default(T);
            EventHelper.ExpectEvent<InitializingNewItemEventArgs>(
                () =>
                {
                    this.DoBeginEditing(editingStepInfo.row, editingStepInfo.col, editingStepInfo.beginAction);
                },
                MyDataGrid,
                "InitializingNewItem",
                (sender, args) =>
                {
                    newItem = (T)args.NewItem;

                    (newItem as Person).FirstName = "Default FirstName";
                    (newItem as Person).LastName = "Default LastName";
                });

            if (!isNewItemInitialized)
            {
                isNewItemInitialized = true;
                expectedData.rowData = newItem;
                expectedData.prevRowData = newItem;
            }
        }

        protected override void VerifyAfterBegin<T>(EditingStepInfo<T> editingStepInfo, ref EditingData<T> expectedData)
        {
            base.VerifyAfterBegin<T>(editingStepInfo, ref expectedData);

            // verify a new row is created
            if (!(expectedData.rowData as IDataGridDataType).CustomEquals((T)MyDataGrid.Items[editingStepInfo.row]))
            {
                throw new TestValidationException(string.Format(
                    "The new item should have been updated to <T>.  Expect: {0}, Actual: {1}",
                    expectedData.rowData,
                    MyDataGrid.Items[editingStepInfo.row]));
            }

            //
        }

        protected override void VerifyAfterCommitRow<T>(EditingStepInfo<T> editingStepInfo, ref EditingData<T> expectedData)
        {
            QueueHelper.WaitTillQueueItemsProcessed();

            base.VerifyAfterCommitRow<T>(editingStepInfo, ref expectedData);

            // verify that the NewItemPlaceholder is created
            if (MyDataGrid.Items[MyDataGrid.Items.Count - 1] != CollectionView.NewItemPlaceholder)
            {
                throw new TestValidationException(string.Format(
                    "The next new item should have been created.  Expect: {0}, Actual: {1}",
                    CollectionView.NewItemPlaceholder,
                    MyDataGrid.Items[MyDataGrid.Items.Count - 1]));
            }
        }

        protected override void VerifyAfterCancelRow<T>(EditingStepInfo<T> editingStepInfo, ref EditingData<T> expectedData)
        {
            // row should be removed
            if (MyDataGrid.Items[editingStepInfo.row] != CollectionView.NewItemPlaceholder)
            {
                throw new TestValidationException(string.Format(
                    "The next new item should have been created.  Expect: {0}, Actual: {1}",
                    CollectionView.NewItemPlaceholder,
                    MyDataGrid.Items[editingStepInfo.row]));
            }

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
