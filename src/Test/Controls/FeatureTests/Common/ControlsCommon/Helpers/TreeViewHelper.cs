using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// TreeViewHelper
    /// </summary>
    public static class TreeViewHelper
    {
        /// <summary>
        /// Get a container from a ItemsControl
        /// </summary>
        /// <param name="itemsControl">A ItemsControl reference</param>
        /// <param name="index">An Index that you'd like to get the container from</param>
        /// <returns>Returns a TreeViewItem reference if found it; returns null otherwise</returns>
        public static TreeViewItem GetContainer(ItemsControl itemsControl, int index)
        {
            return itemsControl.ItemContainerGenerator.ContainerFromIndex(index) as TreeViewItem;
        }

        /// <summary>
        /// Convert TreeViewItem To TreeViewItemHeader
        /// </summary>
        /// <param name="visibleTreeViewItems">A list of TreeViewItem</param>
        /// <returns>A list of ContentPresenter</returns>
        public static List<ContentPresenter> ConvertToTreeViewItemHeader(List<TreeViewItem> visibleTreeViewItems)
        {
            List<ContentPresenter> visibleTreeViewItemContentPresenters = new List<ContentPresenter>();

            foreach (TreeViewItem treeViewItem in visibleTreeViewItems)
            {
                if (treeViewItem != null)
                {
                    ContentPresenter cp = treeViewItem.Template.FindName("PART_Header", treeViewItem) as ContentPresenter;
                    visibleTreeViewItemContentPresenters.Add(cp);
                }
            }

            return visibleTreeViewItemContentPresenters;
        }


        /// <summary>
        /// Scroll TreeView's ScrollView to top.
        /// </summary>
        /// <param name="treeviewitem">A TreeViewItem reference</param>
        public static void ScrollTreeViewToTop(TreeViewItem treeviewitem)
        {
            ScrollViewer scrollViewer = VisualTreeHelper.GetVisualChild<ScrollViewer, TreeView>(TreeViewItemHelper.GetTreeView(treeviewitem));

            scrollViewer.ScrollToHome();
            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);
        }
    }
}


