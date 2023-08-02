using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// ViewportHelper
    /// </summary>
    public static class ViewportHelper
    {
        /// <summary>
        /// Whether the item is in viewport or not.
        /// </summary>
        /// <param name="container">A Control reference</param>
        /// <returns>Returns true if the Control is in viewport; returns false otherwise</returns>
        public static bool IsInViewport(Control item)
        {
            ItemsControl itemsControl = null;

            if (item is ListBoxItem)
            {
                itemsControl = ListBoxHelper.GetListBox(item);
            }
            else if (item is TreeViewItem)
            {
                itemsControl = TreeViewItemHelper.GetTreeView((TreeViewItem)item);
            }
#if TESTBUILD_CLR40
            else if (item is DataGridCell)
            {
                itemsControl = (ItemsControl)item.GetType().InvokeMember("DataGridOwner", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty, null, item, null);
            }
#endif
            else
            {
                throw new NotSupportedException(item.GetType().Name);
            }

            ScrollViewer scrollViewer = VisualTreeHelper.GetVisualChild<ScrollViewer, ItemsControl>(itemsControl);
            ScrollContentPresenter scrollContentPresenter = ScrollViewerHelper.GetScrollContentPresenter(scrollViewer);
            MethodInfo isInViewportMethod = scrollViewer.GetType().GetMethod("IsInViewport", BindingFlags.NonPublic | BindingFlags.Instance);

            return (bool)isInViewportMethod.Invoke(scrollViewer, new object[] { scrollContentPresenter, item });
        }

        /// <summary>
        /// Get First Top Item InViewport Index
        /// </summary>
        /// <param name="listbox">A ListBox reference</param>
        /// <returns>Returns the first top item InViewport index</returns>
        public static int GetFirstTopItemInViewportIndex(ListBox listbox)
        {
            ScrollViewer scrollViewer = VisualTreeHelper.GetVisualChild<ScrollViewer, ListBox>(listbox);
            ScrollContentPresenter scrollContentPresenter = ScrollViewerHelper.GetScrollContentPresenter(scrollViewer);
            VirtualizingStackPanel virtualizingStackPanel = ScrollViewerHelper.GetVirtualizingStackPanel(scrollContentPresenter);

            foreach (ListBoxItem item in virtualizingStackPanel.Children)
            {
                if (item.PointToScreen(new Point()).Y + item.ActualHeight / 2 > scrollContentPresenter.PointToScreen(new Point()).Y)
                {
                    return listbox.ItemContainerGenerator.IndexFromContainer(item);
                }
            }

            throw new ArgumentNullException("Fail: the first top item InViewport index is not found.");
        }

        /// <summary>
        /// Get First Bottom Item Not InViewport Index
        /// </summary>
        /// <param name="listbox">A ListBox reference</param>
        /// <param name="firstItemInViewportIndex">First item InViewport index</param>
        /// <returns>Returns the first bottom item not InViewport index</returns>
        public static int GetFirstBottomItemNotInViewportIndex(ListBox listbox, int firstItemInViewportIndex)
        {
            int firstBottomItemNotInViewportIndex = 0;
            GetNumberOfItemsInViewport(listbox, firstItemInViewportIndex, ref firstBottomItemNotInViewportIndex);
            return firstBottomItemNotInViewportIndex;
        }

        /// <summary>
        /// Get Last Bottom Item InViewport Index
        /// </summary>
        /// <param name="listbox">A ListBox reference</param>
        /// <param name="firstItemInViewportIndex">First item InViewport index </param>
        /// <returns>Returns the last bottom item InViewport index</returns>
        public static int GetLastBottomItemInViewportIndex(ListBox listbox, int firstItemInViewportIndex)
        {
            int firstBottomItemNotInViewportIndex = 0;
            GetNumberOfItemsInViewport(listbox, firstItemInViewportIndex, ref firstBottomItemNotInViewportIndex);
            // the first bottom item not in view port index subtract 1 is the last bottom item in view port.
            return firstBottomItemNotInViewportIndex - 1;
        }

        /// <summary>
        /// Get Number Of Items InViewport
        /// </summary>
        /// <param name="listbox">A ListBox reference</param>
        /// <param name="firstItemInViewportIndex">First item InViewport index</param>
        /// <returns>Returns the number of items InViewport</returns>
        public static int GetNumberOfItemsInViewport(ListBox listbox, int firstItemInViewportIndex)
        {
            int firstBottomItemNotInViewportIndex = 0;
            return GetNumberOfItemsInViewport(listbox, firstItemInViewportIndex, ref firstBottomItemNotInViewportIndex);
        }

        private static int GetNumberOfItemsInViewport(ListBox listbox, int firstItemInViewportIndex, ref int index)
        {
            ScrollViewer scrollViewer = VisualTreeHelper.GetVisualChild<ScrollViewer, ListBox>(listbox);
            double viewPortHeight = scrollViewer.ViewportHeight;
            ScrollContentPresenter scrollContentPresenter = ScrollViewerHelper.GetScrollContentPresenter(scrollViewer);
            double itemsHeight = 0;
            index = firstItemInViewportIndex;
            int count = 1;
            while (itemsHeight < scrollContentPresenter.ActualHeight)
            {
                ListBoxItem listBoxItem = listbox.ItemContainerGenerator.ContainerFromIndex(index++) as ListBoxItem;
                if (listBoxItem == null)
                {
                    break;
                }

                itemsHeight += listBoxItem.ActualHeight;
                count++;
            }

            index = index - 1;
            
            if (listbox.ItemContainerGenerator.ContainerFromIndex(index) != null)
            {
                if (!ViewportHelper.IsInViewport((ListBoxItem)listbox.ItemContainerGenerator.ContainerFromIndex(index)))
                {
                    throw new TestValidationException("Fail: " + index + " is not in Viewport.");
                }

                if (listbox.ItemContainerGenerator.ContainerFromIndex(index + 1) != null)
                {
                    if (ViewportHelper.IsInViewport((ListBoxItem)listbox.ItemContainerGenerator.ContainerFromIndex(index + 1)))
                    {
                        throw new TestValidationException("Fail: " + (index + 1) + " is in Viewport.");
                    }
                }
            }

            // subtract 1 because it includes one that is not in viewport
            return count - 1;
        }
    }
}
