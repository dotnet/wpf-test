using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using Avalon.Test.ComponentModel;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Verification;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Controls.DataSources;



namespace Microsoft.Test.Controls
{
    /// <summary>
    /// 
    /// Basic Styling tests
    ///      a. Default styles for all built-in columns + DataGridBoundColumn + DataGridTemplateColumn 
    ///      b. Editing styles for all built-in columns + DataGridBoundColumn + DataGridTemplateColumn
    ///      c. Swap styles using StyleSelector for built-in columns
    ///      d. Swap templates using DataTemplateSelector for DataGridBoundColumn + DataGridTemplateColumn
    ///      e. StringFormat for one column
    ///      f. Style inheritance tests - from table/row and from column/cell
    ///      
    /// M1 style related APIs -
    ///     DataGrid - Get/Set  RowStyle
    ///     DataGrid - Get/Set  RowStyleSelector
    ///     DataGrid    Get/Set  RowHeaderStyle
    ///     DataGrid    Get/Set  ColumnHeaderStyle
    ///     
    ///     DataGridRow ItemContainerStyle
    ///     DataGridRow Get/Set  HeaderStyle
    /// 
    ///     DataGridColumn - Get/Set  HeaderStyle
    ///     DataGridColumn DP - CellStyle 
    ///     
    /// </summary>
    [Test(0, "DataGrid", "DataGridStylingTestsBVT", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DataGridStylingTests : XamlTest
    {
        #region Private Fields

        Page page;
        DataGrid dataGrid;
        NewPeople people;

        #endregion

        #region Constructor

        public DataGridStylingTests()
            : base(@"DataGridStylingTestsBVT.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(TestTableStyle);
            RunSteps += new TestStep(TestRowStyle);
            RunSteps += new TestStep(TestColumnStyle);
            RunSteps += new TestStep(TestCellStyle);
            RunSteps += new TestStep(TestStyleInheritance);          
        }

        #endregion
        
        #region Test Steps

        TestResult Setup()
        {
            Status("Setup");
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            dataGrid = (DataGrid)RootElement.FindName("DataGrid_Standard");
            Assert.AssertTrue("Can not find the DataGrid in the xaml file!", dataGrid != null);
            
            page = (Page)this.Window.Content;     
            people = (NewPeople)(page.FindResource("people"));  
            Assert.AssertTrue("Can not find the data source in the xaml file!", people != null);            

            LogComment("Setup was successful");
            return TestResult.Pass;
        }

        TestResult CleanUp()
        {
            dataGrid = null;
            page = null;
            return TestResult.Pass;
        }

        /// <summary>
        /// Styles for DataGrid to test
        ///     DataGrid - Style
        ///     DataGrid - ItemContainerStyle
        ///     DataGrid - RowStyle
        ///     DataGrid - RowStyleSelector
        ///     DataGrid - RowHeaderStyle
        ///     DataGrid - ColumnHeaderStyle
        ///     
        /// Verify  
        ///     a. Styles are set at the DataGrid
        ///     b. Styles are propogated down
        /// Clean up
        /// </summary>
        /// <returns></returns>
        TestResult TestTableStyle()
        {
            Status("TestTableStyle");
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            // no dispute
            Assert.AssertTrue("The DataGrid background is in wrong color", dataGrid.Background == Brushes.Teal);
            // RowHeadersStyle wins
            //DataGridRowHeader header = (DataGridRowHeader)DataGridHelper.GetRowHeader(dataGrid, 1);
            Assert.AssertTrue("The row background color is not correct", ((DataGridRowHeader)DataGridHelper.GetRowHeader(dataGrid, 1)).Background == Brushes.DarkRed);
            // ColumnHeadersStyle wins for this column's header
            Assert.AssertTrue("The column background color is not correct", ((DataGridColumnHeader)DataGridHelper.GetColumnHeader(dataGrid, 2)).Background == Brushes.LightGreen);
            // cellstyle wins
            Assert.AssertTrue("The cell background color is not correct", ((DataGridCell)DataGridHelper.GetCell(dataGrid, 2, 2)).Background == Brushes.Pink);

            // Precedence tests

            // individual column's header style wins
            Assert.AssertTrue("The specific column background color is not correct", ((DataGridColumnHeader)DataGridHelper.GetColumnHeader(dataGrid, 1)).Background == Brushes.Yellow);
            // individual column's cell's style style wins
            Assert.AssertTrue("The cell background color is not correct", ((DataGridCell)DataGridHelper.GetCell(dataGrid, 2, 6)).Background == Brushes.Red);

            // clear the current one
            dataGrid.ClearValue(DataGrid.CellStyleProperty);
            // set a new cell style for the datagrid
            dataGrid.SetResourceReference(DataGrid.CellStyleProperty, "newcell"); // bg = Green
            WaitForPriority(DispatcherPriority.Background);
            Style testStyle = dataGrid.CellStyle;
            Assert.AssertTrue("Unable to find the CellStyle test style.", testStyle != null);

            LogComment("TestTableStyle was successful");
            return TestResult.Pass;
        }
                
        /// <summary>
        /// Style to test:
        ///     DataGridRow     ItemContainerStyle
        ///     DataGridRow     Get/Set HeaderStyle
        ///     
        ///  Row style – correct sequence from lowest to highest
        ///     DataGridRow Implicit Style   
        ///     DataGrid.ItemContainerStyle
        ///     DataGrid.RowStyle
        ///     DataGridRow.Style
        /// </summary>
        /// <returns></returns>
        TestResult TestRowStyle()
        {
            Status("TestRowStyle");
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            // Row - get a row and set the row's style to "dataGridRowStyle" 
            DataGridRow row0 = DataGridHelper.GetRow(dataGrid, 0);
            row0.Background = Brushes.RosyBrown;
            WaitForPriority(DispatcherPriority.Background);
            // we have DataGrid.RowStyle.Background = Blue, so local style wins
            Assert.AssertTrue("The local row style is not set", row0.Background == Brushes.RosyBrown);

            // reset
            row0.ClearValue(DataGridRow.BackgroundProperty);
            Assert.AssertEqual("The table style did not propagate down", Brushes.Black, row0.Background);

            LogComment("TestRowStyle was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Tests for column style - 
        ///         DataGridColumn      - CellStyle
        ///         DataGridColumn      - HeaderStyle
        /// </summary>
        /// <returns></returns>
        TestResult TestColumnStyle()
        {
            Status("TestColumnStyle");

            // ColumnHeadersStyle wins for this column's header
            Assert.AssertTrue("The column header background color is not correct", ((DataGridColumnHeader)DataGridHelper.GetColumnHeader(dataGrid, 2)).Background == Brushes.LightGreen);
            // local cellstyle wins
            //Assert.AssertEqual("The cell 2 background color is not correct", Brushes.Blue, ((DataGridCell)DataGridHelper.GetCell(dataGrid, 2, 2)).Background);
            Assert.AssertEqual("The cell 6 background color is not correct", Brushes.Red, ((DataGridCell)DataGridHelper.GetCell(dataGrid, 2, 6)).Background);
            
            // remove the local cell's background 
            DataGridTemplateColumn tc = (DataGridTemplateColumn)dataGrid.Columns[6];
            tc.ClearValue(DataGridColumn.CellStyleProperty);
            WaitForPriority(DispatcherPriority.Background);
            // the dataGrid's cell style should propagate. 
            Assert.AssertTrue("The cell background color is not correct", ((DataGridCell)DataGridHelper.GetCell(dataGrid, 2, 6)).Background == Brushes.Green);

            LogComment("TestColumnStyle was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Test cell styles - Cell styles from lowest to highest: 
        ///     DataGridCell Imlicit style
        ///     DataGrid.CellStyle
        ///     DataGridColumn.CellStyle
        ///     DataGridCell.Style
        /// </summary>
        /// <returns></returns>
        TestResult TestCellStyle()
        {
            Status("TestCellStyle");
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            // 
            Assert.AssertEqual("The cell background color is not correct", Brushes.Green, ((DataGridCell)DataGridHelper.GetCell(dataGrid, 0, 6)).Background);
            
            // remove the DataGrid's cellstyle 
            dataGrid.ClearValue(DataGrid.CellStyleProperty);
            dataGrid.SetResourceReference(DataGrid.CellStyleProperty, "TestCellStyle");
            WaitForPriority(DispatcherPriority.Background);

            // 
            Assert.AssertEqual("The new cell background color is not correct", Brushes.Transparent, ((DataGridCell)DataGridHelper.GetCell(dataGrid, 0, 6)).Background);

            LogComment("TestCellStyle was successful");
            return TestResult.Pass;
        }
        
        /// <summary>
        /// Tests for precedences remains - others have been tested so far
        /// </summary>
        /// <returns></returns>
        TestResult TestStyleInheritance()
        {
            Status("TestStyleInheritance");

            DataGridRow row0 = DataGridHelper.GetRow(dataGrid, 0);
            // use the transferred properties from the DG 
            Assert.AssertEqual("The local row style is not set", Brushes.Black, row0.Background);

            // remove the existing styles
            dataGrid.ClearValue(DataGrid.RowStyleProperty);
            WaitForPriority(DispatcherPriority.Background);
            dataGrid.ClearValue(DataGrid.RowBackgroundProperty);
            WaitForPriority(DispatcherPriority.Background);
            dataGrid.ClearValue(DataGrid.AlternatingRowBackgroundProperty);
            WaitForPriority(DispatcherPriority.Background);
            
            // set a new DataGrid.RowStyle to the alternatingRowStyle
            Style style = (Style)(page.FindResource("alternatingRowStyle"));
            Assert.AssertTrue("Can not find the alternatingRowStyle in the xaml file!", style != null);            

            dataGrid.SetResourceReference(DataGrid.RowStyleProperty, "alternatingRowStyle");
            WaitForPriority(DispatcherPriority.Background);
            dataGrid.SetValue(DataGrid.AlternationCountProperty, 3);
            WaitForPriority(DispatcherPriority.Background);
            dataGrid.SetValue(DataGrid.RowStyleProperty, style);
            WaitForPriority(DispatcherPriority.Background);
            Assert.AssertTrue("The datagrid rowstyle is not set", dataGrid.GetValue(DataGrid.RowStyleProperty) != null);

            row0 = DataGridHelper.GetRow(dataGrid, 0);
            DataGridRow row1 = DataGridHelper.GetRow(dataGrid, 1);
            DataGridRow row2 = DataGridHelper.GetRow(dataGrid, 2);
            // new style shouild transfer
            Assert.AssertEqual("0 - The local row style is not set to alternatingRowStyle", Brushes.LightBlue, row0.Background); 
            Assert.AssertEqual("1 - The local row style is not set to alternatingRowStyle", Brushes.LightGoldenrodYellow, row1.Background); 
            Assert.AssertEqual("2 - The local row style is not set to alternatingRowStyle", Brushes.LightGreen, row2.Background); 

            LogComment("TestStyleInheritance was successful");
            return TestResult.Pass;
        }

        #endregion
        
    }
}
