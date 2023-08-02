using System;
using System.Windows.Controls;
using System.Windows.Input;
using Avalon.Test.ComponentModel;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// DataGrid keyboard navigation validator
    /// </summary>
    public class DataGridKeyboardNavigationValidator : IDisposable
    {
        # region private fields

        private DataGrid dataGrid;
        private DataGridCell focusedCell = null;
        private bool shiftDown = false;
        private bool ctrlDown = false;

        #endregion

        #region constructor

        public DataGridKeyboardNavigationValidator(DataGrid dataGrid)
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
            }
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

        #region public methods

        /// <summary>
        /// Test Tab, Shift+Tab, Ctrl+Tab and Ctrl+Shift+Tab scenarios.
        /// </summary>
        /// <returns>Returns true if test passes, returns false otherwise.</returns>
        public bool LogicalNavigate()
        {
            // We throw exception when datagrid doesn't have the focus rect because 
            // we don't know where is the focus rect.
            if (focusedCell == null)
            {
                throw new TestValidationException("The focus-rect is not within the datagrid.");
            }
            else
            {
                // Tab one more time to move focus rect to next cell if it is a hyperlink column.
                if (focusedCell.Column is DataGridHyperlinkColumn || focusedCell.Column is DataGridTemplateColumn)
                {
                    UserInput.KeyPress(System.Windows.Input.Key.Tab.ToString());
                    QueueHelper.WaitTillQueueItemsProcessed();
                }
            }

            int focusedCellRowIndex = DataGridHelper.GetRowIndex(focusedCell);
            int focusedCellColumnIndex = DataGridHelper.GetColumnDisplayIndex(focusedCell);
            DataGridCell cell = default(DataGridCell);
            CellCoordinate expectedFocusedCellCoordinate = null;

            switch (this.ModifierKeys)
            {
                case ModifierKeys.None:
                    if (focusedCellColumnIndex.Equals(dataGrid.Columns.Count - 1) && focusedCellRowIndex.Equals(dataGrid.Items.Count - 1))
                    {
                        expectedFocusedCellCoordinate = null;
                    }
                    else if (focusedCellColumnIndex.Equals(dataGrid.Columns.Count - 1))
                    {
                        cell = DataGridHelper.GetCell(dataGrid, focusedCellRowIndex + 1, 0);
                    }
                    else
                    {
                        cell = DataGridHelper.GetCell(dataGrid, focusedCellRowIndex, focusedCellColumnIndex + 1);
                    }
                    break;
                case ModifierKeys.Shift:
                    if (focusedCellColumnIndex.Equals(0) && focusedCellRowIndex.Equals(0))
                    {
                        expectedFocusedCellCoordinate = null;
                    }
                    else if (focusedCellColumnIndex.Equals(0))
                    {
                        cell = DataGridHelper.GetCell(dataGrid, focusedCellRowIndex - 1, dataGrid.Columns.Count - 1);
                    }
                    else
                    {
                        cell = DataGridHelper.GetCell(dataGrid, focusedCellRowIndex, focusedCellColumnIndex - 1);
                    }
                    break;
                case ModifierKeys.Ctrl:
                case ModifierKeys.Ctrl | ModifierKeys.Shift:
                    expectedFocusedCellCoordinate = null;
                    break;
            }

            if (cell != null)
            {
                expectedFocusedCellCoordinate = DataGridSelectionHelper.GetCellCoordinate(cell);

                // Tab one more time to move focus rect to next cell if it is a hyperlink column.
                if (cell.Column is DataGridHyperlinkColumn || cell.Column is DataGridTemplateColumn)
                {
                    UserInput.KeyPress(System.Windows.Input.Key.Tab.ToString());
                    QueueHelper.WaitTillQueueItemsProcessed();
                }
            }

            UserInput.KeyPress(System.Windows.Input.Key.Tab.ToString());
            QueueHelper.WaitTillQueueItemsProcessed();

            if (expectedFocusedCellCoordinate == null)
            {
                if (dataGrid.IsKeyboardFocusWithin)
                {
                    GlobalLog.LogEvidence("The focused rect is still in datagrid when we don't expect focus-rect in datagrid.");
                    return false;
                }
            }
            else
            {
                // Capture the focused cell.
                focusedCell = DataGridHelper.GetCell(dataGrid.CurrentCell);

                CellCoordinate actualFocusedCellCoordinate = DataGridSelectionHelper.GetCellCoordinate(focusedCell);

                if (!expectedFocusedCellCoordinate.ToString().Equals(actualFocusedCellCoordinate.ToString()))
                {
                    GlobalLog.LogEvidence("ExpectedFocusedCellCoordinate " + expectedFocusedCellCoordinate.ToString() + "!= " + "ActualFocusedCellCoordinate " + actualFocusedCellCoordinate.ToString());
                    return false;
                }
            }


            return true;
        }

        /// <summary>
        /// Test arrow keys, home, end, pageup, pagedown scenarios.
        /// </summary>
        /// <param name="directionalNavigationKey"></param>
        /// <returns>Returns true if test passes, returns false otherwise.</returns>
        public bool DirectionalNavigate(DirectionalNavigationKey directionalNavigationKey)
        {
            // We throw exception when datagrid doesn't have the focus rect because 
            // we don't know where is the focus rect.
            if (focusedCell == null)
            {
                throw new TestValidationException("The focus-rect is not within the datagrid.");
            }

            double rowHeight = 0;
            double columnWidth = 0;
            
            // Assume all datagrid rows have same height.
            double actualRowHeight = DataGridHelper.GetRowHeader(dataGrid, 0).ActualHeight;
            int focusedCellRowIndex = DataGridHelper.GetRowIndex(focusedCell);

            // In datagrid column, we have display index which is different than logical index, but in datagird row, we don’t have display index
            int focusedCellColumnIndex = DataGridHelper.GetColumnDisplayIndex(focusedCell);
            int expectedSelectColumnIndexInView = 0;
            int expectedSelectRowIndexInView = 0;
            ScrollContentPresenter scrollContentPresenter = DataGridHelper.GetScrollContentPresenter(dataGrid);
            CellCoordinate expectedFocusedCellCoordinate = null;

            switch (directionalNavigationKey)
            {
                case DirectionalNavigationKey.Up:
                case DirectionalNavigationKey.PageUp:
                    if (focusedCellRowIndex.Equals(0))
                    {
                        expectedFocusedCellCoordinate = new CellCoordinate(focusedCellColumnIndex, focusedCellRowIndex);
                    }
                    else
                    {
                        if (directionalNavigationKey == DirectionalNavigationKey.Up)
                        {
                            expectedSelectRowIndexInView = focusedCellRowIndex - 1;
                        }
                        else
                        {
                            expectedSelectRowIndexInView = 0;
                            for (int y = focusedCellRowIndex; y >= 0; y--)
                            {
                                rowHeight += actualRowHeight;

                                if (rowHeight > scrollContentPresenter.ActualHeight)
                                {
                                    expectedSelectRowIndexInView = y + 1;
                                    break;
                                }
                            }
                        }
                        expectedFocusedCellCoordinate = new CellCoordinate(focusedCellColumnIndex, expectedSelectRowIndexInView);
                    }
                    break;
                case DirectionalNavigationKey.Down:
                case DirectionalNavigationKey.PageDown:
                    if (focusedCellRowIndex.Equals(dataGrid.Items.Count - 1))
                    {
                        expectedFocusedCellCoordinate = new CellCoordinate(focusedCellColumnIndex, focusedCellRowIndex);
                    }
                    else
                    {
                        if (directionalNavigationKey == DirectionalNavigationKey.Down)
                        {
                            expectedSelectRowIndexInView = focusedCellRowIndex + 1;
                        }
                        else
                        {
                            expectedSelectRowIndexInView = dataGrid.Items.Count - 1;
                            for (int y = focusedCellRowIndex; y < dataGrid.Items.Count; y++)
                            {
                                rowHeight += actualRowHeight;

                                if (rowHeight > scrollContentPresenter.ActualHeight)
                                {
                                    expectedSelectRowIndexInView = y - 1;
                                    break;
                                }
                            }
                        }
                        expectedFocusedCellCoordinate = new CellCoordinate(focusedCellColumnIndex, expectedSelectRowIndexInView);
                    }
                    break;
                case DirectionalNavigationKey.Left:
                case DirectionalNavigationKey.Home:
                    if (focusedCellColumnIndex.Equals(0))
                    {
                        expectedFocusedCellCoordinate = new CellCoordinate(focusedCellColumnIndex, focusedCellRowIndex);
                    }
                    else
                    {
                        if (directionalNavigationKey == DirectionalNavigationKey.Left)
                        {
                            expectedSelectColumnIndexInView = focusedCellColumnIndex - 1;
                        }
                        else
                        {
                            expectedSelectColumnIndexInView = 0;
                            for (int x = focusedCellColumnIndex; x >= 0; x--)
                            {
                                columnWidth += DataGridHelper.GetCell(dataGrid, focusedCellRowIndex, x).ActualWidth;

                                if (columnWidth > scrollContentPresenter.ActualWidth)
                                {
                                    expectedSelectColumnIndexInView = x + 1;
                                    break;
                                }
                            }
                        }
                        expectedFocusedCellCoordinate = new CellCoordinate(expectedSelectColumnIndexInView, focusedCellRowIndex);
                    }
                    break;
                case DirectionalNavigationKey.Right:
                case DirectionalNavigationKey.End:
                    if (focusedCellColumnIndex.Equals(dataGrid.Columns.Count - 1))
                    {
                        expectedFocusedCellCoordinate = new CellCoordinate(focusedCellColumnIndex, focusedCellRowIndex);
                    }
                    else
                    {
                        if (directionalNavigationKey == DirectionalNavigationKey.Right)
                        {
                            expectedSelectColumnIndexInView = focusedCellColumnIndex + 1;
                        }
                        else
                        {
                            expectedSelectColumnIndexInView = dataGrid.Columns.Count - 1;
                            for (int x = focusedCellColumnIndex; x < dataGrid.Columns.Count; x++)
                            {
                                columnWidth += DataGridHelper.GetCell(dataGrid, focusedCellRowIndex, x).ActualWidth;

                                if (columnWidth > scrollContentPresenter.ActualWidth)
                                {
                                    expectedSelectColumnIndexInView = x - 1;
                                    break;
                                }
                            }
                        }
                        expectedFocusedCellCoordinate = new CellCoordinate(expectedSelectColumnIndexInView, focusedCellRowIndex);
                    }
                    break;
            }

            UserInput.KeyPress(directionalNavigationKey.ToString());
            QueueHelper.WaitTillQueueItemsProcessed();

            if (expectedFocusedCellCoordinate == null)
            {
                if (dataGrid.IsKeyboardFocusWithin)
                {
                    GlobalLog.LogEvidence("The focused rect is still in datagrid when we don't expect focus-rect in datagrid.");
                    return false;
                }
            }
            else
            {
                // Capture the focused cell.
                focusedCell = DataGridHelper.GetCell(dataGrid.CurrentCell);

                CellCoordinate actualFocusedCellCoordinate = DataGridSelectionHelper.GetCellCoordinate(focusedCell);

                if (!expectedFocusedCellCoordinate.ToString().Equals(actualFocusedCellCoordinate.ToString()))
                {
                    GlobalLog.LogEvidence("ExpectedFocusedCellCoordinate " + expectedFocusedCellCoordinate.ToString() + "!= " + "ActualFocusedCellCoordinate " + actualFocusedCellCoordinate.ToString());
                    return false;
                }
            }


            return true;
        }

        #region IDisposable Members

        public void Dispose()
        {
            // Restore the test in a clean when unexpected thing happens during testing.
            ModifierKeys = ModifierKeys.None;
        }

        #endregion

        #endregion
    }
}
