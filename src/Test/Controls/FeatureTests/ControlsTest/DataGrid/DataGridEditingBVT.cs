using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.Collections;


namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    ///
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridEditingBVT", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords = "Localization_Suite")]
    public class DataGridEditingBVT : DataGridEditing
    {
        #region Constructor

        public DataGridEditingBVT()
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
        /// Verify rows and cells can be put into editing mode.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestBeginEditing()
        {
            Status("TestBeginEditing");

            DataGridCommandHelper.BeginEdit(MyDataGrid, CurRow, CurCol);

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

            // current cell should already be in editing mode
            DataGridVerificationHelper.VerifyCurrentCellEditMode(
                MyDataGrid,
                CurRow,
                CurCol,
                true    /* expected IsEditing value */,
                true    /* verify the CurrentCell info */,
                CurRow  /* the new current row */,
                CurCol  /* the new current col */);

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

            LogComment("TestCancelEditing was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify rows and cells can be cancelled from editing mode.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestCommitEditing()
        {
            Status("TestCommitEditing");

            // turn editing mode back on
            DataGridCommandHelper.BeginEdit(MyDataGrid, CurRow, CurCol);
            DataGridVerificationHelper.VerifyCurrentCellEditMode(
                MyDataGrid,
                CurRow,
                CurCol,
                true    /* expected IsEditing value */,
                true    /* verify the CurrentCell info */,
                CurRow  /* the new current row */,
                CurCol  /* the new current col */);

            // edit the cell and record data
            string expectedData;
            DataGridActionHelper.EditCellGenericInput(MyDataGrid, CurRow, CurCol, out expectedData);
            this.WaitForPriority(DispatcherPriority.SystemIdle);

            // commit the changes
            DataGridCommandHelper.CommitEdit(MyDataGrid, CurRow, CurCol);
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
                DataSource as IEnumerable,
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
            DataGridVerificationHelper.VerifyCurrentCellEditMode(
                MyDataGrid,
                CurRow,
                CurCol,
                true    /* expected IsEditing value */,
                true    /* verify the CurrentCell info */,
                CurRow  /* the new current row */,
                CurCol  /* the new current col */);

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
            DataGridVerificationHelper.VerifyCurrentCellEditMode(
                MyDataGrid,
                CurRow,
                CurCol,
                false   /* expected IsEditing value */,
                false   /* do not verify the CurrentCell info */,
                -1      /* the new current row */,
                -1      /* the new current col */);
            this.WaitForPriority(DispatcherPriority.SystemIdle);

            // verify previous data is in tact
            DataGridVerificationHelper.VerifyCellData(
                MyDataGrid,
                DataSource as IEnumerable,
                TypeFromDataSource,
                CurRow,
                CurCol,
                previousData,
                false,
                true,
                GetDataFromTemplateColumn,
                GetDisplayBindingFromTemplateColumn);

            LogComment("TestCancelAfterEditing was successful");
            return TestResult.Pass;
        }

        #endregion Test Steps
    }
}
