using System.Windows.Controls;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

using Avalon.Test.ComponentModel;
using System.Windows.Data;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Regression test : Verify that DataGridTemplateColumn.CellTemplateSelector is being called
    /// for every data item in the DataGrid.
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridRegressionTest23", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DataGridRegressionTest23 : DataGridTest
    {
        #region Private Data

        private TemplateColumnDataTemplateSelector templateSelector;

        #endregion Private Data

        #region Constructor

        public DataGridRegressionTest23()
            : base(@"DataGridRegressionTest23.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestCellTemplateSelector);
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

            Status("Setup specific for DataGridRegressionTest23");           

            this.SetupDataSource();

            templateSelector = (TemplateColumnDataTemplateSelector)RootElement.FindResource("TemplateSelector");
            Assert.AssertTrue("TemplateColumnDataTemplateSelector must exist in order to continue.", templateSelector != null);

            LogComment("Setup for DataGridRegressionTest23 was successful");
            return TestResult.Pass;
        }

        public override TestResult CleanUp()
        {
            templateSelector = null;

            return base.CleanUp();
        }

        /// <summary>
        /// 
        /// </summary>
        private TestResult TestCellTemplateSelector()
        {
            Status("TestCellTemplateSelector");

            foreach (object item in MyDataGrid.Items)
            {
                MyDataGrid.ScrollIntoView(item);
                QueueHelper.WaitTillQueueItemsProcessed();
            }

            LogComment("verify every item was passed to the CellTemplateSelector for the TemplateColumn");
            foreach (object item in MyDataGrid.Items)
            {
                if (this.templateSelector.ItemsFromSelectTemplate.IndexOf(item) == -1 && item != CollectionView.NewItemPlaceholder)
                {
                    throw new TestValidationException(string.Format(
                        "item: {0}, was not received by the template column's template selector.", 
                        item));
                }
            }

            LogComment("TestCellTemplateSelector was successful");
            return TestResult.Pass;
        }                

        #endregion Test Steps      
    }
}
