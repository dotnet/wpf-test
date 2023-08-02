using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System;
using System.Collections.Generic;
using System.Windows.Threading;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Regression Test  App crash when click DataGrid header to sort
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridRegressionTest38", SecurityLevel = TestCaseSecurityLevel.FullTrust, Versions = "4.5.1+")]
    public class DataGridRegressionTest38 : DataGridTest
    {
        #region Constructor

        public DataGridRegressionTest38()
            : base(@"DataGridRegressionTest38.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(ClickDataGridHeaderToSort);
        }

        #endregion

        #region Test Steps

        public override TestResult Setup()
        {
            base.Setup();
            Status("Setup specific for DataGridRegressionTest38");
            Window.Height = 400;
            Window.Width = 400;
            this.SetupDataSource();

            LogComment("Setup for DataGridRegressionTest38 was successful");
            return TestResult.Pass;
        }

        public override TestResult CleanUp()
        {
            return base.CleanUp();
        }

        protected override void SetupDataSource()
        {
            Status("SetupDataSource");
            List<PersonItem> list = new List<PersonItem>();
            list.Add(new PersonItem { Name = "Tom" });
            list.Add(new PersonItem { Name = "----" });
            list.Add(new PersonItem { Name = "Harry" });

            this.MyDataGrid.ItemsSource = list;
            this.WaitForPriority(DispatcherPriority.SystemIdle);
            Assert.AssertTrue("Failed to setup datagrid data source", MyDataGrid.ItemsSource != null);
            LogComment("SetupDataSource was successful");
        }

        private TestResult ClickDataGridHeaderToSort()
        {
            Status("ClickDataGridHeaderToSort");

            LogComment("Click a cell of datagrid to select one item");
            DataGridActionHelper.ClickOnCell(MyDataGrid, 0, 0);
            this.WaitForPriority(DispatcherPriority.SystemIdle);
            LogComment("Click header of datagrid to sort");

            try
            {
                DataGridActionHelper.ClickOnColumnHeader(MyDataGrid, 0);
            }
            catch (InvalidCastException ex)
            {
                LogComment("Case failed due to Regression Bug:\n" + ex.ToString());
                return TestResult.Fail;
            }
            catch (Exception ex)
            {
                LogComment("Unknown Exception:\n" + ex.ToString());
                return TestResult.Fail;
            }

            this.WaitForPriority(DispatcherPriority.SystemIdle);

            LogComment("ClickDataGridHeaderToSort was successful");
            return TestResult.Pass;
        }

        #endregion Test Steps

    }

    #region Helper Classes

    public class PersonItem
    {
        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            PersonItem item = (PersonItem)obj;    // common app error - casting obj before checking its type
            return (this == item);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    #endregion
}
