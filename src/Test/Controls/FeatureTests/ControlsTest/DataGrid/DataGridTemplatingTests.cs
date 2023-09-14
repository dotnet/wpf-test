using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using Avalon.Test.ComponentModel;
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
    ///  Basic templating tests for WPF DataGrid
    ///  
    ///      a. verify Default templates for all built-in columns + DataGridBoundColumn + DataGridTemplateColumn 
    ///      b. verify Editing templates for all built-in columns + DataGridBoundColumn + DataGridTemplateColumn
    ///      c. swap templates using DataTemplateSelector for built-in columns
    ///      d. swap templates using DataTemplateSelector for DataGridBoundColumn + DataGridTemplateColumn
    ///      f. change to a different DS - verify #e + record count
    ///      g. add/remove item in DS - verify #e + record count
    ///      h. Templating inheritance - from table/row and from column/cell
    /// </summary>
    [Test(0, "DataGrid", "DataGridTemplatingTestsBVT", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DataGridTemplatingTests: XamlTest
    {
        #region Private Fields

        Page page;
        DataGrid dataGrid;
        NewPeople people;

        #endregion

        #region Constructor

        public DataGridTemplatingTests()
            : base(@"DataGridTemplatingTestsBVT.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(TestTableTemplate);
            RunSteps += new TestStep(TestRowTemplate);
            RunSteps += new TestStep(TestColumnTemplate);
            RunSteps += new TestStep(TestCellTemplate);
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
        /// Test ItemTemplate for DataGrid
        /// </summary>
        /// <returns></returns>
        TestResult TestTableTemplate()
        {
            Status("TestTableTemplate");


            LogComment("TestTableTemplate was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Tests for DataGridRow's HeaderTemplate
        /// </summary>
        /// <returns></returns>
        TestResult TestRowTemplate()
        {
            Status("TestRowTemplate");

            // get the first row
            DataGridRow row = DataGridHelper.GetRow(dataGrid, 0);
            Assert.AssertTrue("Can not find the row", row != null);
            DataTemplate template = new DataTemplate(); 

            // add a HeaderTemplate 
            FrameworkElementFactory fef = new FrameworkElementFactory(typeof(TextBlock));
            fef.SetValue(TextBlock.TextProperty, "test it");
            template.VisualTree = fef;
            row.HeaderTemplate = template;
            
            // get the row header
            DataGridRowHeader header = DataGridHelper.GetRowHeader(dataGrid, 0);
            Assert.AssertTrue("Can not find the row header", header != null);
            
            // make sure the rowheader's template is correct
            Assert.AssertTrue("The new row header template did not work", header.ContentTemplate == template);
            
            // clear it
            row.ClearValue(DataGridRow.HeaderTemplateProperty);
            WaitForPriority(DispatcherPriority.Background);
            
            // verify it's cleared
            Assert.AssertTrue("The new row header template did not clear", header.ContentTemplate == null);

            LogComment("TestRowTemplate was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Tests for DataGridColumn's HeaderTemplate and DataGridColumnHeader's ContentTemplate
        /// </summary>
        /// <returns></returns>
        TestResult TestColumnTemplate()
        {
            Status("TestColumnTemplate");

            DataGridColumn column;
            DataGridColumnHeader columnHeader;
            DataTemplate template;

            // Column 0 has a header template
            columnHeader = DataGridHelper.GetColumnHeader(dataGrid,0);
            Assert.AssertTrue("The first column header template should not be null", columnHeader.ContentTemplate != null);
            template = columnHeader.ContentTemplate;

            // Column 1 has no header template
            columnHeader = DataGridHelper.GetColumnHeader(dataGrid,1);
            Assert.AssertTrue("The second column header template should be null", columnHeader.ContentTemplate == null);

            // set the datatemplate from column 0 to 1
            column = DataGridHelper.GetColumn(dataGrid, 1);
            column.HeaderTemplate = template;
            Assert.AssertTrue("The Column.HeaderTemplate did not get sent down to the ColumnHeader", columnHeader.ContentTemplate == column.HeaderTemplate);
            
            // reset the column 1's template
            column.ClearValue(DataGridColumn.HeaderTemplateProperty);
            Assert.AssertTrue("The Column header template should have been cleared", columnHeader.ContentTemplate == null);

            //specify a new header template on column 0: DataGridColumn's HeaderTemplate takes precedence and propagates down to the DataGridColumnHeader
            column = dataGrid.Columns[0];                               // DataGridColumn at 0
            columnHeader = DataGridHelper.GetColumnHeader(dataGrid, 0); // DataGridColumnHeader for column 0
            column.HeaderTemplate = new DataTemplate(); 
            Assert.AssertTrue("Column.HeaderTemplate did not get sent down to the ColumnHeader", columnHeader.ContentTemplate == column.HeaderTemplate);

            LogComment("TestColumnTemplate was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Tests for DataGridTemplateColumn's CellTemplate and CellEditingTemplate
        /// </summary>
        /// <returns></returns>
        TestResult TestCellTemplate()
        {
            Status("TestCellTemplate");

            DataGridTemplateColumn templateColumn;
            DataTemplate template;

            // get the templated column
            templateColumn = (DataGridTemplateColumn)dataGrid.Columns[5];
            Assert.AssertTrue("The DataGridTemplateColumn should not be null", templateColumn != null);
            template = templateColumn.CellTemplate;
            Assert.AssertTrue("The Column.CellTemplate should not be null", template != null);

            // find the first cell
            DataGridCell cell = DataGridHelper.GetCell(dataGrid, 0, 5);
            Assert.AssertTrue("The cell.IsEditing should be false", cell.IsEditing == false);

            // find the cell's content 
            DataTemplate dt = ((ContentPresenter)cell.Content).ContentTemplate;
            TextBlock textBlock = (TextBlock)dt.FindName("tb", (ContentPresenter)cell.Content);
            Assert.AssertTrue("The Column.CellTemplate content is not correct", textBlock.Text == "Ha New!");
            
            // set the cell to edit
            cell.IsEditing = true;
            WaitForPriority(DispatcherPriority.Background);
            // validate the CellEdtingTemplate
            template = templateColumn.CellEditingTemplate;
            Assert.AssertTrue("The Column.CellEditingTemplate should not be null", template != null);
            // get the editing content
            dt = ((ContentPresenter)cell.Content).ContentTemplate;
            TextBox textBox = (TextBox)dt.FindName("tbEdit", (ContentPresenter)cell.Content); 
            Assert.AssertTrue("The Column.CellTemplate content is not correct", textBox.Text == "Ha Editing New!");
            
            // reset
            cell.IsEditing = false;
            WaitForPriority(DispatcherPriority.Background);
            dt = ((ContentPresenter)cell.Content).ContentTemplate;
            textBlock = (TextBlock)dt.FindName("tb", (ContentPresenter)cell.Content);
            Assert.AssertTrue("The Column.CellTemplate content is not correct", textBlock.Text == "Ha New!");

            LogComment("TestCellTemplate was successful");
            return TestResult.Pass;
        }
                
        #endregion
        
    }
}
