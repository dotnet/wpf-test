using System.Linq;
using Avalon.Test.ComponentModel;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;


namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Tests around setting the IsReadOnly property on the DataGrid, DataGridColumn, and DataGridCell
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridIsReadOnlyBVT", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords = "Localization_Suite")]
    public class DataGridIsReadOnlyBVT : DataGridEditing
    {
        #region Private Members

        private string filename;
        private bool[] expectedIsReadOnlyColumns;
        private bool expectedDataGridIsReadOnly;

        #endregion Private Members

        #region Constructor

        public DataGridIsReadOnlyBVT()
            : this(@"DataGridIsReadOnly2.xaml")            
        {
        }

        [Variation(@"DataGridIsReadOnly.xaml")]
        [Variation(@"DataGridIsReadOnly2.xaml")]
        public DataGridIsReadOnlyBVT(string filename)
            : base(filename)
        {
            this.filename = filename;

            // setup the data source
            this.DataSourceTypeName = "Microsoft.Test.Controls.DataSources.EditablePeople, ControlsCommon";
            this.TypeNameFromDataSource = "Microsoft.Test.Controls.DataSources.EditablePerson, ControlsCommon";
            this.CreateDataSource();

            InitializeSteps += new TestStep(Setup);
            RunSteps += CreateTestStepFromGeneric(typeof(DataGridIsReadOnlyBVT), "TestDataGridInitialIsReadOnly");            
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

            expectedIsReadOnlyColumns = new bool[MyDataGrid.Columns.Count];

            if (filename == @"DataGridIsReadOnly.xaml")
            {
                for (int i = 0; i < expectedIsReadOnlyColumns.Length; i++)
                {
                    expectedIsReadOnlyColumns[i] = false;
                }

                expectedDataGridIsReadOnly = false;
                expectedIsReadOnlyColumns[1] = true;
                expectedIsReadOnlyColumns[2] = true;
                expectedIsReadOnlyColumns[4] = false;
            }
            else if (filename == @"DataGridIsReadOnly2.xaml")
            {
                for (int i = 0; i < expectedIsReadOnlyColumns.Length; i++)
                {
                    expectedIsReadOnlyColumns[i] = true;
                }

                expectedDataGridIsReadOnly = true;
                expectedIsReadOnlyColumns[1] = true;
                expectedIsReadOnlyColumns[2] = true;
                expectedIsReadOnlyColumns[4] = false;
            }

            LogComment("Setup for DataGridEditingAndIsReadOnly was successful");
            return TestResult.Pass;
        }

        #endregion Setup

        private TestResult TestDataGridInitialIsReadOnly<DT>() where DT : IDataGridDataType, new()
        {
            Status("TestDataGridInitialIsReadOnly");            

            LogComment("verify DataGrid.IsReadOnly by default is set to false");
            if (MyDataGrid.IsReadOnly != expectedDataGridIsReadOnly)
            {
                throw new TestValidationException(string.Format(
                        "DataGrid.IsReadOnly is incorrect.  Expected: {0}, Actual: {1}",
                        expectedDataGridIsReadOnly,
                        MyDataGrid.IsReadOnly));
            }
            
            LogComment("verify DataGridColumn.IsReadOnly and DataGridCell.IsReadOnly have been set accordingly");
            int i = 0;
            foreach (DataGridColumn column in MyDataGrid.Columns)
            {
                if (column.IsReadOnly != expectedIsReadOnlyColumns[i])
                {
                    throw new TestValidationException(string.Format(
                        "column: {0}, IsReadOnly is incorrect.  Expected: {1}, Actual: {2}",
                        i,
                        expectedIsReadOnlyColumns[i],
                        column.IsReadOnly));
                }

                for (int j = 0; j < MyDataGrid.Items.Count; j++)
                {
                    MyDataGrid.ScrollIntoView(MyDataGrid.Items[j], column);
                    QueueHelper.WaitTillQueueItemsProcessed();

                    DataGridCell cell = DataGridHelper.GetCell(MyDataGrid, j, i);

                    if (cell.IsReadOnly != expectedIsReadOnlyColumns[i])
                    {
                        throw new TestValidationException(string.Format(
                            "cell: ({0},{1}) IsReadOnly is incorrect.  Expected: {2}, Actual: {3}",
                            j,
                            i,
                            expectedIsReadOnlyColumns[i],
                            cell.IsReadOnly));
                    }
                }
                i++;
            }

            LogComment("TestDataGridInitialIsReadOnly was successful");
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
