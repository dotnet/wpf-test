using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using Avalon.Test.ComponentModel;
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
    ////////////////////////////////////////////////////////////////////////////////////////////
    // DISABLEDUNSTABLETEST:
    // TestName: DataGridRowAndColumnVirtualization
    // Area: Controls   SubArea: DataGrid
    // Disable this case due to high fail rate, will enable after fix it.
    // to find all disabled tests in test tree, use: “findstr /snip DISABLEDUNSTABLETEST” 
    ////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Tests for Row and Column Virtualizations - the tests can be as complex as you like them be
    /// 
    /// NOTE:
    ///     various factors can affact how a row and/or a column virtualization behave, here we perform 
    ///     basic and guideded tests based on these factors, random combos of all related factors are
    ///     excluded. In other words, for each key factor, we only consider closely related factor changes
    ///     whenever possible. 
    ///  
    /// TODOs: 
    ///     1. low pri - 
    ///         a. tests for CV/RV different values 
    ///         b. tests for different VSP values for 2 presenters
    ///     2. code refactoring - 
    ///         a. validation functionalities 
    ///         b. modulization 
    ///         c. helpers to base class and common
    /// </summary>
    [Test(0, "DataGrid", "DataGridRowAndColumnVirtualization", SecurityLevel = TestCaseSecurityLevel.FullTrust, Disabled = true)]
    public class DataGridRowAndColumnVirtualization : XamlTest
    {
        #region Private Fields

        private DataGrid dataGrid;
        private Page page;
        private NewPeople origPeople;
        private ScrollViewer scrollViewer;
        private ScrollContentPresenter scrollContentPresenter;
        private DataGridColumnHeadersPresenter columnHeadersPresenter;
        private DataGridCellsPresenter dataGridCellsPresenter;
        private DataGridCell cell0;
        private DataGridTextColumn textColumn;
        private IEnumerable origItemsSource;
        private bool origEnableRowVirtualization;
        private bool origEnableColumnVirtualization;
        private int origFrozenColumnCount;
        private int initFirstRowInView;
        private int initLastRowInView;
        private int initFirstColumnInView;
        private int initLastColumnInView;

        #endregion

        #region Public DS 

        public struct TestData
        {
            public ColumnVirtualizationActionTarget Target;
            public int Row;
            public int Column;
            public ColumnVirtualizationActionStatus Status;
        }

        public enum ColumnVirtualizationActionTarget
        {
            Cells,
            Columns,
            Rows,
            All,
        }

        public enum ColumnVirtualizationPropertyUpdateObject
        { 
            DataGrid,
            DataGridColumnHeadersPresenter,
            DataGridCellsPresenter,
            All,
        }

        public enum ColumnVirtualizationActionStatus
        { 
            DeVirtualized,
            Virtualized,
        }

        public enum ColumnVirtualizationScrollDirection
        { 
            Up,
            Down,
            Left,
            Right,
        }

        #endregion 

        #region Constructor

        public DataGridRowAndColumnVirtualization()
            : base(@"DataGridRowAndColumnVirtualization.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(TestColumnsCollectionChange);  
            RunSteps += new TestStep(TestItemCollectionChange);
            RunSteps += new TestStep(TestViewportChange);           //(move it to right after ItemCollectionChange would not get the exception)
            RunSteps += new TestStep(TestScrolling);                
            //RunSteps += new TestStep(TestColumnDisplayIndexChange);             
            RunSteps += new TestStep(TestColumnWidthChange);
            RunSteps += new TestStep(TestHeadersVisibilityChange);  
            RunSteps += new TestStep(TestRowResizing);              
            RunSteps += new TestStep(TestTabbing);                            
            RunSteps += new TestStep(TestVirtualizationModeChange);
            RunSteps += new TestStep(TestProperties);            
        }

        #endregion

        #region Test Steps

        /// <summary>
        /// Initial setups for getting the objects interested and a few initial properties
        /// </summary>
        /// <returns></returns>
        TestResult Setup()
        {
            Status("Setup");
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            // objects
            dataGrid = (DataGrid)RootElement.FindName("DataGrid_Standard");
            Assert.AssertTrue("Can not find the DataGrid in the xaml file!", dataGrid != null);

            page = (Page)this.Window.Content;
            origPeople = (NewPeople)(page.FindResource("people"));
            Assert.AssertTrue("Can not find the data source in the xaml file!", origPeople != null);

            scrollViewer = DataGridHelper.FindVisualChild<ScrollViewer>(dataGrid);
            Assert.AssertTrue("Can not find the DataGrid in the xaml file!", scrollViewer != null);
            scrollContentPresenter = (ScrollContentPresenter)scrollViewer.Template.FindName("PART_ScrollContentPresenter", scrollViewer);
            Assert.AssertTrue("Can not find the scrollContentPresenter!", scrollContentPresenter != null);

            columnHeadersPresenter = DataGridHelper.GetColumnHeadersPresenter(dataGrid);
            Assert.AssertTrue("Can not find the DataGridColumnHeaderPresenter!", columnHeadersPresenter != null);
            dataGridCellsPresenter = DataGridHelper.GetCellsPresenter(dataGrid);
            Assert.AssertTrue("Can not find the DataGridColumnHeaderPresenter!", dataGridCellsPresenter != null);

            origEnableRowVirtualization = dataGrid.EnableRowVirtualization;
            Assert.AssertTrue("The initial EnableRowVirtualization should be true!", origEnableRowVirtualization == true);
            origEnableColumnVirtualization = dataGrid.EnableColumnVirtualization;
            Assert.AssertTrue("The initial EnableColumnVirtualization should be false!", origEnableColumnVirtualization == false);

            // properties
            origItemsSource = dataGrid.ItemsSource;

            origFrozenColumnCount = 0;
            dataGrid.FrozenColumnCount = origFrozenColumnCount;

            SetFocus();

            initFirstRowInView = DataGridHelper.GetFirstRowIndexInView(dataGrid);
            initLastRowInView = DataGridHelper.GetLastRowIndexInView(dataGrid);
            LogComment(string.Format("The InView firstrow = {0} and the lastrow = {1}", initFirstRowInView, initLastRowInView));

            initFirstColumnInView = DataGridHelper.GetFirstVisibleColumnIndex(dataGrid);
            initLastColumnInView = DataGridHelper.GetLastVisibileColumnIndex(dataGrid);
            LogComment(string.Format("The InView firstcolumn = {0} and lastcolumn = {1}", initFirstColumnInView, initLastColumnInView));
           
            LogComment("Setup was successful");
            return TestResult.Pass;
        }

        TestResult CleanUp()
        {
            dataGrid = null;
            scrollViewer = null;
            scrollContentPresenter = null;
            columnHeadersPresenter = null;
            dataGridCellsPresenter = null;
            cell0 = null;
            textColumn = null;
            page = null;
            return TestResult.Pass;
        }

        /// <summary>
        /// Basic tests for properties on DataGrid and the 2 presenters 
        /// for default values and inheritance, etc. 
        /// </summary>
        /// <returns></returns>
        TestResult TestProperties()
        {
            Status("TestProperties");

            // init
            Assert.AssertTrue("The init row virtualization property should be true", VirtualizingStackPanel.GetIsVirtualizing(dataGrid) == true);
            Assert.AssertTrue("The init column virtualization property should be false", VirtualizingStackPanel.GetIsVirtualizing(columnHeadersPresenter) == false);
            Assert.AssertTrue("The init cell virtualization property should be false", VirtualizingStackPanel.GetIsVirtualizing(dataGridCellsPresenter) == false);

            // set DataGrid's VSP property and verify the RV on the dataGridCellsPresenter
            UpdateVirtualizationOnDataGrid("default", true);

            UpdateVirtualizationOnDataGrid("column", true);

            // set CV at the presenters
            dataGrid.ClearValue(DataGrid.EnableColumnVirtualizationProperty);
            WaitForPriority(DispatcherPriority.Background);

            // set the values on presenter - no update up
            UpdateVirtualizationOnPresenter(ColumnVirtualizationPropertyUpdateObject.All, true);           
            // should stay the same 
            Assert.AssertTrue("The datagrid EnableColumnVirtualization property should be false", dataGrid.EnableColumnVirtualization == false);

            // toggle the values - - no update up
            UpdateVirtualizationOnPresenter(ColumnVirtualizationPropertyUpdateObject.All, false);
            Assert.AssertTrue("The datagrid EnableColumnVirtualization property should be false still", dataGrid.EnableColumnVirtualization == false);

            // reset all  
            columnHeadersPresenter.ClearValue(VirtualizingStackPanel.IsVirtualizingProperty);
            dataGridCellsPresenter.ClearValue(VirtualizingStackPanel.IsVirtualizingProperty);
            WaitForPriority(DispatcherPriority.Background);

            EnsureDefault();

            LogComment("TestProperties was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// RV testing for item collection changes from 10 to more items while RV is on and off 
        /// </summary>
        /// <returns></returns>
        TestResult TestItemCollectionChange() 
        {
            Status("TestItemCollectionChange");

            //init
            EnsureDefault();

            // change to the new DS
            NewMorePeople newmorepeople = new NewMorePeople();
            Assert.AssertTrue("The new data source should have 20 items!", newmorepeople.Count == 20);

            dataGrid.ItemsSource = newmorepeople; 
            WaitForPriority(DispatcherPriority.Background);
            Assert.AssertTrue("The datagrid should have 21 items!", dataGrid.Items.Count == newmorepeople.Count+1);

            // actions and verifications           
            LogComment("Init...");            
            VerifyTarget(new TestData { Target = ColumnVirtualizationActionTarget.Rows, Row = 0, Column = 0, Status = ColumnVirtualizationActionStatus.DeVirtualized });
            VerifyTarget(new TestData { Target = ColumnVirtualizationActionTarget.Rows, Row = dataGrid.Items.Count - 2, Column = 0, Status = ColumnVirtualizationActionStatus.Virtualized });

            Paging(ColumnVirtualizationScrollDirection.Down);
            VerifyTarget(new TestData { Target = ColumnVirtualizationActionTarget.Rows, Row = 0, Column = 0, Status = ColumnVirtualizationActionStatus.Virtualized });
            VerifyTarget(new TestData { Target = ColumnVirtualizationActionTarget.Rows, Row = dataGrid.Items.Count - 2, Column = 0, Status = ColumnVirtualizationActionStatus.DeVirtualized });

            Paging(ColumnVirtualizationScrollDirection.Up);
            VerifyTarget(new TestData { Target = ColumnVirtualizationActionTarget.Rows, Row = 0, Column = 0, Status = ColumnVirtualizationActionStatus.DeVirtualized });
            VerifyTarget(new TestData { Target = ColumnVirtualizationActionTarget.Rows, Row = dataGrid.Items.Count - 2, Column = 0, Status = ColumnVirtualizationActionStatus.Virtualized });

            //*************************
            // now set CV = true
            //*************************            
            UpdateVirtualizationOnDataGrid("column", true);
            SetFocus();

            LogComment("After CV set to true...");
            EnsureColumnVirtualization(0);

            Paging(ColumnVirtualizationScrollDirection.Right);
            EnsureColumnVirtualization(0);

            // reset to left and validate
            Paging(ColumnVirtualizationScrollDirection.Left);
            EnsureColumnVirtualization(0);

            EnsureDefault(); 
            
            LogComment("TestItemCollectionChange was successful");
            return TestResult.Pass;
        }
        
        /// <summary>
        /// CV Tests for columns collection change - add a column, remove it and add back, etc. 
        /// </summary>
        /// <returns></returns>
        TestResult TestColumnsCollectionChange() 
        {
            Status("TestColumnsCollectionChange");

            // init
            EnsureDefault();

            // init verifications before any column collection change
            VerifyTarget(new TestData { Target = ColumnVirtualizationActionTarget.Cells, Row = 0, Column = 0, Status = ColumnVirtualizationActionStatus.DeVirtualized });
            VerifyTarget(new TestData { Target = ColumnVirtualizationActionTarget.Cells, Row = 0, Column = 7, Status = ColumnVirtualizationActionStatus.DeVirtualized });

            UpdateVirtualizationOnDataGrid("column", true);
            EnsureColumnVirtualization(0);

            // reset before the column collection changes
            UpdateVirtualizationOnDataGrid("column", false);

            // make column collection changes and perform verifications accordingly
            AddNewColumn();

            UpdateVirtualizationOnDataGrid("column", true); // after this, the columns not in view are not vir; but after paging right and then paging left, all worked
            EnsureColumnVirtualization(0);    

            Paging(ColumnVirtualizationScrollDirection.Right);
            EnsureColumnVirtualization(0);  

            LogComment("Verify after paging right...");
            EnsureColumnVirtualization(0);  

            Paging(ColumnVirtualizationScrollDirection.Left);
            EnsureColumnVirtualization(0);  

            // remove the textcolumn and add it back 
            RemoveTheColumn();
            AddNewColumn();

            EnsureColumnVirtualization(0);  

            // reset all
            RemoveTheColumn();

            EnsureDefault();

            LogComment("TestColumnsCollectionChange was successful");
            return TestResult.Pass;
        }
        
        /// <summary>
        /// CV - tests for column width changes
        /// </summary>
        /// <returns></returns>
        TestResult TestColumnWidthChange()
        {
            Status("TestColumnWidthChange");

            // init
            Assert.AssertTrue("The init ColumnVirtualization should be false", dataGrid.EnableColumnVirtualization == false);
            SetFocus();

            DataGridColumn column = cell0.Column;
            DataGridLength origWidth = column.Width;            
            VerifyTarget(new TestData { Target = ColumnVirtualizationActionTarget.Cells, Row = 0, Column = dataGrid.Columns.Count-1, Status = ColumnVirtualizationActionStatus.DeVirtualized });

            // actions and verifications
            column.Width = 300d;
            WaitForPriority(DispatcherPriority.Background);
            VerifyTarget(new TestData { Target = ColumnVirtualizationActionTarget.Cells, Row = 0, Column = dataGrid.Columns.Count - 1, Status = ColumnVirtualizationActionStatus.DeVirtualized });

            // change CV to true
            UpdateVirtualizationOnDataGrid("column", true);
            EnsureColumnVirtualization(0);  
            //VerifyTarget(new TestData { Target = ColumnVirtualizationActionTarget.Cells, Row = 0, Column = dataGrid.Columns.Count - 1, Status = ColumnVirtualizationActionStatus.DeVirtualized });

            // restore the width
            column.Width = origWidth;
            WaitForPriority(DispatcherPriority.Background);
            VerifyTarget(new TestData { Target = ColumnVirtualizationActionTarget.Cells, Row = 0, Column = dataGrid.Columns.Count - 1, Status = ColumnVirtualizationActionStatus.DeVirtualized });
        
            // reset all
            EnsureDefault();

            LogComment("TestColumnWidthChange was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Tests for column display index change affacting virtualization or de-virtualization
        /// </summary>
        /// <returns></returns>
        TestResult TestColumnDisplayIndexChange()
        {
            Status("TestColumnDisplayIndexChange");

            // init
            bool origDataGridCanUserReorderColumns = dataGrid.CanUserReorderColumns;
            DataGridColumn column = dataGrid.Columns[0];
            int origDisplayIndex = column.DisplayIndex;

            dataGrid.CanUserReorderColumns = true;
            WaitForPriority(DispatcherPriority.Background);

            Assert.AssertTrue("The init ColumnVirtualization should be false", dataGrid.EnableColumnVirtualization == false);
            SetFocus();

            // actions and verifications
            Paging(ColumnVirtualizationScrollDirection.Right);
            LogComment("verify after paging right...");
            VerifyTarget(new TestData { Target = ColumnVirtualizationActionTarget.Cells, Row = 0, Column = 0, Status = ColumnVirtualizationActionStatus.DeVirtualized });

            // toggle cv to true
            UpdateVirtualizationOnDataGrid("column", true);

            LogComment("verify after cv to true...");
            EnsureColumnVirtualization(0);  
                        
            // change the displayindex
            LogComment("change displayindex...");
            column.DisplayIndex = dataGrid.Columns.Count - 1;
            WaitForPriority(DispatcherPriority.Background);

            // column index stays the same
            LogComment("verify again...");
            EnsureColumnVirtualization(0);  

            // reset all
            Paging(ColumnVirtualizationScrollDirection.Left);

            column.DisplayIndex = origDisplayIndex;
            dataGrid.CanUserReorderColumns = origDataGridCanUserReorderColumns;
            
            EnsureDefault();

            LogComment("TestColumnDisplayIndexChange was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Tests for headers visibility changes - show all, column only, row only, none, etc. 
        /// </summary>
        /// <returns></returns>
        TestResult TestHeadersVisibilityChange() 
        {
            Status("TestHeadersVisibilityChange");

            // init
            EnsureDefault();

            double origRowWidth = dataGrid.RowHeaderWidth;
            DataGridHeadersVisibility origHeadersVisibility = dataGrid.HeadersVisibility;
            Assert.AssertTrue("The orig visibility should be All", origHeadersVisibility == DataGridHeadersVisibility.All);

            VerifyTarget(new TestData { Target = ColumnVirtualizationActionTarget.Rows, Row = 7, Column = 0, Status = ColumnVirtualizationActionStatus.DeVirtualized });
            VerifyTarget(new TestData { Target = ColumnVirtualizationActionTarget.Cells, Row = 0, Column = 4, Status = ColumnVirtualizationActionStatus.DeVirtualized });

            // widen rowheaders
            LogComment("Widen rowheaders...");
            dataGrid.RowHeaderWidth = 120d;
            WaitForPriority(DispatcherPriority.Background);

            VerifyTarget(new TestData { Target = ColumnVirtualizationActionTarget.Cells, Row = 0, Column = 4, Status = ColumnVirtualizationActionStatus.DeVirtualized });

            // toggle CV
            UpdateVirtualizationOnDataGrid("column", true);

            EnsureColumnVirtualization(0);  

            // change HeadersVisibilty and verify accordingly
            UpdateHeaderVisibility(DataGridHeadersVisibility.Column);
            EnsureColumnVirtualization(0);  

            UpdateHeaderVisibility(DataGridHeadersVisibility.Row);
            VerifyTarget(new TestData { Target = ColumnVirtualizationActionTarget.Rows, Row = 7, Column = 0, Status = ColumnVirtualizationActionStatus.DeVirtualized });

            UpdateHeaderVisibility(DataGridHeadersVisibility.All);
            EnsureColumnVirtualization(0);
            VerifyTarget(new TestData { Target = ColumnVirtualizationActionTarget.Cells, Row = 0, Column = 4, Status = ColumnVirtualizationActionStatus.DeVirtualized });
            VerifyTarget(new TestData { Target = ColumnVirtualizationActionTarget.Rows, Row = 7, Column = 0, Status = ColumnVirtualizationActionStatus.DeVirtualized });

            // reset all
            dataGrid.RowHeaderWidth = origRowWidth;           
            dataGrid.HeadersVisibility = origHeadersVisibility;

            EnsureDefault();

            LogComment("TestHeadersVisibilityChange was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// ViewPort changes and verifications
        /// </summary>
        /// <returns></returns>
        TestResult TestViewportChange()
        {
            Status("TestViewportChange");

            // init
            double origHeight = dataGrid.Height;
            double origWidth = dataGrid.Width;
            dataGrid.AutoGenerateColumns = true;
            WaitForPriority(DispatcherPriority.Background);

            UpdateVirtualizationOnDataGrid("row", true);
            UpdateVirtualizationOnDataGrid("column", true);
            
            SetFocus();

            LogComment("Init verifications...");
            EnsureColumnVirtualization(0);

            // init paging and verifications
            Paging(ColumnVirtualizationScrollDirection.Right);
            WaitForPriority(DispatcherPriority.Background);
            Paging(ColumnVirtualizationScrollDirection.Down);
            WaitForPriority(DispatcherPriority.Background);

            LogComment("after paging verifications...");
            EnsureColumnVirtualization(0);

            // change viewport and verifications
            dataGrid.Height = 200d;
            dataGrid.Width = 200d;
            dataGrid.UpdateLayout();
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            LogComment("after viewport change verifications...");
            EnsureColumnVirtualization(0);

            // change back to original viewport size and verifications
            dataGrid.Height = origHeight;
            dataGrid.Width = origWidth;
            dataGrid.UpdateLayout();
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            LogComment("after viewport reset verifications...");
            EnsureColumnVirtualization(0);

            // reset all
            Paging(ColumnVirtualizationScrollDirection.Left);
            WaitForPriority(DispatcherPriority.Background);
            Paging(ColumnVirtualizationScrollDirection.Up);
            WaitForPriority(DispatcherPriority.Background);

            EnsureDefault();

            LogComment("TestViewportChange was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Tests for CV's VirtualizationMode changes - less interesting here than in perf tests:
        ///     DataGridCellsPresenter / DataGridColumnHeadersPresenter’s 
        ///         VirtualizingStackPanel.VirtualizationMode="Standard"
        /// </summary>
        /// <returns></returns>
        TestResult TestVirtualizationModeChange()
        {
            Status("TestVirtualizationModeChange");

            // init
            UpdateVirtualizationOnDataGrid("column", true);

            // change the mode
            foreach (VirtualizationMode mode in Enum.GetValues(typeof(VirtualizationMode)))
            {
                LogComment("Mode = " + Enum.GetName(typeof(VirtualizationMode), mode));
                VirtualizingStackPanel.SetVirtualizationMode(dataGridCellsPresenter, mode);
                VirtualizingStackPanel.SetVirtualizationMode(columnHeadersPresenter, mode);
                WaitForPriority(DispatcherPriority.Background);

                VerifyVirtualizationModeResults();
            }

            // change back
            VirtualizingStackPanel.SetVirtualizationMode(dataGridCellsPresenter, VirtualizationMode.Recycling);
            VirtualizingStackPanel.SetVirtualizationMode(columnHeadersPresenter, VirtualizationMode.Recycling);
            WaitForPriority(DispatcherPriority.Background);

            VerifyVirtualizationModeResults();
           
            // reset all
            EnsureDefault();

            LogComment("TestVirtualizationModeChange was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// refactored helper - 
        ///     other tests can follow the same step, but we'd not be able to mark which action in 
        ///     which step that has any issue, so we will leave them as they are for now. 
        /// </summary>
        private void VerifyVirtualizationModeResults()
        {
            EnsureColumnVirtualization(0);
            VerifyTarget(new TestData { Target = ColumnVirtualizationActionTarget.Rows, Row = 0, Column = 0, Status = ColumnVirtualizationActionStatus.DeVirtualized });
            VerifyTarget(new TestData { Target = ColumnVirtualizationActionTarget.Rows, Row = dataGrid.Items.Count - 2, Column = 0, Status = ColumnVirtualizationActionStatus.Virtualized });
        }

        /// <summary>
        /// Tests for tabbing changes - cells and rows non-vir to vir and vice versa
        /// </summary>
        /// <returns></returns>
        TestResult TestTabbing() 
        {
            Status("TestTabbing");

            // init
            DataGridSelectionMode origMode = dataGrid.SelectionMode;
            DataGridSelectionUnit origUnit = dataGrid.SelectionUnit;
            dataGrid.SelectionMode = DataGridSelectionMode.Single;
            dataGrid.SelectionUnit = DataGridSelectionUnit.Cell;

            // actions and verifications
            UpdateVirtualizationOnDataGrid("column", true);

            PrintInViewIndexes();

            LogComment("verify after the CV to true...");
            EnsureColumnVirtualization(1);

            int lastIndex = GetLastRealizedColumnIndex();
            DataGridCell cellbefore = DataGridHelper.GetCellVirtual(dataGrid, 1, lastIndex);
            cellbefore.Focus();
            WaitForPriority(DispatcherPriority.Background);

            UserInput.KeyDown("Tab");
            UserInput.KeyUp("Tab");
            WaitForPriority(DispatcherPriority.Background);

            LogComment("verify after the Tab...");
            VerifyTarget(new TestData { Target = ColumnVirtualizationActionTarget.Cells, Row = 0, Column = lastIndex+1, Status = ColumnVirtualizationActionStatus.DeVirtualized });

            // tab back
            UserInput.KeyDown("LeftShift");
            UserInput.KeyDown("Tab");
            UserInput.KeyUp("Tab");
            UserInput.KeyUp("LeftShift");
            WaitForPriority(DispatcherPriority.Background);

            LogComment("verify after the Tab BACK...");
            EnsureColumnVirtualization(1);
            
            // reset all
            dataGrid.SelectionMode = origMode;
            dataGrid.SelectionUnit = origUnit;

            EnsureDefault();

            LogComment("TestTabbing was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Tests for rowresizing that affact the row's RV
        /// </summary>
        /// <returns></returns>
        TestResult TestRowResizing()
        {
            Status("TestRowResizing");

            // init
            DataGridRow row = DataGridHelper.GetRowVirtual(dataGrid,0);
            double origHeight = row.Height;
            SetFocus();

            EnsureRowVirtualizationOn(); // init     
            VerifyTarget(new TestData { Target = ColumnVirtualizationActionTarget.Rows, Row = dataGrid.Items.Count - 2, Column = 0, Status = ColumnVirtualizationActionStatus.DeVirtualized });

            initLastRowInView = DataGridHelper.GetLastRowIndexInView(dataGrid);
            LogComment(" initLastRowInView = " + initLastRowInView.ToString());

            // increase the height 
            LogComment("enlarging row0 height..."); 
            row.Height = 300d;
            WaitForPriority(DispatcherPriority.Background);

            DataGridRow row6 = DataGridHelper.GetRowVirtual(dataGrid, dataGrid.Items.Count - 2);
            VerifyTarget(new TestData { Target = ColumnVirtualizationActionTarget.Rows, Row = dataGrid.Items.Count - 2, Column = 0, Status = ColumnVirtualizationActionStatus.DeVirtualized });

            // toggle the RV
            UpdateVirtualizationOnDataGrid("row", false); 
            //

            // reset all
            cell0.Height = origHeight;

            EnsureDefault();

            LogComment("TestRowResizing was successful");
            return TestResult.Pass;
        }

        private void EnsureRowVirtualizationOn()
        {
            if (dataGrid.EnableRowVirtualization == false)
            {
                throw new TestValidationException("The init RV should be true");
            }
        }

        /// <summary>
        /// CV / RV - mainly focusing on the CV tests w/ CV toggle
        /// </summary>
        /// <returns></returns>
        TestResult TestScrolling() 
        {
            Status("TestScrolling");
            
            // init            
            UpdateVirtualizationOnDataGrid("column", true);
            UpdateVirtualizationOnDataGrid("row", true);

            SetFocus();

            LogComment("Init verifications after both properties are true...");
            EnsureColumnVirtualization(dataGrid.Items.Count - 2);
            VerifyTarget(new TestData { Target = ColumnVirtualizationActionTarget.Rows, Row = 0, Column = 0, Status = ColumnVirtualizationActionStatus.DeVirtualized });
            VerifyTarget(new TestData { Target = ColumnVirtualizationActionTarget.Rows, Row = dataGrid.Items.Count - 2, Column = 0, Status = ColumnVirtualizationActionStatus.DeVirtualized });

            Paging(ColumnVirtualizationScrollDirection.Down);
            WaitForPriority(DispatcherPriority.Render);
            Paging(ColumnVirtualizationScrollDirection.Right);
            WaitForPriority(DispatcherPriority.Render);

            LogComment("Verifications after paging down and right...");
            EnsureColumnVirtualization(dataGrid.Items.Count - 2);
            VerifyTarget(new TestData { Target = ColumnVirtualizationActionTarget.Rows, Row = 0, Column = 0, Status = ColumnVirtualizationActionStatus.DeVirtualized });
            VerifyTarget(new TestData { Target = ColumnVirtualizationActionTarget.Rows, Row = dataGrid.Items.Count - 2, Column = 0, Status = ColumnVirtualizationActionStatus.DeVirtualized });

            // toggle RV/CV both to false
            UpdateVirtualizationOnDataGrid("column", false);
            UpdateVirtualizationOnDataGrid("row", false);

            LogComment("Verifications after reset the properties to false...");
            VerifyTarget(new TestData { Target = ColumnVirtualizationActionTarget.Cells, Row = 0, Column = 0, Status = ColumnVirtualizationActionStatus.DeVirtualized });
            VerifyTarget(new TestData { Target = ColumnVirtualizationActionTarget.Cells, Row = dataGrid.Items.Count - 2, Column = dataGrid.Columns.Count - 1, Status = ColumnVirtualizationActionStatus.DeVirtualized });
            VerifyTarget(new TestData { Target = ColumnVirtualizationActionTarget.Rows, Row = 0, Column = 0, Status = ColumnVirtualizationActionStatus.DeVirtualized });
            VerifyTarget(new TestData { Target = ColumnVirtualizationActionTarget.Rows, Row = dataGrid.Items.Count - 2, Column = 0, Status = ColumnVirtualizationActionStatus.DeVirtualized });

            // reset all
            Paging(ColumnVirtualizationScrollDirection.Left);
            Paging(ColumnVirtualizationScrollDirection.Up);

            EnsureDefault();

            LogComment("TestScrolling was successful");
            return TestResult.Pass;
        }

        #endregion
        
        #region Local Helpers

        /// <summary>
        /// Print the indexes of the in view rows and columns
        /// </summary>
        private void PrintInViewIndexes()
        {
            LogComment("In PrintInViewIndexes...");
            int firstRowInView = DataGridHelper.GetFirstRowIndexInView(dataGrid);
            int lastRowInView = DataGridHelper.GetLastRowIndexInView(dataGrid);
            LogComment(string.Format("The InView FirstRow = {0} and LastRow = {1}", firstRowInView, lastRowInView));

            int firstColumnInView = DataGridHelper.GetFirstVisibleColumnIndex(dataGrid);
            int lastColumnInView = DataGridHelper.GetLastVisibileColumnIndex(dataGrid);
            LogComment(string.Format("The InView FirstColumn = {0} and LastColumn = {1}", firstColumnInView, lastColumnInView));
        }
        
        /// <summary>
        /// Set the focus at the very first cell
        /// </summary>
        private void SetFocus()
        {
            LogComment("SetFocus...");
            cell0 = DataGridHelper.GetCellVirtual(dataGrid, 0, 0);
            cell0.Focus();
        }

        /// <summary>
        /// ensure the default RV/CV properties are correct
        /// </summary>
        private void EnsureDefault()
        {
            dataGrid.EnableRowVirtualization = true;
            WaitForPriority(DispatcherPriority.Background);

            dataGrid.EnableColumnVirtualization = false;
            WaitForPriority(DispatcherPriority.Background);

            Assert.AssertTrue("The RowVirtualization should be true", dataGrid.EnableRowVirtualization == true);
            Assert.AssertTrue("The DG ColumnVirtualization should be false", dataGrid.EnableColumnVirtualization == false);
            Assert.AssertTrue("The column virtualization property should be false", VirtualizingStackPanel.GetIsVirtualizing(columnHeadersPresenter) == false);
            Assert.AssertTrue("The cell virtualization property should be false", VirtualizingStackPanel.GetIsVirtualizing(dataGridCellsPresenter) == false);
        }

        /// <summary>
        /// Add a new TextColumn to the Columns and set its initial DisplayIndex
        /// </summary>
        private void AddNewColumn()
        {
            LogComment("AddNewColumn");

            textColumn = new DataGridTextColumn();
            textColumn.Width = 100;
            textColumn.Header = "new column";
            dataGrid.Columns.Add(textColumn);            
            WaitForPriority(DispatcherPriority.Render);
        }

        /// <summary>
        /// remove the textcolumn added to the column collection
        /// </summary>
        private void RemoveTheColumn()
        {
            LogComment("RemoveTheColumn");
            dataGrid.Columns.Remove(textColumn);
            WaitForPriority(DispatcherPriority.Background);        
        }

        /// <summary>
        /// Update the row and column virtualization properties on DataGrid
        /// </summary>
        /// <param name="target">row for EnableRowVirtualization; column for EnableColumnVirtualization; other for VSP property</param>
        /// <param name="value">true or false</param>
        private void UpdateVirtualizationOnDataGrid(string target, bool value)
        {
            LogComment(string.Format("Update the virtualization {0} to {1}", target, value));
            if (string.IsNullOrEmpty(target))
            {
                throw new TestValidationException("The target can not be null or empty");
            }
            switch(target.ToLower())
            {
                case "row":
                    dataGrid.EnableRowVirtualization = value;
                    WaitForPriority(DispatcherPriority.Background);
                    Assert.AssertTrue(string.Format("The datagrid EnableRowVirtualization should be {0}", value), 
                        dataGrid.EnableRowVirtualization == value);
                    break;

                case "column":
                    dataGrid.EnableColumnVirtualization = value;
                    WaitForPriority(DispatcherPriority.Background);
                    Assert.AssertTrue(string.Format("The datagrid EnableColumnVirtualization should be {0}", value),
                        dataGrid.EnableColumnVirtualization == value);
                    break;

                default:
                    VirtualizingStackPanel.SetIsVirtualizing(dataGrid,value);
                    WaitForPriority(DispatcherPriority.Background);
                    Assert.AssertTrue(string.Format("The datagrid EnableRowVirtualization should be {0} accordingly", value), 
                        dataGrid.EnableRowVirtualization == value);
                    break;
            }
        }

        /// <summary>
        /// update the column virtualization related properties on the 2 presenters
        ///     refactoring can be done to club this one with the one for datagrid
        /// </summary>
        /// <param name="obj">cellspresenter only; headerpresenter only; or both</param>
        /// <param name="value">true or false</param>
        private void UpdateVirtualizationOnPresenter(ColumnVirtualizationPropertyUpdateObject obj, bool value)
        {
            columnHeadersPresenter = DataGridHelper.GetColumnHeadersPresenter(dataGrid);
            Assert.AssertTrue("Can not find the DataGridColumnHeaderPresenter!", columnHeadersPresenter != null);
            dataGridCellsPresenter = DataGridHelper.GetCellsPresenter(dataGrid);
            Assert.AssertTrue("Can not find the DataGridColumnHeaderPresenter!", dataGridCellsPresenter != null);

            switch(obj)
            {
                case ColumnVirtualizationPropertyUpdateObject.DataGridCellsPresenter:
                    VirtualizingStackPanel.SetIsVirtualizing(dataGridCellsPresenter, value);
                    WaitForPriority(DispatcherPriority.Background);

                    Assert.AssertTrue(string.Format("The cell virtualization is not set to {0}", value.ToString()), 
                        VirtualizingStackPanel.GetIsVirtualizing(dataGridCellsPresenter) == value);
                    break;

                case ColumnVirtualizationPropertyUpdateObject.DataGridColumnHeadersPresenter:
                    VirtualizingStackPanel.SetIsVirtualizing(columnHeadersPresenter, value);
                    WaitForPriority(DispatcherPriority.Background);

                    Assert.AssertTrue(string.Format("The column virtualization is not set to {0}", value.ToString()),
                        VirtualizingStackPanel.GetIsVirtualizing(columnHeadersPresenter) == value);
                    break;
            
                case ColumnVirtualizationPropertyUpdateObject.All:
                    VirtualizingStackPanel.SetIsVirtualizing(dataGridCellsPresenter, value);
                    WaitForPriority(DispatcherPriority.Background);
                    VirtualizingStackPanel.SetIsVirtualizing(columnHeadersPresenter, value);
                    WaitForPriority(DispatcherPriority.Background);

                    Assert.AssertTrue(string.Format("The cell virtualization is not set to {0}", value.ToString()),
                        VirtualizingStackPanel.GetIsVirtualizing(dataGridCellsPresenter) == value);
                    Assert.AssertTrue(string.Format("The column virtualization is not set to {0}", value.ToString()),
                        VirtualizingStackPanel.GetIsVirtualizing(columnHeadersPresenter) == value);
                    break;

                default: // no others
                    break;
            }
        
        }
        
        /// <summary>
        /// Update datagrid's HeadersVisibility property
        /// </summary>
        /// <param name="value">the HeadersVisibility value to set</param>
        private void UpdateHeaderVisibility(DataGridHeadersVisibility value)
        {
            LogComment("HeaderVisibility changes to " + Enum.GetName(typeof(DataGridHeadersVisibility), value));
            dataGrid.HeadersVisibility = value;
            WaitForPriority(DispatcherPriority.Background);
        }

        /// <summary>
        /// Paging action based on the direction
        /// </summary>
        /// <param name="direction">left, right, up, down</param>
        private void Paging(ColumnVirtualizationScrollDirection direction)
        {
            LogComment("Paging " + Enum.GetName(typeof(ColumnVirtualizationScrollDirection), direction));
  
            switch(direction)
            {
                case ColumnVirtualizationScrollDirection.Down:
                    scrollViewer.PageDown();
                    WaitForPriority(DispatcherPriority.Background);
                    break;
                case ColumnVirtualizationScrollDirection.Up:
                    scrollViewer.PageUp();
                    WaitForPriority(DispatcherPriority.Background);
                    break;
                case ColumnVirtualizationScrollDirection.Right:
                    scrollViewer.PageRight();
                    WaitForPriority(DispatcherPriority.Background);
                    break;
                case ColumnVirtualizationScrollDirection.Left:
                    scrollViewer.PageLeft();
                    WaitForPriority(DispatcherPriority.Background);
                    break;
            }            
        }

        /// <summary>
        /// The verification method given the test data for row/cell/column, etc.
        /// </summary>
        /// <param name="data">the test data</param>
        private void VerifyTarget(TestData data)
        {             
            string str = (data.Status == ColumnVirtualizationActionStatus.DeVirtualized) ? "de-virtualized" : "virtualized";      
            switch(data.Target)
            {
                case ColumnVirtualizationActionTarget.Rows: // the column value will be ignored
                    DataGridRow row = DataGridHelper.GetRowVirtual(dataGrid, data.Row); 
                    Assert.AssertTrue(string.Format("The row is expected to be {0} at row = {1}", str, data.Row),
                        ((data.Status == ColumnVirtualizationActionStatus.DeVirtualized) ? (row != null) : (row == null)));
                    break;

                case ColumnVirtualizationActionTarget.Columns: // ignore the row value
                    DataGridColumnHeader columnheader = DataGridHelper.GetColumnHeader(dataGrid, data.Column);
                    Assert.AssertTrue(string.Format("The columnheader is expected to be {0} at column = {1}.", str, data.Column),
                        ((data.Status == ColumnVirtualizationActionStatus.DeVirtualized) ? (columnheader != null) : (columnheader == null)));
                    break;

                case ColumnVirtualizationActionTarget.Cells: // need both row and column
                    DataGridCell cell = DataGridHelper.GetCellVirtual(dataGrid, data.Row, data.Column);
                    Assert.AssertTrue(string.Format("The cell is expected to be {0} at row={1} and column={2}.", str, data.Row, data.Column),
                        ((data.Status == ColumnVirtualizationActionStatus.DeVirtualized) ? (cell != null) : (cell == null)));
                    break;

                case ColumnVirtualizationActionTarget.All: // same as cells.  TBD. 
                    break;
            
                default:
                    break;
            }

        }

        private void EnsureColumnVirtualization(int row)
        {
            if (!IsColumnVirtualizationWorking(row))
            {
                throw new TestValidationException("The Column Virtualization does not work correctly");
            }        
        }

        private bool IsColumnVirtualizationWorking(int row)
        {
            int columnCount = dataGrid.Columns.Count; 
            int realizedCellCount = 0;
            for (int i = 0; i < columnCount; i++)
            {
                if (DataGridHelper.GetCell(dataGrid, row, i) != null)
                {
                    //LogComment("The column at i = " + i.ToString() + " is not null.");
                    realizedCellCount++;
                }
                else
                {
                    //LogComment("The column at i = " + i.ToString() + " is null.");
                    continue;
                }
            }
            if (realizedCellCount == columnCount)
            {
                return false;
            }
            return true;
        }

        private void EnsureRowVirtualization(int column)
        {
            if (!IsRowVirtualizationWorking(column))
            {
                throw new TestValidationException("The Row Virtualization does not work correctly");
            }
        }

        private bool IsRowVirtualizationWorking(int column)
        {
            // given the VSP issue with the virtualization, this would not always work as expected. 
            int rowCount = dataGrid.Items.Count - 1;
            int realizedCellCount = 0;
            for (int i = 0; i < rowCount; i++)
            {
                if (DataGridHelper.GetCell(dataGrid, i, column) != null)
                {
                    LogComment("The row at i = " + i.ToString() + " is not null.");
                    realizedCellCount++;
                }
                else
                {
                    LogComment("The row at i = " + i.ToString() + " is null.");
                }
            }
            if (realizedCellCount == rowCount)
            {
                return false;
            }
            return true;
        }

        private int GetLastRealizedColumnIndex()
        {
            int currentColumnIndex = dataGrid.CurrentCell.Column.DisplayIndex;
            int rowIndex = dataGrid.Items.IndexOf(dataGrid.CurrentCell.Item);

            int columnCount = dataGrid.Columns.Count;
            if ((currentColumnIndex == columnCount - 1) || (columnCount == 0))
            {
                return currentColumnIndex;
            }
            int index = currentColumnIndex;
            for (int i = currentColumnIndex; i < columnCount; i++)
            {
                index = i; 
                if (DataGridHelper.GetCell(dataGrid, rowIndex, i) != null)
                {
                    continue;
                }
                else
                {
                    break;
                }                
            }
            return index - 1;
        }
        
        #endregion
    }
    
    /// <summary>
    /// this is a data source to test the datasource switch for the Rows and Column Virtualization feature
    /// don't modify it unless you also update the tests accordingly
    /// </summary>
    public class NewMorePeople : ObservableCollection<Person>
    {
        public NewMorePeople()
        {
            Add(new Person("George", string.Empty, "Washington", new DateTime(1999, 1, 26)));
            Add(new Person("John", string.Empty, "Adams", new DateTime(2000, 2, 26)));
            Add(new Person("Thomas", string.Empty, "Jefferson", new DateTime(2001, 3, 26)));
            Add(new Person("James", string.Empty, "Madison", new DateTime(2002, 4, 26)));
            Add(new Person("James", string.Empty, "Monroe", new DateTime(2003, 5, 26)));
            Add(new Person("John", "Quincy", "Adams", new DateTime(2004, 6, 26)));
            Add(new Person("Andrew", string.Empty, "Jackson", new DateTime(2005, 7, 26)));
            Add(new Person("Martin", string.Empty, "Van Buren", new DateTime(2006, 8, 26)));            
            Add(new Person("William", "H.", "Harrison", new DateTime(2007, 9, 26)));
            Add(new Person("John", string.Empty, "Tyler", new DateTime(2008, 10, 26)));
            Add(new Person("George2", string.Empty, "Washington", new DateTime(1999, 1, 26)));
            Add(new Person("John2", string.Empty, "Adams", new DateTime(2000, 2, 26)));
            Add(new Person("Thomas2", string.Empty, "Jefferson", new DateTime(2001, 3, 26)));
            Add(new Person("James2", string.Empty, "Madison", new DateTime(2002, 4, 26)));
            Add(new Person("James2", string.Empty, "Monroe", new DateTime(2003, 5, 26)));
            Add(new Person("John2", "Quincy", "Adams", new DateTime(2004, 6, 26)));
            Add(new Person("Andrew2", string.Empty, "Jackson", new DateTime(2005, 7, 26)));
            Add(new Person("Martin2", string.Empty, "Van Buren", new DateTime(2006, 8, 26)));
            Add(new Person("William2", "H.", "Harrison", new DateTime(2007, 9, 26)));
            Add(new Person("John2", string.Empty, "Tyler", new DateTime(2008, 10, 26)));
        }
    }
}
