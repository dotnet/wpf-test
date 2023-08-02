using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Avalon.Test.ComponentModel;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;



namespace Microsoft.Test.Controls
{
    /// <summary>
    /// New tests for DataGridComboBoxColumn new API changes
    /// 
    /// TODOs: the following tests should be considered via exploratory tests time permits
    ///         a. tests w/ CV on/off
    ///         b. test clipboard operations
    ///         c. test 2 custom styles
    ///         d. test enable/disable via checkbox
    ///         e. test cascade combos 
    ///         f. editing w/ keys and cancelling of editing
    ///         g. complex item templates
    /// </summary>

    [Test(0, "DataGrid", "DataGridComboColumnEditingElementBVT", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords = "Localization_Suite,MicroSuite")]
    public class DataGridComboColumnEditingElementBVT : XamlTest
    {
        #region Private Fields

        private DataGrid dataGrid;
        private Page page;
        private NewPeople people;
        private LocalNewPeople localNewPeople;
        private DataGridComboBoxColumn comboCakeText;
        private DataGridComboBoxColumn comboCakeItem;
        private DataGridComboBoxColumn comboCakeDesc;
        private DataGridComboBoxColumn comboCakeDynamic;
        private DataGridComboBoxColumn comboCakeResource;
        private DataGridComboBoxColumn comboCakeEditing;
        private Style cakeElementStyle;
        private BindingBase SelectedValueBinding;
        private BindingBase SelectedItemBinding;
        private BindingBase TextBinding;
        private ComboColumnTestData testData;

        #endregion

        #region Public DS

        public struct ComboColumnTestData
        {
            public string Test;
            public DataGridComboBoxColumn ComboColumn;
            public int RowIndex;
            public int ColumnDisplayIndex;            
            public BindingTarget Binding;
            public string TextContent;
        }

        public enum BindingTarget
        {
            SelectedItemBinding,
            SelectedValueBinding,
            TextBinding,
        }

        #endregion

        #region Constructor

        public DataGridComboColumnEditingElementBVT()
            : base(@"DataGridComboColumnEdtingElementBVT.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(TestStaticText);
            RunSteps += new TestStep(TestStaticSelectedItem);
            RunSteps += new TestStep(TestStaticSelectedValue);
            RunSteps += new TestStep(TestDynamicSelectedValue);
            RunSteps += new TestStep(TestDynamicSelectedItem);
            RunSteps += new TestStep(TestDynamicText);
            RunSteps += new TestStep(TestSorting);
            RunSteps += new TestStep(TestScrolling);    
            RunSteps += new TestStep(TestItemSourceUpdate);
        }

        #endregion

        #region Test Steps

        /// <summary>
        /// Initial setups
        /// </summary>
        /// <returns></returns>
        TestResult Setup()
        {
            Status("Setup");
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            dataGrid = (DataGrid)RootElement.FindName("DataGrid_Standard");
            Assert.AssertTrue("Can not find the DataGrid in the xaml file!", dataGrid != null);

            page = (Page)this.Window.Content;
            people = (NewPeople)(page.FindResource("people"));
            Assert.AssertTrue("Can not find the data source in the xaml file!", people != null);

            localNewPeople = new LocalNewPeople();
            dataGrid.ItemsSource = localNewPeople;
            WaitForPriority(DispatcherPriority.Background);
            
            cakeElementStyle = (Style)page.FindResource("CakeElementStyle");
            Assert.AssertTrue("Can not find the CakeElementStyle in the xaml file!", cakeElementStyle != null);

            comboCakeText = (DataGridComboBoxColumn)dataGrid.FindName("CakeText");                  // TextBinding
            Assert.AssertTrue("Can not find the CakeText column to test", comboCakeText != null);
            comboCakeItem = (DataGridComboBoxColumn)dataGrid.FindName("CakeItem");                  // SelectedItemBinding
            Assert.AssertTrue("Can not find the CakeItem to test", comboCakeItem != null);
            comboCakeDesc = (DataGridComboBoxColumn)dataGrid.FindName("CakeDesc");                  // SelectedValueBinding
            Assert.AssertTrue("Can not find the CakeDesc to test", comboCakeDesc != null);

            comboCakeDynamic = (DataGridComboBoxColumn)dataGrid.FindName("CakeDynamic");            // SelectedValueBinding
            Assert.AssertTrue("Can not find the CakeDynamic to test", comboCakeDynamic != null);
            comboCakeResource = (DataGridComboBoxColumn)dataGrid.FindName("CakeResource");          // SelectedItemBinding
            Assert.AssertTrue("Can not find the CakeResource to test", comboCakeResource != null);
            comboCakeEditing = (DataGridComboBoxColumn)dataGrid.FindName("CakeEditing");            // TextBinding
            Assert.AssertTrue("Can not find the CakeEdting to test", comboCakeEditing != null);

            //dataGrid.SelectionMode = DataGridSelectionMode.Extended;
            //dataGrid.SelectionUnit = DataGridSelectionUnit.CellOrRowHeader;
            //WaitForPriority(DispatcherPriority.Background);

            ResetAllBindings();

            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("Setup was successful");
            return TestResult.Pass;
        }

        TestResult CleanUp()
        {
            dataGrid = null;
            page = null;            
	     people = null;
            cakeElementStyle = null;
            comboCakeText = null;
            comboCakeItem = null;
            comboCakeDesc = null;
            comboCakeDynamic = null;
            comboCakeResource = null;
            comboCakeEditing = null;
            testData.ComboColumn = null;            
            return TestResult.Pass;
        }

        /// <summary>
        /// Test static datasource w/ text binding
        /// </summary>
        /// <returns></returns>
        TestResult TestStaticText() // comboCakeText
        {
            testData = new ComboColumnTestData
            {
                Test = "TestStaticText", 
                ComboColumn = comboCakeText,
                RowIndex = 0,
                ColumnDisplayIndex = comboCakeText.DisplayIndex,                
                Binding = BindingTarget.TextBinding,
                TextContent = "Apple"
            };
            this.ActionAndVerify(testData);

            return TestResult.Pass;
        }

        /// <summary>
        /// Test static datasource w/ selected item binding
        /// </summary>
        /// <returns></returns>
        TestResult TestStaticSelectedItem()  // comboCakeItem
        {
            testData = new ComboColumnTestData
            {
                Test = "TestStaticSelectedItem", 
                ComboColumn = comboCakeItem,
                RowIndex = 1,
                ColumnDisplayIndex = comboCakeItem.DisplayIndex,
                Binding = BindingTarget.SelectedItemBinding,
                TextContent = "Apple"
            };
            this.ActionAndVerify(testData);

            return TestResult.Pass;
        }

        /// <summary>
        /// Test static datasource w/ selected value binding
        /// </summary>
        /// <returns></returns>
        TestResult TestStaticSelectedValue()  // comboCakeDesc
        {
            testData = new ComboColumnTestData
            {
                Test = "TestStaticSelectedValue", 
                ComboColumn = comboCakeDesc,
                RowIndex = 2,
                ColumnDisplayIndex = comboCakeDesc.DisplayIndex,
                Binding = BindingTarget.SelectedValueBinding,
                TextContent = "Sweet"
            };
            this.ActionAndVerify(testData);

            return TestResult.Pass;
        }

        /// <summary>
        /// Test dynamic datasource w/ selected value binding
        /// </summary>
        /// <returns></returns>
        TestResult TestDynamicSelectedValue()  // comboCakeDynamic 
        {           
            testData = new ComboColumnTestData
            {
                Test = "TestDynamicSelectedValue", 
                ComboColumn = comboCakeDynamic,
                RowIndex = 3,
                ColumnDisplayIndex = comboCakeDynamic.DisplayIndex,
                Binding = BindingTarget.SelectedValueBinding,
                TextContent = "1"
            };
            this.ActionAndVerify(testData);

            return TestResult.Pass;
        }

        /// <summary>
        /// Test dynamic datasource w/ selected item binding
        /// </summary>
        /// <returns></returns>
        TestResult TestDynamicSelectedItem()  // comboCakeResource
        {
            testData = new ComboColumnTestData
            {
                Test = "TestDynamicSelectedItem", 
                ComboColumn = comboCakeResource,
                RowIndex = 4,
                ColumnDisplayIndex = comboCakeResource.DisplayIndex,
                Binding = BindingTarget.SelectedItemBinding,
                TextContent = "Apple"
            };
            this.ActionAndVerify(testData);

            return TestResult.Pass;
        }
        
        /// <summary>
        /// Test dynamic datasource w/ text binding
        /// </summary>
        /// <returns></returns>
        TestResult TestDynamicText() // comboCakeEditing 
        {            
            testData = new ComboColumnTestData
            {
                Test = "TestDynamicText", 
                ComboColumn = comboCakeEditing,
                RowIndex = 5,
                ColumnDisplayIndex = comboCakeEditing.DisplayIndex,
                Binding = BindingTarget.TextBinding,
                TextContent = "Apple"
            };
            this.ActionAndVerify(testData);

            return TestResult.Pass;
        }
        
        /// <summary>
        /// Test sorting on the combobox column
        /// </summary>
        /// <returns></returns>
        TestResult TestSorting()
        {
            DoSortAndVerify("TestSorting", comboCakeText);
            return TestResult.Pass;
        }

        /// <summary>
        /// Test scrolling
        /// </summary>
        /// <returns></returns>
        TestResult TestScrolling()
        {
            DoScrollingAndVerify("TestScrolling", comboCakeText);
            return TestResult.Pass;
        }
        
        /// <summary>
        /// Test dynamic itemsource update from EdtingElementStyle
        /// </summary>
        /// <returns></returns>
        TestResult TestItemSourceUpdate()
        {
            Status("TestItemSourceUpdate");

            IEnumerable origItemsSource = comboCakeItem.ItemsSource;
            comboCakeItem.ClearValue(DataGridComboBoxColumn.ItemsSourceProperty);

            comboCakeItem.EditingElementStyle = cakeElementStyle;
            WaitForPriority(DispatcherPriority.Background);

            DataGridCell cell = DataGridHelper.GetCell(dataGrid, 1, comboCakeItem.DisplayIndex);
            cell.IsEditing = true;
            WaitForPriority(DispatcherPriority.Background);
            ComboBox comboBox = (ComboBox)cell.Content;
            Assert.AssertTrue("New ItemsSource from EditingElementStyle did not apply.", ((string)comboBox.Items[3]) == "Cherry");

            cell.IsEditing = false;
            comboCakeItem.ClearValue(DataGridComboBoxColumn.EditingElementStyleProperty);
            comboCakeItem.ItemsSource = origItemsSource;

            LogComment("TestItemSourceUpdate was successful");
            return TestResult.Pass;
        }        

        #endregion

        #region Helpers and Validators 

        /// <summary>
        /// reset all BindingBase variables
        /// </summary>
        private void ResetAllBindings()
        {
            SelectedValueBinding = null;
            SelectedItemBinding = null;
            TextBinding = null;
        }

        /// <summary>
        /// a helper that returns the selection given the binding specified on the combo
        /// </summary>
        /// <param name="comboBox">the combobox to test against</param>
        /// <returns></returns>
        private object GetComboBoxSelectionValue(ComboBox comboBox)
        {
            if (comboBox == null)
            {
                throw new ArgumentNullException();            
            }

            if (SelectedItemBinding != null)
            {
                LogComment("SelectedItemBinding != null");
                return comboBox.SelectedItem;
            }
            else if (SelectedValueBinding != null)
            {
                LogComment("SelectedValueBinding != null");
                return comboBox.SelectedValue;
            }
            else
            {
                LogComment("TextBinding != null");
                return comboBox.Text;
            }
        }

        /// <summary>
        /// Perform test actions and verify related results
        /// </summary>
        /// <param name="testData"></param>
        private void ActionAndVerify(ComboColumnTestData testData)
        {
            LogComment(testData.Test + "...");

            DataGridCell cell;
            ComboBox comboBox;
            int rowIndex = testData.RowIndex;
            int columnIndex = testData.ColumnDisplayIndex;
            
            // verify the binding specified
            switch(testData.Binding)
            {
                case BindingTarget.SelectedItemBinding:
                    SelectedItemBinding = testData.ComboColumn.SelectedItemBinding;
                    Assert.AssertTrue("The SelectedItemBinding should not be null", SelectedItemBinding != null);
                    break;

                case BindingTarget.SelectedValueBinding:
                    SelectedValueBinding = testData.ComboColumn.SelectedValueBinding;
                    Assert.AssertTrue("The SelectedValueBinding should not be null", SelectedValueBinding != null);
                    break;

                case BindingTarget.TextBinding:
                    TextBinding = testData.ComboColumn.TextBinding;
                    Assert.AssertTrue("The TextBinding should not be null", TextBinding != null);
                    break;

                default:
                    break;            
            }
           
            // get the cell
            cell = DataGridHelper.GetCell(dataGrid, rowIndex, columnIndex);
            cell.Focus();

            // get the combo and verify the initial value
            comboBox = (ComboBox)cell.Content;
            string valueToCheck;
            if (SelectedValueBinding != null)
            {
                valueToCheck = (string)comboBox.SelectedValue;
            }
            else
            {
                valueToCheck = comboBox.Text;
            }
            Assert.AssertEqual("The initial text is not correct.", localNewPeople[rowIndex].Cake, valueToCheck);
            LogComment("The origText = " + comboBox.Text);

            // change the selection, verify it and the commit it
            cell.IsEditing = true;
            WaitForPriority(DispatcherPriority.Background);
            
            comboBox = (ComboBox)cell.Content;
            comboBox.Focus();
            WaitForPriority(DispatcherPriority.Background);

            comboBox.SelectedIndex = 0;
            WaitForPriority(DispatcherPriority.Background);

            object obj = GetComboBoxSelectionValue(comboBox);
            // hard-coded here for the local tests
            string valueExpected;
            if (testData.ComboColumn == comboCakeDesc)
            {
                valueExpected = ((CakeData)((ArrayList)comboBox.ItemsSource)[0]).Kind;
            }
            else if ((testData.ComboColumn == comboCakeDynamic) || (testData.ComboColumn == comboCakeEditing))
            {
                valueExpected = ((CakeType)((CakeChoices)comboBox.ItemsSource)[0]).Cake;
            }
            else
            {
                valueExpected = ((ArrayList)comboBox.ItemsSource)[0].ToString();
            }
            Assert.AssertEqual("The selection from the call is not correct.", valueExpected, obj.ToString());
            LogComment("The selection from the call is " + obj.ToString());

            // clicking the cell and pressing enter twice is necessary to close the edit box
            UserInput.MouseLeftClickCenter(comboBox);
            QueueHelper.WaitTillQueueItemsProcessed();

            UserInput.KeyPress(System.Windows.Input.Key.Enter, true);  
            WaitForPriority(DispatcherPriority.Background);

            UserInput.KeyPress(System.Windows.Input.Key.Enter, true);
            WaitForPriority(DispatcherPriority.Background);

            // and check it again
            cell.IsEditing = false;
            WaitForPriority(DispatcherPriority.Background);
            cell = DataGridHelper.GetCell(dataGrid, rowIndex, columnIndex);            
                                   
            comboBox = (ComboBox)cell.Content;
            WaitForPriority(DispatcherPriority.Background);

            WaitForPriority(DispatcherPriority.Background);
            LogComment(string.Format("The new selection = {0} and the expected = {1}", comboBox.Text, testData.TextContent));
            Assert.AssertEqual("The new selection did not apply.", testData.TextContent, comboBox.Text);

            // cleanup
            ResetAllBindings();

            LogComment(testData.Test + " was successful.");
        }       

        /// <summary>
        /// Sort on a given combobox column, and verify the existing selections are staying and sorted
        /// </summary>
        /// <param name="test"></param>
        /// <param name="column"></param>
        private void DoSortAndVerify(string test, DataGridComboBoxColumn column)
        {
            PrintHeader(test);
            if (column == null || string.IsNullOrEmpty(test))
            {
                throw new ArgumentNullException();                
            }

            DataGridColumnHeader header = DataGridHelper.GetColumnHeader(dataGrid, column.DisplayIndex);
            if (header != null)
            {
                UserInput.MouseLeftClickCenter(header);
                WaitForPriority(DispatcherPriority.Background);

                if (!CheckIfColumnValid(column.DisplayIndex, column.SortDirection))
                {
                    throw new TestValidationException("The cake column tested is not sorted");
                }
            }

            PrintFooter(test);
        }

        /// <summary>
        /// Check if all values in a given column are exist as expected, and whether or not they are
        /// sorted as per the sortDirection specified
        /// </summary>
        /// <param name="columnDisplayIndex">the column to test against</param>
        /// <param name="sortDirection">sort direction if also test sorting</param>
        /// <returns>true if all is well, false otherwise</returns>
        private bool CheckIfColumnValid(int columnDisplayIndex, Nullable<ListSortDirection> sortDirection)
        {
            int index = 1;
            DataGridCell cell1 = DataGridHelper.GetCell(dataGrid, 0, columnDisplayIndex);
            DataGridCell cell2 = DataGridHelper.GetCell(dataGrid, 1, columnDisplayIndex);

            while (true)
            {
                if (cell2 == null)
                {
                    break;
                }
                ComboBox comboBox1 = (ComboBox)cell1.Content;
                ComboBox comboBox2 = (ComboBox)cell2.Content;

                if (string.IsNullOrEmpty(comboBox1.Text) || string.IsNullOrEmpty(comboBox2.Text))
                {
                    LogComment(" index = " + index.ToString() + ". Some value in the column is null!");
                    return false;
                }
                else
                {
                    if (sortDirection != null)
                    {
                        if (sortDirection == ListSortDirection.Ascending)
                        {
                            if (comboBox1.Text.CompareTo(comboBox2.Text) > 0)
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (comboBox1.Text.CompareTo(comboBox2.Text) < 0)
                            {
                                return false;
                            }
                        }
                    }
                }

                cell1 = cell2;
                ++index;
                if (index < dataGrid.Items.Count - 1)
                {
                    cell2 = DataGridHelper.GetCell(dataGrid, index, columnDisplayIndex);
                }
                else
                {
                    cell2 = null; 
                }
            }
            return true;
        }

        /// <summary>
        /// Scoll the datagrid up/down and make sure the combobox selections are refreshed
        /// </summary>
        /// <param name="test"></param>
        /// <param name="column"></param>
        private void DoScrollingAndVerify(string test, DataGridComboBoxColumn column)
        {
            PrintHeader(test);

            double origHeight = dataGrid.Height;
            dataGrid.Height = origHeight / 2;

            ScrollViewer scrollViewer = DataGridHelper.FindVisualChild<ScrollViewer>(dataGrid);
            if (scrollViewer == null)
            {
                throw new TestValidationException("Cannot find the scroll viewer!");
            }

            scrollViewer.ScrollToBottom();
            WaitForPriority(DispatcherPriority.Background);
            scrollViewer.ScrollToTop();
            WaitForPriority(DispatcherPriority.Background);

            if (!CheckIfColumnValid(column.DisplayIndex, null))
            {
                throw new TestValidationException("Some value of the cake column are missing!");
            }

            PrintFooter(test);
        }

        private void PrintHeader(string test)
        {
            LogComment(test + "...");
        }
        private void PrintFooter(string test)
        {
            LogComment(test + " was successful.");
        }

        #endregion
    }

    /// <summary>
    /// This is a local copy of the test data that are designed for the combo column tests
    /// </summary>
    public class LocalNewPeople : ObservableCollection<Person>
    {
        public LocalNewPeople()
        {
            Add(new Person("George", string.Empty, "Washington", true, "Chocolate", new DateTime(1999, 1, 26)));
            Add(new Person("John", string.Empty, "Adams", false, "Vanilla", new DateTime(2000, 2, 26)));
            Add(new Person("Thomas", string.Empty, "Jefferson", true, "Apple", new DateTime(2001, 3, 26)));
            Add(new Person("James", string.Empty, "Madison", false, "Chocolate", new DateTime(2002, 4, 26)));
            Add(new Person("James", string.Empty, "Monroe", true, "Chocolate", new DateTime(2003, 5, 26)));
            Add(new Person("John", "Quincy", "Adams", false, "Vanilla", new DateTime(2004, 6, 26)));
            Add(new Person("Andrew", string.Empty, "Jackson", true, "Chocolate", new DateTime(2005, 7, 26)));
            Add(new Person("Martin", string.Empty, "Van Buren", false, "Apple", new DateTime(2006, 8, 26)));
            Add(new Person("William", "H.", "Harrison", true, "Chocolate", new DateTime(2007, 9, 26)));
            Add(new Person("John", string.Empty, "Tyler", true, "Chocolate", new DateTime(2008, 10, 26)));
        }
    }
}
