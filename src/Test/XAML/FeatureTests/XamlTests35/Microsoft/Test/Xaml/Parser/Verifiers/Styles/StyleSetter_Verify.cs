// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Types;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Styles
{
    /// <summary/>
    public static class StyleSetter_Verify
    {
        /// <summary>
        /// Verify that properties set on a Style using Setters are set fine.
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool    result   = true;
            ListBox listbox0 = (ListBox) LogicalTreeHelper.FindLogicalNode(rootElement, "ListBox0");
            ListBox listbox1 = (ListBox) LogicalTreeHelper.FindLogicalNode(rootElement, "ListBox1");
            Button  button0  = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button0");
            Button  button1  = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button1");

            // Verify all the different properties on ListBox0            
            SelectionMode mode = (SelectionMode) listbox0.GetValue(ListBox.SelectionModeProperty);
            if (mode != SelectionMode.Extended)
            {
                GlobalLog.LogEvidence("mode != SelectionMode.Extended");
                result = false;
            }

            SolidColorBrush background = listbox0.GetValue(Control.BackgroundProperty) as SolidColorBrush;
            if (!Color.Equals(background.Color, Colors.Yellow))
            {
                GlobalLog.LogEvidence("!Color.Equals(background.Color, Colors.Yellow)");
                result = false;
            }

            double fontSize = (double) listbox0.GetValue(Control.FontSizeProperty);
            if (fontSize != 48.0)
            {
                GlobalLog.LogEvidence("fontSize != 48.0");
                result = false;
            }

            Dock dock = (Dock) listbox0.GetValue(DockPanel.DockProperty);
            if (dock != Dock.Bottom)
            {
                GlobalLog.LogEvidence("dock != Dock.Bottom");
                result = false;
            }

            MyColor color = (MyColor) listbox0.GetValue(MyClass.MyColorProperty);
            if (color.Color != "Yellow")
            {
                GlobalLog.LogEvidence("color.Color != Yellow");
                result = false;
            }

            FontStyle fontStyle = (FontStyle) listbox0.GetValue(Control.FontStyleProperty);
            if (fontStyle != FontStyles.Italic)
            {
                GlobalLog.LogEvidence("fontStyle != FontStyles.Italic");
                result = false;
            }

            // Check one of ListBox1's properties
            color = (MyColor) listbox1.GetValue(MyClass.MyColorProperty);
            if (color.Color != "Yellow")
            {
                GlobalLog.LogEvidence("color.Color != Yellow(1)");
                result = false;
            }

            // Check Button0's background
            background = button0.GetValue(Control.BackgroundProperty) as SolidColorBrush;
            if (!Color.Equals(background.Color, Colors.Red))
            {
                GlobalLog.LogEvidence("!Color.Equals(background.Color, Colors.Red)");
                result = false;
            }

            // Verify Button1's properties, which it's supposed to get from the 
            // Open-ended style
            background = button1.GetValue(Control.BackgroundProperty) as SolidColorBrush;
            if (!Color.Equals(background.Color, Colors.Yellow))
            {
                GlobalLog.LogEvidence("!Color.Equals(background.Color, Colors.Yellow)(1)");
                result = false;
            }

            fontSize = (double) button1.GetValue(Control.FontSizeProperty);
            if (fontSize != 48.0)
            {
                GlobalLog.LogEvidence("fontSize != 48.0(1)");
                result = false;
            }

            dock = (Dock) button1.GetValue(DockPanel.DockProperty);
            if (dock != Dock.Bottom)
            {
                GlobalLog.LogEvidence("dock != Dock.Bottom(1)");
                result = false;
            }
            return result;
        }
    }
}
