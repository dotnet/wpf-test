using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Tests for setting Column Header Style, HeaderTemplate, and
    /// Header.  Also tests precedence of each.
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridColumnHeaderStyling", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords = "Localization_Suite")]
    public class DataGridColumnHeaderStyling : DataGridTest
    {
        #region Constructor

        public DataGridColumnHeaderStyling()
            : base(@"DataGridColumnHeaderStyling.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestColumnHeaderStyleFromDataGrid);
            RunSteps += new TestStep(TestHeaderOverridesDataGrid);
            RunSteps += new TestStep(TestHeaderStyleOverridesDataGrid);
            RunSteps += new TestStep(TestHeaderTemplateInStyleOverridesDataGrid);
            RunSteps += new TestStep(TestHeaderTemplateOverridesDataGrid);
            RunSteps += new TestStep(TestHeaderAndHeaderStylePrecedence);
            RunSteps += new TestStep(TestHeaderTemplateAndHeaderPrecedence);
            RunSteps += new TestStep(TestHeaderTemplateAndHeaderStylePrecedence);
            RunSteps += new TestStep(TestHeaderTemplateAndHeaderStyleContentTemplate);
            RunSteps += new TestStep(TestHeaderTemplateAndHeaderStyleAndHeader);
            RunSteps += new TestStep(TestHeaderStringFormat);                                                       
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

            Status("Setup specific for DataGridColumnHeaderStyling");

            this.SetupDataSource();

            LogComment("Setup for DataGridColumnHeaderStyling was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify setting ColumnHeaderStyle on DataGrid sets the header style of columns.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestColumnHeaderStyleFromDataGrid()
        {
            Status("TestColumnHeaderStyleFromDataGrid");

            this.VerifyHeaderProperties(
                "testDefaultHeaderStyle",
                Brushes.Orange,
                false,
                "DefaultHeaderContent",
                Brushes.LightGreen,
                Brushes.Yellow);            

            LogComment("TestColumnHeaderStyleFromDataGrid was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify Header overrides ColumnHeaderStyle but other properties set on ColumnHeaderStyle should still work.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestHeaderOverridesDataGrid()
        {
            Status("TestHeaderOverridesDataGrid");

            this.VerifyHeaderProperties(
                "testHeader",
                Brushes.Orange,
                false,
                "right",
                Brushes.LightGreen,
                Brushes.Yellow);

            LogComment("TestHeaderOverridesDataGrid was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify HeaderStyle overrides ColumnHeaderStyle.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestHeaderStyleOverridesDataGrid()
        {
            Status("TestHeaderStyleOverridesDataGrid");

            this.VerifyHeaderProperties(
                "testHeaderStyle",
                Brushes.LightGreen,
                false,
                "0",
                Brushes.LightGreen,
                Brushes.Yellow);

            LogComment("TestHeaderStyleOverridesDataGrid was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify ContentTemplate in HeaderStyle takes precedence over ColumnHeaderStyle
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestHeaderTemplateInStyleOverridesDataGrid()
        {
            Status("TestHeaderTemplateInStyleOverridesDataGrid");

            this.VerifyHeaderProperties(
                "testContentTemplateFromHeaderStyle",
                Brushes.LightGreen,
                true,
                "right",
                Brushes.LightBlue,
                Brushes.Red);

            LogComment("TestHeaderTemplateInStyleOverridesDataGrid was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify HeaderTemplate takes precedence over ColumnHeaderStyle
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestHeaderTemplateOverridesDataGrid()
        {
            Status("TestHeaderTemplateOverridesDataGrid");

            this.VerifyHeaderProperties(
                "testHeaderTemplate",
                Brushes.Orange,
                true,
                "right",
                Brushes.LightBlue,
                Brushes.Red);

            LogComment("TestHeaderTemplateOverridesDataGrid was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify Header takes precedence over HeaderStyle, but other properties in HeaderStyle should be set.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestHeaderAndHeaderStylePrecedence()
        {
            Status("TestHeaderAndHeaderStylePrecedence");

            this.VerifyHeaderProperties(
                "testHeaderAndHeaderStyle",
                Brushes.LightGreen,
                false,
                "right",
                Brushes.LightGreen,
                Brushes.Yellow);

            LogComment("TestHeaderAndHeaderStylePrecedence was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify Header takes precedence over HeaderTemplate, but other properties in HeaderStyle should be set.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestHeaderTemplateAndHeaderPrecedence()
        {
            Status("TestHeaderTemplateAndHeaderPrecedence");

            this.VerifyHeaderProperties(
                "testHeaderTemplateAndHeader",
                Brushes.Orange,
                true,
                "right",
                Brushes.LightBlue,
                Brushes.Red);

            LogComment("TestHeaderTemplateAndHeaderPrecedence was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify HeaderTemplate takes precedence over HeaderStyle, but other properties in HeaderStyle should be set.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestHeaderTemplateAndHeaderStylePrecedence()
        {
            Status("TestHeaderTemplateAndHeaderStylePrecedence");

            this.VerifyHeaderProperties(
                "testHeaderTemplateAndHeaderStyle",
                Brushes.LightGreen,
                false,
                "0",
                Brushes.LightBlue,
                Brushes.Red);


            LogComment("TestHeaderTemplateAndHeaderStylePrecedence was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestHeaderTemplateAndHeaderStyleContentTemplate()
        {
            Status("TestHeaderTemplateAndHeaderStyleContentTemplate");

            this.VerifyHeaderProperties(
                "testHeaderTemplateAndContentTemplate",
                Brushes.LightGreen,
                true,
                "right",
                Brushes.LightBlue,
                Brushes.Red);


            LogComment("TestHeaderTemplateAndHeaderStyleContentTemplate was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestHeaderTemplateAndHeaderStyleAndHeader()
        {
            Status("TestHeaderTemplateAndHeaderStyleAndHeader");

            this.VerifyHeaderProperties(
                "testHeaderAndHeaderTemplateAndHeaderStyle",
                Brushes.LightGreen,
                true,
                "right",
                Brushes.LightBlue,
                Brushes.Red);


            LogComment("TestHeaderTemplateAndHeaderStyleAndHeader was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify HeaderStringFormat basic functionality.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestHeaderStringFormat()
        {
            Status("TestHeaderStringFormat");

            DataGridTextColumn textColumn = MyDataGrid.FindName("testHeaderStringFormat") as DataGridTextColumn;
            if (textColumn == null)
            {
                throw new NullReferenceException(string.Format("Unable to find the DataGridTexColumn by name: {0}", "testHeaderStringFormat"));
            }
            DataGridColumnHeader columnHeader = DataGridHelper.GetColumnHeader(MyDataGrid, DataGridHelper.FindColumnIndex(MyDataGrid, textColumn));

            if (columnHeader.ContentStringFormat != "Test StringFormat: {0}")
            {
                LogComment(string.Format(
                    "Expected HeaderStringFormat: {0}, Actual HeaderStringFormat: {1}",
                    "Test StringFormat: {0}",
                    columnHeader.ContentStringFormat));
                return TestResult.Fail;
            }

            LogComment("TestHeaderStringFormat was successful");
            return TestResult.Pass;
        }

        #endregion Test Steps

        #region Helpers

        private void VerifyHeaderProperties(
            string columnName,
            Brush expectedHeaderBackground,
            bool isExpectedContentOverrideByTemplate,
            string expectedContent,
            Brush expectedContentTemplateBackground,
            Brush expectedContentTemplateForeground)
        {
            DataGridTextColumn textColumn = MyDataGrid.FindName(columnName) as DataGridTextColumn;
            if (textColumn == null)
            {
                throw new NullReferenceException(string.Format("Unable to find the DataGridTexColumn by name: {0}", columnName));
            }
            DataGridColumnHeader columnHeader = DataGridHelper.GetColumnHeader(MyDataGrid, DataGridHelper.FindColumnIndex(MyDataGrid, textColumn));

            DataGridVerificationHelper.VerifyHeaderProperties(
                columnHeader,
                expectedHeaderBackground,
                isExpectedContentOverrideByTemplate,
                expectedContent,
                expectedContentTemplateBackground,
                expectedContentTemplateForeground);
        }

        #endregion Helpers
    }
}
