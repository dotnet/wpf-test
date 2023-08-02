using System.Windows.Controls;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Input;
using Avalon.Test.ComponentModel;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System;


namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Regression Test   Verify that bindings on the row header is updated when changed from the 
    /// data source.
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridRegressionTest29", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DataGridRegressionTest29 : DataGridTest
    {
        private Button debugButton;

        #region Constructor

        public DataGridRegressionTest29()
            : base(@"DataGridRegressionTest29.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestRowHeaderBindingUpdate);
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

            Status("Setup specific for DataGridRegressionTest29");

            debugButton = (Button)RootElement.FindName("btn_Debug");
            Assert.AssertTrue("Unable to find btn_Debug from the resources", debugButton != null);

            this.SetupDataSource();

            //Debug();

            LogComment("Setup for DataGridRegressionTest29 was successful");
            return TestResult.Pass;
        }

        public override TestResult CleanUp()
        {
            debugButton = null;

            return base.CleanUp();
        }
        /// <summary>
        /// 1. Note the value of the row header in row 0.  
        /// 2. Update row 0 ID value from the data source        
        /// 
        /// Verify: row header value updated accordingly
        /// </summary>
        private TestResult TestRowHeaderBindingUpdate()
        {
            Status("TestRowHeaderBindingUpdate");

            DataGridRowHeader rowHeader = DataGridHelper.GetRowHeader(MyDataGrid, 0);
            int originalValue = Int32.Parse(rowHeader.Content.ToString());

            // update the id to a different value
            int newValue = originalValue + 1;
            (MyDataGrid.Items[0] as Person).Id = newValue;

            int actualValue = Int32.Parse(rowHeader.Content.ToString());
            if (actualValue != newValue)
            {
                throw new TestValidationException(string.Format(
                    "Row Header did not update when the data source was updated.  Expected: {0}, Actual: {1}",
                    newValue,
                    actualValue
                    ));
            }

            LogComment("TestRowHeaderBindingUpdate was successful");
            return TestResult.Pass;
        }

        #endregion Test Steps

        private void Debug()
        {
            // To keep this thread busy, we'll have to push a frame.
            DispatcherFrame frame = new DispatcherFrame();

            debugButton.MouseLeftButtonDown += (sender, e) =>
            {
                frame.Continue = false;
            };

            Dispatcher.PushFrame(frame);
        }
    }
}
