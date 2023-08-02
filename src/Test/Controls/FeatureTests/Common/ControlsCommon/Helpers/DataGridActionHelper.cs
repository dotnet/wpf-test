using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using Avalon.Test.ComponentModel;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Input;
using System.Windows.Threading;

namespace Microsoft.Test.Controls.Helpers
{
#if TESTBUILD_CLR40
    /// <summary>
    /// DataGrid helper for general action automation
    /// </summary>
    public static class DataGridActionHelper
    {
        /// <summary>
        /// Navigates to the paritcular row and cell within the datagrid.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public static void NavigateTo(DataGrid dataGrid, int row, int col)
        {
            // set focus
            bool isFocused = dataGrid.Focus();
            if (!isFocused)
            {
                throw new TestValidationException("Unable to set focus to the datagrid");
            }

            // scroll into view
            dataGrid.ScrollIntoView(dataGrid.Items[row]);
            QueueHelper.WaitTillQueueItemsProcessed();

            // click on first cell of row (based on displayIndex)
            DataGridColumn column = dataGrid.ColumnFromDisplayIndex(0);
            int firstIdx = dataGrid.Columns.IndexOf(column);
            ClickOnCell(dataGrid, row, firstIdx);

            // tab to the correct column index
            for (int i = 0; i < col - firstIdx; i++)
            {
                UserInput.KeyPress(System.Windows.Input.Key.Tab, true);
                QueueHelper.WaitTillQueueItemsProcessed();

                // By Design.  Need an extra Tab for Hyperlink and Template columns
                if (i + 1 < col - firstIdx &&
                    (dataGrid.Columns[i + 1] is DataGridHyperlinkColumn ||
                     dataGrid.Columns[i + 1] is DataGridTemplateColumn))
                {
                    UserInput.KeyPress(System.Windows.Input.Key.Tab, true);
                    QueueHelper.WaitTillQueueItemsProcessed();
                }
            }

            QueueHelper.WaitTillQueueItemsProcessed();
        }

        /// <summary>
        /// Navigates to the first column type in the current row
        /// </summary>
        /// <param name="row">row to navigate to</param>
        /// <param name="columnType">the column type to navigate to</param>
        public static void NavigateTo(DataGrid dataGrid, int row, DataGridHelper.ColumnTypes columnType)
        {
            int col = DataGridHelper.FindFirstColumnTypeIndex(dataGrid, columnType);
            NavigateTo(dataGrid, row, col);
        }

        public static void ClickOnCell(DataGrid dataGrid, int row, int col)
        {
            ClickOnCell(dataGrid, row, col, false, false);
        }

        public static void ClickOnCell(DataGrid dataGrid, int row, int col, bool isCtrlPressed, bool isShiftPressed)
        {
            DataGridCell cell = DataGridHelper.GetCell(dataGrid, row, col);
            FrameworkElement elem = cell as FrameworkElement;
            if (cell.Column is DataGridHyperlinkColumn)
            {
                //

                TextBlock hyperlinkTextBlock = cell.Content as TextBlock;
                Inline inline = hyperlinkTextBlock.Inlines.FirstInline;
                Hyperlink hyperlink = inline as Hyperlink;
                hyperlink.PreviewMouseLeftButtonUp += (s, e) =>
                {
                    e.Handled = true;
                };
            }

            if (isCtrlPressed)
            {
                UserInput.KeyDown(System.Windows.Input.Key.LeftCtrl.ToString());
            }
            if (isShiftPressed)
            {
                UserInput.KeyDown(System.Windows.Input.Key.LeftShift.ToString());
            }
            UserInput.MouseLeftClickCenter(elem);
            if (isCtrlPressed)
            {
                UserInput.KeyUp(System.Windows.Input.Key.LeftCtrl.ToString());
            }
            if (isShiftPressed)
            {
                UserInput.KeyUp(System.Windows.Input.Key.LeftShift.ToString());
            }
            QueueHelper.WaitTillQueueItemsProcessed();
        }

        public static void ClickOnColumnHeader(DataGrid dataGrid, int columnIndex)
        {
            ClickOnColumnHeader(dataGrid, columnIndex, false);
        }

        public static void ClickOnColumnHeader(DataGrid dataGrid, int columnIndex, bool isShiftPressed)
        {
            DataGridColumnHeader columnHeader = DataGridHelper.GetColumnHeader(dataGrid, columnIndex);
            FrameworkElement elem = columnHeader as FrameworkElement;

            if (isShiftPressed)
            {
                UserInput.KeyDown(System.Windows.Input.Key.LeftShift.ToString());
                QueueHelper.WaitTillQueueItemsProcessed();
            }

            UserInput.MouseLeftClickCenter(elem);
            QueueHelper.WaitTillQueueItemsProcessed();

            if (isShiftPressed)
            {
                UserInput.KeyUp(System.Windows.Input.Key.LeftShift.ToString());
                QueueHelper.WaitTillQueueItemsProcessed();
            }
        }

        public static void ClickOnRowHeader(DataGrid dataGrid, int rowIndex)
        {
            ClickOnRowHeader(dataGrid, rowIndex, false, false);
        }

        public static void ClickOnRowHeader(DataGrid dataGrid, int rowIndex, bool isCtrlPressed, bool isShiftPressed)
        {
            DataGridRowHeader rowHeader = DataGridHelper.GetRowHeader(dataGrid, rowIndex);
            FrameworkElement elem = rowHeader as FrameworkElement;

            if (isCtrlPressed)
            {
                UserInput.KeyDown(System.Windows.Input.Key.LeftCtrl.ToString());
            }
            if (isShiftPressed)
            {
                UserInput.KeyDown(System.Windows.Input.Key.LeftShift.ToString());
            }
            UserInput.MouseLeftClickCenter(elem);
            if (isCtrlPressed)
            {
                UserInput.KeyUp(System.Windows.Input.Key.LeftCtrl.ToString());
            }
            if (isShiftPressed)
            {
                UserInput.KeyUp(System.Windows.Input.Key.LeftShift.ToString());
            }
            QueueHelper.WaitTillQueueItemsProcessed();
        }

        public static void EditCellCustomInput(DataGrid dataGrid, int row, int col, string input)
        {
            string expectedData;
            EditCell(dataGrid, row, col, false, input, out expectedData);
        }

        public static void EditCellGenericInput(DataGrid dataGrid, int row, int col, out string expectedData)
        {
            EditCell(dataGrid, row, col, true, null, out expectedData);
        }

        private static void EditCell(DataGrid dataGrid, int row, int col, bool useRandomInput, string input, out string expectedData)
        {
            QueueHelper.WaitTillQueueItemsProcessed();

            if (!useRandomInput)
                expectedData = input;

            DataGridCell currentCell = DataGridHelper.GetCell(dataGrid, row, col);
            if (!currentCell.IsEditing)
            {
                throw new TestValidationException("Expects the current cell to be editable.");
            }
            expectedData = null;

            if (currentCell.Column is DataGridTextColumn)
            {
                ClearCell(currentCell.Content as TextBox);
                if (useRandomInput)
                {
                    SendRandomCellInput(5, out expectedData);
                }
                else
                {
                    SendCellInput(input);
                }
            }
            else if (currentCell.Column is DataGridHyperlinkColumn)
            {
                ClearCell(currentCell.Content as TextBox);

                SendCellInput("http://");
                if (useRandomInput)
                {
                    SendRandomCellInput(5, out expectedData);
                }
                else
                {
                    SendCellInput(input);
                }
                SendCellInput(".whitehouse.gov/");

                expectedData = "http://" + expectedData + ".whitehouse.gov/";
            }
            else if (currentCell.Column is DataGridComboBoxColumn)
            {
                ComboBox comboBox = currentCell.Content as ComboBox;
                comboBox.SelectedIndex = (comboBox.SelectedIndex + 1) % comboBox.Items.Count;
                expectedData = comboBox.SelectedItem.ToString();
            }
            else if (currentCell.Column is DataGridCheckBoxColumn)
            {
                CheckBox checkBox = currentCell.Content as CheckBox;
                if (checkBox.IsChecked.Value)
                {
                    checkBox.IsChecked = false;
                }
                else
                {
                    checkBox.IsChecked = true;
                }
                expectedData = checkBox.IsChecked.ToString();
            }
            else if (currentCell.Column is DataGridTemplateColumn)
            {
                ContentPresenter cp = currentCell.Content as ContentPresenter;
                TextBox tb = DataGridHelper.FindVisualChild<TextBox>(cp);
                ClearCell(tb);
                if (useRandomInput)
                {
                    SendRandomCellInput(5, out expectedData);
                }
                else
                {
                    SendCellInput(input);
                }
            }

            if (expectedData != null)
                expectedData = expectedData.ToLower();
        }

        /// <summary>
        /// In edit mode, will type the data into the cell.  Expects focus to be on a textbox.
        /// </summary>
        /// <param name="data"></param>
        public static void SendRandomCellInput(int numChars, out string data)
        {
            Random random = new Random();
            StringBuilder outputString = new StringBuilder(numChars);
            for (int i = 0; i < numChars; i++)
            {
                string character = GenerateRandomKeyboardKey(random);
                outputString.Append(character);
                UserInput.KeyPress(character);
                QueueHelper.WaitTillQueueItemsProcessed();
            }

            data = outputString.ToString();
        }

        //
        public static void SendCellInput(string data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].ToString() == ":")
                {
                    UserInput.KeyDown("LeftShift");
                    UserInput.KeyDown("OemSemicolon");
                    UserInput.KeyUp("OemSemicolon");
                    UserInput.KeyUp("LeftShift");
                }
                else if (data[i].ToString() == "/")
                {
                    UserInput.KeyPress("OemQuestion");
                }
                else if (data[i].ToString() == ".")
                {
                    UserInput.KeyPress("OemPeriod");
                }
                else if (data[i].ToString() == "\\")
                {
                    UserInput.KeyPress("OemPipe");
                }
                else if (data[i].ToString() == "\"")
                {
                    UserInput.KeyDown("LeftShift");
                    UserInput.KeyDown("OemQuotes");
                    UserInput.KeyUp("OemQuotes");
                    UserInput.KeyUp("LeftShift");
                    //UserInput.KeyPress("OemQuotes");
                }
                else if (data[i].ToString() == ",")
                {
                    UserInput.KeyPress("OemComma");
                }
                else if (data[i].ToString() == " ")
                {
                    UserInput.KeyPress("Space");
                }
                else if (data[i].ToString() == "<")
                {
                    UserInput.KeyDown("LeftShift");
                    UserInput.KeyDown("OemComma");
                    UserInput.KeyUp("OemComma");
                    UserInput.KeyUp("LeftShift");
                }
                else if (data[i].ToString() == ">")
                {
                    UserInput.KeyDown("LeftShift");
                    UserInput.KeyDown("OemPeriod");
                    UserInput.KeyUp("OemPeriod");
                    UserInput.KeyUp("LeftShift");
                }
                else if (data[i].ToString() == "&")
                {
                    UserInput.KeyDown("LeftShift");
                    UserInput.KeyDown("D7");
                    UserInput.KeyUp("D7");
                    UserInput.KeyUp("LeftShift");
                }
                else if (Char.IsDigit(data, i))
                {
                    UserInput.KeyPress(String.Format("D{0}", data[i]));
                }
                else
                {
                    UserInput.KeyPress(data[i].ToString().ToUpper());
                }

                QueueHelper.WaitTillQueueItemsProcessed();
            }
        }

        /// <summary>
        /// In edit mode, will clear the cell of data. Expects a textbox.
        /// </summary>
        public static void ClearCell(TextBox textBox)
        {
            textBox.Focus();
            Assert.AssertTrue("textBox needs to be focused to clear", textBox.IsFocused);

            textBox.SelectAll();
            QueueHelper.WaitTillQueueItemsProcessed();

            UserInput.KeyPress(System.Windows.Input.Key.Delete.ToString());
            QueueHelper.WaitTillQueueItemsProcessed();
        }

        public static void ResizeColumnHeader(DataGrid dataGrid, int colIndex, double size)
        {
            // scroll into view
            if (dataGrid.CurrentItem != null)
            {
                dataGrid.ScrollIntoView(dataGrid.CurrentItem, dataGrid.Columns[colIndex]);
            }
            else
            {
                dataGrid.ScrollIntoView(dataGrid.Items[0], dataGrid.Columns[colIndex]);
            }
            QueueHelper.WaitTillQueueItemsProcessed();

            Thumb rightThumbGripper = DataGridHelper.GetColumnHeaderGripper(dataGrid, colIndex);
            FrameworkElement elem = rightThumbGripper as FrameworkElement;
            if (elem == null)
            {
                throw new NullReferenceException("RightThumbGripper is either null or could not be converted to a FrameworkElement.");
            }
            if (!rightThumbGripper.IsVisible)
            {
                // thumb is not accessible therefore do not use mouse to resize
                return;
            }

            // it may be possible that user cannot resize the column header
            // if so a column will be sorted when it is clicked and this
            // may unintentionally throw an exception on an object that does
            // not implement IComparable.
            try
            {
                UserInput.MouseMove(elem, 0, 0);
                QueueHelper.WaitTillQueueItemsProcessed();

                UserInput.MouseLeftDown(elem);
                QueueHelper.WaitTillQueueItemsProcessed();

                UserInput.MouseMove(elem, (int)size, 0);
                QueueHelper.WaitTillQueueItemsProcessed();

                UserInput.MouseLeftUp(elem);
                QueueHelper.WaitTillQueueItemsProcessed();

                UserInput.MouseMove(0, 0);
                QueueHelper.WaitTillQueueItemsProcessed();
            }
            catch (InvalidOperationException)
            {
                // swallow the exception which is triggered by
                // a column type that does not implement IComparable.
            }
            catch (ArgumentException)
            {
                // swallow the exception which is triggered by
                // a column type that does not implement IComparable.
            }

            QueueHelper.WaitTillQueueItemsProcessed();
        }

        /// <summary>
        /// Grip Column's left side thumbgripper to resize
        /// </summary>
        /// <param name="dataGrid"> DataGrid </param>
        /// <param name="colIndex"> Column Index </param>
        /// <param name="size"> Resize quantity</param>
        public static void ResizeColumnHeaderLeft(DataGrid dataGrid, int colIndex, double size)
        {
            // scroll into view
            if (dataGrid.CurrentItem != null)
            {
                dataGrid.ScrollIntoView(dataGrid.CurrentItem, dataGrid.Columns[colIndex]);
            }
            else
            {
                dataGrid.ScrollIntoView(dataGrid.Items[0], dataGrid.Columns[colIndex]);
            }
            QueueHelper.WaitTillQueueItemsProcessed();

            Thumb thumbGripper;
            thumbGripper = DataGridHelper.GetColumnHeaderLeftGripper(dataGrid, colIndex);
            FrameworkElement elem = thumbGripper as FrameworkElement;
            if (elem == null)
            {
                throw new NullReferenceException("LeftThumbGripper is either null or could not be converted to a FrameworkElement.");
            }
            if (!thumbGripper.IsVisible)
            {
                // thumb is not accessible therefore do not use mouse to resize
                return;
            }

            // it may be possible that user cannot resize the column header
            // if so a column will be sorted when it is clicked and this
            // may unintentionally throw an exception on an object that does
            // not implement IComparable.
            try
            {
                UserInput.MouseMove(elem, 0, 0);
                QueueHelper.WaitTillQueueItemsProcessed();

                UserInput.MouseLeftDown(elem);
                QueueHelper.WaitTillQueueItemsProcessed();

                UserInput.MouseMove(elem, (int)size, 0);
                QueueHelper.WaitTillQueueItemsProcessed();

                UserInput.MouseLeftUp(elem);
                QueueHelper.WaitTillQueueItemsProcessed();

                UserInput.MouseMove(0, 0);
                QueueHelper.WaitTillQueueItemsProcessed();
            }
            catch (InvalidOperationException)
            {
                // swallow the exception which is triggered by
                // a column type that does not implement IComparable.
            }
            catch (ArgumentException)
            {
                // swallow the exception which is triggered by
                // a column type that does not implement IComparable.
            }

            QueueHelper.WaitTillQueueItemsProcessed();
        }

        public static void DoubleClickColumnHeaderGripper(DataGrid dataGrid, int colIndex)
        {
            Thumb rightThumbGripper = DataGridHelper.GetColumnHeaderGripper(dataGrid, colIndex);
            FrameworkElement elem = rightThumbGripper as FrameworkElement;
            if (elem == null)
            {
                throw new NullReferenceException("RightThumbGripper is either null or could not be converted to a FrameworkElement.");
            }

            UserInput.MouseMove(elem, 0, 0);
            QueueHelper.WaitTillQueueItemsProcessed();

            UserInput.MouseLeftDown(elem);
            UserInput.MouseLeftUp(elem);
            UserInput.MouseLeftDown(elem);
            UserInput.MouseLeftUp(elem);
            QueueHelper.WaitTillQueueItemsProcessed();

            UserInput.MouseMove(0, 0);
            DataGridHelper.WaitForDataGridMouseLeave(dataGrid);
        }

        public static void ResizeRowHeader(DataGrid dataGrid, int rowIndex, double size)
        {
            // scroll into view
            dataGrid.ScrollIntoView(dataGrid.Items[rowIndex], dataGrid.Columns[0]);
            QueueHelper.WaitTillQueueItemsProcessed();

            Thumb thumbGripper = DataGridHelper.GetRowHeaderGripper(dataGrid, rowIndex);
            FrameworkElement elem = thumbGripper as FrameworkElement;
            if (elem == null)
            {
                throw new NullReferenceException("BottomThumbGripper is either null or could not be converted to a FrameworkElement.");
            }

            UserInput.MouseMove(elem, 0, 0);
            QueueHelper.WaitTillQueueItemsProcessed();

            UserInput.MouseLeftDown(elem);
            QueueHelper.WaitTillQueueItemsProcessed();

            UserInput.MouseMove(elem, 0, (int)size);
            QueueHelper.WaitTillQueueItemsProcessed();

            UserInput.MouseLeftUp(elem);
            QueueHelper.WaitTillQueueItemsProcessed();

            UserInput.MouseMove(0, 0);
            QueueHelper.WaitTillQueueItemsProcessed();
        }

        public static void DoubleClickRowHeaderGripper(DataGrid dataGrid, int rowIndex)
        {
            // scroll into view
            dataGrid.ScrollIntoView(dataGrid.Items[rowIndex], dataGrid.Columns[0]);
            QueueHelper.WaitTillQueueItemsProcessed();

            Thumb thumbGripper = DataGridHelper.GetRowHeaderGripper(dataGrid, rowIndex);
            FrameworkElement elem = thumbGripper as FrameworkElement;
            if (elem == null)
            {
                throw new NullReferenceException("BottomThumbGripper is either null or could not be converted to a FrameworkElement.");
            }

            UserInput.MouseMove(elem, 0, 0);
            QueueHelper.WaitTillQueueItemsProcessed();

            UserInput.MouseLeftDown(elem);
            UserInput.MouseLeftUp(elem);
            UserInput.MouseLeftDown(elem);
            UserInput.MouseLeftUp(elem);
            QueueHelper.WaitTillQueueItemsProcessed();

            UserInput.MouseMove(0, 0);
            DataGridHelper.WaitForDataGridMouseLeave(dataGrid);
        }

        public static void DragColumn(DataGrid dataGrid, int startColumn, int endColumn)
        {
            DataGridColumnHeader currentColumnHeader = DataGridHelper.GetColumnHeader(dataGrid, startColumn);
            DataGridColumnHeader endColumnHeader = DataGridHelper.GetColumnHeader(dataGrid, endColumn);
            DataGridColumnHeadersPresenter columnHeadersPresenter = DataGridHelper.GetColumnHeadersPresenter(dataGrid);

            // retrieve the offsets
            Point startHeaderOffset = currentColumnHeader.TransformToAncestor(columnHeadersPresenter).Transform(new Point((int)currentColumnHeader.ActualWidth / 2, (int)currentColumnHeader.ActualHeight / 2));
            Point endHeaderOffset = endColumnHeader.TransformToAncestor(columnHeadersPresenter).Transform(new Point((int)endColumnHeader.ActualWidth / 2, (int)endColumnHeader.ActualHeight / 2));

            //UserInput.MouseMove(columnHeadersPresenter as FrameworkElement, (int)startHeaderOffset.X, (int)startHeaderOffset.Y);
            QueueHelper.WaitTillQueueItemsProcessed();

            //UserInput.MouseLeftDown(currentColumnHeader as FrameworkElement);
            UserInput.MouseLeftDown(columnHeadersPresenter as FrameworkElement, (int)startHeaderOffset.X, (int)startHeaderOffset.Y);
            QueueHelper.WaitTillQueueItemsProcessed();

            //UserInput.MouseMove(currentColumnHeader as FrameworkElement, (int)endHeaderOffset.X, (int)startHeaderOffset.Y);
            UserInput.MouseMove(columnHeadersPresenter as FrameworkElement, (int)endHeaderOffset.X, (int)startHeaderOffset.Y);
            QueueHelper.WaitTillQueueItemsProcessed();

            //UserInput.MouseLeftUp(currentColumnHeader as FrameworkElement, (int)endHeaderOffset.X, (int)startHeaderOffset.Y);
            UserInput.MouseLeftUp(columnHeadersPresenter as FrameworkElement, (int)endHeaderOffset.X, (int)startHeaderOffset.Y);
            QueueHelper.WaitTillQueueItemsProcessed();

            UserInput.MouseMove(0, 0);
            DataGridHelper.WaitForDataGridMouseLeave(dataGrid);
        }

    #region GenerateRandomKeyboardKey

        // used to generate random keyboard input
        public static readonly System.Windows.Input.Key[] KeyboardKeys =
        {
            System.Windows.Input.Key.A, System.Windows.Input.Key.B, System.Windows.Input.Key.C, System.Windows.Input.Key.D, System.Windows.Input.Key.E, System.Windows.Input.Key.F, System.Windows.Input.Key.G, System.Windows.Input.Key.H, System.Windows.Input.Key.I, System.Windows.Input.Key.J, System.Windows.Input.Key.K, System.Windows.Input.Key.L, System.Windows.Input.Key.M,
            System.Windows.Input.Key.N, System.Windows.Input.Key.O, System.Windows.Input.Key.P, System.Windows.Input.Key.Q, System.Windows.Input.Key.R, System.Windows.Input.Key.S, System.Windows.Input.Key.T, System.Windows.Input.Key.U, System.Windows.Input.Key.V, System.Windows.Input.Key.W, System.Windows.Input.Key.X, System.Windows.Input.Key.Y, System.Windows.Input.Key.Z
        };

        public static string GenerateRandomKeyboardKey(Random random)
        {
            return KeyboardKeys[random.Next(0, Int32.MaxValue) % (KeyboardKeys.Length - 1)].ToString();
        }

    #endregion GenerateRandomKeyboardKey

        public static void GetRandomCellClick(DataGrid dataGrid, int row, int col, DataGridCommandHelper.CommitEditAction action, out int mouseClickRowIdx, out int mouseClickColIdx)
        {
            mouseClickRowIdx = -1;
            mouseClickColIdx = -1;
            if (action == DataGridCommandHelper.CommitEditAction.MouseClickDifferentCell)
            {
                // click cell three columns to the right
                mouseClickRowIdx = row;
                mouseClickColIdx = (col + 3) % dataGrid.Columns.Count;
            }
            else if (action == DataGridCommandHelper.CommitEditAction.MouseClickDifferentRow)
            {
                // click row three rows down
                mouseClickRowIdx = (row + 3) % dataGrid.Items.Count;
                mouseClickColIdx = col;
            }
        }

        public static void SelectRow(DataGrid dataGrid, int rowIndex, bool isCtrlPressed, bool isShiftPressed)
        {
            // expects that it is in SelectionUnit.FullRow or SelectionUnit.CellOrRowHeader
            Assert.AssertTrue("SelectionUnit cannot be Cell in order to select a full row", dataGrid.SelectionUnit != DataGridSelectionUnit.Cell);

            if (dataGrid.SelectionUnit == DataGridSelectionUnit.FullRow)
            {
                ClickOnCell(dataGrid, rowIndex, 0, isCtrlPressed, isShiftPressed);
            }
            else if (dataGrid.SelectionUnit == DataGridSelectionUnit.CellOrRowHeader)
            {
                ClickOnRowHeader(dataGrid, rowIndex, isCtrlPressed, isShiftPressed);
            }

            QueueHelper.WaitTillQueueItemsProcessed();
        }

        public static void DeleteRow(DataGrid dataGrid, int rowIndex)
        {
            DataGridRow row = DataGridHelper.GetRow(dataGrid, rowIndex);
            if (!row.IsSelected)
            {
                SelectRow(dataGrid, rowIndex, false, false);
                QueueHelper.WaitTillQueueItemsProcessed();
            }
            else
            {
                DataGridCell cell = DataGridHelper.GetCell(dataGrid, rowIndex, 0);
                cell.Focus();
            }

            UserInput.KeyPress(System.Windows.Input.Key.Delete, true);
            QueueHelper.WaitTillQueueItemsProcessed();
        }

        public static void DeleteRows(DataGrid dataGrid, int[] rows)
        {
            foreach (int row in rows)
            {
                SelectRow(dataGrid, row, true, false);
            }

            UserInput.KeyPress(System.Windows.Input.Key.Delete, true);
            QueueHelper.WaitTillQueueItemsProcessed();
        }

        public static void CopyToClipboard(DataGrid dataGrid)
        {
            ApplicationCommands.Copy.Execute(null, dataGrid);
            QueueHelper.WaitTillQueueItemsProcessed();
        }
    }
#endif
}
