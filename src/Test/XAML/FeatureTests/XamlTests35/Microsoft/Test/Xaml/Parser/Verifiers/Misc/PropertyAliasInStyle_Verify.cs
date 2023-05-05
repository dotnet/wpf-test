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

namespace Microsoft.Test.Xaml.Parser.Verifiers.Misc
{
    /// <summary/>
    public static class PropertyAliasInStyle_Verify
    {
        /// <summary>
        /// Verify alias properties in styles.
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool result = true;

            Button button0 = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button0");
            Button button1 = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button1");

            // Reach the elements in the visual tree for Button0.

            Border button0border = VisualTreeHelper.GetChild(button0, 0) as Border;

            StackPanel button0stackpanel = VisualTreeHelper.GetChild(button0border, 0) as StackPanel;

            // Verify the properties of Button0's visual tree elements
            if (!Color.Equals((button0border.Background as SolidColorBrush).Color, Colors.Red))
            {
                GlobalLog.LogEvidence("!Color.Equals((Button0Border.Background as SolidColorBrush).Color, Colors.Red)");
                result = false;
            }

            // 
            /*
            MouseUtility.MouseLeftButtonClickonElement(Button0, 1, 1, true);
            Assert(Color.Equals((Button0Border.Background as SolidColorBrush).Color, Colors.Blue), errorMesg);
            */

            if (button0stackpanel.Height != 200)
            {
                GlobalLog.LogEvidence("Button0StackPanel.Height != 200");
                result = false;
            }
            if (button0stackpanel.Width != 200)
            {
                GlobalLog.LogEvidence("Button0StackPanel.Width != 200");
                result = false;
            }

            // Reach the elements in the visual tree for Button1.            
            Border button1border = VisualTreeHelper.GetChild(button1, 0) as Border;

            // Verify the properties of Button1's visual tree elements
            if (!Color.Equals((button1border.Background as SolidColorBrush).Color, Colors.DarkRed))
            {
                GlobalLog.LogEvidence("!Color.Equals((Button1Border.Background as SolidColorBrush).Color, Colors.DarkRed)");
                result = false;
            }
            if (!Color.Equals((button1border.BorderBrush as SolidColorBrush).Color, Colors.Yellow))
            {
                GlobalLog.LogEvidence("!Color.Equals((Button1Border.BorderBrush as SolidColorBrush).Color, Colors.Yellow)");
                result = false;
            }
            if (button1border.BorderThickness.Left != 5)
            {
                GlobalLog.LogEvidence("Button1Border.BorderThickness.Left != 5");
                result = false;
            }
            return result;
        }
    }
}
