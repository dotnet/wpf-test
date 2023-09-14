using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using Avalon.Test.ComponentModel;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Windows.Themes;



namespace Microsoft.Test.Controls
{
    /// <summary> 
    /// All DependencyProperty tests
    /// </summary> 

    [Test(0, "DataGrid", "DataGridDependencyPropertyTests", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords = "Localization_Suite")]
    public class DataGridDependencyPropertyTests : AvalonTest
    {
        #region Private Fields

        private DataGrid dataGrid;
        private DataGridRow dataGridRow;
        private DataGridColumn dataGridColumn;    
        private DataGridCell dataGridCell;        
        private DataGridRowHeader dataGridRowHeader;
        private DataGridColumnHeader dataGridColumnHeader;
        private DataGridBoundColumn dataGridBoundColumn;
        private DataGridTemplateColumn dataGridTemplateColumn;
        private DPPropertyLocal local;

        #endregion
                
        #region Public Enum

        public enum DObject
        { 
            DataGrid, 
            DataGridRow,
            DataGridColumn,
            DataGridCell,
            DataGridTemplateColumn,
            DataGridRowHeader,
            DataGridColumnHeader            
        }

        #endregion
        
        #region Constructor

        public DataGridDependencyPropertyTests()
        {
            InitializeSteps += new TestStep(Setup);

            //******************
            // DataGrid
            //******************
            RunSteps += new TestStep(DataGrid_AlternatingRowBackground);
            RunSteps += new TestStep(DataGrid_AreRowDetailsFrozen);       
            RunSteps += new TestStep(DataGrid_AutoGenerateColumns);
            //RunSteps += new TestStep(DataGrid_CanUserAddRows);
            //RunSteps += new TestStep(DataGrid_CanUserDeleteRows);
            RunSteps += new TestStep(DataGrid_CanUserReorderColumns);
            RunSteps += new TestStep(DataGrid_CanUserResizeColumns);
            RunSteps += new TestStep(DataGrid_CanUserSortColumns);
            RunSteps += new TestStep(DataGrid_CellStyle);
            RunSteps += new TestStep(DataGrid_ClipboardCopyMode);
            RunSteps += new TestStep(DataGrid_ColumnHeaderHeight);              
            RunSteps += new TestStep(DataGrid_ColumnHeaderStyle);
            RunSteps += new TestStep(DataGrid_ColumnWidth); 
            RunSteps += new TestStep(DataGrid_CurrentCell);
            RunSteps += new TestStep(DataGrid_CurrentColumn);
            RunSteps += new TestStep(DataGrid_CurrentItem);                                
            RunSteps += new TestStep(DataGrid_DragIndicatorStyle);
            RunSteps += new TestStep(DataGrid_DropLocationIndicatorStyle);
            RunSteps += new TestStep(DataGrid_FrozenColumnCount);
            RunSteps += new TestStep(DataGrid_GridLineVisibility);
            RunSteps += new TestStep(DataGrid_HeadersVisibility);               
            RunSteps += new TestStep(DataGrid_HorizontalGridLinesBrush);
            RunSteps += new TestStep(DataGrid_HorizontalScrollBarVisibility);
            //RunSteps += new TestStep(DataGrid_HorizontalGridLineThickness); 
            RunSteps += new TestStep(DataGrid_IsReadOnly);                    
            RunSteps += new TestStep(DataGrid_MaxColumnWidth);
            RunSteps += new TestStep(DataGrid_MinColumnWidth);
            RunSteps += new TestStep(DataGrid_MinRowHeight);
            RunSteps += new TestStep(DataGrid_NonFrozenColumnsViewportHorizontalOffset);
            RunSteps += new TestStep(DataGrid_RowBackground);
            RunSteps += new TestStep(DataGrid_RowDetailsTemplate);
            RunSteps += new TestStep(DataGrid_RowDetailsTemplateSelector);  
            RunSteps += new TestStep(DataGrid_RowDetailsVisibilityMode);    
            RunSteps += new TestStep(DataGrid_RowHeaderStyle);
            RunSteps += new TestStep(DataGrid_RowHeaderWidth);
            RunSteps += new TestStep(DataGrid_RowHeight);
            RunSteps += new TestStep(DataGrid_RowStyle);
            RunSteps += new TestStep(DataGrid_RowStyleSelector);
            RunSteps += new TestStep(DataGrid_RowValidationErrorTemplate);
            RunSteps += new TestStep(DataGrid_SelectionMode);
            RunSteps += new TestStep(DataGrid_SelectionUnit);
            RunSteps += new TestStep(DataGrid_VerticalGridLinesBrush);
            //RunSteps += new TestStep(DataGrid_VerticalGridLineThickness);
            RunSteps += new TestStep(DataGrid_VerticalScrollBarVisibility);

            //*******************
            // DataGridRow
            //*******************                       
            RunSteps += new TestStep(DataGridRow_DetailsTemplate);
            RunSteps += new TestStep(DataGridRow_DetailsTemplateSelector);
            RunSteps += new TestStep(DataGridRow_DetailsVisibility);            
            RunSteps += new TestStep(DataGridRow_Header);
            RunSteps += new TestStep(DataGridRow_HeaderStyle);        
            RunSteps += new TestStep(DataGridRow_HeaderTemplate);
            RunSteps += new TestStep(DataGridRow_HeaderTemplateSelector);  
            RunSteps += new TestStep(DataGridRow_IsEditing);
            RunSteps += new TestStep(DataGridRow_IsSelected);
            RunSteps += new TestStep(DataGridRow_Item);
            RunSteps += new TestStep(DataGridRow_ItemsPanel);
            RunSteps += new TestStep(DataGridRow_ValidationErrorTemplate);

            //*******************
            // DataGridColumn
            //*******************
            RunSteps += new TestStep(DataGridColumn_ActualWidth);
            RunSteps += new TestStep(DataGridColumn_CanUserSort);    
            RunSteps += new TestStep(DataGridColumn_CanUserReorder);
            RunSteps += new TestStep(DataGridColumn_CanUserResize);
            RunSteps += new TestStep(DataGridColumn_CellStyle);
            RunSteps += new TestStep(DataGridColumn_DisplayIndex);      
            RunSteps += new TestStep(DataGridColumn_DragIndicatorStyle);
            RunSteps += new TestStep(DataGridColumn_Header);
            RunSteps += new TestStep(DataGridColumn_HeaderStringFormat);
            RunSteps += new TestStep(DataGridColumn_HeaderStyle);
            RunSteps += new TestStep(DataGridColumn_HeaderTemplate);
            RunSteps += new TestStep(DataGridColumn_HeaderTemplateSelector);  
            RunSteps += new TestStep(DataGridColumn_IsAutoGenerated);
            RunSteps += new TestStep(DataGridColumn_IsFrozen);
            RunSteps += new TestStep(DataGridColumn_IsReadOnly);  
            RunSteps += new TestStep(DataGridColumn_MaxWidth);
            RunSteps += new TestStep(DataGridColumn_MinWidth);
            RunSteps += new TestStep(DataGridColumn_SortDirection);
            RunSteps += new TestStep(DataGridColumn_SortMemberPath);
            RunSteps += new TestStep(DataGridColumn_Width);
            
            //*****************
            // DataGridCell
            //*****************
            RunSteps += new TestStep(DataGridCell_Column);
            RunSteps += new TestStep(DataGridCell_IsEditing);
            RunSteps += new TestStep(DataGridCell_IsReadOnly); 
            RunSteps += new TestStep(DataGridCell_IsSelected);

            //**************************
            // DataGridBoundColumn
            //**************************
            RunSteps += new TestStep(DataGridBoundColumn_ElementStyle);
            RunSteps += new TestStep(DataGridBoundColumn_EditingElementStyle);

            //**************************
            // DataGridTemplateColumn
            //**************************
            RunSteps += new TestStep(DataGridTemplateColumn_CellEditingTemplate);
            RunSteps += new TestStep(DataGridTemplateColumn_CellTemplate);
            RunSteps += new TestStep(DataGridTemplateColumn_CellEditingTemplateSelector);
            RunSteps += new TestStep(DataGridTemplateColumn_CellTemplateSelector);

            //*********************
            // DataGridRowHeader
            //*********************
            RunSteps += new TestStep(DataGridRowHeader_IsRowSelected);

            //***********************
            // DataGridColumnHeader
            //***********************
            RunSteps += new TestStep(DataGridColumnHeader_CanUserSort);
            RunSteps += new TestStep(DataGridColumnHeader_DisplayIndex);
            RunSteps += new TestStep(DataGridColumnHeader_IsFrozen);
            RunSteps += new TestStep(DataGridColumnHeader_SortDirection);

            // 
        }
        
        #endregion

        #region Test Steps

        TestResult Setup()
        {
            Status("Setup");
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            dataGrid = new DataGrid();
            Assert.AssertTrue("DataGrid can not be null", dataGrid != null);

            dataGridRow = new DataGridRow();
            Assert.AssertTrue("DataGridRow can not be null", dataGridRow != null);

            dataGridColumn = new DataGridTextColumn() as DataGridColumn;
            Assert.AssertTrue("dataGridColumn can not be null", dataGridColumn != null);

            dataGridCell = new DataGridCell();
            Assert.AssertTrue("DataGridCell can not be null", dataGridCell != null);

            dataGridRowHeader = new DataGridRowHeader();
            Assert.AssertTrue("DataGridRowHeader can not be null", dataGridRowHeader != null);

            dataGridColumnHeader = new DataGridColumnHeader();
            Assert.AssertTrue("DataGridColumnHeader can not be null", dataGridColumnHeader != null);

            dataGridBoundColumn = new DataGridTextColumn() as DataGridBoundColumn; 
            Assert.AssertTrue("DataGridBoundColumn can not be null", dataGridBoundColumn != null);

            dataGridTemplateColumn = new DataGridTemplateColumn();
            Assert.AssertTrue("DataGridTemplateColumn can not be null", dataGridTemplateColumn != null);            
            

            Status("Here are the current DP tests...");
            BindingFlags local = BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            foreach (MethodInfo mi in typeof(DataGridDependencyPropertyTests).GetMethods(local))
            {
                if (mi.ReturnType == typeof(TestResult))
                {
                    Status(mi.Name);
                }
            }

            Status("Here are the DP found for DG... - TODO!");
            IEnumerable<DependencyProperty> dpDataGrid = dataGrid.GetDependencyProperties(); 
            Status(string.Format("The # of DPs in DataGrid is {0}", dpDataGrid.Count()));
            foreach (DependencyProperty dp in dpDataGrid)
            {
                Status("DataGrid DP = " + dp.Name); //.ToString());
            }

            
            LogComment("Setup was successful");
            return TestResult.Pass;
        }

        #region DataGrid DPs

        [Description("Verify Dependency Property: (Brush) DataGrid.AlternatingRowBackgroundProperty.")]
        TestResult DataGrid_AlternatingRowBackground()
        {
            Status("DataGrid_AlternatingRowBackground");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: null
            DataGridVerificationHelper.TestDP<DataGrid, Brush>("AlternatingRowBackground", local, "OnNotifyDataGridAndRowPropertyChanged");

            // Verify that we set what we get 
            DataGridVerificationHelper.ValidateBrushValue(dataGrid, DataGrid.AlternatingRowBackgroundProperty);

            LogComment("DataGrid_AlternatingRowBackground was successful");
            return TestResult.Pass;
        }
        
        [Description("Verify Dependency Property: (bool) DataGrid.AreRowDetailsFrozenProperty.")]
        TestResult DataGrid_AreRowDetailsFrozen()  
        {
            Status("DataGrid_AreRowDetailsFrozen");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = false;
            // default false: 
            DataGridVerificationHelper.TestDP<DataGrid, bool>("AreRowDetailsFrozen", local, "OnAreRowDetailsFrozenPropertyChanged");

            // Verify that we set what we get 
            DataGridVerificationHelper.ValidateBoolenValue(dataGrid, DataGrid.AreRowDetailsFrozenProperty);

            LogComment("DataGrid_AreRowDetailsFrozen was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (bool) DataGrid.AutoGenerateColumns.")]
        TestResult DataGrid_AutoGenerateColumns()
        {
            Status("DataGrid_AutoGenerateColumns");
            
            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: true;
            DataGridVerificationHelper.TestDP<DataGrid, bool>("AutoGenerateColumns", local, "OnAutoGenerateColumnsPropertyChanged");

            // Verify that we set what we get 
            DataGridVerificationHelper.ValidateBoolenValue(dataGrid, DataGrid.AutoGenerateColumnsProperty);

            LogComment("DataGrid_AutoGenerateColumns was successful");
            return TestResult.Pass;
        }
        
        [Description("Verify Dependency Property: (bool) DataGrid.CanUserAddRowsProperty.")]
        TestResult DataGrid_CanUserAddRows()
        {
            Status("DataGrid_CanUserAddRows");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: true;
            DataGridVerificationHelper.TestDP<DataGrid, bool>("CanUserAddRows", local, "OnCanUserAddRowsChanged");

            // set the DS
            ObservableCollection<int> data = new ObservableCollection<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            dataGrid.ItemsSource = data;
            WaitForPriority(DispatcherPriority.Background);
            
            // Verify that we set what we get 
            DataGridVerificationHelper.ValidateBoolenValue(dataGrid, DataGrid.CanUserAddRowsProperty);

            dataGrid.ClearValue(DataGrid.ItemsSourceProperty);
            WaitForPriority(DispatcherPriority.Background);

            LogComment("DataGrid_CanUserAddRows was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (bool) DataGrid.CanUserDeleteRowsProperty.")]
        TestResult DataGrid_CanUserDeleteRows()
        {
            Status("DataGrid_CanUserDeleteRows");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: true;
            DataGridVerificationHelper.TestDP<DataGrid, bool>("CanUserDeleteRows", local, "OnCanUserDeleteRowsChanged");

            // set the DS
            ObservableCollection<int> data = new ObservableCollection<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            dataGrid.ItemsSource = data;
            WaitForPriority(DispatcherPriority.Background);

            // Verify that we set what we get 
            DataGridVerificationHelper.ValidateBoolenValue(dataGrid, DataGrid.CanUserDeleteRowsProperty);

            // reset the DS
            dataGrid.ClearValue(DataGrid.ItemsSourceProperty);
            WaitForPriority(DispatcherPriority.Background);

            LogComment("DataGrid_CanUserDeleteRows was successful");
            return TestResult.Pass;        
        }

        [Description("Verify Dependency Property: (bool) DataGrid.CanUserReorderColumnsProperty.")]
        TestResult DataGrid_CanUserReorderColumns()
        { 
            Status("DataGrid_CanUserReorderColumns");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = false;
            // default: true;
            DataGridVerificationHelper.TestDP<DataGrid, bool>("CanUserReorderColumns", local, "OnCanUserReorderColumnsPropertyChanged");

            // Verify that we set what we get 
            DataGridVerificationHelper.ValidateBoolenValue(dataGrid, DataGrid.CanUserReorderColumnsProperty);

            LogComment("DataGrid_CanUserReorderColumns was successful");
            return TestResult.Pass;        
        }

        [Description("Verify Dependency Property: (bool) DataGrid.CanUserResizeColumnsProperty.")]
        TestResult DataGrid_CanUserResizeColumns()
        {
            Status("DataGrid_CanUserResizeColumns");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: true
            DataGridVerificationHelper.TestDP<DataGrid, bool>("CanUserResizeColumns", local, "OnNotifyColumnHeaderPropertyChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateBoolenValue(dataGrid, DataGrid.CanUserResizeColumnsProperty);

            LogComment("DataGrid_CanUserResizeColumns was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (bool) DataGrid.CanUserSortColumnsProperty.")]
        TestResult DataGrid_CanUserSortColumns()
        {
            Status("DataGrid_CanUserSortColumns");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: true
            DataGridVerificationHelper.TestDP<DataGrid, bool>("CanUserSortColumns", local, "OnCanUserSortColumnsPropertyChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateBoolenValue(dataGrid, DataGrid.CanUserSortColumnsProperty);

            LogComment("DataGrid_CanUserSortColumns was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (Style) DataGrid.CellStyleProperty.")]
        TestResult DataGrid_CellStyle()
        {
            Status("DataGrid_CellStyle");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            DataGridVerificationHelper.TestDP<DataGrid, Style>("CellStyle", local, "OnNotifyColumnAndCellPropertyChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateStyleValue(dataGrid, DataGrid.CellStyleProperty);

            LogComment("DataGrid_CellStyle was successful");
            return TestResult.Pass;
        }
        
        [Description("Verify Dependency Property: (DataGridClipboardCopyMode) DataGrid.ClipboardCopyModeProperty.")]
        TestResult DataGrid_ClipboardCopyMode()
        {
            Status("DataGrid_ClipboardCopyMode");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            DataGridVerificationHelper.TestDP<DataGrid, DataGridClipboardCopyMode>("ClipboardCopyMode", local, "OnClipboardCopyModeChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateEnumValue(dataGrid, DataGrid.ClipboardCopyModeProperty, typeof(DataGridClipboardCopyMode));

            LogComment("DataGrid_ClipboardCopyMode was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (double) DataGrid.ColumnHeaderHeight.")]
        TestResult DataGrid_ColumnHeaderHeight()  
        {
            Status("DataGrid_ColumnHeaderHeight");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = false;
            DataGridVerificationHelper.TestDP<DataGrid, double>("ColumnHeaderHeight", local, "OnColumnHeaderHeightPropertyChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateDoubleValue(dataGrid, DataGrid.ColumnHeaderHeightProperty);

            LogComment("DataGrid_ColumnHeaderHeight was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (Style) DataGrid.ColumnHeaderStyleProperty.")]
        TestResult DataGrid_ColumnHeaderStyle() 
        {
            Status("DataGrid_ColumnHeaderStyle");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            //default: null
            DataGridVerificationHelper.TestDP<DataGrid, Style>("ColumnHeaderStyle", local, "OnNotifyColumnHeaderPropertyChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateStyleValue(dataGrid, DataGrid.ColumnHeaderStyleProperty);

            LogComment("DataGrid_ColumnHeaderStyle was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (DataGridCellInfo) DataGrid.CurrentCellProperty.")]
        TestResult DataGrid_CurrentCell() 
        {
            Status("DataGrid_CurrentCell");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            //default: DataGridCellInfo.Unset;
            DataGridVerificationHelper.TestDP<DataGrid, DataGridCellInfo>("CurrentCell", local, "OnCurrentCellChanged");

            // Verify that we set what we get
            DataGridCellInfo cellinfo = new DataGridCellInfo();
            dataGrid.CurrentCell = cellinfo;
            Assert.AssertTrue("The CurrentCell did not set", cellinfo == dataGrid.CurrentCell);

            LogComment("DataGrid_CurrentCell was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (DataGridColumn) DataGrid.CurrentColumnProperty.")]
        TestResult DataGrid_CurrentColumn() 
        {
            Status("DataGrid_CurrentColumn");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            //local.defaultValue = null; 
            DataGridVerificationHelper.TestDP<DataGrid, DataGridColumn>("CurrentColumn", local, "OnCurrentColumnChanged");

            // Verify that we set what we get
            DataGridTextColumn column = new DataGridTextColumn();
            dataGrid.CurrentColumn = column;
            Assert.AssertTrue("The CurrentColumn did not set", column == dataGrid.CurrentColumn);

            LogComment("DataGrid_CurrentColumn was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (object) DataGrid.CurrentItemProperty.")]
        TestResult DataGrid_CurrentItem() 
        {
            Status("DataGrid_CurrentItem");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            //local.defaultValue = null;
            DataGridVerificationHelper.TestDP<DataGrid, object>("CurrentItem", local, "OnCurrentItemChanged");

            // Verify that we set what we get
            object item = new object();
            dataGrid.CurrentItem = item;
            Assert.AssertTrue("The CurrentItem did not set", item == dataGrid.CurrentItem);

            LogComment("DataGrid_CurrentItem was successful");
            return TestResult.Pass;
        }
        
        [Description("Verify Dependency Property: (DataGridLength) DataGrid.ColumnWidthProperty.")]
        TestResult DataGrid_ColumnWidth()  
        {
            Status("DataGrid_ColumnWidth");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = false;
            DataGridVerificationHelper.TestDP<DataGrid, DataGridLength>("ColumnWidth", local, "OnColumnWidthPropertyChanged");

            // Verify that we set what we get 
            DataGridVerificationHelper.ValidateDataGridLengthValue(dataGrid, DataGrid.ColumnWidthProperty);

            LogComment("DataGrid_ColumnWidth was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (Style) DataGrid.DragIndicatorStyleProperty.")]
        TestResult DataGrid_DragIndicatorStyle()
        {
            Status("DataGrid_DragIndicatorStyle");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = false;
            // default: null;
            DataGridVerificationHelper.TestDP<DataGrid, Style>("DragIndicatorStyle", local, "OnDragIndicatorStylePropertyChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateStyleValue(dataGrid, DataGrid.DragIndicatorStyleProperty);

            LogComment("DataGrid_DragIndicatorStyle was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (Style) DataGrid.DragIndicatorStyleProperty.")]
        TestResult DataGrid_DropLocationIndicatorStyle()
        {
            Status("DataGrid_DropLocationIndicatorStyle");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = false;
            // default: null;
            DataGridVerificationHelper.TestDP<DataGrid, Style>("DropLocationIndicatorStyle", local, "OnDropLocationIndicatorStylePropertyChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateStyleValue(dataGrid, DataGrid.DropLocationIndicatorStyleProperty);

            LogComment("DataGrid_DropLocationIndicatorStyle was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (int) DataGrid.FrozenColumnCountProperty.")]
        TestResult DataGrid_FrozenColumnCount()
        { 
            Status("DataGrid_FrozenColumnCount");

            local.expectGet = true;
            local.expectSet = true;  
            local.hasCallBack = true;
            // default: 0
            DataGridVerificationHelper.TestDP<DataGrid, int>("FrozenColumnCount", local, "OnFrozenColumnCountPropertyChanged");

            // initialize the columns
            DataGridColumn col = new DataGridTextColumn();            
            DataGridColumn col1 = new DataGridTextColumn();            
            DataGridColumn col2 = new DataGridTextColumn();            
            DataGridColumn col3 = new DataGridTextColumn();            
            DataGridColumn col4 = new DataGridTextColumn();            
            DataGridColumn col5 = new DataGridTextColumn();  
            dataGrid.Columns.Add(col);
            dataGrid.Columns.Add(col1);
            dataGrid.Columns.Add(col2);
            dataGrid.Columns.Add(col3);
            dataGrid.Columns.Add(col4);
            dataGrid.Columns.Add(col5);
            WaitForPriority(DispatcherPriority.Background);

            // Verify that we set what we get
            dataGrid.FrozenColumnCount = 3;
            Assert.AssertTrue("The FrozenColumnCount should be 3", (int)dataGrid.GetValue(DataGrid.FrozenColumnCountProperty) == 3);

            dataGrid.FrozenColumnCount = 0;
            Assert.AssertTrue("The FrozenColumnCount should be 3", (int)dataGrid.GetValue(DataGrid.FrozenColumnCountProperty) == 0);

            // reset 
            dataGrid.Columns.Remove(col);
            dataGrid.Columns.Remove(col1);
            dataGrid.Columns.Remove(col2);
            dataGrid.Columns.Remove(col3);
            dataGrid.Columns.Remove(col4);
            dataGrid.Columns.Remove(col5);
            WaitForPriority(DispatcherPriority.Background);

            LogComment("DataGrid_FrozenColumnCount was successful");
            return TestResult.Pass;
        }
                        
        [Description("Verify Dependency Property: (DataGridGridLinesVisibility) DataGrid.GridLinesVisibilityProperty.")]
        TestResult DataGrid_GridLineVisibility()  // 
        {
            Status("DataGrid_GridLineVisibility");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: DataGridGridLinesVisibility.All
            DataGridVerificationHelper.TestDP<DataGrid, DataGridGridLinesVisibility>("GridLinesVisibility", local, "OnNotifyGridLinePropertyChanged");
            
            // Verify that we set what we get 
            DataGridVerificationHelper.ValidateEnumValue(dataGrid, DataGrid.GridLinesVisibilityProperty, typeof(DataGridGridLinesVisibility));

            LogComment("DataGrid_GridLineVisibility was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (double) DataGrid.HorizontalGridLinesThicknessProperty.")]
        TestResult DataGrid_HorizontalGridLineThickness() // 
        { 
            Status("DataGrid_HorizontalGridLineThickness");

            //local.expectGet = true;
            //local.expectSet = true;
            //local.hasCallBack = true;
            //// default: 1d;
            //DataGridVerificationHelper.TestDP<DataGrid, double>("HorizontalGridLinesThickness", local, "OnNotifyGridLinePropertyChanged");

            //// Verify that we set what we get
            //DataGridVerificationHelper.ValidateDoubleValue(dataGrid, DataGrid.HorizontalGridLinesThicknessProperty);

            LogComment("DataGrid_HorizontalGridLineThickness was successful");
            return TestResult.Pass;      
        }

        [Description("Verify Dependency Property: (DataGridHeadersVisibility) DataGrid.HeadersVisibilityProperty.")]
        TestResult DataGrid_HeadersVisibility() 
        {
            Status("DataGrid_HeadersVisibility");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = false;
            // default: DataGridHeadersVisibility.All
            DataGridVerificationHelper.TestDP<DataGrid, DataGridHeadersVisibility>("HeadersVisibility", local, "OnHeadersVisibilityPropertyChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateEnumValue(dataGrid, DataGrid.HeadersVisibilityProperty, typeof(DataGridHeadersVisibility));

            LogComment("DataGrid_HeadersVisibility was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (Brush) DataGrid.HorizontalGridLinesBrushProperty.")]
        TestResult DataGrid_HorizontalGridLinesBrush() 
        {
            Status("DataGrid_HorizontalGridLinesBrush");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: Brushes.Black
            DataGridVerificationHelper.TestDP<DataGrid, Brush>("HorizontalGridLinesBrush", local, "OnNotifyGridLinePropertyChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateBrushValue(dataGrid, DataGrid.HorizontalGridLinesBrushProperty);

            LogComment("DataGrid_HorizontalGridLinesBrush was successful");
            return TestResult.Pass;
        }
                
        [Description("Verify Dependency Property: (ScrollBarVisibility) DataGrid.HorizontalScrollBarVisibility.")]
        TestResult DataGrid_HorizontalScrollBarVisibility()
        {
            Status("DataGrid_HorizontalScrollBarVisibility");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = false;
            // inherited
            DataGridVerificationHelper.TestDP<DataGrid, ScrollBarVisibility>("HorizontalScrollBarVisibility", local, "OnHorizontalScrollBarVisibilityPropertyChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateEnumValue(dataGrid, DataGrid.HorizontalScrollBarVisibilityProperty, typeof(ScrollBarVisibility));

            LogComment("DataGrid_HorizontalScrollBarVisibility was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (bool) DataGrid.IsReadOnlyProperty.")]
        TestResult DataGrid_IsReadOnly()
        {
            Status("DataGrid_IsReadOnly");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: false;
            DataGridVerificationHelper.TestDP<DataGrid, bool>("IsReadOnly", local, "OnIsReadOnlyChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateBoolenValue(dataGrid, DataGrid.IsReadOnlyProperty);

            LogComment("DataGrid_IsReadOnly was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (double) DataGrid.MaxColumnWidthProperty.")]
        TestResult DataGrid_MaxColumnWidth()
        {
            Status("DataGrid_MaxColumnWidth");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: double.PositiveInfinity;
            DataGridVerificationHelper.TestDP<DataGrid, double>("MaxColumnWidth", local, "OnColumnSizeConstraintChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateDoubleValue(dataGrid, DataGrid.MaxColumnWidthProperty);

            LogComment("DataGrid_MaxColumnWidth was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (double) DataGrid.MinColumnWidthProperty.")]
        TestResult DataGrid_MinColumnWidth()
        {
            Status("DataGrid_MinColumnWidth");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: 20d;
            DataGridVerificationHelper.TestDP<DataGrid, double>("MinColumnWidth", local, "OnColumnSizeConstraintChanged");

            // test the set/get
            dataGrid.SetValue(DataGrid.MinColumnWidthProperty, 0.0);
            Assert.AssertEqual(string.Format("Error in setting the DP {0} to 0.0", DataGrid.MinColumnWidthProperty.ToString()), 0.0, dataGrid.GetValue(DataGrid.MinColumnWidthProperty));

            dataGrid.SetValue(DataGrid.MinColumnWidthProperty, 25.0);
            Assert.AssertEqual(string.Format("Error in setting the DP {0} to 25.0", DataGrid.MinColumnWidthProperty.ToString()), 25.0, dataGrid.GetValue(DataGrid.MinColumnWidthProperty));

            LogComment("DataGrid_MinColumnWidth was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (double) DataGrid.MinRowHeightProperty.")]
        TestResult DataGrid_MinRowHeight()
        {
            Status("DataGrid_MinRowHeight");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: 0.0;
            DataGridVerificationHelper.TestDP<DataGrid, double>("MinRowHeight", local, "OnNotifyRowPropertyChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateDoubleValue(dataGrid, DataGrid.MinRowHeightProperty);

            LogComment("DataGrid_MinRowHeight was successful");
            return TestResult.Pass;        
        }
        
        [Description("Verify Dependency Property: (double) DataGrid.NonFrozenColumnsViewportHorizontalOffsetProperty.")]
        TestResult DataGrid_NonFrozenColumnsViewportHorizontalOffset()
        { 
            Status("DataGrid_NonFrozenColumnsViewportHorizontalOffset");

            local.expectGet = true;
            local.expectSet = true; // internal set
            local.hasCallBack = false;
            // default: 0.0;
            DataGridVerificationHelper.TestDP<DataGrid, double>("NonFrozenColumnsViewportHorizontalOffset", local, "OnNonFrozenColumnsViewportHorizontalOffsetPropertyChanged");

            LogComment("DataGrid_NonFrozenColumnsViewportHorizontalOffset was successful");
            return TestResult.Pass;    
        }
        
        [Description("Verify Dependency Property: (Brush) DataGrid.RowBackgroundProperty.")]
        TestResult DataGrid_RowBackground()
        {
            Status("DataGrid_RowBackground");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: null
            DataGridVerificationHelper.TestDP<DataGrid, Brush>("RowBackground", local, "OnNotifyRowPropertyChanged");

            // Verify that we set what we get 
            DataGridVerificationHelper.ValidateBrushValue(dataGrid, DataGrid.RowBackgroundProperty);

            LogComment("DataGrid_AlternatingRowBackground was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (DataTemplate) DataGrid.RowDetailsTemplateProperty.")]
        TestResult DataGrid_RowDetailsTemplate()  
        {
            Status("DataGrid_RowDetailsTemplate");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: null;
            DataGridVerificationHelper.TestDP<DataGrid, DataTemplate>("RowDetailsTemplate", local, "OnNotifyRowAndDetailsPropertyChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateDataTemplateValue(dataGrid, DataGrid.RowDetailsTemplateProperty);

            LogComment("DataGrid_RowDetailsTemplate was successful");
            return TestResult.Pass;    
        }

        [Description("Verify Dependency Property: (DataTemplateSelector) DataGrid.RowDetailsTemplateSelectorProperty.")]
        TestResult DataGrid_RowDetailsTemplateSelector()
        {
            Status("DataGrid_RowDetailsTemplateSelector");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: null;
            DataGridVerificationHelper.TestDP<DataGrid, DataTemplateSelector>("RowDetailsTemplateSelector", local, "OnNotifyRowAndDetailsPropertyChanged");

            DataGridVerificationHelper.ValidateDataTemplateSelectorValue(dataGrid, DataGrid.RowDetailsTemplateSelectorProperty);

            LogComment("DataGrid_RowDetailsTemplateSelector was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (DataGridRowDetailsVisibilityMode) DataGrid.RowDetailsVisibilityModeProperty.")]
        TestResult DataGrid_RowDetailsVisibilityMode() 
        {
            Status("DataGrid_RowDetailsVisibilityMode");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: VisibleWhenSelected;
            DataGridVerificationHelper.TestDP<DataGrid, DataGridRowDetailsVisibilityMode>("RowDetailsVisibilityMode", local, "OnNotifyRowAndDetailsPropertyChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateEnumValue(dataGrid, DataGrid.RowDetailsVisibilityModeProperty, typeof(DataGridRowDetailsVisibilityMode));

            LogComment("DataGrid_RowDetailsVisibilityMode was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (Style) DataGrid.RowHeaderStyleProperty.")]
        TestResult DataGrid_RowHeaderStyle()
        {
            Status("DataGrid_RowHeaderStyle");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: null;
            DataGridVerificationHelper.TestDP<DataGrid, Style>("RowHeaderStyle", local, "OnNotifyRowHeaderPropertyChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateStyleValue(dataGrid, DataGrid.RowHeaderStyleProperty);

            LogComment("DataGrid_RowHeaderStyle was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (double) DataGrid.RowHeaderWidthProperty.")]
        TestResult DataGrid_RowHeaderWidth()  
        {
            Status("DataGrid_RowHeaderWidth");  

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: double.NaN;
            DataGridVerificationHelper.TestDP<DataGrid, double>("RowHeaderWidth", local, "OnNotifyRowHeaderWidthPropertyChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateDoubleValue(dataGrid, DataGrid.RowHeaderWidthProperty);

            LogComment("DataGrid_RowHeaderWidth was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (double) DataGrid.RowHeightProperty.")]
        TestResult DataGrid_RowHeight()
        {
            Status("DataGrid_RowHeight");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: double.NaN;
            DataGridVerificationHelper.TestDP<DataGrid, double>("RowHeight", local, "OnNotifyCellsPresenterPropertyChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateDoubleValue(dataGrid, DataGrid.RowHeightProperty);

            LogComment("DataGrid_RowHeight was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (Style) DataGrid.RowStyleProperty.")]
        TestResult DataGrid_RowStyle()
        {
            Status("DataGrid_RowStyle");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: null;
            DataGridVerificationHelper.TestDP<DataGrid, Style>("RowStyle", local, "OnRowStyleChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateStyleValue(dataGrid, DataGrid.RowStyleProperty);

            LogComment("DataGrid_RowHeight was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (StyleSelector) DataGrid.RowStyleSelectorProperty.")]
        TestResult DataGrid_RowStyleSelector()
        {
            Status("DataGrid_RowStyleSelector");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: null;
            DataGridVerificationHelper.TestDP<DataGrid, StyleSelector>("RowStyleSelector", local, "OnRowStyleSelectorChanged");

            // Verify that we set what we get
            StyleSelector styleSelector = new StyleSelector();
            dataGrid.RowStyleSelector = styleSelector;
            Assert.AssertEqual("The RowStyleSelector did not set to styleSelector", styleSelector, dataGrid.RowStyleSelector);

            LogComment("DataGrid_RowStyleSelector was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (ControlTemplate) DataGrid.RowValidationErrorTemplateProperty.")]
        TestResult DataGrid_RowValidationErrorTemplate()
        {
            Status("DataGrid_RowValidationErrorTemplate");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: null;
            DataGridVerificationHelper.TestDP<DataGrid, ControlTemplate>("RowValidationErrorTemplate", local, "OnNotifyRowPropertyChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateControlTemplateValue(dataGrid, DataGrid.RowValidationErrorTemplateProperty);

            LogComment("DataGrid_RowValidationErrorTemplate was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (DataGridSelectionMode) DataGrid.SelectionModeProperty.")]
        TestResult DataGrid_SelectionMode()  // 
        {
            Status("DataGrid_SelectionMode");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: DataGridSelectionMode.Extended;
            DataGridVerificationHelper.TestDP<DataGrid, DataGridSelectionMode>("SelectionMode", local, "OnSelectionModeChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateEnumValue(dataGrid, DataGrid.SelectionModeProperty, typeof(DataGridSelectionMode));

            LogComment("DataGrid_SelectionMode was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (DataGridSelectionUnit) DataGrid.SelectionUnitProperty.")]
        TestResult DataGrid_SelectionUnit()  // 
        {
            Status("DataGrid_SelectionUnit");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: DataGridSelectionUnit.FullRow;
            DataGridVerificationHelper.TestDP<DataGrid, DataGridSelectionUnit>("SelectionUnit", local, "OnSelectionUnitChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateEnumValue(dataGrid, DataGrid.SelectionUnitProperty, typeof(DataGridSelectionUnit));

            LogComment("DataGrid_SelectionUnit was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (Brush) DataGrid.VerticalGridLinesBrushProperty.")]
        TestResult DataGrid_VerticalGridLinesBrush() 
        {
            Status("DataGrid_VerticalGridLinesBrush");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: Brushes.Black;
            DataGridVerificationHelper.TestDP<DataGrid, Brush>("VerticalGridLinesBrush", local, "OnNotifyGridLinePropertyChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateBrushValue(dataGrid, DataGrid.VerticalGridLinesBrushProperty);

            LogComment("DataGrid_VerticalGridLinesBrush was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (double) DataGrid.VerticalGridLinesThicknessProperty.")]
        TestResult DataGrid_VerticalGridLineThickness()
        {
            Status("DataGrid_VerticalGridLineThickness");

            //local.expectGet = true;
            //local.expectSet = true;
            //local.hasCallBack = true;
            //// default: 1d;
            //DataGridVerificationHelper.TestDP<DataGrid, double>("VerticalGridLinesThickness", local, "OnNotifyGridLinePropertyChanged");

            //// Verify that we set what we get
            //DataGridVerificationHelper.ValidateDoubleValue(dataGrid, DataGrid.VerticalGridLinesThicknessProperty);

            LogComment("DataGrid_VerticalGridLineThickness was successful");
            return TestResult.Pass;    
        }
                
        [Description("Verify Dependency Property: (ScrollBarVisibility) DataGrid.VerticalScrollBarVisibility.")]
        TestResult DataGrid_VerticalScrollBarVisibility()
        {
            Status("DataGrid_VerticalScrollBarVisibility");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = false;
            // default: this is an inherited property
            DataGridVerificationHelper.TestDP<DataGrid, ScrollBarVisibility>("VerticalScrollBarVisibility", local, "OnVerticalScrollBarVisibilityPropertyChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateEnumValue(dataGrid, DataGrid.VerticalScrollBarVisibilityProperty, typeof(ScrollBarVisibility));

            LogComment("DataGrid_VerticalScrollBarVisibility was successful");
            return TestResult.Pass;
        }

        #endregion

        #region DataGridRow DPs

        [Description("Verify Dependency Property: (DataTemplate) DataGridRow.DetailsTemplateProperty.")]
        TestResult DataGridRow_DetailsTemplate()
        {
            Status("DataGridRow_DetailsTemplate");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: null;
            DataGridVerificationHelper.TestDP<DataGridRow, DataTemplate>("DetailsTemplate", local, "OnNotifyDetailsTemplatePropertyChanged");

            DataGridVerificationHelper.ValidateDataTemplateValue(dataGridRow, DataGridRow.DetailsTemplateProperty);

            LogComment("DataGridRow_DetailsTemplate was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (DataTemplateSelector) DataGridRow.DetailsTemplateSelectorProperty.")]
        TestResult DataGridRow_DetailsTemplateSelector()
        {
            Status("DataGridRow_DetailsTemplateSelector");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: null;
            DataGridVerificationHelper.TestDP<DataGridRow, DataTemplateSelector>("DetailsTemplateSelector", local, "OnNotifyDetailsTemplatePropertyChanged");

            DataGridVerificationHelper.ValidateDataTemplateSelectorValue(dataGridRow, DataGridRow.DetailsTemplateSelectorProperty);

            LogComment("DataGridRow_DetailsTemplateSelector was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (Visibility) DataGridRow.DetailsVisibilityProperty.")]
        TestResult DataGridRow_DetailsVisibility()
        {
            Status("DataGridRow_DetailsVisibility");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = false;
            // default: Visibility.Collapsed;
            DataGridVerificationHelper.TestDP<DataGridRow, Visibility>("DetailsVisibility", local, "OnHorizontalScrollBarVisibilityPropertyChanged");

            DataGridVerificationHelper.ValidateEnumValue(dataGridRow, DataGridRow.DetailsVisibilityProperty, typeof(Visibility));

            LogComment("DataGridRow_DetailsVisibility was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (object) DataGridRow.HeaderProperty.")]
        TestResult DataGridRow_Header()
        {
            Status("DataGridRow_Header");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: null;
            DataGridVerificationHelper.TestDP<DataGridRow, object>("Header", local, "OnNotifyRowPropertyChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateObjectValue(dataGridRow, DataGridRow.HeaderProperty);

            LogComment("DataGridRow_Header was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (Style) DataGridRow.HeaderStyleProperty.")]
        TestResult DataGridRow_HeaderStyle()  
        {
            Status("DataGridRow_HeaderStyle");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: null
            DataGridVerificationHelper.TestDP<DataGridRow, Style>("HeaderStyle", local, "OnNotifyRowAndRowHeaderPropertyChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateStyleValue(dataGridRow, DataGridRow.HeaderStyleProperty);

            LogComment("DataGridRow_HeaderStyle was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (DataTemplate) DataGridRow.HeaderTemplateProperty.")]
        TestResult DataGridRow_HeaderTemplate() 
        {
            Status("DataGridRow_HeaderTemplate");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: null;
            DataGridVerificationHelper.TestDP<DataGridRow, DataTemplate>("HeaderTemplate", local, "OnNotifyRowAndRowHeaderPropertyChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateDataTemplateValue(dataGridRow, DataGridRow.HeaderTemplateProperty);

            LogComment("DataGridRow_HeaderTemplate was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (DataTemplateSelector) DataGridRow.HeaderTemplateSelectorProperty.")]
        TestResult DataGridRow_HeaderTemplateSelector()
        {
            Status("DataGridRow_HeaderTemplateSelector");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: null;
            DataGridVerificationHelper.TestDP<DataGridRow, DataTemplateSelector>("HeaderTemplateSelector", local, "OnNotifyRowAndRowHeaderPropertyChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateDataTemplateSelectorValue(dataGridRow, DataGridRow.HeaderTemplateSelectorProperty);

            LogComment("DataGridRow_HeaderTemplateSelector was successful");
            return TestResult.Pass;
        }
        
        [Description("Verify Dependency Property: (bool) DataGridRow.IsEditingProperty.")]
        TestResult DataGridRow_IsEditing()
        {
            Status("DataGridRow_IsEditing");

            local.expectGet = true;
            local.expectSet = true; // internal 
            local.hasCallBack = false;
            // default: false;
            DataGridVerificationHelper.TestDP<DataGridCell, bool>("IsEditing", local, "OnIsEditingPropertyChanged");

            LogComment("DataGridRow_IsEditing was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (bool) DataGridRow.IsSelected.")]
        TestResult DataGridRow_IsSelected()
        {
            Status("DataGridRow_IsSelected");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: false;
            DataGridVerificationHelper.TestDP<DataGridCell, bool>("IsSelected", local, "OnIsSelectedChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateBoolenValue(dataGridRow, DataGridRow.IsSelectedProperty);

            LogComment("DataGridRow_IsSelected was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (object) DataGridRow.ItemProperty.")]
        TestResult DataGridRow_Item()
        {
            Status("DataGridRow_Item");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: null;
            DataGridVerificationHelper.TestDP<DataGridRow, object>("Item", local, "OnNotifyRowPropertyChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateObjectValue(dataGridRow, DataGridRow.ItemProperty);

            LogComment("DataGridRow_Item was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (ItemsPanelTemplate) DataGridRow.ItemsPanelProperty.")]
        TestResult DataGridRow_ItemsPanel()
        {
            Status("DataGridRow_ItemsPanel");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = false;
            // default: inherited;
            DataGridVerificationHelper.TestDP<DataGridRow, ItemsPanelTemplate>("ItemsPanel", local, "OnItemsPanelPropertyChanged");

            // Verify that we set what we get
            ItemsPanelTemplate itemsPanel = new ItemsPanelTemplate();
            dataGridRow.ItemsPanel = itemsPanel;
            Assert.AssertEqual("The row ItemsPanel is not set", itemsPanel, dataGridRow.ItemsPanel);

            LogComment("DataGridRow_ItemsPanel was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (ControlTemplate) DataGridRow.ValidationErrorTemplateProperty.")]
        TestResult DataGridRow_ValidationErrorTemplate()
        {
            Status("DataGridRow_ValidationErrorTemplate");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: null;
            DataGridVerificationHelper.TestDP<DataGridRow, ControlTemplate>("ValidationErrorTemplate", local, "OnNotifyRowPropertyChanged");

            DataGridVerificationHelper.ValidateControlTemplateValue(dataGridRow, DataGridRow.ValidationErrorTemplateProperty);

            LogComment("DataGridRow_ValidationErrorTemplate was successful");
            return TestResult.Pass;
        }
                
        #endregion
                
        #region DataGridColumn DPs

        [Description("Verify Dependency Property: (bool) DataGridColumn.CanUserSortProperty.")]
        TestResult DataGridColumn_CanUserSort()  
        {
            Status("DataGridColumn_CanUserSort");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: true;
            DataGridVerificationHelper.TestDP<DataGridColumn, bool>("CanUserSort", local, "OnCanUserSortPropertyChanged");

            DataGridVerificationHelper.ValidateBoolenValue(dataGridColumn, DataGridColumn.CanUserSortProperty);

            LogComment("DataGridColumn_CanUserSort was successful");
            return TestResult.Pass;        
        }

        [Description("Verify Dependency Property: (bool) DataGridColumn.CanUserReorderProperty.")]
        TestResult DataGridColumn_CanUserReorder()
        {
            Status("DataGridColumn_CanUserReorder");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: true;
            DataGridVerificationHelper.TestDP<DataGridColumn, bool>("CanUserReorder", local, "OnNotifyColumnPropertyChanged");

            DataGridVerificationHelper.ValidateBoolenValue(dataGridColumn, DataGridColumn.CanUserSortProperty);

            LogComment("DataGridColumn_CanUserReorder was successful");
            return TestResult.Pass;
        }

            // CanUserResize - bool
            // CellValue - object 
            // OriginalValue - object
            //RunSteps += new TestStep(DataGridColumn_CellStyle);
            //RunSteps += new TestStep(DataGridColumn_CellValue);

        [Description("Verify Dependency Property: (bool) DataGridColumn.CanUserResizeProperty.")]
        TestResult DataGridColumn_CanUserResize()
        {
            Status("DataGridColumn_CanUserResize");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: true;
            DataGridVerificationHelper.TestDP<DataGridColumn, bool>("CanUserResize", local, "OnNotifyColumnHeaderPropertyChanged");

            DataGridVerificationHelper.ValidateBoolenValue(dataGridColumn, DataGridColumn.CanUserResizeProperty);

            LogComment("DataGridColumn_CanUserResize was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (Style) DataGridColumn.CellStyleProperty.")]
        TestResult DataGridColumn_CellStyle()
        {
            Status("DataGridColumn_CellStyle");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: null;
            DataGridVerificationHelper.TestDP<DataGridColumn, Style>("CellStyle", local, "OnNotifyCellPropertyChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateStyleValue(dataGridColumn, DataGridColumn.CellStyleProperty);

            LogComment("DataGridColumn_CellStyle was successful");
            return TestResult.Pass;        
        }

        [Description("Verify Dependency Property: (int) DataGridColumn.DisplayIndexProperty.")]
        TestResult DataGridColumn_DisplayIndex() 
        {
            Status("DataGridColumn_DisplayIndex");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: -1;
            DataGridVerificationHelper.TestDP<DataGridColumn, int>("DisplayIndex", local, "DisplayIndexChanged"); // this is a new dev code change

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateIntValue(dataGridColumn, DataGridColumn.DisplayIndexProperty);

            LogComment("DataGridColumn_DisplayIndex was successful");
            return TestResult.Pass;        
        }

        [Description("Verify Dependency Property: (Style) DataGridColumn.DragIndicatorStyleProperty.")]
        TestResult DataGridColumn_DragIndicatorStyle()
        {
            Status("DataGridColumn_DragIndicatorStyle");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = false;
            // default: null;
            DataGridVerificationHelper.TestDP<DataGridColumn, Style>("DragIndicatorStyle", local, "OnDragIndicatorStylePropertyChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateStyleValue(dataGridColumn, DataGridColumn.HeaderStyleProperty);

            LogComment("DataGridColumn_DragIndicatorStyle was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (object) DataGridColumn.HeaderProperty.")]
        TestResult DataGridColumn_Header()
        {
            Status("DataGridColumn_Header");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: null;
            DataGridVerificationHelper.TestDP<DataGridColumn, object>("Header", local, "OnNotifyColumnHeaderPropertyChanged");
            
            // Verify that we set what we get
            DataGridVerificationHelper.ValidateObjectValue(dataGridColumn, DataGridColumn.HeaderProperty);

            LogComment("DataGridColumn_Header was successful");
            return TestResult.Pass;        
        }

        [Description("Verify Dependency Property: (string) DataGridColumn.HeaderStringFormatProperty.")]
        TestResult DataGridColumn_HeaderStringFormat()
        {
            Status("DataGridColumn_HeaderStringFormat");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: null;
            DataGridVerificationHelper.TestDP<DataGridColumn, string>("HeaderStringFormat", local, "OnNotifyColumnHeaderPropertyChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateStringValue(dataGridColumn, DataGridColumn.HeaderStringFormatProperty);

            LogComment("DataGridColumn_HeaderStringFormat was successful");
            return TestResult.Pass;        
        }

        [Description("Verify Dependency Property: (Style) DataGridColumn.HeaderStyleProperty.")]
        TestResult DataGridColumn_HeaderStyle()
        {
            Status("DataGridColumn_HeaderStyle");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: null;
            DataGridVerificationHelper.TestDP<DataGridColumn, Style>("HeaderStyle", local, "OnNotifyColumnHeaderPropertyChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateStyleValue(dataGridColumn, DataGridColumn.HeaderStyleProperty);

            LogComment("DataGridColumn_HeaderStyle was successful");
            return TestResult.Pass;              
        }

        [Description("Verify Dependency Property: (DataTemplate) DataGridColumn.HeaderTemplateProperty.")]
        TestResult DataGridColumn_HeaderTemplate()
        {
            Status("DataGridColumn_HeaderTemplate");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: null;
            DataGridVerificationHelper.TestDP<DataGridColumn, DataTemplate>("HeaderTemplate", local, "OnNotifyColumnHeaderPropertyChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateDataTemplateValue(dataGridColumn, DataGridColumn.HeaderTemplateProperty);

            LogComment("DataGridColumn_HeaderTemplate was successful");
            return TestResult.Pass;        
        }

        [Description("Verify Dependency Property: (DataTemplateSelector) DataGridColumn.HeaderTemplateSelectorProperty.")]
        TestResult DataGridColumn_HeaderTemplateSelector()
        {
            Status("DataGridColumn_HeaderTemplateSelector");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: null;
            DataGridVerificationHelper.TestDP<DataGridColumn, DataTemplateSelector>("HeaderTemplateSelector", local, "OnNotifyColumnHeaderPropertyChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateDataTemplateSelectorValue(dataGridColumn, DataGridColumn.HeaderTemplateSelectorProperty);

            LogComment("DataGridColumn_HeaderTemplateSelector was successful");
            return TestResult.Pass;
        }
        
        [Description("Verify Dependency Property: (bool) DataGridColumn.IsAutoGeneratedProperty.")]
        TestResult DataGridColumn_IsAutoGenerated()
        {
            Status("DataGridColumn_IsAutoGenerated");

            local.expectGet = true;
            local.expectSet = true;  // internal set
            local.hasCallBack = false;
            // default: false;
            DataGridVerificationHelper.TestDP<DataGridColumn, bool>("IsAutoGenerated", local, "OnIsAutoGeneratedPropertyChanged");

            LogComment("DataGridColumn_IsAutoGenerated was successful");
            return TestResult.Pass;        
        }

        [Description("Verify Dependency Property: (bool) DataGridColumn.IsFrozenProperty.")]
        TestResult DataGridColumn_IsFrozen()
        {
            Status("DataGridColumn_IsFrozen");

            local.expectGet = true;
            local.expectSet = true; // internal
            local.hasCallBack = false;
            // default: false;
            DataGridVerificationHelper.TestDP<DataGridColumn, bool>("IsFrozen", local, "OnIsFrozenPropertyChanged");

            LogComment("DataGridColumn_IsFrozen was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (bool) DataGridColumn.IsReadOnlyProperty.")]
        TestResult DataGridColumn_IsReadOnly()
        {
            Status("DataGridColumn_IsReadOnly");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: false;
            DataGridVerificationHelper.TestDP<DataGridColumn, bool>("IsReadOnly", local, "OnNotifyCellPropertyChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateBoolenValue(dataGridColumn, DataGridColumn.IsReadOnlyProperty);

            LogComment("DataGridColumn_IsReadOnly was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (double) DataGridColumn.MaxWidthProperty.")]
        TestResult DataGridColumn_MaxWidth()
        {
            Status("DataGridColumn_MaxWidth");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: PositiveInfinity;
            DataGridVerificationHelper.TestDP<DataGridColumn, double>("MaxWidth", local, "OnMaxWidthPropertyChanged"); // api update

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateDoubleValue(dataGridColumn, DataGridColumn.MaxWidthProperty);

            LogComment("DataGridColumn_MaxWidth was successful");
            return TestResult.Pass;               
        }

        [Description("Verify Dependency Property: (double) DataGridColumn.MinWidthProperty.")]
        TestResult DataGridColumn_MinWidth()
        {
            Status("DataGridColumn_MinWidth");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: 20d;
            DataGridVerificationHelper.TestDP<DataGridColumn, double>("MinWidth", local, "OnMinWidthPropertyChanged");

            // Verify that we set what we get
            dataGridColumn.SetValue(DataGridColumn.MinWidthProperty, 5.0);
            Assert.AssertEqual(string.Format("Error in setting DataGridColumn.MinWidthProperty to 5.0"), 5.0, dataGridColumn.GetValue(DataGridColumn.MinWidthProperty));

            dataGridColumn.SetValue(DataGridColumn.MinWidthProperty, 125.0);
            Assert.AssertEqual(string.Format("Error in setting DataGridColumn.MinWidthProperty to 125.0"), 125.0, dataGridColumn.GetValue(DataGridColumn.MinWidthProperty));

            dataGridColumn.SetValue(DataGridColumn.MinWidthProperty, 5.0);
            Assert.AssertEqual(string.Format("Error in re-setting DataGridColumn.MinWidthProperty to 5.0"), 5.0, dataGridColumn.GetValue(DataGridColumn.MinWidthProperty));

            LogComment("DataGridColumn_MinWidth was successful");
            return TestResult.Pass;         
        }

        [Description("Verify Dependency Property: (string) DataGridColumn.SortMemberPathProperty.")]
        TestResult DataGridColumn_SortMemberPath()
        {
            Status("DataGridColumn_SortMemberPath");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = false;
            // default: string.Empty;
            DataGridVerificationHelper.TestDP<DataGridColumn, string>("SortMemberPath", local, "OnSortMemberPathPropertyChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateStringValue(dataGridColumn, DataGridColumn.SortMemberPathProperty);

            LogComment("DataGridColumn_SortMemberPath was successful");
            return TestResult.Pass;           
        }

        [Description("Verify Dependency Property: (Nullable<ListSortDirection>)DataGridColumn.SortDirectionProperty.")]
        TestResult DataGridColumn_SortDirection()
        {
            Status("DataGridColumn_SortDirection");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: null;
            DataGridVerificationHelper.TestDP<DataGridColumn, Nullable<ListSortDirection>>("SortDirection", local, "OnNotifySortPropertyChanged");

            DataGridVerificationHelper.ValidateSortDirectionValue(dataGridColumn, DataGridColumn.SortDirectionProperty);

            LogComment("DataGridColumn_SortDirection was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (DataGridLength) DataGridColumn.WidthProperty.")]
        TestResult DataGridColumn_Width()
        {
            Status("DataGridColumn_Width");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: DataGridLength.SizeToHeader
            DataGridVerificationHelper.TestDP<DataGridColumn, DataGridLength>("Width", local, "OnWidthPropertyChanged");

            // Verify that we set what we get 
            DataGridVerificationHelper.ValidateDataGridLengthValue(dataGridColumn, DataGridColumn.WidthProperty);

            LogComment("DataGridColumn_Width was successful");
            return TestResult.Pass;        
        }

        [Description("Verify Dependency Property: (double) DataGridColumn.ActualWidthProperty.")]
        TestResult DataGridColumn_ActualWidth()
        {
            Status("DataGridColumn_ActualWidth");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = false;            
            // default: 0.0
            DataGridVerificationHelper.TestDP<DataGridColumn, double>("ActualWidth", local, "OnActualWidthPropertyChanged");

            // Verify that we set what we get
            dataGridColumn.SetValue(DataGridColumn.WidthProperty, new DataGridLength(100.0));
            Assert.AssertEqual(string.Format("Error in setting DataGridColumn.WidthProperty to 100.0"), 100.0, dataGridColumn.GetValue(DataGridColumn.ActualWidthProperty));

            dataGridColumn.SetValue(DataGridColumn.MinWidthProperty, 125.0);
            Assert.AssertEqual(string.Format("Error in setting DataGridColumn.MinWidthProperty to 125.0"), 125.0, dataGridColumn.GetValue(DataGridColumn.ActualWidthProperty));

            dataGridColumn.SetValue(DataGridColumn.MinWidthProperty, 5.0);
            dataGridColumn.SetValue(DataGridColumn.MaxWidthProperty, 50.0);
            Assert.AssertEqual(string.Format("Error in re-setting DataGridColumn.MaxWidthProperty to 50.0"), 50.0, dataGridColumn.GetValue(DataGridColumn.ActualWidthProperty));

            LogComment("DataGridColumn_ActualWidth was successful");
            return TestResult.Pass;
        }

        #endregion
        
        #region DataGridCell DPs

        [Description("Verify Dependency Property: (bool) DataGridCell.IsEditingProperty.")]
        TestResult DataGridCell_IsEditing()
        {
            Status("DataGridCell_IsEditing");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: false;
            DataGridVerificationHelper.TestDP<DataGridCell, bool>("IsEditing", local, "OnIsEditingChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateBoolenValue(dataGridCell, DataGridCell.IsEditingProperty);

            LogComment("DataGridCell_IsEditing was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (bool) DataGridCell.OnNotifyIsReadOnlyChanged.")]
        TestResult DataGridCell_IsReadOnly() 
        {
            Status("DataGridCell_IsReadOnly");

            local.expectGet = true;
            local.expectSet = false;
            local.hasCallBack = true;
            // default: false;
            DataGridVerificationHelper.TestDP<DataGridCell, bool>("IsReadOnly", local, "OnNotifyIsReadOnlyChanged");

            LogComment("DataGridCell_IsReadOnly was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (bool) DataGridCell.IsSelectedProperty.")]
        TestResult DataGridCell_IsSelected()
        {
            Status("DataGridCell_IsSelected");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = true;
            // default: false;
            DataGridVerificationHelper.TestDP<DataGridCell, bool>("IsSelected", local, "OnIsSelectedChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateBoolenValue(dataGridCell, DataGridCell.IsSelectedProperty);

            LogComment("DataGridCell_IsSelected was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (DataGridColumn) DataGridCell.ColumnProperty.")]
        TestResult DataGridCell_Column()
        {
            Status("DataGridCell_Column");

            local.expectGet = true;
            local.expectSet = true;  //internal
            local.hasCallBack = true;
            // default: null
            DataGridVerificationHelper.TestDP<DataGridCell, DataGridColumn>("Column", local, "OnColumnChanged");

            LogComment("DataGridCell_Column was successful");
            return TestResult.Pass;
        }

        #endregion

        #region DataGridBoundColumn DPs

        [Description("Verify Dependency Property: (Style) DataGridBoundColumn.ElementStyleProperty.")]
        TestResult DataGridBoundColumn_ElementStyle()
        {
            Status("DataGridBoundColumn_ElementStyle");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = false; // special case, the callback is on DataGridColumn
            // default: null;
            DataGridVerificationHelper.TestDP<DataGridBoundColumn, Style>("ElementStyle", local, "OnNotifyCellPropertyChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateStyleValue(dataGridBoundColumn, DataGridBoundColumn.ElementStyleProperty);

            LogComment("DataGridBoundColumn_ElementStyle was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (Style) DataGridBoundColumn.EditingElementStyleProperty.")]
        TestResult DataGridBoundColumn_EditingElementStyle()
        {
            Status("DataGridBoundColumn_EditingElementStyle");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = false; // special case, the callback is on DataGridColumn;
            // default: null;
            DataGridVerificationHelper.TestDP<DataGridBoundColumn, Style>("EditingElementStyle", local, "OnNotifyCellPropertyChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateStyleValue(dataGridBoundColumn, DataGridBoundColumn.EditingElementStyleProperty);

            LogComment("DataGridBoundColumn_EditingElementStyle was successful");
            return TestResult.Pass;
        }

        #endregion
        
        #region DataGridTemplateColumn DPs

        [Description("Verify Dependency Property: (DataTemplate) DataGridTemplateColumn.CellEditingTemplateProperty.")]
        TestResult DataGridTemplateColumn_CellEditingTemplate()  
        {
            Status("DataGridTemplateColumn_CellEditingTemplate");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = false;
            // default: n/a;
            DataGridVerificationHelper.TestDP<DataGridTemplateColumn, DataTemplate>("CellEditingTemplate", local, "OnCellEditingTemplatePropertyChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateDataTemplateValue(dataGridTemplateColumn, DataGridTemplateColumn.CellEditingTemplateProperty);

            LogComment("DataGridTemplateColumn_CellEditingTemplate was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (DataTemplate) DataGridTemplateColumn.CellTemplateProperty.")]
        TestResult DataGridTemplateColumn_CellTemplate()
        {
            Status("DataGridTemplateColumn_CellTemplate");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = false;
            // default: n/a;
            DataGridVerificationHelper.TestDP<DataGridTemplateColumn, DataTemplate>("CellTemplate", local, "OnCellTemplatePropertyChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateDataTemplateValue(dataGridTemplateColumn, DataGridTemplateColumn.CellTemplateProperty);

            LogComment("DataGridTemplateColumn_CellTemplate was successful");
            return TestResult.Pass;
        }
        
        [Description("Verify Dependency Property: (DataTemplateSelector) DataGridTemplateColumn.CellEditingTemplateSelectorProperty.")]
        TestResult DataGridTemplateColumn_CellEditingTemplateSelector()
        {
            Status("DataGridTemplateColumn_CellEditingTemplateSelector");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = false;
            // default: n/a;
            DataGridVerificationHelper.TestDP<DataGridTemplateColumn, DataTemplateSelector>("CellEditingTemplateSelector", local, "OnCellEditingTemplatePropertyChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateDataTemplateSelectorValue(dataGridTemplateColumn, DataGridTemplateColumn.CellEditingTemplateSelectorProperty);

            LogComment("DataGridTemplateColumn_CellEditingTemplateSelector was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (DataTemplateSelector) DataGridTemplateColumn.CellTemplateSelectorProperty.")]
        TestResult DataGridTemplateColumn_CellTemplateSelector()
        {
            Status("DataGridTemplateColumn_CellTemplateSelector");

            local.expectGet = true;
            local.expectSet = true;
            local.hasCallBack = false;
            // default: n/a;
            DataGridVerificationHelper.TestDP<DataGridTemplateColumn, DataTemplateSelector>("CellTemplateSelector", local, "OnCellTemplateSelectorPropertyChanged");

            // Verify that we set what we get
            DataGridVerificationHelper.ValidateDataTemplateSelectorValue(dataGridTemplateColumn, DataGridTemplateColumn.CellTemplateSelectorProperty);

            LogComment("DataGridTemplateColumn_CellTemplateSelector was successful");
            return TestResult.Pass;
        }

        #endregion
        
        #region DataGridRowHeader DPs

        [Description("Verify Dependency Property: (bool) DataGridRowHeader.IsRowSelectedProperty.")]
        TestResult DataGridRowHeader_IsRowSelected()
        {
            Status("DataGridRowHeader_IsRowSelected");

            local.expectGet = true;
            local.expectSet = false;
            local.hasCallBack = false;
            // default: false;
            DataGridVerificationHelper.TestDP<DataGridRowHeader, bool>("IsRowSelected", local, "OnIIsRowSelectedPropertyChanged");

            LogComment("DataGridRowHeader_IsRowSelected was successful");
            return TestResult.Pass;
        }
        
        #endregion
        
        #region DataGridColumnHeader DPs

        [Description("Verify Dependency Property: (bool) DataGridColumnHeader.CanUserSortProperty.")]
        TestResult DataGridColumnHeader_CanUserSort()
        {
            Status("DataGridColumnHeader_CanUserSort");

            local.expectGet = true;
            local.expectSet = false;
            local.hasCallBack = false;
            // default: true;
            DataGridVerificationHelper.TestDP<DataGridColumnHeader, bool>("CanUserSort", local, "OnCanSortPropertyChanged");

            var defaultValue = dataGridColumnHeader.CanUserSort;
            Assert.AssertTrue("default value for DataGridColumnHeader.CanUserSort is incorrect.", defaultValue);

            LogComment("DataGridColumnHeader_CanUserSort was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (int) DataGridColumn.DisplayIndexProperty.")]
        TestResult DataGridColumnHeader_DisplayIndex()
        {
            Status("DataGridColumnHeader_DisplayIndex");

            local.expectGet = true;
            local.expectSet = false;
            local.hasCallBack = false;
            // default: -1;
            DataGridVerificationHelper.TestDP<DataGridColumnHeader, int>("DisplayIndex", local, "OnDisplayIndexPropertyChanged"); 

            LogComment("DataGridColumnHeader_DisplayIndex was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (bool) DataGridColumnHeader.IsFrozenProperty.")]
        TestResult DataGridColumnHeader_IsFrozen()
        {
            Status("DataGridColumnHeader_IsFrozen");

            local.expectGet = true;
            local.expectSet = false;
            local.hasCallBack = false;
            // default: false;
            DataGridVerificationHelper.TestDP<DataGridColumnHeader, bool>("IsFrozen", local, "OnIsFrozenPropertyChanged");

            LogComment("DataGridColumnHeader_IsFrozen was successful");
            return TestResult.Pass;
        }

        [Description("Verify Dependency Property: (Nullable<ListSortDirection>) DataGridColumnHeader.SortDirectionProperty.")]
        TestResult DataGridColumnHeader_SortDirection()
        {
            Status("DataGridColumnHeader_SortDirection");

            local.expectGet = true;
            local.expectSet = false;
            local.hasCallBack = false;
            // default: null;
            DataGridVerificationHelper.TestDP<DataGridColumnHeader, Nullable<ListSortDirection>>("SortDirection", local, "OnSortDirectionPropertyChanged");

            LogComment("DataGridColumnHeader_SortDirection was successful");
            return TestResult.Pass;
        }

        #endregion

        #endregion

    }
}
