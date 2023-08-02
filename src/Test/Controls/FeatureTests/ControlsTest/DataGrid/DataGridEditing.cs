using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Avalon.Test.ComponentModel;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Logging;
using System.Diagnostics;

using System.ComponentModel;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Base test class for DataGridEditing Tests.
    /// </description>

    /// </summary>
    public abstract class DataGridEditing : DataGridTest
    {
        #region Private and Protected Fields

        //

        protected DataGridCommandHelper.BeginEditAction[] defaultBeginAction = { DataGridCommandHelper.BeginEditAction.F2 };
        protected DataGridCommandHelper.CommitEditAction[] defaultCommitAction = { DataGridCommandHelper.CommitEditAction.Enter };
        protected DataGridCommandHelper.CancelEditAction[] defaultCancelAction = { DataGridCommandHelper.CancelEditAction.Esc };

        // all begin actions used for the text matrix
        protected DataGridCommandHelper.BeginEditAction[] beginActions = { DataGridCommandHelper.BeginEditAction.F2,
                                                                DataGridCommandHelper.BeginEditAction.DoubleClick,
                                                                DataGridCommandHelper.BeginEditAction.FocusClick,
                                                                DataGridCommandHelper.BeginEditAction.MethodCall,
                                                                DataGridCommandHelper.BeginEditAction.Keystroke,
                                                                DataGridCommandHelper.BeginEditAction.AltToggle,
                                                                DataGridCommandHelper.BeginEditAction.F4,
                                                                DataGridCommandHelper.BeginEditAction.SpaceBarToggle};

        // all cancel actions used for the test matrix
        protected DataGridCommandHelper.CancelEditAction[] cancelActions = { DataGridCommandHelper.CancelEditAction.Esc,
                                                                    DataGridCommandHelper.CancelEditAction.MethodCall,
                                                                    DataGridCommandHelper.CancelEditAction.MethodCallRowUnit,
                                                                    DataGridCommandHelper.CancelEditAction.MethodCallCellUnit};

        // all commit actions used for the test matrix
        protected DataGridCommandHelper.CommitEditAction[] commitActions = { DataGridCommandHelper.CommitEditAction.Enter,
                                                                    DataGridCommandHelper.CommitEditAction.MouseClickDifferentCell,
                                                                    DataGridCommandHelper.CommitEditAction.MouseClickDifferentRow,
                                                                    DataGridCommandHelper.CommitEditAction.Tab,
                                                                    DataGridCommandHelper.CommitEditAction.MouseClickRowHeader,
                                                                    DataGridCommandHelper.CommitEditAction.MethodCall,
                                                                    DataGridCommandHelper.CommitEditAction.MethodCallCellUnit,
                                                                    DataGridCommandHelper.CommitEditAction.MethodCallRowUnit,
                                                                    DataGridCommandHelper.CommitEditAction.ShiftTab
                                                                    //

                                                                    /*DataGridCommandHelper.CommitEditAction.FocusLost,*/};

        // all the column types used for the test matrix
        protected DataGridHelper.ColumnTypes[] columnTypes = { DataGridHelper.ColumnTypes.DataGridTextColumn,
                                                      DataGridHelper.ColumnTypes.DataGridCheckBoxColumn,
                                                      DataGridHelper.ColumnTypes.DataGridComboBoxColumn,
                                                      DataGridHelper.ColumnTypes.DataGridHyperlinkColumn,
                                                      DataGridHelper.ColumnTypes.DataGridTemplateColumn};
        #endregion Private and Protected Fields

        #region Constructor

        public DataGridEditing(string filename)
            : base(filename)
        {
        }

        #endregion

        #region Editing Command Matrix Helpers

        #region TestCommandMatrix

        /// <summary>
        /// Simulates begin, commit, and cancel actions.  Also applies verifications at each state.
        /// </summary>
        /// <remarks>
        /// Able to simulate different types of command actions depending on the begin, commit, and cancel
        /// actions that are given to the function.  A beginAction must always to specified to start the test.
        /// commitActions and cancelActions can be left as null.
        /// </remarks>
        /// <param name="row">the row of the cell to do the command</param>
        /// <param name="col">the column of the cell to do the command</param>
        /// <param name="doEditCell">if true will edit the cell, otherwise will not</param>
        /// <param name="beginActions">the list of begin commands to execute</param>
        /// <param name="commitActions">the list of commit commands to execute</param>
        /// <param name="cancelActions">the lsit of cancel commands to execute</param>
        public void TestCommandMatrix(
            int row, int col, bool doEditCell,
            DataGridCommandHelper.BeginEditAction[] beginActions,
            DataGridCommandHelper.CommitEditAction[] commitActions,
            DataGridCommandHelper.CancelEditAction[] cancelActions)
        {
            DataGridHelper.ColumnTypes columnType = DataGridHelper.FindColumnTypeFromIndex(MyDataGrid, col);

            // initState used by SetInitialState()
            CommandInitState initState;
            if (doEditCell)
            {
                initState = CommandInitState.AfterBeginAndEdit;
            }
            else
            {
                initState = CommandInitState.AfterBegin;
            }

            // start the test algorithm
            foreach (DataGridCommandHelper.BeginEditAction beginAction in beginActions)
            {
                TestLog.Current.LogStatus(string.Format("Begin testing with begin action: {0}", beginAction.ToString()));

                // skip if the column does not permit the beginAction
                if (!DataGridCommandHelper.ColumnHasBeginEditAction(columnType, beginAction))
                {
                    continue;
                }

                // data to be verified later
                string previousData = DataGridHelper.GetDataFromCell(MyDataGrid, row, col, false, GetDataFromTemplateColumn);
                string expectedData = null;

                // do begin action and verify state
                this.SetInitialState(initState, row, col, beginAction, out expectedData);
                this.VerifyAfterBegin(row, col);

                if (commitActions != null)
                {
                    foreach (DataGridCommandHelper.CommitEditAction commitAction in commitActions)
                    {
                        TestLog.Current.LogStatus(string.Format("Begin testing with commitAction: {0}", commitAction.ToString()));

                        // do commit and verify state
                        this.DoCommitEditing(row, col, commitAction);
                        QueueHelper.WaitTillQueueItemsProcessed();

                        if (this.IsCellOnlyCommit(row, col, commitAction))
                        {
                            this.VerifyAfterCommitCell(row, col, expectedData, commitAction);

                            // commit action was cell only, do another commit for the row
                            if (commitAction == DataGridCommandHelper.CommitEditAction.MethodCall)
                            {
                                // repeat the same action
                                this.DoCommitEditing(row, col, commitAction);
                            }
                            else if (commitAction == DataGridCommandHelper.CommitEditAction.Tab)
                            {
                                // account for new cell that is in edit mode
                                this.DoCommitEditing(row, IncrementColumnBy(col, 1), DataGridCommandHelper.CommitEditAction.Enter);
                            }
                            else if (commitAction == DataGridCommandHelper.CommitEditAction.ShiftTab)
                            {
                                // account for new cell that is in edit mode
                                this.DoCommitEditing(row, DecrementColumnBy(col, 1), DataGridCommandHelper.CommitEditAction.Enter);
                            }
                            else
                            {
                                // cannot repeat the same action
                                this.DoCommitEditing(row, col, DataGridCommandHelper.CommitEditAction.Enter);
                            }
                        }

                        this.VerifyAfterCommitRow(row);

                        // get back to initial state of commit
                        this.SetInitialState(initState, row, col, beginAction, out expectedData);
                        this.VerifyAfterBegin(row, col);

                        TestLog.Current.LogStatus(string.Format("End testing with commitAction: {0}", commitAction.ToString()));
                    }
                }

                if (cancelActions != null)
                {
                    foreach (DataGridCommandHelper.CancelEditAction cancelAction in cancelActions)
                    {
                        TestLog.Current.LogStatus(string.Format("Begin testing with cancel action: {0}", cancelAction.ToString()));

                        // do cancel and verify state
                        this.DoCancelEditing(row, col, cancelAction);
                        if (this.IsCellOnlyCancel(cancelAction))
                        {
                            this.VerifyAfterCancelCell(row, col, previousData);

                            // cancel action was cell only, do another cancel for the row
                            this.DoCancelEditing(row, col, DataGridCommandHelper.CancelEditAction.Esc);
                        }
                        this.VerifyAfterCancelRow(row, col, previousData);

                        // get back to initial state of cancel
                        this.SetInitialState(initState, row, col, beginAction, out expectedData);
                        this.VerifyAfterBegin(row, col);

                        TestLog.Current.LogStatus(string.Format("End testing with cancel action: {0}", cancelAction.ToString()));
                    }
                }

                // reset for BeginEdit
                if (commitActions == null && cancelActions == null)
                {
                    // get back to initial state before begin
                    this.SetInitialState(CommandInitState.BeforeBegin, row, col, beginAction, out expectedData);
                    this.VerifyAfterCancelRow(row, col, previousData);
                }

                TestLog.Current.LogStatus(string.Format("End testing with begin action: {0}", beginAction.ToString()));
            }

            // clean up an existing cells open for edit
            DataGridCommandHelper.CancelEdit(MyDataGrid);
            DataGridCommandHelper.CancelEdit(MyDataGrid);
        }

        #endregion TestCommandMatrix

        #region Do Editing Commands

        protected virtual void DoBeginEditing(int row, int col, DataGridCommandHelper.BeginEditAction action)
        {
            DataGridCommandHelper.BeginEdit(MyDataGrid, row, col, action);
            QueueHelper.WaitTillQueueItemsProcessed();
        }

        protected virtual void DoCommitEditing(int row, int col, DataGridCommandHelper.CommitEditAction action)
        {
            int mouseClickRowIdx = (row + 1) % MyDataGrid.Items.Count;
            int mouseClickColIdx = IncrementColumnBy(col, 1) % MyDataGrid.Columns.Count;

            this.DoCommitEditing(row, col, action, mouseClickRowIdx, mouseClickColIdx);
        }

        protected virtual void DoCommitEditing(int row, int col, DataGridCommandHelper.CommitEditAction action, int mouseClickRowIdx, int mouseClickColIdx)
        {
            // does not click on a template column, skip template columns
            DataGridColumn column = DataGridHelper.GetColumn(MyDataGrid, mouseClickColIdx);
            if (column is DataGridTemplateColumn)
            {
                mouseClickColIdx = (IncrementColumnBy(mouseClickColIdx, 1)) % MyDataGrid.Columns.Count;
            }

            DataGridCommandHelper.CommitEdit(MyDataGrid, row, col, action, mouseClickRowIdx, mouseClickColIdx);
        }

        protected virtual void DoCancelEditing(int row, int col, DataGridCommandHelper.CancelEditAction action)
        {
            DataGridCommandHelper.CancelEdit(MyDataGrid, action);
            this.WaitForPriority(DispatcherPriority.SystemIdle);
        }

        #endregion Do Editing Commands

        #region State Verifications

        /// <summary>
        /// The cell should be in edit mode.
        /// The row should be in edit mode.
        /// </summary>
        protected virtual void VerifyAfterBegin(int row, int col)
        {
            DataGridVerificationHelper.VerifyCurrentCellEditMode(
                MyDataGrid,
                row,
                col,
                true    /* expected IsEditing value */,
                true    /* verify the CurrentCell info */,
                row     /* the new current row */,
                col     /* the new current col */);
            DataGridVerificationHelper.VerifyCurrentRowEditMode(
                MyDataGrid,
                row,
                true);
        }

        /// <summary>
        /// The cell should not be in edit mode.
        /// The cell data should not have persisted to the data source.
        /// The row should still be in edit mode.
        /// </summary>
        protected virtual void VerifyAfterCommitCell(int row, int col, string expectedData, DataGridCommandHelper.CommitEditAction commitAction)
        {
            DataGridVerificationHelper.VerifyCurrentCellEditMode(
                MyDataGrid,
                row,
                col,
                false       /* expected IsEditing value */,
                false       /* do not verify the CurrentCell info */,
                -1          /* the new current row */,
                -1          /* the new current col */);
            DataGridVerificationHelper.VerifyCellData(
                MyDataGrid,
                DataSource          /* source to verify data from */,
                TypeFromDataSource  /* the type from the data source*/,
                row                 /* row of cell to verify */,
                col                 /* column of cell to verify */,
                expectedData        /* expected cell data */,
                false               /* expected IsEditing value */,
                false               /* cell data should not have persisted to the data source */,
                GetDataFromTemplateColumn,
                GetDisplayBindingFromTemplateColumn);

            DataGridVerificationHelper.VerifyCurrentRowEditMode(
                MyDataGrid,
                row,
                IsCellOnlyCommit(row, col, commitAction));
        }

        /// <summary>
        /// The row should not be in edit mode.
        ///
        /// Note: row data is not verified here.
        /// </summary>
        protected virtual void VerifyAfterCommitRow(int row)
        {
            DataGridVerificationHelper.VerifyCurrentRowEditMode(
                MyDataGrid,
                row,
                false);
        }

        /// <summary>
        /// The cell should not be in edit mode.
        /// The cell data should have been reverted.
        /// The row should still be in edit mode.
        /// </summary>
        protected virtual void VerifyAfterCancelCell(int row, int col, string expectedData)
        {
            DataGridVerificationHelper.VerifyCurrentCellEditMode(
                MyDataGrid,
                row,
                col,
                false   /* expected IsEditing value */,
                false   /* do not verify the CurrentCell info */,
                -1      /* the new current row */,
                -1      /* the new current col */);
            DataGridVerificationHelper.VerifyCellData(
                MyDataGrid,
                DataSource          /* source to verify data from */,
                TypeFromDataSource  /* the type from the data source*/,
                row                 /* row of cell to verify */,
                col                 /* column of cell to verify */,
                expectedData        /* expected cell data */,
                false               /* expected IsEditing value */,
                true                /* cell data should should agree with data source */,
                GetDataFromTemplateColumn,
                GetDisplayBindingFromTemplateColumn);

            DataGridVerificationHelper.VerifyCurrentRowEditMode(
                MyDataGrid,
                row,
                true);
        }

        /// <summary>
        /// The cell should not be in edit mode.
        /// The cell data should have been reverted.
        /// The row should not be in edit mode.
        ///
        /// Note: row data is not verified here
        /// </summary>
        protected virtual void VerifyAfterCancelRow(int row, int col, string expectedData)
        {
            DataGridVerificationHelper.VerifyCurrentCellEditMode(
                MyDataGrid,
                row,
                col,
                false   /* expected IsEditing value */,
                false   /* do not verify the CurrentCell info */,
                -1      /* the new current row */,
                -1      /* the new current col */);
            DataGridVerificationHelper.VerifyCellData(
                MyDataGrid,
                DataSource          /* source to verify data from */,
                TypeFromDataSource  /* the type from the data source*/,
                row                 /* row of cell to verify */,
                col                 /* column of cell to verify */,
                expectedData        /* expected cell data */,
                false               /* expected IsEditing value */,
                true                /* cell data should should agree with data source */,
                GetDataFromTemplateColumn,
                GetDisplayBindingFromTemplateColumn);
            DataGridVerificationHelper.VerifyCurrentRowEditMode(
                MyDataGrid,
                row,
                false);
        }

        #endregion State Verifications

        #region Do Editing Commands and Record Event

        protected void DoBeginAndRecordEvent(
            int row, int col,
            DataGridCommandHelper.BeginEditAction action,
            out DataGridBeginningEditEventArgs expectedArgs,
            out DataGridBeginningEditEventArgs actualEventArgs)
        {
            // temp args needed as actual args cannot be passed 'out'
            // of function
            DataGridBeginningEditEventArgs virtualExpectedArgs = null;
            DataGridBeginningEditEventArgs virtualActualEventArgs = null;

            EventHelper.ExpectEvent<DataGridBeginningEditEventArgs>(
                () =>
                {
                    virtualExpectedArgs = new DataGridBeginningEditEventArgs(
                        DataGridHelper.GetColumn(MyDataGrid, col),
                        DataGridHelper.GetRow(MyDataGrid, row),
                        null);
                    virtualExpectedArgs.Cancel = false;

                    DataGridCommandHelper.BeginEdit(MyDataGrid, row, col);
                    QueueHelper.WaitTillQueueItemsProcessed();
                },
                MyDataGrid,
                "BeginningEdit",
                (sender, args) =>
                {
                    virtualActualEventArgs = args;
                });

            expectedArgs = virtualExpectedArgs;
            actualEventArgs = virtualActualEventArgs;
        }

        protected void DoBeginAndRecordPrepareEvent(
            int row, int col,
            DataGridCommandHelper.BeginEditAction action,
            out DataGridPreparingCellForEditEventArgs expectedArgs,
            out DataGridPreparingCellForEditEventArgs actualEventArgs)
        {
            // temp args needed as actual args cannot be passed 'out'
            // of function
            DataGridPreparingCellForEditEventArgs virtualExpectedArgs = null;
            DataGridPreparingCellForEditEventArgs virtualActualEventArgs = null;

            EventHelper.ExpectEvent<DataGridPreparingCellForEditEventArgs>(
                () =>
                {
                    DataGridCommandHelper.BeginEdit(MyDataGrid, row, col);
                    QueueHelper.WaitTillQueueItemsProcessed();

                    virtualExpectedArgs = new DataGridPreparingCellForEditEventArgs(
                        DataGridHelper.GetColumn(MyDataGrid, col),
                        DataGridHelper.GetRow(MyDataGrid, row),
                        null,
                        DataGridHelper.GetCellEditingElement(MyDataGrid, row, col));
                },
                MyDataGrid,
                "PreparingCellForEdit",
                (sender, args) =>
                {
                    virtualActualEventArgs = args;
                });

            expectedArgs = virtualExpectedArgs;
            actualEventArgs = virtualActualEventArgs;
        }

        protected void DoCancelAndRecordEvent(
            int row, int col,
            DataGridCommandHelper.CancelEditAction action,
            out DataGridCellEditEndingEventArgs expectedCellEventArgs,
            out DataGridCellEditEndingEventArgs actualCellEventArgs,
            out DataGridRowEditEndingEventArgs expectedRowEventArgs,
            out DataGridRowEditEndingEventArgs actualRowEventArgs)
        {
            // temp args needed as actual args cannot be passed 'out' of function
            DataGridCellEditEndingEventArgs virtualExpectedCellEventArgs = null;
            DataGridCellEditEndingEventArgs virtualActualCellEventArgs = null;
            DataGridRowEditEndingEventArgs virtualExpectedRowEventArgs = null;
            DataGridRowEditEndingEventArgs virtualActualRowEventArgs = null;

            EventHandler<DataGridCellEditEndingEventArgs> cancelCellEvent = delegate(object sender, DataGridCellEditEndingEventArgs args)
            {
                virtualActualCellEventArgs = args;
            };
            EventHandler<DataGridRowEditEndingEventArgs> cancelRowEvent = delegate(object sender, DataGridRowEditEndingEventArgs args)
            {
                virtualActualRowEventArgs = args;
            };

            try
            {
                MyDataGrid.CellEditEnding += cancelCellEvent;
                MyDataGrid.RowEditEnding += cancelRowEvent;

                DataGridCell cell = DataGridHelper.GetCell(MyDataGrid, row, col);
                if (cell.IsEditing)
                {
                    virtualExpectedCellEventArgs = new DataGridCellEditEndingEventArgs(
                                    DataGridHelper.GetColumn(MyDataGrid, col),
                                    DataGridHelper.GetRow(MyDataGrid, row),
                                    DataGridHelper.GetCellEditingElement(MyDataGrid, row, col),
                                    DataGridEditAction.Cancel);
                    virtualExpectedCellEventArgs.Cancel = false;

                    if (IsCellOnlyCancel(action))
                    {
                        virtualExpectedRowEventArgs = null;
                    }
                    else
                    {
                        virtualExpectedRowEventArgs = new DataGridRowEditEndingEventArgs(
                                DataGridHelper.GetRow(MyDataGrid, row),
                                DataGridEditAction.Cancel);
                        virtualExpectedRowEventArgs.Cancel = false;
                    }
                }
                else
                {
                    // set for row cancel only
                    virtualExpectedRowEventArgs = new DataGridRowEditEndingEventArgs(
                                DataGridHelper.GetRow(MyDataGrid, row),
                                DataGridEditAction.Cancel);
                    virtualExpectedRowEventArgs.Cancel = false;
                }

                DataGridCommandHelper.CancelEdit(MyDataGrid, action);
                this.WaitForPriority(DispatcherPriority.SystemIdle);
            }
            finally
            {
                MyDataGrid.CellEditEnding -= cancelCellEvent;
                MyDataGrid.RowEditEnding -= cancelRowEvent;
            }

            expectedCellEventArgs = virtualExpectedCellEventArgs;
            actualCellEventArgs = virtualActualCellEventArgs;
            expectedRowEventArgs = virtualExpectedRowEventArgs;
            actualRowEventArgs = virtualActualRowEventArgs;
        }

        protected void DoCommitAndRecordEvent(
            int row, int col,
            DataGridCommandHelper.CommitEditAction action,
            out DataGridCellEditEndingEventArgs expectedCellEventArgs,
            out DataGridCellEditEndingEventArgs actualCellEventArgs,
            out DataGridRowEditEndingEventArgs expectedRowEventArgs,
            out DataGridRowEditEndingEventArgs actualRowEventArgs)
        {
            // temp args needed as actual args cannot be passed 'out' of function
            DataGridCellEditEndingEventArgs virtualExpectedCellEventArgs = null;
            DataGridCellEditEndingEventArgs virtualActualCellEventArgs = null;
            DataGridRowEditEndingEventArgs virtualExpectedRowEventArgs = null;
            DataGridRowEditEndingEventArgs virtualActualRowEventArgs = null;

            EventHandler<DataGridCellEditEndingEventArgs> commitCellEvent = delegate(object sender, DataGridCellEditEndingEventArgs args)
            {
                virtualActualCellEventArgs = args;
            };
            EventHandler<DataGridRowEditEndingEventArgs> commitRowEvent = delegate( object sender, DataGridRowEditEndingEventArgs args)
            {
                virtualActualRowEventArgs = args;
            };

            try
            {
                MyDataGrid.CellEditEnding += commitCellEvent;
                MyDataGrid.RowEditEnding += commitRowEvent;

                DataGridCell cell = DataGridHelper.GetCell(MyDataGrid, row, col);
                if (cell.IsEditing)
                {
                    virtualExpectedCellEventArgs = new DataGridCellEditEndingEventArgs(
                                    DataGridHelper.GetColumn(MyDataGrid, col),
                                    DataGridHelper.GetRow(MyDataGrid, row),
                                    DataGridHelper.GetCellEditingElement(MyDataGrid, row, col),
                                    DataGridEditAction.Commit);
                    virtualExpectedCellEventArgs.Cancel = false;

                    if (IsCellOnlyCommit(row, col, action))
                    {
                        virtualExpectedRowEventArgs = null;
                    }
                    else
                    {
                        virtualExpectedRowEventArgs = new DataGridRowEditEndingEventArgs(
                                DataGridHelper.GetRow(MyDataGrid, row),
                                DataGridEditAction.Commit);
                        virtualExpectedRowEventArgs.Cancel = false;
                    }
                }
                else
                {
                    // set for row commit only
                    virtualExpectedRowEventArgs = new DataGridRowEditEndingEventArgs(
                                DataGridHelper.GetRow(MyDataGrid, row),
                                DataGridEditAction.Commit);
                    virtualExpectedRowEventArgs.Cancel = false;
                }
                int mouseClickRowIdx = (row + 1) % MyDataGrid.Items.Count;
                int mouseClickColIdx = IncrementColumnBy(col, 1) % MyDataGrid.Columns.Count;

                // does not click on a template column, skip template columns
                DataGridColumn column = DataGridHelper.GetColumn(MyDataGrid, mouseClickColIdx);
                if (column is DataGridTemplateColumn)
                {
                    mouseClickColIdx = IncrementColumnBy(mouseClickColIdx, 1) % MyDataGrid.Columns.Count;
                }

                DataGridCommandHelper.CommitEdit(MyDataGrid, row, col, action, mouseClickRowIdx, mouseClickColIdx);
            }
            finally
            {
                MyDataGrid.CellEditEnding += commitCellEvent;
                MyDataGrid.RowEditEnding += commitRowEvent;
            }

            expectedCellEventArgs = virtualExpectedCellEventArgs;
            actualCellEventArgs = virtualActualCellEventArgs;
            expectedRowEventArgs = virtualExpectedRowEventArgs;
            actualRowEventArgs = virtualActualRowEventArgs;
        }

        #endregion Do Editing Commands and Record Event

        #region State Verifications for Events

        protected void VerifyBeginEventArgs(DataGridBeginningEditEventArgs expectedArgs, DataGridBeginningEditEventArgs actualEventArgs)
        {
            if (expectedArgs.Column != actualEventArgs.Column)
            {
                throw new TestValidationException(string.Format(
                    "BeginningEditEventArgs expected column: {0}, actual column: {1}",
                    expectedArgs.Column.ToString(),
                    actualEventArgs.Column.ToString()));
            }
            if (expectedArgs.Row != actualEventArgs.Row)
            {
                throw new TestValidationException(string.Format(
                    "BeginningEditEventArgs expected row: {0}, actual row: {1}",
                    expectedArgs.Row.ToString(),
                    actualEventArgs.Row.ToString()));
            }
            if (expectedArgs.Cancel != actualEventArgs.Cancel)
            {
                throw new TestValidationException(string.Format(
                    "BeginningEditEventArgs expected Cancel: {0}, actual Cancel: {1}",
                    expectedArgs.Cancel.ToString(),
                    actualEventArgs.Cancel.ToString()));
            }
            if (expectedArgs.EditingEventArgs != actualEventArgs.EditingEventArgs)
            {
                throw new TestValidationException(string.Format(
                    "DataGridBeginningEditEventArgs expected EditingEventArgs: {0}, actual: {1}",
                    expectedArgs.EditingEventArgs,
                    actualEventArgs.EditingEventArgs));
            }
        }

        protected void VerifyEndingEventArgs(
            DataGridCellEditEndingEventArgs expectedCellEventArgs,
            DataGridCellEditEndingEventArgs actualCellEventArgs,
            DataGridRowEditEndingEventArgs expectedRowEventArgs,
            DataGridRowEditEndingEventArgs actualRowEventArgs)
        {
            if (expectedCellEventArgs != null || actualCellEventArgs != null)
            {
                if (expectedCellEventArgs == null)
                {
                    throw new TestValidationException("expected cell args are null when actual args are not");
                }
                else if (actualCellEventArgs == null)
                {
                    throw new TestValidationException("actual cell args are null when expected args are not");
                }

                if (expectedCellEventArgs.Column != actualCellEventArgs.Column)
                {
                    throw new TestValidationException(string.Format(
                        "EndingEditEventArgs expected column: {0}, actual column: {1}",
                        expectedCellEventArgs.Column.ToString(),
                        actualCellEventArgs.Column.ToString()));
                }
                if (expectedCellEventArgs.Row != actualCellEventArgs.Row)
                {
                    throw new TestValidationException(string.Format(
                        "EndingEditEventArgs expected row: {0}, actual row: {1}",
                        expectedCellEventArgs.Row.ToString(),
                        actualCellEventArgs.Row.ToString()));
                }
                if (expectedCellEventArgs.Cancel != actualCellEventArgs.Cancel)
                {
                    throw new TestValidationException(string.Format(
                        "EndingEditEventArgs expected Cancel: {0}, actual Cancel: {1}",
                        expectedCellEventArgs.Cancel.ToString(),
                        actualCellEventArgs.Cancel.ToString()));
                }
                if (expectedCellEventArgs.EditingElement != actualCellEventArgs.EditingElement)
                {
                    throw new TestValidationException(string.Format(
                        "EndingEditEventArgs expected editing element: {0}, actual editing element: {1}",
                        expectedCellEventArgs.EditingElement.ToString(),
                        actualCellEventArgs.EditingElement.ToString()));
                }
                if (expectedCellEventArgs.EditAction != actualCellEventArgs.EditAction)
                {
                    throw new TestValidationException(string.Format(
                        "EndingEditEventArgs expected editing unit: {0}, actual editing unit: {1}",
                        expectedCellEventArgs.EditAction.ToString(),
                        actualCellEventArgs.EditAction.ToString()));
                }
            }

            if (expectedRowEventArgs != null || actualRowEventArgs != null)
            {
                if (expectedRowEventArgs == null)
                {
                    throw new TestValidationException("expected row args are null when actual args are not");
                }
                else if (actualRowEventArgs == null)
                {
                    throw new TestValidationException("actual row args are null when expected args are not");
                }

                if (expectedRowEventArgs.Row != actualRowEventArgs.Row)
                {
                    throw new TestValidationException(string.Format(
                        "EndingEditEventArgs expected row: {0}, actual row: {1}",
                        expectedRowEventArgs.Row.ToString(),
                        actualRowEventArgs.Row.ToString()));
                }
                if (expectedRowEventArgs.Cancel != actualRowEventArgs.Cancel)
                {
                    throw new TestValidationException(string.Format(
                        "EndingEditEventArgs expected Cancel: {0}, actual Cancel: {1}",
                        expectedRowEventArgs.Cancel.ToString(),
                        actualRowEventArgs.Cancel.ToString()));
                }
                if (expectedRowEventArgs.EditAction != actualRowEventArgs.EditAction)
                {
                    throw new TestValidationException(string.Format(
                        "EndingEditEventArgs expected editing unit: {0}, actual editing unit: {1}",
                        expectedRowEventArgs.EditAction.ToString(),
                        actualRowEventArgs.EditAction.ToString()));
                }
            }
        }

        protected void VerifyPreparingEventArgs(DataGridPreparingCellForEditEventArgs expectedArgs, DataGridPreparingCellForEditEventArgs actualEventArgs)
        {
            if (expectedArgs.Column != actualEventArgs.Column)
            {
                throw new TestValidationException(string.Format(
                    "PreparingCellForEditEventArgs expected column: {0}, actual column: {1}",
                    expectedArgs.Column.ToString(),
                    actualEventArgs.Column.ToString()));
            }
            if (expectedArgs.Row != actualEventArgs.Row)
            {
                throw new TestValidationException(string.Format(
                    "PreparingCellForEditEventArgs expected row: {0}, actual row: {1}",
                    expectedArgs.Row.ToString(),
                    actualEventArgs.Row.ToString()));
            }
            if (expectedArgs.EditingElement != actualEventArgs.EditingElement)
            {
                throw new TestValidationException(string.Format(
                    "PreparingCellForEditEventArgs expected editing element: {0}, actual editing element: {1}",
                    expectedArgs.EditingElement.ToString(),
                    actualEventArgs.EditingElement.ToString()));
            }
            if (expectedArgs.EditingEventArgs != actualEventArgs.EditingEventArgs)
            {
                throw new TestValidationException(string.Format(
                    "DataGridBeginningEditEventArgs expected EditingEventArgs: {0}, actual: {1}",
                    expectedArgs.EditingEventArgs,
                    actualEventArgs.EditingEventArgs));
            }
        }

        #endregion State Verifications for Events

        #region Helpers

        public string GetDataFromTemplateColumn(DataGridCell currentCell, bool isEditing)
        {
            string cellData = null;

            //NOTE: this is hardcoded to the xaml file and expects
            //      a Button when !Editing and a TextBox when editing
            ContentPresenter cp = currentCell.Content as ContentPresenter;
            if (isEditing)
            {
                TextBox cellBlock = DataGridHelper.FindVisualChild<TextBox>(cp);
                cellData = cellBlock.Text;
            }
            else
            {
                Button cellBlock = DataGridHelper.FindVisualChild<Button>(cp);
                cellData = (string)cellBlock.Content;
            }

            return cellData;
        }

        public string GetDisplayBindingFromTemplateColumn(DataGridCell currentCell)
        {
            string memberName = null;

            Binding binding;
            ContentPresenter cp = currentCell.Content as ContentPresenter;
            if (currentCell.IsEditing)
            {
                TextBox tb = DataGridHelper.FindVisualChild<TextBox>(cp);
                binding = BindingOperations.GetBinding(tb, TextBox.TextProperty);
            }
            else
            {
                Button button = DataGridHelper.FindVisualChild<Button>(cp);
                binding = BindingOperations.GetBinding(button, Button.ContentProperty);
            }
            memberName = binding.Path.Path;

            return memberName;
        }

        public bool IsCellOnlyCommit(int row, int col, DataGridCommandHelper.CommitEditAction commitAction)
        {
            if (commitAction == DataGridCommandHelper.CommitEditAction.Enter ||
                commitAction == DataGridCommandHelper.CommitEditAction.MethodCallRowUnit ||
                commitAction == DataGridCommandHelper.CommitEditAction.MouseClickColumnHeader ||
                commitAction == DataGridCommandHelper.CommitEditAction.MouseClickDifferentRow ||
                commitAction == DataGridCommandHelper.CommitEditAction.MouseClickRowHeader ||
                commitAction == DataGridCommandHelper.CommitEditAction.FocusLost)
            {
                return false;
            }
            else if (commitAction == DataGridCommandHelper.CommitEditAction.Tab && col == MyDataGrid.Columns.Count - 1)
            {
                return false;
            }
            else if (commitAction == DataGridCommandHelper.CommitEditAction.ShiftTab && col == 0)
            {
                return false;
            }
            return true;
        }

        public bool IsCellOnlyCancel(DataGridCommandHelper.CancelEditAction cancelAction)
        {
            if (cancelAction == DataGridCommandHelper.CancelEditAction.MethodCallRowUnit)
            {
                return false;
            }
            return true;
        }

        #endregion Helpers

        #region Private Members

        private enum CommandInitState
        {
            BeforeBegin,
            AfterBegin,
            AfterBeginAndEdit
        }

        /// <summary>
        /// Helper for TestCommandMatrix to set the inital state.  The initial state can be
        /// either before the begin action, after the begin, or after the begin and edit.
        /// </summary>
        private void SetInitialState(
            CommandInitState state,
            int row,
            int col,
            DataGridCommandHelper.BeginEditAction beginAction,
            out string expectedData)
        {
            QueueHelper.WaitTillQueueItemsProcessed();

            expectedData = null;

            if (state == CommandInitState.BeforeBegin)
            {
                this.DoCancelEditing(row, col, DataGridCommandHelper.CancelEditAction.Esc);
                this.DoCancelEditing(row, col, DataGridCommandHelper.CancelEditAction.Esc);

                if (beginAction == DataGridCommandHelper.BeginEditAction.AltToggle || beginAction == DataGridCommandHelper.BeginEditAction.F4)
                {
                    // add an additional 'Esc' for Alt Toggle scenario
                    DataGridCommandHelper.CancelEdit(MyDataGrid, DataGridCommandHelper.CancelEditAction.Esc);
                    this.WaitForPriority(DispatcherPriority.SystemIdle);
                }
            }
            else if (state == CommandInitState.AfterBegin)
            {
                this.DoBeginEditing(row, col, beginAction);
            }
            else if (state == CommandInitState.AfterBeginAndEdit)
            {
                this.DoBeginEditing(row, col, beginAction);

                // edit the cell and record data
                DataGridActionHelper.EditCellGenericInput(MyDataGrid, row, col, out expectedData);
            }
        }

        #endregion Private Members

        #endregion Editing Command Matrix Helpers

        #region Editing Step Helpers

        #region Data

        public delegate void DoAction<T>(EditingStepInfo<T> editingStepInfo, ref EditingData<T> expectedData);
        public delegate void VerifyAfterAction<T>(EditingStepInfo<T> editingStepInfo, ref EditingData<T> expectedData);

        public class EditingStepInfo<T>
        {
            //
            // main delegates
            //
            public DoAction<T> DoAction = null;
            public VerifyAfterAction<T> VerifyAfterAction = null;

            //
            // information used by the delegates
            //

            // row to perform the action on
            public int row;

            // column to perform the action on
            public int col;

            // particular actions that can be used by the delegate
            public DataGridCommandHelper.BeginEditAction beginAction = DataGridCommandHelper.BeginEditAction.None;
            public DataGridCommandHelper.CancelEditAction cancelAction = DataGridCommandHelper.CancelEditAction.None;
            public DataGridCommandHelper.CommitEditAction commitAction = DataGridCommandHelper.CommitEditAction.None;

            public int mouseClickRowIdx = -1;
            public int mouseClickColIdx = -1;

            public bool columnIsSorted = false;

            // additional comments
            public string debugComments = null;
        }

        public class EditingData<T>
        {
            public T rowData = default(T);
            public T prevRowData = default(T);
        }

        #endregion Data

        #region EditingSteps Procedure

        public void DoEditingSteps<T>(EditingStepInfo<T>[] editingSteps) where T : IDataGridDataType, new()
        {
            // make sure the row is in view
            DataGridActionHelper.NavigateTo(MyDataGrid, editingSteps[0].row, editingSteps[0].col);
            QueueHelper.WaitTillQueueItemsProcessed();

            // setup the expected data
            EditingData<T> expectedData = new EditingData<T>();
            IDataGridDataType temp = MyDataGrid.Items[editingSteps[0].row] as IDataGridDataType;
            if (temp != null)
            {
                expectedData.prevRowData = (T)temp.Clone();
                expectedData.rowData = (T)temp.Clone();
            }
            else if (MyDataGrid.Items[editingSteps[0].row] == CollectionView.NewItemPlaceholder)
            {
                // expecting the NewItemPlaceholder
                expectedData.prevRowData = new T();
                expectedData.rowData = new T();
            }
            else
            {
                throw new TestValidationException(string.Format("Item must be of type IDataGridDataType or NewItemPlaceholder"));
            }

            foreach (EditingStepInfo<T> editingStep in editingSteps)
            {
                LogComment(editingStep.debugComments);

                if (editingStep.DoAction != null)
                {
                    editingStep.DoAction(editingStep, ref expectedData);
                    QueueHelper.WaitTillQueueItemsProcessed();
                }
                if (editingStep.VerifyAfterAction != null)
                {
                    editingStep.VerifyAfterAction(editingStep, ref expectedData);
                    QueueHelper.WaitTillQueueItemsProcessed();
                }
            }
        }

        #endregion EditingSteps Procedure

        #region DoActions

        protected virtual void DoBeginEditing<T>(EditingStepInfo<T> editingStepInfo, ref EditingData<T> expectedData)
        {
            Assert.AssertTrue("begin action must be set to do this action.", editingStepInfo.beginAction != DataGridCommandHelper.BeginEditAction.None);

            this.DoBeginEditing(editingStepInfo.row, editingStepInfo.col, editingStepInfo.beginAction);
        }

        protected virtual void DoCommitEditing<T>(EditingStepInfo<T> editingStepInfo, ref EditingData<T> expectedData)
        {
            Assert.AssertTrue("commit action must be set to do this action.", editingStepInfo.commitAction != DataGridCommandHelper.CommitEditAction.None);

            if (editingStepInfo.commitAction == DataGridCommandHelper.CommitEditAction.MouseClickColumnHeader)
            {
                // verify the row that is being edited is selected as the SelectedItem property
                // will be used later to verify the row is committed after a sort
                int selectedIndex = MyDataGrid.SelectedIndex;
                if (selectedIndex != editingStepInfo.row)
                {
                    throw new TestValidationException(string.Format("Row that is being edited must be the selected item. Expect: {0}, Actual: {1}", editingStepInfo.row, selectedIndex));
                }
            }

            if (editingStepInfo.mouseClickColIdx == -1 && editingStepInfo.mouseClickRowIdx == -1)
            {
                this.DoCommitEditing(editingStepInfo.row, editingStepInfo.col, editingStepInfo.commitAction);
            }
            else
            {
                this.DoCommitEditing(editingStepInfo.row, editingStepInfo.col, editingStepInfo.commitAction, editingStepInfo.mouseClickRowIdx, editingStepInfo.mouseClickColIdx);
            }
        }

        protected virtual void DoCancelEditing<T>(EditingStepInfo<T> editingStepInfo, ref EditingData<T> expectedData)
        {
            Assert.AssertTrue("cancel action must be set to do this action.", editingStepInfo.cancelAction != DataGridCommandHelper.CancelEditAction.None);

            this.DoCancelEditing(editingStepInfo.row, editingStepInfo.col, editingStepInfo.cancelAction);
        }

        protected virtual void DoEditCell<T>(EditingStepInfo<T> editingStepInfo, ref EditingData<T> expectedData)
        {
            string expectedStringData;
            DataGridActionHelper.EditCellGenericInput(MyDataGrid, editingStepInfo.row, editingStepInfo.col, out expectedStringData);

            // record the expected data
            this.SetPropertyValue<T>(editingStepInfo, ref expectedData, false, expectedStringData);
        }

        protected virtual void DoScrollIntoView<T>(EditingStepInfo<T> editingStepInfo, ref EditingData<T> expectedData)
        {
            MyDataGrid.ScrollIntoView(MyDataGrid.Items[editingStepInfo.row]);
            QueueHelper.WaitTillQueueItemsProcessed();
        }

        #endregion DoActions

        #region VerifyAfterActions

        /// <summary>
        /// Calls into the standard VerifyAfterBegin method.  No new state recorded.
        /// </summary>
        protected virtual void VerifyAfterBegin<T>(EditingStepInfo<T> editingStepInfo, ref EditingData<T> expectedData)
        {
            this.VerifyAfterBegin(editingStepInfo.row, editingStepInfo.col);
        }

        /// <summary>
        /// Calls into the standard VerifyAfterCommitCell method.  No new state recorded.
        /// Also, verifies the state of the new cell.
        /// </summary>
        protected virtual void VerifyAfterCommitCell<T>(EditingStepInfo<T> editingStepInfo, ref EditingData<T> expectedData)
        {
            // for particular actions, verify still in edit mode
            this.VerifyNewCellCommitResult<T>(editingStepInfo, expectedData);

            object data = this.GetPropertyValue<T>(editingStepInfo, expectedData, false);

            DataGridVerificationHelper.VerifyCurrentCellEditMode(
                MyDataGrid,
                editingStepInfo.row,
                editingStepInfo.col,
                false       /* expected IsEditing value */,
                false       /* do not verify the CurrentCell info */,
                -1          /* the new current row */,
                -1          /* the new current col */);
            DataGridVerificationHelper.VerifyCellData(
                MyDataGrid,
                DataSource                          /* source to verify data from */,
                TypeFromDataSource                  /* the type from the data source*/,
                editingStepInfo.row                 /* row of cell to verify */,
                editingStepInfo.col                 /* column of cell to verify */,
                data.ToString()                     /* expected cell data */,
                false                               /* expected IsEditing value */,
                false                               /* cell data should not have persisted to the data source */,
                GetDataFromTemplateColumn,
                GetDisplayBindingFromTemplateColumn);
        }

        /// <summary>
        /// Calls into the standard VerifyAfterCommitRow method.  No new state recorded.
        /// Also, verifies the complete row data and verifies the state of the new cell.
        /// </summary>
        protected virtual void VerifyAfterCommitRow<T>(EditingStepInfo<T> editingStepInfo, ref EditingData<T> expectedData) where T : IDataGridDataType
        {
            QueueHelper.WaitTillQueueItemsProcessed();

            if (editingStepInfo.commitAction == DataGridCommandHelper.CommitEditAction.MouseClickColumnHeader)
            {
                // need to update the row value due to the sort
                editingStepInfo.row = MyDataGrid.SelectedIndex;

                // scroll into view
                MyDataGrid.ScrollIntoView(MyDataGrid.Items[editingStepInfo.row]);
                QueueHelper.WaitTillQueueItemsProcessed();
            }
            else if (editingStepInfo.commitAction == DataGridCommandHelper.CommitEditAction.Enter && editingStepInfo.columnIsSorted)
            {
                // need to update the row value due to the sort (index will be one lower due to Enter)
                editingStepInfo.row = MyDataGrid.SelectedIndex - 1;

                // scroll into view
                MyDataGrid.ScrollIntoView(MyDataGrid.Items[editingStepInfo.row]);
                QueueHelper.WaitTillQueueItemsProcessed();
            }

            // verify the new row is not in edit mode
            this.VerifyNewCellCommitResult<T>(editingStepInfo, expectedData);

            // verify the current row is not in edit mode
            this.VerifyAfterCommitRow(editingStepInfo.row);

            // verify the complete row data
            T actualRowData = (T)MyDataGrid.Items[editingStepInfo.row];
            if (!expectedData.rowData.CustomEquals(actualRowData))
            {
                throw new TestValidationException(string.Format("Row data does not match after committing row."));
            }
        }

        /// <summary>
        /// Calls into the standard VerifyAfterCancelCell method.  Also updates the state of the expected data.
        /// </summary>
        protected virtual void VerifyAfterCancelCell<T>(EditingStepInfo<T> editingStepInfo, ref EditingData<T> expectedData)
        {
            object prevData = this.GetPropertyValue<T>(editingStepInfo, expectedData, true);
            this.VerifyAfterCancelCell(editingStepInfo.row, editingStepInfo.col, prevData.ToString());

            // need to update the expected data for other further verifications
            object[] args = { prevData };
            this.SetPropertyValue<T>(editingStepInfo, ref expectedData, false, args);
        }

        /// <summary>
        /// Calls into the standard VerifyAfterCancelRow method.
        /// Also, verifies the complete row and updates the state of the expected data.
        /// </summary>
        protected virtual void VerifyAfterCancelRow<T>(EditingStepInfo<T> editingStepInfo, ref EditingData<T> expectedData) where T : IDataGridDataType
        {
            object prevData = this.GetPropertyValue<T>(editingStepInfo, expectedData, true);
            this.VerifyAfterCancelRow(editingStepInfo.row, editingStepInfo.col, prevData.ToString());

            // verify the complete row reverted
            T actualRowData = (T)MyDataGrid.Items[editingStepInfo.row];
            if (!expectedData.prevRowData.CustomEquals(actualRowData))
            {
                throw new TestValidationException(string.Format("Row data does not match after committing row."));
            }

            // need to update the expected data for other further verifications
            expectedData.rowData = (T)expectedData.prevRowData.Clone();
        }

        /// <summary>
        /// Verify the state of the new cell which will depend on the type of commit action.
        /// </summary>
        protected virtual void VerifyNewCellCommitResult<T>(EditingStepInfo<T> editingStepInfo, EditingData<T> expectedData)
        {
            int newRow;
            int newCol;

            if (editingStepInfo.commitAction == DataGridCommandHelper.CommitEditAction.Tab)
            {
                newCol = (IncrementColumnBy(editingStepInfo.col, 1)) % MyDataGrid.Columns.Count;
                newRow = editingStepInfo.row;

                // account for jumping to a new row
                if (ColumnLessThan(newCol, editingStepInfo.col))
                {
                    newRow = (editingStepInfo.row + 1) % MyDataGrid.Items.Count;
                }

                // if still in the same row, should still be in edit mode
                if (newRow == editingStepInfo.row)
                {
                    DataGridColumn column = MyDataGrid.ColumnFromDisplayIndex(newCol);

                    if (column.IsReadOnly)
                    {
                        DataGridVerificationHelper.VerifyCurrentCellEditMode(
                            MyDataGrid,
                            newRow,
                            newCol,
                            false   /* expected IsEditing value */,
                            true    /* verify the CurrentCell info */,
                            newRow  /* the new current row */,
                            newCol  /* the new current col */);
                        DataGridVerificationHelper.VerifyCurrentRowEditMode(
                            MyDataGrid,
                            newRow,
                            true);
                    }
                    else
                    {
                        this.VerifyAfterBegin(newRow, newCol);
                    }
                }
                else
                {
                    DataGridVerificationHelper.VerifyCurrentCellEditMode(
                        MyDataGrid,
                        newRow,
                        newCol,
                        false   /* expected IsEditing value */,
                        true    /* verify the CurrentCell info */,
                        newRow  /* the new current row */,
                        newCol  /* the new current col */);
                    DataGridVerificationHelper.VerifyCurrentRowEditMode(
                        MyDataGrid,
                        newRow,
                        false);
                }
            }
            else if (editingStepInfo.commitAction == DataGridCommandHelper.CommitEditAction.ShiftTab)
            {
                newCol = editingStepInfo.col;
                newRow = editingStepInfo.row;
                if (ColumnEqual(newCol, 0))
                {
                    newCol = ColumnEquivalent(MyDataGrid.Columns.Count - 1);
                    if (newRow == 0)
                        newRow = MyDataGrid.Items.Count - 1;
                    else
                        newRow = newRow - 1;
                }
                else
                {
                    newCol = DecrementColumnBy(newCol, 1);
                }

                // if still in the same row, should still be in edit mode
                if (newRow == editingStepInfo.row)
                {
                    DataGridColumn column = DataGridHelper.GetColumn(MyDataGrid, newCol);
                    if (column.IsReadOnly)
                    {
                        DataGridVerificationHelper.VerifyCurrentCellEditMode(
                            MyDataGrid,
                            newRow,
                            newCol,
                            false   /* expected IsEditing value */,
                            true    /* verify the CurrentCell info */,
                            newRow  /* the new current row */,
                            newCol  /* the new current col */);
                        DataGridVerificationHelper.VerifyCurrentRowEditMode(
                            MyDataGrid,
                            newRow,
                            true);
                    }
                    else
                    {
                        this.VerifyAfterBegin(newRow, newCol);
                    }
                }
                else
                {
                    DataGridVerificationHelper.VerifyCurrentCellEditMode(
                        MyDataGrid,
                        newRow,
                        newCol,
                        false   /* expected IsEditing value */,
                        true    /* verify the CurrentCell info */,
                        newRow  /* the new current row */,
                        newCol  /* the new current col */);
                    DataGridVerificationHelper.VerifyCurrentRowEditMode(
                        MyDataGrid,
                        newRow,
                        false);
                }
            }
            else if (editingStepInfo.commitAction == DataGridCommandHelper.CommitEditAction.MouseClickDifferentCell)
            {
                DataGridVerificationHelper.VerifyCurrentCellEditMode(
                    MyDataGrid,
                    editingStepInfo.mouseClickRowIdx,
                    editingStepInfo.mouseClickColIdx,
                    false                                 /* expected IsEditing value */,
                    true                                  /* verify the CurrentCell info */,
                    editingStepInfo.mouseClickRowIdx      /* the new current row */,
                    editingStepInfo.mouseClickColIdx      /* the new current col */);

                DataGridVerificationHelper.VerifyCurrentRowEditMode(
                    MyDataGrid,
                    editingStepInfo.mouseClickRowIdx,
                    true);
            }
            else if (editingStepInfo.commitAction == DataGridCommandHelper.CommitEditAction.Enter)
            {
                newCol = editingStepInfo.col;
                newRow = editingStepInfo.row + 1;

                DataGridVerificationHelper.VerifyCurrentCellEditMode(
                    MyDataGrid,
                    newRow,
                    newCol,
                    false   /* expected IsEditing value */,
                    true    /* verify the CurrentCell info */,
                    newRow  /* the new current row */,
                    newCol  /* the new current col */);
                DataGridVerificationHelper.VerifyCurrentRowEditMode(
                    MyDataGrid,
                    newRow,
                    false);
            }
            else if (editingStepInfo.commitAction == DataGridCommandHelper.CommitEditAction.MouseClickDifferentRow)
            {
                DataGridVerificationHelper.VerifyCurrentCellEditMode(
                    MyDataGrid,
                    editingStepInfo.mouseClickRowIdx,
                    editingStepInfo.col,
                    false                               /* expected IsEditing value */,
                    true                                /* do not verify the CurrentCell info */,
                    editingStepInfo.mouseClickRowIdx    /* the new current row */,
                    editingStepInfo.col                 /* the new current col */);
                DataGridVerificationHelper.VerifyCurrentRowEditMode(
                    MyDataGrid,
                    editingStepInfo.mouseClickRowIdx,
                    false);
            }
        }

        /// <summary>
        /// Calls into the standard VerifyAfterCancelRow method.
        /// Also, verifies the complete row and updates the state of the expected data.
        /// </summary>
        protected virtual void VerifyCellStateAfterNormalTab<T>(EditingStepInfo<T> editingStepInfo, ref EditingData<T> expectedData) where T : IDataGridDataType
        {
            int newCol = (IncrementColumnBy(editingStepInfo.col, 1)) % MyDataGrid.Columns.Count;
            int newRow = editingStepInfo.row;

            // account for jumping to a new row
            if (ColumnLessThan(newCol, editingStepInfo.col))
            {
                newRow = (editingStepInfo.row + 1) % MyDataGrid.Items.Count;
            }

            // previous cell is not editable
            DataGridVerificationHelper.VerifyCurrentCellEditMode(
                MyDataGrid,
                editingStepInfo.row,
                editingStepInfo.col,
                false   /* expected IsEditing value */,
                false   /* do not verify the CurrentCell info */,
                -1      /* the new current row */,
                -1      /* the new current col */);

            // new cell is not editable
            DataGridVerificationHelper.VerifyCurrentCellEditMode(
                MyDataGrid,
                newRow,
                newCol,
                false   /* expected IsEditing value */,
                true    /* verify the CurrentCell info */,
                newRow  /* the new current row */,
                newCol  /* the new current col */);
        }

        protected virtual void VerifyAfterCommitRowFromSort<T>(EditingStepInfo<T> editingStepInfo, ref EditingData<T> expectedData) where T : IDataGridDataType
        {
            //
        }

        #endregion VerifyAfterActions

        #region General Helpers

        public string GetBindingNameFromCell(int row, int col)
        {
            DataGridCell cell = DataGridHelper.GetCell(MyDataGrid, row, col);
            DataGridColumn column = cell.Column;

            if (column is DataGridBoundColumn)
            {
                DataGridBoundColumn boundColumn = column as DataGridBoundColumn;
                return (boundColumn.Binding as Binding).Path.Path;
            }
            else if (column is DataGridComboBoxColumn)
            {
                DataGridComboBoxColumn comboBoxColumn = column as DataGridComboBoxColumn;
                if (comboBoxColumn.SelectedItemBinding != null)
                {
                    return (comboBoxColumn.SelectedItemBinding as Binding).Path.Path;
                }
                else if (comboBoxColumn.SelectedValueBinding != null)
                {
                    return (comboBoxColumn.SelectedValueBinding as Binding).Path.Path;
                }
                else if (comboBoxColumn.TextBinding != null)
                {
                    return (comboBoxColumn.TextBinding as Binding).Path.Path;
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return this.GetDisplayBindingFromTemplateColumn(cell);
            }
        }

        public object GetPropertyValue<T>(EditingStepInfo<T> editingStepInfo, EditingData<T> expectedData, bool fromPrevData)
        {
            string propertyName = this.GetBindingNameFromCell(editingStepInfo.row, editingStepInfo.col);
            Type type;
            if (fromPrevData)
            {
                type = expectedData.prevRowData.GetType();
                return type.InvokeMember(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty, null, expectedData.prevRowData, null);
            }
            else
            {
                type = expectedData.rowData.GetType();
                return type.InvokeMember(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty, null, expectedData.rowData, null);
            }
        }

        public void SetPropertyValue<T>(EditingStepInfo<T> editingStepInfo, ref EditingData<T> expectedData, bool fromPrevData, object[] args)
        {
            string propertyName = this.GetBindingNameFromCell(editingStepInfo.row, editingStepInfo.col);
            Type type;
            if (fromPrevData)
            {
                type = expectedData.prevRowData.GetType();
                type.InvokeMember(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty, null, expectedData.prevRowData, args);
            }
            else
            {
                type = expectedData.rowData.GetType();
                type.InvokeMember(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty, null, expectedData.rowData, args);
            }
        }

        public void SetPropertyValue<T>(EditingStepInfo<T> editingStepInfo, ref EditingData<T> expectedData, bool fromPrevData, string value)
        {
            string propertyName = this.GetBindingNameFromCell(editingStepInfo.row, editingStepInfo.col);
            Type type;
            if (fromPrevData)
            {
                type = expectedData.prevRowData.GetType();
            }
            else
            {
                type = expectedData.rowData.GetType();
            }
            PropertyInfo pi = type.GetProperty(propertyName);
            object expectedPropertyData = this.TransformDataToCorrectType(value, pi.PropertyType);
            object[] args = { expectedPropertyData };

            this.SetPropertyValue<T>(editingStepInfo, ref expectedData, fromPrevData, args);
        }

        //
        public object TransformDataToCorrectType(string dataAsStr, Type expectingType)
        {
            if (typeof(String).IsAssignableFrom(expectingType))
            {
                return dataAsStr;
            }
            else if (typeof(Int32).IsAssignableFrom(expectingType))
            {
                int intVal = Int32.Parse(dataAsStr);
                return intVal;
            }
            else if (typeof(Boolean).IsAssignableFrom(expectingType))
            {
                bool boolVal = Boolean.Parse(dataAsStr);
                return boolVal;
            }
            else if (typeof(Uri).IsAssignableFrom(expectingType))
            {
                Uri uri = new Uri(dataAsStr);
                return uri;
            }
            else if (typeof(DateTime).IsAssignableFrom(expectingType))
            {
                DateTime dateTime = DateTime.Parse(dataAsStr);
                return dateTime;
            }
            else if (typeof(Person.EyeColor).IsAssignableFrom(expectingType))
            {
                Person.EyeColor eyeColor = (Person.EyeColor)Enum.Parse(typeof(Person.EyeColor), dataAsStr);
                return eyeColor;
            }

            return null;
        }

        #endregion General Helpers

        #endregion Editing Step Helpers

        #region Override Members

        /// <summary>
        /// Initial Setup
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        public override TestResult Setup()
        {
            base.Setup();

            Status("Setup specific for DataGridEditing");

            this.SetupDataSource();

            LogComment("Setup for DataGridEditing was successful");
            return TestResult.Pass;
        }

        #endregion Override Members
    }
}
