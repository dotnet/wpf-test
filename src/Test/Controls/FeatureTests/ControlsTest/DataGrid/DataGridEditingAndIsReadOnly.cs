using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Controls.DataSources;
using System.Windows.Controls;
using System.Collections;
using System;
using System.Reflection;
using Avalon.Test.ComponentModel.Utilities;

using Avalon.Test.ComponentModel;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Tests around setting the IsReadOnly property on the DataGrid, DataGridColumn, and DataGridCell
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridEditingAndIsReadOnly", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DataGridEditingAndIsReadOnly : DataGridEditing
    {
        #region Constructor

        public DataGridEditingAndIsReadOnly()
            : this("Microsoft.Test.Controls.DataSources.EditablePeople, ControlsCommon", "Microsoft.Test.Controls.DataSources.EditablePerson, ControlsCommon")            
        {
        }

        [Variation("Microsoft.Test.Controls.DataSources.EditablePeople, ControlsCommon", "Microsoft.Test.Controls.DataSources.EditablePerson, ControlsCommon")]        
        public DataGridEditingAndIsReadOnly(string dataSourceName, string dataTypeName)
            : base(@"DataGridEditing.xaml")
        {
            // setup the data source
            this.DataSourceTypeName = dataSourceName;
            this.TypeNameFromDataSource = dataTypeName;
            this.CreateDataSource();

            InitializeSteps += new TestStep(Setup);
            RunSteps += CreateTestStepFromGeneric(typeof(DataGridEditingAndIsReadOnly), "TestDataGridInitialIsReadOnly");
            RunSteps += CreateTestStepFromGeneric(typeof(DataGridEditingAndIsReadOnly), "TestDataGridIsReadOnly");
            RunSteps += CreateTestStepFromGeneric(typeof(DataGridEditingAndIsReadOnly), "TestDataGridColumnIsReadOnly");
            RunSteps += CreateTestStepFromGeneric(typeof(DataGridEditingAndIsReadOnly), "TestIsReadOnlyPrecedence");
        }

        #endregion

        #region Test Steps

        #region Setup

        /// <summary>
        /// Initial Setup  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        public override TestResult Setup()
        {
            base.Setup();

            Status("Setup specific for DataGridEditingAndIsReadOnly");


            LogComment("Setup for DataGridEditingAndIsReadOnly was successful");
            return TestResult.Pass;
        }

        #endregion Setup

        private TestResult TestDataGridInitialIsReadOnly<DT>() where DT : IDataGridDataType, new()
        {
            Status("TestDataGridInitialIsReadOnly");

            bool boolValue = false;

            LogComment("verify DataGrid.IsReadOnly by default is set to false");
            if (MyDataGrid.IsReadOnly != boolValue)
            {
                throw new TestValidationException(string.Format(
                        "DataGrid.IsReadOnly is incorrect.  Expected: {0}, Actual: {1}",
                        boolValue,
                        MyDataGrid.IsReadOnly));
            }
            
            LogComment("verify DataGridColumn.IsReadOnly and DataGridCell.IsReadOnly have been set accordingly");
            int i = 0;
            foreach (DataGridColumn column in MyDataGrid.Columns)
            {
                if (column.IsReadOnly != boolValue)
                {
                    throw new TestValidationException(string.Format(
                        "column: {0}, IsReadOnly is incorrect.  Expected: {1}, Actual: {2}",
                        i,
                        boolValue,
                        column.IsReadOnly));
                }

                for (int j = 0; j < MyDataGrid.Items.Count; j++)
                {
                    MyDataGrid.ScrollIntoView(MyDataGrid.Items[j], column);
                    QueueHelper.WaitTillQueueItemsProcessed();

                    DataGridCell cell = DataGridHelper.GetCell(MyDataGrid, j, i);

                    if (cell.IsReadOnly != boolValue)
                    {
                        throw new TestValidationException(string.Format(
                            "cell: ({0},{1}) IsReadOnly is incorrect.  Expected: {2}, Actual: {3}",
                            j,
                            i,
                            boolValue,
                            cell.IsReadOnly));
                    }
                }
                i++;
            }

            LogComment("TestDataGridInitialIsReadOnly was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Set DataGrid.IsReadOnly to true, verify cells are not editable.
        /// 
        /// Set DataGrid.IsReadOnly to false, verify cells are editable.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestDataGridIsReadOnly<DT>() where DT : IDataGridDataType, new()
        {
            Status("TestDataGridIsReadOnly");

            try
            {
                foreach (bool boolValue in new[] { true, false })
                {
                    LogComment(string.Format("Begin testing with DataGrid.IsReadOnly: {0}", boolValue.ToString()));
                    
                    // set IsReadOnly
                    MyDataGrid.IsReadOnly = boolValue;

                    LogComment("go through cells in a row");
                    int testRow = 0;
                    int testCol = 0;
                    foreach (DataGridHelper.ColumnTypes columnType in columnTypes)
                    {
                        LogComment(string.Format("Begin testing with ColumnType: {0}", columnType.ToString()));

                        testCol = DataGridHelper.FindFirstColumnTypeIndex(MyDataGrid, columnType);

                        EditingStepInfo<DT>[] steps = new EditingStepInfo<DT>[] 
                        {
                            new EditingStepInfo<DT> {
                                debugComments = string.Format("try to open the cell: {0}, {1} for edit", testRow, testCol),
                                row = testRow, 
                                col = testCol, 
                                DoAction = this.DoBeginEditing<DT>, 
                                VerifyAfterAction = boolValue ? new VerifyAfterAction<DT>(this.VerifyAfterBeginFails<DT>) : new VerifyAfterAction<DT>(this.VerifyAfterBegin<DT>), 
                                beginAction = DataGridCommandHelper.BeginEditAction.F2 },
                            new EditingStepInfo<DT> {
                                debugComments = string.Format("cancel the cell: {0}, {1} for edit if necessary", testRow, testCol),
                                row = testRow, 
                                col = testCol, 
                                DoAction = this.DoCancelEditing<DT>,
                                VerifyAfterAction = boolValue ? new VerifyAfterAction<DT>(this.VerifyAfterCancelRow<DT>) : new VerifyAfterAction<DT>(this.VerifyAfterCancelCell<DT>), 
                                cancelAction = DataGridCommandHelper.CancelEditAction.Esc }
                        };

                        this.DoEditingSteps<DT>(steps);

                        // cancel any other edits
                        MyDataGrid.CancelEdit(DataGridEditingUnit.Row);

                        LogComment(string.Format("End testing with ColumnType: {0}\n\n", columnType.ToString()));
                    }                    

                    LogComment("go through several rows");
                    testCol = 0;
                    foreach (int row in new[] { 1, 3, MyDataGrid.Items.Count / 2, MyDataGrid.Items.Count - 2 })
                    {
                        LogComment(string.Format("Begin testing with row: {0}", row));

                        testRow = row;

                        EditingStepInfo<DT>[] steps = new EditingStepInfo<DT>[] 
                        {
                            new EditingStepInfo<DT> {
                                debugComments = string.Format("try to open the cell: {0}, {1} for edit", testRow, testCol),
                                row = testRow, 
                                col = testCol, 
                                DoAction = this.DoBeginEditing<DT>, 
                                VerifyAfterAction = boolValue ? new VerifyAfterAction<DT>(this.VerifyAfterBeginFails<DT>) : new VerifyAfterAction<DT>(this.VerifyAfterBegin<DT>), 
                                beginAction = DataGridCommandHelper.BeginEditAction.F2 },
                            new EditingStepInfo<DT> {
                                debugComments = string.Format("cancel the cell: {0}, {1} for edit if necessary", testRow, testCol),
                                row = testRow, 
                                col = testCol, 
                                DoAction = this.DoCancelEditing<DT>, 
                                VerifyAfterAction = boolValue ? new VerifyAfterAction<DT>(this.VerifyAfterCancelRow<DT>) : new VerifyAfterAction<DT>(this.VerifyAfterCancelCell<DT>),                                 
                                cancelAction = DataGridCommandHelper.CancelEditAction.Esc }
                        };

                        this.DoEditingSteps<DT>(steps);

                        // cancel any other edits
                        MyDataGrid.CancelEdit(DataGridEditingUnit.Row);

                        LogComment(string.Format("End testing with row: {0}", row));
                    }

                    LogComment("verify DataGridColumn.IsReadOnly and DataGridCell.IsReadOnly have been updated accordingly");
                    int i = 0;
                    foreach (DataGridColumn column in MyDataGrid.Columns)
                    {
                        if (column.IsReadOnly != boolValue)
                        {
                            throw new TestValidationException(string.Format(
                                "column: {0}, IsReadOnly is incorrect.  Expected: {1}, Actual: {2}",
                                i,
                                boolValue,
                                column.IsReadOnly));
                        }

                        for (int j = 0; j < MyDataGrid.Items.Count; j++)
                        {
                            MyDataGrid.ScrollIntoView(MyDataGrid.Items[j], column);
                            QueueHelper.WaitTillQueueItemsProcessed();

                            DataGridCell cell = DataGridHelper.GetCell(MyDataGrid, j, i);

                            if (cell.IsReadOnly != boolValue)
                            {
                                throw new TestValidationException(string.Format(
                                    "cell: ({0},{1}) IsReadOnly is incorrect.  Expected: {2}, Actual: {3}",
                                    j,
                                    i,
                                    boolValue,
                                    cell.IsReadOnly));
                            }
                        }
                        i++;
                    }

                    LogComment("go back to the top");
                    DataGridActionHelper.NavigateTo(MyDataGrid, 0, 0);

                    LogComment(string.Format("End testing with DataGrid.IsReadOnly: {0}", boolValue.ToString()));
                }
            }
            finally
            {
                CleanupReadOnlyValues();
            }

            LogComment("TestDataGridIsReadOnly was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Set a DataGridColumn.IsReadOnly to true, verify cells in column are not editable. 
        /// Verify cells of other columns are editable.
        /// 
        /// Set DataGridColumn.IsReadOnly to false, verify cells are editable.      
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestDataGridColumnIsReadOnly<DT>() where DT : IDataGridDataType, new()
        {
            Status("TestDataGridColumnIsReadOnly");

            try
            {
                int testCol = 0;
                foreach (DataGridHelper.ColumnTypes columnType in columnTypes)
                {
                    LogComment(string.Format("Begin testing with ColumnType: {0}", columnType.ToString()));

                    testCol = DataGridHelper.FindFirstColumnTypeIndex(MyDataGrid, columnType);
                    DataGridColumn column = DataGridHelper.GetColumn(MyDataGrid, testCol);

                    foreach (bool boolValue in new[] { true, false })
                    {
                        LogComment(string.Format("Begin testing with IsReadOnly: {0}", boolValue.ToString()));

                        // set the column.IsReadOnly
                        column.IsReadOnly = boolValue;
                        QueueHelper.WaitTillQueueItemsProcessed();                        

                        LogComment("go through several cells in the readonly column and cells in another column");
                        foreach (int rowIndex in new[] { 0, 1, MyDataGrid.Items.Count / 2, MyDataGrid.Items.Count - 3 })
                        {
                            LogComment(string.Format("Begin testing with rowIndex: {0}", rowIndex));

                            int testCol2 = (testCol + 1) % MyDataGrid.Columns.Count;
                            EditingStepInfo<DT>[] steps = new EditingStepInfo<DT>[] 
                                {
                                    new EditingStepInfo<DT> {
                                        debugComments = string.Format("try to open the cell: {0}, {1} for edit", rowIndex, testCol),
                                        row = rowIndex, 
                                        col = testCol, 
                                        DoAction = this.DoBeginEditing<DT>, 
                                        VerifyAfterAction = boolValue ? new VerifyAfterAction<DT>(this.VerifyAfterBeginFails<DT>) : new VerifyAfterAction<DT>(this.VerifyAfterBegin<DT>),                                  
                                        beginAction = DataGridCommandHelper.BeginEditAction.F2 },
                                    new EditingStepInfo<DT> {
                                        debugComments = string.Format("cancel the cell: {0}, {1} for edit if necessary", rowIndex, testCol),
                                        row = rowIndex, 
                                        col = testCol, 
                                        DoAction = this.DoCancelEditing<DT>, 
                                        VerifyAfterAction = boolValue ? new VerifyAfterAction<DT>(this.VerifyAfterCancelRow<DT>) : new VerifyAfterAction<DT>(this.VerifyAfterCancelCell<DT>),                                         
                                        cancelAction = DataGridCommandHelper.CancelEditAction.Esc },
                                    new EditingStepInfo<DT> {
                                        debugComments = string.Format("try to open the cell: {0}, {1} for edit", rowIndex, testCol2),
                                        row = rowIndex, 
                                        col = testCol2, 
                                        DoAction = this.DoBeginEditing<DT>, 
                                        VerifyAfterAction = this.VerifyAfterBegin<DT>, 
                                        beginAction = DataGridCommandHelper.BeginEditAction.F2 },
                                    new EditingStepInfo<DT> {
                                        debugComments = string.Format("cancel the cell: {0}, {1} for edit if necessary", rowIndex, testCol2),
                                        row = rowIndex, 
                                        col = testCol2, 
                                        DoAction = this.DoCancelEditing<DT>, 
                                        VerifyAfterAction = this.VerifyAfterCancelCell<DT>, 
                                        cancelAction = DataGridCommandHelper.CancelEditAction.Esc },
                                };

                            this.DoEditingSteps<DT>(steps);

                            // cancel any other edits
                            MyDataGrid.CancelEdit(DataGridEditingUnit.Row);

                            LogComment(string.Format("End testing with rowIndex: {0}", rowIndex));
                        }                        
                        
                        LogComment("verify DataGridCells in that column are ReadOnly");
                        for (int i = 0; i < MyDataGrid.Items.Count; i++)
                        {
                            MyDataGrid.ScrollIntoView(MyDataGrid.Items[i], column);
                            QueueHelper.WaitTillQueueItemsProcessed();

                            DataGridCell cell = DataGridHelper.GetCell(MyDataGrid, i, testCol);
                            if (cell.IsReadOnly != boolValue)
                            {
                                throw new TestValidationException(string.Format(
                                    "cell: ({0},{1}) IsReadOnly is incorrect.  Expected: {2}, Actual: {3}",
                                    i,
                                    testCol,
                                    boolValue,
                                    cell.IsReadOnly));
                            }
                        }

                        LogComment("go back to the top");
                        DataGridActionHelper.NavigateTo(MyDataGrid, 0, 0);

                        LogComment(string.Format("End testing with IsReadOnly: {0}", boolValue.ToString()));
                    }

                    LogComment(string.Format("End testing with ColumnType: {0}\n\n", columnType.ToString()));
                }
            }
            finally
            {
                CleanupReadOnlyValues();
            }

            LogComment("TestDataGridColumnIsReadOnly was successful");
            return TestResult.Pass;
        }
                
        /// <summary>
        /// Verify precedence of DataGrid.IsReadOnly and DataGridColumn.IsReadOnly      
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestIsReadOnlyPrecedence<DT>() where DT : IDataGridDataType, new()
        {
            Status("TestIsReadOnlyPrecedence");

            // the column to test
            int testCol = 2;
            DataGridColumn column = MyDataGrid.Columns[testCol];

            foreach (bool boolValue in new[] { true, false })
            {
                LogComment(string.Format("Begin testing with DataGrid.IsReadOnly: {0}", boolValue.ToString()));

                // set IsReadOnly on the DataGrid
                MyDataGrid.IsReadOnly = boolValue;
                
                foreach (bool boolValue2 in new[] { true, false })
                {
                    LogComment(string.Format("Begin testing with DataGridColumn.IsReadOnly: {0}", boolValue2.ToString()));

                    // set IsReadOnly on the DataGridColumn
                    column.IsReadOnly = boolValue2;

                    LogComment("go through several cells in the readonly column and cells in another column");
                    foreach (int rowIndex in new[] { 0, 1, MyDataGrid.Items.Count / 2, MyDataGrid.Items.Count - 3 })
                    {
                        LogComment(string.Format("Begin testing with rowIndex: {0}", rowIndex));

                        int testCol2 = (testCol + 1) % MyDataGrid.Columns.Count;
                        EditingStepInfo<DT>[] steps = new EditingStepInfo<DT>[] 
                            {
                                new EditingStepInfo<DT> {
                                    debugComments = string.Format("try to open the cell: {0}, {1} for edit", rowIndex, testCol),
                                    row = rowIndex, 
                                    col = testCol, 
                                    DoAction = this.DoBeginEditing<DT>, 
                                    VerifyAfterAction = (boolValue || boolValue2) ? new VerifyAfterAction<DT>(this.VerifyAfterBeginFails<DT>) : new VerifyAfterAction<DT>(this.VerifyAfterBegin<DT>),                                  
                                    beginAction = DataGridCommandHelper.BeginEditAction.F2 },
                                new EditingStepInfo<DT> {
                                    debugComments = string.Format("cancel the cell: {0}, {1} for edit if necessary", rowIndex, testCol),
                                    row = rowIndex, 
                                    col = testCol, 
                                    DoAction = this.DoCancelEditing<DT>, 
                                    VerifyAfterAction = (boolValue || boolValue2) ? new VerifyAfterAction<DT>(this.VerifyAfterCancelRow<DT>) : new VerifyAfterAction<DT>(this.VerifyAfterCancelCell<DT>),                                         
                                    cancelAction = DataGridCommandHelper.CancelEditAction.Esc },
                                new EditingStepInfo<DT> {
                                    debugComments = string.Format("try to open the cell: {0}, {1} for edit", rowIndex, testCol2),
                                    row = rowIndex, 
                                    col = testCol2, 
                                    DoAction = this.DoBeginEditing<DT>, 
                                    VerifyAfterAction = (boolValue) ? new VerifyAfterAction<DT>(this.VerifyAfterBeginFails<DT>) : new VerifyAfterAction<DT>(this.VerifyAfterBegin<DT>),                                                                          
                                    beginAction = DataGridCommandHelper.BeginEditAction.F2 },
                                new EditingStepInfo<DT> {
                                    debugComments = string.Format("cancel the cell: {0}, {1} for edit if necessary", rowIndex, testCol2),
                                    row = rowIndex, 
                                    col = testCol2, 
                                    DoAction = this.DoCancelEditing<DT>, 
                                    VerifyAfterAction = (boolValue) ? new VerifyAfterAction<DT>(this.VerifyAfterCancelRow<DT>) : new VerifyAfterAction<DT>(this.VerifyAfterCancelCell<DT>),                                                                                 
                                    cancelAction = DataGridCommandHelper.CancelEditAction.Esc },
                            };

                        this.DoEditingSteps<DT>(steps);

                        // cancel any other edits
                        MyDataGrid.CancelEdit(DataGridEditingUnit.Row);

                        LogComment(string.Format("End testing with rowIndex: {0}", rowIndex));
                    }                   

                    LogComment("go back to the top");
                    DataGridActionHelper.NavigateTo(MyDataGrid, 0, 0);

                    LogComment(string.Format("End testing with DataGridColumn.IsReadOnly: {0}", boolValue2.ToString()));
                }

                LogComment(string.Format("End testing with DataGrid.IsReadOnly: {0}", boolValue.ToString()));
            }

            LogComment("TestIsReadOnlyPrecedence was successful");
            return TestResult.Pass;
        }        

        #endregion Test Steps

        #region Helpers

        protected virtual void VerifyAfterBeginFails<DT>(EditingStepInfo<DT> editingStepInfo, ref EditingData<DT> expectedData)
        {
            DataGridVerificationHelper.VerifyCurrentCellEditMode(
                MyDataGrid,
                editingStepInfo.row,
                editingStepInfo.col,
                false    /* expected IsEditing value */,
                false    /* verify the CurrentCell info */,
                -1       /* the new current row */,
                -1       /* the new current col */);
            DataGridVerificationHelper.VerifyCurrentRowEditMode(
                MyDataGrid,
                editingStepInfo.row,
                false);
        }

        private void CleanupReadOnlyValues()
        {
            MyDataGrid.IsReadOnly = false;
            foreach (DataGridColumn column in MyDataGrid.Columns)
                column.IsReadOnly = false;
        }

        #endregion Helpers
    }
}
