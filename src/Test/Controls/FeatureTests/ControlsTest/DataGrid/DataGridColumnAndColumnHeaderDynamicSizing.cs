using System.Text;
using System.Windows.Threading;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.Collections.ObjectModel;
using Avalon.Test.ComponentModel.Utilities;
using System.Windows.Controls;


namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Tests for auto-sizing with dynamic changes to the widths.  The factors are the
    /// types of DataGridLength widths: Auto, SizeToCells, SizeToHeader, Absolute, and Star.
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridColumnAndColumnHeaderDynamicSizing", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords = "Localization_Suite")]
    public class DataGridColumnAndColumnHeaderDynamicSizing : DataGridTest
    {
        #region Constructor

        public DataGridColumnAndColumnHeaderDynamicSizing()
            : base(@"DataGridColumnAndColumnHeaderSizing.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestColumnWidthAutoDynamicChange);
            RunSteps += new TestStep(TestColumnWidthSizeToCellsDynamicChange);
            RunSteps += new TestStep(TestColumnWidthSizeToHeaderDynamicChange);
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

            Status("Setup specific for DataGridColumnAndColumnHeaderDynamicSizing");

            this.SetupDataSource();

            LogComment("Setup for DataGridColumnAndColumnHeaderDynamicSizing was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// For the case where a larger header and width set to auto, 
        /// verify adding a column where content is larger updates the 
        /// entire column width to the width value of  the new content.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestColumnWidthAutoDynamicChange()
        {
            Status("TestColumnWidthAutoDynamicChange");

            VerifyColumnWidthHelper(
                "autoHeaderLarger", 
                new Person("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXX", "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"),
                this.VerifyColumnWidthGrowsAfterInsert,
                this.VerifyColumnWidthUnchangedOnReset);            

            LogComment("TestColumnWidthAutoDynamicChange was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// For the case where a larger header and width set to sizeToCells, 
        /// verify adding a column where content is larger updates the entire 
        /// column width to the width value of  the new content.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestColumnWidthSizeToCellsDynamicChange()
        {
            Status("TestColumnWidthSizeToCellsDynamicChange");

            VerifyColumnWidthHelper(
                "sizeToCellsHeaderLarger", 
                new Person("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX", "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"),
                this.VerifyColumnWidthGrowsAfterInsert,
                this.VerifyColumnWidthUnchangedOnReset);            

            LogComment("TestColumnWidthSizeToCellsDynamicChange was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// For the case where a larger header and width set to sizeToHeader, 
        /// verify adding a column where content is larger does not change the
        /// column width.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestColumnWidthSizeToHeaderDynamicChange()
        {
            Status("TestColumnWidthSizeToHeaderDynamicChange");

            VerifyColumnWidthHelper(
                "sizeToHeaderHeaderLarger", 
                new Person("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXX", "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"),
                this.VerifyColumnWidthUnchangedAfterInsert,
                this.VerifyColumnWidthUnchangedOnReset);            

            LogComment("TestColumnWidthSizeToHeaderDynamicChange was successful");
            return TestResult.Pass;
        }        

        #endregion Test Steps

        #region Helpers

        public delegate void VerifyColumnWidths(DataGridHelper.ColumnWidthData columnWidthsBefore, DataGridHelper.ColumnWidthData columnWidthsAfter);

        public void VerifyColumnWidthHelper(
            string columnName,
            Person personToAdd,
            VerifyColumnWidths VerifyColumnWidthsAfterInsert,
            VerifyColumnWidths VerifyColumnWidthsOnReset)
        {
            DataGridHelper.ColumnWidthData columnWidths;
            DataGridHelper.GetColumnWidthData(MyDataGrid, columnName, out columnWidths, false);

            // need to scroll back into view for the column widths to update
            // note that this is by design that column widths do not update
            // when content that is virtualized is updated.
            MyDataGrid.ScrollIntoView(MyDataGrid.Items[0]);
            WaitForPriority(DispatcherPriority.SystemIdle);

            // update data with cell data larger than header
            ObservableCollection<Person> personDataSource = DataSource as ObservableCollection<Person>;
            Assert.AssertTrue("DataSource must be an ObservableCollection<T>", personDataSource != null);

            personDataSource.Insert(0, personToAdd);
            WaitForPriority(DispatcherPriority.SystemIdle);
            
            DataGridHelper.ColumnWidthData columnWidths2;
            DataGridHelper.GetColumnWidthData(MyDataGrid, columnName, out columnWidths2, false);
            
            VerifyColumnWidthsAfterInsert(columnWidths, columnWidths2);            

            // need to scroll back into view for the column widths to update
            // note that this is by design that column widths do not update
            // when content that is virtualized is updated.
            MyDataGrid.ScrollIntoView(MyDataGrid.Items[0]);
            WaitForPriority(DispatcherPriority.SystemIdle);

            //




            //DataGridHelper.ColumnWidths columnWidths3;
            //DataGridHelper.GetWidthValuesFromColumn(MyDataGrid, columnName, out columnWidths3);

            //VerifyColumnWidthsOnReset(columnWidths, columnWidths3);            
        }

        public void VerifyColumnWidthGrowsAfterInsert(
            DataGridHelper.ColumnWidthData columnWidthsBefore, 
            DataGridHelper.ColumnWidthData columnWidthsAfter)
        {
            // verify column width is now larger
            if (columnWidthsAfter.actualWidth <= columnWidthsBefore.actualWidth ||
                columnWidthsAfter.actualWidth != columnWidthsAfter.headerWidth ||
                columnWidthsAfter.actualWidth != columnWidthsAfter.largestCellWidth)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(string.Format("Column Width2: {0} should be the greater than original column width: {1}.\n", columnWidthsAfter.actualWidth, columnWidthsBefore.actualWidth));
                sb.Append(string.Format("Column Width: {0} should be the same size as desired header width: {1}.\n", columnWidthsAfter.actualWidth, columnWidthsAfter.headerWidth));
                sb.Append(string.Format("Column Width: {0} should be the same size as largest cell width: {1}.\n", columnWidthsAfter.actualWidth, columnWidthsAfter.largestCellWidth));
                throw new TestValidationException(sb.ToString());
            }
        }

        public void VerifyColumnWidthUnchangedAfterInsert(
            DataGridHelper.ColumnWidthData columnWidthsBefore,
            DataGridHelper.ColumnWidthData columnWidthsAfter)
        {
            // verify column width stays the same
            if (columnWidthsAfter.actualWidth != columnWidthsBefore.actualWidth ||
                columnWidthsAfter.actualWidth != columnWidthsAfter.headerWidth ||
                columnWidthsAfter.actualWidth != columnWidthsAfter.largestCellWidth)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(string.Format("Column Width2: {0} should be equal to the original column width: {1}.\n", columnWidthsAfter.actualWidth, columnWidthsBefore.actualWidth));
                sb.Append(string.Format("Column Width: {0} should be the same size as desired header width: {1}.\n", columnWidthsAfter.actualWidth, columnWidthsAfter.headerWidth));
                sb.Append(string.Format("Column Width: {0} should be the same size as largest cell width: {1}.\n", columnWidthsAfter.actualWidth, columnWidthsAfter.largestCellWidth));
                throw new TestValidationException(sb.ToString());
            }
        }

        public void VerifyColumnWidthUnchangedOnReset(
            DataGridHelper.ColumnWidthData columnWidthsBefore,
            DataGridHelper.ColumnWidthData columnWidthsAfter)
        {
            // verify it sized back down to the original
            if (columnWidthsAfter.actualWidth != columnWidthsBefore.actualWidth || 
                columnWidthsAfter.headerWidth != columnWidthsBefore.headerWidth || 
                columnWidthsAfter.largestCellWidth != columnWidthsBefore.largestCellWidth)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(string.Format("Column Width3: {0} should be equal to the original column width: {1}.", columnWidthsAfter.actualWidth, columnWidthsBefore.actualWidth));
                sb.Append(string.Format("Column header Width3: {0} should be  equal to the original header width: {1}.", columnWidthsAfter.headerWidth, columnWidthsBefore.headerWidth));
                sb.Append(string.Format("Largest cell Width3: {0} should be equal to the original largest cell width: {1}.", columnWidthsAfter.largestCellWidth, columnWidthsBefore.largestCellWidth));
                throw new TestValidationException(sb.ToString());
            }
        }        

        #endregion Helpers
    }
}
