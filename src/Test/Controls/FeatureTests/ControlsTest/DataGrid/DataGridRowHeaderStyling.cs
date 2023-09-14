using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;



namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Tests for setting RowHeader Style, HeaderTemplate, and Header. Also
    /// tests for precedence.
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridRowHeaderStyling", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DataGridRowHeaderStyling : DataGridTest
    {
        #region Constructor        
                
        public DataGridRowHeaderStyling()
            : base(@"DataGridRowHeaderStyling.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestRowHeaderStyleFromDataGrid);            
            RunSteps += new TestStep(TestRowHeaderStyleFromDataGridRow);
            RunSteps += new TestStep(TestRowHeaderStyleFromDataGridRowAndDataGrid);
            RunSteps += new TestStep(TestRowHeaderTemplateFromDataGridRow_HeaderStyle);
            RunSteps += new TestStep(TestRowHeaderTemplateFromDataGridRow_HeaderStyleAndDataGrid);
            RunSteps += new TestStep(TestRowHeaderTemplateFromDataGridRow_HeaderTemplate);
            //

            RunSteps += new TestStep(TestRowHeaderTemplateFromDataGridRow_HeaderTemplateAndHeader);
            //

            RunSteps += new TestStep(TestRowHeaderTemplateFromDataGridRow_HeaderTemplateAndDataGridCombined);
            RunSteps += new TestStep(TestRowHeaderTemplateFromDataGridRow_HeaderTemplateAndHeaderStyle);
            RunSteps += new TestStep(TestRowHeaderTemplateFromDataGrid);
            //

            RunSteps += new TestStep(TestRowHeaderTemplateFromDataGridAndHeaderCombined);
            RunSteps += new TestStep(TestRowHeaderFromHeader);
            RunSteps += new TestStep(TestRowHeaderFromHeaderAndHeaderStyle);
        }

        #endregion
        
        #region Test Steps

        /// <summary>
        /// Initial Setup  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        public override TestResult Setup()
        {
            base.Setup();

            Status("Setup specific for DataGridRowHeaderStyling");

            this.SetupDataSource();

            LogComment("Setup for DataGridRowHeaderStyling was successful");
            return TestResult.Pass;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestRowHeaderStyleFromDataGrid()
        {
            Status("TestRowHeaderStyleFromDataGrid");

            this.SetHeaderStyles("rowHeaderStyle_DataGrid", string.Empty);           

            DataGridRowHeader header = DataGridHelper.GetRowHeader(MyDataGrid, 0);
            DataGridVerificationHelper.VerifyHeaderProperties(
                header,
                Brushes.LightGreen,
                false,
                "0");

            LogComment("TestRowHeaderStyleFromDataGrid was successful");
            return TestResult.Pass;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestRowHeaderStyleFromDataGridRow()
        {
            Status("TestRowHeaderStyleFromDataGridRow");

            this.SetHeaderStyles(string.Empty, "rowHeaderStyle_DataGridRow");            

            DataGridCell cell = DataGridHelper.GetCell(MyDataGrid, 0, 0);
            TextBlock textBlock = cell.Content as TextBlock;

            DataGridRowHeader header = DataGridHelper.GetRowHeader(MyDataGrid, 0);
            DataGridVerificationHelper.VerifyHeaderProperties(
                header,
                Brushes.Orange,
                false,
                textBlock.Text);

            LogComment("TestRowHeaderStyleFromDataGridRow was successful");
            return TestResult.Pass;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestRowHeaderStyleFromDataGridRowAndDataGrid()
        {
            Status("TestRowHeaderStyleFromDataGridRowAndDataGrid");

            this.SetHeaderStyles("rowHeaderStyle_DataGrid", "rowHeaderStyle_DataGridRow");            

            DataGridCell cell = DataGridHelper.GetCell(MyDataGrid, 0, 0);
            TextBlock textBlock = cell.Content as TextBlock;

            DataGridRowHeader header = DataGridHelper.GetRowHeader(MyDataGrid, 0);
            DataGridVerificationHelper.VerifyHeaderProperties(
                header,
                Brushes.LightGreen,
                false,
                "0");

            LogComment("TestRowHeaderStyleFromDataGridRowAndDataGrid was successful");
            return TestResult.Pass;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestRowHeaderTemplateFromDataGridRow_HeaderStyle()
        {
            Status("TestRowHeaderTemplateFromDataGridRow_HeaderStyle");

            this.SetHeaderStyles(string.Empty, "rowHeaderTemplate_DataGridRow_HeaderStyle");
            
            DataGridRowHeader header = DataGridHelper.GetRowHeader(MyDataGrid, 0);
            DataGridVerificationHelper.VerifyHeaderProperties(
                header,
                Brushes.Violet,
                false,
                "rowHeaderTemplate_DataGridRow_HeaderStyle",
                Brushes.Wheat,
                Brushes.Tomato);

            LogComment("TestRowHeaderTemplateFromDataGridRow_HeaderStyle was successful");
            return TestResult.Pass;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestRowHeaderTemplateFromDataGridRow_HeaderStyleAndDataGrid()
        {
            Status("TestRowHeaderTemplateFromDataGridRow_HeaderStyleAndDataGrid");

            this.SetHeaderStyles("rowHeaderTemplate_DataGridRowHeader_ContentTemplate_Content", "rowHeaderTemplate_DataGridRow_HeaderStyle");            

            DataGridRowHeader header = DataGridHelper.GetRowHeader(MyDataGrid, 0);
            DataGridVerificationHelper.VerifyHeaderProperties(
                header,
                Brushes.LightBlue,
                false,
                "rowHeaderTemplate_DataGridRowHeader_Content",
                Brushes.LightGreen,
                Brushes.Yellow);

            LogComment("TestRowHeaderTemplateFromDataGridRow_HeaderStyleAndDataGrid was successful");
            return TestResult.Pass;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestRowHeaderTemplateFromDataGridRow_HeaderTemplate()
        {
            Status("TestRowHeaderTemplateFromDataGridRow_HeaderTemplate");

            this.SetHeaderStyles(string.Empty, "rowHeaderTemplate_DataGridRow_HeaderTemplate_Header");            

            DataGridRowHeader header = DataGridHelper.GetRowHeader(MyDataGrid, 0);
            DataGridVerificationHelper.VerifyHeaderProperties(
                header,
                null,
                false,
                "rowHeaderTemplate_DataGridRow_Header",
                Brushes.Gold,
                Brushes.DarkRed);

            LogComment("TestRowHeaderTemplateFromDataGridRow_HeaderTemplate was successful");
            return TestResult.Pass;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestRowHeaderTemplateFromDataGridRow_HeaderTemplate_ExplicitBinding()
        {
            Status("TestRowHeaderTemplateFromDataGridRow_HeaderTemplate_ExplicitBinding");

            this.SetHeaderStyles(string.Empty, "rowHeaderTemplate_DataGridRow_HeaderTemplate_ExplicitBinding");

            DataGridCell cell = DataGridHelper.GetCell(MyDataGrid, 0, 0);
            TextBlock textBlock = cell.Content as TextBlock;

            DataGridRowHeader header = DataGridHelper.GetRowHeader(MyDataGrid, 0);
            DataGridVerificationHelper.VerifyHeaderProperties(
                header,
                null,
                false,
                textBlock.Text,
                Brushes.Beige,
                Brushes.Azure);

            LogComment("TestRowHeaderTemplateFromDataGridRow_HeaderTemplate_ExplicitBinding was successful");
            return TestResult.Pass;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestRowHeaderTemplateFromDataGridRow_HeaderTemplateAndHeader()
        {
            Status("TestRowHeaderTemplateFromDataGridRow_HeaderTemplateAndHeader");

            this.SetHeaderStyles(string.Empty, "rowHeaderTemplate_DataGridRow_HeaderTemplate_Header2");
            
            DataGridRowHeader header = DataGridHelper.GetRowHeader(MyDataGrid, 0);
            DataGridVerificationHelper.VerifyHeaderProperties(
                header,
                null,
                true,
                "rowHeaderTemplate_DataGridRow_HeaderTemplate2",
                Brushes.SteelBlue,
                Brushes.SlateGray);

            LogComment("TestRowHeaderTemplateFromDataGridRow_HeaderTemplateAndHeader was successful");
            return TestResult.Pass;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestRowHeaderTemplateFromDataGridRow_HeaderTemplateAndDataGrid()
        {
            Status("TestRowHeaderTemplateFromDataGridRow_HeaderTemplateAndDataGrid");

            this.SetHeaderStyles("rowHeaderTemplate_DataGridRowHeader_ContentTemplate_Content", "rowHeaderTemplate_DataGridRow_HeaderTemplate_Header");
            
            DataGridRowHeader header = DataGridHelper.GetRowHeader(MyDataGrid, 0);
            DataGridVerificationHelper.VerifyHeaderProperties(
                header,
                Brushes.LightBlue,
                false,
                "rowHeaderTemplate_DataGridRowHeader_Content",
                Brushes.LightGreen,
                Brushes.Yellow);

            LogComment("TestRowHeaderTemplateFromDataGridRow_HeaderTemplateAndDataGrid was successful");
            return TestResult.Pass;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestRowHeaderTemplateFromDataGridRow_HeaderTemplateAndDataGridCombined()
        {
            Status("TestRowHeaderTemplateFromDataGridRow_HeaderTemplateAndDataGridCombined");

            this.SetHeaderStyles("rowHeaderStyle_DataGrid", "rowHeaderTemplate_DataGridRow_HeaderTemplate");
           
            DataGridRowHeader header = DataGridHelper.GetRowHeader(MyDataGrid, 0);
            DataGridVerificationHelper.VerifyHeaderProperties(
                header,
                Brushes.LightGreen,
                false,
                "0",
                Brushes.Navy,
                Brushes.Olive);

            LogComment("TestRowHeaderTemplateFromDataGridRow_HeaderTemplateAndDataGridCombined was successful");
            return TestResult.Pass;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestRowHeaderTemplateFromDataGridRow_HeaderTemplateAndHeaderStyle()
        {
            Status("TestRowHeaderTemplateFromDataGridRow_HeaderTemplateAndHeaderStyle");

            this.SetHeaderStyles(string.Empty, "rowHeaderTemplate_DataGridRow_HeaderTemplate_HeaderStyle");

            DataGridRowHeader header = DataGridHelper.GetRowHeader(MyDataGrid, 0);
            DataGridVerificationHelper.VerifyHeaderProperties(
                header,
                Brushes.RosyBrown,
                false,
                "rowHeaderTemplate_DataGridRow_HeaderStyle",
                Brushes.Salmon,
                Brushes.Peru);

            LogComment("TestRowHeaderTemplateFromDataGridRow_HeaderTemplateAndHeaderStyle was successful");
            return TestResult.Pass;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestRowHeaderTemplateFromDataGrid()
        {
            Status("TestRowHeaderTemplateFromDataGrid");

            this.SetHeaderStyles("rowHeaderTemplate_DataGridRowHeader_ContentTemplate_Content", string.Empty);
            
            DataGridRowHeader header = DataGridHelper.GetRowHeader(MyDataGrid, 0);
            DataGridVerificationHelper.VerifyHeaderProperties(
                header,
                Brushes.LightBlue,
                false,
                "rowHeaderTemplate_DataGridRowHeader_Content",
                Brushes.LightGreen,
                Brushes.Yellow);

            LogComment("TestRowHeaderTemplateFromDataGrid was successful");
            return TestResult.Pass;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestRowHeaderTemplateFromDataGrid_ExplicitBinding()
        {
            Status("TestRowHeaderTemplateFromDataGrid_ExplicitBinding");

            this.SetHeaderStyles("rowHeaderTemplate_DataGridRowHeader_ContentTemplate_ExplicitBinding", string.Empty);

            DataGridCell cell = DataGridHelper.GetCell(MyDataGrid, 0, 0);
            TextBlock textBlock = cell.Content as TextBlock;

            DataGridRowHeader header = DataGridHelper.GetRowHeader(MyDataGrid, 0);
            DataGridVerificationHelper.VerifyHeaderProperties(
                header,
                Brushes.Chartreuse,
                false,
                textBlock.Text,
                Brushes.BlanchedAlmond,
                Brushes.Aquamarine);

            LogComment("TestRowHeaderTemplateFromDataGrid_ExplicitBinding was successful");
            return TestResult.Pass;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestRowHeaderTemplateFromDataGridAndHeaderCombined()
        {
            Status("TestRowHeaderTemplateFromDataGridAndHeaderCombined");

            this.SetHeaderStyles("rowHeaderTemplate_DataGridRowHeader_ContentTemplate", "rowHeader_DataGridRow_Header");
            
            DataGridCell cell = DataGridHelper.GetCell(MyDataGrid, 0, 4);
            TextBlock textBlock = cell.Content as TextBlock;
            Inline inline = textBlock.Inlines.FirstInline;
            Hyperlink link = inline as Hyperlink;
            string expectedContent = link.NavigateUri.OriginalString;

            DataGridRowHeader header = DataGridHelper.GetRowHeader(MyDataGrid, 0);
            DataGridVerificationHelper.VerifyHeaderProperties(
                header,
                Brushes.LightBlue,
                false,
                expectedContent,
                Brushes.LightGreen,
                Brushes.Yellow);

            LogComment("TestRowHeaderTemplateFromDataGridAndHeaderCombined was successful");
            return TestResult.Pass;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestRowHeaderFromHeader()
        {
            Status("TestRowHeaderFromHeader");

            this.SetHeaderStyles(string.Empty, "rowHeader_DataGridRow_Header");
            
            DataGridCell cell = DataGridHelper.GetCell(MyDataGrid, 0, 4);
            TextBlock textBlock = cell.Content as TextBlock;
            Inline inline = textBlock.Inlines.FirstInline;
            Hyperlink link = inline as Hyperlink;
            string expectedContent = link.NavigateUri.OriginalString;

            DataGridRowHeader header = DataGridHelper.GetRowHeader(MyDataGrid, 0);
            DataGridVerificationHelper.VerifyHeaderProperties(
                header,
                null,
                false,
                expectedContent);

            LogComment("TestRowHeaderFromHeader was successful");
            return TestResult.Pass;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestRowHeaderFromHeaderAndHeaderStyle()
        {
            Status("TestRowHeaderFromHeaderAndHeaderStyle");

            this.SetHeaderStyles(string.Empty, "rowHeader_DataGridRow_Header_HeaderStyle");
            
            DataGridCell cell = DataGridHelper.GetCell(MyDataGrid, 0, 1);
            TextBlock textBlock = cell.Content as TextBlock;

            DataGridRowHeader header = DataGridHelper.GetRowHeader(MyDataGrid, 0);
            DataGridVerificationHelper.VerifyHeaderProperties(
                header,
                Brushes.Red,
                false,
                textBlock.Text);

            LogComment("TestRowHeaderFromHeaderAndHeaderStyle was successful");
            return TestResult.Pass;
        }

        #endregion Test Steps

        #region Helpers

        private void SetHeaderStyles(string rowHeaderStyleResourceName, string itemContainerStyleResourceName)
        {
            if (rowHeaderStyleResourceName == null)
            {
                MyDataGrid.RowHeaderStyle = null;
            }
            else if (rowHeaderStyleResourceName == string.Empty)
            {
                // treated as clearing the value
                MyDataGrid.ClearValue(DataGrid.RowHeaderStyleProperty);
            }
            else
            {
                Style rowHeaderStyle = (Style)MyDataGrid.FindResource(rowHeaderStyleResourceName);
                MyDataGrid.RowHeaderStyle = rowHeaderStyle;
            }

            if (itemContainerStyleResourceName == null)
            {
                MyDataGrid.ItemContainerStyle = null;
            }
            else if (itemContainerStyleResourceName == string.Empty)
            {
                // treated as clearing the value
                MyDataGrid.ClearValue(DataGrid.ItemContainerStyleProperty);
            }
            else
            {
                Style itemContainerStyle = (Style)MyDataGrid.FindResource(itemContainerStyleResourceName);
                MyDataGrid.ItemContainerStyle = itemContainerStyle;
            }            
            
            MyDataGrid.UpdateLayout();
            this.WaitForPriority(DispatcherPriority.SystemIdle);
        }

        #endregion Helpers
    }
}
