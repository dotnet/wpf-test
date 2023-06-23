using System;
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
    /// Test press up and down arrow keys to select cells in a DataGrid.
    /// </summary>
    [Test(0, "DataGrid", "DataGridSelectionHomeAndEndArrowKeySelect", SecurityLevel = TestCaseSecurityLevel.FullTrust)]  
    public class DataGridSelectionHomeAndEndArrowKeySelect : XamlTest
    {
        public DataGridSelectionHomeAndEndArrowKeySelect()
            : base(@"DataGridSelection.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(TestHomeAndEndArrowKeySelect);
        }

        #region Private Members

        Panel panel;

        private bool PressHomeAndEndArrowKeySelect(NavigationKey navigationKey)
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
                        validator.FocusedCell = DataGridHelper.GetCell(dataGrid, dataGrid.Items.Count / 2, dataGrid.Columns.Count / 2);
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

                            validator.ModifierKeys = ModifierKeys.None;
                        }

                        validator.FocusedCell = DataGridHelper.GetCell(dataGrid, dataGrid.Items.Count / 2, dataGrid.Columns.Count / 2);
                        // Use left and right arrow key to select the focused cell.
                        UserInput.KeyPress(System.Windows.Input.Key.Left.ToString());
                        QueueHelper.WaitTillQueueItemsProcessed();
                        UserInput.KeyPress(System.Windows.Input.Key.Right.ToString());
                        QueueHelper.WaitTillQueueItemsProcessed();

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

        TestResult TestHomeAndEndArrowKeySelect()
        {
            LogComment("TestLeftAndRightArrowKeySelect started");

            if (!PressHomeAndEndArrowKeySelect(NavigationKey.Home))
            {
                LogComment("NavigationKey.Home failed.");
                return TestResult.Fail;
            }

            if (!PressHomeAndEndArrowKeySelect(NavigationKey.End))
            {
                LogComment("NavigationKey.End failed.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
    }
}
