using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;



namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Test click row header to select cells in a DataGrid.
    /// </summary>
    [Test(0, "DataGrid", "DataGridSelectionClickRowHeader", SecurityLevel = TestCaseSecurityLevel.FullTrust)]  
    public class DataGridSelectionClickRowHeader : XamlTest
    {
        public DataGridSelectionClickRowHeader()
            : base(@"DataGridSelection.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(ClickRowHeaderToSelectDataGridCells);
        }

        #region Private Members

        Panel panel;

        private bool ClickRowHeaderToSelect(MouseInput mouseInput)
        {
            foreach (DataGridSelectionMode dataGridSelectionMode in Enum.GetValues(typeof(DataGridSelectionMode)))
            {
                foreach (DataGridSelectionUnit dataGridSelectionUnit in Enum.GetValues(typeof(DataGridSelectionUnit)))
                {
                    panel.Children.Clear();
                    DataGrid dataGrid = DataGridSelectionTestHelper.CreateDataGrid(dataGridSelectionMode, dataGridSelectionUnit);
                    dataGrid.ItemContainerStyle = (Style)panel.FindResource("dataGridRowStyle");
                    panel.Children.Add(dataGrid);

                    using (DataGridSelectionValidator validator = new DataGridSelectionValidator(dataGrid))
                    {
                        int rowIndex = 1;
                        DataGridRowHeader dataGridRowHeader = default(DataGridRowHeader);

                        foreach (ModifierKeys modifierKey in Enum.GetValues(typeof(ModifierKeys)))
                        {
                            validator.ModifierKeys = modifierKey;  // set modifier key state.
                            dataGridRowHeader = DataGridHelper.GetRowHeader(dataGrid, rowIndex);
                            if (!validator.MouseSelect(dataGridRowHeader, mouseInput))
                            {
                                LogComment("Ctrl+LeftClick Row Header " + rowIndex + " failed.");
                                return false;
                            }
                        }

                        validator.ModifierKeys = ModifierKeys.Ctrl | ModifierKeys.Shift;
                        dataGridRowHeader = DataGridHelper.GetRowHeader(dataGrid, ++rowIndex);
                        if (!validator.MouseSelect(dataGridRowHeader, mouseInput))
                        {
                            LogComment("CtrlShift+LeftClick Row Header " + rowIndex + " failed.");
                            return false;
                        }

                        validator.ModifierKeys = ModifierKeys.None;   // restore the modifier key to clean state.
                    }
                }
            }

            return true;
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

        TestResult ClickRowHeaderToSelectDataGridCells()
        {
            LogComment("ClickRowHeaderToSelectDataGridCells started");

            if (!ClickRowHeaderToSelect(MouseInput.LeftClick))
            {
                LogComment("MouseInput.LeftClick failed.");
                return TestResult.Fail;
            }

            if (!ClickRowHeaderToSelect(MouseInput.RightClick))
            {
                LogComment("MouseInput.RightClick failed.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
    }
}
