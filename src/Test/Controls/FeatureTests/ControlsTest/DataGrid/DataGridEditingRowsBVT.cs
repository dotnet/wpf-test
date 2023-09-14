using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Controls.DataSources;
using System.Collections;


namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    ///
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridEditingRowsBVT", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords = "Localization_Suite")]
    public class DataGridEditingRowsBVT : DataGridEditing
    {
        #region Constructor

        public DataGridEditingRowsBVT()
            : base(@"DataGridEditing.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestBeginEditing);
            RunSteps += new TestStep(TestCancelEditing);
            RunSteps += new TestStep(TestCommitEditing);
            RunSteps += new TestStep(TestCancelAfterEditing);
        }

        #endregion

        #region Test Steps

        /// <summary>
        /// Initial Setup
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        public override TestResult Setup()
        {
            Status("Setup specific for DataGridEditingRowsBVT");

            // setup the data source
            DataSource = new EditablePeople();
            TypeFromDataSource = typeof(EditablePerson);

            // sets up the data source
            base.Setup();

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
                LogComment("Can not find a DataGridEditingRowsBVT in dataGrid");
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
        /// Verify rows and cells can be put into editing mode.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestBeginEditing()
        {
            Status("TestBeginEditing");

            DataGridCommandHelper.BeginEdit(MyDataGrid, CurRow, CurCol);

            // verify the row.IsEditing value
            DataGridVerificationHelper.VerifyCurrentRowEditMode(MyDataGrid, CurRow, true);

            // verify cell is editable
            DataGridVerificationHelper.VerifyCurrentCellEditMode(
                MyDataGrid,
                CurRow,
                CurCol,
                true    /* expected IsEditing value */,
                true    /* verify the CurrentCell info */,
                CurRow  /* the new current row */,
                CurCol  /* the new current col */);

            LogComment("TestBeginEditing was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify rows and cells can be cancelled from editing mode.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestCancelEditing()
        {
            Status("TestCancelEditing");

            // verify the row.IsEditing value
            DataGridVerificationHelper.VerifyCurrentRowEditMode(MyDataGrid, CurRow, true);

            // cancel once
            DataGridCommandHelper.CancelEdit(MyDataGrid);
            this.WaitForPriority(DispatcherPriority.SystemIdle);

            // verify cell is not editable
            DataGridVerificationHelper.VerifyCurrentCellEditMode(
                MyDataGrid,
                CurRow,
                CurCol,
                false   /* expected IsEditing value */,
                false   /* do not verify the CurrentCell info */,
                -1      /* the new current row */,
                -1      /* the new current col */);

            // verify still in row editing mode
            DataGridVerificationHelper.VerifyCurrentRowEditMode(MyDataGrid, CurRow, true);

            // cancel again
            DataGridCommandHelper.CancelEdit(MyDataGrid);
            this.WaitForPriority(DispatcherPriority.SystemIdle);

            // verify cell is not editable
            DataGridVerificationHelper.VerifyCurrentCellEditMode(
                MyDataGrid,
                CurRow,
                CurCol,
                false   /* expected IsEditing value */,
                false   /* do not verify the CurrentCell info */,
                -1      /* the new current row */,
                -1      /* the new current col */);

            // verify row is not editable
            DataGridVerificationHelper.VerifyCurrentRowEditMode(MyDataGrid, CurRow, false);

            LogComment("TestCancelEditing was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify rows and cells can be committed from editing mode.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestCommitEditing()
        {
            Status("TestCommitEditing");

            // turn editing mode back on
            DataGridCommandHelper.BeginEdit(MyDataGrid, CurRow, CurCol);
            DataGridVerificationHelper.VerifyCurrentRowEditMode(MyDataGrid, CurRow, true);

            // edit the cell and record data
            string expectedData;
            DataGridActionHelper.EditCellGenericInput(MyDataGrid, CurRow, CurCol, out expectedData);
            this.WaitForPriority(DispatcherPriority.SystemIdle);

            // commit the changes
            DataGridCommandHelper.CommitEdit(MyDataGrid, CurRow, CurCol);

            // verify the row and cell IsEditing values
            DataGridVerificationHelper.VerifyCurrentRowEditMode(MyDataGrid, CurRow, false);
            DataGridVerificationHelper.VerifyCurrentCellEditMode(
                MyDataGrid,
                CurRow,
                CurCol,
                false       /* expected IsEditing value */,
                true        /* verify the CurrentCell info */,
                CurRow + 1  /* the new current row */,
                CurCol      /* the new current col */);

            // verify commit
            DataGridVerificationHelper.VerifyCellData(
                MyDataGrid,
                DataSource,
                TypeFromDataSource,
                CurRow,
                CurCol,
                expectedData,
                false,
                true,
                GetDataFromTemplateColumn,
                GetDisplayBindingFromTemplateColumn);

            LogComment("TestCommitEditing was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify rows and cells can be cancelled from editing mode after edit has occurred.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestCancelAfterEditing()
        {
            Status("TestCancelAfterEditing");

            // turn editing mode back on
            DataGridCommandHelper.BeginEdit(MyDataGrid, CurRow, CurCol);
            DataGridVerificationHelper.VerifyCurrentRowEditMode(MyDataGrid, CurRow, true);

            // retreive current data
            string previousData = DataGridHelper.GetDataFromCell(
                MyDataGrid,
                CurRow,
                CurCol,
                true,
                GetDataFromTemplateColumn);

            // edit the cell and record data
            string expectedData;
            DataGridActionHelper.EditCellGenericInput(MyDataGrid, CurRow, CurCol, out expectedData);
            this.WaitForPriority(DispatcherPriority.SystemIdle);

            // cancel the changes
            DataGridCommandHelper.CancelEdit(MyDataGrid);
            this.WaitForPriority(DispatcherPriority.SystemIdle);

            // veirfy still in row edit mode
            DataGridVerificationHelper.VerifyCurrentRowEditMode(MyDataGrid, CurRow, true);

            // verify previous data is in tact
            DataGridVerificationHelper.VerifyCellData(
                MyDataGrid,
                DataSource,
                TypeFromDataSource,
                CurRow,
                CurCol,
                previousData,
                false,
                true,
                GetDataFromTemplateColumn,
                GetDisplayBindingFromTemplateColumn);

            // cancel again to get out of row edit mode
            DataGridCommandHelper.CancelEdit(MyDataGrid);
            this.WaitForPriority(DispatcherPriority.SystemIdle);

            // verify row is not editable
            DataGridVerificationHelper.VerifyCurrentRowEditMode(MyDataGrid, CurRow, false);

            LogComment("TestCancelAfterEditing was successful");
            return TestResult.Pass;
        }

        #endregion Test Steps
    }
}
