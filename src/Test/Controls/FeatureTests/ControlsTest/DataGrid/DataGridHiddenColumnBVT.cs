using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Controls;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;



namespace Microsoft.Test.Controls
{
    /// <summary>
    /// BVT tests for hidden column - focusing on reasonable number of hidden columns:
    ///     - hiding a column is very useful and common in LOB apps, 
    ///         where a PK or such field is needed but no need to be shown.  
    ///     - DataGridColumn.Visibility (Collapsed, Hidden, Visible)
    /// Not in Scope: 
    ///     add/remove items/columns frenquently 
    /// TODOs: refactoring as needed
    /// </summary>
    [Test(0, "DataGrid", "DataGridHiddenColumnBVT", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords = "Localization_Suite,MicroSuite")]
    public class DataGridHiddenColumnBVT : XamlTest
    {
        #region Private Fields

        private DataGrid dataGrid;
        private Page page;
        private NewPeople people;
        private DataGridColumn column;
        private DataGridColumn starColumn;
        private DataGridColumnHeader header;
        private DataGridLength initColumnWidth;
        private bool origCanUserReorderColumns;
        private int origFrozenColumnCount;
        private DataGridClipboardCopyMode origClipboardCopyMode; 
        private DataGridSelectionMode origMode;
        private DataGridSelectionUnit origUnit;
        private bool origAutoGen;
        private HiddenColumnTestData testdata;

        #endregion

        #region Public DS

        /// <summary>
        /// Type reflects the key factors to evaluate
        /// </summary>
        public enum HiddenColumnTestType
        {
            Defaults,
            Basics,
            ColumnReordering,
            ColumnResizing,
            ColumnVirtualization,
            FrozenColumn,
            Selection,
            Keyboarding,
            ClipboardContent,
            ColumnCollectonChange,
            RegressionTest87,
        }

        /// <summary>
        /// Test data
        /// </summary>
        public struct HiddenColumnTestData
        {
            public int OrigColumnDisplayIndex;      // this is also the column index for the cell interested
            public int NewColumnDisplayIndex;       // if not reorderig: -1
            public int RowIndex;                    // row index for the row intereted and cells interested
            public Visibility OrigVisibility;
            public Visibility NewVisibility;
            public int OrigFrozenColumnCount;
            public int NewFrozenColumnCount; 
            public DataGridSelectionMode SelectionMode; 
            public DataGridSelectionUnit SelectionUnit;          
            public DataGridClipboardCopyMode ClipboardCopyMode;
            public HiddenColumnTestType TestType;
        }

        #endregion

        #region Constructor

        public DataGridHiddenColumnBVT()
            : base(@"DataGridHiddenColumnBVT.xaml")
        {
            InitializeSteps += new TestStep(Setup); 
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(TestDefaults);  
            RunSteps += new TestStep(TestBasics);  
            RunSteps += new TestStep(TestFrozenColumn);
            RunSteps += new TestStep(TestColumnVirtualization); 
            RunSteps += new TestStep(TestColumnReordering); 
            RunSteps += new TestStep(TestColumnWidthChange);
            RunSteps += new TestStep(TestSelection);
            RunSteps += new TestStep(TestClipboardContent);
            RunSteps += new TestStep(TestKeyboarding);       
            RunSteps += new TestStep(TestRegressionTest87);   
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
            
            // init
            origCanUserReorderColumns = dataGrid.CanUserReorderColumns; // true
            origFrozenColumnCount = dataGrid.FrozenColumnCount;         // 0
            origMode = dataGrid.SelectionMode;                          // Extended
            origUnit = dataGrid.SelectionUnit;                          // FullRow
            origClipboardCopyMode = dataGrid.ClipboardCopyMode;         // ExcludeHeader
            origAutoGen = dataGrid.AutoGenerateColumns;                 // true

            starColumn = GetColumn(8);
            SetColumnVisibility(starColumn, Visibility.Hidden);

            LogComment("Setup was successful");
            return TestResult.Pass;
        }

        TestResult CleanUp()
        {
            dataGrid = null;
            page = null;
            starColumn = null;
            column = null;
            header = null;            
            return TestResult.Pass;
        }

        /// <summary>
        /// tests for default values
        /// </summary>
        /// <returns></returns>
        TestResult TestDefaults()
        {
            Status("TestDefaults");

            testdata = new HiddenColumnTestData
            {
                OrigColumnDisplayIndex = 0,
                NewColumnDisplayIndex = 0,
                RowIndex = 0,
                OrigVisibility = Visibility.Visible,
                NewVisibility = Visibility.Visible,
                OrigFrozenColumnCount = 0,
                NewFrozenColumnCount = 0,
                SelectionMode = origMode,
                SelectionUnit = origUnit,
                ClipboardCopyMode = origClipboardCopyMode,
                TestType = HiddenColumnTestType.Defaults
            };
            this.ActionAndVerify(testdata);

            LogComment("TestDefaults was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Basic tests for hidden column
        /// </summary>
        /// <returns></returns>
        TestResult TestBasics()
        {
            Status("TestBasics");

            testdata = new HiddenColumnTestData
            {
                OrigColumnDisplayIndex = 0,
                NewColumnDisplayIndex = 0,
                RowIndex = 0,
                OrigVisibility = Visibility.Visible,
                NewVisibility = Visibility.Hidden,
                OrigFrozenColumnCount = 0,
                NewFrozenColumnCount = 0,
                SelectionMode = origMode,
                SelectionUnit = origUnit,
                ClipboardCopyMode = origClipboardCopyMode,
                TestType = HiddenColumnTestType.Basics
            };
            this.ActionAndVerify(testdata);

            LogComment("TestBasics was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Tests for frozencolumn change in conjunction with column collection changes in range
        /// </summary>
        /// <returns></returns>
        TestResult TestFrozenColumn()
        {
            Status("TestFrozenColumn");

            testdata = new HiddenColumnTestData
            {
                OrigColumnDisplayIndex = 1,
                NewColumnDisplayIndex = 1,
                RowIndex = 1,
                OrigVisibility = Visibility.Visible,
                NewVisibility = Visibility.Hidden,
                OrigFrozenColumnCount = 0,
                NewFrozenColumnCount = 4,
                SelectionMode = origMode,
                SelectionUnit = origUnit,
                ClipboardCopyMode = origClipboardCopyMode,
                TestType = HiddenColumnTestType.FrozenColumn
            };
            this.ActionAndVerify(testdata);

            LogComment("TestFrozenColumn was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Tests for CV on/off
        /// </summary>
        /// <returns></returns>
        TestResult TestColumnVirtualization()
        {
            Status("TestColumnVirtualization");

            testdata = new HiddenColumnTestData
            {
                OrigColumnDisplayIndex = 2,
                NewColumnDisplayIndex = 2,
                RowIndex = 2,
                OrigVisibility = Visibility.Visible,
                NewVisibility = Visibility.Hidden,
                OrigFrozenColumnCount = 0,
                NewFrozenColumnCount = 0,
                SelectionMode = origMode,
                SelectionUnit = origUnit,
                ClipboardCopyMode = origClipboardCopyMode,
                TestType = HiddenColumnTestType.ColumnVirtualization
            };
            this.ActionAndVerify(testdata);

            LogComment("TestColumnVirtualization was successful");
            return TestResult.Pass;
        }
        
        /// <summary>
        /// Tests for column reordering
        /// </summary>
        /// <returns></returns>
        TestResult TestColumnReordering()
        {
            Status("TestColumnReordering");

            testdata = new HiddenColumnTestData
            {
                OrigColumnDisplayIndex = 3,
                NewColumnDisplayIndex = 1,
                RowIndex = 3,
                OrigVisibility = Visibility.Visible,
                NewVisibility = Visibility.Collapsed,
                OrigFrozenColumnCount = 0,
                NewFrozenColumnCount = 0,
                SelectionMode = origMode,
                SelectionUnit = origUnit,
                ClipboardCopyMode = origClipboardCopyMode,
                TestType = HiddenColumnTestType.ColumnReordering
            };
            this.ActionAndVerify(testdata);

            LogComment("TestColumnReordering was successful");
            return TestResult.Pass;
        }
        
        /// <summary>
        /// Tests for column width changes in presence of both star and hidden columns 
        /// </summary>
        /// <returns></returns>
        TestResult TestColumnWidthChange()
        {
            Status("TestColumnWidthChange");

            testdata = new HiddenColumnTestData
            {
                OrigColumnDisplayIndex = 7,
                NewColumnDisplayIndex = 7,
                RowIndex = 0,
                OrigVisibility = Visibility.Visible,
                NewVisibility = Visibility.Collapsed,
                OrigFrozenColumnCount = 0,
                NewFrozenColumnCount = 0,
                SelectionMode = origMode,
                SelectionUnit = origUnit,
                ClipboardCopyMode = origClipboardCopyMode,
                TestType = HiddenColumnTestType.ColumnResizing
            };
            this.ActionAndVerify(testdata);

            LogComment("TestColumnWidthChange was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Tests for selections based on selection mode and unit
        /// </summary>
        /// <returns></returns>
        TestResult TestSelection()
        {
            Status("TestSelection");

            testdata = new HiddenColumnTestData
            {
                OrigColumnDisplayIndex = 4,
                NewColumnDisplayIndex = 6,  // this is also used for end column index
                RowIndex = 4,
                OrigVisibility = Visibility.Visible,
                NewVisibility = Visibility.Hidden,
                OrigFrozenColumnCount = 0,
                NewFrozenColumnCount = 0,
                SelectionMode = DataGridSelectionMode.Extended,
                SelectionUnit = DataGridSelectionUnit.CellOrRowHeader,
                ClipboardCopyMode = origClipboardCopyMode,
                TestType = HiddenColumnTestType.Selection
            };
            this.ActionAndVerify(testdata);

            LogComment("TestSelection was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Tests for keyboarding only in visible columns/cells, etc. 
        /// </summary>
        /// <returns></returns>
        TestResult TestKeyboarding()
        {
            Status("TestKeyboarding");

            testdata = new HiddenColumnTestData
            {
                OrigColumnDisplayIndex = 5,
                NewColumnDisplayIndex = 5,
                RowIndex = 5,
                OrigVisibility = Visibility.Visible,
                NewVisibility = Visibility.Hidden,
                OrigFrozenColumnCount = 0,
                NewFrozenColumnCount = 0,
                SelectionMode = origMode,
                SelectionUnit = origUnit,
                ClipboardCopyMode = origClipboardCopyMode,
                TestType = HiddenColumnTestType.Keyboarding
            };
            this.ActionAndVerify(testdata);

            LogComment("TestKeyboarding was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Tests for clipboard contents for hidden column/cells
        /// </summary>
        /// <returns></returns>
        TestResult TestClipboardContent()
        {
            Status("TestClipboardContent");

            testdata = new HiddenColumnTestData
            {
                OrigColumnDisplayIndex = 6,
                NewColumnDisplayIndex = 6,
                RowIndex = 6,
                OrigVisibility = Visibility.Visible,
                NewVisibility = Visibility.Collapsed,
                OrigFrozenColumnCount = 0,
                NewFrozenColumnCount = 0,
                SelectionMode = origMode,
                SelectionUnit = origUnit,
                ClipboardCopyMode = origClipboardCopyMode,
                TestType = HiddenColumnTestType.RegressionTest87
            };
            this.ActionAndVerify(testdata);

            LogComment("TestClipboardContent was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// RegressionTest87 
        /// </summary>
        /// <returns></returns>
        TestResult TestRegressionTest87()
        {
            Status("TestRegressionTest87");

            testdata = new HiddenColumnTestData
            {
                OrigColumnDisplayIndex = dataGrid.Columns.Count - 1,
                NewColumnDisplayIndex = dataGrid.Columns.Count - 1,
                RowIndex = 0,
                OrigVisibility = Visibility.Visible,
                NewVisibility = Visibility.Hidden,
                OrigFrozenColumnCount = 0,
                NewFrozenColumnCount = 0,
                SelectionMode = DataGridSelectionMode.Extended,
                SelectionUnit = DataGridSelectionUnit.CellOrRowHeader,
                ClipboardCopyMode = DataGridClipboardCopyMode.ExcludeHeader,
                TestType = HiddenColumnTestType.RegressionTest87
            };
            this.ActionAndVerify(testdata);

            LogComment("TestRegressionTest87 was successful");
            return TestResult.Pass;
        }

        #endregion

        #region Actions and Verifications

        private void ActionAndVerify(HiddenColumnTestData testData)
        {
            ActionInit(testData.RowIndex, testData.NewColumnDisplayIndex);

            switch (testData.TestType)
            {
                case HiddenColumnTestType.Defaults:
                    VerifyDefaults(testData);
                    break;

                case HiddenColumnTestType.Basics:
                    VerifyBasics(testData);
                    break;

                case HiddenColumnTestType.FrozenColumn:
                    VerifyFrozenColumn(testData);
                    break;

                case HiddenColumnTestType.ColumnReordering:
                    VerifyColumnReordering(testData);
                    break;

                case HiddenColumnTestType.ColumnResizing:
                    VerifyColumnResizing(testData);
                    break;

                case HiddenColumnTestType.ColumnVirtualization:
                    VerifyColumnVirtualization(testData);
                    break;

                case HiddenColumnTestType.Selection:
                    VerifySelection(testData);
                    break;

                case HiddenColumnTestType.Keyboarding:
                    VerifyKeyboarding(testData);
                    break;

                case HiddenColumnTestType.ClipboardContent:
                    VerifyClipboardContent(testData);
                    break;

                case HiddenColumnTestType.RegressionTest87:
                    VerifyRegressionTest87(testData);
                    break;

                default:
                    break;
            }

            // reset all
            ResetAll(column, testData.OrigColumnDisplayIndex, initColumnWidth);
        }

        private void ActionInit(int rowIndex, int initDisplayIndex)
        {
            SetFrozenColumnCount(origFrozenColumnCount);
            dataGrid.FrozenColumnCount = origFrozenColumnCount;
            dataGrid.CanUserReorderColumns = origCanUserReorderColumns;
            WaitForPriority(DispatcherPriority.Background);

            column = GetColumn(initDisplayIndex);
            header = GetHeader(initDisplayIndex);
            initColumnWidth = column.Width;

            DataGridCell cell00 = DataGridHelper.GetCell(dataGrid, 0, 0);
            cell00.Focus();
            int lastColumnIndexInView = DataGridHelper.GetLastVisibileColumnIndex(dataGrid);
            if (initDisplayIndex > lastColumnIndexInView)
            {
                dataGrid.ScrollIntoView(dataGrid.Items[rowIndex], column);
            }
        }

        /// <summary>
        /// local helper to get the column by displayindex from the DataGridHelper
        /// </summary>
        /// <param name="displayIndex"></param>
        /// <returns></returns>
        private DataGridColumnHeader GetHeader(int displayIndex)
        {
            return DataGridHelper.GetColumnHeaderFromDisplayIndex(dataGrid, displayIndex);
        }

        /// <summary>
        /// Verify frozen and hidden properties based on the knowledge of the columns
        /// </summary>
        /// <param name="frozenColumnCount">FrozenColumnCount on the datagrid</param>
        /// <param name="displayIndex">in range hidden column displayindex</param>
        private void VerifyColumns(int frozenColumnCount, int displayIndex)
        {
            for (int i = 0; i < dataGrid.Columns.Count - 1; i++)
            {
                DataGridColumn columnI = GetColumn(i);
                DataGridColumnHeader headerI = GetHeader(i);

                if (i < frozenColumnCount)
                {
                    Assert.AssertTrue(string.Format("The column should be frozen at {0}", i), columnI.IsFrozen == true);

                    if (i == displayIndex)
                    {
                        Assert.AssertTrue(string.Format("The column should be hidden now at {0}", i), columnI.Visibility != Visibility.Visible);
                        Assert.AssertTrue(string.Format("The column header should be null now at {0}", i), headerI == null);
                    }
                    else
                    {
                        Assert.AssertTrue(string.Format("The column should not be hidden at {0}", i), columnI.Visibility == Visibility.Visible);
                        Assert.AssertTrue(string.Format("The column header should not be null at {0}", i), headerI != null);
                    }
                }
                else
                {
                    Assert.AssertTrue(string.Format("The column should not be frozen at {0}", i), columnI.IsFrozen == false);

                    if (i == 8) // star
                    {
                        Assert.AssertTrue(string.Format("The column should be hidden now at {0}", i), columnI.Visibility != Visibility.Visible);
                        Assert.AssertTrue(string.Format("The column header should be null now at {0}", i), headerI == null);
                    }
                    else
                    {
                        Assert.AssertTrue(string.Format("The column should not be hidden at {0}", i), columnI.Visibility == Visibility.Visible);
                        Assert.AssertTrue(string.Format("The column header should not be null at {0}", i), headerI != null);
                    }
                }
            }
        }

        private void ResetAll(DataGridColumn column, int origDisplayIndex, DataGridLength initColumnWidth)
        {
            SetColumnVisibility(column, Visibility.Visible);

            column.DisplayIndex = origDisplayIndex;
            column.Width = initColumnWidth;
            WaitForPriority(DispatcherPriority.Background);

            dataGrid.CanUserReorderColumns = origCanUserReorderColumns;
            dataGrid.FrozenColumnCount = origFrozenColumnCount;

            dataGrid.EnableColumnVirtualization = false;
            WaitForPriority(DispatcherPriority.Background);
        }

        private void AssertColumnVisible(int displayIndex)
        {
            column = GetColumn(displayIndex);
            Assert.AssertTrue("The column should be visible", column.Visibility == Visibility.Visible);
            header = GetHeader(displayIndex);
            Assert.AssertTrue("The header should not be null", header != null);
        }
        private void AssertColumnHidden(int displayIndex)
        {
            column = GetColumn(displayIndex);
            Assert.AssertTrue("The column should be hidden", column.Visibility != Visibility.Visible);
            header = GetHeader(displayIndex);
            Assert.AssertTrue("The header should be null", header == null);

            Assert.AssertTrue("The column should still be in the collection", dataGrid.Columns.Contains(column) == true);
        }

        private void VerifyDefaults(HiddenColumnTestData testData)
        {
            AssertColumnVisible(testData.NewColumnDisplayIndex);
        }

        private void VerifyBasics(HiddenColumnTestData testData)
        {
            LogComment("initially...");
            AssertColumnVisible(testData.OrigColumnDisplayIndex);

            SetColumnVisibility(column, testData.NewVisibility);
            LogComment("after visibility change...");
            AssertColumnHidden(testData.NewColumnDisplayIndex);
        }

        private void VerifyFrozenColumn(HiddenColumnTestData testData)
        {
            // init
            SetFrozenColumnCount(testData.NewFrozenColumnCount);
            SetColumnVisibility(column, testData.NewVisibility);

            // make sure the frozen columns, hidden column, and non-frozen are correct
            VerifyColumns(testData.NewFrozenColumnCount, testData.NewColumnDisplayIndex);

            // add/remove a frozen column and verify 
            column = dataGrid.ColumnFromDisplayIndex(testData.OrigColumnDisplayIndex);
            int hiddenColumnIndex = dataGrid.Columns.IndexOf(column);
            // the next visible column after the range
            DataGridColumn columnNext = dataGrid.ColumnFromDisplayIndex(testData.NewFrozenColumnCount);
            int columnIndex = dataGrid.Columns.IndexOf(columnNext);

            // remove the first column and then add it back
            DataGridColumn column0 = dataGrid.Columns[0];

            dataGrid.Columns.RemoveAt(0);
            WaitForPriority(DispatcherPriority.Background);
            Assert.AssertTrue("The frozencolumncount should not change", dataGrid.FrozenColumnCount == testData.NewFrozenColumnCount);
            Assert.AssertTrue("The total frozen column count should stay the same", DataGridHelper.GetFrozenColumnCount(dataGrid) == dataGrid.FrozenColumnCount);

            // the hidden column should still be hidden
            Assert.AssertTrue("The hidden column should stay hidden", DataGridHelper.IsColumnIndexHidden(dataGrid, hiddenColumnIndex - 1) == true);

            // the next visible column outside the original frozen range should be frozen now
            Assert.AssertTrue("The next visible column should be frozen", DataGridHelper.IsColumnFrozen(dataGrid, columnIndex - 1) == true);

            // add the column back to the same spot
            dataGrid.Columns.Insert(0, column0);
            WaitForPriority(DispatcherPriority.Background);
            Assert.AssertTrue("The frozencolumncount should not change", dataGrid.FrozenColumnCount == testData.NewFrozenColumnCount);

            Assert.AssertTrue("The column0 should be visible", DataGridHelper.IsColumnIndexHidden(dataGrid, 0) == false);
            Assert.AssertTrue("The column0 should be frozen", DataGridHelper.IsColumnFrozen(dataGrid, 0) == true);
            Assert.AssertTrue("The hidden column should stay hidden", DataGridHelper.IsColumnIndexHidden(dataGrid, hiddenColumnIndex) == true);

            // the next visible column should be un-frozen now
            Assert.AssertTrue("The next visible column should be un-frozen", DataGridHelper.IsColumnFrozen(dataGrid, columnIndex) == false);
        }

        private void VerifyColumnReordering(HiddenColumnTestData testData)
        {
            Assert.AssertTrue("The column should allow user reorder", column.CanUserReorder == true);

            column = GetColumn(testData.OrigColumnDisplayIndex);
            SetColumnVisibility(column, testData.NewVisibility);

            LogComment("after visibility change...");
            AssertColumnHidden(testData.OrigColumnDisplayIndex);

            column.DisplayIndex = testData.NewColumnDisplayIndex;
            WaitForPriority(DispatcherPriority.Background);

            LogComment("after displayindex change...");
            AssertColumnHidden(testData.NewColumnDisplayIndex);
        }

        private void VerifyColumnResizing(HiddenColumnTestData testData)
        {
            // test 1. w/o star column 
            LogComment("Test 1...");
            SetColumnVisibility(column, testData.NewVisibility);

            LogComment("after visibility change...");
            AssertColumnHidden(testData.NewColumnDisplayIndex);

            column.Width = 150d;
            WaitForPriority(DispatcherPriority.Background);

            LogComment("after column resize...");
            AssertColumnHidden(testData.NewColumnDisplayIndex);

            // reset
            column.Width = initColumnWidth;
            WaitForPriority(DispatcherPriority.Background);

            // test 2. w/ star column - 
            LogComment("Test 2...");
            SetAutoGen(false);
            SetColumnVisibility(starColumn, Visibility.Visible);

            LogComment("after star reappear...");
            AssertColumnHidden(testData.NewColumnDisplayIndex);

            SetColumnVisibility(starColumn, Visibility.Collapsed);

            LogComment("after star collapsed...");
            AssertColumnHidden(testData.NewColumnDisplayIndex);

            // rest
            SetAutoGen(true);
        }
        
        private void VerifyColumnVirtualization(HiddenColumnTestData testData)
        {   
            LogComment("initially...");
            AssertColumnVisible(testData.OrigColumnDisplayIndex);

            SetColumnVisibility(column, testData.NewVisibility);

            LogComment("after visibility change...");
            AssertColumnHidden(testData.NewColumnDisplayIndex);

            // toggle CV to true
            dataGrid.EnableColumnVirtualization = true;
            WaitForPriority(DispatcherPriority.Background);

            // change the column's display index so it's at the last index
            column.DisplayIndex = dataGrid.Columns.Count - 1;
            WaitForPriority(DispatcherPriority.Background);

            LogComment("after index change...");
            AssertColumnHidden(dataGrid.Columns.Count - 1);
        }

        private void VerifySelection(HiddenColumnTestData testData)
        {
            // init
            SetSelectionModeUnit(testData.SelectionMode, testData.SelectionUnit);

            DataGridCellInfo cellInfo = new DataGridCellInfo(dataGrid.Items[testData.RowIndex], dataGrid.ColumnFromDisplayIndex(testData.OrigColumnDisplayIndex));
            DataGridCell cellCurrent = DataGridHelper.GetCell(cellInfo);
            cellCurrent.Focus();

            column = GetColumn(testData.OrigColumnDisplayIndex);
            SetColumnVisibility(column, testData.NewVisibility);
            AssertColumnHidden(testData.OrigColumnDisplayIndex);

            ClearSelection();           

            // select the row and verify
            SelectRow(testData.RowIndex);
            AssertIsSelected(DataGridHelper.GetRow(dataGrid, testData.RowIndex), true);
            AssertCellsAreSelected(DataGridHelper.GetRow(dataGrid, testData.RowIndex), true);

            // range cells
            SetAutoGen(false);

            cellInfo = new DataGridCellInfo(dataGrid.Items[testData.RowIndex], dataGrid.ColumnFromDisplayIndex(testData.OrigColumnDisplayIndex - 1));
            cellCurrent = DataGridHelper.GetCell(cellInfo);
            cellCurrent.Focus();

            SelectCells(testData.RowIndex, testData.OrigColumnDisplayIndex - 1, testData.NewColumnDisplayIndex);
            AssertIsSelected(dataGrid.SelectedCells, true);

            // local reset
            SetAutoGen(origAutoGen);
            ClearSelection();
            ResetSelectionModeUnit();
        }

        private void VerifyClipboardContent(HiddenColumnTestData testData)
        {
            // init
            SetSelectionModeUnit(testData.SelectionMode, testData.SelectionUnit);
            SetClipboardCopyMode(DataGridClipboardCopyMode.ExcludeHeader);
            SetAutoGen(false);

            ClearSelection();
            Clipboard.Clear();

            // the cell
            DataGridCellInfo cellInfo = new DataGridCellInfo(dataGrid.Items[testData.RowIndex], dataGrid.ColumnFromDisplayIndex(testData.OrigColumnDisplayIndex));
            DataGridCell thecell = DataGridHelper.GetCell(cellInfo);
            LogComment("The cell content = " + ((TextBlock)thecell.Content).Text);

            // set the column hidden
            column = GetColumn(testData.OrigColumnDisplayIndex);
            SetColumnVisibility(column, testData.NewVisibility);
            AssertColumnHidden(testData.OrigColumnDisplayIndex);

            // select the row 
            dataGrid.SelectedItems.Add(dataGrid.Items[testData.RowIndex]);
            dataGrid.UpdateLayout();
            WaitForPriority(DispatcherPriority.Background);

            AssertIsSelected(DataGridHelper.GetRow(dataGrid, testData.RowIndex), true);
            AssertCellsAreSelected(DataGridHelper.GetRow(dataGrid, testData.RowIndex), true);

            // copy to clipboard
            CopySelectionToClipboard();

            // verify the copy
            IDataObject ido = Clipboard.GetDataObject();
            object actualData = (DataObject)ido.GetData(DataFormats.UnicodeText);
            Assert.AssertTrue("The clipboard content should not contain the hidden column data", ((string)actualData).Contains(((TextBlock)thecell.Content).Text) == false);

            // local reset
            ResetSelectionModeUnit();
            ClearSelection();
            Clipboard.Clear();
            SetAutoGen(origAutoGen);
            SetClipboardCopyMode(origClipboardCopyMode);
        }

        private void VerifyKeyboarding(HiddenColumnTestData testData) 
        {
            // init
            SetAutoGen(false);
            SetSelectionModeUnit(DataGridSelectionMode.Single, DataGridSelectionUnit.Cell);
            AssertColumnVisible(testData.OrigColumnDisplayIndex);

            column = GetColumn(testData.OrigColumnDisplayIndex);
            SetColumnVisibility(column, testData.NewVisibility);
            AssertColumnHidden(testData.OrigColumnDisplayIndex);

            // keys and verifications            
            DataGridCell cellCurrent = DataGridHelper.GetCell(dataGrid, testData.RowIndex, 4);
            cellCurrent.Focus();

            LogComment("tab...");            
            KeyVerification("Tab", 6, false, false);
            Assert.AssertTrue("The current cell should be visible", dataGrid.CurrentCell.Column.Visibility == Visibility.Visible);

            KeyVerification("Tab", 4, false, true);

            KeyVerification("Home", 0, false, false);
            KeyVerification("End", dataGrid.Columns.Count - 2, false, false);
            KeyVerification("Right", dataGrid.Columns.Count - 2, false, false);

            KeyVerification("Left", 6, false, false);
            KeyVerification("Left", 4, false, false);
            
            KeyVerification("Left", 0, true, false);
            KeyVerification("Right", dataGrid.Columns.Count - 2, true, false);
        }

        private void KeyVerification(string key, int indexTo, bool isCtrl, bool isShift)
        {
            LogComment(string.Format("Key = {0}, isCtrl = {1}, isShift = {2}, indexTo = {3}", key, isCtrl.ToString(), isShift.ToString(), indexTo));
            if (isCtrl)
            {
                UserInput.KeyDown("LeftCtrl");
            }
            if (isShift)
            {
                UserInput.KeyDown("LeftShift");
            }
            UserInput.KeyDown(key);
            UserInput.KeyUp(key);
            if (isCtrl)
            {
                UserInput.KeyUp("LeftCtrl");
            }
            if (isShift)
            {
                UserInput.KeyUp("LeftShift");
            }
            WaitForPriority(DispatcherPriority.Background);

            // verify 
            Assert.AssertTrue(string.Format("The current cell after {0} should be {1}", key, indexTo), dataGrid.CurrentCell.Column.DisplayIndex == indexTo);
        }

        private void VerifyRegressionTest87(HiddenColumnTestData testData)
        {
            column = GetColumn(testData.OrigColumnDisplayIndex);            
            SetColumnVisibility(column, testData.NewVisibility);
            
            LogComment("after visibility change...");
            AssertColumnHidden(testData.OrigColumnDisplayIndex);
        }

        #endregion

        #region Helpers

        #region Columns

        private DataGridColumn GetColumn(int displayIndex)
        {
            if (displayIndex < 0 || displayIndex > dataGrid.Columns.Count - 1)
            {
                throw new ArgumentOutOfRangeException();
            }
            return dataGrid.ColumnFromDisplayIndex(displayIndex);        
        }

        #endregion
               
        #region Cells

        /// <summary>
        /// This is datasource specific here. It can be refactored to use an IEnumerable source. 
        /// </summary>
        /// <param name="row">row index</param>
        /// <param name="column">column index</param>
        /// <returns></returns>
        private DataGridCellInfo CellInfo(int row, int column)
        {
            return new DataGridCellInfo(people[row], dataGrid.Columns[column]);
        }
        
        private void AddACellToSelection(int row, int column)
        {
            DataGridCellInfo cell = CellInfo(row, column);
            dataGrid.SelectedCells.Add(cell);
            WaitForPriority(DispatcherPriority.Background);
        }

        #endregion

        #region General 

        /// <summary>
        /// Set the visibility of a column given by displayindex
        /// </summary>
        /// <param name="displayIndex"></param>
        /// <param name="visibility"></param>
        private void SetColumnVisibility(int displayIndex, Visibility visibility)
        {
            EnsureColumnIndexValid(displayIndex);

            DataGridColumn column = dataGrid.ColumnFromDisplayIndex(displayIndex);
            SetColumnVisibility(column, visibility);
        }

        /// <summary>
        /// Set the visibility of a given column
        /// </summary>
        /// <param name="column"></param>
        /// <param name="visibility"></param>
        private void SetColumnVisibility(DataGridColumn column, Visibility visibility)
        {
            if (column == null)
            {
                throw new ArgumentOutOfRangeException("column", "param column should not be null");
            }
            column.Visibility = visibility;
            WaitForPriority(DispatcherPriority.Background);
        }

        /// <summary>
        /// Update the FrozenColumnCount on the datagrid
        /// </summary>
        /// <param name="count">frozen column count to set</param>
        private void SetFrozenColumnCount(int count)
        { 
            if (count < 0 || count > dataGrid.Columns.Count - 1)
            {
                throw new ArgumentOutOfRangeException();
            }
            
            dataGrid.FrozenColumnCount = count;
            WaitForPriority(DispatcherPriority.Background);
        }

        /// <summary>
        /// Set the AutoGenerateColumns property
        /// </summary>
        /// <param name="IsAutoGen"></param>
        private void SetAutoGen(bool IsAutoGen)
        {
            dataGrid.AutoGenerateColumns = IsAutoGen;
            WaitForPriority(DispatcherPriority.Background);
        }

        /// <summary>
        /// Set the ClipboardCopyMode property
        /// </summary>
        /// <param name="mode"></param>
        private void SetClipboardCopyMode(DataGridClipboardCopyMode mode)
        {
            dataGrid.ClipboardCopyMode = mode;
            WaitForPriority(DispatcherPriority.Background);
        }

        /// <summary>
        /// Set the Selection Mode and Unit properties 
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="unit"></param>
        private void SetSelectionModeUnit(DataGridSelectionMode mode, DataGridSelectionUnit unit)
        {
            dataGrid.SelectionMode = mode;
            dataGrid.SelectionUnit = unit;
            WaitForPriority(DispatcherPriority.Background);
        }
        private void ResetSelectionModeUnit()
        {
            SetSelectionModeUnit(origMode, origUnit);
        }

        private void EnsureRowIndexValid(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex > dataGrid.Items.Count - 1)
            {
                throw new ArgumentOutOfRangeException();
            }
        }
        private void EnsureColumnIndexValid(int columnIndex)
        {
            if (columnIndex < 0 || columnIndex > dataGrid.Columns.Count - 1)
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Select a row by index
        /// </summary>
        /// <param name="rowIndex"></param>
        private void SelectRow(int rowIndex)
        {
            // when ran with automation it does not select the first time so
            // repeating the selection again (note this does not repro manually)
            DataGridActionHelper.SelectRow(dataGrid, rowIndex, false, false);
            DataGridActionHelper.SelectRow(dataGrid, rowIndex, false, false);

            dataGrid.UpdateLayout();
            WaitForPriority(DispatcherPriority.Background);

            AssertIsSelected(DataGridHelper.GetRow(dataGrid, rowIndex), true);
            CheckListCount(dataGrid.SelectedItems, 1);
        }

        private void SelectCells(int rowIndex, int minColumnIndex, int maxColumnIndex)
        {
            EnsureRowIndexValid(rowIndex);
            EnsureColumnIndexValid(minColumnIndex);
            EnsureColumnIndexValid(maxColumnIndex);

            DataGridCell cellLeft = DataGridHelper.GetCell(dataGrid, rowIndex, minColumnIndex);
            DataGridCell cellRight = DataGridHelper.GetCell(dataGrid, rowIndex, maxColumnIndex);
            FrameworkElement elemLeft = cellLeft as FrameworkElement;
            FrameworkElement elemRight = cellRight as FrameworkElement;

            UserInput.KeyDown(System.Windows.Input.Key.LeftShift.ToString());
            UserInput.MouseLeftClickCenter(elemLeft);
            UserInput.MouseLeftClickCenter(elemRight);
            UserInput.KeyUp(System.Windows.Input.Key.LeftShift.ToString());
            WaitForPriority(DispatcherPriority.Background);            

            dataGrid.UpdateLayout();
            WaitForPriority(DispatcherPriority.Background);

            CheckListCount(dataGrid.SelectedCells, maxColumnIndex - minColumnIndex + 1);
        }

        /// <summary>
        /// Copy the selected content to clipboard
        /// </summary>
        private void CopySelectionToClipboard()
        {
            DataGridActionHelper.CopyToClipboard(dataGrid);
            WaitForPriority(DispatcherPriority.Background);     
        }
        
        /// <summary>
        /// Clean up all selections
        /// </summary>
        private void ClearSelection()
        {
            if (dataGrid.SelectionUnit != DataGridSelectionUnit.Cell)
            {
                if (dataGrid.SelectionMode == DataGridSelectionMode.Extended)
                {
                    dataGrid.SelectedItems.Clear();
                }
                else
                {
                    dataGrid.SelectedItem = null;
                }
            }

            if (dataGrid.SelectionUnit != DataGridSelectionUnit.FullRow)
            {
                dataGrid.SelectedCells.Clear();
            }
        }

        /// <summary>
        /// Verify the correct count is in the IList
        /// </summary>
        /// <param name="list"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private bool CheckListCount(IList list, int count)
        {
            return ((list != null) && (list.Count == count));
        }

        /// <summary>
        /// Verify the correct cell count is in the selected cells collection
        /// </summary>
        /// <param name="list"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private bool CheckListCount(IList<DataGridCellInfo> list, int count)
        {
            return ((list != null) && (list.Count == count));
        }

        private string ModeAndUnitString
        {
            get
            {
                return String.Format("({0},{1})", dataGrid.SelectionMode, dataGrid.SelectionUnit);
            }
        }

        /// <summary>
        /// Verify the rows are selected, based on the person data source  
        /// </summary>
        /// <param name="dataItems"></param>
        /// <param name="isSelected"></param>
        private void AssertIsSelected(IList dataItems, bool isSelected)
        {
            if (dataItems != null)
            {
                foreach (object dataItem in dataItems)
                {
                    DataGridRow row = DataGridHelper.GetRow(dataGrid, dataItem);
                    if (row != null)
                    {
                        Assert.AssertTrue(String.Format("Row ({0}) at index {1}, IsSelected is not {2}. {3}", dataItem.ToString(), people.IndexOf((Person)dataItem), isSelected, ModeAndUnitString),
                            row.IsSelected == isSelected);
                    }
                }
            }
        }

        /// <summary>
        /// Verify a given row is selected, specific data source based
        /// </summary>
        /// <param name="row"></param>
        /// <param name="isSelected"></param>
        private void AssertIsSelected(DataGridRow row, bool isSelected)
        {
            Assert.AssertTrue(String.Format("Row ({0}) at index {1}, IsSelected is not {2}. {3}", row.Item.ToString(), people.IndexOf((Person)row.Item), isSelected, ModeAndUnitString),
                row.IsSelected == isSelected);
        }

        /// <summary>
        /// Verify a given cellinfo is selected
        /// </summary>
        /// <param name="cellInfo"></param>
        /// <param name="isSelected"></param>
        private void AssertIsSelected(DataGridCellInfo cellInfo, bool isSelected)
        {
            AssertIsSelected(DataGridHelper.GetCell(cellInfo), isSelected);
        }
       
        /// <summary>
        /// Verify a given cell is selected
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="isSelected"></param> 
        private void AssertIsSelected(DataGridCell cell, bool isSelected)
        {
            object dataItem = cell.DataContext;
            Assert.AssertTrue(String.Format("Cell at index {0} on row ({1}) at index {2}, IsSelected is not {3}. {4}", dataGrid.Columns.IndexOf(cell.Column), dataItem, people.IndexOf((Person)dataItem), isSelected, ModeAndUnitString),
                cell.IsSelected == isSelected);
        }

        private void AssertIsSelected(IList<DataGridCellInfo> cells, bool isSelected)
        {
            if (cells != null)
            {
                foreach (DataGridCellInfo cellInfo in cells)
                {
                    DataGridCell cell = DataGridHelper.GetCell(cellInfo);
                    if (cell != null)
                    {
                        Assert.AssertTrue(String.Format("Cell at index {0} on row ({1}) at index {2}, IsSelected is not {3}. {4}", dataGrid.Columns.IndexOf(cell.Column), cellInfo.Item.ToString(), people.IndexOf((Person)cellInfo.Item), isSelected, ModeAndUnitString), 
                            cell.IsSelected == isSelected);
                    }
                }
            }
        }

        /// <summary>
        /// Verify the hidden cells in the selection for a given row
        /// </summary>
        /// <param name="row"></param>
        /// <param name="isSelected"></param>
        private void AssertCellsAreSelected(DataGridRow row, bool isSelected)
        {
            int numColumns = dataGrid.Columns.Count - 1;
            AssertCellsAreSelected(row, 0, numColumns, isSelected);
        }

        /// <summary>
        /// Verify the hidden cells in selection for a given row and given column range
        /// </summary>
        /// <param name="row"></param>
        /// <param name="minColumnIndex"></param>
        /// <param name="maxColumnIndex"></param>
        /// <param name="isSelected"></param>
        private void AssertCellsAreSelected(DataGridRow row, int minColumnIndex, int maxColumnIndex, bool isSelected)
        {
            for (int i = minColumnIndex; i <= maxColumnIndex; i++)
            {
                if (dataGrid.Columns[i].Visibility != Visibility.Visible)
                {
                    continue;
                }
                DataGridCell cell = DataGridHelper.GetCell(row, i);
                AssertIsSelected(cell, isSelected);
            }
        }

        #endregion

        #endregion
    }
}
