using System.Windows.Controls;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;


namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Regression test : Verify that DataTriggers work on DataGridTemplateColumns.
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridRegressionTest22", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DataGridRegressionTest22 : DataGridTest
    {
        #region Constructor

        public DataGridRegressionTest22()
            : base(@"DataGridRegressionTest22.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestDataTriggerFiredOnTemplateColumn);
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

            Status("Setup specific for DataGridRegressionTest22");           

            this.SetupDataSource();

            LogComment("Setup for DataGridRegressionTest22 was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 1. Change the LikesCake value to false.  Verify cell in template column changes to Black
        /// 2. Change the LikesCake value to true. Verify cell in template column changes to Green
        /// </summary>
        private TestResult TestDataTriggerFiredOnTemplateColumn()
        {
            Status("TestDataTriggerFiredOnTemplateColumn");

            // find the template column, assumes only one TemplateColumn exists in columns
            int templateColumnIndex = DataGridHelper.FindFirstColumnTypeIndex(MyDataGrid, DataGridHelper.ColumnTypes.DataGridTemplateColumn);

            LogComment("change the LikesCake value of the first row to false");
            Person person = (MyDataGrid.Items[0] as Person);
            Assert.AssertTrue("Expects data source to have Person objects", person != null);
            person.LikesCake = false;

            LogComment("verify cell in the template column changes to black");
            DataGridCell cell = DataGridHelper.GetCell(MyDataGrid, 0, templateColumnIndex);
            ContentPresenter cp = cell.Content as ContentPresenter;
            Button button = DataGridHelper.FindVisualChild<Button>(cp);
            Assert.AssertTrue("button in TemplateColumn cell must exist for verification to proceed.", button != null);

            if (button.Foreground.ToString() != "#FF000000")
            {
                throw new TestValidationException(string.Format(
                    "button foreground is incorrect based on DataTrigger.  Expect: {0}, Actual: {1}",
                    "#FF000000",
                    button.Foreground));
            }

            LogComment("change the LikesCake value of the first row to true");
            person.LikesCake = true;

            LogComment("verify cell in the template column changes to green");
            if (button.Foreground.ToString() != "#FF008000")
            {
                throw new TestValidationException(string.Format(
                    "button foreground is incorrect based on DataTrigger.  Expect: {0}, Actual: {1}",
                    "#FF008000",
                    button.Foreground));
            }

            LogComment("TestDataTriggerFiredOnTemplateColumn was successful");
            return TestResult.Pass;
        }                

        #endregion Test Steps      
    }
}
