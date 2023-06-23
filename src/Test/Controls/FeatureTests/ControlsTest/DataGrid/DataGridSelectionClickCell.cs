using System;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Avalon.Test.ComponentModel.Utilities;


namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Test click cell to select cells in a DataGrid.
    /// </summary>
    [Test(0, "DataGrid", "DataGridSelectionClickCell", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DataGridSelectionClickCell : XamlTest
    {
        public DataGridSelectionClickCell()
            : base(@"DataGridSelection.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(ClickCellToSelectDataGridCells);
            RunSteps += new TestStep(TestSelectedUnselectedEvents);
        }

        #region Private Members

        Panel panel;

        private bool ClickCellToSelect(MouseInput mouseInput)
        {
            foreach (DataGridSelectionMode dataGridSelectionMode in Enum.GetValues(typeof(DataGridSelectionMode)))
            {
                foreach (DataGridSelectionUnit dataGridSelectionUnit in Enum.GetValues(typeof(DataGridSelectionUnit)))
                {
                    panel.Children.Clear();
                    DataGrid dataGrid = DataGridSelectionTestHelper.CreateDataGrid(dataGridSelectionMode, dataGridSelectionUnit);
                    panel.Children.Add(dataGrid);

                    using (DataGridSelectionValidator validator = new DataGridSelectionValidator(dataGrid))
                    {
                        int rowIndex = 1;
                        int columnIndex = 1;
                        DataGridCell dataGridCell = default(DataGridCell);

                        foreach (ModifierKeys modifierKey in Enum.GetValues(typeof(ModifierKeys)))
                        {
                            validator.ModifierKeys = modifierKey; // set modifier key state.
                            dataGridCell = DataGridHelper.GetCell(dataGrid, rowIndex++, columnIndex++);
                            if (!validator.MouseSelect(dataGridCell, mouseInput))
                            {
                                LogFailureComment(dataGridSelectionMode, dataGridSelectionUnit, validator);
                                return false;
                            }
                        }

                        validator.ModifierKeys = ModifierKeys.Ctrl | ModifierKeys.Shift;
                        dataGridCell = DataGridHelper.GetCell(dataGrid, rowIndex++, columnIndex++);
                        if (!validator.MouseSelect(dataGridCell, mouseInput))
                        {
                            LogFailureComment(dataGridSelectionMode, dataGridSelectionUnit, validator);
                            return false;
                        }

                        validator.ModifierKeys = ModifierKeys.None;   // restore the modifier key to clean state.
                    }
                }
            }
            return true;
        }

        private void LogFailureComment(DataGridSelectionMode dataGridSelectionMode, DataGridSelectionUnit dataGridSelectionUnit, DataGridSelectionValidator validator)
        {
            LogComment("DataGridSelectionMode: " + dataGridSelectionMode.ToString());
            LogComment("DataGridSelectionUnit: " + dataGridSelectionUnit.ToString());
            LogComment("ModifierKeys: " + validator.ModifierKeys.ToString());
        }

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
            return TestResult.Pass;
        }

        TestResult ClickCellToSelectDataGridCells()
        {
            LogComment("ClickCellToSelectDataGridCells started");

            if (!ClickCellToSelect(MouseInput.LeftClick))
            {
                LogComment("MouseInput.LeftClick failed.");
                return TestResult.Fail;
            }

            if (!ClickCellToSelect(MouseInput.RightClick))
            {
                LogComment("MouseInput.RightClick failed.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private bool isSelectedEventFired = false;
        private bool isUnselectedEventFired = false;

        private TestResult TestSelectedUnselectedEvents()
        {
            Status("TestSelectedUnselectedEvents");

            panel.Children.Clear();
            DataGrid dataGrid = DataGridSelectionTestHelper.CreateDataGrid(DataGridSelectionMode.Single, DataGridSelectionUnit.Cell);
            panel.Children.Add(dataGrid);

            int rowIndex = 1;
            int columnIndex = 1;
            DataGridCell dataGridCell = DataGridHelper.GetCell(dataGrid, rowIndex, columnIndex);

            // add event handlers
            dataGridCell.Selected += dataGridCell_Selected;
            dataGridCell.Unselected += dataGridCell_Unselected;

            DataGridActionHelper.ClickOnCell(dataGrid, rowIndex, columnIndex);
            LogComment(string.Format("after clicking the cell: dataGridCell.IsSelected: {0}", dataGridCell.IsSelected));

            int rowIndex2 = 3;
            int columnIndex2 = 3;
            DataGridActionHelper.ClickOnCell(dataGrid, rowIndex2, columnIndex2);
            LogComment(string.Format("after clicking another cell: dataGridCell.IsSelected: {0}", dataGridCell.IsSelected));

            if (!isSelectedEventFired)
            {
                throw new TestValidationException("Selected event did not fire on DataGridCell");
            }

            if (!isUnselectedEventFired)
            {
                throw new TestValidationException("Unselected event did not fire on DataGridCell");
            }

            // remove event handlers here
            dataGridCell.Selected -= dataGridCell_Selected;
            dataGridCell.Unselected -= dataGridCell_Unselected;

            // click on cells again as a smoke check that nothing bad happened
            DataGridActionHelper.ClickOnCell(dataGrid, rowIndex, columnIndex);
            Assert.AssertTrue("dataGridCell is not selected after clicking the cell", dataGridCell.IsSelected);

            DataGridActionHelper.ClickOnCell(dataGrid, rowIndex2, columnIndex2);
            Assert.AssertTrue("dataGridCell is still selected after clicking the cell", !dataGridCell.IsSelected);

            LogComment("TestSelectedUnselectedEvents was successful");
            return TestResult.Pass;
        }

        private void dataGridCell_Selected(object sender, System.Windows.RoutedEventArgs e)
        {
            isSelectedEventFired = true;
        }

        private void dataGridCell_Unselected(object sender, System.Windows.RoutedEventArgs e)
        {
            isUnselectedEventFired = true;
        }
    }
}
