using System.Windows;
using System.Windows.Controls;
using Microsoft.Test.Controls.Helpers;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Subclass TreeViewItem simply so I can bind the IsExpanded property on 
    /// TreeViewItem to the data list.
    /// </summary>
    public class MyTreeViewItem : TreeViewItem
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MyTreeViewItem();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            VirtualizationTreeViewHelper.PrepareContainerForItemOverride((TreeViewItem)element, item);
        }

        public override string ToString()
        {
            if (HasHeader)
            {
                return string.Format("TreeViewItem {0} {1}",
                    Header is System.Xml.XmlElement ? ((System.Xml.XmlElement)Header).Attributes[0].Value : Header.ToString(),
                    (Items != null && Items.Count > 0) ? "Child Count: " + Items.Count.ToString() : "");
            }
            else
            {
                return base.ToString();
            }
        }
    }
}
