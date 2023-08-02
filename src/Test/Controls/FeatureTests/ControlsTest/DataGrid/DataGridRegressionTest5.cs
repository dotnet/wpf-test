using System.Windows.Controls;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.ApplicationControl;
using System;
using System.Windows;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Automation;
using Microsoft.Test.Input;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Regression Test    
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridRegressionTest5", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DataGridRegressionTest5 : StepsTest
    {
        public const int DefaultTimeoutInMS = 60000;
        private string fileName = "ControlsAutomationTest.exe";
        private string windowClassName = "Microsoft.Test.Controls.DataGridRegressionTest5";

        #region Constructor

        public DataGridRegressionTest5()            
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestColumnHeaderItems);
        }

        #endregion
        
        #region Test Steps

        /// <summary>
        /// Initial Setup  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        public TestResult Setup()
        {
            Status("Setup specific for DataGridRegressionTest4");

                   

            LogComment("Setup for DataGridRegressionTest4 was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 1. start the app        
        /// 
        /// Verify the property ColumnHeaderItems is set correctly on each DataGridCell.
        /// </summary>
        private TestResult TestColumnHeaderItems()
        {
            Status("TestColumnHeaderItems");

            var automatedApp = new InProcessApplication(new WpfInProcessApplicationSettings
            {
                InProcessApplicationType = InProcessApplicationType.InProcessSeparateThread,
                Path = fileName,
                WindowClassName = windowClassName,
                ApplicationImplementationFactory = new WpfInProcessApplicationFactory()
            });

            try
            {
                LogComment("1. Start the app");
                automatedApp.Start();
                automatedApp.WaitForMainWindow(TimeSpan.FromMilliseconds(DefaultTimeoutInMS));              

                var dataGridElement = AutomationElement.RootElement.FindFirst(TreeScope.Descendants | TreeScope.Children, new PropertyCondition(AutomationElement.AutomationIdProperty, "testDataGrid"));
                Assert.AssertTrue("unable to find the DataGrid element", dataGridElement != null);

                // get the DataGridRow elements
                var dataGridRowElements = dataGridElement.FindAll(
                    TreeScope.Children | TreeScope.Descendants,
                    new PropertyCondition(AutomationElement.ClassNameProperty, "DataGridRow"));
                Assert.AssertTrue("unable to find the DataGridRow elements", dataGridRowElements != null && dataGridRowElements.Count > 0);

                // get the DataGridCell elements for the first row
                var dataGridCellElements = dataGridRowElements[0].FindAll(
                    TreeScope.Children | TreeScope.Descendants, 
                    new PropertyCondition(AutomationElement.ClassNameProperty, "DataGridCell"));
                Assert.AssertTrue("unable to find the DataGridCell elements", dataGridCellElements != null && dataGridCellElements.Count > 0);

                int i = 0;
                foreach (AutomationElement dataGridCellElement in dataGridCellElements)
                {
                    var table = dataGridCellElement.GetCurrentPattern(TableItemPattern.Pattern) as TableItemPattern;
                    var columnHeaderItems = table.Current.GetColumnHeaderItems();

                    if (columnHeaderItems == null)
                    {
                        throw new TestValidationException("ColumnHeaderItems for a DataGridCell is returning null.");
                    }

                    LogComment("ColumnHeaderItems length: " + columnHeaderItems.Length);
                    if (columnHeaderItems.Length != 1)
                    {
                        throw new TestValidationException("ColumnHeaderItems count is incorrect.");
                    }                 

                    i++;
                }
            }
            finally
            {
                automatedApp.WaitForInputIdle(TimeSpan.FromSeconds(30));
                automatedApp.Close();
            }

            LogComment("TestColumnHeaderItems was successful");
            return TestResult.Pass;
        }                

        #endregion Test Steps      
    }
}
