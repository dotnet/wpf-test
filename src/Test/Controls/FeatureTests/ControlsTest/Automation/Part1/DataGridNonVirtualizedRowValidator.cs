using System;
using System.Windows.Automation;
using System.Windows.Threading;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// DataGridNonVirtualizedRowValidator
    /// 
    /// Ensure we can bring non-virtualized item that is not in view into view.
    /// </summary>
    public class DataGridNonVirtualizedRowValidator : AutomationValidator
    {
        public DataGridNonVirtualizedRowValidator(string args, string targetName, string rowId)
            : base(args, targetName)
        {
            this.rowId = rowId;
        }

        private string rowId;

        public override void Run()
        {
            object pattern;

            if (targetElement.TryGetCurrentPattern(ItemContainerPattern.Pattern, out pattern))
            {
                TestNonVirtualizedItemForRow(pattern);
            }
            else
            {
                throw new TestValidationException("Fail: DataGrid.TryGetCurrentPattern fail.");
            }
        }

        private void TestNonVirtualizedItemForRow(object pattern)
        {
            ItemContainerPattern icpPattern = pattern as ItemContainerPattern;
            AutomationElement datagridRow = null;

            for (int i = 0; i < Convert.ToInt32(rowId); i++)
            {
                datagridRow = icpPattern.FindItemByProperty(datagridRow, AutomationElementIdentifiers.ControlTypeProperty, ControlType.DataItem);
                if (datagridRow == null)
                {
                    throw new TestValidationException("Fail: DataGridRow is null.");
                }
            }

            if (datagridRow.TryGetCurrentPattern(ScrollItemPattern.Pattern, out pattern))
            {
                ScrollItemPattern viPattern = pattern as ScrollItemPattern;
                viPattern.ScrollIntoView();
            }

            bool isOffscreen = (bool)datagridRow.GetCurrentPropertyValue(AutomationElement.IsOffscreenProperty);
            if (isOffscreen == true)
            {
                throw new TestValidationException("Fail: DataGridRow isOffscreen is true.");
            }
        }
    }
}
