using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using Avalon.Test.ComponentModel;
using Avalon.Test.ComponentModel.Actions;
using Avalon.Test.ComponentModel.DataSources;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Verification;


namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Test DataGrid OM for grid/column/row/cell, including tests for 
    ///     Properties - defaults; positive tests; negative tests; DPs tests - differenct cs file
    ///     DataBinding basic tests - ItemSource and 
    /// 
    /// OM Tests - very basic column templates and without styles
    /// 
    ///     a. Verify all default values for public properties; tests for Public DPs will be taken care of 
    ///             in the DependencyPropertyTests.cs  
    ///         Open issues: how do we efficiently keep track of the Property list?
    ///     b. well known Enum tests   
    ///     c. verify record count is correct upon datasource's changes: add/remove item, etc. 
    ///     d. column binding path change - verify the contents
    ///     e. Ad-hoc testing - ContextMenu / Tooltip tests at table/column/row/cell
    ///     f. Unbound rows/cells verification
    ///   
    [Test(0, "DataGrid", "DataGridOMTestsBVT", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DataGridOMTests : XamlTest
    {
        #region Private Fields

        DataGrid dataGrid;
        ComboBox dataGridHeadersVisibilityCombo;
        ComboBox gridlineVisibilityCombo;
        ComboBox verticalScrollBarVisibilityCombo;
        NewPeople people;
        Page page;

        #endregion

        #region Constructor

        public DataGridOMTests()
            : base(@"DataGridOMTestsBVT.xaml")
        {
            InitializeSteps += new TestStep(TestDefaultValues);     // all default values
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(Setup);                        // initial setup for data binding 
            RunSteps += new TestStep(VerifyEnums);                  // verify Enums - DataGridHeadersVisibility missing
            RunSteps += new TestStep(VerifyCommonProperties);       // verify initial properties for the given dg         
            RunSteps += new TestStep(VerifyDataBinding);            // binding record count; binding path updates for a given column;
            RunSteps += new TestStep(TestContextMenu);              // inheritance
            RunSteps += new TestStep(TestTooltips);                 // inheritance
            RunSteps += new TestStep(TestDataGridRowGetRowContainingElement);
            RunSteps += new TestStep(TestDataGridRowGetIndex);
            RunSteps += new TestStep(TestDataGridLength);
        }

        #endregion
        
        #region Test Steps

        /// <summary>
        /// Test all default values for OM objects' properties in the new APIs, inherited properties
        ///     will not be tested unless there are updates.  
        /// 
        /// 


        TestResult TestDefaultValues()
        {
            Status("TestDefaultValues");
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            DataGrid dataGridLoc = new DataGrid();
            Assert.AssertFalse("DataGrid being tested can not be null!", dataGridLoc == null);
            DataGridRow dataGridRow = new DataGridRow();
            Assert.AssertFalse("DataGridRow being tested can not be null!", dataGridRow == null);
            DataGridTextColumn dataGridTextColumn = new DataGridTextColumn();
            Assert.AssertFalse("DataGridTextColumn being tested can not be null!", dataGridTextColumn == null);

            // ****************
            // databinding
            // ****************
            Assert.AssertEqual("ItemsSource wrong default", null, dataGridLoc.ItemsSource);
            Assert.AssertEqual("Binding wrong default", null, dataGridTextColumn.Binding);

            // ************************************************
            // widths and heights of rows and columns
            // ************************************************
            Assert.AssertTrue("ColumnWidth wrong default", ((DataGridLength)dataGridLoc.ColumnWidth).IsSizeToHeader);
            Assert.AssertEqual("MinColumnWidth wrong default", 20.0, dataGridLoc.MinColumnWidth);                 
            Assert.AssertEqual("MaxColumnWidth wrong default", double.PositiveInfinity, dataGridLoc.MaxColumnWidth);                 
            Assert.AssertEqual("ColumnHeaderHeight wrong default", double.NaN, dataGridLoc.ColumnHeaderHeight);     
            Assert.AssertEqual("RowHeight wrong default", double.NaN, dataGridLoc.RowHeight);              
            Assert.AssertTrue("MinRowHeight wrong default", DataGridHelper.AreClose(0.0, dataGridLoc.MinRowHeight, 0.5));
            Assert.AssertEqual("RowHeaderWidth wrong default", double.NaN, dataGridLoc.RowHeaderWidth);
            Assert.AssertTrue("DataGridColumn.Width wrong default", dataGridTextColumn.Width.IsAuto);  
            Assert.AssertEqual("DataGridColumn.MinWidth wrong default", 20.0, dataGridTextColumn.MinWidth);                  
            Assert.AssertEqual("DataGridColumn.MaxWidth wrong default", double.PositiveInfinity, dataGridTextColumn.MaxWidth);  

            // ************************************************
            // header visibility, gridlines, row background
            // ************************************************
            Assert.AssertEqual("DataGrid.HeadersVisibility wrong default", DataGridHeadersVisibility.All, dataGridLoc.HeadersVisibility);
            Assert.AssertEqual("DataGrid.HorizontalScrollBarVisibility wrong default", ScrollBarVisibility.Auto, dataGridLoc.HorizontalScrollBarVisibility); 
            Assert.AssertEqual("DataGrid.VerticalScrollBarVisibility wrong default", ScrollBarVisibility.Auto, dataGridLoc.VerticalScrollBarVisibility);    
            Assert.AssertEqual("DataGrid.GridlinesVisibility wrong default", DataGridGridLinesVisibility.All, dataGridLoc.GridLinesVisibility);
            Assert.AssertEqual("RowBackground  wrong default", null, dataGridLoc.RowBackground);
            Assert.AssertEqual("AlternatingRowBackground  wrong default", null, dataGridLoc.AlternatingRowBackground);
            Assert.AssertEqual("DataGridRow.AlternationIndex  wrong default", 0, dataGridRow.AlternationIndex);

            // ****************
            // templating
            // ****************
            Assert.AssertEqual("ItemTemplate wrong default", null, dataGridLoc.ItemTemplate);
            Assert.AssertEqual("ItemTemplateSelector wrong default", null, dataGridLoc.ItemTemplateSelector);
            Assert.AssertEqual("DataGridRow.HeaderTemplate wrong default", null, dataGridRow.HeaderTemplate);
            Assert.AssertEqual("DataGridRow.HeaderTemplateSelector wrong default", null, dataGridRow.HeaderTemplateSelector); 
            Assert.AssertEqual("DataGridColumn.HeaderTemplate wrong default", null, dataGridTextColumn.HeaderTemplate);
            Assert.AssertEqual("DataGridColumn.HeaderTemplateSelector wrong default", null, dataGridTextColumn.HeaderTemplateSelector); 

            // ****************
            // styling
            // ****************
            Assert.AssertEqual("DataGrid.RowHeaderStyle wrong default", null, dataGridLoc.RowHeaderStyle);
            Assert.AssertEqual("DataGrid.ColumnHeaderStyle wrong default", null, dataGridLoc.ColumnHeaderStyle); 
            Assert.AssertEqual("DataGrid.CellStyle wrong default", null, dataGridLoc.CellStyle);
            Assert.AssertEqual("DataGridRow.HeaderStyle wrong default", null, dataGridRow.HeaderStyle);
            Assert.AssertEqual("DataGridColumn.HeaderStyle wrong default", null, dataGridTextColumn.HeaderStyle);
            Assert.AssertEqual("DataGridColumn.CellStyle wrong default", null, dataGridTextColumn.CellStyle);

            LogComment("TestDefaultValues was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Initial Setup for the Datagrid and bindings
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        TestResult Setup()
        {
            Status("Setup");
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            // locate the DataGrid being tested
            dataGrid = (DataGrid)RootElement.FindName("DataGrid_Standard");
            Assert.AssertTrue("Can not find the DataGrid in the xaml file!", dataGrid != null);

            // locate other controls needed
            dataGridHeadersVisibilityCombo = (ComboBox)RootElement.FindName("DataGridHeadersVisibilityComboBox");
            Assert.AssertTrue("Can not find the DataGridHeadersVisibilityCombo in the xaml file!", dataGridHeadersVisibilityCombo != null);
            gridlineVisibilityCombo = (ComboBox)RootElement.FindName("GridLineVisibilityComboBox");
            Assert.AssertTrue("Can not find the GridlineVisibilityCombo in the xaml file!", gridlineVisibilityCombo != null);
            verticalScrollBarVisibilityCombo = (ComboBox)RootElement.FindName("VerticalScrollBarVisibilityComboBox");
            Assert.AssertTrue("Can not find the VerticalScrollBarVisibilityComboBox in the xaml file!", verticalScrollBarVisibilityCombo != null);

            page = (Page)this.Window.Content;     
            people = (NewPeople)(page.FindResource("people"));
            Assert.AssertTrue("Can not find the data source in the xaml file!", people != null);

            LogComment("Setup was successful");
            return TestResult.Pass;
        }

        TestResult CleanUp()
        {
            dataGrid = null;
            dataGridHeadersVisibilityCombo = null;
            gridlineVisibilityCombo = null;
            verticalScrollBarVisibilityCombo = null;
            page = null;
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify related Enums on DataGrid, including:
        ///     DataGridHeadersVisibility 
        ///     HorizontalScrollBarVisibility - ScrollBarVisibility Enum (Disabled, Auto, Hidden, Visible)
        ///     VerticalScrollBarVisibility - ScrollBarVisibility Enum (Disabled, Auto, Hidden, Visible)
        ///     GridlinesVisibility - DataGridGridlines Enum (All, Horizontal, Vertical, None)
        /// </summary>
        /// <returns></returns>
        TestResult VerifyEnums()
        {
            Status("VerifyEnums");
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            for (int h = 0; h < dataGridHeadersVisibilityCombo.Items.Count; h++)
            {
                dataGridHeadersVisibilityCombo.SelectedIndex = h;
                Assert.AssertEqual("DataGrid - DataGridLineVisibility value is incorrect", (DataGridHeadersVisibility)Enum.Parse(typeof(DataGridHeadersVisibility), dataGridHeadersVisibilityCombo.Items[h].ToString()), dataGrid.HeadersVisibility);
            }
            dataGridHeadersVisibilityCombo.SelectedIndex = 0;

            for (int i = 0; i < gridlineVisibilityCombo.Items.Count; i++)
            {
                gridlineVisibilityCombo.SelectedIndex = i;
                Assert.AssertEqual("DataGrid - DataGridGridLinesVisibility value is incorrect", (DataGridGridLinesVisibility)Enum.Parse(typeof(DataGridGridLinesVisibility), gridlineVisibilityCombo.Items[i].ToString()), dataGrid.GridLinesVisibility);
            }
            gridlineVisibilityCombo.SelectedIndex = 0;

            for (int j = 0; j < verticalScrollBarVisibilityCombo.Items.Count; j++)
            {
                verticalScrollBarVisibilityCombo.SelectedIndex = j;
                Assert.AssertEqual("DataGrid - VerticalScrollBarVisibility value is incorrect", (ScrollBarVisibility)Enum.Parse(typeof(ScrollBarVisibility), verticalScrollBarVisibilityCombo.Items[j].ToString()), dataGrid.VerticalScrollBarVisibility);
            }
            verticalScrollBarVisibilityCombo.SelectedIndex = 1;
           
            LogComment("VerifyEnums was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// For the given datagrid from the xaml file
        ///     Verify the datagrid's widths and heights, gridline visibility, header visibility, etc.
        ///     Verify all columns' widths as specified
        ///     Verify the row background, etc. 
        /// </summary>
        /// <returns></returns>
        TestResult VerifyCommonProperties()
        {
            Status("VerifyCommonProperties");
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            // *********************************************************************************
            // DataGrid's common properties - header visibility, gridlines, row background, etc.
            // *********************************************************************************
            Assert.AssertEqual("The DataGrid.HorizontalScrollBarVisibility wrong value", ScrollBarVisibility.Auto, dataGrid.HorizontalScrollBarVisibility);
            Assert.AssertEqual("The DataGrid.VerticalScrollBarVisibility wrong value", ScrollBarVisibility.Auto, dataGrid.VerticalScrollBarVisibility);
            Assert.AssertEqual("The DataGrid.GridlinesVisibility wrong value", DataGridGridLinesVisibility.All, dataGrid.GridLinesVisibility);
            Assert.AssertEqual("The DataGrid.HorizontalGridlinesBrush wrong value", Brushes.Beige, dataGrid.HorizontalGridLinesBrush);
            Assert.AssertEqual("The DataGrid.VerticalGridlinesBrush wrong value", Brushes.DarkBlue, dataGrid.VerticalGridLinesBrush);
            Assert.AssertEqual("The RowBackground wrong value", Brushes.AliceBlue, dataGrid.RowBackground);
            Assert.AssertEqual("The AlternatingRowBackground  wrong value", Brushes.Bisque, dataGrid.AlternatingRowBackground);
            Assert.AssertEqual("The HorizontalAlignment  wrong value", HorizontalAlignment.Left, dataGrid.HorizontalAlignment);
            Assert.AssertEqual("The ColumnHeaderHeight wrong value", double.NaN, dataGrid.ColumnHeaderHeight);  // 
            Assert.AssertEqual("The RowHeaderWidth wrong value", double.NaN, dataGrid.RowHeaderWidth); 
            Assert.AssertEqual("The DataGrid.HeadersVisibility wrong default", DataGridHeadersVisibility.All, dataGrid.HeadersVisibility);  

            DataGridRow row0 = DataGridHelper.GetRow(dataGrid, 0); 
            DataGridRow row1 = DataGridHelper.GetRow(dataGrid, 1); 
            Assert.AssertTrue("Unable to get the first row.", row0 != null);
            Assert.AssertTrue("Unable to get the second row.", row1 != null);
            Assert.AssertTrue("Row0 Background is not AliceBlue.", row0.Background == Brushes.AliceBlue);
            Assert.AssertTrue("Row1 AlternatingBackground is not Bisque.", row1.Background == Brushes.Bisque);
            
            // ***************************
            // basic column width tests
            // ***************************
            DataGridColumn column = dataGrid.Columns[2]; 
            Assert.AssertTrue("Unable to get the checkbox column.", column != null);
            PropertyMetadata metadata = DataGridColumn.MinWidthProperty.GetMetadata(column);
            Assert.AssertTrue("Width of the checkbox column isn't the default min width", column.ActualWidth == column.MinWidth && column.ActualWidth == (double)metadata.DefaultValue);

            // Set a width too small and ensure it's clamped.
            column = dataGrid.Columns[0];
            column.MinWidth = 30d;
            column.Width = new DataGridLength(10d);
            Assert.AssertTrue("Column didn't honor min width", column.ActualWidth == 30d);

            // Set a width too large
            column.MaxWidth = 50d;
            column.Width = new DataGridLength(80d);
            Assert.AssertTrue("Column didn't honor max width", column.ActualWidth == 50d);

            column.ClearValue(DataGridColumn.MinWidthProperty);
            column.ClearValue(DataGridColumn.MaxWidthProperty);
            
            LogComment("VerifyInitialProperties was successful");
            return TestResult.Pass;
        }
        
        /// <summary>
        /// Tests for DataGrid ItemSource and DataFieldBindingPath for columns
        ///     a. record count is correct
        ///     b. DisplayMember binding path for a given column is correct
        /// </summary>
        /// <returns>true if all is well, otherwiese false</returns>
        TestResult VerifyDataBinding()
        {
            Status("VerifyDataBinding");
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            
            // verify the record count and the binding count 
            int bindingSize = dataGrid.Items.Count; 
            Assert.AssertEqual("DataGrid binding records error!", people.Count, bindingSize); 

            // verify the Binding of column 1         
            this.ValidateTextColumn(1, "LastName");
            BindingBase initBinding = ((DataGridBoundColumn)dataGrid.Columns[1]).Binding;           
            BindingBase newBinding = new Binding("FirstName");
            ((DataGridBoundColumn)dataGrid.Columns[1]).Binding = newBinding;
            WaitForPriority(DispatcherPriority.Background);
            this.ValidateTextColumn(1, "FirstName");
            ((DataGridBoundColumn)dataGrid.Columns[1]).Binding = initBinding;
            WaitForPriority(DispatcherPriority.Background);
            this.ValidateTextColumn(1, "LastName");

            // get initial size of the first auto column
            DataGridCell cellAuto = DataGridHelper.GetCell(dataGrid, 0, 1);
            Assert.AssertTrue("Unable to get the auto cell.", cellAuto != null);
            double autoInitWidth = cellAuto.RenderSize.Width;

            // add an item to/from the collection - verify record count, cell content, and column width update, etc.
            people.Insert(0, new Person("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXX", "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"));
            WaitForPriority(DispatcherPriority.Background);

            Assert.AssertEqual("DataGrid binding records error after insert!", people.Count, bindingSize + 1);

            DataGridCell cell = DataGridHelper.GetCell(dataGrid, 0, 0);
            Assert.AssertTrue("Unable to get the first cell.", cell != null);
            Assert.AssertTrue("Cell content should be a TextBlock.", cell.Content is TextBlock);
            TextBlock tb = (TextBlock)cell.Content;
            Assert.AssertTrue("The first cell content is not right!", "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXX" == tb.Text);
            cell.IsEditing = true;
            Assert.AssertTrue("Cell content did not change to a TextBox.", cell.Content is TextBox);
            cell.IsEditing = false;
            Assert.AssertTrue("Cell content should change back to a TextBlock.", cell.Content is TextBlock);

            Assert.AssertTrue("Auto cell did not increase in size", autoInitWidth < cellAuto.RenderSize.Width);

            // remove the item 
            people.RemoveAt(0);
            WaitForPriority(DispatcherPriority.Background);

            Assert.AssertEqual("DataGrid binding records did not change back!", people.Count, bindingSize);
            
            // the 2 auto actually affact the result
            Assert.AssertEqual("The first cell did not change back in size", 80.0, cell.RenderSize.Width); 

            LogComment("VerifyDataBinding was successful");
            return TestResult.Pass;
        }
        
        /// <summary>
        /// Basic tests for Context menu propagation from the table down to row, column and cell
        /// </summary>
        /// <returns>true if all is well, otherwiese false</returns>
        TestResult TestContextMenu()
        {
            Status("TestContextMenu");
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            ContextMenu cmGrid = dataGrid.ContextMenu;
            Assert.AssertTrue("The ContextMenu on the DataGrid should not be null", cmGrid != null);

            // down to row 
            DataGridRow row = DataGridHelper.GetRow(dataGrid,1);
            Assert.AssertTrue("The Row should not be null", row != null);
            row.SetValue(DataGridRow.ContextMenuProperty, cmGrid);
            WaitForPriority(DispatcherPriority.Background);

            ContextMenu cmRow = row.ContextMenu;
            Assert.AssertTrue("The ContextMenu on the DataGridRow should not be null", cmRow != null);
            Assert.AssertTrue("The ContextMenu did not propagate down to the row", cmRow == cmGrid);

            // down to cells in all rows/columns[2]
            DataGridColumn column = (DataGridTextColumn)DataGridHelper.GetColumn(dataGrid, 1);
            for (int i = 0; i < people.Count; i++)
            {
                DataGridCell cell = DataGridHelper.GetCell(dataGrid, i, 2);
                Assert.AssertTrue(string.Format("The cell at item {0} should not be null", i.ToString()), cell != null);
                
                cell.SetValue(DataGridCell.ContextMenuProperty, cmGrid);
                WaitForPriority(DispatcherPriority.Background);

                ContextMenu cmCell = cell.ContextMenu;
                Assert.AssertTrue("The ContextMenu on the cell should not be null", cmCell != null);
                Assert.AssertTrue("The ContextMenu did not propagate down to the cell", cmCell == cmGrid);
            }

            LogComment("TestContextMenu was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Basic tests for Tooltip propagation from the table down to row, column and cell
        /// </summary>
        /// <returns>true if all is well, otherwiese false</returns>
        TestResult TestTooltips()
        {
            Status("TestTooltips");
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            ToolTip ttDataGrid = (ToolTip)dataGrid.ToolTip;
            Assert.AssertTrue("The ToolTip on the DataGrid should not be null", ttDataGrid != null);

            // down to row 
            DataGridRow row = DataGridHelper.GetRow(dataGrid, 1);
            Assert.AssertTrue("The row should not be null", row != null);
            row.SetValue(DataGridRow.ToolTipProperty, ttDataGrid);
            WaitForPriority(DispatcherPriority.Background);

            ToolTip tt = (ToolTip)row.ToolTip;
            Assert.AssertTrue("The ToolTip did not propagate down to the row", tt == ttDataGrid);
            tt = null;

            // down to a specific column - all cells in the column should have the Tooltip propagated down to
            for (int i = 0; i < people.Count; i++)
            {
                DataGridCell cell = DataGridHelper.GetCell(dataGrid, i, 2);
                Assert.AssertTrue(string.Format("The cell at item {0} should not be null", i.ToString()), cell != null);

                cell.SetValue(DataGridCell.ToolTipProperty, ttDataGrid);
                WaitForPriority(DispatcherPriority.Background);

                tt = (ToolTip)cell.ToolTip;
                Assert.AssertTrue("The ToolTip did not propagate down to the cells", tt == ttDataGrid);
                tt = null;
            }

            LogComment("TestTooltips was successful");
            return TestResult.Pass;
        }

        TestResult TestDataGridRowGetRowContainingElement()
        {
            Status("TestDataGridRowGetRowContainingElement");

            // first get the row and an element within the row
            int rowIndex = 1;            
            var dataGridRow = DataGridHelper.GetRow(dataGrid, rowIndex);
            Assert.AssertTrue(string.Format("dataGridRow should not be null"), dataGridRow != null);

            var dataGridRowHeader = VisualTreeUtils.GetVisualChild<DataGridRowHeader>(dataGridRow);
            Assert.AssertTrue(string.Format("dataGridRowHeader should not be null"), dataGridRowHeader != null);

            var actual = DataGridRow.GetRowContainingElement(dataGridRowHeader);

            if (dataGridRow != actual)
            {
                throw new TestValidationException("GetRowContainingElement returned the incorrect DataGridRow");
            }

            LogComment("TestDataGridRowGetRowContainingElement was successful");
            return TestResult.Pass;
        }

        TestResult TestDataGridRowGetIndex()
        {
            Status("TestDataGridRowGetIndex");

            for (int i = 0; i < dataGrid.Items.Count; i++)
            {
                Status(string.Format("Getting row at: {0}", i));

                int expected = i;
                var dataGridRow = DataGridHelper.GetRow(dataGrid, expected);
                Assert.AssertTrue(string.Format("dataGridRow should not be null"), dataGridRow != null);

                int actual = dataGridRow.GetIndex();
                if (expected != actual)
                {
                    throw new TestValidationException(string.Format("GetIndex returned the incorrect value. Expected: {0}, Actual: {1}", expected, actual));
                }
            }

            LogComment("TestDataGridRowGetIndex was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Test DataGridLength Inequality
        /// </summary>
        /// <returns></returns>
        TestResult TestDataGridLength()
        {
            Status("TestDataGridLength");

            // test basic inequality
            var length1 = new DataGridLength(10.0);
            var length2 = new DataGridLength(10.0);

            LogComment("Test value match");
            if (length1 != length2)
            {
                throw new TestValidationException(string.Format("Inequality evaluation is correct.  Should be equal.  Left: {0}, Right: {1}", length1, length2));
            }

            length1 = new DataGridLength(10.0, DataGridLengthUnitType.SizeToCells);
            length2 = new DataGridLength(10.0, DataGridLengthUnitType.SizeToCells);

            LogComment("Test value match and UnitType match");
            if (length1 != length2)
            {
                throw new TestValidationException(string.Format("Inequality evaluation is correct.  Should be equal.  Left.Value: {0}, Right.Value: {1}", length1.Value, length2.Value));
            }

            length1 = new DataGridLength(10.0, DataGridLengthUnitType.Pixel, 3.0, 2.0);
            length2 = new DataGridLength(10.0, DataGridLengthUnitType.Pixel, 3.0, 2.0);

            LogComment("Test value match, UnitType match, desired match, display match");
            if (length1 != length2)
            {
                throw new TestValidationException(string.Format("Inequality evaluation is correct.  Should be equal.  Left: {0}, Right: {1}", length1, length2));
            }

            LogComment("TestDataGridLength was successful");
            return TestResult.Pass;
        }

        #endregion
        
        #region Private Helper
        
        /// <summary>
        /// This is actually partially M1 feature - we add here anyway
        /// Walks through all rows and ensures that the content in the given column index matches the given property 
        /// for the data item in that row.
        /// This is specific to text columns.
        /// </summary>
        private void ValidateTextColumn(int columnIndex, string propertyName)
        {
            PropertyInfo property = typeof(Person).GetProperty(propertyName);

            if (property != null)
            {
                // Validate that the current binding generated a valid string
                for (int rowIndex = 0; rowIndex < people.Count; rowIndex++)
                {
                    DataGridCell cellContainer = DataGridHelper.GetCell(dataGrid, rowIndex, columnIndex);
                    if (cellContainer == null)
                    {
                        // We encountered virtualization, no more cells
                        break;
                    }
                    Assert.AssertTrue(string.Format("The cell for column {0} has the wrong content; it is of type {1} instead of TextBlock", columnIndex, cellContainer.Content.GetType()), cellContainer.Content is TextBlock);
                    TextBlock textBlock = (TextBlock)cellContainer.Content;
                    string cellValue = textBlock.Text;
                    object propertyValue = property.GetValue(people[rowIndex], null);

                    string dataValue = propertyValue as string;

                    if (dataValue == null)
                    {
                        dataValue = propertyValue.ToString();
                    }

                    Assert.AssertTrue(String.Format("Value of TextBlock.Text for Column {0} (\"{1}\") does not match \"{2}\" (row = {3})", columnIndex, cellValue, dataValue, rowIndex), cellValue == dataValue);
                }
            }
        }

        #endregion
    }
}
