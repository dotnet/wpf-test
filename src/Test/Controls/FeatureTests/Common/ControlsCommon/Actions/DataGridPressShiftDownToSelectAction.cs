using System.Windows.Controls;
using System.Windows.Input;
using Avalon.Test.ComponentModel;
using Microsoft.Test.Input;

namespace Microsoft.Test.Controls.Actions
{
#if TESTBUILD_CLR40
    /// <summary>
    /// DataGrid press ShiftDown to select action.
    /// </summary>
  public class DataGridPressShiftDownToSelectAction : DataGridAction
    {
        public override void Perform(Control control)
        {
            // Select and set focus on the first datagrid row.
            UserInput.MouseLeftClickCenter(control);
            QueueHelper.WaitTillQueueItemsProcessed();

            // Shift+Down to select one more item.
            UserInput.KeyDown(System.Windows.Input.Key.LeftShift.ToString());
            QueueHelper.WaitTillQueueItemsProcessed();
            UserInput.KeyDown(System.Windows.Input.Key.Down.ToString());
            UserInput.KeyUp(System.Windows.Input.Key.Down.ToString());
            QueueHelper.WaitTillQueueItemsProcessed();
            UserInput.KeyUp(System.Windows.Input.Key.LeftShift.ToString());
            QueueHelper.WaitTillQueueItemsProcessed();
        }
    }
#endif
    }


