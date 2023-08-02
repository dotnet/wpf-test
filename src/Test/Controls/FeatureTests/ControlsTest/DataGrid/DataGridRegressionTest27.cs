using System.Windows.Controls;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows.Threading;
using Microsoft.Test.Input;
using Avalon.Test.ComponentModel;


namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Regression test : Verify that drag selection works properly on the DataGrid when
    /// grouping.
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridRegressionTest27", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords = "MicroSuite")]
    public class DataGridRegressionTest27 : DataGridTest
    {
        private Button debugButton;

        #region Constructor

        public DataGridRegressionTest27()
            : base(@"DataGridRegressionTest27.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestDraggingMultipleRowsWhileGrouped);
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

            Status("Setup specific for DataGridRegressionTest27");

            debugButton = (Button)RootElement.FindName("btn_Debug");
            Assert.AssertTrue("Unable to find btn_Debug from the resources", debugButton != null);

            this.SetupDataSource();
            QueueHelper.WaitTillQueueItemsProcessed();

            // setup the grouping
            CollectionView cv = (CollectionView)CollectionViewSource.GetDefaultView(MyDataGrid.ItemsSource);
            cv.GroupDescriptions.Add(new PropertyGroupDescription("FirstName"));
            cv.SortDescriptions.Add(new SortDescription("FirstName", ListSortDirection.Ascending));
            QueueHelper.WaitTillQueueItemsProcessed();

            //Debug();

            LogComment("Setup for DataGridRegressionTest27 was successful");
            return TestResult.Pass;
        }

        public override TestResult CleanUp()
        {
            debugButton = null;

            return base.CleanUp();
        }

        /// <summary>
        /// 0. Setting grouping on the DataGrid
        /// 1. Drag select the first row and move mouse down a couple rows.
        /// 
        /// Verify: Rows 0 and 1 are selected
        /// </summary>
        private TestResult TestDraggingMultipleRowsWhileGrouped()
        {
            Status("TestDraggingMultipleRows");

            Assert.AssertTrue("DataGrid's selected item count should be zero prior to test.", MyDataGrid.SelectedItems.Count == 0);
            Assert.AssertTrue("DataGrid.SelectionMode needs to be extend to run test.", MyDataGrid.SelectionMode == DataGridSelectionMode.Extended);
            Assert.AssertTrue("Assumes DataGrid.SelectionUnit is not Cell.", MyDataGrid.SelectionUnit != DataGridSelectionUnit.Cell);

            // get the cells to drag to
            DataGridCell cellFrom = DataGridHelper.GetCell(MyDataGrid, 0, 0);
            DataGridCell cellTo = DataGridHelper.GetCell(MyDataGrid, 3, 0);

            // do the drag selection
            UserInput.MouseLeftDown(cellFrom);
            //this.WaitFor(1000);
            QueueHelper.WaitTillQueueItemsProcessed();

            UserInput.MouseMove(cellTo, 0, 0);
            //this.WaitFor(1000);
            QueueHelper.WaitTillQueueItemsProcessed();

            UserInput.MouseLeftUp(cellTo);
            //this.WaitFor(1000);
            QueueHelper.WaitTillQueueItemsProcessed();

            // verify the top 4 rows have been selected
            int expectedCount = 4;
            if (MyDataGrid.SelectedItems.Count != expectedCount)
            {
                throw new TestValidationException(string.Format(
                    "SelectedItems.Count is incorrect.  Expected: {0}, Actual: {1}",
                    expectedCount,
                    MyDataGrid.SelectedItems.Count));
            }

            int counter = 0;
            foreach (object item in MyDataGrid.SelectedItems)
            {
                if (counter >= expectedCount)
                    break;

                if (item != MyDataGrid.Items[counter])
                {
                    throw new TestValidationException(string.Format(
                        "Mismatch in the selected items. Expected index: {0}, Expected item: {1}, Actual: {2}",
                        counter,
                        MyDataGrid.Items[counter],
                        item));                        
                }
                counter++;
            }
            LogComment("TestDraggingMultipleRows was successful");
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
