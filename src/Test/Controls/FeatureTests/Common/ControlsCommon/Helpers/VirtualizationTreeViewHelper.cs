using System.Windows.Controls;
using System.Windows.Data;

namespace Microsoft.Test.Controls.Helpers
{
    public static class VirtualizationTreeViewHelper
    {
        public static void PrepareContainerForItemOverride(TreeViewItem element, object item)
        {
            Binding binding = new Binding();
            binding.XPath = "@IsExpanded";
            binding.Source = item;
            element.SetBinding(TreeViewItem.IsExpandedProperty, binding);

            binding = new Binding();
            binding.XPath = "@Height";
            binding.Source = item;
            element.SetBinding(TextBlock.FontSizeProperty, binding);
        }
    }
}


