using Avalon.Test.ComponentModel;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.Windows.Controls.Primitives;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Regression test : Verify it doesn't hung after scrolling
    /// </description>
    /// </summary>
    [Test(0, "DataGrid", "DataGridRegressionTest44", SecurityLevel = TestCaseSecurityLevel.FullTrust, Versions = "4.5.1+")]
    public class DataGridRegressionTest44 : DataGridTest
    {
        private ScrollBar dataGridSrollBar;

        #region Constructor

        public DataGridRegressionTest44()
            : base(@"DataGridRegressionTest44.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestScrolling);
        }

        #endregion

        #region Test Steps

        /// <summary>
        /// Initial Setup
        /// </summary>
        /// <returns></returns>
        public override TestResult Setup()
        {
            base.Setup();

            //Find srollling bar
            dataGridSrollBar = DataGridHelper.FindVisualChild<ScrollBar>(this.MyDataGrid);
            Assert.AssertTrue("Didn't find the ScrollBar!", dataGridSrollBar != null);

            return TestResult.Pass;
        }

        /// <summary>
        /// Verify: if it doesn't freeze after scrolling then pass, or fail;
        /// </summary>
        private TestResult TestScrolling()
        {
            Status("Begin to test scrolling");

            //Click the center of the scrolling bar twice
            Status("First scrolling beginning");
            UserInput.MouseLeftClickCenter(dataGridSrollBar);
            QueueHelper.WaitTillQueueItemsProcessed();
            Status("Second scrolling beginning");
            UserInput.MouseLeftClickCenter(dataGridSrollBar);
            QueueHelper.WaitTillQueueItemsProcessed();

            Status("Finish test scrolling");
            return TestResult.Pass;
        }

        #endregion Test Steps
    }
}
