// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Test;
using System.Windows.Controls.Primitives;
using System.Reflection;

namespace Microsoft.Test.Input.MultiTouch
{
    /// <summary>
    /// This is a temp visual tree helper kind of class, encapsulating many helper methods for various controls
    /// that are useful in the MT testing.  There is no single class existing currently in the test tree that would 
    /// supply all that we need here, so we can either go the hassle of referrencing ControlsCommon (another area) 
    /// or just reuse some of the helpers from there. In MQ - we should come up with a better story as to how we 
    /// manage these common libraries.  
    /// </summary>
    public static class ControlVisualHelper
    {
        #region General VisualTreeHelper

        /// <summary>
        /// Get visual tree children of a type
        /// </summary>
        /// <typeparam name="T">Visual tree children type</typeparam>
        /// <param name="visual">A DependencyObject reference</param>
        /// <param name="children">A collection of one visual tree children type</param>
        private static void GetVisualChildren<T>(DependencyObject current, Collection<T> children) where T : DependencyObject
        {
            if (current != null)
            {
                if (current.GetType() == typeof(T))
                {
                    children.Add((T)current);
                }

                for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(current); i++)
                {
                    GetVisualChildren<T>(System.Windows.Media.VisualTreeHelper.GetChild(current, i), children);
                }
            }
        }

        /// <summary>
        /// Get visual tree children of a type
        /// </summary>
        /// <typeparam name="T">Visaul tree children type</typeparam>
        /// <param name="visual">A DependencyObject reference</param>
        /// <returns>Returns a collection of one visual tree children type</returns>
        public static Collection<T> GetVisualChildren<T>(DependencyObject current) where T : DependencyObject
        {
            if (current == null)
            {
                return null;
            }

            Collection<T> children = new Collection<T>();

            GetVisualChildren<T>(current, children);

            return children;
        }

        /// <summary>
        /// Get the first visual child from a FrameworkElement Template
        /// </summary>
        /// <typeparam name="P">FrameworkElement type</typeparam>
        /// <typeparam name="T">FrameworkElement type</typeparam>
        /// <param name="p">A FrameworkElement typeof P</param>
        /// <returns>Returns a FrameworkElement visual child typeof T if found one; returns null otherwise</returns>
        public static T GetVisualChild<T, P>(P templatedParent)
            where T : FrameworkElement
            where P : FrameworkElement
        {
            Collection<T> children = GetVisualChildren<T>(templatedParent);

            foreach (T child in children)
            {
                if (child.TemplatedParent == templatedParent)
                {
                    return child;
                }
            }

            return null;
        }

        /// <summary>
        /// Get descendant by given type
        /// Given the perf side effect of ApplyTemplate, use this method judiciously 
        /// </summary>
        /// <param name="element">Visual element</param>
        /// <param name="type">type to search for</param>
        /// <returns></returns>
        public static Visual GetDescendantByType(Visual element, Type type)
        {
            if (element == null) return null;
            if (element.GetType() == type) return element;

            Visual foundElement = null;
            if (element is FrameworkElement)
            {
                (element as FrameworkElement).ApplyTemplate();
            }
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                Visual visual = VisualTreeHelper.GetChild(element, i) as Visual;
                foundElement = GetDescendantByType(visual, type);
                if (foundElement != null)
                {
                    break;
                }
            }
            return foundElement;
        }

        /// <summary>
        /// Walk up the visual tree looking for a node with a given type
        /// </summary>
        /// <param name="type">type of desired node</param>
        /// <param name="node">starting node for the search</param>
        /// <param name="includeNode">if false, do not test the node itself</param>
        /// <returns></returns>
        public static DependencyObject FindAncestorByType(Type type, DependencyObject node, bool includeNode)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            if (includeNode)
            {
                if (type == node.GetType())
                    return node;
            }

            for (node = VisualTreeHelper.GetParent(node); node != null; node = VisualTreeHelper.GetParent(node))
            {
                if (type == node.GetType())
                    return node;
            }

            return null;
        }

        #endregion

        #region ScrollViewer

        /// <summary>
        /// Get ScrollContentPresenter from a ScrollViewer
        /// </summary>
        /// <param name="scrollViewer">A ScrollViewer referecnce</param>
        /// <returns>Returns a ScrollContentPresenter if found; returns null otherwise</returns>
        public static ScrollContentPresenter GetScrollContentPresenter(ScrollViewer scrollViewer)
        {
            return (ScrollContentPresenter)scrollViewer.Template.FindName("PART_ScrollContentPresenter", scrollViewer);
        }

        /// <summary>
        /// Get ScrollBar from a ScrollViewer
        /// </summary>
        /// <param name="scrollViewer">A ScrollViewer reference</param>
        /// <param name="scrollBarPartName">ScrollBar part name</param>
        /// <returns>Returns a ScrollBar if found; returns null otherwise</returns>
        public static ScrollBar GetScrollBar(ScrollViewer scrollViewer, ScrollBarPartName scrollBarPartName)
        {
            return (ScrollBar)scrollViewer.Template.FindName(scrollBarPartName.ToString(), scrollViewer);
        }

        /// <summary>
        /// Get VirtualizingStackPanel from a ScrollContentPresenter
        /// </summary>
        /// <param name="scrollContentPresenter">A ScrollContentPresenter reference</param>
        /// <returns>Returns a VirtualizingStackPanel if found; returns null otherwise</returns>
        public static VirtualizingStackPanel GetVirtualizingStackPanel(ScrollContentPresenter scrollContentPresenter)
        {
            Collection<VirtualizingStackPanel> panels = GetVisualChildren<VirtualizingStackPanel>(scrollContentPresenter);

            foreach (VirtualizingStackPanel panel in panels)
            {
                if (String.Compare(panel.TemplatedParent.GetType().Name, "ItemsPresenter") == 0)
                {
                    return panel;
                }
            }

            return null;
        }

        #endregion

        #region ViewPort

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
                itemsControl = GetListBox(item);
            }
            else if (item is TreeViewItem)
            {
                itemsControl = GetTreeView((TreeViewItem)item);
            }
            else if (item is DataGridCell)
            {
                itemsControl = (ItemsControl)item.GetType().InvokeMember("DataGridOwner", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty, null, item, null);
            }
            else
            {
                throw new NotSupportedException(item.GetType().Name);
            }

            ScrollViewer scrollViewer = GetVisualChild<ScrollViewer, ItemsControl>(itemsControl);
            ScrollContentPresenter scrollContentPresenter = GetScrollContentPresenter(scrollViewer);
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
            ScrollViewer scrollViewer = GetVisualChild<ScrollViewer, ListBox>(listbox);
            ScrollContentPresenter scrollContentPresenter = GetScrollContentPresenter(scrollViewer);
            VirtualizingStackPanel virtualizingStackPanel = GetVirtualizingStackPanel(scrollContentPresenter);

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
            ScrollViewer scrollViewer = GetVisualChild<ScrollViewer, ListBox>(listbox);
            double viewPortHeight = scrollViewer.ViewportHeight;
            ScrollContentPresenter scrollContentPresenter = GetScrollContentPresenter(scrollViewer);
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
                if (!IsInViewport((ListBoxItem)listbox.ItemContainerGenerator.ContainerFromIndex(index)))
                {
                    throw new TestValidationException("Fail: " + index + " is not in Viewport.");
                }

                if (listbox.ItemContainerGenerator.ContainerFromIndex(index + 1) != null)
                {
                    if (IsInViewport((ListBoxItem)listbox.ItemContainerGenerator.ContainerFromIndex(index + 1)))
                    {
                        throw new TestValidationException("Fail: " + (index + 1) + " is in Viewport.");
                    }
                }
            }

            // subtract 1 because it includes one that is not in viewport
            return count - 1;
        }

        #endregion

        #region ItemsControl

        #region TreeView

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
        /// Scroll TreeView's ScrollView to top.
        /// </summary>
        /// <param name="treeviewitem">A TreeViewItem reference</param>
        public static void ScrollTreeViewToTop(TreeViewItem treeviewitem)
        {
            ScrollViewer scrollViewer = GetVisualChild<ScrollViewer, TreeView>(GetTreeView(treeviewitem));
            scrollViewer.ScrollToHome();
            
            //
        }

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

        #endregion

        #region ListBox

        /// <summary>
        /// GEt listbox from item container
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public static ListBox GetListBox(Control container)
        {
            return ItemsControl.ItemsControlFromItemContainer(container) as ListBox;
        }

        #endregion

        #region ComboBox

        public static ToggleButton FindDropDownToggleButton(ComboBox combobox)
        {
            Collection<ToggleButton> toggleButtons = GetVisualChildren<ToggleButton>(combobox);

            for (int i = 0; i < toggleButtons.Count; i++)
            {
                if (toggleButtons[i].TemplatedParent == combobox)
                {
                    return toggleButtons[i];
                }
            }

            return null;
        }

        public static void WaitForDropDownOpened(ComboBox combobox)
        {
            DispatcherFrame frame = new DispatcherFrame();
            combobox.DropDownOpened += delegate(Object s, EventArgs e) { frame.Continue = false; };
            Dispatcher.PushFrame(frame);
        }

        public static void WaitForDropDownClosed(ComboBox combobox)
        {
            DispatcherFrame frame = new DispatcherFrame();
            combobox.DropDownClosed += delegate(Object s, EventArgs e) { frame.Continue = false; };
            Dispatcher.PushFrame(frame);
        }

        #endregion

        #region DataGrid

        /// <summary>
        /// Get scroll content presenter for a datagrid
        /// </summary>
        /// <param name="dataGrid">An instance of datagrid.</param>
        /// <returns>Returns datagrid's ScrollContentPresenter</returns>
        public static ScrollContentPresenter GetScrollContentPresenter(DataGrid dataGrid)
        {
            ScrollViewer scrollViewer = GetVisualChild<ScrollViewer, DataGrid>(dataGrid);
            if (scrollViewer == null)
            {
                throw new TestValidationException("ScrollViewer is null.");
            }

            return (ScrollContentPresenter)GetScrollContentPresenter(scrollViewer); 
        }

        /// <summary>
        /// Get DataGridCell from a DataGridCellInfo.
        /// </summary>
        /// <param name="dataGridCellInfo">An instance of DataGridCellInfo.</param>
        /// <returns>DataGridCell if it is not null.</returns>
        public static DataGridCell GetCell(DataGridCellInfo dataGridCellInfo)
        {
            if (!dataGridCellInfo.IsValid)
            {
                return null;
            }

            var cellContent = dataGridCellInfo.Column.GetCellContent(dataGridCellInfo.Item);
            if (cellContent != null)
            {
                return (DataGridCell)cellContent.Parent;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the DataGridCell based on row and column index
        /// </summary>
        /// <param name="row">row index of cell to get</param>
        /// <param name="column">column index of cell to get</param>
        public static DataGridCell GetCell(DataGrid dataGrid, int row, int column)
        {
            DataGridRow rowContainer = GetRow(dataGrid, row);
            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter, DataGridRow>(rowContainer); 

                // try to get the cell but it may possibly be virtualized
                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                if (cell == null)
                {
                    // now try to bring into view and retreive the cell
                    dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[column]);

                    ////

                    cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                }

                return cell;
            }

            return null;
        }

        /// <summary>
        /// this is a version that takes the virtualization into consideration, so the cell 
        /// can be null and don't have to be in view
        /// </summary>
        /// <param name="row">the row index</param>
        /// <param name="column">the column index</param>
        /// <returns></returns>
        public static DataGridCell GetCellVirtual(DataGrid dataGrid, int row, int column)
        {
            DataGridRow rowContainer = GetRow(dataGrid, row);
            if (rowContainer != null)
            {
                return GetCell(rowContainer, column);
            }

            return null;
        }

        /// <summary>
        /// Get a cell by the row container and column index
        /// </summary>
        /// <param name="rowContainer">row</param>
        /// <param name="column">column index</param>
        /// <returns></returns>
        public static DataGridCell GetCell(DataGridRow rowContainer, int column)
        {
            DataGridCellsPresenter presenter = GetCellsPresenter(rowContainer);
            if (presenter != null)
            {
                return presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
            }

            return null;
        }

        /// <summary>
        /// Check if a given cell is in the viewport
        /// </summary>
        /// <param name="dataGrid">the datagrid in test</param>
        /// <param name="cell">cell to evaluate</param>
        /// <returns>true if in view else false</returns>
        public static bool IsCellInView(DataGrid dataGrid, DataGridCell cell)
        {
            if (cell == null)
            {
                throw new ArgumentNullException("cell");
            }

            ScrollContentPresenter scrollContentPresenter = GetScrollContentPresenter(dataGrid);
            int viewWidth = Convert.ToInt32(scrollContentPresenter.ActualWidth);

            GeneralTransform cellTransform = cell.TransformToAncestor(scrollContentPresenter);
            Point ptIn = new Point(0, 0);
            Point cellLeftX;
            cellTransform.TryTransform(ptIn, out cellLeftX);
            int cellRightX = Convert.ToInt32(cellLeftX.X + cell.RenderSize.Width);

            // cell left renders outside of scrollcontentpresenter on X coordinate.
            if (cellLeftX.X < 0)
            {
                return false;
            }

            // cell right renders outside of scrollcontentpresenter on X coordinate.
            if (cellRightX > viewWidth)
            {
                return false;
            }

            return true;
        }

        public static DataGridCellsPresenter GetCellsPresenter(FrameworkElement parent)
        {
            return GetVisualChild<DataGridCellsPresenter, FrameworkElement>(parent); // FindVisualChild<DataGridCellsPresenter>(parent);
        }

        /// <summary>
        /// Gets the DataGridRow based on the given index
        /// </summary>
        /// <param name="index">the index of the container to get</param>
        public static DataGridRow GetRow(DataGrid dataGrid, int index)
        {
            DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                // may be virtualized, bring into view and try again
                dataGrid.ScrollIntoView(dataGrid.Items[index]);
                
                //

                row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(index);
            }

            return row;
        }

        #endregion

        #region ListView
        


        #endregion

        #endregion
        
        #region Others

        // TextBoxBase - TextBox, PasswordBox
        // DatePicker
        // MenuItem
        
        // NavigationWindow
        // BrowserWindow

        #endregion

    }

    public enum ScrollBarPartName
    {
        PART_VerticalScrollBar,
        PART_HorizontalScrollBar,
        Unknown
    };

    public enum ScrollingMode
    {
        LineUp,
        LineDown,
        LineLeft,
        LineRight,
        PageUp,
        PageDown,
        PageLeft,
        PageRight,
        Unknown
    };
}
