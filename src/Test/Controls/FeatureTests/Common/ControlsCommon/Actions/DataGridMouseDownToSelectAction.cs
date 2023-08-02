using System;
using System.Windows.Controls;
using Avalon.Test.ComponentModel;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Input;
using System.Windows.Controls.Primitives;

namespace Microsoft.Test.Controls.Actions
{
#if TESTBUILD_CLR40
    /// <summary>
    /// DataGrid MouseDown to select action.
    /// </summary>
    public class DataGridMouseDownToSelectAction : DataGridAction
    {
        public override void Perform(Control control)
        {
            base.Perform(control);
            DataGrid dataGrid = DataGridHelper.GetDataGridFromChild(control);
            DataGridColumnHeader dataGridColumnHeader = DataGridHelper.GetColumnHeader(dataGrid, 0);

            // Select and set focus on the first datagrid row.
            UserInput.MouseLeftDown(control, Convert.ToInt32(control.ActualWidth / 2), Convert.ToInt32(control.ActualHeight / 2));
            QueueHelper.WaitTillQueueItemsProcessed();
            UserInput.MouseMove(dataGrid, Convert.ToInt32(control.ActualWidth / 2), Convert.ToInt32(control.ActualHeight / 2 + control.ActualHeight + dataGridColumnHeader.ActualHeight));
            QueueHelper.WaitTillQueueItemsProcessed();

            // Clean up.
            UserInput.MouseLeftUp(control, Convert.ToInt32(control.ActualWidth / 2), Convert.ToInt32(control.ActualHeight / 2 + control.ActualHeight));
            QueueHelper.WaitTillQueueItemsProcessed();
        }
    }
#endif
}


