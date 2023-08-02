using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using Avalon.Test.ComponentModel;
using Avalon.Test.ComponentModel.Utilities;

namespace Microsoft.Test.Controls.Helpers
{
#if TESTBUILD_CLR40
    /// <summary>
    /// General DataGrid helper
    /// </summary>
    public static class DataGridHelper
    {
    #region Private Fields

        // Allowing calculated values to be 0.5 pixels off
        public static readonly double ColumnWidthFuzziness = 0.5;

        #endregion Private Fields

    #region Enums

        public enum ColumnTypes
        {
            None,
            DataGridTextColumn,
            DataGridCheckBoxColumn,
            DataGridComboBoxColumn,
            DataGridHyperlinkColumn,
            DataGridTemplateColumn
        }

        public enum TestType
        {
            Assembly,
            Class,
            Method,
            Event,
            EventArgs,
            DependencyProperty,
            Property
        }

        #endregion Enums

    #region Row Helpers

        /// <summary>
        /// Get the first row index in viewport.
        /// </summary>
        /// <param name="dataGrid">An instance of DataGrid.</param>
        /// <returns>Returns the first row index in view.</returns>
        public static int GetFirstRowIndexInView(DataGrid dataGrid)
        {
            ScrollContentPresenter scrollContentPresenter = GetScrollContentPresenter(dataGrid);

            if (scrollContentPresenter == null)
            {
                throw new TestValidationException("ScrollContentPresenter is null.");
            }

            DataGridCell currentCell = GetCell(dataGrid.CurrentCell);
            DataGridRow currentRow = GetRow(currentCell);

            int inViewRowIndex = dataGrid.Items.IndexOf(currentRow.Item);

            if (inViewRowIndex.Equals(0))
            {
                return 0;
            }
            for (int y = inViewRowIndex; y > 0; y--)
            {
                if (!IsRowInView((DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(y)))
                {
                    inViewRowIndex = y + 1;
                    break;
                }
            }

            return inViewRowIndex;
        }

        /// <summary>
        /// Get the last row index in viewport.
        /// </summary>
        /// <param name="dataGrid">An instance of DataGrid.</param>
        /// <returns>Returns the last row index in view.</returns>
        public static int GetLastRowIndexInView(DataGrid dataGrid)
        {
            ScrollContentPresenter scrollContentPresenter = GetScrollContentPresenter(dataGrid);

            if (scrollContentPresenter == null)
            {
                throw new TestValidationException("ScrollContentPresenter is null.");
            }

            DataGridCell currentCell = GetCell(dataGrid.CurrentCell);
            DataGridRow currentRow = GetRow(currentCell);

            int inViewRowIndex = dataGrid.Items.IndexOf(currentRow.Item);

            if (inViewRowIndex.Equals(dataGrid.Items.Count - 1))
            {
                return dataGrid.Items.Count - 1;
            }

            for (int y = inViewRowIndex; y < dataGrid.Items.Count - 1; y++)
            {
                if (!IsRowInView((DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(y)))
                {
                    inViewRowIndex = y - 1;
                    break;
                }
            }

            return inViewRowIndex;
        }

        /// <summary>
        /// Check to see if datagrid row is rendering within viewport.
        /// </summary>
        /// <param name="dataGridRow">An instance of datagridrow/</param>
        /// <returns>Returns true if datagrid row is rendering within viewport, returns false otherwise.</returns>
        public static bool IsRowInView(DataGridRow dataGridRow)
        {
            DataGrid dataGrid = GetDataGridFromChild(dataGridRow);

            if (dataGrid == null)
            {
                throw new TestValidationException("DataGrid is null.");
            }

            ScrollContentPresenter scrollContentPresenter = GetScrollContentPresenter(dataGrid);

            if (scrollContentPresenter == null)
            {
                throw new TestValidationException("ScrollContentPresenter is null.");
            }

            int viewHeight = Convert.ToInt32(scrollContentPresenter.ActualHeight);

            GeneralTransform rowTransform = dataGridRow.TransformToAncestor(scrollContentPresenter);
            Point ptIn = new Point(0, 0);
            Point rowTopY;
            rowTransform.TryTransform(ptIn, out rowTopY);
            int rowBottomY = Convert.ToInt32(rowTopY.Y + dataGridRow.RenderSize.Height);

            // Row top renders outside of scrollcontentpresenter on Y coordinate.
            if (rowTopY.Y < 0)
            {
                return false;
            }

            // Row bottom renders outside of scrollcontentpresenter on Y coordinate.
            if (rowBottomY > viewHeight)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check if a given datagridrow is in the viewport considering column virtualization
        /// </summary>
        /// <param name="dataGridRow">row to evaluate</param>
        /// <returns>true if in view else false</returns>
        public static bool IsRowInViewVirtual(DataGridRow dataGridRow)
        {
            if (dataGridRow == null) // could be a bad param or a virtualized row, treated as RV
                return false;
            return IsRowInView(dataGridRow);
        }

        /// <summary>
        /// Get datagrid row from a datagrid cell.
        /// </summary>
        /// <param name="dataGridCell">An instance of DataGridCell.</param>
        /// <returns>A datagrid row.</returns>
        public static DataGridRow GetRow(DataGridCell dataGridCell)
        {
            int rowIndex = GetRowIndex(dataGridCell);
            DataGrid dataGrid = GetDataGridFromChild(dataGridCell);

            return GetRow(dataGrid, rowIndex);
        }

        /// <summary>
        /// Get DataGridCell row index from a DataGridCell.
        /// </summary>
        /// <param name="dataGridCell">An instance of DataGridCell.</param>
        /// <returns>Row index.</returns>
        public static int GetRowIndex(DataGridCell dataGridCell)
        {
            // Use reflection to get DataGridCell.RowDataItem property value.
            PropertyInfo rowDataItemProperty = dataGridCell.GetType().GetProperty("RowDataItem", BindingFlags.Instance | BindingFlags.NonPublic);

            DataGrid dataGrid = GetDataGridFromChild(dataGridCell);

            // Use DataGrid.Items.IndexOf(DataGridCell.RowDataItem) to get the cell's row index.
            return dataGrid.Items.IndexOf(rowDataItemProperty.GetValue(dataGridCell, null));
        }

        /// <summary>
        /// Get DataGridCell row index from a DataGridRow.
        /// </summary>
        /// <param name="dataGridRow">An instance of DataGridRow.</param>
        /// <returns>Row index.</returns>
        public static int GetRowIndex(DataGridRow dataGridRow)
        {
            DataGrid dataGrid = GetDataGridFromChild(dataGridRow);

            return dataGrid.Items.IndexOf(dataGridRow.Item);
        }

        /// <summary>
        /// Get DataGridCell row index from a DataGridRowHeader
        /// </summary>
        /// <param name="dataGridRowHeader">An instance of DataGridRowHeader.</param>
        /// <returns>Row index.</returns>
        public static int GetRowIndex(DataGridRowHeader dataGridRowHeader)
        {
            DataGridRow dataGridRow = (DataGridRow)dataGridRowHeader.TemplatedParent;

            return GetRowIndex(dataGridRow);
        }

        /// <summary>
        /// Get DataGridRow from a DataGridRowHeader.
        /// </summary>
        /// <param name="dataGridRowHeader">An instance of DataGridRowHeader.</param>
        /// <returns>DataGridRow</returns>
        public static DataGridRow GetRow(DataGridRowHeader dataGridRowHeader)
        {
            return (DataGridRow)dataGridRowHeader.TemplatedParent;
        }

        /// <summary>
        /// Gets the DataGridRowHeader based on the row index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static DataGridRowHeader GetRowHeader(DataGrid dataGrid, int index)
        {
            return GetRowHeader(GetRow(dataGrid, index));
        }

        /// <summary>
        /// Returns the DataGridRowHeader based on the given row.
        /// </summary>
        /// <param name="row">Uses reflection to access and return RowHeader</param>
        public static DataGridRowHeader GetRowHeader(DataGridRow row)
        {
            if (row != null)
            {
                Type type = typeof(DataGridRow);
                return (DataGridRowHeader)type.InvokeMember("RowHeader",
                            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty,
                            null, row, null);
            }

            return null;
        }

        /// <summary>
        /// Gets the DataGridRow based on the given index
        /// </summary>
        /// <param name="index">the index of the container to get</param>
        public static DataGridRow GetRow(DataGrid dataGrid, int index)
        {
            DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                // may be virtualized, bring into view and try again
                dataGrid.ScrollIntoView(dataGrid.Items[index]);
                QueueHelper.WaitTillQueueItemsProcessed();

                row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(index);
            }

            return row;
        }

        /// <summary>
        /// get the row with virtualization in mind
        /// </summary>
        /// <param name="index">the row index</param>
        /// <returns></returns>
        public static DataGridRow GetRowVirtual(DataGrid dataGrid, int index)
        {
            return (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(index);
        }

        /// <summary>
        /// Get the row related to the data item in from the items collection
        /// </summary>
        /// <param name="dataItem">the data item to get the row for</param>
        /// <returns>the DataGridRow for the dataitem</returns>
        public static DataGridRow GetRow(DataGrid dataGrid, object dataItem)
        {
            return (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromItem(dataItem);
        }

        public static object GetRowDataItemFromCell(DataGridCell cell)
        {
            return (object)(typeof(DataGridCell)).InvokeMember("RowDataItem",
                                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty,
                                    null, cell, null);
        }        

        #endregion Row Helpers

    #region Column Helpers        

        /// <summary>
        /// Get DataGridCell column display index from a DataGridCell.
        /// </summary>
        /// <param name="dataGridCell">An instance of DataGridCell.</param>
        /// <returns>Column index.</returns>
        public static int GetColumnDisplayIndex(DataGridCell dataGridCell)
        {
            return dataGridCell.Column.DisplayIndex;
        }

        public static DataGridColumnHeadersPresenter GetColumnHeadersPresenter(DataGrid dataGrid)
        {
            Type type = typeof(DataGrid);
            return (DataGridColumnHeadersPresenter)type.InvokeMember("ColumnHeadersPresenter",
                        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty,
                        null, dataGrid, null);
        }

        public static DataGridColumnHeadersPresenter GetColumnHeadersPresenter(Visual parent)
        {
            return FindVisualChild<DataGridColumnHeadersPresenter>(parent);
        }

        /// <summary>
        /// Gets the column based on its index
        /// </summary>
        /// <param name="column">the index of the column to get</param>
        public static DataGridColumn GetColumn(DataGrid dataGrid, int column)
        {
            return dataGrid.Columns[column];
        }

        /// <summary>
        /// Gets the column based on the header
        /// </summary>
        /// <param name="header">the header of the column to get</param>
        public static DataGridColumn GetColumn(DataGridColumnHeader header)
        {
            return header.Column;
        }

        /// <summary>
        /// Gets the column header based on the given index
        /// </summary>
        /// <param name="index">the index of the column header</param>
        public static DataGridColumnHeader GetColumnHeader(DataGrid dataGrid, int index)
        {
            DataGridColumnHeadersPresenter presenter = FindVisualChild<DataGridColumnHeadersPresenter>(dataGrid);

            if (presenter != null)
            {
                return (DataGridColumnHeader)presenter.ItemContainerGenerator.ContainerFromIndex(index);
            }

            return null;
        }

        /// <summary>
        /// Find the column header from given DisplayIndex
        /// </summary>
        /// <param name="dataGrid">the datagrid to work against</param>
        /// <param name="displayIndex">the display index to look for</param>
        /// <returns></returns>
        public static DataGridColumnHeader GetColumnHeaderFromDisplayIndex(DataGrid dataGrid, int displayIndex)
        {
            int index = GetDisplayIndexMap(dataGrid)[displayIndex];
            return GetColumnHeader(dataGrid, index);
        }

        /// <summary>
        /// Returns the column index of the first column type it finds in the current row
        /// </summary>
        /// <param name="columnType">the column type to find</param>
        /// <returns>if found, return index, -1 otherwise</returns>
        public static int FindFirstColumnTypeIndex(DataGrid dataGrid, ColumnTypes columnType)
        {
            bool found = false;
            int index = 0; 

            if (dataGrid != null && dataGrid.Columns != null)
            {
                foreach (DataGridColumn column in dataGrid.Columns)
                {
                    if (columnType == ColumnTypes.DataGridTextColumn && column is DataGridTextColumn)
                    {
                        found = true;
                        break;
                    }
                    else if (columnType == ColumnTypes.DataGridCheckBoxColumn && column is DataGridCheckBoxColumn)
                    {
                        found = true;
                        break;
                    }
                    else if (columnType == ColumnTypes.DataGridComboBoxColumn && column is DataGridComboBoxColumn)
                    {
                        found = true;
                        break;
                    }
                    else if (columnType == ColumnTypes.DataGridHyperlinkColumn && column is DataGridHyperlinkColumn)
                    {
                        found = true;
                        break;
                    }
                    else if (columnType == ColumnTypes.DataGridTemplateColumn && column is DataGridTemplateColumn)
                    {
                        found = true;
                        break;
                    }

                    index++;
                }
            }

            if (found)
                return index;
            else
                return -1; 
        }

        /// <summary>
        /// Returns the ColumnType based on index
        /// </summary>
        /// <param name="index">the index of the ColumnType to find</param>
        public static ColumnTypes FindColumnTypeFromIndex(DataGrid dataGrid, int index)
        {
            ColumnTypes columnType = ColumnTypes.None;
            if (dataGrid.Columns[index] is DataGridTextColumn)
            {
                columnType = ColumnTypes.DataGridTextColumn;
            }
            else if (dataGrid.Columns[index] is DataGridTemplateColumn)
            {
                columnType = ColumnTypes.DataGridTemplateColumn;
            }
            else if (dataGrid.Columns[index] is DataGridComboBoxColumn)
            {
                columnType = ColumnTypes.DataGridComboBoxColumn;
            }
            else if (dataGrid.Columns[index] is DataGridCheckBoxColumn)
            {
                columnType = ColumnTypes.DataGridCheckBoxColumn;
            }
            else if (dataGrid.Columns[index] is DataGridHyperlinkColumn)
            {
                columnType = ColumnTypes.DataGridHyperlinkColumn;
            }

            return columnType;
        }

        /// <summary>
        /// Returns the column index of a given column
        /// </summary>
        /// <param name="column">the column to find the index of</param>
        public static int FindColumnIndex(DataGrid dataGrid, DataGridColumn column)
        {
            return dataGrid.Columns.IndexOf(column);
        }

        /// <summary>
        /// Get the fisrt in view column's index 
        /// </summary>
        /// <param name="column">the column to find the index of</param>
        /// <returns>the index</returns>
        public static int GetFirstVisibleColumnIndex(DataGrid dataGrid)
        {
            dataGrid.CurrentCell = new DataGridCellInfo(GetCell(dataGrid, 0, 0));

            DataGridCell currentCell = GetCell(dataGrid.CurrentCell);
            DataGridRow currentRow = GetRow(currentCell);

            int inViewColumnIndex = GetColumnDisplayIndex(currentCell);
            if (inViewColumnIndex.Equals(0))
            {
                return 0;
            }

            for (int x = inViewColumnIndex; x > 0; x--)
            {
                if (!IsCellInView(dataGrid, GetCell(currentRow, x)))
                {
                    inViewColumnIndex = x + 1;
                    break;
                }
            }

            return inViewColumnIndex;
        }

        /// <summary>
        /// Get the last in view column's index
        /// </summary>
        /// <param name="column">the column to find the index of</param>       
        /// <returns></returns>
        public static int GetLastVisibileColumnIndex(DataGrid dataGrid)
        {
            dataGrid.CurrentCell = new DataGridCellInfo(GetCell(dataGrid, 0, 0));

            DataGridCell currentCell = DataGridHelper.GetCell(dataGrid.CurrentCell);
            DataGridRow currentRow = DataGridHelper.GetRow(currentCell);

            int inViewColumnIndex = DataGridHelper.GetColumnDisplayIndex(currentCell);
            if (inViewColumnIndex.Equals(dataGrid.Columns.Count - 1))
            {
                return inViewColumnIndex;
            }

            for (int x = inViewColumnIndex; x < dataGrid.Columns.Count - 1; x++)
            {
                if (!IsCellInView(dataGrid, GetCell(currentRow, x)))
                {
                    inViewColumnIndex = x - 1;
                    break;
                }
            }

            return inViewColumnIndex;
        }
        
        /// <summary>
        /// Find out how many columns in the collection are visible 
        /// </summary>
        /// <returns></returns>
        public static int GetVisibleColumnCount(DataGrid dataGrid)
        {
            int count = dataGrid.Columns.Count;

            foreach (DataGridColumn column in dataGrid.Columns)
            {
                if (column.Visibility != Visibility.Visible)
                {
                    count--;
                }
            }
            return count;
        }

        /// <summary>
        /// In light of the hidden columns
        /// </summary>
        /// <param name="dataGrid"></param>
        /// <returns></returns>
        public static int GetFrozenColumnCount(DataGrid dataGrid)
        {
            int count = dataGrid.Columns.Count;

            foreach (DataGridColumn column in dataGrid.Columns)
            {
                if (column.IsFrozen != true)
                {
                    count--;
                }
            }
            return count;
        }

        /// <summary>
        /// Toggle the visibility of a column given its index 
        /// </summary>
        /// <param name="index"></param>
        public static void ToggleHiddenColumn(DataGrid dataGrid, int displayIndex)
        {
            if (displayIndex < 0 || displayIndex > dataGrid.Columns.Count - 1)
            {
                throw new ArgumentOutOfRangeException();
            }

            DataGridColumn column = dataGrid.ColumnFromDisplayIndex(displayIndex);
            ToggleHiddenColumn(column);
        }

        /// <summary>
        /// Toggle the visibility of a given column
        /// </summary>
        /// <param name="column"></param>
        public static void ToggleHiddenColumn(DataGridColumn column)
        {
            if (column == null)
            {
                throw new ArgumentNullException();
            }

            column.Visibility = column.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }

        /// <summary>
        /// Check if a given column is hidden
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public static bool IsColumnHidden(DataGridColumn column)
        {
            if (column == null)
            {
                throw new ArgumentNullException();
            }

            return column.Visibility == Visibility.Visible ? false : true;
        }

        /// <summary>
        /// Check if a column given by index is hidden
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool IsColumnIndexHidden(DataGrid dataGrid, int index)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            DataGridColumn column = DataGridHelper.GetColumn(dataGrid, index);
            return IsColumnHidden(column);
        }

        /// <summary>
        /// Check if a column given by display index is frozen
        /// </summary>
        /// <param name="displayIndex"></param>
        /// <returns></returns>
        public static bool IsColumnFrozen(DataGrid dataGrid, int displayIndex)
        {
            if (displayIndex < 0 || displayIndex > dataGrid.Columns.Count - 1)
            {
                throw new ArgumentOutOfRangeException();
            }
            DataGridColumn column = dataGrid.ColumnFromDisplayIndex(displayIndex);
            return column.IsFrozen == true;
        }

        /// <summary>
        /// Check if column given by displayindex is a star column
        /// </summary>
        /// <param name="dataGrid"></param>
        /// <param name="displayIndex"></param>
        /// <returns></returns>
        public static bool IsStarColumn(DataGrid dataGrid, int displayIndex)
        {
            EnsureColumnIndexValid(dataGrid, displayIndex);
            return ((dataGrid.ColumnFromDisplayIndex(displayIndex)).Width.IsStar == true);
        }

        /// <summary>
        /// Check the display index to make sure it's valid
        /// </summary>
        /// <param name="dataGrid"></param>
        /// <param name="columnIndex"></param>
        public static void EnsureColumnIndexValid(DataGrid dataGrid, int columnIndex)
        {
            if (columnIndex < 0 || columnIndex > dataGrid.Columns.Count - 1)
            {
                throw new ArgumentOutOfRangeException();
            }
        }
        
        #endregion Column Helpers

    #region Column Width helpers

        /// <summary>
        /// returns a list of all the column header widths
        /// </summary>
        public static List<double> GetColumnHeaderWidths(DataGrid dataGrid)
        {
            List<double> headerWidths = new List<double>();

            for (int i = 0; i < dataGrid.Columns.Count; i++)
            {
                headerWidths.Add(GetColumnHeaderWidth(dataGrid, i));
            }

            return headerWidths;
        }

        /// <summary>
        /// returns a list of all the column widths
        /// </summary>
        public static List<double> GetColumnWidths(DataGrid dataGrid)
        {
            List<double> columnWidths = new List<double>();

            for (int i = 0; i < dataGrid.Columns.Count; i++)
            {
                columnWidths.Add(dataGrid.Columns[i].ActualWidth);
            }

            return columnWidths;
        }

        public static double GetColumnHeaderWidth(DataGrid dataGrid, int colIndex)
        {
            DataGridColumnHeader header = GetColumnHeader(dataGrid, colIndex);
            Assert.AssertTrue(string.Format("header at col: {0} should not be null.", colIndex), header != null);            

            return header.RenderSize.Width;
        }

        public static double GetColumnHeaderDesiredWidth(DataGrid dataGrid, int colIndex)
        {
            DataGridColumnHeader header = GetColumnHeader(dataGrid, colIndex);
            Assert.AssertTrue(string.Format("header at col: {0} should not be null.", colIndex), header != null);

            return header.DesiredSize.Width;
        }

        public static double GetCellWidth(DataGrid dataGrid, int rowIndex, int colIndex)
        {
            DataGridCell cell = GetCell(dataGrid, rowIndex, colIndex);
            Assert.AssertTrue(string.Format("cell at row: {0}, col: {1} should not be null.", rowIndex, colIndex), cell != null);

            return cell.RenderSize.Width;
        }

        public static double GetCellDesiredWidth(DataGrid dataGrid, int rowIndex, int colIndex)
        {
            DataGridCell cell = GetCell(dataGrid, rowIndex, colIndex);            
            Assert.AssertTrue(string.Format("cell at row: {0}, col: {1} should not be null.", rowIndex, colIndex), cell != null);

            return cell.DesiredSize.Width;
        }

        public static double FindLargestCellSize(DataGrid dataGrid, int colIndex, bool isDesiredWidth)
        {
            double largestWidth = 0;

            for (int i = 0; i < dataGrid.Items.Count; i++)
            {
                double cellWidth;
                if (isDesiredWidth)
                    cellWidth = GetCellDesiredWidth(dataGrid, i, colIndex);
                else
                    cellWidth = GetCellWidth(dataGrid, i, colIndex);

                if (cellWidth >= largestWidth)
                {
                    largestWidth = cellWidth;
                }
            }

            return largestWidth;
        }

        public struct ColumnWidthData
        {
            public double desiredHeaderWidth;
            public double headerWidth;

            public double desiredLargestCellWidth;
            public double largestCellWidth;

            public double width;
            public double displayWidth;
            public double desiredWidth;
            public double minWidth;
            public double maxWidth;
            public double actualWidth;

            public DataGridLengthUnitType unitType;
            public bool CanUserResize;
        }

        /// <summary>
        /// convenience helper to get all the widths for the column and header
        /// </summary>
        public static void GetColumnWidthData(
            DataGrid dataGrid,
            string columnName,
            out ColumnWidthData columnWidthData,
            bool onlyDataGridColumnData)
        {
            // find the correct column
            DataGridColumn column = dataGrid.FindName(columnName) as DataGridColumn;
            int columnIndex = DataGridHelper.FindColumnIndex(dataGrid, column);

            GetColumnWidthData(dataGrid, columnIndex, out columnWidthData, onlyDataGridColumnData);
        }

        public static void GetColumnWidthData(
            DataGrid dataGrid,
            int columnIndex,
            out ColumnWidthData columnWidthData,
            bool onlyDataGridColumnData)
        {
            columnWidthData = new ColumnWidthData();

            if (!onlyDataGridColumnData)
            {
                // find the desired and actual header widths
                columnWidthData.desiredHeaderWidth = DataGridHelper.GetColumnHeaderDesiredWidth(dataGrid, columnIndex);
                columnWidthData.headerWidth = DataGridHelper.GetColumnHeaderWidth(dataGrid, columnIndex);

                // find the desired and actual largest cell widths
                columnWidthData.desiredLargestCellWidth = DataGridHelper.FindLargestCellSize(dataGrid, columnIndex, true);
                columnWidthData.largestCellWidth = DataGridHelper.FindLargestCellSize(dataGrid, columnIndex, false);
            }

            columnWidthData.width = dataGrid.Columns[columnIndex].Width.Value;
            columnWidthData.displayWidth = dataGrid.Columns[columnIndex].Width.DisplayValue;
            columnWidthData.desiredWidth = dataGrid.Columns[columnIndex].Width.DesiredValue;
            columnWidthData.minWidth = dataGrid.Columns[columnIndex].MinWidth;
            columnWidthData.maxWidth = dataGrid.Columns[columnIndex].MaxWidth;
            columnWidthData.unitType = dataGrid.Columns[columnIndex].Width.UnitType;
            columnWidthData.CanUserResize = dataGrid.Columns[columnIndex].CanUserResize;
            columnWidthData.actualWidth = dataGrid.Columns[columnIndex].ActualWidth;
        }

        #endregion Column Width helpers
        
    #region Cell and Data Helpers

        /// <summary>
        /// Get DataGridCell from a DataGridCellInfo.
        /// </summary>
        /// <param name="dataGridCellInfo">An instance of DataGridCellInfo.</param>
        /// <returns>DataGridCell if it is not null.</returns>
        public static DataGridCell GetCell(DataGridCellInfo dataGridCellInfo)
        {
            if (!dataGridCellInfo.IsValid)
            {
                return null;
            }

            var cellContent = dataGridCellInfo.Column.GetCellContent(dataGridCellInfo.Item);
            if (cellContent != null)
            {
                return (DataGridCell)cellContent.Parent;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the DataGridCell based on row and column index
        /// </summary>
        /// <param name="row">row index of cell to get</param>
        /// <param name="column">column index of cell to get</param>
        public static DataGridCell GetCell(DataGrid dataGrid, int row, int column)
        {
            DataGridRow rowContainer = GetRow(dataGrid, row);
            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);                

                // try to get the cell but it may possibly be virtualized
                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                if (cell == null)
                {
                    // now try to bring into view and retreive the cell
                    dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[column]);
                    QueueHelper.WaitTillQueueItemsProcessed();

                    cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                }

                return cell;
            }            

            return null;
        }

        /// <summary>
        /// this is a version that takes the virtualization into consideration, so the cell 
        /// can be null and don't have to be in view
        /// </summary>
        /// <param name="row">the row index</param>
        /// <param name="column">the column index</param>
        /// <returns></returns>
        public static DataGridCell GetCellVirtual(DataGrid dataGrid, int row, int column)
        {
            DataGridRow rowContainer = GetRow(dataGrid, row);
            if (rowContainer != null)
            {
                return GetCell(rowContainer, column);
            }

            return null;
        }

        /// <summary>
        /// Get a cell by the row container and column index
        /// </summary>
        /// <param name="rowContainer">row</param>
        /// <param name="column">column index</param>
        /// <returns></returns>
        public static DataGridCell GetCell(DataGridRow rowContainer, int column)
        {
            DataGridCellsPresenter presenter = GetCellsPresenter(rowContainer);
            if (presenter != null)
            {
                return presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
            }

            return null;
        }

        /// <summary>
        /// Check if a given cell is in the viewport
        /// </summary>
        /// <param name="dataGrid">the datagrid in test</param>
        /// <param name="cell">cell to evaluate</param>
        /// <returns>true if in view else false</returns>
        public static bool IsCellInView(DataGrid dataGrid, DataGridCell cell)
        {
            if (cell == null)
            {
                throw new ArgumentNullException("cell");
            }

            ScrollContentPresenter scrollContentPresenter = GetScrollContentPresenter(dataGrid);
            int viewWidth = Convert.ToInt32(scrollContentPresenter.ActualWidth);

            GeneralTransform cellTransform = cell.TransformToAncestor(scrollContentPresenter);
            Point ptIn = new Point(0, 0);
            Point cellLeftX;
            cellTransform.TryTransform(ptIn, out cellLeftX);
            int cellRightX = Convert.ToInt32(cellLeftX.X + cell.RenderSize.Width);

            // cell left renders outside of scrollcontentpresenter on X coordinate.
            if (cellLeftX.X < 0)
            {
                return false;
            }

            // cell right renders outside of scrollcontentpresenter on X coordinate.
            if (cellRightX > viewWidth)
            {
                return false;
            }

            return true;
        }

        public static DataGridCellsPresenter GetCellsPresenter(Visual parent)
        {
            return FindVisualChild<DataGridCellsPresenter>(parent);
        }

        /// <summary>
        /// Gets the DataGridCell based on row and column index
        /// </summary>
        /// <param name="row">row index of cell to get</param>
        /// <param name="column">column index of cell to get</param>
        public static FrameworkElement GetCellEditingElement(DataGrid dataGrid, int row, int column)
        {
            DataGridCell cell = GetCell(dataGrid, row, column);
            Type type = typeof(DataGridCell);
            return (FrameworkElement)type.InvokeMember("EditingElement",
                                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty,
                                    null, cell, null);
        }

        /// <summary>
        /// Gets the data to verify from the template column
        /// </summary>
        /// <param name="currentCell">the cell of the template column</param>
        /// <param name="isEditing">if is currently editing or not</param>
        public delegate string GetDataFromTemplateColumn(DataGridCell currentCell, bool isEditing);
        
        public static string GetDataFromCell(
            DataGrid dataGrid, 
            int row, 
            int col, 
            bool isEditing, 
            GetDataFromTemplateColumn GetDataFromTemplateColumn)
        {
            string cellData = null;
            DataGridCell currentCell = GetCell(dataGrid, row, col);

            if (currentCell.Column is DataGridTextColumn || currentCell.Column is DataGridHyperlinkColumn)
            {
                if (isEditing)
                {
                    TextBox cellBlock = currentCell.Content as TextBox;
                    cellData = cellBlock.Text;
                }
                else
                {
                    TextBlock cellBlock = currentCell.Content as TextBlock;

                    if (currentCell.Column is DataGridHyperlinkColumn)
                    {
                        Inline inline = cellBlock.Inlines.FirstInline;
                        try
                        {
                            cellData = (inline as Hyperlink).NavigateUri.AbsoluteUri;
                        }
                        catch (InvalidOperationException)
                        {
                            cellData = (inline as Hyperlink).NavigateUri.OriginalString;
                        }
                    }
                    else
                    {
                        cellData = cellBlock.Text;
                    }

                }
            }
            else if (currentCell.Column is DataGridComboBoxColumn)
            {
                if (isEditing)
                {
                    ComboBox comboBox = currentCell.Content as ComboBox;
                    cellData = comboBox.SelectedItem.ToString();
                }
                else
                {
                    ComboBox cellBlock = currentCell.Content as ComboBox;
                    cellData = cellBlock.SelectedItem.ToString();
                }

            }
            else if (currentCell.Column is DataGridCheckBoxColumn)
            {
                CheckBox checkBox = currentCell.Content as CheckBox;
                cellData = checkBox.IsChecked.ToString();
            }
            else if (currentCell.Column is DataGridTemplateColumn)
            {
                if (GetDataFromTemplateColumn != null)
                {
                    cellData = GetDataFromTemplateColumn(currentCell, isEditing);
                }                
            }

            return cellData;
        }

        /// <summary>
        /// Gets the DataFieldBinding from a template column
        /// </summary>
        /// <param name="cell">the cell of the template column</param>        
        public delegate string GetDisplayBindingFromTemplateColumn(DataGridCell cell);

        public static object GetDataFromDataSource(
            DataGrid dataGrid, 
            IEnumerable dataSource, 
            Type typeFromDataSource, 
            int row, 
            int col,
            GetDisplayBindingFromTemplateColumn GetDisplayBindingFromTemplateColumn)
        {
            object cellData = null;
            string memberName = null;

            DataGridCell currentCell = GetCell(dataGrid, row, col);
            if ((currentCell.Column is DataGridBoundColumn))
            {
                // get the binding path
                BindingBase bindingBase = (currentCell.Column as DataGridBoundColumn).Binding; 
                memberName = (bindingBase as Binding).Path.Path;
            }
            else if (currentCell.Column is DataGridComboBoxColumn)
            {
                DataGridComboBoxColumn comboBoxColumn = currentCell.Column as DataGridComboBoxColumn;
                if (comboBoxColumn.SelectedItemBinding != null)
                {
                    memberName = (comboBoxColumn.SelectedItemBinding as Binding).Path.Path;
                }
                else if (comboBoxColumn.SelectedValueBinding != null)
                {
                    memberName = (comboBoxColumn.SelectedValueBinding as Binding).Path.Path;
                }
                else if (comboBoxColumn.TextBinding != null)
                {
                    memberName = (comboBoxColumn.TextBinding as Binding).Path.Path;
                }                
            }
            else
            {
                if (GetDisplayBindingFromTemplateColumn != null)
                {
                    memberName = GetDisplayBindingFromTemplateColumn(currentCell);
                }
            }

            if (!string.IsNullOrEmpty(memberName))
            {
                // get the actual data from the data source
                IEnumerator enumerator = dataSource.GetEnumerator();
                bool outOfRangeFlag = false;
                int i = 0;
                do
                {
                    if (!enumerator.MoveNext())
                        outOfRangeFlag = true;
                } while (i++ < row);                

                if (!outOfRangeFlag)
                {
                    object dataItem = enumerator.Current;
                    Type type = typeFromDataSource;
                    cellData = type.InvokeMember(memberName,
                                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty,
                                    null, dataItem, null);
                }
            }

            return cellData;
        }
                
        #endregion Cell and Data Helpers
        
    #region General Helpers

        /// <summary>
        /// Get scroll content presenter from a datagrid.
        /// </summary>
        /// <param name="dataGrid">An instance of datagrid.</param>
        /// <returns>Returns datagrid's ScrollContentPresenter.</returns>
        public static ScrollContentPresenter GetScrollContentPresenter(DataGrid dataGrid)
        {
            ScrollViewer scrollViewer = FindVisualChild<ScrollViewer>(dataGrid);
            if (scrollViewer == null)
            {
                throw new TestValidationException("ScrollViewer is null.");
            }

            return (ScrollContentPresenter)scrollViewer.Template.FindName("PART_ScrollContentPresenter", scrollViewer);
        }

        /// <summary>
        /// Get the DisplayIndex list for the columns
        /// </summary>
        /// <param name="dataGrid">DataGrid in test</param>
        /// <returns>a list of DisplayIndexes</returns>
        public static List<int> GetDisplayIndexMap(DataGrid dataGrid)
        {
            return (List<int>)(typeof(DataGrid)).InvokeMember("DisplayIndexMap",
                                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty,
                                    null, dataGrid, null);
        }

        public static ObservableCollection<DataGridColumn> GetDisplayColumnList(DataGrid dataGrid)
        {
            List<int> displayIndexMap = DataGridHelper.GetDisplayIndexMap(dataGrid);
            ObservableCollection<DataGridColumn> columns = new ObservableCollection<DataGridColumn>();
            foreach (int dIndex in displayIndexMap)
            {
                columns.Add(dataGrid.Columns[dIndex]);
            }

            return columns;
        }

        private static readonly string rightThumbGripperName = "PART_RightHeaderGripper";
        public static Thumb GetColumnHeaderGripper(DataGrid dataGrid, int colIndex)
        {
            DataGridColumnHeader header = DataGridHelper.GetColumnHeader(dataGrid, colIndex);
            return VisualTreeUtils.FindPartByName(header, rightThumbGripperName) as Thumb;
        }

        private static readonly string leftThumbGripperName = "PART_LeftHeaderGripper";
        public static Thumb GetColumnHeaderLeftGripper(DataGrid dataGrid, int colIndex)
        {
            DataGridColumnHeader header = DataGridHelper.GetColumnHeader(dataGrid, colIndex);
            return VisualTreeUtils.FindPartByName(header, leftThumbGripperName) as Thumb;
        }

        private static readonly string rowThumbGripperName = "PART_BottomHeaderGripper";
        public static Thumb GetRowHeaderGripper(DataGrid dataGrid, int rowIndex)
        {
            DataGridRowHeader header = DataGridHelper.GetRowHeader(dataGrid, rowIndex);
            return VisualTreeUtils.FindPartByName(header, rowThumbGripperName) as Thumb;
        }

        public static childItem FindVisualChild<childItem>(DependencyObject obj)
            where childItem : DependencyObject
        {
            for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = System.Windows.Media.VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        /// <summary>
        /// Get the root of the object's visual tree. 
        /// </summary>
        /// <param name="d">DependencyObject.</param>
        /// <returns>the Root of the object's visual tree.</returns> 
        public static DependencyObject FindVisualRoot(DependencyObject d)
        {
            DependencyObject root = d;
            for (; ; )
            {
                FrameworkElement element = root as FrameworkElement;
                if (element == null)
                {
                    break;
                }

                DependencyObject parent = element.Parent as DependencyObject;
                if (parent == null)
                {
                    break;
                }

                root = parent;
            }
            return root;
        }

        public static void WaitForDataGridMouseLeave(DataGrid dataGrid)
        {
            // To keep this thread busy, we'll have to push a frame.
            DispatcherFrame frame = new DispatcherFrame();

            dataGrid.MouseLeave += (sender, e) =>
            {
                frame.Continue = false;
            };

            Dispatcher.PushFrame(frame);
        }

        public static void WaitForDataGridCellMouseUp(DataGridCell cell)
        {
            // To keep this thread busy, we'll have to push a frame.
            DispatcherFrame frame = new DispatcherFrame();

            cell.MouseLeftButtonUp += (sender, e) =>
            {
                frame.Continue = false;
            };

            Dispatcher.PushFrame(frame);
        }

        public static bool AreClose(double n1, double n2, double epsilon)
        {
            return (Math.Abs(n1 - n2) < epsilon);
        }

        public static DataGrid GetDataGridFromChild(DependencyObject dataGridPart)
        {
            if (System.Windows.Media.VisualTreeHelper.GetParent(dataGridPart) == null)
            {
                throw new TestValidationException("Control is null.");
            }
            if (System.Windows.Media.VisualTreeHelper.GetParent(dataGridPart) is DataGrid)
            {
                return (DataGrid)System.Windows.Media.VisualTreeHelper.GetParent(dataGridPart);
            }
            else
            {
                return GetDataGridFromChild(System.Windows.Media.VisualTreeHelper.GetParent(dataGridPart));
            }
        }

        public static void ClearAnySortingDescrptions(DataGrid dataGrid)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(dataGrid.ItemsSource);
            if (view != null)
            {
                view.SortDescriptions.Clear();
                foreach (DataGridColumn column in dataGrid.Columns)
                {
                    column.SortDirection = null;
                }
            }

            QueueHelper.WaitTillQueueItemsProcessed();
        }

        #endregion General Helpers    
   
    #region Theme testing helper

        /// <summary>
        /// For theme based testing 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="themeName"></param>
        /// <param name="colorScheme"></param>
        /// <returns></returns>
        public static ResourceDictionary LoadThemeDictionary(Type t, string themeName, string colorScheme)
        {
            Assembly controlAssembly = t.Assembly;
            AssemblyName themeAssemblyName = controlAssembly.GetName();

            object[] attrs = controlAssembly.GetCustomAttributes(typeof(ThemeInfoAttribute), false);
            if (attrs.Length > 0)
            {
                ThemeInfoAttribute ti = (ThemeInfoAttribute)attrs[0];

                if (ti.ThemeDictionaryLocation == ResourceDictionaryLocation.None)
                {
                    // There are no theme-specific resources.
                    return null;
                }

                if (ti.ThemeDictionaryLocation == ResourceDictionaryLocation.ExternalAssembly)
                {
                    themeAssemblyName.Name += "." + themeName;
                }
            }

            string relativePackUriForResources = "/" +
                themeAssemblyName.FullName +
                ";component/themes/" +
                themeName + "." +
                colorScheme + ".xaml";

            Uri resourceLocater = new System.Uri(relativePackUriForResources, System.UriKind.Relative);
            return Application.LoadComponent(resourceLocater) as ResourceDictionary;
        }

        #endregion
    }
#endif
}
