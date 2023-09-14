using Avalon.Test.ComponentModel;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.Windows.Controls;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Regression Test   Verify able to set DataGridColumn.DisplayIndex declaratively in such
    /// a way that the ordering is not the default.
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridRegressionTest30", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DataGridRegressionTest30 : DataGridTest
    {
        #region Constructor

        public DataGridRegressionTest30()
            : base(@"DataGridRegressionTest30.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestDisplayIndexOrdering);
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

            Status("Setup specific for DataGridRegressionTest30");

            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("Setup for DataGridRegressionTest30 was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 1. Verify no ArgumentException is thrown when the DataGrid is presented
        /// 2. Verify the DisplayIndex ordering is correct.
        /// </summary>
        private TestResult TestDisplayIndexOrdering()
        {
            Status("TestDisplayIndexOrdering");
                        
            LogComment("verify the display index ordering");
            Assert.AssertTrue(string.Format("DisplayIndex for Column 0 is incorrect. Expected: {0}, Actual: {1}", 2, MyDataGrid.Columns[0].DisplayIndex), MyDataGrid.Columns[0].DisplayIndex == 2);
            Assert.AssertTrue(string.Format("DisplayIndex for Column 2 is incorrect. Expected: {0}, Actual: {1}", 0, MyDataGrid.Columns[2].DisplayIndex), MyDataGrid.Columns[2].DisplayIndex == 0);

            LogComment("getting to this point signals that no exception was thrown due to setting the DisplayIndexes in the xaml file.");

            LogComment("TestDisplayIndexOrdering was successful");
            return TestResult.Pass;
        }                

        #endregion Test Steps      
    }
}
