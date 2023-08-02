using System.Collections.Generic;
using System.Reflection;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Microsoft.Test.Controls.Helpers
{
#if TESTBUILD_CLR40
    public static class DataGridSelectionHelper
    {
        /// <summary>
        /// Get selected cells from a DataGrid.
        /// </summary>
        /// <param name="dataGrid">An instance of DataGrid.</param>
        /// <returns>A collection of selected cells.</returns>
        public static List<DataGridCell> GetSelectedCells(DataGrid dataGrid)
        {
            List<DataGridCell> cells = new List<DataGridCell>();

            foreach (DataGridCellInfo selectedCellInfo in dataGrid.SelectedCells)
            {
                // Get selected cell.
                DataGridCell selectedCell = DataGridHelper.GetCell(selectedCellInfo);
                if (selectedCell == null)
                {
                    throw new TestValidationException("DataGridCell is null");
                }

                cells.Add(selectedCell);
            }

            return cells;
        }

        /// <summary>
        /// Get a collection of selected cells coordinate of a DataGrid.
        /// </summary>
        /// <param name="dataGrid">An instance of DataGrid.</param>
        /// <returns>A collection of selected cells coordinate.</returns>
        public static List<CellCoordinate> GetSelectedCellsCoordinate(DataGrid dataGrid)
        {
            List<CellCoordinate> cellsCoordinate = new List<CellCoordinate>();

            foreach (DataGridCellInfo selectedCellInfo in dataGrid.SelectedCells)
            {
                // Get selected cell.
                DataGridCell selectedCell = DataGridHelper.GetCell(selectedCellInfo);
                if (selectedCell == null)
                {
                    throw new TestValidationException("DataGridCell is null");
                }

                // Create the selected cell-coordinate.
                CellCoordinate selectedCellCoordinate = new CellCoordinate(DataGridHelper.GetColumnDisplayIndex(selectedCell), DataGridHelper.GetRowIndex(selectedCell));

                // Make sure the new selected cell is not the actual selected cell collection already.
                foreach (CellCoordinate cell in cellsCoordinate)
                {
                    if (cell.ToString().Equals(selectedCellCoordinate.ToString()))
                    {
                        throw new TestValidationException("A duplicate selected cell at " + selectedCellCoordinate.ToString() + " in DataGrid.SelectedCells");
                    }
                }

                // Add the selected cell-coordinate to actual selected cell collection.
                cellsCoordinate.Add(selectedCellCoordinate);
            }

            return cellsCoordinate;
        }

        /// <summary>
        /// Get a collection of cells coordinate of a DataGridRowHeader.
        /// </summary>
        /// <param name="dataGridRowHeader">An instance of DataGridRowHeader.</param>
        /// <returns>A collection of cells coordinate</returns>
        public static List<CellCoordinate> GetCellsCoordinate(DataGridRowHeader dataGridRowHeader)
        {
            List<CellCoordinate> cellsCoordinate = new List<CellCoordinate>();

            DataGridRow row = DataGridHelper.GetRow(dataGridRowHeader);
            if (row == null)
            {
                throw new TestValidationException("DataGridRow is null.");
            }

            int rowIndex = DataGridHelper.GetRowIndex(row);
            if (rowIndex == -1)
            {
                throw new TestValidationException("Could not find row index of DataGridRow.");
            }

            DataGridCellsPresenter cellsPresenter = DataGridHelper.FindVisualChild<DataGridCellsPresenter>(row);
            if (cellsPresenter == null)
            {
                throw new TestValidationException("DataGridCellsPresenter is null.");
            }

            for (int cellColumnIndex = 0; cellColumnIndex < cellsPresenter.Items.Count; cellColumnIndex++)
            {
                CellCoordinate cell = new CellCoordinate(cellColumnIndex, rowIndex);
                cellsCoordinate.Add(cell);
            }

            return cellsCoordinate;
        }

        /// <summary>
        /// Get cell coordinate from a datagrid cell.
        /// </summary>
        /// <param name="dataGridCell">An instance of DataGridCell.</param>
        /// <returns>A cell coordinate of datagrid cell.</returns>
        public static CellCoordinate GetCellCoordinate(DataGridCell dataGridCell)
        {
            int columnIndex = DataGridHelper.GetColumnDisplayIndex(dataGridCell);
            int rowIndex = DataGridHelper.GetRowIndex(dataGridCell);

            return new CellCoordinate(columnIndex, rowIndex);
        }
    }
#endif 
    }
