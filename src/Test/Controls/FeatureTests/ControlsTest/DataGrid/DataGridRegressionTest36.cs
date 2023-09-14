using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// DataGrid in RowDetailsTemplate generates NullReferenceException when DataGridTextColumn is read-only (IsReadOnly="True")
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridRegressionTest36", SecurityLevel = TestCaseSecurityLevel.FullTrust, Versions = "4.5.1+")]
    public class DataGridRegressionTest36 : DataGridTest
    {
        private DataGrid detailsDataGrid;

        #region Constructor

        public DataGridRegressionTest36()
            : base(@"DataGridRegressionTest36.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(ClickDataGridDetailsColumnToEdit);
        }

        #endregion

        #region Test Steps

        public override TestResult Setup()
        {
            Status("Setup specific for DataGridRegressionTest36");

            base.Setup();
            this.SetupDataSource();
            MyDataGrid.LoadingRowDetails += MyDataGrid_LoadingRowDetails;

            LogComment("Setup for DataGridRegressionTest36 was successful");
            return TestResult.Pass;
        }

        public override TestResult CleanUp()
        {
            detailsDataGrid = null;
            return base.CleanUp();
        }

        private void MyDataGrid_LoadingRowDetails(object sender, DataGridRowDetailsEventArgs e)
        {
            Status("MyDataGrid_LoadingRowDetails");

            detailsDataGrid = e.DetailsElement as DataGrid;
            detailsDataGrid.ItemsSource = DataSource;
            this.WaitForPriority(DispatcherPriority.SystemIdle);
            Assert.AssertTrue("Failed to setup datagrid details source", detailsDataGrid.ItemsSource != null);

            LogComment("MyDataGrid_LoadingRowDetails was successful");
        }

        private TestResult ClickDataGridDetailsColumnToEdit()
        {
            Status("ClickDataGridDetailsColumnToEdit");

            LogComment("Navigation Tab Key to details Cell");
            for (int i = 0; i <= 2; i++)
            {
                Microsoft.Test.Input.Keyboard.Press(Microsoft.Test.Input.Key.Tab);
                this.WaitForPriority(DispatcherPriority.SystemIdle);
            }

            DataGridCell Mydatagriddetailcell = DataGridHelper.GetCell(detailsDataGrid, 0, 0);
            Assert.AssertTrue("Failed to set details get Focus", Mydatagriddetailcell.IsFocused == true);

            try
            {
                LogComment("Press space key to edit");
                Microsoft.Test.Input.Keyboard.Press(Microsoft.Test.Input.Key.Space);
                this.WaitForPriority(DispatcherPriority.SystemIdle);
            }
            catch (NullReferenceException ex)
            {

                LogComment("Case failed due to Regression Bug\n" + ex.ToString());
                return TestResult.Fail;
            }
            catch (Exception ex)
            {
                LogComment("Unknown Exception\n" + ex.ToString());
                return TestResult.Fail;
            }

            LogComment("ClickDataGridDetailsColumnToEdit was successful");
            return TestResult.Pass;

        }

        #endregion Test Steps

    }
}
