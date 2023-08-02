using System.Text;
using System.Windows.Threading;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.Windows.Controls;


namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Tests the auto-sizing for column and column headers.  The factors are the
    /// types of DataGridLength widths: Auto, SizeToCells, SizeToHeader, Absolute, 
    /// and Star. The other factor is the width of the column to the column header.
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridColumnAndColumnHeaderSizing", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DataGridColumnAndColumnHeaderSizing : DataGridTest
    {
        #region Constructor

        public DataGridColumnAndColumnHeaderSizing()
            : base(@"DataGridColumnAndColumnHeaderSizing.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestColumnWidthAutoHeaderLarger);
            RunSteps += new TestStep(TestColumnWidthAutoCellLarger);
            RunSteps += new TestStep(TestColumnWidthAutoHeaderAbsoluteLarger);
            //


            RunSteps += new TestStep(TestColumnWidthSizeToCellsHeaderLarger);
            RunSteps += new TestStep(TestColumnWidthSizeToCellsCellLarger);
            //




            RunSteps += new TestStep(TestColumnWidthSizeToHeaderHeaderLarger);
            RunSteps += new TestStep(TestColumnWidthSizeToHeaderCellLarger);
            RunSteps += new TestStep(TestColumnWidthSizeToHeaderHeaderAbsoluteLarger);
            RunSteps += new TestStep(TestColumnWidthSizeToHeaderHeaderAbsoluteAndColumnLarger);

            RunSteps += new TestStep(TestColumnWidthAbsoluteHeaderLarger);
            RunSteps += new TestStep(TestColumnWidthAbsoluteCellLarger);
            //



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

            Status("Setup specific for DataGridColumnAndColumnHeaderSizing");

            this.SetupDataSource();

            LogComment("Setup for DataGridColumnAndColumnHeaderSizing was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify actual width when column header is longer than columns content for width = auto
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestColumnWidthAutoHeaderLarger()
        {
            Status("TestColumnWidthAutoHeaderLarger");

            LogComment("Verify when header is larger for auto");
            VerifyColumnWidthHelper("autoHeaderLarger", this.VerifyWidthsMatchDesiredHeader);

            LogComment("TestColumnWidthAutoHeaderLarger was successful");
            return TestResult.Pass;
        }

        /// <summary>
        ///  Verify actual width when column content width is longer than column header width for width = auto
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestColumnWidthAutoCellLarger()
        {
            Status("TestColumnWidthAutoCellLarger");

            LogComment("Verify when cell is larger for auto");
            VerifyColumnWidthHelper("autoColumnLarger", this.VerifyWidthsMatchDesiredLargestCell);

            LogComment("TestColumnWidthAutoCellLarger was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify actual width when column header absolute size is longer than columns size to content
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestColumnWidthAutoHeaderAbsoluteLarger()
        {
            Status("TestColumnWidthAutoHeaderAbsoluteLarger");

            LogComment("Verify when header is larger for auto");
            VerifyColumnWidthHelper("autoHeaderAbsoluteLarger", this.VerifyWidthsMatchDesiredHeader);

            LogComment("TestColumnWidthAutoHeaderAbsoluteLarger was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify actual width when column size to content is longer than column header absolute size
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestColumnWidthAutoHeaderAbsoluteAndColumnLarger()
        {
            Status("TestColumnWidthAutoHeaderAbsoluteAndColumnLarger");

            LogComment("Verify when header is absolute but cell is larger for auto");
            VerifyColumnWidthHelper("autoHeaderAbsolute_ColumnLarger", this.VerifyWidthsMatchDesiredLargestCell);

            LogComment("TestColumnWidthAutoHeaderAbsoluteAndColumnLarger was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify actual width when column header size to content is longer than columns size to content
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestColumnWidthSizeToCellsHeaderLarger()
        {
            Status("TestColumnWidthSizeToCellsHeaderLarger");

            LogComment("Verify when header is larger for size to cell");
            VerifyColumnWidthHelper("sizeToCellsHeaderLarger", this.VerifyWidthsMatchDesiredLargestCell);

            LogComment("TestColumnWidthSizeToCellsHeaderLarger was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify actual width when column size to content is longer than column header size to content
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestColumnWidthSizeToCellsCellLarger()
        {
            Status("TestColumnWidthSizeToCellsCellLarger");

            LogComment("Verify when cell is larger for size to cell");
            VerifyColumnWidthHelper("sizeToCellsColumnLarger", this.VerifyWidthsMatchDesiredLargestCell);

            LogComment("TestColumnWidthSizeToCellsCellLarger was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify actual width when column header absolute size is longer than columns size to content
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestColumnWidthSizeToCellsHeaderAbsoluteLarger()
        {
            Status("TestColumnWidthSizeToCellsHeaderAbsoluteLarger");

            LogComment("Verify when header is larger for size to cell");
            VerifyColumnWidthHelper("sizeToCellsHeaderAbsoluteLarger", this.VerifyWidthsMatchDesiredLargestCell);

            LogComment("TestColumnWidthSizeToCellsHeaderAbsoluteLarger was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify actual width when column size to content is longer than column header absolute size
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestColumnWidthSizeToCellsHeaderAbsoluteAndColumnLarger()
        {
            Status("TestColumnWidthSizeToCellsHeaderAbsoluteAndColumnLarger");

            LogComment("Verify when header is absolute but cell is larger for size to cells");
            VerifyColumnWidthHelper("sizeToCellsHeaderAbsolute_ColumnLarger", this.VerifyWidthsMatchDesiredLargestCell);

            LogComment("TestColumnWidthSizeToCellsHeaderAbsoluteAndColumnLarger was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 1. Verify actual width when column header size to content is longer than columns size to content
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestColumnWidthSizeToHeaderHeaderLarger()
        {
            Status("TestColumnWidthSizeToHeaderHeaderLarger");

            LogComment("Verify when header is larger for size to header");
            VerifyColumnWidthHelper("sizeToHeaderHeaderLarger", this.VerifyWidthsMatchDesiredHeader);

            LogComment("TestColumnWidthSizeToHeaderHeaderLarger was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify actual width when column size to content is longer than column header size to content
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestColumnWidthSizeToHeaderCellLarger()
        {
            Status("TestColumnWidthSizeToHeaderCellLarger");

            LogComment("Verify when cell is larger for size to header");
            VerifyColumnWidthHelper("sizeToHeaderColumnLarger", this.VerifyWidthsMatchDesiredHeader);

            LogComment("TestColumnWidthSizeToHeaderCellLarger was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify actual width when column header absolute size is longer than columns size to content
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestColumnWidthSizeToHeaderHeaderAbsoluteLarger()
        {
            Status("TestColumnWidthSizeToHeaderHeaderAbsoluteLarger");

            LogComment("Verify when header is larger for size to header");
            VerifyColumnWidthHelper("sizeToHeaderHeaderAbsoluteLarger", this.VerifyWidthsMatchDesiredHeader);

            LogComment("TestColumnWidthSizeToHeaderHeaderAbsoluteLarger was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify actual width when column size to content is longer than column header absolute size
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestColumnWidthSizeToHeaderHeaderAbsoluteAndColumnLarger()
        {
            Status("TestColumnWidthSizeToHeaderHeaderAbsoluteAndColumnLarger");

            LogComment("Verify when header is absolute but cell is larger for size to header");
            VerifyColumnWidthHelper("sizeToHeaderHeaderAbsolute_ColumnLarger", this.VerifyWidthsMatchDesiredHeader);

            LogComment("TestColumnWidthSizeToHeaderHeaderAbsoluteAndColumnLarger was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify actual width when column header size to content is longer than columns size to content
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestColumnWidthAbsoluteHeaderLarger()
        {
            Status("TestColumnWidthAbsoluteHeaderLarger");

            LogComment("Verify when header is larger for absolute");
            VerifyColumnWidthHelper("absoluteHeaderLarger", this.VerifyWidthsMatchDesiredColumn);

            LogComment("TestColumnWidthAbsoluteHeaderLarger was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify actual width when column size to content is longer than column header size to content
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestColumnWidthAbsoluteCellLarger()
        {
            Status("TestColumnWidthAbsoluteCellLarger");

            LogComment("Verify when cell is larger for absolute");
            VerifyColumnWidthHelper("absoluteColumnLarger", this.VerifyWidthsMatchDesiredColumn);

            LogComment("TestColumnWidthAbsoluteCellLarger was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify actual width when column header absolute size is longer than columns size to content
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestColumnWidthAbsoluteHeaderAbsoluteLarger()
        {
            Status("TestColumnWidthAbsoluteHeaderAbsoluteLarger");

            LogComment("Verify when header is larger for absolute");
            VerifyColumnWidthHelper("absoluteAbsoluteLarger", this.VerifyWidthsMatchDesiredColumn);

            LogComment("TestColumnWidthAbsoluteHeaderAbsoluteLarger was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify actual width when column size to content is longer than column header absolute size
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestColumnWidthAbsoluteHeaderAbsoluteAndColumnLarger()
        {
            Status("TestColumnWidthAbsoluteHeaderAbsoluteAndColumnLarger");

            LogComment("Verify when header is absolute but cell is larger for absolute");
            VerifyColumnWidthHelper("absoluteHeaderAbsolute_ColumnLarger", this.VerifyWidthsMatchDesiredColumn);  

            LogComment("TestColumnWidthAbsoluteHeaderAbsoluteAndColumnLarger was successful");
            return TestResult.Pass;
        }        

        #endregion Test Steps

        #region Helpers

        public delegate void VerifyColumnWidths(DataGridHelper.ColumnWidthData columnWidths);
        public void VerifyColumnWidthHelper(string columnName, VerifyColumnWidths VerifyColumnWidths)
        {
            DataGridHelper.ColumnWidthData columnWidths;
            DataGridHelper.GetColumnWidthData(MyDataGrid, columnName, out columnWidths, false);
            VerifyColumnWidths(columnWidths);
        }

        public void VerifyWidthsMatchDesiredColumn(DataGridHelper.ColumnWidthData columnWidths)
        {
            if (columnWidths.actualWidth != columnWidths.width ||
                columnWidths.actualWidth != columnWidths.headerWidth)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(string.Format("Column Width: {0} should be the same size as desired column width: {1}.\n", columnWidths.actualWidth, columnWidths.width));
                sb.Append(string.Format("Column Width: {0} should be the same size as largest cell width: {1}.\n", columnWidths.actualWidth, columnWidths.headerWidth));
                throw new TestValidationException(sb.ToString());
            }
        }

        public void VerifyWidthsMatchDesiredHeader(DataGridHelper.ColumnWidthData columnWidths)
        {
            if (columnWidths.actualWidth != columnWidths.desiredHeaderWidth ||
                columnWidths.actualWidth != columnWidths.largestCellWidth)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(string.Format("Column Width: {0} should be the same size as desired header width: {1}.\n", columnWidths.actualWidth, columnWidths.headerWidth));
                sb.Append(string.Format("Column Width: {0} should be the same size as largest cell width: {1}.\n", columnWidths.actualWidth, columnWidths.largestCellWidth));
                throw new TestValidationException(sb.ToString());
            }
        }

        public void VerifyWidthsMatchDesiredLargestCell(DataGridHelper.ColumnWidthData columnWidths)
        {
            if (columnWidths.actualWidth != columnWidths.desiredLargestCellWidth ||
                columnWidths.actualWidth != columnWidths.headerWidth)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(string.Format("Column Width: {0} should be the same size as desired largest cell width: {1}.\n", columnWidths.actualWidth, columnWidths.desiredLargestCellWidth));
                sb.Append(string.Format("Column Width: {0} should be the same size as header width: {1}.\n", columnWidths.actualWidth, columnWidths.headerWidth));
                throw new TestValidationException(sb.ToString());
            }
        }

        public void VerifyWidthsMatchHeader(DataGridHelper.ColumnWidthData columnWidths)
        {
            if (columnWidths.actualWidth != columnWidths.headerWidth ||
                columnWidths.actualWidth != columnWidths.largestCellWidth)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(string.Format("Column Width: {0} should be the same size as header width: {1}.\n", columnWidths.actualWidth, columnWidths.headerWidth));
                sb.Append(string.Format("Column Width: {0} should be the same size as largest cell width: {1}.\n", columnWidths.actualWidth, columnWidths.largestCellWidth));
                throw new TestValidationException(sb.ToString());
            } 
        }

        #endregion Helpers
    }
}
