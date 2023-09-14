using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// ItemsControl Modifier
    /// </summary>
    public class ItemsControlModifier
    {
        public ItemsControlModifier(ItemsControl itemsControl)
        {
            this.itemsControl = itemsControl;
        }
        private ItemsControl itemsControl;

        /// <summary>
        /// It modifies the itemsControl's virtualization.
        /// </summary>
        /// <param name="isVirtualizing">isVirtualizing bool</param>
        /// <param name="isItemVirtualizing">isItemVirtualizing bool</param>
        /// <param name="virtualizationMode">virtualizationMode enum</param>
        public void Modify(bool isVirtualizing, bool isItemVirtualizing, VirtualizationMode virtualizationMode)
        {
            itemsControl.SetValue(VirtualizingStackPanel.IsVirtualizingProperty, isVirtualizing);

            Style itemContainerStyle = new Style();
            Setter isVirtualizingSetter = new Setter(VirtualizingStackPanel.IsVirtualizingProperty, isItemVirtualizing);
            itemContainerStyle.Setters.Add(isVirtualizingSetter);
            Setter virtualizationModeSetter = new Setter(VirtualizingStackPanel.VirtualizationModeProperty, virtualizationMode);
            itemContainerStyle.Setters.Add(virtualizationModeSetter);
            itemsControl.ItemContainerStyle = itemContainerStyle;
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
        }
    }
}
