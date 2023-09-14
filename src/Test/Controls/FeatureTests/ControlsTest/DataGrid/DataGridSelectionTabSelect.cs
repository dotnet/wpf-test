using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;
using Avalon.Test.ComponentModel;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;


namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Test press tab key to select cells in a DataGrid.
    /// </summary>
    [Test(0, "DataGrid", "DataGridSelectionTabSelect", SecurityLevel = TestCaseSecurityLevel.FullTrust)]  
    public class DataGridSelectionTabSelect : XamlTest
    {
        public DataGridSelectionTabSelect()
            : base(@"DataGridSelection.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(TestTabKeySelect);
        }

        #region Private Members

        Panel panel;
        DataGrid dataGrid;

        private void TestScrollableContentSelectionSetup(DataGridSelectionMode selectionMode, DataGridSelectionUnit selectionUnit)
        {
            panel.Children.Clear();
            dataGrid = new DataGrid();

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml("<DataGridDataInfo>" +
            "<DataGridData StringType='one' IntType='1' DoubleType='1.0' BoolType='true' StructType='{null}' />" +
            "<DataGridData StringType='two' IntType='2' DoubleType='2.0' BoolType='false' StructType='Now' />" +
            "<DataGridData StringType='three' IntType='3' DoubleType='3.0' BoolType='true' StructType='{null}' />" +
            "<DataGridData StringType='four' IntType='4' DoubleType='4.0' BoolType='false' StructType='Now' />" +
            "<DataGridData StringType='five' IntType='5' DoubleType='5.0' BoolType='true' StructType='{null}' />" +
            "<DataGridData StringType='six' IntType='6' DoubleType='6.0' BoolType='false' StructType='Now' />" +
            "<DataGridData StringType='seven' IntType='7' DoubleType='7.0' BoolType='true' StructType='{null}' />" +
            "<DataGridData StringType='eight' IntType='8' DoubleType='8.0' BoolType='false' StructType='Now' />" +
            "<DataGridData StringType='nine' IntType='9' DoubleType='9.0' BoolType='true' StructType='{null}' />" +
            "<DataGridData StringType='ten' IntType='10' DoubleType='10.0' BoolType='false' StructType='Now' />" +
            "<DataGridData StringType='eleven' IntType='11' DoubleType='11.0' BoolType='true' StructType='{null}' />" +
            "<DataGridData StringType='twelve' IntType='12' DoubleType='12.0' BoolType='false' StructType='Now' />" +
            "<DataGridData StringType='thirteen' IntType='13' DoubleType='13.0' BoolType='true' StructType='{null}' />" +
            "<DataGridData StringType='fourteen' IntType='14' DoubleType='14.0' BoolType='false' StructType='Now' />" +
            "<DataGridData StringType='fifteen' IntType='15' DoubleType='15.0' BoolType='true' StructType='{null}' />" +
            "<DataGridData StringType='sixteen' IntType='16' DoubleType='16.0' BoolType='false' StructType='Now' />" +
            "<DataGridData StringType='seventeen' IntType='17' DoubleType='17.0' BoolType='true' StructType='{null}' />" +
            "<DataGridData StringType='eighteen' IntType='18' DoubleType='18.0' BoolType='false' StructType='Now' />" +
            "<DataGridData StringType='nineteen' IntType='19' DoubleType='19.0' BoolType='true' StructType='{null}' />" +
            "<DataGridData StringType='twenty' IntType='20' DoubleType='20.0' BoolType='false' StructType='Now' />" +
            "<DataGridData StringType='one' IntType='21' DoubleType='1.0' BoolType='true' StructType='{null}' />" +
            "<DataGridData StringType='two' IntType='22' DoubleType='2.0' BoolType='false' StructType='Now' />" +
            "<DataGridData StringType='three' IntType='23' DoubleType='3.0' BoolType='true' StructType='{null}' />" +
            "<DataGridData StringType='four' IntType='24' DoubleType='4.0' BoolType='false' StructType='Now' />" +
            "<DataGridData StringType='five' IntType='25' DoubleType='5.0' BoolType='true' StructType='{null}' />" +
            "<DataGridData StringType='six' IntType='26' DoubleType='6.0' BoolType='false' StructType='Now' />" +
            "<DataGridData StringType='seven' IntType='27' DoubleType='7.0' BoolType='true' StructType='{null}' />" +
            "<DataGridData StringType='eight' IntType='28' DoubleType='8.0' BoolType='false' StructType='Now' />" +
            "<DataGridData StringType='nine' IntType='29' DoubleType='9.0' BoolType='true' StructType='{null}' />" +
            "<DataGridData StringType='ten' IntType='30' DoubleType='10.0' BoolType='false' StructType='Now' />" +
            "<DataGridData StringType='eleven' IntType='31' DoubleType='11.0' BoolType='true' StructType='{null}' />" +
            "<DataGridData StringType='twelve' IntType='32' DoubleType='12.0' BoolType='false' StructType='Now' />" +
            "<DataGridData StringType='thirteen' IntType='33' DoubleType='13.0' BoolType='true' StructType='{null}' />" +
            "<DataGridData StringType='fourteen' IntType='34' DoubleType='14.0' BoolType='false' StructType='Now' />" +
            "<DataGridData StringType='fifteen' IntType='35' DoubleType='15.0' BoolType='true' StructType='{null}' />" +
            "<DataGridData StringType='sixteen' IntType='36' DoubleType='16.0' BoolType='false' StructType='Now' />" +
            "<DataGridData StringType='seventeen' IntType='37' DoubleType='17.0' BoolType='true' StructType='{null}' />" +
            "<DataGridData StringType='eighteen' IntType='38' DoubleType='18.0' BoolType='false' StructType='Now' />" +
            "<DataGridData StringType='nineteen' IntType='39' DoubleType='19.0' BoolType='true' StructType='{null}' />" +
            "<DataGridData StringType='twenty' IntType='40' DoubleType='20.0' BoolType='false' StructType='Now' />" +
            "<DataGridData StringType='one' IntType='41' DoubleType='1.0' BoolType='true' StructType='{null}' />" +
            "<DataGridData StringType='two' IntType='42' DoubleType='2.0' BoolType='false' StructType='Now' />" +
            "<DataGridData StringType='three' IntType='43' DoubleType='3.0' BoolType='true' StructType='{null}' />" +
            "<DataGridData StringType='four' IntType='44' DoubleType='4.0' BoolType='false' StructType='Now' />" +
            "<DataGridData StringType='five' IntType='45' DoubleType='5.0' BoolType='true' StructType='{null}' />" +
            "<DataGridData StringType='six' IntType='46' DoubleType='6.0' BoolType='false' StructType='Now' />" +
            "<DataGridData StringType='seven' IntType='47' DoubleType='7.0' BoolType='true' StructType='{null}' />" +
            "<DataGridData StringType='eight' IntType='48' DoubleType='8.0' BoolType='false' StructType='Now' />" +
            "<DataGridData StringType='nine' IntType='49' DoubleType='9.0' BoolType='true' StructType='{null}' />" +
            "<DataGridData StringType='ten' IntType='50' DoubleType='10.0' BoolType='false' StructType='Now' />" +
            "<DataGridData StringType='eleven' IntType='51' DoubleType='11.0' BoolType='true' StructType='{null}' />" +
            "<DataGridData StringType='twelve' IntType='52' DoubleType='12.0' BoolType='false' StructType='Now' />" +
            "<DataGridData StringType='thirteen' IntType='53' DoubleType='13.0' BoolType='true' StructType='{null}' />" +
            "<DataGridData StringType='fourteen' IntType='54' DoubleType='14.0' BoolType='false' StructType='Now' />" +
            "<DataGridData StringType='fifteen' IntType='55' DoubleType='15.0' BoolType='true' StructType='{null}' />" +
            "<DataGridData StringType='sixteen' IntType='56' DoubleType='16.0' BoolType='false' StructType='Now' />" +
            "<DataGridData StringType='seventeen' IntType='57' DoubleType='17.0' BoolType='true' StructType='{null}' />" +
            "<DataGridData StringType='eighteen' IntType='58' DoubleType='18.0' BoolType='false' StructType='Now' />" +
            "<DataGridData StringType='nineteen' IntType='59' DoubleType='19.0' BoolType='true' StructType='{null}' />" +
            "<DataGridData StringType='twenty' IntType='60' DoubleType='20.0' BoolType='false' StructType='Now' />" +
            "</DataGridDataInfo>");

            dataGrid.ItemsSource = DataGridBuilder.Construct(xmlDocument);
            dataGrid.Height = 300;
            dataGrid.Width = 300;

            dataGrid.SelectionMode = selectionMode;
            dataGrid.SelectionUnit = selectionUnit;

            panel.Children.Add(dataGrid);

            // Add a button for ctrl+tab and ctrl+shift+tab scenarios because focus rect will get tab out of datagrid.
            Button button = new Button();
            button.Content = "Button";

            panel.Children.Add(button);
        }

        private bool PressTabKeySelect(NavigationKey navigationKey)
        {
            foreach (DataGridSelectionMode dataGridSelectionMode in Enum.GetValues(typeof(DataGridSelectionMode)))
            {
                foreach (DataGridSelectionUnit dataGridSelectionUnit in Enum.GetValues(typeof(DataGridSelectionUnit)))
                {
                    TestScrollableContentSelectionSetup(dataGridSelectionMode, dataGridSelectionUnit);

                    using (DataGridSelectionValidator validator = new DataGridSelectionValidator(dataGrid))
                    {
                        foreach (ModifierKeys modifierKey in Enum.GetValues(typeof(ModifierKeys)))
                        {
                            validator.FocusedCell = DataGridHelper.GetCell(dataGrid, dataGrid.Items.Count / 2, dataGrid.Columns.Count / 2);
                            // Select the focused cell.
                            UserInput.KeyPress(System.Windows.Input.Key.Left.ToString());
                            QueueHelper.WaitTillQueueItemsProcessed();
                            UserInput.KeyPress(System.Windows.Input.Key.Right.ToString());
                            QueueHelper.WaitTillQueueItemsProcessed();

                            validator.ModifierKeys = modifierKey;  // set modifier key state.

                            if (!validator.KeyboardNavigationSelect(navigationKey))
                            {
                                LogComment(modifierKey.ToString() + " " + navigationKey.ToString() + " failed.");
                                return false;
                            }

                            validator.ModifierKeys = ModifierKeys.None;
                        }

                        validator.FocusedCell = DataGridHelper.GetCell(dataGrid, dataGrid.Items.Count / 2, dataGrid.Columns.Count / 2);
                        // Select the focused cell.
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

            Label label = new Label();
            label.Content = "Click me to set focus";
            panel.Children.Add(label);
            QueueHelper.WaitTillQueueItemsProcessed();

            UserInput.MouseLeftClickCenter(label);
            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("Setup was successful");

            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            panel = null;
            dataGrid = null;
            return TestResult.Pass;
        }

        TestResult TestTabKeySelect()
        {
            LogComment("TestTabKeySelect started");

            if (!PressTabKeySelect(NavigationKey.Tab))
            {
                LogComment("NavigationKey.Tab failed.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
    }
}
