using System;
using System.Collections.Generic;
using System.Windows.Input;
using Avalon.Test.ComponentModel;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Multiple overloads for MouseSelect method are provided for type safety.
    /// The focused cell is the starting point of selection testing. When the datagrid doesn't
    /// have a focused cell, cell(0,0) will be the focused cell if no selected item, the last
    /// selected item will be the focused cell if there is/are selected item(s).
    /// </summary>
    public class DataGridSelectionValidator : IDisposable
    {
        # region private fields

        private DataGrid dataGrid;
        private DataGridCell focusedCell = null;
        private List<CellCoordinate> expectedSelectedCells = new List<CellCoordinate>();
        private bool shiftDown = false;
        private bool ctrlDown = false;
        private NavigationKey navigationKey = default(NavigationKey);

        #endregion

        #region constructor

        public DataGridSelectionValidator(DataGrid dataGrid)
        {
            this.dataGrid = dataGrid;

            // The focused cell is the starting point of selection testing.
            // Try to get the focused cell from the datagrid.
            if (dataGrid.IsKeyboardFocusWithin)
            {
                focusedCell = System.Windows.Input.Keyboard.FocusedElement as DataGridCell;
            }

            if (focusedCell == null)
            {
                focusedCell = DataGridHelper.GetCell(dataGrid.CurrentCell);

                if (focusedCell == null)
                {
                    // Set focused cell to the top-left most cells when datagrid doesn't have selected item, and no focused cell.
                    focusedCell = DataGridHelper.GetCell(dataGrid, 0, 0);
                    focusedCell.Focus();
                }
            }

            expectedSelectedCells = DataGridSelectionHelper.GetSelectedCellsCoordinate(dataGrid);
        }

        #endregion

        #region public properties

        /// <summary>
        /// Set and get the modifier key state.
        /// Consider: see if we have a better way to let user know how to use the modifier keys.
        /// </summary>
        public ModifierKeys ModifierKeys
        {
            set
            {
                if (value == ModifierKeys.None)
                {
                    if (ctrlDown)
                    {
                        // Key up ctrl key
                        UserInput.KeyUp(System.Windows.Input.Key.LeftCtrl.ToString());
                        QueueHelper.WaitTillQueueItemsProcessed();
                        ctrlDown = false;
                    }

                    if (shiftDown)
                    {
                        // Key up shift key
                        UserInput.KeyUp(System.Windows.Input.Key.LeftShift.ToString());
                        QueueHelper.WaitTillQueueItemsProcessed();
                        shiftDown = false;
                    }
                }
                else
                {
                    if ((value & ModifierKeys.Ctrl) == ModifierKeys.Ctrl)
                    {
                        if (!ctrlDown)
                        {
                            // Key down ctrl key
                            UserInput.KeyDown(System.Windows.Input.Key.LeftCtrl.ToString());
                            QueueHelper.WaitTillQueueItemsProcessed();
                            ctrlDown = true;
                        }
                    }

                    if ((value & ModifierKeys.Shift) == ModifierKeys.Shift)
                    {
                        if (!shiftDown)
                        {
                            // Key down shift key
                            UserInput.KeyDown(System.Windows.Input.Key.LeftShift.ToString());
                            QueueHelper.WaitTillQueueItemsProcessed();
                            shiftDown = true;
                        }
                    }
                }
            }
            get
            {
                ModifierKeys modifierKeys = ModifierKeys.None;
                if (shiftDown)
                    modifierKeys |= ModifierKeys.Shift;
                if (ctrlDown)
                    modifierKeys |= ModifierKeys.Ctrl;

                return modifierKeys;
            }
        }

        /// <summary>
        /// Set and get the focused cell.
        /// </summary>
        public DataGridCell FocusedCell
        {
            set
            {
                focusedCell = value;
                focusedCell.Focus();
            }
            get
            {
                return focusedCell;
            }
        }

        #endregion

        #region private methods

        private bool ValidateSelectedCells()
        {
            List<CellCoordinate> actualSelectedCells = DataGridSelectionHelper.GetSelectedCellsCoordinate(dataGrid);

            if (!actualSelectedCells.Count.Equals(expectedSelectedCells.Count))
            {
                GlobalLog.LogEvidence("The actual selected cells count " + actualSelectedCells.Count.ToString() + " != expected selected cells count " + expectedSelectedCells.Count.ToString() + ".");
                return false;
            }

            List<string> expectedSelectedCellsStringTable = GetSelectedCellsStringTable(expectedSelectedCells);

            foreach (CellCoordinate actualSelectedCell in actualSelectedCells)
            {
                if (!expectedSelectedCellsStringTable.Contains(actualSelectedCell.ToString()))
                {
                    GlobalLog.LogEvidence("The actual selected cell " + actualSelectedCell.ToString() + " is not in expected selected cells collection.");
                    return false;
                }
            }

            return true;
        }

        private List<string> GetSelectedCellsStringTable(List<CellCoordinate> expectedSelectedCells)
        {
            List<string> expectedSelectedCellsStringTable = new List<string>();

            foreach (CellCoordinate expectedSelectedCell in expectedSelectedCells)
            {
                if (expectedSelectedCellsStringTable.Contains(expectedSelectedCell.ToString()))
                {
                    throw new TestValidationException("A duplicate selected cell at " + expectedSelectedCell.ToString() + " in expected selected cells collection.");
                }
                expectedSelectedCellsStringTable.Add(expectedSelectedCell.ToString());
            }
            return expectedSelectedCellsStringTable;
        }

        private void DeselectRow(DataGridRowHeader selectedHeader)
        {
            List<string> expectedSelectedCellsStringTable = GetSelectedCellsStringTable(expectedSelectedCells);

            // Deselect the selected cells in a row.
            foreach (CellCoordinate cellCoordinate in DataGridSelectionHelper.GetCellsCoordinate(selectedHeader))
            {
                if (expectedSelectedCellsStringTable.Contains(cellCoordinate.ToString()))
                {
                    expectedSelectedCells.RemoveAt(expectedSelectedCellsStringTable.IndexOf(cellCoordinate.ToString()));
                    expectedSelectedCellsStringTable = GetSelectedCellsStringTable(expectedSelectedCells);
                }
            }
        }

        private void SelectNewRow(DataGridRowHeader selectHeader)
        {
            List<string> expectedSelectedCellsStringTable = GetSelectedCellsStringTable(expectedSelectedCells);

            // Select a new row.
            // Extends the cells selection to the new row cells if DataGridRowHeader.IsRowSelected is false.
            // Deselect the previous selected cells in new row if DataGridRowHeader.IsRowSelected is true.
            foreach (CellCoordinate cellCoordinate in DataGridSelectionHelper.GetCellsCoordinate(selectHeader))
            {
                if (ModifierKeys == ModifierKeys.Shift || ModifierKeys == (ModifierKeys.Ctrl | ModifierKeys.Shift))
                {
                    if (!expectedSelectedCellsStringTable.Contains(cellCoordinate.ToString()))
                    {
                        expectedSelectedCells.Add(cellCoordinate);
                    }
                }
                else if (ModifierKeys == ModifierKeys.Ctrl)
                {
                    if (selectHeader.IsRowSelected)
                    {
                        if (expectedSelectedCellsStringTable.Contains(cellCoordinate.ToString()))
                        {
                            expectedSelectedCells.RemoveAt(expectedSelectedCellsStringTable.IndexOf(cellCoordinate.ToString()));
                            expectedSelectedCellsStringTable = GetSelectedCellsStringTable(expectedSelectedCells);
                        }
                    }
                    else
                    {
                        if (!expectedSelectedCellsStringTable.Contains(cellCoordinate.ToString()))
                        {
                            expectedSelectedCells.Add(cellCoordinate);
                        }
                    }
                }
            }
        }

        private void MouseSingleClickSelect(DataGridRowHeader selectHeader)
        {
            // Build an expected selected cells table for single left or right click.
            if (ModifierKeys == ModifierKeys.None)
            {
                switch (dataGrid.SelectionUnit)
                {
                    case DataGridSelectionUnit.FullRow:
                    case DataGridSelectionUnit.CellOrRowHeader:
                        expectedSelectedCells.Clear();
                        expectedSelectedCells = DataGridSelectionHelper.GetCellsCoordinate(selectHeader);
                        break;
                    // Nothing happens when Unit is cell.
                }
            }
            else
            {
                switch (dataGrid.SelectionUnit)
                {
                    case DataGridSelectionUnit.FullRow:
                    case DataGridSelectionUnit.CellOrRowHeader:
                        switch (dataGrid.SelectionMode)
                        {
                            case DataGridSelectionMode.Single:
                                // Deselect the previous selected cells.
                                expectedSelectedCells.Clear();

                                // Select a new row if DataGridRowHeader.IsRowSelected is false.
                                if ((ModifierKeys & ModifierKeys.Ctrl) == ModifierKeys.Ctrl)
                                {
                                    if (!selectHeader.IsRowSelected)
                                    {
                                        expectedSelectedCells = DataGridSelectionHelper.GetCellsCoordinate(selectHeader);
                                    }
                                }
                                else
                                {
                                    expectedSelectedCells = DataGridSelectionHelper.GetCellsCoordinate(selectHeader);
                                }
                                break;
                            case DataGridSelectionMode.Extended:
                                if ((ModifierKeys & ModifierKeys.Shift) == ModifierKeys.Shift || ModifierKeys == (ModifierKeys.Ctrl | ModifierKeys.Shift))
                                {
                                    int oldRowIndex = DataGridHelper.GetRowIndex(focusedCell);
                                    int newRowIndex = DataGridHelper.GetRowIndex(selectHeader);
                                    int startIndex = 0;
                                    int endIndex = 0;

                                    if (newRowIndex < oldRowIndex)
                                    {
                                        startIndex = newRowIndex;
                                        endIndex = oldRowIndex;
                                    }
                                    else
                                    {
                                        startIndex = oldRowIndex;
                                        endIndex = newRowIndex;
                                    }

                                    bool areAllRowHeadersSelected = true;

                                    // Check to see if all row headers IsRowSelected is true or not.
                                    for (int rowIndex = startIndex; rowIndex <= endIndex; rowIndex++)
                                    {
                                        DataGridRowHeader selectedHeader = DataGridHelper.GetRowHeader(dataGrid, rowIndex);
                                        if (!selectHeader.IsRowSelected)
                                        {
                                            areAllRowHeadersSelected = false;
                                        }
                                    }

                                    // If all row headers IsRowSelected is true, search for the last index of extended row header IsRowSelected is true.
                                    // Build a collection of selected cells.
                                    if (areAllRowHeadersSelected)
                                    {
                                        while (DataGridHelper.GetRowHeader(dataGrid, endIndex).IsRowSelected)
                                        {
                                            if (newRowIndex < oldRowIndex)
                                            {
                                                endIndex--;
                                                if (endIndex < 0)
                                                    break;
                                            }
                                            else
                                            {
                                                endIndex++;
                                                if (endIndex > dataGrid.Items.Count)
                                                    break;
                                            }
                                        }

                                        // Set the end index to the last index of extended row header IsRowSelected is true.
                                        if (newRowIndex < oldRowIndex)
                                        {
                                            endIndex++;
                                        }
                                        else
                                        {
                                            endIndex--;
                                        }

                                        // Setting start index and end index for the new shift click selection.
                                        if (newRowIndex < endIndex)
                                        {
                                            startIndex = newRowIndex;
                                        }
                                        else
                                        {
                                            startIndex = endIndex;
                                            endIndex = newRowIndex;
                                        }

                                        // Create a new collection of selected cells.
                                        expectedSelectedCells.Clear();
                                        for (int rowIndex = startIndex; rowIndex <= endIndex; rowIndex++)
                                        {
                                            DataGridRowHeader selectedHeader = DataGridHelper.GetRowHeader(dataGrid, rowIndex);
                                            foreach (CellCoordinate cellCoordinate in DataGridSelectionHelper.GetCellsCoordinate(selectedHeader))
                                            {
                                                expectedSelectedCells.Add(cellCoordinate);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (focusedCell.IsSelected)
                                        {
                                            // If DataGridRowHeader.IsRowSelected is true, clean expected selected cells collection.
                                            if (selectHeader.IsRowSelected)
                                            {
                                                expectedSelectedCells.Clear();
                                            }

                                            for (int rowIndex = startIndex; rowIndex <= endIndex; rowIndex++)
                                            {
                                                DataGridRowHeader selectedHeader = DataGridHelper.GetRowHeader(dataGrid, rowIndex);
                                                SelectNewRow(selectedHeader);
                                            }
                                        }
                                        else
                                        {
                                            foreach (CellCoordinate cellCoordinate in DataGridSelectionHelper.GetCellsCoordinate(selectHeader))
                                            {
                                                expectedSelectedCells.Add(cellCoordinate);
                                            }
                                        }
                                    }
                                }
                                else if (ModifierKeys == ModifierKeys.Ctrl)
                                {
                                    SelectNewRow(selectHeader);
                                }
                                break;
                        }
                        break;
                    // Nothing happens when Unit is cell.
                }
            }
        }

        private void MouseSingleClickSelect(DataGridCell selectCell)
        {
            // Build an expected selected cells table for single left or right click.
            switch (ModifierKeys)
            {
                case ModifierKeys.None:
                    switch (dataGrid.SelectionUnit)
                    {
                        case DataGridSelectionUnit.FullRow:
                            SingleClickCellOnFullRowMode(selectCell);
                            break;
                        case DataGridSelectionUnit.Cell:
                        case DataGridSelectionUnit.CellOrRowHeader:
                            SingleClickCellOnCellMode(selectCell);
                            break;
                    }
                    break;
                case ModifierKeys.Ctrl:
                    switch (dataGrid.SelectionUnit)
                    {
                        case DataGridSelectionUnit.FullRow:
                            SingleCtrlClickCellOnFullRowMode(selectCell);
                            break;
                        case DataGridSelectionUnit.Cell:
                        case DataGridSelectionUnit.CellOrRowHeader:
                            SingleCtrlClickCellOnCellMode(selectCell);
                            break;
                    }
                    break;
                case ModifierKeys.Shift:
                case ModifierKeys.Ctrl | ModifierKeys.Shift:
                    switch (dataGrid.SelectionUnit)
                    {
                        case DataGridSelectionUnit.FullRow:
                            SingleShiftClickCellOnFullRowMode(selectCell);
                            break;
                        case DataGridSelectionUnit.Cell:
                        case DataGridSelectionUnit.CellOrRowHeader:
                            SingleShiftClickCellOnCellMode(selectCell);
                            break;
                    }
                    break;
            }
        }

        /// <summary>
        /// SelectionUnit.Cell and SelectionMode.Single: deselect the previous selected and select a new cell.
        /// SelectionUnit.Cell and SelectionMode.Extended: select cells from the focused cell to the new cell.
        /// </summary>
        /// <param name="selectCell">An instance of DataGridCell.</param>
        private void SingleShiftClickCellOnCellMode(DataGridCell selectCell)
        {
            expectedSelectedCells.Clear();
            switch (dataGrid.SelectionMode)
            {
                case DataGridSelectionMode.Single:
                    expectedSelectedCells.Add(DataGridSelectionHelper.GetCellCoordinate(selectCell));
                    break;
                case DataGridSelectionMode.Extended:
                    CellCoordinate focusedCellCoordinate = DataGridSelectionHelper.GetCellCoordinate(focusedCell);
                    CellCoordinate selectCellCoordinate = DataGridSelectionHelper.GetCellCoordinate(selectCell);
                    int startX = default(int);
                    int startY = default(int);
                    int endX = default(int);
                    int endY = default(int);
                    if (focusedCellCoordinate.X < selectCellCoordinate.X)
                    {
                        startX = focusedCellCoordinate.X;
                        endX = selectCellCoordinate.X;
                    }
                    else
                    {
                        startX = selectCellCoordinate.X;
                        endX = focusedCellCoordinate.X;
                    }

                    if (focusedCellCoordinate.Y < selectCellCoordinate.Y)
                    {
                        startY = focusedCellCoordinate.Y;
                        endY = selectCellCoordinate.Y;
                    }
                    else
                    {
                        startY = selectCellCoordinate.Y;
                        endY = focusedCellCoordinate.Y;
                    }

                    if (selectCellCoordinate.X > focusedCellCoordinate.X)
                    {
                        if (DataGridHelper.GetCell(dataGrid, startY, 0).IsSelected)
                        {
                            startX = 0;
                        }
                        else
                        {
                            for (int x = startX; x >= 0; x--)
                            {
                                if (!DataGridHelper.GetCell(dataGrid, startY, x).IsSelected)
                                {
                                    startX = ++x;
                                    break;
                                }
                            }
                        }
                    }

                    if (selectCellCoordinate.Y > focusedCellCoordinate.Y)
                    {
                        if (DataGridHelper.GetCell(dataGrid, 0, startX).IsSelected)
                        {
                            startY = 0;
                        }
                        else
                        {
                            for (int y = startY; y >= 0; y--)
                            {
                                if (!DataGridHelper.GetCell(dataGrid, y, startX).IsSelected)
                                {
                                    startY = ++y;
                                    break;
                                }
                            }
                        }
                    }

                    if (selectCellCoordinate.X < focusedCellCoordinate.X)
                    {
                        if (DataGridHelper.GetCell(dataGrid, endY, dataGrid.Columns.Count - 1).IsSelected)
                        {
                            endX = dataGrid.Columns.Count - 1;
                        }
                        else
                        {
                            for (int x = endX; x <= dataGrid.Columns.Count - 1; x++)
                            {
                                if (!DataGridHelper.GetCell(dataGrid, endY, x).IsSelected)
                                {
                                    endX = --x;
                                    break;
                                }
                            }
                        }
                    }

                    if (selectCellCoordinate.Y < focusedCellCoordinate.Y)
                    {
                        if (DataGridHelper.GetCell(dataGrid, dataGrid.Items.Count - 1, endX).IsSelected)
                        {
                            endY = dataGrid.Items.Count - 1;
                        }
                        else
                        {
                            for (int y = endY; y <= dataGrid.Items.Count - 1; y++)
                            {
                                if (!DataGridHelper.GetCell(dataGrid, endX, y).IsSelected)
                                {
                                    endY = --y;
                                    break;
                                }
                            }
                        }
                    }

                    for (int y = startY; y <= endY; y++)
                    {
                        for (int x = startX; x <= endX; x++)
                        {
                            expectedSelectedCells.Add(new CellCoordinate(x, y));
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// SelectionUnit.FullRow and SelectionMode.Single: deselect the previous selected row, and select a new row.
        /// SelectionUnit.FullRow and SelectionMode.Extended: select a new row, and extends the selection rows from previous selected row to new row.
        /// </summary>
        /// <param name="selectCell">An instance of DataGridCell.</param>
        private void SingleShiftClickCellOnFullRowMode(DataGridCell selectCell)
        {
            DataGridRow dataGridRow = DataGridHelper.GetRow(selectCell);
            DataGridRowHeader dataGridRowHeader = DataGridHelper.GetRowHeader(dataGridRow);
            List<string> expectedSelectedCellsStringTable = GetSelectedCellsStringTable(expectedSelectedCells);

            switch (dataGrid.SelectionMode)
            {
                case DataGridSelectionMode.Single:
                    expectedSelectedCells.Clear();
                    // Select a new row.
                    foreach (CellCoordinate cellCoordinate in DataGridSelectionHelper.GetCellsCoordinate(dataGridRowHeader))
                    {
                        expectedSelectedCells.Add(cellCoordinate);
                    }
                    break;
                case DataGridSelectionMode.Extended:
                    // Select a new row.
                    // Extends the cells selection to the new row cells if DataGridCell.IsSelected is false.
                    // Deselect the previous selected cells in new row if DataGridCell.IsSelected is true.
                    foreach (CellCoordinate cellCoordinate in DataGridSelectionHelper.GetCellsCoordinate(dataGridRowHeader))
                    {
                        if (selectCell.IsSelected)
                        {
                            if (expectedSelectedCellsStringTable.Contains(cellCoordinate.ToString()))
                            {
                                expectedSelectedCells.RemoveAt(expectedSelectedCellsStringTable.IndexOf(cellCoordinate.ToString()));
                                expectedSelectedCellsStringTable = GetSelectedCellsStringTable(expectedSelectedCells);
                            }
                        }
                        else
                        {
                            if (!expectedSelectedCellsStringTable.Contains(cellCoordinate.ToString()))
                            {
                                expectedSelectedCells.Add(cellCoordinate);
                            }
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// SelectionUnit.Cell and SelectionMode.Single: deselect the previous selected cell, and select a new cell if the new cell is not selected.
        ///                                              deselect the selected cell if we click on the selected cell.
        /// SelectionUnit.Cell and SelectionMode.Extended:  select a new cell, and extends the selection to the new cell if the new cell is not selected.
        ///                                                 deselect the selected cell if we click on the selected cell.
        /// </summary>
        /// <param name="selectCell">An instance of DataGridCell.</param>
        private void SingleCtrlClickCellOnCellMode(DataGridCell selectCell)
        {
            List<string> expectedSelectedCellsStringTable = GetSelectedCellsStringTable(expectedSelectedCells);

            switch (dataGrid.SelectionMode)
            {
                case DataGridSelectionMode.Single:
                    expectedSelectedCells.Clear();
                    if (!selectCell.IsSelected)
                    {
                        expectedSelectedCells.Add(DataGridSelectionHelper.GetCellCoordinate(selectCell));
                    }
                    break;
                case DataGridSelectionMode.Extended:
                    CellCoordinate cellCoordinate = DataGridSelectionHelper.GetCellCoordinate(selectCell);
                    if (selectCell.IsSelected)
                    {
                        if (expectedSelectedCellsStringTable.Contains(cellCoordinate.ToString()))
                        {
                            expectedSelectedCells.RemoveAt(expectedSelectedCellsStringTable.IndexOf(cellCoordinate.ToString()));
                            expectedSelectedCellsStringTable = GetSelectedCellsStringTable(expectedSelectedCells);
                        }
                    }
                    else
                    {
                        if (!expectedSelectedCellsStringTable.Contains(cellCoordinate.ToString()))
                        {
                            expectedSelectedCells.Add(cellCoordinate);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// SelectionUnit.FullRow and SelectionMode.Single: deselect the previous selected row, and select a new row.
        /// SelectionUnit.FullRow and SelectionMode.Extended: select a new row, and extends the selection one new row cells.
        /// 	                                              deselect the previous selected row if the new row is selected.
        /// </summary>
        /// <param name="selectCell">An instance of DataGridCell.</param>
        private void SingleCtrlClickCellOnFullRowMode(DataGridCell selectCell)
        {
            DataGridRow dataGridRow = DataGridHelper.GetRow(selectCell);
            DataGridRowHeader dataGridRowHeader = DataGridHelper.GetRowHeader(dataGridRow);
            List<string> expectedSelectedCellsStringTable = GetSelectedCellsStringTable(expectedSelectedCells);

            switch (dataGrid.SelectionMode)
            {
                case DataGridSelectionMode.Single:
                    expectedSelectedCells.Clear();
                    if (!selectCell.IsSelected)
                    {
                        expectedSelectedCells = DataGridSelectionHelper.GetCellsCoordinate(dataGridRowHeader);
                    }
                    break;
                case DataGridSelectionMode.Extended:
                    // Select a new row.
                    // Extends the cells selection to the new row cells if DataGridCell.IsSelected is false.
                    // Deselect the previous selected cells in new row if DataGridCell.IsSelected is true.
                    foreach (CellCoordinate cellCoordinate in DataGridSelectionHelper.GetCellsCoordinate(dataGridRowHeader))
                    {
                        if (selectCell.IsSelected)
                        {
                            if (expectedSelectedCellsStringTable.Contains(cellCoordinate.ToString()))
                            {
                                expectedSelectedCells.RemoveAt(expectedSelectedCellsStringTable.IndexOf(cellCoordinate.ToString()));
                                expectedSelectedCellsStringTable = GetSelectedCellsStringTable(expectedSelectedCells);
                            }
                        }
                        else
                        {
                            if (!expectedSelectedCellsStringTable.Contains(cellCoordinate.ToString()))
                            {
                                expectedSelectedCells.Add(cellCoordinate);
                            }
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// SelectionUnit.Cell and SelectionMode.Single: deselect the previous selected cell, and select a new cell if the new cell is not selected.
        ///                                              deselect the selected cell if we click on the selected cell.
        /// SelectionUnit.Cell and SelectionMode.Extended: deselect the previous selected cell, and select a new cell if the new cell is not selected.
        ///                                                deselect the selected cell if we click on the selected cell.
        /// </summary>
        /// <param name="selectCell">An instance of DataGridCell.</param>
        private void SingleClickCellOnCellMode(DataGridCell selectCell)
        {
            expectedSelectedCells.Clear();
            if (!selectCell.IsSelected)
            {
                CellCoordinate cellCoordinate = DataGridSelectionHelper.GetCellCoordinate(selectCell);
                expectedSelectedCells.Add(cellCoordinate);
            }
        }

        /// <summary>
        /// SelectionUnit.FullRow and SelectionMode.Single: deselect the previous selected row, and select a new row.
        /// SelectionUnit.FullRow and SelectionMode.Extended: deselect the previous selected row, and select a new row.
        /// </summary>
        /// <param name="selectCell">An instance of DataGridCell.</param>
        private void SingleClickCellOnFullRowMode(DataGridCell selectCell)
        {
            DataGridRow dataGridRow = DataGridHelper.GetRow(selectCell);
            DataGridRowHeader dataGridRowHeader = DataGridHelper.GetRowHeader(dataGridRow);
            List<string> expectedSelectedCellsStringTable = GetSelectedCellsStringTable(expectedSelectedCells);

            if (!selectCell.IsSelected)
            {
                expectedSelectedCells.Clear();
                expectedSelectedCells = DataGridSelectionHelper.GetCellsCoordinate(dataGridRowHeader);
            }
            else
            {
                CellCoordinate cellCoordinate = DataGridSelectionHelper.GetCellCoordinate(selectCell);
                expectedSelectedCells.RemoveAt(expectedSelectedCellsStringTable.IndexOf(cellCoordinate.ToString()));
            }
        }

        private void CtrlShiftUpOrDownArrowSelect()
        {
            int focusedCellRowIndex = DataGridHelper.GetRowIndex(focusedCell);

            if (dataGrid.SelectionMode == DataGridSelectionMode.Single)
            {
                expectedSelectedCells.Clear();
            }

            switch (dataGrid.SelectionUnit)
            {
                case DataGridSelectionUnit.FullRow:
                    switch (dataGrid.SelectionMode)
                    {
                        case DataGridSelectionMode.Single:
                            DataGridRowHeader topRowHeader = default(DataGridRowHeader);

                            if (navigationKey == NavigationKey.Up)
                            {
                                topRowHeader = DataGridHelper.GetRowHeader(dataGrid, 0);
                            }
                            else if (navigationKey == NavigationKey.Down)
                            {
                                topRowHeader = DataGridHelper.GetRowHeader(dataGrid, dataGrid.Items.Count - 1);
                            }

                            foreach (CellCoordinate cellCoordinate in DataGridSelectionHelper.GetCellsCoordinate(topRowHeader))
                            {
                                expectedSelectedCells.Add(cellCoordinate);
                            }
                            break;
                        case DataGridSelectionMode.Extended:
                            if (navigationKey == NavigationKey.Up)
                            {
                                for (int y = focusedCellRowIndex - 1; y >= 0; y--)
                                {
                                    DataGridRowHeader rowHeader = DataGridHelper.GetRowHeader(dataGrid, y);
                                    foreach (CellCoordinate cellCoordinate in DataGridSelectionHelper.GetCellsCoordinate(rowHeader))
                                    {
                                        expectedSelectedCells.Add(cellCoordinate);
                                    }
                                }
                            }
                            else if (navigationKey == NavigationKey.Down)
                            {
                                for (int y = focusedCellRowIndex + 1; y <= dataGrid.Items.Count - 1; y++)
                                {
                                    DataGridRowHeader rowHeader = DataGridHelper.GetRowHeader(dataGrid, y);
                                    foreach (CellCoordinate cellCoordinate in DataGridSelectionHelper.GetCellsCoordinate(rowHeader))
                                    {
                                        expectedSelectedCells.Add(cellCoordinate);
                                    }
                                }
                            }
                            break;
                    }
                    break;
                case DataGridSelectionUnit.Cell:
                case DataGridSelectionUnit.CellOrRowHeader:
                    int focusedCellColumnIndex = DataGridHelper.GetColumnDisplayIndex(focusedCell);
                    switch (dataGrid.SelectionMode)
                    {
                        case DataGridSelectionMode.Single:
                            DataGridCell dataGridCell = default(DataGridCell);

                            if (navigationKey == NavigationKey.Up)
                            {
                                dataGridCell = DataGridHelper.GetCell(dataGrid, 0, focusedCellColumnIndex);
                            }
                            else if (navigationKey == NavigationKey.Down)
                            {
                                dataGridCell = DataGridHelper.GetCell(dataGrid, dataGrid.Items.Count - 1, focusedCellColumnIndex);
                            }

                            CellCoordinate topSelectCellCoordinate = DataGridSelectionHelper.GetCellCoordinate(dataGridCell);
                            expectedSelectedCells.Add(topSelectCellCoordinate);
                            break;
                        case DataGridSelectionMode.Extended:
                            DataGridCell cell = default(DataGridCell);

                            if (navigationKey == NavigationKey.Up)
                            {
                                for (int y = focusedCellRowIndex - 1; y >= 0; y--)
                                {
                                    cell = DataGridHelper.GetCell(dataGrid, y, focusedCellColumnIndex);
                                    CellCoordinate selectCellCoordinate = DataGridSelectionHelper.GetCellCoordinate(cell);
                                    expectedSelectedCells.Add(selectCellCoordinate);
                                }
                            }
                            else if (navigationKey == NavigationKey.Down)
                            {
                                for (int y = focusedCellRowIndex + 1; y <= dataGrid.Items.Count - 1; y++)
                                {
                                    cell = DataGridHelper.GetCell(dataGrid, y, focusedCellColumnIndex);
                                    CellCoordinate selectCellCoordinate = DataGridSelectionHelper.GetCellCoordinate(cell);
                                    expectedSelectedCells.Add(selectCellCoordinate);
                                }
                            }
                            break;
                    }
                    break;
            }
        }

        private void ShiftUpOrDownArrowSelect()
        {
            int focusedCellRowIndex = DataGridHelper.GetRowIndex(focusedCell);
            DataGridRowHeader rowHeader = default(DataGridRowHeader);

            if (navigationKey == NavigationKey.Up)
            {
                if (focusedCellRowIndex > 0)
                {
                    rowHeader = DataGridHelper.GetRowHeader(dataGrid, focusedCellRowIndex - 1);
                }
            }
            else if (navigationKey == NavigationKey.Down)
            {
                if (focusedCellRowIndex < dataGrid.Items.Count - 1)
                {
                    rowHeader = DataGridHelper.GetRowHeader(dataGrid, focusedCellRowIndex + 1);
                }
            }

            if (dataGrid.SelectionMode == DataGridSelectionMode.Single)
            {
                expectedSelectedCells.Clear();
            }

            switch (dataGrid.SelectionUnit)
            {
                case DataGridSelectionUnit.FullRow:
                    foreach (CellCoordinate cellCoordinate in DataGridSelectionHelper.GetCellsCoordinate(rowHeader))
                    {
                        expectedSelectedCells.Add(cellCoordinate);
                    }
                    break;
                case DataGridSelectionUnit.Cell:
                case DataGridSelectionUnit.CellOrRowHeader:
                    int focusedCellColumnIndex = DataGridHelper.GetColumnDisplayIndex(focusedCell);
                    DataGridCell cell = default(DataGridCell);

                    if (navigationKey == NavigationKey.Up)
                    {
                        if (focusedCellRowIndex > 0)
                        {
                            cell = DataGridHelper.GetCell(dataGrid, focusedCellRowIndex - 1, focusedCellColumnIndex);
                        }
                    }
                    else if (navigationKey == NavigationKey.Down)
                    {
                        if (focusedCellRowIndex < dataGrid.Items.Count - 1)
                        {
                            cell = DataGridHelper.GetCell(dataGrid, focusedCellRowIndex + 1, focusedCellColumnIndex);
                        }
                    }

                    if (cell != null)
                    {
                        expectedSelectedCells.Add(DataGridSelectionHelper.GetCellCoordinate(cell));
                    }
                    break;
            }
        }

        private void CtrlUpOrDownArrowSelect()
        {
            expectedSelectedCells.Clear();
            switch (dataGrid.SelectionUnit)
            {
                case DataGridSelectionUnit.FullRow:
                    DataGridRowHeader rowHeader = default(DataGridRowHeader);

                    if (navigationKey == NavigationKey.Up)
                    {
                        rowHeader = DataGridHelper.GetRowHeader(dataGrid, 0);
                    }
                    else if (navigationKey == NavigationKey.Down)
                    {
                        rowHeader = DataGridHelper.GetRowHeader(dataGrid, dataGrid.Items.Count - 1);
                    }

                    foreach (CellCoordinate cellCoordinate in DataGridSelectionHelper.GetCellsCoordinate(rowHeader))
                    {
                        expectedSelectedCells.Add(cellCoordinate);
                    }
                    break;
                case DataGridSelectionUnit.Cell:
                case DataGridSelectionUnit.CellOrRowHeader:
                    int focusedCellColumnIndex = DataGridHelper.GetColumnDisplayIndex(focusedCell);
                    DataGridCell cell = default(DataGridCell);

                    if (navigationKey == NavigationKey.Up)
                    {
                        cell = DataGridHelper.GetCell(dataGrid, 0, focusedCellColumnIndex);
                    }
                    else if (navigationKey == NavigationKey.Down)
                    {
                        cell = DataGridHelper.GetCell(dataGrid, dataGrid.Items.Count - 1, focusedCellColumnIndex);
                    }

                    if (cell != null)
                    {
                        expectedSelectedCells.Add(DataGridSelectionHelper.GetCellCoordinate(cell));
                    }
                    break;
            }
        }

        private void UpOrDownArrowSelect()
        {
            int focusedCellRowIndex = DataGridHelper.GetRowIndex(focusedCell);
            int focusedCellColumnIndex = DataGridHelper.GetColumnDisplayIndex(focusedCell);
            switch (dataGrid.SelectionUnit)
            {
                case DataGridSelectionUnit.FullRow:
                    expectedSelectedCells.Clear();
                    DataGridRowHeader rowHeader = default(DataGridRowHeader);

                    if (navigationKey == NavigationKey.Up)
                    {
                        if (focusedCellRowIndex > 0)
                        {
                            rowHeader = DataGridHelper.GetRowHeader(dataGrid, focusedCellRowIndex - 1);
                        }
                    }
                    else if (navigationKey == NavigationKey.Down)
                    {
                        if (focusedCellRowIndex < dataGrid.Items.Count - 1)
                        {
                            rowHeader = DataGridHelper.GetRowHeader(dataGrid, focusedCellRowIndex + 1);
                        }
                    }

                    foreach (CellCoordinate cellCoordinate in DataGridSelectionHelper.GetCellsCoordinate(rowHeader))
                    {
                        expectedSelectedCells.Add(cellCoordinate);
                    }
                    break;
                case DataGridSelectionUnit.Cell:
                case DataGridSelectionUnit.CellOrRowHeader:
                    expectedSelectedCells.Clear();
                    DataGridCell cell = default(DataGridCell);

                    if (navigationKey == NavigationKey.Up)
                    {
                        if (focusedCellRowIndex > 0)
                        {
                            cell = DataGridHelper.GetCell(dataGrid, focusedCellRowIndex - 1, focusedCellColumnIndex);
                        }
                    }
                    else if (navigationKey == NavigationKey.Down)
                    {
                        if (focusedCellRowIndex < dataGrid.Items.Count - 1)
                        {
                            cell = DataGridHelper.GetCell(dataGrid, focusedCellRowIndex + 1, focusedCellColumnIndex);
                        }
                    }

                    if (cell != null)
                    {
                        expectedSelectedCells.Add(DataGridSelectionHelper.GetCellCoordinate(cell));
                    }
                    break;
            }
        }

        private void CtrlShiftLeftOrRightArrowSelect()
        {
            int focusedCellRowIndex = DataGridHelper.GetRowIndex(focusedCell);
            int focusedCellColumnIndex = DataGridHelper.GetColumnDisplayIndex(focusedCell);
            switch (dataGrid.SelectionUnit)
            {
                case DataGridSelectionUnit.Cell:
                case DataGridSelectionUnit.CellOrRowHeader:
                    DataGridCell cell = default(DataGridCell);
                    switch (dataGrid.SelectionMode)
                    {
                        case DataGridSelectionMode.Single:
                            if (navigationKey == NavigationKey.Left)
                            {
                                if (focusedCellColumnIndex > 0)
                                {
                                    expectedSelectedCells.Clear();
                                    cell = DataGridHelper.GetCell(dataGrid, focusedCellRowIndex, 0);
                                }
                            }
                            else if (navigationKey == NavigationKey.Right)
                            {
                                if (focusedCellColumnIndex < dataGrid.Columns.Count - 1)
                                {
                                    expectedSelectedCells.Clear();
                                    cell = DataGridHelper.GetCell(dataGrid, focusedCellRowIndex, dataGrid.Columns.Count - 1);
                                }
                            }

                            if (cell != null)
                            {
                                expectedSelectedCells.Add(DataGridSelectionHelper.GetCellCoordinate(cell));
                            }
                            break;
                        case DataGridSelectionMode.Extended:
                            if (navigationKey == NavigationKey.Left)
                            {
                                if (focusedCellColumnIndex > 0)
                                {
                                    expectedSelectedCells.Clear();
                                    for (int x = focusedCellColumnIndex; x >= 0; x--)
                                    {
                                        cell = DataGridHelper.GetCell(dataGrid, focusedCellRowIndex, x);
                                        if (cell != null)
                                        {
                                            expectedSelectedCells.Add(DataGridSelectionHelper.GetCellCoordinate(cell));
                                        }
                                    }
                                }
                            }
                            else if (navigationKey == NavigationKey.Right)
                            {
                                if (focusedCellColumnIndex < dataGrid.Columns.Count - 1)
                                {
                                    expectedSelectedCells.Clear();
                                    for (int x = focusedCellColumnIndex; x <= dataGrid.Columns.Count - 1; x++)
                                    {
                                        cell = DataGridHelper.GetCell(dataGrid, focusedCellRowIndex - 1, x);
                                        if (cell != null)
                                        {
                                            expectedSelectedCells.Add(DataGridSelectionHelper.GetCellCoordinate(cell));
                                        }
                                    }
                                }
                            }
                            break;
                    }
                    break;
            }
        }

        private void ShiftLeftOrRightArrowSelect()
        {
            int focusedCellRowIndex = DataGridHelper.GetRowIndex(focusedCell);
            int focusedCellColumnIndex = DataGridHelper.GetColumnDisplayIndex(focusedCell);
            switch (dataGrid.SelectionUnit)
            {
                case DataGridSelectionUnit.Cell:
                case DataGridSelectionUnit.CellOrRowHeader:
                    if (dataGrid.SelectionMode == DataGridSelectionMode.Single)
                    {
                        expectedSelectedCells.Clear();
                    }

                    DataGridCell cell = default(DataGridCell);

                    if (navigationKey == NavigationKey.Left)
                    {
                        if (focusedCellColumnIndex > 0)
                        {
                            cell = DataGridHelper.GetCell(dataGrid, focusedCellRowIndex, focusedCellColumnIndex - 1);
                        }
                    }
                    else if (navigationKey == NavigationKey.Right)
                    {
                        if (focusedCellColumnIndex < dataGrid.Columns.Count - 1)
                        {
                            cell = DataGridHelper.GetCell(dataGrid, focusedCellRowIndex, focusedCellColumnIndex + 1);
                        }
                    }

                    if (cell != null)
                    {
                        expectedSelectedCells.Add(DataGridSelectionHelper.GetCellCoordinate(cell));
                    }
                    break;
            }
        }

        private void CtrlLeftOrRightArrowSelect()
        {
            switch (dataGrid.SelectionUnit)
            {
                case DataGridSelectionUnit.Cell:
                case DataGridSelectionUnit.CellOrRowHeader:
                    int focusedCellRowIndex = DataGridHelper.GetRowIndex(focusedCell);
                    int focusedCellColumnIndex = DataGridHelper.GetColumnDisplayIndex(focusedCell);
                    DataGridCell cell = default(DataGridCell);

                    if (navigationKey == NavigationKey.Left)
                    {
                        if (focusedCellColumnIndex > 0)
                        {
                            expectedSelectedCells.Clear();
                            cell = DataGridHelper.GetCell(dataGrid, focusedCellRowIndex, 0);
                        }
                    }
                    else if (navigationKey == NavigationKey.Right)
                    {
                        if (focusedCellColumnIndex < dataGrid.Columns.Count - 1)
                        {
                            expectedSelectedCells.Clear();
                            cell = DataGridHelper.GetCell(dataGrid, focusedCellRowIndex, dataGrid.Columns.Count - 1);
                        }
                    }

                    if (cell != null)
                    {
                        expectedSelectedCells.Add(DataGridSelectionHelper.GetCellCoordinate(cell));
                    }
                    break;
            }
        }

        private void LeftOrRightArrowSelect()
        {
            int focusedCellRowIndex = DataGridHelper.GetRowIndex(focusedCell);
            int focusedCellColumnIndex = DataGridHelper.GetColumnDisplayIndex(focusedCell);
            DataGridCell cell = default(DataGridCell);
            expectedSelectedCells.Clear();
            switch (dataGrid.SelectionUnit)
            {
                case DataGridSelectionUnit.FullRow:
                    for (int x = 0; x <= dataGrid.Columns.Count - 1; x++)
                    {
                        cell = DataGridHelper.GetCell(dataGrid, focusedCellRowIndex, x);
                        if (cell != null)
                        {
                            expectedSelectedCells.Add(DataGridSelectionHelper.GetCellCoordinate(cell));
                        }
                    }
                    break;
                case DataGridSelectionUnit.Cell:
                case DataGridSelectionUnit.CellOrRowHeader:
                    if (navigationKey == NavigationKey.Left)
                    {
                        if (focusedCellColumnIndex > 0)
                        {
                            cell = DataGridHelper.GetCell(dataGrid, focusedCellRowIndex, focusedCellColumnIndex - 1);
                        }
                    }
                    else if (navigationKey == NavigationKey.Right)
                    {
                        if (focusedCellColumnIndex < dataGrid.Columns.Count - 1)
                        {
                            cell = DataGridHelper.GetCell(dataGrid, focusedCellRowIndex, focusedCellColumnIndex + 1);
                        }
                    }

                    if (cell != null)
                    {
                        expectedSelectedCells.Add(DataGridSelectionHelper.GetCellCoordinate(cell));
                    }
                    break;
            }
        }

        private void CtrlShiftHomeOrEndSelect()
        {
            int focusedCellRowIndex = DataGridHelper.GetRowIndex(focusedCell);
            int focusedCellColumnIndex = DataGridHelper.GetColumnDisplayIndex(focusedCell);
            DataGridCell cell = default(DataGridCell);
            DataGridRowHeader rowHeader = default(DataGridRowHeader);

            switch (dataGrid.SelectionUnit)
            {
                case DataGridSelectionUnit.FullRow:
                    if (navigationKey == NavigationKey.Home)
                    {
                        if (focusedCellColumnIndex > 0)
                        {
                            expectedSelectedCells.Clear();
                            switch (dataGrid.SelectionMode)
                            {
                                case DataGridSelectionMode.Single:
                                    rowHeader = DataGridHelper.GetRowHeader(dataGrid, 0);
                                    foreach (CellCoordinate cellCoordinate in DataGridSelectionHelper.GetCellsCoordinate(rowHeader))
                                    {
                                        expectedSelectedCells.Add(cellCoordinate);
                                    }
                                    break;
                                case DataGridSelectionMode.Extended:
                                    for (int y = focusedCellRowIndex; y >= 0; y--)
                                    {
                                        rowHeader = DataGridHelper.GetRowHeader(dataGrid, y);
                                        foreach (CellCoordinate cellCoordinate in DataGridSelectionHelper.GetCellsCoordinate(rowHeader))
                                        {
                                            expectedSelectedCells.Add(cellCoordinate);
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                    else if (navigationKey == NavigationKey.End)
                    {
                        if (focusedCellColumnIndex < dataGrid.Columns.Count - 1)
                        {
                            expectedSelectedCells.Clear();
                            switch (dataGrid.SelectionMode)
                            {
                                case DataGridSelectionMode.Single:
                                    rowHeader = DataGridHelper.GetRowHeader(dataGrid, dataGrid.Items.Count - 1);
                                    foreach (CellCoordinate cellCoordinate in DataGridSelectionHelper.GetCellsCoordinate(rowHeader))
                                    {
                                        expectedSelectedCells.Add(cellCoordinate);
                                    }
                                    break;
                                case DataGridSelectionMode.Extended:
                                    for (int y = focusedCellRowIndex; y < dataGrid.Items.Count; y++)
                                    {
                                        rowHeader = DataGridHelper.GetRowHeader(dataGrid, y);
                                        foreach (CellCoordinate cellCoordinate in DataGridSelectionHelper.GetCellsCoordinate(rowHeader))
                                        {
                                            expectedSelectedCells.Add(cellCoordinate);
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                    break;
                case DataGridSelectionUnit.Cell:
                case DataGridSelectionUnit.CellOrRowHeader:
                    if (navigationKey == NavigationKey.Home)
                    {
                        if (focusedCellColumnIndex > 0 || focusedCellRowIndex > 0)
                        {
                            expectedSelectedCells.Clear();
                            switch (dataGrid.SelectionMode)
                            {
                                case DataGridSelectionMode.Single:
                                    cell = DataGridHelper.GetCell(dataGrid, 0, 0);
                                    expectedSelectedCells.Add(DataGridSelectionHelper.GetCellCoordinate(cell));
                                    break;
                                case DataGridSelectionMode.Extended:
                                    for (int x = focusedCellColumnIndex; x >= 0; x--)
                                    {
                                        for (int y = focusedCellRowIndex; y >= 0; y--)
                                        {
                                            cell = DataGridHelper.GetCell(dataGrid, y, x);
                                            expectedSelectedCells.Add(DataGridSelectionHelper.GetCellCoordinate(cell));
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                    else if (navigationKey == NavigationKey.End)
                    {
                        if (focusedCellColumnIndex < dataGrid.Columns.Count - 1 || focusedCellRowIndex < dataGrid.Items.Count - 1)
                        {
                            expectedSelectedCells.Clear();
                            switch (dataGrid.SelectionMode)
                            {
                                case DataGridSelectionMode.Single:
                                    cell = DataGridHelper.GetCell(dataGrid, dataGrid.Items.Count - 1, dataGrid.Columns.Count - 1);
                                    expectedSelectedCells.Add(DataGridSelectionHelper.GetCellCoordinate(cell));
                                    break;
                                case DataGridSelectionMode.Extended:
                                    for (int x = focusedCellColumnIndex; x < dataGrid.Columns.Count; x++)
                                    {
                                        for (int y = focusedCellRowIndex; y < dataGrid.Items.Count; y++)
                                        {
                                            cell = DataGridHelper.GetCell(dataGrid, y, x);
                                            expectedSelectedCells.Add(DataGridSelectionHelper.GetCellCoordinate(cell));
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                    break;
            }
        }

        private void ShiftHomeOrEndSelect()
        {
            int focusedCellRowIndex = DataGridHelper.GetRowIndex(focusedCell);
            int focusedCellColumnIndex = DataGridHelper.GetColumnDisplayIndex(focusedCell);
            DataGridCell cell = default(DataGridCell);
            switch (dataGrid.SelectionUnit)
            {
                case DataGridSelectionUnit.Cell:
                case DataGridSelectionUnit.CellOrRowHeader:
                    if (navigationKey == NavigationKey.Home)
                    {
                        if (focusedCellColumnIndex > 0)
                        {
                            switch (dataGrid.SelectionMode)
                            {
                                case DataGridSelectionMode.Single:
                                    expectedSelectedCells.Clear();
                                    cell = DataGridHelper.GetCell(dataGrid, focusedCellRowIndex, 0);
                                    break;
                                case DataGridSelectionMode.Extended:
                                    for (int x = focusedCellColumnIndex - 1; x >= 0; x--)
                                    {
                                        cell = DataGridHelper.GetCell(dataGrid, focusedCellRowIndex, x);
                                    }
                                    break;
                            }
                        }
                    }
                    else if (navigationKey == NavigationKey.End)
                    {
                        if (focusedCellColumnIndex < dataGrid.Columns.Count - 1)
                        {
                            switch (dataGrid.SelectionMode)
                            {
                                case DataGridSelectionMode.Single:
                                    expectedSelectedCells.Clear();
                                    cell = DataGridHelper.GetCell(dataGrid, focusedCellRowIndex, dataGrid.Columns.Count - 1);
                                    break;
                                case DataGridSelectionMode.Extended:
                                    for (int x = focusedCellColumnIndex + 1; x < dataGrid.Columns.Count; x++)
                                    {
                                        cell = DataGridHelper.GetCell(dataGrid, focusedCellRowIndex, x);
                                    }
                                    break;
                            }
                        }
                    }

                    if (cell != null)
                    {
                        expectedSelectedCells.Add(DataGridSelectionHelper.GetCellCoordinate(cell));
                    }
                    break;
            }
        }

        private void CtrlHomeOrEndSelect()
        {
            int focusedCellRowIndex = DataGridHelper.GetRowIndex(focusedCell);
            int focusedCellColumnIndex = DataGridHelper.GetColumnDisplayIndex(focusedCell);
            DataGridCell cell = default(DataGridCell);
            DataGridRowHeader rowHeader = default(DataGridRowHeader);

            switch (dataGrid.SelectionUnit)
            {
                case DataGridSelectionUnit.FullRow:
                    if (navigationKey == NavigationKey.Home)
                    {
                        expectedSelectedCells.Clear();
                        rowHeader = DataGridHelper.GetRowHeader(dataGrid, 0);
                        foreach (CellCoordinate cellCoordinate in DataGridSelectionHelper.GetCellsCoordinate(rowHeader))
                        {
                            expectedSelectedCells.Add(cellCoordinate);
                        }
                    }
                    else if (navigationKey == NavigationKey.End)
                    {
                        expectedSelectedCells.Clear();
                        rowHeader = DataGridHelper.GetRowHeader(dataGrid, dataGrid.Items.Count - 1);
                        foreach (CellCoordinate cellCoordinate in DataGridSelectionHelper.GetCellsCoordinate(rowHeader))
                        {
                            expectedSelectedCells.Add(cellCoordinate);
                        }
                    }
                    break;
                case DataGridSelectionUnit.Cell:
                case DataGridSelectionUnit.CellOrRowHeader:
                    if (navigationKey == NavigationKey.Home)
                    {
                        if (focusedCellColumnIndex > 0 || focusedCellRowIndex > 0)
                        {
                            expectedSelectedCells.Clear();
                            cell = DataGridHelper.GetCell(dataGrid, 0, 0);
                        }
                    }
                    else if (navigationKey == NavigationKey.End)
                    {
                        if (focusedCellColumnIndex < dataGrid.Columns.Count - 1 || focusedCellRowIndex < dataGrid.Items.Count - 1)
                        {
                            expectedSelectedCells.Clear();
                            cell = DataGridHelper.GetCell(dataGrid, dataGrid.Items.Count - 1, dataGrid.Columns.Count - 1);
                        }
                    }

                    if (cell != null)
                    {
                        expectedSelectedCells.Add(DataGridSelectionHelper.GetCellCoordinate(cell));
                    }
                    break;
            }
        }

        private void HomeOrEndSelect()
        {
            int focusedCellRowIndex = DataGridHelper.GetRowIndex(focusedCell);
            int focusedCellColumnIndex = DataGridHelper.GetColumnDisplayIndex(focusedCell);
            DataGridCell cell = default(DataGridCell);
            switch (dataGrid.SelectionUnit)
            {
                case DataGridSelectionUnit.FullRow:
                    expectedSelectedCells.Clear();
                    DataGridRowHeader rowHeader = DataGridHelper.GetRowHeader(dataGrid, focusedCellRowIndex);
                    foreach (CellCoordinate cellCoordinate in DataGridSelectionHelper.GetCellsCoordinate(rowHeader))
                    {
                        expectedSelectedCells.Add(cellCoordinate);
                    }
                    break;
                case DataGridSelectionUnit.Cell:
                case DataGridSelectionUnit.CellOrRowHeader:
                    if (navigationKey == NavigationKey.Home)
                    {
                        if (focusedCellColumnIndex > 0)
                        {
                            expectedSelectedCells.Clear();
                            cell = DataGridHelper.GetCell(dataGrid, focusedCellRowIndex, 0);
                        }
                    }
                    else if (navigationKey == NavigationKey.End)
                    {
                        if (focusedCellColumnIndex < dataGrid.Columns.Count - 1)
                        {
                            expectedSelectedCells.Clear();
                            cell = DataGridHelper.GetCell(dataGrid, focusedCellRowIndex, dataGrid.Columns.Count - 1);
                        }
                    }

                    if (cell != null)
                    {
                        expectedSelectedCells.Add(DataGridSelectionHelper.GetCellCoordinate(cell));
                    }
                    break;
            }
        }

        private void ShiftPageDownOrPageUpSelect()
        {
            double rowHeight = 0;
            // Assume all the rows have same height.
            double actualRowHeight = DataGridHelper.GetRowHeader(dataGrid, 0).ActualHeight;
            int focusedCellRowIndex = DataGridHelper.GetRowIndex(focusedCell);
            int focusedCellColumnIndex = DataGridHelper.GetColumnDisplayIndex(focusedCell);
            int expectedSelectRowIndexInView = 0;
            ScrollContentPresenter scrollContentPresenter = DataGridHelper.GetScrollContentPresenter(dataGrid);
            expectedSelectedCells.Clear();

            switch (dataGrid.SelectionUnit)
            {
                case DataGridSelectionUnit.FullRow:
                    if (navigationKey == NavigationKey.PageDown)
                    {
                        for (int y = focusedCellRowIndex; y < dataGrid.Items.Count; y++)
                        {
                            rowHeight += actualRowHeight;

                            if (rowHeight > scrollContentPresenter.ActualHeight)
                            {
                                rowHeight -= actualRowHeight;
                                expectedSelectRowIndexInView = y - 1;
                                break;
                            }
                        }
                        switch (dataGrid.SelectionMode)
                        {
                            case DataGridSelectionMode.Single:
                                for (int x = 0; x < dataGrid.Columns.Count; x++)
                                {
                                    expectedSelectedCells.Add(new CellCoordinate(x, expectedSelectRowIndexInView));
                                }
                                break;
                            case DataGridSelectionMode.Extended:
                                for (int y = focusedCellRowIndex; y <= expectedSelectRowIndexInView; y++)
                                {
                                    for (int x = 0; x < dataGrid.Columns.Count; x++)
                                    {
                                        expectedSelectedCells.Add(new CellCoordinate(x, y));
                                    }
                                }
                                break;
                        }
                    }
                    else if (navigationKey == NavigationKey.PageUp)
                    {
                        for (int y = focusedCellRowIndex; y >= 0; y--)
                        {
                            rowHeight += actualRowHeight;

                            if (rowHeight > scrollContentPresenter.ActualHeight)
                            {
                                rowHeight -= actualRowHeight;
                                expectedSelectRowIndexInView = y + 1;
                                break;
                            }
                        }
                        switch (dataGrid.SelectionMode)
                        {
                            case DataGridSelectionMode.Single:
                                for (int x = 0; x < dataGrid.Columns.Count; x++)
                                {
                                    expectedSelectedCells.Add(new CellCoordinate(x, expectedSelectRowIndexInView));
                                }
                                break;
                            case DataGridSelectionMode.Extended:
                                for (int y = focusedCellRowIndex; y >= expectedSelectRowIndexInView; y--)
                                {
                                    for (int x = 0; x < dataGrid.Columns.Count; x++)
                                    {
                                        expectedSelectedCells.Add(new CellCoordinate(x, y));
                                    }
                                }
                                break;
                        }
                    }
                    break;
                case DataGridSelectionUnit.Cell:
                case DataGridSelectionUnit.CellOrRowHeader:
                    if (navigationKey == NavigationKey.PageDown)
                    {
                        for (int y = focusedCellRowIndex; y < dataGrid.Items.Count; y++)
                        {
                            rowHeight += actualRowHeight;

                            if (rowHeight > scrollContentPresenter.ActualHeight)
                            {
                                rowHeight -= actualRowHeight;
                                expectedSelectRowIndexInView = y - 1;
                                break;
                            }
                        }
                        switch (dataGrid.SelectionMode)
                        {
                            case DataGridSelectionMode.Single:
                                expectedSelectedCells.Add(new CellCoordinate(focusedCellColumnIndex, expectedSelectRowIndexInView));
                                break;
                            case DataGridSelectionMode.Extended:
                                for (int y = focusedCellRowIndex; y <= expectedSelectRowIndexInView; y++)
                                {
                                    expectedSelectedCells.Add(new CellCoordinate(focusedCellColumnIndex, y));
                                }
                                break;
                        }
                    }
                    else if (navigationKey == NavigationKey.PageUp)
                    {
                        for (int y = focusedCellRowIndex; y >= 0; y--)
                        {
                            rowHeight += actualRowHeight;

                            if (rowHeight > scrollContentPresenter.ActualHeight)
                            {
                                rowHeight -= actualRowHeight;
                                expectedSelectRowIndexInView = y + 1;
                                break;
                            }
                        }
                        switch (dataGrid.SelectionMode)
                        {
                            case DataGridSelectionMode.Single:
                                expectedSelectedCells.Add(new CellCoordinate(focusedCellColumnIndex, expectedSelectRowIndexInView));
                                break;
                            case DataGridSelectionMode.Extended:
                                for (int y = focusedCellRowIndex; y >= expectedSelectRowIndexInView; y--)
                                {
                                    expectedSelectedCells.Add(new CellCoordinate(focusedCellColumnIndex, y));
                                }
                                break;
                        }
                    }
                    break;
            }

            UserInput.KeyPress(navigationKey.ToString());
            QueueHelper.WaitTillQueueItemsProcessed();

            switch (navigationKey)
            {
                case NavigationKey.PageDown:
                    if (!focusedCellRowIndex.Equals(DataGridHelper.GetFirstRowIndexInView(dataGrid)))
                    {
                        throw new TestValidationException("expectedFirstRowIndexInView != actualFirstRowIndexInView");
                    }

                    if (!expectedSelectRowIndexInView.Equals(DataGridHelper.GetLastRowIndexInView(dataGrid)))
                    {
                        throw new TestValidationException("expectedLastRowIndexInView != actualLastRowIndexInView");
                    }
                    break;
                case NavigationKey.PageUp:
                    if (!expectedSelectRowIndexInView.Equals(DataGridHelper.GetFirstRowIndexInView(dataGrid)))
                    {
                        throw new TestValidationException("expectedFirstRowIndexInView != actualFirstRowIndexInView");
                    }

                    if (!focusedCellRowIndex.Equals(DataGridHelper.GetLastRowIndexInView(dataGrid)))
                    {
                        throw new TestValidationException("expectedLastRowIndexInView != actualLastRowIndexInView");
                    }
                    break;
            }
        }

        private void PageDownOrPageUpSelect()
        {
            double rowHeight = 0;
            // Assume all the rows have same height.
            double actualRowHeight = DataGridHelper.GetRowHeader(dataGrid, 0).ActualHeight;
            int focusedCellRowIndex = DataGridHelper.GetRowIndex(focusedCell);
            int focusedCellColumnIndex = DataGridHelper.GetColumnDisplayIndex(focusedCell);
            int expectedSelectRowIndexInView = 0;
            ScrollContentPresenter scrollContentPresenter = DataGridHelper.GetScrollContentPresenter(dataGrid);
            expectedSelectedCells.Clear();

            switch (dataGrid.SelectionUnit)
            {
                case DataGridSelectionUnit.FullRow:
                    if (navigationKey == NavigationKey.PageDown)
                    {
                        for (int y = focusedCellRowIndex; y < dataGrid.Items.Count; y++)
                        {
                            rowHeight += actualRowHeight;

                            if (rowHeight > scrollContentPresenter.ActualHeight)
                            {
                                rowHeight -= actualRowHeight;
                                expectedSelectRowIndexInView = y - 1;
                                break;
                            }
                        }
                    }
                    else if (navigationKey == NavigationKey.PageUp)
                    {
                        for (int y = focusedCellRowIndex; y >= 0; y--)
                        {
                            rowHeight += actualRowHeight;

                            if (rowHeight > scrollContentPresenter.ActualHeight)
                            {
                                rowHeight -= actualRowHeight;
                                expectedSelectRowIndexInView = y + 1;
                                break;
                            }
                        }
                    }

                    for (int x = 0; x < dataGrid.Columns.Count; x++)
                    {
                        expectedSelectedCells.Add(new CellCoordinate(x, expectedSelectRowIndexInView));
                    }
                    break;
                case DataGridSelectionUnit.Cell:
                case DataGridSelectionUnit.CellOrRowHeader:
                    if (navigationKey == NavigationKey.PageDown)
                    {
                        for (int y = focusedCellRowIndex; y < dataGrid.Items.Count; y++)
                        {
                            rowHeight += actualRowHeight;

                            if (rowHeight > scrollContentPresenter.ActualHeight)
                            {
                                rowHeight -= actualRowHeight;
                                expectedSelectRowIndexInView = y - 1;
                                break;
                            }
                        }
                    }
                    else if (navigationKey == NavigationKey.PageUp)
                    {
                        for (int y = focusedCellRowIndex; y >= 0; y--)
                        {
                            rowHeight += actualRowHeight;

                            if (rowHeight > scrollContentPresenter.ActualHeight)
                            {
                                rowHeight -= actualRowHeight;
                                expectedSelectRowIndexInView = y + 1;
                                break;
                            }
                        }
                    }

                    expectedSelectedCells.Add(new CellCoordinate(focusedCellColumnIndex, expectedSelectRowIndexInView));
                    break;
            }

            UserInput.KeyPress(navigationKey.ToString());
            QueueHelper.WaitTillQueueItemsProcessed();

            switch (navigationKey)
            {
                case NavigationKey.PageDown:
                    if (!focusedCellRowIndex.Equals(DataGridHelper.GetFirstRowIndexInView(dataGrid)))
                    {
                        throw new TestValidationException("expectedFirstRowIndexInView != actualFirstRowIndexInView");
                    }

                    if (!expectedSelectRowIndexInView.Equals(DataGridHelper.GetLastRowIndexInView(dataGrid)))
                    {
                        throw new TestValidationException("expectedLastRowIndexInView != actualLastRowIndexInView");
                    }
                    break;
                case NavigationKey.PageUp:
                    if (!expectedSelectRowIndexInView.Equals(DataGridHelper.GetFirstRowIndexInView(dataGrid)))
                    {
                        throw new TestValidationException("expectedFirstRowIndexInView != actualFirstRowIndexInView");
                    }

                    if (!focusedCellRowIndex.Equals(DataGridHelper.GetLastRowIndexInView(dataGrid)))
                    {
                        throw new TestValidationException("expectedLastRowIndexInView != actualLastRowIndexInView");
                    }
                    break;
            }
        }

        private void CtrlTabOrCtrlShiftTabSelect()
        {
            int focusedCellRowIndex = DataGridHelper.GetRowIndex(focusedCell);
            int focusedCellColumnIndex = DataGridHelper.GetColumnDisplayIndex(focusedCell);
            DataGridCell cell = default(DataGridCell);
            DataGridRowHeader rowHeader = default(DataGridRowHeader);
            expectedSelectedCells.Clear();

            switch (dataGrid.SelectionUnit)
            {
                case DataGridSelectionUnit.FullRow:
                    rowHeader = DataGridHelper.GetRowHeader(dataGrid, focusedCellRowIndex);

                    foreach (CellCoordinate cellCoordinate in DataGridSelectionHelper.GetCellsCoordinate(rowHeader))
                    {
                        expectedSelectedCells.Add(cellCoordinate);
                    }
                    break;
                case DataGridSelectionUnit.Cell:
                case DataGridSelectionUnit.CellOrRowHeader:
                    cell = DataGridHelper.GetCell(dataGrid, focusedCellRowIndex, focusedCellColumnIndex);

                    if (cell != null)
                    {
                        expectedSelectedCells.Add(DataGridSelectionHelper.GetCellCoordinate(cell));
                    }
                    break;
            }
        }

        private void TabOrShiftTabSelect()
        {
            int focusedCellRowIndex = DataGridHelper.GetRowIndex(focusedCell);
            int focusedCellColumnIndex = DataGridHelper.GetColumnDisplayIndex(focusedCell);
            DataGridCell cell = default(DataGridCell);
            DataGridRowHeader rowHeader = default(DataGridRowHeader);
            expectedSelectedCells.Clear();

            switch (dataGrid.SelectionUnit)
            {
                case DataGridSelectionUnit.FullRow:
                    if (this.ModifierKeys == ModifierKeys.None)
                    {
                        if (focusedCellColumnIndex.Equals(dataGrid.Columns.Count - 1))
                        {
                            if (!focusedCellRowIndex.Equals(dataGrid.Items.Count - 1))
                            {
                                rowHeader = DataGridHelper.GetRowHeader(dataGrid, focusedCellRowIndex + 1);
                            }
                        }
                    }
                    else if (this.ModifierKeys == ModifierKeys.Shift)
                    {
                        if (focusedCellColumnIndex.Equals(0))
                        {
                            if (!focusedCellRowIndex.Equals(0))
                            {
                                rowHeader = DataGridHelper.GetRowHeader(dataGrid, focusedCellRowIndex - 1);
                            }
                        }
                    }

                    if (rowHeader == null)
                    {
                        rowHeader = DataGridHelper.GetRowHeader(dataGrid, focusedCellRowIndex);
                    }

                    foreach (CellCoordinate cellCoordinate in DataGridSelectionHelper.GetCellsCoordinate(rowHeader))
                    {
                        expectedSelectedCells.Add(cellCoordinate);
                    }
                    break;
                case DataGridSelectionUnit.Cell:
                case DataGridSelectionUnit.CellOrRowHeader:
                    if (this.ModifierKeys == ModifierKeys.None)
                    {
                        if (focusedCellColumnIndex.Equals(dataGrid.Columns.Count - 1))
                        {
                            cell = DataGridHelper.GetCell(dataGrid, focusedCellRowIndex + 1, 0);
                        }
                        else
                        {
                            cell = DataGridHelper.GetCell(dataGrid, focusedCellRowIndex, focusedCellColumnIndex + 1);
                        }
                    }
                    else if (this.ModifierKeys == ModifierKeys.Shift)
                    {
                        if (focusedCellColumnIndex.Equals(0))
                        {
                            cell = DataGridHelper.GetCell(dataGrid, focusedCellRowIndex - 1, dataGrid.Columns.Count - 1);
                        }
                        else
                        {
                            cell = DataGridHelper.GetCell(dataGrid, focusedCellRowIndex, focusedCellColumnIndex - 1);
                        }
                    }

                    if (cell != null)
                    {
                        // Tab one more time to move focus rect to next cell if it is a hyperlink column.
                        if (cell.Column is DataGridHyperlinkColumn)
                        {
                            UserInput.KeyPress(System.Windows.Input.Key.Tab.ToString());
                            QueueHelper.WaitTillQueueItemsProcessed();
                        }

                        expectedSelectedCells.Add(DataGridSelectionHelper.GetCellCoordinate(cell));
                    }
                    break;
            }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Test mouse click on row-header to select datagrid cells.
        /// </summary>
        /// <param name="selectHeader">DataGridRowHeader</param>
        /// <param name="mouseInput">MouseInput</param>
        /// <returns>Returns true if test pass, returns false otherwise.</returns>
        public bool MouseSelect(DataGridRowHeader selectHeader, MouseInput mouseInput)
        {
            switch (mouseInput)
            {
                // BVTs
                case MouseInput.LeftClick:
                    MouseSingleClickSelect(selectHeader);
                    UserInput.MouseLeftClickCenter(selectHeader);
                    break;
                // Pri1s
                case MouseInput.DoubleClick:
                    throw new NotImplementedException();
                // Pri1s
                case MouseInput.LeftClickDrag:
                    throw new NotImplementedException();
                // BVTs
                case MouseInput.RightClick:
                    // right-click changes selection only if row is not selected
                    if (!selectHeader.IsRowSelected)
                    {
                        MouseSingleClickSelect(selectHeader);
                    }
                    UserInput.MouseRightClickCenter(selectHeader);
                    break;
            }
            QueueHelper.WaitTillQueueItemsProcessed();

            focusedCell = DataGridHelper.GetCell(dataGrid.CurrentCell);

            // Validation
            return ValidateSelectedCells();
        }

        /// <summary>
        /// Test mouse click on cell to select datagrid cells.
        /// </summary>
        /// <param name="selectCell">DataGridCell</param>
        /// <param name="mouseInput">MouseInput</param>
        /// <returns>Returns true if test pass, returns false otherwise.</returns>
        public bool MouseSelect(DataGridCell selectCell, MouseInput mouseInput)
        {
            switch (mouseInput)
            {
                // BVTs
                case MouseInput.LeftClick:
                    MouseSingleClickSelect(selectCell);

                    // Click on the top-left most of the cell, so we can select hyperlink cell.
                    UserInput.MouseLeftDown(selectCell, 2, 2);
                    UserInput.MouseLeftUp(selectCell, 2, 2);
                    break;
                // Pri1s
                case MouseInput.DoubleClick:
                    throw new NotImplementedException();
                // Pri1s
                case MouseInput.LeftClickDrag:
                    throw new NotImplementedException();
                // BVTs
                case MouseInput.RightClick:
                    MouseSingleClickSelect(selectCell);

                    // Click on the top-left most of the cell, so we can select hyperlink cell.
                    UserInput.MouseButton(selectCell, 2, 2, "RightDown");
                    UserInput.MouseButton(selectCell, 2, 2, "RightUp");
                    break;
            }
            QueueHelper.WaitTillQueueItemsProcessed();

            // Capture the focused cell that is the starting point of selection.
            focusedCell = DataGridHelper.GetCell(dataGrid.CurrentCell);

            // Validation
            return ValidateSelectedCells();
        }

        /// <summary>
        /// Test press navigation key to select datagrid cells.
        /// </summary>
        /// <param name="navigationKey">NavigationKey</param>
        /// <returns>Returns true if test pass, returns false otherwise.</returns>
        public bool KeyboardNavigationSelect(NavigationKey navigationKey)
        {
            this.navigationKey = navigationKey;
            switch (navigationKey)
            {
                case NavigationKey.Up:
                case NavigationKey.Down:

                    switch (ModifierKeys)
                    {
                        case ModifierKeys.None:
                            UpOrDownArrowSelect();
                            break;
                        case ModifierKeys.Ctrl:
                            CtrlUpOrDownArrowSelect();
                            break;
                        case ModifierKeys.Shift:
                            ShiftUpOrDownArrowSelect();
                            break;
                        case ModifierKeys.Ctrl | ModifierKeys.Shift:
                            CtrlShiftUpOrDownArrowSelect();
                            break;
                    }
                    UserInput.KeyPress(navigationKey.ToString());
                    QueueHelper.WaitTillQueueItemsProcessed();
                    break;
                case NavigationKey.Left:
                case NavigationKey.Right:
                    switch (ModifierKeys)
                    {
                        case ModifierKeys.None:
                            LeftOrRightArrowSelect();
                            break;
                        case ModifierKeys.Ctrl:
                            CtrlLeftOrRightArrowSelect();
                            break;
                        case ModifierKeys.Shift:
                            ShiftLeftOrRightArrowSelect();
                            break;
                        case ModifierKeys.Ctrl | ModifierKeys.Shift:
                            CtrlShiftLeftOrRightArrowSelect();
                            break;
                    }
                    UserInput.KeyPress(navigationKey.ToString());
                    QueueHelper.WaitTillQueueItemsProcessed();
                    break;
                case NavigationKey.Home:
                case NavigationKey.End:
                    switch (ModifierKeys)
                    {
                        case ModifierKeys.None:
                            HomeOrEndSelect();
                            break;
                        case ModifierKeys.Ctrl:
                            CtrlHomeOrEndSelect();
                            break;
                        case ModifierKeys.Shift:
                            ShiftHomeOrEndSelect();
                            break;
                        case ModifierKeys.Ctrl | ModifierKeys.Shift:
                            CtrlShiftHomeOrEndSelect();
                            break;
                    }
                    UserInput.KeyPress(navigationKey.ToString());
                    QueueHelper.WaitTillQueueItemsProcessed();
                    break;
                case NavigationKey.PageDown:
                case NavigationKey.PageUp:
                    switch (ModifierKeys)
                    {
                        case ModifierKeys.None:
                        case ModifierKeys.Ctrl:
                            PageDownOrPageUpSelect();
                            break;
                        case ModifierKeys.Shift:
                        case ModifierKeys.Ctrl | ModifierKeys.Shift:
                            ShiftPageDownOrPageUpSelect();
                            break;
                    }
                    break;
                case NavigationKey.Tab:
                    switch (ModifierKeys)
                    {
                        case ModifierKeys.None:
                        case ModifierKeys.Shift:
                            TabOrShiftTabSelect();
                            break;
                        case ModifierKeys.Ctrl:
                        case ModifierKeys.Ctrl | ModifierKeys.Shift:
                            CtrlTabOrCtrlShiftTabSelect();
                            break;
                    }

                    UserInput.KeyPress(navigationKey.ToString());
                    QueueHelper.WaitTillQueueItemsProcessed();
                    break;
            }

            // Capture the focused cell that is the starting point of selection.
            focusedCell = DataGridHelper.GetCell(dataGrid.CurrentCell);

            return ValidateSelectedCells();
        }

        public bool KeyboardEnterSelect()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Press control+A to select cells in a datagrid.
        /// </summary>
        /// <returns>Returns true if test passes, returns false otherwise.</returns>
        public bool KeyboardCtrlASelect()
        {
            int focusedCellRowIndex = DataGridHelper.GetRowIndex(focusedCell);
            int focusedCellColumnIndex = DataGridHelper.GetColumnDisplayIndex(focusedCell);
            DataGridRowHeader rowHeader = default(DataGridRowHeader);
            expectedSelectedCells.Clear();

            switch (dataGrid.SelectionMode)
            {
                case DataGridSelectionMode.Single:
                    if (DataGridHelper.GetCell(dataGrid, focusedCellRowIndex, focusedCellColumnIndex).IsSelected)
                    {
                        switch (dataGrid.SelectionUnit)
                        {
                            case DataGridSelectionUnit.FullRow:
                                rowHeader = DataGridHelper.GetRowHeader(dataGrid, focusedCellRowIndex);

                                foreach (CellCoordinate cellCoordinate in DataGridSelectionHelper.GetCellsCoordinate(rowHeader))
                                {
                                    expectedSelectedCells.Add(cellCoordinate);
                                }
                                break;
                            case DataGridSelectionUnit.Cell:
                            case DataGridSelectionUnit.CellOrRowHeader:
                                expectedSelectedCells.Add(new CellCoordinate(focusedCellColumnIndex, focusedCellRowIndex));
                                break;
                        }
                    }
                    break;
                case DataGridSelectionMode.Extended:
                    for (int y = 0; y < dataGrid.Items.Count; y++)
                    {
                        for (int x = 0; x < dataGrid.Columns.Count; x++)
                        {
                            expectedSelectedCells.Add(new CellCoordinate(x, y));
                        }
                    }
                    break;
            }

            if (ModifierKeys != ModifierKeys.Ctrl)
            {
                ModifierKeys = ModifierKeys.None;
                ModifierKeys = ModifierKeys.Ctrl;
            }

            UserInput.KeyPress(System.Windows.Input.Key.A.ToString());
            QueueHelper.WaitTillQueueItemsProcessed();

            // Capture the focused cell that is the starting point of selection.
            focusedCell = DataGridHelper.GetCell(dataGrid.CurrentCell);

            if (expectedSelectedCells.Count.Equals(1))
            {
                return ValidateSelectedCells();
            }

            if (!expectedSelectedCells.Count.Equals(dataGrid.SelectedCells.Count))
                return false;

            return true;
        }

        #region IDisposable Members

        public void Dispose()
        {
            ModifierKeys = ModifierKeys.None;
        }

        #endregion

        #endregion
    }
}
