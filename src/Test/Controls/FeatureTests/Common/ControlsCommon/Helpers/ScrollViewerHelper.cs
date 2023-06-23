using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Microsoft.Test.Controls
{
    public enum ScrollBarPartName
    {
        PART_VerticalScrollBar,
        PART_HorizontalScrollBar,
        Unknown
    };

    /// <summary>
    /// ScrollViewerHelper
    /// </summary>
    public static class ScrollViewerHelper
    {
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
            Collection<VirtualizingStackPanel> panels = VisualTreeHelper.GetVisualChildren<VirtualizingStackPanel>(scrollContentPresenter);

            foreach(VirtualizingStackPanel panel in panels)
            {
                if (String.Compare(panel.TemplatedParent.GetType().Name, "ItemsPresenter") == 0)
                {
                    return panel;
                }
            }

            return null;
        }
    }
}


