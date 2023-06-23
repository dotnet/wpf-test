using System;
using System.Collections.Generic;
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
    /// Test directional navigation in datagrid.
    /// </summary>
    [Test(0, "DataGrid", "DataGridKeyboardingDirectionalNavigate", SecurityLevel = TestCaseSecurityLevel.FullTrust)] 
    public class DataGridKeyboardingDirectionalNavigate : XamlTest
    {
        #region Constructor
        
        public DataGridKeyboardingDirectionalNavigate()
            : base(@"DataGridSelection.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(TestKeyboardingDirectionalNavigate);
        }
        
        #endregion

        #region Private Members

        private Panel panel;
        private DataGrid dataGrid;

        private void TestKeyboardNavigationSetup(DataGridSelectionMode dataGridSelectionMode, DataGridSelectionUnit dataGridSelectionUnit)
        {
            panel.Children.Clear();
            Button button = new Button();
            button.Content = "Button";
            panel.Children.Add(button);
            dataGrid = DataGridSelectionTestHelper.CreateDataGrid(dataGridSelectionMode, dataGridSelectionUnit);
            dataGrid.ItemContainerStyle = (Style)panel.FindResource("dataGridRowStyle");
            dataGrid.Height = 500;
            dataGrid.Width = 600;
            panel.Children.Add(dataGrid);
            QueueHelper.WaitTillQueueItemsProcessed();

            UserInput.MouseLeftClickCenter(button);
            QueueHelper.WaitTillQueueItemsProcessed();
        }

        #endregion

        #region Public Methods

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

        public TestResult TestKeyboardingDirectionalNavigate()
        {
            LogComment("TestKeyboardingDirectionalNavigate started");

            foreach (DataGridSelectionMode dataGridSelectionMode in Enum.GetValues(typeof(DataGridSelectionMode)))
            {
                foreach (DataGridSelectionUnit dataGridSelectionUnit in Enum.GetValues(typeof(DataGridSelectionUnit)))
                {
                    TestKeyboardNavigationSetup(dataGridSelectionMode, dataGridSelectionUnit);

                    using (DataGridKeyboardNavigationValidator validator = new DataGridKeyboardNavigationValidator(dataGrid))
                    {
                        validator.ModifierKeys = ModifierKeys.None;

                        foreach (DirectionalNavigationKey directionalNavigationKey in Enum.GetValues(typeof(DirectionalNavigationKey)))
                        {
                            validator.FocusedCell = DataGridHelper.GetCell(dataGrid, dataGrid.Items.Count / 2, dataGrid.Columns.Count / 2);

                            if (!validator.DirectionalNavigate(directionalNavigationKey))
                            {
                                LogComment("Middle directional navigation: " + directionalNavigationKey.ToString() + " failed.");
                                return TestResult.Fail;
                            }
                        }

                        List<DirectionalNavigationTestInfo> directionalNavigationTestInfos = new List<DirectionalNavigationTestInfo>();
                        directionalNavigationTestInfos.Add(new DirectionalNavigationTestInfo(new CellCoordinate(dataGrid.Columns.Count / 2, 0), DirectionalNavigationKey.Up));
                        directionalNavigationTestInfos.Add(new DirectionalNavigationTestInfo(new CellCoordinate(dataGrid.Columns.Count / 2, 0), DirectionalNavigationKey.PageUp));
                        directionalNavigationTestInfos.Add(new DirectionalNavigationTestInfo(new CellCoordinate(dataGrid.Columns.Count / 2, dataGrid.Items.Count - 1), DirectionalNavigationKey.Down));
                        directionalNavigationTestInfos.Add(new DirectionalNavigationTestInfo(new CellCoordinate(dataGrid.Columns.Count / 2, dataGrid.Items.Count - 1), DirectionalNavigationKey.PageDown));
                        directionalNavigationTestInfos.Add(new DirectionalNavigationTestInfo(new CellCoordinate(0, dataGrid.Items.Count / 2), DirectionalNavigationKey.Left));
                        directionalNavigationTestInfos.Add(new DirectionalNavigationTestInfo(new CellCoordinate(0, dataGrid.Items.Count / 2), DirectionalNavigationKey.Home));
                        directionalNavigationTestInfos.Add(new DirectionalNavigationTestInfo(new CellCoordinate(dataGrid.Columns.Count - 1, dataGrid.Items.Count / 2), DirectionalNavigationKey.Right));
                        directionalNavigationTestInfos.Add(new DirectionalNavigationTestInfo(new CellCoordinate(dataGrid.Columns.Count - 1, dataGrid.Items.Count / 2), DirectionalNavigationKey.End));

                        foreach (DirectionalNavigationTestInfo directionalNavigationTestInfo in directionalNavigationTestInfos)
                        {
                            validator.FocusedCell = DataGridHelper.GetCell(dataGrid, directionalNavigationTestInfo.CellCoordinate.Y, directionalNavigationTestInfo.CellCoordinate.X);

                            if (!validator.DirectionalNavigate(directionalNavigationTestInfo.DirectionalNavigationKey))
                            {
                                LogComment("Edge directional navigation: (" + directionalNavigationTestInfo.CellCoordinate.X + "," + directionalNavigationTestInfo.CellCoordinate.Y + ") " + directionalNavigationTestInfo.DirectionalNavigationKey.ToString() + " failed.");
                                return TestResult.Fail;
                            }
                        }
                    }
                }
            }

            return TestResult.Pass;
        }

        #endregion
    }
}
