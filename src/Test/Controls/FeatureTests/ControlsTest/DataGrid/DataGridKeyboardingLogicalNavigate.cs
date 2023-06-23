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
    /// Test logical navigate in datagrid.
    /// </summary>
    [Test(0, "DataGrid", "DataGridKeyboardingLogicalNavigate", SecurityLevel = TestCaseSecurityLevel.FullTrust)]  
    public class DataGridKeyboardingLogicalNavigate : XamlTest
    {
        public DataGridKeyboardingLogicalNavigate()
            : base(@"DataGridSelection.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(TestKeyboardingLogicalNavigate);
        }

        #region Private Members

        Panel panel;
        DataGrid dataGrid;

        private void TestKeyboardNavigationSetup(DataGridSelectionMode dataGridSelectionMode, DataGridSelectionUnit dataGridSelectionUnit)
        {
            panel.Children.Clear();
            dataGrid = DataGridSelectionTestHelper.CreateDataGrid(dataGridSelectionMode, dataGridSelectionUnit);
            dataGrid.ItemContainerStyle = (Style)panel.FindResource("dataGridRowStyle");
            dataGrid.Height = 500;
            dataGrid.Width = 600;
            panel.Children.Add(dataGrid);

            Button button = new Button();
            button.Content = "Button";
            panel.Children.Add(button);

            QueueHelper.WaitTillQueueItemsProcessed();
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
            dataGrid = null;
            return TestResult.Pass;
        }

        TestResult TestKeyboardingLogicalNavigate()
        {
            LogComment("TestKeyboardingLogicalNavigate started");

            foreach (DataGridSelectionMode dataGridSelectionMode in Enum.GetValues(typeof(DataGridSelectionMode)))
            {
                foreach (DataGridSelectionUnit dataGridSelectionUnit in Enum.GetValues(typeof(DataGridSelectionUnit)))
                {
                    TestKeyboardNavigationSetup(dataGridSelectionMode, dataGridSelectionUnit);

                    using (DataGridKeyboardNavigationValidator validator = new DataGridKeyboardNavigationValidator(dataGrid))
                    {
                        validator.FocusedCell = DataGridHelper.GetCell(dataGrid, dataGrid.Items.Count - 1, dataGrid.Columns.Count - 1);
                        UserInput.MouseLeftClickCenter(validator.FocusedCell);
                        QueueHelper.WaitTillQueueItemsProcessed();

                        validator.ModifierKeys = ModifierKeys.None;
                        if (!validator.LogicalNavigate())
                        {
                            LogComment(validator.ModifierKeys.ToString() + " failed.");
                            return TestResult.Fail;
                        }

                        validator.FocusedCell = DataGridHelper.GetCell(dataGrid, 0, 0);
                        UserInput.MouseLeftClickCenter(validator.FocusedCell);
                        QueueHelper.WaitTillQueueItemsProcessed();

                        validator.ModifierKeys = ModifierKeys.Shift;
                        if (!validator.LogicalNavigate())
                        {
                            LogComment(validator.ModifierKeys.ToString() + " failed.");
                            return TestResult.Fail;
                        }
                        validator.ModifierKeys = ModifierKeys.None;

                        validator.FocusedCell = DataGridHelper.GetCell(dataGrid, dataGrid.Items.Count / 2, dataGrid.Columns.Count / 2);
                        UserInput.MouseLeftClickCenter(validator.FocusedCell);
                        QueueHelper.WaitTillQueueItemsProcessed();

                        validator.ModifierKeys = ModifierKeys.Ctrl;
                        if (!validator.LogicalNavigate())
                        {
                            LogComment(validator.ModifierKeys.ToString() + " failed.");
                            return TestResult.Fail;
                        }
                        validator.ModifierKeys = ModifierKeys.None;

                        validator.FocusedCell = DataGridHelper.GetCell(dataGrid, dataGrid.Items.Count / 2, dataGrid.Columns.Count / 2);
                        UserInput.MouseLeftClickCenter(validator.FocusedCell);
                        QueueHelper.WaitTillQueueItemsProcessed();

                        validator.ModifierKeys = ModifierKeys.Ctrl | ModifierKeys.Shift;
                        if (!validator.LogicalNavigate())
                        {
                            LogComment(validator.ModifierKeys.ToString() + " failed.");
                            return TestResult.Fail;
                        }
                        validator.ModifierKeys = ModifierKeys.None;
                    }
                }
            }

            return TestResult.Pass;
        }
    }
}
