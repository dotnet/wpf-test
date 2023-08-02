using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using System.Xml;
using System.Windows;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// TreeViewItemHelper
    /// </summary>
    public static class TreeViewItemHelper
    {
        /// <summary>
        /// Get TreeView from TreeViewItem
        /// </summary>
        /// <param name="item">A TreeViewItem reference</param>
        /// <returns>Returns a TreeView reference if found it; returns null otherwise</returns>
        public static TreeView GetTreeView(TreeViewItem item)
        {
            return (TreeView)item.GetType().InvokeMember("ParentTreeView", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty, null, item, null); 
        }

        /// <summary>
        /// Get treeviewitem count that is XmlNode treeviewitem node(data) count.
        /// </summary>
        /// <param name="treeview">A TreeView reference</param>
        /// <param name="treeviewitemName">TreeViewItem name</param>
        /// <returns>Returns TreeViewItems count</returns>
        public static int GetTreeViewItemCount(TreeViewItem treeviewitem, string xpathName)
        {
            string xpath = "descendant::" + xpathName;

            XmlElement firstItem = treeviewitem.Items[0] as XmlElement;

            // Plus treeviewitem parameter itself
            return firstItem.ParentNode.SelectNodes(xpath).Count + 1;
        }

        /// <summary>
        /// Check to see if TreeViewItem subtree is expanded and get the expanded item count.
        /// </summary>
        /// <param name="container">A TreeViewItem reference</param>
        /// <param name="count">A counter for expanded items</param>
        /// <returns>Returns true if subtree is expanded; returns false otherwise</returns>
        public static bool IsSubtreeExpanded(TreeViewItem container, ref int count)
        {
            if (!container.IsExpanded)
            {
                throw new ArgumentOutOfRangeException("Fail: container is not expanded.");
            }
            else
            {
                count++;
            }

            container.BringIntoView();
            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);

            for (int i = 0; i < container.Items.Count; i++)
            {
                TreeViewItem treeviewitem = container.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItem;

                if (treeviewitem != null)
                {
                    IsSubtreeExpanded(treeviewitem, ref count);
                }
            }

            return true;
        }
    }
}
