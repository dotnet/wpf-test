using System;
using System.Windows;
using System.Windows.Controls.Primitives;

using WpfControlToolkit;

namespace Avalon.Test.ComponentModel.Actions
{
    namespace TreeMapTest
    {
        /// <summary>
        /// The following were originally written as (.Net 3.5 supported)
        /// FrameworkElement Extension Methods. Now rewritten as just regular
        /// static methods, so call locations become more syntactically cluttered.
        /// </summary>
        internal static class TreeMapElementExtensions
        {
            public static Point SlotLocation(FrameworkElement element)
            {
                return LayoutInformation.GetLayoutSlot(element).Location;
            }

            public static Size SlotSize(FrameworkElement element)
            {
                return LayoutInformation.GetLayoutSlot(element).Size;
            }

            public static Double Weight(FrameworkElement element)
            {
                return (Double)element.GetValue(TreeMapPanel.AreaProperty);
            }
        }
    }
}
