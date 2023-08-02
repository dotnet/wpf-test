using System;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Microsoft.Test.Controls
{
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
    }

    /// <summary>
    /// ScrollBarHelper
    /// </summary>
    public static class ScrollBarHelper
    {
        /// <summary>
        /// Get a repeatbutton from a scrollbar
        /// </summary>
        /// <param name="scrollBar">A scrollbar reference</param>
        /// <param name="scrollingMode">Scrolling Mode</param>
        /// <returns>Returns a repeatbutton if found one; returns null otherwise</returns>
        public static RepeatButton GetRepeatButton(ScrollBar scrollBar, ScrollingMode scrollingMode)
        {
            Collection<RepeatButton> repeatButtons = VisualTreeHelper.GetVisualChildren<RepeatButton>(scrollBar);

            foreach (RepeatButton repeatButton in repeatButtons)
            {
                if (repeatButton.TemplatedParent == scrollBar)
                {
                    RoutedCommand routedCommand = repeatButton.Command as RoutedCommand;
                    if (String.Compare(routedCommand.Name, scrollingMode.ToString(), true) == 0)
                    {
                        return repeatButton;
                    }
                }
            }

            return null;
        }
    }
}


