using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;


namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Tests for displayIndex when initially set in xaml.
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridColumnReorderingDisplayIndexInXaml", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords = "Localization_Suite")] 
    public class DataGridColumnReorderingDisplayIndexInXaml : DataGridTest
    {
        #region Constructor

        public DataGridColumnReorderingDisplayIndexInXaml()
            : base(@"DataGridDisplayIndexInXaml.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestInitialDisplayOrder);            
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

            Status("Setup specific for DataGridColumnReorderingDisplayIndexInXaml");

            this.SetupDataSource();

            LogComment("Setup for DataGridColumnReorderingDisplayIndexInXaml was successful");
            return TestResult.Pass;
        }

        private TestResult TestInitialDisplayOrder()
        {
            Status("TestInitialDisplayOrder");

            // hard coded verification based on xaml
            string[] expectedColumnOrder = new string[] { "Column0", "Column1", "Column2", "Column3", "Column4", "Column5" };
            int[] expectedDisplayOrder = new int[] { 2, 5, 0, 3, 1, 4 };

            // verify the internal column order
            for (int i = 0; i < expectedColumnOrder.Length; i++)
            {
                DataGridColumn expectedColumn = (DataGridColumn)MyDataGrid.FindName(expectedColumnOrder[i]);
                if (expectedColumn != MyDataGrid.Columns[i])
                {
                    LogComment(string.Format("Column order is incorrect. Expecting: {0}, Actual: {1}", expectedColumn.Header, MyDataGrid.Columns[i].Header));
                    return TestResult.Fail;
                }
            }

            // verify the display order
            for (int i = 0; i < expectedDisplayOrder.Length; i++)
            {
                if (MyDataGrid.Columns[i].DisplayIndex != expectedDisplayOrder[i])
                {
                    LogComment(string.Format("Column display order is incorrect. Expecting: {0}, Actual: {1}", expectedDisplayOrder[i], MyDataGrid.Columns[i].DisplayIndex));
                    return TestResult.Fail;
                }
            }

            DataGridVerificationHelper.VerifyDisplayIndices(MyDataGrid);            

            LogComment("TestInitialDisplayOrder was successful");
            return TestResult.Pass;
        }

        #endregion Test Steps
    }
}
