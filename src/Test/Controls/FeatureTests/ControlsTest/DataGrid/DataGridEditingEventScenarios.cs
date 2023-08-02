using System.Windows.Controls;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;


namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Editing Event tests including cancelling begin, commit, and cancel commands
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridEditingEventScenarios", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DataGridEditingEventScenarios : DataGridEditing
    {
        #region Private Fields

        private bool cancelBeginEdit = false;
        private bool cancelCellEdit = false;
        private bool cancelRowEdit = false;

        #endregion Private Fields

        #region Constructor

        public DataGridEditingEventScenarios()
            : this("Microsoft.Test.Controls.DataSources.EditablePeople, ControlsCommon", "Microsoft.Test.Controls.DataSources.EditablePerson, ControlsCommon")
        {
        }

        [Variation("Microsoft.Test.Controls.DataSources.EditablePeople, ControlsCommon", "Microsoft.Test.Controls.DataSources.EditablePerson, ControlsCommon")]
        public DataGridEditingEventScenarios(string dataSourceName, string dataTypeName)
            : base(@"DataGridEditing.xaml")
        {
            this.DataSourceTypeName = dataSourceName;
            this.TypeNameFromDataSource = dataTypeName;
            this.CreateDataSource();

            InitializeSteps += new TestStep(Setup);
            RunSteps += CreateTestStepFromGeneric(typeof(DataGridEditingEventScenarios), "TestCancellingBeginningEdit");
            RunSteps += CreateTestStepFromGeneric(typeof(DataGridEditingEventScenarios), "TestCancellingCellEditEndingForCancelEdit");
            RunSteps += CreateTestStepFromGeneric(typeof(DataGridEditingEventScenarios), "TestCancellingRowEditEndingForCancelEdit");
            RunSteps += CreateTestStepFromGeneric(typeof(DataGridEditingEventScenarios), "TestCancellingCellEditEndingForCommitEdit");
            RunSteps += CreateTestStepFromGeneric(typeof(DataGridEditingEventScenarios), "TestCancellingRowEditEndingForCommitEdit");
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

            Status("Setup specific for DataGridEditingEventScenarios");

            MyDataGrid.BeginningEdit += new System.EventHandler<DataGridBeginningEditEventArgs>(MyDataGrid_BeginningEdit);
            MyDataGrid.CellEditEnding += new System.EventHandler<DataGridCellEditEndingEventArgs>(MyDataGrid_CellEditEnding);
            MyDataGrid.RowEditEnding += new System.EventHandler<DataGridRowEditEndingEventArgs>(MyDataGrid_RowEditEnding);

            LogComment("Setup for DataGridEditingEventScenarios was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestCancellingBeginningEdit<DT>() where DT : IDataGridDataType, new()
        {
            Status("TestCancellingBeginningEdit");

            int testRow = 0;
            int testCol = 0;

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
                        debugComments = "commit the entire row",
                        row = testRow, 
                        col = testCol, 
                        DoAction = this.DoCommitEditing<DT>, 
                        VerifyAfterAction = this.VerifyAfterCommitRow<DT>, 
                        commitAction = DataGridCommandHelper.CommitEditAction.Enter },

                    new EditingStepInfo<DT> { 
                        debugComments = "set BeginingEdit to cancel BeginEdit",
                        row = testRow, 
                        col = testCol, 
                        DoAction = this.SetupBeginEditForCancel<DT> },

                    new EditingStepInfo<DT> {
                        debugComments = "try to open first cell for edit",
                        row = testRow, 
                        col = testCol, 
                        DoAction = this.DoBeginEditing<DT>, 
                        VerifyAfterAction = this.VerifyBeginFails<DT>, 
                        beginAction = DataGridCommandHelper.BeginEditAction.F2 },

                    new EditingStepInfo<DT> { 
                        debugComments = "Cleanup: set BeginingEdit back to execute",
                        row = testRow, 
                        col = testCol, 
                        DoAction = this.SetupBeginEditToExecute<DT> }
                };

            this.DoEditingSteps<DT>(steps);

            LogComment("TestCancellingBeginningEdit was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestCancellingCellEditEndingForCancelEdit<DT>() where DT : IDataGridDataType, new()
        {
            Status("TestCancellingCellEditEndingForCancelEdit");

            int testRow = 0;
            int testCol = 0;

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
                        cancelAction = DataGridCommandHelper.CancelEditAction.Esc },

                    new EditingStepInfo<DT> { 
                        debugComments = "set CellEditEnding to cancel CancelEdit",
                        row = (testRow = 0), 
                        col = (testCol = 0), 
                        DoAction = this.SetupCellEditEndingForCancel<DT> },

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
                        debugComments = "try to cancel the cell edit",
                        row = testRow, 
                        col = testCol, 
                        DoAction = this.DoCancelEditing<DT>, 
                        VerifyAfterAction = this.VerifyCancelFails<DT>, 
                        cancelAction = DataGridCommandHelper.CancelEditAction.Esc },

                    new EditingStepInfo<DT> { 
                        debugComments = "Cleanup: set CancellingEdit back to execute",
                        row = testRow, 
                        col = testCol, 
                        DoAction = this.SetupCellEditEndingToExecute<DT> },

                    new EditingStepInfo<DT> { 
                        debugComments = "Cleanup: cancel the cell edit",
                        row = testRow, 
                        col = testCol, 
                        DoAction = this.DoCancelEditing<DT>, 
                        VerifyAfterAction = this.VerifyAfterCancelCell<DT>, 
                        cancelAction = DataGridCommandHelper.CancelEditAction.Esc },

                    new EditingStepInfo<DT> { 
                        debugComments = "Cleanup: cancel the entire row",
                        row = testRow, 
                        col = testCol, 
                        DoAction = this.DoCancelEditing<DT>, 
                        VerifyAfterAction = this.VerifyAfterCancelRow<DT>, 
                        cancelAction = DataGridCommandHelper.CancelEditAction.Esc },
                };

            this.DoEditingSteps<DT>(steps);

            LogComment("TestCancellingCellEditEndingForCancelEdit was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestCancellingRowEditEndingForCancelEdit<DT>() where DT : IDataGridDataType, new()
        {
            Status("TestCancellingRowEditEndingForCancelEdit");

            int testRow = 0;
            int testCol = 0;

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
                        cancelAction = DataGridCommandHelper.CancelEditAction.Esc },

                    new EditingStepInfo<DT> { 
                        debugComments = "set RowEditEnding to cancel CancelEdit",
                        row = (testRow = 0), 
                        col = (testCol = 0), 
                        DoAction = this.SetupRowEditEndingForCancel<DT> },

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
                        debugComments = "cancel the cell edit",
                        row = testRow, 
                        col = testCol, 
                        DoAction = this.DoCancelEditing<DT>, 
                        VerifyAfterAction = this.VerifyAfterCancelCell<DT>, 
                        cancelAction = DataGridCommandHelper.CancelEditAction.Esc },

                    new EditingStepInfo<DT> { 
                        debugComments = "try to cancel the row edit",
                        row = testRow, 
                        col = testCol, 
                        DoAction = this.DoCancelEditing<DT>, 
                        VerifyAfterAction = this.VerifyCancelRowFails<DT>, 
                        cancelAction = DataGridCommandHelper.CancelEditAction.Esc },

                    new EditingStepInfo<DT> { 
                        debugComments = "Cleanup: set CancellingEdit back to execute",
                        row = testRow, 
                        col = testCol, 
                        DoAction = this.SetupRowEditEndingToExecute<DT> },                   

                    new EditingStepInfo<DT> { 
                        debugComments = "Cleanup: cancel the entire row",
                        row = testRow, 
                        col = testCol, 
                        DoAction = this.DoCancelEditing<DT>, 
                        VerifyAfterAction = this.VerifyAfterCancelRow<DT>, 
                        cancelAction = DataGridCommandHelper.CancelEditAction.Esc },
                };

            this.DoEditingSteps<DT>(steps);

            LogComment("TestCancellingRowEditEndingForCancelEdit was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestCancellingCellEditEndingForCommitEdit<DT>() where DT : IDataGridDataType, new()
        {
            Status("TestCancellingCellEditEndingForCommitEdit");

            int testRow = 0;
            int testCol = 0;

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
                        commitAction = DataGridCommandHelper.CommitEditAction.Enter },

                    new EditingStepInfo<DT> { 
                        debugComments = "set CommittingEdit to cancel CommitEdit",
                        row = testRow, 
                        col = testCol, 
                        DoAction = this.SetupCellEditEndingForCancel<DT> },

                    new EditingStepInfo<DT> {
                        debugComments = "open first cell for edit",
                        row = (testRow = 0), 
                        col = (testCol = 0), 
                        DoAction = this.DoBeginEditing<DT>, 
                        VerifyAfterAction = this.VerifyAfterBegin<DT>, 
                        beginAction = DataGridCommandHelper.BeginEditAction.F2 },

                    new EditingStepInfo<DT> { 
                        debugComments = "begin editing",
                        row = testRow, 
                        col = testCol, 
                        DoAction = this.DoEditCell<DT> },

                    new EditingStepInfo<DT> { 
                        debugComments = "tab to the next cell: verify commit fails",
                        row = testRow, 
                        col = testCol, 
                        DoAction = this.DoCommitEditing<DT>, 
                        VerifyAfterAction = this.VerifyCommitFails<DT>, 
                        commitAction = DataGridCommandHelper.CommitEditAction.Tab},                    

                    new EditingStepInfo<DT> { 
                        debugComments = "Cleanup: set CommittingEdit back to execute",
                        row = (testRow = 0), 
                        col = (testCol = 0), 
                        DoAction = this.SetupCellEditEndingToExecute<DT> },

                    new EditingStepInfo<DT> { 
                        debugComments = "begin editing",
                        row = testRow, 
                        col = testCol, 
                        DoAction = this.DoEditCell<DT> },

                    new EditingStepInfo<DT> { 
                        debugComments = "Cleanup: commit the entire row",
                        row = testRow, 
                        col = testCol, 
                        DoAction = this.DoCommitEditing<DT>,                          
                        commitAction = DataGridCommandHelper.CommitEditAction.Enter }
                };

            this.DoEditingSteps<DT>(steps);

            LogComment("TestCancellingCellEditEndingForCommitEdit was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestCancellingRowEditEndingForCommitEdit<DT>() where DT : IDataGridDataType, new()
        {
            Status("TestCancellingRowEditEndingForCommitEdit");

            int testRow = 0;
            int testCol = 0;

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
                        commitAction = DataGridCommandHelper.CommitEditAction.Enter },

                    new EditingStepInfo<DT> { 
                        debugComments = "set RowEditEnding to cancel CommitEdit",
                        row = testRow, 
                        col = testCol, 
                        DoAction = this.SetupRowEditEndingForCancel<DT> },

                    new EditingStepInfo<DT> {
                        debugComments = "open first cell for edit",
                        row = (testRow = 0), 
                        col = (testCol = 0), 
                        DoAction = this.DoBeginEditing<DT>, 
                        VerifyAfterAction = this.VerifyAfterBegin<DT>, 
                        beginAction = DataGridCommandHelper.BeginEditAction.F2 },

                    new EditingStepInfo<DT> { 
                        debugComments = "begin editing",
                        row = testRow, 
                        col = testCol, 
                        DoAction = this.DoEditCell<DT> },

                    new EditingStepInfo<DT> { 
                        debugComments = "tab to the next cell: verify commit succeeds",
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
                        debugComments = "commit the entire row: verify commit fails",
                        row = testRow, 
                        col = testCol, 
                        DoAction = this.DoCommitEditing<DT>, 
                        VerifyAfterAction = this.VerifyCommitRowFails<DT>, 
                        commitAction = DataGridCommandHelper.CommitEditAction.Enter },

                    new EditingStepInfo<DT> { 
                        debugComments = "Cleanup: set CommittingEdit back to execute",
                        row = (testRow = 0), 
                        col = (testCol = 0), 
                        DoAction = this.SetupRowEditEndingToExecute<DT> },

                    new EditingStepInfo<DT> {
                        debugComments = "open first cell for edit",
                        row = (testRow = 0), 
                        col = (testCol = 0), 
                        DoAction = this.DoBeginEditing<DT>, 
                        VerifyAfterAction = this.VerifyAfterBegin<DT>, 
                        beginAction = DataGridCommandHelper.BeginEditAction.F2 },

                    new EditingStepInfo<DT> { 
                        debugComments = "begin editing",
                        row = testRow, 
                        col = testCol, 
                        DoAction = this.DoEditCell<DT> },

                    new EditingStepInfo<DT> { 
                        debugComments = "Cleanup: commit the entire row",
                        row = testRow, 
                        col = testCol, 
                        DoAction = this.DoCommitEditing<DT>,                          
                        commitAction = DataGridCommandHelper.CommitEditAction.Enter }
                };

            this.DoEditingSteps<DT>(steps);

            LogComment("TestCancellingRowEditEndingForCommitEdit was successful");
            return TestResult.Pass;
        }

        #endregion Test Steps

        #region Helpers

        protected virtual void VerifyBeginFails<T>(EditingStepInfo<T> editingStepInfo, ref EditingData<T> expectedData)
        {
            DataGridVerificationHelper.VerifyCurrentCellEditMode(
                MyDataGrid,
                editingStepInfo.row,
                editingStepInfo.col,
                false       /* expected IsEditing value */,
                false       /* do not verify the CurrentCell info */,
                -1          /* the new current row */,
                -1          /* the new current col */);
            DataGridVerificationHelper.VerifyCurrentRowEditMode(
                MyDataGrid,
                editingStepInfo.row,
                false);
        }

        protected virtual void VerifyCancelFails<T>(EditingStepInfo<T> editingStepInfo, ref EditingData<T> expectedData)
        {
            this.VerifyAfterBegin(editingStepInfo.row, editingStepInfo.col);
        }

        protected virtual void VerifyCancelRowFails<T>(EditingStepInfo<T> editingStepInfo, ref EditingData<T> expectedData)
        {
            DataGridVerificationHelper.VerifyCurrentCellEditMode(
                MyDataGrid,
                editingStepInfo.row,
                editingStepInfo.col,
                false    /* expected IsEditing value */,
                false   /* verify the CurrentCell info */,
                -1      /* the new current row */,
                -1      /* the new current col */);
            DataGridVerificationHelper.VerifyCurrentRowEditMode(
                MyDataGrid,
                editingStepInfo.row,
                true);
        }

        protected virtual void VerifyCommitFails<T>(EditingStepInfo<T> editingStepInfo, ref EditingData<T> expectedData)
        {
            // for particular actions, verify still in edit mode
            this.VerifyNewCellCommitResult<T>(editingStepInfo, expectedData);

            DataGridVerificationHelper.VerifyCurrentCellEditMode(
                MyDataGrid,
                editingStepInfo.row,
                editingStepInfo.col,
                true    /* expected IsEditing value */,
                false    /* verify the CurrentCell info */,
                -1     /* the new current row */,
                -1     /* the new current col */);
            DataGridVerificationHelper.VerifyCurrentRowEditMode(
                MyDataGrid,
                editingStepInfo.row,
                true);
        }

        protected virtual void VerifyCommitRowFails<T>(EditingStepInfo<T> editingStepInfo, ref EditingData<T> expectedData)
        {
            // currency should stay on the current row
            DataGridVerificationHelper.VerifyCurrentCellEditMode(
                    MyDataGrid,
                    editingStepInfo.row,
                    editingStepInfo.col,
                    false   /* expected IsEditing value */,
                    true    /* verify the CurrentCell info */,
                    editingStepInfo.row  /* the new current row */,
                    editingStepInfo.col  /* the new current col */);

            DataGridVerificationHelper.VerifyCurrentRowEditMode(
                MyDataGrid,
                editingStepInfo.row,
                true);
        }

        protected virtual void SetupBeginEditForCancel<T>(EditingStepInfo<T> editingStepInfo, ref EditingData<T> expectedData)
        {
            this.cancelBeginEdit = true;
        }

        protected virtual void SetupCellEditEndingForCancel<T>(EditingStepInfo<T> editingStepInfo, ref EditingData<T> expectedData)
        {
            this.cancelCellEdit = true;
        }

        protected virtual void SetupRowEditEndingForCancel<T>(EditingStepInfo<T> editingStepInfo, ref EditingData<T> expectedData)
        {
            this.cancelRowEdit = true;
        }

        protected virtual void SetupBeginEditToExecute<T>(EditingStepInfo<T> editingStepInfo, ref EditingData<T> expectedData)
        {
            this.cancelBeginEdit = false;
        }

        protected virtual void SetupCellEditEndingToExecute<T>(EditingStepInfo<T> editingStepInfo, ref EditingData<T> expectedData)
        {
            this.cancelCellEdit = false;
        }

        protected virtual void SetupRowEditEndingToExecute<T>(EditingStepInfo<T> editingStepInfo, ref EditingData<T> expectedData)
        {
            this.cancelRowEdit = false;
        }

        void MyDataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = this.cancelBeginEdit;
        }

        void MyDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            e.Cancel = this.cancelRowEdit;
        }

        void MyDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            e.Cancel = this.cancelCellEdit;
        }

        #endregion Helpers
    }
}
