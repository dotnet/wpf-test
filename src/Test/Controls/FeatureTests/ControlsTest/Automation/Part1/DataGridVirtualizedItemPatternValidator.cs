using System;
using System.Windows.Automation;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// DataGridVirtualizedItemPatternValidator
    /// 
    /// Since we are not able to set id on virtualized item, so test DataGridRow is enough for VirtualizedItemPattern.
    /// </summary>
    public class DataGridVirtualizedItemPatternValidator : AutomationValidator
    {
        public DataGridVirtualizedItemPatternValidator(string args, string targetName, string rowId, bool isVirtualized)
            : base(args, targetName)
        {
            this.rowId = rowId;
            this.isVirtualized = isVirtualized;
        }

        private string rowId;
        private bool isVirtualized;

        public override void Run()
        {
            object pattern;

            if (targetElement.TryGetCurrentPattern(ItemContainerPattern.Pattern, out pattern))
            {
                TestVirtualizedItemPatternForRow(pattern);
            }
            else
            {
                throw new TestValidationException("Fail: DataGrid.TryGetCurrentPattern fail.");
            }
        }

        private void TestVirtualizedItemPatternForRow(object pattern)
        {
            // Allow time for this to propagate... 
            DispatcherOperations.WaitFor(System.Windows.Threading.DispatcherPriority.SystemIdle);
            ItemContainerPattern icpPattern = pattern as ItemContainerPattern;
            AutomationElement datagridRow = null;

            if (isVirtualized)
            {
                // For virtualized rows, we need to use ControlTypeProperty to get row one by one to the row index that we want.
                // Hence, we are using for loop to find the row since we have hard coded to setup the test scenario.
                for (int i = 0; i < Convert.ToInt32(rowId); i++)
                {
                    datagridRow = icpPattern.FindItemByProperty(datagridRow, AutomationElementIdentifiers.ControlTypeProperty, ControlType.DataItem);
                    if (datagridRow == null)
                    {
                        throw new TestValidationException("Fail: DataGridRow is null.");
                    }
                }

                if (datagridRow.TryGetCurrentPattern(VirtualizedItemPattern.Pattern, out pattern))
                {
                    VirtualizedItemPattern viPattern = pattern as VirtualizedItemPattern;
                    viPattern.Realize();
                }

                bool isOffscreen = (bool)datagridRow.GetCurrentPropertyValue(AutomationElement.IsOffscreenProperty);
                if (isOffscreen == true)
                {
                    throw new TestValidationException("Fail: DataGridRow isOffscreen is true.");
                }
            }
            else
            {
                datagridRow = icpPattern.FindItemByProperty(datagridRow, AutomationElementIdentifiers.AutomationIdProperty, rowId);
                if (datagridRow == null)
                {
                    throw new TestValidationException("Fail: DataGridRow is null.");
                }

                if (datagridRow.Current.AutomationId != rowId)
                {
                    throw new TestValidationException("Fail: DataGridRow AutomationId is not " + rowId);
                }

                if (datagridRow.TryGetCurrentPattern(VirtualizedItemPattern.Pattern, out pattern))
                {
                    throw new TestValidationException("Fail: DataGridRow TryGet VirtualizedItemPattern returns true when on a non-virtualized item.");
                }
            }
        }
    }
}
