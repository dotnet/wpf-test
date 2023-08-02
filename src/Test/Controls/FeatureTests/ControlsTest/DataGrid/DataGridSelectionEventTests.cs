using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Avalon.Test.ComponentModel;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;


namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Test DataGrid selection events
    /// </summary>
    [Test(0, "DataGrid", "DataGridSelectionEventTests", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DataGridSelectionEventTests : XamlTest
    {
        public DataGridSelectionEventTests()
            : base(@"DataGridSelectionMode.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(TestDataGridSelectionChanged);
            RunSteps += new TestStep(TestDataGridSelectedCellsChanged);
            RunSteps += new TestStep(TestDataGridCellSelectedAndUnSelected);
            RunSteps += new TestStep(TestDataGridRowSelectedAndUnSelected);
        }

        #region Private Members

        Panel panel;
        DataGrid dataGrid;
        
        #endregion

        public TestResult Setup()
        {
            Status("Setup");

            WaitForPriority(DispatcherPriority.ApplicationIdle);

            panel = (Panel)RootElement.FindName("panel");
            if (panel == null)
            {
                throw new TestValidationException("Panel is null");
            }

            LogComment("Setup was successful");

            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            panel = null;
            dataGrid = null;
	     typeof(EventHelper).InvokeMember("sender", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.SetField, null, null, new object[] { null });
            typeof(EventHelper).InvokeMember("actualEventArgs", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.SetField, null, null, new object[] { null });
            return TestResult.Pass;
        }

        private void TestEventSetup()
        {
            panel.Children.Clear();
            dataGrid = new DataGrid();
            dataGrid.ItemsSource = DataGridBuilder.Construct();
            panel.Children.Add(dataGrid);
        }

        /// <summary>
        /// Test DataGrid SelectionChanged event
        /// </summary>
        /// <returns></returns>
        TestResult TestDataGridSelectionChanged()
        {
            LogComment("TestDataGridSelectionChanged started");

            TestEventSetup();

            SelectionChangedEventArgs actualSelectionChangedEventArgs = null;
            SelectionChangedEventHandler selectionChangedEvent = (sender, args) =>
            {
                actualSelectionChangedEventArgs = args;
            };

            dataGrid.SelectionChanged += selectionChangedEvent;

            // Get the top-left most Cell.
            DataGridCell dataGridCell = DataGridHelper.GetCell(dataGrid, 0, 0);
            if (dataGridCell == null)
            {
                throw new TestValidationException("dataGridCell is null.");
            }

            // Perform action.
            // Select and set focus on the first datagrid row.
            UserInput.MouseLeftClickCenter(dataGridCell);
            QueueHelper.WaitTillQueueItemsProcessed();

            if (actualSelectionChangedEventArgs.AddedItems.Count != 1)
            {
                throw new TestValidationException("AddedItems Count should be 1, but it is " + actualSelectionChangedEventArgs.AddedItems.Count + ".");
            }

            if (actualSelectionChangedEventArgs.RemovedItems.Count != 0)
            {
                throw new TestValidationException("RemovedItems Count should be 0, but it is " + actualSelectionChangedEventArgs.RemovedItems.Count + ".");
            }

            return TestResult.Pass;
        }

        /// <summary>
        /// Test DataGrid SelectedCellsChanged event
        /// </summary>
        /// <returns></returns>
        TestResult TestDataGridSelectedCellsChanged()
        {
            LogComment("TestDataGridSelectedCellsChanged started");

            TestEventSetup();

            dataGrid.SelectionUnit = DataGridSelectionUnit.Cell;

            SelectedCellsChangedEventArgs actualSelectedCellsChangedEventArgs = null;
            SelectedCellsChangedEventHandler selectedCellsChangedEventHandler = (sender, args) =>
            {
                actualSelectedCellsChangedEventArgs = args;
            };
            dataGrid.SelectedCellsChanged += selectedCellsChangedEventHandler;
            QueueHelper.WaitTillQueueItemsProcessed();

            DataGridCell dataGridCell = DataGridHelper.GetCell(dataGrid, 0, 0);
            if (dataGridCell == null)
            {
                throw new TestValidationException("DataGridCell is null.");
            }

            // Select and set focus on the first datagrid row.
            UserInput.MouseLeftClickCenter(dataGridCell);
            QueueHelper.WaitTillQueueItemsProcessed();

            if (actualSelectedCellsChangedEventArgs.AddedCells.Count != 1)
            {
                throw new TestValidationException("AddedCells count should be 1, but it is " + actualSelectedCellsChangedEventArgs.AddedCells.Count + ".");
            }

            if (actualSelectedCellsChangedEventArgs.RemovedCells.Count != 0)
            {
                throw new TestValidationException("RemovedCells count should be 0, but it is " + actualSelectedCellsChangedEventArgs.RemovedCells.Count + ".");
            }

            return TestResult.Pass;
        }

        /// <summary>
        /// Test DataGridCell Selected and UnSelected events.
        /// </summary>
        /// <returns></returns>
        TestResult TestDataGridCellSelectedAndUnSelected()
        {
            LogComment("TestDataGridCellSelectedAndUnSelected started");

            TestEventSetup();

            dataGrid.SelectionUnit = DataGridSelectionUnit.Cell;

            // Select the top-left most cell.
            string eventName = "Selected";
            DataGridCell dataGridCell = DataGridHelper.GetCell(dataGrid, 0, 0);
            if (dataGridCell == null)
            {
                throw new TestValidationException("DataGridCell is null.");
            }

            RoutedEventArgs routedEventArgs = new RoutedEventArgs(DataGridCell.SelectedEvent);
            routedEventArgs.Source = dataGridCell;
            QueueHelper.WaitTillQueueItemsProcessed();

            EventHelper.ExpectEvent<RoutedEventArgs>(
                () =>
                {
                    // Select the first datagrid row.
                    UserInput.MouseLeftClickCenter(dataGridCell);
                    QueueHelper.WaitTillQueueItemsProcessed();
                }, dataGridCell, eventName, routedEventArgs);

            // Unselect the selected cell.
            eventName = "Unselected";
            routedEventArgs = new RoutedEventArgs(DataGridCell.UnselectedEvent);
            routedEventArgs.Source = dataGridCell;
            QueueHelper.WaitTillQueueItemsProcessed();

            EventHelper.ExpectEvent<RoutedEventArgs>(
                () =>
                {
                    // Press Tab key to unselect the selected item.
                    UserInput.KeyDown(System.Windows.Input.Key.Tab.ToString());
                    QueueHelper.WaitTillQueueItemsProcessed();
                }, dataGridCell, eventName, routedEventArgs);

            return TestResult.Pass;
        }

        private bool isSelectedEventFired = false;
        private bool isUnselectedEventFired = false;

        /// <summary>
        /// Test DataGridRow Selected and UnSelected events.
        /// </summary>
        /// <returns></returns>
        TestResult TestDataGridRowSelectedAndUnSelected()
        {
            LogComment("TestDataGridRowSelectedAndUnSelected started");

            TestEventSetup();

            dataGrid.SelectionUnit = DataGridSelectionUnit.FullRow;

            int rowIndex = 1;
            int columnIndex = 1;
            var dataGridRow = DataGridHelper.GetRow(dataGrid, rowIndex);
            dataGridRow.Selected += dataGridRow_Selected;
            dataGridRow.Unselected += dataGridRow_Unselected;

            DataGridActionHelper.ClickOnCell(dataGrid, rowIndex, columnIndex);
            LogComment(string.Format("after clicking the cell: dataGridRow.IsSelected: {0}", dataGridRow.IsSelected));

            int rowIndex2 = 3;
            int columnIndex2 = 3;
            DataGridActionHelper.ClickOnCell(dataGrid, rowIndex2, columnIndex2);
            LogComment(string.Format("after clicking another cell: dataGridRow.IsSelected: {0}", dataGridRow.IsSelected));

            if (!isSelectedEventFired)
            {
                throw new TestValidationException("Selected event did not fire on dataGridRow");
            }

            if (!isUnselectedEventFired)
            {
                throw new TestValidationException("Unselected event did not fire on dataGridRow");
            }

            // remove event handlers here
            dataGridRow.Selected -= dataGridRow_Selected;
            dataGridRow.Unselected -= dataGridRow_Unselected;
           
            return TestResult.Pass;
        }

        private void dataGridRow_Selected(object sender, System.Windows.RoutedEventArgs e)
        {
            isSelectedEventFired = true;
        }

        private void dataGridRow_Unselected(object sender, System.Windows.RoutedEventArgs e)
        {
            isUnselectedEventFired = true;
        }
    }
}
