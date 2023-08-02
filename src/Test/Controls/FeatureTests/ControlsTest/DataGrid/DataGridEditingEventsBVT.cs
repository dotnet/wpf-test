using System.Windows.Controls;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

using Microsoft.Test.Controls.DataSources;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// 
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridEditingEventsBVT", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords = "Localization_Suite")]
    public class DataGridEditingEventsBVT : DataGridEditing
    {
        #region Constructor

        public DataGridEditingEventsBVT()
            : base(@"DataGridEditing.xaml")
        {
            DataSource = new EditablePeople();
            TypeFromDataSource = typeof(EditablePerson);
            this.CreateDataSource();

            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestBeginningEdit);
            RunSteps += new TestStep(TestCancellingEdit);
            RunSteps += new TestStep(TestCommitingEdit);
            RunSteps += new TestStep(TestPrepareCellForEdit);
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

            Status("Setup specific for DataGridEditingBVT");

            // set the current cell to be edited as a DataGridTextColumn
            int col = -1;
            foreach (DataGridColumn column in MyDataGrid.Columns)
            {
                col++;
                if (column is DataGridTextColumn)
                {
                    break;
                }
            }

            if (col == -1)
            {
                LogComment("Can not find a DataGridTextColumn in dataGrid");
                return TestResult.Fail;
            }
            else
            {
                CurCol = col;

                // row is arbitrary
                CurRow = 0;
            }

            LogComment("Setup for DataGridEditingBVT was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestBeginningEdit()
        {
            Status("TestBeginningEdit");

            DataGridBeginningEditEventArgs expectedArgs = null;
            DataGridBeginningEditEventArgs actualEventArgs = null;

            this.DoBeginAndRecordEvent(CurRow, CurCol, DataGridCommandHelper.BeginEditAction.F2, out expectedArgs, out actualEventArgs);
            this.VerifyBeginEventArgs(expectedArgs, actualEventArgs);

            LogComment("TestBeginningEdit was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestCancellingEdit()
        {
            Status("TestCancellingEdit");

            DataGridCellEditEndingEventArgs expectedCellEventArgs = null;
            DataGridCellEditEndingEventArgs actualCellEventArgs = null;
            DataGridRowEditEndingEventArgs expectedRowEventArgs = null;
            DataGridRowEditEndingEventArgs actualRowEventArgs = null;

            LogComment("cancel the cell edit");
            this.DoCancelAndRecordEvent(
                CurRow,
                CurCol,
                DataGridCommandHelper.CancelEditAction.Esc,
                out expectedCellEventArgs,
                out actualCellEventArgs,
                out expectedRowEventArgs,
                out actualRowEventArgs);

            LogComment("verify the cell cancel");
            this.VerifyEndingEventArgs(
                expectedCellEventArgs,
                actualCellEventArgs,
                expectedRowEventArgs,
                actualRowEventArgs);

            LogComment("cancel the entire row");
            this.DoCancelAndRecordEvent(
                CurRow,
                CurCol,
                DataGridCommandHelper.CancelEditAction.Esc,
                out expectedCellEventArgs,
                out actualCellEventArgs,
                out expectedRowEventArgs,
                out actualRowEventArgs);

            LogComment("verify the row cancel");
            this.VerifyEndingEventArgs(
                expectedCellEventArgs,
                actualCellEventArgs,
                expectedRowEventArgs,
                actualRowEventArgs);

            LogComment("TestCancellingEdit was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestCommitingEdit()
        {
            Status("TestCommitingEdit");

            DataGridCellEditEndingEventArgs expectedCellEventArgs = null;
            DataGridCellEditEndingEventArgs actualCellEventArgs = null;
            DataGridRowEditEndingEventArgs expectedRowEventArgs = null;
            DataGridRowEditEndingEventArgs actualRowEventArgs = null;

            DataGridCommandHelper.BeginEdit(MyDataGrid, CurRow, CurCol);
            this.DoCommitAndRecordEvent(
                CurRow,
                CurCol,
                DataGridCommandHelper.CommitEditAction.Enter,
                out expectedCellEventArgs,
                out actualCellEventArgs,
                out expectedRowEventArgs,
                out actualRowEventArgs);

            this.VerifyEndingEventArgs(
                expectedCellEventArgs,
                actualCellEventArgs,
                expectedRowEventArgs,
                actualRowEventArgs);

            LogComment("TestCommitingEdit was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestPrepareCellForEdit()
        {
            Status("TestPrepareCellForEdit");

            DataGridPreparingCellForEditEventArgs expectedArgs = null;
            DataGridPreparingCellForEditEventArgs actualEventArgs = null;

            this.DoBeginAndRecordPrepareEvent(CurRow, CurCol, DataGridCommandHelper.BeginEditAction.F2, out expectedArgs, out actualEventArgs);
            this.VerifyPreparingEventArgs(expectedArgs, actualEventArgs);

            DataGridCommandHelper.CancelEdit(MyDataGrid);

            LogComment("TestPrepareCellForEdit was successful");
            return TestResult.Pass;
        }

        #endregion Test Steps
    }
}
