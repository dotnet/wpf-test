using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Avalon.Test.ComponentModel;
using Microsoft.Test.Input;

namespace Microsoft.Test.Controls.Helpers
{
#if TESTBUILD_CLR40
    /// <summary>
    /// DataGrid helper for command automation 
    /// </summary>
   public static class DataGridCommandHelper
    {        
    #region Enums

        public enum BeginEditAction
        {
            None,
            MethodCall,
            F2,
            FocusClick,
            DoubleClick,

            /// <summary>
            /// Only works for Text and Hyperlink Columns
            /// </summary>
            Keystroke,

            /// <summary>
            /// Only works for CheckBoxColumn
            /// </summary>
            SpaceBarToggle,

            /// <summary>
            /// Only works for ComboBoxColumn
            /// </summary>
            AltToggle,

            /// <summary>
            /// Only works for ComboBoxColumn
            /// </summary>
            F4
        }

        public enum CancelEditAction
        {
            None,
            MethodCall,
            MethodCallCellUnit,
            MethodCallRowUnit,

            /// <summary>
            /// Cancels cell edit, but leaves cell and row in edit mode
            /// </summary>
            Esc
        }

        public enum CommitEditAction
        {
            None,
            MethodCall,
            MethodCallCellUnit,
            MethodCallRowUnit,
            
            /// <summary>
            /// Commits changes to the entire row, exits edit mode, moves to the next row
            /// </summary>
            Enter,            

            /// <summary>
            /// Commits changes to the current cell, stays in edit mode, moves to the next cell 
            /// in the row.  Note: if you tab out of the row, it will commit the entire row and move focus 
            /// to the first cell of the next row
            /// </summary>
            Tab,

            /// <summary>
            /// Commits changes to the current cell, stays in edit mode, moves to the previous cell 
            /// in the row.  Note: if you tab out of the row, it will commit the entire row and move focus 
            /// to the last cell of the previous row
            /// </summary>
            ShiftTab,

            /// <summary>
            /// Commits entire row, exits edit mode
            /// </summary>
            FocusLost,

            /// <summary>
            /// Commits entire row, exits edit mode
            /// </summary>
            MouseClickDifferentRow,

            /// <summary>
            /// Commits cell, exits in edit mode
            /// </summary>
            MouseClickDifferentCell,

            /// <summary>
            /// Commits cell, exits edit mode
            /// </summary>
            MouseClickColumnHeader,

            /// <summary>
            /// Commits cell, exits edit mode
            /// </summary>
            MouseClickRowHeader
        }

        #endregion Enums

        public static void BeginEdit(DataGrid dataGrid, int row, int col)
        {
            BeginEdit(dataGrid, row, col, BeginEditAction.F2);
        }

        public static void BeginEdit(DataGrid dataGrid, int row, int col, BeginEditAction action)
        {
            QueueHelper.WaitTillQueueItemsProcessed();
            DataGridCell cellToEdit = DataGridHelper.GetCell(dataGrid, row, col);

            // make sure focus isn't already on cell for double click and focus click actions
            if (cellToEdit.IsFocused && (action == BeginEditAction.DoubleClick || action == BeginEditAction.FocusClick))
            {
                // move away from cell
                if (dataGrid.Columns.Count - 1 == col)
                {
                    UserInput.KeyPress(System.Windows.Input.Key.Left, true);
                }
                else
                {
                    UserInput.KeyPress(System.Windows.Input.Key.Right, true);
                }
                QueueHelper.WaitTillQueueItemsProcessed();
            }

            switch (action)
            {
                case BeginEditAction.F2:
                    DataGridActionHelper.NavigateTo(dataGrid, row, col);
                    UserInput.KeyPress(System.Windows.Input.Key.F2, true);
                    QueueHelper.WaitTillQueueItemsProcessed();
                    break;

                case BeginEditAction.F4:
                    DataGridActionHelper.NavigateTo(dataGrid, row, col);
                    UserInput.KeyPress(System.Windows.Input.Key.F4, true);
                    QueueHelper.WaitTillQueueItemsProcessed();
                    break;

                case BeginEditAction.Keystroke:
                    DataGridActionHelper.NavigateTo(dataGrid, row, col);
                    UserInput.KeyPress(System.Windows.Input.Key.E, true);
                    QueueHelper.WaitTillQueueItemsProcessed();
                    break;

                case BeginEditAction.AltToggle:
                    DataGridActionHelper.NavigateTo(dataGrid, row, col);
                    UserInput.KeyDown(System.Windows.Input.Key.LeftAlt.ToString());
                    UserInput.KeyDown(System.Windows.Input.Key.Down.ToString());
                    UserInput.KeyUp(System.Windows.Input.Key.Down.ToString());
                    UserInput.KeyUp(System.Windows.Input.Key.LeftAlt.ToString());
                    QueueHelper.WaitTillQueueItemsProcessed();
                    break;

                case BeginEditAction.SpaceBarToggle:
                    DataGridActionHelper.NavigateTo(dataGrid, row, col);
                    UserInput.KeyDown(System.Windows.Input.Key.Space.ToString());
                    UserInput.KeyDown(System.Windows.Input.Key.Add.ToString());
                    UserInput.KeyUp(System.Windows.Input.Key.Add.ToString());
                    UserInput.KeyUp(System.Windows.Input.Key.Space.ToString());
                    QueueHelper.WaitTillQueueItemsProcessed();
                    break;

                case BeginEditAction.FocusClick:                    
                    // put focus on cell
                    DataGridActionHelper.NavigateTo(dataGrid, row, col);                   

                    // click to begin editing
                    FrameworkElement elem = cellToEdit as FrameworkElement;
                    UserInput.MouseMove(elem, 0, 0);
                    QueueHelper.WaitTillQueueItemsProcessed();
                    UserInput.MouseLeftDown(elem);
                    UserInput.MouseLeftUp(elem);                    
                    QueueHelper.WaitTillQueueItemsProcessed();
                    break;

                case BeginEditAction.DoubleClick:
                    elem = cellToEdit as FrameworkElement;

                    UserInput.MouseMove(elem, 0, 0);
                    QueueHelper.WaitTillQueueItemsProcessed();

                    UserInput.MouseLeftDown(elem);
                    UserInput.MouseLeftUp(elem);
                    UserInput.MouseLeftDown(elem);
                    UserInput.MouseLeftUp(elem);
                    QueueHelper.WaitTillQueueItemsProcessed();
                    break;

                case BeginEditAction.MethodCall:
                    DataGridActionHelper.NavigateTo(dataGrid, row, col);
                    dataGrid.BeginEdit();
                    QueueHelper.WaitTillQueueItemsProcessed();
                    break;
                default:
                    break;
            }

            QueueHelper.WaitTillQueueItemsProcessed();
        }

        public static void CancelEdit(DataGrid dataGrid)
        {
            CancelEdit(dataGrid, CancelEditAction.Esc);
        }

        public static void CancelEdit(DataGrid dataGrid, CancelEditAction action)
        {
            QueueHelper.WaitTillQueueItemsProcessed();

            switch (action)
            {
                case CancelEditAction.Esc:
                    UserInput.KeyPress(System.Windows.Input.Key.Escape, true);
                    QueueHelper.WaitTillQueueItemsProcessed();
                    break;

                case CancelEditAction.MethodCall:
                    dataGrid.CancelEdit();
                    break;
                case CancelEditAction.MethodCallCellUnit:
                    dataGrid.CancelEdit(DataGridEditingUnit.Cell);
                    break;
                case CancelEditAction.MethodCallRowUnit:
                    dataGrid.CancelEdit(DataGridEditingUnit.Row);
                    break;
                default:
                    break;
            }
        }

        public static void CommitEdit(DataGrid dataGrid, int row, int col)
        {
            CommitEdit(
                dataGrid, 
                row, 
                col, 
                CommitEditAction.Enter);
        }

        public static void CommitEdit(DataGrid dataGrid, int row, int col, CommitEditAction action)
        {
            CommitEdit(
                dataGrid,
                row,
                col,
                action,
                (row + 1) % dataGrid.Items.Count,
                (col + 1) % dataGrid.Columns.Count);
        }

        public static void CommitEdit(DataGrid dataGrid, int row, int col, CommitEditAction action, int mouseClickRowIdx, int mouseClickColIdx)
        {
            QueueHelper.WaitTillQueueItemsProcessed();

            switch (action)
            {
                case CommitEditAction.Enter:
                    UserInput.KeyPress(System.Windows.Input.Key.Enter, true);
                    QueueHelper.WaitTillQueueItemsProcessed();
                    break;
                
                case CommitEditAction.MouseClickDifferentCell:
                    DataGridActionHelper.ClickOnCell(dataGrid, row, mouseClickColIdx);                    
                    break;

                case CommitEditAction.MouseClickDifferentRow:
                    DataGridActionHelper.ClickOnCell(dataGrid, mouseClickRowIdx, 0);                    
                    break;

                case CommitEditAction.Tab:
                    UserInput.KeyPress(System.Windows.Input.Key.Tab, true);
                    QueueHelper.WaitTillQueueItemsProcessed();
                    break;

                case CommitEditAction.ShiftTab:
                    UserInput.KeyDown("LeftShift");
                    UserInput.KeyDown("Tab");
                    UserInput.KeyUp("Tab");
                    UserInput.KeyUp("LeftShift");
                    QueueHelper.WaitTillQueueItemsProcessed();
                    break;

                case CommitEditAction.MethodCall:
                    dataGrid.CommitEdit();
                    QueueHelper.WaitTillQueueItemsProcessed();
                    break;

                case CommitEditAction.MethodCallCellUnit:
                    dataGrid.CommitEdit(DataGridEditingUnit.Cell, true);
                    QueueHelper.WaitTillQueueItemsProcessed();
                    break;

                case CommitEditAction.MethodCallRowUnit:
                    dataGrid.CommitEdit(DataGridEditingUnit.Row, true);
                    QueueHelper.WaitTillQueueItemsProcessed();
                    break;

                case CommitEditAction.FocusLost:
                    //
                    break;

                case CommitEditAction.MouseClickColumnHeader:
                    DataGridActionHelper.ClickOnColumnHeader(dataGrid, mouseClickColIdx);
                    break;

                case CommitEditAction.MouseClickRowHeader:
                    DataGridActionHelper.ClickOnRowHeader(dataGrid, mouseClickRowIdx);
                    break; 

                default:
                    break;
            }
        }

        /// <summary>
        /// Checks to see if the column type supports the begin edit default input gesture
        /// </summary>
        /// <param name="columnType">the column type to check</param>
        /// <param name="gesture">the gesture to check</param>
        /// <returns>true if has gesture, false otherwise</returns>
        public static bool ColumnHasBeginEditAction(DataGridHelper.ColumnTypes columnType, DataGridCommandHelper.BeginEditAction gesture)
        {
            bool retVal = true;
            if (gesture == DataGridCommandHelper.BeginEditAction.Keystroke)
            {
                if (columnType != DataGridHelper.ColumnTypes.DataGridTextColumn && columnType != DataGridHelper.ColumnTypes.DataGridHyperlinkColumn)
                    retVal = false;
            }
            else if (gesture == DataGridCommandHelper.BeginEditAction.F4 || gesture == DataGridCommandHelper.BeginEditAction.AltToggle)
            {
                if (columnType != DataGridHelper.ColumnTypes.DataGridComboBoxColumn)
                    retVal = false;
            }
            else if (gesture == DataGridCommandHelper.BeginEditAction.SpaceBarToggle)
            {
                if (columnType != DataGridHelper.ColumnTypes.DataGridCheckBoxColumn)
                    retVal = false;
            }            
            else if ((gesture == DataGridCommandHelper.BeginEditAction.FocusClick || gesture == DataGridCommandHelper.BeginEditAction.DoubleClick))
            {
                if (columnType == DataGridHelper.ColumnTypes.DataGridHyperlinkColumn)
                {
                    retVal = false;
                }
                else if (columnType == DataGridHelper.ColumnTypes.DataGridTemplateColumn)
                {
                    retVal = false;
                }
            }

            return retVal;
        }
    }    
#endif
}
