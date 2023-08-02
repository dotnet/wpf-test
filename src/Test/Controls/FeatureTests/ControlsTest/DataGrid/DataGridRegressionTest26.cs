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
using System.Windows.Data;
using System.ComponentModel;


namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Regression test : Verify that pop-up inside a row details presenter (but physically overlapping elements
    /// outside the presenter receive mouse input.
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridRegressionTest26", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DataGridRegressionTest26 : DataGridTest
    {
        private Button debugButton;

        #region Constructor

        public DataGridRegressionTest26()
            : base(@"DataGridRegressionTest26.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestPopupInRowDetails);
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

            Status("Setup specific for DataGridRegressionTest26");

            debugButton = (Button)RootElement.FindName("btn_Debug");
            Assert.AssertTrue("Unable to find btn_Debug from the resources", debugButton != null);

            this.SetupDataSource();           

            //Debug();

            LogComment("Setup for DataGridRegressionTest26 was successful");
            return TestResult.Pass;
        }

        public override TestResult CleanUp()
        {
            debugButton = null;

            return base.CleanUp();
        }

        /// <summary>
        /// 1. Select a row so the row details becomes visible.
        /// 2. Open the combobox within the row details so a popup opens
        /// 3. click on an iten in the drop down list
        /// 
        /// Verify: item in drop down list is selected and focus stays within the row details
        /// </summary>
        private TestResult TestPopupInRowDetails()
        {
            Status("TestPopupInRowDetails");

            // set selected
            DataGridRow row = DataGridHelper.GetRow(MyDataGrid, MyDataGrid.Items[0]);
            row.IsSelected = true;
            QueueHelper.WaitTillQueueItemsProcessed();

            DataGridCellInfo cellInfo = MyDataGrid.CurrentCell;

            // open the combobox
            DataGridDetailsPresenter detailsPresenter = DataGridHelper.FindVisualChild<DataGridDetailsPresenter>(row);
            ComboBox cb = DataGridHelper.FindVisualChild<ComboBox>(detailsPresenter);

            cb.IsDropDownOpen = true;
            this.WaitFor(1000);
            QueueHelper.WaitTillQueueItemsProcessed();

            // get a reference to one of the comboboxitems
            int expectedIndex = 2;
            ComboBoxItem item = (ComboBoxItem)cb.ItemContainerGenerator.ContainerFromIndex(expectedIndex);

            UserInput.MouseLeftDown(item, 0, 0);
            QueueHelper.WaitTillQueueItemsProcessed();

            UserInput.MouseLeftUp(item);
            QueueHelper.WaitTillQueueItemsProcessed();

            this.WaitFor(1000);

            // verify the combobox item is selected
            if (cb.SelectedIndex != expectedIndex)
            {
                throw new TestValidationException(string.Format(
                    "ComboBoxItem was not selected from the row details view.  Expected index: {0}, Actual Index: {1}",
                    expectedIndex,
                    cb.SelectedIndex));
            }

            // verify current cell is still selected
            if (cellInfo != MyDataGrid.CurrentCell)
            {
                throw new TestValidationException(string.Format(
                    "DataGridCell has changed since the click in row details view.  Expected: ({0},{1}), Actual: ({2},{3})",
                    MyDataGrid.Items.IndexOf(cellInfo.Item),
                    MyDataGrid.Columns.IndexOf(cellInfo.Column),
                     MyDataGrid.Items.IndexOf(MyDataGrid.CurrentCell.Item),
                    MyDataGrid.Columns.IndexOf(MyDataGrid.CurrentCell.Column)));
            }

            LogComment("TestPopupInRowDetails was successful");
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
