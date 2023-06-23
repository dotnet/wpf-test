using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
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
    /// Corner Headers BVT Tests
    ///     Currently we only have Selection All basic functionality to test on; more corner header implementation
    ///     will be done in M4, and we will add coverage as needed in M4/M5. 
    /// 
    /// 1. SelectAll using the Corner Header - pre-conditions: 
    ///     SelectionMode == DataGridSelectionMode.Extended
    ///             
    /// 2. Others to be added in M4/M5  
    /// 




    [Test(0, "DataGrid", "DataGridCornerHeadersBVT", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DataGridCornerHeadersBVT : XamlTest
    {
        #region Private Fields

        DataGrid dataGrid;
        Button button;
        bool isCommandExectued;

        #endregion

        #region Constructor

        public DataGridCornerHeadersBVT()
            : base(@"DataGridCornerHeadersBVT.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(TestSelectionAllByCornerHeader); 
            RunSteps += new TestStep(TestSelectionAllByAPIs);
            //RunSteps += new TestStep(TestSelectionAllInvalidCondition);
        }

        #endregion

        #region TestSteps

        TestResult Setup()
        {
            Status("Setup");
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            dataGrid = (DataGrid)RootElement.FindName("DataGrid_Standard");
            Assert.AssertTrue("Can not find the DataGrid in the xaml file!", dataGrid != null);

            dataGrid.CommandBindings.Add(new CommandBinding(DataGrid.SelectAllCommand, DG_SelectAllExecuted));
            isCommandExectued = false;

            button = DataGridHelper.FindVisualChild<Button>(dataGrid);
            Assert.AssertTrue("Can not find the top corner header in the DataGrid!", button != null);
            
            LogComment("Setup was successful");
            return TestResult.Pass;            
        }

        TestResult CleanUp()
        {
            dataGrid = null;
            button = null;
            return TestResult.Pass;
        }

        /// <summary>
        /// Tests for the SelectAll button results 
        /// Add the selection unit conditions to be more explicit
        /// </summary>
        /// <returns></returns>
        TestResult TestSelectionAllByCornerHeader()
        {
            Status("TestSelectionAllByCornerHeader");

            // precondition 
            dataGrid.SelectionMode = DataGridSelectionMode.Extended;
            WaitForPriority(DispatcherPriority.Background);

            // set to FullRow and validate
            dataGrid.SelectionUnit = DataGridSelectionUnit.FullRow;
            WaitForPriority(DispatcherPriority.Background);
            DoSelectAll(button);
            ValidateSelection("By the Corner Header - FullRow", true, true);

            // set to CellOrRowHeader and validate
            dataGrid.SelectionUnit = DataGridSelectionUnit.CellOrRowHeader;
            WaitForPriority(DispatcherPriority.Background);
            DoSelectAll(button);
            ValidateSelection("By the Corner Header - CellOrRowHeader", true, true);

            // set to Cell and validate
            dataGrid.SelectionUnit = DataGridSelectionUnit.Cell;
            WaitForPriority(DispatcherPriority.Background);
            DoSelectAll(button);
            ValidateSelection("By the Corner Header - Cell", false, true);

            LogComment("TestSelectionAllByCornerHeader was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// the apis were called in the test above, this is to just to make sure 
        /// the public calls are fine - should.  
        /// </summary>
        /// <returns></returns>
        TestResult TestSelectionAllByAPIs()
        {
            Status("TestSelectionAllByAPIs");

            // precondition 
            dataGrid.SelectionMode = DataGridSelectionMode.Extended;
            WaitForPriority(DispatcherPriority.Background);

            // 1. rows and validate
            dataGrid.SelectionUnit = DataGridSelectionUnit.FullRow;
            WaitForPriority(DispatcherPriority.Background);

            dataGrid.SelectAll();
            WaitForPriority(DispatcherPriority.Background);
            ValidateSelection("By SelectAll API - FullRow", true, false);

            WaitForPriority(DispatcherPriority.Background);

            dataGrid.SelectionUnit = DataGridSelectionUnit.CellOrRowHeader;
            WaitForPriority(DispatcherPriority.Background);

            dataGrid.SelectAll();
            WaitForPriority(DispatcherPriority.Background);
            ValidateSelection("By SelectAll API - CellOrRowHeader", true, false);

            // 2. cells and validate
            dataGrid.SelectionUnit = DataGridSelectionUnit.Cell;
            WaitForPriority(DispatcherPriority.Background);

            dataGrid.SelectAllCells();
            WaitForPriority(DispatcherPriority.Background);
            ValidateSelection("By SelectAllCells API", false, false);

            LogComment("TestSelectionAllByAPIs was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify that SelectAll not enabled for other selection modes 
        /// </summary>
        /// <returns></returns>
        TestResult TestSelectionAllInvalidCondition() 
        {
            Status("TestSelectionAllInvalidCondition");

            dataGrid.SelectionMode = DataGridSelectionMode.Single;
            WaitForPriority(DispatcherPriority.Background);
            Assert.AssertTrue("The SelectAll should be disabled", button.IsEnabled == false);

            LogComment("TestSelectionAllInvalidCondition was successful");
            return TestResult.Pass;
        }

        #endregion

        #region Helpers

        private void DoSelectAll(Button button)
        {
            UserInput.MouseMove(button, 0, 0);
            QueueHelper.WaitTillQueueItemsProcessed();

            UserInput.MouseLeftDown(button);
            QueueHelper.WaitTillQueueItemsProcessed();

            UserInput.MouseLeftUp(button);
            QueueHelper.WaitTillQueueItemsProcessed();
        }

        private void ValidateSelection(string scope, bool rowUnit, bool isCommand)
        {
            Status(scope);
            IList rowList = dataGrid.SelectedItems;;

            // verify the command
            if (isCommand)
            {
                Assert.AssertTrue("The SelectAll should have been executed.", isCommandExectued == true);
            }
            // reset
            isCommandExectued = false; 

            // check the rows
            if (rowUnit)
            {
                Assert.AssertTrue("No row was selected!", rowList.Count > 0);            
            }

            // check the cells
            IList<DataGridCellInfo> list = dataGrid.SelectedCells;
            Assert.AssertTrue("No cell was selected!", list.Count > 0);

            IList items = dataGrid.Items;
            int numColumns = dataGrid.Columns.Count;
            IEnumerator enumerator = ((IEnumerable)items).GetEnumerator();
            while (enumerator.MoveNext())
            {
                object rowItem = enumerator.Current;
                if (rowUnit)
                {
                    Assert.AssertTrue("The row should have been selected.", rowList.Contains(rowItem));
                }

                for (int i = 0; i < numColumns; i++)
                {
                    DataGridColumn column = dataGrid.Columns[i];
                    DataGridCellInfo cellinfo = new DataGridCellInfo(rowItem, column);
                    Assert.AssertTrue("The cell should have been selected.", list.Contains(cellinfo));
                }
            }        
        }

        private void DG_SelectAllExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)sender;
            if (dataGrid.SelectionUnit == DataGridSelectionUnit.Cell)
            {
                dataGrid.SelectAllCells();
            }
            else
            {
                dataGrid.SelectAll();
            }
            isCommandExectued = true;
            e.Handled = true;
        }

        #endregion

    }
}
