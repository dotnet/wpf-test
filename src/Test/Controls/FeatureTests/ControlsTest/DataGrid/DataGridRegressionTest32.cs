using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;


namespace Microsoft.Test.Controls
{
    [Test(0, "DataGrid", "RegressionTest32", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DataGridRegressionTest32 : XamlTest
    {
        private DataGrid dataGrid;

        #region Constructor

        public DataGridRegressionTest32()
            : base("DataGridRegressionTest32.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(TestClickComboBox);            
        }
        #endregion

        #region Test Steps

        public TestResult Setup()
        {
            Status("Setup for DataGridRegressionTest32");
            dataGrid = (DataGrid)RootElement.FindName("dataGrid");
            Assert.AssertTrue("Unable to find datagrid from the resources", dataGrid != null);            
            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            dataGrid = null;
            return TestResult.Pass;
        }

        public TestResult TestClickComboBox()
        {
            dataGrid.ItemsSource = new List<String>() { "M" };
            DispatcherHelper.DoEvents();            
            
            DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(0);
            Assert.AssertTrue("Did not find DG Row", row != null);
            
            DataGridDetailsPresenter presenter = VisualTreeHelper.GetVisualChild<DataGridDetailsPresenter, DataGridRow>(row);
            Assert.AssertTrue("Did not find Row Details Presenter", presenter != null);

            ComboBox myComboBox = VisualTreeHelper.GetVisualChild<ComboBox, DataGridDetailsPresenter>(presenter);
            Assert.AssertTrue("Did not find ComboBox", myComboBox != null);

            //Click on the dataGrid row first to select it
            InputHelper.Click(row);

            //Now Click on the combobox twice to open and close it
            InputHelper.Click(myComboBox);
            DispatcherHelper.DoEvents();            

            InputHelper.Click(myComboBox);            
            DispatcherHelper.DoEvents();

            //Check if the Drop Down closed after 2nd click
            Assert.AssertTrue("ComboBox did not close on second click", myComboBox.IsDropDownOpen == false);            

            return TestResult.Pass;
        }
        
        #endregion
    }
}
