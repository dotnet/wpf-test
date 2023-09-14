using System;
using System.Windows;
using System.Windows.Controls;
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
    /// Test press up and down arrow keys to select cells in a DataGrid.
    /// </summary>
    [Test(0, "DataGrid", "DataGridSelectionUpAndDownArrowKeySelect", SecurityLevel = TestCaseSecurityLevel.FullTrust)]    
    public class DataGridSelectionUpAndDownArrowKeySelect : XamlTest
    {
        public DataGridSelectionUpAndDownArrowKeySelect()
            : base(@"DataGridSelection.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(TestUpAndDownArrowKeySelect);
        }

        #region Private Members

        Panel panel;

        private bool PressUpAndDownArrowKeySelect(NavigationKey navigationKey)
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
                        validator.FocusedCell = DataGridHelper.GetCell(dataGrid, dataGrid.Items.Count / 2, 0);
                        UserInput.MouseLeftClickCenter(validator.FocusedCell);
                        QueueHelper.WaitTillQueueItemsProcessed();

                        foreach (ModifierKeys modifierKey in Enum.GetValues(typeof(ModifierKeys)))
                        {
                            validator.ModifierKeys = modifierKey; // set modifier key state.
                            if (!validator.KeyboardNavigationSelect(navigationKey))
                            {
                                LogComment(modifierKey.ToString() + navigationKey.ToString() + " failed.");
                                return false;
                            }
                        }

                        validator.ModifierKeys = ModifierKeys.Ctrl | ModifierKeys.Shift;
                        if (!validator.KeyboardNavigationSelect(navigationKey))
                        {
                            LogComment("Ctrl+Shift " + navigationKey.ToString() + " failed.");
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
        TestResult TestUpAndDownArrowKeySelect()
        {
            LogComment("TestUpAndDownArrowKeySelect started");

            if (!PressUpAndDownArrowKeySelect(NavigationKey.Up))
            {
                LogComment("NavigationKey.Up failed.");
                return TestResult.Fail;
            }

            if (!PressUpAndDownArrowKeySelect(NavigationKey.Down))
            {
                LogComment("NavigationKey.Down failed.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
    }
}
