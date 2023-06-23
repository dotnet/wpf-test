using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.Collections;

using Microsoft.Test.Controls.DataSources;
using Avalon.Test.ComponentModel;
using Avalon.Test.ComponentModel.Utilities;
using System.Windows.Data;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Behaviorial tests for DataGrid Row Validation
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridRowValidation", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DataGridRowValidation : DataGridEditing
    {
        #region Private Fields

        private bool isNewItemInitialized = false;

        #endregion Private Fields

        #region Constructor

        public DataGridRowValidation()
            : this("Microsoft.Test.Controls.DataSources.EditablePeople, ControlsCommon", "Microsoft.Test.Controls.DataSources.EditablePerson, ControlsCommon")
        {
        }

        [Variation("Microsoft.Test.Controls.DataSources.EditablePeople, ControlsCommon", "Microsoft.Test.Controls.DataSources.EditablePerson, ControlsCommon")]
        public DataGridRowValidation(string dataSourceName, string dataTypeName)
            : base(@"DataGridRowValidation.xaml")
        {
            this.DataSourceTypeName = dataSourceName;
            this.TypeNameFromDataSource = dataTypeName;
            this.CreateDataSource();

            InitializeSteps += new TestStep(Setup);
            RunSteps += CreateTestStepFromGeneric(typeof(DataGridRowValidation), "TestEditExistingRowAndValidationFails");
            RunSteps += CreateTestStepFromGeneric(typeof(DataGridRowValidation), "TestEditANewRowAndValidationFails");
            RunSteps += CreateTestStepFromGeneric(typeof(DataGridRowValidation), "TestEditExistingRowAndValidationSucceeds");
            RunSteps += CreateTestStepFromGeneric(typeof(DataGridRowValidation), "TestEditANewRowAndValidationSucceeds");
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

            Status("Setup specific for DataGridRowValidation");


            LogComment("Setup for DataGridRowValidation was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Action: edit a row and have validation fail
        /// Verify: Error template is shown, row is not committed, row is still in edit mode
        /// 
        /// 2nd Action: Cancel the row edit
        /// Verify: Error template is removed, row data is reverted back, row is not in edit mode        
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestEditExistingRowAndValidationFails<DT>() where DT : IDataGridDataType, new()
        {
            Status("TestEditExistingRowAndValidationFails");

            // set a row to begin with
            int testRow = 0;
            int testCol = -1;

            // look for FirstName column
            foreach(DataGridColumn column in MyDataGrid.Columns)
            {
                if(column.Header.ToString() == "FirstName")
                {
                    testCol = column.DisplayIndex;
                }
            }
            Assert.AssertTrue("The FirstName column must exist for the step to continue.", testCol != -1);

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
                        debugComments = "begin editing with bad input",
                        row = testRow, 
                        col = testCol, 
                        DoAction = this.DoEditCellWithBadInput<DT> },

                    new EditingStepInfo<DT> { 
                        debugComments = "tab to the next cell",
                        row = testRow, 
                        col = testCol, 
                        DoAction = this.DoCommitEditing<DT>, 
                        VerifyAfterAction = this.VerifyAfterCommitCell<DT>, 
                        commitAction = DataGridCommandHelper.CommitEditAction.Tab},                                              

                    new EditingStepInfo<DT> { 
                        debugComments = "commit the entire row",
                        row = testRow, 
                        col = testCol + 1, 
                        DoAction = this.DoCommitEditing<DT>, 
                        VerifyAfterAction = this.VerifyAfterCommitRowWithValidationFailure<DT>, 
                        commitAction = DataGridCommandHelper.CommitEditAction.Enter },                    

                    new EditingStepInfo<DT> {
                        debugComments = "navigate back to first cell",
                        row = testRow, 
                        col = testCol, 
                        DoAction = this.DoNavigation<DT>, 
                        VerifyAfterAction = null },

                    new EditingStepInfo<DT> { 
                        debugComments = "cancel the entire row",
                        row = testRow, 
                        col = testCol, 
                        DoAction = this.DoCancelEditing<DT>, 
                        VerifyAfterAction = this.VerifyAfterCancelRow<DT>, 
                        cancelAction = DataGridCommandHelper.CancelEditAction.Esc }
                };

            this.DoEditingSteps<DT>(steps);

            LogComment("TestEditExistingRowAndValidationFails was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Action: edit a new row and have validation fail
        /// Verify: Error template is shown, row is not committed, row is still in edit mode
        /// 
        /// 2nd Action: Cancel the row edit
        /// Verify: Error template is removed, row data is reverted back, row is not in edit mode        
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestEditANewRowAndValidationFails<DT>() where DT : IDataGridDataType, new()
        {
            Status("TestEditANewRowAndValidationFails");

            this.ValidateInitialStep();

            // set a row to begin with
            int testRow = MyDataGrid.Items.Count - 1;
            int testCol = -1;

            // look for FirstName column
            foreach (DataGridColumn column in MyDataGrid.Columns)
            {
                if (column.Header.ToString() == "FirstName")
                {
                    testCol = column.DisplayIndex;
                }
            }
            Assert.AssertTrue("The FirstName column must exist for the step to continue.", testCol != -1);

            EditingStepInfo<DT>[] steps = new EditingStepInfo<DT>[] 
                {
                    new EditingStepInfo<DT> {
                        debugComments = "open first cell for edit",
                        row = testRow, 
                        col = testCol, 
                        DoAction = this.DoBeginEditingOnNewRow<DT>, 
                        VerifyAfterAction = this.VerifyAfterBegin<DT>, 
                        beginAction = DataGridCommandHelper.BeginEditAction.F2 },

                    new EditingStepInfo<DT> { 
                        debugComments = "begin editing with bad input",
                        row = testRow, 
                        col = testCol, 
                        DoAction = this.DoEditCellWithBadInput<DT> },

                    new EditingStepInfo<DT> { 
                        debugComments = "tab to the next cell",
                        row = testRow, 
                        col = testCol, 
                        DoAction = this.DoCommitEditing<DT>, 
                        VerifyAfterAction = this.VerifyAfterCommitCell<DT>, 
                        commitAction = DataGridCommandHelper.CommitEditAction.Tab},                                              

                    new EditingStepInfo<DT> { 
                        debugComments = "commit the entire row",
                        row = testRow, 
                        col = testCol + 1, 
                        DoAction = this.DoCommitEditing<DT>, 
                        VerifyAfterAction = this.VerifyAfterCommitRowWithValidationFailure<DT>, 
                        commitAction = DataGridCommandHelper.CommitEditAction.Enter },                    

                    new EditingStepInfo<DT> {
                        debugComments = "navigate back to first cell",
                        row = testRow, 
                        col = testCol, 
                        DoAction = this.DoNavigation<DT>, 
                        VerifyAfterAction = null },

                    new EditingStepInfo<DT> { 
                        debugComments = "cancel the entire row",
                        row = testRow, 
                        col = testCol, 
                        DoAction = this.DoCancelEditing<DT>, 
                        VerifyAfterAction = this.VerifyAfterCancelNewRow<DT>, 
                        cancelAction = DataGridCommandHelper.CancelEditAction.Esc }
                };

            this.DoEditingSteps<DT>(steps);

            LogComment("TestEditANewRowAndValidationFails was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Action: edit a row and have validation succeed
        /// Verify: row is committed
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestEditExistingRowAndValidationSucceeds<DT>() where DT : IDataGridDataType, new()
        {
            Status("TestEditExistingRowAndValidationSucceeds");

            // set a row to begin with
            int testRow = 1;
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
                        VerifyAfterAction = this.VerifyAfterCommitRowWithValidationSuccess<DT>, 
                        commitAction = DataGridCommandHelper.CommitEditAction.Enter }
                };

            this.DoEditingSteps<DT>(steps);

            LogComment("TestEditExistingRowAndValidationSucceeds was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Action: edit a new row and have validation succeed
        /// Verify: row is committed
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestEditANewRowAndValidationSucceeds<DT>() where DT : IDataGridDataType, new()
        {
            Status("TestEditANewRowAndValidationSucceeds");

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
                        DoAction = this.DoBeginEditingOnNewRow<DT>, 
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
                        VerifyAfterAction = this.VerifyAfterCommitRowWithValidationSuccess<DT>, 
                        commitAction = DataGridCommandHelper.CommitEditAction.Enter }
                };

            this.DoEditingSteps<DT>(steps);

            LogComment("TestEditANewRowAndValidationSucceeds was successful");
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

        protected virtual void DoNavigation<T>(EditingStepInfo<T> editingStepInfo, ref EditingData<T> expectedData)
        {
            DataGridActionHelper.NavigateTo(MyDataGrid, editingStepInfo.row, editingStepInfo.col);
        }

        protected virtual void DoEditCellWithBadInput<T>(EditingStepInfo<T> editingStepInfo, ref EditingData<T> expectedData)
        {
            //NOTE: this badStringData is specific to ItemValidationRule1
            string badStringData = "XX";
            DataGridActionHelper.EditCellCustomInput(MyDataGrid, editingStepInfo.row, editingStepInfo.col, badStringData);

            // record the expected data
            this.SetPropertyValue<T>(editingStepInfo, ref expectedData, false, badStringData);
        }

        protected virtual void DoBeginEditingOnNewRow<T>(EditingStepInfo<T> editingStepInfo, ref EditingData<T> expectedData)
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
                IDataGridDataType temp = (IDataGridDataType)newItem;
                expectedData.rowData = (T)temp.Clone();
                expectedData.prevRowData = (T)temp.Clone();
            }
        }

        protected virtual void VerifyAfterCommitRowWithValidationFailure<T>(EditingStepInfo<T> editingStepInfo, ref EditingData<T> expectedData) where T : IDataGridDataType
        {
            //

            Assert.AssertTrue("Expected Enter action.", editingStepInfo.commitAction == DataGridCommandHelper.CommitEditAction.Enter);

            // verify that currency remains on the row
            DataGridVerificationHelper.VerifyCurrentCellEditMode(
                    MyDataGrid,
                    editingStepInfo.row,
                    editingStepInfo.col,
                    false   /* expected IsEditing value */,
                    true    /* verify the CurrentCell info */,
                    editingStepInfo.row  /* the current row */,
                    editingStepInfo.col  /* the current col */);

            // verify the current row is STILL in edit mode (if still in edit mode, then the row is not committed)
            DataGridVerificationHelper.VerifyCurrentRowEditMode(
                MyDataGrid,
                editingStepInfo.row,
                true);            

            // verify ErrorTemplate is shown
            DataGridRow row = DataGridHelper.GetRow(MyDataGrid, editingStepInfo.row);
            object hasErrorObj = row.GetValue(Validation.HasErrorProperty);
            bool hasError = (bool)hasErrorObj;
            if(!hasError)
            {
                throw new TestValidationException(string.Format(
                    "Validation.HasError attached property is not true on row: {0}, after a validation failure", 
                    editingStepInfo.row));
            }
        }

        protected virtual void VerifyAfterCommitRowWithValidationSuccess<T>(EditingStepInfo<T> editingStepInfo, ref EditingData<T> expectedData) where T : IDataGridDataType
        {
            this.VerifyAfterCommitRow<T>(editingStepInfo, ref expectedData);            

            // verify ErrorTemplate is NOT shown
            DataGridRow row = DataGridHelper.GetRow(MyDataGrid, editingStepInfo.row);
            object hasErrorObj = row.GetValue(Validation.HasErrorProperty);
            bool hasError = (bool)hasErrorObj;
            if (hasError)
            {
                throw new TestValidationException(string.Format(
                    "Validation.HasError attached property is true on row: {0}, after a validation success",
                    editingStepInfo.row));
            }
        }

        protected override void VerifyAfterCancelRow<T>(EditingStepInfo<T> editingStepInfo, ref EditingData<T> expectedData)
        {
            base.VerifyAfterCancelRow<T>(editingStepInfo, ref expectedData);

            // verify ErrorTemplate is removed
            DataGridRow row = DataGridHelper.GetRow(MyDataGrid, editingStepInfo.row);
            object hasErrorObj = row.GetValue(Validation.HasErrorProperty);
            bool hasError = (bool)hasErrorObj;

        }

        protected virtual void VerifyAfterCancelNewRow<T>(EditingStepInfo<T> editingStepInfo, ref EditingData<T> expectedData)
        {
            if (MyDataGrid.Items[editingStepInfo.row] != CollectionView.NewItemPlaceholder)
            {
                throw new TestValidationException(string.Format(
                    "item in row: {0} is suppose to be the NewItemPlaceholder.", 
                    editingStepInfo.row));
            }            
        }

        #endregion Helpers
    }
}
