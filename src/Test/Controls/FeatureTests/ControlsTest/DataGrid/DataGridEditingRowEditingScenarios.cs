using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;


namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Tests for row editing scenarios.
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridEditingRowEditingScenarios", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DataGridEditingRowEditingScenarios : DataGridEditing
    {
        #region Constructor

        public DataGridEditingRowEditingScenarios()
            : this("Microsoft.Test.Controls.DataSources.EditablePeople, ControlsCommon", "Microsoft.Test.Controls.DataSources.EditablePerson, ControlsCommon")
        {
        }

        [Variation("Microsoft.Test.Controls.DataSources.People, ControlsCommon", "Microsoft.Test.Controls.DataSources.Person, ControlsCommon")]
        [Variation("Microsoft.Test.Controls.DataSources.EditablePeople, ControlsCommon", "Microsoft.Test.Controls.DataSources.EditablePerson, ControlsCommon")]
        public DataGridEditingRowEditingScenarios(string dataSourceName, string dataTypeName)
            : base(@"DataGridEditing.xaml")
        {
            this.DataSourceTypeName = dataSourceName;
            this.TypeNameFromDataSource = dataTypeName;
            this.CreateDataSource();

            InitializeSteps += new TestStep(Setup);
            RunSteps += CreateTestStepFromGeneric(typeof(DataGridEditingRowEditingScenarios), "TestEditingMultipleRowCellsWithTab");
            RunSteps += CreateTestStepFromGeneric(typeof(DataGridEditingRowEditingScenarios), "TestEditingMultipleRowCellsWithMouseClicks");
            RunSteps += CreateTestStepFromGeneric(typeof(DataGridEditingRowEditingScenarios), "TestUndoingRowEdits");
            RunSteps += CreateTestStepFromGeneric(typeof(DataGridEditingRowEditingScenarios), "TestUndoingCellEditButCommitingRest");
            RunSteps += CreateTestStepFromGeneric(typeof(DataGridEditingRowEditingScenarios), "TestUndoingCellEditsAndReEditing");
            RunSteps += CreateTestStepFromGeneric(typeof(DataGridEditingRowEditingScenarios), "TestUndoingCellEditsAfterCommiting");
            RunSteps += CreateTestStepFromGeneric(typeof(DataGridEditingRowEditingScenarios), "TestCommitingRowAndSorting");
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

            Status("Setup specific for DataGridEditingRowEditingScenarios");



            LogComment("Setup for DataGridEditingRowEditingScenarios was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Edit multiple cells using: {tab} and commit row.  Verify all cells are committed.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        protected virtual TestResult TestEditingMultipleRowCellsWithTab<DT>() where DT : IDataGridDataType, new()
        {
            Status("TestEditingMultipleRowCellsWithTab");

            // set a row to begin with
            int testRow = 0;
            int testCol = 0;

            this.TestEditingMultipleRowCellsWithTabHelper<DT>(testRow, testCol);

            LogComment("TestEditingMultipleRowCellsWithTab was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Edit multiple cells using: {mouse clicks on different cells of same row} and commit row.  Verify all cells are committed.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        protected virtual TestResult TestEditingMultipleRowCellsWithMouseClicks<DT>() where DT : IDataGridDataType, new()
        {
            Status("TestEditingMultipleRowCellsWithMouseClicks");

            // set a row to begin with
            int testRow = 3;
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
        protected virtual TestResult TestUndoingRowEdits<DT>() where DT : IDataGridDataType, new()
        {
            Status("TestUndoingRowEdits");

            // set a row to begin with
            int testRow = 1;
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
        protected virtual TestResult TestUndoingCellEditButCommitingRest<DT>() where DT : IDataGridDataType, new()
        {
            Status("TestUndoingCellEditButCommitingRest");

            // set a row to begin with
            int testRow = 4;
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
        protected virtual TestResult TestUndoingCellEditsAndReEditing<DT>() where DT : IDataGridDataType, new()
        {
            Status("TestUndoingCellEditsAndReEditing");

            // set a row to begin with
            int testRow = 5;
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
        protected virtual TestResult TestUndoingCellEditsAfterCommiting<DT>() where DT : IDataGridDataType, new()
        {
            Status("TestUndoingCellEditsAfterCommiting");

            // set a row to begin with
            int testRow = 0;
            int testCol = 0;

            this.TestUndoingCellEditsAfterCommitingHelper<DT>(testRow, testCol);

            LogComment("TestUndoingCellEditsAfterCommiting was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// While in row editing mode, edit several cells.  Do a commit by pressing on a column header.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        protected virtual TestResult TestCommitingRowAndSorting<DT>() where DT : IDataGridDataType, new()
        {
            Status("TestCommitingRowAndSorting");

            // set a row to begin with
            int testRow = 0;
            int testCol = 0;

            this.TestCommitingRowAndSortingHelper<DT>(testRow, testCol);

            // clean up any sorting
            DataGridHelper.ClearAnySortingDescrptions(MyDataGrid);

            LogComment("TestCommitingRowAndSorting was successful");
            return TestResult.Pass;
        }

        #endregion Test Steps

        #region Helpers

        protected void TestEditingMultipleRowCellsWithTabHelper<DT>(int testRow, int testCol) where DT : IDataGridDataType, new()
        {
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
                        DoAction = this.DoCommitEditing<DT>,
                        VerifyAfterAction = this.VerifyAfterCommitRow<DT>,
                        commitAction = DataGridCommandHelper.CommitEditAction.Enter }
                };

            this.DoEditingSteps<DT>(steps);
        }

        protected void TestEditingMultipleRowCellsWithMouseClicksHelper<DT>(int testRow, int testCol) where DT : IDataGridDataType, new()
        {
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
                        debugComments = "click a cell in the same row",
                        row = testRow,
                        col = testCol,
                        mouseClickRowIdx = testRow,
                        mouseClickColIdx = testCol + 1,
                        DoAction = this.DoCommitEditing<DT>,
                        VerifyAfterAction = this.VerifyAfterCommitCell<DT>,
                        commitAction = DataGridCommandHelper.CommitEditAction.MouseClickDifferentCell},

                    new EditingStepInfo<DT> {
                        debugComments = "open the new cell for edit",
                        row = testRow,
                        col = (testCol = testCol + 1),
                        DoAction = this.DoBeginEditing<DT>,
                        VerifyAfterAction = this.VerifyAfterBegin<DT>,
                        beginAction = DataGridCommandHelper.BeginEditAction.F2 },

                    new EditingStepInfo<DT> {
                        debugComments = "begin editing",
                        row = testRow,
                        col = testCol,
                        DoAction = this.DoEditCell<DT> },

                    new EditingStepInfo<DT> {
                        debugComments = "click another cell in the same row",
                        row = testRow,
                        col = testCol,
                        mouseClickRowIdx = testRow,
                        mouseClickColIdx = testCol + 2,
                        DoAction = this.DoCommitEditing<DT>,
                        VerifyAfterAction = this.VerifyAfterCommitCell<DT>,
                        commitAction = DataGridCommandHelper.CommitEditAction.MouseClickDifferentCell},

                    new EditingStepInfo<DT> {
                        debugComments = "open the new cell for edit",
                        row = testRow,
                        col = (testCol = testCol + 2),
                        DoAction = this.DoBeginEditing<DT>,
                        VerifyAfterAction = this.VerifyAfterBegin<DT>,
                        beginAction = DataGridCommandHelper.BeginEditAction.F2 },

                    new EditingStepInfo<DT> {
                        debugComments = "begin editing",
                        row = testRow,
                        col = testCol,
                        DoAction = this.DoEditCell<DT> },

                    new EditingStepInfo<DT> {
                        debugComments = "commit the entire row",
                        row = testRow,
                        col = testCol,
                        DoAction = this.DoCommitEditing<DT>,
                        VerifyAfterAction = this.VerifyAfterCommitRow<DT>,
                        commitAction = DataGridCommandHelper.CommitEditAction.Enter }
                };

            this.DoEditingSteps<DT>(steps);
        }

        protected void TestUndoingRowEditsHelper<DT>(int testRow, int testCol) where DT : IDataGridDataType, new()
        {
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
                        debugComments = "cancel the cell edit",
                        row = testRow,
                        col = testCol,
                        DoAction = this.DoCancelEditing<DT>,
                        VerifyAfterAction = this.VerifyAfterCancelCell<DT>,
                        cancelAction = DataGridCommandHelper.CancelEditAction.Esc },

                    new EditingStepInfo<DT> {
                        debugComments = "cancel the entire row",
                        row = testRow,
                        col = testCol,
                        DoAction = this.DoCancelEditing<DT>,
                        VerifyAfterAction = this.VerifyAfterCancelRow<DT>,
                        cancelAction = DataGridCommandHelper.CancelEditAction.Esc }
                };

            this.DoEditingSteps<DT>(steps);
        }

        protected void TestUndoingCellEditButCommitingRestHelper<DT>(int testRow, int testCol) where DT : IDataGridDataType, new()
        {
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
                        debugComments = "cancel the cell edit",
                        row = testRow,
                        col = testCol,
                        DoAction = this.DoCancelEditing<DT>,
                        VerifyAfterAction = this.VerifyAfterCancelCell<DT>,
                        cancelAction = DataGridCommandHelper.CancelEditAction.Esc },

                    new EditingStepInfo<DT> {
                        debugComments = "commit the rest of the entire row",
                        row = testRow,
                        col = testCol,
                        DoAction = this.DoCommitEditing<DT>,
                        VerifyAfterAction = this.VerifyAfterCommitRow<DT>,
                        commitAction = DataGridCommandHelper.CommitEditAction.Enter }
                };

            this.DoEditingSteps<DT>(steps);
        }

        protected void TestUndoingCellEditsAndReEditingHelper<DT>(int testRow, int testCol) where DT : IDataGridDataType, new()
        {
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
                        debugComments = "cancel the cell edit",
                        row = testRow,
                        col = testCol,
                        DoAction = this.DoCancelEditing<DT>,
                        VerifyAfterAction = this.VerifyAfterCancelCell<DT>,
                        cancelAction = DataGridCommandHelper.CancelEditAction.Esc },

                    new EditingStepInfo<DT> {
                        debugComments = "click another cell in the same row",
                        row = testRow,
                        col = testCol,
                        mouseClickRowIdx = testRow,
                        mouseClickColIdx = testCol + 2,
                        DoAction = this.DoCommitEditing<DT>,
                        VerifyAfterAction = this.VerifyAfterCommitCell<DT>,
                        commitAction = DataGridCommandHelper.CommitEditAction.MouseClickDifferentCell},

                    new EditingStepInfo<DT> {
                        debugComments = "open the new cell for edit",
                        row = testRow,
                        col = (testCol = testCol + 2),
                        DoAction = this.DoBeginEditing<DT>,
                        VerifyAfterAction = this.VerifyAfterBegin<DT>,
                        beginAction = DataGridCommandHelper.BeginEditAction.F2 },

        /*  This cell is bound to the same property as the cell edited in the fourth step.
            If both are edited, it's ambiguous which value should be written back
            to the source item during the commit step.

                    new EditingStepInfo<DT> {
                        debugComments = "begin editing",
                        row = testRow,
                        col = testCol,
                        DoAction = this.DoEditCell<DT> },
        */

                    new EditingStepInfo<DT> {
                        debugComments = "commit the rest of the entire row",
                        row = testRow,
                        col = testCol,
                        DoAction = this.DoCommitEditing<DT>,
                        VerifyAfterAction = this.VerifyAfterCommitRow<DT>,
                        commitAction = DataGridCommandHelper.CommitEditAction.Enter }
                };

            this.DoEditingSteps<DT>(steps);
        }

        protected void TestUndoingCellEditsAfterCommitingHelper<DT>(int testRow, int testCol) where DT : IDataGridDataType, new()
        {
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
                        DoAction = this.DoCommitEditing<DT>,
                        VerifyAfterAction = this.VerifyAfterCommitRow<DT>,
                        commitAction = DataGridCommandHelper.CommitEditAction.MethodCallRowUnit },

                    new EditingStepInfo<DT> {
                        debugComments = "try to cancel",
                        row = testRow,
                        col = testCol,
                        DoAction = this.DoCancelEditing<DT>,
                        VerifyAfterAction = this.VerifyAfterCommitRow<DT> /* data should remain intact and state should be the same */,
                        cancelAction = DataGridCommandHelper.CancelEditAction.Esc },

                    new EditingStepInfo<DT> {
                        debugComments = "try to cancel again",
                        row = testRow,
                        col = testCol,
                        DoAction = this.DoCancelEditing<DT>,
                        VerifyAfterAction = this.VerifyAfterCommitRow<DT> /* data should remain intact and state should be the same */,
                        cancelAction = DataGridCommandHelper.CancelEditAction.Esc }
                };

            this.DoEditingSteps<DT>(steps);
        }

        protected void TestCommitingRowAndSortingHelper<DT>(int testRow, int testCol) where DT : IDataGridDataType, new()
        {
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
                        debugComments = "commit the entire row by clicking on a column header",
                        row = testRow,
                        col = testCol,
                        DoAction = this.DoCommitEditing<DT>,
                        VerifyAfterAction = this.VerifyAfterCommitRow<DT>,
                        commitAction = DataGridCommandHelper.CommitEditAction.MouseClickColumnHeader,
                        mouseClickColIdx = testCol }
                };

            this.DoEditingSteps<DT>(steps);
        }

        #endregion Helpers
    }
}
