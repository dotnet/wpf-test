using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Test.Controls
{
    public static class UIADataGridHelper
    {
        public static void SetupAutomationIdForEachCell(DataGrid datagrid, Button ready)
        {
            for (int y = 0; y < datagrid.Items.Count; y++)
            {
                DataGridRow dataGridRow = (DataGridRow)datagrid.ItemContainerGenerator.ContainerFromIndex(y);
                dataGridRow.BringIntoView();
                DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

                DataGridCellsPresenter presenter = Microsoft.Test.Controls.VisualTreeHelper.GetVisualChild<DataGridCellsPresenter, DataGridRow>(dataGridRow);
                for (int x = 0; x < presenter.Items.Count; x++)
                {
                    DataGridCell datagridCell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(x);
                    if (datagridCell != null)
                    {
                        string cellCoordinate = string.Format("{0},{1}", x, y);
                        AutomationProperties.SetAutomationId(datagridCell, cellCoordinate);
                    }
                }
            }

            ScrollViewer scrollViewer = VisualTreeHelper.GetVisualChild<ScrollViewer, DataGrid>(datagrid);
            scrollViewer.ScrollToHome();
            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);

            InputHelper.MouseClickButtonCenter(ready, ready.ClickMode, MouseButton.Left);
        }
    }
}
