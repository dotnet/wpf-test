using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Avalon.Test.ComponentModel;
using Microsoft.Test;

namespace Microsoft.Test.Controls.Helpers
{
    /// <summary>
    /// These methods are in lieu of "extension" methods featured in .Net 3.5,
    /// but not available in .Net 3.0. Without the "extension" feature these 
    /// these can still be scoped together.    /// </summary>
    public static class GridSplitterExtend
    {
        /// <summary>
        /// Pick-out and assign GridSplitter property values from test case context. 
        /// This method implements a mapping from Dictionary keys in the test context
        /// to GridSplitter properties
        /// </summary>
        public static void SetPropertiesFromVariationContext
            (GridSplitter gs, GridSplitterVariationContext context)
        {
            foreach (String key in context.Keys)
            {
                if (context[key] != null)
                {
                    switch (key)
                    {
                        case "SplitterMinWidth":
                            gs.MinWidth = (Int32)context[key];
                            break;
                        case "SplitterMinHeight":
                            gs.MinHeight = (Int32)context[key];
                            break;
                        case "SplitterColor":
                            gs.Background = (context[key] as Brush);
                            break;
                        case "Grid.Row":
                            gs.SetValue(Grid.RowProperty, (Int32)context[key]);
                            break;
                        case "Grid.Column":
                            gs.SetValue(Grid.ColumnProperty, (Int32)context[key]);
                            break;
                        case "WidthHeight":
                            gs.Width = ((Size)context[key]).Width;
                            gs.Height = ((Size)context[key]).Height;
                            break;
                        case "MinWidthHeight":
                            gs.MinWidth = ((Size)context[key]).Width;
                            gs.MinHeight = ((Size)context[key]).Height;
                            break;
                        case "ResizeDirection":
                            gs.ResizeDirection = (GridResizeDirection)context[key];
                            break;
                        case "ResizeBehavior":
                            gs.ResizeBehavior = (GridResizeBehavior)context[key];
                            break;
                        case "HorizontalAlignment":
                            gs.HorizontalAlignment = (HorizontalAlignment)context[key];
                            break;
                        case "VerticalAlignment":
                            gs.VerticalAlignment = (VerticalAlignment)context[key];
                            break;
                        case "KeyboardIncrement":
                            gs.KeyboardIncrement = (Double)context[key];
                            break;
                        case "DragIncrement":
                            gs.DragIncrement = (Double)context[key];
                            break;
                        case "ShowsPreview":
                            gs.ShowsPreview = (Boolean)context[key];
                            break;
                    }
                }
            }
        }
    }
}
