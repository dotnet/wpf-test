using System.Windows;
using System.Windows.Controls;
using Avalon.Test.ComponentModel;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;


namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Regression test : Verify when retemplating a NewItemPlaceHolder through
    /// a RowStyleSelector, no assertion is thrown when the row is loaded.
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridRegressionTest25", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DataGridRegressionTest25 : DataGridTest
    {
        #region Constructor

        public DataGridRegressionTest25()
            : base(@"DataGridRegressionTest25.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestNewItemPlaceHolderTemplate);
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

            Status("Setup specific for DataGridRegressionTest25");

            Style defaultRowStyle = (Style)RootElement.FindResource("dataGridRowStyle");
            Assert.AssertTrue("defaultRowStyle is not defined", defaultRowStyle != null);
            Style newItemRowStyle = (Style)RootElement.FindResource("newItemRowStyle");
            Assert.AssertTrue("newItemRowStyle is not defined", newItemRowStyle != null);

            RowStyleSelector rowStyleSelector = new RowStyleSelector();
            rowStyleSelector.DefaultStyle = defaultRowStyle;
            rowStyleSelector.NewItemStyle = newItemRowStyle;
            MyDataGrid.RowStyleSelector = rowStyleSelector;

            this.SetupDataSource();

            LogComment("Setup for DataGridRegressionTest25 was successful");
            return TestResult.Pass;
        }
                
        private TestResult TestNewItemPlaceHolderTemplate()
        {
            Status("TestNewItemPlaceHolderTemplate");

            // scroll to the bottom
            MyDataGrid.ScrollIntoView(MyDataGrid.Items[MyDataGrid.Items.Count - 1]);
            QueueHelper.WaitTillQueueItemsProcessed();

            DataGridRow row = DataGridHelper.GetRow(MyDataGrid, MyDataGrid.Items.Count - 1);
            TextBlock tb = DataGridHelper.FindVisualChild<TextBlock>(row);

            if (tb == null || tb.Text != "Click here to add a new item.")
            {
                throw new TestValidationException(string.Format("Row either doesn't contain a TextBlock or the text does not match."));
            }

            LogComment("TestNewItemPlaceHolderTemplate was successful");
            return TestResult.Pass;
        }                

        #endregion Test Steps

        #region Helpers

        private void CleanupHelper()
        {
            MyDataGrid.UnselectAll();
            MyDataGrid.UnselectAllCells();
            Clipboard.Clear();
            QueueHelper.WaitTillQueueItemsProcessed();
        }          
               
        #endregion Helpers
    }
}
