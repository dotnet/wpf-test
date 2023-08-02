using System.Windows.Controls;

namespace Microsoft.Test.Controls.Helpers
{
#if TESTBUILD_CLR40
    /// <summary>
    /// DataGrid selection test helper
    /// </summary>
    public class DataGridSelectionTestHelper
    {
        /// <summary>
        /// Create a new DataGrid with selection mode and unit, and bind data to ItemsSource.
        /// </summary>
        /// <param name="selectionMode">DataGridSelectionMode</param>
        /// <param name="selectionUnit">DataGridSelectionUnit</param>
        /// <returns>returns a datagrid.</returns>
        public static DataGrid CreateDataGrid(DataGridSelectionMode selectionMode, DataGridSelectionUnit selectionUnit)
        {
            DataGrid dataGrid = new DataGrid();
            dataGrid.ItemsSource = DataGridBuilder.Construct();
            dataGrid.SelectionMode = selectionMode;
            dataGrid.SelectionUnit = selectionUnit;

            return dataGrid;
        }
    }
#endif
}
