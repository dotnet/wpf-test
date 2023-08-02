using System.Windows.Controls;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.Collections.ObjectModel;
using System;
using Avalon.Test.ComponentModel;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Regression test : Verify that DataGrid does not throw an exception when bound
    /// to a read-only property and the column is set to be read-only.
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridRegressionTest24", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DataGridRegressionTest24 : DataGridTest
    {
        #region Constructor

        public DataGridRegressionTest24()
            : base(@"DataGridRegressionTest24.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestReadOnlyProperty);
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

            Status("Setup specific for DataGridRegressionTest24");

            LogComment("Setup for DataGridRegressionTest24 was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 
        /// </summary>
        private TestResult TestReadOnlyProperty()
        {
            Status("TestReadOnlyProperty");

            Exception exception = null;

            try
            {
                MyDataGrid.ItemsSource = new Customers();
                MyDataGrid.UpdateLayout();
                QueueHelper.WaitTillQueueItemsProcessed();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (exception != null)
            {
                throw new TestValidationException("Exception was fired when setting DataGrid.ItemsSource with a read-only property", exception);
            }
            
            LogComment("TestReadOnlyProperty was successful");
            return TestResult.Pass;
        }                

        #endregion Test Steps      
    }    
}
