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
    [Test(0, "DataGrid", "DataGridRegressionTest41", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords = "MicroSuite")]
    public class DataGridRegressionTest41 : StepsTest
    {
        public const int DefaultTimeoutInMS = 60000;
        private string fileName = "ControlsAutomationTest.exe";
        private string windowClassName = "Microsoft.Test.Controls.DataGridRegressionTest5";

        #region Constructor

        public DataGridRegressionTest41()            
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestRepro);
        }

        #endregion
        
        #region Test Steps

        /// <summary>
        /// Initial Setup  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        public TestResult Setup()
        {
            Status("Setup specific for DataGridRegressionTest41");


            LogComment("Setup for DataGridRegressionTest41 was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 1. start the app        
        /// 2. invoke a cell in row 0 to put in edit mode
        /// 3. select the row 0
        /// 4. invoke a different cell in row 0 to put that cell in edit mode
        /// 
        /// Verify row is still selected after step 4
        /// </summary>
        private TestResult TestRepro()
        {
            Status("TestRepro");

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

                LogComment("2. invoke a cell in row 0 to put in edit mode");
                var invokePattern = dataGridCellElements[0].GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                invokePattern.Invoke();

                // verify the cell is in edit mode
                var subElements = dataGridCellElements[0].FindAll(
                    TreeScope.Children | TreeScope.Descendants,
                    new PropertyCondition(AutomationElement.LocalizedControlTypeProperty, "edit"));
                if (subElements == null || subElements.Count <= 0)
                {
                    throw new TestValidationException("Invoking cell 0,0 did not put the cell into edit mode.");
                }

                LogComment("3. select the row 0");
                var selectionPattern = dataGridRowElements[0].GetCurrentPattern(SelectionItemPattern.Pattern) as SelectionItemPattern;
                selectionPattern.Select();

                LogComment("4. invoke a different cell in row 0 to put that cell in edit mode");
                invokePattern = dataGridCellElements[1].GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                invokePattern.Invoke();

                // verify the cell is in edit mode
                subElements = dataGridCellElements[1].FindAll(
                    TreeScope.Children | TreeScope.Descendants,
                    new PropertyCondition(AutomationElement.LocalizedControlTypeProperty, "edit"));
                if (subElements == null || subElements.Count <= 0)
                {
                    throw new TestValidationException("Invoking cell 0,1 did not put the cell into edit mode.");
                }
                
                // Verify row is still selected after step 4
                selectionPattern = dataGridRowElements[0].GetCurrentPattern(SelectionItemPattern.Pattern) as SelectionItemPattern;
                if (!selectionPattern.Current.IsSelected)
                {
                    throw new TestValidationException("Row 0 should still be selected but is not.");
                }
            }
            finally
            {
                automatedApp.WaitForInputIdle(TimeSpan.FromSeconds(30));
                automatedApp.Close();
            }

            LogComment("TestRepro was successful");
            return TestResult.Pass;
        }                

        #endregion Test Steps      
    }
}
