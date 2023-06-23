using System.Windows;
using System.Windows.Controls;
using Microsoft.Test.Controls.Helpers;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Subclass TreeView simply so I can bind the IsExpanded property on 
    /// TreeViewItem to the data list.
    /// </summary>
    public class MyTreeView : TreeView
    {
        private bool testIsVirtualizing;
        public bool TestIsVirtualizing
        {
            set
            {
                testIsVirtualizing = value;
                if (testIsVirtualizing)
                {
                    VirtualizingStackPanel.SetIsVirtualizing(this, true);
                }
                else
                {
                    VirtualizingStackPanel.SetIsVirtualizing(this, false);
                }
            }
            get
            {
                return testIsVirtualizing;
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MyTreeViewItem();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            VirtualizationTreeViewHelper.PrepareContainerForItemOverride((TreeViewItem)element, item);
        }
    }
}
